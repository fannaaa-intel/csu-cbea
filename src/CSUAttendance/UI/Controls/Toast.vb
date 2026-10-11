Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Small non-blocking message that slides up at the bottom-center of the window, then fades out.
''' Use for success feedback ("Attendance saved"). Use Notice only for confirmations/errors.
''' Usage: Toast.Show(Me.FindForm(), "Saved", "Success")   kinds: Success | Info | Warning | Danger
''' </summary>
Public Class Toast
    Inherits Form

    Private Shared _current As Toast
    Private ReadOnly _timer As New Timer With {.Interval = 16}
    Private ReadOnly _message As String
    Private ReadOnly _kind As String
    Private _ticks As Integer
    Private _targetTop As Integer

    Private Sub New(message As String, kind As String)
        _message = message
        _kind = kind
        FormBorderStyle = FormBorderStyle.None
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        BackColor = Theme.Ink
        Opacity = 0
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint, True)

        Dim sz = TextRenderer.MeasureText(message, Theme.FontBold, New Size(Theme.Dpi(420), 0), TextFormatFlags.WordBreak)
        ClientSize = New Size(sz.Width + Theme.Dpi(84), Math.Max(Theme.Dpi(52), sz.Height + Theme.Dpi(28)))
        Region = New Region(Theme.RoundedRect(New Rectangle(0, 0, ClientSize.Width, ClientSize.Height), ClientSize.Height \ 2))
        AddHandler _timer.Tick, AddressOf OnTick
    End Sub

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.Clear(Theme.Ink)
        Dim colors = Theme.StatusColors(_kind)
        Dim d = Theme.Dpi(28)
        Dim dot As New Rectangle(Theme.Dpi(14), (Height - d) \ 2, d, d)
        Using br As New SolidBrush(colors.Fore)
            g.FillEllipse(br, dot)
        End Using
        Dim glyph = If(_kind = "Success", Icons.Check, If(_kind = "Danger" OrElse _kind = "Warning", Icons.WarningSign, Icons.Info))
        TextRenderer.DrawText(g, glyph, Theme.FontIcon, dot, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        TextRenderer.DrawText(g, _message, Theme.FontBold,
                              New Rectangle(dot.Right + Theme.Dpi(12), 0, Width - dot.Right - Theme.Dpi(30), Height), Color.White,
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak Or TextFormatFlags.NoPrefix)
    End Sub

    Private Sub OnTick(sender As Object, e As EventArgs)
        _ticks += 1
        If _ticks <= 12 Then                          ' slide up + fade in (~0.2 s)
            Opacity = _ticks / 12.0
            Top = _targetTop + CInt(Theme.Dpi(18) * (12 - _ticks) / 12.0)
        ElseIf _ticks > 175 Then                      ' ~2.8 s visible, then fade out
            Opacity -= 0.08
            If Opacity <= 0.05 Then
                _timer.Stop()
                Close()
            End If
        End If
    End Sub

    Public Overloads Shared Sub Show(owner As Form, message As String, Optional kind As String = "Success")
        If owner Is Nothing Then Return
        If _current IsNot Nothing AndAlso Not _current.IsDisposed Then _current.Close()   ' one toast at a time
        Dim t As New Toast(message, kind)
        Dim area = owner.RectangleToScreen(owner.ClientRectangle)
        t._targetTop = area.Bottom - t.Height - Theme.Dpi(32)
        t.Location = New Point(area.Left + (area.Width - t.Width) \ 2, t._targetTop + Theme.Dpi(18))
        t.Show(owner)
        t._timer.Start()
        _current = t
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _timer.Dispose()
        MyBase.Dispose(disposing)
    End Sub

End Class
