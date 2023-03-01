// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 


using Annotations.Test.Framework; // TestSuite.

using System;
using System.Threading; using System.Windows.Threading;
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
    public class ConstructorBVTs : TestSuite
    {
        #region BVT TESTS

        #region Constants

        const string EMPTY_ID = "00000000-0000-0000-0000-000000000000";

        #endregion Constants

        #region Test Helper Methods

        protected void VerifyTypeAndNamespace(Annotation anno, string type, string nsp)
        {
            AssertEquals("Verify annotation type.", type, anno.AnnotationType.Name);
            AssertEquals("Verify annotation Namespace.", nsp, anno.AnnotationType.Namespace);
        }

        /// <summary>
        /// Pass the given arguments to Annotation constructor and return the
        /// exception that is thrown.  If no exception, FAIL test. Never
        /// returns null;
        /// </summary>
        protected Exception ExpectConstructorFailure(Guid id, string typeName, string nameSpace)
        {
            try
            {
                new Annotation(new XmlQualifiedName(typeName, nameSpace), id, new DateTime(), new DateTime());
            }
            catch (Exception e)
            {
                return e;
            }
            failTest("Expected an exception but none occurred.");
            return null;
        }

        /// <summary>
        /// Pass the given arguments to Annotation constructor and return the
        /// exception that is thrown.  If no exception, FAIL test. Never
        /// returns null;
        /// </summary>
        protected Exception ExpectConstructorFailure(string typeName, string nameSpace)
        {
            try
            {
                new Annotation(new XmlQualifiedName(typeName, nameSpace));
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
        /// Id		Type	Namespace	Id		Type	Namespace	Result
        ///	-		-		-			Empty	Null	Null		Valid
        /// </summary>
        [Priority(0)]
        public void case1_1()
        {
            Annotation anno = new Annotation();
            AssertEquals("Verify annotation Id is empty.", EMPTY_ID, anno.Id.ToString());
            Assert("Verify annotation Type is null.", anno.AnnotationType == null);
            passTest("Annotation properties verified.");
        }

        /// <summary>
        /// Id		Type		Namespace		Id		Type		Namespace		Result
        ///	-		MyType	MyNamspace	Set		MyType	MyNamespace	Valid
        /// </summary>
        [Priority(0)]
        public void case2_1()
        {
            Annotation anno = new Annotation(new XmlQualifiedName("myType", "MyNamespace"));
            Assert("Verify annotation Id is.", !anno.Id.ToString().Equals(EMPTY_ID));
            VerifyTypeAndNamespace(anno, "myType", "MyNamespace");
            passTest("Annotation properties verified.");
        }

        /// <summary>
        /// Id		Type	Namespace	Id		Type	Namespace	Result
        ///	-		MT				-		-		-			Exception
        /// </summary>
        [Priority(0)]
        public void case2_2()
        {
            Exception e = ExpectConstructorFailure("MT", "");
            Assert("Verify exception type.", e is ArgumentException);
            passTest("Expected exception occurred.");
        }

        /// <summary>
        /// Id		Type					Namespace	Id		Type	Namespace	Result
        ///	-		MT\ otherAtt=\1\		ANS			-		-		-			No Exception.
        /// </summary>
        [Priority(0)]
        public void case2_3()
        {
            Annotation anno = new Annotation(new XmlQualifiedName("MT\"otherAtt=\"1", "ANS"));
            VerifyTypeAndNamespace(anno, "MT\"otherAtt=\"1", "ANS");
            passTest("Verified that nested quotations are ok.");
        }

        /// <summary>
        /// Id		Type		Namespace	Id		Type		Namespace	Result
        /// Id		fullType	fullNsp	Id		fullType	fullNsp	Valid
        /// </summary>
        [Priority(0)]
        public void case3_1()
        {
            DateTime creationDate = new DateTime(2004, 8, 24);
            DateTime modifyDate = new DateTime(2004, 8, 25);
            Guid id = new Guid("12345678-1234-1234-5555-987654321012");

            Annotation anno = new Annotation(new XmlQualifiedName("fullType", "fullNsp"), id, creationDate, modifyDate);

            AssertEquals("Verify annotation Id is empty.", id, anno.Id);
            VerifyTypeAndNamespace(anno, "fullType", "fullNsp");
            AssertEquals("Verify annotation CreationTime.", creationDate, anno.CreationTime);
            AssertEquals("Verify annotation LastModificationTime.", modifyDate, anno.LastModificationTime);

            passTest("All annotation properties verified.");
        }

        /// <summary>
        /// Id			Type		Namespace	Id		Type		Namespace	Result
        ///	Id of		otherType	notherNS	Id		otherType	notherNS	Valid
        /// existing 
        /// annotation	
        /// </summary>
        [Priority(0)]
        public void case3_2()
        {
            Annotation preExistingAnno = new Annotation(new XmlQualifiedName("typeOne", "nspOne"));
            Annotation anno = new Annotation(new XmlQualifiedName("otherType", "notherNS"), preExistingAnno.Id, new DateTime(), new DateTime());

            AssertEquals("Verify annotation Id is empty.", preExistingAnno.Id, anno.Id);
            VerifyTypeAndNamespace(anno, "otherType", "notherNS");
            passTest("Annotation properties verified.");
        }

        /// <summary>
        /// Id		Type		Namespace	Id		Type	Namespace	Result
        /// Empty	tpName	NsName	-		-		-			Exception
        /// </summary>
        [Priority(0)]
        public void case3_7()
        {
            //Exception e = ExpectConstructorFailure(new Guid(), "tpName", "NsName");
            Exception e = ExpectConstructorFailure(Guid.Empty, "tpName", "NsName");
            //Assert("Verify exception type.", e is ArgumentException);
            passTest("Expected exception occurred.");
        }

        #endregion Test Case Methods

        #endregion BVT TESTS

        #region PRIORITY TESTS
        #endregion PRIORITY TESTS
    }
}

