// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ComponentModel;
using System.Windows.Markup;
using Microsoft.Test.Integration;

namespace Microsoft.Test.Integration.Windows
{
    ///<summary>
    ///</summary>    
    public class ActionSequenceVariationGenerator : CallbackVariationGenerator
    {
        #region IVariationGenerator Members

        ///<summary>
        ///</summary>
        public override List<VariationItem> Generate()
        {
            List<VariationItem> viList = new List<VariationItem>(ContentItems.Count);

            foreach (ContentItem ci in ContentItems)
            {
                string xtcFileName = (string)ci.Content;

                TestContract defaultContract = new TestContract();
                defaultContract.SupportFiles.Add(xtcFileName);

                XmlDocument doc = new XmlDocument();
                doc.Load(TestExtenderHelper.QualifyPath(xtcFileName));

                // Construct the XmlNamespaceManager used for xpath queries later.
                NameTable ntable = new NameTable();
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(ntable);
                nsmgr.AddNamespace("x", doc.DocumentElement.NamespaceURI);

                // Query for all TEST nodes.
                XmlNodeList testNodes = doc.SelectNodes("//x:TEST", nsmgr);

                // Set each TEST node in new CallbackVariationItem.
                foreach (XmlNode testNode in testNodes)
                {
                    CallbackVariationItem cvi = new CallbackVariationItem();
                    cvi.Creator = "CallbackVariationGenerator";
                    cvi.MethodName = this.Callback;

                    ContentItem contentItem = new ContentItem();
                    contentItem.Content = testNode.OuterXml;

                    cvi.Content = contentItem;

                    if (testNode.Attributes["Title"] != null)
                    {
                        cvi.Title = testNode.Attributes["Title"].Value as string;
                    }

                    cvi.Merge(this);
                    cvi.Merge(defaultContract);
                    cvi.Merge(ci);

                    viList.Add(cvi);
                }

            }

            return viList;

        }

        #endregion
    }
}
