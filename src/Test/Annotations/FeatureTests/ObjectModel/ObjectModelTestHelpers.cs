// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Class containing static helper methods that are useful
//				 across a number of Annotation TestSuites.

using Annotations.Test.Framework; // TestSuite.

using System;
using System.Xml;

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

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Listen for AnchorChanged events for the given Annotation.  Collect
	/// and provide an api for verifying eventing statistics.
	/// </summary>
	public class AnchorEventListener : AnnotationEventListener
	{		
		public AnchorEventListener(TestSuite suite, Annotation anno) : base(suite)
		{
			anno.AnchorChanged += new AnnotationResourceChangedEventHandler(HandleAnchorChanged);
		}

		public void HandleAnchorChanged(object sender, AnnotationResourceChangedEventArgs args)
		{
			if (args.Action == AnnotationAction.Added)
				nAddEvents++;
			if (args.Action == AnnotationAction.Removed)
				nRemovedEvents++;
			if (args.Action == AnnotationAction.Modified)
				nModifyEvents++;

			lastChangedAnchor = args.Resource;
		}

		/// <summary>
		/// Fail test if AnnotationAuthorChangedEventArgs.Author of the last event received is
		/// not the same object as the given argument. 
		/// </summary>
		public void VerifyLastChangedAnchor(object anchor)
		{
			suite.Assert("Verify last 'AnchorChangedEventArgs.Anchor'.", anchor == lastChangedAnchor);
			suite.printStatus("Identity of last anchor changed confirmed.");
		}

		private object lastChangedAnchor = null;
	}

	/// <summary>
	/// Listen for AuthorChanged events for the given Annotation.  Collect
	/// and provide an api for verifying eventing statistics.
	/// </summary>
	class AuthorEventListener : AnnotationEventListener
	{
		private object lastChangedAuthor = null;

		public AuthorEventListener(TestSuite suite, Annotation anno) : base(suite)
		{
			anno.AuthorChanged += new AnnotationAuthorChangedEventHandler(HandleAuthorChanged);
		}

		public void HandleAuthorChanged(object sender, AnnotationAuthorChangedEventArgs args)
		{
			if (args.Action == AnnotationAction.Added)
				nAddEvents++;
			if (args.Action == AnnotationAction.Removed)
				nRemovedEvents++;
			if (args.Action == AnnotationAction.Modified)
				nModifyEvents++;

			lastChangedAuthor = args.Author;
		}

		/// <summary>
		/// Fail test if AnnotationAuthorChangedEventArgs.Author of the last event received is
		/// not the same object as the given argument. 
		/// </summary>
		public void VerifyLastChangedAuthor(object author)
		{
			suite.Assert("Verify last 'AnnotationAuthorChangedEventArgs.Author'.", author == lastChangedAuthor);
			suite.printStatus("Identity of last author changed confirmed.");
		}
	}

	abstract public class AnnotationEventListener
	{
		public AnnotationEventListener(TestSuite suite)
		{
			this.suite = suite;
		}

		/// <summary>
		/// Fail test if event counts do not match these expected values.
		/// </summary>
		public virtual void VerifyEventCount(int nAdd, int nRemoved, int nModify)
		{
			if (nAdd == nAddEvents && nRemoved == nRemovedEvents && nModify == nModifyEvents)
				suite.printStatus("Number of AnchorChanged events verified.");
			else
				suite.failTest("Expected " + nAdd + " Add, " + nRemoved + " Removed, and " + nModify + " Modify events but received " + nAddEvents + " Add, " + nRemovedEvents + " Remove, " + nModifyEvents + " Modify events.");
		}

		/// <summary>
		/// Asserts that the given number of events have occurred since the last time this method was called.
		/// </summary>
		/// <param name="nAdd"></param>
		/// <param name="nRemove"></param>
		/// <param name="nModify"></param>
		public virtual void VerifyChangeInEvents(string status, int nAdd, int nRemove, int nModify)
		{
			int addDelta = nAddEvents - lastAddCountChecked;
			int removeDelta = nRemovedEvents - lastRemovedCountChecked;
			int modifyDelta = nModifyEvents - lastModifyCountChecked;

			if (addDelta == nAdd && removeDelta == nRemove && modifyDelta == nModify)
				suite.printStatus(status);
			else
				suite.failTest("Expected " + nAdd + " Add, " + nRemove + " Removed, and " + nModify + " Modify events since last call to 'VerifyChangeInEvents' but received " + addDelta + " Add, " + removeDelta + " Remove, " + modifyDelta + " Modify events.");

			lastAddCountChecked = nAddEvents;
			lastRemovedCountChecked = nRemovedEvents;
			lastModifyCountChecked = nModifyEvents;
		}

		protected TestSuite suite;

		protected int nAddEvents = 0;
		protected int nRemovedEvents = 0;
		protected int nModifyEvents = 0;

		protected int lastAddCountChecked = 0;
		protected int lastRemovedCountChecked = 0;
		protected int lastModifyCountChecked = 0;
	}
} 

