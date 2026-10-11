Imports System.Windows.Forms

''' <summary>
''' Asks what the student must do in lieu of attendance (CSU Student Manual s.II.B.4).
''' The absence counts while Pending and stops counting once marked Complied.
''' </summary>
Public Class ExcuseDialog

    Private ReadOnly _studentName As String

    Public Sub New()
        Me.New("", "")
    End Sub

    Public Sub New(studentName As String, currentRequirement As String)
        InitializeComponent()
        _studentName = studentName
        txtRequirement.Text = currentRequirement
        txtRequirement.MaxLength = 255
    End Sub

    ''' <summary>The requirement the teacher typed.</summary>
    Public ReadOnly Property Requirement As String
        Get
            Return txtRequirement.Text.Trim()
        End Get
    End Property

    Private Sub ExcuseDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        btnSave.Text = "Mark excused"
        Theme.FitWidth(btnSave, 112)
        btnSave.Left = ClientSize.Width - Theme.Dpi(30) - btnSave.Width
        btnCancel.Left = btnSave.Left - Theme.Dpi(8) - btnCancel.Width
        SetRail(Icons.CheckList, "Excused absence", "Faculty may require work in lieu of attendance (Student Manual s.II.B.4). The hours count until it is complied.")
        lblInfo.Text = $"{_studentName} will be marked Excused with a Pending requirement."
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        errProvider.Clear()
        If Requirement = "" Then
            errProvider.SetError(txtRequirement, "Describe what the student must submit or do.")
            Return
        End If
        DialogResult = DialogResult.OK
    End Sub

End Class
