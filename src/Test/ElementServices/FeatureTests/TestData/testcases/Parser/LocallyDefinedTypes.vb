Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports  System.Windows.Markup
[assembly: XmlnsDefinition(
   "loc",
   "LocallyDefined")]
Namespace LocallyDefined
    Public Class LocallyDefinedButton 
        Inherits Button
        
        Public Static Readonly LocallyDefinedPropertyProperty As DependencyProperty = 
            DependencyProperty.Register("LocallyDefinedProperty", 
                                        GetType(string),
                                        GetType(LocallyDefinedButton),
                                        new FrameworkPropertyMetadata("NotSet",
                                        new PropertyChangedCallback(OnLocallyDefinedPropertyChanged)))

        Public Static Readonly LocallyDefinedPropertyChangedEventID As RoutedEvent = 
            EventManager.RegisterRoutedEvent("LocallyDefinedPropertyChanged",
                                               RoutingStrategy.Bubble,
                                               GetType(RoutedEventHandler),
                                               GetType(LocallyDefinedButton))

        Public Event LocallyDefinedPropertyChanged As RoutedEventHandler
        

        Public Static Readonly  LocallyDefinedEventID As RoutedEvent=
            EventManager.RegisterRoutedEvent("LocallyDefinedEvent",
                                       RoutingStrategy.Tunnel,
                                       GetType(RoutedEventHandler),
                                       GetType(LocallyDefinedButton))


        Public Event  LocallyDefinedEvent As RoutedEventHandler


        Public Property  LocallyDefinedProperty As String
        
            Get
              Return (String)GetValue(LocallyDefinedPropertyProperty)
            End Get
            Set
                SetValue(LocallyDefinedPropertyProperty, value)
            End Set
        End Property


        Private Sub static OnLocallyDefinedPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            LocallyDefinedButton cb = d as LocallyDefinedButton
            String oldLocallyDefinedProperty = e.OldValue As String 
            String newLocallyDefinedProperty = e.NewValue As String

            If oldLocallyDefinedProperty <> null
            Then
                RoutedEventArgs ev = new RoutedEventArgs()
                ev.RoutedEvent = LocallyDefinedPropertyChangedEventID
                ev.Source = cb
                RaiseEvent cb.RaiseEvent(ev)
            End Id
        End Sub

        Public Property EventInvoked As Bool
        
            Set
                _eventInvoked = value
            End Set 
            Get
                Return _eventInvoked
            End Get
        End Property

        private bool _eventInvoked = false
    End Class
End Namespace
