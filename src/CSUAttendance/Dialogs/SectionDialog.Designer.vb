<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SectionDialog
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
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtName = New Global.CSUAttendance.TextField()
        Me.lblProgram = New System.Windows.Forms.Label()
        Me.txtProgram = New Global.CSUAttendance.TextField()
        Me.lblYear = New System.Windows.Forms.Label()
        Me.numYear = New System.Windows.Forms.NumericUpDown()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.errProvider = New System.Windows.Forms.ErrorProvider(Me.components)
        CType(Me.numYear, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lblTitle.Size = New System.Drawing.Size(122, 28)
        Me.lblTitle.TabIndex = 10
        Me.lblTitle.Text = "Add section"
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblName.Location = New System.Drawing.Point(28, 76)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(83, 15)
        Me.lblName.TabIndex = 11
        Me.lblName.Text = "Section name"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(30, 96)
        Me.txtName.MaxLength = 30
        Me.txtName.Name = "txtName"
        Me.txtName.Placeholder = "e.g. BSAIS 3B"
        Me.txtName.Size = New System.Drawing.Size(400, 36)
        Me.txtName.TabIndex = 0
        '
        'lblProgram
        '
        Me.lblProgram.AutoSize = True
        Me.lblProgram.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblProgram.Location = New System.Drawing.Point(28, 146)
        Me.lblProgram.Name = "lblProgram"
        Me.lblProgram.Size = New System.Drawing.Size(55, 15)
        Me.lblProgram.TabIndex = 12
        Me.lblProgram.Text = "Program"
        '
        'txtProgram
        '
        Me.txtProgram.Location = New System.Drawing.Point(30, 166)
        Me.txtProgram.MaxLength = 20
        Me.txtProgram.Name = "txtProgram"
        Me.txtProgram.Placeholder = "e.g. BSAIS"
        Me.txtProgram.Size = New System.Drawing.Size(240, 36)
        Me.txtProgram.TabIndex = 1
        '
        'lblYear
        '
        Me.lblYear.AutoSize = True
        Me.lblYear.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!)
        Me.lblYear.Location = New System.Drawing.Point(288, 146)
        Me.lblYear.Name = "lblYear"
        Me.lblYear.Size = New System.Drawing.Size(62, 15)
        Me.lblYear.TabIndex = 13
        Me.lblYear.Text = "Year level"
        '
        'numYear
        '
        Me.numYear.Font = New System.Drawing.Font("Segoe UI", 10.5!)
        Me.numYear.Location = New System.Drawing.Point(290, 171)
        Me.numYear.Maximum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.numYear.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numYear.Name = "numYear"
        Me.numYear.Size = New System.Drawing.Size(100, 26)
        Me.numYear.TabIndex = 2
        Me.numYear.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(334, 232)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(96, 36)
        Me.btnSave.TabIndex = 3
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(230, 232)
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
        'SectionDialog
        '
        Me.AcceptButton = Me.btnSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(460, 292)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.numYear)
        Me.Controls.Add(Me.lblYear)
        Me.Controls.Add(Me.txtProgram)
        Me.Controls.Add(Me.lblProgram)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.lblName)
        Me.Controls.Add(Me.lblTitle)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SectionDialog"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Section"
        CType(Me.numYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.errProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents lblName As System.Windows.Forms.Label
    Friend WithEvents txtName As Global.CSUAttendance.TextField
    Friend WithEvents lblProgram As System.Windows.Forms.Label
    Friend WithEvents txtProgram As Global.CSUAttendance.TextField
    Friend WithEvents lblYear As System.Windows.Forms.Label
    Friend WithEvents numYear As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents errProvider As System.Windows.Forms.ErrorProvider
End Class
