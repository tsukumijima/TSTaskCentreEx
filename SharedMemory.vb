Imports System
Imports System.Runtime.InteropServices
Imports System.IO.MemoryMappedFiles
Imports System.Threading

Public Class SharedMemory

    Private _Size As Integer = Marshal.SizeOf(GetType(ServerTaskSharedInfo))
    Private _Task As Boolean = False
    Private _info As ServerTaskSharedInfo = New ServerTaskSharedInfo()

    Private _SharedMemoryName As String = "RecTask_Server_SharedMemory_"

    'rectaskは最大30だけど
    Private Const SharedMemoryChkCount As Integer = 15

    Public Sub New(ProcessID As UInteger)

        Dim i As Integer
        For i = 1 To SharedMemoryChkCount
            Try
                Using mmf As MemoryMappedFile = MemoryMappedFile.OpenExisting(_SharedMemoryName + i.ToString)
                    Dim mma As MemoryMappedViewAccessor = mmf.CreateViewAccessor(0, _Size)
                    mma.Read(0, Me._info)
                End Using
                If Me._info.Task.ProcessID = ProcessID Then
                    Me._Task = True
                    Exit For
                End If
            Catch ex As Exception
            End Try
        Next

    End Sub

    Public ReadOnly Property TaskID() As UInteger
        Get
            If Me._Task = False Then
                Return 0
            Else
                Return Me._info.Task.TaskID
            End If
        End Get
    End Property

    Public Structure ServerTaskSharedInfo
        Public Header As TaskSharedInfoHeader
        Public Task As TaskInfo
        Public StatisticsUpdateCount As ULong
        Public Statistics As StreamStatistics
        Public TotTime As ULong
    End Structure

    Public Structure TaskSharedInfoHeader
        Public Size As UInteger
        Public Version As UInteger
    End Structure

    Public Structure TaskInfo
        Public TaskID As UInteger
        Public Type As TaskType
        Public ProcessID As UInteger
        Public Version As UInteger
        Public State As TaskState
    End Structure

    Enum TaskType
        TASK_TYPE_SERVER = 0
        TASK_TYPE_CLIENT
    End Enum

    Enum TaskState
        TASK_STATE_STARTING = 0
        TASK_STATE_RUNNING
        TASK_STATE_ENDING
    End Enum

    Public Structure StreamStatistics
        Public SignalLevel As Single
        Public BitRate As UInteger
        Public InputPacketCount As ULong
        Public ErrorPacketCount As ULong
        Public DiscontinuityCount As ULong
        Public ScramblePacketCount As ULong
    End Structure

End Class
