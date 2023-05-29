// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;  //For DataBind
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI;
//using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.PropertyEngine.RegressionTests
{
    [Test(0, "PropertyEngine.RegressionTests", TestCaseSecurityLevel.FullTrust, "BugVerification30")]
    public class BugVerification30 : TestCase
    {
        #region Constructor
        /******************************************************************************
        * Function:          BugVerification30 Constructor
        ******************************************************************************/
        /// <summary>
        /// 

        public BugVerification30()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Repro the 

        TestResult StartTest()
        {
            Avalon.Test.CoreUI.UtilityHelper.Utilities.StartRunAllTests("BugVerification30");
            Avalon.Test.CoreUI.UtilityHelper.Utilities.PrintTitle("Repro/Validate Bug 30");
            FrameworkElementFactory dockPanel = new FrameworkElementFactory(typeof(DockPanel));
            FrameworkElementFactory label = new FrameworkElementFactory(typeof(TextBlock), "LabelID");
            label.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            label.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            label.SetValue(DockPanel.DockProperty, Dock.Left);
            label.SetValue(TextBlock.MarginProperty, new Thickness(20));
            //public Binding(string path, BindingMode bindType, ObjectRef source) :
            // this(path, bindType, source, BindingDefaults.DefaultUpdateType, null, null, BindingDefaults.DefaultBindFlags) {}
            Binding bind = new Binding("LabelText");
            bind.RelativeSource = RelativeSource.TemplatedParent;
            label.SetBinding(TextBlock.TextProperty, bind);
            dockPanel.AppendChild(label);

            FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock), "ValueID");
            text.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            text.SetValue(TextBlock.TextWrappingProperty, TextWrapping.NoWrap);
            text.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            text.SetValue(DockPanel.DockProperty, Dock.Left);
            bind = new Binding("ValueText");
            bind.RelativeSource = RelativeSource.TemplatedParent;
            text.SetBinding(TextBlock.TextProperty, bind);  //<<<---Line A

            dockPanel.AppendChild(text);

            Brush b = new SolidColorBrush(SystemColors.GrayTextColor);
            b.Freeze();
            Trigger trigger = new Trigger();
            trigger.Property = Control.IsEnabledProperty;
            trigger.Value = false;
            trigger.Setters.Add(new Setter(TextBlock.ForegroundProperty, b, "LabelID"));
            trigger.Setters.Add(new Setter(TextBlock.TextProperty, "[not enabled]", "ValueID"));

            Style testStyle = new Style(typeof(Button));

            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = dockPanel;
            template.Triggers.Add(trigger);
            testStyle.Setters.Add(new Setter(Button.TemplateProperty, template));

            Button testButton = new Button();
            testButton.Style = testStyle;
            testButton.ApplyTemplate();

            //Default Value
            int count = VisualTreeHelper.GetChildrenCount(testButton);   
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(count == 1, "count ==1");
            DockPanel dp = (DockPanel)VisualTreeHelper.GetChild(testButton,0);
            count = VisualTreeHelper.GetChildrenCount(dp);              
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(count == 2, "count== 2");
            TextBlock label1 = (TextBlock)VisualTreeHelper.GetChild(dp,0);
            TextBlock text1 = (TextBlock)VisualTreeHelper.GetChild(dp,1);
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(label1.Text == "", "label1.Text == \"\"");
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(label1.Foreground != b, "label1.Foreground != b");
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(text1.Text == "", "text1.Text == \"\"");

            //Activate Trigger
            testButton.IsEnabled = false;
            count = VisualTreeHelper.GetChildrenCount(testButton);       
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(count == 1, "count == 1");
            dp = (DockPanel)VisualTreeHelper.GetChild(testButton,0);

            count = VisualTreeHelper.GetChildrenCount(dp);             
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(count == 2, "count == 2");          
            label1 = (TextBlock)VisualTreeHelper.GetChild(dp,0);
            text1 = (TextBlock)VisualTreeHelper.GetChild(dp,1);
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(label1.Text == "", "label1.Text == \"\"");
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(label1.Foreground == b, "label1.Foreground == b");

            if (text1.Text == "")
            {
                Avalon.Test.CoreUI.UtilityHelper.Utilities.PrintStatus("Bug Not Fixed");
            }
            else if (text1.Text == "[not enabled]")
            {
                Avalon.Test.CoreUI.UtilityHelper.Utilities.PrintStatus("Bug Fixed");
            }
            else
            {
                Avalon.Test.CoreUI.UtilityHelper.Utilities.PrintStatus("Strange Thing Happens here");
            }
            //When Fixed, Remove comment for the following line
            Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert(text1.Text == "[not enabled]", "");

            Avalon.Test.CoreUI.UtilityHelper.Utilities.StopRunAllTests();

            //Any test failures will be caught by Avalon.Test.CoreUI.UtilityHelper.Utilities.Assert.
            return TestResult.Pass;
        }
        #endregion
    }
}


