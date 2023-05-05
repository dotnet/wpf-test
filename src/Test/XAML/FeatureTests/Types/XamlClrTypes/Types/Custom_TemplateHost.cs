// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom TemplateHost
    /// </summary>
    [System.Windows.Markup.ContentProperty("Template")]
    public class Custom_TemplateHost : object, ISupportInitialize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Custom_TemplateHost"/> class.
        /// </summary>
        public Custom_TemplateHost()
        {
        }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child value.</value>
        public object Child { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name value.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public Custom_Template Template { get; set; }

        #region ISupportInitialize Members

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            Child = Template.LoadTemplate(null);
        }

        #endregion
    }
}
