// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
\***************************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading; 
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Parser Serialization - SaveAsXml - simple
    /// </summary>
    /// <remarks>
    /// This is Serialize BVT test that parse XAML using Custom Control under \common\parser\customcontrol.cs
    /// and create outerxml using SaveAsXml.
    /// </remarks>
    public class ParserSerialize
    {
        #region ParserSerialize
        /// <summary>
        /// 
        /// </summary>
        public ParserSerialize()
        {
        }
        #endregion ParserSerialize
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
			string strParams = DriverState.DriverParameters["Params"];
			CoreLogger.LogStatus("Core:ParserSerialize Started ..." + "\n"); // Start ParserBVT test
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "SERIALIZE_SIMPLE":
                    TestSerialize ();
                    break;
                default:
                    CoreLogger.LogStatus("ParserSerialize.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest
        #region TestSerialize
        /// <summary>
        /// Parser Serialization - SaveAsXml - simple
        /// scenario:
        ///     - Load serialize.xaml
        ///     - check element count
        ///     - SaveAsXml to create from serialize.xaml root
        /// Verify:
        ///     - TempGetOuterXml.xaml is created.
        ///     - Correct element count.
        ///     - Display Output file.
        ///     - make sure rendering occur.
        /// </summary>
        void TestSerialize() // check for controls persistence
        {
            CoreLogger.LogStatus("Core:PARSERBVT.TestStreamParser Started ..." + "\n");
            CreateContext();
            try
            {

                CreateFiles(_serializeFile);
                if (!_isRenderOccurr)
                    throw new Microsoft.Test.TestValidationException("On Render was not called");
            }
            catch(Exception exp2)
            {
                _exp1 = exp2;
            }
            finally
            {
                DisposeContext();
                if (File.Exists("TempGetOuterXml.xaml"))
                {
                    CoreLogger.LogStatus("Clean up created file...");
                    try
                    {
                        File.Delete("TempGetOuterXml.xaml");
                    }
                    catch (IOException e)
                    {
                        CoreLogger.LogStatus("Unable to delete TempGetOuterXml.xaml\n" + e.ToString());
                    }
                }
                if (_exp1 != null)
                    throw new Microsoft.Test.TestValidationException("Test Fail, catch exception.... " + _exp1.ToString());
                CoreLogger.LogStatus("ParserSerialize Test Pass!...");
            }
        }
        #region files creation
        /// <summary>
        /// TestSerialize
        /// scenario:
        ///     - Load serialize.xaml
        ///     - check element count
        ///     - GetOuterXml to create from serialize.xaml root
        /// </summary>
        /// <param name="filename"></param>
        void CreateFiles(string filename)
        {
            if(File.Exists("TempGetOuterXml.xaml"))
            {
                CoreLogger.LogStatus("Clean up file...");
                try
                {
                    File.Delete("TempGetOuterXml.xaml");
                }
                catch(IOException e)
                {
                    CoreLogger.LogStatus("Unable to delete TempGetOuterXml.xaml\n" + e.ToString());
                }
            }
            // read file to serialize
            CoreLogger.LogStatus("Reading the file..." + filename + "\n");
            Stream stream = File.OpenRead (filename);
            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            UIElement el = (UIElement)System.Windows.Markup.XamlReader.Load (stream, pc);
            // verify we have the right number of elements in the tree
            int elementCount1 = TotalElementCount(el) ;

            CoreLogger.LogStatus("Test starting with the correct number of elements \n" + elementCount1.ToString());
            // Call Parser.GetOutXml to get representation of tree
            //
            Window win = new Window();
            win.Content = el;
            string outer = SerializationHelper.SerializeObjectTree(el);
            if (outer.Length == 0)
            {
                throw new Microsoft.Test.TestValidationException("Failed on XamlWriter.Save on original xaml file");  // Failed on GetOuter
            }
            CoreLogger.LogStatus("Succeeded in calling SaveAsXml on original file"); // PASS
            //Write the newly created string out to a file
            CoreLogger.LogStatus("Writing SaveAsXml string to file - TempGetOuterXml \n");
            // create a new file for comparison
            FileStream fs = new FileStream("TempGetOuterXml.xaml", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            // create a xaml with the expected markup
            sw.Write(outer);
            sw.Close();
            fs.Close();
            CoreLogger.LogStatus("Created temporary file TempGetOuterXml file..." + "\n");
            //Load the newly created file using XamlReader.Load()
            CoreLogger.LogStatus("Reading the newly created file with XamlReader.Load \n");
            Stream stream2 = File.OpenRead ("TempGetOuterXml.xaml");
            UIElement el2 = System.Windows.Markup.XamlReader.Load (stream2, pc) as UIElement;
            // Verify we have two Elements in the tree to complete the round trip.
            int elementCount2 = TotalElementCount(el2) ;

            PanelFlow foo = ((Page)el2).Content as PanelFlow;
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
            foo.RenderCalledEvent += new EventHandler(RenderHandler);
            DisplayXAML(el2);
            CoreLogger.LogStatus("Test SaveAsXml round trip passed.");
        }
        #endregion files creation
        #endregion TestSerialize
        #region TotalElementCount
        /// <summary>
        /// Count Child element from input element.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        int TotalElementCount( DependencyObject e )
        {
            if ( null == e )
                return 0;

            int count = VisualTreeHelper.GetChildrenCount( e );

            int nTotal = count;
            for ( int i = 0; i < count; i++ )
            {
                DependencyObject child = VisualTreeHelper.GetChild( e, i );
                nTotal += TotalElementCount( child );
            }

            return nTotal;
        }
        #endregion TotalElementCount
        #region Defined
        // UiContext defined here
        Exception _exp1 = null;
        HwndSource _source;
        static Dispatcher s_dispatcher;
        bool _isRenderOccurr = false;
        #endregion Defined
        #region filenames
        // predefine the xaml files as strings here for
        // the logging methods second arguments
        string _serializeFile = "serialize.xaml";
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
            if((msg == NativeConstants.WM_CLOSE) )
            {
                s_dispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                
            }
            handled = false;
            return IntPtr.Zero;
        }

        #endregion Context
        #region DisplayXAML
        /// <summary>
        /// Display XAML in HWNDSRC to use for verify OnRender Event.
        /// and Close Dispatcher when OnRender is called.
        /// </summary>
        /// <param name="root"></param>
        void DisplayXAML(UIElement root)
        {
            CoreLogger.LogStatus("Load XAML in Window.");
            _source = SourceHelper.CreateHwndSource(800, 700, 0,0);
            _source.AddHook(new HwndSourceHook(ApplicationFilterMessage));
            _source.RootVisual = root;
            CoreLogger.LogStatus("Dispatcher Run.");
            Dispatcher.Run();
        }
        /// <summary>
        /// EventHandler action when OnRender is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RenderHandler(object sender, EventArgs e)
        {
            _isRenderOccurr = true;
            NativeMethods.PostMessage(new HandleRef(null,_source.Handle),NativeConstants.WM_CLOSE,IntPtr.Zero,IntPtr.Zero);
        }
        #endregion DisplayXAML
    }
}

