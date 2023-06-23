using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace HangTest
{
    public class Controller : DispatcherObject
    {
        Grid _mainGrid;
        ItemsControl _errorList;
        FrameworkElement _resourceHolder;
        TreeView _treeView;
        ScrollViewer _scrollViewer;
        VirtualizingStackPanel _vsp;

        Model _model = new Model();
        DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);
        DispatcherTimer _progressTimer = new DispatcherTimer(DispatcherPriority.Normal);
        const int ProgressLimit = 3;    // fail if progress timer ticks this many times with no progress
        const double LineHeight = 16.0;     // this is hard-wired in ScrollViewer

        int _stepCount;
        int _progressCount;
        TestDirection _testDirection;
        RealizedItem _firstInViewport;
        StreamWriter _log;

        public Controller(Grid mainGrid, ItemsControl errorList, FrameworkElement resourceHolder)
        {
            _mainGrid = mainGrid;
            _errorList = errorList;
            _resourceHolder = resourceHolder;

            _mainGrid.DataContext = _model;
            _model.IsRunningChanged += OnIsRunningChanged;
            _timer.Tick += OnTimeExpired;
            _progressTimer.Tick += OnProgress;
            _progressTimer.Interval = TimeSpan.FromSeconds(2);

            _log = new StreamWriter("HangTest.log", append:false);
        }

        public List<string> Errors
        {
            get { return (RunParameters != null) ? RunParameters.Errors : null; }
        }

        void OnIsRunningChanged(object sender, EventArgs e)
        {
            if (_model.IsRunning)
            {
                // starting a test.  first discard tree view from previous test (if any)
                _mainGrid.Children.Remove(_treeView);
                _treeView = null;
                HideErrors();

                // set up the run parameters
                _model.SetRunParameters();
                Log("\n{0} Running test with parameters \"{1}\".", DateTime.Now, _model.Parameters);

                // prepare the data and UI
                if (!RunParameters.HasErrors)
                {
                    RunParameters.Prepare();
                    _model.PrepareData();
                    PrepareTreeView();
                }

                // if all is OK, start the test
                if (!RunParameters.HasErrors)
                {
                    Dispatcher.BeginInvoke(new Action(StartTest), DispatcherPriority.ApplicationIdle);
                }
                else
                {
                    // if not, stop it before even starting
                    _model.IsRunning = false;
                }
            }
            else
            {
                // stopping a test - show what happened
                _progressTimer.Stop();
                if (RunParameters.HasErrors)
                {
                    ShowErrors();
                    foreach (string s in RunParameters.Errors)
                    {
                        Log("{0}", s);
                    }
                    Log("{0} Test failed.", DateTime.Now);
                }
                else
                {
                    Log("{0} Test succeeded.", DateTime.Now);
                }
            }

            FlushLog();
        }

        void ShowErrors()
        {
            if (RunParameters.HasErrors)
            {
                _errorList.Visibility = Visibility.Visible;
                _errorList.ItemsSource = RunParameters.Errors;

                if (_treeView != null)
                {
                    _treeView.Visibility = Visibility.Hidden;
                }
            }
        }

        void HideErrors()
        {
            _errorList.ItemsSource = null;
            _errorList.Visibility = Visibility.Hidden;
        }

        void AddError(string format, params object[] args)
        {
            RunParameters.AddRawError(String.Format(CultureInfo.InvariantCulture, format, args));
        }

        void Log(string format, params object[] args)
        {
            if (_log != null)
            {
                _log.WriteLine(String.Format(CultureInfo.InvariantCulture, format, args));
            }
        }

        void FlushLog()
        {
            if (_log != null)
                _log.Flush();
        }

        void PrepareTreeView()
        {
            _treeView = new TreeView();
            VirtualizingPanel.SetIsVirtualizing(_treeView, true);
            VirtualizingPanel.SetScrollUnit(_treeView, RunParameters.ScrollUnit);
            VirtualizingPanel.SetVirtualizationMode(_treeView, RunParameters.VirtualizationMode);
            VirtualizingPanel.SetCacheLength(_treeView, RunParameters.CacheLength);
            VirtualizingPanel.SetCacheLengthUnit(_treeView, RunParameters.CacheUnit);
            _treeView.UseLayoutRounding = RunParameters.UseLayoutRounding;
            _treeView.FlowDirection = RunParameters.FlowDirection;
            _treeView.SetBinding(TreeView.ItemsSourceProperty, new Binding("Data"));
            _treeView.ItemContainerStyle = RunParameters.BindItemMargin
                ? (Style)PrivateResources["MarginItemContainerStyle"]
                : (Style)PrivateResources["DefaultItemContainerStyle"];
            string hdtName = RunParameters.BindItemHeight ? "HeightHDT" : "DefaultHDT";
            _treeView.Resources.Add(new DataTemplateKey(typeof(Data)), PrivateResources[hdtName]);

            if (RunParameters.TreeViewHeight > 0.0)
                _treeView.Height = RunParameters.TreeViewHeight;

            _mainGrid.Children.Add(_treeView);
        }

        void OnTimeExpired(object sender, EventArgs e)
        {
            StopTest();
        }

        void OnProgress(object sender, EventArgs e)
        {
            RealizedItem bestItem = GetBetterItem(_firstInViewport, _model.BestItem);
            string bestPath = (bestItem != null) ? bestItem.Path : null;
            if (_stepCount == _model.LastStepCount || (bestPath != null && bestPath == _model.LastPosition))
            {
                // no progress since last tick
                if (++_progressCount >= ProgressLimit)
                {
                    AddError("Test appears to be {0} after {1} steps", 
                        (_stepCount == _model.LastStepCount) ? "hung" : "stuck",
                        _stepCount);
                    StopTest();
                }
            }
            else
            {
                _progressCount = 0;
            }

            _model.BestItem = (bestItem != null) ? bestItem : _firstInViewport;
            _model.LastStepCount = _stepCount;
        }

        // return the item that's closer to the end-goal of the current test
        RealizedItem GetBetterItem(RealizedItem item1, RealizedItem item2)
        {
            if (_testDirection == TestDirection.None) return null;
            if (!RunParameters.VerifyProgress) return null;
            if (item1 == null) return item2;
            if (item2 == null) return item1;

            string[] a1 = item1.Path.Split('.');
            string[] a2 = item2.Path.Split('.');

            // find length of longest common prefix
            int prefixLength = 0;
            while (prefixLength < a1.Length &&
                    prefixLength < a2.Length &&
                    a1[prefixLength] == a2[prefixLength])
            {
                ++prefixLength;
            }

            bool isItem1Earlier;
            if (prefixLength == a1.Length)
                isItem1Earlier = !item1.IsHeaderBelowChildren;
            else if (prefixLength == a2.Length)
                isItem1Earlier = item2.IsHeaderBelowChildren;
            else
            {
                int index1 = int.Parse(a1[prefixLength]);
                int index2 = int.Parse(a2[prefixLength]);
                isItem1Earlier = (index1 < index2);
            }

            switch (_testDirection)
            {
                case TestDirection.Forward:
                    return isItem1Earlier ? item2 : item1;
                case TestDirection.Backward:
                    return isItem1Earlier ? item1 : item2;
                default:
                    return null;
            }
        }

        void StartTest()
        {
            _stepCount = 0;
            _model.LastStepCount = 0;
            _model.BestItem = null;

            _scrollViewer = FindVisualChild<ScrollViewer>(_treeView);
            _vsp = FindVisualChild<VirtualizingStackPanel>(_treeView);
            _vsp.Margin = RunParameters.ViewportMargin;

            if (RunParameters.TimeLimit > TimeSpan.Zero)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(InitializeTest));
            }

            if (RunParameters.TimeLimit != TimeSpan.MaxValue)
            {
                _timer.Interval = RunParameters.TimeLimit;
                _timer.Start();
            }
        }

        void InitializeTest()
        {
            ScrollRequest request = new ScrollRequest(ScrollRequestAction.None);
            switch (RunParameters.ScrollAction)
            {
                case ScrollAction.PageDown:
                case ScrollAction.LineDown:
                case ScrollAction.MouseWheelDown:
                    _testDirection = TestDirection.Forward;
                    request = new ScrollRequest(ScrollRequestAction.Top);
                    break;

                case ScrollAction.PageUp:
                case ScrollAction.LineUp:
                case ScrollAction.MouseWheelUp:
                    _testDirection = TestDirection.Backward;
                    request = new ScrollRequest(ScrollRequestAction.Bottom);
                    break;

                default:
                    _testDirection = TestDirection.None;
                    break;
            }

            MakeRequest(request);

            Dispatcher.BeginInvoke(new Action<ScrollRequest>(DoTestStep), RunParameters.ScrollPriority, request);
            _progressTimer.Start();
        }

        void DoTestStep(ScrollRequest request)
        {
            ++_stepCount;

            VerifyAction(request);

            if (IsTestDone())
            {
                StopTest();
                return;
            }

            switch (RunParameters.ScrollAction)
            {
                default:
                    request = new ScrollRequest(RunParameters.ScrollAction);
                    break;
                case ScrollAction.Jump:
                    // in practice, jump-scrolls are typically followed by a bunch of local
                    // scrolls.  Simulate that by choosing a random action, distinguishing
                    // between long and short scrolls.  Each long scroll will be followed by
                    // 15 short scrolls, on average.
                    ScrollRequestAction randomAction;
                    if (RunParameters.ActionRNG.Next(0, 15) == 0)
                    {
                        // among long scrolls, choose Jump more often than Top/Bottom - there's
                        // more variance in the resulting behavior, so more chance for bugs
                        switch (RunParameters.ActionRNG.Next(0, 10))
                        {
                            case 0: randomAction = ScrollRequestAction.Top; break;
                            case 1: randomAction = ScrollRequestAction.Bottom; break;
                            default: randomAction = ScrollRequestAction.Jump; break;
                        }
                    }
                    else
                    {
                        // among short scrolls, choose equally likely
                        randomAction = (ScrollRequestAction)RunParameters.ActionRNG.Next((int)ScrollRequestAction.PageUp, (int)ScrollRequestAction.MouseWheelDown + 1);
                    }

                    switch (randomAction)
                    {
                        default:
                            request = new ScrollRequest(randomAction);
                            break;
                        case ScrollRequestAction.Jump:
                            request = new ScrollRequest(randomAction, RunParameters.ActionRNG.NextDouble() * _vsp.ExtentHeight);
                            break;
                    }
                    break;
            }

            MakeRequest(request);

            Dispatcher.BeginInvoke(new Action<ScrollRequest>(DoTestStep), RunParameters.ScrollPriority, request);
        }

        void VerifyAction(ScrollRequest request)
        {
            Snapshot previous = request.Snapshot, current = TakeSnapshot();

            if (previous.LastInViewport == null || current.LastInViewport == null)
            {
                AddError("After {0} steps, viewport is empty", _stepCount);
                StopTest();
            }
            else
            {
                _firstInViewport = current.FirstInViewport;

                if ((RunParameters.VerifyLayout && !IsValidLayout(current.Root, current.ViewportHeight)) ||
                    (RunParameters.VerifyScrolling && !IsAcceptableScroll(request, previous, current)))
                {
                    AddError("After {0} steps, last scroll was from {1} to {2}",
                        _stepCount, previous.FirstInViewport.Path, current.FirstInViewport.Path);
                    StopTest();
                }
            }
        }

        bool IsValidLayout(RealizedItem parent, double viewportHeight)
        {
            List<RealizedItem> children = parent.Children;
            int itemCount = parent.ItemCount;
            bool result = true;
            RealizedItem prevInViewport = null;

            foreach (RealizedItem item in children)
            {
                if (IsInViewport(item))
                {
                    if (prevInViewport != null)
                    {
                        // visible items must be consecutive
                        if (item.Index != prevInViewport.Index + 1)
                        {
                            AddError("Non-consecutive visible items {0} and {1}",
                                prevInViewport.Path, item.Path);
                            result = false;
                        }

                        // no overlaps
                        if (LayoutDoubleUtil.LessThan(item.Top, prevInViewport.Bottom))
                        {
                            AddError("Overlap between {0} {1} and {2} {3}",
                                prevInViewport.Path, prevInViewport.Rect,
                                item.Path, item.Rect);
                            result = false;
                        }

                        // no gaps
                        if (!LayoutDoubleUtil.AreClose(item.Top, prevInViewport.Bottom))
                        {
                            AddError("Gap between {0} {1} and {2} {3}",
                                prevInViewport.Path, prevInViewport.Rect,
                                item.Path, item.Rect);
                            result = false;
                        }

                        // recursively check subtree
                        if (!IsValidLayout(item, viewportHeight))
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        // no initial gap
                        if (item.Index > 0 && LayoutDoubleUtil.LessThan(0.0, item.Top))
                        {
                            AddError("Gap before {0} {1}",
                                item.Path, item.Rect);
                            result = false;
                        }
                    }

                    prevInViewport = item;
                }
            }

            // no final gap
            if (prevInViewport != null && prevInViewport.Index < itemCount - 1)
            {
                if (!LayoutDoubleUtil.LessThan(viewportHeight, prevInViewport.Bottom))
                {
                    AddError("Gap after {0} {1} - item count = {2}",
                        prevInViewport.Path, prevInViewport.Rect, itemCount);
                    result = false;
                }
            }

            return result;
        }

        bool IsAcceptableScroll(ScrollRequest request, Snapshot previous, Snapshot current)
        {
            bool result = true;
            Tuple<RealizedItem, RealizedItem> tuple;
            double delta;
            int itemDelta;

            switch (request.Action)
            {
                case ScrollRequestAction.Top:
                    if (!IsAtTop(current.Root, addError: true))
                    {
                        result = false;
                    }
                    break;

                case ScrollRequestAction.Bottom:
                    if (!IsAtBottom(current.Root, addError: true))
                    {
                        result = false;
                    }
                    break;

                case ScrollRequestAction.Jump:
                    tuple = FindCommonItem(previous.Root, current.Root);
                    delta = GetPixelDelta(previous, current, tuple);
                    if (LayoutDoubleUtil.LessThan(Math.Abs(delta), _vsp.ActualHeight))
                    {
                        // the scroll turned out to be short after all.
                        // Accept it - the display will look OK to users.
                    }
                    else
                    {
                        // 
                    }
                    break;

                default:
                    tuple = FindCommonItem(previous.Root, current.Root);
                    delta = GetPixelDelta(previous, current, tuple);
                    itemDelta = GetItemDelta(previous, current);
                    if (Double.IsNaN(delta))
                    {
                        // we can't determine the delta, so just accept the scroll
                        // 
                    }
                    else
                    {
                        ScrollRequestAction action = AdjustAction(request.Action);
                        switch (action)
                        {
                            case ScrollRequestAction.PageUp:
                                result = IsAcceptable(action, previous, current,
                                                        delta, +previous.ViewportPixelHeight,
                                                        itemDelta, +1);
                                break;
                            case ScrollRequestAction.PageDown:
                                result = IsAcceptable(action, previous, current,
                                                        delta, -previous.ViewportPixelHeight,
                                                        itemDelta, -1);
                                break;
                            case ScrollRequestAction.LineUp:
                                result = IsAcceptable(action, previous, current,
                                                        delta, +LineHeight,
                                                        itemDelta, +1);
                                break;
                            case ScrollRequestAction.LineDown:
                                result = IsAcceptable(action, previous, current,
                                                        delta, -LineHeight,
                                                        itemDelta, -1);
                                break;
                            case ScrollRequestAction.MouseWheelUp:
                                result = IsAcceptable(action, previous, current,
                                                        delta, +SystemParameters.WheelScrollLines * LineHeight,
                                                        itemDelta, +SystemParameters.WheelScrollLines);
                                break;
                            case ScrollRequestAction.MouseWheelDown:
                                result = IsAcceptable(action, previous, current,
                                                        delta, -SystemParameters.WheelScrollLines * LineHeight,
                                                        itemDelta, -SystemParameters.WheelScrollLines);
                                break;
                        }
                    }
                    break;
            }

            return result;
        }

        ScrollRequestAction AdjustAction(ScrollRequestAction action)
        {
            if (SystemParameters.WheelScrollLines < 0)
            {
                switch (action)
                {
                    case ScrollRequestAction.MouseWheelUp:
                        action = ScrollRequestAction.PageUp;
                        break;
                    case ScrollRequestAction.MouseWheelDown:
                        action = ScrollRequestAction.PageDown;
                        break;
                }
            }
            return action;
        }

        // check that a short scroll is acceptable
        bool IsAcceptable(ScrollRequestAction action, Snapshot previous, Snapshot current,
                            double actual, double expected,
                            int itemActual, int itemExpected)
        {
            bool result = true;
            bool expectedUp = (expected > 0.0);

            if (RunParameters.ScrollUnit == ScrollUnit.Pixel)
            {
                if (expectedUp ? (actual < 0.0) : (actual > 0.0))
                {
                    AddError("{0} scrolled in the wrong direction.  Expected {1}, observed {2}",
                        action, expected, actual);
                    return false;   // don't bother checking any more
                }

                if (LayoutDoubleUtil.LessThan(previous.ViewportPixelHeight, Math.Abs(actual)))
                {
                    AddError("{0} scrolled more than a page.  Viewport height {1}, observed {2}",
                        action, previous.ViewportPixelHeight, actual);
                    result = false;
                }

                if (!LayoutDoubleUtil.AreClose(actual, expected))
                {
                    if (expectedUp)
                    {
                        if (LayoutDoubleUtil.LessThan(expected, actual) || !IsAtTop(current.Root))
                        {
                            AddError("{0} scrolled the wrong distance.  Expected {1}, observed {2}",
                                action, expected, actual);
                            result = false;
                        }
                    }
                    else
                    {
                        if (LayoutDoubleUtil.LessThan(actual, expected) || !IsAtBottom(current.Root))
                        {
                            AddError("{0} scrolled the wrong distance.  Expected {1}, observed {2}",
                                action, expected, actual);
                            result = false;
                        }
                    }
                }
            }
            else    // if item scrolling...
            {
                if (expectedUp ? (itemActual < 0) : (itemActual > 0))
                {
                    AddError("{0} scrolled in the wrong direction.  Expected {1}, observed {2}",
                        action, itemExpected, itemActual);
                    AddError("   scroll from {0} to {1}", previous.FirstInViewport.Path, current.FirstInViewport.Path);
                    return false;   // don't bother checking any more
                }

                if (LayoutDoubleUtil.LessThan(previous.ViewportPixelHeight, Math.Abs(actual)))
                {
                    AddError("{0} scrolled more than a page.  Viewport height {1}, observed {2}",
                        action, previous.ViewportPixelHeight, actual);
                    result = false;
                }

                if (itemActual != itemExpected)
                {
                    switch (action)
                    {
                        case ScrollRequestAction.PageUp:
                        case ScrollRequestAction.PageDown:
                            // 



                            break;

                        default:
                            if (expectedUp)
                            {
                                if (itemExpected < itemActual || !IsAtTop(current.Root))
                                {
                                    AddError("{0} scrolled the wrong distance.  Expected {1}, observed {2}",
                                        action, itemExpected, itemActual);
                                    AddError("   scroll from {0} to {1}", previous.FirstInViewport.Path, current.FirstInViewport.Path);
                                    result = false;
                                }
                            }
                            else
                            {
                                if (itemActual < itemExpected || !IsAtBottom(current.Root))
                                {
                                    AddError("{0} scrolled the wrong distance.  Expected {1}, observed {2}",
                                        action, itemExpected, itemActual);
                                    AddError("   scroll from {0} to {1}", previous.FirstInViewport.Path, current.FirstInViewport.Path);
                                    result = false;
                                }
                            }
                            break;
                    }
                }

            }

            return result;
        }

        // find the perceived distance (in pixels) that the viewport moved between snapshots
        double GetPixelDelta(Snapshot previous, Snapshot current, Tuple<RealizedItem, RealizedItem> tuple)
        {
            // Return the difference in offset.  This is correct
            // provided that no other changes (add/delete, height
            // changes) have confused the issue.
            return (tuple != null) ? (tuple.Item2.HeaderRect.Top - tuple.Item1.HeaderRect.Top) : Double.NaN;
        }

        // find the perceived distance (in items) that the viewport moved between snapshots
        int GetItemDelta(Snapshot previous, Snapshot current)
        {
            if (RunParameters.ScrollUnit == ScrollUnit.Item)
            {
                RealizedItem[] prevPath = previous.GetAncestors(previous.FirstInViewport);
                RealizedItem[] currPath = current.GetAncestors(current.FirstInViewport);
                return -GetItemDelta(prevPath, currPath);
            }
            else
            {
                return int.MinValue;    // the result isn't used - don't bother computing it
            }
        }

        int GetItemDelta(RealizedItem[] path1, RealizedItem[] path2)
        {
            DataCollection children;
            RealizedItem item1 = path1[path1.Length - 1];
            RealizedItem item2 = path2[path2.Length - 1];

            // find length of longest common prefix
            int prefixLength = 0;
            while (prefixLength < path1.Length &&
                    prefixLength < path2.Length &&
                    path1[prefixLength].Data == path2[prefixLength].Data)
            {
                ++prefixLength;
            }

            // swap if necessary, so that item1 occurs before item2
            bool swap = (prefixLength == path1.Length) ? item1.IsHeaderBelowChildren
                      : (prefixLength == path2.Length) ? !item2.IsHeaderBelowChildren
                      : (path1[prefixLength].Index > path2[prefixLength].Index);
            if (swap)
            {
                RealizedItem tempItem = item1; item1 = item2; item2 = tempItem;
                RealizedItem[] tempPath = path1; path1 = path2; path2 = tempPath;
            }

            // traverse the data tree between item1 and item2, accumulating item count
            int count = 1;      // always count item2

            // account for partial subtrees on tail of path1
            for (int level = prefixLength + 1; level < path1.Length; ++level)
            {
                children = path1[level - 1].Data.Children;
                for (int i = path1[level].Index + 1, n = children.Count; i < n; ++i)
                {
                    count += GetDescendantCount(children[i]);
                }
                if (path1[level].IsHeaderBelowChildren)
                {
                    ++count;
                }
            }

            // if item1 is not an ancestor of item2 and comes before its subtree,
            // account for the subtree of item1 except for item1 itself
            if (prefixLength < path1.Length && !item1.IsHeaderBelowChildren)
            {
                count += GetDescendantCount(item1.Data) - 1;
            }

            // account for full subtrees between path1 and path2
            children = path1[prefixLength - 1].Data.Children;
            int start = (prefixLength < path1.Length) ? path1[prefixLength].Index + 1
                        : !item1.IsHeaderBelowChildren ? 0 : int.MaxValue;
            int stop = (prefixLength < path2.Length) ? path2[prefixLength].Index
                        : item2.IsHeaderBelowChildren ? children.Count : int.MinValue;
            for (int i = start; i < stop; ++i)
            {
                count += GetDescendantCount(children[i]);
            }

            // account for partial subtrees on tail of path2
            for (int level = prefixLength + 1; level < path2.Length; ++level)
            {
                children = path2[level - 1].Data.Children;
                for (int i = 0, n = path2[level].Index; i < n; ++i)
                {
                    count += GetDescendantCount(children[i]);
                }
                if (!path2[level].IsHeaderBelowChildren)
                {
                    ++count;
                }
            }

            // if item2 is not an ancestor of item1 and comes after its subtree,
            // account for the subtree of item2, except for item2 itself
            if (prefixLength < path2.Length && item2.IsHeaderBelowChildren)
            {
                count += GetDescendantCount(item2.Data) - 1;
            }

            return swap ? -count : count;
        }

        int[] GetPath(RealizedItem item)
        {
            string[] a = item.Path.Split('.');
            int[] path = new int[a.Length];
            for (int i = 0; i < a.Length; ++i)
            {
                path[i] = Int32.Parse(a[i]);
            }
            return path;
        }

        DataCollection[] GetChildListsOnPath(int[] path)
        {
            DataCollection[] result = new DataCollection[path.Length];
            DataCollection children = _model.Data;

            for (int i = 0; i < path.Length; ++i)
            {
                result[i] = children;
                children = children[path[i]].Children;
            }

            return result;
        }

        public int GetDescendantCount(Data data)
        {
            int count = 1;
            foreach (Data child in data.Children)
            {
                count += GetDescendantCount(child);
            }
            return count;
        }

        bool TryGetItemOffset(List<RealizedItem> children, RealizedItem target, ref int result)
        {
            // depth-first search, counting visible headers until we see the target
            foreach (RealizedItem item in children)
            {
                if (IsInViewport(item))
                {
                    // if header is visible - start counting, or bump the count
                    if (IsInViewport(item.HeaderRect))
                    {
                        result = (result < 0) ? 0 : result + 1;
                    }

                    // stop when we've reached the target
                    if (item == target)
                    {
                        return true;
                    }

                    // recursively search subtree
                    if (TryGetItemOffset(item.Children, target, ref result))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // find an item whose header is in the viewport in both snapshots.
        Tuple<RealizedItem, RealizedItem> FindCommonItem(RealizedItem parent1, RealizedItem parent2)
        {
            Tuple<RealizedItem, RealizedItem> result = null;
            var e1 = parent1.Children.GetEnumerator(); bool more1 = e1.MoveNext();
            var e2 = parent2.Children.GetEnumerator(); bool more2 = e2.MoveNext();

            while (more1 && more2 && result == null)
            {
                RealizedItem item1 = e1.Current;
                RealizedItem item2 = e2.Current;

                if (item1.Data == item2.Data)
                {
                    result = FindCommonItem(item1, item2);

                    // if no deeper candidate found, use this one
                    if (result == null && IsInViewport(item1.HeaderRect) && IsInViewport(item2.HeaderRect))
                    {
                        result = new Tuple<RealizedItem, RealizedItem>(item1, item2);
                    }

                    // if no candidate found, advance
                    if (result == null)
                    {
                        more1 = e1.MoveNext();
                        more2 = e2.MoveNext();
                    }
                }
                else if (item1.Index < item2.Index)
                {
                    more1 = e1.MoveNext();
                }
                else
                {
                    more2 = e1.MoveNext();
                }
            }

            return result;
        }

        bool IsAtTop(RealizedItem parent, bool addError = false)
        {
            bool result = true;
            foreach (RealizedItem item in parent.Children)
            {
                if (IsInViewport(item))
                {
                    // first visible item must have index 0
                    if (item.Index != 0)
                    {
                        if (addError)
                        {
                            AddError("Not at top - first visible item is {0} {1}",
                                item.Path, item.Rect);
                        }
                        result = false;
                    }
                    else
                    {
                        // this node is OK, check subtree recursively
                        result = IsAtTop(item, addError);
                    }

                    break;
                }
            }

            return result;
        }

        bool IsAtBottom(RealizedItem parent, bool addError = false)
        {
            bool result = true;
            List<RealizedItem> children = parent.Children;

            for (int i = children.Count - 1; i >= 0; --i)
            {
                RealizedItem item = children[i];
                if (IsInViewport(item))
                {
                    // last visible item must have index itemCount-1
                    if (item.Index != parent.ItemCount - 1)
                    {
                        if (addError)
                        {
                            AddError("Not at bottom - last visible item is {0} {1}",
                                item.Path, item.Rect);
                        }
                        result = false;
                    }
                    else
                    {
                        // this node is OK, check subtree recursively
                        result = IsAtBottom(item, addError);
                    }

                    break;
                }
            }

            return result;
        }

        bool IsInViewport(RealizedItem item)
        {
            return IsInViewport(item.FullRect);
        }

        bool IsInViewport(Rect rect)
        {
            return LayoutDoubleUtil.LessThan(rect.Top, _vsp.ActualHeight) &&
                   LayoutDoubleUtil.LessThan(0.0, rect.Bottom);
        }

        bool IsTestDone()
        {
            if (!_model.IsRunning)
                return true;

            switch (_testDirection)
            {
                case TestDirection.Forward:
                    return (_scrollViewer.VerticalOffset >= _scrollViewer.ExtentHeight - _scrollViewer.ViewportHeight);

                case TestDirection.Backward:
                    return (_scrollViewer.VerticalOffset <= 0.0);
            }

            return false;
        }

        void StopTest()
        {
            _timer.Stop();
            _model.IsRunning = false;
            _model.RestoreDefaultTimeLimit();

            // in stress mode, start another test
            if (_model.IsStress && _pendingRestart == null)
            {
                // do this at low priority, to allow the tasks from the current
                // test to complete
                _pendingRestart = Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, 
                    new Action(StartAnotherTest));
            }
        }

        DispatcherOperation _pendingRestart;

        void StartAnotherTest()
        {
            _pendingRestart = null;
            _model.IsRunning = true;
        }

        void MakeRequest(ScrollRequest request)
        {
            request.Snapshot = TakeSnapshot();

            switch (request.Action)
            {
                case ScrollRequestAction.Top: _scrollViewer.ScrollToTop(); break;
                case ScrollRequestAction.Bottom: _scrollViewer.ScrollToBottom(); break;
                case ScrollRequestAction.PageUp: _scrollViewer.PageUp(); break;
                case ScrollRequestAction.PageDown: _scrollViewer.PageDown(); break;
                case ScrollRequestAction.LineUp: _scrollViewer.LineUp(); break;
                case ScrollRequestAction.LineDown: _scrollViewer.LineDown(); break;
                case ScrollRequestAction.MouseWheelUp: _vsp.MouseWheelUp(); break;
                case ScrollRequestAction.MouseWheelDown: _vsp.MouseWheelDown(); break;
                case ScrollRequestAction.Jump: _scrollViewer.ScrollToVerticalOffset(request.Arg); break;
            }
        }

        Snapshot TakeSnapshot()
        {
            Snapshot snapshot = new Snapshot(_scrollViewer, _vsp, _model.DataRoot);
            SnapChildren(snapshot.Root, _treeView);
            FindFirstAndLastInViewport(snapshot, snapshot.Root);
            return snapshot;
        }

        void SnapChildren(RealizedItem parent, ItemsControl itemsControl)
        {
            List<RealizedItem> children = parent.Children;
            string parentPath = parent.Path;

            parent.ItemCount = itemsControl.Items.Count;

            Panel panel = FindVisualChild<VirtualizingStackPanel>(itemsControl);
            if (panel != null && panel.Children.Count > 0)
            {
                ItemContainerGenerator generator = itemsControl.ItemContainerGenerator;
                foreach (TreeViewItem tvi in panel.Children)
                {
                    RealizedItem item = new RealizedItem();
                    children.Add(item);

                    item.Data = (Data)generator.ItemFromContainer(tvi);

                    int index = generator.IndexFromContainer(tvi);
                    item.Index = index;

                    string path = String.IsNullOrEmpty(parentPath) ? String.Format("{0}", index) : String.Format("{0}.{1}", parentPath, index);
                    item.Path = path;

                    Thickness margin = tvi.Margin;
                    item.Margin = margin;

                    Rect rect = ViewportRect(tvi);
                    item.Rect = rect;

                    Rect fullRect = new Rect(rect.Left - margin.Left,
                                    rect.Top - margin.Top,
                                    rect.Width + margin.Left + margin.Right,
                                    rect.Height + margin.Top + margin.Bottom);
                    item.FullRect = fullRect;

                    Panel itemsHostPanel = FindVisualChild<VirtualizingStackPanel>(tvi);
                    Rect itemsHostRect = ViewportRect(itemsHostPanel);
                    item.ItemsHostRect = itemsHostRect;

                    // extend the header rect from the items host to the margin, so that
                    // anything outside the items counts as "header"
                    Rect headerRect;
                    if (itemsHostRect.IsEmpty)
                    {
                        headerRect = fullRect;
                    }
                    else
                    {
                        UIElement header = (UIElement)tvi.Template.FindName("PART_Header", tvi);
                        headerRect = ViewportRect(header);
                        if (!headerRect.IsEmpty)
                        {
                            // extend in the direction the header sticks out from the items host
                            if (LayoutDoubleUtil.LessThan(headerRect.Top, itemsHostRect.Top))
                            {
                                headerRect = new Rect(fullRect.Left, fullRect.Top, fullRect.Width, itemsHostRect.Top - fullRect.Top);
                            }
                            else if (LayoutDoubleUtil.LessThan(itemsHostRect.Bottom, headerRect.Bottom))
                            {
                                headerRect = new Rect(fullRect.Left, itemsHostRect.Bottom, fullRect.Width, fullRect.Bottom - itemsHostRect.Bottom);
                                item.IsHeaderBelowChildren = true;
                            }
                            else
                            {
                                // side-by-side layout - extend to full height (this might be wrong...)
                                headerRect = fullRect;
                            }
                        }
                        else
                        {
                            // no header found, use the longer gap from the items host (a heuristic)
                            double topGap = itemsHostRect.Top - rect.Top;
                            double bottomGap = rect.Bottom - itemsHostRect.Bottom;
                            if (LayoutDoubleUtil.LessThan(topGap, bottomGap))
                            {
                                headerRect = new Rect(fullRect.Left, itemsHostRect.Bottom, fullRect.Width, fullRect.Bottom - itemsHostRect.Bottom);
                                item.IsHeaderBelowChildren = true;
                            }
                            else
                            {
                                headerRect = new Rect(fullRect.Left, fullRect.Top, fullRect.Width, itemsHostRect.Top - fullRect.Top);
                            }
                        }
                    }
                    item.HeaderRect = headerRect;

                    SnapChildren(item, tvi);
                }

                children.Sort();
            }
        }

        void FindFirstAndLastInViewport(Snapshot snapshot, RealizedItem parent)
        {
            foreach (RealizedItem item in parent.Children)
            {
                if (IsInViewport(item))
                {
                    if (item.IsHeaderBelowChildren)
                    {
                        FindFirstAndLastInViewport(snapshot, item);
                    }

                    if (IsInViewport(item.HeaderRect))
                    {
                        if (snapshot.FirstInViewport == null)
                        {
                            snapshot.FirstInViewport = item;
                        }
                        snapshot.LastInViewport = item;
                    }

                    if (!item.IsHeaderBelowChildren)
                    {
                        FindFirstAndLastInViewport(snapshot, item);
                    }
                }
            }
        }

        // return the element's rect, in viewport coordinates
        Rect ViewportRect(UIElement element)
        {
            if (element == null)
                return Rect.Empty;

            Rect rect = new Rect(new Point(), element.RenderSize);
            return CorrectCatastrophicCancellation(element.TransformToAncestor(_vsp)).TransformBounds(rect);
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            T result = null;
            for (int i = 0, n = VisualTreeHelper.GetChildrenCount(obj); result == null && i < n; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if ((result = child as T) == null)
                {
                    result = FindVisualChild<T>(child);
                }
            }
            return result;
        }

        private static GeneralTransform CorrectCatastrophicCancellation(GeneralTransform transform)
        {
            MatrixTransform matrixTransform = transform as MatrixTransform;
            if (matrixTransform != null)
            {
                bool needNewTransform = false;
                Matrix matrix = matrixTransform.Matrix;

                if (matrix.OffsetX != 0.0 && LayoutDoubleUtil.AreClose(matrix.OffsetX, 0.0))
                {
                    matrix.OffsetX = 0.0;
                    needNewTransform = true;
                }

                if (matrix.OffsetY != 0.0 && LayoutDoubleUtil.AreClose(matrix.OffsetY, 0.0))
                {
                    matrix.OffsetY = 0.0;
                    needNewTransform = true;
                }

                if (needNewTransform)
                {
                    transform = new MatrixTransform(matrix);
                }
            }

            return transform;
        }

        RunParameters RunParameters { get { return _model.RunParameters; } }
        ResourceDictionary PrivateResources { get { return _resourceHolder.Resources; } }
    }

    public enum ScrollRequestAction
    {
        None,
        Top, Bottom, Jump,                                                  // long scrolls
        PageUp, PageDown, LineUp, LineDown, MouseWheelUp, MouseWheelDown    // short scrolls
    }

    public enum TestDirection { None, Forward, Backward }

    public class ScrollRequest
    {
        public ScrollRequestAction Action { get; set; }
        public Double Arg { get; set; }
        public Snapshot Snapshot { get; set; }

        public ScrollRequest(ScrollRequestAction action, double arg = 0.0)
        {
            Action = action;
            Arg = arg;
        }

        public ScrollRequest(ScrollAction action) : this(ConvertAction(action))
        { }

        static ScrollRequestAction ConvertAction(ScrollAction action)
        {
            switch (action)
            {
                case ScrollAction.PageUp: return ScrollRequestAction.PageUp;
                case ScrollAction.PageDown: return ScrollRequestAction.PageDown;
                case ScrollAction.LineUp: return ScrollRequestAction.LineUp;
                case ScrollAction.LineDown: return ScrollRequestAction.LineDown;
                case ScrollAction.MouseWheelUp: return ScrollRequestAction.MouseWheelUp;
                case ScrollAction.MouseWheelDown: return ScrollRequestAction.MouseWheelDown;
                default:
                    throw new ArgumentException(String.Format("'{0}' unexpected", action), nameof(action));
            }
        }
    }

    public class Snapshot
    {
        List<RealizedItem> _children = new List<RealizedItem>();
        public double Offset { get; set; }
        public double Extent { get; set; }
        public double ViewportHeight { get; set; }
        public double ViewportPixelHeight { get; set; }
        public RealizedItem Root { get; private set; }
        public RealizedItem FirstInViewport { get; set; }
        public RealizedItem LastInViewport { get; set; }
        public int ItemCount { get; set; }

        public Snapshot(ScrollViewer scrollViewer, FrameworkElement viewportElement, Data root)
        {
            Offset = scrollViewer.VerticalOffset;
            Extent = scrollViewer.ExtentHeight;
            ViewportHeight = scrollViewer.ViewportHeight;
            ViewportPixelHeight = viewportElement.ActualHeight;
            Root = new RealizedItem { Data = root, Path = String.Empty };
        }

        public RealizedItem[] GetAncestors(RealizedItem item)
        {
            string path = item.Path;
            string pathEx = path + ".";
            string[] a = path.Split('.');
            RealizedItem[] result = new RealizedItem[a.Length + 1];

            result[0] = Root;

            for (int i = 0, n = a.Length; i < n; ++i)
            {
                foreach (RealizedItem child in result[i].Children)
                {
                    if (pathEx.StartsWith(child.Path + "."))
                    {
                        result[i + 1] = child;
                        break;
                    }
                }
            }

            return result;
        }
    }

    public class RealizedItem : IComparable<RealizedItem>
    {
        List<RealizedItem> _children = new List<RealizedItem>();
        public Data Data { get; set; }
        public int Index { get; set; }
        public string Path { get; set; }
        public Rect Rect { get; set; }
        public Thickness Margin { get; set; }
        public Rect FullRect { get; set; }
        public Rect ItemsHostRect { get; set; }
        public Rect HeaderRect { get; set; }
        public List<RealizedItem> Children { get { return _children; } }
        public int ItemCount { get; set; }
        public bool IsHeaderBelowChildren { get; set; }

        public double Top { get { return FullRect.Top; } }
        public double Bottom { get { return FullRect.Bottom; } }

        public int CompareTo(RealizedItem x)
        {
            return (this.Index - x.Index);
        }
    }

    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = (int)value;
            return Enum.GetValues(targetType).GetValue(i);
        }
    }

    public class HeightToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value < 16.0) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double margin = (double)value;
            return new Thickness(0, margin, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal static class LayoutDoubleUtil
    {
        private const double eps = 0.00000153; //more or less random more or less small number

        internal static bool AreClose(double value1, double value2)
        {
            if (value1 == value2) return true;

            double diff = value1 - value2;
            return (diff < eps) && (diff > -eps);
        }

        internal static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }
    }
}
