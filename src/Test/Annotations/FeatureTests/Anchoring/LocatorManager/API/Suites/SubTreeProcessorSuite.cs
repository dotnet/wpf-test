// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVT test cases for the SubTreeProcessorSuite test suite.
//  
//  The SubTreeProcessor test suite consists of testing the following API and 
//  functionality:
//
//  - GetSubTreeProcessor
//  - GetSubTreeProcessorForLocatorPart
//  - RegisterSubTreeProcessor
//  - RegisterAndGetSubTreeProcessor
//

using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;

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
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;
using System.Xml;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class SubTreeProcessorSuite_BVT : ASubTreeProcessorSuite
    {
        // -----------------------------------------------------------------------------------------------
        //							GETSUBTREEPROCESSORFORLOCATORPART TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// New (12-7-04): valid ContentLocatorPart (FixedPageProcessor)
        /// Old: valid ContentLocatorPart (TextFingerprint)
        /// </summary>
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart4()
        {
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("DataId", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            result = manager.GetSubTreeProcessorForLocatorPart(part);
            VerifyDataIdProcessorReturned(result);
        }


        // -----------------------------------------------------------------------------------------------
        //								GETSUBTREEPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// a set to use DataIdProcessor, b set to use FixedPageProcessor, tree has multiple levels
        /// If, at any point before the test's completion, the wrong SubTreeProcessor is returned, print
        /// failure message and end the test.
        /// </summary>
        [Priority(0)]
        private void getsubtreeprocessors9()
        {
            BuildComplexTree();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(b, FixedPageProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(FixedPageProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "c")) return;

            result = manager.GetSubTreeProcessor(d);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(FixedPageProcessor), "d")) return;

            result = manager.GetSubTreeProcessor(e);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(FixedPageProcessor), "e")) return;

            // If we've reached this point with no failures, then the test passed
            passTest("DataIdProcessors for a, c and FixedPageProcessors for b, d, e correctly returned");
        }


        // -----------------------------------------------------------------------------------------------
        //								REGISTERSUBTREEPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// valid processor, processorId
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registersubtreeprocessor6()
        {
            try
            {
                manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "CAFTest3");
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  " + exp.ToString());
            }
            passTest("No exceptions expected");
        }


        // -----------------------------------------------------------------------------------------------
        //								REGISTERANDGETSUBTREEPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Set DataIdProcessor on root, GetSubTreeProcessor on root
        /// </summary>
        [Priority(0)]
        private void registerandgetsubtreeprocessor1()
        {
            BuildSimpleTree();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            result = manager.GetSubTreeProcessor(b);
            VerifyDataIdProcessorReturned(result);
        }

        // -----------------------------------------------------------------------------------------------
        //							GETSUBTREEPROCESSORFORLOCATORPART TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Null ContentLocatorPart
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart1()
        {
            try
            {
                result = manager.GetSubTreeProcessorForLocatorPart(null);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not" + exp.ToString());
            }
        }

        /// <summary>
        /// Unregistered ContentLocatorPart (empty type and namespace)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart2()
        {
            bool exception = false;
            try
            {
                new ContentLocatorPart(new XmlQualifiedName("", ""));
            }
            //catch (Exception e)
            catch (Exception)
            {
                exception = true;
            }
            Assert("Expected exception but none occurred.", exception);
            passTest("Exception thrown for LP with empty type/namespace");

            //result = _manager.GetSubTreeProcessorForLocatorPart(lp);

            //if (result != null)
            //    failTest("Invalid return value.  Expecting null");
            //else
            //    passTest("Null correctly returned");
        }

        /// <summary>
        /// Unregistered ContentLocatorPart (empty locator type only)
        /// </summary>
        [TestId("getsubtreeprocessorforlocatorpart2.1")]
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart2_1()
        {
            bool exception = false;
            try
            {
                new ContentLocatorPart(new XmlQualifiedName("", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            }
            //catch (Exception e)
            catch (Exception)
            {
                exception = true;
            }
            Assert("Expected exception but none occurred.", exception);
            passTest("Exception thrown for LP with empty type");

            //ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            //result = _manager.GetSubTreeProcessorForLocatorPart(part);
            //VerifyNullReturned(result);
        }

        /// <summary>
        /// Valid locatorPart (DataId)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart3()
        {
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("DataId", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            result = manager.GetSubTreeProcessorForLocatorPart(part);
            VerifyDataIdProcessorReturned(result);
        }

        /// <summary>
        /// Invalid locatorpart format (incorrect SchemaNamespaceUri)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart5()
        {
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("TextFingerprint", AnnotationXmlConstants.Namespaces.CoreSchemaNamespace));
            result = manager.GetSubTreeProcessorForLocatorPart(part);
            VerifyNullReturned(result);
        }

        /// <summary>
        /// Newly registered SubTreeProcessor
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessorforlocatorpart6()
        {
            // Register new SubTreeProcessor
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "CAFTest");

            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("CAFTest", "http://schemas.microsoft.com/caf2"));
            result = manager.GetSubTreeProcessorForLocatorPart(part);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor));
        }


        // -----------------------------------------------------------------------------------------------
        //								GETSUBTREEPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Null argument passed in, single-node tree
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors1()
        {
            BuildSingleNodeTree();
            try
            {
                result = manager.GetSubTreeProcessor(null);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
        }

        /// <summary>
        /// No subtree processor set on element, single-node tree
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors2()
        {
            BuildSingleNodeTree();
            result = manager.GetSubTreeProcessor(a);
            VerifyDataIdProcessorReturned(result);
        }

        /// <summary>
        /// a set to use DataIdProcessor, single-node tree
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors3()
        {
            BuildSingleNodeTree();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            VerifyDataIdProcessorReturned(result);
        }

        /// <summary>
        /// a set to use TextFingerprintProcessor, single-node tree
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors4()
        {
            BuildSingleNodeTree();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            VerifyTextFingerprintProcessorReturned(result);
        }

        /// <summary>
        /// No subtree processor set on element, tree has root with 2 children
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors5()
        {
            BuildSimpleTree();
            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "c")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("DataIdProcessors for a, b, c correctly returned");
        }

        /// <summary>
        /// a set to use DataIdProcessor, tree has root with 2 children
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors6()
        {
            BuildSimpleTree();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "c")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("DataIdProcessors for b, c correctly returned");
        }

        /// <summary>
        /// a set to use TextFingerprintProcessor, tree has root with 2 children
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors7()
        {
            BuildSimpleTree();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "c")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("TextFingerprintProcessors for a, b, c correctly returned");
        }

        /// <summary>
        /// b set to use DataIdProcessor, tree has multiple levels
        /// </summary>
        [Priority(0)]
        private void getsubtreeprocessors8()
        {
            BuildComplexTree();
            LocatorManager.SetSubTreeProcessorId(b, DataIdProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "c")) return;

            result = manager.GetSubTreeProcessor(d);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "d")) return;

            result = manager.GetSubTreeProcessor(e);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "e")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("DataIdProcessors for a, b, c, d, e correctly returned");
        }

        /// <summary>
        /// a set to use TextFingerprintProcessor, b set to use DataIdProcessor, tree has multiple levels
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors10()
        {
            BuildComplexTree();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(b, DataIdProcessor.Id);

            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "c")) return;

            result = manager.GetSubTreeProcessor(d);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "d")) return;

            result = manager.GetSubTreeProcessor(e);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "e")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("DataIdProcessors for b, d, e; TextFingerprintProcessors for a, c correctly returned");
        }

        /// <summary>
        /// a set to use DataIdProcessor, b set to use DataIdProcessor, tree has root with 2 children
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors11()
        {
            BuildSimpleTree();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            LocatorManager.SetSubTreeProcessorId(b, DataIdProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(DataIdProcessor), "c")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("DataIdProcessors for a, b, c correctly returned");
        }

        /// <summary>
        /// a set to use TextFingerprintProcessor, tree has multiple levels
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getsubtreeprocessors12()
        {
            BuildComplexTree();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "a")) return;

            result = manager.GetSubTreeProcessor(b);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "b")) return;

            result = manager.GetSubTreeProcessor(c);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "c")) return;

            result = manager.GetSubTreeProcessor(d);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "d")) return;

            result = manager.GetSubTreeProcessor(e);
            if (!CheckSubTreeProcessorTypeForNode(result, typeof(TextFingerprintProcessor), "e")) return;

            // If we get to this point without failures in the previous checks, then test passes
            passTest("TextFingerprintProcessors for a, b, c, d, e correctly returned");
        }


        // -----------------------------------------------------------------------------------------------
        //								REGISTERSUBTREEPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Null SubTreeProcessor
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registersubtreeprocessor1()
        {
            try
            {
                manager.RegisterSubTreeProcessor(null, "CAFTest");
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
        }

        /// <summary>
        /// Null processorId
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registersubtreeprocessor2()
        {
            try
            {
                manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), null);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception exp)
            {
                failTest("ArgumentNullException should have been thrown, not " + exp.ToString());
            }
        }

        /// <summary>
        /// Null array for locatorPartTypes
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registersubtreeprocessor3()
        {
            try
            {
                manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor_NullLocatorPartTypes(manager), "CAFTest2");
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  " + exp.ToString());
            }
            passTest("No exceptions expected");
        }

        /// <summary>
        /// Already registered processorId (DataIdProcessor.Id)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registersubtreeprocessor4()
        {
            try
            {
                manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "Id");
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  " + exp.ToString());
            }
            passTest("No exceptions expected");
        }


        /// <summary>
        /// Empty locatorPartTypes array
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registersubtreeprocessor5()
        {
            try
            {
                manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor_EmptyLocatorPartTypes(manager), "CAFTest3");
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception: " + exp.ToString());
            }
            passTest("No exceptions expected");
        }


        // -----------------------------------------------------------------------------------------------
        //							SUBTREEPROCESSORIDPROPERTY TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// This tests whether the value set for this property is the same value returned
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void subtreeprocessoridproperty1()
        {
            DockPanel dp = new DockPanel();
            String strPropValue = "test";

            // element.GetValue(SubTreeProcessorId) should return "test"
            LocatorManager.SetSubTreeProcessorId(dp, strPropValue);
            String strResult = dp.GetValue(Proxies.MS.Internal.Annotations.Anchoring.LocatorManager.SubTreeProcessorIdProperty) as String;

            if (strPropValue.Equals(strResult))
                passTest("GetValue(SubTreeProcessorIdProperty) == SetValue(SubTreeProcessorIdProperty)");
            else
                failTest("GetValue(SubTreeProcessorIdProperty) != SetValue(SubTreeProcessorIdProperty)");
        }


        // -----------------------------------------------------------------------------------------------
        //								REGISTERANDGETSUBTREEPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Set DataIdProcessor on root, GetSubTreeProcessor on root
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor2()
        {
            BuildFunctionalTree();
            LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            VerifyDataIdProcessorReturned(result);
        }

        /// <summary>
        /// Set TextFingerprintProcessor on root, GetSubTreeProcessor on root
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor3()
        {
            BuildFunctionalTree();
            LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
            result = manager.GetSubTreeProcessor(a);
            VerifyTextFingerprintProcessorReturned(result);
        }

        /// <summary>
        /// Set CustomSubTreeProcessor on root, GetSubTreeProcessor on root
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor4()
        {
            BuildFunctionalTree();
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "MyID");
            LocatorManager.SetSubTreeProcessorId(a, "MyID");
            result = manager.GetSubTreeProcessor(a);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor));
        }

        /// <summary>
        /// Register CustomSubTreeProcessor with Name "MyID", then register 
        /// CustomSubTreeProcessor_NullLocatorPartTypes with the same Name
        /// </summary>
        [TestId("registerandgetsubtreeprocessor4.1")]
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor4_1()
        {
            BuildFunctionalTree();
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "MyID");

            // Override processor Name
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor_NullLocatorPartTypes(manager), "MyID");
            LocatorManager.SetSubTreeProcessorId(a, "MyID");
            result = manager.GetSubTreeProcessor(a);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor_NullLocatorPartTypes));
        }

        /// <summary>
        /// Set CustomSubTreeProcessor on root, GetSubTreeProcessor on a child of the root
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor5()
        {
            BuildFunctionalTree();
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "MyNewSP");
            LocatorManager.SetSubTreeProcessorId(a, "MyNewSP");
            result = manager.GetSubTreeProcessor(c);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor));
        }

        /// <summary>
        /// Set CustomSubTreeProcessor on root, GetSubTreeProcessor on a leaf node
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor6()
        {
            BuildFunctionalTree();
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "MyNewSP");
            LocatorManager.SetSubTreeProcessorId(a, "MyNewSP");
            result = manager.GetSubTreeProcessor(h);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor));
        }

        /// <summary>
        /// Set CustomSubTreeProcessor on root, GetSubTreeProcessor on ContentLocatorPart with type "MyNewSP"
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor7()
        {
            BuildFunctionalTree();
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "MyNewSP");
            LocatorManager.SetSubTreeProcessorId(a, "MyNewSP");

            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("MyNewSP", "http://schemas.microsoft.com/caf2"));
            result = manager.GetSubTreeProcessorForLocatorPart(part);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor));
        }

        /// <summary>
        /// Set CustomSubTreeProcessor on root, GetSubTreeProcessor on ContentLocatorPart with type "MyNewSP2"
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetsubtreeprocessor8()
        {
            BuildFunctionalTree();
            manager.RegisterSubTreeProcessor(new CustomSubTreeProcessor(manager), "MyNewSP");
            LocatorManager.SetSubTreeProcessorId(a, "MyNewSP");

            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("MyNewSP2", "http://schemas.microsoft.com/caf2"));
            result = manager.GetSubTreeProcessorForLocatorPart(part);
            VerifyCustomSubTreeProcessorReturned(result, typeof(CustomSubTreeProcessor));
        }
    }
}

