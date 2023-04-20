// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Documents;
using System.Xaml;
using System.Xaml.Permissions;
using System.Xml;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;
using XamlTestsDev10.Microsoft.Test.Xaml.Security.MethodTests;

namespace Microsoft.Test.Xaml.Security.MethodTests
{
    /// <summary>
    /// Targeted security tests for the System.Xaml stack 
    /// </summary>
    public class SecurityTests
    {
        /// <summary>
        /// White list scenario.
        /// Xaml with assembly in white-list should load fine
        /// </summary>
        public static void TestWhiteListScenarioGood()
        {
            Assembly good = Assembly.LoadWithPartialName("GoodAssembly");
            Assembly bad = Assembly.LoadWithPartialName("BadAssembly");

            List<Assembly> whiteList = new List<Assembly>();
            whiteList.Add(good);

            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext(whiteList);

            XamlServices.Save("good.xaml", new GoodAssembly.GoodType());
            GlobalLog.LogDebug(File.ReadAllText("good.xaml"));

            object result = null;
            using (XmlReader reader = XmlReader.Create("good.xaml"))
            {
                XamlXmlReader xamlReader = new XamlXmlReader(reader, xamlSchemaContext);
                XamlObjectWriter xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext);
                XamlServices.Transform(xamlReader, xamlWriter);
                result = xamlWriter.Result;
            }

            if (result.GetType() != typeof(GoodAssembly.GoodType))
            {
                throw new Exception("Load failed for good type");
            }
        }

        /// <summary>
        /// White-list scenario - negative
        /// Xaml that contains assembly not in white-list should fail when loading
        /// This test uses clr-namespace
        /// </summary>
        public static void TestWhiteListScenarioBad()
        {
            Assembly good = Assembly.LoadWithPartialName("GoodAssembly");

            List<Assembly> whiteList = new List<Assembly>();
            whiteList.Add(good);

            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext(whiteList);

            XamlServices.Save("bad.xaml", new BadAssembly.BadType());
            GlobalLog.LogDebug("Saving xaml content to load - " + File.ReadAllText("bad.xaml"));

            bool failed = false;
            try
            {
                object result = null;
                using (XmlReader reader = XmlReader.Create("bad.xaml"))
                {
                    XamlXmlReader xamlReader = new XamlXmlReader(reader, xamlSchemaContext);
                    XamlObjectWriter xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext);
                    XamlServices.Transform(xamlReader, xamlWriter);
                    result = xamlWriter.Result;
                    failed = true;
                }
            }
            catch (XamlObjectWriterException ex)
            {
                GlobalLog.LogEvidence("Expected failure - Got exception :" + ex.ToString());
                GlobalLog.LogEvidence("Inner Exception :" + ex.InnerException);
                TestLog.Current.Result = TestResult.Pass;
            }

            if (failed)
            {
                GlobalLog.LogEvidence("ERROR: Failed");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// White-list scenario - negative
        /// Xaml with assembly not in white-list, loading using xml namespace
        /// </summary>
        public static void TestWhiteListScenarioBadXmlns()
        {
            Assembly good = Assembly.LoadWithPartialName("GoodAssembly");

            List<Assembly> whiteList = new List<Assembly>();
            whiteList.Add(good);

            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext(whiteList);

            XamlServices.Save("badxmlns.xaml", new BadAssemblyXmlns.BadType());
            GlobalLog.LogDebug("Saving xaml content to load - " + File.ReadAllText("badxmlns.xaml"));

            bool failed = false;
            try
            {
                object result = null;
                using (XmlReader reader = XmlReader.Create("badxmlns.xaml"))
                {
                    XamlXmlReader xamlReader = new XamlXmlReader(reader, xamlSchemaContext);
                    XamlObjectWriter xamlWriter = new XamlObjectWriter(xamlReader.SchemaContext);
                    XamlServices.Transform(xamlReader, xamlWriter);
                    result = xamlWriter.Result;
                    failed = true;
                }
            }
            catch (XamlObjectWriterException ex)
            {
                GlobalLog.LogEvidence("Expected failure - Got exception " + ex.ToString());
                TestLog.Current.Result = TestResult.Pass;
            }

            if (failed)
            {
                GlobalLog.LogEvidence("ERROR: Loaded bad type when it was expected to throw");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Load a protected property without XamlLoadPermission
        /// </summary>
        public static void LoadProtectedWithoutPermission()
        {
            string xaml = @"<CustomAccessModifiers ProtectedIntProperty='5'
                xmlns='clr-namespace:XamlCommon.Microsoft.Test.Xaml.CustomTypes;assembly=XamlCommon' />";

            try
            {
                XamlServices.Parse(xaml);
                GlobalLog.LogEvidence("Expected to fail when loading proptected property without XamlLoadPermissions but didnt fail");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (XamlObjectWriterException ex)
            {
                GlobalLog.LogDebug(ex.ToString());
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Load an internal property without XamlLoadPermission
        /// </summary>
        public static void LoadInternalWithoutPermission()
        {
            string xaml = @"<CustomAccessModifiers InternalIntProperty='5'
                xmlns='clr-namespace:XamlCommon.Microsoft.Test.Xaml.CustomTypes;assembly=XamlCommon' />";

            try
            {
                XamlServices.Parse(xaml);
                GlobalLog.LogEvidence("Expected to fail when loading internal property without XamlLoadPermissions but didnt fail");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (XamlObjectWriterException ex)
            {
                GlobalLog.LogDebug(ex.ToString());
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Load a private property without XamlLoadPermission
        /// </summary>
        public static void LoadPrivateWithoutPermission()
        {
            string xaml = @"<CustomAccessModifiers PrivateIntProperty='5'
                xmlns='clr-namespace:XamlCommon.Microsoft.Test.Xaml.CustomTypes;assembly=XamlCommon' />";

            try
            {
                XamlServices.Parse(xaml);
                GlobalLog.LogEvidence("Expected to fail when loading private property without XamlLoadPermissions but didnt fail");
                TestLog.Current.Result = TestResult.Fail;
            }
            catch (XamlObjectWriterException ex)
            {
                GlobalLog.LogDebug(ex.ToString());
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Assembly.LoadComponent should not allow internal markup
        /// to be loaded from outside the declaring assembly. 
        /// Bug: 689889
        /// </summary>
        public static void LoadXamlExternal()
        {
            App application = new App();
            application.Run();
        }

        /// <summary>
        /// Spoof using a custom type extending from Type
        /// </summary>
        public static void SpoofRuntimeType()
        {
            XamlSchemaContext xsc = new XamlSchemaContext();
            Type equalityLyingType = new EqualityLyingType();
            XamlType xt = xsc.GetXamlType(equalityLyingType);
            Assert.IsTrue(object.ReferenceEquals(equalityLyingType, xt.UnderlyingType));

            Type elementType = typeof(Element);
            xt = xsc.GetXamlType(elementType);
            Assert.IsTrue(object.ReferenceEquals(elementType, xt.UnderlyingType));
            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Spoof using a custom type extending from Type - Wpf Schema context 
        /// should throw on trying to resolve a non runtimetype.
        /// </summary>
        public static void SpoofRuntimeTypeWpfSchemaContext()
        {
            XamlSchemaContext xsc = System.Windows.Markup.XamlReader.GetWpfSchemaContext();
            string message = Exceptions.GetMessage("RuntimeTypeRequired", WpfBinaries.PresentationFramework);
            message = String.Format(message, "Type: EqualityLyingType");
            ExceptionHelper.ExpectException<ArgumentException>(() => xsc.GetXamlType(new EqualityLyingType()), new ArgumentException(message, "type"));
        }
        
        /// <summary>
        /// Null ref tests for XamlLoadPermission
        /// </summary>
        public static void XamlLoadPermissionNullRefTests()
        {
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlLoadPermission((XamlAccessLevel)null), new ArgumentNullException("allowedAccess"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlLoadPermission((IEnumerable<XamlAccessLevel>)null), new ArgumentNullException("allowedAccess"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new XamlLoadPermission(PermissionState.Unrestricted).Includes(null), new ArgumentNullException("requestedAccess"));
            ExceptionHelper.ExpectException<ArgumentException>(() => new XamlLoadPermission(new List<XamlAccessLevel>() { null }), new ArgumentException("allowedAccess"));
            ExceptionHelper.ExpectException<ArgumentException>(() => new XamlLoadPermission(PermissionState.Unrestricted).Includes(null), new ArgumentNullException("requestedAccess"));
        }
    }

    /// <summary>
    /// Custom application that loads an external markup
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Overriding the startup method
        /// </summary>
        /// <param name="e">event arguments</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Pre load the assembly //
                Assembly presentationUI = typeof(PresentationUIStyleResources).Assembly;

                object pp = LoadComponent(new Uri("/PresentationUI;component/TenFeetInstallationProgress.xaml", UriKind.Relative));
                TestLog.Current.Result = TestResult.Fail;
                throw new Exception("Loaded internal markup when it was expected to throw");
            }
            catch (System.Windows.Markup.XamlParseException xpe)
            {
                GlobalLog.LogDebug("Caught Expected Exception:");
                GlobalLog.LogDebug(xpe.ToString());
                TestLog.Current.Result = TestResult.Pass;
            }

            Application.Current.Shutdown();
        }
    }
}
