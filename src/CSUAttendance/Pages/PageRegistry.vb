''' <summary>One sidebar entry.</summary>
Public Class NavItem
    Public Property Key As String
    Public Property Text As String
    Public Property Glyph As String
    ''' <summary>Section caption shown above the first item of a group (e.g. "RECORDS"); "" = no caption.</summary>
    Public Property Group As String
    Public Sub New(key As String, text As String, glyph As String, Optional group As String = "")
        Me.Key = key
        Me.Text = text
        Me.Glyph = glyph
        Me.Group = group
    End Sub
End Class

''' <summary>
''' Sidebar menu per role + the factory that creates a page for a key.
''' Each role only gets its own menu, and Create() refuses pages the role may not open.
''' </summary>
Public Module PageRegistry

    Public Function MenuFor(role As String) As List(Of NavItem)
        Select Case role
            Case "Admin"
                Return New List(Of NavItem) From {
                    New NavItem("dashboard", "Dashboard", Icons.Dashboard),
                    New NavItem("students", "Students", Icons.Students, "Records"),
                    New NavItem("teachers", "Teachers", Icons.Teachers, "Records"),
                    New NavItem("users", "User Accounts", Icons.Users, "Records"),
                    New NavItem("terms", "School Terms", Icons.Terms, "Records"),
                    New NavItem("subjects", "Subjects", Icons.Subjects, "Records"),
                    New NavItem("sections", "Sections", Icons.Sections, "Records"),
                    New NavItem("classes", "Classes", Icons.Classes, "Records"),
                    New NavItem("enrollment", "Enrollment", Icons.Enrollment, "Records"),
                    New NavItem("records", "Attendance Records", Icons.Records, "Attendance"),
                    New NavItem("mystudents", "Section Attendance", Icons.Sections, "Attendance"),
                    New NavItem("reports", "Reports", Icons.Reports, "Attendance"),
                    New NavItem("settings", "Settings", Icons.Settings, "System"),
                    New NavItem("audit", "Audit Log", Icons.AuditLog, "System")
                }
            Case "Teacher"
                Return New List(Of NavItem) From {
                    New NavItem("dashboard", "Dashboard", Icons.Dashboard),
                    New NavItem("take", "Take Attendance", Icons.TakeAttendance, "Attendance"),
                    New NavItem("records", "Attendance Records", Icons.Records, "Attendance"),
                    New NavItem("mystudents", "Section Attendance", Icons.Sections, "Attendance"),
                    New NavItem("reports", "Reports", Icons.Reports, "Reports")
                }
            Case Else ' Student
                Return New List(Of NavItem) From {
                    New NavItem("dashboard", "My Attendance", Icons.Dashboard),
                    New NavItem("history", "Attendance History", Icons.Records, "My records"),
                    New NavItem("profile", "My Profile", Icons.Person, "Account")
                }
        End Select
    End Function

    ''' <summary>True when the signed-in role may open the page (menu items + a few pages reached by links).</summary>
    Public Function IsAllowed(role As String, key As String) As Boolean
        If key = "notifications" OrElse MenuFor(role).Any(Function(i) i.Key = key) Then Return True
        Return role = "Admin" AndAlso (key = "take" OrElse key = "mystudents")
    End Function

    ''' <summary>Creates the page for a key (roles are checked first).</summary>
    Public Function Create(key As String) As PageBase
        Dim role = If(Session.User Is Nothing, "", Session.User.Role)
        If Not IsAllowed(role, key) Then Throw New UnauthorizedAccessException("Your account cannot open this page.")
        Select Case key
            ' ---------- shared ----------
            Case "dashboard"
                If role = "Student" Then Return New StudentDashboardPage()
                If role = "Teacher" Then Return New TeacherDashboardPage()
                Return New AdminDashboardPage()
            Case "notifications" : Return New NotificationsPage()

            ' ---------- admin records ----------
            Case "students" : Return New StudentsPage()
            Case "teachers" : Return New TeachersPage()
            Case "users" : Return New UsersPage()
            Case "terms" : Return New TermsPage()
            Case "subjects" : Return New SubjectsPage()
            Case "sections" : Return New SectionsPage()
            Case "classes" : Return New ClassesPage()
            Case "enrollment" : Return New EnrollmentPage()

            ' ---------- attendance ----------
            Case "take" : Return New TakeAttendancePage()
            Case "records" : Return New RecordsPage()
            Case "mystudents" : Return New SectionAttendancePage()
            Case "reports" : Return New ReportsPage()

            ' ---------- student ----------
            Case "history" : Return New HistoryPage()
            Case "profile" : Return New ProfilePage()

            ' ---------- system ----------
            Case "settings" : Return New SettingsPage()
            Case "audit" : Return New AuditLogPage()

            Case Else : Return New PlaceholderPage(key, 0)
        End Select
    End Function

End Module
