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
        Me.lblBrandFooter = New System.Windows.Forms.Label()
        Me.lblBrandTag = New System.Windows.Forms.Label()
        Me.lblBrandTitle = New System.Windows.Forms.Label()
        Me.lblSchool = New System.Windows.Forms.Label()
        Me.lblBrandIcon = New System.Windows.Forms.Label()
        Me.pnlForm = New System.Windows.Forms.Panel()
        Me.lblHint = New System.Windows.Forms.Label()
        Me.btnLogin = New System.Windows.Forms.Button()
        Me.lblError = New System.Windows.Forms.Label()
        Me.chkShowPassword = New System.Windows.Forms.CheckBox()
        Me.txtPassword = New Global.CSUAttendance.TextField()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.txtUsername = New Global.CSUAttendance.TextField()
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.lblSub = New System.Windows.Forms.Label()
        Me.lblWelcome = New System.Windows.Forms.Label()
        Me.pnlBrand.SuspendLayout()
        Me.pnlForm.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlBrand
        '
        Me.pnlBrand.BackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(236, Byte), Integer))
        Me.pnlBrand.Controls.Add(Me.lblBrandFooter)
        Me.pnlBrand.Controls.Add(Me.lblBrandTag)
        Me.pnlBrand.Controls.Add(Me.lblBrandTitle)
        Me.pnlBrand.Controls.Add(Me.lblSchool)
        Me.pnlBrand.Controls.Add(Me.lblBrandIcon)
        Me.pnlBrand.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlBrand.Location = New System.Drawing.Point(0, 0)
        Me.pnlBrand.Name = "pnlBrand"
        Me.pnlBrand.Size = New System.Drawing.Size(400, 560)
        Me.pnlBrand.TabIndex = 0
        '
        'lblBrandFooter
        '
        Me.lblBrandFooter.AutoSize = True
        Me.lblBrandFooter.Font = New System.Drawing.Font("Segoe UI", 8.5!)
        Me.lblBrandFooter.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblBrandFooter.Location = New System.Drawing.Point(48, 508)
        Me.lblBrandFooter.Name = "lblBrandFooter"
        Me.lblBrandFooter.Size = New System.Drawing.Size(196, 15)
        Me.lblBrandFooter.TabIndex = 4
        Me.lblBrandFooter.Text = "BSAIS 3B  •  System Project  •  2026"
        '
        'lblBrandTag
        '
        Me.lblBrandTag.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblBrandTag.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblBrandTag.Location = New System.Drawing.Point(48, 252)
        Me.lblBrandTag.Name = "lblBrandTag"
        Me.lblBrandTag.Size = New System.Drawing.Size(310, 64)
        Me.lblBrandTag.TabIndex = 3
        Me.lblBrandTag.Text = "Record, monitor and report student attendance — following the CSU Student Manual attendance policy."
        '
        'lblBrandTitle
        '
        Me.lblBrandTitle.AutoSize = True
        Me.lblBrandTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 21.0!)
        Me.lblBrandTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblBrandTitle.Location = New System.Drawing.Point(43, 160)
        Me.lblBrandTitle.Name = "lblBrandTitle"
        Me.lblBrandTitle.Size = New System.Drawing.Size(298, 80)
        Me.lblBrandTitle.TabIndex = 2
        Me.lblBrandTitle.Text = "Student Attendance" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Management System"
        '
        'lblSchool
        '
        Me.lblSchool.AutoSize = True
        Me.lblSchool.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblSchool.ForeColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(79, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.lblSchool.Location = New System.Drawing.Point(48, 136)
        Me.lblSchool.Name = "lblSchool"
        Me.lblSchool.Size = New System.Drawing.Size(176, 15)
        Me.lblSchool.TabIndex = 1
        Me.lblSchool.Text = "CAGAYAN STATE UNIVERSITY"
        '
        'lblBrandIcon
        '
        Me.lblBrandIcon.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(79, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.lblBrandIcon.Font = New System.Drawing.Font("Segoe MDL2 Assets", 20.0!)
        Me.lblBrandIcon.ForeColor = System.Drawing.Color.White
        Me.lblBrandIcon.Location = New System.Drawing.Point(48, 64)
        Me.lblBrandIcon.Name = "lblBrandIcon"
        Me.lblBrandIcon.Size = New System.Drawing.Size(52, 52)
        Me.lblBrandIcon.TabIndex = 0
        Me.lblBrandIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlForm
        '
        Me.pnlForm.BackColor = System.Drawing.Color.White
        Me.pnlForm.Controls.Add(Me.lblHint)
        Me.pnlForm.Controls.Add(Me.btnLogin)
        Me.pnlForm.Controls.Add(Me.lblError)
        Me.pnlForm.Controls.Add(Me.chkShowPassword)
        Me.pnlForm.Controls.Add(Me.txtPassword)
        Me.pnlForm.Controls.Add(Me.lblPassword)
        Me.pnlForm.Controls.Add(Me.txtUsername)
        Me.pnlForm.Controls.Add(Me.lblUsername)
        Me.pnlForm.Controls.Add(Me.lblSub)
        Me.pnlForm.Controls.Add(Me.lblWelcome)
        Me.pnlForm.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlForm.Location = New System.Drawing.Point(400, 0)
        Me.pnlForm.Name = "pnlForm"
        Me.pnlForm.Size = New System.Drawing.Size(520, 560)
        Me.pnlForm.TabIndex = 1
        '
        'lblHint
        '
        Me.lblHint.AutoSize = True
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 8.5!)
        Me.lblHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblHint.Location = New System.Drawing.Point(82, 474)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(268, 15)
        Me.lblHint.TabIndex = 9
        Me.lblHint.Text = "Forgot your password? Ask the system administrator."
        '
        'btnLogin
        '
        Me.btnLogin.Location = New System.Drawing.Point(82, 416)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(356, 42)
        Me.btnLogin.TabIndex = 3
        Me.btnLogin.Text = "Sign in"
        Me.btnLogin.UseVisualStyleBackColor = True
        '
        'lblError
        '
        Me.lblError.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(220, Byte), Integer), CType(CType(38, Byte), Integer), CType(CType(38, Byte), Integer))
        Me.lblError.Location = New System.Drawing.Point(82, 384)
        Me.lblError.Name = "lblError"
        Me.lblError.Size = New System.Drawing.Size(356, 28)
        Me.lblError.TabIndex = 7
        Me.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'chkShowPassword
        '
        Me.chkShowPassword.AutoSize = True
        Me.chkShowPassword.Cursor = System.Windows.Forms.Cursors.Hand
        Me.chkShowPassword.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.chkShowPassword.Location = New System.Drawing.Point(82, 358)
        Me.chkShowPassword.Name = "chkShowPassword"
        Me.chkShowPassword.Size = New System.Drawing.Size(108, 19)
        Me.chkShowPassword.TabIndex = 2
        Me.chkShowPassword.TabStop = False
        Me.chkShowPassword.Text = "Show password"
        Me.chkShowPassword.UseVisualStyleBackColor = True
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(82, 312)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Placeholder = "Enter your password"
        Me.txtPassword.Size = New System.Drawing.Size(356, 40)
        Me.txtPassword.TabIndex = 1
        Me.txtPassword.UseSystemPasswordChar = True
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblPassword.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblPassword.Location = New System.Drawing.Point(80, 290)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(58, 15)
        Me.lblPassword.TabIndex = 5
        Me.lblPassword.Text = "Password"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(82, 236)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Placeholder = "Username or student number"
        Me.txtUsername.Size = New System.Drawing.Size(356, 40)
        Me.txtUsername.TabIndex = 0
        '
        'lblUsername
        '
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblUsername.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblUsername.Location = New System.Drawing.Point(80, 214)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Size = New System.Drawing.Size(62, 15)
        Me.lblUsername.TabIndex = 4
        Me.lblUsername.Text = "Username"
        '
        'lblSub
        '
        Me.lblSub.AutoSize = True
        Me.lblSub.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblSub.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblSub.Location = New System.Drawing.Point(80, 160)
        Me.lblSub.Name = "lblSub"
        Me.lblSub.Size = New System.Drawing.Size(237, 19)
        Me.lblSub.TabIndex = 8
        Me.lblSub.Text = "Sign in to your account to continue."
        '
        'lblWelcome
        '
        Me.lblWelcome.AutoSize = True
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI Semibold", 18.0!)
        Me.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblWelcome.Location = New System.Drawing.Point(76, 120)
        Me.lblWelcome.Name = "lblWelcome"
        Me.lblWelcome.Size = New System.Drawing.Size(181, 32)
        Me.lblWelcome.TabIndex = 6
        Me.lblWelcome.Text = "Welcome back"
        '
        'LoginForm
        '
        Me.AcceptButton = Me.btnLogin
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(920, 560)
        Me.Controls.Add(Me.pnlForm)
        Me.Controls.Add(Me.pnlBrand)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "LoginForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "CSU Attendance  -  Sign in"
        Me.pnlBrand.ResumeLayout(False)
        Me.pnlBrand.PerformLayout()
        Me.pnlForm.ResumeLayout(False)
        Me.pnlForm.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlBrand As System.Windows.Forms.Panel
    Friend WithEvents lblBrandFooter As System.Windows.Forms.Label
    Friend WithEvents lblBrandTag As System.Windows.Forms.Label
    Friend WithEvents lblBrandTitle As System.Windows.Forms.Label
    Friend WithEvents lblSchool As System.Windows.Forms.Label
    Friend WithEvents lblBrandIcon As System.Windows.Forms.Label
    Friend WithEvents pnlForm As System.Windows.Forms.Panel
    Friend WithEvents lblHint As System.Windows.Forms.Label
    Friend WithEvents btnLogin As System.Windows.Forms.Button
    Friend WithEvents lblError As System.Windows.Forms.Label
    Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
    Friend WithEvents txtPassword As Global.CSUAttendance.TextField
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtUsername As Global.CSUAttendance.TextField
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents lblSub As System.Windows.Forms.Label
    Friend WithEvents lblWelcome As System.Windows.Forms.Label
End Class
