Imports System.Windows.Forms

''' <summary>Students and their login accounts (username/password = student no., created automatically).</summary>
Public Class StudentsPage

    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Students"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StylePrimary(btnAdd, Icons.Add)
        Theme.StyleSecondary(btnEdit, Icons.Edit)
        Theme.StyleDanger(btnDelete, Icons.Delete)
        Theme.StyleCombo(cboSection)
        Theme.StyleGrid(dgv)
        LoadSections()
        LoadData()
    End Sub

    ''' <summary>Fills the section filter with "All sections" + every section.</summary>
    Private Sub LoadSections()
        _loading = True
        Dim dt = Db.Query("SELECT section_id, section_name FROM sections ORDER BY section_name")
        Dim all = dt.NewRow()
        all("section_id") = 0
        all("section_name") = "All sections"
        dt.Rows.InsertAt(all, 0)
        cboSection.DisplayMember = "section_name"
        cboSection.ValueMember = "section_id"
        cboSection.DataSource = dt
        cboSection.SelectedIndex = 0
        _loading = False
    End Sub

    Private Function SelectedSectionId() As Integer
        Return If(cboSection.SelectedValue Is Nothing, 0, Db.ToInt(cboSection.SelectedValue))
    End Function

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim sectionId = SelectedSectionId()
        Dim dt = Db.Query(
            "SELECT s.student_id, s.student_no, " & TextUtil.SortableNameSql("s") & " AS student_name, " &
            "       sec.section_name, s.sex, s.status " &
            "FROM students s LEFT JOIN sections sec ON sec.section_id = s.section_id " &
            "WHERE (@q = '' OR s.student_no LIKE @like OR s.first_name LIKE @like OR s.last_name LIKE @like " &
            "       OR CONCAT(s.first_name, ' ', s.last_name) LIKE @like) " &
            "  AND (@sec = 0 OR s.section_id = @sec) " &
            "ORDER BY s.last_name, s.first_name",
            Db.P("@q", q), Db.P("@like", "%" & q & "%"), Db.P("@sec", sectionId))

        dgv.DataSource = dt
        dgv.Columns("student_id").Visible = False
        dgv.Columns("student_no").HeaderText = "Student no."
        dgv.Columns("student_no").FillWeight = 16
        dgv.Columns("student_name").HeaderText = "Name"
        dgv.Columns("student_name").FillWeight = 38
        dgv.Columns("section_name").HeaderText = "Section"
        dgv.Columns("section_name").FillWeight = 18
        dgv.Columns("sex").HeaderText = "Sex"
        dgv.Columns("sex").FillWeight = 12
        dgv.Columns("status").HeaderText = "Status"
        dgv.Columns("status").FillWeight = 16

        lblCount.Text = $"{dt.Rows.Count} student{If(dt.Rows.Count = 1, "", "s")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = If(q = "" AndAlso sectionId = 0,
                           "No students yet. Click ""Add student"" to create one.",
                           "No students match your search or section filter.")
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("student_id").Value)
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

    Private Sub cboSection_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSection.SelectedIndexChanged
        If Not _loading Then LoadData()
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
        Using f As New StudentDialog(0, SelectedSectionId())
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Student added")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        EditSelected()
    End Sub

    Private Sub EditSelected()
        Dim id = SelectedId()
        If id = 0 Then Return
        Using f As New StudentDialog(id)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Student updated")
            End If
        End Using
    End Sub

    ''' <summary>Only students without attendance can be deleted; the login account is deleted too.</summary>
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim studentNo = Db.ToStr(dgv.CurrentRow.Cells("student_no").Value)
        Dim name = Db.ToStr(dgv.CurrentRow.Cells("student_name").Value)
        Try
            Dim records = Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM attendance_records WHERE student_id = @id", Db.P("@id", id)))
            If records > 0 Then
                MessageBox.Show(FindForm(), $"{name} has {records} attendance record(s) and cannot be deleted." & Environment.NewLine &
                                "Edit the student and set Status to Inactive instead.",
                                "Student has attendance", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            If Not Confirm($"Delete student {studentNo} {name} and their login account? This cannot be undone.", "Delete student") Then Return

            Db.InTransaction(Sub(cn, tx)
                                 Dim userId = Db.ToInt(Db.Scalar(cn, tx, "SELECT user_id FROM students WHERE student_id = @id", Db.P("@id", id)))
                                 Db.Execute(cn, tx, "DELETE FROM students WHERE student_id = @id", Db.P("@id", id))
                                 If userId > 0 Then Db.Execute(cn, tx, "DELETE FROM users WHERE user_id = @u", Db.P("@u", userId))
                             End Sub)
            Audit.Log("DELETE", "Student", id, $"{studentNo} {name}")
            LoadData()
            ShowToast("Student deleted")
        Catch ex As Exception
            ShowError(ex, "delete the student")
        End Try
    End Sub

End Class
