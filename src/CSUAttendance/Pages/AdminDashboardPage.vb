Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Admin home: greeting, four stat cards, absence hours by subject, the standing ladder,
''' the latest warnings and the classes whose attendance has not been taken today.
''' Status words are counted in VB (never compared in SQL) so collations cannot break the page.
''' </summary>
Public Class AdminDashboardPage
    Inherits PageBase

    Private ReadOnly cardStudents As New StatCard With {.Glyph = Icons.Students, .Caption = "Active students"}
    Private ReadOnly cardClasses As New StatCard With {.Glyph = Icons.Classes, .Caption = "Classes this term"}
    Private ReadOnly cardRate As New StatCard With {.Glyph = Icons.CheckList, .Caption = "Today's attendance rate"}
    Private ReadOnly cardRisk As New StatCard With {.Glyph = Icons.WarningSign, .Caption = "Students needing attention"}
    Private ReadOnly chart As New SimpleBarChart With {.Dock = DockStyle.Fill, .ValueFormat = "0.#"}
    Private ReadOnly ladder As New StandingLadder With {.Dock = DockStyle.Top}
    Private ReadOnly gridWarnings As DataGridView = Ui.ListGrid()
    Private ReadOnly gridPending As DataGridView = Ui.ListGrid()
    Private ReadOnly gridRisk As DataGridView = Ui.ListGrid()
    Private lblChartNote, lblLadderNote, lblWarnNote, lblPendingNote As Label
    Private emptyWarnings, emptyPending, emptyRisk As Label
    Private greeting As Panel

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Dashboard"
        End Get
    End Property

    Public Sub New()
        cardRate.SetAccent(Theme.Success, Theme.SuccessSoft)
        cardRisk.SetAccent(Theme.Orange, Theme.OrangeSoft)
        cardClasses.SetAccent(Theme.Info, Theme.InfoSoft)

        ' 2 x 2 board of cards
        Dim board As New TableLayoutPanel With {.Dock = DockStyle.Fill, .ColumnCount = 2, .RowCount = 2, .BackColor = Theme.Background}
        board.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 55))
        board.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 45))
        board.RowStyles.Add(New RowStyle(SizeType.Percent, 50))
        board.RowStyles.Add(New RowStyle(SizeType.Percent, 50))

        board.Controls.Add(MakeCard("Absence hours by subject", lblChartNote, chart, 0, 0), 0, 0)
        Dim ladderHost As New Panel With {.Dock = DockStyle.Fill, .BackColor = Theme.Surface}
        Dim riskCaption = Ui.Caption("Classes with students at risk", Theme.FontSmallBold, Theme.TextMuted)
        riskCaption.AutoSize = False
        riskCaption.Dock = DockStyle.Top
        riskCaption.Height = Theme.Dpi(40)
        riskCaption.TextAlign = ContentAlignment.BottomLeft
        gridRisk.Dock = DockStyle.Fill
        ladderHost.Controls.Add(gridRisk)        ' Fill first, then the Top docks (last added = topmost)
        ladderHost.Controls.Add(riskCaption)
        ladderHost.Controls.Add(ladder)
        board.Controls.Add(MakeCard("Standing across all classes", lblLadderNote, ladderHost, 1, 0), 1, 0)
        board.Controls.Add(MakeCard("Latest warnings", lblWarnNote, gridWarnings, 0, 1), 0, 1)
        board.Controls.Add(MakeCard("Attendance not yet taken today", lblPendingNote, gridPending, 1, 1), 1, 1)
        emptyWarnings = Ui.EmptyState(gridWarnings, "No warnings issued yet this term.")
        emptyPending = Ui.EmptyState(gridPending, "")
        emptyRisk = Ui.EmptyState(gridRisk, "Every class is in good standing.")

        Dim stats = Ui.EqualRow(116, cardStudents, cardClasses, cardRate, cardRisk)
        Controls.Add(board)
        Controls.Add(Ui.Gap(DockStyle.Top))
        Controls.Add(stats)
        greeting = Ui.Greeting(Session.User.FullName, "")
        Controls.Add(greeting)

        AddHandler gridWarnings.CellPainting, Sub(s, e) If e.ColumnIndex >= 0 AndAlso gridWarnings.Columns(e.ColumnIndex).Name = "level" Then Theme.PaintBadgeCell(e)
        AddHandler gridPending.CellDoubleClick, Sub(s, e) If e.RowIndex >= 0 Then OpenPending(e.RowIndex)
    End Sub

    Private Function MakeCard(title As String, ByRef note As Label, content As Control, col As Integer, row As Integer) As CardPanel
        Dim c = Ui.Card()
        c.Margin = New Padding(If(col = 0, 0, Theme.Dpi(8)), If(row = 0, 0, Theme.Dpi(8)), If(col = 0, Theme.Dpi(8), 0), If(row = 0, Theme.Dpi(8), 0))
        content.Dock = DockStyle.Fill
        c.Controls.Add(content)
        c.Controls.Add(Ui.CardHeader(title, note, 36))
        Return c
    End Function

    Public Overrides Sub OnPageShown()
        Dim termId = Lookups.ActiveTermId()
        DirectCast(greeting.Tag, Label).Text = $"{Date.Today:dddd, MMMM d}  ·  {Lookups.ActiveTermLabel()}"

        ' --- stat cards ---
        cardStudents.Value = Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM students WHERE status = 'Active'")).ToString()
        cardStudents.Note = $"{Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM sections"))} sections"
        cardClasses.Value = Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM classes WHERE term_id = @t AND is_active = 1", Db.P("@t", termId))).ToString()
        cardClasses.Note = $"{Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM teachers"))} teachers"

        Dim today = Db.Query("SELECT COUNT(*) AS total, COALESCE(SUM(ar.status IN ('Present','Late')), 0) AS here " &
                             "FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id WHERE s.session_date = CURDATE()").Rows(0)
        Dim total = Db.ToInt(today("total"))
        cardRate.Value = If(total = 0, "—", $"{Db.ToInt(today("here")) * 100.0 / total:0}%")
        cardRate.Note = If(total = 0, "No attendance taken yet today", $"{Db.ToInt(today("here"))} of {total} students present")

        ' --- standing (counted in VB) ---
        Dim status = Db.Query("SELECT v.student_id, v.class_id, v.subject_code, v.section_name, v.attendance_status " &
                              "FROM v_attendance_status v WHERE v.term_id = @t", Db.P("@t", termId))
        Dim count = Function(s As String) status.Rows.Cast(Of DataRow)().Count(Function(r) Db.ToStr(r("attendance_status")) = s)
        Dim atRisk = status.Rows.Cast(Of DataRow)().Where(Function(r) Db.ToStr(r("attendance_status")) <> "Good").Select(Function(r) Db.ToInt(r("student_id"))).Distinct().Count()
        cardRisk.Value = atRisk.ToString()
        cardRisk.Note = $"{count("Dropped")} reached the drop limit"
        ladder.SetCounts(count("Good"), count("Warning 1"), count("Warning 2"), count("Dropped"))
        lblLadderNote.Text = $"{Ui.Plural(status.Rows.Count, "enrollment")}"
        LoadRiskByClass(status)

        ' --- chart ---
        Dim hours = Db.Query("SELECT v.subject_code, SUM(v.hours_absent) AS hrs FROM v_attendance_status v WHERE v.term_id = @t " &
                             "GROUP BY v.subject_code ORDER BY v.subject_code", Db.P("@t", termId))
        chart.SetData(hours.Rows.Cast(Of DataRow)().Select(Function(r) Db.ToStr(r("subject_code"))),
                      hours.Rows.Cast(Of DataRow)().Select(Function(r) Db.ToDec(r("hrs"))))
        lblChartNote.Text = "total hours absent this term"

        LoadWarnings()
        LoadPending()
    End Sub

    Private Sub LoadWarnings()
        Dim dt = Db.Query(
            "SELECT w.issued_at, CONCAT(st.last_name, ', ', st.first_name) AS student, sub.subject_code, w.level " &
            "FROM attendance_warnings w JOIN students st ON st.student_id = w.student_id JOIN classes c ON c.class_id = w.class_id " &
            "JOIN subjects sub ON sub.subject_id = c.subject_id ORDER BY w.issued_at DESC, w.warning_id DESC LIMIT 8")
        gridWarnings.DataSource = dt
        Ui.Col(gridWarnings, "issued_at", "", 18, "MMM d")
        Ui.Col(gridWarnings, "student", "", 42)
        Ui.Col(gridWarnings, "subject_code", "", 18)
        Ui.Col(gridWarnings, "level", "", 22)
        gridWarnings.Columns("issued_at").DefaultCellStyle.ForeColor = Theme.TextMuted
        lblWarnNote.Text = "most recent first"
        emptyWarnings.Visible = dt.Rows.Count = 0
    End Sub

    ''' <summary>One row per class that has students on warning or dropped (counted in VB, worst classes first).</summary>
    Private Sub LoadRiskByClass(status As DataTable)
        Dim dt As New DataTable()
        dt.Columns.Add("class_label")
        dt.Columns.Add("summary")
        Dim groups = status.Rows.Cast(Of DataRow)().
            Where(Function(r) Db.ToStr(r("attendance_status")) <> "Good").
            GroupBy(Function(r) $"{r("subject_code")} · {r("section_name")}").
            OrderByDescending(Function(g) g.Count())
        For Each g In groups
            Dim parts As New List(Of String)
            For Each level In {"Warning 1", "Warning 2", "Dropped"}
                Dim n = g.Count(Function(r) Db.ToStr(r("attendance_status")) = level)
                If n > 0 Then parts.Add($"{n} {level}")
            Next
            dt.Rows.Add(g.Key, String.Join("  ·  ", parts))
        Next
        gridRisk.DataSource = dt
        Ui.Col(gridRisk, "class_label", "", 45)
        Ui.Col(gridRisk, "summary", "", 55)
        gridRisk.Columns("summary").DefaultCellStyle.ForeColor = Theme.TextMuted
        emptyRisk.Visible = dt.Rows.Count = 0
    End Sub

    ''' <summary>Classes that meet today (by their days) but have no session saved for today.</summary>
    Private Sub LoadPending()
        Dim dt = Db.Query(
            "SELECT c.class_id, c.days, CONCAT(sub.subject_code, ' · ', sec.section_name) AS class_label, " &
            "       TRIM(TIME_FORMAT(c.start_time, '%l:%i %p')) AS starts, t.last_name AS teacher " &
            "FROM classes c JOIN subjects sub ON sub.subject_id = c.subject_id JOIN sections sec ON sec.section_id = c.section_id " &
            "JOIN teachers t ON t.teacher_id = c.teacher_id " &
            "LEFT JOIN attendance_sessions s ON s.class_id = c.class_id AND s.session_date = CURDATE() " &
            "WHERE c.term_id = @t AND c.is_active = 1 AND s.session_id IS NULL ORDER BY c.start_time",
            Db.P("@t", Lookups.ActiveTermId()))
        For Each r In dt.Rows.Cast(Of DataRow)().ToList()
            If Not AttendanceRules.MeetsOn(Db.ToStr(r("days")), Date.Today) Then dt.Rows.Remove(r)
        Next
        gridPending.DataSource = dt
        Ui.Hide(gridPending, "class_id", "days")
        Ui.Col(gridPending, "starts", "", 26)
        Ui.Col(gridPending, "class_label", "", 46)
        Ui.Col(gridPending, "teacher", "", 28)
        gridPending.Columns("starts").DisplayIndex = 0
        gridPending.Columns("starts").DefaultCellStyle.ForeColor = Theme.TextMuted
        lblPendingNote.Text = If(dt.Rows.Count = 0, "", "double-click to take it")
        emptyPending.Text = If(Date.Today.DayOfWeek = DayOfWeek.Sunday, "No classes meet on Sundays.", "All of today's classes have attendance. ")
        emptyPending.Visible = dt.Rows.Count = 0
    End Sub

    Private Sub OpenPending(rowIndex As Integer)
        Dim id = Db.ToInt(gridPending.Rows(rowIndex).Cells("class_id").Value)
        Host.Navigate("take", Tuple.Create(id, Date.Today))
    End Sub

End Class
