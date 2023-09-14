// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Logging;



namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Tests for MarkupExtensionReturnTypeAttribute.
    /// </summary>
    [TestDefaults]
    public class MarkupExtensionReturnTypeTest
    {
        private Dictionary<Type, Type> _returnTypeMap = new Dictionary<Type,Type>();

        /// <summary>
        /// Initializes a map from MarkupExtension to ReturnType.
        /// </summary>
        public MarkupExtensionReturnTypeTest()
        {
            _returnTypeMap.Add(typeof(ResourceKey), typeof(ResourceKey));
            _returnTypeMap.Add(typeof(BindingBase), typeof(Object));
            _returnTypeMap.Add(typeof(Binding), typeof(Object));
            _returnTypeMap.Add(typeof(MultiBinding), typeof(Object));
            _returnTypeMap.Add(typeof(PriorityBinding), typeof(Object));
            _returnTypeMap.Add(typeof(RelativeSource), typeof(RelativeSource));
            _returnTypeMap.Add(typeof(TemplateKey), typeof(ResourceKey));
            _returnTypeMap.Add(typeof(DataTemplateKey), typeof(ResourceKey));
            _returnTypeMap.Add(typeof(DynamicResourceExtension), typeof(Object));
            _returnTypeMap.Add(typeof(ColorConvertedBitmapExtension), typeof(System.Windows.Media.Imaging.ColorConvertedBitmap));
            _returnTypeMap.Add(typeof(StaticResourceExtension), typeof(Object));
            _returnTypeMap.Add(typeof(ArrayExtension), typeof(Array));
            _returnTypeMap.Add(typeof(NullExtension), typeof(Object));
            _returnTypeMap.Add(typeof(StaticExtension), typeof(Object));
            _returnTypeMap.Add(typeof(TypeExtension), typeof(Type));
            _returnTypeMap.Add(typeof(ThemeDictionaryExtension), typeof(Uri));
        }

        /// <summary>
        /// Verify MarkupExtensionReturnTypeAttribute mappings.
        /// </summary>
        public void VerifyBasicBehavior() 
        {
            using (TestLog log = new TestLog(DriverState.TestName))
            {
                log.Result = TestResult.Pass;
                try
                {
                    List<Type> types = Utility.GetAssemblyTypes(typeof(FrameworkElement).Assembly);

                    foreach (Type type in types)
                    {
                        foreach (Attribute attrib in TypeDescriptor.GetAttributes(type))
                        {
                            MarkupExtensionReturnTypeAttribute markupAttrib =
                                attrib as MarkupExtensionReturnTypeAttribute;

                            if (markupAttrib != null)
                            {
                                CoreLogger.LogStatus(type.Name + " -- ReturnType: " + markupAttrib.ReturnType);

                                if (_returnTypeMap.ContainsKey(type))
                                {
                                    Type returnType = _returnTypeMap[type];

                                    if (returnType != markupAttrib.ReturnType) throw new Microsoft.Test.TestValidationException("Failed");
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    log.LogEvidence(e);
                    log.Result = TestResult.Fail;
                }
            }
        }
    }
}
