// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  The DialogResultConverter BVT tests the basic usage of 
//  the class's public methods [CanConvertTo, CanConvertFrom, ConvertTo,
//  ConvertFrom].  No error or boundary cases are addressed in this BVT
//  (i.e invalid parameters, etc.)
//


using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.BVT
{
    public partial class DialogResultConverterApp
    {
        internal enum CurrentTest
        {
            Start,
            Constructor,
            CanConvertFrom,
            CanConvertTo,
            ConvertFrom,
            ConvertTo,
            End
        }

        CurrentTest _currentTest = CurrentTest.Start;
        private TestLog _frmwk = null;
        private DialogResultConverter _drc = null;
        private Window _mainWindow = null;

        /// <summary>
        /// Sets up the AutomationFramework, creates the object tree, 
        /// registers eventhandlers and sets the starting point of the BVT
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            _frmwk = TestLog.Current;
            if (null == _frmwk) _frmwk = new TestLog("test.log");
            //frmwk.Stage = AutomationFramework.STAGE_INIT;
            _frmwk.LogEvidence("Inside OnStartup");

            // Create element tree (in this test, we only need a Window)
            _mainWindow = new Window();
            _mainWindow.Left = 0;
            _mainWindow.Top = 0;
            _mainWindow.Width = 500;
            _mainWindow.Height = 500;
            _mainWindow.Title = "DialogResultConverter BVT - Main Window";

            // Register event-handlers
            _mainWindow.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            // Set up the BVT
            _currentTest = CurrentTest.Constructor;
            //frmwk.Stage = AutomationFramework.STAGE_RUN;
            _frmwk.LogEvidence("Starting DialogResultConverter BVTs.");

            // Show window
            _mainWindow.Show();
            base.OnStartup(e);
        }


        /// <summary>
        /// Routes the sub-parts of this BVT, based on the value of currentTest.
        /// Return value checking (for the non-constructor public API) is done here too.
        /// </summary>
        private void MainWindow_Loaded(object source, RoutedEventArgs e)
        {
            bool isConvertible;

            // Dummy CustomDescriptorContext and CultureInfo to use in testing
            // ConvertFrom and ConvertTo
            CustomDescriptorContext customDesc = new CustomDescriptorContext();
            CultureInfo customCultureInfo = new CultureInfo("en-PH", false);

            // While we haven't reached the end, keep iterating through one part
            // after another...
            while (!_currentTest.Equals(CurrentTest.End))
            {
                switch (_currentTest)
                {
                    // [1] DialogResultConverter constructor
                    // Check that a non-null instance of DialogResultConverter is returned
                    // This instance will be used in subsequent tests for the DialogResultConverter
                    // public API.
                    case CurrentTest.Constructor:
                        _drc = new DialogResultConverter();
                        CheckConstructorResults();
                        _currentTest = CurrentTest.CanConvertFrom;
                        break;

                    // [2] DialogResultConverter.CanConvertFrom (not supported)
                    // This method should always return false, no matter what gets passed in
                    case CurrentTest.CanConvertFrom:
                        isConvertible = _drc.CanConvertFrom(customDesc, typeof(String));
                        if (!isConvertible)
                        {
                            _currentTest = CurrentTest.CanConvertTo;
                            _frmwk.LogEvidence("CanConvertFrom correctly returned false");
                        }
                        else
                        {
                            FailTest("CanConvertFrom always returns false.  Result:  true returned");
                        }
                        break;

                    // [3] DialogResultConverter.CanConvertTo (not supported)
                    // This method should always return false, no matter what gets passed in
                    case CurrentTest.CanConvertTo:
                        isConvertible = _drc.CanConvertTo(customDesc, typeof(FrameworkContentElement));
                        if (!isConvertible)
                        {
                            _currentTest = CurrentTest.ConvertFrom;
                            _frmwk.LogEvidence("CanConvertTo correctly returned false");
                        }
                        else
                        {
                            FailTest("CanConvertTo always returns false.  Result:  true returned");
                        }
                        break;

                    // [4] DialogResultConverter.ConvertFrom (not supported)
                    // This method should always throw an InvalidOperationException, 
                    // no matter what gets passed in 
                    case CurrentTest.ConvertFrom:
                        try
                        {
                            _drc.ConvertFrom(customDesc, customCultureInfo, _mainWindow);
                        }
                        catch (InvalidOperationException)
                        {
                            _frmwk.LogEvidence("InvalidOperationException correctly thrown by call to ConvertFrom");
                            _currentTest = CurrentTest.ConvertTo;
                        }
                        catch (Exception exp)
                        {
                            FailTest("ConvertFrom should always throw InvalidOperationException, not " + exp.ToString());
                        }
                        break;

                    // [5] DialogResultConverter.ConvertTo (not supported)
                    // This method should always throw an InvalidOperationException, 
                    // no matter what gets passed in 
                    case CurrentTest.ConvertTo:
                        try
                        {
                            _drc.ConvertTo(customDesc, customCultureInfo, _mainWindow, typeof(Window));
                        }
                        catch (InvalidOperationException)
                        {
                            _frmwk.LogEvidence("InvalidOperationException correctly thrown by call to ConvertTo");
                            _currentTest = CurrentTest.End;
                        }
                        catch (Exception exp)
                        {
                            FailTest("ConvertTo should always throw InvalidOperationException, not " + exp.ToString());
                        }
                        break;

                    // BVT sub-test was not one of the defined choices.  End BVT.
                    default:
                        FailTest("Not a valid BVT test.  Ending BVT.");
                        break;
                }
            }

            // All our tests have passed and reached the end.
            // Spew out success message and end the BVT
            if (_currentTest.Equals(CurrentTest.End))
                PassTest("Called all public API for DialogResultConverter.  Passed.");
        }


        /// <summary>
        /// Checks that the constructor returned a non-null instance of DialogResultConverter
        /// and this was assigned to drc (global variable)
        /// </summary>
        private void CheckConstructorResults()
        {
            // if the return value is null or not a DialogResultConverter
            if (_drc == null || !(_drc is DialogResultConverter))
            {
                FailTest("DialogResultConverter() did not produce a new instance of DialogResultConverter");
            }
            else
            {
                // drc != null AND drc is DialogResultConverter
                _frmwk.LogEvidence("DialogResultConverter() produced a new non-null instance of DialogResultConverter");
            }
        }


        /// <summary>
        /// Prints out the message describing that the tests rans to 
        /// completion, then shuts down the app.
        /// </summary>
        /// <param name="successMessage">Generic message indicating BVT ran successfully</param>
        private void PassTest(String successMessage)
        {
            _frmwk.LogEvidence(successMessage);
            _frmwk.Result = TestResult.Pass;
            _frmwk.Close();
            this.Shutdown();
        }


        /// <summary>
        /// Prints out the message describing what caused the BVT to fail,
        /// then shuts down the app.
        /// </summary>
        /// <param name="failMessage">Message to print in AutomationFramework/Piper</param>
        private void FailTest(String failMessage)
        {
            _frmwk.LogEvidence(failMessage);
            _frmwk.Result = TestResult.Fail;
            _frmwk.Close();
            this.Shutdown();
        }

    }       // end:  DialogResultConverterApp class


    /// <summary>
    /// Dummy ITypeDescriptorContext to use in testing DialogResultConverter's public API
    /// </summary>
    public class CustomDescriptorContext : ITypeDescriptorContext
    {
        public void OnComponentChanged() { }
        public bool OnComponentChanging() { return false; }
        public IContainer Container { get { return null; } }
        public object Instance { get { return new Object(); } }
        public PropertyDescriptor PropertyDescriptor { get { return null; } }
        public object GetService(Type serviceType) { return new Object(); }
    }       // end:  CustomDescriptorContext class

}           // end:  Microsoft.Windows.Test.Client.AppSec.BVT namespace
