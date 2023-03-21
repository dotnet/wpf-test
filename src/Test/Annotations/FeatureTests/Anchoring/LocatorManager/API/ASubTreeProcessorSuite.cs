// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the SubTreeProcessorSuite test suite.  This provides
//  some common functions and builds the trees used by the SubTreeProcessorSuite tests.
//  
//  The SubTreeProcessor test suite consists of testing the following API and 
//  functionality:
//
//  - GetSubTreeProcessor
//		GetSubTreeProcessor takes in a node on a tree and returns the SubTreeProcessor in
//		effect for the node's subtree (i.e. all its descendant nodes but *not* itself)
//  - GetSubTreeProcessorForLocatorPart
//		GetSubTreeProcessorForLocatorPart takes in a ContentLocatorPart and finds the specific  
//		SubTreeProcessor that deals with this type of ContentLocatorPart
//  - RegisterSubTreeProcessor
//		RegisterSubTreeProcessor adds a new SubTreeProcessor to the list of
//		recognized processors used by the LocatorManager when generating/resolving Locators
//  - RegisterAndGetSubTreeProcessor
//		Registers a SubTreeProcessor (programmatically or by default) and calls
//		GetSubTreeProcessor at diff points in the tree to check if the right processor is returned
//  - SubTreeProcessorIdProperty
//		This tests whether the value set for this property is the same value returned

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
	public abstract class ASubTreeProcessorSuite : AAnchoringAPITests
	{
		#region globals
		protected Canvas a = null;
		protected Canvas b = null;
		protected Canvas c = null;
		protected Canvas d = null;
		protected Canvas e = null;
		protected TextBlock f = null;
		protected TextBlock g = null;
		protected Canvas h = null;

		protected SubTreeProcessor result = null;
		#endregion

		// ---------------------------------------------------------------------------------------------
		//									BUILD VARIATIONS OF TREE
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Builds a tree made up of a single Canvas node
		/// </summary>
		protected void BuildSingleNodeTree()
		{
			a = new Canvas();
		}

		/// <summary>
		/// Builds a tree of depth 2, composed entirely of Canvas objects.
		/// </summary>
		protected void BuildSimpleTree()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();

			a.Children.Add(b);
			a.Children.Add(c);
		}

		/// <summary>
		/// Builds a tree of depth 3, composed entirely of Canvas objects.
		/// </summary>
		protected void BuildComplexTree()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(d);
			b.Children.Add(e);
		}

		/// <summary>
		/// Builds a tree for the functional tests (RegisterAndGetSubTreeProcessor)
		/// </summary>
		protected void BuildFunctionalTree()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			f = new TextBlock();
			g = new TextBlock();
			h = new Canvas();

			a.Children.Add(b);
			a.Children.Add(c);
			c.Children.Add(d);
			c.Children.Add(e);
			d.Children.Add(f);
			d.Children.Add(g);
			e.Children.Add(h);
		}


		// ---------------------------------------------------------------------------------------------
		//						HELPER METHODS: CHECK SUBTREEPROCESSOR TYPE
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Checks that the SubTreeProcessor has a null value
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyNullReturned(SubTreeProcessor s)
		{
			if (s != null)
				failTest("Invalid return value.  Expecting null");
			else
				passTest("Null correctly returned");
		}

		/// <summary>
		/// Checks that the SubTreeProcessor is a DataIdProcessor
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyDataIdProcessorReturned(SubTreeProcessor s)
		{
			if (s is DataIdProcessor)
				passTest("DataIdProcessor correctly returned");
			else if (s == null)
				failTest("DataIdProcessor expected.  Null returned");
			else
				failTest("DataIdProcessor expected.  Invalid SubTreeProcessor returned\n");
		}

		/// <summary>
		/// Checks that the SubTreeProcessor is a TextFingerprintProcessor
		/// </summary>
		/// <param name="s">processor to check</param>
		protected void VerifyTextFingerprintProcessorReturned(SubTreeProcessor s)
		{
			if (s is TextFingerprintProcessor)
				passTest("TextFingerprintProcessor correctly returned");
			else if (s == null)
				failTest("TextFingerprintProcessor expected.  Null returned");
			else
				failTest("TextFingerprintProcessor expected.  Invalid SubTreeProcessor returned");
		}

		/// <summary>
		/// Checks that the SubTreeProcessor is of type sType
		/// </summary>
		/// <param name="s">processor to check</param>
		/// <param name="sType">expected Type for the processor</param>
		protected void VerifyCustomSubTreeProcessorReturned(SubTreeProcessor s, Type sType)
		{
			if (s.GetType().Equals(sType))
				passTest(sType.ToString() + " correctly returned");
			else if (s == null)
				failTest(sType.ToString() + " expected.  Null returned");
			else
				failTest(sType.ToString() + " expected.  " + s.ToString() + " returned");
		}

		/// <summary>
		/// Checks that the SubTreeProcessor set on nodeName is of type sType
		/// </summary>
		/// <param name="s">processor to check</param>
		/// <param name="sType">expected type for processor</param>
		/// <param name="nodeName">name of node</param>
		/// <returns>true if the processor is the correct type, false otherwise</returns>
		protected bool CheckSubTreeProcessorTypeForNode(SubTreeProcessor s, Type sType, string nodeName)
		{
			if (!(s.GetType().Equals(sType)))
			{
				failTest(sType.ToString() + " expected for " + nodeName + ".  Invalid SubTreeProcessor returned");
				return false;
			}

			return true;
		}


		// ---------------------------------------------------------------------------------------------
		//									CUSTOM SUBTREE PROCESSORS
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Custom SubTreeProcessor with a list of pre-defined recognized ContentLocatorBase types
		/// </summary>
		public class CustomSubTreeProcessor : SubTreeProcessor
		{
			public CustomSubTreeProcessor(LocatorManager manager) : base(manager)
			{
			}

			public override IList<IAttachedAnnotation> PreProcessNode(DependencyObject startNode, out bool calledProcessAnnotations)
			{
				calledProcessAnnotations = false;
				return null;
			}

			public override ContentLocator GenerateLocator(PathNode node, out bool continueGenerating)
			{
				continueGenerating = false;
				return null;
			}

			public override DependencyObject ResolveLocatorPart(ContentLocatorPart locatorPart, DependencyObject startNode, out bool continueResolving)
			{
				continueResolving = false;
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
				new XmlQualifiedName("Paragraph"), 
				new XmlQualifiedName("MyID", "http://schemas.microsoft.com/caf2"), 
				new XmlQualifiedName("MyNewSP", "http://schemas.microsoft.com/caf2"), 
				new XmlQualifiedName("MyNewSP2", "http://schemas.microsoft.com/caf2")
            };
		}

		/// <summary>
		/// Custom SubTreeProcessor with a null list of recognized ContentLocatorBase part types
		/// </summary>
		protected class CustomSubTreeProcessor_NullLocatorPartTypes : CustomSubTreeProcessor
		{
			public CustomSubTreeProcessor_NullLocatorPartTypes(LocatorManager manager) : base(manager)
			{
				// ContentLocatorBase part types understood by this processor
				LocatorPartTypeNames = null;
			}
		}

		/// <summary>
		/// Custom SubTreeProcessor with an empty array of recognized ContentLocatorBase part types
		/// </summary>
		protected class CustomSubTreeProcessor_EmptyLocatorPartTypes : CustomSubTreeProcessor
		{
			public CustomSubTreeProcessor_EmptyLocatorPartTypes(LocatorManager manager) : base(manager)
			{
				// ContentLocatorBase part types understood by this processor (new array, no values)
				LocatorPartTypeNames = new XmlQualifiedName[0];
			}
		}


	}		// end of ASubTreeProcessorSuite class

}			// end of namespace

