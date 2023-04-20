Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports Microsoft.Test
Imports Microsoft.Test.Input

  Partial Class WithEventsClass

    Sub OnLoaded (ByVal Sender As Object, ByVal args As System.Windows.RoutedEventArgs)
        Console.WriteLine("About to click on the button.")
        UserInput.MouseLeftClickCenter (MyButton)
	Console.WriteLine("Just clicked on the button.")
    End Sub

    Private Sub HandleClick(ByVal Sender As Object, ByVal args As RoutedEventArgs) Handles MyButton.Click
	Console.WriteLine("Just entered click event handler.")
        Console.WriteLine("Verified click handler works when wired using WithEvents and Handles.")
	If (sender Is MyButton)
	    Console.WriteLine("Click event was sent by the correct button.")
            Application.Current.Shutdown(0)
	Else
	    Console.WriteLine("Click event was sent by some source other than the clicked button.")
            Application.Current.Shutdown(1)
	End If

    End Sub

  End Class

