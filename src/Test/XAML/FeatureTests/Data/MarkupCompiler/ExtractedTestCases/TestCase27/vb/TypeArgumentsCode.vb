
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
            System.Windows.Application.Current.Shutdown()
        End Sub

    End Class
 
    namespace TypeArguments
                                      Public Class PageFunction(Of T1, T2)
                                      End Class
                                  end namespace
