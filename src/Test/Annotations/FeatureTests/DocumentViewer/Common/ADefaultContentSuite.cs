// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Handles setting up DocumentViewer with Fixed or Flow content.

using System;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// TestSuite to provides default fixed and flow content.
	/// </summary>
    public abstract class ADefaultContentSuite : ADocumentViewerSuite
    {
        #region Public Methods

        /// <summary>
        /// For fixed, just switch to 2 page layout.
        /// For flow, switch to 2 page layout and decrease the zoom to make reflow close to original.
        /// </summary>
        public void ViewAsTwoPages()
        {
            PageLayout(2);
            if (ContentMode == TestMode.Flow)
                SetZoom(79); // Rough value that produces similar flow when pages are half the original size.
        }


        virtual public IDocumentPaginatorSource FlowContent
        {
            get
            {
                return AnnotationTestHelper.LoadContent(ViewerTestConstants.SimpleFlowContent);
            }
        }

        virtual public IDocumentPaginatorSource FixedContent
        {
            get
            {
                if (UseFDS)
                   return AnnotationTestHelper.LoadContent(ViewerTestConstants.DocumentSequence);
                return AnnotationTestHelper.LoadContent(ViewerTestConstants.SimpleFixedContent);
            }
        }

        public IDocumentPaginatorSource EmptyFixedContent
        {
            get { return new FixedDocument(); }
        }

        public void GoToPageRange(int a, int b)
        {
            if (ContentMode == TestMode.Flow)
                ViewAsTwoPages();
            DocViewerWrapper.GoToPageRange(a, b);
        }

        #endregion

        #region Protected Methods

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            foreach (string arg in args)
            {
                if (arg.Equals("/fds=false"))
                {
                    UseFDS = false;
                    break;
                }
            }
        }

        protected override IList<string> UsageExamples()
        {
            IList<string> examples = base.UsageExamples();
            examples.Add("XXX.exe fixed /fds=false - use FixedDocument instead of FixedDocumentSequence.");
            return examples;
        }

        protected override IList<string> UsageParameters()
        {
            IList<string> usage = base.UsageParameters();
            usage.Add("[/fds=false] - run fixed test using FixedDocument instead of a FixedDocumentSequence.");
            return usage;
        }

		/// <summary>
		/// Where the window is setup.
		/// By default window is created with Content.  Called OnLoaded when 
		/// </summary>		
        [TestCase_Setup()]
		protected virtual void DoSetup()
		{
			SetupTestWindow();
			SetDocumentViewerContent();
			WholePageLayout();

			DoExtendedSetup();

            // Wait until load is complete.
            while (ViewerBase.Document != null && !ViewerBase.Document.DocumentPaginator.IsPageCountValid)
                DispatcherHelper.DoEvents();
		}
		
		protected void ClearDocumentViewerContent()
		{
			ViewerBase.Document = null;
		}

        /// <summary>
        /// Optional chance for subclasses to add additional setup steps to the default
        /// DoSetup steps.  If DoSetup is overridden than this method will be a no-op.
        /// </summary>
        protected virtual void DoExtendedSetup()
        {
            // Nothing by default.
        }

        protected void SetDocumentViewerContent(string document)
        {
            IDocumentPaginatorSource idps = null;
            if (!string.IsNullOrEmpty(document))
                idps = LoadContent(document);
            SetContent(idps);
        }

		/// <summary>
		/// Load and set the content of the test DocumentViewerBase.
		/// </summary>
		virtual protected void SetDocumentViewerContent()
		{
            SetContent(LoadTestDocument());
		}

		/// <summary>
		/// Load either Fixed or Flow content depending on ContentMode.
		/// </summary>
		/// <remarks>
		/// Each time that this method is called it will return a new IDocumentPaginator instance.
		/// </remarks>
		virtual protected IDocumentPaginatorSource LoadTestDocument()
		{
			IDocumentPaginatorSource content;
			if (ContentMode == TestMode.Fixed)
				content = FixedContent;
			else if (ContentMode == TestMode.Flow)
			{
				FlowDocument flowdoc = (FlowDocument) FlowContent;
                if (flowdoc != null)
                {
                    // Override defaults to buffer us from constantly changing defaults.
                    flowdoc.FontSize = 12;
                }

				content = flowdoc;
			}
			else
				throw new NotSupportedException("Only 'fixed' and 'flow' modes are supported.");
			return content;
		}

		#endregion Protected Methods

        #region Fields

        bool UseFDS = true;

        #endregion
    }
}	

