Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Temporary page for modules not built yet. Delete once every module is done.</summary>
Public Class PlaceholderPage
    Inherits PageBase

    Private ReadOnly _title As String

    Public Sub New()
        Me.New("Page", 0)
    End Sub

    Public Sub New(title As String, moduleNo As Integer)
        _title = title
        Dim card As New CardPanel With {.Dock = DockStyle.Fill}
        Dim lbl As New Label With {
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Theme.TextMuted,
            .Font = Theme.FontSubtitle,
            .BackColor = Theme.Surface,
            .Text = $"{title}" & Environment.NewLine & Environment.NewLine &
                    If(moduleNo > 0, $"This page is built in Module {moduleNo} (see docs/PROMPTS.md).", "Unknown page.")
        }
        card.Controls.Add(lbl)
        Controls.Add(card)
    End Sub

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return _title
        End Get
    End Property

End Class
