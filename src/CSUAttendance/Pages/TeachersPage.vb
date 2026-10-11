Imports System.Windows.Forms

''' <summary>Teachers and their login accounts (created automatically when a teacher is added).</summary>
Public Class TeachersPage

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Teachers"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StylePrimary(btnAdd, Icons.Add)
        Theme.StyleSecondary(btnEdit, Icons.Edit)
        Theme.StyleDanger(btnDelete, Icons.Delete)
        Theme.StyleGrid(dgv)
        If TypeOf NavigationArg Is String Then txtSearch.Text = CStr(NavigationArg)   ' opened from User Accounts
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim dt = Db.Query(
            "SELECT t.teacher_id, t.user_id, t.employee_no, " & TextUtil.SortableNameSql("t") & " AS teacher_name, " &
            "       t.department, u.username, " &
            "       (SELECT COUNT(*) FROM classes c WHERE c.teacher_id = t.teacher_id) AS class_count, " &
            "       IF(u.is_active = 1, 'Active', 'Inactive') AS account_status " &
            "FROM teachers t JOIN users u ON u.user_id = t.user_id " &
            "WHERE @q = '' OR t.employee_no LIKE @like OR t.first_name LIKE @like OR t.last_name LIKE @like " &
            "   OR CONCAT(t.first_name, ' ', t.last_name) LIKE @like OR u.username LIKE @like " &
            "ORDER BY t.last_name, t.first_name",
            Db.P("@q", q), Db.P("@like", "%" & q & "%"))

        dgv.DataSource = dt
        dgv.Columns("teacher_id").Visible = False
        dgv.Columns("user_id").Visible = False
        dgv.Columns("employee_no").HeaderText = "Employee no."
        dgv.Columns("employee_no").FillWeight = 14
        dgv.Columns("teacher_name").HeaderText = "Name"
        dgv.Columns("teacher_name").FillWeight = 26
        dgv.Columns("department").HeaderText = "Department"
        dgv.Columns("department").FillWeight = 26
        dgv.Columns("username").HeaderText = "Username"
        dgv.Columns("username").FillWeight = 14
        dgv.Columns("class_count").HeaderText = "Classes"
        dgv.Columns("class_count").FillWeight = 8
        dgv.Columns("account_status").HeaderText = "Account"
        dgv.Columns("account_status").FillWeight = 12

        lblCount.Text = $"{dt.Rows.Count} teacher{If(dt.Rows.Count = 1, "", "s")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = If(q = "", "No teachers yet. Click ""Add teacher"" to create one.", "No teachers match your search.")
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("teacher_id").Value)
    End Function

    Private Sub UpdateButtons()
        Dim has = SelectedId() > 0
        btnEdit.Enabled = has
        btnDelete.Enabled = has
    End Sub

    ' ---------------- events ----------------
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        LoadData()
    End Sub

    Private Sub dgv_SelectionChanged(sender As Object, e As EventArgs) Handles dgv.SelectionChanged
        UpdateButtons()
    End Sub

    Private Sub dgv_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv.CellDoubleClick
        If e.RowIndex >= 0 Then EditSelected()
    End Sub

    Private Sub dgv_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles dgv.CellPainting
        If e.ColumnIndex >= 0 AndAlso dgv.Columns(e.ColumnIndex).Name = "account_status" Then Theme.PaintBadgeCell(e)
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using f As New TeacherDialog(0)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Teacher added")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        EditSelected()
    End Sub

    Private Sub EditSelected()
        Dim id = SelectedId()
        If id = 0 Then Return
        Using f As New TeacherDialog(id)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Teacher updated")
            End If
        End Using
    End Sub

    ''' <summary>Deletes the login account; the teacher row is removed with it (ON DELETE CASCADE).</summary>
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim name = Db.ToStr(dgv.CurrentRow.Cells("teacher_name").Value)
        Dim userId = Db.ToInt(dgv.CurrentRow.Cells("user_id").Value)
        If Not Confirm($"Delete teacher {name} and their login account? This cannot be undone.", "Delete teacher", "Delete", True) Then Return
        Try
            Db.Execute("DELETE FROM users WHERE user_id = @u", Db.P("@u", userId))
            Audit.Log("DELETE", "Teacher", id, name)
            LoadData()
            ShowToast("Teacher deleted")
        Catch ex As Exception When Db.IsInUse(ex)
            Notice.Info(Me, "Teacher in use", $"{name} is assigned to one or more classes and cannot be deleted.")
        Catch ex As Exception
            ShowError(ex, "delete the teacher")
        End Try
    End Sub

End Class
