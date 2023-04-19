Imports System

Imports System.Windows
Imports System.Windows.Navigation
Imports System.Windows.Controls

Namespace TestDll

    Partial Public Class CodeBehindTest 
        Inherits System.Windows.Application
    
        
        Private Shared log As Microsoft.Test.Logging.TestLog
        Private _browserhostedapp As Boolean

        Protected Overrides Sub OnStartup(ByVal e As System.Windows.StartupEventArgs)

          log = Microsoft.Test.Logging.TestLog.Current; 

        _browserhostedapp = False
        If ( AppDomain.CurrentDomain.FriendlyName.ToString().Contains(".xbap") = True )        

            _browserhostedapp = True

        End If
        
          log.LogStatus("OnStartup method - done")

          MyBase.OnStartup(e)

        End Sub

        Protected Overrides Sub OnExit(ByVal e As System.Windows.ExitEventArgs)

            If ( _browserhostedapp = False )
            
             log.LogStatus("Shutting down application");

            End If
            
            MyBase.OnExit(e)
        End Sub
     
        Public ReadOnly Property BrowserHostedApp() As Boolean

            Get
                _browserhostedapp = True
            End Get
            
        End Property

    End Class
    
End Namespace