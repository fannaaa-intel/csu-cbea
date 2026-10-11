Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>Foreground/background pair for badges and status chips.</summary>
Public Structure BadgeColors
    Public Fore As Color
    Public Back As Color
    Public Sub New(fore As Color, back As Color)
        Me.Fore = fore
        Me.Back = back
    End Sub
End Structure

''' <summary>
''' Design tokens + styling helpers. Use these everywhere instead of hard-coded colors/fonts.
''' Minimal look: white cards on light gray, one rose accent, 8px grid, Segoe UI.
''' </summary>
Public Module Theme

    ' ---------- Colors ----------
    Public ReadOnly Background As Color = ColorTranslator.FromHtml("#F7F7F8")
    Public ReadOnly Surface As Color = Color.White
    Public ReadOnly Border As Color = ColorTranslator.FromHtml("#E6E6EA")
    Public ReadOnly TextPrimary As Color = ColorTranslator.FromHtml("#1F2328")
    Public ReadOnly TextMuted As Color = ColorTranslator.FromHtml("#6B7280")
    Public ReadOnly Accent As Color = ColorTranslator.FromHtml("#D94F70")
    Public ReadOnly AccentHover As Color = ColorTranslator.FromHtml("#C23F60")
    Public ReadOnly AccentSoft As Color = ColorTranslator.FromHtml("#FCE8EC")
    Public ReadOnly RowAlt As Color = ColorTranslator.FromHtml("#FAFAFB")
    Public ReadOnly Hover As Color = ColorTranslator.FromHtml("#F3F4F6")

    Public ReadOnly Success As Color = ColorTranslator.FromHtml("#16A34A")
    Public ReadOnly SuccessSoft As Color = ColorTranslator.FromHtml("#DCFCE7")
    Public ReadOnly Warning As Color = ColorTranslator.FromHtml("#D97706")
    Public ReadOnly WarningSoft As Color = ColorTranslator.FromHtml("#FEF3C7")
    Public ReadOnly Orange As Color = ColorTranslator.FromHtml("#EA580C")
    Public ReadOnly OrangeSoft As Color = ColorTranslator.FromHtml("#FFEDD5")
    Public ReadOnly Danger As Color = ColorTranslator.FromHtml("#DC2626")
    Public ReadOnly DangerSoft As Color = ColorTranslator.FromHtml("#FEE2E2")
    Public ReadOnly Info As Color = ColorTranslator.FromHtml("#2563EB")
    Public ReadOnly InfoSoft As Color = ColorTranslator.FromHtml("#DBEAFE")
    Public ReadOnly Neutral As Color = ColorTranslator.FromHtml("#4B5563")
    Public ReadOnly NeutralSoft As Color = ColorTranslator.FromHtml("#F3F4F6")

    ' ---------- Fonts ----------
    Public ReadOnly FontBase As New Font("Segoe UI", 10.0F)
    Public ReadOnly FontSmall As New Font("Segoe UI", 9.0F)
    Public ReadOnly FontBold As New Font("Segoe UI Semibold", 10.0F)
    Public ReadOnly FontSubtitle As New Font("Segoe UI Semibold", 12.0F)
    Public ReadOnly FontTitle As New Font("Segoe UI Semibold", 18.0F)
    Public ReadOnly FontStat As New Font("Segoe UI Semibold", 22.0F)
    Public ReadOnly FontIcon As New Font("Segoe MDL2 Assets", 12.0F)
    Public ReadOnly FontIconLarge As New Font("Segoe MDL2 Assets", 16.0F)

    ' ---------- Sizes (8px grid) ----------
    Public Const ControlHeight As Integer = 36
    Public Const RowHeight As Integer = 34
    Public Const Radius As Integer = 8

    ''' <summary>Colors for any status text used in the app.</summary>
    Public Function StatusColors(status As String) As BadgeColors
        Select Case status
            Case "Present", "Good", "Active", "Complied", "Success"
                Return New BadgeColors(Success, SuccessSoft)
            Case "Late", "Warning 1", "Pending", "Warning", "Tardiness"
                Return New BadgeColors(Warning, WarningSoft)
            Case "Warning 2"
                Return New BadgeColors(Orange, OrangeSoft)
            Case "Absent", "Dropped", "Inactive", "Danger"
                Return New BadgeColors(Danger, DangerSoft)
            Case "Excused", "Info"
                Return New BadgeColors(Info, InfoSoft)
            Case Else
                Return New BadgeColors(Neutral, NeutralSoft)
        End Select
    End Function

    ' ---------- Buttons ----------
    Public Sub StylePrimary(btn As Button, Optional glyph As String = Nothing)
        StyleButtonBase(btn, Accent, Color.White, Accent, glyph)
        btn.FlatAppearance.MouseOverBackColor = AccentHover
        btn.FlatAppearance.MouseDownBackColor = AccentHover
    End Sub

    Public Sub StyleSecondary(btn As Button, Optional glyph As String = Nothing)
        StyleButtonBase(btn, Surface, TextPrimary, Border, glyph)
        btn.FlatAppearance.MouseOverBackColor = Hover
        btn.FlatAppearance.MouseDownBackColor = Border
    End Sub

    Public Sub StyleDanger(btn As Button, Optional glyph As String = Nothing)
        StyleButtonBase(btn, Surface, Danger, Border, glyph)
        btn.FlatAppearance.MouseOverBackColor = DangerSoft
        btn.FlatAppearance.MouseDownBackColor = DangerSoft
    End Sub

    ''' <summary>Text-only button (no border), e.g. "Cancel", "Show password".</summary>
    Public Sub StyleGhost(btn As Button, Optional glyph As String = Nothing)
        StyleButtonBase(btn, Surface, TextMuted, Surface, glyph)
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = Hover
        btn.FlatAppearance.MouseDownBackColor = Border
    End Sub

    Private Sub StyleButtonBase(btn As Button, back As Color, fore As Color, borderColor As Color, glyph As String)
        btn.FlatStyle = FlatStyle.Flat
        btn.UseVisualStyleBackColor = False
        btn.BackColor = back
        btn.ForeColor = fore
        btn.FlatAppearance.BorderColor = borderColor
        btn.FlatAppearance.BorderSize = 1
        btn.Font = FontBold
        btn.Cursor = Cursors.Hand
        btn.Height = ControlHeight
        If Not String.IsNullOrEmpty(glyph) Then
            btn.Image = GlyphImage(glyph, fore, 14)
            btn.ImageAlign = ContentAlignment.MiddleLeft
            btn.TextImageRelation = TextImageRelation.ImageBeforeText
            btn.Padding = New Padding(10, 0, 10, 0)
        End If
    End Sub

    ' ---------- Inputs ----------
    Public Sub StyleCombo(cbo As ComboBox)
        cbo.FlatStyle = FlatStyle.Flat
        cbo.Font = FontBase
        cbo.BackColor = Surface
        cbo.ForeColor = TextPrimary
    End Sub

    Public Sub StyleLabelMuted(lbl As Label)
        lbl.Font = FontSmall
        lbl.ForeColor = TextMuted
    End Sub

    ' ---------- Grids ----------
    ''' <summary>Flat, borderless, read-only grid with 34px rows and soft-pink selection.</summary>
    Public Sub StyleGrid(dgv As DataGridView)
        dgv.BorderStyle = BorderStyle.None
        dgv.BackgroundColor = Surface
        dgv.GridColor = Border
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgv.RowHeadersVisible = False
        dgv.EnableHeadersVisualStyles = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.AllowUserToResizeRows = False
        dgv.ReadOnly = True
        dgv.MultiSelect = False
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgv.ColumnHeadersHeight = 40
        dgv.RowTemplate.Height = RowHeight
        dgv.Font = FontBase

        dgv.ColumnHeadersDefaultCellStyle.BackColor = Surface
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted
        dgv.ColumnHeadersDefaultCellStyle.Font = FontBold
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Surface
        dgv.ColumnHeadersDefaultCellStyle.Padding = New Padding(8, 0, 8, 0)

        dgv.DefaultCellStyle.BackColor = Surface
        dgv.DefaultCellStyle.ForeColor = TextPrimary
        dgv.DefaultCellStyle.SelectionBackColor = AccentSoft
        dgv.DefaultCellStyle.SelectionForeColor = TextPrimary
        dgv.DefaultCellStyle.Padding = New Padding(8, 0, 8, 0)
        dgv.AlternatingRowsDefaultCellStyle.BackColor = RowAlt

        EnableDoubleBuffer(dgv)
    End Sub

    ''' <summary>Paints a status badge inside a grid cell. Hook from CellPainting for status columns.</summary>
    Public Sub PaintBadgeCell(e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.Value Is Nothing OrElse TypeOf e.Value Is DBNull Then Return
        e.PaintBackground(e.CellBounds, (e.State And DataGridViewElementStates.Selected) <> 0)
        Dim text = Convert.ToString(e.Value)
        Dim colors = StatusColors(text)
        Dim sz = TextRenderer.MeasureText(text, FontSmall)
        Dim rect As New Rectangle(e.CellBounds.X + 8, e.CellBounds.Y + (e.CellBounds.Height - 22) \ 2, sz.Width + 14, 22)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using path = RoundedRect(rect, 11), br As New SolidBrush(colors.Back)
            e.Graphics.FillPath(br, path)
        End Using
        TextRenderer.DrawText(e.Graphics, text, FontSmall, rect, colors.Fore, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        e.Handled = True
    End Sub

    ' ---------- Drawing helpers ----------
    Public Function RoundedRect(r As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d = Math.Max(1, Math.Min(radius * 2, Math.Min(r.Width, r.Height)))
        path.AddArc(r.X, r.Y, d, d, 180, 90)
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90)
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    ''' <summary>Renders a Segoe MDL2 Assets glyph to a bitmap (for Button.Image / PictureBox).</summary>
    Public Function GlyphImage(glyph As String, color As Color, size As Integer) As Bitmap
        Dim bmp As New Bitmap(size + 4, size + 4)
        Using g = Graphics.FromImage(bmp), f As New Font("Segoe MDL2 Assets", size * 0.75F, GraphicsUnit.Point)
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            TextRenderer.DrawText(g, glyph, f, New Rectangle(0, 0, bmp.Width, bmp.Height), color,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
        End Using
        Return bmp
    End Function

    ''' <summary>Turns on double buffering for any control (stops flicker on grids/panels).</summary>
    Public Sub EnableDoubleBuffer(c As Control)
        Dim prop = GetType(Control).GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
        prop?.SetValue(c, True, Nothing)
    End Sub

End Module

''' <summary>Segoe MDL2 Assets glyphs (Windows 10/11). Use with Theme.FontIcon or Theme.GlyphImage.</summary>
Public Module Icons
    Public Const Dashboard As String = ChrW(&HE80F)
    Public Const Students As String = ChrW(&HE716)
    Public Const Teachers As String = ChrW(&HE77B)
    Public Const Users As String = ChrW(&HE8D7)
    Public Const Subjects As String = ChrW(&HE8F1)
    Public Const Sections As String = ChrW(&HE71D)
    Public Const Terms As String = ChrW(&HE787)
    Public Const Classes As String = ChrW(&HE7BE)
    Public Const Enrollment As String = ChrW(&HE8FA)
    Public Const TakeAttendance As String = ChrW(&HE8FB)
    Public Const Records As String = ChrW(&HE81C)
    Public Const Reports As String = ChrW(&HE9F9)
    Public Const Settings As String = ChrW(&HE713)
    Public Const AuditLog As String = ChrW(&HEA18)
    Public Const Bell As String = ChrW(&HEA8F)
    Public Const Logout As String = ChrW(&HE7E8)
    Public Const Lock As String = ChrW(&HE72E)
    Public Const Person As String = ChrW(&HE77B)
    Public Const Search As String = ChrW(&HE721)
    Public Const Add As String = ChrW(&HE710)
    Public Const Edit As String = ChrW(&HE70F)
    Public Const Delete As String = ChrW(&HE74D)
    Public Const Save As String = ChrW(&HE74E)
    Public Const Print As String = ChrW(&HE749)
    Public Const Export As String = ChrW(&HE896)
    Public Const WarningSign As String = ChrW(&HE7BA)
    Public Const Check As String = ChrW(&HE73E)
    Public Const Close As String = ChrW(&HE711)
    Public Const View As String = ChrW(&HE890)
    Public Const Refresh As String = ChrW(&HE72C)
    Public Const Clock As String = ChrW(&HE917)
End Module
