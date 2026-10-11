Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

''' <summary>
''' Dark see-through layer laid over a window while a dialog is open, so the eye goes to the dialog.
''' It fades in and out and ignores clicks. Used by SheetForm and Notice.
''' </summary>
Public Class Backdrop
    Inherits Form

    Private Const MaxOpacity As Double = 0.42
    Private ReadOnly _timer As New Timer With {.Interval = 16}
    Private _fadingOut As Boolean

    Private Sub New(target As Form)
        FormBorderStyle = FormBorderStyle.None
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        BackColor = Theme.Ink
        Opacity = 0
        Bounds = target.Bounds
        AddHandler _timer.Tick, AddressOf OnTick
    End Sub

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>Covers the given window and fades in. Returns Nothing if there is nothing to cover.</summary>
    Public Shared Function Cover(target As Form) As Backdrop
        If target Is Nothing OrElse Not target.Visible OrElse target.WindowState = FormWindowState.Minimized Then Return Nothing
        Dim b As New Backdrop(target)
        b.Show(target)
        b.Enabled = False               ' clicks on the backdrop do nothing
        b._timer.Start()
        Return b
    End Function

    ''' <summary>Fades out, then closes itself.</summary>
    Public Sub FadeOut()
        If IsDisposed Then Return
        _fadingOut = True
        _timer.Start()
    End Sub

    Private Sub OnTick(sender As Object, e As EventArgs)
        If _fadingOut Then
            Opacity = Math.Max(0, Opacity - 0.07)
            If Opacity <= 0 Then
                _timer.Stop()
                Close()
            End If
        Else
            Opacity = Math.Min(MaxOpacity, Opacity + 0.06)
            If Opacity >= MaxOpacity Then _timer.Stop()
        End If
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then _timer.Dispose()
        MyBase.Dispose(disposing)
    End Sub

End Class

''' <summary>Windows 11 window touches for borderless windows: rounded corners and a drop shadow.</summary>
Friend Module WindowChrome

    <DllImport("dwmapi.dll")>
    Private Function DwmSetWindowAttribute(hwnd As IntPtr, attr As Integer, ByRef value As Integer, size As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Function ReleaseCapture() As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    Public Const CS_DROPSHADOW As Integer = &H20000

    ''' <summary>Asks Windows 11 to round the window corners (ignored on Windows 10).</summary>
    Public Sub RoundCorners(f As Form)
        Try
            Dim pref = 2                                    ' DWMWCP_ROUND
            DwmSetWindowAttribute(f.Handle, 33, pref, 4)    ' DWMWA_WINDOW_CORNER_PREFERENCE
        Catch
            ' older Windows: square corners are fine
        End Try
    End Sub

    ''' <summary>Lets the user drag a borderless window by any non-input area.</summary>
    Public Sub BeginDrag(f As Form)
        ReleaseCapture()
        SendMessage(f.Handle, &HA1, New IntPtr(2), IntPtr.Zero)   ' WM_NCLBUTTONDOWN, HTCAPTION
    End Sub

End Module
