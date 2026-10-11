<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class StudentDialog
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
        Me.lblStudentNo = New System.Windows.Forms.Label()
        Me.txtStudentNo = New Global.CSUAttendance.TextField()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.cboStatus = New System.Windows.Forms.ComboBox()
        Me.lblFirst = New System.Windows.Forms.Label()
        Me.txtFirst = New Global.CSUAttendance.TextField()
        Me.lblMiddle = New System.Windows.Forms.Label()
        Me.txtMiddle = New Global.CSUAttendance.TextField()
        Me.lblLast = New System.Windows.Forms.Label()
        Me.txtLast = New Global.CSUAttendance.TextField()
        Me.lblSex = New System.Windows.Forms.Label()
        Me.cboSex = New System.Windows.Forms.ComboBox()
        Me.lblSection = New System.Windows.Forms.Label()
        Me.cboSection = New System.Windows.Forms.ComboBox()
        Me.lblContact = New System.Windows.Forms.Label()
        Me.txtContact = New Global.CSUAttendance.TextField()
        Me.lblGuardian = New System.Windows.Forms.Label()
        Me.txtGuardian = New Global.CSUAttendance.TextField()
        Me.lblGuardianContact = New System.Windows.Forms.Label()
        Me.txtGuardianContact = New Global.CSUAttendance.TextField()
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
        Me.lblTitle.Location = New System.Drawing.Point(238, 24)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(124, 28)
        Me.lblTitle.TabIndex = 30
        Me.lblTitle.Text = "Add student"
        '
        'lblStudentNo
        '
        Me.lblStudentNo.AutoSize = True
        Me.lblStudentNo.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblStudentNo.Location = New System.Drawing.Point(238, 76)
        Me.lblStudentNo.Name = "lblStudentNo"
        Me.lblStudentNo.Size = New System.Drawing.Size(71, 15)
        Me.lblStudentNo.TabIndex = 31
        Me.lblStudentNo.Text = "Student no."
        '
        'txtStudentNo
        '
        Me.txtStudentNo.Location = New System.Drawing.Point(240, 96)
        Me.txtStudentNo.MaxLength = 7
        Me.txtStudentNo.Name = "txtStudentNo"
        Me.txtStudentNo.Placeholder = "e.g. 24-1041"
        Me.txtStudentNo.Size = New System.Drawing.Size(150, 36)
        Me.txtStudentNo.TabIndex = 0
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblStatus.Location = New System.Drawing.Point(548, 76)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(40, 15)
        Me.lblStatus.TabIndex = 32
        Me.lblStatus.Text = "Status"
        '
        'cboStatus
        '
        Me.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatus.FormattingEnabled = True
        Me.cboStatus.Items.AddRange(New Object() {"Active", "Inactive"})
        Me.cboStatus.Location = New System.Drawing.Point(550, 96)
        Me.cboStatus.Name = "cboStatus"
        Me.cboStatus.Size = New System.Drawing.Size(150, 25)
        Me.cboStatus.TabIndex = 1
        '
        'lblFirst
        '
        Me.lblFirst.AutoSize = True
        Me.lblFirst.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblFirst.Location = New System.Drawing.Point(238, 146)
        Me.lblFirst.Name = "lblFirst"
        Me.lblFirst.Size = New System.Drawing.Size(66, 15)
        Me.lblFirst.TabIndex = 33
        Me.lblFirst.Text = "First name"
        '
        'txtFirst
        '
        Me.txtFirst.Location = New System.Drawing.Point(240, 166)
        Me.txtFirst.MaxLength = 50
        Me.txtFirst.Name = "txtFirst"
        Me.txtFirst.Size = New System.Drawing.Size(150, 36)
        Me.txtFirst.TabIndex = 2
        '
        'lblMiddle
        '
        Me.lblMiddle.AutoSize = True
        Me.lblMiddle.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblMiddle.Location = New System.Drawing.Point(398, 146)
        Me.lblMiddle.Name = "lblMiddle"
        Me.lblMiddle.Size = New System.Drawing.Size(82, 15)
        Me.lblMiddle.TabIndex = 34
        Me.lblMiddle.Text = "Middle name"
        '
        'txtMiddle
        '
        Me.txtMiddle.Location = New System.Drawing.Point(400, 166)
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
        Me.lblLast.Location = New System.Drawing.Point(548, 146)
        Me.lblLast.Name = "lblLast"
        Me.lblLast.Size = New System.Drawing.Size(64, 15)
        Me.lblLast.TabIndex = 35
        Me.lblLast.Text = "Last name"
        '
        'txtLast
        '
        Me.txtLast.Location = New System.Drawing.Point(550, 166)
        Me.txtLast.MaxLength = 50
        Me.txtLast.Name = "txtLast"
        Me.txtLast.Size = New System.Drawing.Size(150, 36)
        Me.txtLast.TabIndex = 4
        '
        'lblSex
        '
        Me.lblSex.AutoSize = True
        Me.lblSex.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblSex.Location = New System.Drawing.Point(238, 216)
        Me.lblSex.Name = "lblSex"
        Me.lblSex.Size = New System.Drawing.Size(25, 15)
        Me.lblSex.TabIndex = 36
        Me.lblSex.Text = "Sex"
        '
        'cboSex
        '
        Me.cboSex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSex.FormattingEnabled = True
        Me.cboSex.Items.AddRange(New Object() {"Male", "Female"})
        Me.cboSex.Location = New System.Drawing.Point(240, 236)
        Me.cboSex.Name = "cboSex"
        Me.cboSex.Size = New System.Drawing.Size(150, 25)
        Me.cboSex.TabIndex = 5
        '
        'lblSection
        '
        Me.lblSection.AutoSize = True
        Me.lblSection.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblSection.Location = New System.Drawing.Point(398, 216)
        Me.lblSection.Name = "lblSection"
        Me.lblSection.Size = New System.Drawing.Size(48, 15)
        Me.lblSection.TabIndex = 37
        Me.lblSection.Text = "Section"
        '
        'cboSection
        '
        Me.cboSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSection.FormattingEnabled = True
        Me.cboSection.Location = New System.Drawing.Point(400, 236)
        Me.cboSection.Name = "cboSection"
        Me.cboSection.Size = New System.Drawing.Size(300, 25)
        Me.cboSection.TabIndex = 6
        '
        'lblContact
        '
        Me.lblContact.AutoSize = True
        Me.lblContact.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblContact.Location = New System.Drawing.Point(238, 286)
        Me.lblContact.Name = "lblContact"
        Me.lblContact.Size = New System.Drawing.Size(71, 15)
        Me.lblContact.TabIndex = 38
        Me.lblContact.Text = "Contact no."
        '
        'txtContact
        '
        Me.txtContact.Location = New System.Drawing.Point(240, 306)
        Me.txtContact.MaxLength = 20
        Me.txtContact.Name = "txtContact"
        Me.txtContact.Placeholder = "optional"
        Me.txtContact.Size = New System.Drawing.Size(150, 36)
        Me.txtContact.TabIndex = 7
        '
        'lblGuardian
        '
        Me.lblGuardian.AutoSize = True
        Me.lblGuardian.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblGuardian.Location = New System.Drawing.Point(398, 286)
        Me.lblGuardian.Name = "lblGuardian"
        Me.lblGuardian.Size = New System.Drawing.Size(92, 15)
        Me.lblGuardian.TabIndex = 39
        Me.lblGuardian.Text = "Guardian name"
        '
        'txtGuardian
        '
        Me.txtGuardian.Location = New System.Drawing.Point(400, 306)
        Me.txtGuardian.MaxLength = 100
        Me.txtGuardian.Name = "txtGuardian"
        Me.txtGuardian.Placeholder = "optional"
        Me.txtGuardian.Size = New System.Drawing.Size(140, 36)
        Me.txtGuardian.TabIndex = 8
        '
        'lblGuardianContact
        '
        Me.lblGuardianContact.AutoSize = True
        Me.lblGuardianContact.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblGuardianContact.Location = New System.Drawing.Point(548, 286)
        Me.lblGuardianContact.Name = "lblGuardianContact"
        Me.lblGuardianContact.Size = New System.Drawing.Size(103, 15)
        Me.lblGuardianContact.TabIndex = 40
        Me.lblGuardianContact.Text = "Guardian contact"
        '
        'txtGuardianContact
        '
        Me.txtGuardianContact.Location = New System.Drawing.Point(550, 306)
        Me.txtGuardianContact.MaxLength = 20
        Me.txtGuardianContact.Name = "txtGuardianContact"
        Me.txtGuardianContact.Placeholder = "optional"
        Me.txtGuardianContact.Size = New System.Drawing.Size(150, 36)
        Me.txtGuardianContact.TabIndex = 9
        '
        'lblHint
        '
        Me.lblHint.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblHint.Location = New System.Drawing.Point(238, 354)
        Me.lblHint.Name = "lblHint"
        Me.lblHint.Size = New System.Drawing.Size(462, 34)
        Me.lblHint.TabIndex = 41
        Me.lblHint.Text = "The username and first password will be the student no. The student must change the password at first login."
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(604, 396)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(96, 36)
        Me.btnSave.TabIndex = 10
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(500, 396)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(96, 36)
        Me.btnCancel.TabIndex = 11
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'errProvider
        '
        Me.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.errProvider.ContainerControl = Me
        '
        'StudentDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(730, 460)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblHint)
        Me.Controls.Add(Me.txtGuardianContact)
        Me.Controls.Add(Me.lblGuardianContact)
        Me.Controls.Add(Me.txtGuardian)
        Me.Controls.Add(Me.lblGuardian)
        Me.Controls.Add(Me.txtContact)
        Me.Controls.Add(Me.lblContact)
        Me.Controls.Add(Me.cboSection)
        Me.Controls.Add(Me.lblSection)
        Me.Controls.Add(Me.cboSex)
        Me.Controls.Add(Me.lblSex)
        Me.Controls.Add(Me.txtLast)
        Me.Controls.Add(Me.lblLast)
        Me.Controls.Add(Me.txtMiddle)
        Me.Controls.Add(Me.lblMiddle)
        Me.Controls.Add(Me.txtFirst)
        Me.Controls.Add(Me.lblFirst)
        Me.Controls.Add(Me.cboStatus)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.txtStudentNo)
        Me.Controls.Add(Me.lblStudentNo)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "StudentDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Student"
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblStudentNo As System.Windows.Forms.Label
    Friend WithEvents txtStudentNo As Global.CSUAttendance.TextField
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cboStatus As System.Windows.Forms.ComboBox
    Friend WithEvents lblFirst As System.Windows.Forms.Label
    Friend WithEvents txtFirst As Global.CSUAttendance.TextField
    Friend WithEvents lblMiddle As System.Windows.Forms.Label
    Friend WithEvents txtMiddle As Global.CSUAttendance.TextField
    Friend WithEvents lblLast As System.Windows.Forms.Label
    Friend WithEvents txtLast As Global.CSUAttendance.TextField
    Friend WithEvents lblSex As System.Windows.Forms.Label
    Friend WithEvents cboSex As System.Windows.Forms.ComboBox
    Friend WithEvents lblSection As System.Windows.Forms.Label
    Friend WithEvents cboSection As System.Windows.Forms.ComboBox
    Friend WithEvents lblContact As System.Windows.Forms.Label
    Friend WithEvents txtContact As Global.CSUAttendance.TextField
    Friend WithEvents lblGuardian As System.Windows.Forms.Label
    Friend WithEvents txtGuardian As Global.CSUAttendance.TextField
    Friend WithEvents lblGuardianContact As System.Windows.Forms.Label
    Friend WithEvents txtGuardianContact As Global.CSUAttendance.TextField
    Friend WithEvents lblHint As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
