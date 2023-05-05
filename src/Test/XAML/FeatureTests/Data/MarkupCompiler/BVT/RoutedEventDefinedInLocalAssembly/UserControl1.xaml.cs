// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace RoutedEventDefinedInLocalAssembly
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public static RoutedEvent MyRequestRoutedEvent = EventManager.RegisterRoutedEvent("MyRequest",
    RoutingStrategy.Direct,
    typeof(EventHandler<MyRequestEventArgs>),
    typeof(UserControl1));

        public event RoutedEventHandler MyRequest
        {
            add { base.AddHandler(MyRequestRoutedEvent, value); }
            remove { base.RemoveHandler(MyRequestRoutedEvent, value); }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            this.RaiseEvent(new MyRequestEventArgs(MyRequestRoutedEvent));
        }
    }

    public class MyRequestEventArgs : RoutedEventArgs
    {
        public MyRequestEventArgs(RoutedEvent ev)
            : base(ev)
        {
        }
    }
}
