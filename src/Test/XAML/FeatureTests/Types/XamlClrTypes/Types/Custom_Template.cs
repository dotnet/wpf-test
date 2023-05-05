// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xaml;
using XAML3 = System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom Template
    /// </summary>
    [XAML3.XamlDeferLoad(typeof(Custom_TemplateConverter), typeof(object))]
    public class Custom_Template
    {
        /// <summary>
        /// xaml Node List
        /// </summary>
        private readonly XamlNodeList _xamlNodeList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_Template"/> class.
        /// </summary>
        /// <param name="xamlReader">The xaml reader.</param>
        /// <param name="factory">The factory.</param>
        public Custom_Template(XamlReader xamlReader, IXamlObjectWriterFactory factory)
        {
            XamlObjectWriterFactory = factory;
            _xamlNodeList = new XamlNodeList(xamlReader.SchemaContext);
            XamlServices.Transform(xamlReader, _xamlNodeList.Writer);
        }

        /// <summary>
        /// Gets the xaml object writer factory.
        /// </summary>
        /// <value>The xaml object writer factory.</value>
        public IXamlObjectWriterFactory XamlObjectWriterFactory { get; private set; }

        /// <summary>
        /// Gets the xaml reader.
        /// </summary>
        /// <returns>Xaml Reader</returns>
        public XamlReader GetXamlReader()
        {
            return _xamlNodeList.GetReader();
        }

        /// <summary>
        /// Loads the template.
        /// this function is just a helper.  The calling code could do this.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>object value</returns>
        public object LoadTemplate(XamlObjectWriterSettings settings)
        {
            XamlObjectWriter xamlWriter = XamlObjectWriterFactory.GetXamlObjectWriter(settings);
            XamlReader xamlReader = GetXamlReader();
            XamlServices.Transform(xamlReader, xamlWriter);
            return xamlWriter.Result;
        }
    }

    /// <summary>
    /// Custom TemplateConverter
    /// </summary>
    public class Custom_TemplateConverter : XamlDeferringLoader
    {
        /// <summary>
        /// Load the instance from the xamlReader
        /// </summary>
        /// <param name="xamlReader">the xamlReader instance</param>
        /// <param name="serviceProvider">Service provide</param>
        /// <returns>loaded object</returns>
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService(typeof(IXamlObjectWriterFactory)) as IXamlObjectWriterFactory;
            if (factory == null)
            {
                throw new InvalidOperationException("Missing Service Provider Service 'IXamlObjectWriterFactory'");
            }

            var testTemplate = new Custom_Template(xamlReader, factory);
            return testTemplate;
        }

        /// <summary>
        /// Save the instance
        /// </summary>
        /// <param name="value">value to save</param>
        /// <param name="serviceProvider">service provider</param>
        /// <returns>Saved XamlReader</returns>
        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }
}
