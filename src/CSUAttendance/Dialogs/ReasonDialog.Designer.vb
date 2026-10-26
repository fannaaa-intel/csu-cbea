<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReasonDialog
    Inherits Global.CSUAttendance.SheetForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.lblReason = New System.Windows.Forms.Label()
        Me.txtReason = New Global.CSUAttendance.TextField()
        Me.lblAttachment = New System.Windows.Forms.Label()
        Me.btnAttach = New System.Windows.Forms.Button()
        Me.lnkFile = New System.Windows.Forms.LinkLabel()
        Me.btnRemoveFile = New System.Windows.Forms.Button()
        Me.lblRequirement = New System.Windows.Forms.Label()
        Me.txtRequirement = New Global.CSUAttendance.TextField()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.errProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 15.0!)
        Me.lblTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblTitle.Location = New System.Drawing.Point(238, 24)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(160, 28)
        Me.lblTitle.TabIndex = 100
        Me.lblTitle.Text = "Reason"
        '
        'lblInfo
        '
        Me.lblInfo.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblInfo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblInfo.Location = New System.Drawing.Point(236, 64)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(472, 40)
        Me.lblInfo.TabIndex = 101
        Me.lblInfo.Text = "-"
        '
        'lblReason
        '
        Me.lblReason.AutoSize = True
        Me.lblReason.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblReason.Location = New System.Drawing.Point(236, 110)
        Me.lblReason.Name = "lblReason"
        Me.lblReason.Size = New System.Drawing.Size(60, 15)
        Me.lblReason.TabIndex = 102
        Me.lblReason.Text = "Reason (required)"
        '
        'txtReason
        '
        Me.txtReason.Location = New System.Drawing.Point(238, 130)
        Me.txtReason.Name = "txtReason"
        Me.txtReason.Placeholder = "e.g. Sick (fever), heavy traffic, family emergency"
        Me.txtReason.Size = New System.Drawing.Size(472, 36)
        Me.txtReason.TabIndex = 0
        '
        'lblAttachment
        '
        Me.lblAttachment.AutoSize = True
        Me.lblAttachment.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblAttachment.Location = New System.Drawing.Point(236, 182)
        Me.lblAttachment.Name = "lblAttachment"
        Me.lblAttachment.Size = New System.Drawing.Size(120, 15)
        Me.lblAttachment.TabIndex = 103
        Me.lblAttachment.Text = "Attachment (optional) - photo or PDF of the excuse letter / medical certificate"
        '
        'btnAttach
        '
        Me.btnAttach.Location = New System.Drawing.Point(238, 202)
        Me.btnAttach.Name = "btnAttach"
        Me.btnAttach.Size = New System.Drawing.Size(124, 36)
        Me.btnAttach.TabIndex = 1
        Me.btnAttach.Text = "Attach file"
        Me.btnAttach.UseVisualStyleBackColor = True
        '
        'lnkFile
        '
        Me.lnkFile.AutoEllipsis = True
        Me.lnkFile.Location = New System.Drawing.Point(372, 210)
        Me.lnkFile.Name = "lnkFile"
        Me.lnkFile.Size = New System.Drawing.Size(250, 20)
        Me.lnkFile.TabIndex = 2
        Me.lnkFile.TabStop = True
        Me.lnkFile.Text = "No file attached"
        '
        'btnRemoveFile
        '
        Me.btnRemoveFile.Location = New System.Drawing.Point(626, 202)
        Me.btnRemoveFile.Name = "btnRemoveFile"
        Me.btnRemoveFile.Size = New System.Drawing.Size(84, 36)
        Me.btnRemoveFile.TabIndex = 3
        Me.btnRemoveFile.Text = "Remove"
        Me.btnRemoveFile.UseVisualStyleBackColor = True
        '
        'lblRequirement
        '
        Me.lblRequirement.AutoSize = True
        Me.lblRequirement.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblRequirement.Location = New System.Drawing.Point(236, 254)
        Me.lblRequirement.Name = "lblRequirement"
        Me.lblRequirement.Size = New System.Drawing.Size(80, 15)
        Me.lblRequirement.TabIndex = 104
        Me.lblRequirement.Text = "Requirement in lieu of attendance (required)"
        '
        'txtRequirement
        '
        Me.txtRequirement.Location = New System.Drawing.Point(238, 274)
        Me.txtRequirement.Name = "txtRequirement"
        Me.txtRequirement.Placeholder = "e.g. Submit a one-page reflection on Chapter 3"
        Me.txtRequirement.Size = New System.Drawing.Size(472, 36)
        Me.txtRequirement.TabIndex = 4
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(598, 346)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(112, 36)
        Me.btnSave.TabIndex = 5
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(490, 346)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 36)
        Me.btnCancel.TabIndex = 6
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'errProvider
        '
        Me.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.errProvider.ContainerControl = Me
        '
        'ReasonDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(740, 410)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.txtRequirement)
        Me.Controls.Add(Me.lblRequirement)
        Me.Controls.Add(Me.btnRemoveFile)
        Me.Controls.Add(Me.lnkFile)
        Me.Controls.Add(Me.btnAttach)
        Me.Controls.Add(Me.lblAttachment)
        Me.Controls.Add(Me.txtReason)
        Me.Controls.Add(Me.lblReason)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ReasonDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Reason"
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents lblReason As System.Windows.Forms.Label
    Friend WithEvents txtReason As Global.CSUAttendance.TextField
    Friend WithEvents lblAttachment As System.Windows.Forms.Label
    Friend WithEvents btnAttach As System.Windows.Forms.Button
    Friend WithEvents lnkFile As System.Windows.Forms.LinkLabel
    Friend WithEvents btnRemoveFile As System.Windows.Forms.Button
    Friend WithEvents lblRequirement As System.Windows.Forms.Label
    Friend WithEvents txtRequirement As Global.CSUAttendance.TextField
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
