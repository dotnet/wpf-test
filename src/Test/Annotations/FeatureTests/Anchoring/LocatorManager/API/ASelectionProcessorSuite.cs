// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the SelectionProcessorSuite test suite.  This provides
//  some common functions and builds the trees used by the SelectionProcessorSuite tests.
//  
//  The SelectionProcessor test suite consists of testing the following API and 
//  functionality:
//
//  - GetSelectionProcessor
//		GetSelectionProcessor takes an object type and returns the SelectionProcessor
//		that is registered with the LocatorManager for this type
//  - GetSelectionProcessorForLocatorPart
//		GetSelectionProcessorForLocatorPart takes in a ContentLocatorPart and returns
//		the SelectionProcessor that deals with this type of ContentLocatorPart
//  - RegisterSelectionProcessor
//		RegisterSelectionProcessor adds a new SelectionProcessor to the list of 
//		recognized processors used by the LocatorManager when generating/resolving Locators
//  - RegisterAndGetSelectionProcessor
//		Registers a SelectionProcessor (programmatically or by default) and calls
//		GetSelectionProcessor to check if the right processor is returned for certain
//		object types

using System;
using System.Windows;
using System.Windows.Controls;

using System.Xml;
using System.Collections;
using System.Collections.Generic;

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
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;
using Proxies.MS.Internal.Annotations;					// TestSuite.


namespace Avalon.Test.Annotations
{
	public abstract class ASelectionProcessorSuite : AAnchoringAPITests
	{
		#region globals
		protected SelectionProcessor result = null;
		#endregion

		// ---------------------------------------------------------------------------------------------
		//						HELPER METHODS: CHECK SELECTIONPROCESSOR TYPE
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks that the SelectionProcessor has a null value
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyNullReturned(SelectionProcessor s)
		{
			if (s != null)
				failTest("Invalid return value.  Expecting null");
			else
				passTest("Null correctly returned");
		}

		/// <summary>
		/// Checks that the SelectionProcessor is a TreeNodeSelectionProcessor
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyTreeNodeSelectionProcessorReturned(SelectionProcessor s)
		{
			if (s is TreeNodeSelectionProcessor)
				passTest("TreeNodeSelectionProcessor correctly returned");
			else if (s == null)
				failTest("TreeNodeSelectionProcessor expected.  Null returned");
			else
				failTest("TreeNodeSelectionProcessor expected.  Invalid SelectionProcessor returned\n");
		}

		/// <summary>
		/// Checks that the SelectionProcessor is a TextSelectionProcessor
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyTextSelectionProcessorReturned(SelectionProcessor s)
		{
			if (s is TextSelectionProcessor)
				passTest("TextSelectionProcessor correctly returned");
			else if (s == null)
				failTest("TextSelectionProcessor expected.  Null returned");
			else
				failTest("TextSelectionProcessor expected.  Invalid SelectionProcessor returned");
		}

		/// <summary>
		/// Checks that the SelectionProcessor is a DynamicSelectionProcessor
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyDynamicSelectionProcessorReturned(SelectionProcessor s)
		{
			if (s is DynamicSelectionProcessor)
				passTest("DynamicSelectionProcessor correctly returned");
			else if (s == null)
				failTest("DynamicSelectionProcessor expected.  Null returned");
			else
				failTest("DynamicSelectionProcessor expected.  Invalid SelectionProcessor returned\n");
		}

		/// <summary>
		/// Checks that the SelectionProcessor is of type sType
		/// </summary>
		/// <param name="s">processor to check</param>
		/// <param name="sType">expected Type for the processor</param>
		protected void VerifyCustomSelectionProcessorReturned(SelectionProcessor s, Type sType)
		{
			if (s.GetType().Equals(sType))
				passTest(sType.ToString() + " correctly returned");
			else if (s == null)
				failTest(sType.ToString() + " expected.  Null returned");
			else
				failTest(sType.ToString() + " expected.  " + s.ToString() + " returned");
		}


		// ---------------------------------------------------------------------------------------------
		//									CUSTOM SELECTION PROCESSORS
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Custom SelectionProcessor with a list of pre-defined recognized ContentLocatorBase types
		/// </summary>
		protected class CustomSelectionProcessor : SelectionProcessor
		{
			public CustomSelectionProcessor(LocatorManager manager) : base(manager)
			{
			}

			public override bool MergeSelections(object selection1, object selection2, out object newSelection)
			{
				newSelection = null;
				return false;
			}

			public override IList<DependencyObject> GetSelectedNodes(object selection)
			{
				return null;
			}

			public override UIElement GetParent(object selection)
			{
				return null;
			}

			public override Point GetAnchorPoint(object selection)
			{
				return new Point();
			}

			public override IList<ContentLocatorPart> GenerateLocatorParts(object selection, DependencyObject startNode)
			{
				return null;
			}

			public override object ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out AttachmentLevel attachementLevel)
			{
				attachementLevel = AttachmentLevel.Unresolved;
				return null;
			}

			public override XmlQualifiedName[] GetLocatorPartTypes()
			{
				return LocatorPartTypeNames;
			}

			// ContentLocatorBase part types understood by this processor
			protected XmlQualifiedName[] LocatorPartTypeNames = new XmlQualifiedName[] {
                new XmlQualifiedName("CAFTest", "http://schemas.microsoft.com/caf2"), 
				new XmlQualifiedName("CAFTest2", "http://schemas.microsoft.com/caf2"), 
				new XmlQualifiedName("Button"), 
				new XmlQualifiedName("TextRange"), 
				new XmlQualifiedName("TextBoxSP", "http://schemas.microsoft.com/caf2")
            };
		}

		/// <summary>
		/// Custom SelectionProcessor with a null list of recognized ContentLocatorBase part types
		/// </summary>
		protected class CustomSelectionProcessor_NullLocatorPartTypes : CustomSelectionProcessor
		{
			public CustomSelectionProcessor_NullLocatorPartTypes(LocatorManager manager) : base(manager)
			{
				// ContentLocatorBase part types understood by this processor
				LocatorPartTypeNames = null;
			}
		}

		/// <summary>
		/// Custom SelectionProcessor with an empty array of recognized ContentLocatorBase part types
		/// </summary>
		protected class CustomSelectionProcessor_EmptyLocatorPartTypes : CustomSelectionProcessor
		{
			public CustomSelectionProcessor_EmptyLocatorPartTypes(LocatorManager manager) : base(manager)
			{
				// ContentLocatorBase part types understood by this processor (new array, no values)
				LocatorPartTypeNames = new XmlQualifiedName[0];
			}
		}

	}		// end of ASelectionProcessorSuite class

}			// end of namespace

