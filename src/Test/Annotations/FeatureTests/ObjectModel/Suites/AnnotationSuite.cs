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
	public class AnnotationSuite : TestSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Verify default ctor.
        /// </summary>
        [Priority(0)]
        protected void annotation_ctor1()
        {
            VerifyAnnotation(new Annotation(), null, Guid.Empty, DateTime.MinValue, DateTime.MinValue);
            passTest("Verified default constructor.");
        }

        /// <summary>
        /// Verify ctor(XmlQualifiedName)
        /// </summary>
        [Priority(0)]
        protected void annotation_ctor2()
        {
            XmlQualifiedName type = new XmlQualifiedName("someType", "someNsp");
            VerifyAnnotation(new Annotation(type), type);
            passTest("Verified ctor(XmlQualifiedName).");
        }

        /// <summary>
        /// Verify ctor(XmlQualifiedName, Guid, DateTime, DateTime)
        /// </summary>
        [Priority(0)]
        protected void annotation_ctor3()
        {
            XmlQualifiedName type = new XmlQualifiedName("someType", "someNsp");
            Guid id = Guid.NewGuid();
            DateTime createTime = DateTime.Now;
            DateTime modifiedTime = DateTime.MaxValue;
            VerifyAnnotation(new Annotation(type, id, createTime, modifiedTime), type, id, createTime, modifiedTime);
            passTest("Verified ctor(XmlQualifiedName, Guid, DateTime, DateTime).");
        }


        private void VerifyAnnotation(Annotation annotation, XmlQualifiedName expectedType, Guid expectedId, DateTime expectedCreateTime, DateTime expectedModifiedTime)
        {
            AssertEquals("Verify type.", expectedType, annotation.AnnotationType);
            AssertEquals("Verify ID.", expectedId, annotation.Id);
            AssertEquals("Verify CreateTime.", expectedCreateTime, annotation.CreationTime);
            AssertEquals("Verify ModifiedTime.", expectedModifiedTime, annotation.LastModificationTime);
        }

        private void VerifyAnnotation(Annotation annotation, XmlQualifiedName expectedType)
        {
            AssertEquals("Verify type.", expectedType, annotation.AnnotationType);
            Assert("Verify ID is not empty.", !Guid.Empty.Equals(annotation.Id));
            AssertEquals("Verify CreateTime == modified time", annotation.CreationTime, annotation.LastModificationTime);
        }

        #endregion

        #region PRIORITY TESTS

        [Priority(1)]
        protected void annotation_ctor4()
        {
            VerifyException(null);
            passTest("Exception for null type.");
        }

        [Priority(1)]
        protected void annotation_ctor5()
        {
            VerifyException(new XmlQualifiedName(null, "nsp"));
            passTest("Exception for null typename.");
        }

        [Priority(1)]
        protected void annotation_ctor6()
        {
            VerifyException(new XmlQualifiedName("", "nsp"));
            passTest("Exception for empty typename.");
        }

        [Priority(1)]
        protected void annotation_ctor7()
        {
            VerifyException(new XmlQualifiedName("type", null));
            passTest("Exception for null namespace.");
        }

        [Priority(1)]
        protected void annotation_ctor8()
        {
            VerifyException(new XmlQualifiedName("type", ""));
            passTest("Exception for empty namespace.");
        }

        [Priority(1)]
        protected void annotation_ctor9()
        {
            VerifyException(null, Guid.NewGuid(), DateTime.Now, DateTime.Now);
            passTest("Exception for null type.");
        }

        [Priority(1)]
        protected void annotation_ctor10()
        {
            VerifyException(new XmlQualifiedName("", "nsp"), Guid.NewGuid(), DateTime.Now, DateTime.Now);
            passTest("Exception for empty typename.");
        }

        [Priority(1)]
        protected void annotation_ctor11()
        {
            VerifyException(new XmlQualifiedName("type", ""), Guid.NewGuid(), DateTime.Now, DateTime.Now);
            passTest("Exception for empty namespace.");
        }

        [Priority(1)]
        protected void annotation_ctor12()
        {
            VerifyException(new XmlQualifiedName("type", "nsp"), Guid.Empty, DateTime.Now, DateTime.Now);
            passTest("Exception for empty Id.");
        }

        [Priority(1)]
        protected void annotation_ctor13()
        {
            VerifyException(new XmlQualifiedName("type", "nsp"), Guid.NewGuid(), new DateTime(5000), new DateTime(1000));
            passTest("Exception for Created > LastModified.");
        }

        #endregion PRIORITY TESTS

        #region Private Helpers

        private void VerifyException(XmlQualifiedName type)
		{
			bool exceptionOccurred = false;
			try
			{
				new Annotation(type);
			}
			catch (Exception e)
			{
				Assert("Verify exception type is either ArugmentNullException or ArgumentException.", e.GetType().Equals(typeof(ArgumentException)) || e.GetType().Equals(typeof(ArgumentNullException)));
				exceptionOccurred = true;
			}
			Assert("Verify exception occurred.", exceptionOccurred);
		}

		private void VerifyException(XmlQualifiedName type, Guid id, DateTime creationTime, DateTime modifiedTime)
		{
			bool exceptionOccurred = false;
			try
			{
				new Annotation(type, id, creationTime, modifiedTime);
			}
			catch (Exception e)
			{
				Assert("Verify exception type is either ArugmentNullException or ArgumentException.", e.GetType().Equals(typeof(ArgumentException)) || e.GetType().Equals(typeof(ArgumentNullException)));
				exceptionOccurred = true;
			}
			Assert("Verify exception occurred.", exceptionOccurred);
		}

		#endregion
	}		
}	

