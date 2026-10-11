Imports System.Windows.Forms

''' <summary>
''' REFERENCE ADD/EDIT DIALOG — copy this pattern for other entities.
''' id = 0 → add mode, id > 0 → edit mode. Returns DialogResult.OK after saving.
''' </summary>
Public Class SubjectDialog

    Private ReadOnly _id As Integer

    Public Sub New()
        Me.New(0)
    End Sub

    Public Sub New(subjectId As Integer)
        InitializeComponent()
        _id = subjectId
    End Sub

    Private Sub SubjectDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        SetRail(Icons.Subjects, "Subjects", "A subject is a course in the curriculum, like AIS 311. Classes are created from subjects.")
        Theme.StyleSecondary(btnCancel)
        If _id > 0 Then
            lblTitle.Text = "Edit subject"
            Dim dt = Db.Query("SELECT subject_code, description, units FROM subjects WHERE subject_id = @id", Db.P("@id", _id))
            If dt.Rows.Count = 0 Then
                DialogResult = DialogResult.Cancel
                Return
            End If
            Dim r = dt.Rows(0)
            txtCode.Text = Db.ToStr(r("subject_code"))
            txtDescription.Text = Db.ToStr(r("description"))
            numUnits.Value = Math.Min(numUnits.Maximum, Math.Max(numUnits.Minimum, Db.ToDec(r("units"))))
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        errProvider.Clear()
        Dim ok = True
        If txtCode.Text.Trim() = "" Then
            errProvider.SetError(txtCode, "Subject code is required.")
            ok = False
        End If
        If txtDescription.Text.Trim() = "" Then
            errProvider.SetError(txtDescription, "Description is required.")
            ok = False
        End If
        Return ok
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not ValidateInputs() Then Return
        Dim code = txtCode.Text.Trim().ToUpper()
        Try
            If _id = 0 Then
                Dim newId = Db.Insert("INSERT INTO subjects (subject_code, description, units) VALUES (@c, @d, @u)",
                                      Db.P("@c", code), Db.P("@d", txtDescription.Text.Trim()), Db.P("@u", numUnits.Value))
                Audit.Log("CREATE", "Subject", newId, code)
            Else
                Db.Execute("UPDATE subjects SET subject_code = @c, description = @d, units = @u WHERE subject_id = @id",
                           Db.P("@c", code), Db.P("@d", txtDescription.Text.Trim()), Db.P("@u", numUnits.Value), Db.P("@id", _id))
                Audit.Log("UPDATE", "Subject", _id, code)
            End If
            DialogResult = DialogResult.OK
        Catch ex As Exception When Db.IsDuplicate(ex)
            errProvider.SetError(txtCode, $"Subject code {code} already exists.")
        Catch ex As Exception
            Notice.Fail(Me, "Could not save the subject", "Please check your connection and the details, then try again.", ex.Message)
        End Try
    End Sub

End Class
