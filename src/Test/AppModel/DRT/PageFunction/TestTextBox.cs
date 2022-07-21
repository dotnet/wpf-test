// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;


namespace DrtPagefunctionTest
{
    /// <summary>
    /// TestTextBox, Derive from TextBox with ability to track the change of Text DP value.
    /// </summary>
    public class TestTextBox : TextBox
    {
        private int    _iCountTextSet;
        private string _oldText;
        private string _newText;


        /// <summary>
        /// ctor
        /// </summary>
        public TestTextBox()
            : base()
        {
            _iCountTextSet = 0;
            _oldText = null;
            _newText = null;
        }

        ///
        /// override OnPropertyChanged
        ///
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //  always call base.OnPropertyChanged, otherwise Property Engine will not work.
            base.OnPropertyChanged(e);

            if (e.Property == TextProperty)
            {
                _oldText = (string)e.OldValue;
                _newText = (string)e.NewValue;

                _iCountTextSet++;
            }
        }

        // 
        // Track how many differnt Text values are set for TextProperty in this TextBox.
        //
        internal int CountTextSet
        {
            get { return _iCountTextSet; }
        }

        // 
        // Get the previous Text value.
        //
        internal string PreviousText
        {
            get { return _oldText; }
        }


        // 
        // Get the new Text value.
        //
        internal string NewText
        {
            get { return _newText; }
        }
    }
}
