// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using Microsoft.Test.Logging;
    using Microsoft.Test.TestTypes;
    using Microsoft.Test.Threading;

    public abstract class INotifyDataErrorInfoTest : StepsTest
    {
        #region Constructors

        /// <summary>
        /// API test constructor, no window created
        /// </summary>
        public INotifyDataErrorInfoTest() 
        {
            this.RunSteps += Action;
        }

        /// <summary>
        /// Constructor for tests that require a Window
        /// </summary>
        public INotifyDataErrorInfoTest(string file, bool async, bool complex)
        {
            File = file;
            Async = async;
            Complex = complex;

            this.InitializeSteps += Initialize;
            this.RunSteps += Action;
            this.CleanUpSteps += Finalize;
        }

        #endregion Constructors

        #region Public Properties 

        /// <summary>
        /// Window used by INotifyDataErrorInfoTest
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// Name of xaml file to be loaded in Window
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// If true notify updates asynchronously
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// If true use complex error object ErrorObject.  
        /// </summary>
        public bool Complex { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Initialize step, load xaml file and show Window
        /// </summary>
        public virtual TestResult Initialize()
        {
            Assert.IsTrue(System.IO.File.Exists(File), string.Format("File '{0}' does not exist", File));

            object ui = XamlReader.Load(System.IO.File.OpenRead(File));

            Assert.IsNotNull(ui, string.Format("Could not load xaml file '{0}'.", File));

            Window = new Window { Height = 350, Width = 350, Top = 0, Left = 0, Content = ui };
            Window.Show();
            DispatcherHelper.DoEvents();

            return TestResult.Pass;
        }

        /// <summary>
        /// Cleanup step, closes window
        /// </summary>
        public virtual TestResult Finalize()
        {
            if (Window != null)
            {
                Window.Close();
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// Test action, override in subclass
        /// </summary>
        public virtual TestResult Action() { Log.LogStatus("Methods");  return TestResult.Pass; }

        /// <summary>
        /// This will create an instance of INotifyDataErrorObject
        /// </summary>
        /// <param name="s">Initial value for StringProperty</param>
        /// <param name="i">Initial value for IntProperty</param>
        /// <returns>Instance of INotifyDataErrorObject</returns>
        public object CreateDataObject(string s, int i)
        {
            if (Complex)
            {
                return new DataObjectErrors(Async) { IntProperty = i, StringProperty = s };
            }
            else
            {
                return new DataStringErrors(Async) { IntProperty = i, StringProperty = s };
            }
        }

        /// <summary>
        /// Initialize binding object
        /// </summary>
        /// <param name="source">Value for Binding.Source</param>
        /// <param name="prop">Value for Binding.Path</param>
        /// <param name="validatesOnNotifyDataErrors">Value for Binding.ValidatesOnNotifyDataErrors</param>
        /// <returns>new Binding()</returns>
        public Binding CreateBinding(object source, string prop, bool validatesOnNotifyDataErrors)
        {
            return new Binding(prop)
            {
                Source = source,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnExceptions = true,
                ValidatesOnNotifyDataErrors = validatesOnNotifyDataErrors
            };
        }

        /// <summary>
        /// Initialize binding object with out specifying a source object
        /// </summary>
        /// <param name="prop">Value for Binding.Path</param>
        /// <param name="validatesOnNotifyDataErrors">Value for Binding.ValidatesOnNotifyDataErrors</param>
        /// <returns>new Binding()</returns>
        public Binding CreateBinding(string prop, bool validatesOnNotifyDataErrors)
        {
            return new Binding(prop)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                ValidatesOnExceptions = true,
                ValidatesOnNotifyDataErrors = validatesOnNotifyDataErrors
            };
        }

        /// <summary>
        /// Find DependencyObject
        /// </summary>
        /// <typeparam name="T">Type of expected object</typeparam>
        /// <param name="root">Root element to search in</param>
        /// <param name="name">Name of expected object</param>
        /// <returns>Object of type T if found</returns>
        public T FindDependecyObject<T>(UIElement root, string name) where T : DependencyObject
        {
            return (T)LogicalTreeHelper.FindLogicalNode(root, name);
        }

        /// <summary>
        /// Wait for dispatcher.
        /// </summary>
        public void Wait() 
        {
            if (Async)
                // wait at least 500 milliseconds if we are async.. 
                DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);
        }

        /// <summary>
        /// Verify error count for expected object
        /// </summary>
        /// <param name="depObj">DependencyObject to get errors for</param>
        /// <param name="expected">Expected error count</param>
        /// <returns>True if Validation.GetErrors(depObj) == expected</returns>
        public bool ValidateErrors(DependencyObject depObj, int expected)
        {
            return ValidateErrors(depObj, expected, null);
        }

        /// <summary>
        /// Verify error count for expected object
        /// </summary>
        /// <param name="depObj">DependencyObject to get errors for</param>
        /// <param name="expected">Expected error count</param>
        /// <param name="listObj">List that is bound to Validation.Errors</param>
        /// <returns>True if Validation.GetErrors(depObj) == expected</returns>/// <summary>
        public bool ValidateErrors(DependencyObject depObj, int expected, DependencyObject listObj)
        {
            if (Async)
                // wait at least 500 milliseconds if we are async.. 
                DispatcherHelper.DoEvents(500, DispatcherPriority.SystemIdle);

            var errors = Validation.GetErrors(depObj);

            if (errors.Count.Equals(expected))
            {
                if (listObj != null && listObj is ItemsControl)
                {
                    if (!((ItemsControl)listObj).Items.Count.Equals(errors.Count))
                    {
                        Log.LogStatus(string.Format("* List bound to Validation.Error count is wrong for {0}", GetObjectName(listObj)));
                    }
                }

                return true;
            }

            Log.LogStatus(string.Format("* Error count is wrong for {0}", GetObjectName(depObj)));
            Log.LogStatus(string.Format("* Actual   : {0}", errors.Count));
            Log.LogStatus(string.Format("* Expected : {0}", expected));

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="binding"></param>
        public void SetBinding(DependencyObject obj, DependencyProperty prop, Binding binding)
        {
            BindingOperations.SetBinding(obj, prop, binding);
            Wait();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetBindings(DependencyObject obj)
        {
            BindingOperations.ClearAllBindings(obj);
            Wait();
        }

        #endregion Public Methods

        #region Private Methods

        private string GetObjectName(DependencyObject depObj)
        {
            string type = depObj.GetType().Name;
            string name = depObj.GetValue(FrameworkElement.NameProperty).ToString();
            return string.Format("{0}{1}", type, string.IsNullOrEmpty(name) ? string.Empty : string.Format("[{0}]", name));
        }

        #endregion Private Methods
    }


}
