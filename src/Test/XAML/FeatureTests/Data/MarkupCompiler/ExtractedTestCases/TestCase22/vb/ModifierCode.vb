
    Imports System
    Imports System.Windows
    Imports System.Windows.Navigation
    Imports System.Windows.Controls
    Imports System.IO
    Imports System.Windows.Markup
    Imports System.Reflection

    Public partial Class MyAppName
	 Inherits Application

        Private  Sub AppStartup(ByVal sender As Object, ByVal args As StartupEventArgs)
            Dim fi As FieldInfo =  GetType(MyClassName).GetField("NameField", BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance) 
 
            If fi.IsFamily Then
                Console.WriteLine("Field has correct access level defined by x:FieldModifier")
                Application.Current.Shutdown(0)
            Else
                Console.WriteLine("Field has incorrect access level")
                Application.Current.Shutdown(1)
            End If
        End Sub

    End Class
 
    Public partial Class MyClassName
	 Inherits Page
        Private  Sub StubFunction()
        End Sub
    End Class

