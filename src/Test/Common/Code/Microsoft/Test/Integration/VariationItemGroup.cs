// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Markup;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProperty("Children")]
    public class VariationItemGroup : VariationItem
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<VariationItem> GetVIChildren()
        {
            return Children;
        }


        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VariationItemCollection Children
        {
            get
            {
                return _children;
            }
        }

        ///<summary>
        ///</summary>
        public override void Execute()
        {
            foreach (VariationItem v in Children)
            {
                v.Execute();
            }
        }

        private VariationItemCollection _children = new VariationItemCollection();
    }
}
