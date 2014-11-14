Imports System.Diagnostics

Module Main


    Sub Main(args As String())
        Dim Pid As Integer = -1
        Dim num As Integer = 0
        Dim Com As String = ""
        Dim Opt As String = ""
        Dim Pcn As String = "."

        Dim comList As List(Of String) = New List(Of String)()
        If args Is Nothing Then
            Console.Write("引数がありません。" + vbCrLf)
            Return
        End If
        If args.Length = 0 Then
            Console.Write("引数がありません。" + vbCrLf)
            Return
        End If

        Dim i As Integer = 0
        For i = 0 To My.Application.CommandLineArgs.Count - 1
            Dim s1 As String = My.Application.CommandLineArgs(i)
            If s1.IndexOf("/") = 0 Then
                i = i + 1
                Try
                    Dim s2 As String = My.Application.CommandLineArgs(i)
                    If s2.IndexOf("/") <> 0 Then
                        comList.Add(s1 + " " + s2)
                        'Console.Write(s1 + " " + s2 + vbCrLf)
                    Else
                        i = i - 1
                    End If
                Catch ex As Exception
                    Exit For
                End Try

            End If
        Next

        For i = 0 To comList.Count - 1
            If comList(i).ToLower.IndexOf("/p") = 0 Then            'プロセスID
                Pid = Convert.ToInt32(comList(i).Replace("/p ", ""))
                Continue For
            End If
            If comList(i).ToLower.IndexOf("/c") = 0 Then
                Com = comList(i).Replace("/c ", "")
                Continue For
            End If
            If comList(i).ToLower.IndexOf("/o") = 0 Then
                Opt = comList(i).Replace("/o ", "")
                Continue For
            End If
            If comList(i).ToLower.IndexOf("/t") = 0 Then
                num = Convert.ToInt32(comList(i).Replace("/t ", ""))
                Continue For
            End If
            If comList(i).ToLower.IndexOf("/n") = 0 Then
                Pcn = comList(i).Replace("/n ", "")
                Continue For
            End If
        Next

        Select Case Com.ToLower
            Case "list"
                Dim ps() As Process = Process.GetProcessesByName("rectask")
                For Each p In ps
                    Dim sm As SharedMemory = New SharedMemory(p.Id)
                    If sm.TaskID() > 0 Then
                        Console.Write("ID:" + p.Id.ToString("d08") + " TaskID:" + sm.TaskID.ToString("d02") + vbCrLf)
                    End If
                Next p
            Case Else
                If Pid = -1 And num = 0 Then
                    Console.WriteLine("PIDもしくはTaskIDの指定がありません。")
                    Exit Select
                End If
                If (num = 0) Then
                    Dim sm As SharedMemory = New SharedMemory(Pid)
                    num = sm.TaskID()
                End If
                If num > 0 Then
                    Dim rtcs As RecTaskCommandSender = New RecTaskCommandSender(num)
                    Console.Write(rtcs.Send(Com + vbCrLf + Opt, Pcn))
                End If
        End Select
    End Sub

End Module
