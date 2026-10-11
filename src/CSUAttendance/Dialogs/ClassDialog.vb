Imports System.Windows.Forms

''' <summary>
''' Add/edit a class (subject + section + teacher + schedule in one term). id = 0 → add.
''' Total class hours are computed live from the term dates, meeting days and times (CSU rule R2),
''' with an optional override. Warns when the teacher or section already has a class at that time.
''' </summary>
Public Class ClassDialog

    Private ReadOnly _id As Integer
    Private ReadOnly _defaultTermId As Integer
    Private _terms As DataTable
    Private _loading As Boolean

    Public Sub New()
        Me.New(0)
    End Sub

    Public Sub New(classId As Integer, Optional defaultTermId As Integer = 0)
        InitializeComponent()
        _id = classId
        _defaultTermId = defaultTermId
    End Sub

    Private ReadOnly Property DayBoxes As CheckBox()
        Get
            Return {chkMon, chkTue, chkWed, chkThu, chkFri, chkSat}
        End Get
    End Property

    Private Sub ClassDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        SetRail(Icons.Classes, "Class", "One subject taught to one section by one teacher in a term. Students are enrolled into classes, and attendance is taken per class.")
        For Each c In {cboTerm, cboSubject, cboSection, cboTeacher}
            Theme.StyleCombo(c)
        Next
        lblTotal.Font = Theme.FontHeading
        lblTotal.ForeColor = Theme.TextPrimary

        _loading = True
        _terms = Db.Query("SELECT term_id, CONCAT(semester, ' ', school_year) AS label, start_date, end_date, is_active FROM school_terms ORDER BY start_date DESC")
        Bind(cboTerm, _terms, "term_id", "label")
        Bind(cboSubject, Db.Query("SELECT subject_id, CONCAT(subject_code, '  ·  ', description) AS label FROM subjects ORDER BY subject_code"), "subject_id", "label")
        Bind(cboSection, Db.Query("SELECT section_id, section_name FROM sections ORDER BY section_name"), "section_id", "section_name")
        Bind(cboTeacher, Db.Query("SELECT teacher_id, " & TextUtil.SortableNameSql("t") & " AS label FROM teachers t ORDER BY t.last_name, t.first_name"), "teacher_id", "label")

        If _id = 0 Then
            Dim activeTerm = _terms.Rows.Cast(Of DataRow)().FirstOrDefault(Function(r) Db.ToBool(r("is_active")))
            Dim termId = If(_defaultTermId > 0, _defaultTermId, If(activeTerm Is Nothing, 0, Db.ToInt(activeTerm("term_id"))))
            If termId > 0 Then cboTerm.SelectedValue = termId
            dtpStart.Value = Date.Today.Add(New TimeSpan(8, 0, 0))
            dtpEnd.Value = Date.Today.Add(New TimeSpan(9, 30, 0))
        Else
            LoadClass()
        End If
        _loading = False
        UpdateOverrideState()
        RecomputeHours()
    End Sub

    Private Shared Sub Bind(cbo As ComboBox, dt As DataTable, valueMember As String, displayMember As String)
        cbo.DisplayMember = displayMember
        cbo.ValueMember = valueMember
        cbo.DataSource = dt
        cbo.SelectedIndex = -1
    End Sub

    Private Sub LoadClass()
        lblTitle.Text = "Edit class"
        Dim dt = Db.Query("SELECT * FROM classes WHERE class_id = @id", Db.P("@id", _id))
        If dt.Rows.Count = 0 Then
            DialogResult = DialogResult.Cancel
            Return
        End If
        Dim r = dt.Rows(0)
        cboTerm.SelectedValue = Db.ToInt(r("term_id"))
        cboSubject.SelectedValue = Db.ToInt(r("subject_id"))
        cboSection.SelectedValue = Db.ToInt(r("section_id"))
        cboTeacher.SelectedValue = Db.ToInt(r("teacher_id"))
        Dim days = Db.ToStr(r("days")).Split(","c)
        For Each box In DayBoxes
            box.Checked = days.Contains(box.Text)
        Next
        dtpStart.Value = Date.Today.Add(Db.ToTime(r("start_time")))
        dtpEnd.Value = Date.Today.Add(Db.ToTime(r("end_time")))
        txtRoom.Text = Db.ToStr(r("room"))

        ' If the saved total differs from the computed one, it was overridden.
        Dim saved = Db.ToDec(r("total_hours"))
        If saved <> ComputedHours() Then
            chkOverride.Checked = True
            numHours.Value = Math.Min(numHours.Maximum, saved)
        End If
    End Sub

    ' ---------------- total hours ----------------
    Private Function SelectedDays() As String
        Return String.Join(",", DayBoxes.Where(Function(b) b.Checked).Select(Function(b) b.Text))
    End Function

    Private Function SelectedTerm() As DataRow
        If cboTerm.SelectedIndex < 0 Then Return Nothing
        Return DirectCast(cboTerm.SelectedItem, DataRowView).Row
    End Function

    Private Function ComputedHours() As Decimal
        Dim term = SelectedTerm()
        If term Is Nothing OrElse SelectedDays() = "" OrElse dtpEnd.Value.TimeOfDay <= dtpStart.Value.TimeOfDay Then Return 0D
        Return AttendanceRules.ComputeTotalHours(Db.ToDate(term("start_date")), Db.ToDate(term("end_date")), SelectedDays(),
                                                 dtpStart.Value.TimeOfDay, dtpEnd.Value.TimeOfDay)
    End Function

    ''' <summary>Shows e.g. "54.0 hours" and "36 meetings × 1.5 h".</summary>
    Private Sub RecomputeHours()
        If _loading Then Return
        Dim total = ComputedHours()
        Dim perMeeting = If(dtpEnd.Value.TimeOfDay > dtpStart.Value.TimeOfDay,
                            AttendanceRules.SessionHours(dtpStart.Value.TimeOfDay, dtpEnd.Value.TimeOfDay), 0D)
        If total = 0D Then
            lblTotal.Text = "—"
            lblHoursNote.Text = "Choose the term, meeting days and times to compute the total."
        Else
            Dim meetings = CInt(total / perMeeting)
            lblTotal.Text = $"{total:0.0#} hours"
            lblHoursNote.Text = $"{meetings} meetings × {perMeeting:0.0#} h. Drop limit (20%) = {total * AppSettings.DropPct / 100D:0.0#} h."
        End If
        If Not chkOverride.Checked Then numHours.Value = Math.Min(numHours.Maximum, total)
    End Sub

    Private Sub UpdateOverrideState()
        numHours.Enabled = chkOverride.Checked
        lblHours.Enabled = chkOverride.Checked
    End Sub

    Private Sub Schedule_Changed(sender As Object, e As EventArgs) Handles cboTerm.SelectedIndexChanged, dtpStart.ValueChanged, dtpEnd.ValueChanged,
            chkMon.CheckedChanged, chkTue.CheckedChanged, chkWed.CheckedChanged, chkThu.CheckedChanged, chkFri.CheckedChanged, chkSat.CheckedChanged
        RecomputeHours()
    End Sub

    Private Sub chkOverride_CheckedChanged(sender As Object, e As EventArgs) Handles chkOverride.CheckedChanged
        UpdateOverrideState()
        If Not chkOverride.Checked Then RecomputeHours()
    End Sub

    ' ---------------- validation + save ----------------
    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        For Each pair In {Tuple.Create(cboTerm, "Choose a school term."), Tuple.Create(cboSubject, "Choose a subject."),
                          Tuple.Create(cboSection, "Choose a section."), Tuple.Create(cboTeacher, "Choose a teacher.")}
            If pair.Item1.SelectedIndex < 0 Then
                errProvider.SetError(pair.Item1, If(pair.Item1.Items.Count = 0, "Nothing to choose yet - add one first.", pair.Item2))
                ok = False
            End If
        Next
        If SelectedDays() = "" Then
            errProvider.SetError(chkSat, "Tick at least one meeting day.")
            ok = False
        End If
        If dtpEnd.Value.TimeOfDay <= dtpStart.Value.TimeOfDay Then
            errProvider.SetError(dtpEnd, "End time must be after the start time.")
            ok = False
        End If
        If numHours.Value <= 0D Then
            errProvider.SetError(numHours, "Total class hours must be more than zero.")
            ok = False
        End If
        Return ok
    End Function

    ''' <summary>Returns a description of another class that overlaps this schedule for the same teacher or section, or "".</summary>
    Private Function FindScheduleConflict() As String
        Dim dt = Db.Query(
            "SELECT c.days, c.teacher_id, c.section_id, sub.subject_code, sec.section_name, " &
            "       TIME_FORMAT(c.start_time, '%l:%i %p') AS t1, TIME_FORMAT(c.end_time, '%l:%i %p') AS t2 " &
            "FROM classes c JOIN subjects sub ON sub.subject_id = c.subject_id JOIN sections sec ON sec.section_id = c.section_id " &
            "WHERE c.term_id = @term AND c.class_id <> @id AND (c.teacher_id = @tch OR c.section_id = @sec) " &
            "  AND c.start_time < @end AND c.end_time > @start",
            Db.P("@term", cboTerm.SelectedValue), Db.P("@id", _id), Db.P("@tch", cboTeacher.SelectedValue), Db.P("@sec", cboSection.SelectedValue),
            Db.P("@start", dtpStart.Value.TimeOfDay), Db.P("@end", dtpEnd.Value.TimeOfDay))
        Dim mine = SelectedDays().Split(","c)
        For Each r As DataRow In dt.Rows
            Dim commonDays = Db.ToStr(r("days")).Split(","c).Intersect(mine).ToList()
            If commonDays.Count = 0 Then Continue For
            Dim who = If(Db.ToInt(r("teacher_id")) = Db.ToInt(cboTeacher.SelectedValue), "The teacher", "The section")
            Return $"{who} already has {r("subject_code")} · {r("section_name")} on {String.Join(", ", commonDays)} {Db.ToStr(r("t1")).Trim()} – {Db.ToStr(r("t2")).Trim()}."
        Next
        Return ""
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Try
            Dim conflict = FindScheduleConflict()
            If conflict <> "" AndAlso Not Notice.Ask(Me, "Schedule overlap", conflict & " Save this class anyway?", "Save anyway", "Go back") Then Return

            Dim ps = {Db.P("@term", cboTerm.SelectedValue), Db.P("@sub", cboSubject.SelectedValue), Db.P("@sec", cboSection.SelectedValue),
                      Db.P("@tch", cboTeacher.SelectedValue), Db.P("@days", SelectedDays()), Db.P("@start", dtpStart.Value.TimeOfDay),
                      Db.P("@end", dtpEnd.Value.TimeOfDay), Db.P("@room", TextUtil.OrNull(txtRoom.Text)), Db.P("@hrs", numHours.Value),
                      Db.P("@id", _id)}
            Dim label = $"{cboSubject.Text} · {cboSection.Text}"
            If _id = 0 Then
                Dim newId = Db.Insert("INSERT INTO classes (term_id, subject_id, section_id, teacher_id, days, start_time, end_time, room, total_hours) " &
                                      "VALUES (@term, @sub, @sec, @tch, @days, @start, @end, @room, @hrs)", ps)
                Audit.Log("CREATE", "Class", newId, $"{label} ({numHours.Value:0.##} h)")
            Else
                Db.Execute("UPDATE classes SET term_id = @term, subject_id = @sub, section_id = @sec, teacher_id = @tch, days = @days, " &
                           "start_time = @start, end_time = @end, room = @room, total_hours = @hrs WHERE class_id = @id", ps)
                Audit.Log("UPDATE", "Class", _id, $"{label} ({numHours.Value:0.##} h)")
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(cboSubject, "This section already has this subject in the selected term.")
        Catch ex As Exception
            Notice.Fail(Me, "Could not save the class", "Please check your connection and the details, then try again.", ex.Message)
        End Try
    End Sub

End Class
