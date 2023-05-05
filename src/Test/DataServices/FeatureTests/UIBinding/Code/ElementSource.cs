// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests binding the background of a button to its Name (btn.Name="Red")
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Binding", "ElementSource")]
    public class ElementSource: WindowTest
    {
        Button _btn;
        Binding _bind;
        public ElementSource()
        {
            InitializeSteps += new TestStep(InitializeTestCase);
            RunSteps += new TestStep(BindElementSource);
        }

        private TestResult InitializeTestCase()
        {
            Status("Creating button and adding to tree.");

            NameScope.SetNameScope(Window, new NameScope());
            WaitForPriority(DispatcherPriority.Background);

            NameScope.GetNameScope(Window);

            _btn = new Button();
            _btn.Name = "Red";
            Window.RegisterName("Red", _btn);
            _btn.Content = "Hola!";
            _btn.Width = 50;
            _btn.Height = 50;
            Window.Content = _btn;
            WaitForPriority(DispatcherPriority.Render);
            //This WaitFor shouldn't be here
            // WaitFor(2000);

            return TestResult.Pass;
        }

        /// <summary>
        /// Validates binding to ElementSource Name
        /// </summary>
        private TestResult BindElementSource()
        {
            Status("Creating binding.");
            _bind = new Binding("Name");
            _bind.ElementName = "Red";
            _btn.SetBinding(Button.BackgroundProperty, _bind);
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Verifying bound value.");
            if (_btn.Background != Brushes.Red)
            {
                LogComment("Bound value is incorrect.  Expected:  Red background.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
