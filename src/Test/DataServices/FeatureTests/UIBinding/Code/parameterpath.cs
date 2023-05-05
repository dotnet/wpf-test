// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{

    /// <summary>
    /// <description>
    /// This test PathParmeters in PropertyPath class.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>



    [Test(3, "Binding", "ParameterPathTest")]
    public class ParameterPathTest : WindowTest
    {
        public ParameterPathTest()
        {
            InitializeSteps += new TestStep(datasetTest_InitializeSteps);
            RunSteps += new TestStep(VerifyPathDependencyProperties);
            RunSteps += new TestStep(datasetTest_InitializeSteps1);
            RunSteps += new TestStep(VerifyPathUpdatedWithTreeChange);
            RunSteps += new TestStep(BadDPParameters); //

        }

        TextBlock _text;
        DockPanel _dockpanel;
        TestResult datasetTest_InitializeSteps()
        {
            _dockpanel = new DockPanel();

            _dockpanel.Name = "DockPanelID";
            _text = new TextBlock();
            this.Window.DataContext = this.Window;
            _text.Name = "TextID";
            Binding _bind = new Binding();

            _bind.Path = new PropertyPath("(1).(0)");
            _bind.Path.PathParameters.Add(FrameworkElement.NameProperty);
            _bind.Path.PathParameters.Add(Window.ContentProperty);
            _text.SetBinding(TextBlock.TextProperty, _bind);
            _text.SetValue(DockPanel.DockProperty, Dock.Top);
            _dockpanel.Children.Add(_text);

            Window.Content = _dockpanel;

            return TestResult.Pass;
        }

        TestResult VerifyPathDependencyProperties()
        {
            Status("Verifying that DependencyProperties worked");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);
            if (_text.Text != "DockPanelID")
            {
                LogComment("TextBlock.Text is incorrected Expected: 'DockPanelID' Actual: '" + _text.Text + "'");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult datasetTest_InitializeSteps1()
        {
            Status("Changing Window Content to see if Change notifications work.");
            _dockpanel.Children.Remove(_text);
            Window.Content = _text;
            return TestResult.Pass;
        }
        TestResult VerifyPathUpdatedWithTreeChange()
        {
            Status("Verifying that DependencyProperties worked");
            if (_text.Text != "TextID")
            {
                LogComment("TextBlock.Text is incorrected Expected: 'TextID' Actual: '" + _text.Text + "'");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult BadDPParameters()
        {

            Binding b = new Binding();
            b.Path = new PropertyPath("(1).(0)");
            b.Path.PathParameters.Add(Window.ContentProperty);
            b.Path.PathParameters.Add(FrameworkElement.NameProperty);
            _text.SetBinding(TextBlock.TextProperty, b);
            if (_text.Text != "")
            {
                LogComment("Fail - TextBlock _text should be empty");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }
}
