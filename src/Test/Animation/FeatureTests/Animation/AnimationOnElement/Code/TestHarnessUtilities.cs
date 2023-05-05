// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// $Id:$ $Change:$
using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting; //Needed for Activator.CreateInstance
using System.Threading; using System.Windows.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Documents;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    /// <summary>
    /// General useful utilities container
    /// </summary>
    public class IntegrationUtilities
    {

        /// <summary>
        /// Gets command line arguments, or equivalents
        /// </summary>
        public static string[] GetCommandLineArgs()
        {
            return TrustedHelper.GetCommandLineArgs();
        }

        /// <summary>
        /// Does a navigation to the given page
        /// </summary>
        public static void NavigateToPage( string uri )
        {
       //     System.Windows.Navigation.NavigationWindow NavWin =
        //        (System.Windows.Navigation.NavigationWindow) Application.Current.MainWindow;
        //    NavWin.Navigate( new Uri(uri , UriKind.RelativeOrAbsolute) );
        }

        /// <summary>
        /// Does a navigation to the given pageFunction object
        /// </summary>
//        public static void NavigateToPageObject( object pageFunctionObject )
//        {
//            System.Windows.Navigation.NavigationWindow NavWin =
//                (System.Windows.Navigation.NavigationWindow) Application.Current.MainWindow;
//            NavWin.Navigate( pageFunctionObject as FrameworkElement);
//        }

        /// <summary>
        /// Does a navigation to the given page through the proxy
        /// </summary>
//        public static void NavigateToPageUsingProxy( string uri )
//        {
              // Set Proxy Page Property
//            AddApplicationProperty(CommonConstants.flagProxyTarget, uri);
              // Navigate to ProxyPage ...
//            System.Windows.Navigation.NavigationWindow NavWin =
//                (System.Windows.Navigation.NavigationWindow) Application.Current.MainWindow;
//            NavWin.Navigate( new Uri(CommonConstants.proxyPage, UriKind.RelativeOrAbsolute) );
//        }

        /// <summary>
        /// Does a navigation to the set proxy page given page
        /// </summary>
//        public static void ProxyNavigateToPage()
//        {
//            Application thisApp =
//                (System.Windows.Application) Application.Current;
//            string currentValue =
//                (null == thisApp)? null : thisApp.Properties[CommonConstants.flagProxyTarget] as string;
//            IntegrationUtilities.NavigateToPage( currentValue );
//        }

        /// <summary>
        /// Does Initial parsing and navigation
        /// </summary>
        public static void BeginCommonTest()
        {
            // Get command line arguments
            //LogHelper.LogTestMessage("### Exploring Command Line Arguments ###");
             if( ParseArguments( IntegrationUtilities.GetCommandLineArgs() ) == true ) return;
        }

        /// <summary>
        /// Parses known arguments from a string of parameters
        /// </summary>
        private static bool ParseArguments( string[] arguments )
        {
            // if there is an argument
            if ( arguments != null )
            {
                string navigationTarget = null;
                string repetitions = null;
                string loopURL = null;
                string pageFunction = null;

                // parse command line arguments
                for( int i=0; i<arguments.Length; i++ )
                {
                    // all arguments must start with '/'
                    if ( !arguments[i].StartsWith("/") )
                    {
                        //LogHelper.LogTestMessage("### Ignoring invalid switch: " + arguments[i] );
                        continue;
                    }
                    // remove " if any ...
                    arguments[i] = arguments[i].Replace("\"",null);
                    // arguments may have parameters, use '=' as a separator
                    string[] currentArgument = arguments[i].Split('=');
                    // parse arguments as needed
                    switch( currentArgument[0].ToLower() )
                    {
                        case "/target":
                        case "/t":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            navigationTarget = currentArgument[1];
                            break;

                        case "/repeat":
                        case "/r":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            repetitions = currentArgument[1];
                            break;

                        case "/loop":
                        case "/l":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            loopURL = DynamicContent.GetRandomTestURL();
                            break;

                        case "/pf":
                        case "/pagefunction":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            pageFunction = currentArgument[1];
                            break;

                        case "/te":
                        case "/testelement":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            AddApplicationProperty( CommonConstants.flagTestElement, currentArgument[1]);
                            break;

                        case "/tv":
                        case "/testvariation":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            AddApplicationProperty( CommonConstants.flagTestVariation, currentArgument[1]);
                            break;

                        case "/f":
                        case "/filter":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            AddApplicationProperty( CommonConstants.flagFilterString, currentArgument[1]);
                            break;

                        case "/k":
                        case "/kill":
                            //LogHelper.LogTestMessage("### Parsing switch: " + arguments[i] );
                            AddApplicationProperty( CommonConstants.flagKill, "TRUE" );
                            break;

                        default:
                            //LogHelper.LogTestMessage("### Ignoring switch: " + arguments[i] );
                            break;
                    }
                }

                // process command line arguments
                // set repeat count:
                if ( null != repetitions )
                {
                    AddApplicationProperty(CommonConstants.flagRepetitions, repetitions );
                }
                // page function override
//                if ( null != pageFunction )
//                {
//                    // create pagefunction object
//                    NavigateToPageObject( CreatePageFunctionObject( pageFunction ));
//                    return true;
//                }
                // navigate to given page:
                if ( null != navigationTarget )
                {
                    AddApplicationProperty(CommonConstants.flagTarget, navigationTarget );
                    NavigateToPage( navigationTarget );
                    return true;
                }
                // randomly navigate in an endless loop
                if ( null != loopURL )
                {
                    AddApplicationProperty(CommonConstants.flagLoop, loopURL );
                    NavigateToPage( loopURL );
                    return true;
                }
            }
            return false;
        }

        #if TODO_WPP_BLOCKED
        //
        #endif // Need PageFunction overloads or subclasses
        private static PageFunction<string> CreatePageFunctionObject( string objectName )
        {
            ObjectHandle handle = Activator.CreateInstance( null, objectName );
            return (PageFunction<string>) handle.Unwrap();
        }

        /// <summary>
        /// Decides what to do on test end
        /// </summary>
        public static void EndCurrentTest()
        {
//            Application.Current.Shutdown();
        }
        /// <summary>
        /// Decides what to do on test end
        /// </summary>
        public static void EndCurrentTest( string finalMessage )
        {
            //LogHelper.LogTestMessage("### Ending Test ...");
            // Get a reference to a navigation app
            if (null != AnimationOnElementTest.applicationProps)
            {
                // get repetitions
                string repetitions = AnimationOnElementTest.applicationProps.Get(CommonConstants.flagRepetitions) as string;
                // get navigation target
                string target = AnimationOnElementTest.applicationProps.Get(CommonConstants.flagTarget) as string;
                // get looping behavior
                string loopURL = AnimationOnElementTest.applicationProps.Get(CommonConstants.flagLoop) as string;

                // check for endless loop mode
                if ( loopURL != null )
                {
                    //LogHelper.LogTestMessage("####################################################");
                    //LogHelper.LogTestMessage("### Completed test: " + loopURL );
                    loopURL = DynamicContent.GetRandomTestURL();
                    string randomElement = DynamicContent.GetRandomElement();
                    AddApplicationProperty( CommonConstants.flagTestElement, randomElement);
                    //LogHelper.LogTestMessage(String.Format("### New random target test: {0} ({1})", loopURL, randomElement ));
                    // add new URL for next test
                    AddApplicationProperty(CommonConstants.flagLoop, loopURL );
                    // navigate and exit
                    NavigateToPage( loopURL );
                    return;
                }

                // Check for repetitions
                if ( repetitions != null && target != null )
                {
                    // decrement repetition counter
                    int repetitionsLeft = Int32.Parse(repetitions);
                    if ( repetitionsLeft > 0 )
                    {
                        AddApplicationProperty(CommonConstants.flagRepetitions, (repetitionsLeft-1).ToString() );
                        //LogHelper.LogTestMessage("### Repeat Navigation to test: " + target + "   " + (repetitionsLeft-1).ToString() + " repetition(s) left." );
                        NavigateToPage( target );
                        return;
                    }
                }
            }

            // Log The Final result to the Framework
            //LogHelper.LogTestMessage("### End Test: Logging final result ...");
            //LogHelper.LogTestToFramework( finalMessage );

            // Close the test case window if it is running in the lab automation framework
            // or we specifically ask it to
            string killSwitch = AnimationOnElementTest.applicationProps.Get(CommonConstants.flagKill) as string;
            if ( killSwitch != null )
            {
                //LogHelper.LogTestMessage("### End Test: Closing the window ...");
//                Application.Current.MainWindow.Close();
            }
        }

        /// <summary>
        /// Adds a string property to the navigation app
        /// </summary>
        public static void AddApplicationProperty( string name, string newValue )
        {
            string currentValue = null;

            if ( null != AnimationOnElementTest.applicationProps )
            {
                currentValue = AnimationOnElementTest.applicationProps.Get( name ) as string;
            }

            if ( null == currentValue )
            {
                AnimationOnElementTest.applicationProps.Add( name, newValue );
            }
            else
            {
                AnimationOnElementTest.applicationProps.Set( name, newValue );
            }
        }

        /// <summary>
        /// Gets a string property to the navigation app
        /// </summary>
        public static string GetApplicationProperty( string name )
        {
            string argValue = null;
            argValue = AnimationOnElementTest.applicationProps.Get( name ) as string;
            return argValue;
        }

        /// <summary>
        /// Gets the name of the element's type
        /// </summary>
        public static string GetElementTypeName( UIElement e )
        {
            return e.GetType().Name;
        }
        /// <summary>
        /// Gets a standard version of any given element
        /// </summary>
        public static DependencyObject[] GetStockElementAndParent( string elementFile, string elementTag )
        {
            // create the element
            DependencyObject[] stockElements = new DependencyObject[2];
            try
            {
                // get the element factory and extract its xaml
                ElementFactory elmFactory = new ElementFactory( elementFile );

                // create the element using the parser
                stockElements[0] = elmFactory.GetLogicalElement(elementTag);
                // get parent element
                stockElements[1] = elmFactory.TargetElementRoot;
                //Special case for tooltip and contextmenu
                if(stockElements[0] == null && stockElements[1] != null)
                {
                    switch(elementTag.ToUpper())
                    {
                        case "TOOLTIP":
                            Button button = stockElements[1] as Button;
                            stockElements[0] = button.ToolTip as DependencyObject;
                            break;
                        case "CONTEXTMENU":
                            Button buttonCM = stockElements[1] as Button;
                            stockElements[0] = buttonCM.ContextMenu;
                            break;
                        case "HEADER":
                            Table headerTable = stockElements[1] as Table;
                            stockElements[0] = headerTable.RowGroups[0] as DependencyObject;
                            break;
                        case "FOOTER":
                            Table footerTable = stockElements[1] as Table;
                            if (footerTable.RowGroups.Count > 0)
                            {
                                stockElements[0] = footerTable.RowGroups[footerTable.RowGroups.Count - 1];
                            }
                            break;
                        case "BODY":
                            Table bodyTable = stockElements[1] as Table;
                            if (bodyTable.RowGroups.Count > 1)
                            {
                                stockElements[0] = bodyTable.RowGroups[1];
                            }
                            break;
                        case "COLUMN":
                            Table colTable = stockElements[1] as Table;
                            TableColumnCollection columns = colTable.Columns;
                            if (columns.Count >=1)
                            {
                                stockElements[0] =  columns[0];
                            }
                            else
                            {
                                //LogHelper.LogTestMessage("ERROR: Table has 0 Columns");
                            }
                            break;
                        case "ROW":
                            Table rowTable = stockElements[1] as Table;
                            TableRowGroup header = rowTable.RowGroups[0];
                            TableRowCollection rows = header.Rows;
                            if(rows.Count >=1)
                            {
                                stockElements[0] =  rows[0];
                            }
                            else
                            {
                                //LogHelper.LogTestMessage("ERROR: Table Header has 0 Rows");
                            }
                            break;
                    }
                }
            }
            catch( System.Exception ex )
            {
                LogHelper.LogTestMessage( ex.ToString() );
            }

            // return built element or null for default
            return stockElements;
        }

        /// <summary>
        /// Return value for the named argument
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetArgumentValueByName(string name)
        {
            return GetApplicationProperty( name );
        }
        /// <summary>
        /// Moves and resizes the window so that it becomes a default size
        /// </summary>
        /// <param name="SizeX">X dimension of the window</param>
        /// <param name="SizeY">Y dimension of the window</param>
/*
        public static void SetDefaultWindow(int SizeX, int SizeY)
        {
            // Use the App instance to move stuff to correct position
            // This is essential for using PAW to control the UI ...
            Application app = AnimationOnElementTest.applicationCurrent;
            Window window = app.Windows[0];

            Double xLength = SizeX;
            Double yLength = SizeY;
            window.Width   = xLength;
            window.Height  = yLength;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            Double leftTopLength = 0;
            window.Top = leftTopLength;
            window.Left = leftTopLength;
        }
*/
        /// <summary>
        /// Moves and resizes the window so that it becomes a default size
        /// Fallback for default settings in TestCases
        /// </summary>
/*
        public static void SetDefaultWindow()
        {
            SetDefaultWindow(800,600);
        }
*/
        /// <summary>
        /// Converts a string to a stream, using UTF-8 encoding
        /// </summary>
        public static System.IO.Stream GetStreamFromString( string text )
        {
            Encoding encoding = new UTF8Encoding(false, true);
            return new System.IO.MemoryStream( encoding.GetBytes(text) );
        }

    }

}

