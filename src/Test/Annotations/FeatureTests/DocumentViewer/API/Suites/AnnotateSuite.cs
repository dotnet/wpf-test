// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Tests of the Annotations V1 public api.  Therefore these
//				 tests do NOT use proxy namespaces.

using Annotations.Test;
using Annotations.Test.Framework;				// TestSuite.
using Annotations.Test.Reflection;

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Xml;
using System.Threading;
using System.Windows.Threading;

using System.Windows.Annotations;

namespace Avalon.Test.Annotations.Suites
{
	/// <summary>
	/// BVTs of 'CreateXXXForSelection(service, ...)' API.
	/// </summary>
    [TestDimension("flow")]
	public class AnnotateSuite_BVT : ADocumentViewerSuite
	{
		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// AnnotationHelper.CreateXXXForSelection:
		///		Viewer = DocumentViewer with no selection
		///		annotationType = valid type
		/// </summary>        
        [Keywords("Setup_SanitySuite")]
        [Priority(0)]
        void annotate4()
		{
			DocumentViewerTree tree = new DocumentViewerTree(contentType);
			tree._navWindow.Show();
			EnableService(tree.DocumentViewer, new MemoryStream());
			VerifyCreateAnnotationFails(tree.DocumentViewer, annotationType, typeof(InvalidOperationException));
			passTest("Exception for default selection.");		
		}

		// ----------------------------------------------------------------------------------
		//                                 PRIVATE METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// Parse args that are passed to this test and determine what annotation type and content to use.
		/// </summary>
		/// <param name="args">Command line args.</param>
		/// <param name="annotationType">Fully qualified type name of annotation to test.</param>
		/// <param name="contentType">Type of content to test with (e.g. Fixed or Flow).</param>
		[TestCase_Helper()]
        public override void ProcessArgs(string[] args)
        {
     	     base.ProcessArgs(args);

			// Defaults.
			annotationType = AnnotationMode.StickyNote;
			contentType = ContentType.Flow;

            foreach (string arg in args)
            {
                if (arg.Equals("fixed")) 
                {
                    contentType = ContentType.Fixed;
                    break;
                }
            }
		}

        AnnotationMode annotationType;
        ContentType contentType;
	}
}

