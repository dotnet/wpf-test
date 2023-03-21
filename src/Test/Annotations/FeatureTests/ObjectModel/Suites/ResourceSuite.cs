// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using Annotations.Test.Framework;
using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Annotations.Storage;

namespace Avalon.Test.Annotations.Suites
{
	public class ResourceBVTs : TestSuite
    {
        #region BVT TESTS
        
        #region Test Helper Methods

		/// <summary>
		/// Given a AnnotationResource, test the properties of Name and Id. As Id is a
		/// Guid we can specify valid Guid, or not. If the Guid is not valid it
		/// should be Guid.Empty
		/// </summary>
        [TestCase_Helper()]
        protected void VerifyIdAndName(AnnotationResource res, bool valid, string name)
		{
			if (valid)
				Assert("Verify AnnotationResource Id is set.", res.Id != Guid.Empty);
			else
				AssertEquals("Verify AnnotationResource Id is Guid.Empty.", Guid.Empty, res.Id);
			AssertEquals("Verify AnnotationResource Name.", name, res.Name);
		}

		/// <summary>
		/// Given a AnnotationResource, test the properties of Name and Id. This method
		/// expects an actual Id to compare against.
		/// </summary>
        [TestCase_Helper()]
        protected void VerifyIdAndName(AnnotationResource res, Guid id, string name)
		{
			AssertEquals("Verify AnnotationResource Id is set.", id, res.Id);
			AssertEquals("Verify AnnotationResource Name.", name, res.Name);
		}

		/// <summary>
		/// Pass the given argument to AnnotationResource constructor and return the
		/// exception that is thrown.  If no exception, FAIL test. Never
		/// returns null.
		/// </summary>
        [TestCase_Helper()]
        protected Exception ExpectConstructorFailure(string name)
		{
			try
			{
				new AnnotationResource(name);
			}
			catch (Exception e)
			{
				return e;
			}
			failTest("Expected an exception but none occurred.");
			return null;
		}

		/// <summary>
		/// Pass the given argument to AnnotationResource constructor and return the
		/// exception that is thrown.  If no exception, FAIL test. Never
		/// returns null.
		/// </summary>
        [TestCase_Helper()]
        protected Exception ExpectConstructorFailure(Guid id)
		{
			try
			{
				new AnnotationResource(id);
			}
			catch (Exception e)
			{
				return e;
			}
			failTest("Expected an exception but none occurred.");
			return null;
        }

        #endregion Test Helper Methods

        #region Test Case Methods

		/// <summary>
		/// Name	Id	->	Name		Id		Result
		///	-		-		-			Empty	Valid
		/// </summary>
        [Priority(0)]
        public void case1_1()
		{
			AnnotationResource res = new AnnotationResource();
			VerifyIdAndName(res, true, null);
			passTest("AnnotationResource properties verified.");
		}

		/// <summary>
		/// Name	Id	->	Name		Id		Result
		///	Bill	-		Bill		set		Valid
		/// </summary>
        [Priority(0)]
        public void case1_2()
		{
			AnnotationResource res = new AnnotationResource("Bill");
			VerifyIdAndName(res, true, "Bill");
			passTest("AnnotationResource properties verified.");
		}

		/// <summary>
		/// Name	Id	->	Name		Id		Result
		///	-		valid	null		set		Valid
		/// </summary>
        [Priority(0)]
        public void case1_3()
		{
			Guid myGuid = Guid.NewGuid();
			AnnotationResource res = new AnnotationResource(myGuid);
			VerifyIdAndName(res, myGuid, null);
			passTest("AnnotationResource properties verified.");
		}

		/// <summary>
		/// Name	Id	->	Name		Id		Result
		///	Bill	-		Bill		empty	Valid
		/// set Id -> exception
		/// </summary>
        [Priority(0)]
        public void case1_8()
		{
			AnnotationResource res = new AnnotationResource("junk");
			VerifyIdAndName(res, true, "junk");
			try
			{
				Guid myGuid = Guid.NewGuid();
				//res.Id = myGuid;
			}
            //catch (Exception e)
            catch (Exception)
			{
				passTest("Exception on setting AnnotationResource.Id");
			}
			//failTest("Expect exception not thrown.");
            passTest("No Exception thrown on setting AnnotationResource.Id because this is no longer a settable property");
		}

		/// <summary>
		/// Name	Id	->	Name		Id		Result
		///	-		valid	-			set		Valid
		/// set Name="zorries" -> valid
		///	-		valid	zorries		set		Valid
		/// </summary>
        [Priority(0)]
        public void case1_9()
		{
			Guid myGuid = Guid.NewGuid();
			AnnotationResource res = new AnnotationResource(myGuid);
			VerifyIdAndName(res, myGuid, null);
			res.Name = "zorries";
			VerifyIdAndName(res, myGuid, "zorries");
			passTest("AnnotationResource properties verified.");
		}

		/// <summary>
		/// Name	Id	->	Name		Id		Result
		///	-		-		-			empty	Valid
		/// set Name="zorries" -> valid
		///	-		valid	zorries		set		Valid
		/// </summary>
        [Priority(0)]
        public void case1_10()
		{
			AnnotationResource res = new AnnotationResource();
			VerifyIdAndName(res, true, null);
			res.Name = "resName";
			VerifyIdAndName(res, true, "resName");
			passTest("AnnotationResource properties verified.");
        }

        #endregion Test Case Methods

        #endregion BVT TESTS

        #region PRIORITY TESTS
        #endregion PRIORITY TESTS
    }
}


