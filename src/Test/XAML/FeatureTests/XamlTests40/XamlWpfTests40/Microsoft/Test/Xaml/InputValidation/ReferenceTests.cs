// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Globalization;

namespace Microsoft.Test.Xaml.InputValidation
{
    /// <summary>
    /// Tests for input validation of Reference class members
    /// </summary>
    public class ReferenceTests
    {
        /// <summary>
        /// Tests ProvideValue inputs
        /// </summary>
        public void ProvideValueTest()
        {
            Reference reference = new Reference();

            ExceptionHelper.ExpectException<ArgumentNullException>(() => reference.ProvideValue(null), new ArgumentNullException("serviceProvider"));

            IServiceProvider serviceProvider = new CustomServiceProvider();
            ExceptionHelper.ExpectException<InvalidOperationException>(() => reference.ProvideValue(serviceProvider), new InvalidOperationException(), "MissingNameResolver", WpfBinaries.SystemXaml);
        }

        /// <summary>
        /// Verify namescopes work properly with a dependency object
        /// </summary>
        public void AttachableNameScopePropertyAttrTest()
        {
            string xaml = @"<AttachedNameScope xmlns='clr-namespace:Microsoft.Test.Xaml.InputValidation;assembly=XamlWpfTests40' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                <AttachedNameScope.BarForward>
                                    <MultipleRefBar x:Name='__Reference_ID_0' MultipleRefBar.IntProperty='15' MultipleRefBar.StringProperty='Hello world' />
                                </AttachedNameScope.BarForward>
                                <AttachedNameScope.Bar>
                                    <MultipleRefBar x:Name='__Reference_ID_1' MultipleRefBar.IntProperty='15' MultipleRefBar.StringProperty='Hello world' />
                                </AttachedNameScope.Bar>
                                <AttachedNameScope.BarBackward>
                                    <MultipleRefBar x:Name='__Reference_ID_2' MultipleRefBar.IntProperty='15' MultipleRefBar.StringProperty='Hello world' />
                                </AttachedNameScope.BarBackward>
                            </AttachedNameScope>";

            var instance = (AttachedNameScope)XamlServices.Parse(xaml);

            INameScope scope = NameScope.GetNameScope(instance);
            if (scope == null)
            {
                throw new TestValidationException("Namescope is null.");
            }

            foreach (string name in new string[]
                                        {
                                            "__Reference_ID_0", "__Reference_ID_1", "__Reference_ID_2"
                                        })
            {
                if (scope.FindName(name) == null)
                {
                    throw new TestValidationException("Unable to find refernce for " + name);
                }
            }
        }

        /// <summary>
        /// verify reasonable exceptions for bugs 658337, 658341, 658348
        /// </summary>
        public void VerifyCorrectExceptions()
        {
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new EventSetterHandlerConverter().ConvertFrom((ITypeDescriptorContext)null, (CultureInfo)null, (object)null), new ArgumentNullException("typeDescriptorContext"));
            ExceptionHelper.ExpectException<ArgumentNullException>(() => new SetterTriggerConditionValueConverter().ConvertFrom((ITypeDescriptorContext)null, (CultureInfo)null, string.Empty), new ArgumentNullException("serviceProvider"));
        }
    }

    /// <summary>
    /// A DependencyObject with the NamescopePropertyAttribute
    /// </summary>
    [NameScopeProperty("NameScope", typeof(NameScope))]
    public class AttachedNameScope : DependencyObject
    {
        /// <summary>
        /// Gets or sets the property
        /// </summary>
        public MultipleRefBar Bar { get; set; }

        /// <summary>
        /// Gets or sets the property
        /// </summary>
        public MultipleRefBar BarForward { get; set; }

        /// <summary>
        /// Gets or sets the property
        /// </summary>
        public MultipleRefBar BarBackward { get; set; }
    }

    /// <summary>
    /// A class with some properties
    /// </summary>
    public class MultipleRefBar
    {
        /// <summary>
        /// Gets or sets the property
        /// </summary>
        public int IntProperty { get; set; }

        /// <summary>
        /// Gets or sets the property
        /// </summary>
        public string StringProperty { get; set; }
    }

    /// <summary>
    /// Custom service provider for internal use
    /// </summary>
    internal class CustomServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Gets service
        /// </summary>
        /// <param name="serviceType">Type of service to get</param>
        /// <returns>Service object</returns>
        public object GetService(Type serviceType)
        {
            return new object();
        }
    }
}
