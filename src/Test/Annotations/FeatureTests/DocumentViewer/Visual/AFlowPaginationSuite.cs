// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Threading;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Collections.Generic;
using Annotations.Test.Reflection;
using System.Windows.Media;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
    [TestDimension("flow")]
    [TestDimension("stickynote,highlight")]
	public abstract class AFlowPaginationSuite : AVisualSuite
    {
		#region Private Methods

        [TestCase_Setup()]
        protected override void DoSetup()
        {
            SetupWindowWithDoumentViewer();
            DoTestSetup();
            ViewerBase.PreviewMouseDoubleClick += PrintAttachedAnchors;
            Script = CreateSetupScript();
        }

        [TestCase_Cleanup()]
        protected override void CleanupVariation()
        {
            base.CleanupVariation();
            Script = null;
        }

        /// <summary>
        /// Chance for test cases to do their setup.  Code will be executed before RunMode is evaluated.
        /// </summary>
        protected virtual void DoTestSetup() 
        {
            // Nothing by default.
        }

		protected override TestMode DetermineTestMode(string[] args)
		{
			return TestMode.Flow; // Always return flow.
		}

		private void PrintAttachedAnchors(object sender, EventArgs args)
		{
			Proxies.System.Windows.Annotations.AnnotationService service = Proxies.System.Windows.Annotations.AnnotationService.GetService(ViewerBase);
			IEnumerator<IAttachedAnnotation> attachedAnnotations = service.GetAttachedAnnotations().GetEnumerator();
			string anchors = "";
			while (attachedAnnotations.MoveNext())
				anchors += "'" + AnnotationTestHelper.GetText(attachedAnnotations.Current.AttachedAnchor) + "',";

			printStatus("AttachedAnchors:\n" + anchors);
		}

		/// <returns>Script containing setup steps for all tests.</returns>
		private AsyncTestScript CreateSetupScript()
		{
			SetZoom(100);
			WholePageLayout();
			return new AsyncTestScript();	
		}

		#endregion Private Methods

		#region Protected Methods

		protected void SetupWindowWithDoumentViewer()
		{
			SetupTestWindow(); // Initialize basic LogicalTree and globals.
			SetDocumentViewerContent();
        }

        /// <summary>
        /// Added script actions to go to the given page.
        /// 
        /// Precondition: current page is 0.
        /// </summary>
        protected void GoToPage(ref AsyncTestScript script, int page)
        {
            for (int i = 0; i < page; i++)
                script.Add("PageDown", null);
        }

        protected void EnsureWholePage(ref AsyncTestScript script)
        {
            script.Add("WholePageLayout", null);
        }

		/// <summary>
		/// Content type is always flow for subclasses of this class.
		/// </summary>
		override public TestMode ContentMode
		{
			get { return TestMode.Flow; }
		}

        protected IDocumentPaginatorSource SinglePageFlowDocument()
        {
            FlowDocument flow = new FlowDocument();
            Paragraph para = new Paragraph();
            para.Inlines.Clear();
            para.Inlines.Add(new Run(SinglePageText()));
            ((IAddChild)flow).AddChild(para);

            // Override viewer defaults.
            flow.FontSize = 12;

            return flow;
        }

        protected string SinglePageText()
        {
            return "You must use Product Studio for defect management " +
                    "Product Studio's Bugs mode focuses on new features that increase productivity. The redesigned bug form includes a separate, editable repro section that can be \"pinned on top.\" By using this feature, you can see the steps for a bug while keeping your product open, thereby speeding up the repro process. Bug history stores every change to a bug to enable accurate reporting and regression tracking over time. Integration with Source Depot provides a list of check-ins associated with any given bug right on the bug form." +
                    "Product Studio InfoCenter (Productivity Tools)" +
                    "You must use these guidelines to setup the Product Studio database " +
                    "It is well worth your time to properly set up your Product Studio database so that everyone on the team can easily access and use it. Follow these guidelines:" +
                    "Pre-populate common data fields with meaningful values." +
                    "Create useful taxonomic structures for information that needs to be categorized, such as area and sub-area, bug class and category, source, and source ID." +
                    "Create custom data fields that capture information that can be acted on." +
                    "Lock the data fields to prevent random entries that corrupt the data gathering and make searches more difficult." +
                    "Coordinate the common and custom data fields with development partners in other groups." +
                    "Product Studio InfoCenter (Productivity Tools)" +
                    "You must establish guidelines for Product Studio usage " +
                    "Establish clear guidelines for entering, triaging, resolving, and closing bug reports. Document the group’s guidelines, train the team on the guidelines, and post the guidelines where they are easily accessible." +
                    "Filing and Managing Bugs (Interface)";
        }

		#endregion Protected Methods

        #region Protected Fields

        protected AsyncTestScript Script;

        #endregion
    }
}	

