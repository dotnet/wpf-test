// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DrtPagefunctionTest
{
    public partial class MarkupPF1 : PageFunction<Boolean>
    {
        private Boolean _returnData;
        private ReturnEventArgs<Boolean> _returnArgs;
        private String _returnTextFromChild;

        public MarkupPF1( ) 
        {
            InitializeComponent( );
            _returnData = false;
        }

        private void Init(object sender, RoutedEventArgs args)
        {
            _returnData = true;
            _returnArgs = new ReturnEventArgs<Boolean>(_returnData);

            Console.WriteLine("returnArgs is {0} and {1}.", _returnData, _returnArgs);
        }

        internal TestTextBox TestTextBox
        {
            get { return testTextBox; }
        }

        internal String ReturnTextFromChild
        {
            get { return _returnTextFromChild; }
        }

        void MarkupPF2_Return(object sender, ReturnEventArgs<String> args)
        {
            _returnTextFromChild = (String)args.Result;

        }

        public void LaunchMarkupPF2(NavigationWindow nw )
        {
            MarkupPF2 pf2 = new MarkupPF2();
            pf2.Return += new ReturnEventHandler<string>(MarkupPF2_Return);
            nw.Navigate(pf2);
        }
    }
}
