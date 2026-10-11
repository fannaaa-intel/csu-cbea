Imports System.Windows.Forms

''' <summary>
''' My Students: every student of a class with P/L/A/E counts, absence hours and status from v_attendance_status.
''' Students at risk are listed first. Double-click → dated history of that student in this class.
''' </summary>
Public Class MyStudentsPage
    Inherits PageBase

    Private ReadOnly cboClass As ComboBox = Ui.Combo(380)
    Private ReadOnly txtSearch As TextField = Ui.Search("Search student no. or name", 240)
    Private ReadOnly chips As New CountChips()
    Private ReadOnly btnHistory As Button = Ui.Action("View history", "primary", Icons.View, 140)
    Private ReadOnly grid As DataGridView = Ui.Grid()
    Private lblNote As Label
    Private emptyLabel As Label
    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "My Students"
        End Get
    End Property

    Public Sub New()
        Dim card = Ui.Card()
        card.Controls.Add(grid)
        card.Controls.Add(Ui.CardHeader("Attendance standing", lblNote))
        emptyLabel = Ui.EmptyState(grid, "")

        Dim strip As New Panel With {.Dock = DockStyle.Top, .Height = Theme.Dpi(52), .BackColor = Theme.Background}
        chips.Location = New Drawing.Point(0, Theme.Dpi(4))
        strip.Controls.Add(chips)

        Dim bar = Ui.Toolbar()
        Dim filters = Ui.Row()
        filters.Controls.AddRange({cboClass, txtSearch})
        Dim actions = Ui.Row(DockStyle.Right)
        actions.Controls.Add(btnHistory)
        bar.Controls.Add(filters)
        bar.Controls.Add(actions)

        Controls.Add(card)
        Controls.Add(strip)
        Controls.Add(bar)

        AddHandler cboClass.SelectedIndexChanged, Sub() If Not _loading Then LoadData()
        AddHandler txtSearch.TextChanged, Sub() If Not _loading Then LoadData()
        AddHandler grid.CellPainting, AddressOf Grid_CellPainting
        AddHandler grid.CellDoubleClick, Sub(s, e) If e.RowIndex >= 0 Then ShowHistory()
        AddHandler grid.SelectionChanged, Sub() btnHistory.Enabled = grid.CurrentRow IsNot Nothing AndAlso grid.Rows.Count > 0
        AddHandler btnHistory.Click, Sub() ShowHistory()
    End Sub

    Public Overrides Sub OnPageShown()
        _loading = True
        Lookups.BindClasses(cboClass, Lookups.ActiveTermId())
        If TypeOf NavigationArg Is Integer AndAlso CInt(NavigationArg) > 0 Then
            cboClass.SelectedValue = CInt(NavigationArg)
        ElseIf cboClass.Items.Count > 0 Then
            cboClass.SelectedIndex = 0
        End If
        _loading = False
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim classId = Lookups.SelectedId(cboClass)
        Dim q = txtSearch.Text.Trim()
        Dim tardy = AppSettings.TardyAlertCount
        Dim dt = Db.Query(
            "SELECT v.student_id, v.student_no, v.student_name, v.present_count, v.late_count, v.absent_count, v.excused_count, " &
            "       v.hours_absent, v.allowed_hours, v.percent_absent, v.attendance_status AS standing " &
            "FROM v_attendance_status v WHERE v.class_id = @c " &
            "  AND (@q = '' OR v.student_no LIKE @like OR v.student_name LIKE @like)",
            Db.P("@c", classId), Db.P("@q", q), Db.P("@like", "%" & q & "%"))
        RankByRisk(dt, tardy)
        grid.DataSource = dt.DefaultView
        Ui.Hide(grid, "student_id", "risk")
        Ui.Col(grid, "student_no", "Student no.", 10)
        Ui.Col(grid, "student_name", "Name", 20)
        Ui.Col(grid, "present_count", "P", 5)
        Ui.Col(grid, "late_count", "L", 5)
        Ui.Col(grid, "absent_count", "A", 5)
        Ui.Col(grid, "excused_count", "E", 5)
        Ui.Col(grid, "hours_absent", "Hours absent", 10, "0.0#")
        Ui.Col(grid, "allowed_hours", "Allowed", 8, "0.0#")
        Ui.Col(grid, "percent_absent", "% absent", 8, "0.0'%'")
        Ui.Col(grid, "standing", "Status", 12)

        ' Summary chips (whole class, not just the search result)
        Dim all = Db.Query("SELECT attendance_status, late_count FROM v_attendance_status WHERE class_id = @c", Db.P("@c", classId))
        Dim count = Function(s As String) all.Rows.Cast(Of DataRow)().Count(Function(r) Db.ToStr(r("attendance_status")) = s)
        Dim tardies = all.Rows.Cast(Of DataRow)().Count(Function(r) tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy)
        chips.SetItems({New KeyValuePair(Of String, String)("Good", count("Good").ToString()),
                        New KeyValuePair(Of String, String)("Warning 1", count("Warning 1").ToString()),
                        New KeyValuePair(Of String, String)("Warning 2", count("Warning 2").ToString()),
                        New KeyValuePair(Of String, String)("Dropped", count("Dropped").ToString()),
                        New KeyValuePair(Of String, String)("Tardiness", tardies.ToString())})

        Dim atRisk = all.Rows.Count - count("Good")
        lblNote.Text = $"{Ui.Plural(all.Rows.Count, "student")} · {atRisk} need attention · at-risk students first"
        emptyLabel.Text = If(classId = 0, "No classes in the active term.", If(q = "", "No students enrolled in this class.", "No students match your search."))
        emptyLabel.Visible = dt.Rows.Count = 0
        btnHistory.Enabled = dt.Rows.Count > 0
    End Sub

    ''' <summary>Adds Tardiness for Good students with too many lates, then sorts at-risk students first (done in VB, not SQL).</summary>
    Private Shared Sub RankByRisk(dt As DataTable, tardy As Integer)
        dt.Columns.Add("risk", GetType(Integer))
        For Each r As DataRow In dt.Rows
            Dim s = Db.ToStr(r("standing"))
            If s = "Good" AndAlso tardy > 0 AndAlso Db.ToInt(r("late_count")) >= tardy Then r("standing") = "Tardiness"
            r("risk") = Array.IndexOf({"Dropped", "Warning 2", "Warning 1", "Tardiness", "Good"}, Db.ToStr(r("standing")))
        Next
        dt.DefaultView.Sort = "risk ASC, percent_absent DESC, late_count DESC, student_name ASC"
    End Sub

    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        Dim name = grid.Columns(e.ColumnIndex).Name
        If name = "standing" Then
            Theme.PaintBadgeCell(e)
        ElseIf name = "late_count" AndAlso AppSettings.TardyAlertCount > 0 AndAlso Db.ToInt(e.Value) >= AppSettings.TardyAlertCount Then
            e.CellStyle.ForeColor = Theme.Warning
            e.CellStyle.Font = Theme.FontBold
        ElseIf name = "percent_absent" AndAlso Db.ToDec(e.Value) >= AppSettings.Warning1Pct Then
            e.CellStyle.ForeColor = If(Db.ToDec(e.Value) >= AppSettings.DropPct, Theme.Danger, Theme.Orange)
            e.CellStyle.Font = Theme.FontBold
        End If
    End Sub

    Private Sub ShowHistory()
        If grid.CurrentRow Is Nothing Then Return
        Using f As New StudentHistoryDialog(Lookups.SelectedId(cboClass), Db.ToInt(grid.CurrentRow.Cells("student_id").Value))
            f.ShowDialog(FindForm())
        End Using
    End Sub

End Class
