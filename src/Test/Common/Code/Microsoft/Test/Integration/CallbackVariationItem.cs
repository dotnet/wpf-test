// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Markup;
using System.ComponentModel;

namespace Microsoft.Test.Integration
{
    ///<summary>
    ///</summary>
    [ContentProperty("Content")]
    public class CallbackVariationItem : VariationItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<VariationItem> GetVIChildren()
        {
            return new List<VariationItem>();
        }
        
        ///<summary>
        ///</summary>
        public override void Execute()
        {
            object o = null;
            
            Delegate t = MethodName.GetDelegate(typeof(CallbackContentItem), ref o, true);

            t.DynamicInvoke(new object[] { Content });
        }

        /// <summary>
        /// 
        /// </summary>
        public MethodDesc MethodName
        {
            get { return _methodName; }
            set 
            { 
                if (value != null)
                {
                    value.ValidateState();
                }
                
                _methodName = value; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(null)]
        public ContentItem Content
        {
            get { return _content; }
            set { _content = value; }
        }

        private ContentItem _content = null;
        private MethodDesc _methodName = null;       

    }
}
