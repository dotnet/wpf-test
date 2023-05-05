Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports Microsoft.Test.Logging
Imports Avalon.Test.ComponentModel.Utilities
Imports Microsoft.Test.Input

Namespace Avalon.Test.ComponentModel
  Partial Class CodeLanguagesVBClass

    Dim log As New Microsoft.Test.Logging.TestLog("CodeLanguageVB")

    Sub OnLoaded (ByVal Sender As Object, ByVal args As System.Windows.RoutedEventArgs)
    	log.LogStatus("About to click on the button.")
        UserInput.MouseLeftClickCenter (testButton)
	log.LogStatus("Just clicked on the button.")
    End Sub

    Sub HandleClick(ByVal Sender As Object, ByVal args As RoutedEventArgs)
	log.LogStatus("Just entered click event handler.")
        log.LogStatus("Verified click handler works when written in VB.")
	If (sender Is testButton)
	    log.LogStatus("Click event was sent by the correct button.")
	    log.Result = TestResult.Pass
	Else
	    log.LogStatus("Click event was sent by some source other than the clicked button.")
	    log.Result = TestResult.Fail
	End If
	log.Close()
	System.Windows.Application.Current.Shutdown()
    End Sub

  End Class
End Namespace