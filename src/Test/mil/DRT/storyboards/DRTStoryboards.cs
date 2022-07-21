// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;  // ISupportInitialize
using System.IO; // StreamWriter
using System.Reflection; 
using System.Windows.Threading;

using System.Windows;
using System.Windows.Controls; // Button as test object
using System.Windows.Data;  // Binding for DataTrigger tests
using System.Windows.Input; // Events for EventTrigger smoke test
using System.Windows.Media;
using System.Windows.Media.Animation; // Storyboard support
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DRT
{
    // Timelines don't run unless inside an Application object, and that object
    //  must show a Window.  This is needed to have all the related objects
    //  (MediaContext, ec.) set up correctly.
    public class StoryboardsTest : Application
    {
        [STAThread]
        public static int Main(string[] args)
        {
            Console.WriteLine("Storyboards Developer Regression Tests [Contact: Microsoft]");

            StreamWriter logFile = File.CreateText("DRTStoryboards.log");
            logFile.AutoFlush = true;

            // Turn on dummy tracing; it won't go anywhere, but will exercise the code.
            EnableNoopTracing();
            
            using( logFile ) // So in case of failure, it'll be Disposed and contents flushed.
            {
                logFile.WriteLine("DRTStoryboards - logging mechanism up and running.");
                StoryboardsTest storyboardsTest = new StoryboardsTest();
                storyboardsTest.LogFile = logFile;
                storyboardsTest.Run();

                logFile.WriteLine("DRTStoryboards - Successful execution, now terminating.");
                Console.WriteLine("SUCCESS!");
            }
            return 0;
        }

        ///////////////////////////////////////////////////////////////////
        // Handle to logfile
        public StreamWriter LogFile
        {
            get
            {
                return _logFile;
            }
            set
            {
                _logFile = value;
            }
        }


        ///////////////////////////////////////////////////////////////////
        // This helper is coped from DrtBase, since this DRT doesn't
        // subclass it.
        static void EnableNoopTracing()
        {
            PresentationTraceSources.Refresh();

            PropertyInfo[] propertyInfos = typeof(PresentationTraceSources).GetProperties( BindingFlags.Static | BindingFlags.Public );
            foreach( PropertyInfo propertyInfo in propertyInfos )
            {
                if( propertyInfo.PropertyType != typeof(TraceSource) )
                {
                    continue;
                }

                TraceSource traceSource = propertyInfo.GetValue(null, null) as TraceSource;
                traceSource.Switch.Level = SourceLevels.All;
            }
        }


        ///////////////////////////////////////////////////////////////////
        // Set up a timer that paces the test state and ensures that we
        //  don't hang forever.
        private void SetupHeartbeat()
        {
            LogFile.WriteLine("5000 - Setting up the heartbeat UITimer for polling values");
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Tick += new EventHandler(OnTimerTick);
            _timer.Interval = TimeSpan.FromMilliseconds(125);
            _timer.Start();
            LogFile.WriteLine("5900 - Heartbeat UITimer kicked off");
        }

        ///////////////////////////////////////////////////////////////////
        // My favorite way to test Storyboard functionality is to set up
        //  two SetterTimelines that toggle a property between two values.
        // This function serves as the common state machine used by several
        //  such tests.
        // The string parameters are used to give meaningful exceptions
        //  in case of failure.

        bool CheckToggledPropertyState( string testName,
            ref int toggleState, object currentValue, object[] valuesSeen,
            object[] valuesExpected, string unexpectedValueMessage )
        {
            bool testSuccess = false;

            // If we want to generalize, turn this into a loop
            if( !currentValue.Equals(valuesExpected[0]) &&
                !currentValue.Equals(valuesExpected[1]) )
            {
                throw new Exception(unexpectedValueMessage + currentValue);
            }

            // State machine.
            switch(toggleState)
            {
                case 0: // == We just started up, just see what we've got.
                    valuesSeen[0] = currentValue;
                    toggleState = 1;
                    break;
                case 1: // == We've picked up an initial value, look for the
                        //  first transition.
                    if( !currentValue.Equals(valuesSeen[0]) )
                    {
                        valuesSeen[1] = currentValue;
                        toggleState = 2;
                    }
                    break;
                case 2: // == Now looking for transition back to the first value
                    if( !currentValue.Equals(valuesSeen[1]) )
                    {
                        if( currentValue.Equals(valuesSeen[0]) )
                        {
                            // Looks good, all done.
                            toggleState = 3;
                            testSuccess = true;
                        }
                    }
                    break;
                default: // Unhappy
                    throw new Exception("DRTProperty entered an invalid state for " +
                        testName+".  Please e-mail DRT owner - Should not be re-checking property values once test is successful.");
            }

            return testSuccess;
        }


        ///////////////////////////////////////////////////////////////////
        //  Set up tests upon application initialization
        protected override void OnStartup(StartupEventArgs e)
        {
            LogFile.WriteLine("0000 - OnStartingUp begin.");

            TestInheritanceContext();

            ///////////////////////////////////////////////////////////////////
            //  Tests for old deprecated Storyboard

            
            // Display a window, or else none of the timeline things work.
            _window = new Window();

            _window.Title = "DRTStoryboards";
            _panel = new StackPanel();
            
            ((StackPanel)_panel).Orientation = Orientation.Vertical;

            _onLoadedTest = new OnLoadedTest(LogFile);
            _onLoadedTest.Setup(_panel);

            SetupStyleTriggerEnterExits( LogFile, _panel );

            _window.Content = _panel;

            TestClock.Setup(_panel, LogFile);

            // Display the window
            
            _window.Show();

            // Start pinging the window to verify the timing
            
            SetupHeartbeat();       // Heartbeat timer.
            
            LogFile.WriteLine("6000 - OnStartingUp end.");
        }

        ///////////////////////////////////////////////////////////////////
        // This is fired once every second, and where we check for the current
        //  state of the system and measure our progress.
        void OnTimerTick( object source, EventArgs e )
        {
            bool testsPassed;

            LogFile.WriteLine(_timerTicks++ + "================================================");

            // run the TestClock test after 10 ticks
            // timer ticks every 125ms
            if (!_testClockPassed && _timerTicks == 8)
            {
                _testClockPassed = TestClock.Verify();
            }

            if ( !_onLoadedTest.Passed )
            {
                _onLoadedTest.Verify();
            }

            if ( !StyleTriggerEnterExitsPassed )
            {
                VerifyStyleTriggerEnterExits();
            }

            testsPassed = 
                _testClockPassed &&
                _onLoadedTest.Passed &&
                StyleTriggerEnterExitsPassed;

            // Check all goals against success
            if( testsPassed )
            {
                // All test conditions passed, move on to the markup test.
                TestMarkupParse();
            }

            // Throw because we timed out before accomplishing all goals.
            if( _timerTicks > _timerTickMax )
            {
                throw new Exception("DRTStoryboards has timed out.");
            }
        }

        ///////////////////////////////////////////////////////////////////////
        // Aggregates the tests dealing with enter/exit actions for Style

        // Aggregates setup of the trigger enter/exit tests for Style
        private void SetupStyleTriggerEnterExits( StreamWriter logFile, Panel panel )
        {
            _triggerTestStyle = new TriggerTestStyle(logFile);
            _triggerTestStyle.Setup(panel);

            _multiTriggerTestStyle = new MultiTriggerTestStyle(logFile);
            _multiTriggerTestStyle.Setup(panel);

            _dataTriggerTestStyle = new DataTriggerTestStyle(logFile);
            _dataTriggerTestStyle.Setup(panel);

            _multiDataTriggerTestStyle = new MultiDataTriggerTestStyle(logFile);
            _multiDataTriggerTestStyle.Setup(panel);
        }
        
        // Aggregates verification of the trigger enter/exit tests for Style
        private void VerifyStyleTriggerEnterExits()
        {
            if ( !_triggerTestStyle.Passed )
            {
                _triggerTestStyle.Verify();
            }

            if (!_multiTriggerTestStyle.Passed )
            {
                _multiTriggerTestStyle.Verify();
            }

            if (!_dataTriggerTestStyle.Passed )
            {
                _dataTriggerTestStyle.Verify();
            }

            if (!_multiDataTriggerTestStyle.Passed )
            {
                _multiDataTriggerTestStyle.Verify();
            }
        }

        // Aggregates test state of the trigger enter/exit tests for Style
        private bool StyleTriggerEnterExitsPassed
        {
            get
            {
                return  _triggerTestStyle.Passed &&
                        _multiTriggerTestStyle.Passed &&
                        _dataTriggerTestStyle.Passed &&
                        _multiDataTriggerTestStyle.Passed;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        //
        // Validate the checking we do to ensure that storyboards are only on
        // the logical root.
        void TestInheritanceContext()
        {


			int i = 0;
            //while(true) Blocked by bug 1205782
            {
                /*
                    window
                      \
                      TriggerCollection
                        \
                        EventTrigger
                          \
                          BeginStoryboard
                            \
                            Storyboard
                              \
                              DoubleAnimation (with resource reference)

                 */
                 
                Window window = new Window();
                EventTrigger eventTrigger = new EventTrigger();
                Storyboard storyboard = new Storyboard();
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                BeginStoryboard beginStoryboard = new BeginStoryboard();

                window.Triggers.Add( eventTrigger );
                eventTrigger.Actions.Add( beginStoryboard );
                beginStoryboard.Storyboard = storyboard;
                storyboard.Children.Add( doubleAnimation );

                window.Resources[ "isAdditive" ] = false;

                DynamicResourceExtension resource = new DynamicResourceExtension();
                resource.ResourceKey = "isAdditive";
                doubleAnimation.SetValue( DoubleAnimation.IsAdditiveProperty, resource.ProvideValue(null) );

                if (doubleAnimation.IsAdditive)
                {
                    throw new Exception( "IsAdditive resource reference not inheriting to double animation (a)" );
                }

                window.Resources[ "isAdditive" ] = true;

                if (!doubleAnimation.IsAdditive)
                {
                    throw new Exception( "IsAdditive resource reference not inheriting to double animation (b)" );
                }
                
                //switch(i)
                //{
                    /*  Blocked by bug 1205769
                    
                    case 0:
                        storyboard.Children.Remove( doubleAnimation );
                        break;

                    case 1:
                        storyboard.Children.Clear();
                        break;

                    case 2:
                        storyboard.Children.RemoveAt(0);
                        break;
                    */
                    /* Blocked by bug 1205782
                    case 3:
                        beginStoryboard.ClearValue( BeginStoryboard.StoryboardProperty );
                        break;

                    case 4:
                        beginStoryboard.Storyboard = new Storyboard();
                        break;

                    case 5:
                        eventTrigger.Actions.Remove( beginStoryboard );
                        break;

                    case 6:
                        eventTrigger.Actions.Clear();
                        break;

                    case 7:
                        eventTrigger.Actions.RemoveAt (0);
                        break;

                    case 8:
                        window.Triggers.Remove( eventTrigger );
                        break;

                    case 9:
                        window.Triggers.Clear();
                        break;

                    case 10:
                        window.Triggers.RemoveAt(0);
                        break;

                    case 11:
                        return;

                    default:
                        i++;
                        continue;*/
                //}

                /* Blocked by bug 1205782
                if (doubleAnimation.IsAdditive)
                {
                    string str = "IsAdditive resource reference not inheriting to double animation (" + i + ")";
                    throw new Exception( str );
                }*/

                i++;
            }

        }

        // No verification of functionality here, this one will just make
        //  sure that everything parses.  Some functionality is triggered
        //  via FrameworkElement.Loaded but no verification is done.
        void TestMarkupParse()
        {
            if( _markupWindowTimer != null )
            {
                // We may get triggered multiple times by the other Storyboard
                //  timer.  A non-null timer means we've already started and there
                //  is no need to start again.
                return;
            }
            
            LogFile.WriteLine("Creating a window to use for testing Storyboard markup parsing.");
            
            _markupWindow = new Window();
            _markupWindow.Title = "Storyboard Markup Parse Test";
            _markupWindow.Width = 800;
            _markupWindow.Height= 600;
            _markupWindow.Show();

            LogFile.WriteLine("Opening Storyboard test markup file.");
            FileStream markupTest = new FileStream(@"DrtFiles\DRTStoryboards\DRTStoryboards.xaml", FileMode.Open);

            LogFile.WriteLine("Parsing Storyboard test markup file.");
            _markupWindow.Content = XamlReader.Load(markupTest);
            
            LogFile.WriteLine("Parse complete for Storyboard test markup file, start timer for shutdown.");
            _markupWindowTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _markupWindowTimer.Tick += new EventHandler(CloseTestMarkupParseWindow);
            _markupWindowTimer.Interval = TimeSpan.FromMilliseconds(500);
            _markupWindowTimer.Start();
        }

        void CloseTestMarkupParseWindow(object source, EventArgs e)
        {
            LogFile.WriteLine("Closing window for Storyboard test markup file.");
            _markupWindow.Close();
            Shutdown();
        }

        // App window needed for timelines to work
        Window    _window = null;

        // Panel where all the objects live
        Panel     _panel  = null;

        // Timer to keep things going
        DispatcherTimer   _timer = null;
        int       _timerTicks = 0; // Number of seconds
        const int _timerTickMax = 120; // Timeout limit

        // Handle to log file
        StreamWriter _logFile = null;

        // Test pass flags
        bool      _testClockPassed = false;

        // Test classes
        OnLoadedTest          _onLoadedTest;
        
        TriggerTestStyle        _triggerTestStyle;
        MultiTriggerTestStyle   _multiTriggerTestStyle;
        DataTriggerTestStyle    _dataTriggerTestStyle;
        MultiDataTriggerTestStyle _multiDataTriggerTestStyle;

        // The window we use to display the markup parse results, and the timer
        //  used to shut it down.
        Window          _markupWindow = null;
        DispatcherTimer _markupWindowTimer = null;
        
    }

    internal class TestBase
    {
        internal TestBase(StreamWriter logFile)
        {
            _logFile = logFile;
            _testPassed = false;
        }

        internal virtual void Setup(Panel panel)
        {
        }

        internal virtual void Verify()
        {
        }

        internal bool Passed
        {
            get
            {
                return _testPassed;
            }
        }

        protected StreamWriter _logFile;
        protected bool _testPassed;
    }

    // This class creates a Storyboard that is to be triggered by 
    //  FrameworkElement.Loaded.  The Storyboard is a simple zero duration
    //  animation.  The Verify routine looks for the "to" value of the animation.
    internal class OnLoadedTest : TestBase
    {
        internal OnLoadedTest( StreamWriter logFile ) : base(logFile) {}
        
        internal override void Setup(Panel panel)
        {
            _logFile.Write("OnLoadedTest Setup...");

            DoubleAnimation da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0);
            da.To = 200;

            Storyboard s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            BeginStoryboard bs = new BeginStoryboard();
            bs.Storyboard = s;

            EventTrigger et = new EventTrigger();
            et.RoutedEvent = FrameworkElement.LoadedEvent;
            et.Actions.Add(bs);            
            
            _testRect = new Rectangle();
            _testRect.Fill = new SolidColorBrush(Colors.Blue);
            _testRect.Height=20;
            _testRect.Width=40;
            
            _testRect.Triggers.Add(et);

            panel.Children.Add(_testRect);

            _logFile.WriteLine("completed");
        }

        internal override void Verify()
        {
            if( !Passed )
            {

                _logFile.Write("OnLoadedTest Verify in progress... ");

                if( _testRect.Width == 200 )
                {
                    _testPassed = true;
                }

                if( Passed )
                {
                    _logFile.WriteLine("pass.");
                }
                else
                {
                    _logFile.WriteLine("fail.");
                }
            }
        }

        private Rectangle _testRect;
    }

    // Tests the functionality of a trigger animation.  Test consists
    //  of multiple states.
    //  1) Trigger inactive - verify "before" of Style Setter value
    //  2) Trigger active - verify "after" of the hold-at-end value of EnterAction
    //  3) Trigger inactive again - verify the hold-at-end value of ExitAction
    //  4) All verification complete.
    
    internal class TriggerTestStyle : TestBase
    {
        internal TriggerTestStyle( StreamWriter logFile ) : base(logFile) {}

        internal override void Setup(Panel panel)
        {
            _logFile.Write("TriggerTestStyle Setup...");

            _verifyState = 1;

            // Markup equivalent of what we're building here in code:
            //
            // <Rectangle Height="20" Fill="AliceBlue">
            //  <Rectangle.Style>
            //   <Style TargetType="{x:Type Rectangle}">
            //    <Setter Property="Width" Value="40" />
            //    <Style.Triggers>
            //     <Trigger Property="DockPanel.Dock" Value="Left">
            //      <Trigger.EnterActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="200" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </Trigger.EnterActions>
            //      <Trigger.ExitActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="100" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </Trigger.ExitActions>
            //     </Trigger>
            //    </Style.Triggers>
            //   </Style>
            //  </Rectangle.Style>
            // </Rectangle>

            _testRect = new Rectangle();
            _testRect.Fill = new SolidColorBrush(Colors.AliceBlue);
            _testRect.Height=20;

            DockPanel.SetDock(_testRect, Dock.Top);
            
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 200;

            Storyboard s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            BeginStoryboard bs = new BeginStoryboard();
            bs.Storyboard = s;

            Trigger t = new Trigger();
            t.Property=DockPanel.DockProperty;
            t.Value=Dock.Left;
            t.EnterActions.Add(bs);

            da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 100;

            s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            bs = new BeginStoryboard();
            bs.Storyboard = s;

            t.ExitActions.Add(bs);

            Style style = new Style();
            style.TargetType = typeof(Rectangle);

            Setter setter = new Setter();
            setter.Property=FrameworkElement.WidthProperty;
            setter.Value = (double)40;

            style.Setters.Add(setter);
            
            style.Triggers.Add(t);

            _testRect.Style = style;

            panel.Children.Add(_testRect);
            
            _logFile.WriteLine("completed");
        }

        internal override void Verify()
        {
            if( !Passed )
            {
                double width = _testRect.Width;
                
                _logFile.Write("TriggerTestStyle Verify in progress...");

                if( _verifyState == 1 && width == 40 )
                {
                    _verifyState = 2;
                    DockPanel.SetDock(_testRect, Dock.Left);
                }

                if( _verifyState == 2 && width == 200 )
                {
                    _verifyState = 3;
                    DockPanel.SetDock(_testRect, Dock.Top);
                }

                if( _verifyState == 3 && width == 100 )
                {
                    _verifyState = 4;
                }

                if( _verifyState == 4 )
                {
                    _testPassed = true;
                    _logFile.WriteLine("pass.");
                }
                else
                {
                    _logFile.WriteLine("pending at state " + _verifyState + " on width of " + width);
                }
            }
        }

        private Rectangle _testRect;
        private int _verifyState;
    }

    // Tests a Storyboard triggered from MultiTrigger.  Same state as TriggerTest.
    internal class MultiTriggerTestStyle : TestBase
    {
        internal MultiTriggerTestStyle( StreamWriter logFile ) : base(logFile) {}

        internal override void Setup(Panel panel)
        {
            _logFile.Write("MultiTriggerTestStyle Setup...");

            _verifyState = 1;

            // Markup equivalent of what we're building here in code:
            //
            // <Rectangle Height="20" Fill="Chartreuse">
            //  <Rectangle.Style>
            //   <Style TargetType="{x:Type Rectangle}">
            //    <Setter Property="Width" Value="200" />
            //    <Style.Triggers>
            //     <MultiTrigger>
            //      <MultiTrigger.Conditions>
            //       <Condition Property="FrameworkElement.Height" Value="20" />
            //       <Condition Property="DockPanel.Dock" Value="Left" />
            //      </MultiTrigger.Conditions>
            //      <MultiTrigger.EnterActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="20" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </MultiTrigger.EnterActions>
            //      <MultiTrigger.ExitActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="120" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </MultiTrigger.ExitActions>
            //     </MultiTrigger>
            //    </Style.Triggers>
            //   </Style>
            //  </Rectangle.Style>
            // </Rectangle>

            _testRect = new Rectangle();
            _testRect.Fill = new SolidColorBrush(Colors.Chartreuse);
            _testRect.Height=20;

            DockPanel.SetDock(_testRect, Dock.Top);
            
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 20;

            Storyboard s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            BeginStoryboard bs = new BeginStoryboard();
            bs.Storyboard = s;

            MultiTrigger t = new MultiTrigger();
            t.Conditions.Add(new Condition( FrameworkElement.HeightProperty, (double)20));
            t.Conditions.Add(new Condition( DockPanel.DockProperty, Dock.Left));
            
            t.EnterActions.Add(bs);

            da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 120;

            s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            bs = new BeginStoryboard();
            bs.Storyboard = s;

            t.ExitActions.Add(bs);

            Style style = new Style();
            style.TargetType = typeof(Rectangle);

            Setter setter = new Setter();
            setter.Property=FrameworkElement.WidthProperty;
            setter.Value = (double)200;

            style.Setters.Add(setter);
            
            style.Triggers.Add(t);

            _testRect.Style = style;

            panel.Children.Add(_testRect);
            
            _logFile.WriteLine("completed");
        }

        internal override void Verify()
        {
            if( !Passed )
            {
                double width = _testRect.Width;
                
                _logFile.Write("MultiTriggerTestStyle Verify in progress...");

                if( _verifyState == 1 && width == 200 )
                {
                    _verifyState = 2;
                    DockPanel.SetDock(_testRect, Dock.Left);
                }

                if( _verifyState == 2 && width == 20 )
                {
                    _verifyState = 3;
                    DockPanel.SetDock(_testRect, Dock.Top);
                }

                if( _verifyState == 3 && width == 120 )
                {
                    _verifyState = 4;
                }

                if( _verifyState == 4 )
                {
                    _testPassed = true;
                    _logFile.WriteLine("pass.");
                }
                else
                {
                    _logFile.WriteLine("pending at state " + _verifyState + " on width of " + width);
                }
            }
        }

        private Rectangle _testRect;
        private int _verifyState;
    }

    internal class DataTriggerTestStyle : TestBase
    {
        internal DataTriggerTestStyle( StreamWriter logFile ) : base(logFile) {}

        internal override void Setup(Panel panel)
        {
            _logFile.Write("DataTriggerTestStyle Setup...");

            _verifyState = 1;

            // Markup equivalent of what we're building here in code:
            //
            // <Rectangle Height="20" Fill="DarkMagenta">
            //  <Rectangle.Style>
            //   <Style TargetType="{x:Type Rectangle}">
            //    <Setter Property="Width" Value="10" />
            //    <Style.Triggers>
            //     <DataTrigger Binding="{Binding RelativeSource={RelativeSource Self}, Path=DockPanel.Dock}">
            //       (DataTrigger.Value = Dock.Left, however you do that in markup...)
            //      <DataTrigger.EnterActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="200" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </DataTrigger.EnterActions>
            //      <DataTrigger.ExitActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="150" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </DataTrigger.ExitActions>
            //     </DataTrigger>
            //    </Style.Triggers>
            //   </Style>
            //  </Rectangle.Style>
            // </Rectangle>

            _testRect = new Rectangle();
            _testRect.Fill = new SolidColorBrush(Colors.DarkMagenta);
            _testRect.Height=20;

            DockPanel.SetDock(_testRect, Dock.Top);
            
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 200;

            Storyboard s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            BeginStoryboard bs = new BeginStoryboard();
            bs.Storyboard = s;

            DataTrigger dt = new DataTrigger();

            Binding b = new Binding();
            b.RelativeSource = RelativeSource.Self;
            b.Path = new PropertyPath("(0)", DockPanel.DockProperty);
            
            dt.Binding=b;
            dt.Value=Dock.Left;
            dt.EnterActions.Add(bs);

            da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 150;

            s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            bs = new BeginStoryboard();
            bs.Storyboard = s;

            dt.ExitActions.Add(bs);

            Style style = new Style();
            style.TargetType = typeof(Rectangle);

            Setter setter = new Setter();
            setter.Property=FrameworkElement.WidthProperty;
            setter.Value = (double)10;

            style.Setters.Add(setter);
            
            style.Triggers.Add(dt);

            _testRect.Style = style;

            panel.Children.Add(_testRect);
            
            _logFile.WriteLine("completed");
        }

        internal override void Verify()
        {
            if( !Passed )
            {
                double width = _testRect.Width;
                
                _logFile.Write("DataTriggerTestStyle Verify in progress...");

                if( _verifyState == 1 && width == 10 )
                {
                    _verifyState = 2;
                    DockPanel.SetDock(_testRect, Dock.Left);
                }

                if( _verifyState == 2 && width == 200 )
                {
                    _verifyState = 3;
                    DockPanel.SetDock(_testRect, Dock.Top);
                }

                if( _verifyState == 3 && width == 150 )
                {
                    _verifyState = 4;
                }

                if( _verifyState == 4 )
                {
                    _testPassed = true;
                    _logFile.WriteLine("pass.");
                }
                else
                {
                    _logFile.WriteLine("pending at state " + _verifyState + " on width of " + width);
                }
            }
        }

        private Rectangle _testRect;
        private int _verifyState;
    }

    internal class MultiDataTriggerTestStyle : TestBase
    {
        internal MultiDataTriggerTestStyle( StreamWriter logFile ) : base(logFile) {}

        internal override void Setup(Panel panel)
        {
            _logFile.Write("MultiDataTriggerTestStyle Setup...");

            _verifyState = 1;

            // Markup equivalent of what we're building here in code:
            //
            // <Rectangle Height="20" Fill="Gold">
            //  <Rectangle.Style>
            //   <Style TargetType="{x:Type Rectangle}">
            //    <Setter Property="Width" Value="300" />
            //    <Style.Triggers>
            //     <MultiDataTrigger>
            //      <MultiDataTrigger.Conditions>
            //       <Condition Binding="{Binding RelativeSource={RelativeSource Self}, Path=DockPanel.Dock}">
            //        (Condition.Value = Dock.Left, however you do that in markup...)
            //       </Condition>
            //       <Condition Binding="{Binding RelativeSource={RelativeSource Self}, Path="FrameworkElement.Height}">
            //        (Condition.Value = 20, however you do that in markup... this one is always true.)
            //       </Condition>
            //      </MultiDataTrigger.Conditions>
            //      <MultiDataTrigger.EnterActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="20" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </MultiDataTrigger.EnterActions>
            //      <MultiDataTrigger.ExitActions>
            //       <BeginStoryboard>
            //        <Storyboard TargetProperty="(FrameworkElement.Width)">
            //         <DoubleAnimation Duration="0:0:0.5" To="200" />
            //        </Storyboard>
            //       </BeginStoryboard>
            //      </MultiDataTrigger.ExitActions>
            //     </MultiDataTrigger>
            //    </Style.Triggers>
            //   </Style>
            //  </Rectangle.Style>
            // </Rectangle>

            _testRect = new Rectangle();
            _testRect.Height=20;
            _testRect.Fill = new SolidColorBrush(Colors.Gold);

            DockPanel.SetDock(_testRect, Dock.Top);
            
            DoubleAnimation da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 20;

            Storyboard s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            BeginStoryboard bs = new BeginStoryboard();
            bs.Storyboard = s;

            MultiDataTrigger mdt = new MultiDataTrigger();

            Binding b = new Binding();
            b.RelativeSource = RelativeSource.Self;
            b.Path = new PropertyPath("(0)", DockPanel.DockProperty);

            Condition c = new Condition();
            c.Binding = b;
            c.Value = Dock.Left;

            mdt.Conditions.Add(c);

            b = new Binding();
            b.RelativeSource = RelativeSource.Self;
            b.Path = new PropertyPath("(0)", FrameworkElement.HeightProperty);
            
            c = new Condition();
            c.Binding = b;
            c.Value = (double)20;

            mdt.Conditions.Add(c);
            
            mdt.EnterActions.Add(bs);

            da = new DoubleAnimation();
            da.Duration = TimeSpan.FromSeconds(0.5);
            da.To = 200;

            s = new Storyboard();
            s.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0)", FrameworkElement.WidthProperty));
            s.Children.Add(da);

            bs = new BeginStoryboard();
            bs.Storyboard = s;

            mdt.ExitActions.Add(bs);

            Style style = new Style();
            style.TargetType = typeof(Rectangle);

            Setter setter = new Setter();
            setter.Property=FrameworkElement.WidthProperty;
            setter.Value = (double)300;

            style.Setters.Add(setter);
            
            style.Triggers.Add(mdt);

            _testRect.Style = style;

            panel.Children.Add(_testRect);
            
            _logFile.WriteLine("completed");
        }

        internal override void Verify()
        {
            if( !Passed )
            {
                double width = _testRect.Width;
                
                _logFile.Write("MultiDataTriggerTestStyle Verify in progress...");

                if( _verifyState == 1 && width == 300 )
                {
                    _verifyState = 2;
                    DockPanel.SetDock(_testRect, Dock.Left);
                }

                if( _verifyState == 2 && width == 20 )
                {
                    _verifyState = 3;
                    DockPanel.SetDock(_testRect, Dock.Top);
                }

                if( _verifyState == 3 && width == 200 )
                {
                    _verifyState = 4;
                }

                if( _verifyState == 4 )
                {
                    _testPassed = true;
                    _logFile.WriteLine("pass.");
                }
                else
                {
                    _logFile.WriteLine("pending at state " + _verifyState + " on width of " + width);
                }
            }
        }

        private Rectangle _testRect;
        private int _verifyState;
    }

    //
    // TestClock checks the Storyboard APIs that 
    // provide pass-through access to various methods
    // on the clocks.
    //
    public static class TestClock
    {
        public static void Setup(Panel panel, StreamWriter logfile)
        {
            LogFile = logfile;
            _rectangle = new Rectangle();
            _rectangle.Width = 40;
            _rectangle.Height = 20;
            _rectangle.Fill = Brushes.Red;
            panel.Children.Add(_rectangle);

            DoubleAnimation widthAnimation = new DoubleAnimation(200, TimeSpan.FromSeconds(10));
            _storyboard = new Storyboard();
            _storyboard.SpeedRatio = 2.0;
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath("Width"));
            _storyboard.Children.Add(widthAnimation);
            _storyboard.Begin(_rectangle, HandoffBehavior.SnapshotAndReplace, true);
        }

        public static bool Verify()
        {
            double? speed;
            int? iteration;
            double? progress;
            ClockState state = ClockState.Stopped;
            TimeSpan? time;

            // Verify that speed is 2, iteration 1, progress != 0, state == Active, 
            // and time != 0

            LogFile.WriteLine("Running GetCurrent* Tests");

            speed = _storyboard.GetCurrentGlobalSpeed(_rectangle);
            if (speed == null || speed != 2.0)
            {
                throw new Exception("GetCurrentGlobalSpeed returned incorrect value: " + speed);
            }

            iteration = _storyboard.GetCurrentIteration(_rectangle);
            if (iteration == null || iteration != 1)
            {
                throw new Exception("GetCurrentIteration returned incorrect value: " + iteration);
            }
            
            progress = _storyboard.GetCurrentProgress(_rectangle);
            if (progress == null || progress == 0.0)
            {
                throw new Exception("GetCurrentProgress returned incorrect value: " + progress);
            }

            state = _storyboard.GetCurrentState(_rectangle);
            if (state != ClockState.Active)
            {
                throw new Exception("GetCurrentState returned incorrect value: " + state);
            }

            time = _storyboard.GetCurrentTime(_rectangle);
            if (time == null || time == TimeSpan.Zero)
            {
                throw new Exception("GetCurrentTime returned incorrect value: " + time);
            }

            return true;
        }

        public static StreamWriter LogFile
        {
            get 
            {
                return _logFile;
            }

            set
            {
                _logFile = value;
            }
        }

        static Rectangle _rectangle = null;
        static Storyboard _storyboard = null;
        static StreamWriter _logFile = null;
    }
}

