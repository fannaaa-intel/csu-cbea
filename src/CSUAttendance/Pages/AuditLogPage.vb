Imports System.Windows.Forms

''' <summary>Audit log (Admin): who did what and when. Filters: date range, action, search. Newest first, at most 500 rows.</summary>
Public Class AuditLogPage

    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Audit Log"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StyleCombo(cboAction)
        Theme.StyleGrid(dgv)
        _loading = True
        dtpFrom.Value = Date.Today.AddDays(-7)
        dtpTo.Value = Date.Today
        cboAction.Items.Add("All actions")
        For Each r As DataRow In Db.Query("SELECT DISTINCT action FROM audit_log ORDER BY action").Rows
            cboAction.Items.Add(Db.ToStr(r("action")))
        Next
        cboAction.SelectedIndex = 0
        _loading = False
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim action = If(cboAction.SelectedIndex <= 0, "", CStr(cboAction.SelectedItem))
        Dim dt = Db.Query(
            "SELECT a.created_at, COALESCE(u.username, '(system)') AS username, a.action, a.entity, a.entity_id, a.details " &
            "FROM audit_log a LEFT JOIN users u ON u.user_id = a.user_id " &
            "WHERE a.created_at >= @from AND a.created_at < @to AND (@act = '' OR a.action = @act) " &
            "  AND (@q = '' OR u.username LIKE @like OR a.entity LIKE @like OR a.details LIKE @like) " &
            "ORDER BY a.created_at DESC, a.log_id DESC LIMIT 500",
            Db.P("@from", dtpFrom.Value.Date), Db.P("@to", dtpTo.Value.Date.AddDays(1)), Db.P("@act", action),
            Db.P("@q", q), Db.P("@like", "%" & q & "%"))
        dgv.DataSource = dt
        Ui.Col(dgv, "created_at", "Date and time", 22, "MMM d, yyyy  h:mm tt")
        Ui.Col(dgv, "username", "User", 12)
        Ui.Col(dgv, "action", "Action", 11)
        Ui.Col(dgv, "entity", "Record", 11)
        Ui.Col(dgv, "entity_id", "ID", 5)
        Ui.Col(dgv, "details", "Details", 38)
        lblCount.Text = If(dt.Rows.Count >= 500, "latest 500 entries", Ui.Plural(dt.Rows.Count, "entry", "entries"))
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.BringToFront()
    End Sub

    Private Sub Filters_Changed(sender As Object, e As EventArgs) Handles dtpFrom.ValueChanged, dtpTo.ValueChanged, cboAction.SelectedIndexChanged, txtSearch.TextChanged
        If Not _loading Then LoadData()
    End Sub

End Class
