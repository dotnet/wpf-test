// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace DrtPagefunctionTest
{
    public partial class MarkupPF2 : PageFunction<String>
    {
        private String _returnData;
        private ReturnEventArgs<String> _returnArgs;

        public MarkupPF2() 
        {
            _returnData = "Page is Initialized";
            InitializeComponent( );
        }

        internal void End( )
        {
            _returnData = "The PF2 returns to its parent PageFunction";

            _returnArgs = new ReturnEventArgs<String>(_returnData);

            OnReturn(_returnArgs);
        }

        internal void Cancel( )
        {
            _returnData = "PF2 is Cancelled";

            _returnArgs = new ReturnEventArgs<String>(_returnData);

            OnReturn(_returnArgs);
        }

    }
}
