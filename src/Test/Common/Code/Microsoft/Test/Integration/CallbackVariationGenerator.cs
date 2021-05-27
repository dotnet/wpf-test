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
    /// <param name="item"></param>
    public delegate void CallbackContentItem(ContentItem item);
    
    ///<summary>
    /// The delegate for the callback is:
    /// void Callback(ContentItem o)
    ///</summary>    
    [ContentProperty("ContentItems")]
    public class CallbackVariationGenerator : BaseVariationGenerator, IVariationGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        public MethodDesc Callback
        {
            get { return _callback; }
            set { _callback = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ContentItemCollection ContentItems 
        {
            get
            {
                return _content;
            }
        }

        #region IVariationGenerator Members

        ///<summary>
        ///</summary>
        public override List<VariationItem>  Generate()
        {
            List<VariationItem> viList = new List<VariationItem>(ContentItems.Count);

            foreach (ContentItem ci in ContentItems)
            {
                CallbackVariationItem cvi = new CallbackVariationItem();                
                cvi.Creator = "CallbackVariationGenerator";
                cvi.MethodName = this.Callback;
                cvi.Content = ci;
                if (String.IsNullOrEmpty(ci.Title))         //check to see if the title is explicitly set, if not, set it
                    ci.Title = ci.Content as string;
                cvi.Merge(this);
                cvi.Merge(ci);
                
                viList.Add(cvi);
            }
            
            return viList;

        }

        #endregion

        private ContentItemCollection _content = new ContentItemCollection();
        private MethodDesc _callback;        

    }
}
