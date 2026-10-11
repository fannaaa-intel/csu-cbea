<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TeacherDialog
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
        Me.components = New System.ComponentModel.Container()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblEmpNo = New System.Windows.Forms.Label()
        Me.txtEmpNo = New Global.CSUAttendance.TextField()
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.txtUsername = New Global.CSUAttendance.TextField()
        Me.lblFirst = New System.Windows.Forms.Label()
        Me.txtFirst = New Global.CSUAttendance.TextField()
        Me.lblMiddle = New System.Windows.Forms.Label()
        Me.txtMiddle = New Global.CSUAttendance.TextField()
        Me.lblLast = New System.Windows.Forms.Label()
        Me.txtLast = New Global.CSUAttendance.TextField()
        Me.lblDepartment = New System.Windows.Forms.Label()
        Me.txtDepartment = New Global.CSUAttendance.TextField()
        Me.lblContact = New System.Windows.Forms.Label()
        Me.txtContact = New Global.CSUAttendance.TextField()
        Me.lblHint = New System.Windows.Forms.Label()
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
        Me.lblTitle.Location = New System.Drawing.Point(28, 24)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(124, 28)
        Me.lblTitle.TabIndex = 20
        Me.lblTitle.Text = "Add teacher"
        '
        'lblEmpNo
        '
        Me.lblEmpNo.AutoSize = True
        Me.lblEmpNo.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblEmpNo.Location = New System.Drawing.Point(28, 76)
        Me.lblEmpNo.Name = "lblEmpNo"
        Me.lblEmpNo.Size = New System.Drawing.Size(80, 15)
        Me.lblEmpNo.TabIndex = 21
        Me.lblEmpNo.Text = "Employee no."
        '
        'txtEmpNo
        '
        Me.txtEmpNo.Location = New System.Drawing.Point(30, 96)
        Me.txtEmpNo.MaxLength = 20
        Me.txtEmpNo.Name = "txtEmpNo"
        Me.txtEmpNo.Placeholder = "e.g. EMP-1004"
        Me.txtEmpNo.Size = New System.Drawing.Size(200, 36)
        Me.txtEmpNo.TabIndex = 0
        '
        'lblUsername
        '
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblUsername.Location = New System.Drawing.Point(248, 76)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Size = New System.Drawing.Size(61, 15)
        Me.lblUsername.TabIndex = 22
        Me.lblUsername.Text = "Username"
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(250, 96)
        Me.txtUsername.MaxLength = 50
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Placeholder = "e.g. jdelacruz"
        Me.txtUsername.Size = New System.Drawing.Size(240, 36)
        Me.txtUsername.TabIndex = 1
        '
        'lblFirst
        '
        Me.lblFirst.AutoSize = True
        Me.lblFirst.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblFirst.Location = New System.Drawing.Point(28, 146)
        Me.lblFirst.Name = "lblFirst"
        Me.lblFirst.Size = New System.Drawing.Size(66, 15)
        Me.lblFirst.TabIndex = 23
        Me.lblFirst.Text = "First name"
        '
        'txtFirst
        '
        Me.txtFirst.Location = New System.Drawing.Point(30, 166)
        Me.txtFirst.MaxLength = 50
        Me.txtFirst.Name = "txtFirst"
        Me.txtFirst.Size = New System.Drawing.Size(150, 36)
        Me.txtFirst.TabIndex = 2
        '
        'lblMiddle
        '
        Me.lblMiddle.AutoSize = True
        Me.lblMiddle.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblMiddle.Location = New System.Drawing.Point(188, 146)
        Me.lblMiddle.Name = "lblMiddle"
        Me.lblMiddle.Size = New System.Drawing.Size(82, 15)
        Me.lblMiddle.TabIndex = 24
        Me.lblMiddle.Text = "Middle name"
        '
        'txtMiddle
        '
        Me.txtMiddle.Location = New System.Drawing.Point(190, 166)
        Me.txtMiddle.MaxLength = 50
        Me.txtMiddle.Name = "txtMiddle"
        Me.txtMiddle.Placeholder = "optional"
        Me.txtMiddle.Size = New System.Drawing.Size(140, 36)
        Me.txtMiddle.TabIndex = 3
        '
        'lblLast
        '
        Me.lblLast.AutoSize = True
        Me.lblLast.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblLast.Location = New System.Drawing.Point(338, 146)
        Me.lblLast.Name = "lblLast"
        Me.lblLast.Size = New System.Drawing.Size(64, 15)
        Me.lblLast.TabIndex = 25
        Me.lblLast.Text = "Last name"
        '
        'txtLast
        '
        Me.txtLast.Location = New System.Drawing.Point(340, 166)
        Me.txtLast.MaxLength = 50
        Me.txtLast.Name = "txtLast"
        Me.txtLast.Size = New System.Drawing.Size(150, 36)
        Me.txtLast.TabIndex = 4
        '
        'lblDepartment
        '
        Me.lblDepartment.AutoSize = True
        Me.lblDepartment.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblDepartment.Location = New System.Drawing.Point(28, 216)
        Me.lblDepartment.Name = "lblDepartment"
        Me.lblDepartment.Size = New System.Drawing.Size(74, 15)
        Me.lblDepartment.TabIndex = 26
        Me.lblDepartment.Text = "Department"
        '
        'txtDepartment
        '
        Me.txtDepartment.Location = New System.Drawing.Point(30, 236)
        Me.txtDepartment.MaxLength = 100
        Me.txtDepartment.Name = "txtDepartment"
        Me.txtDepartment.Placeholder = "optional"
        Me.txtDepartment.Size = New System.Drawing.Size(300, 36)
        Me.txtDepartment.TabIndex = 5
        '
        'lblContact
        '
        Me.lblContact.AutoSize = True
        Me.lblContact.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblContact.Location = New System.Drawing.Point(338, 216)
        Me.lblContact.Name = "lblContact"
        Me.lblContact.Size = New System.Drawing.Size(71, 15)
        Me.lblContact.TabIndex = 27
        Me.lblContact.Text = "Contact no."
        '
        'txtContact
        '
        Me.txtContact.Location = New System.Drawing.Point(340, 236)
        Me.txtContact.MaxLength = 20
        Me.txtContact.Name = "txtContact"
        Me.txtContact.Placeholder = "optional"
        Me.txtContact.Size = New System.Drawing.Size(150, 36)
        Me.txtContact.TabIndex = 6
        '
        'lblHint
        '
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblHint.Location = New System.Drawing.Point(28, 284)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(462, 34)
        Me.lblHint.TabIndex = 28
        Me.lblHint.Text = "The first password will be the employee no. The teacher must change it at first login."
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(394, 330)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(96, 36)
        Me.btnSave.TabIndex = 7
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(290, 330)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(96, 36)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'errProvider
        '
        Me.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.errProvider.ContainerControl = Me
        '
        'TeacherDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(520, 390)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblHint)
        Me.Controls.Add(Me.txtContact)
        Me.Controls.Add(Me.lblContact)
        Me.Controls.Add(Me.txtDepartment)
        Me.Controls.Add(Me.lblDepartment)
        Me.Controls.Add(Me.txtLast)
        Me.Controls.Add(Me.lblLast)
        Me.Controls.Add(Me.txtMiddle)
        Me.Controls.Add(Me.lblMiddle)
        Me.Controls.Add(Me.txtFirst)
        Me.Controls.Add(Me.lblFirst)
        Me.Controls.Add(Me.txtUsername)
        Me.Controls.Add(Me.lblUsername)
        Me.Controls.Add(Me.txtEmpNo)
        Me.Controls.Add(Me.lblEmpNo)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "TeacherDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Teacher"
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblEmpNo As System.Windows.Forms.Label
    Friend WithEvents txtEmpNo As Global.CSUAttendance.TextField
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents txtUsername As Global.CSUAttendance.TextField
    Friend WithEvents lblFirst As System.Windows.Forms.Label
    Friend WithEvents txtFirst As Global.CSUAttendance.TextField
    Friend WithEvents lblMiddle As System.Windows.Forms.Label
    Friend WithEvents txtMiddle As Global.CSUAttendance.TextField
    Friend WithEvents lblLast As System.Windows.Forms.Label
    Friend WithEvents txtLast As Global.CSUAttendance.TextField
    Friend WithEvents lblDepartment As System.Windows.Forms.Label
    Friend WithEvents txtDepartment As Global.CSUAttendance.TextField
    Friend WithEvents lblContact As System.Windows.Forms.Label
    Friend WithEvents txtContact As Global.CSUAttendance.TextField
    Friend WithEvents lblHint As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
