// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Test raw touch
    /// 1. Touch events 
    /// 2. Stylus to Touch
    /// 3. Touch to Manipulations
    /// </summary>
    public partial class TestTouch : Window
    { 
        #region Constructor

        public TestTouch(string wintitle)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(wintitle))
            {
                this.Title = _title;
            }
            else
            {
                this.Title = wintitle;
            }

            //
            Attach();
        }
 
        #endregion

        #region Properties

        public GeometryModel3D ThumbModel
        {
            get
            {
                return this.FindResource("Thumb3DModel") as GeometryModel3D;
            }
        }

        /// <summary>
        /// default event handler
        /// </summary>
        public RoutedEventHandler DefaultEventHandler
        {
            get
            {
                return this._defaultEventHandler;
            }
            set
            {
                this._defaultEventHandler = value;
            }
        }

        /// <summary>
        /// existing touches
        /// </summary>
        public Dictionary<int, TouchDevice> ExistingTouches
        {
            get
            {
                return this._existingTouches;
            }
            set
            {
                this._existingTouches = value;
            }            
        }

        #endregion

        #region Touch Testing Helpers - TODOs

        /// <summary>
        /// test touch related DPs 
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="touchMap">touch map</param>
        /// <param name="getActualValue">delegate to get property value</param>
        /// <param name="getExpectedValue">delegate to get the expected property value</param>
        /// <param name="mergeExpectedValues">delegate to merge the expected</param>
        /// <param name="logFilter">log filter</param>
        /// <param name="captureMode">touch capture mode</param>
        /// <param name="captureInterval">touch capture internal</param>
        /// <param name="changeVisualTree">delegate to change visual tree</param>
        public void TestDependencyProperty(string message, Dictionary<int, TouchData> touchMap, 
            GetPropertyValue getActualValue, GetPropertyValue getExpectedValue, 
            MergePropertyValues mergeExpectedValues, Predicate<object> logFilter,
            CaptureMode captureMode, int captureInterval, AfterFrameSimulated changeVisualTree)
        {            
            // 

            try
            {
                // move the mouse to a well known position inside the window to avoid extra mouse movements later 
                // when we generate baseline values for each frame
                Point mousePosition = new Point(0, 0);
                MoveMouseTo(mousePosition);

                bool ok = true;
                SimulateTouches(message, touchMap,

                    // for each frame
                    delegate(int frameNumber, Dictionary<int, TouchData> currentTouchMap)
                    {
                        if (changeVisualTree != null)
                        {
                            changeVisualTree(frameNumber, currentTouchMap);
                        }

                        // read actual value
                        Dictionary<DependencyObject, object> actualValues = MultiTouchVerifier.ReadAllValues(getActualValue, this);

                        // generate baseline
                        Dictionary<DependencyObject, object> expectedValues = GenerateAllBaselineValues(ref mousePosition, currentTouchMap,
                            actualValues.Keys, getExpectedValue, mergeExpectedValues);

                        if (!MultiTouchVerifier.ComparePropertyValues(frameNumber, expectedValues, actualValues, logFilter))
                        {
                            ok = false;
                        }
                    }, captureMode, captureInterval);

                Utils.Assert(ok == true, "Mismatch between Mouse and Touch dependency properties.");
            }
            finally
            {
                // 
            }
        }

        /// <summary>
        /// simulate touches
        /// </summary>
        /// <param name="message"></param>
        /// <param name="touchMap"></param>
        /// <param name="afterFrameSimulated"></param>
        /// <param name="captureMode"></param>
        /// <param name="captureInterval"></param>
        public void SimulateTouches(string message, Dictionary<int, TouchData> touchMap,
            AfterFrameSimulated afterFrameSimulated, CaptureMode captureMode, int captureInterval)
        {
            // simulate touches and collect events
            try
            {
                for (int frameNumber = 0; ; frameNumber++)
                {
                    bool existActiveTouch = false;
                    bool existDelayedTouch = false;
                    foreach (KeyValuePair<int, TouchData> pair in touchMap)
                    {
                        TouchData touchData = pair.Value;

                        if (touchData.currentSnapshot < 0)
                        {
                            existDelayedTouch = true;
                        }
                        else if (touchData.currentSnapshot < touchData.snapshots.Count)
                        {
                            // 

                            //TouchDeviceAction action;
                            //if (touchData.currentSnapshot == 0)
                            //{
                            //    action = TouchDeviceAction.TouchAdd;
                            //}
                            //else if (touchData.currentSnapshot == touchData.snapshots.Count - 1)
                            //{
                            //    action = TouchDeviceAction.TouchRemove;
                            //}
                            //else
                            //{
                            //    action = TouchDeviceAction.TouchChange;
                            //}

                            TouchDevice touch = touchData.snapshots[touchData.currentSnapshot];
                            // simulate touch
                            //

                            existActiveTouch = true;

                            // capture touch
                            //
                        }
                        else if (touchData.currentSnapshot == touchData.snapshots.Count)
                        {
                            // hide the contact adorner and go to the next contact
                            //
                        }
                    }

                    // no touches left
                    if (!existDelayedTouch && !existActiveTouch)
                    {
                        break;
                    }

                    if (existActiveTouch)
                    {
                        // make sure that UI is updated - DoEvents();  
                        LocalDoEvents();
                        Thread.Sleep(15);
                    }

                    // invoke caller-provided delegate
                    if (afterFrameSimulated != null)
                    {
                        afterFrameSimulated(frameNumber, touchMap);
                    }

                    // increase currentSnapshot index
                    foreach (KeyValuePair<int, TouchData> pair in touchMap)
                    {
                        TouchData touchData = pair.Value;
                        touchData.currentSnapshot++;
                    }
                }
            }
            finally
            {
                //
            }
        }

        /// <summary>
        /// append simultaneous touches
        /// </summary>
        /// <param name="touchMap"></param>
        /// <param name="touchCount"></param>
        /// <param name="touchStartId"></param>
        /// <param name="numberOfJoins"></param>
        public void AppendRandomTouchSeriesSimultaneousStart(Dictionary<int, TouchData> touchMap, int touchCount,
            int touchStartId, int numberOfJoins)
        {
            // use Int64 to avoid overflow in arithmetic operations
            int index = 0;
            for (Int64 touchId64 = touchStartId; touchId64 < (Int64)touchStartId + (Int64)touchCount; touchId64++, index++)
            {
                int touchId = (int)(touchId64 & 0xffffffff);

                // create a contact that starts and finishes at random points inside Root element
                TouchData touchData = new TouchData();
                touchData.touchId = touchId;

                //start at -1 to so there is one frame with no contacts
                touchData.currentSnapshot = -1;

                touchData.snapshots = GenerateTouchSeries(TopGrid, touchId,
                    delegate()
                    {
                        return RandomGenerator.GetIntPoint(new Point(0, 0), new Point(TopGrid.ActualWidth, TopGrid.ActualHeight - 1));
                    },
                    numberOfJoins, 7);

                Utils.Assert(touchData.snapshots.Count >= 2);

                touchMap[touchId] = touchData;
            }
        }

        /// <summary>
        /// dump logical tree
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string DumpLogicTree(FrameworkElement obj)
        {
            StringBuilder builder = new StringBuilder(1000);
            DoDumpLogicalTree(obj, builder, 0);
            return builder.ToString();
        }

        /// <summary>
        /// build random visual tree for advanced tree change related tests
        /// </summary>
        public void BuildRandomVisualTree()
        {
            Debug.WriteLine("Building Visual Tree");

            VisualTreeOptions options = new VisualTreeOptions();
            options.RemoveCount = VisualTreeOptions.AllElements;
            options.AddCount = 50;
            options.GetIsEnabled = delegate() { return RandomGenerator.GetBoolean(); };
            options.GetIsHitTestVisible = options.GetIsEnabled;
            options.GetIsVisible = delegate() { return RandomGenerator.GetEnum<Visibility>(); };
            options.RandomPanelTransform = true;

            MultiTouchVerifier.ModifyVisualTree(TopGrid, options);
        }

        /// <summary>
        /// test touch down/move/up events
        /// </summary>
        /// <param name="message"></param>
        /// <param name="touchMap"></param>
        /// <param name="verifyParentChain"></param>
        /// <param name="verifyEnterLeave"></param>
        public void TestTouchDownMoveUp(string message, Dictionary<int, TouchData> touchMap,
            bool verifyParentChain, bool verifyEnterLeave)
        {
            try
            {
                // 

                TouchEventCollector touchEventCollector = new TouchEventCollector(TopGrid, verifyEnterLeave, false/*verifyGestures*/);
                try
                {
                    // add event handlers
                    touchEventCollector.AddHandlers(this, verifyParentChain);

                    // do simulations
                    SimulateTouches(message, touchMap, null, CaptureMode.None, -1);
                }
                finally
                {
                    // remove handlers
                    touchEventCollector.RemoveHandlers(this, verifyParentChain);
                }

                Debug.WriteLine("");
                Debug.WriteLine("ALL ACTUAL EVENTS:");
                touchEventCollector.Dump();

                // build baseline and verify the collected events
                foreach (KeyValuePair<int, TouchData> pair in touchMap)
                {
                    // actual events for the current touch
                    TouchData touchData = pair.Value;
                    ReadOnlyCollection<EventParameters> actualEvents = touchEventCollector.CollectedEventsForTouch(touchData.touchId);

                    // build baseline using the Mouse
                    ReadOnlyCollection<EventParameters> baselineEvents = null;
                    string error;

                    if (!BaselineEventBuilder.Generate(this, TopGrid, this, verifyParentChain, verifyEnterLeave, touchData.snapshots,
                        delegate(TouchDevice snapshot)
                        {
                            // display touch position
                            MoveMouseTo(snapshot.GetTouchPoint(TopGrid).Position);
                        },
                        out baselineEvents, out error))
                    {
                        // failed to generate the baseline, skip this run
                        //
                        continue;
                    }

                    // verify events against the built baseline
                    bool ok = EventCollector.CompareEvents(baselineEvents, actualEvents);

                    Utils.Assert(ok, "Mismatch between Mouse and Contact events.");
                }
            }
            finally
            {
                //todo - title change
            }
        }

        /// <summary>
        /// move the mouse to a given point
        /// </summary>
        /// <param name="position"></param>
        private void MoveMouseTo(Point position)
        {
            Point globalPosistion = TopGrid.PointToScreen(position);
            UserInput.MouseMove((int)globalPosistion.X, (int)globalPosistion.Y);
            UserInput.MouseLeftDown(TopGrid, (int)globalPosistion.X, (int)globalPosistion.Y);
            UserInput.MouseLeftUp(TopGrid);

            LocalDoEvents();
        }

        private void UpdateAllBaselineValues(IEnumerable<DependencyObject> elements,
            Dictionary<DependencyObject, object> values, GetPropertyValue getValue, MergePropertyValues mergeValues)
        {
            // enumerate all child elements and read the value
            foreach (DependencyObject obj in elements)
            {
                object newValue = getValue(obj);
                object oldValue;
                if (values.TryGetValue(obj, out oldValue))
                {
                    // merge values (simulate multi-touch)
                    newValue = mergeValues(oldValue, newValue);
                }
                values[obj] = newValue;
            }
        }

        /// <summary>
        /// generate a baseline events from mouse counterparts
        /// </summary>
        /// <param name="previousPosition"></param>
        /// <param name="touchMap"></param>
        /// <param name="elements"></param>
        /// <param name="getValue"></param>
        /// <param name="mergeValues"></param>
        /// <returns></returns>
        private Dictionary<DependencyObject, object> GenerateAllBaselineValues(ref Point previousPosition,
            Dictionary<int, TouchData> touchMap,
            IEnumerable<DependencyObject> elements,
            GetPropertyValue getValue,
            MergePropertyValues mergeValues)
        {
            Dictionary<DependencyObject, object> values = new Dictionary<DependencyObject, object>();

            // move mouse outside the window
            Point outsideWindow = new Point(10000000, 10000000);
            if (previousPosition != outsideWindow)
            {
                UserInput.MouseMove((int)outsideWindow.X, (int)outsideWindow.Y); 
                previousPosition = outsideWindow;
            }

            // release capture
            Mouse.Capture(null);

            // enumerate all child elements and read the value
            UpdateAllBaselineValues(elements, values, getValue, mergeValues);

            // go through touches
            foreach (KeyValuePair<int, TouchData> pair in touchMap)
            {
                // take a contact that has a series that is not finished yet
                TouchData touchData = pair.Value;
                int snapshotIndex = touchData.currentSnapshot;
                if (snapshotIndex >= 0 && snapshotIndex < touchData.snapshots.Count - 1) // the last snapshot corresponds to TouchRemove, so ignore it
                {
                    // move mouse to the touch position if it's different from the previous position
                    Point position = touchData.snapshots[snapshotIndex].GetTouchPoint(TopGrid).Position;  
                    if (position != previousPosition)
                    {
                        UserInput.MouseMove((int)position.X, (int)position.Y);
                        previousPosition = position;
                        MoveMouseTo(position);
                    }

                    // capture or release the Mouse
                    Mouse.Capture(touchData.capturedBy, touchData.captureMode);

                    // enumerate all child elements and read the value
                    UpdateAllBaselineValues(elements, values, getValue, mergeValues);
                }
            }

            // release capture
            Mouse.Capture(null);

            return values;
        }

        private static List<TouchDevice> GenerateTouchSeries(Panel root, int touchtId, GetPointMethod getPoint,
            int numberOfJoins, int maxIntervalBetweenSnapshots)
        {
            List<TouchDevice> touchSnapshots = new List<TouchDevice>();

            // get two random points
            if (numberOfJoins == 0)
            {
                // add two touch snapshots for Up and Down
                Point point = getPoint(); // call getPoint() once to make sure that the start and end positions are the same
                for (int i = 0; i < 2; i++)
                {
                    // 
                }
            }
            else
            {
                Point start = getPoint();
                for (int i = 0; i < numberOfJoins; i++)
                {
                    Point end = getPoint();
                    int numberOfInternalPointsX = (int)Math.Abs(start.X - end.X) / maxIntervalBetweenSnapshots;
                    int numberOfInternalPointsY = (int)Math.Abs(start.Y - end.Y) / maxIntervalBetweenSnapshots;
                    int numberOfInternalPoints = Math.Max(numberOfInternalPointsX, numberOfInternalPointsY);

                    //
                    // 


                    if (i != numberOfJoins - 1)
                    {
                        // if this is not the last join then remove the last snapshot to avoid
                        // several snapshots with the same position because the next join will start
                        // from that point
                        touchSnapshots.RemoveAt(touchSnapshots.Count - 1);
                    }
                    start = end;
                }

                // make sure that last two points have the same position,
                // consider the last ContactChange and ContactUp events, they should have the same position
                Debug.Assert(touchSnapshots.Count > 0);
                touchSnapshots.Add(touchSnapshots[touchSnapshots.Count - 1]);
            }

            return touchSnapshots;
        }

        /// <summary>
        /// private method to write out the touch over data through a logical tree
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="builder"></param>
        /// <param name="indent"></param>
        private static void DoDumpLogicalTree(FrameworkElement obj, StringBuilder builder, int indent)
        {
            builder.Append(new string(' ', indent));
            builder.Append(obj.Name);
            builder.Append(' ');
            builder.Append(obj.AreAnyTouchesOver ? "AreAnyTouchesOver=true" : "");

            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                FrameworkElement element = child as FrameworkElement;
                if (element != null && element.Name.Length > 0 && element.Visibility == Visibility.Visible)
                {
                    DoDumpLogicalTree(element, builder, indent + 2);
                }
            }
        }

        /// <summary>
        /// the good old helper
        /// </summary>
        private static void LocalDoEvents()
        {
            // To keep this thread busy, we'll have to push a frame.
            System.Windows.Threading.DispatcherFrame frame = new System.Windows.Threading.DispatcherFrame();

            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                new System.Windows.Threading.DispatcherOperationCallback(
                    delegate(object arg)
                    {
                        frame.Continue = false;
                        return null;
                    }), null);

            // Keep the thread busy processing events until the timeout has expired.
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }

        #endregion

        #region Event Handlers - Stylus, Mouse, Touch, Manipulations

        private void DefaultEventHandlerThunk(object sender, RoutedEventArgs args)
        {
            if (DefaultEventHandler != null)
            {
                DefaultEventHandler(sender, args);
            }
        }

        #region Stylus

        void OnStylusDownXaml(object sender, StylusDownEventArgs e)
        {
            // get the location for this touch
            Point p = e.GetPosition(this);
            AddWatchEvent(string.Format("Xaml - TT - StylusDown: p = ({0},{1})", p.X, p.Y));
        }

        void OnStylusUpXaml(object sender, StylusEventArgs e)
        {
            Point p = e.GetPosition(this);
            AddWatchEvent(string.Format("Xaml - TT - StylusUp: p = ({0},{1})", p.X, p.Y));
        }

        void OnStylusDownWin(object sender, StylusDownEventArgs e)
        {
            // get the location for this touch
            Point p = e.GetPosition(this);   
            AddWatchEvent(string.Format("TT - StylusDown: p = ({0},{1})", p.X, p.Y));

#if adhoc
            //// attribute an id with a touch point
            //if (TouchDevice1 == 0)
            //{
            //    TouchDevice1 = e.StylusDevice.Id;

            //    // move the rectangle to the given location
            //    Touch1.SetValue(Canvas.LeftProperty, p.X - Touch1.Width / 2);
            //    Touch1.SetValue(Canvas.TopProperty, p.Y - Touch1.Height / 2);
            //}
            //else if (TouchDevice2 == 0)
            //{
            //    TouchDevice2 = e.StylusDevice.Id;

            //    // move the rectangle to the given location
            //    Touch2.SetValue(Canvas.LeftProperty, p.X - Touch2.Width / 2);
            //    Touch2.SetValue(Canvas.TopProperty, p.Y - Touch2.Height / 2);
            //}
#endif
        }
 
        void OnStylusMoveWin(object sender, StylusEventArgs e)
        {
            Point p = e.GetPosition(this);
            AddWatchEvent(string.Format("TT - StylusMove: p = ({0},{1})", p.X, p.Y));

#if adhoc           
            //// determine which touch this belongs to
            //if (TouchDevice1 == e.StylusDevice.Id)
            //{
            //    // move the rectangle to the given location
            //    Touch1.SetValue(Canvas.LeftProperty, p.X - Touch1.Width / 2);
            //    Touch1.SetValue(Canvas.TopProperty, p.Y - Touch1.Height / 2);
            //}
            //else if (TouchDevice2 == e.StylusDevice.Id)
            //{
            //    // move the rectangle to the given location
            //    Touch2.SetValue(Canvas.LeftProperty, p.X - Touch2.Width / 2);
            //    Touch2.SetValue(Canvas.TopProperty, p.Y - Touch2.Height / 2);
            //}
#endif
        }
 
        void OnStylusUpWin(object sender, StylusEventArgs e)
        {
            Point p = e.GetPosition(this);
            AddWatchEvent(string.Format("TT - StylusUp: p = ({0},{1})", p.X, p.Y));

#if adhoc
            //// reinitialize touch id and hide the rectangle
            //if (e.StylusDevice.Id == TouchDevice1)
            //{
            //    Touch1.SetValue(Canvas.LeftProperty, -Touch1.Width);
            //    TouchDevice1 = 0;
            //}
            //else if (e.StylusDevice.Id == TouchDevice2)
            //{
            //    Touch2.SetValue(Canvas.LeftProperty, -Touch2.Width);
            //    TouchDevice2 = 0;
            //}
#endif
        }

        private void OnStylusSystemGestureXaml(object sender, StylusSystemGestureEventArgs e)
        {
            AddWatchEvent(String.Format("Xaml - Gesture ({0}) - {1}", e.StylusDevice.TabletDevice.Type, e.SystemGesture));
        }

        #endregion

        #region Manipulations

        private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = this;
            e.Mode = ManipulationModes.All;

            AddWatchEvent("TT - ManipStarting");
        }

        #endregion

        #region Touch

        private void OnTouchDownWin(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("TT - TouchDown: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        private void OnTouchMoveWin(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TT - TouchMove");
        }

        private void OnTouchUpWin(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("TT - TouchUp: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        private void TestTouch_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("TT - PreviewTouchUp: id={0}, origSource={1} ", e.TouchDevice.Id, e.OriginalSource));
        }

        private void TestTouch_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TT - PreviewTouchMove");
        }

        private void TestTouch_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("TT - PreviewTouchDown: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        private void TestTouch_LostTouchCapture(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("TT - LostTouchCapture: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        private void TestTouch_GotTouchCapture(object sender, TouchEventArgs e)
        {
            AddWatchEvent(string.Format("TT - GotTouchCapture: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        private void TestTouch_TouchLeave(object sender, TouchEventArgs e)
        {
            leaveEventWin++;
            AddWatchEvent(string.Format("TT - TouchLeave: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        private void TestTouch_TouchEnter(object sender, TouchEventArgs e)
        {
            enterEventWin++;
            AddWatchEvent(string.Format("TT - TouchEnter: id={0}, orig={1}", e.TouchDevice.Id, e.OriginalSource));
        }

        #endregion

        #region Mouse

        private void TestTouch_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AddWatchEvent("TT - MouseUp");
        }

        private void TestTouch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddWatchEvent("TT - MouseDown");
        }

        private void TestTouch_MouseEnter(object sender, MouseEventArgs e)
        {
            AddWatchEvent("TT - MouseEnter");
        }

        private void TestTouch_MouseMove(object sender, MouseEventArgs e)
        {
            AddWatchEvent("TT - MouseMove");
        }

        private void LuckyButton_Click(object sender, RoutedEventArgs e)
        {
            AddWatchEvent("TT - LuckyButton clicked");
        }

        #endregion 

        #region Others

        void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Canvas.SetLeft(LuckyThumb, Canvas.GetLeft(LuckyThumb) + e.HorizontalChange);
            Canvas.SetTop(LuckyThumb, Canvas.GetTop(LuckyThumb) + e.VerticalChange);
        }

        #endregion 

        #endregion

        #region Local Helpers

        private void Attach()
        {
            //
                      
            this.StylusDown += new StylusDownEventHandler(OnStylusDownWin);
            this.StylusMove += new StylusEventHandler(OnStylusMoveWin);
            this.StylusUp += new StylusEventHandler(OnStylusUpWin);

            this.TouchEnter += new EventHandler<TouchEventArgs>(TestTouch_TouchEnter);
            this.TouchLeave += new EventHandler<TouchEventArgs>(TestTouch_TouchLeave);
            this.TouchDown += new EventHandler<TouchEventArgs>(OnTouchDownWin);
            this.TouchMove += new EventHandler<TouchEventArgs>(OnTouchMoveWin);
            this.TouchUp += new EventHandler<TouchEventArgs>(OnTouchUpWin);
            this.GotTouchCapture += new EventHandler<TouchEventArgs>(TestTouch_GotTouchCapture);
            this.LostTouchCapture += new EventHandler<TouchEventArgs>(TestTouch_LostTouchCapture);

            this.PreviewTouchDown += new EventHandler<TouchEventArgs>(TestTouch_PreviewTouchDown);
            this.PreviewTouchMove += new EventHandler<TouchEventArgs>(TestTouch_PreviewTouchMove);
            this.PreviewTouchUp += new EventHandler<TouchEventArgs>(TestTouch_PreviewTouchUp);

            this.MouseEnter += new MouseEventHandler(TestTouch_MouseEnter);
            this.MouseDown += new MouseButtonEventHandler(TestTouch_MouseDown);
            this.MouseUp += new MouseButtonEventHandler(TestTouch_MouseUp);

            ResetCounters();
        }

        private void Detach()
        {
            //

            this.StylusDown -= new StylusDownEventHandler(OnStylusDownWin);
            this.StylusMove -= new StylusEventHandler(OnStylusMoveWin);
            this.StylusUp -= new StylusEventHandler(OnStylusUpWin);

            this.TouchEnter -= new EventHandler<TouchEventArgs>(TestTouch_TouchEnter);
            this.TouchLeave -= new EventHandler<TouchEventArgs>(TestTouch_TouchLeave);
            this.TouchDown -= new EventHandler<TouchEventArgs>(OnTouchDownWin);
            this.TouchMove -= new EventHandler<TouchEventArgs>(OnTouchMoveWin);
            this.TouchUp -= new EventHandler<TouchEventArgs>(OnTouchUpWin);
            this.GotTouchCapture -= new EventHandler<TouchEventArgs>(TestTouch_GotTouchCapture);
            this.LostTouchCapture -= new EventHandler<TouchEventArgs>(TestTouch_LostTouchCapture);

            this.PreviewTouchDown -= new EventHandler<TouchEventArgs>(TestTouch_PreviewTouchDown);
            this.PreviewTouchMove -= new EventHandler<TouchEventArgs>(TestTouch_PreviewTouchMove);
            this.PreviewTouchUp -= new EventHandler<TouchEventArgs>(TestTouch_PreviewTouchUp);

            this.MouseEnter -= new MouseEventHandler(TestTouch_MouseEnter);
            this.MouseDown -= new MouseButtonEventHandler(TestTouch_MouseDown);
            this.MouseUp -= new MouseButtonEventHandler(TestTouch_MouseUp);           
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
            ResetCounters();
            this.Close();
        }

        /// <summary>
        /// write the trace
        /// </summary>
        /// <param name="s"></param>
        private void AddWatchEvent(string s)
        {
            if (WatchList.Items.Count >= MaxWatchEvents)
            {
                WatchList.Items.RemoveAt(0);
            }

            WatchList.Items.Add(s);
        }

        /// <summary>
        /// find the visual parent
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static UIElement GetParent(object element)
        {
            return (UIElement)VisualTreeHelper.GetParent((UIElement)element);
        }

        private void ResetCounters()
        {
            enterEventWin = 0;
            leaveEventWin = 0;           
        }
        
        private void OnNavigateButtonClick(object sender, RoutedEventArgs e)
        {
            // Get URI to navigate to
            Uri uri = new Uri(this.addressTextBox.Text, UriKind.RelativeOrAbsolute);

            // Only absolute URIs can be navigated to
            if (!uri.IsAbsoluteUri)
            {
                MessageBox.Show("The Address URI must be absolute eg 'http://www.microsoft.com'");
                return;
            }

            // Navigate to the desired URL by calling the .Navigate method
            this.MyWebBrowser.Navigate(uri);
        }
 
        #endregion

        #region Fields

        private string _title = "Multi-Touch Raw Touch Testing";
        private const int MaxWatchEvents = 30;
        Dictionary<int, TouchData> _contactMap = new Dictionary<int, TouchData>();
        private RoutedEventHandler _defaultEventHandler = null;
        private delegate Point GetPointMethod();
        private Dictionary<int, TouchDevice> _existingTouches = new Dictionary<int, TouchDevice>();
        internal int enterEventWin = 0, leaveEventWin = 0;

        #endregion

    }
}
