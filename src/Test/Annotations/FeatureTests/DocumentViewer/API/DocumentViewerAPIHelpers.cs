// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Static helper methods for V1 Api tests.


using Annotations.Test.Reflection;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Annotations;
using System.Windows.Controls.Primitives;	// Use "Real" annotations namespace.

namespace Avalon.Test.Annotations.Suites
{
	public class DocumentViewerAPIHelpers
	{
		/// <summary>
		/// Return public AnnotationService for the given DocumentViewer.
		/// </summary>
		/// <returns>AnnotationService for DocumentViewer or null if none exists.</returns>
		public static AnnotationService GetService(DocumentViewerBase documentViewer)
		{
			return (AnnotationService)ReflectionHelper.InvokeStaticMethod(
				typeof(AnnotationService),
				"GetService",
				new Type[] { typeof(DependencyObject) },
				new object[] { documentViewer });
		}

		public static bool IsReady(DocumentViewerBase documentViewer)
        {
            AnnotationService service = GetService(documentViewer);
            return (Boolean) ReflectionHelper.GetProperty(service, "IsReady");
        }
	}
}

