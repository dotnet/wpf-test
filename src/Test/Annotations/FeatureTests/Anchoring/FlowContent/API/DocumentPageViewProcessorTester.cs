// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


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
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows.Media;
using System.Xml;
using System.Collections;
using System.Globalization;
using System.Windows.Annotations;
using Proxies.MS.Internal.Annotations;


namespace Avalon.Test.Annotations
{
	public class DocumentPageViewerProcessorTester : SelectionProcessorTester
	{
		public DocumentPageViewerProcessorTester(TestSuite suite, SelectionProcessor processor)
			: base(suite, processor)
		{
            // nothing.
		}

		/// <summary>
		/// Verify that MergeSelections always returns false and null selection.
		/// </summary>
		public void VerifyMergeSelections(DependencyObject documentPageView1, DependencyObject documentPageView2) 
		{
			object newSelection;
			suite.Assert("Verify MergeSelections returns false.", !processor.MergeSelections(documentPageView1, documentPageView2, out newSelection));
			suite.AssertNull("Verify newSelection is null.", newSelection);
		}

		/// <summary>
		/// Verify that DocumentGridSelectionProcessor.GenerateLocatorParts returns a single DocumentGridLocatorPart with the
		/// correct Offset and Length.
		/// </summary>
		public void VerifyGenerateLocatorParts(object selection, DependencyObject startNode, int expectedStartOffset, int expectedEndOffset)
		{
			IList<ContentLocatorPart> locators = processor.GenerateLocatorParts(selection, startNode);
			suite.AssertNotNull("Verify locators are not null.", locators);
			suite.AssertEquals("Verify number of locators.", 1, locators.Count);

			ContentLocatorPart locatorPart = locators[0] as ContentLocatorPart;

            string value = locatorPart.NameValuePairs[TextSelectionProcessor.SegmentAttribute + "0"];
            string[] values = value.Split(new char[] { ',' });
			suite.printStatus("ContentLocatorPart - StartOffset='" + values[0] + "' EndOffset='" + values[1] + "'.");
			suite.AssertEquals("Verify StartOffset.", expectedStartOffset.ToString(NumberFormatInfo.InvariantInfo), values[0]);
            suite.AssertEquals("Verify EndOffset.", (expectedEndOffset).ToString(NumberFormatInfo.InvariantInfo), values[1]);
		}

        /// <summary>
        /// Create a valid DocumentGridLocatorPart;
        /// </summary>
        public static ContentLocatorPart ValidLocatorPart(int start, int end)
        {
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("CharacterRange", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
			part.NameValuePairs.Add(TextSelectionProcessor.SegmentAttribute+"0", start.ToString(NumberFormatInfo.InvariantInfo)+","+end.ToString(NumberFormatInfo.InvariantInfo));
			part.NameValuePairs.Add(TextSelectionProcessor.CountAttribute, "1");
            part.NameValuePairs.Add(TextSelectionProcessor.IncludeOverlaps, Boolean.TrueString);
            return part;
        }
	}
}

