Imports System.Threading
Imports System.Windows.Threading


Class Application

    Dim brokenSemaphore As Semaphore

    Public Sub New()
        If (Environment.GetCommandLineArgs().Length = 2) Then
            If (Environment.GetCommandLineArgs()(1).ToLowerInvariant() = "pauseonstartup") Then
                brokenSemaphore = New Semaphore(1, 1)
                brokenSemaphore.WaitOne()
                MyBase.Dispatcher.BeginInvoke(DispatcherPriority.Send, CType(dispatcherHangingDelegate(), System.Delegate))
            End If
        End If
    End Sub

    ' Defines an instance method.
    Public Function dispatcherHangingDelegate() As Object
        brokenSemaphore.WaitOne()
        Return Nothing
    End Function 'myStringMethod





End Class
