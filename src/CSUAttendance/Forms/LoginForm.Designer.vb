<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LoginForm
    Inherits System.Windows.Forms.Form

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
        Me.pnlBrand = New System.Windows.Forms.Panel()
        Me.pnlForm = New System.Windows.Forms.Panel()
        Me.pnlCard = New System.Windows.Forms.Panel()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.btnLogin = New Global.CSUAttendance.PillButton()
        Me.lblError = New System.Windows.Forms.Label()
        Me.chkShowPassword = New System.Windows.Forms.CheckBox()
        Me.txtPassword = New Global.CSUAttendance.TextField()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.txtUsername = New Global.CSUAttendance.TextField()
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.lblSub = New System.Windows.Forms.Label()
        Me.lblWelcome = New System.Windows.Forms.Label()
        Me.pnlForm.SuspendLayout()
        Me.pnlCard.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlBrand
        '
        Me.pnlBrand.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(79, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.pnlBrand.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlBrand.Location = New System.Drawing.Point(0, 0)
        Me.pnlBrand.Name = "pnlBrand"
        Me.pnlBrand.Size = New System.Drawing.Size(560, 700)
        Me.pnlBrand.TabIndex = 1
        '
        'pnlForm
        '
        Me.pnlForm.BackColor = System.Drawing.Color.White
        Me.pnlForm.Controls.Add(Me.pnlCard)
        Me.pnlForm.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlForm.Location = New System.Drawing.Point(560, 0)
        Me.pnlForm.Name = "pnlForm"
        Me.pnlForm.Size = New System.Drawing.Size(640, 700)
        Me.pnlForm.TabIndex = 0
        '
        'pnlCard
        '
        Me.pnlCard.BackColor = System.Drawing.Color.White
        Me.pnlCard.Controls.Add(Me.lblHint)
        Me.pnlCard.Controls.Add(Me.btnLogin)
        Me.pnlCard.Controls.Add(Me.lblError)
        Me.pnlCard.Controls.Add(Me.chkShowPassword)
        Me.pnlCard.Controls.Add(Me.txtPassword)
        Me.pnlCard.Controls.Add(Me.lblPassword)
        Me.pnlCard.Controls.Add(Me.txtUsername)
        Me.pnlCard.Controls.Add(Me.lblUsername)
        Me.pnlCard.Controls.Add(Me.lblSub)
        Me.pnlCard.Controls.Add(Me.lblWelcome)
        Me.pnlCard.Location = New System.Drawing.Point(120, 110)
        Me.pnlCard.Name = "pnlCard"
        Me.pnlCard.Size = New System.Drawing.Size(400, 440)
        Me.pnlCard.TabIndex = 0
        '
        'lblHint
        '
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblHint.Location = New System.Drawing.Point(0, 402)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(400, 22)
        Me.lblHint.TabIndex = 9
        Me.lblHint.Text = "Forgot your password? Ask the system administrator to reset it."
        Me.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnLogin
        '
        Me.btnLogin.Location = New System.Drawing.Point(0, 340)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(400, 50)
        Me.btnLogin.TabIndex = 3
        Me.btnLogin.Text = "Sign in"
        '
        'lblError
        '
        Me.lblError.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.lblError.Location = New System.Drawing.Point(0, 290)
        Me.lblError.Name = "lblError"
        Me.lblError.Size = New System.Drawing.Size(400, 42)
        Me.lblError.TabIndex = 8
        '
        'chkShowPassword
        '
        Me.chkShowPassword.AutoSize = True
        Me.chkShowPassword.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.chkShowPassword.Location = New System.Drawing.Point(2, 262)
        Me.chkShowPassword.Name = "chkShowPassword"
        Me.chkShowPassword.Size = New System.Drawing.Size(108, 19)
        Me.chkShowPassword.TabIndex = 2
        Me.chkShowPassword.Text = "Show password"
        Me.chkShowPassword.UseVisualStyleBackColor = True
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(0, 210)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Placeholder = "Your password"
        Me.txtPassword.Size = New System.Drawing.Size(400, 44)
        Me.txtPassword.TabIndex = 1
        Me.txtPassword.UseSystemPasswordChar = True
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblPassword.Location = New System.Drawing.Point(0, 186)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(58, 15)
        Me.lblPassword.TabIndex = 7
        Me.lblPassword.Text = "Password"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(0, 126)
        Me.txtUsername.MaxLength = 50
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Placeholder = "Username or student no."
        Me.txtUsername.Size = New System.Drawing.Size(400, 44)
        Me.txtUsername.TabIndex = 0
        '
        'lblUsername
        '
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblUsername.Location = New System.Drawing.Point(0, 102)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Size = New System.Drawing.Size(62, 15)
        Me.lblUsername.TabIndex = 6
        Me.lblUsername.Text = "Username"
        '
        'lblSub
        '
        Me.lblSub.Font = New System.Drawing.Font("Segoe UI", 10.5!)
        Me.lblSub.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblSub.Location = New System.Drawing.Point(0, 50)
        Me.lblSub.Name = "lblSub"
        Me.lblSub.Size = New System.Drawing.Size(400, 26)
        Me.lblSub.TabIndex = 5
        Me.lblSub.Text = "Sign in to continue to your dashboard."
        '
        'lblWelcome
        '
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI Semibold", 22.0!)
        Me.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblWelcome.Location = New System.Drawing.Point(-3, 0)
        Me.lblWelcome.Name = "lblWelcome"
        Me.lblWelcome.Size = New System.Drawing.Size(403, 46)
        Me.lblWelcome.TabIndex = 4
        Me.lblWelcome.Text = "Welcome back"
        '
        'LoginForm
        '
        Me.AcceptButton = Me.btnLogin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(1200, 700)
        Me.Controls.Add(Me.pnlForm)
        Me.Controls.Add(Me.pnlBrand)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.MinimumSize = New System.Drawing.Size(1000, 640)
        Me.Name = "LoginForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "CSU Student Attendance Management System  -  Sign in"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlForm.ResumeLayout(False)
        Me.pnlCard.ResumeLayout(False)
        Me.pnlCard.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlBrand As System.Windows.Forms.Panel
    Friend WithEvents pnlForm As System.Windows.Forms.Panel
    Friend WithEvents pnlCard As System.Windows.Forms.Panel
    Friend WithEvents lblHint As System.Windows.Forms.Label
    Friend WithEvents btnLogin As Global.CSUAttendance.PillButton
    Friend WithEvents lblError As System.Windows.Forms.Label
    Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
    Friend WithEvents txtPassword As Global.CSUAttendance.TextField
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtUsername As Global.CSUAttendance.TextField
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents lblSub As System.Windows.Forms.Label
    Friend WithEvents lblWelcome As System.Windows.Forms.Label
End Class
