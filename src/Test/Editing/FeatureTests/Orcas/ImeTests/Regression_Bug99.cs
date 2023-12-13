// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



using System;
using System.Globalization;
using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.IO;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Microsoft.Test.Threading;
using Microsoft.Test.Imaging;
using Microsoft.Test.Input;
using System.Windows.Threading;

namespace Test.Uis.TextEditing
{
    /// <summary>Tests Regression_Bug99 ...  xp japanese IME is ignoing the backspace character during an active composition, sometimes</summary>
    [Test(0, "IME", "Regression_Bug99", MethodParameters = "/TestCaseType=ImeRegression_Bug99", Timeout = 120, Keywords = "JapaneseIME")]
    public class ImeRegression_Bug99 : CustomTestCase
    {
        #region Public Members

        /// <summary>
        /// Entry point to test case
        /// </summary>
        public override void RunTestCase()
        {
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                if (!KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Japanese))
                {
                    Logger.Current.ReportResult(false, "Japanese IME was not installed");
                }
                //The key is included here since its use is specific to this case
                KeyboardLayout naturalInputJapanese = new KeyboardLayout("Japanese Natural Input",
                    "NA", "0411:E0010411;0411:{B3209488-CB34-4017-8E13-10CFCA2519FE}{75E61AD7-8D9E-4436-A0BE-2098C0DDA2C1}");
                KeyboardLayoutHelper.ActivateKeyboardLayout(naturalInputJapanese);
            }
            DispatcherHelper.DoEvents();

            _textbox = new TextBox();
            _richtextbox = new RichTextBox();
            StackPanel panel = new StackPanel();
            panel.Children.Add(_textbox);
            panel.Children.Add(_richtextbox); 
            TestWindow.Content = panel;

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new SingleParamDelegate(PerformBugRepro),_textbox);
        }

        delegate void SingleParamDelegate(UIElement ele);
        delegate void SimpleDelegate();

        #endregion

        #region Private Members

        private void PerformBugRepro(object element)
        {
            if (element is TextBox)
            {
                _textbox.Focus();
            }
            else
            {
                _richtextbox.Focus();
            }
            PerformInputOperations();
        }

        private void PerformInputOperations()
        {
            KeyboardInput.TypeString("aaaa");
            KeyboardInput.TypeString("{BACKSPACE 3}");
            KeyboardInput.TypeString("a");
            DispatcherHelper.DoEvents();
            //App crashes if bug exists and test framework will catch it
            if (_textbox.IsFocused)
            {
                QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 1), new SingleParamDelegate(PerformBugRepro), _richtextbox);
            }
            else
            {
                QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 1), (SimpleDelegate) delegate{Logger.Current.ReportSuccess();});
            }
        }

        #endregion

        #region private Data

        private RichTextBox _richtextbox = null;
        private TextBox _textbox = null;

        #endregion
    }
}