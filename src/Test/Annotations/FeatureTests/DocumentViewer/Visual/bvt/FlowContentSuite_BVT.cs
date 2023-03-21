// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTs that verify annotation compatibility with different
//               kinds of flow content.

using System;
using System.Windows;
using System.Drawing;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Collections.Generic;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations.BVTs
{
    [TestDimension("stickynote,highlight")]
	public class FlowContentSuite_BVT : AFlowSuite
	{
        #region Overrides

        public override IDocumentPaginatorSource  FlowContent
        {
            get 
            {                
                return LoadContent(ViewerTestConstants.ComplexFlowContent);
            }
        }

        #endregion

        #region flowcontent Tests

        /// <summary>
        /// Flow document with completely empty page N.
        /// 1. Annotate page N-1.
        /// 2. Scroll down to page N+1. Verify annotation unloaded.
        /// 2. Annotate page N+1.
        /// 3. Scroll up to N-1. Verify annotation is correct.
        /// 4. Scroll down to N+1. Verify annotation is correct.
        /// </summary>
        private void flowcontent1()
        {
            AsyncTestScript script = new AsyncTestScript();
            int pageN = 1;
            script.Add("GoToPage", new object[] { pageN - 1 });
            script.Add("CreateAnnotation", new object[] { pageN-1, 9, 15 }); // Page n-1.
            script.Add("PageDown");
            script.Add("VerifyAnnotations", new object[] { 0, null }); // Page n.
            script.Add("PageDown");
            script.Add("CreateAnnotation", new object[] { pageN+1, 19, 35 }); // Page n+1.
            script.Add("PageUp");
            script.Add("PageUp");
            script.Add("VerifyPageIsVisible", new object[] { pageN - 1 });
			script.Add("VerifyAnnotation", new object[] { GetText(new SimpleSelectionData(pageN-1, 9, 15)) });
            script.Add("PageDown");
            script.Add("PageDown");
            script.Add("VerifyPageIsVisible", new object[] { pageN + 1 });
			script.Add("VerifyAnnotation", new object[] { GetText(new SimpleSelectionData(pageN+1, 19, 35)) });
            RunScript(script);
        }

        /// <summary>
        /// 1. Annotate an image on middle page.
        /// 2. Scroll down.  Verify annotation unloaded.
        /// 3. Scroll up. Verify annotation reloaded and correct.
        /// </summary>
        private void flowcontent2()
        {
			string anchor = new SimpleSelectionData(0, 200, 100).GetSelection(DocViewerWrapper.SelectionModule);
            AsyncTestScript script = new AsyncTestScript();
            script.Add("CreateAnnotation", new object[] { 0, 200, 100 });
			script.Add("VerifyAnnotation", new object[] { anchor });
            script.Add("PageDown");
            script.Add("VerifyPageIsVisible", new object[] { 1 });
            script.Add("VerifyAnnotations", new object[] { 0, null });
            script.Add("PageUp");
            script.Add("VerifyPageIsVisible", new object[] { 0 });
			script.Add("VerifyAnnotation", new object[] { anchor });
            RunScript(script);
        }

        #endregion
    }
}	

