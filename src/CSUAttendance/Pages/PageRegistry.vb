''' <summary>One sidebar entry.</summary>
Public Class NavItem
    Public Property Key As String
    Public Property Text As String
    Public Property Glyph As String
    Public Sub New(key As String, text As String, glyph As String)
        Me.Key = key
        Me.Text = text
        Me.Glyph = glyph
    End Sub
End Class

''' <summary>
''' Sidebar menu per role + the factory that creates a page for a key.
''' When a module is built, replace its PlaceholderPage line in Create() with the real page.
''' </summary>
Public Module PageRegistry

    Public Function MenuFor(role As String) As List(Of NavItem)
        Select Case role
            Case "Admin"
                Return New List(Of NavItem) From {
                    New NavItem("dashboard", "Dashboard", Icons.Dashboard),
                    New NavItem("students", "Students", Icons.Students),
                    New NavItem("teachers", "Teachers", Icons.Teachers),
                    New NavItem("users", "User Accounts", Icons.Users),
                    New NavItem("terms", "School Terms", Icons.Terms),
                    New NavItem("subjects", "Subjects", Icons.Subjects),
                    New NavItem("sections", "Sections", Icons.Sections),
                    New NavItem("classes", "Classes", Icons.Classes),
                    New NavItem("enrollment", "Enrollment", Icons.Enrollment),
                    New NavItem("records", "Attendance Records", Icons.Records),
                    New NavItem("reports", "Reports", Icons.Reports),
                    New NavItem("settings", "Settings", Icons.Settings),
                    New NavItem("audit", "Audit Log", Icons.AuditLog)
                }
            Case "Teacher"
                Return New List(Of NavItem) From {
                    New NavItem("dashboard", "Dashboard", Icons.Dashboard),
                    New NavItem("take", "Take Attendance", Icons.TakeAttendance),
                    New NavItem("records", "Attendance Records", Icons.Records),
                    New NavItem("mystudents", "My Students", Icons.Students),
                    New NavItem("reports", "Reports", Icons.Reports)
                }
            Case Else ' Student
                Return New List(Of NavItem) From {
                    New NavItem("dashboard", "My Attendance", Icons.Dashboard),
                    New NavItem("history", "Attendance History", Icons.Records),
                    New NavItem("profile", "My Profile", Icons.Person)
                }
        End Select
    End Function

    ''' <summary>Creates the page for a key. Keys not built yet show a placeholder.</summary>
    Public Function Create(key As String) As PageBase
        Dim role = If(Session.User Is Nothing, "", Session.User.Role)
        Select Case key
            ' ---------- built ----------
            Case "subjects" : Return New SubjectsPage()
            Case "terms" : Return New TermsPage()
            Case "sections" : Return New SectionsPage()
            Case "teachers" : Return New TeachersPage()
            Case "students" : Return New StudentsPage()

            ' ---------- Module 2: Admin data ----------
            Case "users" : Return New PlaceholderPage("User Accounts", 2)
            Case "classes" : Return New PlaceholderPage("Classes", 2)
            Case "enrollment" : Return New PlaceholderPage("Enrollment", 2)

            ' ---------- Module 3: Attendance ----------
            Case "take" : Return New PlaceholderPage("Take Attendance", 3)
            Case "records" : Return New PlaceholderPage("Attendance Records", 3)
            Case "mystudents" : Return New PlaceholderPage("My Students", 3)

            ' ---------- Module 4: Student side ----------
            Case "history" : Return New PlaceholderPage("Attendance History", 4)
            Case "profile" : Return New PlaceholderPage("My Profile", 4)

            ' ---------- Module 5: Dashboards, notifications ----------
            Case "dashboard" : Return New PlaceholderPage(If(role = "Student", "My Attendance", "Dashboard"), 5)
            Case "notifications" : Return New PlaceholderPage("Notifications", 5)

            ' ---------- Module 6: Reports, settings, audit ----------
            Case "reports" : Return New PlaceholderPage("Reports", 6)
            Case "settings" : Return New PlaceholderPage("Settings", 6)
            Case "audit" : Return New PlaceholderPage("Audit Log", 6)

            Case Else : Return New PlaceholderPage(key, 0)
        End Select
    End Function

End Module
