// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace Microsoft.Test.Globalization
{
    public class AssemblyResourceInfo : ISupportInitialize
    {
        public AssemblyResourceInfo()
        {
            this.AssemblyName = String.Empty;
            this.AssemblyPath = String.Empty;
            this.ResourceTables = new ArrayList();
        }

        public string AssemblyName { get; set; }
        
        public ArrayList ResourceTables { get; set; }

        [DefaultValue("")]
        public string AssemblyPath { get; set; }

        [DefaultValue("")]
        public string AssemblyFullName { get; set; }


        #region ISupportInitialize Members

        public void BeginInit()
        {
            //do nothing
        }

        public void EndInit()
        {
            //parse path, set processor arch, culture info

            if (!String.IsNullOrEmpty(this.AssemblyFullName))
            {
                this.AssemblyFullName = Parse(this.AssemblyFullName);
            }
            if (!String.IsNullOrEmpty(this.AssemblyPath))
            {
                this.AssemblyPath = Parse(this.AssemblyPath);
            }
        }

        private string Parse(string p)
        {
            if (p.Contains("[SystemDrive]"))
            {
                p = p.Replace("[SystemDrive]", Environment.GetEnvironmentVariable("SystemDrive"));
            }
            if (p.Contains("[ProcessorArchitecture]"))
            {
                p = p.Replace("[ProcessorArchitecture]", Environment.GetEnvironmentVariable("Processor_Architecture"));
            }
            if(p.Contains("[CurrentCulture]"))
            {
                p = p.Replace("[CurrentCulture]", CultureInfo.CurrentCulture.ToString());
            }
            return p;
        }

        #endregion
    }
}
