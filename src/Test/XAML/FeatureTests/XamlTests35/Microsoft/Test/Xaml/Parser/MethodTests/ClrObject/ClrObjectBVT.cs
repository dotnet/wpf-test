// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.MethodTests.ClrObject
{
    /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// This is parser BVT test that parse XAML for CLR objects
    /// Test cases are:
    ///         - LoadXml with public CLR Object
    ///         - LoadXml with public CLR Object with IParseLiteral
    ///         - LoadXml with protected CLR Object
    ///         - LoadXml with private CLR Object
    ///         - LoadXml with internal CLR Object
    ///         - LoadXml with internal CLR Property
    ///         - LoadXml with private CLR Property
    ///         - LoadXml with protected CLR Property
    ///         - LoadXml with UnSecure CLR Object (which try to open a file).
    ///         - LoadXml with Window tag with permission and without.
    ///         - CLR events on the CLR object.
    ///         - Parser will add children to objects that implement IList
    /// </remarks>
    public class CLRObjectParser
    {
        #region RunTest
        /// <summary>
        /// Test case Entry point
        /// </summary>
        //  [Test(0, @"Parser\ClrObject", TestParameters="Params=parsewindow" Description="LoadXml with Window tag with permission and without." )]
        public void RunTest()
        {
            string strParams = DriverState.DriverParameters["TestParams"];
            GlobalLog.LogStatus("Core:CLRObjectParser Started ..." + "\n");// Start ParserBVT test

            // use CultureInfo.InvariantCulture to ensure argument string does not fail in languages such as Turkish
            strParams = strParams.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            switch (strParams)
            {
                case "PARSECLROBJ":
                    TestClrObjParser();
                    break;
                case "PARSEPROTECT":
                    TestProtectedObjParser();
                    break;
                case "PARSEPRIVATE":
                    TestPrivateObjParser();
                    break;
                case "PARSEINTERNAL":
                    TestInternalObjParser();
                    break;
                case "PARSEINTPROP":
                    TestInternalPropParser();
                    break;
                case "PARSEPRIPROP":
                    TestPrivatePropParser();
                    break;
                case "PARSEPROTPROP":
                    TestProtectedPropParser();
                    break;
                case "PARSEUNSECURE":
                    TestUnSecureTag();
                    break;
                case "PARSEWINDOW":
                    TestWindowTag();
                    break;
                case "ILIST":
                    TestIList();
                    break;
                default:
                    GlobalLog.LogStatus("CLRObjectParser.RunTest was called with an unsupported parameter.");
                    throw new Microsoft.Test.TestSetupException("Parameter is not supported");
            }
        }
        #endregion RunTest
        /// <summary>
        ///  
        /// </summary>
        public CLRObjectParser()
        {
        }
        #region TestClrObjParser
        /// <summary>
        /// TestClrObjParser for public CLR Object
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse without error.
        /// </summary>
        void TestClrObjParser()
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:CLRObjectParser.TestClrObjParser Started ..." + "\n");
            CreateContext();

            IXamlTestParser parser = XamlTestParserFactory.Create();
            ClrObject1 foo = parser.LoadXaml(_CLRObjectXaml, null) as ClrObject1;
            if (foo == null)
                throw new Microsoft.Test.TestValidationException("Expecting a ClrObject1");
            if (foo.Prop != "Public Test")
            {
                throw new Microsoft.Test.TestValidationException("Return incorrect property value..." + foo.Prop);
            }

            DisposeContext();
            GlobalLog.LogStatus("TestClrObjParser Exit without error. Test Pass!");
        }
        #endregion TestClrObjParser
        #region TestProtectedObjParser
        /// <summary>
        /// TestProtectedObjParser for protected CLR Object
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestProtectedObjParser()
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestProtectedObjParser Started ..." + "\n");
            ExpectXamlParseException(_protectedObjectXaml, 2, 2);
        }
        #endregion TestProtectedObjParser
        #region TestPrivateObjParser
        /// <summary>
        /// TestPrivateObjParser for private CLR Object
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestPrivateObjParser()
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestPrivateObjParser Started ..." + "\n");
            ExpectXamlParseException(_privateObjectXaml, 2, 2);
        }
        #endregion TestPrivateObjParser
        #region TestInternalObjParser
        /// <summary>
        /// TestInternalObjParser for internal CLR Object
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestInternalObjParser()
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestInternalObjParser Started ..." + "\n");
            ExpectXamlParseException(_internalObjectXaml, 2, 2);
        }
        #endregion TestInternalObjParser
        #region TestInternalPropParser
        /// <summary>
        /// TestInternalPropParser for internal CLR Property
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestInternalPropParser()
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestInternalPropParser Started ..." + "\n");
            ExpectXamlParseException(_internalPropXaml, 3, 2);
        }
        #endregion TestInternalPropParser
        #region TestPrivatePropParser
        /// <summary>
        /// TestPrivatePropParser for private CLR Property
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestPrivatePropParser()
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestPrivatePropParser Started ..." + "\n");
            ExpectXamlParseException(_privatePropXaml, 3, 2);
        }
        #endregion TestPrivatePropParser
        #region TestProtectedPropParser
        /// <summary>
        /// TestProtectedPropParser for protected CLR Property
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestProtectedPropParser()
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestProtectedPropParser Started ..." + "\n");
            ExpectXamlParseException(_proptectedPropXaml, 3, 2);

        }
        #endregion TestProtectedPropParser
        #region TestUnSecureTag
        /// <summary>
        /// TestUnSecureTag for unsecure CLR object
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestUnSecureTag() // Parse through unsecuretag.xaml
        {
            GlobalLog.LogStatus("Core:CLRObjectParser.TestProtectedPropParser Started ..." + "\n");
            Stream xamlFileStream = File.OpenRead(_unSecureXaml);
            try
            {
                ParserContext pc = new ParserContext();
                pc.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
                OpenTextTag root = (OpenTextTag)System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);
            }
            catch (XamlParseException exp)
            {
                if (exp.InnerException.InnerException.InnerException is SecurityException)
                {
                    GlobalLog.LogDebug("TestUnSecureTag: Test Pass! Parsing Unsecure Object threw exception.");
                }
                else
                {
                    throw new Microsoft.Test.TestValidationException("TestUnSecureTag: InnerException Failed to match.");
                }
                return;
            }
            throw new Microsoft.Test.TestValidationException("TestUnSecureTag: Test Failed! Was able to Parse Unsecure CLR Object.");
        }

        #endregion TestUnSecureTag
        #region TestWindowTag
        /// <summary>
        /// TestWindowTag for unsecure CLR object
        /// Scenario:
        /// - Creating Context and Access Context
        /// - LoadXML(CLCR Object) using Stream
        /// Verify:
        /// - Parse with error.
        /// </summary>
        void TestWindowTag() // Parse through windowstest.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:CLRObjectParser.TestWindowTag Started ..." + "\n");
            CreateContext();

            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            Stream xamlFileStream = File.OpenRead(_windowXaml);
            GlobalLog.LogStatus("TestWindowTag LoadXml with Window tag without permission..." + "\n");
            try
            {

                DockPanel root = (DockPanel)System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);
            }
            catch (XamlParseException exp)
            {
                GlobalLog.LogStatus("TestWindowTag: Test Pass! Parsing Unsecure Object throw exception.\n" + exp.ToString());
            }
            GlobalLog.LogStatus("TestWindowTag LoadXml with Window tag with permission..." + "\n");
            try
            {
                DockPanel root = (DockPanel)System.Windows.Markup.XamlReader.Load(xamlFileStream, pc);
            }
            catch (Exception exp)
            {
                throw new Microsoft.Test.TestValidationException("TestWindowTag: Test Pass! Parsing Unsecure Object throw exception.\n" + exp.ToString());
            }

            DisposeContext();
            GlobalLog.LogStatus("TestWindowTag: Test PASS! Be able to Parse Unsecure CLR Object.");
        }
        #endregion TestWindowTag
        #region TestIList
        /// <summary>
        /// Parser will add children to objects that implement IList
        /// Scenario:
        /// Children or subobjects(both tags and text)
        /// can be added to classes that support one of three interfaces:
        /// IAddChild: IList: IDictionary:
        /// </summary>
        void TestIList() // Parse through TestIList.xaml
        {
            //Create Context here.
            GlobalLog.LogStatus("Core:CLRObjectParser.TestIList Started ..." + "\n");
            IXamlTestParser parser = XamlTestParserFactory.Create();
            Page page = (Page)parser.LoadXaml(_IListXaml, null);
            Canvas root = page.Content as Canvas;

            GlobalLog.LogStatus("Verifying TestIList ...");
            ResourceDictionary res = root.Resources;
            ArrayList a = (ArrayList)res["list"];
            Human h = (Human)a[0];
            if (h.Name != "Fred")
                throw new Microsoft.Test.TestValidationException("Incorrect Property Value in IList..." + h.Name);
            Human h1 = (Human)a[1];
            if (h1.Name != "George")
                throw new Microsoft.Test.TestValidationException("Incorrect Property Value in IList..." + h1.Name);
            if (res.Count != 1)
                throw new Microsoft.Test.TestValidationException("Incorrect resource count..." + res.Count.ToString());

            GlobalLog.LogStatus("TestIList: Test PASS!");
        }
        #endregion TestIList

        void ExpectXamlParseException(string filename)
        {
            IXamlTestParser parser = XamlTestParserFactory.Create();
            XamlParseException xpe = CreateXamlParseException(null, null, null);
            ExceptionHelper.ExpectException(delegate { parser.LoadXaml(filename, null); }, xpe);
        }

        void ExpectXamlParseException(string filename, int lineNum, int linePos)
        {
            IXamlTestParser parser = XamlTestParserFactory.Create();
            XamlParseException xpe = CreateXamlParseException(lineNum, linePos, null);
            ExceptionHelper.ExpectException(delegate { parser.LoadXaml(filename, null); }, xpe);
        }

        void ExpectXamlParseException(string filename, int lineNum, int linePos, Exception innerException)
        {
            IXamlTestParser parser = XamlTestParserFactory.Create();
            XamlParseException xpe = CreateXamlParseException(lineNum, linePos, innerException);
            ExceptionHelper.ExpectException(delegate { parser.LoadXaml(filename, null); }, xpe);
        }

        void SetInternalProp(object targetObject, string targetProperty, object targetValue)
        {
            Type type = targetObject.GetType();
            PropertyInfo propInfo = type.GetProperty(targetProperty);
            propInfo.SetValue(targetObject, targetValue, null);
        }

        /// <summary>
        /// Creates a XamlParseException with the BaseUri property set to pack://siteoforigin:,,,/
        /// </summary>
        XamlParseException CreateXamlParseException(int? lineNum, int? linePos, Exception innerException)
        {
            XamlParseException xpe = null;
            if (lineNum != null && linePos != null)
            {
                if (innerException != null)
                {
                    xpe = new XamlParseException(null, (int)lineNum, (int)linePos, innerException);
                }
                else
                {
                    xpe = new XamlParseException(null, (int)lineNum, (int)linePos);
                }
            }
            else
            {
                xpe = new XamlParseException();
            }

            return xpe;
        }

        #region Defined
        // UiContext defined here
        static Dispatcher s_dispatcher;
        #endregion Defined
        #region filenames
        string _CLRObjectXaml = "clrobject.xaml";
        string _protectedObjectXaml = "clrprotected.xaml";
        string _privateObjectXaml = "clrprivate.xaml";
        string _internalObjectXaml = "clrinternal.xaml";
        string _internalPropXaml = "clrinternalprop.xaml";
        string _privatePropXaml = "clrprivateprop.xaml";
        string _proptectedPropXaml = "clrprotectedprop.xaml";
        string _unSecureXaml = "unsecuretag.xaml";
        string _windowXaml = "windowstest.xaml";
        string _IListXaml = "IListTest.xaml";
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
        #endregion Context
    }
}
