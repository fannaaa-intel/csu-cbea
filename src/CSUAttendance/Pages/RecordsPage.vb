Imports System.Windows.Forms

''' <summary>
''' Attendance Records: saved sessions (double-click to open in Take Attendance) and
''' pending excused absences (mark the requirement as complied → those hours stop counting).
''' Teachers only see their own classes.
''' </summary>
Public Class RecordsPage
    Inherits PageBase

    Private ReadOnly cboClass As ComboBox = Ui.Combo(330)
    Private ReadOnly dtpFrom As DateTimePicker = Ui.DatePicker(150)
    Private ReadOnly dtpTo As DateTimePicker = Ui.DatePicker(150)
    Private ReadOnly lblTo As Label = Ui.Muted("to")
    Private ReadOnly tabs As New TabStrip()
    Private ReadOnly btnOpen As Button = Ui.Action("Open session", "primary", Icons.View, 130)
    Private ReadOnly btnComplied As Button = Ui.Action("Mark complied", "primary", Icons.Check, 140)
    Private ReadOnly btnSummary As Button = Ui.Action("Class summary", "secondary", Icons.Students, 140)
    Private ReadOnly grid As DataGridView = Ui.Grid(True)
    Private lblNote As Label
    Private emptyLabel As Label
    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Attendance Records"
        End Get
    End Property

    Public Sub New()
        Dim card = Ui.Card()
        card.Controls.Add(grid)
        card.Controls.Add(Ui.CardHeader("", lblNote, 30))
        emptyLabel = Ui.EmptyState(grid, "")

        Dim tabRow As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(48), .BackColor = Theme.Background}
        tabs.SetTabs("Sessions", "Pending excuses")
        tabs.Location = New Drawing.Point(0, Theme.Dpi(2))
        tabRow.Controls.Add(tabs)

        Dim bar = Ui.Toolbar()
        Dim filters = Ui.Row()
        lblTo.Margin = New Padding(0, Theme.Dpi(10), Theme.Dpi(12), 0)
        filters.Controls.AddRange({cboClass, dtpFrom, lblTo, dtpTo})
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.AddRange({btnOpen, btnComplied, btnSummary})
        bar.Controls.Add(filters)
        bar.Controls.Add(actions)

        Controls.Add(card)
        Controls.Add(tabRow)
        Controls.Add(bar)

        btnSummary.Visible = Session.IsAdmin
        btnComplied.Visible = False
        Theme.StyleFilled(btnComplied, Theme.Success, Icons.Check)
        Theme.FitWidth(btnComplied, 140)

        AddHandler cboClass.SelectedIndexChanged, Sub() If Not _loading Then LoadData()
        AddHandler dtpFrom.ValueChanged, Sub() If Not _loading Then LoadData()
        AddHandler dtpTo.ValueChanged, Sub() If Not _loading Then LoadData()
        AddHandler tabs.SelectedChanged, Sub() TabChanged()
        AddHandler grid.CellDoubleClick, Sub(s, e) If e.RowIndex >= 0 AndAlso tabs.SelectedIndex = 0 Then OpenSession()
        AddHandler grid.CellPainting, AddressOf Grid_CellPainting
        AddHandler grid.SelectionChanged, Sub() UpdateButtons()
        AddHandler btnOpen.Click, Sub() OpenSession()
        AddHandler btnComplied.Click, Sub() MarkComplied()
        AddHandler btnSummary.Click, Sub() Host.Navigate("mystudents", Lookups.SelectedId(cboClass))
    End Sub

    Public Overrides Sub OnPageShown()
        _loading = True
        Dim dt = Lookups.ClassesFor(Lookups.ActiveTermId())
        Dim all = dt.NewRow()
        all("class_id") = 0
        all("label") = If(Session.IsTeacher, "All my classes", "All classes")
        dt.Rows.InsertAt(all, 0)
        Lookups.Bind(cboClass, dt, "class_id", "label")
        cboClass.SelectedIndex = 0
        dtpFrom.Value = New Date(Date.Today.Year, Date.Today.Month, 1)
        dtpTo.Value = Date.Today
        _loading = False
        LoadData()
    End Sub

    Private Sub TabChanged()
        dtpFrom.Visible = tabs.SelectedIndex = 0
        dtpTo.Visible = tabs.SelectedIndex = 0
        lblTo.Visible = tabs.SelectedIndex = 0
        btnOpen.Visible = tabs.SelectedIndex = 0
        btnComplied.Visible = tabs.SelectedIndex = 1
        LoadData()
    End Sub

    Private ReadOnly Property TeacherFilter As Integer
        Get
            Return If(Session.IsTeacher, Session.User.TeacherId, 0)
        End Get
    End Property

    Private Sub LoadData()
        Try
            If tabs.SelectedIndex = 0 Then LoadSessions() Else LoadPending()
            RefreshPendingCount()
        Catch ex As Exception
            ShowError(ex, "load the attendance records")
        End Try
        UpdateButtons()
    End Sub

    Private Sub LoadSessions()
        Dim dt = Db.Query(
            "SELECT s.session_id, s.class_id, s.session_date, DATE_FORMAT(s.session_date, '%a') AS day_name, " &
            "       CONCAT(sub.subject_code, ' · ', sec.section_name) AS class_label, " &
            "       SUM(ar.status = 'Present') AS p, SUM(ar.status = 'Late') AS l, SUM(ar.status = 'Absent') AS a, SUM(ar.status = 'Excused') AS e, " &
            "       u.full_name AS recorded_by, s.updated_at " &
            "FROM attendance_sessions s JOIN classes c ON c.class_id = s.class_id " &
            "JOIN subjects sub ON sub.subject_id = c.subject_id JOIN sections sec ON sec.section_id = c.section_id " &
            "LEFT JOIN attendance_records ar ON ar.session_id = s.session_id LEFT JOIN users u ON u.user_id = s.recorded_by " &
            "WHERE c.term_id = @term AND (@c = 0 OR s.class_id = @c) AND (@tch = 0 OR c.teacher_id = @tch) " &
            "  AND s.session_date BETWEEN @from AND @to " &
            "GROUP BY s.session_id, s.class_id, s.session_date, sub.subject_code, sec.section_name, u.full_name, s.updated_at " &
            "ORDER BY s.session_date DESC, class_label",
            Db.P("@term", Lookups.ActiveTermId()), Db.P("@c", Lookups.SelectedId(cboClass)), Db.P("@tch", TeacherFilter),
            Db.P("@from", dtpFrom.Value.Date), Db.P("@to", dtpTo.Value.Date))
        grid.MultiSelect = False
        grid.DataSource = dt
        Ui.Hide(grid, "session_id", "class_id")
        Ui.Col(grid, "session_date", "Date", 13, "MMM d, yyyy")
        Ui.Col(grid, "day_name", "Day", 7)
        Ui.Col(grid, "class_label", "Class", 20)
        Ui.Col(grid, "p", "Present", 8)
        Ui.Col(grid, "l", "Late", 7)
        Ui.Col(grid, "a", "Absent", 8)
        Ui.Col(grid, "e", "Excused", 8)
        Ui.Col(grid, "recorded_by", "Recorded by", 16)
        Ui.Col(grid, "updated_at", "Last updated", 15, "MMM d, h:mm tt")
        lblNote.Text = $"{Ui.Plural(dt.Rows.Count, "session")} · double-click a session to open it"
        emptyLabel.Text = "No attendance was saved for this class and date range."
        emptyLabel.Visible = dt.Rows.Count = 0
    End Sub

    Private Sub LoadPending()
        Dim dt = Db.Query(
            "SELECT ar.record_id, s.class_id, s.session_date, st.student_no, " & TextUtil.SortableNameSql("st") & " AS student_name, " &
            "       CONCAT(sub.subject_code, ' · ', sec.section_name) AS class_label, ar.excuse_requirement, ar.excuse_status " &
            "FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id " &
            "JOIN classes c ON c.class_id = s.class_id JOIN subjects sub ON sub.subject_id = c.subject_id " &
            "JOIN sections sec ON sec.section_id = c.section_id JOIN students st ON st.student_id = ar.student_id " &
            "WHERE ar.status = 'Excused' AND ar.excuse_status = 'Pending' AND c.term_id = @term " &
            "  AND (@c = 0 OR s.class_id = @c) AND (@tch = 0 OR c.teacher_id = @tch) " &
            "ORDER BY s.session_date, st.last_name",
            Db.P("@term", Lookups.ActiveTermId()), Db.P("@c", Lookups.SelectedId(cboClass)), Db.P("@tch", TeacherFilter))
        grid.MultiSelect = True
        grid.DataSource = dt
        Ui.Hide(grid, "record_id", "class_id")
        Ui.Col(grid, "session_date", "Date", 12, "MMM d, yyyy")
        Ui.Col(grid, "student_no", "Student no.", 11)
        Ui.Col(grid, "student_name", "Name", 20)
        Ui.Col(grid, "class_label", "Class", 16)
        Ui.Col(grid, "excuse_requirement", "Requirement", 31)
        Ui.Col(grid, "excuse_status", "Status", 10)
        lblNote.Text = $"{Ui.Plural(dt.Rows.Count, "pending requirement")} · these hours count as absent until complied"
        emptyLabel.Text = "No pending excuse requirements. Nice!"
        emptyLabel.Visible = dt.Rows.Count = 0
    End Sub

    Private Sub RefreshPendingCount()
        Dim n = Db.ToInt(Db.Scalar(
            "SELECT COUNT(*) FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id " &
            "JOIN classes c ON c.class_id = s.class_id WHERE ar.status = 'Excused' AND ar.excuse_status = 'Pending' " &
            "AND c.term_id = @term AND (@tch = 0 OR c.teacher_id = @tch)",
            Db.P("@term", Lookups.ActiveTermId()), Db.P("@tch", TeacherFilter)))
        tabs.SetTabText(1, If(n > 0, $"Pending excuses ({n})", "Pending excuses"))
    End Sub

    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.ColumnIndex >= 0 AndAlso grid.Columns(e.ColumnIndex).Name = "excuse_status" Then Theme.PaintBadgeCell(e)
    End Sub

    Private Sub UpdateButtons()
        btnOpen.Enabled = tabs.SelectedIndex = 0 AndAlso grid.CurrentRow IsNot Nothing AndAlso grid.Rows.Count > 0
        btnComplied.Enabled = tabs.SelectedIndex = 1 AndAlso grid.SelectedRows.Count > 0
        btnSummary.Enabled = Lookups.SelectedId(cboClass) > 0
    End Sub

    Private Sub OpenSession()
        If grid.CurrentRow Is Nothing OrElse tabs.SelectedIndex <> 0 Then Return
        Dim classId = Db.ToInt(grid.CurrentRow.Cells("class_id").Value)
        Dim day = Db.ToDate(grid.CurrentRow.Cells("session_date").Value)
        Host.Navigate("take", Tuple.Create(classId, day))
    End Sub

    ''' <summary>Requirement done: the excused hours stop counting (view uses excuse_status), then re-check warnings.</summary>
    Private Sub MarkComplied()
        Dim rows = grid.SelectedRows.Cast(Of DataGridViewRow)().ToList()
        If rows.Count = 0 Then Return
        If Not Confirm($"Mark {Ui.Plural(rows.Count, "requirement")} as complied? Those excused hours will no longer count as absences.",
                       "Mark complied", "Mark complied") Then Return
        Try
            Db.InTransaction(Sub(cn, tx)
                                 For Each r In rows
                                     Db.Execute(cn, tx, "UPDATE attendance_records SET excuse_status = 'Complied', updated_by = @u WHERE record_id = @id",
                                                Db.P("@u", Session.UserId), Db.P("@id", r.Cells("record_id").Value))
                                 Next
                             End Sub)
            For Each r In rows
                Audit.Log("COMPLIED", "Attendance", Db.ToInt(r.Cells("record_id").Value),
                          $"{r.Cells("student_no").Value} {r.Cells("class_label").Value} {Db.ToDate(r.Cells("session_date").Value):yyyy-MM-dd}")
            Next
            For Each classId In rows.Select(Function(r) Db.ToInt(r.Cells("class_id").Value)).Distinct()
                AttendanceRules.EvaluateClass(classId)
            Next
            LoadData()
            ShowToast($"{Ui.Plural(rows.Count, "requirement")} marked complied")
        Catch ex As Exception
            ShowError(ex, "update the excuse status")
        End Try
    End Sub

End Class
