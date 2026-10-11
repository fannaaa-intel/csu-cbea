<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ClassDialog
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
        Me.lblTerm = New System.Windows.Forms.Label()
        Me.cboTerm = New System.Windows.Forms.ComboBox()
        Me.lblSubject = New System.Windows.Forms.Label()
        Me.cboSubject = New System.Windows.Forms.ComboBox()
        Me.lblSection = New System.Windows.Forms.Label()
        Me.cboSection = New System.Windows.Forms.ComboBox()
        Me.lblTeacher = New System.Windows.Forms.Label()
        Me.cboTeacher = New System.Windows.Forms.ComboBox()
        Me.capDays = New System.Windows.Forms.Label()
        Me.chkMon = New System.Windows.Forms.CheckBox()
        Me.chkTue = New System.Windows.Forms.CheckBox()
        Me.chkWed = New System.Windows.Forms.CheckBox()
        Me.chkThu = New System.Windows.Forms.CheckBox()
        Me.chkFri = New System.Windows.Forms.CheckBox()
        Me.chkSat = New System.Windows.Forms.CheckBox()
        Me.lblStart = New System.Windows.Forms.Label()
        Me.dtpStart = New System.Windows.Forms.DateTimePicker()
        Me.lblEnd = New System.Windows.Forms.Label()
        Me.dtpEnd = New System.Windows.Forms.DateTimePicker()
        Me.lblRoom = New System.Windows.Forms.Label()
        Me.txtRoom = New Global.CSUAttendance.TextField()
        Me.capHours = New System.Windows.Forms.Label()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.chkOverride = New System.Windows.Forms.CheckBox()
        Me.lblHours = New System.Windows.Forms.Label()
        Me.numHours = New System.Windows.Forms.NumericUpDown()
        Me.lblHoursNote = New System.Windows.Forms.Label()
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
        Me.lblTitle.Text = "Add class"
        '
        'lblTerm
        '
        Me.lblTerm.AutoSize = True
        Me.lblTerm.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblTerm.Location = New System.Drawing.Point(236, 76)
        Me.lblTerm.Name = "lblTerm"
        Me.lblTerm.Size = New System.Drawing.Size(80, 15)
        Me.lblTerm.TabIndex = 101
        Me.lblTerm.Text = "School term"
        '
        'cboTerm
        '
        Me.cboTerm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTerm.FormattingEnabled = True
        Me.cboTerm.Location = New System.Drawing.Point(238, 96)
        Me.cboTerm.Name = "cboTerm"
        Me.cboTerm.Size = New System.Drawing.Size(240, 25)
        Me.cboTerm.TabIndex = 0
        '
        'lblSubject
        '
        Me.lblSubject.AutoSize = True
        Me.lblSubject.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblSubject.Location = New System.Drawing.Point(488, 76)
        Me.lblSubject.Name = "lblSubject"
        Me.lblSubject.Size = New System.Drawing.Size(80, 15)
        Me.lblSubject.TabIndex = 102
        Me.lblSubject.Text = "Subject"
        '
        'cboSubject
        '
        Me.cboSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSubject.FormattingEnabled = True
        Me.cboSubject.Location = New System.Drawing.Point(490, 96)
        Me.cboSubject.Name = "cboSubject"
        Me.cboSubject.Size = New System.Drawing.Size(248, 25)
        Me.cboSubject.TabIndex = 1
        '
        'lblSection
        '
        Me.lblSection.AutoSize = True
        Me.lblSection.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblSection.Location = New System.Drawing.Point(236, 146)
        Me.lblSection.Name = "lblSection"
        Me.lblSection.Size = New System.Drawing.Size(80, 15)
        Me.lblSection.TabIndex = 103
        Me.lblSection.Text = "Section"
        '
        'cboSection
        '
        Me.cboSection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSection.FormattingEnabled = True
        Me.cboSection.Location = New System.Drawing.Point(238, 166)
        Me.cboSection.Name = "cboSection"
        Me.cboSection.Size = New System.Drawing.Size(240, 25)
        Me.cboSection.TabIndex = 2
        '
        'lblTeacher
        '
        Me.lblTeacher.AutoSize = True
        Me.lblTeacher.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblTeacher.Location = New System.Drawing.Point(488, 146)
        Me.lblTeacher.Name = "lblTeacher"
        Me.lblTeacher.Size = New System.Drawing.Size(80, 15)
        Me.lblTeacher.TabIndex = 104
        Me.lblTeacher.Text = "Teacher"
        '
        'cboTeacher
        '
        Me.cboTeacher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTeacher.FormattingEnabled = True
        Me.cboTeacher.Location = New System.Drawing.Point(490, 166)
        Me.cboTeacher.Name = "cboTeacher"
        Me.cboTeacher.Size = New System.Drawing.Size(248, 25)
        Me.cboTeacher.TabIndex = 3
        '
        'capDays
        '
        Me.capDays.AutoSize = True
        Me.capDays.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.capDays.Location = New System.Drawing.Point(236, 216)
        Me.capDays.Name = "capDays"
        Me.capDays.Size = New System.Drawing.Size(200, 15)
        Me.capDays.TabIndex = 105
        Me.capDays.Text = "Meeting days"
        '
        'chkMon
        '
        Me.chkMon.AutoSize = True
        Me.chkMon.Location = New System.Drawing.Point(238, 244)
        Me.chkMon.Name = "chkMon"
        Me.chkMon.Size = New System.Drawing.Size(70, 21)
        Me.chkMon.TabIndex = 4
        Me.chkMon.Text = "Mon"
        Me.chkMon.UseVisualStyleBackColor = True
        '
        'chkTue
        '
        Me.chkTue.AutoSize = True
        Me.chkTue.Location = New System.Drawing.Point(320, 244)
        Me.chkTue.Name = "chkTue"
        Me.chkTue.Size = New System.Drawing.Size(70, 21)
        Me.chkTue.TabIndex = 5
        Me.chkTue.Text = "Tue"
        Me.chkTue.UseVisualStyleBackColor = True
        '
        'chkWed
        '
        Me.chkWed.AutoSize = True
        Me.chkWed.Location = New System.Drawing.Point(402, 244)
        Me.chkWed.Name = "chkWed"
        Me.chkWed.Size = New System.Drawing.Size(70, 21)
        Me.chkWed.TabIndex = 6
        Me.chkWed.Text = "Wed"
        Me.chkWed.UseVisualStyleBackColor = True
        '
        'chkThu
        '
        Me.chkThu.AutoSize = True
        Me.chkThu.Location = New System.Drawing.Point(484, 244)
        Me.chkThu.Name = "chkThu"
        Me.chkThu.Size = New System.Drawing.Size(70, 21)
        Me.chkThu.TabIndex = 7
        Me.chkThu.Text = "Thu"
        Me.chkThu.UseVisualStyleBackColor = True
        '
        'chkFri
        '
        Me.chkFri.AutoSize = True
        Me.chkFri.Location = New System.Drawing.Point(566, 244)
        Me.chkFri.Name = "chkFri"
        Me.chkFri.Size = New System.Drawing.Size(70, 21)
        Me.chkFri.TabIndex = 8
        Me.chkFri.Text = "Fri"
        Me.chkFri.UseVisualStyleBackColor = True
        '
        'chkSat
        '
        Me.chkSat.AutoSize = True
        Me.chkSat.Location = New System.Drawing.Point(648, 244)
        Me.chkSat.Name = "chkSat"
        Me.chkSat.Size = New System.Drawing.Size(70, 21)
        Me.chkSat.TabIndex = 9
        Me.chkSat.Text = "Sat"
        Me.chkSat.UseVisualStyleBackColor = True
        '
        'lblStart
        '
        Me.lblStart.AutoSize = True
        Me.lblStart.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblStart.Location = New System.Drawing.Point(236, 286)
        Me.lblStart.Name = "lblStart"
        Me.lblStart.Size = New System.Drawing.Size(80, 15)
        Me.lblStart.TabIndex = 106
        Me.lblStart.Text = "Starts"
        '
        'dtpStart
        '
        Me.dtpStart.CustomFormat = "h:mm tt"
        Me.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpStart.ShowUpDown = True
        Me.dtpStart.Location = New System.Drawing.Point(238, 311)
        Me.dtpStart.Name = "dtpStart"
        Me.dtpStart.Size = New System.Drawing.Size(150, 25)
        Me.dtpStart.TabIndex = 10
        '
        'lblEnd
        '
        Me.lblEnd.AutoSize = True
        Me.lblEnd.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblEnd.Location = New System.Drawing.Point(398, 286)
        Me.lblEnd.Name = "lblEnd"
        Me.lblEnd.Size = New System.Drawing.Size(80, 15)
        Me.lblEnd.TabIndex = 107
        Me.lblEnd.Text = "Ends"
        '
        'dtpEnd
        '
        Me.dtpEnd.CustomFormat = "h:mm tt"
        Me.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpEnd.ShowUpDown = True
        Me.dtpEnd.Location = New System.Drawing.Point(400, 311)
        Me.dtpEnd.Name = "dtpEnd"
        Me.dtpEnd.Size = New System.Drawing.Size(150, 25)
        Me.dtpEnd.TabIndex = 11
        '
        'lblRoom
        '
        Me.lblRoom.AutoSize = True
        Me.lblRoom.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblRoom.Location = New System.Drawing.Point(560, 286)
        Me.lblRoom.Name = "lblRoom"
        Me.lblRoom.Size = New System.Drawing.Size(80, 15)
        Me.lblRoom.TabIndex = 108
        Me.lblRoom.Text = "Room"
        '
        'txtRoom
        '
        Me.txtRoom.Location = New System.Drawing.Point(562, 306)
        Me.txtRoom.Name = "txtRoom"
        Me.txtRoom.Placeholder = "e.g. CBEA 201"
        Me.txtRoom.Size = New System.Drawing.Size(176, 36)
        Me.txtRoom.TabIndex = 12
        '
        'capHours
        '
        Me.capHours.AutoSize = True
        Me.capHours.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.capHours.Location = New System.Drawing.Point(236, 356)
        Me.capHours.Name = "capHours"
        Me.capHours.Size = New System.Drawing.Size(260, 15)
        Me.capHours.TabIndex = 109
        Me.capHours.Text = "Total class hours this term"
        '
        'lblTotal
        '
        Me.lblTotal.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblTotal.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblTotal.Location = New System.Drawing.Point(236, 377)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(300, 56)
        Me.lblTotal.TabIndex = 110
        Me.lblTotal.Text = "-"
        '
        'chkOverride
        '
        Me.chkOverride.AutoSize = True
        Me.chkOverride.Location = New System.Drawing.Point(562, 384)
        Me.chkOverride.Name = "chkOverride"
        Me.chkOverride.Size = New System.Drawing.Size(176, 21)
        Me.chkOverride.TabIndex = 13
        Me.chkOverride.Text = "Type a different total"
        Me.chkOverride.UseVisualStyleBackColor = True
        '
        'lblHours
        '
        Me.lblHours.AutoSize = True
        Me.lblHours.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblHours.Location = New System.Drawing.Point(560, 426)
        Me.lblHours.Name = "lblHours"
        Me.lblHours.Size = New System.Drawing.Size(80, 15)
        Me.lblHours.TabIndex = 111
        Me.lblHours.Text = "Custom total (hours)"
        '
        'numHours
        '
        Me.numHours.DecimalPlaces = 2
        Me.numHours.Font = New System.Drawing.Font("Segoe UI", 10.5!)
        Me.numHours.Location = New System.Drawing.Point(562, 451)
        Me.numHours.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.numHours.Minimum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.numHours.Name = "numHours"
        Me.numHours.Size = New System.Drawing.Size(120, 26)
        Me.numHours.TabIndex = 14
        Me.numHours.Value = New Decimal(New Integer() {0, 0, 0, 0})
        '
        'lblHoursNote
        '
        Me.lblHoursNote.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblHoursNote.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblHoursNote.Location = New System.Drawing.Point(236, 433)
        Me.lblHoursNote.Name = "lblHoursNote"
        Me.lblHoursNote.Size = New System.Drawing.Size(300, 56)
        Me.lblHoursNote.TabIndex = 112
        Me.lblHoursNote.Text = "Computed from the term dates and the meeting days."
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(626, 550)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(112, 36)
        Me.btnSave.TabIndex = 15
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(518, 550)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 36)
        Me.btnCancel.TabIndex = 16
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'errProvider
        '
        Me.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.errProvider.ContainerControl = Me
        '
        'ClassDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(768, 614)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.lblHoursNote)
        Me.Controls.Add(Me.numHours)
        Me.Controls.Add(Me.lblHours)
        Me.Controls.Add(Me.chkOverride)
        Me.Controls.Add(Me.lblTotal)
        Me.Controls.Add(Me.capHours)
        Me.Controls.Add(Me.txtRoom)
        Me.Controls.Add(Me.lblRoom)
        Me.Controls.Add(Me.dtpEnd)
        Me.Controls.Add(Me.lblEnd)
        Me.Controls.Add(Me.dtpStart)
        Me.Controls.Add(Me.lblStart)
        Me.Controls.Add(Me.chkSat)
        Me.Controls.Add(Me.chkFri)
        Me.Controls.Add(Me.chkThu)
        Me.Controls.Add(Me.chkWed)
        Me.Controls.Add(Me.chkTue)
        Me.Controls.Add(Me.chkMon)
        Me.Controls.Add(Me.capDays)
        Me.Controls.Add(Me.cboTeacher)
        Me.Controls.Add(Me.lblTeacher)
        Me.Controls.Add(Me.cboSection)
        Me.Controls.Add(Me.lblSection)
        Me.Controls.Add(Me.cboSubject)
        Me.Controls.Add(Me.lblSubject)
        Me.Controls.Add(Me.cboTerm)
        Me.Controls.Add(Me.lblTerm)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ClassDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add class"
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblTerm As System.Windows.Forms.Label
    Friend WithEvents cboTerm As System.Windows.Forms.ComboBox
    Friend WithEvents lblSubject As System.Windows.Forms.Label
    Friend WithEvents cboSubject As System.Windows.Forms.ComboBox
    Friend WithEvents lblSection As System.Windows.Forms.Label
    Friend WithEvents cboSection As System.Windows.Forms.ComboBox
    Friend WithEvents lblTeacher As System.Windows.Forms.Label
    Friend WithEvents cboTeacher As System.Windows.Forms.ComboBox
    Friend WithEvents capDays As System.Windows.Forms.Label
    Friend WithEvents chkMon As System.Windows.Forms.CheckBox
    Friend WithEvents chkTue As System.Windows.Forms.CheckBox
    Friend WithEvents chkWed As System.Windows.Forms.CheckBox
    Friend WithEvents chkThu As System.Windows.Forms.CheckBox
    Friend WithEvents chkFri As System.Windows.Forms.CheckBox
    Friend WithEvents chkSat As System.Windows.Forms.CheckBox
    Friend WithEvents lblStart As System.Windows.Forms.Label
    Friend WithEvents dtpStart As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblEnd As System.Windows.Forms.Label
    Friend WithEvents dtpEnd As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblRoom As System.Windows.Forms.Label
    Friend WithEvents txtRoom As Global.CSUAttendance.TextField
    Friend WithEvents capHours As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents chkOverride As System.Windows.Forms.CheckBox
    Friend WithEvents lblHours As System.Windows.Forms.Label
    Friend WithEvents numHours As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblHoursNote As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
