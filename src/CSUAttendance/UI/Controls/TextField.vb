Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

''' <summary>
''' Minimal text input: rounded 1px border that turns rose on focus, inner padding, placeholder text.
''' Use instead of a plain TextBox. Access the inner TextBox via .Inner when needed.
''' </summary>
<DefaultEvent("TextChanged")>
Public Class TextField
    Inherits Control

    Private WithEvents _box As New TextBox()
    Private _placeholder As String = ""
    Private _focused As Boolean

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As String) As IntPtr
    End Function
    Private Const EM_SETCUEBANNER As Integer = &H1501

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.UserPaint, True)
        BackColor = Theme.Surface
        _box.BorderStyle = BorderStyle.None
        _box.Font = Theme.FontBase
        _box.ForeColor = Theme.TextPrimary
        _box.BackColor = Theme.Surface
        Controls.Add(_box)
        Size = New Size(Theme.Dpi(240), Theme.Dpi(Theme.ControlHeight))
        Cursor = Cursors.IBeam
    End Sub

    <Browsable(False)>
    Public ReadOnly Property Inner As TextBox
        Get
            Return _box
        End Get
    End Property

    <Browsable(True), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Overrides Property Text As String
        Get
            Return _box.Text
        End Get
        Set(value As String)
            _box.Text = value
        End Set
    End Property

    <Category("Behavior"), DefaultValue("")>
    Public Property Placeholder As String
        Get
            Return _placeholder
        End Get
        Set(value As String)
            _placeholder = If(value, "")
            ApplyCue()
        End Set
    End Property

    <Category("Behavior"), DefaultValue(False)>
    Public Property UseSystemPasswordChar As Boolean
        Get
            Return _box.UseSystemPasswordChar
        End Get
        Set(value As Boolean)
            _box.UseSystemPasswordChar = value
        End Set
    End Property

    <Category("Behavior"), DefaultValue(32767)>
    Public Property MaxLength As Integer
        Get
            Return _box.MaxLength
        End Get
        Set(value As Integer)
            _box.MaxLength = value
        End Set
    End Property

    <Category("Behavior"), DefaultValue(False)>
    Public Property [ReadOnly] As Boolean
        Get
            Return _box.ReadOnly
        End Get
        Set(value As Boolean)
            _box.ReadOnly = value
            _box.BackColor = If(value, Theme.NeutralSoft, Theme.Surface)
            Invalidate()
        End Set
    End Property

    Public Sub SelectAllText()
        _box.Focus()
        _box.SelectAll()
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        _box.Focus()
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        _box.Focus()
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        ApplyCue()
    End Sub

    Protected Overrides Sub OnLayout(levent As LayoutEventArgs)
        MyBase.OnLayout(levent)
        _box.Location = New Point(Theme.Dpi(12), Math.Max(0, (Height - _box.Height) \ 2))
        _box.Width = Math.Max(10, Width - Theme.Dpi(24))
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim parentColor = If(Parent IsNot Nothing, Parent.BackColor, Theme.Surface)
        e.Graphics.Clear(parentColor)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        Dim r As New Rectangle(0, 0, Width - 1, Height - 1)
        Using path = Theme.RoundedRect(r, Theme.Dpi(6)), br As New SolidBrush(_box.BackColor),
              pen As New Pen(If(_focused, Theme.Accent, Theme.Border), If(_focused, 1.5F, 1.0F))
            e.Graphics.FillPath(br, path)
            e.Graphics.DrawPath(pen, path)
        End Using
    End Sub

    Private Sub ApplyCue()
        If _box.IsHandleCreated Then SendMessage(_box.Handle, EM_SETCUEBANNER, New IntPtr(1), _placeholder)
    End Sub

    ' ----- forward inner TextBox events -----
    Private Sub Box_TextChanged(sender As Object, e As EventArgs) Handles _box.TextChanged
        OnTextChanged(e)
    End Sub
    Private Sub Box_KeyDown(sender As Object, e As KeyEventArgs) Handles _box.KeyDown
        OnKeyDown(e)
    End Sub
    Private Sub Box_KeyPress(sender As Object, e As KeyPressEventArgs) Handles _box.KeyPress
        OnKeyPress(e)
    End Sub
    Private Sub Box_Enter(sender As Object, e As EventArgs) Handles _box.Enter
        _focused = True
        Invalidate()
    End Sub
    Private Sub Box_Leave(sender As Object, e As EventArgs) Handles _box.Leave
        _focused = False
        Invalidate()
    End Sub
    Private Sub Box_HandleCreated(sender As Object, e As EventArgs) Handles _box.HandleCreated
        ApplyCue()
    End Sub

End Class
