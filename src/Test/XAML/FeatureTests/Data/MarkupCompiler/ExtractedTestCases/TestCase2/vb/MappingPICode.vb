
    Imports System
    Imports System.Windows
    Imports System.Windows.Navigation
    Imports System.Windows.Controls
    Imports Microsoft.Test

    <assembly:CLSCompliant(true)> _

    
    Namespace Local
        Public Class MyButton
            Inherits Button
        End Class
    End Namespace
    

    Namespace Harness
        Public Class OnStartupBVT
            Inherits System.Windows.Application

            Protected Overrides  Sub OnStartup(ByVal e As System.Windows.StartupEventArgs)
                Me.StartupUri = New Uri("MappingPIMarkup.xaml", UriKind.RelativeOrAbsolute)
                AddHandler Me.LoadCompleted, AddressOf OnLoad
                MyBase.OnStartup(e)
            End Sub
 
            Private  Sub OnLoad(ByVal sender As Object, ByVal e As NavigationEventArgs)
                Console.WriteLine("LoadCompleted event fired")
 
                ShutdownApp()
            End Sub
 
            Protected Overrides  Sub OnExit(ByVal e As ExitEventArgs)
                Console.WriteLine("Shutting down application")
            End Sub
 
            Private  Sub ShutdownApp()
                System.Windows.Application.Current.Shutdown(0)
                Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring()
            End Sub
        End Class

        Class EntryPoint
            <System.STAThread()> _ 
            Public Shared Function Main(ByVal args() As String) As Integer
                System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA)
                Dim app As OnStartupBVT =  New OnStartupBVT() 
                Return app.Run()
            End Function
        End Class
           
    End Namespace  

