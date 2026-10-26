Imports System.IO
Imports System.Windows.Forms

''' <summary>
''' Asks why a student is Late, Absent or Excused (the reason is required) and lets the teacher attach proof.
''' For Excused it also asks what the student must do in lieu of attendance (CSU Student Manual s.II.B.4);
''' the absence counts while Pending and stops counting once marked Complied.
''' </summary>
Public Class ReasonDialog

    Private ReadOnly _studentName As String
    Private ReadOnly _status As String
    Private _attachment As String          ' stored file name, or the full path of a file picked in this dialog

    Public Sub New()
        Me.New("", "Absent", "", "", "")
    End Sub

    ''' <param name="attachment">Current attachment: stored file name, a not-yet-saved full path, or "".</param>
    Public Sub New(studentName As String, status As String, reason As String, requirement As String, attachment As String)
        InitializeComponent()
        _studentName = studentName
        _status = status
        _attachment = attachment
        txtReason.Text = reason
        txtReason.MaxLength = 255
        txtRequirement.Text = requirement
        txtRequirement.MaxLength = 255
    End Sub

    Public ReadOnly Property Reason As String
        Get
            Return txtReason.Text.Trim()
        End Get
    End Property

    Public ReadOnly Property Requirement As String
        Get
            Return txtRequirement.Text.Trim()
        End Get
    End Property

    ''' <summary>The attachment after editing: stored file name (unchanged), full path of a newly picked file, or "" (none).</summary>
    Public ReadOnly Property Attachment As String
        Get
            Return _attachment
        End Get
    End Property

    Private ReadOnly Property IsExcused As Boolean
        Get
            Return _status = "Excused"
        End Get
    End Property

    Private Sub ReasonDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Theme.StylePrimary(btnSave)
        Theme.StyleSecondary(btnCancel)
        Theme.StyleSecondary(btnAttach, Icons.Attach)
        Theme.StyleGhost(btnRemoveFile)
        lnkFile.Font = Theme.FontSmall
        lnkFile.LinkColor = Theme.Accent
        lnkFile.ActiveLinkColor = Theme.AccentDeep
        lnkFile.LinkBehavior = LinkBehavior.HoverUnderline

        btnSave.Text = $"Mark {_status.ToLowerInvariant()}"
        Theme.FitWidth(btnSave, 112)
        btnSave.Left = ClientSize.Width - Theme.Dpi(30) - btnSave.Width
        btnCancel.Left = btnSave.Left - Theme.Dpi(8) - btnCancel.Width

        lblTitle.Text = If(IsExcused, "Excused absence", $"Reason for being {_status.ToLowerInvariant()}")
        If IsExcused Then
            SetRail(Icons.CheckList, "Excused absence", "Faculty may require work in lieu of attendance (Student Manual s.II.B.4). The hours count until it is complied.")
            lblInfo.Text = $"{_studentName} will be marked Excused with a Pending requirement."
        Else
            SetRail(If(_status = "Late", Icons.Clock, Icons.WarningSign), _status,
                    "Every Late, Absent and Excused mark needs a reason. Attach the excuse letter or medical certificate if the student has one.")
            lblInfo.Text = $"Why is {_studentName} {_status.ToLowerInvariant()}?"
            HideRequirement()
        End If
        ShowAttachment()
    End Sub

    ''' <summary>Late / Absent have no requirement: hide that field and pull the buttons up.</summary>
    Private Sub HideRequirement()
        Dim shift = btnSave.Top - txtRequirement.Top
        lblRequirement.Visible = False
        txtRequirement.Visible = False
        btnSave.Top -= shift
        btnCancel.Top -= shift
        ClientSize = New Drawing.Size(ClientSize.Width, ClientSize.Height - shift)
    End Sub

    Private Sub ShowAttachment()
        Dim has = _attachment <> ""
        lnkFile.Text = If(has, Path.GetFileName(_attachment), "No file attached")
        lnkFile.Enabled = has
        btnRemoveFile.Visible = has
        btnAttach.Text = If(has, "Replace", "Attach file")
    End Sub

    Private Sub btnAttach_Click(sender As Object, e As EventArgs) Handles btnAttach.Click
        Using pick As New OpenFileDialog With {.Title = "Attach proof", .Filter = Attachments.FileFilter}
            If pick.ShowDialog(Me) <> DialogResult.OK Then Return
            Dim problem = Attachments.Problem(pick.FileName)
            If problem <> "" Then
                Notice.Warn(Me, "Cannot attach this file", problem)
                Return
            End If
            _attachment = pick.FileName
        End Using
        ShowAttachment()
    End Sub

    Private Sub btnRemoveFile_Click(sender As Object, e As EventArgs) Handles btnRemoveFile.Click
        _attachment = ""
        ShowAttachment()
    End Sub

    Private Sub lnkFile_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkFile.LinkClicked
        Attachments.Open(Me, _attachment)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        errProvider.Clear()
        If Reason = "" Then
            errProvider.SetError(txtReason, "A reason is required for Late, Absent and Excused.")
            txtReason.Focus()
            Return
        End If
        If IsExcused AndAlso Requirement = "" Then
            errProvider.SetError(txtRequirement, "Describe what the student must submit or do.")
            txtRequirement.Focus()
            Return
        End If
        DialogResult = DialogResult.OK
    End Sub

End Class
