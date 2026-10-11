<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.pnlTopbar = New System.Windows.Forms.Panel()
        Me.lblBellCount = New System.Windows.Forms.Label()
        Me.btnBell = New System.Windows.Forms.Button()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.lblPageTitle = New System.Windows.Forms.Label()
        Me.pnlSidebar = New System.Windows.Forms.Panel()
        Me.flpNav = New System.Windows.Forms.FlowLayoutPanel()
        Me.pnlUser = New System.Windows.Forms.Panel()
        Me.btnLogout = New System.Windows.Forms.Button()
        Me.lblUserRole = New System.Windows.Forms.Label()
        Me.lblUserName = New System.Windows.Forms.Label()
        Me.lblAvatar = New System.Windows.Forms.Label()
        Me.pnlBrand = New System.Windows.Forms.Panel()
        Me.lblAppSub = New System.Windows.Forms.Label()
        Me.lblAppName = New System.Windows.Forms.Label()
        Me.lblLogo = New System.Windows.Forms.Label()
        Me.pnlTopbar.SuspendLayout()
        Me.pnlSidebar.SuspendLayout()
        Me.pnlUser.SuspendLayout()
        Me.pnlBrand.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlContent
        '
        Me.pnlContent.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(248, Byte), Integer))
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Location = New System.Drawing.Point(240, 64)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Padding = New System.Windows.Forms.Padding(24)
        Me.pnlContent.Size = New System.Drawing.Size(1040, 696)
        Me.pnlContent.TabIndex = 0
        '
        'pnlTopbar
        '
        Me.pnlTopbar.BackColor = System.Drawing.Color.White
        Me.pnlTopbar.Controls.Add(Me.lblBellCount)
        Me.pnlTopbar.Controls.Add(Me.btnBell)
        Me.pnlTopbar.Controls.Add(Me.lblDate)
        Me.pnlTopbar.Controls.Add(Me.lblPageTitle)
        Me.pnlTopbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTopbar.Location = New System.Drawing.Point(240, 0)
        Me.pnlTopbar.Name = "pnlTopbar"
        Me.pnlTopbar.Size = New System.Drawing.Size(1040, 64)
        Me.pnlTopbar.TabIndex = 1
        '
        'lblBellCount
        '
        Me.lblBellCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblBellCount.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(79, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.lblBellCount.Font = New System.Drawing.Font("Segoe UI Semibold", 7.5!)
        Me.lblBellCount.ForeColor = System.Drawing.Color.White
        Me.lblBellCount.Location = New System.Drawing.Point(1002, 12)
        Me.lblBellCount.Name = "lblBellCount"
        Me.lblBellCount.Size = New System.Drawing.Size(22, 16)
        Me.lblBellCount.TabIndex = 3
        Me.lblBellCount.Text = "0"
        Me.lblBellCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.lblBellCount.Visible = False
        '
        'btnBell
        '
        Me.btnBell.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBell.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnBell.FlatAppearance.BorderSize = 0
        Me.btnBell.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(243, Byte), Integer), CType(CType(244, Byte), Integer), CType(CType(246, Byte), Integer))
        Me.btnBell.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBell.Font = New System.Drawing.Font("Segoe MDL2 Assets", 13.0!)
        Me.btnBell.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.btnBell.Location = New System.Drawing.Point(976, 12)
        Me.btnBell.Name = "btnBell"
        Me.btnBell.Size = New System.Drawing.Size(40, 40)
        Me.btnBell.TabIndex = 2
        Me.btnBell.UseVisualStyleBackColor = False
        '
        'lblDate
        '
        Me.lblDate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDate.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblDate.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblDate.Location = New System.Drawing.Point(700, 22)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(264, 20)
        Me.lblDate.TabIndex = 1
        Me.lblDate.Text = "Date"
        Me.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPageTitle
        '
        Me.lblPageTitle.AutoSize = True
        Me.lblPageTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 15.0!)
        Me.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblPageTitle.Location = New System.Drawing.Point(22, 16)
        Me.lblPageTitle.Name = "lblPageTitle"
        Me.lblPageTitle.Size = New System.Drawing.Size(115, 28)
        Me.lblPageTitle.TabIndex = 0
        Me.lblPageTitle.Text = "Dashboard"
        '
        'pnlSidebar
        '
        Me.pnlSidebar.BackColor = System.Drawing.Color.White
        Me.pnlSidebar.Controls.Add(Me.flpNav)
        Me.pnlSidebar.Controls.Add(Me.pnlUser)
        Me.pnlSidebar.Controls.Add(Me.pnlBrand)
        Me.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlSidebar.Location = New System.Drawing.Point(0, 0)
        Me.pnlSidebar.Name = "pnlSidebar"
        Me.pnlSidebar.Size = New System.Drawing.Size(240, 760)
        Me.pnlSidebar.TabIndex = 2
        '
        'flpNav
        '
        Me.flpNav.AutoScroll = True
        Me.flpNav.BackColor = System.Drawing.Color.White
        Me.flpNav.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flpNav.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flpNav.Location = New System.Drawing.Point(0, 76)
        Me.flpNav.Name = "flpNav"
        Me.flpNav.Padding = New System.Windows.Forms.Padding(14, 8, 14, 8)
        Me.flpNav.Size = New System.Drawing.Size(240, 608)
        Me.flpNav.TabIndex = 0
        Me.flpNav.WrapContents = False
        '
        'pnlUser
        '
        Me.pnlUser.BackColor = System.Drawing.Color.White
        Me.pnlUser.Controls.Add(Me.btnLogout)
        Me.pnlUser.Controls.Add(Me.lblUserRole)
        Me.pnlUser.Controls.Add(Me.lblUserName)
        Me.pnlUser.Controls.Add(Me.lblAvatar)
        Me.pnlUser.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlUser.Location = New System.Drawing.Point(0, 684)
        Me.pnlUser.Name = "pnlUser"
        Me.pnlUser.Size = New System.Drawing.Size(240, 76)
        Me.pnlUser.TabIndex = 1
        '
        'btnLogout
        '
        Me.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnLogout.FlatAppearance.BorderSize = 0
        Me.btnLogout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(CType(CType(254, Byte), Integer), CType(CType(226, Byte), Integer), CType(CType(226, Byte), Integer))
        Me.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogout.Font = New System.Drawing.Font("Segoe MDL2 Assets", 11.0!)
        Me.btnLogout.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.btnLogout.Location = New System.Drawing.Point(190, 20)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(36, 36)
        Me.btnLogout.TabIndex = 3
        Me.btnLogout.UseVisualStyleBackColor = False
        '
        'lblUserRole
        '
        Me.lblUserRole.AutoEllipsis = True
        Me.lblUserRole.Font = New System.Drawing.Font("Segoe UI", 8.5!)
        Me.lblUserRole.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblUserRole.Location = New System.Drawing.Point(62, 40)
        Me.lblUserRole.Name = "lblUserRole"
        Me.lblUserRole.Size = New System.Drawing.Size(124, 18)
        Me.lblUserRole.TabIndex = 2
        Me.lblUserRole.Text = "Role"
        '
        'lblUserName
        '
        Me.lblUserName.AutoEllipsis = True
        Me.lblUserName.Font = New System.Drawing.Font("Segoe UI Semibold", 9.5!)
        Me.lblUserName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblUserName.Location = New System.Drawing.Point(62, 20)
        Me.lblUserName.Name = "lblUserName"
        Me.lblUserName.Size = New System.Drawing.Size(124, 20)
        Me.lblUserName.TabIndex = 1
        Me.lblUserName.Text = "User Name"
        '
        'lblAvatar
        '
        Me.lblAvatar.BackColor = System.Drawing.Color.FromArgb(CType(CType(252, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(236, Byte), Integer))
        Me.lblAvatar.Font = New System.Drawing.Font("Segoe UI Semibold", 10.0!)
        Me.lblAvatar.ForeColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(79, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.lblAvatar.Location = New System.Drawing.Point(16, 20)
        Me.lblAvatar.Name = "lblAvatar"
        Me.lblAvatar.Size = New System.Drawing.Size(38, 38)
        Me.lblAvatar.TabIndex = 0
        Me.lblAvatar.Text = "AB"
        Me.lblAvatar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlBrand
        '
        Me.pnlBrand.BackColor = System.Drawing.Color.White
        Me.pnlBrand.Controls.Add(Me.lblAppSub)
        Me.pnlBrand.Controls.Add(Me.lblAppName)
        Me.pnlBrand.Controls.Add(Me.lblLogo)
        Me.pnlBrand.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlBrand.Location = New System.Drawing.Point(0, 0)
        Me.pnlBrand.Name = "pnlBrand"
        Me.pnlBrand.Size = New System.Drawing.Size(240, 76)
        Me.pnlBrand.TabIndex = 2
        '
        'lblAppSub
        '
        Me.lblAppSub.AutoSize = True
        Me.lblAppSub.Font = New System.Drawing.Font("Segoe UI", 8.5!)
        Me.lblAppSub.ForeColor = System.Drawing.Color.FromArgb(CType(CType(107, Byte), Integer), CType(CType(114, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.lblAppSub.Location = New System.Drawing.Point(63, 40)
        Me.lblAppSub.Name = "lblAppSub"
        Me.lblAppSub.Size = New System.Drawing.Size(115, 15)
        Me.lblAppSub.TabIndex = 2
        Me.lblAppSub.Text = "Attendance Management"
        '
        'lblAppName
        '
        Me.lblAppName.AutoSize = True
        Me.lblAppName.Font = New System.Drawing.Font("Segoe UI Semibold", 11.0!)
        Me.lblAppName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(35, Byte), Integer), CType(CType(40, Byte), Integer))
        Me.lblAppName.Location = New System.Drawing.Point(62, 18)
        Me.lblAppName.Name = "lblAppName"
        Me.lblAppName.Size = New System.Drawing.Size(124, 20)
        Me.lblAppName.TabIndex = 1
        Me.lblAppName.Text = "CSU Attendance"
        '
        'lblLogo
        '
        Me.lblLogo.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(79, Byte), Integer), CType(CType(112, Byte), Integer))
        Me.lblLogo.Font = New System.Drawing.Font("Segoe MDL2 Assets", 14.0!)
        Me.lblLogo.ForeColor = System.Drawing.Color.White
        Me.lblLogo.Location = New System.Drawing.Point(16, 18)
        Me.lblLogo.Name = "lblLogo"
        Me.lblLogo.Size = New System.Drawing.Size(38, 38)
        Me.lblLogo.TabIndex = 0
        Me.lblLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(247, Byte), Integer), CType(CType(247, Byte), Integer), CType(CType(248, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1280, 760)
        Me.Controls.Add(Me.pnlContent)
        Me.Controls.Add(Me.pnlTopbar)
        Me.Controls.Add(Me.pnlSidebar)
        Me.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.MinimumSize = New System.Drawing.Size(1200, 720)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "CSU Student Attendance Management System"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlTopbar.ResumeLayout(False)
        Me.pnlTopbar.PerformLayout()
        Me.pnlSidebar.ResumeLayout(False)
        Me.pnlUser.ResumeLayout(False)
        Me.pnlBrand.ResumeLayout(False)
        Me.pnlBrand.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlContent As System.Windows.Forms.Panel
    Friend WithEvents pnlTopbar As System.Windows.Forms.Panel
    Friend WithEvents lblBellCount As System.Windows.Forms.Label
    Friend WithEvents btnBell As System.Windows.Forms.Button
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents lblPageTitle As System.Windows.Forms.Label
    Friend WithEvents pnlSidebar As System.Windows.Forms.Panel
    Friend WithEvents flpNav As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents pnlUser As System.Windows.Forms.Panel
    Friend WithEvents btnLogout As System.Windows.Forms.Button
    Friend WithEvents lblUserRole As System.Windows.Forms.Label
    Friend WithEvents lblUserName As System.Windows.Forms.Label
    Friend WithEvents lblAvatar As System.Windows.Forms.Label
    Friend WithEvents pnlBrand As System.Windows.Forms.Panel
    Friend WithEvents lblAppSub As System.Windows.Forms.Label
    Friend WithEvents lblAppName As System.Windows.Forms.Label
    Friend WithEvents lblLogo As System.Windows.Forms.Label
End Class
