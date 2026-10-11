Imports System.Security.Cryptography

''' <summary>
''' Password hashing with PBKDF2 (HMAC-SHA256, 10,000 iterations, 16-byte salt).
''' Stored format:  PBKDF2$iterations$saltBase64$hashBase64
''' Plain-text passwords are never stored.
''' </summary>
Public Module PasswordHasher

    Private Const Iterations As Integer = 10000
    Private Const SaltSize As Integer = 16
    Private Const HashSize As Integer = 32

    Public Function HashPassword(password As String) As String
        Dim salt(SaltSize - 1) As Byte
        Using rng = RandomNumberGenerator.Create()
            rng.GetBytes(salt)
        End Using
        Dim hash = Derive(password, salt, Iterations)
        Return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}"
    End Function

    Public Function VerifyPassword(password As String, stored As String) As Boolean
        If String.IsNullOrEmpty(stored) Then Return False
        Dim parts = stored.Split("$"c)
        If parts.Length <> 4 OrElse parts(0) <> "PBKDF2" Then Return False
        Dim iter = Integer.Parse(parts(1))
        Dim salt = Convert.FromBase64String(parts(2))
        Dim expected = Convert.FromBase64String(parts(3))
        Dim actual = Derive(password, salt, iter)
        Return FixedTimeEquals(expected, actual)
    End Function

    ''' <summary>Minimum password policy: 8+ characters with at least one letter and one digit.</summary>
    Public Function IsStrongEnough(password As String, ByRef reason As String) As Boolean
        If password Is Nothing OrElse password.Length < 8 Then
            reason = "Password must be at least 8 characters."
            Return False
        End If
        If Not password.Any(AddressOf Char.IsLetter) OrElse Not password.Any(AddressOf Char.IsDigit) Then
            reason = "Password must contain letters and numbers."
            Return False
        End If
        reason = ""
        Return True
    End Function

    Private Function Derive(password As String, salt As Byte(), iter As Integer) As Byte()
        Using kdf As New Rfc2898DeriveBytes(password, salt, iter, HashAlgorithmName.SHA256)
            Return kdf.GetBytes(HashSize)
        End Using
    End Function

    Private Function FixedTimeEquals(a As Byte(), b As Byte()) As Boolean
        If a.Length <> b.Length Then Return False
        Dim diff = 0
        For i = 0 To a.Length - 1
            diff = diff Or (CInt(a(i)) Xor CInt(b(i)))
        Next
        Return diff = 0
    End Function

End Module
