
    Imports System
    Imports System.Windows
    Imports System.Windows.Navigation
    Imports System.Windows.Controls
    Imports System.IO
    Imports System.Windows.Markup
    Imports System.Reflection
    Imports Microsoft.Test.Serialization

    Public partial Class MyAppName
	 Inherits Application

        Private  Sub AppStartup(ByVal sender As Object, ByVal args As StartupEventArgs)
            Dim log As Microsoft.Test.Logging.TestLog =  New Microsoft.Test.Logging.TestLog("CompilerTest") 
            log.Stage = Microsoft.Test.Logging.TestStage.Initialize
            log.Stage = Microsoft.Test.Logging.TestStage.Run
 
            Dim fi As FieldInfo =  GetType(MyClassName).GetField("NameField", BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance) 
 
            
            If fi.IsPublic Then
            
                log.LogStatus("Field has correct access level defined by x:FieldModifier")
                log.Result = Microsoft.Test.Logging.TestResult.Pass
            Else
                log.LogStatus("Field has incorrect access level")
                log.Result = Microsoft.Test.Logging.TestResult.Fail
            End If
 
            log.Stage = Microsoft.Test.Logging.TestStage.Cleanup
            log.Close()
            System.Windows.Application.Current.Shutdown()
        End Sub

    End Class
 
    friend partial Class MyClassName
	 Inherits Page
        Private  Sub StubFunction()
        End Sub
    End Class

