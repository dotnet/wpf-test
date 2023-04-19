Imports System

Imports System.Windows
Imports System.Windows.Navigation
Imports System.Windows.Controls

Namespace TestDll

    Partial Public Class SimpleImageCodeBehind
    
        
        Private Shared log As Microsoft.Test.Logging.TestLog

        Private Sub OnLoad(ByVal sender As System.Object, ByVal e As RoutedEventArgs)


            log = Microsoft.Test.Logging.TestLog.Current

            log.LogStatus("Image Loaded event.")

            log.Result = Microsoft.Test.Logging.TestResult.Pass

            If ( MyApplication.BrowserHostedApp = False )        
            
                System.Windows.Application.Current.Shutdown()
                
            Else
                
                log.LogStatus("Shutting down application");
                
            End If

            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring()
            
            
        End Sub

     
    End Class
    
End Namespace