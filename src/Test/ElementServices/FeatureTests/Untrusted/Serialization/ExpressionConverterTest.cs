// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Parameter Validation and Negitive test for class ValueSerializer.
 
  
 * Revision:         $Revision: 1 $
 
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using System.ComponentModel;
using System.Windows.Controls;
using System.Globalization;

using System.Collections.Generic;

namespace Avalon.Test.CoreUI.Serialization.Converter
{
    /// <summary>
    /// Parameter Validation and Nagitive Tests for ValueSerializer. 
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ExpressionConverterTest
    {
        /// <summary>
        /// Verify basic behavior of ExpressionConverter.
        /// </summary>
        [TestCase("0", @"Serialization\ExpressionConverter", "Verify basic behavior of ExpressionConverter via ResourceReferenceExpressionConverter.")]
        public void VerifyResourceReferenceExpressionConverter()
        {
            // Create valid ResourceReferenceExpression.
            DynamicResourceExtension ext = new DynamicResourceExtension("foo");
            object exp = ext.ProvideValue(null);

            ResourceReferenceExpressionConverter resRefConverter = new ResourceReferenceExpressionConverter();
            TestTypeDescriptorContext typeDescContext = new TestTypeDescriptorContext(Panel.BackgroundProperty, "red");

            //
            // Verify API for invalid conversions.
            //
            // - CanConvertTo --> false
            // - CanConvertFrom --> false
            // - ConvertTo --> throws
            // - ConvertFrom --> throws
            //
            if (resRefConverter.CanConvertTo(typeDescContext, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            if (resRefConverter.CanConvertFrom(typeDescContext, typeof(string))) throw new Microsoft.Test.TestValidationException("FAILED");

            Exception caughtException = null;
            try
            {
                resRefConverter.ConvertTo(typeDescContext, CultureInfo.InvariantCulture, exp, typeof(string));
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            if (caughtException == null) throw new Microsoft.Test.TestValidationException("FAILED");

            caughtException = null;
            try
            {
                resRefConverter.ConvertFrom(typeDescContext, CultureInfo.InvariantCulture, "foo");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            if (caughtException == null) throw new Microsoft.Test.TestValidationException("FAILED");


            //
            // Verify API for valid conversions.
            //
            // - CanConvertTo --> true
            // - CanConvertFrom --> false
            // - ConvertTo --> DynamicResourceExtension
            // - ConvertFrom --> throws
            //
            if (!resRefConverter.CanConvertTo(typeDescContext, typeof(MarkupExtension))) throw new Microsoft.Test.TestValidationException("FAILED");

            if (resRefConverter.CanConvertFrom(typeDescContext, typeof(MarkupExtension))) throw new Microsoft.Test.TestValidationException("FAILED");

            ext = resRefConverter.ConvertTo(typeDescContext, CultureInfo.InvariantCulture, exp, typeof(MarkupExtension)) as DynamicResourceExtension;
            if (ext == null) throw new Microsoft.Test.TestValidationException("FAILED");

            caughtException = null;
            try
            {
                resRefConverter.ConvertFrom(typeDescContext, CultureInfo.InvariantCulture, ext);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            if (caughtException == null) throw new Microsoft.Test.TestValidationException("FAILED");

        }
    }

    // Dummy ITypeDescriptorContext to use for testing ExpressionConverter.
    class TestTypeDescriptorContext : ITypeDescriptorContext
    {
        public TestTypeDescriptorContext(DependencyProperty property, object propertyValue)
        {
            _property = property;
            _propertyValue = propertyValue;
        }

        #region ITypeDescriptorContext Members

        System.ComponentModel.IContainer System.ComponentModel.ITypeDescriptorContext.Container
        {
            get { return null; }
        }

        // Returns a value of a property - to be detected for convertability to string in a type converter
        object System.ComponentModel.ITypeDescriptorContext.Instance
        {
            get
            {
                return _propertyValue;
            }
        }

        void System.ComponentModel.ITypeDescriptorContext.OnComponentChanged()
        {
        }

        bool System.ComponentModel.ITypeDescriptorContext.OnComponentChanging()
        {
            return false;
        }

        System.ComponentModel.PropertyDescriptor System.ComponentModel.ITypeDescriptorContext.PropertyDescriptor
        {
            get { return null; }
        }

        #endregion

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            return null;
        }

        #endregion

        private DependencyProperty _property = null;
        private object _propertyValue = null;
    }
}
