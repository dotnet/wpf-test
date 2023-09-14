using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.ComponentModel.Utilities;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// This class keeps track of tooltip behavior on a given page.
    /// </summary>
    public class ToolTipTracker
    {
        #region Public members

        public ToolTipTracker()
        {
            NullWaiter = new Waiter(null, null, 0);
            ActiveWaiter = NullWaiter;
        }

        public void Initialize(FrameworkElement root)
        {
            RegisterForToolTipEvents(root);
            RegisterForKeyboardFocusEvents(root);
        }

        public List<LogEntry> Log { get; private set; }
        public ToolTip CurrentToolTip { get; private set; }
        public DependencyObject CurrentToolTipOwner { get; private set; }
        public List<DependencyObject> ToolTipOwners { get; private set; }

        public void BeginLogging()
        {
            Log = new List<LogEntry>();
            _startTime = DateTime.Now;
        }

        public void LogEvent(DependencyObject element, string description)
        {
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now - _startTime,
                Owner = element,
                Description = description,
            };
            Log.Add(entry);
        }

        public void WaitForToolTipOpened(DependencyObject owner, int timeout, bool checkCurrent=true)
        {
            // if the owner doesn't have a tooltip, give up now
            if (owner.GetValue(ToolTipService.ToolTipProperty) == null)
                return;

            // wait for owner's ToolTipOpening event, if necessary
            if (!checkCurrent || owner != CurrentToolTipOwner)
            {
                WaitFor(owner, ToolTipService.ToolTipOpeningEvent, timeout);
            }

            // wait for the ToolTip.Opened event, if necessary
            if (CurrentToolTipOwner == owner && CurrentToolTip == null)
            {
                WaitFor(null, ToolTip.OpenedEvent, 0);
            }
        }

        public void WaitForToolTipClosed(DependencyObject owner, int timeout, bool checkCurrent=true)
        {
            // if the owner doesn't have a tooltip, give up now
            if (owner.GetValue(ToolTipService.ToolTipProperty) == null)
                return;

            // wait for owner's ToolTipClosing event, if necessary
            bool waitForClosed;
            if (!checkCurrent || owner == CurrentToolTipOwner)
            {
                waitForClosed = WaitFor(owner, ToolTipService.ToolTipClosingEvent, timeout) &&
                    CurrentToolTipOwner == null && CurrentToolTip != null;
            }
            else
            {
                waitForClosed = (CurrentToolTip != null);
            }

            // wait for the ToolTip.Closed event, if necessary
            if (waitForClosed)
            {
                WaitFor(CurrentToolTip, ToolTip.ClosedEvent, 0);
            }
        }

        // return a list of all log entries relevant to 'element'.
        // if exclude==true, return a list of entries not relevant to 'element'.
        public List<LogEntry> LogFor(DependencyObject element, bool exclude=false)
        {
            DependencyObject openingOwner=null, closingOwner=null;
            List<LogEntry> list = new List<LogEntry>();
            list.Add(LogEntry.Empty);   // always start with a placeholder

            foreach (LogEntry entry in Log)
            {
                DependencyObject owner = entry.Owner;

                // treat ToolTip.Opened/Closed events as relevant to the owner of the
                // preceding ToolTipOpening/Closing events
                switch (entry.Description)
                {
                    case "ToolTipOpening":
                        openingOwner = entry.Owner;
                        break;
                    case "Opened":
                        owner = openingOwner;
                        openingOwner = null;
                        break;

                    case "ToolTipClosing":
                        closingOwner = entry.Owner;
                        break;
                    case "Closed":
                        owner = closingOwner;
                        closingOwner = null;
                        break;
                }

                if ((owner != element) == exclude)
                {
                    list.Add(entry);
                }
            }

            return list;
        }
        #endregion Public members

        #region Nested Types

        public class LogEntry
        {
            public TimeSpan Timestamp { get; set; }
            public DependencyObject Owner { get; set; }
            public string Description { get; set; }

            public static readonly LogEntry Empty = new LogEntry();
        }

        private class Waiter : DispatcherFrame
        {
            public bool Success { get; set; }

            public Waiter(DependencyObject d, RoutedEvent routedEvent, int timeout)
            {
                _source = d;
                _event = routedEvent;
                _timeout = (timeout > 0) ? timeout : 10000; // force timeout after 10 seconds, to avoid hanging
            }

            public void Wait()
            {
                if (_timeout > 0)
                {
                    _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(_timeout),
                                                 DispatcherPriority.Normal,
                                                 new EventHandler(OnTick),
                                                 this.Dispatcher);
                    _timer.Start();
                }

                Dispatcher.PushFrame(this);
            }

            public void Check(DependencyObject source, RoutedEvent routedEvent)
            {
                if (routedEvent == _event && (_source == null || _source == source))
                {
                    Success = true;
                    EndWait();
                }
            }

            private void OnTick(object sender, EventArgs e)
            {
                EndWait();
            }

            private void EndWait()
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer = null;
                }

                this.Continue = false;
            }

            DependencyObject _source;
            RoutedEvent _event;
            int _timeout;
            DispatcherTimer _timer;
        }

        #endregion Nested Types

        #region Private members

        // Add all elements under d that own a tooltip to the ToolTipOwners list
        private void FindToolTipOwners(DependencyObject d, List<DependencyObject> owners)
        {
            // check d itself
            if (d.GetValue(ToolTipService.ToolTipProperty) != null)
            {
                owners.Add(d);
            }

            // recursively check the visual subtree
            UIElement uie = d as UIElement;
            if (uie != null)
            {
                int n = VisualTreeHelper.GetChildrenCount(d);
                for (int i = 0; i < n; ++i)
                {
                    FindToolTipOwners(VisualTreeHelper.GetChild(d, i), owners);
                }
            }

            // recursively check the logical subtree of ContentElement and 
            // certain special-case UIElements
            ContentElement ce = d as ContentElement;
            d = null;

            if (ce != null) d = ce;
            else if (uie != null)
            {
                FlowDocumentScrollViewer fdsv;
                if (uie is TextBlock) d = uie;
                else if ((fdsv = uie as FlowDocumentScrollViewer) != null) d = fdsv.Document;
            }

            if (d != null)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(d))
                {
                    DependencyObject doChild = child as DependencyObject;
                    if (doChild != null)
                    {
                        FindToolTipOwners(doChild, owners);
                    }
                }
            }
        }

        // Register for ToolTipOpening, ToolTipClosing, ToolTip.Opened, and ToolTip.Closed events
        private void RegisterForToolTipEvents(FrameworkElement root)
        {
            // the ToolTip events are raised from a ToolTip, so register a class handler 
            RoutedEventHandler ttHandler = new RoutedEventHandler(OnToolTipEvent);
            EventManager.RegisterClassHandler(typeof(ToolTip), ToolTip.OpenedEvent, ttHandler, handledEventsToo: true);
            EventManager.RegisterClassHandler(typeof(ToolTip), ToolTip.ClosedEvent, ttHandler, handledEventsToo: true);

            // the ToolTipService attached events are direct, so we have to register on each owner
            List<DependencyObject> owners = new List<DependencyObject>();
            FindToolTipOwners(root, owners);
            ToolTipOwners = owners;
            
            ToolTipEventHandler handler = new ToolTipEventHandler(OnToolTipServiceEvent);

            foreach (DependencyObject d in owners)
            {
                UIElement uie = d as UIElement;
                ContentElement ce = d as ContentElement;

                if (uie != null)
                {
                    uie.AddHandler(ToolTipService.ToolTipOpeningEvent, handler, handledEventsToo: true);
                    uie.AddHandler(ToolTipService.ToolTipClosingEvent, handler, handledEventsToo: true);
                }
                else if (ce != null)
                {
                    ce.AddHandler(ToolTipService.ToolTipOpeningEvent, handler, handledEventsToo: true);
                    ce.AddHandler(ToolTipService.ToolTipClosingEvent, handler, handledEventsToo: true);
                }
            }
        }

        // Register for GotKeyboardFocus and LostKeyboardFocus events
        private void RegisterForKeyboardFocusEvents(FrameworkElement root)
        {
            if (root != null)
            {
                KeyboardFocusChangedEventHandler handler = new KeyboardFocusChangedEventHandler(OnKeyboardFocusEvent);
                root.AddHandler(Keyboard.GotKeyboardFocusEvent, handler, handledEventsToo: true);
                root.AddHandler(Keyboard.LostKeyboardFocusEvent, handler, handledEventsToo: true);
            }
        }

        // wait for d to raise the routedEvent, or for the timeout to expire, whichever comes first.
        // Returns true if the event came before the timeout.
        private bool WaitFor(DependencyObject d, RoutedEvent routedEvent, int timeout)
        {
            if (ActiveWaiter != NullWaiter) throw new InvalidOperationException("Nested waiters not allowed");
            Waiter waiter = new Waiter(d, routedEvent, timeout);
            ActiveWaiter = waiter;

            waiter.Wait();

            if (ActiveWaiter != waiter) throw new InvalidOperationException("Waiter disappeared unexpectedly");
            ActiveWaiter = NullWaiter;
            return waiter.Success;
        }

        private void OnToolTipEvent(object sender, RoutedEventArgs e)
        {
            ToolTip tooltip = e.OriginalSource as ToolTip;
            LogEvent(tooltip, e.RoutedEvent.Name);

            CurrentToolTip = (e.RoutedEvent == ToolTip.OpenedEvent) ? tooltip : null;

            ActiveWaiter.Check(tooltip, e.RoutedEvent);
        }

        private void OnToolTipServiceEvent(object sender, ToolTipEventArgs e)
        {
            DependencyObject owner = e.Source as DependencyObject;
            LogEvent(owner, e.RoutedEvent.Name);

            CurrentToolTipOwner = (e.RoutedEvent == ToolTipService.ToolTipOpeningEvent) ? owner : null;

            ActiveWaiter.Check(owner, e.RoutedEvent);
        }

        private void OnKeyboardFocusEvent(object sender, KeyboardFocusChangedEventArgs e)
        {
            LogEvent((DependencyObject)e.OriginalSource, e.RoutedEvent.Name);
        }
        
        static readonly string TTOpening = ToolTipService.ToolTipOpeningEvent.Name;
        static readonly string TTClosing = ToolTipService.ToolTipClosingEvent.Name;
        static readonly string TTOpened = ToolTip.OpenedEvent.Name;
        static readonly string TTClosed = ToolTip.ClosedEvent.Name;

        private Waiter ActiveWaiter { get; set; }
        private Waiter NullWaiter { get; set; }
        private DateTime _startTime;

        #endregion Private members
    }
}
