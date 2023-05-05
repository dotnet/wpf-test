// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Xaml;
using System.Xaml.Permissions;
using Microsoft.Test.CDFInfrastructure;
using Microsoft.Test.Globalization;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Types.Security;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.XamlTests
{
    public class SecurityTests : MarshalByRefObject
    {
        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void InternalProp_WithXamlAccess()
        {
            XamlAccessLevel xal = XamlAccessLevel.AssemblyAccessTo(typeof(TypeWithModifiers).Assembly.GetName());
            SecurityTests tests = GetSecureAppDomain(xal);
            tests.WriteNode(typeof(TypeWithModifiers), "InternalProp", 25, xal);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void ProtectedProp_WithXamlAccess()
        {
            XamlAccessLevel xal = XamlAccessLevel.PrivateAccessTo(typeof(TypeWithModifiers).AssemblyQualifiedName);
            SecurityTests tests = GetSecureAppDomain(xal);
            tests.WriteNode(typeof(TypeWithModifiers), "ProtectedProp", 25, xal);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void FactoryMethod_WithXamlAccess()
        {
            XamlAccessLevel xal = XamlAccessLevel.AssemblyAccessTo(typeof(TypeWithModifiers).Assembly.GetName());
            SecurityTests tests = GetSecureAppDomain(xal);
            tests.WriteFactoryMethod(typeof(TypeWithModifiers), "Create1", xal);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void TypeConverter_WithXamlAccess()
        {
            XamlAccessLevel xal = XamlAccessLevel.AssemblyAccessTo(typeof(TypeWithModifiers).Assembly.GetName());
            SecurityTests tests = GetSecureAppDomain(xal);
            tests.WriteTypeConverted(typeof(TypeWithModifiers), "TCProp", typeof(TCProp), "hello", xal);
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void InternalProp_NoXamlAccess()
        {
            SecurityTests tests = GetSecureAppDomain(null);
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => tests.WriteNode(typeof(TypeWithModifiers), "InternalProp", 25, null),
                new XamlObjectWriterException());
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void ProtectedProp_NoXamlAccess()
        {
            SecurityTests tests = GetSecureAppDomain(null);
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => tests.WriteNode(typeof(TypeWithModifiers), "ProtectedProp", 25, null),
                new XamlObjectWriterException());
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void ProtectedProp_WithoutPrivateAccess()
        {
            XamlAccessLevel xal = XamlAccessLevel.AssemblyAccessTo(typeof(TypeWithModifiers).Assembly.GetName());
            SecurityTests tests = GetSecureAppDomain(xal);
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => tests.WriteNode(typeof(TypeWithModifiers), "ProtectedProp", 25, xal),
                 new XamlObjectWriterException());
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void FactoryMethod_NoXamlAccess()
        {
            SecurityTests tests = GetSecureAppDomain(null);
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => tests.WriteFactoryMethod(typeof(TypeWithModifiers), "Create", null),
                new XamlObjectWriterException());
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void TypeConverter_NoXamlAccess()
        {
            SecurityTests tests = GetSecureAppDomain(null);
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => tests.WriteTypeConverted(typeof(TypeWithModifiers), "TCProp", typeof(TCProp), "hello", null),
                new XamlObjectWriterException());
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void InternalProp_FullTrust()
        {
            WriteNode(typeof(TypeWithModifiers), "InternalProp", 25, null);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void ProtectedProp_FullTrust()
        {
            WriteNode(typeof(TypeWithModifiers), "ProtectedProp", 25, null);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void FactoryMethod_FullTrust()
        {
            WriteFactoryMethod(typeof(TypeWithModifiers), "Create1", null);
        }

        [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void TypeConverter_FullTrust()
        {
            WriteTypeConverted(typeof(TypeWithModifiers), "TCProp", typeof(TCProp), "hello", null);
        }

        /// <summary>
        /// Regression test
        /// System.Xaml should prevent XamlType creation of List<InternalTypeDefinedInSystemXaml> (in partial trust)
        /// </summary>
        // [DISABLED]
        // [TestCase]
        public void InternalTypeArg()
        {
            XamlSchemaContext schemaContext = new XamlSchemaContext();
            Type type = typeof(XamlLanguage).Assembly.GetType("MS.Internal.Xaml.Context.ObjectWriterContext");
            XamlType xamlType = schemaContext.GetXamlType(typeof(List<>).MakeGenericType(type));
            try
            {
                xamlType.Invoker.CreateInstance(null);
                throw new TestCaseFailedException("Expected SecurityException not thrown");
            }
            catch (SecurityException ex)
            {
                Tracer.LogTrace("Caught Expected SecurityException" + ex.ToString());
            }
        }

        // [DISABLED]
        // [TestCase(Keywords = "MicroSuite")]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void LoadInternalEventHandlerTest()
        {
            string xaml = @"<TypeWithEvent PublicEvent='InternalHandler' 
              xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Security;assembly=XamlClrTypes'/>";
            XamlAccessLevel xal = XamlAccessLevel.PrivateAccessTo(typeof(TypeWithEvent).AssemblyQualifiedName);
            SecurityTests tests = GetSecureAppDomain(xal);
            tests.LoadXamlWithEventHandler(xal, xaml);
        }

        // [DISABLED]
        // [TestCase]
        [SecurityLevel(SecurityLevel.FullTrust)]
        public void LoadPrivateEventHandlerTest()
        {
            string xaml = @"<TypeWithEvent PublicEvent='PrivateHandler' 
              xmlns='clr-namespace:Microsoft.Test.Xaml.Types.Security;assembly=XamlClrTypes'/>";
            XamlAccessLevel xal = XamlAccessLevel.PrivateAccessTo(typeof(TypeWithEvent).AssemblyQualifiedName);
            SecurityTests tests = GetSecureAppDomain(xal);

            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), () => tests.LoadXamlWithEventHandler(xal, xaml));
        }

        public void LoadXamlWithEventHandler(XamlAccessLevel xal, string xaml)
        {
            XamlSchemaContext context = new XamlSchemaContext();
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings()
            {
                LocalAssembly = typeof(TypeWithEvent).Assembly,
            };

            XamlXmlReader reader = new XamlXmlReader(new StringReader(xaml), context, settings);
            XamlObjectWriterSettings writerSettings = new XamlObjectWriterSettings()
            {
                AccessLevel = xal,
            };

            XamlObjectWriter writer = new XamlObjectWriter(context, writerSettings);
            XamlServices.Transform(reader, writer);

            var obj = writer.Result as TypeWithEvent;

            obj.RaisePublicEvent();
            if (obj.EventCount != 1)
            {
                throw new TestCaseFailedException("event handler not hook up correctly");
            }
        }


        private void WriteTypeConverted(Type type, string memberName, Type propType, string value, XamlAccessLevel xal)
        {
            XamlSchemaContext context = new XamlSchemaContext();
            XamlObjectWriterSettings settings = new XamlObjectWriterSettings();
            if (xal != null)
            {
                settings.AccessLevel = xal;
            }

            XamlObjectWriter xor = new XamlObjectWriter(context, settings);

            var xt = context.GetXamlType(type);
            xor.WriteStartObject(xt);
            xor.WriteStartMember(xt.GetMember(memberName));
            xor.WriteStartObject(context.GetXamlType(propType));
            xor.WriteStartMember(XamlLanguage.Initialization);
            xor.WriteValue(value);
            xor.WriteEndMember();
            xor.WriteEndObject();
            xor.WriteEndMember();
            xor.WriteEndObject();

            var obj = xor.Result;

            Tracer.LogTrace("Done writing... " + obj.ToString());
        }

        private void WriteNode(Type type, string memberName, object value, XamlAccessLevel xal)
        {
            XamlSchemaContext context = new XamlSchemaContext();
            XamlObjectWriterSettings settings = new XamlObjectWriterSettings();

            if (xal != null)
            {
                settings.AccessLevel = xal;
            }

            XamlObjectWriter xor = new XamlObjectWriter(context, settings);

            var xt = context.GetXamlType(type);
            xor.WriteStartObject(xt);
            xor.WriteStartMember(xt.GetMember(memberName));
            xor.WriteValue(value);
            xor.WriteEndMember();
            xor.WriteEndObject();

            var obj = xor.Result;

            Tracer.LogTrace("Done writing... " + obj.ToString());
        }

        private void WriteFactoryMethod(Type type, string methodName, XamlAccessLevel xal)
        {
            XamlSchemaContext context = new XamlSchemaContext();
            XamlObjectWriterSettings settings = new XamlObjectWriterSettings();

            if (xal != null)
            {
                settings.AccessLevel = xal;
            }

            XamlObjectWriter xor = new XamlObjectWriter(context, settings);

            var xt = context.GetXamlType(type);
            xor.WriteStartObject(xt);
            xor.WriteStartMember(XamlLanguage.FactoryMethod);
            xor.WriteValue(methodName);
            xor.WriteEndMember();
            xor.WriteEndObject();

            var obj = xor.Result;

            Tracer.LogTrace("Done writing... " + obj.ToString());
        }

        /// <summary>
        /// Create an appdomain with Internet Zone
        /// permissions and provided XamlAccessLevel
        /// </summary>
        /// <param name="xal">The XamlPermissions to use</param>
        /// <returns>Object created in the secure appdomain</returns>
        private SecurityTests GetSecureAppDomain(XamlAccessLevel xal)
        {
            // .NET Core 3.0. This is no longer relevant in .NET Core, so we just return the current instance.
            // Create Internet Zone Evidence
            // Evidence evidence = new Evidence();
            // evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
            // PermissionSet permissionSet = SecurityManager.GetStandardSandbox(evidence);

            // // Provide FileIO access to be able to access this assembly
            // permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Directory.GetCurrentDirectory()));

            // if (xal != null)
            // {
                // permissionSet.AddPermission(new XamlLoadPermission(xal));
            // }

            // Tracer.LogTrace("Permission Set of AppDomain :");
            // Tracer.LogTrace(permissionSet.ToString());

            // AppDomainSetup ads = new AppDomainSetup();
            // ads.ApplicationBase = ".";

            // // Create the sandboxed domain.
            // AppDomain sandbox = AppDomain.CreateDomain(
               // "Sandboxed Domain",
               // evidence,
               // ads,
               // permissionSet,
               // null);
            // SecurityTests tests = (SecurityTests)sandbox.CreateInstanceAndUnwrap(
                // Assembly.GetExecutingAssembly().FullName,
                // typeof(SecurityTests).FullName);

            // return tests;
            return this;
        }
    }
}
