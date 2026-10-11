<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AdminDialog
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
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.txtUsername = New Global.CSUAttendance.TextField()
        Me.lblFullName = New System.Windows.Forms.Label()
        Me.txtFullName = New Global.CSUAttendance.TextField()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.txtPassword = New Global.CSUAttendance.TextField()
        Me.lblConfirm = New System.Windows.Forms.Label()
        Me.txtConfirm = New Global.CSUAttendance.TextField()
        Me.lblInfo = New System.Windows.Forms.Label()
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
        Me.lblTitle.Text = "Add admin"
        '
        'lblUsername
        '
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblUsername.Location = New System.Drawing.Point(236, 76)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Size = New System.Drawing.Size(80, 15)
        Me.lblUsername.TabIndex = 101
        Me.lblUsername.Text = "Username"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(238, 96)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Placeholder = "e.g. registrar"
        Me.txtUsername.Size = New System.Drawing.Size(220, 36)
        Me.txtUsername.TabIndex = 0
        '
        'lblFullName
        '
        Me.lblFullName.AutoSize = True
        Me.lblFullName.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblFullName.Location = New System.Drawing.Point(468, 76)
        Me.lblFullName.Name = "lblFullName"
        Me.lblFullName.Size = New System.Drawing.Size(80, 15)
        Me.lblFullName.TabIndex = 102
        Me.lblFullName.Text = "Full name"
        '
        'txtFullName
        '
        Me.txtFullName.Location = New System.Drawing.Point(470, 96)
        Me.txtFullName.Name = "txtFullName"
        Me.txtFullName.Placeholder = "e.g. Juan Dela Cruz"
        Me.txtFullName.Size = New System.Drawing.Size(240, 36)
        Me.txtFullName.TabIndex = 1
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblPassword.Location = New System.Drawing.Point(236, 146)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(80, 15)
        Me.lblPassword.TabIndex = 103
        Me.lblPassword.Text = "Temporary password"
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(238, 166)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Placeholder = "At least 8 characters"
        Me.txtPassword.UseSystemPasswordChar = True
        Me.txtPassword.Size = New System.Drawing.Size(220, 36)
        Me.txtPassword.TabIndex = 2
        '
        'lblConfirm
        '
        Me.lblConfirm.AutoSize = True
        Me.lblConfirm.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblConfirm.Location = New System.Drawing.Point(468, 146)
        Me.lblConfirm.Name = "lblConfirm"
        Me.lblConfirm.Size = New System.Drawing.Size(80, 15)
        Me.lblConfirm.TabIndex = 104
        Me.lblConfirm.Text = "Confirm password"
        '
        'txtConfirm
        '
        Me.txtConfirm.Location = New System.Drawing.Point(470, 166)
        Me.txtConfirm.Name = "txtConfirm"
        Me.txtConfirm.Placeholder = "Type it again"
        Me.txtConfirm.UseSystemPasswordChar = True
        Me.txtConfirm.Size = New System.Drawing.Size(240, 36)
        Me.txtConfirm.TabIndex = 3
        '
        'lblInfo
        '
        Me.lblInfo.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblInfo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblInfo.Location = New System.Drawing.Point(236, 216)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(472, 56)
        Me.lblInfo.TabIndex = 105
        Me.lblInfo.Text = "The new admin must replace this temporary password at their first sign-in."
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(598, 340)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(112, 36)
        Me.btnSave.TabIndex = 4
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(490, 340)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 36)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'errProvider
        '
        Me.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.errProvider.ContainerControl = Me
        '
        'AdminDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(740, 404)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblInfo)
        Me.Controls.Add(Me.txtConfirm)
        Me.Controls.Add(Me.lblConfirm)
        Me.Controls.Add(Me.txtPassword)
        Me.Controls.Add(Me.lblPassword)
        Me.Controls.Add(Me.txtFullName)
        Me.Controls.Add(Me.lblFullName)
        Me.Controls.Add(Me.txtUsername)
        Me.Controls.Add(Me.lblUsername)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AdminDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add admin"
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents txtUsername As Global.CSUAttendance.TextField
    Friend WithEvents lblFullName As System.Windows.Forms.Label
    Friend WithEvents txtFullName As Global.CSUAttendance.TextField
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtPassword As Global.CSUAttendance.TextField
    Friend WithEvents lblConfirm As System.Windows.Forms.Label
    Friend WithEvents txtConfirm As Global.CSUAttendance.TextField
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
