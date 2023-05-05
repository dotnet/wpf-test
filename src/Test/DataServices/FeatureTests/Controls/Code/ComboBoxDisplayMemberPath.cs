// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Verifies that DisplayMemberPath works correctly with ComboBox.
	/// </description>
	/// <relatedBugs>
    /// </relatedBugs>
	/// </summary>

    [Test(2, "Controls", "ComboBoxDisplayMemberPath")]
	public class ComboBoxDisplayMemberPath : XamlTest
	{
        private ComboBox _comboBox;

        public ComboBoxDisplayMemberPath()
            : base(@"ComboBoxDisplayMemberPath.xaml")
		{
            RunSteps += new TestStep(VerifyContentOfTextBox);
        }

        TestResult VerifyContentOfTextBox()
        {
            Status("VerifyContentOfTextBox");

            _comboBox = (ComboBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "comboBox"));
            _comboBox.SelectedIndex = 1;

            TabItem selectedTabItem = (TabItem)(_comboBox.SelectedItem);

            TextBox textBox = (TextBox)(Util.FindVisualByType(typeof(TextBox), _comboBox));
            if ((string)(textBox.Text) != (string)(selectedTabItem.Content))
            {
                LogComment("Fail");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }
}

