<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ChangePasswordForm
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
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblSub = New System.Windows.Forms.Label()
        Me.lblCurrent = New System.Windows.Forms.Label()
        Me.txtCurrent = New Global.CSUAttendance.TextField()
        Me.lblNew = New System.Windows.Forms.Label()
        Me.txtNew = New Global.CSUAttendance.TextField()
        Me.lblConfirm = New System.Windows.Forms.Label()
        Me.txtConfirm = New Global.CSUAttendance.TextField()
        Me.lblError = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 15.0!)
        Me.lblTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblTitle.Location = New System.Drawing.Point(238, 24)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(168, 28)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Change password"
        '
        'lblSub
        '
        Me.lblSub.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblSub.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblSub.Location = New System.Drawing.Point(240, 56)
        Me.lblSub.Name = "lblSub"
        Me.lblSub.Size = New System.Drawing.Size(360, 36)
        Me.lblSub.TabIndex = 1
        Me.lblSub.Text = "Use at least 8 characters with letters and numbers."
        '
        'lblCurrent
        '
        Me.lblCurrent.AutoSize = True
        Me.lblCurrent.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblCurrent.Location = New System.Drawing.Point(238, 100)
        Me.lblCurrent.Name = "lblCurrent"
        Me.lblCurrent.Size = New System.Drawing.Size(101, 15)
        Me.lblCurrent.TabIndex = 2
        Me.lblCurrent.Text = "Current password"
        '
        'txtCurrent
        '
        Me.txtCurrent.Location = New System.Drawing.Point(240, 120)
        Me.txtCurrent.Name = "txtCurrent"
        Me.txtCurrent.Size = New System.Drawing.Size(360, 38)
        Me.txtCurrent.TabIndex = 0
        Me.txtCurrent.UseSystemPasswordChar = True
        '
        'lblNew
        '
        Me.lblNew.AutoSize = True
        Me.lblNew.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblNew.Location = New System.Drawing.Point(238, 170)
        Me.lblNew.Name = "lblNew"
        Me.lblNew.Size = New System.Drawing.Size(85, 15)
        Me.lblNew.TabIndex = 4
        Me.lblNew.Text = "New password"
        '
        'txtNew
        '
        Me.txtNew.Location = New System.Drawing.Point(240, 190)
        Me.txtNew.Name = "txtNew"
        Me.txtNew.Size = New System.Drawing.Size(360, 38)
        Me.txtNew.TabIndex = 1
        Me.txtNew.UseSystemPasswordChar = True
        '
        'lblConfirm
        '
        Me.lblConfirm.AutoSize = True
        Me.lblConfirm.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblConfirm.Location = New System.Drawing.Point(238, 240)
        Me.lblConfirm.Name = "lblConfirm"
        Me.lblConfirm.Size = New System.Drawing.Size(130, 15)
        Me.lblConfirm.TabIndex = 6
        Me.lblConfirm.Text = "Confirm new password"
        '
        'txtConfirm
        '
        Me.txtConfirm.Location = New System.Drawing.Point(240, 260)
        Me.txtConfirm.Name = "txtConfirm"
        Me.txtConfirm.Size = New System.Drawing.Size(360, 38)
        Me.txtConfirm.TabIndex = 2
        Me.txtConfirm.UseSystemPasswordChar = True
        '
        'lblError
        '
        Me.lblError.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.lblError.Location = New System.Drawing.Point(240, 306)
        Me.lblError.Name = "lblError"
        Me.lblError.Size = New System.Drawing.Size(360, 24)
        Me.lblError.TabIndex = 8
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(464, 336)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(136, 36)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save password"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(360, 336)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(96, 36)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'ChangePasswordForm
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(630, 400)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblError)
        Me.Controls.Add(Me.txtConfirm)
        Me.Controls.Add(Me.lblConfirm)
        Me.Controls.Add(Me.txtNew)
        Me.Controls.Add(Me.lblNew)
        Me.Controls.Add(Me.txtCurrent)
        Me.Controls.Add(Me.lblCurrent)
        Me.Controls.Add(Me.lblSub)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ChangePasswordForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Change password"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblSub As System.Windows.Forms.Label
    Friend WithEvents lblCurrent As System.Windows.Forms.Label
    Friend WithEvents txtCurrent As Global.CSUAttendance.TextField
    Friend WithEvents lblNew As System.Windows.Forms.Label
    Friend WithEvents txtNew As Global.CSUAttendance.TextField
    Friend WithEvents lblConfirm As System.Windows.Forms.Label
    Friend WithEvents txtConfirm As Global.CSUAttendance.TextField
    Friend WithEvents lblError As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
