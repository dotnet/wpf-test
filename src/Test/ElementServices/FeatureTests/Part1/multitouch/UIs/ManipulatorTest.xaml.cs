// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for ManipulatorTest.xaml
    /// 
    /// 

    public partial class ManipulatorTest : Window
    {
        #region Fields

        private readonly ManipulableItem _item;

        #endregion

        #region Constructor

        public ManipulatorTest()
        {
            InitializeComponent();

            this._item = new ManipulableItem();
            this._item.Container = LayoutRoot;

            ItemHost.Children.Add(this._item);
        }

        #endregion

    }
}
