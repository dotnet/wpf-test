// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

using System.Windows.Controls;

namespace System.Windows.Controls
{
    public class LocalButton : Button
    {
    }
    
    public class CodeButton : Button
    {
        public static readonly DependencyProperty TextProperty = 
                DependencyProperty.Register(
                        "Text", 
                        typeof(string),
                        typeof(CodeButton),
                        new FrameworkPropertyMetadata(
                                string.Empty,
                                new PropertyChangedCallback(OnTextChanged)));

        public static readonly RoutedEvent TextChangedEventID = 
            EventManager.RegisterRoutedEvent("TextChanged",
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedEventHandler),
                                               typeof(CodeButton));

        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEventID, value); }
            remove { RemoveHandler(TextChangedEventID, value); }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CodeButton cb = d as CodeButton;
            //cb._textCacheValid = false;

            if (e.OldValue != null)
            {
                RoutedEventArgs re = new RoutedEventArgs();
                re.RoutedEvent=TextChangedEventID;
                re.Source = cb;
                cb.RaiseEvent(re);
                
            }
        }
    }
}
