// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Xaml;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.ObjectReaderTests
{
    /// <summary>
    /// XamlXmlWriter XmlSupport Tests
    /// </summary>
    public static class ObjectReaderBasicTests
    {
        /// <summary>
        /// ITypeDescriptorContext  Instance using Tranform (Regression for 640338)
        /// </summary>
        public static void ItdsContextInstance_Transform()
        {
            string expected = @"<RotateTransform Angle=""45"" CenterX=""0"" CenterY=""0"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" />";
            
            RotateTransform transform = new RotateTransform(45.0);
            string generated = XamlServices.Save(transform);

            Assert.AreEqual(expected, generated);
        }
    }
}
