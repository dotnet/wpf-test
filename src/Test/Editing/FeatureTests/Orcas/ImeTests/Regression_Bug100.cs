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

namespace Test.Uis.TextEditing
{    
    /// <summary>Tests regression of Regression_Bug100 ... Clicking on the More option of the IME menu throws an assert</summary>
    [Test(2, "IME", "ImeRegression_Bug100", MethodParameters = "/TestCaseType=ImeRegression_Bug100", Timeout = 120, Keywords = "JapaneseIME")]
    public class ImeRegression_Bug100 : CustomTestCase
    {
        #region Public Members

        /// <summary>
        /// Entry point to test case
        /// </summary>
        public override void RunTestCase()
        {
            KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Japanese);
            KeyboardInput.SetActiveInputLocale(InputLocaleData.JapaneseMsIme2002.Identifier);
            DispatcherHelper.DoEvents();

            _textbox = new TextBox();
            TestWindow.Content = _textbox;
            _textbox.Text = s_initialText;
            _textbox.SelectAll();

            QueueDelegate(OpenIme);
        }

        #endregion

        #region Private Members

        private void OpenIme()
        {
            Clipboard.Clear();
            _textbox.Focus();

            Rect rect = _textbox.GetRectFromCharacterIndex(3);
            System.Windows.Point result;
            //rect.Right correponds to X co-ord. since the rect is the center char,
            //we dont need to get the rects center along the x axis
            result = new System.Windows.Point(rect.Right, rect.Top + rect.Height / 2);
            result = ElementUtils.GetScreenRelativePoint(_textbox, result);

            MouseInput.RightMouseDown((int)result.X, (int)result.Y);
            MouseInput.RightMouseUp();
            DispatcherHelper.DoEvents();

            try
            {
                //Go up 3 options in the context menu using keyboard
                KeyboardInput.TypeString("{UP 3}{ENTER}");
                DispatcherHelper.DoEvents(1000);
            }
            catch (Exception)
            {
                Logger.Current.ReportResult(false, "Regression_Bug100 regressed");
            }

            Logger.Current.ReportSuccess();
        }

        #endregion

        #region private Data

        private TextBox _textbox = null;
        private static readonly string s_initialText = "\uff7f\uD480\uDC0B\ud842\udf9f\u8868\u9dd7";

        #endregion
    }
}
