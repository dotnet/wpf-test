// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;

namespace PropertyOrderUsingNestedME
{
    public class CustomME : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new CustomME2();
        }
    }
}
