// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Tests RoutedEvents, Initialized Event and Loaded Event
//
//

using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Threading;

namespace DRT
{
    internal sealed class DrtEvents : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtEvents();
            return drt.Run(args);
        }

        private DrtEvents()
        {
            WindowTitle = "DrtEvents";
            TeamContact = "WPF";
            Contact = "Microsoft";
            DrtName = "DrtEvents";
            IsPrivate = false;
            Suites = new DrtTestSuite[]{
                        new TestSuite1(),
                        new TestSuite2(),
                        new TestSuite3(),
                        new TestSuite4(),
                        new TestSuite5(),
                        new TestSuite6(),
                        new TestSuite7(),
                        new TestSuite9(this),
                        new TestSuite10(this),
                        new TestSuite21(),
                        new TestSuite22(),
                        };

            // Turn on dummy tracing; it won't go anywhere, but will exercise the code.
            EnableNoopTracing();
        }

        // Override this in derived classes to handle command-line arguments one-by-one.
        // Return true if handled.
        protected override bool HandleCommandLineArgument(string arg, bool option, string[] args, ref int k)
        {
            // start by giving the base class the first chance
            if (base.HandleCommandLineArgument(arg, option, args, ref k))
                return true;

            // process your own arguments here, using these parameters:
            //      arg     - current argument
            //      option  - true if there was a leading - or /.
            //      args    - all arguments
            //      k       - current index into args
            // Here's a typical sketch:

            if (option)
            {
                switch (arg)                // arg is lower-case, no leading - or /
                {
                    case "private":         // simple boolean option:   -foo
                        IsPrivate = true;
                        Suites = new DrtTestSuite[]{
                                    new TestSuite1(),
                                    new TestSuite2(),
                                    new TestSuite3(),
                                    new TestSuite4(),
                                    new TestSuite5(),
                                    new TestSuite6(),
                                    new TestSuite7(),
                                    new TestSuite9(this),
                                    new TestSuite10(this),
                                    new TestSuite11(),
                                    new TestSuite12(),
                                    new TestSuite13(),
                                    new TestSuite14(),
                                    new TestSuite15(),
                                    new TestSuite16(),
                                    new TestSuite17(),
                                    new TestSuite18(),
                                    new TestSuite19(),
                                    new TestSuite20(),
                                    new TestSuite21(),
                                    new TestSuite22(),
                                    };
                        return true;
                }
            }

            return false;
        }

        // Print a description of command line arguments.  Derived classes should
        // override this to describe their own arguments, and then call
        // base.PrintOptions() to get the DrtBase description.
        protected override void PrintOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("  -private        also run private tests");
            base.PrintOptions();
        }

        static DrtEvents()
        {
            // Register a RoutedEvent1
            RoutedEvent1 = EventManager.RegisterRoutedEvent("RoutedEvent1",
                                              RoutingStrategy.Bubble,
                                              typeof(CustomRoutedEventHandler),
                                              typeof(DrtEvents));

            // Register a RoutedEvent2
            RoutedEvent2 = EventManager.RegisterRoutedEvent("RoutedEvent2",
                                RoutingStrategy.Tunnel,
                                typeof(CustomRoutedEventHandler),
                                typeof(DrtEvents));
        }

        internal static RoutedEvent RoutedEvent1;
        internal static RoutedEvent RoutedEvent2;

        internal static void OnStaticRoutedEvent(object sender, CustomRoutedEventArgs args)
        {
            if (args.Source != args.Targets[args.NumEventsFired].Source)
            {
                throw new Exception("Incorrect Source");
            }

            if (sender != args.Targets[args.NumEventsFired].Sender)
            {
                throw new Exception("Incorrect Sender");
            }

            if (args.ShouldMath == true && args.NumClassEventsFired != args.NumEventsFired /2)
            {
                throw new Exception("NumClassEventsFired & NumEventsFired mismatch");
            }

            args.NumEventsFired++;
            args.NumClassEventsFired++;
        }

        internal static void OnRoutedEvent(object sender, CustomRoutedEventArgs args)
        {
            if (args.Source != args.Targets[args.NumEventsFired].Source)
            {
                throw new Exception("Incorrect Source");
            }

            if (sender != args.Targets[args.NumEventsFired].Sender)
            {
                throw new Exception("Incorrect Sender");
            }

            args.NumEventsFired++;
        }

        internal bool IsPrivate;
    }

    internal sealed class TestSuite1 : DrtTestSuite
    {
        internal TestSuite1(): base("InstanceHandlers")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Simple tree with control1 visual parent of control2 visual parent of control3
            // Tests instance handler invocation for bubble and tunnel events on all three tree nodes

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();

            // Construct tree
            control1.AppendChild(control2);
            control2.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[3];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control3;
            targets[2].Sender = control1;
            targets[2].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[3];
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control3;
            targets[0].Sender = control1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite2 : DrtTestSuite
    {
        internal TestSuite2() : base("RemoveHandler")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 43 in framework spec
            // Tests RemoveHandler on both UIElement and ContentElement
            // Also tests ContentElement.RaiseEvent

            // Create controls
            contentHost1 = new CustomContentHost();
            contentElement2 = new CustomContentElement();
            control3 = new CustomControl();

            // Construct tree
            contentHost1.AppendModelChild(contentElement2);
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent),
                        new DrtTest(RemoveNRaiseTunnelEvent),
                        new DrtTest(AddRemoveNRaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[3];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = contentElement2;
            targets[1].Source = control3;
            targets[2].Sender = contentHost1;
            targets[2].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[3];
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[1].Sender = contentElement2;
            targets[1].Source = control3;
            targets[0].Sender = contentHost1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RemoveNRaiseTunnelEvent()
        {
            // Remove event handlers for tunnel event
            contentHost1.RemoveHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[2];
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[0].Sender = contentElement2;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 2)
            {
                throw new Exception("Expected 2 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }

            // Remove event handlers for tunnel event
            contentElement2.RemoveHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Raise the tunnel event
            targets = new RouteTarget[1];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 1)
            {
                throw new Exception("Expected 1 tunnel event handler to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void AddRemoveNRaiseTunnelEvent()
        {
            // Add and remove event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.RemoveHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[2];
            targets[1].Sender = contentElement2;
            targets[1].Source = contentElement2;
            targets[0].Sender = contentHost1;
            targets[0].Source = contentElement2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            contentElement2.RaiseEvent(args);

            if (args.NumEventsFired != 2)
            {
                throw new Exception("Expected 2 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost       contentHost1;
        private CustomContentElement    contentElement2;
        private CustomControl           control3;

        private RoutedEvent           bubbleEvent;
        private RoutedEvent           tunnelEvent;

    }

    internal sealed class TestSuite3 : DrtTestSuite
    {
        internal TestSuite3() : base("ClassHandlers")
        {
        }


        public override DrtTest[] PrepareTests()
        {
            // Class and Instance handler invocation on figure 43 in framework spec
            // Also tests that class handlers are invoked before instance handler
            // with some math on the listener method

            // Create controls
            contentHost1 = new CustomSubContentHost();
            contentElement2 = new CustomSubContentElement();
            control3 = new CustomSubControl();

            // Construct tree
            contentHost1.AppendModelChild(contentElement2);
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[6];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[2].Sender = contentElement2;
            targets[2].Source = control3;
            targets[3].Sender = contentElement2;
            targets[3].Source = control3;
            targets[4].Sender = contentHost1;
            targets[4].Source = control3;
            targets[5].Sender = contentHost1;
            targets[5].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumClassEventsFired != 3)
            {
                throw new Exception("Expected 3 bubble class event handlers to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[6];
            targets[5].Sender = control3;
            targets[5].Source = control3;
            targets[4].Sender = control3;
            targets[4].Source = control3;
            targets[3].Sender = contentElement2;
            targets[3].Source = control3;
            targets[2].Sender = contentElement2;
            targets[2].Source = control3;
            targets[1].Sender = contentHost1;
            targets[1].Source = control3;
            targets[0].Sender = contentHost1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumClassEventsFired != 3)
            {
                throw new Exception("Expected 3 tunnel class event handlers to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost       contentHost1;
        private CustomContentElement    contentElement2;
        private CustomControl           control3;

        private RoutedEvent           bubbleEvent;
        private RoutedEvent           tunnelEvent;
    }

    internal sealed class TestSuite4 : DrtTestSuite
    {
        internal TestSuite4() : base("GetRoutedEvents")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(GetRoutedEvents)
                        };
        }

        private void GetRoutedEvents()
        {
            // Tests EventManager.GetRoutedEvents

            RoutedEvent[] routedEvents = EventManager.GetRoutedEvents();

            bool found1 = false;
            bool found2 = false;
            for (int i=0; i< routedEvents.Length; i++)
            {
                if (routedEvents[i].Name.Equals("RoutedEvent1") == true)
                {
                    found1 = true;
                }
                else if (routedEvents[i].Name.Equals("RoutedEvent2") == true)
                {
                    found2 = true;
                }
            }

            if (found1 = false || found2 == false)
            {
                throw new Exception("GetRoutedEvents failed");
            }

            // Tests EventManager.GetRoutedEvents

            routedEvents = EventManager.GetRoutedEventsForOwner(typeof(DrtEvents));

            if (routedEvents[0].Name.Equals("RoutedEvent1") == false)
            {
                throw new Exception("GetRoutedEventsForOwner failed");
            }

            if (routedEvents[1].Name.Equals("RoutedEvent2") == false)
            {
                throw new Exception("GetRoutedEventsForOwner failed");
            }
        }
    }

    internal sealed class TestSuite5 : DrtTestSuite
    {
        internal TestSuite5() : base("Handledness")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Simple tree with control1 visual parent of subControl2
            // Tests class handler invocation on subControl
            // Also tests effect of event handledness set to true

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Create controls
            // NOTE: Class Listener for CustomSubControl type was already registered in Test14
            control1 = new CustomSubControl();
            subControl2 = new CustomSubSubControl();

            // Construct tree
            control1.AppendChild(subControl2);

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[4];
            targets[0].Sender = subControl2;
            targets[0].Source = subControl2;
            targets[1].Sender = subControl2;
            targets[1].Source = subControl2;
            targets[2].Sender = subControl2; // For super class listener
            targets[2].Source = subControl2;
            targets[3].Sender = control1;
            targets[3].Source = subControl2;

            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            args.ShouldMath = false;
            subControl2.RaiseEvent(args);

            if (args.NumClassEventsFired != 4)
            {
                throw new Exception("Expected 4 bubble class event handlers to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }

            // Raise the bubble event once more
            targets[0].Sender = subControl2;
            targets[0].Source = subControl2;
            args = new CustomRoutedEventArgs(bubbleEvent, targets, true);
            args.ShouldMath = false;
            subControl2.RaiseEvent(args);

            if (args.NumClassEventsFired != 1)
            {
                throw new Exception("Expected 1 bubble class event handler to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 1)
            {
                throw new Exception("Expected 1 bubble event handler to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[4];
            targets[3].Sender = subControl2;
            targets[3].Source = subControl2;
            targets[2].Sender = subControl2;
            targets[2].Source = subControl2;
            targets[1].Sender = subControl2; // For super class listener
            targets[1].Source = subControl2;
            targets[0].Sender = control1;
            targets[0].Source = subControl2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            args.ShouldMath = false;
            subControl2.RaiseEvent(args);

            if (args.NumClassEventsFired != 4)
            {
                throw new Exception("Expected 4 tunnel class event handlers to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }

            // Raise the tunnel event once more
            targets[0].Sender = subControl2;
            targets[0].Source = subControl2;
            args = new CustomRoutedEventArgs(tunnelEvent, targets, true);
            args.ShouldMath = false;
            subControl2.RaiseEvent(args);

            if (args.NumClassEventsFired != 1)
            {
                throw new Exception("Expected 1 tunnel class event handler to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 1)
            {
                throw new Exception("Expected 1 tunnel event handler to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomSubControl control1;
        private CustomSubSubControl subControl2;

        private RoutedEvent           bubbleEvent;
        private RoutedEvent           tunnelEvent;
    }

    internal sealed class TestSuite6 : DrtTestSuite
    {
        internal TestSuite6() : base("SubclassHandlers")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Simple tree with control1 visual parent of subControl2
            // Tests class handler invocation on control1 and subControl2
            // Also tests subclass handlers updation in n-level cache

            // Register a RoutedEvent3
            bubbleEvent = EventManager.RegisterRoutedEvent("RoutedEvent3",
                                              RoutingStrategy.Bubble,
                                              typeof(CustomRoutedEventHandler),
                                              typeof(DrtEvents));

            // Create controls
            control1 = new CustomSubControl();
            subControl2 = new CustomSubSubControl();

            // Construct tree
            control1.AppendChild(subControl2);

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(AddNRaiseBubbleEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = null;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            args.ShouldMath = false;
            subControl2.RaiseEvent(args);

            if (args.NumClassEventsFired != 0)
            {
                throw new Exception("Expected 0 bubble class event handlers to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 0)
            {
                throw new Exception("Expected 0 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void AddNRaiseBubbleEvent()
        {
            // Register Class Handler for CustomSubControl
            EventManager.RegisterClassHandler(typeof(CustomSubControl), bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));

            // Register Class Handler for CustomSubSubControl
            EventManager.RegisterClassHandler(typeof(CustomSubSubControl), bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));

            // Register one more Class Handler for CustomSubControl
            EventManager.RegisterClassHandler(typeof(CustomSubControl), bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnStaticRoutedEvent));

            // Raise the bubble event once more
            RouteTarget[] targets = new RouteTarget[5];
            targets[0].Sender = subControl2;
            targets[0].Source = subControl2;
            targets[1].Sender = subControl2; // For super class listener
            targets[1].Source = subControl2;
            targets[2].Sender = subControl2; // For super class listener
            targets[2].Source = subControl2;
            targets[3].Sender = control1;
            targets[3].Source = subControl2;
            targets[4].Sender = control1;
            targets[4].Source = subControl2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            args.ShouldMath = false;
            subControl2.RaiseEvent(args);

            if (args.NumClassEventsFired != 5)
            {
                throw new Exception("Expected 5 bubble class event handlers to be invoked. Got " + args.NumClassEventsFired + " instead.");
            }
            if (args.NumEventsFired != 5)
            {
                throw new Exception("Expected 5 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomSubControl    control1;
        private CustomSubSubControl subControl2;

        private RoutedEvent       bubbleEvent;
    }

    internal sealed class TestSuite7 : DrtTestSuite
    {
        internal TestSuite7() : base("SourceChange")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Simple tree with control1 visual parent of strangecontrol2 visual parent of control3 visula parent of control4
            // Tests source change on instance handler invocation for bubble and tunnel events on all these tree nodes

            // Create controls
            control1 = new CustomControl();
            strangeControl2 = new StrangeControl();
            control3 = new CustomControl();
            control4 = new CustomControl();

            // Construct tree
            control1.AppendChild(strangeControl2);
            strangeControl2.AppendChild(control3);
            control3.AppendChild(control4);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            strangeControl2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            strangeControl2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[4];
            targets[0].Sender = control4;
            targets[0].Source = control4;
            targets[1].Sender = control3;
            targets[1].Source = control4;
            targets[2].Sender = strangeControl2;
            targets[2].Source = control4;
            targets[3].Sender = control1;
            targets[3].Source = control4;

            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control4.RaiseEvent(args);

            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[4];
            targets[3].Sender = control4;
            targets[3].Source = control4;
            targets[2].Sender = control3;
            targets[2].Source = control4;
            targets[1].Sender = strangeControl2;
            targets[1].Source = control4;
            targets[0].Sender = control1;
            targets[0].Source = control4;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control4.RaiseEvent(args);

            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        CustomControl   control1;
        StrangeControl  strangeControl2;
        CustomControl   control3;
        CustomControl   control4;

        RoutedEvent   bubbleEvent;
        RoutedEvent   tunnelEvent;
    }

    internal sealed class TestSuite9 : DrtTestSuite
    {
        internal TestSuite9(DrtEvents drt) : base("Initialized")
        {
            this.drt = drt;
        }

        public override DrtTest[] PrepareTests()
        {
            if (drt.IsPrivate)
            {
                // return the lists of tests to run against the tree
                return new DrtTest[]{
                            new DrtTest(TestParserTreeConstruction),
                            new DrtTest(TestProgrammaticTreeConstruction)
                            };
            }
            else
            {
                // return the lists of tests to run against the tree
                return new DrtTest[]{
                            new DrtTest(TestProgrammaticTreeConstruction)
                            };
            }
        }

        private void TestParserTreeConstruction()
        {
            // Reset Initialized Count
            initializedCount = 0;

            // Parse xaml to build tree
            Stream xamlFileStream = File.OpenRead(@"DrtFiles\Events\DrtEvents_1.xaml");

            try
            {
                drt.Show((UIElement) XamlReader.Load(xamlFileStream));
            }
            finally
            {
                // done with the stream
                xamlFileStream.Close();
            }

            // Verify Initialized Event
            if (initializedCount != 3)
            {
                throw new Exception("Expected 3 initialized event handlers to be invoked. Got " + initializedCount + " instead.");
            }

        }

        private void TestProgrammaticTreeConstruction()
        {
            // Reset Initialized Count
            initializedCount = 0;

            // Create controls
            DockPanel dockPanel = new DockPanel();
            Button button = new Button();
            TextBlock text = new TextBlock();

            // Add Handlers
            dockPanel.Initialized += new EventHandler(OnInitialized);
            button.Initialized += new EventHandler(OnInitialized);
            text.Initialized += new EventHandler(OnInitialized);

            // Build Tree
            button.Content = text;
            dockPanel.Children.Add(button);

            drt.Show(dockPanel);

            if (initializedCount != 3)
            {
                throw new Exception("Expected 3 initialized event handlers to be invoked. Got " + initializedCount + " instead.");
            }

        }

        internal static void OnInitialized(object sender, EventArgs e)
        {
            initializedCount++;
        }

        private static int  initializedCount;
        private DrtEvents   drt;
    }

    internal sealed class TestSuite10 : DrtTestSuite
    {
        internal TestSuite10(DrtEvents drt) : base("LoadedUnloaded")
        {
            this.drt = drt;
            this.Root = null;
        }

        public override DrtTest[] PrepareTests()
        {
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(Test1),
                        new DrtTest(Test2),
                        new DrtTest(Test3),
                        new DrtTest(Test4),
                        new DrtTest(Test5),
                        new DrtTest(Test6),
                        new DrtTest(Test7),
                        new DrtTest(Test8),
                        new DrtTest(Test9),
                        new DrtTest(Test10),
                        new DrtTest(Test11),
                        new DrtTest(Test12),
                        new DrtTest(Test13),
                        new DrtTest(Test14),
                        new DrtTest(Test15),
                        new DrtTest(Test16)
                        };
        }

        // Test Broadcast of the Loaded event as we set the HwndSource.RootVisual
        private void Test1()
        {
            ClearResults();

            Root = BuildTree(11);

            // Set it to be the Root visual
            drt.Show(Root);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedDockPanel] = 11;
            expected[(int)ResultNames.LoadedButton] = 11;
            CheckResults(expected);
        }

        // Test Broadcast of the Loaded event when an unloaded subtree is added to an already loaded tree
        private void Test2()
        {
            ClearResults();

            // Build the subtree
            DockPanel dp = BuildTree(12);

            // Add subtree to the main tree
            Root.Children.Add(dp);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedDockPanel] = 12;
            expected[(int)ResultNames.LoadedButton] = 12;
            CheckResults(expected);
        }

        
        // Test Broadcast of the Unloaded event when a loaded subtree is removed from an already loaded tree
        private void Test3()
        {
            ClearResults();

            // Get the Root of subtree
            DockPanel dp = (DockPanel)Root.Children[1];

            // Remove subtree from the main tree
            Root.Children.Remove(dp);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.UnloadedDockPanel] = 12;
            expected[(int)ResultNames.UnloadedButton] = 12;
            CheckResults(expected);
        }

        // Test Broadcast of the Loaded event when an unloaded subtree is added to an already loaded tree. No handlers attached
        private void Test4()
        {
            ClearResults();

            // Build the subtree
            DockPanel dp = BuildTree(14);

            // Remove Loaded event handlers
            dp.Loaded -= new RoutedEventHandler(OnLoaded);
            ((Button)dp.Children[0]).Loaded -= new RoutedEventHandler(OnLoaded);

            // Remove Unloaded event handlers
            dp.Unloaded -= new RoutedEventHandler(OnUnloaded);
            ((Button)dp.Children[0]).Unloaded -= new RoutedEventHandler(OnUnloaded);

            // Add subtree to the main tree
            Root.Children.Add(dp);

            this.WaitForLoaded();

            // Check the results.  (nothing happens)
            int[] expected = new int[FiredResults.Length];
            CheckResults(expected);
        }

        // Test Broadcast of the Unloaded event when a loaded subtree is removed from an already loaded tree
        private void Test5()
        {
            ClearResults();

            // Get the Root of subtree
            DockPanel dp = (DockPanel)Root.Children[1];

            // Add Unloaded event handlers
            dp.Unloaded += new RoutedEventHandler(OnUnloaded);
            ((Button)dp.Children[0]).Unloaded += new RoutedEventHandler(OnUnloaded);
            
            // Remove subtree from the main tree
            Root.Children.Remove(dp);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.UnloadedDockPanel] = 14;
            expected[(int)ResultNames.UnloadedButton] = 14;
            CheckResults(expected);
        }

        // Test Broadcast of Loaded event being aborted when tree is prematurely disconnected from the HwndSource
        private void Test6()
        {
            ClearResults();

            // Build the tree and reset HwndSource.RootVisual
            Root = BuildTree(16);
            drt.Show(Root);

            // Now quickly set the RootVisual to null so that we abort Loaded event broadcast
            drt.Show(null);

            this.WaitForLoaded();

            // Check the results.
            // The new tree #16 should never fire but the original root (#11) should Unload.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.UnloadedDockPanel] = 11;
            expected[(int)ResultNames.UnloadedButton] = 11;
            CheckResults(expected);
        }

        // Test Broadcast of Loaded event being cascaded without duplication to a sub-tree
        // even as the sub-tree was built in a Loaded event handler for the parent
        private void Test7()
        {
            ClearResults();

            // Build the tree
            Root = BuildTree(71);
            Root.Loaded += new RoutedEventHandler(OnLoadedBuildTree);

            // Reset HwndSource.RootVisual
            drt.Show(Root);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedDockPanel] = 99;
            expected[(int)ResultNames.LoadedButton] = 99;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();

            expected[(int)ResultNames.UnloadedDockPanel] = 99;
            expected[(int)ResultNames.UnloadedButton] = 99;
            CheckResults(expected);
        }

        // Test Broadcast of Loaded event after having done some add and remove
        // operations in the sub tree with handlers
        private void Test8()
        {
            ClearResults();

            // Build the tree
            Root = BuildTree(81);
            ((Button)Root.Children[0]).Loaded -= new RoutedEventHandler(OnLoaded);

            drt.Show(Root);
            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedDockPanel] = 81;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();
            expected[(int)ResultNames.UnloadedDockPanel] = 81;
            expected[(int)ResultNames.UnloadedButton] = 81;
            CheckResults(expected);
        }

        // Test having loaded/unloaded handlers in a style
        private void Test9()
        {
            ClearResults();

            Root = new DockPanel();
            Control control = new Control();

            control.Name = "Style";
            Root.Children.Add( control );

            Style style = new Style();

            EventSetter eventSetter = new EventSetter();
            eventSetter.Event = FrameworkElement.LoadedEvent;
            eventSetter.Handler = new RoutedEventHandler(OnLoaded);
            style.Setters.Add( eventSetter );

            eventSetter = new EventSetter();
            eventSetter.Event = FrameworkElement.UnloadedEvent;
            eventSetter.Handler = new RoutedEventHandler(OnUnloaded);
            style.Setters.Add( eventSetter );

            control.Style = style;

            drt.Show(Root);
            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedStyle] = 1;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();

            // Check the results.
            expected[(int)ResultNames.UnloadedStyle] = 1;
            CheckResults(expected);
        }


        // Test having loaded/unloaded handlers in a control template
        private void Test10()
        {
            ClearResults();

            Root = new DockPanel();
            Control control = new Control();
            Root.Children.Add( control );

            ControlTemplate template = new ControlTemplate(typeof(Control));

            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Control));
            fef.SetValue( FrameworkElement.NameProperty, "ControlTemplate" );
            fef.AddHandler( FrameworkElement.LoadedEvent, new RoutedEventHandler(OnLoaded));
            fef.AddHandler( FrameworkElement.UnloadedEvent, new RoutedEventHandler(OnUnloaded));
            template.VisualTree = fef;

            control.Template = template;

            drt.Show(Root);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedControlTemplate] = 1;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();

            // Check the results.
            expected[(int)ResultNames.UnloadedControlTemplate] = 1;
            CheckResults(expected);
        }

        // check that mixed Loaded / unloaded elements work correctly.
        private void Test11()
        {
            ClearResults();

            Root = BuildTree(111, true, false);
            drt.Show(Root);

            // Add subtree to the main tree
            DockPanel dp = BuildTree(112, false, true);
            Root.Children.Add(dp);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.LoadedDockPanel] = 111;
            expected[(int)ResultNames.LoadedButton] = 111;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();

            // Check the results.
            expected[(int)ResultNames.UnloadedDockPanel] = 112;
            expected[(int)ResultNames.UnloadedButton] = 112;
            CheckResults(expected);
        }

        // check that IsLoaded in maintained even if we use only "unloaded" handlers.
        private void Test12()
        {
            ClearResults();
            DockPanel dp;
            Button b;

            Root = BuildTree(121, false, true);
            drt.Show(Root);

            this.WaitForLoaded();

            // check that the button says it is loaded.
            dp = (DockPanel)Root;
            b = (Button)Root.Children[0];
            CheckIsLoaded(dp, true);
            CheckIsLoaded(b, true);

            // Build a sub-tree
            dp = BuildTree(122, false, true);
            b = (Button)dp.Children[0];

            // confirm that sub-tree is not loaded
            CheckIsLoaded(dp, false);
            CheckIsLoaded(b, false);

            // Add subtree to the main tree
            Root.Children.Add(dp);

            this.WaitForLoaded();

            // check that the button says it is loaded.
            CheckIsLoaded(dp, true);
            CheckIsLoaded(b, true);

            drt.Show(null);
            this.WaitForLoaded();
        }

        // Check Unloaded is fired when Content is cleared from a ContentPresenter.
        private void Test13()
        {
            ClearResults();

            Root = NewDockPanel(131, false, false);
            ContentControl cc = new ContentControl();
            Button b = NewButton(131, false, true);
            cc.Content = b;
            Root.Children.Add(cc);
            drt.Show(Root);

            this.WaitForLoaded();

            // Check the results.
            int[] expected = new int[FiredResults.Length];
            CheckResults(expected);

            cc.Content = null;

            this.WaitForLoaded();
            expected[(int)ResultNames.UnloadedButton] = 131;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();
        }

        // Check that Unloaded is fired even after child nodes w/ handlers are removed.
        private void Test14()
        {
            ClearResults();

            Root= BuildTree(141, false, true);
            drt.Show(Root);
            this.WaitForLoaded();

            // Get the Root of subtree
            Button b = (Button)Root.Children[0];

            // Remove subtree from the main tree
            Root.Children.Remove(b);

            this.WaitForLoaded();
            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.UnloadedButton] = 141;
            CheckResults(expected);

            drt.Show(null);
            this.WaitForLoaded();

            expected[(int)ResultNames.UnloadedDockPanel] = 141;
            CheckResults(expected);
        }

        // Test while unloading a subtree, remove child nodes in the Unload
        // handler.  These removed nodes should still recieve Unloaded Events.
        private void Test15()
        {
            ClearResults();

            Root= BuildTree(151, false, true);
            Root.Unloaded += new RoutedEventHandler(TestSuite10.OnUnloadedRemoveButton);

            drt.Show(Root);
            this.WaitForLoaded();

            // get a reference to the Button to hold it alive.
            Button b = (Button)Root.Children[0];

            // When we unload the tree the parent will Remove the button in the
            // unloaded hander.  The Button's unloaded should still fire.
            drt.Show(null);
            this.WaitForLoaded();

            int[] expected = new int[FiredResults.Length];
            expected[(int)ResultNames.UnloadedDockPanel] = 151;
            expected[(int)ResultNames.UnloadedButton] = 151;
            CheckResults(expected);

            GC.KeepAlive(b);
        }

        // Test multiple loading and unloaded.
        private void Test16()
        {
            ClearResults();
            int[] expected = new int[FiredResults.Length];

            Root = BuildTree(161, true, true);
            drt.Show(Root);
            this.WaitForLoaded();
            ClearResults();  // we don't need to look.  Other tests cover this setup.

            DockPanel dp = BuildTree(162, true, false);
            Root.Children.Add(dp);
            Root.Children.Remove(dp);

            this.WaitForLoaded();
            CheckResults(expected);  // nothing should happen because "loaded" is after render completes

            Root.Children.Add(dp);
            Root.Children.Remove(dp);
            Root.Children.Add(dp);

            this.WaitForLoaded();
            expected[(int)ResultNames.LoadedDockPanel] = 162;
            expected[(int)ResultNames.LoadedButton] = 162;
            CheckResults(expected);  // nothing should happen because "loaded" is after render completes

            drt.Show(null);
        }

        // Helper to build a tree
        private static DockPanel BuildTree(int treeNumber)
        {
            return BuildTree(treeNumber, true, true);
        }

        // Helper to build a tree
        private static DockPanel BuildTree(int treeNumber, bool withLoaded, bool withUnloaded)
        {
            DockPanel dp = NewDockPanel(treeNumber, withLoaded, withUnloaded);
            Button b = NewButton(treeNumber, withLoaded, withUnloaded);
            dp.Children.Add(b);
            return dp;
        }

        private static DockPanel NewDockPanel(int number, bool withLoaded, bool withUnloaded)
        {
            DockPanel dp = new DockPanel();
            dp.Name = "DockPanel_"+number.ToString();
            if(withLoaded)
                dp.Loaded += new RoutedEventHandler(TestSuite10.OnLoaded);
            if(withUnloaded)
                dp.Unloaded += new RoutedEventHandler(TestSuite10.OnUnloaded);
            return dp;
        }

        private static Button NewButton(int number, bool withLoaded, bool withUnloaded)
        {
            Button b = new Button();
            b.Name = "Button_"+number.ToString();
            if(withLoaded)
                b.Loaded += new RoutedEventHandler(TestSuite10.OnLoaded);
            if(withUnloaded)
                b.Unloaded += new RoutedEventHandler(TestSuite10.OnUnloaded);
            return b;
        }

        private static void OnLoaded(object sender, RoutedEventArgs args)
        {
            string name = ((FrameworkElement)sender).Name;

            if(sender is DockPanel)
            {
                FiredResults[(int)ResultNames.LoadedDockPanel] = Convert.ToInt32(name.Substring(10));
            }
            else if(sender is Button)
            {
                FiredResults[(int)ResultNames.LoadedButton] = Convert.ToInt32(name.Substring(7));
            }
            else if(sender is Control)
            {
                if(name == "Style")
                    FiredResults[(int)ResultNames.LoadedStyle] = 1;
                else if(name == "ControlTemplate")
                    FiredResults[(int)ResultNames.LoadedControlTemplate] = 1;
            }
            else
                throw new Exception("bad type");
        }

        private static void OnLoadedBuildTree(object sender, EventArgs args)
        {
            DockPanel dp = (DockPanel)sender;
            ((Button)dp.Children[0]).Content = BuildTree(99);
        }

        private static void OnUnloaded(object sender, RoutedEventArgs args)
        {
            string name = ((FrameworkElement)sender).Name;

            if(sender is DockPanel)
            {
                FiredResults[(int)ResultNames.UnloadedDockPanel] = Convert.ToInt32(name.Substring(10));
            }
            else if(sender is Button)
            {
                FiredResults[(int)ResultNames.UnloadedButton] = Convert.ToInt32(name.Substring(7));
            }
            else if(sender is Control)
            {
                if(name == "Style")
                    FiredResults[(int)ResultNames.UnloadedStyle] = 1;
                else if(name == "ControlTemplate")
                    FiredResults[(int)ResultNames.UnloadedControlTemplate] = 1;
            }
            else
                throw new Exception("bad type");
        }

        private static void OnUnloadedRemoveButton(object sender, RoutedEventArgs args)
        {
            DockPanel dp = (DockPanel)sender;
            Button b = (Button)dp.Children[0];
            dp.Children.Remove(b);
        }


        private object DoNothing(object unused) { return null; }

        private void WaitForLoaded()
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Loaded,
                new DispatcherOperationCallback(DoNothing), null);
        }

        private void CheckResults(int[] expected)
        {
            for(int i=0; i<expected.Length; i++)
            {
                if(FiredResults[i] != expected[i])
                {
                    throw new Exception("Expected "+((ResultNames)i).ToString()
                                       +" to be "+expected[i].ToString()
                                       +" got "+FiredResults[i].ToString());
                }
            }
        }

        private void CheckIsLoaded(FrameworkElement fe, bool shouldBe)
        {
            if(fe.IsLoaded != shouldBe)
            {
                throw new Exception("Element.IsLoaded is "+fe.IsLoaded.ToString()
                                   +" when it should be "+shouldBe.ToString());
            }
        }

        private void ClearResults()
        {
            for(int i=0; i<FiredResults.Length; i++)
                FiredResults[i] = 0;
        }

        private DrtEvents          drt;
        private DockPanel          Root;

        private enum ResultNames
        {
            LoadedDockPanel, UnloadedDockPanel,
            LoadedButton,    UnloadedButton, 
            LoadedStyle,     UnloadedStyle,
            LoadedControlTemplate, UnloadedControlTemplate
        };
        private static int[] FiredResults = { 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    internal sealed class TestSuite11 : DrtTestSuite
    {
        internal TestSuite11(): base("Figure41")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 41 in framework spec

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();

            // Construct tree
            control1.AppendModelChild(control2);
            control2.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[3];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control2;
            targets[2].Sender = control1;
            targets[2].Source = control2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[3];
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control2;
            targets[0].Sender = control1;
            targets[0].Source = control2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite12 : DrtTestSuite
    {
        internal TestSuite12(): base("Figure42")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 42 in framework spec

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();

            // Construct tree
            control1.AppendChild(control2);
            control2.AppendChild(control3);
            control1.AppendModelChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[3];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control2;
            targets[2].Sender = control1;
            targets[2].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[3];
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control2;
            targets[0].Sender = control1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite13 : DrtTestSuite
    {
        internal TestSuite13(): base("Figure43")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 43 in framework spec

            // Create controls
            contentHost1    = new CustomContentHost();
            contentElement2 = new CustomContentElement();
            control3        = new CustomControl();

            // Construct tree
            contentHost1.AppendModelChild(contentElement2);
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[3];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = contentElement2;
            targets[1].Source = control3;
            targets[2].Sender = contentHost1;
            targets[2].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[3];
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[1].Sender = contentElement2;
            targets[1].Source = control3;
            targets[0].Sender = contentHost1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 3)
            {
                throw new Exception("Expected 3 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost       contentHost1;
        private CustomContentElement    contentElement2;
        private CustomControl           control3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite14 : DrtTestSuite
    {
        internal TestSuite14(): base("Figure44")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 44 in framework spec

            // Create controls
            contentHost1 = new CustomContentHost();
            contentElement2 = new CustomContentElement();
            control3 = new CustomControl();
            control4 = new CustomControl();

            // Construct tree
            contentHost1.AppendModelChild(contentElement2);
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control4);
            control4.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[4];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = control4;
            targets[1].Source = control4;
            targets[2].Sender = contentElement2;
            targets[2].Source = control3;
            targets[3].Sender = contentHost1;
            targets[3].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[4];
            targets[3].Sender = control3;
            targets[3].Source = control3;
            targets[2].Sender = control4;
            targets[2].Source = control4;
            targets[1].Sender = contentElement2;
            targets[1].Source = control3;
            targets[0].Sender = contentHost1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost    contentHost1;
        private CustomContentElement contentElement2;
        private CustomControl        control3;
        private CustomControl        control4;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite15 : DrtTestSuite
    {
        internal TestSuite15(): base("Figure45")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 45 in framework spec

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();
            control4 = new CustomControl();

            // Construct tree
            control1.AppendChild(control2);
            control2.AppendChild(control3);
            control3.AppendChild(control4);
            control1.AppendModelChild(control4);
            control2.AppendModelChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[4];
            targets[0].Sender = control4;
            targets[0].Source = control4;
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[2].Sender = control2;
            targets[2].Source = control3;
            targets[3].Sender = control1;
            targets[3].Source = control4;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control4.RaiseEvent(args);

            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[4];
            targets[3].Sender = control4;
            targets[3].Source = control4;
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control3;
            targets[0].Sender = control1;
            targets[0].Source = control4;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control4.RaiseEvent(args);

            if (args.NumEventsFired != 4)
            {
                throw new Exception("Expected 4 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;
        private CustomControl control4;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite16 : DrtTestSuite
    {
        internal TestSuite16(): base("Figure46")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 46 in framework spec

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();

            // Construct tree
            control1.AppendModelChild(control3);
            control2.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[2];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = control2;
            targets[1].Source = control2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 2)
            {
                throw new Exception("Expected 2 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[2];
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[0].Sender = control2;
            targets[0].Source = control2;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 2)
            {
                throw new Exception("Expected 2 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite17 : DrtTestSuite
    {
        internal TestSuite17(): base("Figure47")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 47 in framework spec

            // Create controls
            contentHost1    = new CustomContentHost();
            contentElement2 = new CustomContentElement();
            control3        = new CustomControl();

            // Construct tree
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[2];
            targets[0].Sender = control3;
            targets[0].Source = control3;
            targets[1].Sender = contentHost1;
            targets[1].Source = contentHost1;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 2)
            {
                throw new Exception("Expected 2 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[2];
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[0].Sender = contentHost1;
            targets[0].Source = contentHost1;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control3.RaiseEvent(args);

            if (args.NumEventsFired != 2)
            {
                throw new Exception("Expected 2 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost       contentHost1;
        private CustomContentElement    contentElement2;
        private CustomControl           control3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite18 : DrtTestSuite
    {
        internal TestSuite18(): base("Figure48")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 48 in framework spec

            // Create controls
            contentHost1 = new CustomContentHost();
            contentElement2 = new CustomContentElement();
            control3 = new CustomControl();
            control4 = new CustomControl();
            control5 = new CustomControl();
            control6 = new CustomControl();

            // Construct tree
            contentHost1.AppendModelChild(contentElement2);
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control4);
            control4.AppendChild(control3);
            control5.AppendModelChild(control6);
            control5.AppendChild(contentHost1);
            control3.AppendChild(control6);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control5.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control6.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control5.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control6.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[6];
            targets[0].Sender = control6;
            targets[0].Source = control6;
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[2].Sender = control4;
            targets[2].Source = control4;
            targets[3].Sender = contentElement2;
            targets[3].Source = control3;
            targets[4].Sender = contentHost1;
            targets[4].Source = control3;
            targets[5].Sender = control5;
            targets[5].Source = control6;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control6.RaiseEvent(args);

            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[6];
            targets[5].Sender = control6;
            targets[5].Source = control6;
            targets[4].Sender = control3;
            targets[4].Source = control3;
            targets[3].Sender = control4;
            targets[3].Source = control4;
            targets[2].Sender = contentElement2;
            targets[2].Source = control3;
            targets[1].Sender = contentHost1;
            targets[1].Source = control3;
            targets[0].Sender = control5;
            targets[0].Source = control6;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control6.RaiseEvent(args);

            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost    contentHost1;
        private CustomContentElement contentElement2;
        private CustomControl        control3;
        private CustomControl        control4;
        private CustomControl        control5;
        private CustomControl        control6;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite19 : DrtTestSuite
    {
        internal TestSuite19(): base("Figure49")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 49 in framework spec

            // Create controls
            contentHost1 = new CustomContentHost();
            contentElement2 = new CustomContentElement();
            control3 = new CustomControl();
            control4 = new CustomControl();
            contentHost5 = new CustomContentHost();
            contentElement6 = new CustomContentElement();
            control7 = new CustomControl();
            control8 = new CustomControl();

            // Construct tree
            contentHost1.AppendModelChild(contentElement2);
            contentElement2.AppendModelChild(control3);
            contentHost1.AppendChild(control4);
            control4.AppendChild(control3);
            contentHost5.AppendModelChild(contentElement6);
            contentElement6.AppendModelChild(control7);
            contentHost5.AppendChild(control8);
            control8.AppendChild(control7);
            control3.AppendChild(contentHost5);
            control3.AppendModelChild(contentHost5);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentHost5.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement6.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control7.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control8.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentHost5.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement6.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control7.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control8.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[8];
            targets[0].Sender = control7;
            targets[0].Source = control7;
            targets[1].Sender = control8;
            targets[1].Source = control8;
            targets[2].Sender = contentElement6;
            targets[2].Source = control7;
            targets[3].Sender = contentHost5;
            targets[3].Source = control7;
            targets[4].Sender = control3;
            targets[4].Source = control7;
            targets[5].Sender = control4;
            targets[5].Source = control4;
            targets[6].Sender = contentElement2;
            targets[6].Source = control7;
            targets[7].Sender = contentHost1;
            targets[7].Source = control7;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control7.RaiseEvent(args);

            if (args.NumEventsFired != 8)
            {
                throw new Exception("Expected 8 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[8];
            targets[7].Sender = control7;
            targets[7].Source = control7;
            targets[6].Sender = control8;
            targets[6].Source = control8;
            targets[5].Sender = contentElement6;
            targets[5].Source = control7;
            targets[4].Sender = contentHost5;
            targets[4].Source = control7;
            targets[3].Sender = control3;
            targets[3].Source = control7;
            targets[2].Sender = control4;
            targets[2].Source = control4;
            targets[1].Sender = contentElement2;
            targets[1].Source = control7;
            targets[0].Sender = contentHost1;
            targets[0].Source = control7;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control7.RaiseEvent(args);

            if (args.NumEventsFired != 8)
            {
                throw new Exception("Expected 8 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost       contentHost1;
        private CustomContentElement    contentElement2;
        private CustomControl           control3;
        private CustomControl           control4;
        private CustomContentHost       contentHost5;
        private CustomContentElement    contentElement6;
        private CustomControl           control7;
        private CustomControl           control8;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite20: DrtTestSuite
    {
        internal TestSuite20(): base("Figure50")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Instance handler invocation on figure 50 in framework spec

            // Create controls
            contentHost1 = new CustomContentHost();
            control2 = new CustomControl();
            control3 = new CustomControl();
            control4 = new CustomControl();
            contentElement5 = new CustomContentElement();
            contentElement6 = new CustomContentElement();

            // Construct tree
            contentHost1.AppendModelChild(contentElement5);
            contentElement5.AppendModelChild(contentElement6);
            contentElement6.AppendModelChild(control4);
            contentHost1.AppendChild(control2);
            control2.AppendChild(control3);
            control3.AppendChild(control4);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Add event handlers for bubble event
            contentHost1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement5.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement6.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            contentHost1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control4.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement5.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            contentElement6.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[6];
            targets[0].Sender = control4;
            targets[0].Source = control4;
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[2].Sender = control2;
            targets[2].Source = control3;
            targets[3].Sender = contentElement6;
            targets[3].Source = control4;
            targets[4].Sender = contentElement5;
            targets[4].Source = control4;
            targets[5].Sender = contentHost1;
            targets[5].Source = control4;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            control4.RaiseEvent(args);

            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[6];
            targets[5].Sender = control4;
            targets[5].Source = control4;
            targets[4].Sender = control3;
            targets[4].Source = control3;
            targets[3].Sender = control2;
            targets[3].Source = control3;
            targets[2].Sender = contentElement6;
            targets[2].Source = control4;
            targets[1].Sender = contentElement5;
            targets[1].Source = control4;
            targets[0].Sender = contentHost1;
            targets[0].Source = control4;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            control4.RaiseEvent(args);

            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomContentHost       contentHost1;
        private CustomControl           control2;
        private CustomControl           control3;
        private CustomControl           control4;
        private CustomContentElement    contentElement5;
        private CustomContentElement    contentElement6;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite21 : DrtTestSuite
    {

        private class MyDockPanel : DockPanel
        {
            public MyDockPanel()
            {
                _children = new VisualCollection(this);
            }

            new public VisualCollection Children
            {
                get
                {
                    return _children;
                }
            }

            /// <summary>
            ///   Derived class must implement to support Visual children. The method must return
            ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
            ///
            ///    By default a Visual does not have any children.
            ///
            ///  Remark: 
            ///       During this virtual call it is not valid to modify the Visual tree. 
            /// </summary>
            protected override Visual GetVisualChild(int index)
            {            
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
            ///  Derived classes override this property to enable the Visual code to enumerate 
            ///  the Visual children. Derived classes need to return the number of children
            ///  from this method.
            ///
            ///    By default a Visual does not have any children.
            ///
            ///  Remark: During this virtual method the Visual tree must not be modified.
            /// </summary>        
            protected override int VisualChildrenCount
            {           
                get 
                { 
                    if(_children == null)
                    {
                        throw new ArgumentOutOfRangeException("_children is null");
                    }                
                    return _children.Count; 
                }
            }                  

            private VisualCollection _children;
        }
    
        internal TestSuite21(): base("StyleHandlers")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Simple tree with control1 logical parent of control2 logical parent of control3
            // Tests instance and style handler invocation for bubble and tunnel events on all three tree nodes

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();

            // Construct tree
            control1.AppendModelChild(control2);
            control2.AppendModelChild(control3);

            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;

            // Create a style for CustomControl
            Style s = new Style(typeof(CustomControl));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(MyDockPanel));
            ControlTemplate template = new ControlTemplate(typeof(CustomControl));
            template.VisualTree = fef;
            s.Setters.Add(new Setter(Control.TemplateProperty, template));

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            EventSetter eventSetter = new EventSetter();
            eventSetter.Event = bubbleEvent;
            eventSetter.Handler = new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent);
            eventSetter.HandledEventsToo = false;
            s.Setters.Add(eventSetter);
            fef.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            eventSetter = new EventSetter();
            eventSetter.Event = tunnelEvent;
            eventSetter.Handler = new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent);
            eventSetter.HandledEventsToo = false;
            s.Setters.Add(eventSetter);
            fef.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Add the style to the ResourceDictionary for the root of the tree
            control1.Resources[typeof(CustomControl)] = s;

            // Expand the style visual tree
            control1.ApplyTemplate();
            control2.ApplyTemplate();
            control3.ApplyTemplate();

            // Remember the visual tree nodes that are generated 
            // so we can verify the target that the event has been fired on
            dockPanel1 = (MyDockPanel)VisualTreeHelper.GetChild(control1,0);
            dockPanel2 = (MyDockPanel)VisualTreeHelper.GetChild(control2,0);
            dockPanel3 = (MyDockPanel)VisualTreeHelper.GetChild(control3,0);

            // Connect the DockPanel to the logical child of the control. 
            // This should be happening automatically in a real life scenario
            dockPanel1.Children.Add(control2);
            dockPanel2.Children.Add(control3);
            
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[9];
            targets[0].Sender = dockPanel3;
            targets[0].Source = dockPanel3;
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[2].Sender = control3;
            targets[2].Source = control3;
            targets[3].Sender = dockPanel2;
            targets[3].Source = dockPanel2;
            targets[4].Sender = control2;
            targets[4].Source = control3;
            targets[5].Sender = control2;
            targets[5].Source = control3;
            targets[6].Sender = dockPanel1;
            targets[6].Source = dockPanel1;
            targets[7].Sender = control1;
            targets[7].Source = control3;
            targets[8].Sender = control1;
            targets[8].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            dockPanel3.RaiseEvent(args);

            if (args.NumEventsFired != 9)
            {
                throw new Exception("Expected 9 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[9];
            targets[8].Sender = dockPanel3;
            targets[8].Source = dockPanel3;
            targets[7].Sender = control3;
            targets[7].Source = control3;
            targets[6].Sender = control3;
            targets[6].Source = control3;
            targets[5].Sender = dockPanel2;
            targets[5].Source = dockPanel2;
            targets[4].Sender = control2;
            targets[4].Source = control3;
            targets[3].Sender = control2;
            targets[3].Source = control3;
            targets[2].Sender = dockPanel1;
            targets[2].Source = dockPanel1;
            targets[1].Sender = control1;
            targets[1].Source = control3;
            targets[0].Sender = control1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            dockPanel3.RaiseEvent(args);

            if (args.NumEventsFired != 9)
            {
                throw new Exception("Expected 9 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;

        private MyDockPanel dockPanel1;
        private MyDockPanel dockPanel2;
        private MyDockPanel dockPanel3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }

    internal sealed class TestSuite22 : DrtTestSuite
    {

        private class MyDockPanel : DockPanel
        {
            public MyDockPanel()
            {
                _children = new VisualCollection(this);
            }

            new public VisualCollection Children
            {
                get
                {
                    return _children;
                }
            }

            /// <summary>
            ///   Derived class must implement to support Visual children. The method must return
            ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
            ///
            ///    By default a Visual does not have any children.
            ///
            ///  Remark: 
            ///       During this virtual call it is not valid to modify the Visual tree. 
            /// </summary>
            protected override Visual GetVisualChild(int index)
            {            
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
            ///  Derived classes override this property to enable the Visual code to enumerate 
            ///  the Visual children. Derived classes need to return the number of children
            ///  from this method.
            ///
            ///    By default a Visual does not have any children.
            ///
            ///  Remark: During this virtual method the Visual tree must not be modified.
            /// </summary>        
            protected override int VisualChildrenCount
            {           
                get 
                { 
                    if(_children == null)
                    {
                        throw new ArgumentOutOfRangeException("_children is null");
                    }                
                    return _children.Count; 
                }
            }                  

            private VisualCollection _children;
        }
    

    
        internal TestSuite22(): base("TemplateHandlers")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // Simple tree with control1 logical parent of control2 logical parent of control3
            // Tests instance and template handler invocation for bubble and tunnel events on all three tree nodes

            // Create controls
            control1 = new CustomControl();
            control2 = new CustomControl();
            control3 = new CustomControl();

            // Construct tree
            control1.AppendModelChild(control2);
            control2.AppendModelChild(control3);

            // Create a template for CustomControl
            ControlTemplate t = new ControlTemplate(typeof(CustomControl));
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(MyDockPanel));
            t.VisualTree = fef;

            // Add the style to the ResourceDictionary for the root of the tree
            control1.Resources["myTemplate"] = t;
            
            // Fetch RoutedEvent for bubble event
            bubbleEvent = DrtEvents.RoutedEvent1;            

            // Add event handlers for bubble event
            control1.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            fef.AddHandler(bubbleEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Fetch RoutedEvent for tunnel event
            tunnelEvent = DrtEvents.RoutedEvent2;

            // Add event handlers for tunnel event
            control1.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control2.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            control3.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));
            fef.AddHandler(tunnelEvent, new CustomRoutedEventHandler(DrtEvents.OnRoutedEvent));

            // Set resource references for the template property
            control1.SetResourceReference(Control.TemplateProperty, "myTemplate");
            control2.SetResourceReference(Control.TemplateProperty, "myTemplate");
            control3.SetResourceReference(Control.TemplateProperty, "myTemplate");

            // Expand the template visual tree
            control1.ApplyTemplate();
            control2.ApplyTemplate();
            control3.ApplyTemplate();

            // Remember the visual tree nodes that are generated 
            // so we can verify the target that the event has been fired on
            dockPanel1 = (MyDockPanel)VisualTreeHelper.GetChild(control1,0);
            dockPanel2 = (MyDockPanel)VisualTreeHelper.GetChild(control2,0);
            dockPanel3 = (MyDockPanel)VisualTreeHelper.GetChild(control3,0);

            // Connect the DockPanel to the logical child of the control. 
            // This should be happening automatically in a real life scenario
            dockPanel1.Children.Add(control2);
            dockPanel2.Children.Add(control3);
            
            // return the lists of tests to run against the tree
            return new DrtTest[]{
                        new DrtTest(RaiseBubbleEvent),
                        new DrtTest(RaiseTunnelEvent)
                        };
        }

        private void RaiseBubbleEvent()
        {
            // Raise the bubble event
            RouteTarget[] targets = new RouteTarget[6];
            targets[0].Sender = dockPanel3;
            targets[0].Source = dockPanel3;
            targets[1].Sender = control3;
            targets[1].Source = control3;
            targets[2].Sender = dockPanel2;
            targets[2].Source = dockPanel2;
            targets[3].Sender = control2;
            targets[3].Source = control3;
            targets[4].Sender = dockPanel1;
            targets[4].Source = dockPanel1;
            targets[5].Sender = control1;
            targets[5].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(bubbleEvent, targets);
            dockPanel3.RaiseEvent(args);

            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 bubble event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private void RaiseTunnelEvent()
        {
            // Raise the tunnel event
            RouteTarget[] targets = new RouteTarget[6];
            targets[5].Sender = dockPanel3;
            targets[5].Source = dockPanel3;
            targets[4].Sender = control3;
            targets[4].Source = control3;
            targets[3].Sender = dockPanel2;
            targets[3].Source = dockPanel2;
            targets[2].Sender = control2;
            targets[2].Source = control3;
            targets[1].Sender = dockPanel1;
            targets[1].Source = dockPanel1;
            targets[0].Sender = control1;
            targets[0].Source = control3;
            CustomRoutedEventArgs args = new CustomRoutedEventArgs(tunnelEvent, targets);
            dockPanel3.RaiseEvent(args);

            if (args.NumEventsFired != 6)
            {
                throw new Exception("Expected 6 tunnel event handlers to be invoked. Got " + args.NumEventsFired + " instead.");
            }
        }

        private CustomControl control1;
        private CustomControl control2;
        private CustomControl control3;

        private MyDockPanel dockPanel1;
        private MyDockPanel dockPanel2;
        private MyDockPanel dockPanel3;

        private RoutedEvent bubbleEvent;
        private RoutedEvent tunnelEvent;
    }
    
    internal class CustomRoutedEventArgs : RoutedEventArgs
    {
        #region ExternalAPI

        /// <summary>
        ///     Gettor and Settor for NumEventsFired
        /// </summary>
        public int NumEventsFired
        {
            get {return _numEventsFired;}
            set {_numEventsFired = value;}
        }

        /// <summary>
        ///     Gettor and Settor for NumClassEventsFired
        /// </summary>
        public int NumClassEventsFired
        {
            get {return _numClassEventsFired;}
            set {_numClassEventsFired = value;}
        }

        /// <summary>
        ///     Set Shouldmath flag to the value given
        /// </summary>
        public bool ShouldMath
        {
            get {return _shouldMath;}
            set {_shouldMath = value;}
        }

        /// <summary>
        ///     Returns targets array
        /// </summary>
        public RouteTarget[] Targets
        {
            get {return _targets;}
        }

        /// <summary>
        ///     Invokes the event handler with the
        ///     appropriate arguments
        /// </summary>
        /// <param name="genericHandler">
        ///     Generic Handler to be invoked
        /// </param>
        /// <param name="genericTarget">
        ///     Target on whom the Handler will be invoked
        /// </param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            CustomRoutedEventHandler handler = (CustomRoutedEventHandler)genericHandler;
            handler(genericTarget, this);
        }

        #endregion ExternalAPI

        #region Construction

        /// <summary>
        ///     Constructor for CustomRoutedEventArgs
        /// </summary>
        public CustomRoutedEventArgs(
            RoutedEvent routedEvent,
            RouteTarget[] targets) : base()
        {
            base.RoutedEvent=routedEvent;
            _targets = targets;
            _numEventsFired = 0;
            _numClassEventsFired = 0;
            _shouldMath = true;
        }

        /// <summary>
        ///     Constructor for CustomRoutedEventArgs
        /// </summary>
        public CustomRoutedEventArgs(
            RoutedEvent routedEvent,
            RouteTarget[] targets,
            bool handled) : this(
                routedEvent,
                targets)
        {
            base.Handled = handled;
        }

        #endregion Construction

        #region Data

        private RouteTarget[]   _targets;
        private int             _numEventsFired;
        private int             _numClassEventsFired;
        bool                    _shouldMath;

        #endregion Data

    }

    internal struct RouteTarget
    {
        public object Sender;
        public object Source;
    }

    /// <summary>
    ///     CustomRoutedEventHandler Definition
    /// </summary>
    internal delegate void CustomRoutedEventHandler(object sender, CustomRoutedEventArgs args);

    /// <summary>
    ///     CustomDelegate Definition
    /// </summary>
    public delegate void CustomDelegate(object sender, EventArgs args, object other);

}



