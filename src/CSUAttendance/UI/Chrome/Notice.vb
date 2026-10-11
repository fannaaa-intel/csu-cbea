Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

''' <summary>
''' The system's own message dialog (replaces the plain Windows MessageBox).
''' Usage:
'''   If Notice.Confirm(Me, "Delete subject", "Delete AIS 311?", "Delete", danger:=True) Then ...
'''   Notice.Info(Me, "Section in use", "BSAIS 3B still has students.")
'''   Notice.Fail(Me, "Could not save", ex.Message)
''' </summary>
Public Module Notice

    Public Function Confirm(owner As Control, title As String, message As String,
                            Optional okText As String = "Yes, continue", Optional cancelText As String = "Cancel",
                            Optional danger As Boolean = False) As Boolean
        Return NoticeForm.Display(owner, If(danger, NoticeKind.Danger, NoticeKind.Question), title, message, okText, cancelText) = DialogResult.OK
    End Function

    Public Sub Info(owner As Control, title As String, message As String, Optional okText As String = "Got it")
        NoticeForm.Display(owner, NoticeKind.Info, title, message, okText, "")
    End Sub

    Public Sub Warn(owner As Control, title As String, message As String, Optional okText As String = "Okay")
        NoticeForm.Display(owner, NoticeKind.Warning, title, message, okText, "")
    End Sub

    Public Sub Success(owner As Control, title As String, message As String, Optional okText As String = "Done")
        NoticeForm.Display(owner, NoticeKind.Success, title, message, okText, "")
    End Sub

    ''' <summary>Error message (e.g. a failed save). Shows the technical detail in smaller text.</summary>
    Public Sub Fail(owner As Control, title As String, message As String, Optional detail As String = "")
        Dim text = If(detail = "", message, message & Environment.NewLine & Environment.NewLine & detail)
        NoticeForm.Display(owner, NoticeKind.Failure, title, text, "Close", "")
    End Sub

    ''' <summary>Yes/No question where "No" is also a real answer (e.g. overlapping schedule: continue anyway?).</summary>
    Public Function Ask(owner As Control, title As String, message As String, yesText As String, noText As String) As Boolean
        Return NoticeForm.Display(owner, NoticeKind.Warning, title, message, yesText, noText) = DialogResult.OK
    End Function

End Module

Public Enum NoticeKind
    Question
    Danger
    Warning
    Failure
    Success
    Info
End Enum

''' <summary>
''' Centered card: a colored band (glyph in a ring + small caption), then the title, message
''' and equal-width rounded buttons. Slides down and fades in over a dimmed backdrop.
''' </summary>
Friend Class NoticeForm
    Inherits Form

    Private ReadOnly _kind As NoticeKind
    Private ReadOnly _title As String
    Private ReadOnly _message As String
    Private ReadOnly _timer As New Timer With {.Interval = 15}
    Private ReadOnly _bandHeight As Integer = Theme.Dpi(118)
    Private _messageRect As Rectangle
    Private _titleRect As Rectangle
    Private _step As Integer
    Private _finalTop As Integer

    Private Sub New(kind As NoticeKind, title As String, message As String, okText As String, cancelText As String)
        _kind = kind
        _title = title
        _message = message
        FormBorderStyle = FormBorderStyle.None
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        BackColor = Theme.Surface
        KeyPreview = True
        Opacity = 0
        Text = title
        Font = Theme.FontBase
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint, True)

        Dim w = Theme.Dpi(440)
        Dim pad = Theme.Dpi(32)
        Dim textW = w - pad * 2
        Dim flags = TextFormatFlags.WordBreak Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.NoPrefix
        Dim titleH = TextRenderer.MeasureText(title, Theme.FontHeading, New Size(textW, 0), flags).Height
        Dim msgH = Math.Min(Theme.Dpi(320), TextRenderer.MeasureText(message, Theme.FontBase, New Size(textW, 0), flags).Height)
        _titleRect = New Rectangle(pad, _bandHeight + Theme.Dpi(24), textW, titleH)
        _messageRect = New Rectangle(pad, _titleRect.Bottom + Theme.Dpi(8), textW, msgH)

        ' Buttons: equal width across the bottom (Cancel left, main action right)
        Dim btnTop = _messageRect.Bottom + Theme.Dpi(28)
        Dim gap = Theme.Dpi(12)
        Dim ok As New PillButton With {.Text = okText, .FillColor = KindColor(), .DialogResult = DialogResult.OK, .TabIndex = 0}
        If cancelText <> "" Then
            Dim half = (textW - gap) \ 2
            Dim cancel As New PillButton With {.Text = cancelText, .Filled = False, .DialogResult = DialogResult.Cancel, .TabIndex = 1}
            cancel.SetBounds(pad, btnTop, half, Theme.Dpi(44))
            ok.SetBounds(pad + half + gap, btnTop, half, Theme.Dpi(44))
            Controls.Add(cancel)
            CancelButton = cancel
        Else
            ok.SetBounds(pad, btnTop, textW, Theme.Dpi(44))
            CancelButton = ok
        End If
        Controls.Add(ok)
        AcceptButton = ok
        ClientSize = New Size(w, btnTop + Theme.Dpi(44) + Theme.Dpi(28))
        AddHandler _timer.Tick, AddressOf Animate
    End Sub

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp = MyBase.CreateParams
            cp.ClassStyle = cp.ClassStyle Or WindowChrome.CS_DROPSHADOW
            Return cp
        End Get
    End Property

    ''' <summary>Shows the notice over a dimmed owner window and returns OK / Cancel.</summary>
    Public Shared Function Display(owner As Control, kind As NoticeKind, title As String, message As String,
                                   okText As String, cancelText As String) As DialogResult
        Dim ownerForm = If(owner IsNot Nothing, owner.FindForm(), Form.ActiveForm)
        Using f As New NoticeForm(kind, title, message, okText, cancelText)
            Dim shade = Global.CSUAttendance.Backdrop.Cover(ownerForm)
            Try
                Dim area = If(ownerForm IsNot Nothing AndAlso ownerForm.Visible, ownerForm.Bounds, Screen.PrimaryScreen.WorkingArea)
                f._finalTop = area.Top + (area.Height - f.Height) \ 2
                f.Location = New Point(area.Left + (area.Width - f.Width) \ 2, f._finalTop - Theme.Dpi(28))
                Return If(ownerForm IsNot Nothing AndAlso ownerForm.Visible, f.ShowDialog(ownerForm), f.ShowDialog())
            Finally
                If shade IsNot Nothing Then shade.FadeOut()
            End Try
        End Using
    End Function

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)
        WindowChrome.RoundCorners(Me)
        _timer.Start()
    End Sub

    ''' <summary>Ease-out slide down + fade in (~0.25 s).</summary>
    Private Sub Animate(sender As Object, e As EventArgs)
        _step += 1
        Dim t = Math.Min(1.0, _step / 16.0)
        Dim ease = 1 - Math.Pow(1 - t, 3)
        Opacity = ease
        Top = _finalTop - CInt(Theme.Dpi(28) * (1 - ease))
        If t >= 1 Then _timer.Stop()
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = MouseButtons.Left Then WindowChrome.BeginDrag(Me)
    End Sub

    Private Function KindColor() As Color
        Select Case _kind
            Case NoticeKind.Danger, NoticeKind.Failure : Return Theme.Danger
            Case NoticeKind.Warning : Return Theme.Warning
            Case NoticeKind.Success : Return Theme.Success
            Case NoticeKind.Info : Return Theme.Info
            Case Else : Return Theme.Accent
        End Select
    End Function

    Private Function KindGlyph() As String
        Select Case _kind
            Case NoticeKind.Danger, NoticeKind.Warning : Return Icons.WarningSign
            Case NoticeKind.Failure : Return Icons.ErrorBadge
            Case NoticeKind.Success : Return Icons.Check
            Case NoticeKind.Info : Return Icons.Info
            Case Else : Return Icons.Help
        End Select
    End Function

    Private Function KindCaption() As String
        Select Case _kind
            Case NoticeKind.Danger : Return "PLEASE CONFIRM"
            Case NoticeKind.Warning : Return "HEADS UP"
            Case NoticeKind.Failure : Return "SOMETHING WENT WRONG"
            Case NoticeKind.Success : Return "ALL SET"
            Case NoticeKind.Info : Return "GOOD TO KNOW"
            Case Else : Return "PLEASE CONFIRM"
        End Select
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.Clear(Theme.Surface)
        Dim kc = KindColor()

        ' Colored band with soft decorative rings
        Dim band As New Rectangle(0, 0, Width, _bandHeight)
        Using br As New LinearGradientBrush(band, kc, Theme.Darken(kc, 0.28F), LinearGradientMode.ForwardDiagonal)
            g.FillRectangle(br, band)
        End Using
        Theme.DrawStripes(g, New Rectangle(Width - Theme.Dpi(130), 0, Theme.Dpi(130), _bandHeight), 30)
        Theme.DrawStripes(g, New Rectangle(0, 0, Theme.Dpi(90), _bandHeight), 22)

        ' Glyph inside a white ring
        Dim d = Theme.Dpi(58)
        Dim ring As New Rectangle((Width - d) \ 2, Theme.Dpi(18), d, d)
        Using br As New SolidBrush(Color.FromArgb(45, Color.White)), pen As New Pen(Color.FromArgb(150, Color.White), Theme.Dpi(2))
            g.FillEllipse(br, ring)
            g.DrawEllipse(pen, ring)
        End Using
        Using f As New Font("Segoe MDL2 Assets", 18.0F)
            TextRenderer.DrawText(g, KindGlyph(), f, ring, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)
        End Using
        TextRenderer.DrawText(g, KindCaption(), Theme.FontSmallBold, New Rectangle(0, ring.Bottom + Theme.Dpi(8), Width, Theme.Dpi(20)),
                              Color.FromArgb(235, Color.White), TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter)

        ' Title + message
        Dim flags = TextFormatFlags.WordBreak Or TextFormatFlags.HorizontalCenter Or TextFormatFlags.NoPrefix
        TextRenderer.DrawText(g, _title, Theme.FontHeading, _titleRect, Theme.TextPrimary, flags)
        TextRenderer.DrawText(g, _message, Theme.FontBase, _messageRect, Theme.TextMuted, flags Or TextFormatFlags.EndEllipsis)

        Using pen As New Pen(Theme.Border)
            g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1)
        End Using
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _timer.Dispose()
        MyBase.Dispose(disposing)
    End Sub

End Class
