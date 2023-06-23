using System;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// UIAEventsValidator
    /// </summary>
    public class UIAEventsValidator : AutomationValidator
    {
        public UIAEventsValidator(string args, string windowElementName)
            : base(args, windowElementName)
        {
        }

        public override void Run()
        {
            TestAsyncContentLoaded();
            TestNotification();
            TestActiveTextPositionChanged();
        }

        #region AsyncContentLoaded

        void TestAsyncContentLoaded()
        {
            // register for the event
            AutomationElement asyncContentButton = FindElement(windowElement, "_asyncContentButton");
            Automation.AddAutomationEventHandler(
                                AutomationElementIdentifiers.AsyncContentLoadedEvent,
                                asyncContentButton,
                                TreeScope.Element,
                                OnAutomationEvent);

            // get the elements involved in raising the event
            AutomationElement aclButton = FindElement(windowElement, "_asyncContentButton");
            InvokePattern raiseEvent = aclButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            AutomationElement aclCombo = FindElement(windowElement, "_asyncContentCombo");
            EnsureComboBox(aclCombo);

            AutomationElement aclCycle = FindElement(windowElement, "_asyncContentComboCycle");
            InvokePattern cycle = aclCycle.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            // ask those elements to raise the event, once for each combination of args
            for (AutomationElement item = Cycle(aclCombo, cycle); item != null; item = Cycle(aclCombo, cycle))
            {
                // build the expected event args
                string name = item.Current.Name;
                AsyncContentLoadedState state = Enum.Parse<AsyncContentLoadedState>(name);
                double percent = 0;
                switch (state)
                {
                    case AsyncContentLoadedState.Beginning: percent = 0; break;
                    case AsyncContentLoadedState.Progress: percent = 50; break;
                    case AsyncContentLoadedState.Completed: percent = 100; break;
                }

                // raise the event (in target app), and check that it is received
                using (EventVerifier verifier = ExpectArgs(asyncContentButton, new AsyncContentLoadedEventArgs(state, percent)))
                {
                    raiseEvent.Invoke();
                    verifier.WaitForEvent(TimeSpan.FromMilliseconds(1000));
                }
            }
        }

        bool VerifyAsyncContentLoaded(AsyncContentLoadedEventArgs expected, AsyncContentLoadedEventArgs actual)
        {
            if (actual != null)
            {
                Verify(expected.AsyncContentLoadedState == actual.AsyncContentLoadedState, "unexpected AsyncContentLoadedState");
                Verify(expected.PercentComplete == actual.PercentComplete, "unexpected PercentComplete");
                return true;
            }
            return false;
        }

        #endregion AsyncContentLoaded

        #region Notification

        void TestNotification()
        {
            // register for the event
            AutomationElement notificationButton = FindElement(windowElement, "_notificationButton");
            Automation.AddAutomationEventHandler(
                                AutomationElementIdentifiers.NotificationEvent,
                                notificationButton,
                                TreeScope.Element,
                                OnAutomationEvent);

            // get the elements involved in raising the event
            InvokePattern raiseEvent = notificationButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            AutomationElement nkCombo = FindElement(windowElement, "_notificationKindCombo");
            EnsureComboBox(nkCombo);

            AutomationElement notificationKindCycle = FindElement(windowElement, "_notificationKindComboCycle");
            InvokePattern nkCycle = notificationKindCycle.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            AutomationElement npCombo = FindElement(windowElement, "_notificationProcessingCombo");
            EnsureComboBox(npCombo);

            AutomationElement notificationProcessingCycle = FindElement(windowElement, "_notificationProcessingComboCycle");
            InvokePattern npCycle = notificationProcessingCycle.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            AutomationElement displayStringText = FindElement(windowElement, "_notificationString");
            TextPattern dsText = displayStringText.GetCurrentPattern(TextPattern.Pattern) as TextPattern;

            AutomationElement activityIdText = FindElement(windowElement, "_notificationActivity");
            TextPattern aiText = activityIdText.GetCurrentPattern(TextPattern.Pattern) as TextPattern;

            // ask those elements to raise the event, once for each combination of args
            for (AutomationElement nkItem = Cycle(nkCombo, nkCycle); nkItem != null; nkItem = Cycle(nkCombo, nkCycle))
            {
                for (AutomationElement npItem = Cycle(npCombo, npCycle); npItem != null; npItem = Cycle(npCombo, npCycle))
                {
                    // build the expected event args
                    string name = nkItem.Current.Name;
                    AutomationNotificationKind notificationKind = Enum.Parse<AutomationNotificationKind>(name);

                    name = npItem.Current.Name;
                    AutomationNotificationProcessing notificationProcessing = Enum.Parse<AutomationNotificationProcessing>(name);

                    TextPatternRange displayStringRange = dsText.DocumentRange;
                    string displayString = displayStringRange.GetText(-1);

                    TextPatternRange activityIdRange = aiText.DocumentRange;
                    string activityId = activityIdRange.GetText(-1);

                    // raise the event (in target app), and check that it is received
                    using (EventVerifier verifier = ExpectArgs(notificationButton, new NotificationEventArgs(notificationKind, notificationProcessing, displayString, activityId)))
                    {
                        raiseEvent.Invoke();
                        verifier.WaitForEvent(TimeSpan.FromMilliseconds(1000));
                    }
                }
            }
        }

        bool VerifyNotification(NotificationEventArgs expected, NotificationEventArgs actual)
        {
            if (actual != null)
            {
                Verify(expected.NotificationKind == actual.NotificationKind, "unexpected NotificationKind");
                Verify(expected.NotificationProcessing == actual.NotificationProcessing, "unexpected NotificationProcessing");
                Verify(expected.DisplayString == actual.DisplayString, "unexpected DisplayString");
                Verify(expected.ActivityId == actual.ActivityId, "unexpected ActivityId");
                return true;
            }
            return false;
        }

        #endregion Notification
        
        #region ActiveTextPositionChanged

        void TestActiveTextPositionChanged()
        {
            // get the elements involved in raising the event
            AutomationElement atpcButton = FindElement(windowElement, "_atpcButton");
            InvokePattern raiseEvent = atpcButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            AutomationElement atpcTargetCombo = FindElement(windowElement, "_atpcTargetCombo");
            EnsureComboBox(atpcTargetCombo);

            AutomationElement atpcTargetComboCycle = FindElement(windowElement, "_atpcTargetComboCycle");
            InvokePattern cycle = atpcTargetComboCycle.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;

            AutomationElement startEdgeCheckBox = FindElement(windowElement, "_startEdge");
            TogglePattern startEdgePattern = startEdgeCheckBox.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
            EnsureCheckBox(startEdgePattern);

            AutomationElement endEdgeCheckBox = FindElement(windowElement, "_endEdge");
            TogglePattern endEdgePattern = endEdgeCheckBox.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
            EnsureCheckBox(endEdgePattern);

            // ask those elements to raise the event, once for each combination of args
            for (AutomationElement item = Cycle(atpcTargetCombo, cycle); item != null; item = Cycle(atpcTargetCombo, cycle))
            {
                // register for the event
                string name = item.Current.AutomationId;
                AutomationElement target = FindElement(windowElement, name);
                Automation.AddAutomationEventHandler(
                                    AutomationElementIdentifiers.ActiveTextPositionChangedEvent,
                                    target,
                                    TreeScope.Element,
                                    OnAutomationEvent);

                for (bool? startEdge = Cycle(startEdgePattern); startEdge != null; startEdge = Cycle(startEdgePattern))
                {
                    for (bool? endEdge = Cycle(endEdgePattern); endEdge != null; endEdge = Cycle(endEdgePattern))
                    {
                        // build the expected event args

                        // we only check the expected range for null vs. non-null;
                        // use the document range to represent non-null
                        TextPattern textPattern = target.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                        TextPatternRange documentRange = textPattern.DocumentRange;
                        TextPatternRange expectedRange = (startEdge==true && endEdge==true) ? null : documentRange;

                        string search = "Nevertheless";
                        string fullText = documentRange.GetText(-1);
                        int index = fullText.IndexOf(search);
                        int startIndex = (startEdge == true) ? 0 : index;
                        int endIndex = (endEdge == true) ? fullText.Length : index + search.Length;
                        string expectedText = fullText.Substring(startIndex, endIndex-startIndex);

                        // raise the event (in target app), and check that it is received
                        using (EventVerifier verifier = ExpectArgs(target, new ExtendedATPCEventArgs(expectedText, expectedRange)))
                        {
                            raiseEvent.Invoke();
                            verifier.WaitForEvent(TimeSpan.FromMilliseconds(1000));
                        }
                    }
                }
            }
        }

        bool VerifyActiveTextPositionChanged(ExtendedATPCEventArgs expectedEx, ActiveTextPositionChangedEventArgs actual)
        {
            if (actual != null)
            {
                TextPatternRange expectedRange = expectedEx.ExpectedRange;
                if (expectedRange == null)
                {
                    Verify(actual.TextRange == null, "unexpected non-null TextRange");
                }
                else
                {
                    Verify(actual.TextRange != null, "unexpected null TextRange");
                    string actualText = actual.TextRange.GetText(-1);
                    Verify(expectedEx.ExpectedText == actualText, "unexpected TextRange content");
                }
                return true;
            }
            return false;
        }

        public class ExtendedATPCEventArgs : AutomationEventArgs
        {
            public ExtendedATPCEventArgs(string expectedText, TextPatternRange expectedRange)
                : base(AutomationElement.ActiveTextPositionChangedEvent)
            {
                ExpectedText = expectedText;
                ExpectedRange = expectedRange;
            }

            public string ExpectedText { get; private set; }
            public TextPatternRange ExpectedRange { get; private set; }
        }

        #endregion AsyncContentLoaded

        #region Helpers

        AutomationElement FindElement(AutomationElement root, string name)
        {
            System.Windows.Automation.Condition condition = new PropertyCondition(AutomationElement.AutomationIdProperty, name);
            return root.FindFirst(TreeScope.Descendants, condition);
        }
        
        void OnAutomationEvent(object sender, AutomationEventArgs e)
        {
            _verifier?.VerifyArgs(sender, e);
        }

        // expand/collapse the combobox, so that its items are visible to automation
        private void EnsureComboBox(AutomationElement combobox)
        {
            ExpandCollapsePattern expand = combobox.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            expand.Expand();
            expand.Collapse();
        }

        // select the next item in the combobox
        AutomationElement Cycle(AutomationElement combo, InvokePattern cycle)
        {
            // advance the combo's SelectedIndex
            cycle.Invoke();
            DispatcherOperations.WaitFor(TimeSpan.FromMilliseconds(100));

            // return the new selected item
            AutomationElement[] selection = combo.GetCurrentPropertyValue(SelectionPattern.SelectionProperty) as AutomationElement[];
            return (selection.Length > 0) ? selection[0] : null;
        }

        // restore a checkbox to Indeterminate state
        void EnsureCheckBox(TogglePattern togglePattern)
        {
            while (Cycle(togglePattern) != null)
            { /* empty */ }
        }

        // cylce a checkbox
        bool? Cycle(TogglePattern togglePattern)
        {
            togglePattern.Toggle();

            switch (togglePattern.Current.ToggleState)
            {
                case ToggleState.Off: return false;
                case ToggleState.On: return true;
            }
            return null;
        }

        #endregion Helpers

        #region Verification

        EventVerifier _verifier;

        private EventVerifier ExpectArgs(AutomationElement sender, AutomationEventArgs args)
        {
            return new EventVerifier(this, sender, args);
        }

        private class EventVerifier : IDisposable
        {
            public EventVerifier(UIAEventsValidator owner, AutomationElement sender, AutomationEventArgs args)
            {
                _expectedSender = sender;
                _expectedArgs = args;

                _owner = owner;
                owner._verifier = this;
            }

            // wait for the expected event, up to the given maximum time
            public void WaitForEvent(TimeSpan timeout)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = timeout;
                _timer.Tick += OnTimeout;
                _timer.Start();

                _frame = new DispatcherFrame(false);
                Dispatcher.PushFrame(_frame);
            }

            void OnTimeout(object sender, EventArgs e)
            {
                EndWait();
            }

            void EndWait()
            {
                DispatcherFrame frame = _frame;
                _frame = null;

                DispatcherTimer timer = _timer;
                _timer = null;

                if (timer != null)
                { 
                    timer.Tick -= OnTimeout;
                    timer.Stop();
                }

                if (frame != null)
                {
                    frame.Continue = false;
                }
            }

            public void VerifyArgs(object osender, AutomationEventArgs args)
            {
                AsyncContentLoadedEventArgs aclArgs;
                NotificationEventArgs notificationArgs;
                ExtendedATPCEventArgs atpcArgs;
                AutomationElement sender;

                try
                {
                    sender = osender as AutomationElement;
                }
                catch (ElementNotAvailableException)
                {
                    return;
                }

                if (sender != _expectedSender)
                    return;


                bool verified = false;
                if ((aclArgs = _expectedArgs as AsyncContentLoadedEventArgs) != null)
                    verified = _owner.VerifyAsyncContentLoaded(aclArgs, args as AsyncContentLoadedEventArgs);
                else if ((notificationArgs = _expectedArgs as NotificationEventArgs) != null)
                    verified = _owner.VerifyNotification(notificationArgs, args as NotificationEventArgs);
                else if ((atpcArgs = _expectedArgs as ExtendedATPCEventArgs) != null)
                    verified = _owner.VerifyActiveTextPositionChanged(atpcArgs, args as ActiveTextPositionChangedEventArgs);

                if (verified)
                {
                    _verified = true;
                    EndWait();
                }
            }

            public void Dispose()
            {
                _owner._verifier = null;
                Verify(_verified, _expectedArgs.EventId.ProgrammaticName + " not received");
            }

            UIAEventsValidator _owner;
            AutomationElement _expectedSender;
            AutomationEventArgs _expectedArgs;
            DispatcherFrame _frame;
            DispatcherTimer _timer;
            bool _verified;
        }

        private static void Verify(bool condition, string message)
        {
            if (!condition)
                throw new TestValidationException(message);
        }

        #endregion Verification
    }
}
