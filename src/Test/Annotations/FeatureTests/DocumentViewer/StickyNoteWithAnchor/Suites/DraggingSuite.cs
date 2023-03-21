// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;
using Annotations.Test;

namespace Avalon.Test.Annotations.Bvts
{
    public class DraggingSuite_BVT : AStickyNoteWithAnchorSuite
    {
        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();

            if (ContentMode == TestMode.Flow)
                MakeWholeDocumentVisible();
            else
                PageLayout(3);
        }        

        [TestCase_Helper()]
		protected void MakeWholeDocumentVisible()
		{
			// "Magic" values that, for flow, will keep the same pagination while displaying the entire document.			
			PageLayout(6);
			SetZoom(40);
		}
        [TestCase_Helper()]
		protected void DoDragTest(int pageNum, int position, string vscan)
		{			
			Rect pageBounds = DocViewerWrapper.GetBoundsOfVisiblePages()[pageNum];
			DocViewerWrapper.SelectionModule.SetSelection(pageNum, PagePosition.Middle, 10);
			StickyNoteWrapper sn = CreateStickyNoteWithAnchor();

			switch (position)
			{
				case 1:
					sn.Drag(pageBounds.TopLeft);
					break;
				case 2:
					sn.Drag(new Point(pageBounds.Left + (pageBounds.Width / 2), pageBounds.Top));
					break;
				case 3:
					sn.Drag(pageBounds.TopRight);
					break;
				case 4:
					sn.Drag(new Point(pageBounds.Right, pageBounds.Top + (pageBounds.Height / 2)));
					break;
				default:
					throw new Exception("Unknown SN position '" + position + "'.");
			}

			if (string.IsNullOrEmpty(vscan))
			{
				printStatus("TODO: need VScan verification.");				
			}
		}

		#region TestSuite tests

        [DisabledTestCase()]
        private void dragging1() { DoDragTest(4, 1, null); }
        [DisabledTestCase()]
        private void dragging2() { DoDragTest(4, 2, null); }
        [DisabledTestCase()]
        private void dragging3() { DoDragTest(3, 3, null); }
        [DisabledTestCase()]
        private void dragging4() { DoDragTest(1, 4, null); }

        [DisabledTestCase()]
		private void dragging_testbounds()
		{
			PageLayout(5);
			Rect[] bounds = DocViewerWrapper.GetBoundsOfVisiblePages();
			AssertEquals("Verify number of pages.", 5, bounds.Length);
			AsyncTestScript script = new AsyncTestScript();
			script.Add("MoveTo", new object[] { bounds[0] });
			script.Add("MoveTo", new object[] { bounds[1] });
			script.Add("MoveTo", new object[] { bounds[2] });
			script.Add("MoveTo", new object[] { bounds[3] });
			script.Add("MoveTo", new object[] { bounds[4] });
			AsyncTestScriptRunner runner = new AsyncTestScriptRunner(this);
			runner.ActionDelay = new TimeSpan(0, 0, 2);
			runner.Run(script);
		}

		#endregion
    }
}	

