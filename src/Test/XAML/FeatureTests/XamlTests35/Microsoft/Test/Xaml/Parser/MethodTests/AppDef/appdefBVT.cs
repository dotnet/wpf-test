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
using System.Windows.Navigation;
using System.Windows.Markup;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.AppDef
{ /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT test that parse XAML for AppDef testing.
    /// Test cases are:
    ///         - LoadXml with AppDef file.
    ///         - LoadXml with AppDef + Resource.
    ///         - Loading external styles + resources
    /// </remarks>
    public class AppDefBVT
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest ()
        {
            string strParams = DriverState.DriverParameters["TestParams"];
            if (String.IsNullOrEmpty(strParams))
                throw new InvalidOperationException("DriverState.DriverParameters[Params] cannot be null");
            GlobalLog.LogStatus("Core:AppDefBVT Started ..." + "\n");// Start ParserBVT test
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper (System.Globalization.CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "PARSEAPPDEF":
                    TestAppDefParser ();
                    break;
                case "PARSEAPPDEFRES":
                    TestAppDefResParser();
                    break;
                case "EXTERNAL":
                    TestResExternalParser();
                    break;
                default:
                    GlobalLog.LogStatus("AppDefBVT.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException ("Parameter is not supported");
            }
        }
        #endregion RunTest
        /// <summary>
        ///  
        /// </summary>
        public AppDefBVT()
        {
        }
        #region TestAppDefParser
        /// <summary>
        /// TestAppDefParser for AppDefBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(AppDef) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestAppDefParser() // Parse through ParserApp.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:AppDefBVT.TestAppDefParser Started ..." + "\n");
            GlobalLog.LogStatus("Parsing AppDef file...");

            LoadFileUsingWpfParser(_appDefTestXaml);
            
            GlobalLog.LogStatus("Verifying AppDef parsed...");
            s_na.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedFired);  
            s_na.Run();
            GlobalLog.LogStatus("TestAppDefParser PASS!!!");
        }

        /// <summary>
        /// LoadFile Using Wpf Parser
        /// </summary>
        private void LoadFileUsingWpfParser(string XamlFileName)
        {
            Stream stream = File.OpenRead(XamlFileName);
            try
            {
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                s_na = (Application)XamlReader.Load(stream, pc);
            }
            finally
            {
                stream.Close();
            }
        }

        #endregion TestAppDefParser
        #region TestAppDefResParser
        /// <summary>
        /// TestAppDefResParser for AppDefBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(AppDef + Resource) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestAppDefResParser() // Parse through parserresapp.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:AppDefBVT.TestAppDefResParser Started ..." + "\n");
            GlobalLog.LogStatus("Parsing AppDef file...");

            LoadFileUsingWpfParser(_appDefResTestXaml);

            GlobalLog.LogStatus("Verifying AppDef parsed...");
            ResourceDictionary resources = s_na.Resources;
            if(resources.Count != 3)
            {
                throw new Microsoft.Test.TestValidationException("FAIL!!! App Resource count incorrect.");
            }
            else
            {
                // Verify Red Color.
                System.Windows.Media.Color red = ((SolidColorBrush)resources["RedBrush"]).Color;

                if(!(red.R == 255 && red.G == 0 && red.B == 0))
                {
                    throw new Microsoft.Test.TestValidationException("FAIL!!!! 'RedBrush' resource had unexpected value at App level.  " + "'R' = " + red.R + " 'G' = " + red.G + " 'B' = " + red.B);
                }

                // Verify Green Color.
                System.Windows.Media.Color green = ((SolidColorBrush)resources["GreenBrush"]).Color;

                if(!(green.R == 0 && green.G == 255 && green.B == 0))
                {
                    throw new Microsoft.Test.TestValidationException("FAIL!!! 'GreenBrush' resource had unexpected value at App level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
                }

                // Verify Blue Color.  This is not really Blue at level, this is overridden
                // by the Page to be true blue color.
                System.Windows.Media.Color blue = ((SolidColorBrush)resources["BlueBrush"]).Color;

                if(!(blue.R == 128 && blue.G == 128 && blue.B == 0))
                {
                    throw new Microsoft.Test.TestValidationException("FAIL!!! 'BlueBrush' resource had unexpected value at App level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
                }
            }
            GlobalLog.LogStatus("Run Navigation Application...");
            s_na.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedFired);
            s_na.Run();
            GlobalLog.LogStatus("TestAppDefResParser PASS!!!");
        }
        #endregion TestAppDefResParser
        #region TestResExternalParser
        /// <summary>
        /// TestResExternalParser for AppDefBVT case
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(AppDef + Resource) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestResExternalParser() // Parse through AppResApp.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:AppDefBVT.TestResExternalParser Started ..." + "\n");
            GlobalLog.LogStatus("Parsing AppDef file...");

            LoadFileUsingWpfParser(_appDefResXaml);

            GlobalLog.LogStatus("Verifying AppDef parsed...");
            VerifyAppResources();
            // Invoke the parser to parse the page markup file.
            
				s_na.Dispatcher.Invoke(DispatcherPriority.Normal,(DispatcherOperationCallback)
					delegate(object NotUsed)
				{                    
                    IXamlTestParser parser2 = XamlTestParserFactory.Create();
                    Page rootPage = (Page)parser2.LoadXaml(_appDefPageXaml, null);
   
                    if (null == rootPage)
                        throw new Microsoft.Test.TestSetupException("Page cast filed");

                    s_dockPanel = (DockPanel)rootPage.Content;
                    rootPage.Content = null;
                    return null;
				}, null);
			
            
				s_na.Dispatcher.Invoke(DispatcherPriority.Normal, (DispatcherOperationCallback)
				delegate(object NotUsed)
				{

					// The DockPanel is not yet parented to a Window and hence
					// should not fall back to the App level resources.
					GlobalLog.LogStatus("Verifying Page Resource parse...");
					VerifyPageResources(false);
					// Add the DockPanel as a child of Window and verify that it
					// now sees the App level resources.
					GlobalLog.LogStatus("Add the DockPanel as a child of Window...");
					Window window = new Window();
					window.Content = s_dockPanel;
					GlobalLog.LogStatus("Verifying Page Resource...");
					VerifyPageResources(true);
					GlobalLog.LogStatus("TestResExternalParser PASS!!!");

					return null;
				}, null);

	}
        /// <summary>
        /// 
        /// </summary>
        private static void VerifyAppResources()
        {
            ResourceDictionary resources = s_na.Resources;

            if(resources.Count != 4)
            {
                throw new Microsoft.Test.TestValidationException("App Resource count incorrect.");
            }
            else
            {
                // Verify Red Color.
                System.Windows.Media.Color red = ((SolidColorBrush)resources["RedBrush"]).Color;

                if(!(red.R == 255 && red.G == 0 && red.B == 0))
                {
                    throw new Microsoft.Test.TestValidationException("FAIL!!!! 'RedBrush' resource had unexpected value at App level.  " + "'R' = " + red.R + " 'G' = " + red.G + " 'B' = " + red.B);
                }
                // Verify Green Color.
                System.Windows.Media.Color green = ((SolidColorBrush)resources["GreenBrush"]).Color;

                if(!(green.R == 0 && green.G == 255 && green.B == 0))
                {
                    throw new Microsoft.Test.TestValidationException("FAIL!!! 'GreenBrush' resource had unexpected value at App level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
                }
                // Verify Blue Color.  This is not really Blue at level, this is overridden
                // by the Page to be true blue color.
                System.Windows.Media.Color blue = ((SolidColorBrush)resources["BlueBrush"]).Color;
                if(!(blue.R == 128 && blue.G == 128 && blue.B == 0))
                {
                    throw new Microsoft.Test.TestValidationException("FAIL!!! 'BlueBrush' resource had unexpected value at App level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bRedBrush"></param>
        private static void VerifyPageResources(bool bRedBrush)
        {
            IEnumerator ienum = LogicalTreeHelper.GetChildren(s_dockPanel).GetEnumerator();
            System.Windows.Media.Color green, blue, red, yellow, limegreen;
            green = blue = red = yellow = limegreen = new System.Windows.Media.Color();
            while(ienum.MoveNext())
            {
                string id = ((Button)ienum.Current).Name;

                switch(id)
                {
                    case "green":
                        green = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "blue":
                        blue = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;

                    case "red":
                        if (!bRedBrush)
                        {
                        }
                        else
                        {
                            red = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        }
                        break;

                    case "yellow":
                        yellow = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        break;
                    case "limegreen":
                        if (!bRedBrush)
                        {
                        }
                        else
                        {
                            limegreen = ((SolidColorBrush)((Button)ienum.Current).Background).Color;
                        }
                        break;
                    default:
                        throw new Microsoft.Test.TestSetupException("Unexpected element Name encountered - " + id);
                }
            }

            // Verify Green Color.
            if(!(green.R == 0 && green.G == 255 && green.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("FAIL!!! 'GreenBrush' resource had unexpected value at Page level.  " + "'R' = " + green.R + " 'G' = " + green.G + " 'B' = " + green.B);
            }

            // Verify Blue Color.
            if(!(blue.R == 0 && blue.G == 0 && blue.B == 255))
            {
                throw new Microsoft.Test.TestValidationException("FAIL!!! 'BlueBrush' resource had unexpected value at Page level.  " + "'R' = " + blue.R + " 'G' = " + blue.G + " 'B' = " + blue.B);
            }

            // Verify Red Color.
            if(bRedBrush && !(red.R == 255 && red.G == 0 && red.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("FAIL!!!! 'RedBrush' resource had unexpected value at Page level.  " + "'R' = " + red.R + " 'G' = " + red.G + " 'B' = " + red.B);
            }

            // Verify Yellow Color.
            if(!(yellow.R == 255 && yellow.G == 255 && yellow.B == 0))
            {
                throw new Microsoft.Test.TestValidationException("FAIL!!! 'YellowBrush' resource had unexpected value at Page level.  " + "'R' = " + yellow.R + " 'G' = " + yellow.G + " 'B' = " + yellow.B);
            }
            // Verify LimeGreen Color.
            if(bRedBrush && !(limegreen.ToString().Equals("#FF32CD32")))
            {
                throw new Microsoft.Test.TestValidationException(limegreen.ToString() + " FAIL!!! 'LimeGreen' resource had unexpected value at Page level.  " + "'R' = " + limegreen.R + " 'G' = " + limegreen.G + " 'B' = " + limegreen.B);
            }
        }
        #endregion TestResExternalParser
        #region Event QuitApp
        /// <summary>
        /// Close Application When is Idle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLoadCompletedFired(object sender, NavigationEventArgs e)
        {
            GlobalLog.LogStatus("INSIDE EVENT");
            System.Windows.Application.Current.Windows[0].Close();
            System.Windows.Application.Current.Shutdown();
            s_na.Shutdown();
        }
        #endregion Event QuitApp
        #region Defined
        static DockPanel s_dockPanel;
        static Application s_na;
        #endregion Defined
        #region filenames
        string _appDefTestXaml = "ParserApp.xaml";
        string _appDefResTestXaml = "parserresapp.xaml";
        string _appDefResXaml = "AppResApp.xaml";
        string _appDefPageXaml = "AppResPage.xaml";
        #endregion filenames
    }
}
