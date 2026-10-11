Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' Date picker that looks like a TextField: 36 px, rounded border, "Oct 11, 2026" text and a calendar icon.
''' It IS a DateTimePicker (same Value / ValueChanged / MinDate), only the painting is ours.
''' Clicking anywhere opens the calendar; arrow keys still change the date.
''' Time pickers (ShowUpDown = True) keep the normal Windows look.
''' </summary>
Public Class DateField
    Inherits DateTimePicker

    Private _hover As Boolean

    Public Sub New()
        Format = DateTimePickerFormat.Custom
        CustomFormat = "MMM d, yyyy"
        Font = Theme.FontBase
        SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
        MinimumSize = New Size(0, Theme.Dpi(36))
        Height = Theme.Dpi(36)
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        If ShowUpDown Then                      ' time picker: let Windows draw the spin buttons
            SetStyle(ControlStyles.UserPaint, False)
            MinimumSize = Size.Empty
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(If(Parent IsNot Nothing, Theme.BackOf(Parent), Theme.Surface))
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(0, 0, Width - 1, Height - 1)
        Dim borderColor = If(Focused OrElse _hover, Theme.Accent, Theme.Border)
        Using path = Theme.RoundedRect(r, Theme.Dpi(6)), br As New SolidBrush(If(Enabled, Theme.Surface, Theme.NeutralSoft)),
              pen As New Pen(borderColor, If(Focused, 1.5F, 1.0F))
            g.FillPath(br, path)
            g.DrawPath(pen, path)
        End Using
        Dim iconW = Theme.Dpi(36)
        TextRenderer.DrawText(g, Value.ToString(CustomFormat), Font, New Rectangle(Theme.Dpi(12), 0, Width - iconW - Theme.Dpi(12), Height),
                              If(Enabled, Theme.TextPrimary, Theme.TextMuted), TextFormatFlags.VerticalCenter Or TextFormatFlags.Left Or TextFormatFlags.EndEllipsis)
        TextRenderer.DrawText(g, Icons.Calendar, Theme.FontIcon, New Rectangle(Width - iconW, 0, iconW - Theme.Dpi(4), Height),
                              If(Enabled, Theme.Accent, Theme.TextMuted), TextFormatFlags.VerticalCenter Or TextFormatFlags.HorizontalCenter)
    End Sub

    ' The native control opens the calendar only from its drop-down button; open it from anywhere.
    <Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function PostMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As Boolean
    End Function
    Private Const WM_KEYDOWN As Integer = &H100
    Private Const VK_F4 As Integer = &H73
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If ShowUpDown OrElse e.Button <> MouseButtons.Left Then Return
        If e.X < Width - Theme.Dpi(36) Then
            PostMessage(Handle, WM_KEYDOWN, New IntPtr(VK_F4), IntPtr.Zero)   ' F4 = open the calendar
        End If
    End Sub

    Protected Overrides Sub OnValueChanged(e As EventArgs)
        MyBase.OnValueChanged(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        _hover = True
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        _hover = False
        Invalidate()
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        MyBase.OnLostFocus(e)
        Invalidate()
    End Sub

End Class
