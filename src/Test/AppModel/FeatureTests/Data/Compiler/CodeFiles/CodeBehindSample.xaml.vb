Imports System

Imports System.Windows
Imports System.Windows.Navigation
Imports System.Windows.Controls

Namespace TestDll

    Partial Public Class CodeBehindBVT
    
        
        Private Shared log As Microsoft.Test.Logging.TestLog
        Private _browserhostedapp As Boolean

        Protected Overrides Sub OnStartup(ByVal e As System.Windows.StartupEventArgs)

          log = Microsoft.Test.Logging.TestLog.Current

            _browserhostedapp = False
            If ( AppDomain.CurrentDomain.FriendlyName.ToString().Contains(".xbap") = True )        
            
                _browserhostedapp = True
            
            End If

            log.LogStatus("Hooking up Navigation events")

            StartupUri = New System.Uri("Simple.xaml", UriKind.RelativeOrAbsolute)
            log.LogStatus("OnStartup method - done")

            AddHandler LoadCompleted, AddressOf OnLoad

            MyBase.OnStartup(e)

        End Sub

        Private Sub OnLoad(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.NavigationEventArgs)

            log.LogStatus("OnLoad event fired")
            log.Result = Microsoft.Test.Logging.TestResult.Pass
            ShutdownApp()
        End Sub

        Protected Overrides Sub OnExit(ByVal e As ExitEventArgs)

            If (_browserhostedapp = False) Then

                log.LogStatus("Shutting down application")

            End If

        End Sub

        Private Sub ShutdownApp()

            If (_browserhostedapp = False) Then

                System.Windows.Application.Current.Shutdown()

            End If

            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring()

        End Sub
     
        Public ReadOnly Property BrowserHostedApp() As Boolean

            Get
                _browserhostedapp = True
            End Get
            
        End Property

    End Class
    
End Namespace