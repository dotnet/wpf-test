// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Windows.Markup;

namespace Avalon.Test.CoreUI.Parser.Common
{
    /// <summary>
    /// ValueSerializerContext class used for serializing values.
    /// It contains a ParserContext that others can retrieve via GetService.
    /// </summary>
    public class TestValueSerializerContext : TestTypeDescriptorContext, IValueSerializerContext
    {
        /// <summary>
        /// Get serializer for a property descriptor.
        /// </summary>
        /// <param name="descriptor">Descriptor.</param>
        /// <returns>Serializer object if a parser context exists, false otherwise.</returns>
        public ValueSerializer GetValueSerializerFor(PropertyDescriptor descriptor)
        {
            if (_parserContext != null)
            {
                return ValueSerializer.GetSerializerFor(descriptor);
            }

            return null;
        }

        /// <summary>
        /// Get serializer for a type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>Serializer object if a parser context exists, false otherwise.</returns>
        public ValueSerializer GetValueSerializerFor(Type type)
        {
            if (_parserContext != null)
            {
                return ValueSerializer.GetSerializerFor(type);
            }

            return null;
        }
    }
}
