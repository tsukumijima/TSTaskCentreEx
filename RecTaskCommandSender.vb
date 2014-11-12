Imports System.IO
Imports System.IO.Pipes
Imports System.Text

Public Class RecTaskCommandSender

    Private _ServerPipeName As String
    Private _TaskID As String

    Public Sub New(TaskID As Integer)

        _TaskID = TaskID
        _ServerPipeName = "RecTask_Server_Pipe_" + _TaskID.ToString()

    End Sub

    Public Function Send(Command As String, ServerName As String) As String

        Dim ret As String = ""
        Try
            Using Pipe As NamedPipeClientStream = New NamedPipeClientStream(ServerName, _ServerPipeName, PipeDirection.InOut, PipeOptions.None)
                Pipe.Connect(1000)
                If Pipe.IsConnected Then
                    Dim msg As Byte() = Encoding.Unicode.GetBytes(Command)
                    Dim wsize As Byte() = New Byte() {CByte(msg.Length), 0, 0, 0}
                    Dim sendSize As UInteger = CUInt(msg.Length)
                    Pipe.Write(wsize, 0, wsize.Length)
                    Pipe.WaitForPipeDrain()
                    Pipe.Write(msg, 0, sendSize)

                    Dim receiveField(3 * 1024 * 1024) As Byte
                    Dim reclen As Integer = Pipe.Read(receiveField, 0, receiveField.Length)
                    If reclen = 4 Then
                        Dim datalen As Long = 0
                        datalen = BitConverter.ToInt32(receiveField, 0)
                        reclen = Pipe.Read(receiveField, 0, datalen)
                        ret = Encoding.Unicode.GetString(receiveField, 0, reclen).Replace(vbLf, vbCrLf)
                    Else
                        ret = Encoding.Unicode.GetString(receiveField, 4, reclen - 4).Replace(vbLf, vbCrLf)
                    End If
                End If

            End Using

        Catch ex As Exception
            Return Nothing
        End Try
        Return ret
    End Function

End Class

