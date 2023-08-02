using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// SelectorEventTestBase
    /// </summary>
    public abstract class SelectorEventTestBase : XamlTest
    {
        public SelectorEventTestBase(Dictionary<string, string> variation, string testInfo)
            : base(variation["XamlFileName"])
        {
            this.variation = variation;
            this.testInfo = testInfo;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        protected Dictionary<string, string> variation;
        private string testInfo;
        protected Selector source;

        private TestResult Setup()
        {
            Status("Setup");
            LogComment(testInfo);
            string controlName = variation["ControlName"];

            source = (Selector)RootElement.FindName(controlName);
            if (source == null)
            {
                throw new ArgumentNullException("Fail: the source element " + controlName + " is null.");
            }

            System.Windows.Window.GetWindow(source).FlowDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), variation["FlowDirection"]);

            ItemsControlModifier modifier = new ItemsControlModifier(source);
            modifier.Modify(Convert.ToBoolean(variation["IsVirtualizing"]), Convert.ToBoolean(variation["IsItemVirtualizing"]), (VirtualizationMode)Enum.Parse(typeof(VirtualizationMode), variation["VirtualizationMode"]));

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            
            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public abstract TestResult RunTest();
    }
}


