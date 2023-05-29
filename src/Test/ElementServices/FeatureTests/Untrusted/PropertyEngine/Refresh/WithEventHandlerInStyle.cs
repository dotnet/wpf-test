// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithEventHandlerInStyleTest
{
    /******************************************************************************
    * CLASS:          WithEventHandlerInStyle
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "WithEventHandlerInStyle")]
    public class WithEventHandlerInStyle : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("ApiTestEventSetter")]
        [Variation("ApiTestStyleSetters")]
        [Variation("ApiTestFefHandlers")]
        [Variation("TestStyleEventHandler")]
        [Variation("TestStyleEventHandlerWithBasedOn")]
        [Variation("TestStyleEventHandlerWithMultipleHandlerOnSameEvent")]
        [Variation("TestStyleEventHandlerWithInstanceHandlers")]
        [Variation("TestFefEventHandler")]
        [Variation("TestFefEventHandlerWithBasedOn")]
        [Variation("TestFefEventHandlerWithMultipleHandlerOnSameEvent")]
        [Variation("TestFefEventHandlerWithInstanceHandlers")]
        [Variation("TestBothStyleAndFefEventHandler")]
        [Variation("TestFefEventHandlerWithClassHandlers")]
//        [Variation("TestStyleEventHandlerWithClassHandlers")]
//        [Variation("TestFefEventHandlerWithClassHandlersSecondPass")]

        /******************************************************************************
        * Function:          WithEventHandlerInStyle Constructor
        ******************************************************************************/
        public WithEventHandlerInStyle(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            TestWithStyleEventHandler test = new TestWithStyleEventHandler();

            Utilities.StartRunAllTests("WithEventHandlerInStyle");

            switch (_testName)
            {
                case "ApiTestEventSetter":
                    test.ApiTestEventSetter();
                    break;
                case "ApiTestStyleSetters":
                    test.ApiTestStyleSetters();
                    break;
                case "ApiTestFefHandlers":
                    test.ApiTestFefHandlers();
                    break;
                case "TestStyleEventHandler":
                    test.TestStyleEventHandler();
                    break;
                case "TestStyleEventHandlerWithBasedOn":
                    test.TestStyleEventHandlerWithBasedOn();
                    break;
                case "TestStyleEventHandlerWithMultipleHandlerOnSameEvent":
                    test.TestStyleEventHandlerWithMultipleHandlerOnSameEvent();
                    break;
                case "TestStyleEventHandlerWithInstanceHandlers":
                    test.TestStyleEventHandlerWithInstanceHandlers();
                    break;
                case "TestFefEventHandler":
                    test.TestFefEventHandler();
                    break;
                case "TestFefEventHandlerWithBasedOn":
                    test.TestFefEventHandlerWithBasedOn();
                    break;
                case "TestFefEventHandlerWithMultipleHandlerOnSameEvent":
                    test.TestFefEventHandlerWithMultipleHandlerOnSameEvent();
                    break;
                case "TestFefEventHandlerWithInstanceHandlers":
                    test.TestFefEventHandlerWithInstanceHandlers();
                    break;
                case "TestBothStyleAndFefEventHandler":
                    test.TestBothStyleAndFefEventHandler();
                    break;
                case "TestFefEventHandlerWithClassHandlers":
                    //Section: With Class handlers (Keep the sequence) No change after this point.
                    //(1) add Class handler for DockPanel
                    test.TestFefEventHandlerWithClassHandlers();
                    //(2) Now add class handler for TestControl
                    test.TestStyleEventHandlerWithClassHandlers();
                    //(3) Now test when (1),(2) registers class handler for both DockPanel and Classhandler
                    test.TestFefEventHandlerWithClassHandlersSecondPass();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            Utilities.StopRunAllTests();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestWithStyleEventHandler
    ******************************************************************************/
    /// <summary></summary>
    public class TestWithStyleEventHandler
    {
        private static System.Text.StringBuilder s_sbTrace = new System.Text.StringBuilder();
        private static RoutedEvent s_eventB1;
        private static RoutedEvent s_eventT1;
        private static RoutedEvent s_eventD1;

        private static void AssertHelper(string expected, string message)
        {
            if (s_sbTrace.ToString() == expected)
            {
                Utilities.Assert(true, message + System.Environment.NewLine + "We get: " + expected);
            }
            else
            {
                Utilities.Assert(false, "Expected is: " + expected + " but actual is: " + s_sbTrace.ToString() + ". " + message);
            }

            //Clean up sbTrace for less code
            s_sbTrace.Remove(0, s_sbTrace.Length);
        }


        /// <summary>
        /// Register one Bubble RoutedEvent and one Tunnel RoutedEvent
        /// </summary>
        static TestWithStyleEventHandler()
        {
            s_eventB1 = EventManager.RegisterRoutedEvent("B1", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TestWithStyleEventHandler));
            s_eventT1 = EventManager.RegisterRoutedEvent("T1", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(TestWithStyleEventHandler));
            s_eventD1 = EventManager.RegisterRoutedEvent("D1", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TestWithStyleEventHandler));
        }

        #region Event Handler
        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            return;
        }

        private void StyleEventHandler1(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("style1");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        private void StyleEventHandler2(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("style2");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        private void StyleEventHandler3(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("style3");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        private void FefEventHandler1(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("Fef");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        private void FefEventHandler2(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("Fef2");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        private void FefEventHandler3(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("Fef3");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        private void InstanceEventHandler(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("Inst");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }




        #endregion

        /// <summary>
        /// Test EventSetter class
        /// </summary>
        public void ApiTestEventSetter()
        {
            Utilities.PrintTitle("API Test: EventSetter is derived from SetterBase");
            //Ctor
            EventSetter myEventSetter = new EventSetter();
            //Property: Event
            Utilities.Assert(myEventSetter.Event == null, "Property: Event, initial value is n");
            myEventSetter.Event = System.Windows.Documents.Hyperlink.RequestNavigateEvent;
            Utilities.Assert(myEventSetter.Event == System.Windows.Documents.Hyperlink.RequestNavigateEvent, "Property: Event, set and get value.");
            //Property: Handler
            Utilities.Assert(myEventSetter.Handler == null, "Property: Handler, initial value is a null");
            myEventSetter.Handler = new System.Windows.Navigation.RequestNavigateEventHandler(OnNavigate);

            //does not work in PD6
            //Utilities.Assert(myEventSetter.Handler == new System.Windows.Navigation.RequestNavigateEventHandler(OnNavigate), "Property: Handler, set and get value").

            //Property: HandledEventsToo
            Utilities.Assert(myEventSetter.HandledEventsToo == false, "Property: HandledEventsToo, initial value is false");
            myEventSetter.HandledEventsToo = true;
            Utilities.Assert(myEventSetter.HandledEventsToo == true, "Property: HandledEventsToo, set and get value");
        }

        /// <summary>
        /// Style.Setters provide support for Event Handler in Style
        /// Note: IAddChild interface is not covered as they are going to be removed.
        /// 
        /// 




        public void ApiTestStyleSetters()
        {
            Utilities.PrintTitle("API Test: Style.Setters");
            Style myStyle = new Style();
            Utilities.Assert(myStyle.Setters != null, "Setters will never be null");
            Utilities.Assert(myStyle.Setters.Count == 0, "Setters is empty at the beginning");

            Utilities.PrintStatus("Basic Collection Operation on Style.Setters");
            myStyle.Setters.Add(new EventSetter(System.Windows.Documents.Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(OnNavigate)));
            myStyle.Setters.Add(new EventSetter(System.Windows.Documents.Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(OnNavigate)));
            EventSetter testSetter = new EventSetter(System.Windows.Documents.Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(OnNavigate));
            myStyle.Setters.Add(testSetter);
            Utilities.Assert(myStyle.Setters.Count == 3, "Setters has 3 instances");
            myStyle.Setters.Remove(testSetter);
            Utilities.Assert(myStyle.Setters.Count == 2, "Setters has 2 instances after removing one");
            myStyle.Setters.Clear();
            Utilities.Assert(myStyle.Setters.Count == 0, "Setters is empty after clearing it");

            Utilities.PrintStatus("Sealing Setters, and trying to change its value");
            EventSetter setter1 = new EventSetter();
            setter1.Event = System.Windows.Documents.Hyperlink.RequestNavigateEvent;
            setter1.Handler = new System.Windows.Navigation.RequestNavigateEventHandler(OnNavigate);
            myStyle.Setters.Add(setter1);

            Button testButton = new Button();
            testButton.Style = myStyle;
            try
            {
                myStyle.Setters.Add(new EventSetter());  //Should not be allowed;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }


            Utilities.PrintStatus("Negative Test Cases");
            myStyle = new Style();
            SetterBaseCollection sc = myStyle.Setters;
            sc.Add(new EventSetter());

            try
            {                
                Button b1 = new Button();
                b1.Style = myStyle;
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            // SetterBase should be sealed now.
            if (sc.IsSealed != true)
            {
                throw new Microsoft.Test.TestValidationException("Setter should have been sealed but wasnt");
            }
            
            Setter s1 = new Setter(Button.BackgroundProperty, Brushes.Green);
            Style sy1 = new Style();
            sy1.Setters.Add(s1);

            Button b2 = new Button();
            b2.Style = sy1;
            
            // Setter should be sealed
            if (s1.IsSealed != true)
            {
                throw new Microsoft.Test.TestValidationException("Setter should have been sealed but wasnt");
            }

        }

        /// <summary>
        /// FrameworkElementFactory class has API to support Event Handler in the visualtree/template
        /// </summary>
        public void ApiTestFefHandlers()
        {
            Utilities.PrintTitle("FrameworkElementFactory supports EventHandlers");
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.SetValue(DockPanel.BackgroundProperty, System.Windows.Media.Brushes.AliceBlue);
            FrameworkElementFactory fefchild = new FrameworkElementFactory(typeof(StackPanel));
            fefchild.SetValue(StackPanel.MarginProperty, new Thickness(5));
            fef.AppendChild(fefchild);
            FrameworkElementFactory fefChildChild = new FrameworkElementFactory(typeof(Canvas));
            fefchild.AppendChild(fefChildChild);
            fefChildChild.SetValue(Canvas.BackgroundProperty, System.Windows.Media.Brushes.DarkOrange);

            Style testStyle = new Style();
            ControlTemplate template = new ControlTemplate(typeof(TestControl));
            template.VisualTree = fef;
            testStyle.Setters.Add(new Setter(TestControl.TemplateProperty, template));

            //API tests starts here
            Utilities.PrintStatus("Positive Call");
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1), true);
            fefChildChild.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1), false);

            Utilities.PrintStatus("Event cannot be null");
            try
            {
                fefChildChild.AddHandler(null, new RoutedEventHandler(FefEventHandler1), true);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Utilities.PrintStatus("Handler cannot be null");
            try
            {
                fefChildChild.AddHandler(s_eventD1, null, true);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Utilities.PrintStatus("Handler type must match");
            try
            {
                fefChildChild.AddHandler(s_eventD1, new RequestNavigateEventHandler(OnNavigate), false);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }


            TestControl testControl = new TestControl();
            testControl.Style = testStyle;

            Utilities.PrintStatus("InvalidOperationException to AddHandler when Fef is already sealed");
            try
            {
                fefchild.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }


            //fefChildChild.AddHandler(eventB1, new RoutedEventHandler(
        }

        /// <summary>
        /// Style.Setters contain EventSetters
        /// </summary>
        public void TestStyleEventHandler()
        {
            Utilities.PrintTitle("Style.Setters contain EventSetters");


            //testStyle1 has three EventSetters
            //Style testStyle1 = new Style(typeof(TestControl)).
            Style testStyle1 = new Style();
            EventSetter setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventD1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);

            //testStyle2 has only one EventSetter
            Style testStyle2 = new Style();
            setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler2);
            testStyle2.Setters.Add(setter);

            //testStyle3 has two EventSetters
            Style testStyle3 = new Style();
            setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler3);
            testStyle3.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler3);
            testStyle3.Setters.Add(setter);

            //Just one control. Only EventHandler is from Style.
            TestControl control1 = new TestControl();
            control1.Name = "con1";
            Utilities.PrintStatus("Before assigning style, no event handler");
            TestControl.SetTrace(s_sbTrace);
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            control1.RaiseEvent(args);
            AssertHelper("", "No event handlers");

            Utilities.PrintStatus("After assigning style, its event handlers are invoked");
            control1.Style = testStyle1;
            control1.RaiseEvent(args);
            AssertHelper("[B1,style1,con1,con1]", "Event Handlers are invoked");

            //Now with three controls. Same Style
            TestControl control2 = new TestControl();
            control2.Name = "con2";
            control2.Style = testStyle1;
            TestControl control3 = new TestControl();
            control3.Name = "con3";
            control3.Style = testStyle1;
            //Build up tree
            control1.AddLogicalTreeChild(control2);
            control2.AddVisualTreeChild(control3);
            Utilities.PrintStatus("Three Controls with same style");
            control3.RaiseEvent(args); //SourceEvent is B1
            AssertHelper("[B1,style1,con3,con3][B1,style1,con2,con2][B1,style1,con1,con2]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            control3.RaiseEvent(args); //SourceEvent is T1
            AssertHelper("[T1,style1,con1,con2][T1,style1,con2,con2][T1,style1,con3,con3]", "EventRoute for Tunnal Evnet");
            args.RoutedEvent = s_eventD1;
            control3.RaiseEvent(args); //SourceEvent is D1
            AssertHelper("[D1,style1,con3,con3]", "EventRoute for Direct Event");

            //Still with three controls, but with different Style
            control2.Style = testStyle2; //handelr for B1
            control3.Style = testStyle3; //handler for B1 and T1
            Utilities.PrintStatus("Three Controls with different styles");
            args.RoutedEvent = s_eventB1;
            control3.RaiseEvent(args);  //three controls handle B1 event
            AssertHelper("[B1,style3,con3,con3][B1,style2,con2,con2][B1,style1,con1,con2]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            control3.RaiseEvent(args);  //con3 and con1 handle T1 event
            AssertHelper("[T1,style1,con1,con2][T1,style3,con3,con3]", "EventRoute for Tunnal Evnet");
            args.RoutedEvent = s_eventD1;
            control3.RaiseEvent(args); //Con3 has B1 and T1 but not D1 event
            AssertHelper("", "EventRoute for Direct Event");

            //


            //

        }

        /// <summary>
        /// EventHandler with BasedOn Event
        /// </summary>
        public void TestStyleEventHandlerWithBasedOn()
        {
            Utilities.PrintTitle("EventHandler with BasedOn Event");

            //testStyle1 has one EventSetter
            Style testStyle1 = new Style();
            EventSetter setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);

            //testStyle2 has one EventSetter but based on testStyle1
            Style testStyle2 = new Style();
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler2);
            testStyle2.Setters.Add(setter);
            testStyle2.BasedOn = testStyle1;

            //testStyle3 has two EventSetters and based on testStyle2
            Style testStyle3 = new Style();
            setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler3);
            testStyle3.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler3);
            testStyle3.Setters.Add(setter);
            testStyle3.BasedOn = testStyle2;

            //Create three controls with three style
            TestControl control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            TestControl control2 = new TestControl();
            control2.Name = "con2";
            control2.Style = testStyle2;
            TestControl control3 = new TestControl();
            control3.Name = "con3";
            control3.Style = testStyle3;

            //More interesting is to see how control2 and control3 behaves
            Utilities.PrintStatus("One style based on another style");
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventT1;
            control2.RaiseEvent(args);
            AssertHelper("[T1,style2,con2,con2]", "T1 is defined in this style");
            args.RoutedEvent = s_eventB1;
            control2.RaiseEvent(args);
            AssertHelper("[B1,style1,con2,con2]", "B1 is defined in the style this style bases on");
            //Now control3
            args.RoutedEvent = s_eventT1;
            control3.RaiseEvent(args);
            //Note the sequence: <Style listeners> should be called before <BasedOn Style listeners>
            AssertHelper("[T1,style3,con3,con3][T1,style2,con3,con3]", "T1 is defined in this style, and also the style this one bases on");
            args.RoutedEvent = s_eventB1;
            control3.RaiseEvent(args);
            AssertHelper("[B1,style3,con3,con3][B1,style1,con3,con3]", "B1 is defined in this style, and also the style its based-on style bases on");

        }

        /// <summary>
        /// Same Event, three Handerls
        /// </summary>
        public void TestStyleEventHandlerWithMultipleHandlerOnSameEvent()
        {
            //Same event, more than one setter
            Utilities.PrintTitle("Same Event, three Handerls");

            Utilities.PrintStatus("(1)Bubble Events");
            Style testStyle1 = new Style();
            EventSetter setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);

            setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler2);
            testStyle1.Setters.Add(setter);

            setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler3);
            testStyle1.Setters.Add(setter);

            TestControl control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventT1;
            control1.RaiseEvent(args);
            AssertHelper("", "Only respond to event B1");

            args.RoutedEvent = s_eventB1;
            control1.RaiseEvent(args);
            AssertHelper("[B1,style1,con1,con1][B1,style2,con1,con1][B1,style3,con1,con1]", "Three handelr for the same Bubble event on my style");

            Utilities.PrintStatus("(2)Tunnal Events");
            Style testStyle2 = new Style();
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle2.Setters.Add(setter);

            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler2);
            testStyle2.Setters.Add(setter);

            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler3);
            testStyle2.Setters.Add(setter);

            TestControl control2 = new TestControl();
            control2.Name = "con2";
            control2.Style = testStyle2;

            args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            control2.RaiseEvent(args);
            AssertHelper("", "Only respond to event T1");

            args.RoutedEvent = s_eventT1;
            control2.RaiseEvent(args);
            AssertHelper("[T1,style1,con2,con2][T1,style2,con2,con2][T1,style3,con2,con2]", "Three handelr for the same Tunnel event on my style");

        }

        /// <summary>
        /// Instance handlers first, then Style Event Handler
        /// </summary>
        public void TestStyleEventHandlerWithInstanceHandlers()
        {
            Utilities.PrintTitle("Instance handlers first, then Style Event Handler");

            //testStyle1 has three EventSetters
            //Style testStyle1 = new Style(typeof(TestControl)).
            Style testStyle1 = new Style();
            EventSetter setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventD1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);


            TestControl control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;

            //Add instance handler
            control1.AddHandler(s_eventB1, new RoutedEventHandler(InstanceEventHandler));
            control1.AddHandler(s_eventT1, new RoutedEventHandler(InstanceEventHandler));
            control1.AddHandler(s_eventD1, new RoutedEventHandler(InstanceEventHandler));

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            control1.RaiseEvent(args);
            AssertHelper("[B1,Inst,con1,con1][B1,style1,con1,con1]", "Instance handler, Bubble Event");

            args.RoutedEvent = s_eventT1;
            control1.RaiseEvent(args);
            AssertHelper("[T1,Inst,con1,con1][T1,style1,con1,con1]", "Instance handler, Tunnel Event");

            args.RoutedEvent = s_eventD1;
            control1.RaiseEvent(args);
            AssertHelper("[D1,Inst,con1,con1][D1,style1,con1,con1]", "Instance handler, Tunnel Direct");


        }

        /// <summary>
        /// Class handlers first, then Style Event Handler
        /// </summary>
        public void TestStyleEventHandlerWithClassHandlers()
        {
            //Note: Only run at the end of test suite
            EventManager.RegisterClassHandler(typeof(TestControl), s_eventB1, new RoutedEventHandler(TestControl.ClasslEventHandler));
            EventManager.RegisterClassHandler(typeof(TestControl), s_eventT1, new RoutedEventHandler(TestControl.ClasslEventHandler));
            EventManager.RegisterClassHandler(typeof(TestControl), s_eventD1, new RoutedEventHandler(TestControl.ClasslEventHandler));


            Utilities.PrintTitle("Class handlers first, then Style Event Handler");


            //testStyle1 has three EventSetters
            //Style testStyle1 = new Style(typeof(TestControl)).
            Style testStyle1 = new Style();
            EventSetter setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventD1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);

            TestControl.SetTrace(s_sbTrace);

            TestControl control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            control1.RaiseEvent(args);
            AssertHelper("[B1,class,con1,con1][B1,style1,con1,con1]", "Class handler, Bubble Event");

            args.RoutedEvent = s_eventT1;
            control1.RaiseEvent(args);
            AssertHelper("[T1,class,con1,con1][T1,style1,con1,con1]", "Class handler, Tunnel Event");

            args.RoutedEvent = s_eventD1;
            control1.RaiseEvent(args);
            AssertHelper("[D1,class,con1,con1][D1,style1,con1,con1]", "Instance handler, Direct Event");


        }

        // used by test case 3
        private class MyPanel : Panel
        {
            public MyPanel()
            {
                _children = new VisualCollection(this);
            }
            
            public VisualCollection VChildren
            {
                get
                {
                    return _children;
                }
            }

            protected override Visual GetVisualChild(int index)
            {
                // if you have a template
                if(base.VisualChildrenCount != 0 && index == 0)
                {
                    return base.GetVisualChild(0);
                }            
                // otherwise you can have your own children
                if(_children == null)
                {
                    throw new ArgumentOutOfRangeException("index is out of range");
                }
                index--;
                if(index < 0 || index >= _children.Count)
                {
                    throw new ArgumentOutOfRangeException("index is out of range");
                }

                return _children[index];
            }
                  
            protected override int VisualChildrenCount
            {           
                get 
                {               
                    return base.Children.Count + _children.Count; 
                }
            }                  

            private VisualCollection _children;
            
        }


        /// <summary>
        /// Simple FrameworkElementFactory Event Handler tests
        /// </summary>
        public void TestFefEventHandler()
        {
            Panel element, element1, element2, element3;
            FrameworkElementFactory fef, fefchild, fefChildChild;
            TestControl control1;
            Utilities.PrintTitle("FrameworkElementFactory contains Event Handlers");

            //-------------------- Case 1 -----------------------------
            Utilities.PrintStatus("Case 1: One element with one visual child. EventHandler on Visual Child");
            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            ControlTemplate template1 = new ControlTemplate(typeof(TestControl));
            template1.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template1));

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);          
            System.Diagnostics.Debug.Assert(count == 1);
            element = (Panel)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Fef,vDP1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("[T1,Fef,vDP1,vDP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("[D1,Fef,vDP1,vDP1]", "EventRoute for Direct Event");


            //------------------------  Case 2 ---------------------------
            Utilities.PrintStatus("Case 2: One element with three visual child. EventHandler on Visual Child");
            Style testStyle2 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fefchild = new FrameworkElementFactory(typeof(StackPanel));
            fefChildChild = new FrameworkElementFactory(typeof(Canvas));
            fef.AppendChild(fefchild);
            fefchild.AppendChild(fefChildChild);
            ControlTemplate template2 = new ControlTemplate(typeof(TestControl));
            template2.VisualTree = fef;
            testStyle2.Setters.Add(new Setter(TestControl.TemplateProperty, template2));
            //Adding handlers
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            fefchild.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fefchild.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fefchild.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle2;
            control1.ApplyTemplate();
            count = VisualTreeHelper.GetChildrenCount(control1);
            System.Diagnostics.Debug.Assert(count == 1);
            element1 = (Panel)VisualTreeHelper.GetChild(control1,0);
            element1.Name = "vDP1"; //DockPanel

            element2 = (Panel)VisualTreeHelper.GetChild(element1,0);
            element2.Name = "vFP1";  //... conatins StackPanel

            element3 = (Panel)VisualTreeHelper.GetChild(element2,0);
            element3.Name = "vCVS1";  //... conatins Canvas

            Utilities.PrintStatus("Raise event from the bottom Visual");
            args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element3.RaiseEvent(args);
            AssertHelper("[B1,Fef,vCVS1,vCVS1][B1,Fef,vFP1,vCVS1][B1,Fef,vDP1,vCVS1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element3.RaiseEvent(args);
            AssertHelper("[T1,Fef,vDP1,vCVS1][T1,Fef,vFP1,vCVS1][T1,Fef,vCVS1,vCVS1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element3.RaiseEvent(args);
            AssertHelper("[D1,Fef,vCVS1,vCVS1]", "EventRoute for Direct Event");
            Utilities.PrintStatus("Raise event from middle Visual");
            args.RoutedEvent = s_eventB1;
            element2.RaiseEvent(args);
            AssertHelper("[B1,Fef,vFP1,vFP1][B1,Fef,vDP1,vFP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element2.RaiseEvent(args);
            AssertHelper("[T1,Fef,vDP1,vFP1][T1,Fef,vFP1,vFP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element2.RaiseEvent(args);
            AssertHelper("[D1,Fef,vFP1,vFP1]", "EventRoute for Direct Event");

       }

        /// <summary>
        /// Unlike Style EventSetter, Fef handler will not be accumulated if another style redefine Fef.
        /// </summary>
        public void TestFefEventHandlerWithBasedOn()
        {
            Utilities.PrintTitle("FrameworkElementFactory Event Handler with BasedOn");

            FrameworkElementFactory fef;
            TestControl control1;
            FrameworkElement element;

            Utilities.PrintStatus("Case 1: A new style based on aother style should inherit FEF, including handlers");
            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            ControlTemplate template1 = new ControlTemplate(typeof(TestControl));
            template1.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template1));

            Style testStyle2 = new Style(typeof(TestControl), testStyle1);
            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle2;  //Style2 is based on Style1
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);         
            System.Diagnostics.Debug.Assert(count == 1);
            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Fef,vDP1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("[T1,Fef,vDP1,vDP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("[D1,Fef,vDP1,vDP1]", "EventRoute for Direct Event");

            Utilities.PrintStatus("Case 2: A  new style that re-defines VisualTree will not inherit Fef Event Handler");
            Style testStyle3 = new Style(typeof(TestControl), testStyle1);
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            ControlTemplate template3 = new ControlTemplate(typeof(TestControl));
            template3.VisualTree = fef;
            testStyle3.Setters.Add(new Setter(TestControl.TemplateProperty, template3));
            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle3;  //Style3 is based on Style1
            control1.ApplyTemplate();
            count = VisualTreeHelper.GetChildrenCount(control1);         
            System.Diagnostics.Debug.Assert(count == 1);

            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel
            args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Fef,vDP1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("", "EventRoute for Tunnal Event yet No Event Handler");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("", "EventRoute for Direct Event yet No Event Handler");


        }

        /// <summary>
        /// Instance handlers first, then Fef event handlers
        /// </summary>
        public void TestFefEventHandlerWithInstanceHandlers()
        {
            Utilities.PrintTitle("Instance handlers first, then Style Event Handler");


            FrameworkElementFactory fef;
            TestControl control1;
            FrameworkElement element;

            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));

            ControlTemplate template1 = new ControlTemplate(typeof(TestControl));
            template1.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template1));

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);           
            System.Diagnostics.Debug.Assert(count == 1);
            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel
            //Add instance handler
            element.AddHandler(s_eventB1, new RoutedEventHandler(InstanceEventHandler));
            element.AddHandler(s_eventT1, new RoutedEventHandler(InstanceEventHandler));
            element.AddHandler(s_eventD1, new RoutedEventHandler(InstanceEventHandler));

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Inst,vDP1,vDP1][B1,Fef,vDP1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("[T1,Inst,vDP1,vDP1][T1,Fef,vDP1,vDP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("[D1,Inst,vDP1,vDP1][D1,Fef,vDP1,vDP1]", "EventRoute for Direct Event");

        }

        /// <summary>
        /// TestFefEventHandlerWithMultipleHandlerOnSameEvent
        /// </summary>
        public void TestFefEventHandlerWithMultipleHandlerOnSameEvent()
        {
            Utilities.PrintTitle("TestFefEventHandlerWithMultipleHandlerOnSameEvent");


            FrameworkElementFactory fef;
            TestControl control1;
            FrameworkElement element;

            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler2));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler3));
            ControlTemplate template1 = new ControlTemplate(typeof(TestControl));
            template1.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template1));

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);          
            System.Diagnostics.Debug.Assert(count == 1);
            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Fef,vDP1,vDP1][B1,Fef2,vDP1,vDP1][B1,Fef3,vDP1,vDP1]", "EventRoute for Bubble Event (3 handlers)");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("", "EventRoute for Tunnal Event (0 handler)");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("", "EventRoute for Direct Event (0 handler)");

        }

        /// <summary>
        /// When both Style event setters and Fef event handlers are used
        /// </summary>
        public void TestBothStyleAndFefEventHandler()
        {
            FrameworkElement element, element1, element2, element3;
            FrameworkElementFactory fef, fefchild, fefChildChild;
            TestControl control1;//
            Utilities.PrintTitle("When both Style event setters and Fef event handlers are used");

            //-------------------- Case 1 -----------------------------
            Utilities.PrintStatus("Case 1: One element with one visual child.");
            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            //...Both Fef Event Handler
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            ControlTemplate template1 = new ControlTemplate(typeof(TestControl));
            template1.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template1));
            //...And Style Event Setter
            EventSetter setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventD1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle1.Setters.Add(setter);

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);     
            System.Diagnostics.Debug.Assert(count == 1);
            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Fef,vDP1,vDP1][B1,style1,con1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("[T1,style1,con1,vDP1][T1,Fef,vDP1,vDP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("[D1,Fef,vDP1,vDP1]", "EventRoute for Direct Event");

            //-------------------- Case 2 -----------------------------
            Utilities.PrintStatus("Case 2: One element with three visual children.");
            Style testStyle2 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fefchild = new FrameworkElementFactory(typeof(StackPanel));
            fefChildChild = new FrameworkElementFactory(typeof(Canvas));
            fef.AppendChild(fefchild);
            fefchild.AppendChild(fefChildChild);
            ControlTemplate template2 = new ControlTemplate(typeof(TestControl));
            template2.VisualTree = fef;
            testStyle2.Setters.Add(new Setter(TestControl.TemplateProperty, template2));
            //...Both Fef Event Handler
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            fefchild.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fefchild.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fefchild.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fefChildChild.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            //...And Style Event Setter
            setter = new EventSetter();
            setter.Event = s_eventB1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle2.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventT1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle2.Setters.Add(setter);
            setter = new EventSetter();
            setter.Event = s_eventD1;
            setter.Handler = new RoutedEventHandler(StyleEventHandler1);
            testStyle2.Setters.Add(setter);

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle2;
            control1.ApplyTemplate();
            count = VisualTreeHelper.GetChildrenCount(control1);           
            System.Diagnostics.Debug.Assert(count == 1);
            element1 = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element1.Name = "vDP1"; //DockPanel

            element2 = (FrameworkElement)VisualTreeHelper.GetChild(element1,0);
            element2.Name = "vFP1";  //... conatins StackPanel

            element3 = (FrameworkElement)VisualTreeHelper.GetChild(element2,0);
            element3.Name = "vCVS1";  //... conatins Canvas

            args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element3.RaiseEvent(args);
            AssertHelper("[B1,Fef,vCVS1,vCVS1][B1,Fef,vFP1,vCVS1][B1,Fef,vDP1,vCVS1][B1,style1,con1,vCVS1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element3.RaiseEvent(args);
            AssertHelper("[T1,style1,con1,vCVS1][T1,Fef,vDP1,vCVS1][T1,Fef,vFP1,vCVS1][T1,Fef,vCVS1,vCVS1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element3.RaiseEvent(args);
            AssertHelper("[D1,Fef,vCVS1,vCVS1]", "EventRoute for Direct Event");

        }

        //Class event handler
        private static void ClasslEventHandler(object sender, RoutedEventArgs e)
        {
            RoutedEvent routedEvent = e.RoutedEvent;
            FrameworkElement element = (FrameworkElement)sender;
            FrameworkElement source = (FrameworkElement)e.Source;

            //[Event,Hander,Sender,Source]
            s_sbTrace.Append("[");
            s_sbTrace.Append(routedEvent.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append("Class");
            s_sbTrace.Append(",");
            s_sbTrace.Append(element.Name);
            s_sbTrace.Append(",");
            s_sbTrace.Append(source.Name);
            s_sbTrace.Append("]");
        }

        /// <summary>
        /// Class handlers first, then Style Event Handler
        /// </summary>
        public void TestFefEventHandlerWithClassHandlers()
        {
            Utilities.PrintTitle("Class handlers first, then Style Event Handler");

            //Note: Only run at the end of test suite
            EventManager.RegisterClassHandler(typeof(DockPanel), s_eventB1, new RoutedEventHandler(ClasslEventHandler));
            EventManager.RegisterClassHandler(typeof(DockPanel), s_eventT1, new RoutedEventHandler(ClasslEventHandler));
            EventManager.RegisterClassHandler(typeof(DockPanel), s_eventD1, new RoutedEventHandler(ClasslEventHandler));


            FrameworkElementFactory fef;
            TestControl control1;
            FrameworkElement element;

            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            ControlTemplate template = new ControlTemplate(typeof(TestControl));
            template.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template));

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);   
            System.Diagnostics.Debug.Assert(count == 1);
            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Class,vDP1,vDP1][B1,Fef,vDP1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("[T1,Class,vDP1,vDP1][T1,Fef,vDP1,vDP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("[D1,Class,vDP1,vDP1][D1,Fef,vDP1,vDP1]", "EventRoute for Direct Event");

        }


        /// <summary>
        /// called after TestFefEventHandlerWithClassHandlers and TestStyleEventHandlerWithClassHandlers
        /// </summary>
        public void TestFefEventHandlerWithClassHandlersSecondPass()
        {
            Utilities.PrintTitle("called after TestFefEventHandlerWithClassHandlers and TestStyleEventHandlerWithClassHandlers");

            FrameworkElementFactory fef;
            TestControl control1;
            FrameworkElement element;

            Style testStyle1 = new Style();
            fef = new FrameworkElementFactory(typeof(DockPanel));
            fef.AddHandler(s_eventB1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventT1, new RoutedEventHandler(FefEventHandler1));
            fef.AddHandler(s_eventD1, new RoutedEventHandler(FefEventHandler1));
            ControlTemplate template = new ControlTemplate(typeof(TestControl));
            template.VisualTree = fef;
            testStyle1.Setters.Add(new Setter(TestControl.TemplateProperty, template));

            control1 = new TestControl();
            control1.Name = "con1";
            control1.Style = testStyle1;
            control1.ApplyTemplate();
            int count = VisualTreeHelper.GetChildrenCount(control1);          
            System.Diagnostics.Debug.Assert(count == 1);
            element = (FrameworkElement)VisualTreeHelper.GetChild(control1,0);
            element.Name = "vDP1"; //DockPanel

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = s_eventB1;
            element.RaiseEvent(args);
            AssertHelper("[B1,Class,vDP1,vDP1][B1,Fef,vDP1,vDP1][B1,class,con1,vDP1]", "EventRoute for Bubble Event");
            args.RoutedEvent = s_eventT1;
            element.RaiseEvent(args);
            AssertHelper("[T1,class,con1,vDP1][T1,Class,vDP1,vDP1][T1,Fef,vDP1,vDP1]", "EventRoute for Tunnal Event");
            args.RoutedEvent = s_eventD1;
            element.RaiseEvent(args);
            AssertHelper("[D1,Class,vDP1,vDP1][D1,Fef,vDP1,vDP1]", "EventRoute for Direct Event");

        }

        /******************************************************************************
        * CLASS:          TestControl
        ******************************************************************************/
        /// <summary>
        /// A TestControl that derives from control class
        /// </summary>
        public class TestControl : Control
        {
            private static System.Text.StringBuilder s_sbTrace;
            private System.Collections.ObjectModel.Collection<DependencyObject> _logicalTreeChilren = new System.Collections.ObjectModel.Collection<DependencyObject>();

            /// <summary>
            /// No need to be static in this case (as we do not register class handelr in static
            /// ctor. Still, we make it static.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            internal static void ClasslEventHandler(object sender, RoutedEventArgs e)
            {
                RoutedEvent routedEvent = e.RoutedEvent;
                FrameworkElement element = (FrameworkElement)sender;
                FrameworkElement source = (FrameworkElement)e.Source;

                //[Event,Hander,Sender,Source]
                s_sbTrace.Append("[");
                s_sbTrace.Append(routedEvent.Name);
                s_sbTrace.Append(",");
                s_sbTrace.Append("class");
                s_sbTrace.Append(",");
                s_sbTrace.Append(element.Name);
                s_sbTrace.Append(",");
                s_sbTrace.Append(source.Name);
                s_sbTrace.Append("]");
            }

            /// <summary>
            /// Default contructor
            /// </summary>
            public TestControl()
                : base()
            {
                _children = new VisualCollection(this);                    
            }


            /// <summary>
            /// Set trace StringBuilder
            /// </summary>
            /// <param name="sb">StringBuilder to set</param>
            public static void SetTrace(System.Text.StringBuilder sb)
            {
                System.Diagnostics.Debug.Assert(sb != null);
                s_sbTrace = sb;
                s_sbTrace.Remove(0, s_sbTrace.Length);
            }


            /// <summary>
            /// Add into logical tree
            /// </summary>
            /// <param name="d">child</param>
            public void AddLogicalTreeChild(DependencyObject d)
            {
                AddLogicalChild(d);
                _logicalTreeChilren.Add(d);
            }

            /// <summary>
            /// Add v into Visual tree
            /// </summary>
            /// <param name="v">visual child</param>
            public void AddVisualTreeChild(Visual v)
            {
                _children.Add(v);
            }

            /// <summary>
            /// Returns the child at the specified index.
            /// </summary>
            protected override Visual GetVisualChild(int index)
            {
                // if you have a template
                if(base.VisualChildrenCount != 0 && index == 0)
                {
                    return base.GetVisualChild(0);
                }            
                // otherwise you can have your own children
                if(_children == null)
                {
                    throw new ArgumentOutOfRangeException("index is out of range");
                }
                if(index < 0 || index >= _children.Count)
                {
                    throw new ArgumentOutOfRangeException("index is out of range");
                }

                return _children[index];
            }

            /// <summary>
            /// Returns the Visual children count.
            /// </summary>                   
            protected override int VisualChildrenCount
            {           
                get 
                {
                    //you can either have a Template or your own children
                    if(base.VisualChildrenCount > 0) return 1;
                    else return  _children.Count; 
                }            
            }

            private VisualCollection _children;            
            
        }
    }
}


