// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Annotations;
using System.Xml;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations.Suites
{
    public class LocatorGroupSuite : TestSuite
    {
        #region BVT TESTS
        #endregion BVT TESTS

        #region PRIORITY TESTS
        
        #region Clone

        /// <summary>
		/// Parameters: ContentLocatorGroup with 1 ContentLocator.  	
		/// Verify: Deep copy with 1 Locators
		/// </summary>
        [Priority(1)]
        protected void locatorgroup_clone1()
		{
			ContentLocatorGroup original = AnnotationTestHelper.CreateLocatorGroup(1, 15, "A");
			VerifyClone(original);
			passTest("Verified clone group with 1 ContentLocator.");
		}

		/// <summary>
		/// Parameters: ContentLocatorGroup with N locators.  	
		/// Verify: Deep copy with N locators
		/// </summary>
        [Priority(1)]
        protected void locatorgroup_clone2()
		{
			ContentLocatorGroup original = AnnotationTestHelper.CreateLocatorGroup(10, 1, "A");
			VerifyClone(original);
			passTest("Verified clone group with N ContentLocator.");
		}

		/// <summary>
		/// Parameters: empty ContentLocatorGroup
		/// Verify: Deep copy with 0 locators
		/// </summary>
        [Priority(1)]
        protected void locatorgroup_clone3()
		{
			ContentLocatorGroup original = AnnotationTestHelper.CreateLocatorGroup(0, 0, "A");
			VerifyClone(original);
			passTest("Verified clone group with 0 ContentLocator.");
		}

		private void VerifyClone(ContentLocatorGroup original)
		{
			ContentLocatorGroup clone = (ContentLocatorGroup)original.Clone();
			Assert("Verify clone is not the same object.", original != clone);
			Assert("Verify all elements are the same.", AnnotationTestHelper.LocatorSetsEqual(original, clone));
		}

		#endregion

		#region GetSchema

        [Priority(1)]
        protected void locatorgroup_getschema1()
		{
			AssertNull("Verify schema is null", new ContentLocatorGroup().GetSchema());
			passTest("Schema is null.");
		}

		#endregion

		#region Merge

		/// <summary>
		/// Parameters: Other=Null	
		/// Verify: Return this.
		/// </summary>
        [Priority(1)]
        protected void locatorgroup_merge1()
		{
			ContentLocatorGroup original = AnnotationTestHelper.CreateLocatorGroup(1, 1, "A");
			ContentLocatorGroup result = DoMerge(original, null);
			Assert("Verify result is 'this'.", original == result);
			passTest("Verified merge(null).");
		}

		/// <summary>
		/// Parameters: ContentLocatorGroup with N locators.
		///				Other=ContentLocator X	
		///	Verify: X appended to each ContentLocator.
		/// </summary>
        [Priority(1)]
        protected void locatorgroup_merge2()
		{
			ContentLocatorGroup groupA= AnnotationTestHelper.CreateLocatorGroup(2, 1, "A");
			ContentLocator locatorB = AnnotationTestHelper.CreateLocator(new string[] { "B" });
			ContentLocatorGroup result = DoMerge(groupA, locatorB);

			ContentLocator expectedLocators = AnnotationTestHelper.CreateLocator(new string[] { "A", "B" });
			AssertEquals("Verify number of locators.", 2, result.Locators.Count);
			foreach (ContentLocator loc in result.Locators)
				Assert("Verify equality.", AnnotationTestHelper.LocatorPartListsEqual(expectedLocators, loc));

			passTest("Verified merge(ContentLocator).");
		}

		/// <summary>
		/// Parameters: ContentLocatorGroup with 2 locators.
		///				Other=ContentLocatorGroup 3 locators	
		///	Verify: ContentLocatorGroup with 6 (2*3) locators.
		/// </summary>
        [Priority(1)]
        protected void locatorgroup_merge3()
		{
			ContentLocatorGroup groupA = AnnotationTestHelper.CreateLocatorGroup(2, 1, "A");
			ContentLocatorGroup groupB = AnnotationTestHelper.CreateLocatorGroup(3, 1, "C");
			ContentLocatorGroup result = DoMerge(groupA, groupB);

			ContentLocator expectedLocators = AnnotationTestHelper.CreateLocator(new string[] { "A", "C" });
			AssertEquals("Verify number of locators.", 6, result.Locators.Count);
			foreach (ContentLocator loc in result.Locators)
				Assert("Verify equality.", AnnotationTestHelper.LocatorPartListsEqual(expectedLocators, loc));

			passTest("Verified merge(ContentLocatorGroup).");
		}

		private ContentLocatorGroup DoMerge(ContentLocatorGroup groupA, ContentLocatorBase groupB)
		{
			return (ContentLocatorGroup) ReflectionHelper.InvokeMethod(groupA, "Merge", new object[] { groupB });
		}

		#endregion
    
        #endregion PRIORITY TESTS
    }
}	

