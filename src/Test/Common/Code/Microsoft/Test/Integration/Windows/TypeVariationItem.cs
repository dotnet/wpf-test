// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Integration.Windows
{
    /// <summary>
    /// 
    /// </summary>
	public class TypeVariationItem : VariationItem
	{
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<VariationItem> GetVIChildren()
        {
            return new List<VariationItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            Type type = TypeName.GetCurrentType();

            GlobalLog.LogStatus("Type (" + type.FullName + ") is store int CommonStorage under the name Type");
            CommonStorage.Current.Store("Type", type);        
        }

        /// <summary>
        /// 
        /// </summary>
        public TypeDesc TypeName
        {
            get { return _type; }
            set { _type = value; }
        }

        private TypeDesc _type = null;
    }
}
