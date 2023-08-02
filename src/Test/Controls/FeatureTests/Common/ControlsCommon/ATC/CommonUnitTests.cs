

//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------


#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xml;
using System.ComponentModel;
using System.Windows.Markup;

using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Validations;

#endregion

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// generic unit test for actions, validations
    /// </summary>
    public class ControlUnitTest : AssertUnitTestBase<FrameworkElement>
    {
        protected override void Run(FrameworkElement testObject, XmlElement variation)
        {
            DoSetUp(testObject, variation);
            IEventTester eventTester = GetEventTester(variation);
            if (eventTester != null)
            {
                eventTester.Hook(testObject);
                try
                {
                    DoActions(testObject, variation);
                    string failed = eventTester.Verify();
                    if (failed != null)
                    {
                        Assert.Fail(failed + " event tests not passed");
                    }
                }
                finally
                {
                    eventTester.Unhook();
                }
            }
            else
                DoActions(testObject, variation);

            DoValidations(testObject, variation);
        }

        private static IEventTester GetEventTester(XmlElement variation)
        {
            XmlElement eventTesterXml = variation["Events"] == null ? variation["Event"] : variation["Events"];
            IEventTester eventTester = null;
            if (eventTesterXml != null)
            {
                eventTester = XtcTestHelper.GetEventTesterFromXml(eventTesterXml);
            }
            return eventTester;
        }

        private static void DoValidations(FrameworkElement testObject, XmlElement variation)
        {
            if (variation["Validations"] != null)
            {
                string failedValidation = XtcTestHelper.DoValidations(testObject, variation["Validations"]);
                Assert.AssertTrue(variation.Name + ".Validations." + failedValidation + " failed",
                    string.Empty == failedValidation);
            }
        }

        private static void DoActions(FrameworkElement testObject, XmlElement variation)
        {
            if (variation["Actions"] != null)
            {
                XtcTestHelper.DoActions(testObject, variation["Actions"]);
                if (XmlHelper.GetAttribute(variation, "WaitAfterAction", true))
                    QueueHelper.WaitTillQueueItemsProcessed();
            }
        }

        private static void DoSetUp(FrameworkElement testObject, XmlElement variation)
        {
            if (variation["SetUp"] != null)
            {
                XtcTestHelper.DoActions(testObject, variation["SetUp"]);
                if (XmlHelper.GetAttribute(variation, "WaitAfterSetUp", true))
                    QueueHelper.WaitTillQueueItemsProcessed();
            }
        }
    }

    /// <summary>
    /// generic unit test for running test with steps
    /// </summary>
    [InnerPropertyParser("Steps")]
    public class StepsUnitTest : AssertUnitTestBase<FrameworkElement>
    {
        #region Steps

        public List<ITestStep> Steps
        {
            get { return _steps; }
        }

        private List<ITestStep> _steps = new List<ITestStep>();

        #endregion

        protected override void Run(FrameworkElement testObject, XmlElement variation)
        {
            XmlParserHelper.Parse(this, variation);
            Test(testObject);
        }

        private void Test(FrameworkElement testObject)
        {
            foreach (ITestStep step in Steps)
            {
                step.Do(testObject);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
        }
    }

    /// <summary>
    /// generic unit test with its steps written xaml code
    /// </summary>
    [XamlParser("Step")]
    public class XamlStepsUnitTest : AssertUnitTestBase<FrameworkElement>
    {
        #region Step

        public ITestStep Step
        {
            get { return _step; }
            set { _step = value; }
        }

        private ITestStep _step = null;

        #endregion

        protected override void Run(FrameworkElement testObject, XmlElement variation)
        {
            XmlParserHelper.Parse(this, variation);
            Test(testObject);
        }

        private void Test(FrameworkElement testObject)
        {
            Assert.AssertTrue("Step should not be null", Step != null);
            Step.Do(testObject);
        }
    }

    /// <summary>
    /// container for ITestStep
    /// </summary>
    [ContentProperty("Items")]
    public sealed class Steps : FrameworkContentElement, ITestStep
    {
        #region Finder AttachedProperty

        public static IFinder GetFinder(DependencyObject element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            return (IFinder)element.GetValue(FinderProperty);
        }

        public static void SetFinder(DependencyObject element, IFinder finder)
        {
            if (element == null) { throw new ArgumentNullException("element"); }

            element.SetValue(FinderProperty, finder);
        }

        public static readonly DependencyProperty FinderProperty =
                DependencyProperty.RegisterAttached(
                        "Finder",
                        typeof(IFinder),
                        typeof(Steps));

        #endregion

        #region Items CLR property

        private List<ITestStep> _testSteps = new List<ITestStep>();

        public List<ITestStep> Items
        {
            get
            {
                return _testSteps;
            }
        }

        #endregion

        #region private methods

        private IFinder GetAttachedFinder(object o)
        {
            DependencyObject d = o as DependencyObject;
            if (d != null)
            {
                return d.GetValue(Steps.FinderProperty) as IFinder;
            }
            return null;
        }

        #endregion

        #region public methods

        public void Do(object testObject)
        {
            for (int i = 0; i < LoopCount; ++i)
            {
                foreach (ITestStep step in Items)
                {
                    IFinder finder = GetAttachedFinder(step as object);
                    if (finder != null)
                        step.Do(finder.Find(testObject));
                    else
                        step.Do(testObject);
                    QueueHelper.WaitTillQueueItemsProcessed();
                }
            }
        }

        #endregion

        #region LoopCount

        public int LoopCount
        {
            get { return _loopCount; }
            set { _loopCount = value; }
        }

        private int _loopCount = 1;

        #endregion
    }

    /// <summary>
    /// adapter for IAction and IValidation
    /// </summary>
    [ContentProperty("Parameters")]
    public class LegacyAdapter : TestObject
    {
        #region Class

        public Type Class
        {
            get { return _class; }
            set 
            {
                if (typeof(IAction).IsAssignableFrom(value) || typeof(IValidation).IsAssignableFrom(value))
                {
                    _class = value;
                }
                else
                    throw new ArgumentException("only IAction and IValidation class supported");
            }
        }

        private Type _class;

        #endregion

        #region Parameters

        public List<string> Parameters
        {
            get { return _parameters; }
        }

        private List<string> _parameters = new List<string>();

        #endregion

        #region TestObject Members

        protected override void DoCore()
        {
            if (Class != null)
            {
                object obj = Activator.CreateInstance(Class);
                if (obj is IAction)
                {
                    ((IAction)obj).Do(Target as FrameworkElement, Parameters.ToArray());
                }
                else if (obj is IValidation)
                {
                    Assert.AssertTrue("Validation [" + Class.Name + "] Failed", ((IValidation)obj).Validate(Target, Parameters.ToArray()));
                }
            }
        }

        #endregion
    }
}
