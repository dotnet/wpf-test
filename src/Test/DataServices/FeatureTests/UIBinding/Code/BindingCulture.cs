// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using Microsoft.Test;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Checks to see that CultureInfo is inherited down to the Binding
    /// </description>
    /// </summary>



    [Test(1, "Binding", "BindingCulture")]

    public class BindingCulture : XamlTest
   {
        TextBlock _us_textblock, _fr_textblock, _tr_textblock;
        Button _button, _button2;
        public BindingCulture()
            : base(@"BindingCulture.xaml")
      {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(VerifyBindingCultures);

      }

        TestResult Init()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            _us_textblock = LogicalTreeHelper.FindLogicalNode(RootElement, "UStextblock") as TextBlock;

            if (_us_textblock == null)
            {
                LogComment("US TextBlock was not found in the Logical Tree!");
                return TestResult.Fail;
            }
            _fr_textblock = LogicalTreeHelper.FindLogicalNode(RootElement, "FRtextblock") as TextBlock;

            if (_fr_textblock == null) 
            {
                LogComment("FR TextBlock was not found in the Logical Tree!");
                return TestResult.Fail;
            }

            _tr_textblock = LogicalTreeHelper.FindLogicalNode(RootElement, "TRtextblock") as TextBlock;

            if (_tr_textblock == null) 
            {
                LogComment("TR TextBlock was not found in the Logical Tree!");
                return TestResult.Fail;
            }


            _button = LogicalTreeHelper.FindLogicalNode(RootElement, "button") as Button;

            if (_button == null)
            {
                LogComment("Button was not found in the Logical Tree!"); 
                return TestResult.Fail;
            }

            _button2 = LogicalTreeHelper.FindLogicalNode(RootElement, "button2") as Button;

            if (_button2 == null)
            {
                LogComment("Button was not found in the Logical Tree!");
                return TestResult.Fail;
            }


            return TestResult.Pass;

        }


        TestResult VerifyBindingCultures()
        {
            if (!_fr_textblock.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("fr-FR").DisplayName))
            {
                LogComment("TextBlock didn't pick up French Culture info.  Actual:  " + _fr_textblock.Text);
                return TestResult.Fail;
            }

            if (!_us_textblock.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("en-US").DisplayName))
            {
                LogComment("TextBlock didn't pick up English Culture info.  Actual:  " + _us_textblock.Text);
                return TestResult.Fail;
            }

            if (!_tr_textblock.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("tr-TR").DisplayName))
            {
                LogComment("TextBlock didn't pick up Turkish Culture info.  Actual:  " + _tr_textblock.Text);
                return TestResult.Fail;
            }

            TextBlock _textblock = Util.FindVisualByType(typeof(TextBlock), _button) as TextBlock;
            if (_textblock == null)
            {
                LogComment("TextBlock in DataTemplate was null!!");
                return TestResult.Fail;
            }
            if (!_textblock.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("fr-FR").DisplayName))
            {
                LogComment("TextBlock didn't pick up French Culture info.  Actual:  " + _textblock.Text);
                return TestResult.Fail;
            }
            _textblock = null;

            _textblock = Util.FindVisualByType(typeof(TextBlock), _button2) as TextBlock;

            if (_textblock == null)
            {
                LogComment("TextBlock in DataTemplate was null!!");
                return TestResult.Fail;
            }

            if (!_textblock.Text.EndsWith(CultureInfo.GetCultureInfoByIetfLanguageTag("en-US").DisplayName))
            {
                LogComment("TextBlock didn't pick up English Culture info.  Actual:  " + _textblock.Text);
                return TestResult.Fail;
            }


            return TestResult.Pass;
            
        }
   }

}
