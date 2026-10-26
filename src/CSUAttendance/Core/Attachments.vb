Imports System.IO
Imports System.Windows.Forms

''' <summary>
''' Proof files for Late / Absent / Excused marks (excuse letter, medical certificate...).
''' Files are copied into an "attachments" folder next to the app; the database only keeps the file name
''' (attendance_records.attachment_path), so the folder can be backed up or moved together with the app.
''' </summary>
Public Module Attachments

    ''' <summary>File types the teacher can attach (photos and PDF).</summary>
    Public Const FileFilter As String = "Photos and PDF (*.jpg;*.jpeg;*.png;*.bmp;*.pdf)|*.jpg;*.jpeg;*.png;*.bmp;*.pdf"

    ''' <summary>Largest file accepted, in megabytes.</summary>
    Public Const MaxMegabytes As Integer = 10

    Public ReadOnly Property Folder As String
        Get
            Return Path.Combine(Application.StartupPath, "attachments")
        End Get
    End Property

    Public Function FullPath(fileName As String) As String
        Return Path.Combine(Folder, fileName)
    End Function

    ''' <summary>"" when the file is fine, otherwise why it cannot be attached.</summary>
    Public Function Problem(sourcePath As String) As String
        If Not File.Exists(sourcePath) Then Return "The file no longer exists."
        If New FileInfo(sourcePath).Length > MaxMegabytes * 1024L * 1024L Then Return $"The file is larger than {MaxMegabytes} MB."
        Return ""
    End Function

    ''' <summary>Copies the file into the attachments folder as "{prefix}_{timestamp}.ext" and returns the stored file name.</summary>
    Public Function Store(sourcePath As String, prefix As String) As String
        Directory.CreateDirectory(Folder)
        Dim name = $"{prefix}_{Date.Now:yyyyMMddHHmmssfff}{Path.GetExtension(sourcePath).ToLowerInvariant()}"
        File.Copy(sourcePath, FullPath(name))
        Return name
    End Function

    ''' <summary>Opens a stored attachment with the default Windows viewer (Photos, PDF reader...).</summary>
    Public Sub Open(owner As Control, fileName As String)
        If fileName = "" Then Return
        Dim target = If(Path.IsPathRooted(fileName), fileName, FullPath(fileName))
        If Not File.Exists(target) Then
            Notice.Warn(owner, "Attachment not found", $"The file ""{Path.GetFileName(target)}"" is missing from the attachments folder:{vbCrLf}{Folder}")
            Return
        End If
        Try
            Process.Start(New ProcessStartInfo(target) With {.UseShellExecute = True})
        Catch ex As Exception
            Notice.Fail(owner, "Cannot open the attachment", "Windows has no program to open this file.", ex.Message)
        End Try
    End Sub

End Module
