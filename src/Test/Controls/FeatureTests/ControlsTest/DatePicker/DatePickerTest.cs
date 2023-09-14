using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.RenderingVerification;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>Base Class for DatePicker Tests.</summary>
    public abstract class DatePickerTest : StepsTest
    {
        #region Constructor

        public DatePickerTest()
            : base()
        {
            this.InitializeSteps += Setup;
            this.CleanUpSteps += Cleanup;
        }

        #endregion Constructor

        public virtual TestResult Setup()
        {
            //
            /*var profilerVar = System.Environment.GetEnvironmentVariable("Cor_Enable_Profiling", EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(profilerVar))
            {
                try
                {
                    CLRProfilerControl.DumpHeap();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }*/

            this.window = new Window();
            this.window.Title = string.Format("DatePicker Test : {0}", this.GetType().Name);
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Width = 600;
            this.window.Height = 400;
            this.window.Content = this.TestUI;
            this.window.Show();

            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        public virtual TestResult Cleanup()
        {
            //
            /*var profilerVar = System.Environment.GetEnvironmentVariable("Cor_Enable_Profiling", EnvironmentVariableTarget.Process);
            if (!string.IsNullOrEmpty(profilerVar))
            {
                GlobalLog.LogEvidence("in CleanUp");
                Panel rootPanel = Window.Content as Panel;
                if (rootPanel != null)
                {
                    GlobalLog.LogEvidence("clear all children");
                    rootPanel.Children.Clear();
                }
                else if (Window.Content is Page)
                {
                    var page = (Window.Content as Page);
                    if (page.Content is Panel)
                    {
                        (page.Content as Panel).Children.Clear();
                    }
                }

                GlobalLog.LogEvidence("remove template");
                Window.Content = null;
                Window.Template = null;

                GC.Collect(3);
                GC.WaitForPendingFinalizers();
                GC.Collect(3);
                Thread.Sleep(100);
            }*/

            testui = null;
            if (this.Window != null)
                this.Window.Close();

            //
            /*if (!string.IsNullOrEmpty(profilerVar))
            {
                Dispatcher.CurrentDispatcher.InvokeShutdown();

                TimeSpan timeout = TimeSpan.FromMilliseconds(120);
                TimeSpan zero = TimeSpan.FromMilliseconds(0);
                TimeSpan delta = TimeSpan.FromMilliseconds(10);

                while (!Dispatcher.CurrentDispatcher.HasShutdownFinished && timeout > zero)
                {
                    Thread.Sleep(10);
                    timeout -= delta;
                }

                window = null;

                GlobalLog.LogEvidence("collect in cleanup");
                GC.Collect(3);
                GC.WaitForPendingFinalizers();
                GC.Collect(3);

                Thread.Sleep(3000);

                try
                {
                    CLRProfilerControl.DumpHeap();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }*/

            return TestResult.Pass;
        }

        public Window Window
        {
            get { return window; }
        }
      
        public Grid TestUI
        {
            get
            {
                if (testui == null)
                    testui = new Grid();
                return testui;
            }
        }

        public AutomationElement DatePickerAE
        {
            get
            {
                if (datepickerAE == null)
                {
                    AutomationHelper.DoUIAAction(DispatcherPriority.Background, new DispatcherOperationCallback(FindDatePickerAE), new object());
                }
                return datepickerAE;
            }
        }

        private object FindDatePickerAE(object arg)
        {
            AutomationElement windowAE = AutomationHelper.GetRootAutomationElement();
            PropertyCondition condition = new PropertyCondition(AutomationElement.ClassNameProperty, "DatePicker");
            datepickerAE = windowAE.FindFirst(TreeScope.Descendants, condition);
            return null;
        }

        public void ResetTest()
        {
            this.TestUI.Children.Clear();
            isLoaded = false;
        }

        public FrameworkElement LoadXaml(string xaml)
        {
            return ResourceHelper.LoadXamlResource(string.Format("datepicker/xaml/{0}", xaml));
        }

        public DatePicker FindDatePicker(string name)
        {
            DependencyObject datepicker = System.Windows.LogicalTreeHelper.FindLogicalNode(this.window, name);
            if (datepicker is DatePicker)
                return datepicker as DatePicker;
            else
                return null;
        }

        public void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
        }
        
        public bool isLoaded = false;

        private Grid testui = null;
        private Window window = null;
        private AutomationElement datepickerAE = null;
    }
}
