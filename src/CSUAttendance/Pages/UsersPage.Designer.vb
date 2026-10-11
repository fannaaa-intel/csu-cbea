<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class UsersPage
    Inherits Global.CSUAttendance.PageBase

    'UserControl overrides dispose to clean up the component list.
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
        Me.pnlToolbar = New System.Windows.Forms.Panel()
        Me.txtSearch = New Global.CSUAttendance.TextField()
        Me.cboRole = New System.Windows.Forms.ComboBox()
        Me.btnToggle = New System.Windows.Forms.Button()
        Me.btnReset = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.lblCount = New System.Windows.Forms.Label()
        Me.cardList = New Global.CSUAttendance.CardPanel()
        Me.lblEmpty = New System.Windows.Forms.Label()
        Me.dgv = New System.Windows.Forms.DataGridView()
        Me.pnlToolbar.SuspendLayout()
        Me.cardList.SuspendLayout()
        CType(Me.dgv, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlToolbar
        '
        Me.pnlToolbar.Controls.Add(Me.lblCount)
        Me.pnlToolbar.Controls.Add(Me.btnAdd)
        Me.pnlToolbar.Controls.Add(Me.btnReset)
        Me.pnlToolbar.Controls.Add(Me.btnToggle)
        Me.pnlToolbar.Controls.Add(Me.cboRole)
        Me.pnlToolbar.Controls.Add(Me.txtSearch)
        Me.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlToolbar.Location = New System.Drawing.Point(0, 0)
        Me.pnlToolbar.Name = "pnlToolbar"
        Me.pnlToolbar.Size = New System.Drawing.Size(960, 52)
        Me.pnlToolbar.TabIndex = 0
        '
        'txtSearch
        '
        Me.txtSearch.Location = New System.Drawing.Point(0, 0)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Placeholder = "Search username or name"
        Me.txtSearch.Size = New System.Drawing.Size(240, 36)
        Me.txtSearch.TabIndex = 0
        '
        'cboRole
        '
        Me.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRole.FormattingEnabled = True
        Me.cboRole.Location = New System.Drawing.Point(252, 0)
        Me.cboRole.Name = "cboRole"
        Me.cboRole.Size = New System.Drawing.Size(150, 25)
        Me.cboRole.TabIndex = 1
        '
        'btnToggle
        '
        Me.btnToggle.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnToggle.Location = New System.Drawing.Point(828, 0)
        Me.btnToggle.Name = "btnToggle"
        Me.btnToggle.Size = New System.Drawing.Size(132, 36)
        Me.btnToggle.TabIndex = 2
        Me.btnToggle.Text = "Deactivate"
        Me.btnToggle.UseVisualStyleBackColor = True
        '
        'btnReset
        '
        Me.btnReset.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReset.Location = New System.Drawing.Point(664, 0)
        Me.btnReset.Name = "btnReset"
        Me.btnReset.Size = New System.Drawing.Size(156, 36)
        Me.btnReset.TabIndex = 3
        Me.btnReset.Text = "Reset password"
        Me.btnReset.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAdd.Location = New System.Drawing.Point(528, 0)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(128, 36)
        Me.btnAdd.TabIndex = 4
        Me.btnAdd.Text = "Add admin"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'lblCount
        '
        Me.lblCount.AutoSize = True
        Me.lblCount.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblCount.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblCount.Location = New System.Drawing.Point(418, 10)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(60, 15)
        Me.lblCount.TabIndex = 5
        Me.lblCount.Text = "0 users"
        '
        'cardList
        '
        Me.cardList.Controls.Add(Me.lblEmpty)
        Me.cardList.Controls.Add(Me.dgv)
        Me.cardList.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cardList.Location = New System.Drawing.Point(0, 52)
        Me.cardList.Name = "cardList"
        Me.cardList.Padding = New System.Windows.Forms.Padding(8)
        Me.cardList.Size = New System.Drawing.Size(960, 548)
        Me.cardList.TabIndex = 1
        '
        'lblEmpty
        '
        Me.lblEmpty.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.lblEmpty.BackColor = System.Drawing.Color.White
        Me.lblEmpty.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblEmpty.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblEmpty.Location = New System.Drawing.Point(230, 240)
        Me.lblEmpty.Name = "lblEmpty"
        Me.lblEmpty.Size = New System.Drawing.Size(500, 60)
        Me.lblEmpty.TabIndex = 1
        Me.lblEmpty.Text = "No user accounts found."
        Me.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblEmpty.Visible = False
        '
        'dgv
        '
        Me.dgv.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgv.Location = New System.Drawing.Point(8, 8)
        Me.dgv.Name = "dgv"
        Me.dgv.Size = New System.Drawing.Size(944, 532)
        Me.dgv.TabIndex = 0
        '
        'UsersPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.Controls.Add(Me.cardList)
        Me.Controls.Add(Me.pnlToolbar)
        Me.Name = "UsersPage"
        Me.Size = New System.Drawing.Size(960, 600)
        Me.pnlToolbar.ResumeLayout(False)
        Me.pnlToolbar.PerformLayout()
        Me.cardList.ResumeLayout(False)
        CType(Me.dgv, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlToolbar As System.Windows.Forms.Panel
    Friend WithEvents txtSearch As Global.CSUAttendance.TextField
    Friend WithEvents cboRole As System.Windows.Forms.ComboBox
    Friend WithEvents btnToggle As System.Windows.Forms.Button
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents lblCount As System.Windows.Forms.Label
    Friend WithEvents cardList As Global.CSUAttendance.CardPanel
    Friend WithEvents lblEmpty As System.Windows.Forms.Label
    Friend WithEvents dgv As System.Windows.Forms.DataGridView
End Class
