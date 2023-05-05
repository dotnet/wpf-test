// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.CDATA
{
    /// <summary>
    /// Parser cdata xaml simple
    /// </summary>
    /// <remarks>
    /// This is Parser BVT test that parse XAML using CDATA tag, and UIBinding.
    /// </remarks>
    public class ParserCDATA
    {
        #region ParserCDATA
        /// <summary>
        /// 
        /// </summary>
        public ParserCDATA()
        {
        }
        #endregion ParserCDATA

        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
            string strParams = DriverState.DriverParameters["TestParams"];

            GlobalLog.LogStatus("Core:ParserCDATA Started ..." + "\n"); // Start ParserBVT test

            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "CDATAPARSE":
                    TestCDATAParse();
                    break;
                case "UIBINDPARSE":
                    TestUIBind();
                    break;
                case "COMPACT":
                    TestCompactSyntax();
                    break;
                default:
                    GlobalLog.LogStatus("ParserCDATA.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest
        #region TestCDATAParse
        /// <summary>
        /// Parser BVT parsing CDATA tag inside Button.
        /// scenario:
        ///     - Load cdata.xaml
        /// Verify:
        ///     - Button Content value
        /// </summary>
        void TestCDATAParse()
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ParserCDATA.TestCDATAParse Started ..." + "\n");
            CreateContext();

                UIElement root = StreamParser(_CDATAFile);

                Page rootPage = root as Page;
                if (null == rootPage)
                    throw new Microsoft.Test.TestSetupException("Page cast filed");

                DockPanel foo = rootPage.Content as DockPanel;
                GlobalLog.LogStatus("Verifying CDATA Tag Test...");
                if (foo == null)
                    throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");
                IEnumerator pf = LogicalTreeHelper.GetChildren(foo).GetEnumerator();
                pf.MoveNext();
                Button b = (Button)pf.Current;
                if ((string)b.Content != "hello")
                    throw new Microsoft.Test.TestValidationException("CDATA tag is not parsing properly....Value is " + (string)b.Content);
                
                //_helper.DisplayTree(root); //The tree is already displayed after the LoadXaml call
            
            DisposeContext();
            GlobalLog.LogStatus("TestCDATAParse Test Pass!...");
        }
        #endregion TestCDATAParse
        #region TestUIBind
        /// <summary>
        /// Parser BVT parsing TestUIBind tag.
        /// scenario:
        ///     - Load DataBindTest.xaml
        /// Verify:
        ///     - TextBlock Text Value
        ///     - TextBlock Text Value
        ///     - Button Background value
        /// </summary>
        void TestUIBind()
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ParserCDATA.TestUIBind Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_DATAFile);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");

            Border foo = rootPage.Content as Border;
            GlobalLog.LogStatus("Verifying TestUIBind Tag Test...");
            if(foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");
            IEnumerator pf = LogicalTreeHelper.GetChildren(foo).GetEnumerator();
            pf.MoveNext();
            DockPanel dp = (DockPanel)pf.Current;
            s_data = (TestDataBind)dp.DataContext;
            IEnumerator ie = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            ie.MoveNext();
            TextBlock e1 = (TextBlock)ie.Current;
            if (e1 != null)
            {
                        GlobalLog.LogStatus("String: " + s_data.String);
                    if (e1.Text != s_data.String)
                        throw new Microsoft.Test.TestValidationException("Test Fail! Binding on e1 does not return correct value...." + e1.Text);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Expecting a Text");
            }
                    ie.MoveNext();
                    TextBlock e2 = (TextBlock)ie.Current;
            if (e2 != null)
            {
                        GlobalLog.LogStatus("String: " + s_data.String);
                        if(e2.Text != s_data.String)
                            throw new Microsoft.Test.TestValidationException("Test Fail! Binding on e2 does not return correct value...." + e2.Text);
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Expecting a Text");
            }
                    ie.MoveNext();
                    Button e3 = (Button)ie.Current;
            if (e3 != null)
            {
                        GlobalLog.LogStatus("Color: " + s_data.Color);
                        if(((SolidColorBrush)e3.Background).Color != (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(s_data.Color))
                           throw new Microsoft.Test.TestValidationException("Test Fail! Binding on e3 does not return correct value...." + ((SolidColorBrush)e3.Background).Color.ToString());
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Expecting a Button.");
            }
                    

            _helper.DisplayTree(foo);
                

            DisposeContext();
            GlobalLog.LogStatus("TestUIBind Test Pass!...");
        }
        #endregion TestUIBind
        #region TestCompactSyntax
        /// <summary>
        /// Parser BVT parsing TestCompactSyntax tag.
        /// scenario:
        ///     - Load CompactBinding.xaml
        /// Verify:
        ///     - TextBlock Text Value
        ///     - Button Background value
        /// </summary>
        void TestCompactSyntax()
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:ParserCDATA.TestCompactSyntax Started ..." + "\n");
            CreateContext();

            UIElement root = StreamParser(_compactSyn);
            Page rootPage = root as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");

            Border foo = rootPage.Content as Border;
            GlobalLog.LogStatus("Verifying TestUIBind Tag Test...");
            if(foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a DockPanel");
            IEnumerator pf = LogicalTreeHelper.GetChildren(foo).GetEnumerator();
            pf.MoveNext();
            DockPanel dp = (DockPanel)pf.Current;
            s_data = (TestDataBind)dp.DataContext;
            IEnumerator ie = LogicalTreeHelper.GetChildren(dp).GetEnumerator();
            ie.MoveNext();
            TextBlock e1 = (TextBlock)ie.Current;
            ie.MoveNext();
            Button e3 = (Button)ie.Current;
            dp.AddHandler(Binding.TargetUpdatedEvent, new EventHandler<DataTransferEventArgs>(OnDataTransfer));
            e3.AddHandler(Binding.TargetUpdatedEvent, new EventHandler<DataTransferEventArgs>(OnDataTransfer));
            DoChangeSourceValue();
            
            _helper.DisplayTree(foo);

            if(e1.Text != s_data.String)
                throw new Microsoft.Test.TestValidationException("Test Fail! Binding on e1 does not return correct value...." + e1.Text);
            GlobalLog.LogStatus(e1.Text);
            if(((SolidColorBrush)e3.Background).Color != (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(s_data.Color))
                throw new Microsoft.Test.TestValidationException("Test Fail! Binding on e3 does not return correct value...." + ((SolidColorBrush)e3.Background).Color.ToString());
        
            if (_change != 3)
                throw new Microsoft.Test.TestValidationException("DataTransfer count is wrong..." + _change); 
            DisposeContext();
            GlobalLog.LogStatus("TestCompactSyntax Test Pass!...");
        }
        #endregion TestCompactSyntax
        #region ChangeValue
        /// <summary>
        /// Change source values (one-way binding, DataTransfer event)
        /// </summary>
        void DoChangeSourceValue()
        {
            s_data.String = "new value";    // e1
            s_data.Color = "green";            // e3
        }
        private void OnDataTransfer(object sender, DataTransferEventArgs args)
        {
            _change++;
        }
        #endregion
        #region LoadXml
        /// <summary>
        /// StreamParser is used to LoadXml(xaml).
        /// </summary>
        public UIElement StreamParser(string filename)
        {
            UIElement root = null;

            // see if it loads
            GlobalLog.LogStatus("Parse XAML using Stream..." + filename);
            IXamlTestParser parser = XamlTestParserFactory.Create();
            root = (UIElement)parser.LoadXaml(filename, null); 
            return root;
        }
        #endregion LoadXml
        #region Defined

        readonly SerializationHelper _helper = new SerializationHelper();
        // UiContext defined here
        static TestDataBind s_data = null;
        int _change = 0;
        static Dispatcher s_dispatcher;
        #endregion Defined
        #region filenames
        // predefine the xaml files as strings here for
        // the logging methods second arguments
        string _CDATAFile = "cdatatest.xaml";
        string _DATAFile = "DataBindTest.xaml";
        string _compactSyn = "CompactBinding.xaml";
        #endregion filenames
        #region Context
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;   
        }

        /// <summary>
        /// Disposing Dispatcher here
        /// </summary>
        private void DisposeContext()
        {
        
        }

        /// <summary>
        /// Intercept window messages looking for a close.  If found, stop the dispatcher,
        /// causing the process to stop
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the source window is closed.
            if ((msg == NativeConstants.WM_CLOSE))
            {
                s_dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
            }

            handled = false;
            return IntPtr.Zero;
        }


        #endregion Context
    }
}
