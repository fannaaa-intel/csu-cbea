Imports System.Drawing
Imports System.Windows.Forms

''' <summary>Student's read-only profile card + "Change password".</summary>
Public Class ProfilePage
    Inherits PageBase

    Private ReadOnly card As CardPanel = Ui.Card(DockStyle.None, 32)
    Private ReadOnly btnPassword As Button = Ui.Action("Change password", "primary", Icons.Key, 170)

    Public Overrides ReadOnly Property PageTitle As String
        Get
            Return "My Profile"
        End Get
    End Property

    Public Sub New()
        card.Size = New Size(Theme.Dpi(760), Theme.Dpi(470))
        Controls.Add(card)
        AddHandler btnPassword.Click, Sub()
                                          Using f As New ChangePasswordForm()
                                              If f.ShowDialog(FindForm()) = DialogResult.OK Then ShowToast("Password changed")
                                          End Using
                                      End Sub
    End Sub

    Public Overrides Sub OnPageShown()
        Dim dt = Db.Query(
            "SELECT s.*, sec.section_name, sec.program, sec.year_level, u.username, u.last_login " &
            "FROM students s LEFT JOIN sections sec ON sec.section_id = s.section_id LEFT JOIN users u ON u.user_id = s.user_id " &
            "WHERE s.student_id = @s", Db.P("@s", Session.User.StudentId))
        If dt.Rows.Count = 0 Then Return
        Dim r = dt.Rows(0)
        Dim pad = Theme.Dpi(32)

        Dim avatar As New Label With {.Text = (Db.ToStr(r("first_name")).Substring(0, 1) & Db.ToStr(r("last_name")).Substring(0, 1)).ToUpper(),
                                      .Font = Theme.FontHeading, .ForeColor = Theme.Accent, .BackColor = Theme.AccentSoft,
                                      .TextAlign = ContentAlignment.MiddleCenter, .Bounds = New Rectangle(pad, pad, Theme.Dpi(64), Theme.Dpi(64))}
        avatar.Region = New Region(Theme.RoundedRect(New Rectangle(0, 0, avatar.Width, avatar.Height), avatar.Width \ 2))
        Dim name = Ui.Caption(TextUtil.FullName(Db.ToStr(r("first_name")), Db.ToStr(r("middle_name")), Db.ToStr(r("last_name"))), Theme.FontHeading)
        name.Location = New Point(pad + Theme.Dpi(80), pad + Theme.Dpi(6))
        Dim sub1 = Ui.Muted($"{r("student_no")}  ·  {r("program")} {r("year_level")}  ·  {r("section_name")}")
        sub1.Font = Theme.FontBase
        sub1.Location = New Point(pad + Theme.Dpi(82), pad + Theme.Dpi(38))
        Dim badge As New StatusBadge With {.Text = Db.ToStr(r("status"))}
        badge.Location = New Point(card.Width - pad - badge.Width, pad + Theme.Dpi(10))
        card.Controls.AddRange({avatar, name, sub1, badge})

        ' Two columns of label / value pairs
        Dim fields = {
            ("Sex", Db.ToStr(r("sex"))), ("Contact no.", Db.ToStr(r("contact_no"))),
            ("Guardian", Db.ToStr(r("guardian_name"))), ("Guardian contact", Db.ToStr(r("guardian_contact"))),
            ("Username", Db.ToStr(r("username"))), ("Last sign-in", If(r("last_login") Is DBNull.Value, "—", Db.ToDate(r("last_login")).ToString("MMM d, yyyy h:mm tt")))
        }
        Dim colW = (card.Width - pad * 2) \ 2
        For i = 0 To fields.Length - 1
            Dim x = pad + (i Mod 2) * colW
            Dim y = pad + Theme.Dpi(110) + (i \ 2) * Theme.Dpi(66)
            Dim l = Ui.Muted(fields(i).Item1.ToUpper())
            l.Font = Theme.FontSmallBold
            l.Location = New Point(x, y)
            Dim v = Ui.Caption(If(fields(i).Item2 = "", "—", fields(i).Item2), Theme.FontSubtitle)
            v.Location = New Point(x, y + Theme.Dpi(20))
            card.Controls.AddRange({l, v})
        Next

        Dim hint = Ui.Muted("To correct your details, ask the registrar or the system administrator.")
        hint.Location = New Point(pad, card.Height - pad - Theme.Dpi(30))
        btnPassword.Location = New Point(card.Width - pad - btnPassword.Width, card.Height - pad - btnPassword.Height)
        card.Controls.AddRange({hint, btnPassword})
    End Sub

End Class
