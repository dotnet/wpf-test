// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Tests of the Annotations V1 public api.  Therefore these
//				 tests do NOT use proxy namespaces.

using Annotations.Test;
using Annotations.Test.Framework;
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
	/// BVTs of 'Disable(DocumentViewer)' API.
	/// </summary>
	public class IsEnabledSuite_BVT : ADocumentViewerSuite
	{
		// ----------------------------------------------------------------------------------
		//                                 TEST METHODS
		// ----------------------------------------------------------------------------------

		/// <summary>
		/// AnnotationService.IsEnabled:
		///		False for DocumentViewer that is not enabled.
		/// </summary>
        [Keywords("Setup_SanitySuite")]
        [Priority(0)]
        void isenabled2()
		{
			DocumentViewerTree tree = new DocumentViewerTree();
			AnnotationService service = new AnnotationService(tree.DocumentViewer);
			Assert("Verify !IsEnabled.", !service.IsEnabled);
			passTest("Verified that IsEnabled returns false is initially.");
		}

		/// <summary>
		/// AnnotationService.IsEnabled:
		///		True for DocumentViewer that is enabled.
		/// </summary>
        [Priority(0)]
        void isenabled3()
		{
			DocumentViewerTree tree = new DocumentViewerTree();
			AsyncTestScript script = new AsyncTestScript();
			script.Add("EnableService", new object[] { tree.DocumentViewer, new MemoryStream() });
			script.Add("VerifyServiceEnabled", new object[] { tree.DocumentViewer });
			new AsyncTestScriptRunner(this).Run(script);
		}
	}
}

