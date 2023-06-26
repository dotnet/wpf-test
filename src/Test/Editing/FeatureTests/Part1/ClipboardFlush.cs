// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides coverage for Clipboard.Flush() method.

using System;
using System.Reflection;
using System.Windows;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Test.Uis.Loggers;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// This test case tests Clipboard.Flush(). 
    /// Clipboard.Flush() permanently renders the contents of the last DataObject 
    /// onto the clipboard, meaning the data will remain on the clipboard after 
    /// the application exits. 
    /// Test has two parameters flush and copy.
    ///   flush : If true, test will call Clipboard.Flush() before test application exits.  
    ///           This will render clipboard data to system clipboard, even if copy is false.
    ///   copy  : If true, test will ensure that Clipboard.SetDataObject() is called with copy==true
    ///           This also renders data permanently to clipboard.  Need to enusure this still works when Flush() is not called
    /// </summary>
    [Test(0, "Clipboard", "ClipboardFlush_flush:false_copy:true", MethodName = "Run", TestParameters = "flush=false,copy=true")]
    [Test(0, "Clipboard", "ClipboardFlush_flush:true_copy:true", MethodName = "Run", TestParameters = "flush=true,copy=true")]
    [Test(0, "Clipboard", "ClipboardFlush_flush:false_copy:false", MethodName = "Run", TestParameters = "flush=false,copy=false")]
    [Test(0, "Clipboard", "ClipboardFlush_flush:true_copy:false", MethodName = "Run", TestParameters = "flush=true,copy=false")]
    public class ClipboardFlushTest : StepsTest
    {
        #region Constructor

        public ClipboardFlushTest()
        {
            _data = "This is a string for the clipboard.";
            _copy = Convert.ToBoolean(DriverState.DriverParameters["copy"]);
            _flush = Convert.ToBoolean(DriverState.DriverParameters["flush"]);

            this.RunSteps += RunTestCase;
        }

        #endregion Constructor

        #region Main flow

        public TestResult RunTestCase()
        {
            if (ClipboardFlushTest.GetFlushMethod() == null)
            {
                // ignore case if current bits do not contain this Flush() method
                Logger.Current.Log("Ignoring test, current bits do not contain method System.Windows.Clipboard.Flush()");
                return TestResult.Ignore;
            }

            // may need to start process
            FlushApplication flushapp = new FlushApplication(_data, _copy, _flush);
            flushapp.Run();

            string clipboardText = Clipboard.GetText();

            // false only if both copy and flush are false
            bool expectTextOnClipboard = (_copy | _flush);

            bool result = expectTextOnClipboard ? _data.Equals(clipboardText) : !_data.Equals(clipboardText);
            string message = expectTextOnClipboard ?
                string.Format("Clipboard text should be equal to '{0}'", _data) :
                string.Format("Clipboard text should be empty and not equal to '{0}'", _data);

            Verifier.Verify(result, message);

            return TestResult.Pass;
        }

        #endregion Main flow

        public static MethodInfo GetFlushMethod()
        {
            Assembly presentationcore = typeof(Clipboard).Assembly;
            Type type = presentationcore.GetType("System.Windows.Clipboard");
            MethodInfo method = type.GetMethod("Flush", BindingFlags.Public | BindingFlags.Static);
            return method;
        }

        private readonly string _data = string.Empty;
        private readonly bool _copy = false;
        private readonly bool _flush = false;
    }

    public class FlushApplication : System.Windows.Application
    {
        public FlushApplication(string data, bool copy, bool flush)
        {
            this._data = data;
            this._copy = copy;
            this._flush = flush;
        }

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetText(_data);
            Clipboard.SetDataObject(dataObject, _copy);
            this.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_flush)
            {
                // use reflection to invoke flush method.
                MethodInfo flushMethod = ClipboardFlushTest.GetFlushMethod();
                flushMethod.Invoke(null, null);
            }
        }

        private readonly string _data = string.Empty;
        private readonly bool _copy = false;
        private readonly bool _flush = false;
    }
}