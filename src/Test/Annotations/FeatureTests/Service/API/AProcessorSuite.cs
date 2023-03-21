// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Abstract base class that contains common helper methods
//				for bvt and pri1 tests for AnnotationService processor
//				API.

using Annotations.Test.Framework;				// TestSuite.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading; using System.Windows.Threading;						// DispatcherOperationCallback.
using System.Windows;
using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Proxies.MS.Internal.Annotations.Component;
using System.Windows.Annotations.Storage;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Annotations;							// XmlQualifiedName.

namespace Avalon.Test.Annotations
{
	abstract public class AProcessorSuite : TestSuite
	{
		/// <summary>
		/// SubTreeProcessor that keeps track of how many times its GenerateLocator
		/// method is called.  Is useful for testing exactly which processor an
		/// AnnotationService uses in different situations.
		/// </summary>
		protected class CountingSubTreeProcessor : DataIdProcessor
		{
			int count = 0;
			public CountingSubTreeProcessor(LocatorManager manager) : base(manager)
			{
				// Nothing.
			}

			public override ContentLocator GenerateLocator(PathNode node, out bool continueGenerating)
			{
				count++;
				return base.GenerateLocator(node, out continueGenerating);
			}

			public int CallCount() { return count; }
		}

		/// <summary>
		/// Subclass of TreeNodeSelectionProcessor.
		/// Keeps track of the number of times its GetSelectedNodes methods is called.  
		/// This is useful when testing the AnnotationService is using the correct
		/// Selection processor for different types.
		/// </summary>
		protected class CountingSelectionProcessor : TreeNodeSelectionProcessor
		{
			int count;

			public int CallCount
			{
				get
				{
					return count;
				}
			}

			public CountingSelectionProcessor(LocatorManager manager) : base(manager) { }

			public override IList<DependencyObject> GetSelectedNodes(Object selection)
			{
				count++;
				return base.GetSelectedNodes(selection);
			}
		}
	}
}

