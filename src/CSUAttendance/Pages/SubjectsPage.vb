Imports System.Windows.Forms

''' <summary>
''' REFERENCE CRUD PAGE — copy this pattern for Students, Teachers, Sections, Terms, Classes, Users.
''' Toolbar (search left, actions right) → card with a styled grid → modal dialog for add/edit.
''' </summary>
Public Class SubjectsPage

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Subjects"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StylePrimary(btnAdd, Icons.Add)
        Theme.StyleSecondary(btnEdit, Icons.Edit)
        Theme.StyleDanger(btnDelete, Icons.Delete)
        Theme.StyleGrid(dgv)
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim dt = Db.Query(
            "SELECT s.subject_id, s.subject_code, s.description, s.units, " &
            "       (SELECT COUNT(*) FROM classes c WHERE c.subject_id = s.subject_id) AS class_count " &
            "FROM subjects s " &
            "WHERE @q = '' OR s.subject_code LIKE @like OR s.description LIKE @like " &
            "ORDER BY s.subject_code",
            Db.P("@q", q), Db.P("@like", "%" & q & "%"))

        dgv.DataSource = dt
        dgv.Columns("subject_id").Visible = False
        dgv.Columns("subject_code").HeaderText = "Code"
        dgv.Columns("subject_code").FillWeight = 20
        dgv.Columns("description").HeaderText = "Description"
        dgv.Columns("description").FillWeight = 55
        dgv.Columns("units").HeaderText = "Units"
        dgv.Columns("units").FillWeight = 10
        dgv.Columns("class_count").HeaderText = "Classes"
        dgv.Columns("class_count").FillWeight = 15

        lblCount.Text = $"{dt.Rows.Count} subject{If(dt.Rows.Count = 1, "", "s")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = If(q = "", "No subjects yet. Click ""Add subject"" to create one.", "No subjects match your search.")
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("subject_id").Value)
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

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using f As New SubjectDialog(0)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Subject added")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        EditSelected()
    End Sub

    Private Sub EditSelected()
        Dim id = SelectedId()
        If id = 0 Then Return
        Using f As New SubjectDialog(id)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Subject updated")
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim code = Db.ToStr(dgv.CurrentRow.Cells("subject_code").Value)
        If Not Confirm($"Delete subject {code}? This cannot be undone.", "Delete subject") Then Return
        Try
            Db.Execute("DELETE FROM subjects WHERE subject_id = @id", Db.P("@id", id))
            Audit.Log("DELETE", "Subject", id, code)
            LoadData()
            ShowToast("Subject deleted")
        Catch ex As Exception When Db.IsInUse(ex)
            MessageBox.Show(FindForm(), $"{code} is used by one or more classes and cannot be deleted.",
                            "Subject in use", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            ShowError(ex, "delete the subject")
        End Try
    End Sub

End Class
