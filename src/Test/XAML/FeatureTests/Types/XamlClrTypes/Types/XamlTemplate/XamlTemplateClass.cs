// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xaml;

    [System.Windows.Markup.XamlDeferLoad(typeof(XamlTemplateTypeConverterClass), typeof(object))]
    public class XamlTemplateClass<T>
    {
        protected XamlNodeList nodes;
        protected IXamlObjectWriterFactory loader;

        public XamlTemplateClass(IXamlObjectWriterFactory loader,
            XamlReader reader)
        {
            this.loader = loader;
            if (reader == null)
            {
                this.nodes = null;
            }
            else
            {
                this.nodes = new XamlNodeList(reader.SchemaContext);
                XamlServices.Transform(reader, nodes.Writer);
            }
        }

        public XamlNodeList Nodes
        {
            get
            {
                return nodes;
            }
        }

        public virtual T Evaluate()
        {
            return Evaluate(null);
        }

        public virtual T Evaluate(object context)
        {
            XamlObjectWriter writer = loader.GetXamlObjectWriter(null);
            XamlServices.Transform(nodes.GetReader(), writer);
            return (T)writer.Result;
        }
    }

    public class XamlTemplateDerivedClass<T> : XamlTemplateClass<T>
    {
        public XamlTemplateDerivedClass(IXamlObjectWriterFactory loader,
            XamlReader reader)
            : base(loader, reader)
        {
        }
    }
}
