// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;

namespace DRT
{
    public class GroupBoxSuite : DrtTestSuite
    {
        public GroupBoxSuite()
            : base("GroupBox")
        {
            Contact = "Microsoft";
        }

        #region Window setup and common hooks

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);

            DRT.LoadXamlFile("DrtGroupBox.xaml");
            DRT.Show(DRT.RootElement);

            List<DrtTest> tests = new List<DrtTest>();
            if (!DRT.KeepAlive)
            {
                tests.Add(new DrtTest(BasicTest));
            }
            return tests.ToArray();
        }

        #endregion

        #region BasicTest

        public void BasicTest()
        {
            AdornerDecorator decorator = (AdornerDecorator)DRT.RootElement;
            Label label = decorator.FindName("label") as Label;
            Button button = decorator.FindName("button") as Button;
            DRT.Assert(label.Target == button, "Label's target isn't the button.");
        }

        #endregion

    }
}
