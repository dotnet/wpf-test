Imports System.Windows.Automation
Imports System.Windows
Imports System.Reflection
Imports System.Threading
Imports System

Class Window1
    Private Shared currentSplashScreen As SplashScreen
    Private Shared splashTimeoutMillis As TimeSpan

    Private Sub DoSplashScreenAPITest(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim cmdLineArgs As String() = Environment.GetCommandLineArgs()
        Dim resourceName As String = "splash.bmp"
        Dim assembly As String = "none"
        Dim forceClose As Boolean = False
        If ((Not cmdLineArgs Is Nothing) AndAlso (cmdLineArgs.Length = 4)) Then
            resourceName = cmdLineArgs(1)
            [assembly] = cmdLineArgs(2)
            forceClose = Boolean.Parse(cmdLineArgs(3))
            Application.Current.MainWindow.Title = String.Concat(New String() {"Using:", resourceName, " from ", IIf(([assembly].ToLowerInvariant = "none"), " self", [assembly]).ToString(), " and ", IIf(forceClose, "", "not").ToString(), " forcibly closing splash screen"})
        End If
        Dim name As String = DirectCast(sender, Button).Name
        If (Not name Is Nothing) Then
            If (name = "SplashTest1") Then
                Me.TestWPFSplashScreen(resourceName, [assembly], TimeSpan.FromMilliseconds(3000), False, forceClose, False)
                Return
            End If
            If (name = "SplashTest2") Then
                Me.TestWPFSplashScreen(resourceName, [assembly], TimeSpan.FromMilliseconds(3000), True, forceClose, False)
                Return
            End If
            If (name = "SplashTest3") Then
                Me.TestWPFSplashScreen(resourceName, [assembly], TimeSpan.FromMilliseconds(3000), False, False, True)
                Return
            End If
            If (name = "SplashTest4") Then
                Me.TestWPFSplashScreen(resourceName, [assembly], TimeSpan.FromMilliseconds(3000), False, False, False)
                Dim newThread As New Thread(AddressOf Me.DoWork)
                newThread.Start()
            End If
            If (name = "SplashTest5") Then
                Try
                    Dim splashScreen As SplashScreen = New SplashScreen("splash.bmp")
                    splashScreen.Close(New System.TimeSpan(0, 0, 0))
                    Me.Title = "No Null Ref observed for closing un-Show()-n SplashScreen"
                Catch ex As System.NullReferenceException
                    Me.Title = "ERROR : Null Ref Exception caught"
                End Try
            End If
        End If

    End Sub

    Private Sub DoWork()
        Window1.currentSplashScreen.Close(TimeSpan.FromMilliseconds(200))
    End Sub


    Private Function TestWPFSplashScreen(ByVal resourceName As String, ByVal assemblyName As String, ByVal closeTimeSpan As TimeSpan, ByVal autoClose As Boolean, ByVal forceClose As Boolean, ByVal topMost As Boolean) As Boolean
        splashTimeoutMillis = closeTimeSpan
        Dim screen As SplashScreen = Nothing

        Dim isSplashScreen As PropertyCondition = New PropertyCondition(AutomationElement.ClassNameProperty, "SplashScreen")
        If (assemblyName.ToLowerInvariant = "none") Then
            screen = New SplashScreen(resourceName)
        Else
            screen = New SplashScreen(Assembly.LoadFile(System.IO.Path.GetFullPath(assemblyName)), resourceName)
        End If
        If (screen Is Nothing) Then
            Return False
        End If
        currentSplashScreen = screen

#if TESTBUILD_CLR40
        screen.Show(autoClose, topMost)
#end if 
#if TESTBUILD_CLR20
        screen.Show(autoClose)
#end if 

        ' Start closing the dialog as early as possible and do it twice.
        ' This ensures 1) it's OK to call close twice (should be)
        ' and 2) no issues with the dispatcher timer used in fadeout being disposed twice.
        If forceClose Then
            screen.Close(TimeSpan.Zero)
            screen.Close(TimeSpan.Zero)
        End If
        Return True
    End Function
    Private Sub CloseSplashScreen(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If (Not currentSplashScreen Is Nothing) Then
            currentSplashScreen.Close(splashTimeoutMillis)
        End If
    End Sub

End Class
