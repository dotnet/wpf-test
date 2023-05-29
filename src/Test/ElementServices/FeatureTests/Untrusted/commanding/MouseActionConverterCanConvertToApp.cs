// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.CoreInput.Common;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify MouseActionConverter CanConvertTo method.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify MouseActionConverter CanConvertTo.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\TypeConverter")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class MouseActionConverterCanConvertToApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new MouseActionConverterCanConvertToApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
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
            
            _converter = TypeDescriptor.GetConverter(typeof(MouseAction));
            Debug.Assert(_converter is MouseActionConverter);

            // Tell the parser how to map XML namespaces to CLR namespaces and assemblies.
            CoreLogger.LogStatus("Getting Type Descriptor Context....");
            ParserContext pc = new ParserContext();
            pc.XamlTypeMapper = XamlTypeMapper.DefaultMapper;
            pc.XamlTypeMapper.AddMappingProcessingInstruction("cmd", "System.Windows.Input", "PresentationCore");

            _typeDescriptorContext = new TestTypeDescriptorContext(pc, MouseAction.LeftClick);
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

            // Note: for this test we are concerned about whether the proper conversion can happen.

            bool bConvertToString = _converter.CanConvertTo(_typeDescriptorContext, typeof(String));
            bool bConvertToStringNullContext = _converter.CanConvertTo(null, typeof(String));
            bool bConvertToInt = _converter.CanConvertTo(typeof(int));
            bool bConvertToNull = _converter.CanConvertTo(null);

            // Since MouseActions are not objects, we don't support InstanceDescriptor conversion.
            bool bConvertToInstanceDescriptor = (_converter.CanConvertTo(null, typeof(InstanceDescriptor)));

            CoreLogger.LogStatus("Convertible to contextstring,nullcontextstring,int,null,descriptor? " +
                               bConvertToString + "," + bConvertToStringNullContext + "," + bConvertToInt + "," + bConvertToNull + "," + bConvertToInstanceDescriptor);

            bool actual = (bConvertToString) && (!bConvertToStringNullContext) && (!bConvertToInt) && (!bConvertToNull) && (!bConvertToInstanceDescriptor);
            bool expected = true;
            CoreLogger.LogStatus("Convertible? " + actual + ", expected: "+expected);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        private TypeConverter _converter;

        /// <summary>
        /// Store a type descriptor context, for use with the type converter API.
        /// </summary>
        private ITypeDescriptorContext _typeDescriptorContext;
    }
}

