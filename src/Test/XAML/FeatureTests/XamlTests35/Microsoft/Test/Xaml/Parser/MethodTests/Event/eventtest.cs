// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
\***************************************************************************/
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
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Event
{ /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT test that parse XAML for Event testing.
    /// Test cases are:
    ///         - LoadXml with Event
    /// </remarks>
    public class EventTestBVT
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>        
        public void RunTest ()
        {
            string strParams = DriverState.DriverParameters["TestParams"];
            GlobalLog.LogStatus("Core:EventTestBVT Started ..." + "\n");// Start ParserBVT test
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper (System.Globalization.CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "PARSEEVENT":
                    TestEventParser ();
                    break;
                default:
                    GlobalLog.LogStatus("EventTestBVT.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException ("Parameter is not supported");
            }
        }
        #endregion RunTest
        /// <summary>
        ///  
        /// </summary>
        public EventTestBVT()
        {
        }
        #region TestEventParser
        /// <summary>
        /// TestEventParser for EventTestBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(Event) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestEventParser() // Parse through eventtest.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:EventTestBVT.TestEventParser Started ..." + "\n");
            CreateContext ();

            try
            {
                UIElement root = StreamParser(_eventTestXaml);
            }
            catch (XamlParseException exp)
            {
                GlobalLog.LogStatus("TestEventParser PASS! You will need to compile xaml that has eventing first.\n" + exp.ToString());
                return;
            }
            throw new Microsoft.Test.TestValidationException("TestEventParser Fail! Parsing xaml contain eventing without error...");
            
        }
        #endregion TestEventParser

        #region LoadXml
        /// <summary>
        /// StreamParser is used to LoadXml(xaml).
        /// </summary>
        public UIElement StreamParser(string filename)
        {
            UIElement root = null;
            // see if it loads
            GlobalLog.LogStatus("Parse XAML using Stream.");
            IXamlTestParser parser = XamlTestParserFactory.Create();
            root = (UIElement)parser.LoadXaml(filename, null);
            return root;
        }
        #endregion LoadXml
        #region Defined
        // UiContext defined here
        static Dispatcher s_dispatcher;
        #endregion Defined
        #region filenames
        string _eventTestXaml = "eventtest.xaml";
        #endregion filenames
        #region Context
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext ()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;   
        }

        /// <summary>
        /// Disposing Dispatcher here
        /// </summary>
        private void DisposeContext ()
        {

        }
        #endregion Context
    }
}
