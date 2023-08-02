using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Reflection;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// AccessKeyTestBase
    /// </summary>
    public abstract class AccessKeyTestBase : XamlTest
    {
        public AccessKeyTestBase(string fileName, FlowDirection rootElementFlowDirection, string sourceName, string eventName, bool isEventFired, Key key)
            : base(fileName)
        {
            isAccessKeyButtonBaseEventTest = true;
            this.eventName = eventName;
            if (String.Compare(eventName, "Click") == 0)
            {
                routedEventArgs = new RoutedEventArgs(Button.ClickEvent);
            }
            else
            {
                throw new TestValidationException("Fail: unknown event.");
            }

            this.isEventFired = isEventFired;
            Initialize(rootElementFlowDirection, sourceName, key);
        }

        public AccessKeyTestBase(string fileName, FlowDirection rootElementFlowDirection, string sourceName, bool isAccessKeyPressed, Key key)
            : base(fileName)
        {
            this.isAccessKeyPressed = isAccessKeyPressed;
            Initialize(rootElementFlowDirection, sourceName, key);
        }

        private void Initialize(FlowDirection rootElementFlowDirection, string sourceName, Key key)
        {
            this.rootElementFlowDirection = rootElementFlowDirection;
            this.sourceName = sourceName;
            this.key = key;
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        bool isAccessKeyButtonBaseEventTest = false;
        FlowDirection rootElementFlowDirection;
        string sourceName;
        string eventName;
        RoutedEventArgs routedEventArgs;
        bool isEventFired;
        bool isAccessKeyPressed;
        Key key;
        FrameworkElement sourceElement;

        private TestResult Setup()
        {
            Status("Setup");

            Panel panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new NullReferenceException("Fail: the panel is null.");
            }
            System.Windows.Window.GetWindow(panel).FlowDirection = rootElementFlowDirection;

            sourceElement = (FrameworkElement)RootElement.FindName(sourceName);
            if (sourceElement == null)
            {
                throw new NullReferenceException("Fail: the sourceElement is null.");
            }

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);

            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            sourceElement = null;
            routedEventArgs = null;
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            using (AccessKeyValidator validator = AccessKeyValidator.GetValidator)
            {
                if (isAccessKeyButtonBaseEventTest)
                {
                    validator.Validate<RoutedEventArgs>((ButtonBase)sourceElement, eventName, routedEventArgs, isEventFired, key);
                }
                else
                {
                    validator.Validate(sourceElement, isAccessKeyPressed, key);
                }
            }

            return TestResult.Pass;
        }
    }
}


