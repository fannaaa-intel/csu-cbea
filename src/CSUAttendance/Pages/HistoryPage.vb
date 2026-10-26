Imports System.Windows.Forms

''' <summary>Student's own dated attendance (newest first), filterable by subject. NavigationArg = subject code to preselect.</summary>
Public Class HistoryPage
    Inherits PageBase

    Private ReadOnly cboSubject As ComboBox = Ui.Combo(300)
    Private ReadOnly chips As New CountChips()
    Private ReadOnly grid As DataGridView = Ui.Grid()
    Private lblNote As Label
    Private emptyLabel As Label
    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Attendance History"
        End Get
    End Property

    Public Sub New()
        Dim card = Ui.Card()
        card.Controls.Add(grid)
        card.Controls.Add(Ui.CardHeader("Class meetings", lblNote))
        emptyLabel = Ui.EmptyState(grid, "No attendance has been recorded for you yet.")

        Dim bar = Ui.Toolbar()
        Dim row = Ui.Row()
        chips.Margin = New Padding(Theme.Dpi(8), Theme.Dpi(4), 0, 0)
        row.Controls.AddRange({cboSubject, chips})
        bar.Controls.Add(row)
        Controls.Add(card)
        Controls.Add(bar)

        AddHandler cboSubject.SelectedIndexChanged, Sub() If Not _loading Then LoadData()
        AddHandler grid.CellPainting, Sub(s, e) If e.ColumnIndex >= 0 AndAlso (grid.Columns(e.ColumnIndex).Name = "status" OrElse grid.Columns(e.ColumnIndex).Name = "excuse_status") Then Theme.PaintBadgeCell(e)
        AddHandler grid.CellFormatting, AddressOf FormatTime
        AddHandler grid.CellClick, AddressOf OpenAttachment
    End Sub

    Public Overrides Sub OnPageShown()
        _loading = True
        Dim dt = Db.Query(
            "SELECT DISTINCT sub.subject_code, CONCAT(sub.subject_code, '  ·  ', sub.description) AS label " &
            "FROM class_enrollments e JOIN classes c ON c.class_id = e.class_id JOIN subjects sub ON sub.subject_id = c.subject_id " &
            "JOIN school_terms t ON t.term_id = c.term_id AND t.is_active = 1 WHERE e.student_id = @s ORDER BY sub.subject_code",
            Db.P("@s", Session.User.StudentId))
        Dim all = dt.NewRow()
        all("subject_code") = ""
        all("label") = "All subjects"
        dt.Rows.InsertAt(all, 0)
        Lookups.Bind(cboSubject, dt, "subject_code", "label")
        cboSubject.SelectedIndex = 0
        If TypeOf NavigationArg Is String Then cboSubject.SelectedValue = CStr(NavigationArg)
        _loading = False
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim code = If(cboSubject.SelectedValue Is Nothing, "", CStr(cboSubject.SelectedValue))
        Dim dt = Db.Query(
            "SELECT s.session_date, DATE_FORMAT(s.session_date, '%a') AS day_name, sub.subject_code, ar.status, ar.time_in, " &
            "       ar.remarks, ar.attachment_path, ar.excuse_requirement, ar.excuse_status " &
            "FROM attendance_records ar JOIN attendance_sessions s ON s.session_id = ar.session_id " &
            "JOIN classes c ON c.class_id = s.class_id JOIN subjects sub ON sub.subject_id = c.subject_id " &
            "JOIN school_terms t ON t.term_id = c.term_id AND t.is_active = 1 " &
            "WHERE ar.student_id = @s AND (@code = '' OR sub.subject_code = @code) ORDER BY s.session_date DESC, sub.subject_code",
            Db.P("@s", Session.User.StudentId), Db.P("@code", code))
        grid.DataSource = dt
        Ui.Col(grid, "session_date", "Date", 12, "MMM d, yyyy")
        Ui.Col(grid, "day_name", "Day", 6)
        Ui.Col(grid, "subject_code", "Subject", 10)
        Ui.Col(grid, "status", "Status", 11)
        Ui.Col(grid, "time_in", "Time in", 9)
        Ui.Col(grid, "remarks", "Reason / remarks", 18)
        Ui.Col(grid, "attachment_path", "Attachment", 10)
        Ui.Col(grid, "excuse_requirement", "Requirement", 22)
        Ui.Col(grid, "excuse_status", "Excuse", 11)
        Dim count = Function(s As String) dt.Rows.Cast(Of DataRow)().Count(Function(r) Db.ToStr(r("status")) = s).ToString()
        chips.SetItems(AttendanceRules.Statuses.Select(Function(s) New KeyValuePair(Of String, String)(s, count(s))))
        lblNote.Text = $"{Ui.Plural(dt.Rows.Count, "meeting")} · newest first"
        emptyLabel.Visible = dt.Rows.Count = 0
    End Sub

    Private Sub FormatTime(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If e.ColumnIndex < 0 Then Return
        Dim name = grid.Columns(e.ColumnIndex).Name
        If name = "time_in" AndAlso TypeOf e.Value Is TimeSpan Then
            e.Value = Date.Today.Add(DirectCast(e.Value, TimeSpan)).ToString("h:mm tt")
            e.FormattingApplied = True
        ElseIf name = "attachment_path" AndAlso Db.ToStr(e.Value) <> "" Then
            e.Value = "View file"                ' click the cell to open it
            e.CellStyle.ForeColor = Theme.Accent
            e.CellStyle.Font = Theme.FontSmallBold
            e.FormattingApplied = True
        End If
    End Sub

    Private Sub OpenAttachment(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 OrElse grid.Columns(e.ColumnIndex).Name <> "attachment_path" Then Return
        Attachments.Open(Me, Db.ToStr(grid.Rows(e.RowIndex).Cells(e.ColumnIndex).Value))
    End Sub

End Class
