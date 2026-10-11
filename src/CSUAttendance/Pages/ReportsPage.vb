Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Reports: pick a report on the left, set its filters, preview it, then Print preview (→ PDF) or Export CSV.
''' Teachers only see their own classes and students (Lookups.ClassesFor filters by teacher).
''' </summary>
Public Class ReportsPage
    Inherits PageBase

    Private Shared ReadOnly Reports As (Title As String, Note As String)() = {
        ("Daily Attendance", "One class on one date"),
        ("Class Attendance Summary", "Hours absent and status per student"),
        ("Student Attendance Report", "One student, every subject"),
        ("Warning & Drop List", "Students at Warning 1, 2, Dropped or Tardiness"),
        ("Attendance by Date Range", "Counts per student between two dates")}

    Private ReadOnly picker As DataGridView = Ui.ListGrid()
    Private ReadOnly filters As FlowLayoutPanel = Ui.Row(DockStyle.Fill)
    Private ReadOnly grid As DataGridView = Ui.Grid()
    Private ReadOnly btnPrint As Button = Ui.Action("Print preview", "primary", Icons.Print, 140)
    Private ReadOnly btnCsv As Button = Ui.Action("Export CSV", "secondary", Icons.Export, 120)
    Private ReadOnly cboClass As ComboBox = Ui.Combo(320)
    Private ReadOnly cboStudent As ComboBox = Ui.Combo(320)
    Private ReadOnly cboSection As ComboBox = Ui.Combo(180)
    Private ReadOnly dtpDate As DateTimePicker = Ui.DatePicker(150)
    Private ReadOnly dtpFrom As DateTimePicker = Ui.DatePicker(150)
    Private ReadOnly dtpTo As DateTimePicker = Ui.DatePicker(150)
    Private lblTitle As Label
    Private lblNote As Label
    Private emptyLabel As Label
    Private _summary As New List(Of String)
    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Reports"
        End Get
    End Property

    Public Sub New()
        ' Left: report picker
        Dim left = Ui.Card(DockStyle.Left, 12)
        left.Width = Theme.Dpi(300)
        picker.RowTemplate.Height = Theme.Dpi(64)
        picker.Columns.Add(New DataGridViewTextBoxColumn With {.Name = "item"})
        For Each r In Reports
            picker.Rows.Add(r.Title)
        Next
        left.Controls.Add(picker)
        Dim pickNote As Label = Nothing
        left.Controls.Add(Ui.CardHeader("Choose a report", pickNote, 36))

        ' Right: header (title + buttons), filters row, preview grid
        Dim right = Ui.Card()
        Dim head As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(48), .BackColor = Color.Transparent}
        lblTitle = Ui.Caption("", Theme.FontHeading)
        lblTitle.Location = New Point(0, Theme.Dpi(4))
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.AddRange({btnPrint, btnCsv})
        head.Controls.Add(lblTitle)
        head.Controls.Add(actions)
        Dim filterBar As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(50), .BackColor = Color.Transparent}
        filterBar.Controls.Add(filters)
        right.Controls.Add(grid)
        right.Controls.Add(Ui.CardHeader("Preview", lblNote, 34))
        right.Controls.Add(filterBar)
        right.Controls.Add(head)
        emptyLabel = Ui.EmptyState(grid, "No records for the selected filters.")

        Controls.Add(right)
        Controls.Add(Ui.Gap(DockStyle.Left))
        Controls.Add(left)

        AddHandler picker.CellPainting, AddressOf PaintPickerItem
        AddHandler picker.SelectionChanged, Sub() If Not _loading Then ReportChanged()
        AddHandler grid.CellPainting, AddressOf Grid_CellPainting
        AddHandler grid.CellFormatting, AddressOf Grid_CellFormatting
        For Each c In New Control() {cboClass, cboStudent, cboSection}
            AddHandler DirectCast(c, ComboBox).SelectedIndexChanged, Sub() If Not _loading Then RunReport()
        Next
        For Each d In {dtpDate, dtpFrom, dtpTo}
            AddHandler d.ValueChanged, Sub() If Not _loading Then RunReport()
        Next
        AddHandler btnPrint.Click, Sub() PrintPreview()
        AddHandler btnCsv.Click, Sub() ExportCsv()
    End Sub

    Public Overrides Sub OnPageShown()
        _loading = True
        Lookups.BindClasses(cboClass, Lookups.ActiveTermId())
        If cboClass.Items.Count > 0 Then cboClass.SelectedIndex = 0
        Lookups.BindSections(cboSection)
        Dim students = Db.Query(
            "SELECT DISTINCT s.student_id, CONCAT(s.student_no, '  ·  ', " & TextUtil.SortableNameSql("s") & ") AS label, s.last_name, s.first_name " &
            "FROM students s JOIN class_enrollments e ON e.student_id = s.student_id JOIN classes c ON c.class_id = e.class_id " &
            "WHERE c.term_id = @t AND (@tch = 0 OR c.teacher_id = @tch) ORDER BY s.last_name, s.first_name",
            Db.P("@t", Lookups.ActiveTermId()), Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0)))
        Lookups.Bind(cboStudent, students, "student_id", "label")
        If cboStudent.Items.Count > 0 Then cboStudent.SelectedIndex = 0
        ' Daily report opens on the most recent day that has saved attendance
        Dim lastDay = Db.Scalar("SELECT MAX(s.session_date) FROM attendance_sessions s JOIN classes c ON c.class_id = s.class_id " &
                                "WHERE c.term_id = @t AND (@tch = 0 OR c.teacher_id = @tch)", Db.P("@t", Lookups.ActiveTermId()),
                                Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0)))
        dtpDate.Value = If(lastDay Is Nothing, Date.Today, Db.ToDate(lastDay))
        dtpFrom.Value = Date.Today.AddDays(-30)
        dtpTo.Value = Date.Today
        _loading = False
        picker.CurrentCell = picker.Rows(0).Cells(0)
        ReportChanged()
    End Sub

    Private ReadOnly Property ReportIndex As Integer
        Get
            Return If(picker.CurrentRow Is Nothing, 0, picker.CurrentRow.Index)
        End Get
    End Property

    ''' <summary>Shows only the filters this report needs, then runs it.</summary>
    Private Sub ReportChanged()
        lblTitle.Text = Reports(ReportIndex).Title
        filters.Controls.Clear()
        Select Case ReportIndex
            Case 0 : filters.Controls.AddRange({cboClass, dtpDate})
            Case 1 : filters.Controls.Add(cboClass)
            Case 2 : filters.Controls.Add(cboStudent)
            Case 3 : filters.Controls.Add(cboSection)
            Case 4 : filters.Controls.AddRange({cboClass, dtpFrom, Ui.Muted("to"), dtpTo})
        End Select
        For Each c As Control In filters.Controls
            If TypeOf c Is Label Then c.Margin = New Padding(0, Theme.Dpi(10), Theme.Dpi(12), 0)
        Next
        RunReport()
    End Sub

    ' =====================================================================
    '  Queries
    ' =====================================================================
    Private Sub RunReport()
        _summary = New List(Of String)
        Try
            Dim dt As DataTable
            Select Case ReportIndex
                Case 0 : dt = DailyAttendance()
                Case 1 : dt = ClassSummary()
                Case 2 : dt = StudentReport()
                Case 3 : dt = WarningList()
                Case Else : dt = DateRange()
            End Select
            grid.DataSource = dt
            ApplyHeaders()
            lblNote.Text = Ui.Plural(dt.Rows.Count, "row")
            emptyLabel.Visible = dt.Rows.Count = 0
            btnPrint.Enabled = True
            btnCsv.Enabled = dt.Rows.Count > 0
        Catch ex As Exception
            ShowError(ex, "run the report")
        End Try
    End Sub

    Private ReadOnly Property ClassId As Integer
        Get
            Return Lookups.SelectedId(cboClass)
        End Get
    End Property

    Private Function DailyAttendance() As DataTable
        Dim dt = Db.Query(
            "SELECT st.student_no, " & TextUtil.SortableNameSql("st") & " AS student_name, ar.status, ar.time_in, ar.remarks " &
            "FROM class_enrollments e JOIN students st ON st.student_id = e.student_id " &
            "LEFT JOIN attendance_sessions s ON s.class_id = e.class_id AND s.session_date = @d " &
            "LEFT JOIN attendance_records ar ON ar.session_id = s.session_id AND ar.student_id = e.student_id " &
            "WHERE e.class_id = @c ORDER BY st.last_name, st.first_name", Db.P("@c", ClassId), Db.P("@d", dtpDate.Value.Date))
        If dt.Rows.Cast(Of DataRow)().All(Function(r) r("status") Is DBNull.Value) Then _summary.Add("No attendance was saved for this class on this date.")
        Return dt
    End Function

    Private Function ClassSummary() As DataTable
        Return Db.Query(
            "SELECT student_no, student_name, present_count, late_count, absent_count, excused_count, hours_absent, allowed_hours, " &
            "       percent_absent, attendance_status FROM v_attendance_status WHERE class_id = @c ORDER BY student_name", Db.P("@c", ClassId))
    End Function

    ''' <summary>Per-subject summary goes into the summary lines; the table is the dated history.</summary>
    Private Function StudentReport() As DataTable
        Dim sid = Lookups.SelectedId(cboStudent)
        Dim subjects = Db.Query(
            "SELECT v.subject_code, v.hours_absent, v.allowed_hours, v.percent_absent, v.attendance_status FROM v_attendance_status v " &
            "WHERE v.student_id = @s AND v.term_id = @t AND (@tch = 0 OR v.teacher_id = @tch) ORDER BY v.subject_code",
            Db.P("@s", sid), Db.P("@t", Lookups.ActiveTermId()), Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0)))
        For Each r As DataRow In subjects.Rows
            _summary.Add($"{r("subject_code")}:  {Db.ToDec(r("hours_absent")):0.0#} of {Db.ToDec(r("allowed_hours")):0.0#} allowed hours  ({Db.ToDec(r("percent_absent")):0.0}%)  -  {r("attendance_status")}")
        Next
        Return Db.Query(
            "SELECT s.session_date, sub.subject_code, ar.status, ar.time_in, ar.hours_absent, ar.remarks, ar.excuse_requirement, ar.excuse_status " &
            "FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id JOIN classes c ON c.class_id = s.class_id " &
            "JOIN subjects sub ON sub.subject_id = c.subject_id " &
            "WHERE ar.student_id = @s AND c.term_id = @t AND (@tch = 0 OR c.teacher_id = @tch) ORDER BY s.session_date, sub.subject_code",
            Db.P("@s", sid), Db.P("@t", Lookups.ActiveTermId()), Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0)))
    End Function

    ''' <summary>Students at a warning level or with too many lates (filtered in VB, highest level first).</summary>
    Private Function WarningList() As DataTable
        Dim tardy = AppSettings.TardyAlertCount
        Dim dt = Db.Query(
            "SELECT v.student_no, v.student_name, v.section_name, v.subject_code, v.hours_absent, v.percent_absent, v.late_count, " &
            "       v.attendance_status, w.last_issued " &
            "FROM v_attendance_status v JOIN classes c ON c.class_id = v.class_id " &
            "LEFT JOIN (SELECT class_id, student_id, MAX(issued_at) AS last_issued FROM attendance_warnings GROUP BY class_id, student_id) w " &
            "       ON w.class_id = v.class_id AND w.student_id = v.student_id " &
            "WHERE v.term_id = @t AND (@sec = 0 OR c.section_id = @sec) AND (@tch = 0 OR v.teacher_id = @tch)",
            Db.P("@t", Lookups.ActiveTermId()), Db.P("@sec", Lookups.SelectedId(cboSection)), Db.P("@tch", If(Session.IsTeacher, Session.User.TeacherId, 0)))
        dt.Columns.Add("rank", GetType(Integer))
        For Each r In dt.Rows.Cast(Of DataRow)().ToList()
            Dim s = Db.ToStr(r("attendance_status"))
            If s = "Good" AndAlso tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy Then r("attendance_status") = "Tardiness" : s = "Tardiness"
            If s = "Good" Then
                dt.Rows.Remove(r)
            Else
                r("rank") = Array.IndexOf({"Dropped", "Warning 2", "Warning 1", "Tardiness"}, s)
            End If
        Next
        Dim view = dt.DefaultView
        view.Sort = "rank ASC, percent_absent DESC, student_name ASC"
        Dim sorted = view.ToTable()
        sorted.Columns.Remove("rank")
        _summary.Add($"Dropped: {sorted.Select("attendance_status = 'Dropped'").Length}    Warning 2: {sorted.Select("attendance_status = 'Warning 2'").Length}    " &
                     $"Warning 1: {sorted.Select("attendance_status = 'Warning 1'").Length}    Tardiness: {sorted.Select("attendance_status = 'Tardiness'").Length}")
        Return sorted
    End Function

    Private Function DateRange() As DataTable
        _summary.Add($"Meetings from {dtpFrom.Value:MMMM d, yyyy} to {dtpTo.Value:MMMM d, yyyy}")
        Return Db.Query(
            "SELECT st.student_no, " & TextUtil.SortableNameSql("st") & " AS student_name, " &
            "       COALESCE(SUM(ar.status = 'Present'), 0) AS present_count, COALESCE(SUM(ar.status = 'Late'), 0) AS late_count, " &
            "       COALESCE(SUM(ar.status = 'Absent'), 0) AS absent_count, COALESCE(SUM(ar.status = 'Excused'), 0) AS excused_count, " &
            "       COALESCE(SUM(CASE WHEN ar.status = 'Absent' OR (ar.status = 'Excused' AND ar.excuse_status = 'Pending') THEN ar.hours_absent ELSE 0 END), 0) AS hours_absent " &
            "FROM class_enrollments e JOIN students st ON st.student_id = e.student_id " &
            "LEFT JOIN attendance_sessions s ON s.class_id = e.class_id AND s.session_date BETWEEN @from AND @to " &
            "LEFT JOIN attendance_records ar ON ar.session_id = s.session_id AND ar.student_id = e.student_id " &
            "WHERE e.class_id = @c GROUP BY st.student_id, st.student_no, st.last_name, st.first_name, st.middle_name ORDER BY st.last_name, st.first_name",
            Db.P("@c", ClassId), Db.P("@from", dtpFrom.Value.Date), Db.P("@to", dtpTo.Value.Date))
    End Function

    ''' <summary>Friendly column headers + formats (shared by preview, print and CSV).</summary>
    Private Sub ApplyHeaders()
        Ui.Col(grid, "student_no", "Student no.", 11)
        Ui.Col(grid, "student_name", "Name", 22)
        Ui.Col(grid, "section_name", "Section", 10)
        Ui.Col(grid, "subject_code", "Subject", 10)
        Ui.Col(grid, "session_date", "Date", 12, "MMM d, yyyy")
        Ui.Col(grid, "status", "Status", 14)
        Ui.Col(grid, "time_in", "Time in", 9)
        Ui.Col(grid, "remarks", "Remarks", 16)
        Ui.Col(grid, "present_count", "Present", 7)
        Ui.Col(grid, "late_count", "Late", 6)
        Ui.Col(grid, "absent_count", "Absent", 7)
        Ui.Col(grid, "excused_count", "Excused", 7)
        Ui.Col(grid, "hours_absent", "Hours absent", 9, "0.0#")
        Ui.Col(grid, "allowed_hours", "Allowed", 8, "0.0#")
        Ui.Col(grid, "percent_absent", "% absent", 8, "0.0'%'")
        Ui.Col(grid, "attendance_status", "Status", 11)
        Ui.Col(grid, "last_issued", "Last warning", 11, "MMM d, yyyy")
        Ui.Col(grid, "excuse_requirement", "Requirement", 18)
        Ui.Col(grid, "excuse_status", "Excuse", 9)
        If grid.Columns.Contains("status") Then grid.Columns("status").DefaultCellStyle.NullValue = "Not recorded"
        If ReportIndex = 3 Then Ui.Hide(grid, "late_count")
    End Sub

    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.ColumnIndex < 0 Then Return
        Dim name = grid.Columns(e.ColumnIndex).Name
        If name = "status" OrElse name = "attendance_status" OrElse name = "excuse_status" Then Theme.PaintBadgeCell(e)
    End Sub

    Private Sub Grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.ColumnIndex >= 0 AndAlso grid.Columns(e.ColumnIndex).Name = "time_in" AndAlso TypeOf e.Value Is TimeSpan Then
            e.Value = Date.Today.Add(DirectCast(e.Value, TimeSpan)).ToString("h:mm tt")
            e.FormattingApplied = True
        End If
    End Sub

    Private Sub PaintPickerItem(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 Then Return
        Dim active = e.RowIndex = ReportIndex
        Dim g = e.Graphics
        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
        g.FillRectangle(Brushes.White, e.CellBounds)
        Dim box = Rectangle.Inflate(e.CellBounds, -Theme.Dpi(2), -Theme.Dpi(3))
        If active Then
            Using path = Theme.RoundedRect(box, Theme.Dpi(8)), br As New SolidBrush(Theme.AccentSoft)
                g.FillPath(br, path)
            End Using
            Using br As New SolidBrush(Theme.Accent)
                g.FillRectangle(br, box.X, box.Y + Theme.Dpi(14), Theme.Dpi(3), box.Height - Theme.Dpi(28))
            End Using
        End If
        Dim r = Reports(e.RowIndex)
        TextRenderer.DrawText(g, r.Title, Theme.FontBold, New Rectangle(box.X + Theme.Dpi(14), box.Y + Theme.Dpi(8), box.Width - Theme.Dpi(20), Theme.Dpi(24)),
                              If(active, Theme.Accent, Theme.TextPrimary), TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)
        TextRenderer.DrawText(g, r.Note, Theme.FontSmall, New Rectangle(box.X + Theme.Dpi(14), box.Y + Theme.Dpi(32), box.Width - Theme.Dpi(20), Theme.Dpi(20)),
                              Theme.TextMuted, TextFormatFlags.Left Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        e.Handled = True
    End Sub

    ' =====================================================================
    '  Output
    ' =====================================================================
    Private Function FilterLine() As String
        Select Case ReportIndex
            Case 0 : Return $"Class: {cboClass.Text}    Date: {dtpDate.Value:dddd, MMMM d, yyyy}"
            Case 1 : Return $"Class: {cboClass.Text}    Term: {Lookups.ActiveTermLabel()}"
            Case 2 : Return $"Student: {cboStudent.Text}    Term: {Lookups.ActiveTermLabel()}"
            Case 3 : Return $"Section: {cboSection.Text}    Term: {Lookups.ActiveTermLabel()}    Basis: CSU Student Manual s.II.B.4, s.II.C.2.1"
            Case Else : Return $"Class: {cboClass.Text}    From {dtpFrom.Value:MMM d, yyyy} to {dtpTo.Value:MMM d, yyyy}"
        End Select
    End Function

    Private Sub PrintPreview()
        Try
            Dim printer As New ReportPrinter(Reports(ReportIndex).Title, FilterLine().Replace("  ·  ", " · "), grid, _summary)
            printer.ShowPreview(FindForm())
            Audit.Log("PRINT", "Report", 0, $"{Reports(ReportIndex).Title} - {FilterLine()}")
        Catch ex As Exception
            ShowError(ex, "open the print preview")
        End Try
    End Sub

    Private Sub ExportCsv()
        Try
            Dim path = CsvExporter.Export(grid, Reports(ReportIndex).Title, FindForm())
            If path = "" Then Return
            Audit.Log("EXPORT", "Report", 0, $"{Reports(ReportIndex).Title} → {IO.Path.GetFileName(path)}")
            ShowToast($"Saved {IO.Path.GetFileName(path)}")
        Catch ex As Exception
            ShowError(ex, "export the CSV file")
        End Try
    End Sub

End Class
