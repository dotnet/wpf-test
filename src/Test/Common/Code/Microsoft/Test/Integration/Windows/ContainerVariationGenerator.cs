// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Markup;
using Microsoft.Test.Integration;

namespace Microsoft.Test.Integration.Windows
{
    ///<summary>
    ///</summary>    
    public class ContainerVariationGenerator : CombinationGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        public List<ContentItem> ApplicationTypes 
        {
            get
            {
                return _applicationTypes;
            }
        }

        #region IVariationGenerator Members

        ///<summary>
        ///</summary>
        public override List<VariationItem>  Generate()
        {
            List<VariationItem> list = new List<VariationItem>();

            //
            // Get combinations -- each item should be a VariationItemGroup.
            //
            List<VariationItem> groups = base.Generate();

            //
            // Multiply each combination by the app types.
            //
            foreach (ContentItem appType in _applicationTypes)
            {
                TestContract defaultContract = new TestContract();
                string appTypeStr = (string)appType.Content;

                // If the apptype is a deployed application, add the prebuilt
                // app to default SupportFiles.
                if (String.Equals(appTypeStr, "WpfApplication", StringComparison.InvariantCultureIgnoreCase))
                    defaultContract.SupportFiles.Add("Common\\ControllerWpfApp.*");
                else if (String.Equals(appTypeStr, "Xbap", StringComparison.InvariantCultureIgnoreCase))
                    defaultContract.SupportFiles.Add("Common\\ControllerBrowserApp.*");

                foreach (VariationItem item in groups)
                {
                    ContainerVariationItem cvi = new ContainerVariationItem();
                    cvi.Title = appTypeStr;
                    cvi.Creator = typeof(ContainerVariationGenerator).Name;
                    cvi.ApplicationType = appTypeStr;

                    cvi.Merge(this);
                    cvi.Merge(defaultContract);
                    cvi.Merge(item);
                    cvi.Merge(appType);

                    if (item is VariationItemGroup)
                    {
                        cvi.Children.AddRange(((VariationItemGroup)item).Children);
                    }
                    else
                    {
                        cvi.Children.Add(item);
                    }

                    list.Add(cvi);
                }
            }

            return list;

        }

        #endregion

        private string _currentAppType = String.Empty;
        private List<ContentItem> _applicationTypes = new List<ContentItem>();
    }
}
