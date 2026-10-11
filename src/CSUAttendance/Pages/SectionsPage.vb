Imports System.Windows.Forms

''' <summary>Sections (e.g. BSAIS 3B) with their student and class counts.</summary>
Public Class SectionsPage

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Sections"
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
            "SELECT x.section_id, x.section_name, x.program, x.year_level, " &
            "       (SELECT COUNT(*) FROM students s WHERE s.section_id = x.section_id) AS student_count, " &
            "       (SELECT COUNT(*) FROM classes c WHERE c.section_id = x.section_id) AS class_count " &
            "FROM sections x " &
            "WHERE @q = '' OR x.section_name LIKE @like OR x.program LIKE @like " &
            "ORDER BY x.program, x.year_level, x.section_name",
            Db.P("@q", q), Db.P("@like", "%" & q & "%"))

        dgv.DataSource = dt
        dgv.Columns("section_id").Visible = False
        dgv.Columns("section_name").HeaderText = "Section"
        dgv.Columns("section_name").FillWeight = 28
        dgv.Columns("program").HeaderText = "Program"
        dgv.Columns("program").FillWeight = 22
        dgv.Columns("year_level").HeaderText = "Year level"
        dgv.Columns("year_level").FillWeight = 16
        dgv.Columns("student_count").HeaderText = "Students"
        dgv.Columns("student_count").FillWeight = 17
        dgv.Columns("class_count").HeaderText = "Classes"
        dgv.Columns("class_count").FillWeight = 17

        lblCount.Text = $"{dt.Rows.Count} section{If(dt.Rows.Count = 1, "", "s")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = If(q = "", "No sections yet. Click ""Add section"" to create one.", "No sections match your search.")
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("section_id").Value)
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
        Using f As New SectionDialog(0)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Section added")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        EditSelected()
    End Sub

    Private Sub EditSelected()
        Dim id = SelectedId()
        If id = 0 Then Return
        Using f As New SectionDialog(id)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Section updated")
            End If
        End Using
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim name = Db.ToStr(dgv.CurrentRow.Cells("section_name").Value)
        ' students.section_id would silently become NULL, so block the delete ourselves.
        If Db.ToInt(dgv.CurrentRow.Cells("student_count").Value) > 0 Then
            MessageBox.Show(FindForm(), $"{name} still has students. Move them to another section first.",
                            "Section in use", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If Not Confirm($"Delete section {name}? This cannot be undone.", "Delete section") Then Return
        Try
            Db.Execute("DELETE FROM sections WHERE section_id = @id", Db.P("@id", id))
            Audit.Log("DELETE", "Section", id, name)
            LoadData()
            ShowToast("Section deleted")
        Catch ex As Exception When Db.IsInUse(ex)
            MessageBox.Show(FindForm(), $"{name} is used by one or more classes and cannot be deleted.",
                            "Section in use", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            ShowError(ex, "delete the section")
        End Try
    End Sub

End Class
