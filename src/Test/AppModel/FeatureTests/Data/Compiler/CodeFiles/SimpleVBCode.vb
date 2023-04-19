Option Strict Off
Option Explicit On

Imports System
Imports System.Windows.Controls
Imports System.Windows.Navigation
Imports System.Windows
Imports MS.Internal.Utility
    
Namespace TestDll

    Public Class MYVBApp Inherits System.Windows.Application

        Private Shared log As Microsoft.Test.Logging.TestLog
        Private _browserhostedapp As Boolean
        
        Protected Overrides Sub OnStartup(ByVal e As System.Windows.StartupEventArgs)

            log = Microsoft.Test.Logging.TestLog.Current; 

            _browserhostedapp = False
            If ( AppDomain.CurrentDomain.FriendlyName.ToString().Contains(".xbap") = True )        
            
                _browserhostedapp = True
            
            End If

            this.StartupUri = new Uri(@"Simple.xaml",false,true)

            this.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(OnLoad)

            this.NavigationError += new NavigationErrorCancelEventHandler(this.OnNavigateError)

        End Sub

        Private Sub OnLoad(ByVal sender As Object, ByVal e As NavigationEventArgs)

            log.LogStatus("LoadCompleted event fired - Pass")
            log.Result = Microsoft.Test.Logging.TestResult.Pass

            ShutdownApp()

        End Sub

        Private Sub OnNavigateError(ByVal sender As Object, ByVal e As NavigationErrorCancelEventArgs)
            log.Result = Microsoft.Test.Logging.TestResult.Fail
            log.LogStatus("Navigation Error event fired")

            ShutdownApp()
        End Sub

        Protected Overrides Sub OnExit(ByVal e As ExitEventArgs)

            If (_browserhostedapp = False) Then

             log.LogStatus("Shutting down application");

            End If


        End Sub

        Private Sub ShutdownApp()

            If (_browserhostedapp = False) Then

                System.Windows.Application.Current.Shutdown();

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
