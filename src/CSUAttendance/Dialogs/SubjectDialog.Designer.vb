<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SubjectDialog
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
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtCode = New Global.CSUAttendance.TextField()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.txtDescription = New Global.CSUAttendance.TextField()
        Me.lblUnits = New System.Windows.Forms.Label()
        Me.numUnits = New System.Windows.Forms.NumericUpDown()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.errProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.numUnits, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblTitle.Size = New System.Drawing.Size(126, 28)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Add subject"
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblCode.Location = New System.Drawing.Point(238, 76)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(79, 15)
        Me.lblCode.TabIndex = 1
        Me.lblCode.Text = "Subject code"
        '
        'txtCode
        '
        Me.txtCode.Location = New System.Drawing.Point(240, 96)
        Me.txtCode.MaxLength = 20
        Me.txtCode.Name = "txtCode"
        Me.txtCode.Placeholder = "e.g. AIS 311"
        Me.txtCode.Size = New System.Drawing.Size(200, 36)
        Me.txtCode.TabIndex = 0
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblDescription.Location = New System.Drawing.Point(238, 146)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(69, 15)
        Me.lblDescription.TabIndex = 3
        Me.lblDescription.Text = "Description"
        '
        'txtDescription
        '
        Me.txtDescription.Location = New System.Drawing.Point(240, 166)
        Me.txtDescription.MaxLength = 150
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Placeholder = "e.g. Accounting Information Systems"
        Me.txtDescription.Size = New System.Drawing.Size(400, 36)
        Me.txtDescription.TabIndex = 1
        '
        'lblUnits
        '
        Me.lblUnits.AutoSize = True
        Me.lblUnits.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblUnits.Location = New System.Drawing.Point(238, 216)
        Me.lblUnits.Name = "lblUnits"
        Me.lblUnits.Size = New System.Drawing.Size(34, 15)
        Me.lblUnits.TabIndex = 5
        Me.lblUnits.Text = "Units"
        '
        'numUnits
        '
        Me.numUnits.DecimalPlaces = 1
        Me.numUnits.Font = New System.Drawing.Font("Segoe UI", 10.5!)
        Me.numUnits.Increment = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.numUnits.Location = New System.Drawing.Point(240, 236)
        Me.numUnits.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.numUnits.Minimum = New Decimal(New Integer() {5, 0, 0, 65536})
        Me.numUnits.Name = "numUnits"
        Me.numUnits.Size = New System.Drawing.Size(100, 26)
        Me.numUnits.TabIndex = 2
        Me.numUnits.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(544, 292)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(96, 36)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(440, 292)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(96, 36)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'errProvider
        '
        Me.errProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.errProvider.ContainerControl = Me
        '
        'SubjectDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(670, 356)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.numUnits)
        Me.Controls.Add(Me.lblUnits)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.txtCode)
        Me.Controls.Add(Me.lblCode)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SubjectDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Subject"
        CType(Me.numUnits, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblCode As System.Windows.Forms.Label
    Friend WithEvents txtCode As Global.CSUAttendance.TextField
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents txtDescription As Global.CSUAttendance.TextField
    Friend WithEvents lblUnits As System.Windows.Forms.Label
    Friend WithEvents numUnits As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
