
    Imports System
    Imports System.Windows
    Imports System.Windows.Navigation
    Imports System.Windows.Controls
    Imports System.IO
    Imports System.Windows.Markup
    Imports Microsoft.Test
    Imports Microsoft.Test.Serialization
 
    Public partial Class Application__
	 Inherits Application
        Private  Sub AppStartup(ByVal sender As Object, ByVal args As StartupEventArgs)
        End Sub
    End Class
 
    Public partial Class MyAppName
	 Inherits Application
        Private  Sub AppStartup(ByVal sender As Object, ByVal args As StartupEventArgs)
            Dim fs As FileStream =  New FileStream(".\\obj\\release\\ClassMarkup.baml",FileMode.Open) 
            Dim reader As BamlReaderWrapper =  New BamlReaderWrapper(fs) 
            reader.Read()
            reader.Read()
            Dim name As String =  reader.Name 
 
            If name = "MyRootNamespace.MyClassName" Then
                Console.WriteLine("BAML has correct name as defined by x:Subclass")
                Application.Current.Shutdown(0)
            Else 
                Console.WriteLine("BAML record has incorrect value: " + name)
                Application.Current.Shutdown(1)
            End If
 
            Dim instance As MyRootNamespace.MyClassName = new MyRootNamespace.MyClassName()

            instance.Title="Foo"
        End Sub
    End Class

    
