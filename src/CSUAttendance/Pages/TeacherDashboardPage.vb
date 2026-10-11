Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Teacher home: greeting, four stat cards, a timeline of today's classes (take / edit attendance from it)
''' and the students who need attention in the teacher's classes.
''' </summary>
Public Class TeacherDashboardPage
    Inherits PageBase

    Private ReadOnly cardClasses As New StatCard With {.Glyph = Icons.Classes, .Caption = "My classes"}
    Private ReadOnly cardStudents As New StatCard With {.Glyph = Icons.Students, .Caption = "My students"}
    Private ReadOnly cardRate As New StatCard With {.Glyph = Icons.CheckList, .Caption = "This week's attendance"}
    Private ReadOnly cardRisk As New StatCard With {.Glyph = Icons.WarningSign, .Caption = "Needs attention"}
    Private ReadOnly timeline As New ClassTimeline With {.Dock = DockStyle.Fill}
    Private ReadOnly gridRisk As DataGridView = Ui.ListGrid()
    Private lblTodayNote, lblRiskNote, emptyToday, emptyRisk As Label
    Private greeting As Panel

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Dashboard"
        End Get
    End Property

    Public Sub New()
        cardClasses.SetAccent(Theme.Info, Theme.InfoSoft)
        cardRate.SetAccent(Theme.Success, Theme.SuccessSoft)
        cardRisk.SetAccent(Theme.Orange, Theme.OrangeSoft)

        Dim board As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 1, .BackColor = Theme.Background}
        board.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 56))
        board.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 44))

        Dim today = Ui.Card()
        today.Margin = New Padding(0, 0, Theme.Dpi(8), 0)
        today.Controls.Add(timeline)
        today.Controls.Add(Ui.CardHeader("Today's classes", lblTodayNote))
        emptyToday = New Label With {.Dock = DockStyle.Fill, .TextAlign = ContentAlignment.MiddleCenter, .ForeColor = Theme.TextMuted,
                                     .BackColor = Theme.Surface, .Visible = False}
        timeline.Controls.Add(emptyToday)


        Dim risk = Ui.Card()
        risk.Margin = New Padding(Theme.Dpi(8), 0, 0, 0)
        risk.Controls.Add(gridRisk)
        risk.Controls.Add(Ui.CardHeader("Needs attention", lblRiskNote))
        emptyRisk = Ui.EmptyState(gridRisk, "Everyone is in good standing.")

        board.Controls.Add(today, 0, 0)
        board.Controls.Add(risk, 1, 0)

        greeting = Ui.Greeting(Ui.FirstName(Session.User.FullName), "")
        Controls.Add(board)
        Controls.Add(Ui.Gap(DockStyle.Top))
        Controls.Add(Ui.EqualRow(116, cardClasses, cardStudents, cardRate, cardRisk))
        Controls.Add(greeting)

        AddHandler timeline.OpenClass, Sub(id) Host.Navigate("take", Tuple.Create(id, Date.Today))
        AddHandler gridRisk.CellPainting, Sub(s, e) If e.ColumnIndex >= 0 AndAlso gridRisk.Columns(e.ColumnIndex).Name = "standing" Then Theme.PaintBadgeCell(e)
        AddHandler gridRisk.CellDoubleClick, Sub(s, e) If e.RowIndex >= 0 Then Host.Navigate("mystudents", Db.ToInt(gridRisk.Rows(e.RowIndex).Cells("class_id").Value))
    End Sub

    Public Overrides Sub OnPageShown()
        Dim termId = Lookups.ActiveTermId()
        Dim me_ = Session.User.TeacherId
        DirectCast(greeting.Tag, Label).Text = $"{Date.Today:dddd, MMMM d}  ·  {Lookups.ActiveTermLabel()}"

        Dim classes = Lookups.ClassesFor(termId)
        cardClasses.Value = classes.Rows.Count.ToString()
        cardClasses.Note = $"{classes.Rows.Cast(Of DataRow)().Select(Function(r) Db.ToStr(r("section_name"))).Distinct().Count()} sections"
        cardStudents.Value = Db.ToInt(Db.Scalar(
            "SELECT COUNT(DISTINCT e.student_id) FROM class_enrollments e JOIN classes c ON c.class_id = e.class_id " &
            "WHERE c.term_id = @t AND c.teacher_id = @me", Db.P("@t", termId), Db.P("@me", me_))).ToString()
        cardStudents.Note = "enrolled in your classes"

        Dim monday = Date.Today.AddDays(-((7 + CInt(Date.Today.DayOfWeek) - 1) Mod 7))
        Dim week = Db.Query(
            "SELECT COUNT(*) AS total, COALESCE(SUM(ar.status IN ('Present','Late')), 0) AS here FROM attendance_records ar " &
            "JOIN attendance_sessions s ON s.session_id = ar.session_id JOIN classes c ON c.class_id = s.class_id " &
            "WHERE c.teacher_id = @me AND s.session_date BETWEEN @from AND CURDATE()", Db.P("@me", me_), Db.P("@from", monday)).Rows(0)
        Dim total = Db.ToInt(week("total"))
        cardRate.Value = If(total = 0, "—", $"{Db.ToInt(week("here")) * 100.0 / total:0}%")
        cardRate.Note = If(total = 0, "No attendance taken yet this week", $"since Monday, {monday:MMM d}")

        LoadToday(classes)
        LoadRisk(termId, me_)
    End Sub

    Private Sub LoadToday(classes As DataTable)
        Dim taken = Db.Query(
            "SELECT s.class_id, SUM(ar.status = 'Present') AS p, SUM(ar.status = 'Late') AS l, SUM(ar.status = 'Absent') AS a, SUM(ar.status = 'Excused') AS e " &
            "FROM attendance_sessions s LEFT JOIN attendance_records ar ON ar.session_id = s.session_id " &
            "WHERE s.session_date = CURDATE() GROUP BY s.class_id")
        Dim entries As New List(Of TimelineEntry)
        For Each r As DataRow In classes.Rows
            If Not AttendanceRules.MeetsOn(Db.ToStr(r("days")), Date.Today) Then Continue For
            Dim id = Db.ToInt(r("class_id"))
            Dim t = taken.Rows.Cast(Of DataRow)().FirstOrDefault(Function(x) Db.ToInt(x("class_id")) = id)
            Dim info = Db.Query("SELECT sub.description, c.room FROM classes c JOIN subjects sub ON sub.subject_id = c.subject_id WHERE c.class_id = @c",
                                Db.P("@c", id)).Rows(0)
            entries.Add(New TimelineEntry With {
                .ClassId = id, .Start = Db.ToTime(r("start_time")), .[End] = Db.ToTime(r("end_time")),
                .Title = $"{r("subject_code")} · {r("section_name")}",
                .Detail = $"{info("description")}{If(Db.ToStr(info("room")) = "", "", " · " & Db.ToStr(info("room")))}",
                .Taken = t IsNot Nothing,
                .Summary = If(t Is Nothing, "", $"{Db.ToInt(t("p"))} P · {Db.ToInt(t("l"))} L · {Db.ToInt(t("a"))} A · {Db.ToInt(t("e"))} E")
            })
        Next
        timeline.SetEntries(entries.OrderBy(Function(x) x.Start))

        emptyToday.Text = $"No classes on {Date.Today:dddd}. Use Take Attendance to record a make-up session."
        emptyToday.Visible = entries.Count = 0
        lblTodayNote.Text = If(entries.Count = 0, "", $"{entries.Where(Function(x) x.Taken).Count()} of {entries.Count} taken")
    End Sub

    ''' <summary>My students with a warning level or too many lates (status checked in VB).</summary>
    Private Sub LoadRisk(termId As Integer, teacherId As Integer)
        Dim tardy = AppSettings.TardyAlertCount
        Dim dt = Db.Query(
            "SELECT v.class_id, v.student_name, v.subject_code, v.percent_absent, v.late_count, v.attendance_status AS standing " &
            "FROM v_attendance_status v WHERE v.term_id = @t AND v.teacher_id = @me ORDER BY v.percent_absent DESC, v.late_count DESC",
            Db.P("@t", termId), Db.P("@me", teacherId))
        For Each r In dt.Rows.Cast(Of DataRow)().ToList()
            Dim s = Db.ToStr(r("standing"))
            If s = "Good" AndAlso tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy Then
                r("standing") = "Tardiness"
            ElseIf s = "Good" Then
                dt.Rows.Remove(r)
            End If
        Next
        gridRisk.DataSource = dt
        Ui.Hide(gridRisk, "class_id", "late_count")
        Ui.Col(gridRisk, "student_name", "", 34)
        Ui.Col(gridRisk, "subject_code", "", 20)
        Ui.Col(gridRisk, "percent_absent", "", 20, "0.0'%'")
        Ui.Col(gridRisk, "standing", "", 26)
        cardRisk.Value = dt.Rows.Cast(Of DataRow)().Select(Function(r) Db.ToStr(r("student_name"))).Distinct().Count().ToString()
        cardRisk.Note = "warnings, drops and tardiness"
        lblRiskNote.Text = If(dt.Rows.Count = 0, "", "double-click to open the class")
        emptyRisk.Visible = dt.Rows.Count = 0
    End Sub

End Class
