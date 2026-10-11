Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Progress bar for absence hours against the drop limit.
''' Fill = PercentAbsent / DropPct. Color follows the status (green → amber → orange → red).
''' Thin ticks mark the Warning 1 and Warning 2 thresholds.
''' </summary>
Public Class HoursBar
    Inherits Control

    Private _percent As Decimal
    Public Property Warning1Pct As Decimal = 10D
    Public Property Warning2Pct As Decimal = 15D
    Public Property DropPct As Decimal = 20D

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        Size = New Size(Theme.Dpi(240), Theme.Dpi(10))
        BackColor = Theme.Surface
    End Sub

    Public Property PercentAbsent As Decimal
        Get
            Return _percent
        End Get
        Set(value As Decimal)
            _percent = Math.Max(0D, value)
            Invalidate()
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(BackColor)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim track As New Rectangle(0, 0, Width - 1, Height - 1)
        Using path = Theme.RoundedRect(track, Height \ 2), br As New SolidBrush(Theme.NeutralSoft)
            g.FillPath(br, path)
        End Using

        Dim limit = If(DropPct <= 0D, 20D, DropPct)
        Dim ratio = CSng(Math.Min(1D, _percent / limit))
        If ratio > 0 Then
            Dim fillColor As Color
            If _percent >= DropPct Then
                fillColor = Theme.Danger
            ElseIf _percent >= Warning2Pct Then
                fillColor = Theme.Orange
            ElseIf _percent >= Warning1Pct Then
                fillColor = Theme.Warning
            Else
                fillColor = Theme.Success
            End If
            Dim fill As New Rectangle(0, 0, Math.Max(Height, CInt((Width - 1) * ratio)), Height - 1)
            Using path = Theme.RoundedRect(fill, Height \ 2), br As New SolidBrush(fillColor)
                g.FillPath(br, path)
            End Using
        End If

        Using pen As New Pen(Color.FromArgb(120, Theme.TextMuted))
            For Each t In {Warning1Pct, Warning2Pct}
                Dim x = CInt((Width - 1) * CSng(t / limit))
                g.DrawLine(pen, x, 0, x, Height)
            Next
        End Using
    End Sub

End Class
