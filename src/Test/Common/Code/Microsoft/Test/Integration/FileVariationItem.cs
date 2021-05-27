// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// 
    /// </summary>
	public class FileVariationItem : VariationItem
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
            if (!File.Exists(FileName))
            {
                throw new InvalidOperationException("The XamlFile cannot be found. " + FileName);
            }

            CommonStorage.Current.Store("FileName", FileName);
            GlobalLog.LogStatus("The file use is: " + FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }


        private string _fileName = "";

    }
}
