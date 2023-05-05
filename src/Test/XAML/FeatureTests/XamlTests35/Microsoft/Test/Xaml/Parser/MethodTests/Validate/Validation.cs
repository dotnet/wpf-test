// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.IO.Packaging;
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
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Validate
{
    /// <summary>
    /// XamlParseException Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT Validation test that parse XAML using Invalid XAML format to test parsing validation.
    /// Test cases are:
    ///         - Parse XAML with Tag misplaced XAML format.
    ///         - Parse XAML with No closing tag XAML format.
    ///         - Parse XAML with missing quotes XAML format.
    ///         - Parse XAML with No opening tag XAML format.
    ///         - Parse XAML with No namespace XAML format.
    ///         - Parse XAML with wrong namespace XAML format.
    ///         - Parse XAML with misspell namespace XAML format.
    ///         - Parse XAML with misspell property XAML format.
    ///         - Parse XAML with misspell tag XAML format.
    ///         - Parse XAML with wrong comment position XAML format.
    ///         - Parse XAML with space XAML format.
    ///         - Parse XAML with invalid XAML to get Line Position and Line Number in Exception.
    ///         - Recovery from XamlParseException by re parsing valid xaml again.
    ///         - Parse XAML with invalid Dependency Property.
    ///         - Parse XAML with invalid Clr Property.
    ///         - Parse XAML with unknown tag in a style
    ///         - Parse XAML with unknown attribute in a style
    /// </remarks>
   public class Validation
	{
        /// <summary>
        /// test
        /// </summary>
        public Validation()
        {
        }
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
    
            string strParams = DriverState.DriverParameters["TestParams"];

            GlobalLog.LogStatus("Core:Validation Started ..." + "\n"); // Start ParserBVT test
            
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

            switch (strParams)
            {
                case "TAGMISPLACED":
                    TestTagMisplaced();
                    break;
                case "TAGNOTCLOSED":
                    TestTagNotClosed();
                    break;
                case "QUOTESMISSING":
                    TestQuotesMissing();
                    break;
                case "TAGNOTOPENED":
                    TestTagNotOpened();
                    break;
                case "NONAMESPACE":
                    TestNoNamespace();
                    break;
                case "WRONGNAMESPACE":
                    TestWrongNamespace();
                    break;
                case "MISPELLNAMESPACE":
                    TestMispellNamespace();
                    break;
                case "MISPELLPROP":
                    TestMispellProp();
                    break;
                case "MISPELLTAG":
                    TestMispellTag();
                    break;
                case "UNKNOWN_TAG_IN_STYLE":
                    TestUnknownTagInStyle("UnknownTagInStyle.xaml");
                    break;
                case "UNKNOWN_TAG_IN_STYLE2":
                    TestUnknownTagInStyle("UnknownTagInStyle2.xaml");
                    break;
                case "UNKNOWN_ATTRIBUTE_IN_STYLE":
                    TestUnknownAttributeInStyle("UnknownAttributeInStyle.xaml");
                    break;    
                case "UNKNOWN_ATTRIBUTE_IN_STYLE2":
                    TestUnknownAttributeInStyle("UnknownAttributeInStyle2.xaml");
                    break;
                case "COMMENTSWRONG":
                    TestCommentsWrong ();
                    break;
                case "SPACE":
                    TestSpace ();
                    break;
                case "RECOVERY":
                    TestRecovery ();
                    break;
                case "DEPENDENCY":
                    TestDependencyProperty();
                    break;
                case "CLR":
                    TestClrProperty();
                    break;
                default:
                    GlobalLog.LogStatus("Validation.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest
        #region Run the tests
        /// <summary>
        /// Loading XAML in stream and Parsing XAML 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>UIElement</returns>
		UIElement RunTests(string filename)
		{
            GlobalLog.LogStatus("Parsing File...." + filename);
            IXamlTestParser parser = XamlTestParserFactory.Create();
            Page rootPage = parser.LoadXaml(filename, null) as Page;
            if (null == rootPage)
                throw new Microsoft.Test.TestSetupException("Page cast filed");
            UIElement elt = rootPage.Content as UIElement;

            return elt;

		}
        
        #endregion Run the tests
	    #region BVT cases
		#region Tag Misplaced
        /// <summary>
        /// Parse XAML with Tag misplaced XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestTagMisplaced() // Validate XAML: tag is misplaced
		{
			try
			{
				RunTests(_tmisplaced);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestTagMisplaced: Did not throw Parser Exception."); // FAILURE	
		}
		#endregion Tag Misplaced
		#region Tag not closed
        /// <summary>
        /// Parse XAML with No closing tag XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestTagNotClosed() // Validate XAML: a tag is not closed
		{
			try
			{
				RunTests(_tnclosed);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestTagNotClosed: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Tag not closed
		#region Quotes missing
        /// <summary>
        /// Parse XAML with missing quotes XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestQuotesMissing() // Validate XAML: some quotes are missing
		{
			try
			{
				RunTests(_quotesMiss);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestQuotesMissing: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Quotes missing
		#region Tag not opened
        /// <summary>
        /// Parse XAML with No opening tag XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestTagNotOpened() // Validate XAML: a tag is not opened
		{
			try
			{
				RunTests(_tnopened);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestTagNotOpened: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Tag not opened
		#region No namespace
        /// <summary>
        /// Parse XAML with No namespace XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestNoNamespace() // Validate XAML: no namespace referenced
		{
			try
			{
				RunTests(_nonamespace);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestNoNamespace: Did not throw Parser Exception."); // FAILURE
		}
		#endregion No namespace
		#region Wrong namespace
        /// <summary>
        /// Parse XAML with wrong namespace XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestWrongNamespace() // Validate XAML: bogus namespace
		{
			try
			{
				RunTests(_wnamespace);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestWrongNamespace: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Wrong namespace
		#region Misspelled namespace
        /// <summary>
        /// Parse XAML with misspell namespace XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestMispellNamespace() // Validate XAML: namespace is mispelled
		{
			try
			{
				RunTests(_mispnamespace);

			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestMispellNamespace: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Mispelled namespace	
		#region Misspelled property
        /// <summary>
        /// Parse XAML with misspell property XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestMispellProp() // Validate XAML: a property is mispelled
		{
			try
			{
				RunTests(_mispellprop);

			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestMispellProp: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Misspelled property
        #region Unknown tag in style
        /// <summary>
        /// Parse XAML with unknown tag in a style.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is thrown.
        /// </summary>
        /// <param name="filename">Filename to be parsed</param>
        void TestUnknownTagInStyle(string filename) 
        {
            try
            {
                RunTests(filename);
            }
            catch (System.Windows.Markup.XamlParseException exp)		// PASS
            {
                LogParserException(exp);
                return;
            }
            throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestUnknownTagInStyle: Did not throw Parser Exception."); // FAILURE
        }
		#endregion Unknown tag in style
        #region Unknown attribute in style
        /// <summary>
        /// Parse XAML with unknown attribute in a style.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is thrown.
        /// </summary>
        /// <param name="filename">Filename to be parsed</param>
        void TestUnknownAttributeInStyle(string filename)
        {
            try
            {
                RunTests(filename);
            }
            catch (System.Windows.Markup.XamlParseException exp)		// PASS
            {
                LogParserException(exp);
                return;
            }
            throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestUnknownAttributeInStyle: Did not throw Parser Exception."); // FAILURE
        }
		#endregion Unknown attribute in style
		#region Misspelled tag
        /// <summary>
        /// Parse XAML with misspell tag XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
		void TestMispellTag() // Validate XAML: a tag is mispelled
		{
			try
			{
				RunTests(_mispelltag);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
                return;
			}
			throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestMispellTag: Did not throw Parser Exception."); // FAILURE
		}
		#endregion Misspelled tag
        #region Comments wrong
        /// <summary>
        /// Parse XAML with wrong comment position XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
        void TestCommentsWrong() // Validate XAML: comments are placed inside a tag
        {
            try
            {
                RunTests(_commentsw);
            }
            catch(System.Windows.Markup.XamlParseException exp)		// PASS
            {
                LogParserException(exp);
                return;
            }
            throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestCommentsWrong: Did not throw Parser Exception."); // FAILURE
        }
        #endregion Comments wrong
        #region Space
        /// <summary>
        /// Parse XAML with space XAML format.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XAMLParseException is throw.
        /// </summary>
        void TestSpace() // Validate XAML: empty XAML file with space added
        {
            try
            {
                RunTests(_space);
            }
            catch(System.Windows.Markup.XamlParseException exp)		// PASS
            {
                LogParserException(exp);
                return;
            }
            throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestSpace: Did not throw Parser Exception."); // FAILURE
        }
        #endregion Space
		#region recovery from parser exception
        /// <summary>
        /// Recovery from XamlParseException by re parsing valid xaml again.
        /// Recovery: exception should not prevent parser from going on
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        ///     - Catch Exception
        /// Verify:
        ///     - XAMLParseException is throw.
        ///     - Verify Exception LineNumber and LinePosition
        /// </summary>
		void TestRecovery() // Recovery: exception should not prevent parser from going on
		{
			try
			{
				RunTests(_nonamespace);
			}
			catch(System.Windows.Markup.XamlParseException exp)		// PASS
			{
				LogParserException(exp);
				// We found the exception
				GlobalLog.LogStatus("Good, we expected the exception, now let's try to go on..." + "\n");
			}
			try
			{
                GlobalLog.LogStatus("Now Parsing..." + _customControl + "\n");
				// See if parser can load a good xaml file now
				RunTests(_customControl);
			}
			// variable exp never used??
			catch (Exception exp)
			{
				GlobalLog.LogStatus("Cough, cough... Can't load a good xaml file..." + "\n");
				LogParserException(exp);
				throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestRecovery: Exception occur during parsing Good XAML."); //FAIL
			}
			GlobalLog.LogStatus("Good, Test Pass...." + "\n"); // PASS
		}
		#endregion recovery from parse exception
        #region Dependency
        /// <summary>
        /// Parse XAML with invalid Dependency Property.
        /// Catch the correct exception
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XamlParseException is throw.
	/// </summary>
        void TestDependencyProperty () // Catch the correct line and position number for the exception
        {
            try
            {
                RunTests(_dependencyXAML);
            }
            catch (System.Windows.Markup.XamlParseException exp)
            {
                // 



                GlobalLog.LogStatus("Test Pass!");
        		LogParserException(exp);
                return;  
            }
            throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestDependencyProperty: Did not throw Parser Exception."); // FAILURE
        }
        #endregion Dependency
        #region Clr
        /// <summary>
        /// Parse XAML with invalid Clr Property.
        /// Catch the correct exception.
        /// Scenario:
        ///     - Open XAML in Stream
        ///     - Parse XAML
        /// Verify:
        ///     - XamlParseException is throw.
	/// </summary>
        void TestClrProperty () // Catch the correct line and position number for the exception
        {
            try
            {
                RunTests(_clrXAML);
            }
            catch (System.Windows.Markup.XamlParseException exp)
            {
                GlobalLog.LogStatus("Test Pass!");
        		LogParserException(exp);
                return;  
            }
            throw new Microsoft.Test.TestValidationException("FAIL: Validation.TestClrProperty: Did not throw Parser Exception."); // FAILURE
        }
        #endregion Clr
	#endregion BVT cases
        #region Defined
        // UiContext defined here
        static Dispatcher s_dispatcher;
        #endregion Defined
        #region filenames
        
        // predefine the xaml files as strings here for
        // the logging methods second arguments
        string _tmisplaced = "tagmisplaced.xaml";
        string _tnclosed = "tagnotclosed.xaml";
        string _tnopened = "tagnotopened.xaml";
        string _nonamespace = "nonamespace.xaml";
        string _mispnamespace = "mispellnamespace.xaml";
        string _wnamespace = "wrongnamespace.xaml";
        string _mispellprop = "mispellprop.xaml";
        string _mispelltag = "mispelltag.xaml";
        string _commentsw = "commentswrong.xaml";
        string _space = "space.xaml";
        string _quotesMiss = "quotesmissing.xaml";
        string _customControl = "CustomControl.xaml";
        string _dependencyXAML = "DependencyPropertyException.xaml";
        string _clrXAML = "ClrPropertyException.xaml";

        #endregion filenames
        #region Context
        /// <summary>
        /// Creating Dispatcher
        /// </summary>
        private void CreateContext()
        {
            s_dispatcher = Dispatcher.CurrentDispatcher;
        }
        #endregion Context
        #region Logging
        /// <summary>
        /// Log Logging exception for XamlParseException test.
        /// </summary>
        /// <param name="exp"></param>
        void LogParserException(Exception exp)
        {
            // log the exception using frm.Status
            GlobalLog.LogStatus("EXCEPTION: " + exp.Message + "\n");
            GlobalLog.LogStatus(exp.ToString() + "\n");
        }
        #endregion Logging
	}
}
