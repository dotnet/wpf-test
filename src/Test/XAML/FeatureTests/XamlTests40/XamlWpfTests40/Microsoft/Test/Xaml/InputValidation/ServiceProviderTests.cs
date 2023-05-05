// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.InputValidation
{
    /// <summary>
    /// Input validation tests for service providers.
    /// </summary>
    public class ServiceProviderTests
    {
        /// <summary>
        /// Tests GetAllAmbientValues
        /// </summary>
        public void GetAllAmbientValuesTest()
        {
            string xaml = @"<MyME xmlns='clr-namespace:Microsoft.Test.Xaml.InputValidation;assembly=XamlWpfTests40'>1</MyME>";
            ExceptionHelper.ExpectException<XamlObjectWriterException>(() => XamlServices.Parse(xaml), new XamlObjectWriterException());
        }
    }

    /// <summary>
    /// Markup extension that tests GetAllAmbientValues 
    /// </summary>
    [ContentProperty("Content")]
    public class MyME : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the Content property
        /// </summary>
        public int Content { get; set; }

        /// <summary>
        /// Provide value for markup extension
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <returns>the created object</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IAmbientProvider ambient = serviceProvider.GetService(typeof(IAmbientProvider)) as IAmbientProvider;

            foreach (AmbientPropertyValue ambientValue in ambient.GetAllAmbientValues(null))
            {
                GlobalLog.LogDebug(ambientValue.RetrievedProperty + " " + ambientValue.Value);
            }

            return 0;
        }
    }
}
