Imports System.Windows.Forms

''' <summary>Classes per term: subject + section + teacher + schedule, with total class hours and enrolled count.</summary>
Public Class ClassesPage

    Private _loading As Boolean

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "Classes"
        End Get
    End Property

    Public Overrides Sub OnPageShown()
        Theme.StylePrimary(btnAdd, Icons.Add)
        Theme.StyleSecondary(btnEdit, Icons.Edit)
        Theme.StyleDanger(btnDelete, Icons.Delete)
        Theme.StyleCombo(cboTerm)
        Theme.StyleCombo(cboSection)
        Theme.StyleGrid(dgv)
        _loading = True
        Lookups.BindTerms(cboTerm)
        Lookups.BindSections(cboSection)
        _loading = False
        LoadData()
    End Sub

    Private Sub LoadData()
        Dim q = txtSearch.Text.Trim()
        Dim dt = Db.Query(
            "SELECT c.class_id, sub.subject_code, sub.description, sec.section_name, " &
            "       CONCAT(t.last_name, ', ', t.first_name) AS teacher_name, " &
            "       CONCAT(REPLACE(c.days, ',', ' · '), '   ', TRIM(TIME_FORMAT(c.start_time, '%l:%i')), '–', TRIM(TIME_FORMAT(c.end_time, '%l:%i %p'))) AS schedule, " &
            "       c.room, c.total_hours, " &
            "       (SELECT COUNT(*) FROM class_enrollments e WHERE e.class_id = c.class_id) AS enrolled " &
            "FROM classes c JOIN subjects sub ON sub.subject_id = c.subject_id JOIN sections sec ON sec.section_id = c.section_id " &
            "JOIN teachers t ON t.teacher_id = c.teacher_id " &
            "WHERE c.term_id = @term AND (@sec = 0 OR c.section_id = @sec) " &
            "  AND (@q = '' OR sub.subject_code LIKE @like OR sub.description LIKE @like OR t.last_name LIKE @like OR t.first_name LIKE @like) " &
            "ORDER BY sub.subject_code, sec.section_name",
            Db.P("@term", Lookups.SelectedId(cboTerm)), Db.P("@sec", Lookups.SelectedId(cboSection)),
            Db.P("@q", q), Db.P("@like", "%" & q & "%"))

        dgv.DataSource = dt
        dgv.Columns("class_id").Visible = False
        SetColumn("subject_code", "Code", 10)
        SetColumn("description", "Subject", 24)
        SetColumn("section_name", "Section", 11)
        SetColumn("teacher_name", "Teacher", 17)
        SetColumn("schedule", "Schedule", 20)
        SetColumn("room", "Room", 9)
        SetColumn("total_hours", "Hours", 7)
        SetColumn("enrolled", "Students", 8)
        dgv.Columns("total_hours").DefaultCellStyle.Format = "0.#"

        lblCount.Text = $"{dt.Rows.Count} class{If(dt.Rows.Count = 1, "", "es")}"
        lblEmpty.Visible = dt.Rows.Count = 0
        lblEmpty.Text = If(q = "" AndAlso Lookups.SelectedId(cboSection) = 0,
                           "No classes in this term yet. Click ""Add class"" to schedule one.",
                           "No classes match your search or section filter.")
        lblEmpty.BringToFront()
        UpdateButtons()
    End Sub

    Private Sub SetColumn(name As String, header As String, weight As Integer)
        dgv.Columns(name).HeaderText = header
        dgv.Columns(name).FillWeight = weight
    End Sub

    Private Function SelectedId() As Integer
        If dgv.CurrentRow Is Nothing Then Return 0
        Return Db.ToInt(dgv.CurrentRow.Cells("class_id").Value)
    End Function

    Private Sub UpdateButtons()
        Dim has = SelectedId() > 0
        btnEdit.Enabled = has
        btnDelete.Enabled = has
    End Sub

    ' ---------------- events ----------------
    Private Sub Filters_Changed(sender As Object, e As EventArgs) Handles cboTerm.SelectedIndexChanged, cboSection.SelectedIndexChanged
        If Not _loading Then LoadData()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        If Not _loading Then LoadData()
    End Sub

    Private Sub dgv_SelectionChanged(sender As Object, e As EventArgs) Handles dgv.SelectionChanged
        UpdateButtons()
    End Sub

    Private Sub dgv_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv.CellDoubleClick
        If e.RowIndex >= 0 Then EditSelected()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Using f As New ClassDialog(0, Lookups.SelectedId(cboTerm))
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Class added")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        EditSelected()
    End Sub

    Private Sub EditSelected()
        Dim id = SelectedId()
        If id = 0 Then Return
        Using f As New ClassDialog(id)
            If f.ShowDialog(FindForm()) = DialogResult.OK Then
                LoadData()
                ShowToast("Class updated")
            End If
        End Using
    End Sub

    ''' <summary>Sessions and enrollments would be deleted with the class (CASCADE), so block when attendance exists.</summary>
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim id = SelectedId()
        If id = 0 Then Return
        Dim label = $"{dgv.CurrentRow.Cells("subject_code").Value} · {dgv.CurrentRow.Cells("section_name").Value}"
        Try
            Dim sessions = Db.ToInt(Db.Scalar("SELECT COUNT(*) FROM attendance_sessions WHERE class_id = @id", Db.P("@id", id)))
            If sessions > 0 Then
                Notice.Info(Me, "Class has attendance", $"{label} already has {sessions} attendance session(s) and cannot be deleted.")
                Return
            End If
            Dim enrolled = Db.ToInt(dgv.CurrentRow.Cells("enrolled").Value)
            Dim extra = If(enrolled > 0, $" Its {enrolled} enrolled student(s) will be unenrolled.", "")
            If Not Confirm($"Delete class {label}?{extra} This cannot be undone.", "Delete class", "Delete", True) Then Return
            Db.Execute("DELETE FROM classes WHERE class_id = @id", Db.P("@id", id))
            Audit.Log("DELETE", "Class", id, label)
            LoadData()
            ShowToast("Class deleted")
        Catch ex As Exception
            ShowError(ex, "delete the class")
        End Try
    End Sub

End Class
