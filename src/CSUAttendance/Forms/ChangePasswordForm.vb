Imports System.Windows.Forms

''' <summary>
''' Change own password. Forced mode (first login / after admin reset) hides Cancel.
''' Open from anywhere:  Using f As New ChangePasswordForm() : f.ShowDialog(Me) : End Using
''' </summary>
Public Class ChangePasswordForm

    Private ReadOnly _forced As Boolean

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub New(forced As Boolean)
        InitializeComponent()
        _forced = forced
        ShowCloseButton = Not forced
    End Sub

    Private Sub ChangePasswordForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        SetRail(Icons.Key, "Your password", "Passwords are stored as salted PBKDF2 hashes - nobody, not even the administrator, can read them.")
        If _forced Then
            lblTitle.Text = "Set a new password"
            lblSub.Text = "For your security, please replace your temporary password. Use at least 8 characters with letters and numbers."
            btnCancel.Text = "Not now"
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        lblError.Text = ""
        If txtNew.Text <> txtConfirm.Text Then
            lblError.Text = "New passwords do not match."
            txtConfirm.SelectAllText()
            Return
        End If
        Dim message As String = ""
        Try
            If AuthService.ChangePassword(Session.UserId, txtCurrent.Text, txtNew.Text, message) Then
                DialogResult = DialogResult.OK
            Else
                lblError.Text = message
            End If
        Catch ex As Exception
            lblError.Text = "Could not save: " & ex.Message
        End Try
    End Sub

End Class
