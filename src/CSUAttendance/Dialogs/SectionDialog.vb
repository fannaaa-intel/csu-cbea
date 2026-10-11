Imports System.Text.RegularExpressions
Imports System.Windows.Forms

''' <summary>Add/edit a section. id = 0 → add. Typing "BSAIS 3B" pre-fills program BSAIS and year 3.</summary>
Public Class SectionDialog

    Private ReadOnly _id As Integer

    Public Sub New()
        Me.New(0)
    End Sub

    Public Sub New(sectionId As Integer)
        InitializeComponent()
        _id = sectionId
    End Sub

    Private Sub SectionDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        If _id > 0 Then
            lblTitle.Text = "Edit section"
            Dim dt = Db.Query("SELECT section_name, program, year_level FROM sections WHERE section_id = @id", Db.P("@id", _id))
            If dt.Rows.Count = 0 Then
                DialogResult = DialogResult.Cancel
                Return
            End If
            Dim r = dt.Rows(0)
            txtName.Text = Db.ToStr(r("section_name"))
            txtProgram.Text = Db.ToStr(r("program"))
            numYear.Value = Math.Min(numYear.Maximum, Math.Max(numYear.Minimum, Db.ToInt(r("year_level"))))
        End If
    End Sub

    ''' <summary>When the program is still empty, guess it from a name like "BSAIS 3B".</summary>
    Private Sub txtName_Leave(sender As Object, e As EventArgs) Handles txtName.Leave
        If txtProgram.Text.Trim() <> "" Then Return
        Dim m = Regex.Match(txtName.Text.Trim(), "^([A-Za-z]+)\s*(\d)")
        If Not m.Success Then Return
        txtProgram.Text = m.Groups(1).Value.ToUpper()
        numYear.Value = Math.Min(numYear.Maximum, Math.Max(numYear.Minimum, CInt(m.Groups(2).Value)))
    End Sub

    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        If txtName.Text.Trim() = "" Then
            errProvider.SetError(txtName, "Section name is required.")
            ok = False
        End If
        If txtProgram.Text.Trim() = "" Then
            errProvider.SetError(txtProgram, "Program is required.")
            ok = False
        End If
        Return ok
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Dim name = txtName.Text.Trim().ToUpper()
        Dim program = txtProgram.Text.Trim().ToUpper()
        Dim year = CInt(numYear.Value)
        Try
            If _id = 0 Then
                Dim newId = Db.Insert("INSERT INTO sections (section_name, program, year_level) VALUES (@n, @p, @y)",
                                      Db.P("@n", name), Db.P("@p", program), Db.P("@y", year))
                Audit.Log("CREATE", "Section", newId, name)
            Else
                Db.Execute("UPDATE sections SET section_name = @n, program = @p, year_level = @y WHERE section_id = @id",
                           Db.P("@n", name), Db.P("@p", program), Db.P("@y", year), Db.P("@id", _id))
                Audit.Log("UPDATE", "Section", _id, name)
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(txtName, $"Section {name} already exists.")
        Catch ex As Exception
            MessageBox.Show(Me, "Could not save the section." & Environment.NewLine & Environment.NewLine & ex.Message,
                            "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class
