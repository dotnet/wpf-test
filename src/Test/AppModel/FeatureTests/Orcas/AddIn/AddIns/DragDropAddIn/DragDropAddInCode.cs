// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Windows.Input;

namespace Microsoft.Test.AddIn
{
    [AddIn("DragDrop", Version = "1.0.0.0")]
    public class DragDropAddInCode : AddInDragDropView
    {
        #region Private Members

        private StackPanel _panel;
        private TextBox _tb;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DragDropAddInCode()
        {
            _panel = new StackPanel();
            _panel.Name = "RootPanel";
            _panel.Height = 300;
            _panel.Width = 300;
            _panel.Background = new SolidColorBrush(Colors.CornflowerBlue);

            _tb = new TextBox();
            _tb.Name = "AddIn_TextBox1";
            _tb.TabIndex = 1;
                        
            _panel.Children.Add(_tb);

        }

        #endregion

        #region Public Overrides

        public override void Initialize(string addInParameters)
        {
            //
        }

        public override FrameworkElement GetAddInUserInterface()
        {
            return _panel;
        }

        public override string GetTextBoxText()
        {
            return _tb.Text;            
        }

        #endregion

    }
}
