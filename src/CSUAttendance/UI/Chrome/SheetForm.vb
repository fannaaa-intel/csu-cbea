Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Base class for every add/edit dialog. Draws a rose side rail on the left (icon, caption, short help text),
''' leaves the right side white for the fields, adds a close button, dims the window behind it and slides in.
''' Designer rule: place your controls to the right of the rail (X ≥ RailWidth + 28).
''' In the dialog's Load: SetRail(Icons.Subjects, "Subjects", "One line of help for this form.")
''' </summary>
Public Class SheetForm
    Inherits Form

    ''' <summary>Width of the rail at 100% scaling. Designer controls start at RailWidth + 28.</summary>
    Public Const RailWidth As Integer = 210

    Private _railGlyph As String = Icons.Edit
    Private _railCaption As String = ""
    Private _railNote As String = ""
    Private _backdrop As Backdrop
    Private _closeButton As Label
    Private ReadOnly _anim As New Timer With {.Interval = 15}
    Private _step As Integer
    Private _finalTop As Integer

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        AddHandler _anim.Tick, AddressOf Animate
    End Sub

    ''' <summary>Hide the top-right close button (e.g. forced password change).</summary>
    <DefaultValue(True)>
    Public Property ShowCloseButton As Boolean = True

    ''' <summary>Sets the icon, caption and help text shown on the rail.</summary>
    Public Sub SetRail(glyph As String, caption As String, note As String)
        _railGlyph = glyph
        _railCaption = caption
        _railNote = note
        Invalidate()
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp = MyBase.CreateParams
            cp.ClassStyle = cp.ClassStyle Or WindowChrome.CS_DROPSHADOW
            Return cp
        End Get
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        If DesignMode Then Return
        If ShowCloseButton Then AddCloseButton()
        _backdrop = Backdrop.Cover(Owner)
        Opacity = 0
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        If DesignMode Then Return
        WindowChrome.RoundCorners(Me)
        _finalTop = Top
        Top = _finalTop + Theme.Dpi(24)
        _anim.Start()
    End Sub

    ''' <summary>Ease-out slide up + fade in (~0.25 s).</summary>
    Private Sub Animate(sender As Object, e As EventArgs)
        _step += 1
        Dim t = Math.Min(1.0, _step / 16.0)
        Dim ease = 1 - Math.Pow(1 - t, 3)
        Opacity = ease
        Top = _finalTop + CInt(Theme.Dpi(24) * (1 - ease))
        If t >= 1 Then _anim.Stop()
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        _anim.Stop()
        If _backdrop IsNot Nothing Then _backdrop.FadeOut()
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub AddCloseButton()
        Dim size = Theme.Dpi(34)
        _closeButton = New Label With {
            .Text = Icons.Close, .Font = Theme.FontIcon, .ForeColor = Theme.TextMuted, .BackColor = Theme.Surface,
            .TextAlign = ContentAlignment.MiddleCenter, .Cursor = Cursors.Hand, .AutoSize = False,
            .Bounds = New Rectangle(ClientSize.Width - size - Theme.Dpi(12), Theme.Dpi(12), size, size),
            .Anchor = AnchorStyles.Top Or AnchorStyles.Right
        }
        AddHandler _closeButton.MouseEnter, Sub() _closeButton.BackColor = Theme.Hover
        AddHandler _closeButton.MouseLeave, Sub() _closeButton.BackColor = Theme.Surface
        AddHandler _closeButton.Click, Sub()
                                           DialogResult = DialogResult.Cancel
                                       End Sub
        Controls.Add(_closeButton)
        _closeButton.BringToFront()
    End Sub

    ''' <summary>Drag the dialog by the rail or the empty top strip.</summary>
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = MouseButtons.Left AndAlso (e.X < Theme.Dpi(RailWidth) OrElse e.Y < Theme.Dpi(64)) Then WindowChrome.BeginDrag(Me)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim railW = Theme.Dpi(RailWidth)
        Dim rail As New Rectangle(0, 0, railW, ClientSize.Height)

        Using br As New LinearGradientBrush(rail, Theme.Accent, Theme.AccentDeep, LinearGradientMode.Vertical)
            g.FillRectangle(br, rail)
        End Using

        ' Decoration: dotted grid near the bottom + a soft ring
        g.SetClip(rail)
        Using dot As New SolidBrush(Color.FromArgb(34, Color.White))
            Dim stepPx = Theme.Dpi(14)
            For y = rail.Bottom - Theme.Dpi(112) To rail.Bottom - Theme.Dpi(58) Step stepPx
                For x = Theme.Dpi(28) To railW - Theme.Dpi(20) Step stepPx
                    g.FillEllipse(dot, x, y, Theme.Dpi(3), Theme.Dpi(3))
                Next
            Next
        End Using
        Theme.DrawStripes(g, New Rectangle(railW - Theme.Dpi(96), 0, Theme.Dpi(96), Theme.Dpi(96)), 30)
        g.ResetClip()

        ' Icon tile
        Dim tile As New Rectangle(Theme.Dpi(28), Theme.Dpi(30), Theme.Dpi(52), Theme.Dpi(52))
        Using path = Theme.RoundedRect(tile, Theme.Dpi(14)), br As New SolidBrush(Color.FromArgb(48, Color.White))
            g.FillPath(br, path)
        End Using
        Using f As New Font("Segoe MDL2 Assets", 17.0F)
            TextRenderer.DrawText(g, _railGlyph, f, tile, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Using

        ' Caption + help text
        Dim textW = railW - Theme.Dpi(56)
        TextRenderer.DrawText(g, _railCaption.ToUpper(), Theme.FontSmallBold, New Rectangle(Theme.Dpi(28), Theme.Dpi(100), textW, Theme.Dpi(20)),
                              Color.White, TextFormatFlags.Left Or TextFormatFlags.EndEllipsis)
        TextRenderer.DrawText(g, _railNote, Theme.FontSmall,
                              New Rectangle(Theme.Dpi(28), Theme.Dpi(126), textW, Math.Max(0, ClientSize.Height - Theme.Dpi(126) - Theme.Dpi(122))),
                              Color.FromArgb(225, Color.White), TextFormatFlags.Left Or TextFormatFlags.WordBreak Or TextFormatFlags.NoPrefix)
        TextRenderer.DrawText(g, "CSU  ·  Attendance", Theme.FontSmall, New Rectangle(Theme.Dpi(28), ClientSize.Height - Theme.Dpi(40), textW, Theme.Dpi(20)),
                              Color.FromArgb(170, Color.White), TextFormatFlags.Left)

        Using pen As New Pen(Theme.Border)
            g.DrawRectangle(pen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1)
        End Using
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _anim.Dispose()
        MyBase.Dispose(disposing)
    End Sub

End Class
