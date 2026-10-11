''' <summary>The user currently signed in.</summary>
Public Class CurrentUser
    Public Property UserId As Integer
    Public Property Username As String = ""
    Public Property FullName As String = ""
    Public Property Role As String = ""          ' Admin | Teacher | Student
    Public Property TeacherId As Integer         ' 0 when not a teacher
    Public Property StudentId As Integer         ' 0 when not a student
End Class

Public Module Session

    Public Property User As CurrentUser

    Public ReadOnly Property IsAdmin As Boolean
        Get
            Return User IsNot Nothing AndAlso User.Role = "Admin"
        End Get
    End Property

    Public ReadOnly Property IsTeacher As Boolean
        Get
            Return User IsNot Nothing AndAlso User.Role = "Teacher"
        End Get
    End Property

    Public ReadOnly Property IsStudent As Boolean
        Get
            Return User IsNot Nothing AndAlso User.Role = "Student"
        End Get
    End Property

    ''' <summary>The user id to store in recorded_by / updated_by / audit columns.</summary>
    Public ReadOnly Property UserId As Integer
        Get
            Return If(User Is Nothing, 0, User.UserId)
        End Get
    End Property

    Public Sub Clear()
        User = Nothing
    End Sub

End Module
