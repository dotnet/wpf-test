
    Imports System
    Imports System.Windows
    Imports System.Windows.Navigation
    Imports System.Windows.Controls
    Imports System.IO
    Imports System.Windows.Markup
    Imports Microsoft.Test.Serialization
 
    Public partial Class Application__
	 Inherits Application
        Private  Sub AppStartup(ByVal sender As Object, ByVal args As StartupEventArgs)
        End Sub
    End Class
 
    Public partial Class MyAppName
	 Inherits Application
        Private  Sub AppStartup(ByVal sender As Object, ByVal args As StartupEventArgs)
            Dim log As Microsoft.Test.Logging.TestLog =  New Microsoft.Test.Logging.TestLog("CompilerTest") 
            log.Stage = Microsoft.Test.Logging.TestStage.Initialize
            log.Stage = Microsoft.Test.Logging.TestStage.Run
 
            Dim fs As FileStream =  New FileStream(".\\obj\\release\\ClassMarkup.baml",FileMode.Open) 
            Dim reader As BamlReaderWrapper =  New BamlReaderWrapper(fs) 
            reader.Read()
            reader.Read()
            Dim name As String =  reader.Name 
 
            If name = "MyRootNamespace.MyNamespace.MyClassNameLogic" Then
                log.LogStatus("BAML has correct name as defined by x:Subclass")
                log.Result = Microsoft.Test.Logging.TestResult.Pass
            Else 
                log.LogStatus("BAML record has incorrect value: " + name)
                log.Result = Microsoft.Test.Logging.TestResult.Fail
            End If
            log.Stage = Microsoft.Test.Logging.TestStage.Cleanup
            log.Close()
            System.Windows.Application.Current.Shutdown()
 
            Dim instance As MyNamespace.MyClassNameLogic = new MyNamespace.MyClassNameLogic()

            instance.Title="Foo"
        End Sub
    End Class

    Namespace MyNamespace
                                    Public Class MyClassNameLogic
                                      inherits MyClassName
                                    End Class
                                  End Namespace
