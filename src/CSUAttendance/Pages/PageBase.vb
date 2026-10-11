Imports System.Windows.Forms

''' <summary>
''' Base class for every page shown inside MainForm's content area.
''' Each page is a UserControl that inherits PageBase (Add > User Control, then change "Inherits UserControl" to "Inherits PageBase").
''' </summary>
Public Class PageBase
    Inherits UserControl

    Public Sub New()
        BackColor = Theme.Background
        Font = Theme.FontBase
        Theme.EnableDoubleBuffer(Me)
    End Sub

    ''' <summary>The shell hosting this page (set by MainForm). Use Host.Navigate(...) to open other pages.</summary>
    Public Property Host As MainForm

    ''' <summary>Optional argument passed by Navigate (e.g. a class_id).</summary>
    Public Property NavigationArg As Object

    ''' <summary>Text shown in the top bar.</summary>
    Public Overridable ReadOnly Property PageTitle As String
        Get
            Return "Page"
        End Get
    End Property

    ''' <summary>Called once after the page is docked and visible. Load data here (not in the constructor).</summary>
    Public Overridable Sub OnPageShown()
    End Sub

    ''' <summary>Return False to stop navigation (e.g. unsaved attendance). Ask the user inside.</summary>
    Public Overridable Function CanLeave() As Boolean
        Return True
    End Function

    ''' <summary>Shortcut for success toasts from inside a page.</summary>
    Protected Sub ShowToast(message As String, Optional kind As String = "Success")
        Toast.Show(FindForm(), message, kind)
    End Sub

    ''' <summary>Standard error message for unexpected failures.</summary>
    Protected Sub ShowError(ex As Exception, Optional action As String = "complete the action")
        MessageBox.Show(FindForm(), $"Could not {action}." & Environment.NewLine & Environment.NewLine & ex.Message,
                        "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Protected Function Confirm(message As String, Optional title As String = "Please confirm") As Boolean
        Return MessageBox.Show(FindForm(), message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes
    End Function

End Class
