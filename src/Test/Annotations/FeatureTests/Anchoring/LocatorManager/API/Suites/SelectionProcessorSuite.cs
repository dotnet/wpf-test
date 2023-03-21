// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVT test cases for the SelectionProcessorSuite test suite.
//  
//  The SelectionProcessor test suite consists of testing the following API and 
//  functionality:
//
//  - GetSelectionProcessor
//  - GetSelectionProcessorForLocatorPart
//  - RegisterSelectionProcessor
//  - RegisterAndGetSelectionProcessor
//

using System;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Documents;
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
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows.Annotations.Storage;
using Annotations.Test.Framework;
using System.Xml;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class SelectionProcessorSuite_BVT : ASelectionProcessorSuite
    {
        // -----------------------------------------------------------------------------------------------
        //							GETSELECTIONPROCESSORFORLOCATORPART TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// ContentLocatorPart = CharacterRange
        /// </summary>
        [Priority(0)]
        private void getselectionprocessorforlocatorpart2()
        {
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("CharacterRange", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            result = manager.GetSelectionProcessorForLocatorPart(part);
            VerifyTextSelectionProcessorReturned(result);
        }


        // -----------------------------------------------------------------------------------------------
        //								GETSELECTIONPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// selectionType is typeof(TextRange)
        /// </summary>
        [Priority(0)]
        private void getselectionprocessor3()
        {
            result = manager.GetSelectionProcessor(typeof(TextRange));
            VerifyTextSelectionProcessorReturned(result);
        }


        // -----------------------------------------------------------------------------------------------
        //								REGISTERSELECTIONPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// valid processor, selectionType not registered with processor
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerselectionprocessor6()
        {
            try
            {
                CustomSelectionProcessor csp = new CustomSelectionProcessor(manager);
                manager.RegisterSelectionProcessor(csp, typeof(TextRange));
                manager.RegisterSelectionProcessor(csp, typeof(Point));
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  " + exp.ToString());
            }
            passTest("No exceptions expected");
        }


        // -----------------------------------------------------------------------------------------------
        //						REGISTERANDGETSELECTIONPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// TreeNodeSelectionProcessor registered by default.  Returned by GetSelectionProcessor for selection type Canvas
        /// </summary>
        [Priority(0)]
        private void registerandgetselectionprocessor2()
        {
            result = manager.GetSelectionProcessor(typeof(Canvas));
            VerifyTreeNodeSelectionProcessorReturned(result);
        }


        // -----------------------------------------------------------------------------------------------
        //							GETSELECTIONPROCESSORFORLOCATORPART TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// locatorPart = null
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessorforlocatorpart1()
        {
            try
            {
                result = manager.GetSelectionProcessorForLocatorPart(null);
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
        /// locatorPart = poorly formatted ContentLocatorPart (wrong SchemaNamespace)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessorforlocatorpart3()
        {
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("CharacterRange", AnnotationXmlConstants.Namespaces.CoreSchemaNamespace));
            result = manager.GetSelectionProcessorForLocatorPart(part);
            VerifyNullReturned(result);
        }


        /// <summary>
        /// locatorPart = ContentLocatorPart recognized by new SelectionProcessor
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessorforlocatorpart4()
        {
            // register new SelectionProcessor
            manager.RegisterSelectionProcessor(new CustomSelectionProcessor(manager), typeof(TextBox));
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("CAFTest2", "http://schemas.microsoft.com/caf2"));
            result = manager.GetSelectionProcessorForLocatorPart(part);
            VerifyCustomSelectionProcessorReturned(result, typeof(CustomSelectionProcessor));
        }


        /// <summary>
        /// locatorPart = unregistered ContentLocatorPart (empty type, namespace)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessorforlocatorpart5()
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

            //result = _manager.GetSelectionProcessorForLocatorPart(new ContentLocatorPart(new XmlQualifiedName("", "")));
            //VerifyNullReturned(result);
        }


        // -----------------------------------------------------------------------------------------------
        //								GETSELECTIONPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Null selectionType
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessor1()
        {
            try
            {
                result = manager.GetSelectionProcessor(null);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception e)
            {
                failTest("ArgumentNullException should have been thrown, not " + e.ToString());
            }
        }

        /// <summary>
        /// Unregistered selectionType
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessor2()
        {
            result = manager.GetSelectionProcessor(typeof(Application));
            VerifyNullReturned(result);
        }

        /// <summary>
        /// selectionType is typeof(FrameworkElement)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessor4()
        {
            result = manager.GetSelectionProcessor(typeof(FrameworkElement));
            VerifyTreeNodeSelectionProcessorReturned(result);
        }

        /// <summary>
        /// selectionType is typeof(Button) [subclass of FrameworkElement]
        /// </summary>
        [TestId("getselectionprocessor4.1")]
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessor4_1()
        {
            result = manager.GetSelectionProcessor(typeof(Button));
            VerifyTreeNodeSelectionProcessorReturned(result);
        }

        /// <summary>
        /// selectionType is typeof(FrameworkContentElement)
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void getselectionprocessor5()
        {
            result = manager.GetSelectionProcessor(typeof(FrameworkContentElement));
            VerifyTreeNodeSelectionProcessorReturned(result);
        }


        // -----------------------------------------------------------------------------------------------
        //								REGISTERSELECTIONPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// Null SelectionProcessor
        /// </summary>
        [Priority(0)]
        [DisabledTestCase()]
        private void registerselectionprocessor1()
        {
            try
            {
                manager.RegisterSelectionProcessor(null, typeof(Button));
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception e)
            {
                failTest("ArgumentNullException should have been thrown, not " + e.ToString());
            }
        }

        /// <summary>
        /// Null selectionType
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerselectionprocessor2()
        {
            try
            {
                manager.RegisterSelectionProcessor(new CustomSelectionProcessor(manager), null);
                failTest("ArgumentNullException should have been thrown");
            }
            catch (System.ArgumentNullException)
            {
                passTest("ArgumentNullException correctly thrown");
            }
            catch (Exception e)
            {
                failTest("ArgumentNullException should have been thrown, not " + e.ToString());
            }
        }

        /// <summary>
        /// Null locatorPartTypes array
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerselectionprocessor3()
        {
            try
            {
                manager.RegisterSelectionProcessor(new CustomSelectionProcessor_NullLocatorPartTypes(manager),
                    typeof(Button));
            }
            catch (Exception e)
            {
                failTest("Unexpected exception:  " + e.ToString());
            }
            passTest("No exceptions expected");
        }

        /// <summary>
        /// Already registered selectionType
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerselectionprocessor4()
        {
            try
            {
                manager.RegisterSelectionProcessor(new CustomSelectionProcessor(manager), typeof(TextRange));
                manager.RegisterSelectionProcessor(new CustomSelectionProcessor_EmptyLocatorPartTypes(manager),
                    typeof(TextRange));
            }
            catch (Exception e)
            {
                failTest("Exception thrown registering SelectionProcessor: " + e.ToString());
            }
            passTest("No exceptions expected");
        }

        /// <summary>
        /// Empty locatorPartTypes array. Valid selection processor and selection type
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerselectionprocessor5()
        {
            try
            {
                manager.RegisterSelectionProcessor(new CustomSelectionProcessor_EmptyLocatorPartTypes(manager),
                    typeof(TextBlock));
            }
            catch (Exception e)
            {
                failTest("Unexpected exception: " + e.ToString());
            }
            passTest("No exceptions expected");
        }

        /// <summary>
        /// Register DynamicSelectionProcessor with object type Text
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerselectionprocessor7()
        {
            try
            {
                manager.RegisterSelectionProcessor(new DynamicSelectionProcessor(manager), typeof(TextBlock));
            }
            catch (Exception e)
            {
                failTest("Unexpected exception:  " + e.ToString());
            }
            passTest("No exceptions expected");
        }


        // -----------------------------------------------------------------------------------------------
        //						REGISTERANDGETSELECTIONPROCESSOR TEST CASES
        // -----------------------------------------------------------------------------------------------

        /// <summary>
        /// TextSelectionProcessor registered by default.  Returned by GetSelectionProcessor for selection type TextRange
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetselectionprocessor1()
        {
            result = manager.GetSelectionProcessor(typeof(TextRange));
            VerifyTextSelectionProcessorReturned(result);
        }

        /// <summary>
        /// Register custom SelectionProcessor and GetSelectionProcessor for selection type registered
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetselectionprocessor3()
        {
            manager.RegisterSelectionProcessor(new CustomSelectionProcessor(manager), typeof(TextBox));
            result = manager.GetSelectionProcessor(typeof(TextBox));
            VerifyCustomSelectionProcessorReturned(result, typeof(CustomSelectionProcessor));
        }

        /// <summary>
        /// Register custom SelectionProcessor and override the selection type
        /// </summary>
        [TestId("registerandgetselectionprocessor3.1")]
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetselectionprocessor3_1()
        {
            manager.RegisterSelectionProcessor(new CustomSelectionProcessor(manager), typeof(TextBox));

            // Override
            manager.RegisterSelectionProcessor(new CustomSelectionProcessor_NullLocatorPartTypes(manager),
                typeof(TextBox));
            result = manager.GetSelectionProcessor(typeof(TextBox));
            VerifyCustomSelectionProcessorReturned(result, typeof(CustomSelectionProcessor_NullLocatorPartTypes));
        }

        /// <summary>
        /// Register custom SelectionProcessor and GetSelectionProcessor for ContentLocatorPart registered
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        private void registerandgetselectionprocessor4()
        {
            manager.RegisterSelectionProcessor(new CustomSelectionProcessor(manager), typeof(TextBox));
            ContentLocatorPart part = new ContentLocatorPart(new XmlQualifiedName("TextBoxSP", "http://schemas.microsoft.com/caf2"));
            result = manager.GetSelectionProcessorForLocatorPart(part);
            VerifyCustomSelectionProcessorReturned(result, typeof(CustomSelectionProcessor));
        }
    }
}

