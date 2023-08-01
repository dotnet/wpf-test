// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Parser.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify CursorConverter CanConvertTo method with null context works as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CursorConverter CanConvertTo method with null context works as expected.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"CoreInput\Cursor")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CursorTypeConverterCanConvertToNullContextApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CursorTypeConverterCanConvertToNullContextApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.Run();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Getting Converter....");
            _converter = TypeDescriptor.GetConverter(typeof(Cursor));

            // Tell the parser how to map XML namespaces to CLR namespaces and assemblies.
            CoreLogger.LogStatus("Getting Type Descriptor Context....");
            ParserContext pc = new ParserContext();
            pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
            pc.XamlTypeMapper.AddMappingProcessingInstruction("inputcore", "System.Windows.Input", "PresentationCore");
            _typeDescriptorContextNull = new TestTypeDescriptorContext(pc);
            _typeDescriptorContextObject = new TestTypeDescriptorContext(pc, new Object());

            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether the proper conversion can occur.

            // expect matching conversion results - string doesn't, InstanceDescriptor doesn't.

            // null context
            bool actualStringNull = (_converter.CanConvertTo(null, typeof(string)));
            bool actualStringInstanceNull = (_converter.CanConvertTo(_typeDescriptorContextNull, typeof(string)));
            bool actualStringInstanceObject = (_converter.CanConvertTo(_typeDescriptorContextObject, typeof(string)));
            bool actualInstanceDescriptorNull = (_converter.CanConvertTo(null, typeof(InstanceDescriptor)));
            CoreLogger.LogStatus("Convertible to string, string with null context instance, string with non-Cursor context instance, instance descriptor? (expect true,true,true,false) " + 
                actualStringNull + "," +
                actualStringInstanceNull + "," +
                actualStringInstanceObject + "," +
                actualInstanceDescriptorNull
                );

            bool actual = (actualStringNull) && (actualStringInstanceNull) && (actualStringInstanceObject) && (!actualInstanceDescriptorNull);
            bool expected = true;
            CoreLogger.LogStatus("Convertible? " + actual + ", expected: " + expected);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private TypeConverter _converter;

        /// <summary>
        /// Store a type descriptor context, for use with the CanConvertTo API.
        /// </summary>
        private ITypeDescriptorContext _typeDescriptorContextNull;

        /// <summary>
        /// Store a type descriptor context, for use with the CanConvertTo API.
        /// </summary>
        private ITypeDescriptorContext _typeDescriptorContextObject;

    }
}

