// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;

namespace RegressionIssue57
{
    public class StringExtension : MarkupExtension
    {
        string _payload;

        public StringExtension(string data)
        {
            _payload = data;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _payload;
        }
    }
}
