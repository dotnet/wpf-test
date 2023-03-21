// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Annotations;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Annotations.Storage;
using Avalon.Test.Annotations.Suites;
using Annotations.Test;

namespace Avalon.Test.Annotations.Suites
{
    public class EnableSuite : ADocumentViewerSuite
    {
        #region BVT TESTS
    
        /// <summary>
        /// AnnotationService Enable:
        ///		Viewer=viewer has no content
        ///		stream = valid stream.
        /// </summary>
        [Keywords("Setup_SanitySuite")]
        [Priority(0)]
        void enable3()
        {
            DocumentViewerTree tree = new DocumentViewerTree();

            AsyncTestScript script = new AsyncTestScript();
            script.Add("EnableService", new object[] { tree.DocumentViewer, new MemoryStream() });
            script.Add("VerifyServiceEnabled", new object[] { tree.DocumentViewer });
            new AsyncTestScriptRunner(this).Run(script);
        }

        /// <summary>
        /// AnnotationService Enable:
        ///		Viewer=viewer has content
        ///		stream = valid stream.
        /// </summary>
        [Priority(0)]
        void enable4()
        {
            DocumentViewerTree tree = new DocumentViewerTree(ContentType.Flow);

            AsyncTestScript script = new AsyncTestScript();
            script.Add("EnableService", new object[] { tree.DocumentViewer, new MemoryStream() });
            script.Add("VerifyServiceEnabled", new object[] { tree.DocumentViewer });
            new AsyncTestScriptRunner(this).Run(script);
        }

        /// <summary>
        /// AnnotationService Enable:
        ///   Viewer=already enabled DocumentViewer.
        /// </summary>
        [Priority(0)]
        void enable8()
        {
            DocumentViewerTree tree = new DocumentViewerTree();
            AnnotationService service = new AnnotationService(tree.DocumentViewer);
            service.Enable(new XmlStreamStore(new MemoryStream()));
            try
            {
                AnnotationService service2 = new AnnotationService(tree.DocumentViewer);
                service2.Enable(new XmlStreamStore(new MemoryStream()));
            }
            //catch (InvalidOperationException e)
            catch (InvalidOperationException)
            {
                passTest("Excepted exception occurred.");
            }
            failTest("Expected exception for 2nd enable but none occurred.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        [TestCase_Cleanup]
        void cleanup()
        {
            // do nothing.
        }

		/// <summary>
		/// Nested DocumentViewers:
		/// B inside A.
		/// Enable A.
		/// Enable B.
		/// Verify: InvalidOperationException.
		/// </summary>
        [Priority(1)]
        void enable10_1()
		{
			TestEnablingNestedViewers(true);
		}

		/// <summary>
		/// Nested DocumentViewers:
		/// B inside A.
		/// Enable B.
		/// Enable A.
		/// Verify: InvalidOperationException.
		/// </summary>
        [Priority(1)]
        void enable10_2()
		{
			TestEnablingNestedViewers(false);
		}

		/// <summary>
		/// Put B inside A.
		/// If enableOuterFirst -> enable A first.
		/// Else -> enable B first.
		/// Exception InvalidOperationException on second enable.
		/// </summary>
		/// <param name="enableOuterFirst"></param>
        [TestCase_Helper()]
		private void TestEnablingNestedViewers(bool enableOuterFirst)
		{
            DocumentViewerBase viewerA = new FlowDocumentPageViewer();
            DocumentViewerBase viewerB = new FlowDocumentPageViewer();

			viewerA.Document = WrapInFlowDocument(viewerB);

			AnnotationService serviceA = new AnnotationService(viewerA);
			AnnotationService serviceB = new AnnotationService(viewerB);

			AnnotationService first = (enableOuterFirst) ? serviceA : serviceB;
			AnnotationService second = (enableOuterFirst) ? serviceB : serviceA;

            first.Enable(StreamStore());
			VerifyEnableFails(second, typeof(InvalidOperationException));

			passTest("Enabling on nested DV fails.");
		}

        [TestCase_Helper()]
		private void VerifyEnableFails(AnnotationService service, Type expectedExceptionType)
		{
			bool exceptionOccurred = false;
			try
			{
                service.Enable(StreamStore());
			}
			catch (Exception e)
			{
				AssertEquals("Verify exception type.", expectedExceptionType, e.GetType());
				exceptionOccurred = true;
			}

			Assert("Expected exception.", exceptionOccurred);
		}

        [TestCase_Helper()]
		private FlowDocument WrapInFlowDocument(DocumentViewerBase viewer)
		{
			FlowDocument doc = new FlowDocument();
			Paragraph para = new Paragraph();
			((IAddChild)para).AddChild(viewer);
			((IAddChild)doc).AddChild(para);
			return doc;
		}

        [TestCase_Helper()]
        private AnnotationStore StreamStore()
        {
            return new XmlStreamStore(new MemoryStream());
        }

        #endregion PRIORITY TESTS
    }
}

