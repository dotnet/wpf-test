// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// A null-reference occurs when the target of a binding in a data trigger
    /// gets GC'd while the binding is notifying the trigger of a value-change.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Styles", "DataTriggerTargetGC", Versions="4.8+")]
    public class DataTriggerTargetGC : XamlTest
    {
        #region Constructors

        public DataTriggerTargetGC()
            : base(@"DataTriggerTargetGC.xaml")
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(VerifyDataTriggerTargetGC);
            CleanUpSteps += new TestStep(Cleanup);
        }

        #endregion

        #region Private Members

        PulseTimer _pulseTimer;
        PulseConverter _converter;
        Timer _timer;
        ManualResetEvent _timerFinished;

        TestResult Initialize()
        {
            _pulseTimer = (PulseTimer)RootElement.Resources["PulseTimer"];
            _converter = (PulseConverter)RootElement.Resources["PulseConverter"];

            _converter.Owner = this;

            _timer = new Timer(OnTick, null, 0, 500);

            return TestResult.Pass;
        }

        public void AddLogMessage(string format, params object[] args)
        {
            Log.LogStatus(format, args);
        }

        void OnTick(object state)
        {
            if (_timerFinished == null)
            {
                _pulseTimer.Dispatcher.Invoke((Action)TogglePulse);
            }
        }

        void TogglePulse()
        {
            Log.LogStatus("Toggle Pulse");
            _pulseTimer.Pulse = !_pulseTimer.Pulse;
        }


        // when a Binding (or MultiBinding) in a DataTrigger (or
        // MultiDataTrigger) gets a new value, it notifies the trigger.  If the
        // target element gets GC'd after the Binding gets the value but before
        // the trigger receives the notification, the trigger code can hit a
        // NullReference exception.
        // In practice this happens with very low probability - the
        // window of opportunity for the GC is short.   For this test, we
        // force the GC to happen during the bad window.
        //
        // The NullReference only happens in retail builds of WPF.  In debug
        // builds, local variables on the stack keep the target element alive
        // throughout the notification.  In retail builds these are optimized
        // away, making the target element GC-eligible.   (In other words,
        // this test always passes in debug builds.)
        //
        // Also, the test gives occasional false positives even in retail builds.
        // It passes (when it shouldn't) about 5% of the time, from limited trials.
        // I can't get GC to disable the weak reference deterministically.
        TestResult VerifyDataTriggerTargetGC()
        {
            RemoveTriggerTarget();

            // wait for the reference to be released
            while (_converter.HasTarget)
            {
                WaitForPriority(DispatcherPriority.Background);
                Thread.Sleep(500);
            }

            // if we get here, no exception happened
            // Before returning, stop the timer
            _timerFinished = new ManualResetEvent(false);
            _timer.Dispose(_timerFinished);

            return TestResult.Pass;
        }

        // separate method, so locals don't interfere with GC
        void RemoveTriggerTarget()
        {
            Panel panel = RootElement as Panel;
            if (panel.Children.Count > 0)
            {
                // remove the target element
                DependencyObject target = panel.Children[0];
                panel.Children.RemoveAt(0);

                // save a reference to the departed element, to be released later
                _converter.Target = target;
                Log.LogStatus("Remove Target");
            }
        }

        TestResult Cleanup()
        {
            if (_timerFinished != null)
            {
                _timerFinished.WaitOne();
                _timerFinished.Dispose();
            }
            return TestResult.Pass;
        }

        #endregion
    }

    public class PulseTimer : DependencyObject
    {
        public static readonly DependencyProperty PulseProperty =
            DependencyProperty.Register("Pulse", typeof(bool), typeof(PulseTimer));

        public bool Pulse
        {
            get { return (bool)GetValue(PulseProperty); }
            set { SetValue(PulseProperty, value); }
        }
    }

    public class PulseConverter : IMultiValueConverter
    {
        public DependencyObject Target { get; set; }
        public bool HasTarget { get { return (Target != null); } }
        public DataTriggerTargetGC Owner { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // release the reference to the target (if any), and force it to be GC'd.
            // This simulates the race condition where GC occurs after TransferValue
            // but before OnBindingValueIn{Style|Template|ThemeStyle}Changed.
            if (ReleaseTarget())
            {
                GC.GetTotalMemory(true);
                GC.WaitForPendingFinalizers();
            }

            AddLogMessage("Convert");
            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        // separate method to keep locals from interfering with GC
        bool ReleaseTarget()
        {
            if (Target != null)
            {
                AddLogMessage("Release Target");
                Target = null;
                return true;
            }
            return false;
        }

        void AddLogMessage(string format, params object[] args)
        {
            if (Owner != null)
            {
                Owner.AddLogMessage(format, args);
            }
        }
    }
}

