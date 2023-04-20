// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml.Schema;
using System.Xaml;

namespace Microsoft.Xaml.Tools
{
    internal class WrapperXamlValueConverter<T> : XamlValueConverter<T> where T : class
    {
        XamlValueConverter<T> _valueConverter;
        UISchemaContext _schemaContext;

        public WrapperXamlValueConverter(XamlValueConverter<T> valueConverter, UISchemaContext schemaContext)
            : base(valueConverter.ConverterType, schemaContext.GetWrappedXamlType(valueConverter.TargetType))
        {
            _schemaContext = schemaContext;
            _valueConverter = valueConverter;
        }

        protected override T CreateInstance()
        {
            return _valueConverter.ConverterInstance;
        }
    }
}
