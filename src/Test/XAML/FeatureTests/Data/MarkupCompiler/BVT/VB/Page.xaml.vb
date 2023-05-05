Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Collections

Partial Class VBBVT
    Private casePassed As Boolean
    
    Public Sub New() 
        InitializeComponent
        casePassed = False
    End Sub 'New
    
    
    Sub Done(ByVal myTestResult As Boolean) 
        If myTestResult = True Then
            System.Windows.Application.Current.Shutdown(0)
        Else
            System.Windows.Application.Current.Shutdown(1)
        End If
    End Sub 'Done
    
    
    Sub OnVBLoaded(ByVal sender As Object, ByVal e As RoutedEventArgs) 
        If Not CheckNonLocalizedText() Then
            Done(False)
        ElseIf Not CheckWireupContents() Then
            Done(False)
        ElseIf Not CheckWireupEventHandlers() Then
            Done(False)
        ElseIf Not CheckCodeBesideBehind() Then
            Done(False)
        
        ' Add checking id, property, event, etc. on locally and externally defined controls
        Else
            Done(True)
        End If
    
    End Sub 'OnLoaded
    
    
    Sub HandleClickCodeBehind(ByVal sender As Object, ByVal e As RoutedEventArgs) 
        Console.WriteLine("We verified click handler defined in Code-Behind works.")
        casePassed = True
    
    End Sub 'HandleClickCodeBehind
    
    
    Sub MethodInCodeBehind() 
        Console.WriteLine("We verified a method defined in Code-Behind can be called from Code-Beside.")
        casePassed = True
    
    End Sub 'MethodInCodeBehind
    
    
    Sub HandleButton3WireupClick(ByVal sender As Object, ByVal e As RoutedEventArgs) 
        Console.WriteLine("We verified click handler on Button 3 works.")
        
        If sender Is Wireup.Children(2) Then
            Console.WriteLine("Click event was sent by the correct button.")
            casePassed = True
        Else
            Console.WriteLine("Click event was sent by some source other than the clicked button.")
            casePassed = False
        End If
    
    End Sub 'HandleButton3WireupClick
     
    
    Sub HandleButton4WireupClick(ByVal sender As Object, ByVal e As RoutedEventArgs) 
        Console.WriteLine("We verified click handler on Button 4 works.")
        If sender Is Button4Wireup Then
            Console.WriteLine("Click event was sent by the correct button.")
            casePassed = True
        Else
            Console.WriteLine("Click event was sent by some source other than the clicked button.")
            casePassed = False
        End If
    
    End Sub 'HandleButton4WireupClick
     
    
    ' This checks for the contents of each button, so any changes made in either markup or code need to be synced.
    ' Code behind also references content and elemnt type explicitly.
    Function CheckWireupContents() As Boolean 
        ' Index will be used to track which button we are looking at.
        Dim i As Integer = 0
        ' Content of each button, corresponding to the XAML declarations above. Need to keep them synchronized.
        Dim contents As String() =  {"No ID, No Event Handler", "ID, No Event Handler", "No ID, Event Handler", "ID, Event Handler"}
        
        ' Walk through each UIElement child and verify it is both a button and that it's content matches the expectation.
        Dim myElement As UIElement
        For Each myElement In  Wireup.Children
            If myElement.GetType().FullName = "System.Windows.Controls.Button" Then
                Dim myButton As Button = DirectCast(myElement, Button)
		If CType(myButton.Content, String) = contents(i) Then
                        Console.WriteLine("Button " + CType((i + 1), String) + "'s content of '" + myButton.Content.ToString + "' matched expectation.")
		Else
                        Console.WriteLine("Button " + CType((i + 1), String) + "'s content of '" + myButton.Content.ToString + "' did not match expectation of '" + contents(i) + "'.")
                  Return False
                End If
            Else
                    Console.WriteLine("Element " + CType((i + 1), String) + " was of type '" + myElement.GetType().FullName + "' when it should have been of type System.Windows.Controls.Button.")
                Return False
            End If
            i += 1
        Next myElement
        
        ' Refer to content explicity for those with ID
        If Button2Wireup.Content.ToString = "ID, No Event Handler" Then
            Console.WriteLine("Content of Button 2 as expected, able to reference by ID.")
	Else
            Console.WriteLine("Content of Button 2 was '" + Button2Wireup.Content.ToString + "' did not match expectation of 'ID, No Event Handler'.")
            Return False
        End If
        
        ' Refer to content explicity for those with ID
        If Button4Wireup.Content.ToString = "ID, Event Handler" Then
	    Console.WriteLine("Content of Button 4 as expected, able to reference by ID.")
        Else
            Console.WriteLine("Content of Button 4 was '" + Button4Wireup.Content.ToString + "' did not match expectation of 'ID, Event Handler'.")
	    Return False
        End If
        
        Return True
    
    End Function 'CheckWireupContents
    
    
    Function CheckWireupEventHandlers() As Boolean 
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        Microsoft.Test.Input.UserInput.MouseLeftClickCenter(CType(Wireup.Children(2), FrameworkElement))
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        If Not casePassed Then
            Console.WriteLine("Clicking on third button did not trigger event.")
            Return False
        End If
        casePassed = False
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        Microsoft.Test.Input.UserInput.MouseLeftClickCenter(CType(Wireup.Children(3), FrameworkElement))
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        If Not casePassed Then
            Console.WriteLine("Clicking on fourth button did not trigger event.")
            Return False
        End If
        casePassed = False
        
        ' also do for element with explicit id
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        Microsoft.Test.Input.UserInput.MouseLeftClickCenter(Button4Wireup)
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        If Not casePassed Then
            Console.WriteLine("Clicking on fourth button did not trigger event.")
            Return False
        End If
        casePassed = False
        
        Return True
    
    End Function 'CheckWireupEventHandlers
    
    
    Function CheckNonLocalizedText() As Boolean 
        Console.WriteLine("Able to compile xaml and code with non-localized characters")
        
        If DirectCast(GlobalText.Content, String) = "Text using masculin, 按钮, ボタン, 단추, кнопка, κουμπί" Then
            Console.WriteLine("Able to verify foreign character contents of a control")
        Else
            Console.WriteLine("Unable to verify foreign character contents of a control")
            Return False
        End If
        
        If DirectCast(masculin.Content, String) = "ID using Language1" Then
            Console.WriteLine("Able to verify foreign character ID of a control")
        Else
            Console.WriteLine("Unable to verify foreign character ID of a control")
            Return False
        End If
        
        If DirectCast(按钮.Content, String) = "ID using Language2" Then
            Console.WriteLine("Able to verify foreign character ID of a control")
        Else
            Console.WriteLine("Unable to verify foreign character ID of a control")
            Return False
        End If
        
        If DirectCast(ボタン.Content, String) = "ID using Language3" Then
            Console.WriteLine("Able to verify foreign character ID of a control")
        Else
            Console.WriteLine("Unable to verify foreign character ID of a control")
            Return False
        End If
        
        If DirectCast(단추.Content, String) = "ID using Language4" Then
            Console.WriteLine("Able to verify foreign character ID of a control")
        Else
            Console.WriteLine("Unable to verify foreign character ID of a control")
            Return False
        End If
        
        If DirectCast(кнопка.Content, String) = "ID using Language5" Then
            Console.WriteLine("Able to verify foreign character ID of a control")
        Else
            Console.WriteLine("Unable to verify foreign character ID of a control")
            Return False
        End If
        
        If DirectCast(κουμπί.Content, String) = "ID using Language6" Then
            Console.WriteLine("Able to verify foreign character ID of a control")
        Else
            Console.WriteLine("Unable to verify foreign character ID of a control")
            Return False
        End If
        
        Console.WriteLine("Verified non-localized characters accepted in both IDs and content.")
        Return True
    
    End Function 'CheckNonLocalizedText
    
    
    Function CheckCodeBesideBehind() As Boolean 
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        Microsoft.Test.Input.UserInput.MouseLeftClickCenter(CodeBeside)
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        If Not casePassed Then
            Console.WriteLine("Clicking on button did not trigger event defined in Code Beside.")
            Return False
        End If
        casePassed = False
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        Microsoft.Test.Input.UserInput.MouseLeftClickCenter(CodeBehind)
        Avalon.Test.ComponentModel.QueueHelper.WaitTillQueueItemsProcessed()
        If Not casePassed Then
            Console.WriteLine("Clicking on button did not trigger event defined in Code Behind.")
            Return False
        End If
        casePassed = False
        
        MethodInCodeBeside()
        If Not casePassed Then
            Console.WriteLine("Was unable to call methods in code behind from code beside and vice versa.")
            Return False
        End If
        casePassed = False
        
        Return True
    
    End Function 'CheckCodeBesideBehind
    
    
    Sub HandleClick(ByVal sender As Object, ByVal e As RoutedEventArgs) 
    
    End Sub 'HandleClick
End Class 'VBBVT