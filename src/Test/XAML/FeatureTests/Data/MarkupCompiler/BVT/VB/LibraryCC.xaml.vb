 ' Namespace must be the same as what you set in project file

Imports System
Imports System.Windows
Imports System.Windows.Controls


Namespace CustomControl
Public Class LibraryVBButton
    Inherits Button
    Private Shared IntegerProperty As DependencyProperty = DependencyProperty.Register("Integer", GetType(Integer), GetType(LibraryVBButton))
    
    
    Public Property [Integer]() As Integer 
        Get
            If Not _IntegerCacheValid Then
                _IntegerCache = DirectCast((GetValue(IntegerProperty)), Integer)
                _IntegerCacheValid = True
            End If
            Return _IntegerCache
        End Get
        Set
            SetValue(IntegerProperty, value)
        End Set
    End Property
     
    
    
    Private Shared Function OnGetInteger(ByVal d As DependencyObject) As Object 
        Return CType(d, LibraryVBButton).Integer
    
    End Function 'OnGetInteger
    
    
    Private Shared Sub OnIntegerPropertyInvalidated(ByVal d As DependencyObject) 
        Dim ctrl As LibraryVBButton = CType(d, LibraryVBButton)
        ctrl._IntegerCacheValid = False
        Dim oldInteger As Integer = ctrl._IntegerCache
        Dim newInteger As Integer = DirectCast(ctrl.GetValue(LibraryVBButton.IntegerProperty), Integer)
        
        If oldInteger <> newInteger Then
            Dim e As New DependencyPropertyChangedEventArgs(LibraryVBButton.IntegerProperty, oldInteger, newInteger)
            ctrl.OnIntegerChanged(e)
        End If
    
    End Sub 'OnIntegerPropertyInvalidated
    
    
    'protected virtual void OnDraggingChanged(DependencyPropertyChangedEventArgs e)
    Private Sub OnIntegerChanged(ByVal e As DependencyPropertyChangedEventArgs) 
    
    End Sub 'OnIntegerChanged
    
    
    Private _IntegerCache As Integer ' Cache for Integer property
    Private _IntegerCacheValid As Boolean = False
End Class 'LibraryVBButton
End Namespace