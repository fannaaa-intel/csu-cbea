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
''' Pixel sizes set in code must go through Dpi() so they grow with Windows display scaling (125%, 150%).
''' </summary>
Public Module Theme

    ' ---------- Display scaling ----------
    ''' <summary>1.0 at 100% Windows scaling, 1.5 at 150%.</summary>
    Public ReadOnly ScaleFactor As Single = ReadScale()

    Private Function ReadScale() As Single
        Using g = Graphics.FromHwnd(IntPtr.Zero)
            Return g.DpiX / 96.0F
        End Using
    End Function

    ''' <summary>Converts a size designed at 100% into real pixels for the current display scaling.</summary>
    Public Function Dpi(value As Integer) As Integer
        Return CInt(Math.Round(value * ScaleFactor))
    End Function

    ' ---------- Colors ----------
    Public ReadOnly Background As Color = ColorTranslator.FromHtml("#F7F7F8")
    Public ReadOnly Surface As Color = Color.White
    Public ReadOnly Border As Color = ColorTranslator.FromHtml("#E6E6EA")
    Public ReadOnly TextPrimary As Color = ColorTranslator.FromHtml("#1F2328")
    Public ReadOnly TextMuted As Color = ColorTranslator.FromHtml("#6B7280")
    Public ReadOnly Accent As Color = ColorTranslator.FromHtml("#D94F70")
    Public ReadOnly AccentHover As Color = ColorTranslator.FromHtml("#C23F60")
    Public ReadOnly AccentDeep As Color = ColorTranslator.FromHtml("#8E2A45")
    Public ReadOnly AccentSoft As Color = ColorTranslator.FromHtml("#FCE8EC")
    Public ReadOnly Ink As Color = ColorTranslator.FromHtml("#241A20")
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
    Public ReadOnly Violet As Color = ColorTranslator.FromHtml("#7C3AED")
    Public ReadOnly VioletSoft As Color = ColorTranslator.FromHtml("#EDE9FE")
    Public ReadOnly Neutral As Color = ColorTranslator.FromHtml("#4B5563")
    Public ReadOnly NeutralSoft As Color = ColorTranslator.FromHtml("#F3F4F6")

    ' ---------- Fonts (points, so they already follow display scaling) ----------
    Public ReadOnly FontBase As New Font("Segoe UI", 10.0F)
    Public ReadOnly FontSmall As New Font("Segoe UI", 9.0F)
    Public ReadOnly FontSmallBold As New Font("Segoe UI Semibold", 9.0F)
    Public ReadOnly FontBold As New Font("Segoe UI Semibold", 10.0F)
    Public ReadOnly FontSubtitle As New Font("Segoe UI Semibold", 12.0F)
    Public ReadOnly FontHeading As New Font("Segoe UI Semibold", 14.0F)
    Public ReadOnly FontTitle As New Font("Segoe UI Semibold", 18.0F)
    Public ReadOnly FontStat As New Font("Segoe UI Semibold", 22.0F)
    Public ReadOnly FontIcon As New Font("Segoe MDL2 Assets", 12.0F)
    Public ReadOnly FontIconLarge As New Font("Segoe MDL2 Assets", 16.0F)

    ' ---------- Sizes at 100% (wrap with Dpi() when setting in code) ----------
    Public Const ControlHeight As Integer = 38
    Public Const RowHeight As Integer = 40
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
            Case "Excused", "Info", "Teacher"
                Return New BadgeColors(Info, InfoSoft)
            Case "Admin"
                Return New BadgeColors(Violet, VioletSoft)
            Case Else
                Return New BadgeColors(Neutral, NeutralSoft)
        End Select
    End Function

    ''' <summary>The first non-transparent background behind a control (for controls that paint their own corners).</summary>
    Public Function BackOf(c As Control) As Color
        Dim p = c
        While p IsNot Nothing
            If p.BackColor.A = 255 Then Return p.BackColor
            p = p.Parent
        End While
        Return Background
    End Function

    ''' <summary>Mixes a color toward black (amount 0..1). Used for hover/pressed shades.</summary>
    Public Function Darken(c As Color, amount As Single) As Color
        Return Color.FromArgb(c.A, CInt(c.R * (1 - amount)), CInt(c.G * (1 - amount)), CInt(c.B * (1 - amount)))
    End Function

    ' ---------- Buttons ----------
    Public Sub StylePrimary(btn As Button, Optional glyph As String = Nothing)
        StyleButtonBase(btn, Accent, Color.White, Accent, glyph)
        btn.FlatAppearance.MouseOverBackColor = AccentHover
        btn.FlatAppearance.MouseDownBackColor = AccentDeep
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

    ''' <summary>Solid button in any color (e.g. green "Mark complied").</summary>
    Public Sub StyleFilled(btn As Button, back As Color, Optional glyph As String = Nothing)
        StyleButtonBase(btn, back, Color.White, back, glyph)
        btn.FlatAppearance.MouseOverBackColor = Darken(back, 0.1F)
        btn.FlatAppearance.MouseDownBackColor = Darken(back, 0.2F)
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
        btn.UseMnemonic = False
        btn.Height = Dpi(ControlHeight)
        btn.TextAlign = ContentAlignment.MiddleCenter
        If Not String.IsNullOrEmpty(glyph) Then
            btn.Image = GlyphImage(glyph, fore, Dpi(14))
            btn.ImageAlign = ContentAlignment.MiddleCenter          ' icon + text centered together as one group
            btn.TextImageRelation = TextImageRelation.ImageBeforeText
            btn.Padding = New Padding(Dpi(12), 0, Dpi(12), 0)
        End If
        ' Never let the label wrap or get cut ("Delet", "Edi"): grow the button to fit its text
        Dim need = NeededWidth(btn)
        If btn.Width < need Then btn.Width = need
    End Sub

    ''' <summary>Makes a button wide enough for its text (+ icon) so labels are never squeezed.</summary>
    Public Sub FitWidth(btn As Button, Optional minWidth As Integer = 96)
        btn.Width = Math.Max(Dpi(minWidth), NeededWidth(btn))
    End Sub

    Private Function NeededWidth(btn As Button) As Integer
        Return TextRenderer.MeasureText(btn.Text, btn.Font).Width + Dpi(40) + If(btn.Image IsNot Nothing, btn.Image.Width + Dpi(8), 0)
    End Function

    ''' <summary>
    ''' Re-stacks the right-anchored buttons of a toolbar from the right edge with an 8 px gap,
    ''' after they were widened to fit their text (designer widths are only a starting point).
    ''' </summary>
    Public Sub ArrangeToolbar(bar As Control)
        Dim x = bar.ClientSize.Width
        For Each b In bar.Controls.OfType(Of Button)().Where(Function(c) c.Visible AndAlso (c.Anchor And AnchorStyles.Right) <> 0).OrderByDescending(Function(c) c.Left).ToList()
            x -= b.Width
            b.Left = x
            b.Top = 0
            x -= Dpi(8)
        Next
    End Sub

    ' ---------- Inputs ----------
    ''' <summary>Taller owner-drawn combo (matches the 36 px text fields) with roomy, rose-highlighted items.</summary>
    Public Sub StyleCombo(cbo As ComboBox)
        cbo.FlatStyle = FlatStyle.Standard
        cbo.Font = FontBase
        cbo.BackColor = Surface
        cbo.ForeColor = TextPrimary
        cbo.IntegralHeight = False
        cbo.MaxDropDownItems = 12
        If cbo.DrawMode = DrawMode.OwnerDrawFixed Then Return
        cbo.DrawMode = DrawMode.OwnerDrawFixed
        cbo.ItemHeight = Dpi(36) - 6          ' edit box + 6 px frame = 36 px, same as a TextField
        AddHandler cbo.DrawItem, AddressOf Combo_DrawItem
    End Sub

    Private Sub Combo_DrawItem(sender As Object, e As DrawItemEventArgs)
        Dim cbo = DirectCast(sender, ComboBox)
        Dim isEdit = (e.State And DrawItemState.ComboBoxEdit) <> 0
        Dim selected = (e.State And DrawItemState.Selected) <> 0 AndAlso Not isEdit
        Using br As New SolidBrush(If(selected, AccentSoft, If(cbo.Enabled, Surface, NeutralSoft)))
            e.Graphics.FillRectangle(br, e.Bounds)
        End Using
        If e.Index < 0 Then Return
        Dim r = New Rectangle(e.Bounds.X + Dpi(8), e.Bounds.Y, e.Bounds.Width - Dpi(10), e.Bounds.Height)
        TextRenderer.DrawText(e.Graphics, cbo.GetItemText(cbo.Items(e.Index)), cbo.Font, r,
                              If(cbo.Enabled, TextPrimary, TextMuted),
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.Left Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)
    End Sub

    Public Sub StyleLabelMuted(lbl As Label)
        lbl.Font = FontSmall
        lbl.ForeColor = TextMuted
    End Sub

    ' ---------- Grids ----------
    ''' <summary>
    ''' Flat, borderless, read-only grid with roomy rows and soft-pink selection.
    ''' Scrollbars are hidden; the mouse wheel and arrow keys still scroll.
    ''' </summary>
    Public Sub StyleGrid(dgv As DataGridView, Optional multiSelect As Boolean = False)
        dgv.BorderStyle = BorderStyle.None
        dgv.BackgroundColor = Surface
        dgv.GridColor = Border
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.None       ' we draw one clean line under each row ourselves
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
        dgv.RowHeadersVisible = False
        dgv.EnableHeadersVisualStyles = False
        dgv.AllowUserToAddRows = False
        dgv.AllowUserToDeleteRows = False
        dgv.AllowUserToResizeRows = False
        dgv.ReadOnly = True
        dgv.MultiSelect = multiSelect
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        dgv.ColumnHeadersHeight = Dpi(44)
        dgv.RowTemplate.Height = Dpi(RowHeight)
        dgv.Font = FontBase
        dgv.ScrollBars = ScrollBars.None
        dgv.StandardTab = True
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Surface
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted
        dgv.ColumnHeadersDefaultCellStyle.Font = FontSmallBold
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Surface
        dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextMuted
        dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False
        dgv.ColumnHeadersDefaultCellStyle.Padding = New Padding(Dpi(10), 0, Dpi(6), 0)
        dgv.DefaultCellStyle.BackColor = Surface
        dgv.DefaultCellStyle.ForeColor = TextPrimary
        dgv.DefaultCellStyle.SelectionBackColor = AccentSoft
        dgv.DefaultCellStyle.SelectionForeColor = TextPrimary
        dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False
        dgv.DefaultCellStyle.Padding = New Padding(Dpi(10), 0, Dpi(6), 0)
        dgv.AlternatingRowsDefaultCellStyle.BackColor = RowAlt
        EnableDoubleBuffer(dgv)

        ' Only add the scrolling/focus handlers once per grid
        If dgv.Tag Is Nothing OrElse Not dgv.Tag.Equals("styled") Then
            dgv.Tag = "styled"
            AddHandler dgv.MouseWheel, AddressOf Grid_MouseWheel
            AddHandler dgv.SelectionChanged, AddressOf Grid_KeepCurrentRowVisible
            AddHandler dgv.RowPrePaint, Sub(s, e) e.PaintParts = e.PaintParts And Not DataGridViewPaintParts.Focus
            AddHandler dgv.CellPainting, AddressOf Grid_CellPainting
        End If
    End Sub

    ''' <summary>
    ''' Paints every data cell: background (rose tint when selected), text, and a single line under the row.
    ''' Skips cells a page already painted (e.g. status badges set e.Handled).
    ''' </summary>
    Private Sub Grid_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs)
        If e.Handled OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        FillCell(e)
        e.PaintContent(e.CellBounds)
        DrawRowLine(e)
        e.Handled = True
    End Sub

    ''' <summary>Fills the whole cell (no 1-px seams between columns) with its normal or selected background.</summary>
    Public Sub FillCell(e As DataGridViewCellPaintingEventArgs)
        Dim selected = (e.State And DataGridViewElementStates.Selected) <> 0
        Using br As New SolidBrush(If(selected, e.CellStyle.SelectionBackColor, e.CellStyle.BackColor))
            e.Graphics.FillRectangle(br, e.CellBounds)
        End Using
    End Sub

    ''' <summary>The thin divider under a grid row (call from custom cell painting).</summary>
    Public Sub DrawRowLine(e As DataGridViewCellPaintingEventArgs)
        Using pen As New Pen(Border)
            e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1)
        End Using
    End Sub

    ''' <summary>Mouse-wheel scrolling for grids whose scrollbars are hidden.</summary>
    Private Sub Grid_MouseWheel(sender As Object, e As MouseEventArgs)
        Dim grid = DirectCast(sender, DataGridView)
        If grid.RowCount = 0 OrElse grid.DisplayedRowCount(False) = 0 Then Return
        Dim visible = grid.DisplayedRowCount(False)
        Dim maxFirst = Math.Max(0, grid.RowCount - visible)
        Dim target = grid.FirstDisplayedScrollingRowIndex - Math.Sign(e.Delta) * Math.Max(1, SystemInformation.MouseWheelScrollLines)
        SetFirstRow(grid, Math.Max(0, Math.Min(maxFirst, target)))
        If TypeOf e Is HandledMouseEventArgs Then DirectCast(e, HandledMouseEventArgs).Handled = True
    End Sub

    ''' <summary>Keeps the selected row on screen when moving with the arrow keys.</summary>
    Private Sub Grid_KeepCurrentRowVisible(sender As Object, e As EventArgs)
        Dim grid = DirectCast(sender, DataGridView)
        If grid.CurrentRow Is Nothing OrElse grid.DisplayedRowCount(False) = 0 Then Return
        Dim index = grid.CurrentRow.Index
        Dim first = grid.FirstDisplayedScrollingRowIndex
        Dim visible = grid.DisplayedRowCount(False)
        If index < first Then
            SetFirstRow(grid, index)
        ElseIf index >= first + visible Then
            SetFirstRow(grid, Math.Max(0, index - visible + 1))
        End If
    End Sub

    Private Sub SetFirstRow(grid As DataGridView, index As Integer)
        If index < 0 OrElse index = grid.FirstDisplayedScrollingRowIndex Then Return
        Try
            grid.FirstDisplayedScrollingRowIndex = index
        Catch ex As InvalidOperationException
            ' grid is being resized and has no room for rows yet
        End Try
    End Sub

    ''' <summary>Paints a status badge inside a grid cell. Hook from CellPainting for status columns.</summary>
    Public Sub PaintBadgeCell(e As DataGridViewCellPaintingEventArgs)
        If e.RowIndex < 0 OrElse e.Value Is Nothing OrElse TypeOf e.Value Is DBNull Then Return
        Dim text = Convert.ToString(e.Value)
        If text = "" Then Return
        FillCell(e)
        Dim colors = StatusColors(text)
        Dim sz = TextRenderer.MeasureText(text, FontSmallBold)
        Dim h = Dpi(24)
        Dim rect As New Rectangle(e.CellBounds.X + Dpi(10), e.CellBounds.Y + (e.CellBounds.Height - h) \ 2,
                                  Math.Min(sz.Width + Dpi(18), e.CellBounds.Width - Dpi(14)), h)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Using path = RoundedRect(rect, h \ 2), br As New SolidBrush(colors.Back)
            e.Graphics.FillPath(br, path)
        End Using
        TextRenderer.DrawText(e.Graphics, text, FontSmallBold, rect, colors.Fore,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis)
        DrawRowLine(e)
        e.Handled = True
    End Sub

    ' ---------- Drawing helpers ----------
    ''' <summary>CSU decoration: thin diagonal white stripes clipped to an area (used on the login panel, dialog rails and notices).</summary>
    Public Sub DrawStripes(g As Graphics, area As Rectangle, alpha As Integer, Optional gap As Integer = 16)
        Dim state = g.Save()
        g.SetClip(area, CombineMode.Intersect)
        Using pen As New Pen(Color.FromArgb(alpha, Color.White), Dpi(2))
            For x = area.Left - area.Height To area.Right Step Dpi(gap)
                g.DrawLine(pen, x, area.Bottom, x + area.Height, area.Top)
            Next
        End Using
        g.Restore(state)
    End Sub

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

    ''' <summary>Renders a Segoe MDL2 Assets glyph to a bitmap (for Button.Image / PictureBox). size is in pixels.</summary>
    ''' The glyph is turned into a shape and centered by its ink, so icons sit exactly in the middle of the button.
    Public Function GlyphImage(glyph As String, color As Color, size As Integer) As Bitmap
        Dim bmp As New Bitmap(size + 4, size + 4)
        Using g = Graphics.FromImage(bmp), path As New GraphicsPath(), family As New FontFamily("Segoe MDL2 Assets"), br As New SolidBrush(color)
            g.SmoothingMode = SmoothingMode.AntiAlias
            path.AddString(glyph, family, 0, size, Point.Empty, StringFormat.GenericTypographic)
            Dim b = path.GetBounds()
            If b.Width > 0 AndAlso b.Height > 0 Then
                Dim scale = Math.Min(1.0F, size / Math.Max(b.Width, b.Height))
                Using m As New Matrix()
                    m.Translate(bmp.Width / 2.0F, bmp.Height / 2.0F)
                    m.Scale(scale, scale)
                    m.Translate(-(b.X + b.Width / 2.0F), -(b.Y + b.Height / 2.0F))
                    path.Transform(m)
                End Using
                g.FillPath(br, path)
            End If
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
    Public Const Info As String = ChrW(&HE946)
    Public Const ErrorBadge As String = ChrW(&HEA39)
    Public Const Help As String = ChrW(&HE897)
    Public Const Key As String = ChrW(&HE192)
    Public Const Calendar As String = ChrW(&HE787)
    Public Const ArrowRight As String = ChrW(&HE72A)
    Public Const ArrowLeft As String = ChrW(&HE72B)
    Public Const People As String = ChrW(&HE716)
    Public Const CheckList As String = ChrW(&HE9D5)
    Public Const Scan As String = ChrW(&HE8B3)
End Module
