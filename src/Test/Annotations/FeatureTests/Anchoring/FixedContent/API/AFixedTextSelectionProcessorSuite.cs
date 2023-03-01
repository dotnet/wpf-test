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
using System.Windows.Threading;

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
using System.Globalization;
using Proxies.MS.Internal.Annotations;

/// <summary>
/// AFixedContentSuite provides all the general, common functionality needed in the 
/// test suite for annotating fixed paginated content.  
/// Abstract base class for FixedContent test suite (API tests).
/// </summary>
namespace Avalon.Test.Annotations
{
	public abstract class AFixedTextSelectionProcessorSuite : AFixedContentSuite
	{
		#region Protected Methods

        protected override void DoSetup()
        {
            base.DoSetup();

            fixedTextSelectionProcessor = new FixedTextSelectionProcessor();
            processorTester = new FixedTextSelectionProcessorTester(this, fixedTextSelectionProcessor);
        }

		#endregion Protected Methods

		#region Protected Fields

		protected FixedTextSelectionProcessor fixedTextSelectionProcessor;
		protected FixedTextSelectionProcessorTester processorTester;

		#endregion Protected Fields
	}

	public class FixedTextSelectionProcessorTester : SelectionProcessorTester
	{
		public FixedTextSelectionProcessorTester(TestSuite suite, SelectionProcessor processor)
			:base(suite, processor)
		{
			// nothing.
		}

		/// <summary>
		/// Verify that selected nodes are of type FixedPageProxy and that they correspond to the 
		/// expected page numbers.
		/// </summary>
		override public void VerifyGetSelectedNodes(object selection, object[] expectedPageNumbers)
		{
			IList<DependencyObject> nodes = processor.GetSelectedNodes(selection);

			suite.AssertNotNull("Verify result is not null.", nodes);
			suite.AssertEquals("Verify number of selected nodes.", expectedPageNumbers.Length, nodes.Count);
			for (int i = 0; i < expectedPageNumbers.Length; i++)
			{
				FixedPageProxy fpp = nodes[i] as FixedPageProxy;
				suite.AssertNotNull("Verify type of node is of type FixedPageProxy.", fpp);
				suite.AssertEquals("Verify page number for FixedPageProxy " + i, expectedPageNumbers[i], fpp.Page);
			}
		}

		/// <summary>
		/// Verifies that the returned ContentLocatorPart has the same start/end points as the given FixedPageProxy.
		/// </summary>
		/// <param name="startNode">expected FixedPageProxy</param>
		public void VerifyGenerateLocatorParts(object selection, DependencyObject startNode)
		{
			FixedPageProxy fpp = startNode as FixedPageProxy;
            suite.Assert("Don't support testing multiple PointSegments.", fpp.Segments.Count <= 1);
            PointSegment segment = (fpp.Segments.Count == 0) ? new PointSegment(new Point(double.NaN, double.NaN),new Point(double.NaN, double.NaN)) : fpp.Segments[0];

			IList<ContentLocatorPart> lps = processor.GenerateLocatorParts(selection, fpp);
			suite.AssertEquals("Verify number of ContentLocatorParts.", 1, lps.Count);

            string value = lps[0].NameValuePairs["Segment0"];
            string[] values = value.Split(new char[] { ',' });

            if (double.IsNaN(segment.Start.X) || double.IsNaN(segment.Start.Y))
			{
				suite.AssertEquals("StartX should be empty string for NaN.", values[0], "");
				suite.AssertEquals("StartY should be empty string for NaN.", values[1], "");
			}
			else
			{
                suite.AssertEquals("Verify ContentLocatorPart has correct StartX.", segment.Start.X.ToString(NumberFormatInfo.InvariantInfo), values[0]);
                suite.AssertEquals("Verify ContentLocatorPart has correct StartY.", segment.Start.Y.ToString(NumberFormatInfo.InvariantInfo), values[1]);
			}
            if (double.IsNaN(segment.End.X) || double.IsNaN(segment.End.Y))
			{
				suite.AssertEquals("EndX should be empty string for NaN.", values[2], "");
				suite.AssertEquals("EndY should be empty string for NaN.", values[3], "");
			}
			else
			{
                suite.AssertEquals("Verify ContentLocatorPart has correct EndX.", segment.End.X.ToString(NumberFormatInfo.InvariantInfo), values[2]);
                suite.AssertEquals("Verify ContentLocatorPart has correct EndY.", segment.End.Y.ToString(NumberFormatInfo.InvariantInfo), values[3]);
			}
		}

		/// <summary>
		/// Verify that ContentLocatorPart resolves to a TextAnchor with the given expectedText.
		/// </summary>
		/// <param name="expectedText">string that resulting TextAnchor should contain.</param>
		override public void VerifyResolveLocatorPart(ContentLocatorPart part, DependencyObject node, object expectedText, AttachmentLevel expectedAttachmentLevel)
		{
			AttachmentLevel attachmentLevel;
			object result = processor.ResolveLocatorPart(part, node, out attachmentLevel);

			if (expectedText == null)
				suite.AssertNull("Result should be null but is: '" + result + "'", result);
			else
			{
				suite.AssertNotNull("Result should not be null.", result);
				suite.Assert("Result should be of type TextAnchor.", AnnotationTestHelper.IsTextAnchor(result));
                suite.AssertEquals("Check resolved text.", SelectionModule.PrintFriendlySelection((string)expectedText), SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(result)));
			}
		}
	}
}

