// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  The StockPageFunction BVT tests the basic usage of the 
//  public API of the following classes: StringReturnEventArgs, StringPageFunction,
//  ObjectReturnEventArgs, ObjectPageFunction, Int32ReturnEventArgs, Int32PageFunction,
//  BooleanReturnEventArgs, BooleanPageFunction.
//  No error or boundary cases are addressed in this BVT (i.e invalid parameters, misuse of
//  of methods, etc)
//






using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // StockPageFunction
    public partial class NavigationTests : Application
    {
        // Each element in the enum represents a property/method/event 
        // the public API for the classes in StockPageFunction.cs
        internal enum StockPageFunction_CurrentTest
        {
            Start,
            StringReturnEventArgs_Constructor,
            StringReturnEventArgs_Result,
            StringPageFunction_Constructor,
            StringPageFunction_OnFinish,
            StringPageFunction_ReturnEvent,
            ObjectReturnEventArgs_Constructor,
            ObjectReturnEventArgs_Result,
            ObjectPageFunction_Constructor,
            ObjectPageFunction_OnFinish,
            ObjectPageFunction_ReturnEvent,
            Int32ReturnEventArgs_Constructor,
            Int32ReturnEventArgs_Result,
            Int32PageFunction_Constructor,
            Int32PageFunction_OnFinish,
            Int32PageFunction_ReturnEvent,
            BooleanReturnEventArgs_Constructor,
            BooleanReturnEventArgs_Result,
            BooleanPageFunction_Constructor,
            BooleanPageFunction_OnFinish,
            BooleanPageFunction_ReturnEvent,
            End
        }

        private Window _mainWindow = null;
        private TextBlock _mwContent = null;
        StockPageFunction_CurrentTest _stockPageFunctionTest = StockPageFunction_CurrentTest.Start;

        private ReturnEventArgs<String> _sre = null;
        private PageFunction<String> _spf = null;
        private ReturnEventArgs<Object> _ore = null;
        private PageFunction<Object> _opf = null;
        private ReturnEventArgs<Int32> _i32re = null;
        private PageFunction<Int32> _i32pf = null;
        private ReturnEventArgs<bool> _bre = null;
        private PageFunction<bool> _bpf = null;

        private const String TEST_STRING = "a test string for you";
        private const String STRPGFUNC_MSG = "Created for OnFinish.";
        private Button _testButton = new Button();


        /// <summary>
        /// Sets up the TestLog, registers eventhandlers, 
        /// creates the object tree, creates the timers used for automation
        /// and starts the BVT by rendering the Window onscreen.
        /// </summary>
        void StockPageFunction_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("StockPageFunction");
            NavigationHelper.Output("App starting up");

            // Create element tree (Window and its contents)
            _mainWindow = new Window();
            _mainWindow.Left = 0;
            _mainWindow.Top = 0;
            _mainWindow.Width = 500;
            _mainWindow.Height = 500;
            _mainWindow.Title = "StockPageFunction BVT - Main Window";

            _mwContent = new TextBlock();
            _mwContent.Text = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Etiam metus. Vivamus sed leo.\n";
            _mwContent.Text += "Fusce in tellus rhoncus orci bibendum dapibus. Class aptent taciti sociosqu ad litora\n";
            _mwContent.Text += "torquent per conubia nostra, per inceptos hymenaeos. Etiam ullamcorper.  Proin consectetuer arcu dictum sapien.\n";
            _mwContent.Text += "Fusce urna. In vitae metus. Nullam in pede commodo nulla molestie feugiat.\n";
            _mwContent.Text += "Nulla facilisi. Vestibulum bibendum augue gravida urna placerat sollicitudin.\n";
            _mwContent.Text += "Cras porta. In non leo id lacus aliquet consectetuer. Proin lobortis tristique turpis.\n";
            _mwContent.Text += "Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae;\n";
            _mwContent.Text += "Etiam est felis, euismod vitae, scelerisque sit amet, ultrices non, nulla. Morbi id wisi.\n";
            _mwContent.Text += "Cras malesuada. Nullam a felis quis dui molestie aliquet. Praesent orci. Mauris fringilla mauris ut eros.\n";
            _mwContent.Text += "Proin massa est, vestibulum at, tempor sed, ullamcorper eget, arcu.\n";
            _mwContent.Text += "Donec et nibh eu mi adipiscing sagittis. Maecenas vel orci nec purus tincidunt dignissim.\n";

            // Register event-handlers for the main window
            _mainWindow.Activated += new EventHandler(StockPageFunction_WindowActivated);

            // Set up the BVT
            // Set the starting test for the BVT
            _stockPageFunctionTest = StockPageFunction_CurrentTest.StringReturnEventArgs_Constructor;
            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
            NavigationHelper.Output("Starting StockPageFunction BVTs.");

            // Show window
            _mainWindow.Content = _mwContent;
            _mainWindow.Show();
            base.OnStartup(e);
        }

        private void StockPageFunction_WindowActivated(object sender, EventArgs e)
        {
            while (!_stockPageFunctionTest.Equals(StockPageFunction_CurrentTest.End))
            {
                switch (_stockPageFunctionTest)
                {
                    // [1] StringReturnEvents() constructor
                    // Call the constructor and check that a non-null instance
                    // of StringReturnEventArgs is returned
                    case StockPageFunction_CurrentTest.StringReturnEventArgs_Constructor:
                        _sre = new ReturnEventArgs<String>(TEST_STRING);
                        CheckConstructor(_sre, typeof(ReturnEventArgs<String>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.StringReturnEventArgs_Result;
                        break;

                    // [2] StringReturnEvents.Result property
                    // Get the value for this property and make sure that the value 
                    // returned is the one set in the constructor
                    case StockPageFunction_CurrentTest.StringReturnEventArgs_Result:
                        CheckResultProperty(_sre.Result, TEST_STRING);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.StringPageFunction_Constructor;
                        break;

                    // [3] StringPageFunction() constructor
                    // Call the constructor and check that a non-null instance
                    // of StringPageFunction is returned
                    case StockPageFunction_CurrentTest.StringPageFunction_Constructor:
                        _spf = new PageFunction<String>();
                        CheckConstructor(_spf, typeof(PageFunction<String>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.StringPageFunction_OnFinish;
                        break;

                    // [4] StringPageFunction.OnFinish(string result) method
                    // Checks that the Return event is fired when OnFinish is called,
                    // and checks the Result property of the StringReturnEventArg to see
                    // if it equals STRPGFUNC_MSG
                    case StockPageFunction_CurrentTest.StringPageFunction_OnFinish:
                        //spf.Return += new StringReturnEventHandler(OnStringReturnEvent);
                        //spf.OnFinish(STRPGFUNC_MSG);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.ObjectReturnEventArgs_Constructor;
                        break;

                    // [5] ObjectReturnEventArgs() constructor
                    // Call the constructor and check that a non-null instance
                    // of ObjectReturnEventArgs is returned
                    case StockPageFunction_CurrentTest.ObjectReturnEventArgs_Constructor:
                        _ore = new ReturnEventArgs<Object>(_testButton);
                        CheckConstructor(_ore, typeof(ReturnEventArgs<Object>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.ObjectReturnEventArgs_Result;
                        break;

                    // [6] ObjectReturnArgs.Result property
                    // Get the value for this property and make sure that the value 
                    // returned is the one set in the constructor
                    case StockPageFunction_CurrentTest.ObjectReturnEventArgs_Result:
                        CheckResultProperty(_ore.Result, _testButton);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.ObjectPageFunction_Constructor;
                        break;

                    // [7] ObjectPageFunction() constructor
                    // Call the constructor and check that a non-null instance
                    // of ObjectPageFunction is returned
                    case StockPageFunction_CurrentTest.ObjectPageFunction_Constructor:
                        _opf = new PageFunction<Object>();
                        CheckConstructor(_opf, typeof(PageFunction<Object>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.ObjectPageFunction_OnFinish;
                        break;

                    // [8] ObjectPageFunction.OnFinish(object result) method
                    // Checks that the Return event is fired when OnFinish is called,
                    // and checks the Result property of the ObjectReturnEventArg to see
                    // if it equals the specified object
                    case StockPageFunction_CurrentTest.ObjectPageFunction_OnFinish:
                        //opf.Return += new ObjectReturnEventHandler(OnObjectReturnEvent);
                        //// Modify testButton slightly
                        //testButton.Content = TEST_STRING;
                        //opf.OnFinish(testButton);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.Int32ReturnEventArgs_Constructor;
                        break;

                    // [9] Int32ReturnEventArgs() constructor
                    // Call the constructor and check that a non-null instance
                    // of Int32ReturnEventArgs is returned
                    case StockPageFunction_CurrentTest.Int32ReturnEventArgs_Constructor:
                        _i32re = new ReturnEventArgs<Int32>(1000);
                        CheckConstructor(_i32re, typeof(ReturnEventArgs<Int32>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.Int32ReturnEventArgs_Result;
                        break;

                    // [10] Int32ReturnEventArgs.Result property
                    // Get the value for this property and make sure that the value 
                    // returned is the one set in the constructor
                    case StockPageFunction_CurrentTest.Int32ReturnEventArgs_Result:
                        CheckResultProperty(_i32re.Result, 1000);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.Int32PageFunction_Constructor;
                        break;

                    // [11] Int32PageFunction() constructor
                    // Call the constructor and check that a non-null instance
                    // of Int32PageFunction is returned
                    case StockPageFunction_CurrentTest.Int32PageFunction_Constructor:
                        _i32pf = new PageFunction<Int32>();
                        CheckConstructor(_i32pf, typeof(PageFunction<Int32>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.Int32PageFunction_OnFinish;
                        break;

                    // [12] Int32PageFunction.OnFinish(int result) method
                    // Checks that the Return event is fired when OnFinish is called,
                    // and checks the Result property of the Int32ReturnEventArg to see
                    // if it equals the specified integer
                    case StockPageFunction_CurrentTest.Int32PageFunction_OnFinish:
                        //i32pf.Return += new Int32ReturnEventHandler(OnInt32ReturnEvent);
                        //i32pf.OnFinish(-1);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.BooleanReturnEventArgs_Constructor;
                        break;

                    // [13] BooleanReturnEventArgs() constructor
                    // Call the constructor and check that a non-null instance
                    // of BooleanReturnEventArgs is returned
                    case StockPageFunction_CurrentTest.BooleanReturnEventArgs_Constructor:
                        _bre = new ReturnEventArgs<bool>(true);
                        CheckConstructor(_bre, typeof(ReturnEventArgs<bool>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.BooleanReturnEventArgs_Result;
                        break;

                    // [14] BooleanReturnEventArgs.Result property
                    // Get the value for this property and make sure that the value 
                    // returned is the one set in the constructor
                    case StockPageFunction_CurrentTest.BooleanReturnEventArgs_Result:
                        CheckResultProperty(_bre.Result, true);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.BooleanPageFunction_Constructor;
                        break;

                    // [15] BooleanPageFunction() constructor,
                    // Call the constructor and check that a non-null instance
                    // of BooleanPageFunction is returned
                    case StockPageFunction_CurrentTest.BooleanPageFunction_Constructor:
                        _bpf = new PageFunction<bool>();
                        CheckConstructor(_bpf, typeof(PageFunction<bool>));
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.BooleanPageFunction_OnFinish;
                        break;

                    // [16] BooleanPageFunction.OnFinish(bool result) method
                    // Checks that the Return event is fired when OnFinish is called,
                    // and checks the Result property of the BooleanReturnEventArg to see
                    // if it equals the specified boolean value
                    case StockPageFunction_CurrentTest.BooleanPageFunction_OnFinish:
                        //bpf.Return += new BooleanReturnEventHandler(OnBooleanReturnEvent);
                        //bpf.OnFinish(false);
                        _stockPageFunctionTest = StockPageFunction_CurrentTest.End;
                        break;

                    // We've somehow reached a non-enum value.  
                    // This is an error so do not continue running the BVT
                    default:
                        NavigationHelper.Fail("Not a valid BVT test.  Ending BVT.");
                        break;
                }
            }

            // Finish:  All the preceding tests have been run serially with no failures
            // Log that the test passed and shut down the BVT
            if (_stockPageFunctionTest.Equals(StockPageFunction_CurrentTest.End))
                NavigationHelper.Pass("All subtests passed.");
        }


        /// <summary>
        /// This verifies whether the constructor assigned a new non-null instance of
        /// the expectedObjType to the global variable assigned to a particular sub-test.
        /// If it has, then print out a successful status message.  Otherwise, fail the test
        /// and shut down the BVT.
        /// </summary>
        /// <param name="created">Instance created by the constructor</param>
        /// <param name="expectedObjType">The type of object we expect the constructor to create</param>
        private void CheckConstructor(object created, Type expectedObjType)
        {
            // if we have an instance of the expected object type and that instance is not null
            if (created.GetType().Equals(expectedObjType) && created != null)
                NavigationHelper.Output("Constructor correctly created a new instance of " + expectedObjType.ToString());
            else
                // the object created is either null or not the type we were expecting
                NavigationHelper.Fail("Expecting " + expectedObjType.ToString() + " to be created, not " + created.GetType().ToString());
        }


        /// <summary>
        /// Checks that the Result property value equals the value we originally set
        /// </summary>
        /// <param name="resultValue">Value of Result property</param>
        /// <param name="expectedValue">Value set for the Result property</param>
        private void CheckResultProperty(object resultValue, object expectedValue)
        {
            // if the value set in Result is the same as what we expected, then 
            // print out status message
            if (resultValue.Equals(expectedValue))
                NavigationHelper.Output("Result property equals the argument passed into the constructor");
            else
                // Result does not hold the value specified in the constructor
                NavigationHelper.Fail("Result property:  Expecting " + expectedValue.ToString() + ", got " + resultValue.ToString());
        }


        #region XXXReturnEventHandlers

        /// <summary>
        /// If this method gets called, it implies that the Return event was 
        /// correctly fired when StringPageFunction.OnFinish was called.  
        /// Check that the StringReturnEventArg that gets passed in has the same string
        /// as its Result property as the argument passed into OnFinish
        /// </summary>
        private void OnStringReturnEvent(object sender, ReturnEventArgs<String> e)
        {
            // If this was invoked, that means that StringPageFunction.OnFinish
            // generated a Return event and generated a StringReturnEventArg
            NavigationHelper.Output("StringPageFunction.OnFinish correctly fired the Return event");

            // Check that the StringReturnEventArgs contains the same string that
            // was passed into OnFinish
            if (e.Result.Equals(STRPGFUNC_MSG))
                NavigationHelper.Output("OnFinish generated/returned the correct StringReturnEventArgs");
            else
                NavigationHelper.Fail("OnFinish did not generate/return the correct StringReturnEventArgs");
        }


        /// <summary>
        /// If this method gets called, it implies that the Return event was 
        /// correctly fired when ObjectPageFunction.OnFinish was called.  
        /// Check that the ObjectReturnEventArg that gets passed in has the same object
        /// as its Result property as the argument passed into OnFinish
        /// </summary>
        private void OnObjectReturnEvent(object sender, ReturnEventArgs<Object> e)
        {
            // If this was invoked, that means that ObjectPageFunction.OnFinish
            // generated a Return event and generated a ObjectReturnEventArg
            NavigationHelper.Output("ObjectPageFunction.OnFinish correctly fired the Return event");

            // Check that the ObjectReturnEventArgs.Result property is the same object that
            // was passed into OnFinish
            if (e.Result.Equals(_testButton) && _testButton.Content.Equals(TEST_STRING))
                NavigationHelper.Output("OnFinish generated/returned the correct ObjectReturnEventArgs");
            else
                NavigationHelper.Fail("OnFinish did not generate/return the correct ObjectReturnEventArgs");
        }


        /// <summary>
        /// If this method gets called, it implies that the Return event was 
        /// correctly fired when Int32PageFunction.OnFinish was called.  
        /// Check that the Int32ReturnEventArg that gets passed in has the same integer
        /// as its Result property as the argument passed into OnFinish
        /// </summary>
        private void OnInt32ReturnEvent(object sender, ReturnEventArgs<Int32> e)
        {
            // If this was invoked, that means that Int32PageFunction.OnFinish
            // generated a Return event and generated a Int32ReturnEventArg
            NavigationHelper.Output("Int32PageFunction.OnFinish correctly fired the Return event");

            // Check that the Int32ReturnEventArgs.Result property is the same integer that
            // was passed into OnFinish
            if (e.Result.Equals(-1))
                NavigationHelper.Output("OnFinish generated/returned the correct Int32ReturnEventArgs");
            else
                NavigationHelper.Fail("OnFinish did not generate/return the correct Int32ReturnEventArgs");
        }


        /// <summary>
        /// If this method gets called, it implies that the Return event was 
        /// correctly fired when BooleanPageFunction.OnFinish was called.  
        /// Check that the BooleanReturnEventArg that gets passed in has the same boolean
        /// value as its Result property as the argument passed into OnFinish
        /// </summary>
        private void OnBooleanReturnEvent(object sender, ReturnEventArgs<bool> e)
        {
            // If this was invoked, that means that BooleanPageFunction.OnFinish
            // generated a Return event and generated a BooleanReturnEventArg
            NavigationHelper.Output("BooleanPageFunction.OnFinish correctly fired the Return event");

            // Check that the BooleanReturnEventArgs.Result property is the same integer that
            // was passed into OnFinish
            if (e.Result.Equals(false))
                NavigationHelper.Output("OnFinish generated/returned the correct BooleanReturnEventArgs");
            else
                NavigationHelper.Fail("OnFinish did not generate/return the correct BooleanReturnEventArgs");
        }

        #endregion
    }
}
