// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xaml;

    // 

    public struct XamlTemplateStruct<T>
    {
        private readonly XamlNodeList _nodes;
        private readonly IXamlObjectWriterFactory _loader;

        public XamlTemplateStruct(IXamlObjectWriterFactory loader,
                                  XamlReader reader)
        {
            this._loader = loader;
            if (reader == null)
            {
                this._nodes = null;
            }
            else
            {
                this._nodes = new XamlNodeList(reader.SchemaContext);
                XamlServices.Transform(reader, _nodes.Writer);
            }
        }

        public XamlNodeList Nodes
        {
            get
            {
                return _nodes;
            }
        }

        public T Evaluate()
        {
            return Evaluate(null);
        }

        public T Evaluate(object context)
        {
            XamlObjectWriter writer = _loader.GetXamlObjectWriter(null);
            XamlServices.Transform(_nodes.GetReader(), writer);
            return (T) writer.Result;
        }
    }
}
