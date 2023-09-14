using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    ///ICommandRegressionTest17 Test (WPF: ICommand CanExecuteChanged behaviour change in .NET 4.5)
    /// </summary>
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(0, "Commanding", "ICommandRegressionTest17", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ICommandRegressionTest17 : XamlTest
    {
        #region Private Fields
        private Panel panel;
        private CanExecuteChangedEventTestViewModel model;
        private Button button;
        private TextBox textbox;
        #endregion Private Fields

        #region Constructor
        public ICommandRegressionTest17()
            : base(@"ICommandRegressionTest17.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunCanExecuteChangedEventTest);
        }
        #endregion

        #region TestStep
        public TestResult RunCanExecuteChangedEventTest()
        {
            Status("Run CanExecuteChangedEventTest");
            WaitForPriority(DispatcherPriority.Render);

            LogComment("Input characters 'WPF'");
            Microsoft.Test.Input.Keyboard.Type("WPF");
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            if (button.IsEnabled)
            {
                throw new TestValidationException("the OK button should not become enabled when less than 8 characters are entered into the textbox.");
            }
            else
            {
                LogComment("the OK button does not become enabled when less than 8 characters are entered into the textbox.");
            }

            LogComment("Input characters ' Test' after 'WPF'");
            Microsoft.Test.Input.Keyboard.Type(" Test");
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            if (!button.IsEnabled)
            {
                throw new TestValidationException(
                    "Expected: The OK button should become enabled, " +
                    "Actual: The OK button doesn't become enabled due to known bug (WPF: ICommand CanExecuteChanged behaviour change in .NET 4.5)");
            }
            else
            {
                LogComment("The OK button become enabled when 8 or more characters are entered into the textbox.");
            }
            return TestResult.Pass;
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            model = new CanExecuteChangedEventTestViewModel();
            this.Window.DataContext = model;
            panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new TestValidationException("Panel is null");
            }
            button = (Button)panel.FindName("btnTest");
            if (button == null)
            {
                throw new TestValidationException("button is null");
            }
            if (button.IsEnabled)
            {
                throw new TestValidationException("button should not be enable before test start.");
            }

            textbox = (TextBox)RootElement.FindName("textBox1");
            if (textbox == null)
            {
                throw new TestValidationException("text box is null");
            }

            LogComment("Setup was successful");
            textbox.Focus();
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            typeof(EventHelper).InvokeMember("sender", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            typeof(EventHelper).InvokeMember("actualEventArgs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField, null, null, new object[] { null });
            panel = null;
            button = null;
            textbox = null;
            model = null;
            return TestResult.Pass;
        }
        #endregion TestStep
    }

    #region Helper Classes

    public class CanExecuteChangedEventTestViewModel : INotifyPropertyChanged
    {
        public CanExecuteChangedEventTestViewModel()
        {
            MyButton = new EnabledButton(text.Length > 7);
        }

        private string text = "";

        public string MyText
        {
            get { return text; }
            set
            {
                PropertyChanged.ChangeAndNotify(ref text, value, () => MyText);
                MyButton.Enabled.SetCondition(text.Length > 7);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public EnabledButton MyButton { get; set; }
    }

    public class EnabledButton : ICommand
    {
        public static bool WorkingSolution { get; set; }

        public class ConcreteCondition
        {
            public ConcreteCondition(bool condition)
            {
                Condition = condition;
            }

            public void SetCondition(bool value)
            {
                Condition = value;
            }

            public event EventHandler ConditionChanged;

            private bool condition = false;
            public bool Condition
            {
                get
                {
                    return condition;
                }
                protected set
                {
                    if (condition != value)
                    {
                        condition = value;
                        var handle = ConditionChanged;
                        if (handle != null)
                        {
                            handle(this, new EventArgs());
                        }
                    }
                }
            }
        }

        public ConcreteCondition Enabled { get; set; }

        public EnabledButton(bool enabled)
        {
            Enabled = new ConcreteCondition(enabled);
            Enabled.ConditionChanged += OnEnableConditionChanged;
        }

        private void OnEnableConditionChanged(object sender, EventArgs e)
        {
            CanExecuteChanged(WorkingSolution ? this : sender, e);
        }

        #region  ICommand Implementation

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            return Enabled.Condition;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class PropertyChangedExtensions
    {
        public static bool ChangeAndNotify<T>(this PropertyChangedEventHandler handler,
             ref T field, T value, Expression<Func<T>> memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException("memberExpression");
            }
            var body = memberExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Lambda must return a property.");
            }
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            var vmExpression = body.Expression as ConstantExpression;
            if (vmExpression != null)
            {
                LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(vmExpression);
                Delegate vmFunc = lambda.Compile();
                object sender = vmFunc.DynamicInvoke();

                if (handler != null)
                {
                    // we must run the handler on a separate thread to support the textbox
                    var async = handler.BeginInvoke(sender, new PropertyChangedEventArgs(body.Member.Name), null, null);
                    while (!async.IsCompleted) async.AsyncWaitHandle.WaitOne();
                    handler.EndInvoke(async);
                    async.AsyncWaitHandle.Close();
                }
            }

            field = value;
            return true;
        }
    }

    #endregion  Helper  Classes
}
