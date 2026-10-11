Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' Small non-blocking message at the bottom-right of a form that fades out.
''' Use for success feedback ("Attendance saved"). Use MessageBox only for confirmations/errors.
''' Usage: Toast.Show(Me.FindForm(), "Saved", "Success")   kinds: Success | Info | Warning | Danger
''' </summary>
Public Class Toast
    Inherits Form

    Private ReadOnly _timer As New Timer()
    Private _ticks As Integer

    Private Sub New(message As String, kind As String)
        FormBorderStyle = FormBorderStyle.None
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        BackColor = Theme.TextPrimary
        Padding = New Padding(16, 12, 16, 12)

        Dim colors = Theme.StatusColors(kind)
        Dim glyph = If(kind = "Success", Icons.Check, If(kind = "Danger" OrElse kind = "Warning", Icons.WarningSign, Icons.Bell))
        Dim icon As New Label With {.Text = glyph, .Font = Theme.FontIcon, .ForeColor = colors.Back,
                                    .AutoSize = False, .Size = New Size(24, 24), .TextAlign = ContentAlignment.MiddleCenter}
        Dim lbl As New Label With {.Text = message, .Font = Theme.FontBase, .ForeColor = Color.White,
                                   .AutoSize = True, .MaximumSize = New Size(360, 0)}
        Dim sz = TextRenderer.MeasureText(message, Theme.FontBase, New Size(360, 0), TextFormatFlags.WordBreak)
        ClientSize = New Size(Math.Min(420, sz.Width + 76), Math.Max(48, sz.Height + 24))
        icon.Location = New Point(14, (ClientSize.Height - 24) \ 2)
        lbl.Location = New Point(46, (ClientSize.Height - sz.Height) \ 2)
        Controls.Add(icon)
        Controls.Add(lbl)
        Region = New Region(Theme.RoundedRect(New Rectangle(0, 0, ClientSize.Width, ClientSize.Height), 8))

        _timer.Interval = 50
        AddHandler _timer.Tick, AddressOf OnTick
    End Sub

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    Private Sub OnTick(sender As Object, e As EventArgs)
        _ticks += 1
        If _ticks > 50 Then                     ' ~2.5 s visible, then fade
            Opacity -= 0.1
            If Opacity <= 0.05 Then
                _timer.Stop()
                Close()
            End If
        End If
    End Sub

    Public Overloads Shared Sub Show(owner As Form, message As String, Optional kind As String = "Success")
        If owner Is Nothing Then Return
        Dim t As New Toast(message, kind)
        Dim area = owner.RectangleToScreen(owner.ClientRectangle)
        t.Location = New Point(area.Right - t.Width - 24, area.Bottom - t.Height - 24)
        t.Show(owner)
        t._timer.Start()
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _timer.Dispose()
        MyBase.Dispose(disposing)
    End Sub

End Class
