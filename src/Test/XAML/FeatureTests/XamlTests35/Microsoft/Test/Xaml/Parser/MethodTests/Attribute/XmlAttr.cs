// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.IO.Packaging;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Attribute
{
    /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT test that parse XAML using Custom Control under \common\parser\customcontrol.cs
    /// Test cases are:
    ///         - Load xaml and examine XmlAttributeProperties on one element
    ///         - Load xaml and namespace XmlAttributeProperties on one element
    ///         - Load xaml and check XmlDictionary on one element is not null
    ///         - Load xaml and check XmlDictionary.Count on one element
    ///         - Load xaml and check XmlDictionary  on one element for IDictionary properties
    ///         - Load xaml and check XmlDictionary  on one element for ICollection properties
    ///         - Load xaml and check XmlDictionary.Count on a copy of the XmlDictionary object
    ///         - Test XmlDictionary.Add(object, object)
    ///         - Test XmlDictionary.Clear() method
    ///         - Test XmlDictionary.Contains( object ) method
    ///         - Test XmlDictionary.Remove( string ) method
    ///         - Test XmlDictionary enumerators
    ///         - Test XmlDictionary.CopyTo method
    ///         - Load xaml and test XmlDictionary.LookupNamespace on one element
    ///         - Load xaml and test XmlDictionary.LookupPrefix on one element
    ///         - Load xaml and test XmlDictionary.DefaultNamespace on one element
    /// </remarks>
    public class XmlAttrTest
    {
        /// <summary>
        /// 
        /// </summary>      
        public XmlAttrTest()
        {
        }

        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
    
            string strParams = DriverState.DriverParameters["TestParams"];

            GlobalLog.LogStatus("Core:XmlAttrTest Started ..." + "\n"); // Start ParserBVT test
            
            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

            switch (strParams)
            {
                case "XMLATTRBVT1":
                    XmlAttrBVT1();
                    break;
                case "XMLATTRBVT2":
                    XmlAttrBVT2();
                    break;
				case "XMLLANGBVT1":
					XmlLangBVT1();
					break;
				case "XMLLANGBVT2":
					XmlLangBVT2();
					break;
				case "XMLLANGBVT3":
					XmlLangBVT3();
					break;
				case "XMLDICTBVT1":
                    XmlDictBVT1();
                    break;
                case "XMLDICTBVT2":
                    XmlDictBVT2();
                    break;
                case "XMLDICTBVT3":
                    XmlDictBVT3();
                    break;
                case "XMLDICTBVT4":
                    XmlDictBVT4();
                    break;
                case "XMLDICTBVT5":
                    XmlDictBVT5();
                    break;
                case "XMLDICTBVT6":
                    XmlDictBVT6();
                    break;
                case "XMLDICTBVT7":
                    XmlDictBVT7();
                    break;
                case "XMLDICTBVT8":
                    XmlDictBVT8();
                    break;
                case "XMLDICTBVT9":
                    XmlDictBVT9();
                    break;
                case "XMLDICTBVT10":
                    XmlDictBVT10();
                    break;
                case "XMLDICTBVT11":
                    XmlDictBVT11();
                    break;
                case "XMLDICTBVT12":
                    XmlDictBVT12();
                    break;
                case "XMLDICTBVT13":
                    XmlDictBVT13();
                    break;
                case "XMLDICTBVT14":
                    XmlDictBVT14();
                    break;
                default:
                    GlobalLog.LogStatus("XmlAttrTest.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest
        #region XmlAttrBVT1
        /// <summary>
        /// Scenario:
        ///     Load xaml and examine XmlAttributeProperties on one element
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlAttributeProperties with expected values.
        /// </summary>
        void XmlAttrBVT1()
        {
            _xamlPath = Assembly.GetAssembly(this.GetType()).Location;
            _xamlPath = Path.GetDirectoryName(_xamlPath) + "\\xmlattr1.xaml";
            CreateContext();

            GlobalLog.LogStatus( "Parse" );
            // Invoke the parser to create a tree
            IXamlTestParser parser = XamlTestParserFactory.Create();
            UIElement root = (UIElement)parser.LoadXaml(_xamlPath, null);
            GlobalLog.LogStatus( "Done" );

            GlobalLog.LogStatus( "Validate" );
            // Check if the tree is what we expect.
            if (root != null)
            {
                Page rootPage = root as Page;
                if (null == rootPage)
                    throw new Microsoft.Test.TestSetupException("Page cast filed");
                PanelFlow foo = rootPage.Content as PanelFlow; 
                if (foo == null)
                {
                    throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
                }
                else
                {
                    string s;
                    //XmlnsDictionary object that holds parsed xmlns info
                    XmlnsDictionary XD = XmlAttributeProperties.GetXmlnsDictionary(foo);
                    s = XD.DefaultNamespace();
                    if ( s != "http://schemas.microsoft.com/winfx/2006/xaml/presentation" )
                    {
						throw new Microsoft.Test.TestValidationException("Default namespace expected 'http://schemas.microsoft.com/winfx/2006/xaml/presentation' got '" + s + "'");
					}
                    else
                    {
                        s = XD.LookupNamespace("cc");
                        if ( s != _ccns)
                        {
                            throw new Microsoft.Test.TestValidationException("namespace for cc prefix was expected to be '" + _ccns + "' ,got '" + s + "'" );
                        }
                        else
                        {
                            s = XD.LookupPrefix("http://test/moretest");
                            if ( s != "cc2" )
                            {
                                throw new Microsoft.Test.TestValidationException("Prefix expected 'cc2' got '" + s + "'" );
                            }
                            else
                            {
                                s = foo.Language.IetfLanguageTag;
                                if ( s != "en-us" )
                                {
                                    throw new Microsoft.Test.TestValidationException("XmlLang expected 'en-us' got '" + s + "'" );
                                }
                                else
                                {
                                    s = XmlAttributeProperties.GetXmlSpace(foo);
                                    if ( s != "default" )
                                    {
                                        throw new Microsoft.Test.TestValidationException("XmlSpace expected 'default' got '" + s + "'" );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("root return null");
            }
            
            DisposeContext();
            return;
        }
        #endregion XmlAttrBVT1
        #region XmlAttrBVT2
        /// <summary>
        /// Scenario:
        ///     Load xaml and namespace XmlAttributeProperties on one element
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlAttributeProperties with expected values.
        /// </summary>
        void XmlAttrBVT2()
        {
            _xamlPath = Assembly.GetAssembly(this.GetType()).Location;
            _xamlPath = Path.GetDirectoryName(_xamlPath) + "\\xmlattr2.xaml";
            CreateContext();

            GlobalLog.LogStatus( "Parse" );
            // Invoke the parser to create a tree
            IXamlTestParser parser = XamlTestParserFactory.Create();
            UIElement root = (UIElement)parser.LoadXaml(_xamlPath, null);
            GlobalLog.LogStatus( "Done" );

            GlobalLog.LogStatus( "Validate" );
            // Check if the tree is what we expect.
            if ( root != null)
            {
                // Try to get the PanelFlow instance
                Page rootPage = root as Page;
                if (null == rootPage)
                    throw new Microsoft.Test.TestSetupException("Page cast filed");
                PanelFlow foo = rootPage.Content as PanelFlow; 
                if (foo == null)
                {
                    throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
                }
                else
                {
                    string s;
                    //XmlnsDictionary object that holds parsed xmlns info
                    XmlnsDictionary XD = XmlAttributeProperties.GetXmlnsDictionary(foo);
                    s = XD.LookupNamespace("cc");
                    if ( s != "http://test" )
                    {
                        throw new Microsoft.Test.TestValidationException("cc namespace expected 'http://test' got '" + s + "'" );
                    }
                    else
                    {
                        s = XD.LookupPrefix("http://test/moretest" );
                        if ( s != "cc2" )
                        {
                            throw new Microsoft.Test.TestValidationException("Prefix expected 'cc2' got '" + s + "'" );
                        }
                    }
                }
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Tree not as expected" );
            }
        
            // Close Avalon gracefuly.
            DisposeContext();
            return;
        }
        #endregion XmlAttrBVT2

		#region XmlLangBVT1
		/// <summary>
		/// This method parses a XAML in which the root is a Custom DO
		/// with no property defined with [XmlLang] attribute.
		/// </summary>
		void XmlLangBVT1()
		{
			// Make sure that this XAML does not throw an exception even if there is no property on 
			// Custom_DO with [XmlLang] attribute set. By design, we ignore xml:lang in such cases. 
            WPFXamlParser parser = new WPFXamlParser();
            parser.LoadXaml("XmlLangBvt1.xaml", null);           						
		}
		#endregion XmlLangBVT1
		#region XmlLangBVT2
		/// <summary>
		/// This method parses a XAML in which the root is a Custom DO
		/// with a property defined with [XmlLang] attribute.
		/// </summary>
		void XmlLangBVT2()
		{
            WPFXamlParser parser = new WPFXamlParser();
            Custom_DO_Accepting_XmlLang customDO = (Custom_DO_Accepting_XmlLang)parser.LoadXaml("XmlLangBvt2.xaml", null); 			

			// Check the culture property
			String xmlLangValue = customDO.Culture.TwoLetterISOLanguageName;
			if (xmlLangValue != "de")
				throw new Microsoft.Test.TestValidationException("Culture property not set on Custom_DO_Accepting_XmlLang as expected");
		}
		#endregion XmlLangBVT2
		#region XmlLangBVT3
		/// <summary>
		/// </summary>
		void XmlLangBVT3()
		{
            IXamlTestParser parser = XamlTestParserFactory.Create();
            Custom_Clr_Accepting_XmlLang customClr = (Custom_Clr_Accepting_XmlLang)parser.LoadXaml("XmlLangBvt3.xaml", null);

			// Check the culture property
			String xmlLangValue = customClr.Culture;
			if (xmlLangValue != "de-DE")
				throw new Microsoft.Test.TestValidationException("Culture property not set on Custom_Clr_Accepting_XmlLang as expected");
		}
		#endregion XmlLangBVT3

        #region XmlDictBVT1
        /// <summary>
        /// Scenario:
        ///     Load xaml and check XmlDictionary on one element is not null
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlDictionary for null.
        /// </summary>
        void XmlDictBVT1()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );

            // Close Avalon gracefuly.
            DisposeContext();
            return;
        }
        #endregion XmlDictBVT1
        #region XmlDictBVT2
        /// <summary>
        /// Scenario:
        ///     Load xaml and check XmlDictionary.Count on one element
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlDictionary.Count for expected value.
        /// </summary>
        void XmlDictBVT2()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );

            if ( dict != null )
            {
                GlobalLog.LogStatus( "check XmlDictionary.Count" );
                if ( dict.Count != 3 )
                {
                    throw new ApplicationException("Expected 3, got " + dict.Count );
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT2
        #region XmlDictBVT3
        /// <summary>
        /// Scenario:
        ///     Load xaml and check XmlDictionary  on one element for IDictionary properties
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlDictionary IDictionary properties for expected value.
        /// </summary>
        public void XmlDictBVT3()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            if ( dict != null )
            {
                GlobalLog.LogStatus( "Checking dict.IsFixedSize..." );
                if ( dict.IsFixedSize != false )
                {
                    throw new Microsoft.Test.TestValidationException("IsFixedSize expected false, got " + dict.IsFixedSize );
                }
                else
                {
                    GlobalLog.LogStatus( "Checking dict.IsReadOnly..." );
                    if ( dict.IsReadOnly != true )
                    {
                        throw new Microsoft.Test.TestValidationException("IsReadOnly expected true, got " + dict.IsReadOnly );
                    }
                    else
                    {
                        GlobalLog.LogStatus( "Checking dict.Keys, and Count..." );
                        if ( dict.Keys == null || dict.Keys.Count != 3 )
                        {
                            throw new Microsoft.Test.TestValidationException("Invalid Keys property" );
                        }
                        else
                        {
                            GlobalLog.LogStatus( "Checking dict.Values..." );
                            if ( dict.Values == null || dict.Values.Count != 3 )
                            {
                                throw new Microsoft.Test.TestValidationException("Invalid Values property" );
                            }
                            else
                            {
                                GlobalLog.LogStatus( "Checking dict..." );
                                if ( dict["cc"] == null || dict["cc"] != _ccns )
                                {
                                    throw new Microsoft.Test.TestValidationException("Indexer for ['cc'] was expected to return '" + _ccns + "', got " + dict["cc"] );
                                }
                                else
                                {
                                    GlobalLog.LogStatus( "Checking dict[object]..." );
                                    if ( dict[(object)"cc"] == null || (string)dict[(object)"cc"] != _ccns )
                                    {
                                        throw new Microsoft.Test.TestValidationException("Indexer for [(object)'cc'] was expected to return '" + _ccns + "', got " + dict[(object)"cc"] );
                                    }
                                    else
                                    {
                                        GlobalLog.LogStatus( "Checking dict.Sealed..." );
                                        if ( dict.Sealed != true )
                                        {
                                            throw new Microsoft.Test.TestValidationException("Sealed expected true, got " + dict.Sealed );
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        
            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT3
        #region XmlDictBVT4
        /// <summary>
        /// Scenario:
        ///     Load xaml and check XmlDictionary  on one element for ICollection properties
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlDictionary ICollection properties for expected value.
        /// </summary>
        public void XmlDictBVT4()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            if ( dict != null )
            {
                GlobalLog.LogStatus( "Checking dict.IsSynchronized..." );
                if ( dict.IsSynchronized != false )
                {
                    throw new Microsoft.Test.TestValidationException("IsSynchronized expected false, got " + dict.IsSynchronized );
                }
                else
                {
                    GlobalLog.LogStatus( "Checking dict.SyncRoot..." );
                    if ( dict.SyncRoot == null )
                    {
                        throw new Microsoft.Test.TestValidationException("SyncRoot is null" );
                    }
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT4
        #region XmlDictBVT5
        /// <summary>
        /// Scenario:
        ///     Load xaml and check XmlDictionary.Count on a copy of the XmlDictionary object
        ///
        ///     Validation is done, by checking the number of elements in the tree
        /// and verifying the XmlDictionary.Count is the same when copying XmlnsDictionary
        /// </summary>
        public void XmlDictBVT5()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            XmlnsDictionary dict1 = new XmlnsDictionary( dict );
            if ( dict1 != null )
            {
                GlobalLog.LogStatus( "Checking dict1.Count..." );
                if ( dict1.Count != 3 )
                {
                    throw new Microsoft.Test.TestValidationException("Expected 3, got " + dict1.Count );
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT5
        #region XmlDictBVT6
        /// <summary>
        /// Scenario:
        ///     Test XmlDictionary.Add(object, object)
        ///
        ///     Validation is done, by checking if the dictionary contains the element.
        /// </summary>
        public void XmlDictBVT6()
        {
            CreateContext();

            GlobalLog.LogStatus( "Create XmlnsDictionary" );
            XmlnsDictionary dict = new XmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            dict.Add( (object)"cc", (object)"http://test" );
            if ( dict["cc"] == null )
            {
                throw new Microsoft.Test.TestValidationException("dict['cc'] is null " );
            }
            else
            {
                if ( dict["cc"] != "http://test" )
                {
                    throw new Microsoft.Test.TestValidationException("Expected 'http://test', got '" + dict["cc"] + "'" );
                }
            }
            // Close Avalon gracefuly.
            DisposeContext();
            return;
        }
        #endregion XmlDictBVT6
        #region XmlDictBVT7
        /// <summary>
        /// Scenario:
        ///     Test XmlDictionary.Clear() method
        ///
        ///     Validation is done, by checking if the dictionary is empty.
        /// </summary>
        public void XmlDictBVT7()
        {
            CreateContext();

            GlobalLog.LogStatus( "Create XmlnsDictionary" );
            XmlnsDictionary dict = new XmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            GlobalLog.LogStatus( "Adding Dict..." );
            dict.Add( "cc", "http://test" );
            GlobalLog.LogStatus( "Counting Dict..." );
            if ( dict.Count != 1 )
            {
                throw new Microsoft.Test.TestValidationException("Count was expected 1, got " + dict.Count );
            }
            else
            {
                GlobalLog.LogStatus( "Clearing Dict...." );
                dict.Clear();
                GlobalLog.LogStatus( "Counting Dict...." );
                if ( dict.Count != 0 )
                {
                    throw new Microsoft.Test.TestValidationException("Count was expected 0, got " + dict.Count );
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT7
        #region XmlDictBVT8
        /// <summary>
        /// Scenario:
        ///     Test XmlDictionary.Contains( object ) method
        ///
        ///     Validation is done, by checking inserting a test element and checking the contains 
        ///     method before and after insertion.
        /// </summary>
        public void XmlDictBVT8()
        {
            CreateContext();

            GlobalLog.LogStatus( "Create XmlnsDictionary" );
            XmlnsDictionary dict = new XmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            GlobalLog.LogStatus( "Contains...." );
            if ( dict.Contains("cc") )
            {
                throw new Microsoft.Test.TestValidationException("Expected false, got true" );
            }
            else
            {
                GlobalLog.LogStatus( "Adding...." );
                dict.Add( "cc", "http://test" );
                GlobalLog.LogStatus( "Contains...." );
                if ( !dict.Contains("cc") )
                {
                    throw new Microsoft.Test.TestValidationException("Expected true, got false" );
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT8
        #region XmlDictBVT9
        /// <summary>
        /// Scenario:
        ///     Test XmlDictionary.Remove( string ) method
        ///
        ///     Validation is done, by invoking Remove, before and after insertion of a test element, then
        ///     checking the Count to make sure element has been removed.
        /// </summary>
        public void XmlDictBVT9()
        {
            CreateContext();

            GlobalLog.LogStatus( "Create XmlnsDictionary" );
            XmlnsDictionary dict = new XmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            GlobalLog.LogStatus( "Removing Dict..." );
            dict.Remove("cc");
            GlobalLog.LogStatus( "Adding Dict..." );
            dict.Add( "cc", "http://test" );
            GlobalLog.LogStatus( "Counting Dict..." );
            if ( dict.Count != 1 )
            {
                throw new Microsoft.Test.TestValidationException("Count expected 1, got" + dict.Count );
            }
            else
            {
                GlobalLog.LogStatus( "Removing Dict..." );
                dict.Remove("cc");
                GlobalLog.LogStatus( "Counting Dict..." );
                if ( dict.Count != 0 )
                {
                    throw new Microsoft.Test.TestValidationException("Count expected 0, got" + dict.Count );
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT9
        #region XmlDictBVT10
        /// <summary>
        /// Scenario:
        ///     Test XmlDictionary enumerators
        ///
        ///     Validation is done, by checking enumerator iteration counts.
        /// </summary>
        public void XmlDictBVT10()
        {
            CreateContext();

            GlobalLog.LogStatus( "Create XmlnsDictionary" );
            XmlnsDictionary dict = new XmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            GlobalLog.LogStatus( "Adding 1st Dict..." );
            dict.Add( "cc1", "http://test1" );
            GlobalLog.LogStatus( "Adding 2nd Dict..." );
            dict.Add( "cc2", "http://test2" );
            GlobalLog.LogStatus( "Adding 3rd Dict..." );
            dict.Add( "cc3", "http://test3" );
            IDictionaryEnumerator de = ((IDictionary)dict).GetEnumerator();
            IEnumerator e = ((IEnumerable)dict).GetEnumerator();
            GlobalLog.LogStatus( "Do MoveNext..." );
            while(de.MoveNext())
            {
                if ( !e.MoveNext() )
                {
                    throw new Microsoft.Test.TestValidationException("e.MoveNext() expected true, returned false" );
                }
            }
            if ( e.MoveNext() )
            {
                throw new Microsoft.Test.TestValidationException("e.MoveNext() expected false, returned true" );
            }
        
            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT10
        #region XmlDictBVT11
        /// <summary>
        /// Scenario:
        ///     Test XmlDictionary.CopyTo method
        ///
        ///     Validation is done, by invoking CopyTo and verifying all 
        ///     entries were copied correctly
        /// </summary>
        public void XmlDictBVT11()
        {
            CreateContext();

            GlobalLog.LogStatus( "Create XmlnsDictionary" );
            XmlnsDictionary dict = new XmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            GlobalLog.LogStatus( "Adding 1st Dict..." );
            dict.Add( "cc1", "http://test1" );
            GlobalLog.LogStatus( "Adding 2nd Dict..." );
            dict.Add( "cc2", "http://test2" );
            GlobalLog.LogStatus( "Adding 3rd Dict..." );
            dict.Add( "cc3", "http://test3" );
            object[] arr = new object[3];
            GlobalLog.LogStatus( "Copying Dict..." );
            dict.CopyTo( arr, 0 );
            foreach( DictionaryEntry de in arr)
            {
                GlobalLog.LogStatus( "Checking Dict Keys..." );
                if ( dict[de.Key] != de.Value )
                {
                    throw new Microsoft.Test.TestValidationException("Entry with key '" + de.Key + "' vas expected to have value '" + de.Value + "' but got '" + dict[de.Key] + "'" );
                }
            }
        
            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT11
        #region XmlDictBVT12
        /// <summary>
        /// Scenario:
        ///     Load xaml and test XmlDictionary.LookupNamespace on one element
        ///
        ///     Validation is done by comparing result with expected value
        /// </summary>
        public void XmlDictBVT12()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            if ( dict != null )
            {
                GlobalLog.LogStatus( "Looking up Namespace 'cc'..." );
                if ( dict.LookupNamespace("cc") != _ccns )
                {
                    throw new Microsoft.Test.TestValidationException("Expected '" + _ccns + "', got '" + dict.LookupNamespace("cc") + "'" );
                }
                else
                {
                    GlobalLog.LogStatus( "Looking up Namespace 'invalid'..." );
                    if ( dict.LookupNamespace("invalid") != null )
                    {
                        throw new Microsoft.Test.TestValidationException("Expected null, got '" + dict.LookupNamespace("invalid") + "'" );
                    }
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT12
        #region XmlDictBVT13
        /// <summary>
        /// Scenario:
        ///     Load xaml and test XmlDictionary.LookupPrefix on one element
        ///
        ///     Validation is done by comparing result with expected value
        /// </summary>
        public void XmlDictBVT13()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            if ( dict != null )
            {
                GlobalLog.LogStatus( "Looking up Prefix 'ccns'..." );
                if ( dict.LookupPrefix(_ccns) != "cc" )
                {
                    throw new Microsoft.Test.TestValidationException("Expected 'cc', got '" + dict.LookupPrefix(_ccns) + "'" );
                }
                else
                {
                    GlobalLog.LogStatus( "Looking up Prefix 'invalid'..." );
                    if ( dict.LookupPrefix( "invalid" ) != null )
                    {
                        throw new Microsoft.Test.TestValidationException("Expected null, got '" + dict.LookupPrefix("invalid") + "'" );
                    }
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus("Test Pass!");
            return;
        }
        #endregion XmlDictBVT13
        #region XmlDictBVT14
        /// <summary>
        /// Scenario:
        ///     Load xaml and test XmlDictionary.DefaultNamespace on one element
        ///
        ///     Validation is done by comparing result with expected value
        /// </summary>
        public void XmlDictBVT14()
        {
            CreateContext();

            GlobalLog.LogStatus( "Loading XAML" );
            XmlnsDictionary dict = GetXmlnsDictionary();
            GlobalLog.LogStatus( "Done" );
            if ( dict != null )
            {
                GlobalLog.LogStatus( "Checking Default Namespace..." );
                if ( dict.DefaultNamespace() != "http://schemas.microsoft.com/winfx/2006/xaml/presentation" )
                {
                    throw new Microsoft.Test.TestValidationException("Expected 'http://schemas.microsoft.com/winfx/2006/xaml/presentation', got '" + dict.DefaultNamespace() + "'" );
                }
            }

            // Close Avalon gracefuly.
            DisposeContext();
            GlobalLog.LogStatus( "Test Pass!" );
            return;
        }
        #endregion XmlDictBVT14
        #region GetXmlnsDictionary
        /// <summary>
        /// Open XAML, and Parse XAML, Get XmlnsDictionay
        /// </summary>
        /// <returns>XmlnsDictionary</returns>
        XmlnsDictionary GetXmlnsDictionary()
        {
            _xamlPath = Assembly.GetAssembly(this.GetType()).Location;
            _xamlPath = Path.GetDirectoryName(_xamlPath) + "\\xmlattr1.xaml";
            CreateContext();

            GlobalLog.LogStatus( "Parse" );
            // Invoke the parser to create a tree
            IXamlTestParser parser = XamlTestParserFactory.Create();
            UIElement root = (UIElement)parser.LoadXaml(_xamlPath, null);
            GlobalLog.LogStatus( "Done" );

            GlobalLog.LogStatus( "Validate" );
            // Check if the tree is what we expect.
            if ( root != null)
            {
                // Try to get the PanelFlow instance
                Page rootPage = root as Page;
                if (null == rootPage)
                    throw new Microsoft.Test.TestSetupException("Page cast filed");
                PanelFlow foo = rootPage.Content as PanelFlow; 
                if (foo == null)
                {
                    throw new Microsoft.Test.TestValidationException("Expecting a PanelFlow");
                }
                else
                {
                    return XmlAttributeProperties.GetXmlnsDictionary(foo);
                }
            }
            else
            {
                throw new Microsoft.Test.TestValidationException("Tree not as expected" );
            }
        
        }
        #endregion GetXmlnsDictionary
        #region Defined
        // UiContext defined here
        string _xamlPath;
        static Dispatcher s_dispatcher;
        private const string _ccns = "http://XamlTestTypes";
        #endregion Defined
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
        #endregion Context
    }
}
