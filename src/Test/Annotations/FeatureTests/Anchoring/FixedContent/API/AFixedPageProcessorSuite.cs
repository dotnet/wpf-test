// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;

using Annotations.Test.Framework;


using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;

/// <summary>
/// AFixedContentSuite provides all the general, common functionality needed in the 
/// test suite for annotating fixed paginated content.  
/// Abstract base class for FixedContent test suite (API tests).
/// </summary>
namespace Avalon.Test.Annotations
{
	public abstract class AFixedPageProcessorSuite : AFixedContentSuite 
	{
		#region Protected Methods

        protected override void DoSetup()
        {
            base.DoSetup();
            fixedPageProcessor = new FixedPageProcessor(AnnotationService.GetService(ViewerBase).LocatorManager);
        }

		#endregion Protected Methods

		#region Protected Fields

		protected FixedPageProcessor fixedPageProcessor;		

		#endregion Protected Fields
	}
}

