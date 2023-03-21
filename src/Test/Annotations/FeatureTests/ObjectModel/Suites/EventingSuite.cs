// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Annotations;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Avalon.Test.Annotations.Suites
{
    public class EventingSuite : TestSuite
    {
        #region BVT TESTS
        #endregion BVT TESTS

        #region PRIORITY TESTS

        [Priority(1)]
        protected void eventing_authors()
		{
			Annotation annot = new Annotation();
			string author1 = "Derek";
			string author2 = "Ralph";
			AnnotationEventListener eventListener = new AuthorEventListener(this, annot);
			CollectionEventTester<string>.TestCollectionEvents(eventListener, annot.Authors, author1, author2);
			passTest("Verified Author collection events.");
		}

        [Priority(1)]
        protected void eventing_anchors()
		{
			Annotation annot = new Annotation();			
			AnnotationResource anchor1 = AnnotationTestHelper.CreateAnchor("anchor1", "nsp");
			AnnotationResource anchor2 = AnnotationTestHelper.CreateAnchor("anchor2", "nsp");
			AnnotationEventListener eventListener = new AnchorEventListener(this, annot);
			CollectionEventTester<AnnotationResource>.TestCollectionEvents(eventListener, annot.Anchors, anchor1, anchor2);
			passTest("Verified Anchor collection events.");
		}

        [Priority(1)]
        protected void eventing_contents()
		{
			Annotation annot = new Annotation();
			AnnotationResource anchor = AnnotationTestHelper.CreateAnchor("anchor1", "nsp");
			XmlElement content1 = AnnotationTestHelper.CreateContent("content1", "this is only a test");
			XmlElement content2 = AnnotationTestHelper.CreateContent("content2", "more test content");
			annot.Anchors.Add(anchor);
			AnnotationEventListener eventListener = new AnchorEventListener(this, annot);
			SubCollectionEventTester<XmlElement>.TestCollectionEvents(eventListener, annot.Anchors[0].Contents, content1, content2);
			passTest("Verified Anchor.Contents collection events.");
		}

        [Priority(1)]
        protected void eventing_locator()
		{
			Annotation annot = new Annotation();
			AnnotationResource anchor = AnnotationTestHelper.CreateAnchor("anchor1", "nsp");			
			ContentLocator loc1 = AnnotationTestHelper.CreateLPS("lsp1", "nsp", "content1");
			ContentLocator loc2 = AnnotationTestHelper.CreateLPS("lsp2", "nsp", "content2");
			annot.Anchors.Add(anchor);
			AnnotationEventListener eventListener = new AnchorEventListener(this, annot);
			SubCollectionEventTester<ContentLocatorBase>.TestCollectionEvents(eventListener, annot.Anchors[0].ContentLocators, loc1, loc2);
			passTest("Verified Anchor.Locators collection events.");
		}

        [Priority(1)]
        protected void eventing_locatorgroup()
		{
			Annotation annot = new Annotation();
			AnnotationResource anchor = AnnotationTestHelper.CreateAnchor("anchor1", "nsp");
			ContentLocatorGroup group = new ContentLocatorGroup();
			ContentLocator loc1 = AnnotationTestHelper.CreateLPS("lsp1", "nsp", "content1");
			ContentLocator loc2 = AnnotationTestHelper.CreateLPS("lsp2", "nsp", "content2");
			annot.Anchors.Add(anchor);
			annot.Anchors[0].ContentLocators.Add(group);
			AnnotationEventListener eventListener = new AnchorEventListener(this, annot);
			SubCollectionEventTester<ContentLocator>.TestCollectionEvents(eventListener, group.Locators, loc1, loc2);
			passTest("Verified Anchor.Locators collection events.");
		}

        [Priority(1)]
        protected void eventing_locatorparts()
		{
			Annotation annot = new Annotation();
			AnnotationResource anchor = AnnotationTestHelper.CreateAnchor("anchor1", "nsp");
			ContentLocator loc = AnnotationTestHelper.CreateLPS("lsp1", "nsp", "content1");
			ContentLocatorPart lp1 = AnnotationTestHelper.CreateLocatorPart("lp1", "nsp", "content1");
			ContentLocatorPart lp2 = AnnotationTestHelper.CreateLocatorPart("lp2", "nsp", "content2");
			annot.Anchors.Add(anchor);
			annot.Anchors[0].ContentLocators.Add(loc);
			AnnotationEventListener eventListener = new AnchorEventListener(this, annot);
			SubCollectionEventTester<ContentLocatorPart>.TestCollectionEvents(eventListener, loc.Parts, lp1, lp2);
			passTest("Verified ContentLocator.Parts collection events.");
        }

        #endregion PRIORITY TESTS
    }

	/// <summary>
	/// Test the number of events that get fired for different operations on a Collection of objects.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class SubCollectionEventTester<T>
	{
		public static void TestCollectionEvents(AnnotationEventListener _eventListener, Collection<T> collection, T content1, T content2)
		{
			T nullT = default(T);			
			collection.Add(content1);
			_eventListener.VerifyChangeInEvents("Add(X1)", 0, 0, 1);
			collection.Add(nullT);
			_eventListener.VerifyChangeInEvents("Add(null)", 0, 0, 1);
			collection[1] = content2;
			_eventListener.VerifyChangeInEvents("[1] = X2", 0, 0, 1);
			collection.Remove(content2);
			_eventListener.VerifyChangeInEvents("Remove(X2)", 0, 0, 1);
			collection[0] = nullT;
			_eventListener.VerifyChangeInEvents("[0] = null", 0, 0, 1);
			collection.Remove(nullT);
			_eventListener.VerifyChangeInEvents("Remove(null)", 0, 0, 1);	
		}
	}

	/// <summary>
	/// AnnotationResource has its own expected event count so it has its own class.
	/// </summary>
	class CollectionEventTester<T>
	{
		public static void TestCollectionEvents(AnnotationEventListener _eventListener, Collection<T> collection, T content1, T content2)
		{
			T nullT = default(T);
			collection.Add(content1);
			_eventListener.VerifyChangeInEvents("Add(X1)", 1, 0, 0);
			collection.Add(nullT);
			_eventListener.VerifyChangeInEvents("Add(null)", 1, 0, 0);
			collection[1] = content2;
			_eventListener.VerifyChangeInEvents("[1] = X2", 1, 1, 0);
			collection.Remove(content2);
			_eventListener.VerifyChangeInEvents("Remove(X2)", 0, 1, 0);
			collection[0] = nullT;
			_eventListener.VerifyChangeInEvents("[0] = null", 1, 1, 0);
			collection.Remove(nullT);
			_eventListener.VerifyChangeInEvents("Remove(null)", 0, 1, 0);
		}
	}
}	

