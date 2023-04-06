// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Test;
using Microsoft.Test.CrossProcess;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace TestXbap
{
    /// <summary>
    /// Test Xbap used for verifying HTML script interop
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public dynamic scriptObject;

        bool _retval;

        /// <summary>
        /// Get TestToRun from harness property bag and call the proper test, then shut down the test.
        /// Note: Each test name in the .xtc file looks like: "ScriptInterop_<Name>_[PT | FT](_FireFox)"
        /// Each name corresponds to a case in the switch statement below.  PT/FT is partial/full trust.
        /// </summary>
        private void OnLoaded(object sender, EventArgs e)
        {
            bool success = false;
            DictionaryStore.StartClient();
            if (DictionaryStore.Current["TestToRun"] != null)
            {
                string curTest = DictionaryStore.Current["TestToRun"];
                GlobalLog.LogEvidence("Running test: " + curTest);

                try
                {
                    switch(curTest)
                    {
                        case "ActiveXObject":
                            success = ActiveXObjectTest();
                            break;

                        case "ActiveXObjectNegative":
                            success = ActiveXObjectNegativeTest();
                            break;

                        case "AllArgs":
                            success = AllArgsTest();
                            break;

                        case "AppExit":
                            Application.Current.Exit += AppExit;
                            Application.Current.Shutdown();
                            return;   //we'll log pass/fail from AppExit

                        case "Array":
                            success = ArrayTest();
                            break;
 
                        case "BogusName":
                            success = BogusNameTest();
                            break;

                        case "Callback":
                            success = CallbackTest();
                            break;

                        case "DateTime":
                            success = DateTimeTest();
                            break;

                        case "Eval":
                            success = EvalTest();
                            break;

                        case "HostScriptNoFrame":
                        case "HostScriptXDomain":
                            success = HostScriptNullTest();
                            break;

                        case "Indexer":
                            success = IndexerTest();
                            break;

                        case "Json":
                            success = JsonTest();
                            break;

                        case "ManagedException":
                            success = ManagedExceptionTest();
                            break;

                        case "MtaThread":
                            success = MtaThreadTest();
                            break;

                        case "NonComVisible":
                            success = NonComVisibleTest();
                            break;

                        case "NonUIThread":
                            success = NonUIThreadTest();
                            break;

                        case "NullBinder":
                            success = NullBinderTest();
                            break;

                        case "NullIndex":
                            success = NullIndexTest();
                            break;

                        case "ObjectRoundTrip":
                            success = ObjectRoundTripTest();
                            break;

                        case "PropertyGetSet":
                            success = PropertyGetSetTest();
                            break;

                        case "ReturnAllArgs":
                            success = ReturnAllArgsTest();
                            break;

                        case "ReturnJsFunction":
                            success = ReturnJsFunctionTest();
                            break;

                        case "ScriptEvent":
                            success = ScriptEventTest();
                            break;

                        case "ScriptException":
                            success = ScriptExceptionTest();
                            break;

                        case "ScriptRoundTrip":
                            success = ScriptRoundTripTest();
                            break;

                        case "ScriptVariable":
                            success = ScriptVariableTest();
                            break;

                        case "ScriptWrongNumberOfArgs":
                            success = ScriptWrongNumberOfArgsTest();
                            break;

                        case "StructRoundTrip":
                            success = StructRoundTripTest();
                            break;

                        case "TwoXbaps":
                            success = TwoXbapsTest();
                            break;

                        case "UnpackArg":
                            success = UnpackArgTest();
                            break;

                        case "WrongNumberOfArgs":
                            success = WrongNumberOfArgsTest();
                            break;
 

                        default:
                            GlobalLog.LogEvidence("TestToRun not valid - nothing to run!");
                            break;
                    }                    
                }
                catch (Exception exception)
                {
                    GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                }

            } 
            else // bad test name
            {
                GlobalLog.LogEvidence("TestToRun not set - nothing to run!");
                TestLog.Current.Result = TestResult.Pass;
            }

            if (success)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }

            ApplicationMonitor.NotifyStopMonitoring();
        }

        /// <summary>
        /// Verify script interop can interact with an ActiveXObject on the parent html page
        /// </summary>
        private bool ActiveXObjectTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                  result = script.vid.AutoStart;
                  script.vid.Stop();
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                if(Convert.ToBoolean(result) == true)
                {
                    GlobalLog.LogEvidence("ActiveXObject use succeeded - got back true as expected");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("ActiveXObject use failed, got back: " + result.ToString() + "  Expected: true");
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop gets the proper exceptions when improperly interacting with an ActiveXObject on the parent html page
        /// </summary>
        private bool ActiveXObjectNegativeTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;

            bool returnValue = true;
            bool caught = false;
          
            GlobalLog.LogEvidence("Case 1 - call Stop with extra argument.  Expect TargetParameterCountException");
            try
            {
                script.vid.Stop(null);
            }
            catch (System.Reflection.TargetParameterCountException exception)
            {
                GlobalLog.LogEvidence("Got expected TargetParameterCountException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }
            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;
            GlobalLog.LogEvidence("Case 2 - call GetMediaParameterName with wrong argument type.  Expect ArgumentException");
            try
            {
                  result = script.vid.GetMediaParameterName("fail", "now");
            }
            catch (ArgumentException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }
            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Verify script interop can pass all (or most) argument types to script, and that they arrive intact.
        /// </summary>
        private bool AllArgsTest()
        {
            const string resultFor32Bit = "undefinednumbernumbernumberobjectbooleannumbernumbernumbernumbernumbernumbernumberunknownunknownstringnumber";
            const string resultFor64Bit = "undefinednumbernumbernumberobjectbooleannumbernumbernumbernumbernumbernumbernumbernumbernumberstringnumber";

            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            try
            {
                result = script.ArgTest(null,        //undefined
                    (float)3.14159,                  //number
                    (double)3.14159,                 //number
                    (decimal)3.14159,                //number
                    new ManagedObject(),             //object
                    false,                           //boolean
                    (byte)42,                        //number
                    (sbyte)49,                       //number
                    'a',                             //number
                    (int)43,                         //number
                    (uint)44,                        //number
                    (short)45,                       //number
                    (ushort)46,                      //number
                    (long)47,                        //unknown on 32 bit, number on 64 bit
                    (ulong)48,                       //unknown on 32 bit, number on 64 bit
                    "testing",                       //string
                    (int)JournalOwnership.OwnsJournal);   //int

            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                bool is32Bit = IntPtr.Size == 4;

                // Special case - on FireFox the 64-bit compare string is also right for 32-bit
                // On IE9, the apparent issue has also been fixed
                if ( (!script.navigator.userAgent.Contains("MSIE")) ||  (SystemInformation.Current.IEVersion.StartsWith("9.0")))
                {
                   is32Bit = false;
                   GlobalLog.LogEvidence("Using 'special case' logic - Assume JS engine can handle 64-bit values");
                }

                GlobalLog.LogEvidence("AllArgsTest result is: " + result);

                if (result.ToString() != (is32Bit ? resultFor32Bit : resultFor64Bit))
                {
                    GlobalLog.LogEvidence("AllArgsTest failed, expected: " + (is32Bit ? resultFor32Bit : resultFor64Bit));
                    return false;
                }
                else
                {
                    GlobalLog.LogEvidence("AllArgsTest passed - results matched");
                    return true;
                }
            }
        }

        /// <summary>
        /// Verify script interop can function properly during app shutdown.
        /// </summary>
        private void AppExit(object sender, ExitEventArgs e)
        {
            try
            {
                dynamic script = BrowserInteropHelper.HostScript;
                object result = null;

                scriptObject = script.eval("3 + 4");

                result = scriptObject;

                if(result == null)
                {
                    GlobalLog.LogEvidence("unexpected null result");
                    TestLog.Current.Result = TestResult.Fail;
                }
                else if (result.ToString() == "7")
                {
                    GlobalLog.LogEvidence("Got expected result during AppExit");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Got unexpected result during AppExit: " + result.ToString() + " , expected: 7");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
            }
            ApplicationMonitor.NotifyStopMonitoring();
        }


        /// <summary>
        /// Verify passing arrays of value and reference types to and from script works properly.
        /// </summary>
        private bool ArrayTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            int[] result = null;
            object[] result2 = null;
            int[] valueArray = new int[] {3,4};
            object[] referenceArray = new ManagedObject[2];
            referenceArray[0] = new ManagedObject(42);
            referenceArray[1] = new ManagedObject(999);
            bool returnValue = false;
          
            try
            {
                result = script.ArrayTest(valueArray);
                result2 = script.ArrayTest(referenceArray);
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Got unexpected exception: " + exception.ToString());
                return false;
            }
            if(result.Length < 2)
            {
                GlobalLog.LogEvidence("result length was < 2.  Expected length 2.");
                returnValue = false;
            }
            else
            {
                if((result[0] == 3) && (result[1] == 4))
                {
                    GlobalLog.LogEvidence("Got expected integer array {3,4}");
                    returnValue = true;
                }
                else
                {
                    GlobalLog.LogEvidence("Did not get expected integer array {3,4}.  Instead got: " + result[0] + " and " + result[1]);
                    returnValue = false;
                }
            }
            if(result2.Length < 2)
            {
                GlobalLog.LogEvidence("result2 length was < 2.  Expected length 2.");
                returnValue = false;
            }
            else
            {
                if((result2[0] is ManagedObject) && (result2[1] is ManagedObject) && (((ManagedObject)result2[0]).MyValue == 42) &&
                    (((ManagedObject)result2[1]).MyValue == 999))
                {
                    GlobalLog.LogEvidence("Got expected ManagedObject array {42,999}");
                    returnValue = true;
                }
                else
                {
                    GlobalLog.LogEvidence("Did not get expected ManagedObject array {42,999}.  Instead got: " + result2[0].ToString() + " and " + result2[1].ToString());
                    returnValue = false;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Verify script interop fails properly when someone tries to call a nonexistent script function.
        /// </summary>
        private bool BogusNameTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.BogusNameTest();
            }
            catch (MissingMethodException exception)
            {
                GlobalLog.LogEvidence("Got expected exception calling bogus function: " + exception.ToString());
                return true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Got unexpected exception calling bogus function: " + exception.ToString());
                return false;
            }

            GlobalLog.LogEvidence("Didn't get an exception calling bogus function when we should have");
            return false;
        }

        /// <summary>
        /// Verify script interop can send a managed object to script code, which can call back into the managed object.
        /// </summary>
        private bool CallbackTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;

            ManagedObject testObject = new ManagedObject(4);
          
            try
            {
                result = script.CallbackTest(testObject);
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(testObject.MyValue == 345)
            {
                GlobalLog.LogEvidence("Callback succeeded - value 345 was set");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Callback apparently failed - value 345 was not set, got: " + testObject.MyValue);
                return false;
            }
        }

        /// <summary>
        /// Verify script interop can pass a managed DateTime to script which can parse it.
        /// </summary>
        private bool DateTimeTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            DateTime dateTime = new DateTime(2006, 11, 28);
           
            try
            {
                result = script.DateTimeTest(dateTime);
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                
                if(result.ToString() == "2006/11/28")
                {
                    GlobalLog.LogEvidence("DateTime test succeeded - got back 2006/11/28");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("DateTime test failed, got: " + result.ToString() + " expected: 2006/11/28");
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop can use the script eval function.
        /// </summary>
        private bool EvalTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.eval("3 + 4");
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                if(result.ToString() == "7")
                {
                    GlobalLog.LogEvidence("Eval test succeeded - got back 7");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Eval test failed, got: " + result.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop fails to get a valid host script object in the two cases where there shouldn't be one -
        /// an xbap directly hosted in browser, or an xbap in a cross-domain frame.
        /// </summary>
        private bool HostScriptNullTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;

            if(script == null)
            {
                GlobalLog.LogEvidence("As expected, BrowserInteropHelper.HostScript is null when the XBAP isn't in a frame or the frame is cross-domain");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("BrowserInteropHelper.HostScript was not null when it should have been.");

                // if this fails, do some diagnostics to see how bad it fails
                GlobalLog.LogEvidence(script.ToString());
                result = script.eval("3 + 4");
                GlobalLog.LogEvidence(result.ToString());

                return false;
            }
        }

        /// <summary>
        /// Verify script interop can use an indexer into an html array.
        /// </summary>
        private bool IndexerTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.document.anchors[0].href;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                if(result.ToString().Contains("about:blank"))
                {
                    GlobalLog.LogEvidence("Indexer test succeeded - got back anchors[0].href");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Indexer test failed, got: " + result.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop can interact with JSON (JavaScript Object Notation) objects.  See Wikipedia for JSON.
        /// </summary>
        private bool JsonTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            dynamic result;
            int resultNumber;
            string resultName;
            int objectNumber;
          
            try
            {
                result = script.JsonTest();
                resultName = result.name;
                resultNumber = result.number;

                result.number = new ManagedObject(23);
                objectNumber = ((ManagedObject)result.number).MyValue;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if((resultName == "Nerf Herder") && (resultNumber == 1) && (objectNumber == 23))
            {
                GlobalLog.LogEvidence("JSON test passed - got correct name and numbers back");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("JSON test failed, got: name: " + resultName + " number: " + resultNumber + " objectNumber: " + objectNumber);
                return false;
            }
        }

        /// <summary>
        /// Verify script interop can call script with a managed object, and if a call into the managed object throws,
        /// the exception is passed back into the script code.
        /// </summary>
        private bool ManagedExceptionTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.ManagedException(new ManagedObject());
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                if(result.ToString().Contains("proves the rule"))
                {
                    GlobalLog.LogEvidence("Managed exception test succeeded - got back exception string");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Managed exception test failed, got back '" + result.ToString() + "' expected: 'proves the rule'");
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop does not work from an MTA thread.
        /// </summary>
        private bool MtaThreadTest()
        {
            GlobalLog.LogEvidence("Creating MTA thread to call EvalTest from.  Should throw.");

            // default is MTA
            Thread t = new Thread(EvalTestWrap);
            t.Start();
            t.Join();

            return !_retval;
        }

        /// <summary>
        /// Verify script interop cannot pass Non-COMVisible objects to script.
        /// </summary>
        private bool NonComVisibleTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.RoundTripTest(new NonComVisibleObject());
            }
            catch (ArgumentException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentException: " + exception.ToString());
                return true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                return false;
            }

            GlobalLog.LogEvidence("Passing a Non-ComVisible object unexpectedly didn't get an exception.");
            return false;
        }

        /// <summary>
        /// Verify script interop works from a non-UI STA thread.
        /// </summary>
        private bool NonUIThreadTest()
        {
            GlobalLog.LogEvidence("Creating STA thread to call EvalTest from.  Should succeed.");

            Thread t = new Thread(EvalTestWrap);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return _retval;
        }

        private void EvalTestWrap()
        {
            try
            {
                _retval = EvalTest();
            }
            catch(Exception exception)
            {
                GlobalLog.LogEvidence("Exception: " + exception.ToString());
                _retval = false;
            }
        }

        /// <summary>
        /// Verify script interop code throws ArgumentNullException for a null binder.
        /// </summary>
        private bool NullBinderTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            bool returnValue = true;
            bool caught = false;
            object result = null;

            DynamicScriptObject dynamicObject = script as DynamicScriptObject;
          

            GlobalLog.LogEvidence("Negative test for TryGetIndex (null binder)");
            try
            {
                dynamicObject.TryGetIndex(null, null, out result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;

            GlobalLog.LogEvidence("Negative test for TryGetMember (null binder)");
            try
            {
                dynamicObject.TryGetMember(null, out result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;

            GlobalLog.LogEvidence("Negative test for TryInvoke (null binder)");
            try
            {
                dynamicObject.TryInvoke(null, null, out result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;

            GlobalLog.LogEvidence("Negative test for TryInvokeMember (null binder)");
            try
            {
                dynamicObject.TryInvokeMember(null, null, out result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }


            caught = false;

            GlobalLog.LogEvidence("Negative test for TrySetIndex (null binder)");
            try
            {
                dynamicObject.TrySetIndex(null, null, result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;

            GlobalLog.LogEvidence("Negative test for TrySetMember (null binder)");
            try
            {
                dynamicObject.TrySetMember(null, result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Verify script interop code throws the proper Exception for a null or bad index.
        /// </summary>
        private bool NullIndexTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            bool returnValue = true;
            bool caught = false;
            //object result = null;


            //we don't really "use" any of this junk, but I can't test the index arg of TryGet/SetIndex without passing a valid binder, so...
            List<CSharpArgumentInfo> argInfo = new List<CSharpArgumentInfo>();
            argInfo.Add(CSharpArgumentInfo.Create(0, null));
            
            // CSharpGetIndexBinder getBinder = new CSharpGetIndexBinder(this.GetType(), argInfo);
            argInfo.Add(CSharpArgumentInfo.Create(0, null));
            // CSharpGetIndexBinder setBinder = new CSharpGetIndexBinder(this.GetType(), argInfo);

            DynamicScriptObject dynamicObject = script as DynamicScriptObject;
          

            GlobalLog.LogEvidence("Negative test for TryGetIndex (null index)");
            try
            {
                // 
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;
            GlobalLog.LogEvidence("Negative test for TrySetIndex (null index)");
            try
            {   
                // dynamicObject.TrySetIndex(setBinder, null, result);
            }
            catch (ArgumentNullException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentNullException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;
            GlobalLog.LogEvidence("Negative test for TrySetIndex (index[2])");
            try
            {
                // 

            }
            catch (ArgumentException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            caught = false;
            GlobalLog.LogEvidence("Negative test for TrySetIndex (index[1] with null)");
            try
            {
                object[] testObj = new object[1];
                testObj[0] = null;  

                // 

            }
            catch (ArgumentOutOfRangeException exception)
            {
                GlobalLog.LogEvidence("Got expected ArgumentOutOfRangeException: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                caught = true;
                returnValue = false;
            }

            if (!caught)
            {
                GlobalLog.LogEvidence("Expected an exception but none was caught");
                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Verify script interop can pass managed objects to script, which can pass them back to us intact.
        /// </summary>
        private bool ObjectRoundTripTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.RoundTripTest(new ManagedObject(42));
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;   
            }
            else
            {
                GlobalLog.LogEvidence("Result was: " + result.ToString());

                ManagedObject returnedObject = result as ManagedObject;
                if (returnedObject == null)
                {
                    GlobalLog.LogEvidence("unexpected null returnedObject");
                    return false;   
                }

                if(returnedObject.MyValue == 42)
                {
                    GlobalLog.LogEvidence("Round trip succeeded - value 42 was set");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Round trip failed - value 42 was not set, got: " + returnedObject.MyValue);
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop can get or set properties on the parent html page
        /// </summary>
        private bool PropertyGetSetTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                script.document.title = "日本語";
                result = script.document.title;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                if(result.ToString() == "日本語")
                {
                    GlobalLog.LogEvidence("Property Get/Set test succeeded - got back 日本語");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Property Get/Set test failed, got: " + result.ToString() + " expected: 日本語");
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop can call script and have it return the various variable types allowed in script.
        /// </summary>
        private bool ReturnAllArgsTest()
        {
            //"unknown" apparently can't be created in Javascript, so I've removed it                
            const string compareTo = "true,3.14,1e+308,Infinity,0,-Infinity,test,,,Infinity,-Infinity,NaN";

            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            try
            {
                result = script.ReturnArgsTest();

            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;
            }
            else
            {
                GlobalLog.LogEvidence("return args: " + result.ToString());
            }

            if(result.ToString() == compareTo)
            {
                GlobalLog.LogEvidence("Got expected return args");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Got unexpected return args.  Expected: " + compareTo);
                return false;
            }
        }

        /// <summary>
        /// Verify script interop can call script which returns a javascript function, then call into that function.
        /// </summary>
        private bool ReturnJsFunctionTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            try
            {
                dynamic jsFunction = script.ReturnJsFunction();
                result = jsFunction.call(null, 3, 4);

            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + exception.ToString());
                return false;
            }

            if(result.ToString() == "7")
            {
                GlobalLog.LogEvidence("Got expected result 7 from calling Javascript function");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Got unexpected return args.  Got: " + result.ToString()  + " Expected: 7");
                return false;
            }
        }

        /// <summary>
        /// Verify script interop can call script with a managed object, have the script hook up an html event to call a method
        /// on that object, then cause the event to fire.
        /// </summary>
        private bool ScriptEventTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.ScriptEventTest(new ManagedObject(42));
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;   
            }
            else
            {
                GlobalLog.LogEvidence("Result was: " + result.ToString());

                ManagedObject returnedObject = result as ManagedObject;
                if (returnedObject == null)
                {
                    GlobalLog.LogEvidence("unexpected null returnedObject");
                    return false;   
                }

                if(returnedObject.MyValue == 409)
                {
                    GlobalLog.LogEvidence("script event succeeded - value 409 was set");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Script event failed - value 409 was not set, got: " + returnedObject.MyValue);
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop can call script, have the script throw an exception, and get back the proper exception
        /// </summary>
        private bool ScriptExceptionTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
          
            try
            {
                result = script.ScriptExceptionTest();
            }
            catch (TargetInvocationException exception)
            {
                GlobalLog.LogEvidence("Got expected TargetInvocationException: " + exception.ToString());
                return true;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;   
            }
            else
            {
                GlobalLog.LogEvidence("Script exception test failed - exception was not thrown, got: " + result);
                return false;
            }
        }

        /// <summary>
        /// Verify script interop can pass script a managed object, have it pass back several variable types into that object,
        /// then pass those variables back into script and verify they're still the same.
        /// </summary>
        private bool ScriptRoundTripTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            ManagedObject managedObject = new ManagedObject(0);
          
            try
            {
                GlobalLog.LogEvidence("About to make first call into script");
                result = script.ScriptRoundTripTest(managedObject);

                GlobalLog.LogEvidence("results: " + managedObject.ReturnedArg1 + "," + managedObject.ReturnedArg2 + "," + managedObject.ReturnedArg3 + "," +  
                    managedObject.ReturnedArg4 + "," + managedObject.ReturnedArg5);

                GlobalLog.LogEvidence("About to make second call into script");
                result = script.VerifyRoundTrip(managedObject.ReturnedArg1, managedObject.ReturnedArg2, managedObject.ReturnedArg3, managedObject.ReturnedArg4, managedObject.ReturnedArg5);
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;   
            }
            else
            {
                GlobalLog.LogEvidence("Result was: " + result.ToString());

                if(result.ToString() == "booleannumbernumberstringobject")
                {
                    GlobalLog.LogEvidence("Script round trip succeeded");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Script round trip failed.  Expected: booleannumbernumberstringobject");
                    return false;
                }
            }
        }

        /// <summary>
        /// Verify script interop can set script variable to a reference type and get the proper item back.
        /// </summary>
        private bool ScriptVariableTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;

            try
            {   
                script.scriptVariable = "test";
                result = script.scriptVariable;
                if (!VerifyResult(result, typeof(string), "test")) return false;

                script.scriptVariable = 42;
                result = script.scriptVariable;
                if (!VerifyResult(result, typeof(int), 42)) return false;

                script.scriptVariable = false;
                result = script.scriptVariable;
                if (!VerifyResult(result, typeof(bool), false)) return false;

                script.scriptVariable = (double)-3.14;
                result = script.scriptVariable;
                if (!VerifyResult(result, typeof(double), -3.14)) return false;

                script.scriptVariable = new ManagedObject(90);
                result = script.scriptVariable;                
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }
            if (result is ManagedObject)
            {
                int returnValue = ((ManagedObject)result).MyValue;
                GlobalLog.LogEvidence("Result: " + returnValue.ToString());
                
                if(returnValue == 90)
                {
                    GlobalLog.LogEvidence("Script variable test passed.  Return value was 90 as expected.");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Script variable test failed.  Got '" + returnValue + "' Expected: 90");
                    return false;
                }
            }
            else if (result == null)
            {
                GlobalLog.LogEvidence("Script variable test failed.  Got null, expected:  ManagedObject");
                return false;
            }
            else
            {
                GlobalLog.LogEvidence("Script variable test failed.  Got '" + result.GetType() + "' Expected:  ManagedObject");
                return false;
            }
        }

        /// <summary>
        /// Verify result of script interop is proper type and value
        /// </summary>
        private bool VerifyResult(object result, Type expectedType, object expectedValue)
        {
            if(result.GetType() != expectedType)
            {
                GlobalLog.LogEvidence("Result should have been type " + expectedType.ToString() + " but was " + result.GetType().ToString());
                return false; 
            }
            if(result.ToString() == expectedValue.ToString())
            {
                GlobalLog.LogEvidence("Result was " + result.ToString() + " as expected.");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Result should have been " + expectedValue.ToString() + " but was " + result.ToString());
                return false; 
            }            
        }


        /// <summary>
        /// Verify script interop can call script which tries to call back into managed code, but with the wrong name
        /// or number of arguments, and that we get the proper error codes back from script.
        /// </summary>
        private bool ScriptWrongNumberOfArgsTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;

            try
            {                
                result = script.WrongNumberOfArgsTest(new ManagedObject(0));                
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result.ToString() == "-2146828283 / -2146827838")
            {
                GlobalLog.LogEvidence("Script wrong number of args test passed");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Script wrong number of args test failed.  got '" + result.ToString() + "' Expected:  '-2146828283 / -2146827838'");
                return false;
            }
        }

        /// <summary>
        /// Verify script interop can pass a managed struct to script, which can access that struct.
        /// </summary>
        private bool StructRoundTripTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            ManagedStruct returnedStruct;
          
            try
            {
                result = script.RoundTripTest(new ManagedStruct(42));
                returnedStruct = (ManagedStruct)result;
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("unexpected null result");
                return false;   
            }
            else
            {
                GlobalLog.LogEvidence("Result was: " + result.ToString());

                if(returnedStruct.MyValue == 42)
                {
                    GlobalLog.LogEvidence("Round trip succeeded - value 42 was set");
                    return true;
                }
                else
                {
                    GlobalLog.LogEvidence("Round trip failed - value 42 was not set, got: " + returnedStruct.MyValue);
                    return false;
                }
            }
        }

        /// <summary>
        /// This gets called by two separate xbaps in different frames in an html page.  One will be the setter app, the other
        /// will be the getter.  The setter creates a managed object and calls into script to set the object there.  It then
        /// loops waiting for the object to be nulled out.  The getter loops calling into script to see if the object is set.
        /// If it is, it sets it to null.  When the setter sees the object null out, the test passes.
        /// </summary>
        private bool TwoXbapsTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            string sourceUri = BrowserInteropHelper.Source.ToString();
            ManagedObject managedObject = null;
          
            try
            {
                if (sourceUri.Contains("Browser=1")) //setter
                {
                    managedObject = new ManagedObject(1);
                    result = script.SetSavedObject(managedObject);
                    GlobalLog.LogEvidence("Setter: set managed object");

                    for (int index = 1; index <= 20; index++)
                    {
                        Thread.Sleep(1000);
                        GlobalLog.LogEvidence("Setter: Checking for object set to null, try #" + index);
                        result = script.GetSavedObject();
                        if (result == null)
                        {
                            GlobalLog.LogEvidence("Setter: Other browser cleared the object.  Pass.");
                            return true;
                        }
                    }

                    GlobalLog.LogEvidence("Setter: Waited 20 seconds.  Getter didn't clear the object.  Fail");
                    return false;
                }
                else //getter
                {
                    for (int index = 1; index <= 20; index++)
                    {
                        Thread.Sleep(1000);
                        GlobalLog.LogEvidence("Getter: Checking for object, try #" + index);
                        result = script.GetSavedObject(); 
                        if (result != null)
                        {
                            GlobalLog.LogEvidence("Getter: Got object: " + result.ToString());
                            //note: we can't actually do anything with the object.  It's not from our app.
                            script.SetSavedObject(null);
                            GlobalLog.LogEvidence("Getter: Set object to null");
                            break;
                        }                   
                    }
                }
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(result == null)
            {
                GlobalLog.LogEvidence("Getter: Unexpected null result after 20 tries");
                return false;   
            }
            else
            {
                //This browser needs to just wait for the other one to log pass and close.
                while(true)
                {
                    Thread.Sleep(10000);
                }
            }
        }

        /// <summary>
        /// Verify script interop can get back an object from script, pass it back into the script which can unpack it properly.
        /// (We have to unwrap the object before passing it into script again.)
        /// </summary>
        private bool UnpackArgTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            dynamic result;
            string resultName;
          
            try
            {
                result = script.JsonTest();
                resultName = script.UnpackArg(result);             
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            if(resultName == "Nerf Herder")
            {
                GlobalLog.LogEvidence("Unpack arg test passed - got correct name back");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Unpack arg test failed, got: name: " + resultName);
                return false;
            }
        }

        /// <summary>
        /// Verify script interop gets the proper result when calling script with too few or too many arguments.
        /// Too few = NaN is filled in for missing aruments.  Too many = drop extra arguments.
        /// </summary>
        private bool WrongNumberOfArgsTest()
        {
            dynamic script = BrowserInteropHelper.HostScript;
            object result = null;
            object result2 = null;

            try
            {
                // TestAdd takes 2 args.  We expect this to return NaN since that's what 3 + NaN is.
                result = script.TestAdd(3);

                // Javascript is cool.  It just calls TestAdd(3,4) and throws the 5 away.
                result2 = script.TestAdd(3, 4, 5);                
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("unexpected exception: " + exception.ToString());
                return false;
            }

            GlobalLog.LogEvidence("Result: " + result.ToString() + " Result2: " + result2.ToString());
            int ignoreThis;

            if(!Int32.TryParse(result.ToString(), out ignoreThis) && (result2.ToString() == "7"))
            {
                GlobalLog.LogEvidence("Got expected results NaN and 7 from calling Javascript function with wrong numbers of arguments");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Calling TestAdd with wrong number of args returned the wrong result. Expected: NaN and 7");
                return false;
            }
        }
    }

    /// <summary>
    /// This is the managed object several tests use to pass to script for various reasons.
    /// </summary>
    [ComVisible(true)]
    public class ManagedObject
    {
        public ManagedObject()
        {
        }

        public ManagedObject(int input)
        {
            MyValue = input;
        }

        public void Callback(object inputObject)
        {
            GlobalLog.LogEvidence("callback: " + inputObject.ToString());
            MyValue = 345;
        }

        [DispId(0)]
        public void Clicked()
        {
            GlobalLog.LogEvidence("Managed object got click event");
            MyValue = 409;
        }

        public void CauseException()
        {
            throw new Exception("proves the rule");
        }

        public void ArgTypes(object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            ReturnedArg1 = arg1;
            ReturnedArg2 = arg2;
            ReturnedArg3 = arg3;
            ReturnedArg4 = arg4;
            ReturnedArg5 = arg5;
        }

        public int MyValue;

        public object ReturnedArg1;
        public object ReturnedArg2;
        public object ReturnedArg3;
        public object ReturnedArg4;
        public object ReturnedArg5;
    }

    /// <summary>
    /// This is the managed object used to verify non COMVisible objects can't be passed to script.
    /// </summary>
    public class NonComVisibleObject
    {
        public int MyValue;
    }

    /// <summary>
    /// This is the managed struct one test passes to script.
    /// </summary>
    [ComVisible(true)]
    public struct ManagedStruct
    {
        public ManagedStruct(int input)
        {
            MyValue = input;
        }

        [DispId(0)]
        public void Callback(object inputObject)
        {
            GlobalLog.LogEvidence("callback: " + inputObject.ToString());
        }

        public int MyValue;
    }
}
