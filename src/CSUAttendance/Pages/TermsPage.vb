Imports System.Windows.Forms

''' <summary>School terms (school year + semester). Only one term is active; pages default to it.</summary>
Public Class TermsPage

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "School Terms"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StylePrimary(btnAdd, Icons.Add)
        Theme.StyleSecondary(btnEdit, Icons.Edit)
        Theme.StyleDanger(btnDelete, Icons.Delete)
        Theme.StyleSecondary(btnSetActive, Icons.Check)
        Theme.StyleGrid(dgv)
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim dt = Db.Query(
            "SELECT t.term_id, t.school_year, t.semester, t.start_date, t.end_date, " &
            "       (SELECT COUNT(*) FROM classes c WHERE c.term_id = t.term_id) AS class_count, " &
            "       IF(t.is_active = 1, 'Active', NULL) AS status " &
            "FROM school_terms t " &
            "WHERE @q = '' OR t.school_year LIKE @like OR t.semester LIKE @like " &
            "ORDER BY t.start_date DESC",
            Db.P("@q", q), Db.P("@like", "%" & q & "%"))

        dgv.DataSource = dt
        dgv.Columns("term_id").Visible = False
        dgv.Columns("school_year").HeaderText = "School year"
        dgv.Columns("school_year").FillWeight = 18
        dgv.Columns("semester").HeaderText = "Semester"
        dgv.Columns("semester").FillWeight = 20
        dgv.Columns("start_date").HeaderText = "Start"
        dgv.Columns("start_date").FillWeight = 17
        dgv.Columns("start_date").DefaultCellStyle.Format = "MMM d, yyyy"
        dgv.Columns("end_date").HeaderText = "End"
        dgv.Columns("end_date").FillWeight = 17
        dgv.Columns("end_date").DefaultCellStyle.Format = "MMM d, yyyy"
        dgv.Columns("class_count").HeaderText = "Classes"
        dgv.Columns("class_count").FillWeight = 12
        dgv.Columns("status").HeaderText = "Status"
        dgv.Columns("status").FillWeight = 16

        lblCount.Text = $"{dt.Rows.Count} term{If(dt.Rows.Count = 1, "", "s")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = If(q = "", "No school terms yet. Click ""Add term"" to create one.", "No terms match your search.")
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("term_id").Value)
    End Function

    Private Function SelectedIsActive() As Boolean
        Return dgv.CurrentRow IsNot Nothing AndAlso Db.ToStr(dgv.CurrentRow.Cells("status").Value) = "Active"
    End Function

    Private Function SelectedLabel() As String
        Return $"{Db.ToStr(dgv.CurrentRow.Cells("semester").Value)} {Db.ToStr(dgv.CurrentRow.Cells("school_year").Value)}"
    End Function

    Private Sub UpdateButtons()
        Dim has = SelectedId() > 0
        btnEdit.Enabled = has
        btnDelete.Enabled = has
        btnSetActive.Enabled = has AndAlso Not SelectedIsActive()
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
        If e.ColumnIndex >= 0 AndAlso dgv.Columns(e.ColumnIndex).Name = "status" Then Theme.PaintBadgeCell(e)
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using f As New TermDialog(0)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Term added")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        EditSelected()
    End Sub

    Private Sub EditSelected()
        Dim id = SelectedId()
        If id = 0 Then Return
        Using f As New TermDialog(id)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Term updated")
            End If
        End Using
    End Sub

    ''' <summary>Makes the selected term the only active one (both updates in one transaction).</summary>
    Private Sub btnSetActive_Click(sender As Object, e As EventArgs) Handles btnSetActive.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim label = SelectedLabel()
        If Not Confirm($"Make {label} the active term? Pages will show this term by default.", "Set active term") Then Return
        Try
            Db.InTransaction(Sub(cn, tx)
                                 Db.Execute(cn, tx, "UPDATE school_terms SET is_active = 0 WHERE term_id <> @id", Db.P("@id", id))
                                 Db.Execute(cn, tx, "UPDATE school_terms SET is_active = 1 WHERE term_id = @id", Db.P("@id", id))
                             End Sub)
            Audit.Log("SET_ACTIVE", "Term", id, label)
            LoadData()
            ShowToast($"{label} is now the active term")
        Catch ex As Exception
            ShowError(ex, "set the active term")
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim label = SelectedLabel()
        If SelectedIsActive() Then
            Notice.Info(Me, "Active term", $"{label} is the active term. Set another term as active before deleting it.")
            Return
        End If
        If Not Confirm($"Delete term {label}? This cannot be undone.", "Delete term", "Delete", True) Then Return
        Try
            Db.Execute("DELETE FROM school_terms WHERE term_id = @id", Db.P("@id", id))
            Audit.Log("DELETE", "Term", id, label)
            LoadData()
            ShowToast("Term deleted")
        Catch ex As Exception When Db.IsInUse(ex)
            Notice.Info(Me, "Term in use", $"{label} has classes and cannot be deleted.")
        Catch ex As Exception
            ShowError(ex, "delete the term")
        End Try
    End Sub

End Class
