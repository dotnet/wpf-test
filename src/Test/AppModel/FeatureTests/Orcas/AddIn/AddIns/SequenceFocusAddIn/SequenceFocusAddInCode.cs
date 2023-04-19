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
    [AddIn("SequenceFocus", Version = "1.0.0.0")]
    public class SequenceFocusAddInCode : AddInSequenceFocusView
    {
        #region Private Members

        private List<FocusItem> _list;
        private StackPanel _panel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public SequenceFocusAddInCode()
        {
            _list = new List<FocusItem>();

            _panel = new StackPanel();
            _panel.Name = "RootPanel";
            _panel.Height = 300;
            _panel.Width = 300;
            _panel.Background = new SolidColorBrush(Colors.CornflowerBlue);

            RichTextBox tb1 = new RichTextBox();
            tb1.Name = "AddIn_TextBox1";
            tb1.TabIndex = 1;
            tb1.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Item_GotFocus);

            TextBox tb2 = new TextBox();
            tb2.Name = "AddIn_TextBox2";
            tb2.TabIndex = 2;
            tb2.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Item_GotFocus);

            TextBox tb3 = new TextBox();
            tb3.Name = "AddIn_TextBox3";
            tb3.TabIndex = 3;
            tb3.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Item_GotFocus);
                        
            _panel.Children.Add(tb1);
            _panel.Children.Add(tb2);
            _panel.Children.Add(tb3);
        }

        #endregion

        #region Private Methods

        private void Item_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            if (element != null)
            {
                FocusItem item = new FocusItem();
                item.Name = element.Name;
                item.DateTime = DateTime.Now;
                lock (_list)
                {
                    _list.Add(item);
                }
            }
        }

        #endregion 

        #region Public Overrides

        public override FocusItem[] GetFocusSequence()
        {
            FocusItem[] items;
            lock (_list)
            {
                items = new FocusItem[_list.Count];
                _list.CopyTo(items);
            }
            return items;
        }

        public override void ClearSequence()
        {
            lock (_list)
            {
                _list.Clear();
            }
        }

        public override void Initialize(string addInParameters)
        {
            //
        }

        public override FrameworkElement GetAddInUserInterface()
        {
            return _panel;
        }

        #endregion

    }
}
