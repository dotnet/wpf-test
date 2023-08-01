// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Xml;
using System.Collections;
using System.Windows.Markup;
using Microsoft.Test.Modeling;
using Microsoft.Test.Logging;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser.Error;
using Microsoft.Test;
using Avalon.Test.Xaml.Markup;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// This model contains negative cases (error cases) for XmlCompatibilityReader.
    /// There are 4 action handlers, which test various aspects of Compat reader.
    /// There are only 4 fixed Xaml files, one for each action.
    /// We modify that Xaml file depending on the value of the input parameters,
    /// and save the resultant Xaml into a temporary file. We then parse the temporary
    /// file and check whether the error thrown is as expected.
    /// Expected errors are pre-populated in master files, which are in Xml format
    /// </summary>
    /// 
    /// 
    [Model(@"FeatureTests\ElementServices\TestIgnorable.xtc", 1, @"Parser\Error\CompatReader",  SupportFiles = @"FeatureTests\ElementServices\Error_TestIgnorable.xaml,FeatureTests\ElementServices\Error_TestMustUnderstand.xaml,FeatureTests\ElementServices\Error_TestAlternateContent.xaml,FeatureTests\ElementServices\Error_TestProcessContent.xaml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestIgnorable.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestMustUnderstand.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestAlternateContent.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestProcessContent.xml,FeatureTests\ElementServices\ParserTestControlsV1.dll,FeatureTests\ElementServices\ParserTestControlsV2.dll")]
    [Model(@"FeatureTests\ElementServices\TestMustUnderstand.xtc", 1, @"Parser\Error\CompatReader", SupportFiles = @"FeatureTests\ElementServices\Error_TestIgnorable.xaml,FeatureTests\ElementServices\Error_TestMustUnderstand.xaml,FeatureTests\ElementServices\Error_TestAlternateContent.xaml,FeatureTests\ElementServices\Error_TestProcessContent.xaml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestIgnorable.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestMustUnderstand.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestAlternateContent.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestProcessContent.xml,FeatureTests\ElementServices\ParserTestControlsV1.dll,FeatureTests\ElementServices\ParserTestControlsV2.dll")]
    [Model(@"FeatureTests\ElementServices\TestAlternateContent.xtc", 1, @"Parser\Error\CompatReader", SupportFiles = @"FeatureTests\ElementServices\Error_TestIgnorable.xaml,FeatureTests\ElementServices\Error_TestMustUnderstand.xaml,FeatureTests\ElementServices\Error_TestAlternateContent.xaml,FeatureTests\ElementServices\Error_TestProcessContent.xaml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestIgnorable.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestMustUnderstand.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestAlternateContent.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestProcessContent.xml,FeatureTests\ElementServices\ParserTestControlsV1.dll,FeatureTests\ElementServices\ParserTestControlsV2.dll")]
    [Model(@"FeatureTests\ElementServices\TestProcessContent.xtc", 1, 3, 1, @"Parser\Error\CompatReader", "", SupportFiles = @"FeatureTests\ElementServices\Error_TestIgnorable.xaml,FeatureTests\ElementServices\Error_TestMustUnderstand.xaml,FeatureTests\ElementServices\Error_TestAlternateContent.xaml,FeatureTests\ElementServices\Error_TestProcessContent.xaml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestIgnorable.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestMustUnderstand.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestAlternateContent.xml,FeatureTests\ElementServices\Errors_CompatReaderModel_TestProcessContent.xml,FeatureTests\ElementServices\ParserTestControlsV1.dll,FeatureTests\ElementServices\ParserTestControlsV2.dll")]
    public class CompatReaderModel : CoreModel 
    {
        /// <summary>
        /// Creates a CompatReaderModel instance
        /// </summary>
        public CompatReaderModel()
            : base()
        {
            Name = "CompatReaderModel";

            //Add Action Handlers
            AddAction("TestIgnorable", new ActionHandler(TestIgnorable));
            AddAction("TestMustUnderstand", new ActionHandler(TestMustUnderstand));
            AddAction("TestAlternateContent", new ActionHandler(TestAlternateContent));
            AddAction("TestProcessContent", new ActionHandler(TestProcessContent));
        }

        #region TestIgnorable
        /// <summary>
        /// Handler for TestIgnorable.
        /// 
        /// This function takes a fixed Xaml file as input and modifies it by setting 
        /// an mc:Ignorable attribute. The value of the attribute comes from
        /// the inParams.
        /// </summary>
        /// <remarks>Handler for TestIgnorable</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestIgnorable(State endState, State inParams, State outParams)
        {
            // Determine the value of mc:Ignorable
            string IgnorableValue = "";
            switch(inParams["IgnorableValue"])
            {
                case "Empty":
                    IgnorableValue = "";
                    break;

                case "v1":
                    IgnorableValue = "v1";
                    break;

                case "v1commav2":
                    IgnorableValue = "v1,v2";
                    break;

                case "v5":
                    IgnorableValue = "v5";
                    break;
            }

            // Load assembly that makes v1 available 
            XamlTestRunner.LoadAssemblies("ParserTestControlsV1");

            string baseXamlFile = "Error_TestIgnorable.xaml";
            string testXamlFile = "__IgnorableValue_" + inParams["IgnorableValue"] + ".xaml";

            // Load the Xaml file into a DOM tree
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(baseXamlFile);
            // Find the node with the name StackPanel0 and set mc:Ignorable on it
            string mcns = "http://schemas.openxmlformats.org/markup-compatibility/2006";
            FindElementAndSetAttribute(doc, "StackPanel0", "Ignorable", mcns, IgnorableValue);
            // Save the DOM tree into a Xaml file 
            doc.Save(testXamlFile);

            string errorDatafile = "Errors_CompatReaderModel_TestIgnorable.xml";
            bool expectedErrorThrown = ErrorVerifier.Verify(testXamlFile, errorDatafile);

            File.Delete(testXamlFile); //Delete the newly created test file
            if (!expectedErrorThrown)
            {
                throw new Microsoft.Test.TestValidationException("Expected error not thrown");
            }
            return true;
        }
        #endregion TestIgnorable

        #region TestMustUnderstand
        /// <summary>
        /// Handler for TestMustUnderstand
        /// 
        /// This function takes a fixed Xaml file as input and modifies it by setting 
        /// an mc:MustUnderstand attribute. The value of the attribute comes from
        /// the inParams.
        /// </summary>
        /// <remarks>Handler for TestMustUnderstand</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestMustUnderstand(State endState, State inParams, State outParams)
        {
            // Determine the value of mc:MustUnderstand
            string MustUnderstandValue = "";
            switch (inParams["MustUnderstandValue"])
            {
                case "Empty":
                    MustUnderstandValue = "";
                    break;

                case "v1":
                    MustUnderstandValue = "v1";
                    break;

                case "v1commav2":
                    MustUnderstandValue = "v1,v2";
                    break;

                case "v5":
                    MustUnderstandValue = "v5";
                    break;
            }

            string baseXamlFile = "Error_TestMustUnderstand.xaml";
            string testXamlFile = "__MustUnderstandValue_" + inParams["MustUnderstandValue"] + ".xaml";

            // Load the Xaml file into a DOM tree
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(baseXamlFile);
            // Find the node with the name StackPanel0 and set mc:MustUnderstand on it
            string mcns = "http://schemas.openxmlformats.org/markup-compatibility/2006";
            FindElementAndSetAttribute(doc, "StackPanel0", "MustUnderstand", mcns, MustUnderstandValue);
            // Save the DOM tree into a Xaml file 
            doc.Save(testXamlFile);

            string errorDatafile = "Errors_CompatReaderModel_TestMustUnderstand.xml";
            bool expectedErrorThrown = ErrorVerifier.Verify(testXamlFile, errorDatafile);

            File.Delete(testXamlFile); //Delete the newly created test file
            if (!expectedErrorThrown)
            {
                throw new Microsoft.Test.TestValidationException("Expected error not thrown");
            }
            return true;
        }
        #endregion TestMustUnderstand

        #region TestAlternateContent
        /// <summary>
        /// Handler for TestAlternateContent
        /// </summary>
        /// <remarks>Handler for TestAlternateContent</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestAlternateContent(State endState, State inParams, State outParams)
        {
            string baseXamlFile = "Error_TestAlternateContent.xaml";
            string testXamlFile = "__Scenario_" + inParams["Scenario"] + ".xaml";
            string mcns = "http://schemas.openxmlformats.org/markup-compatibility/2006";
            string avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            // Load the Xaml file into a DOM tree
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(baseXamlFile);

            XmlElement alternateContent = FindElementByType(doc, "AlternateContent", mcns);
            XmlElement fallback = FindElementByType(doc, "Fallback", mcns);
            XmlElement v2choice = FindElementByName(doc, "v2choice");
            XmlElement v1choice = FindElementByName(doc, "v1choice");
            // We have found the elements using Name attributes. Now remove those attributes.
            v2choice.RemoveAttribute("Name");
            v1choice.RemoveAttribute("Name");

            switch (inParams["Scenario"])
            {
                case "No_Choice":
                    // Remove all children of <AlternateContent>
                    alternateContent.RemoveAll();
                    break;

                case "Two_Fallbacks":
                    // Duplicate the <Fallback> node
                    XmlElement secondFallback = fallback.Clone() as XmlElement;
                    alternateContent.InsertAfter(secondFallback, fallback);
                    break;

                case "Fallback_Before_Choice":
                    // Through re-arrangement, put a <Fallback> before a <Choice>
                    alternateContent.RemoveChild(fallback);
                    alternateContent.InsertAfter(fallback, v2choice);
                    break;

                case "Invalid_Child":
                    // Insert a child which is neither <Choice> nor <Fallback>,
                    // under <AlternateContent>
                    XmlElement newElement = doc.CreateElement("Button", avalonNS);
                    alternateContent.InsertAfter(newElement, fallback);
                    break;

                case "Choice_Without_Requires":
                    // Remove the Requires attribute on <Choice>
                    v2choice.RemoveAttribute("Requires");
                    break;

                case "Empty_Requires_Value":
                    v2choice.SetAttribute("Requires", string.Empty);
                    break;

                case "Comma_Separated_Requires_Value":
                    // Use comma to separate namespace prefixes in Requires value
                    // The legal separator is a space.
                    v2choice.SetAttribute("Requires", "v2,v3");
                    break;

                case "Undefined_Prefix_In_Requires":
                    // Specify an undefined prefix as Required
                    v2choice.SetAttribute("Requires", "v5");
                    break;

                case "Requires_On_Fallback":
                    // Specify a Requires attribute on <Fallback>.
                    // It's not legal there.
                    fallback.SetAttribute("Requires", "v1");
                    break;
            }

            // Save the DOM tree into a Xaml file 
            doc.Save(testXamlFile);

            string errorDatafile = "Errors_CompatReaderModel_TestAlternateContent.xml";
            bool expectedErrorThrown = ErrorVerifier.Verify(testXamlFile, errorDatafile);

            File.Delete(testXamlFile); //Delete the newly created test file
            if (!expectedErrorThrown)
            {
                throw new Microsoft.Test.TestValidationException("Expected error not thrown");
            }
            return true;
        }
        #endregion TestAlternateContent

        #region TestProcessContent
        /// <summary>
        /// Handler for TestProcessContent
        /// </summary>
        /// <remarks>Handler for TestProcessContent</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestProcessContent(State endState, State inParams, State outParams)
        {
            string baseXamlFile = "Error_TestProcessContent.xaml";
            string testXamlFile = "__Scenario_" + inParams["Scenario"] + ".xaml";
            string mcns = "http://schemas.openxmlformats.org/markup-compatibility/2006";
            //string avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            // Load the Xaml file into a DOM tree
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(baseXamlFile);

            XmlElement StackPanel1 = FindElementByName(doc, "StackPanel1");

            switch (inParams["Scenario"])
            {
                case "Comma_Separated_Value":
                    // mc:ProcessContent="v1:TransButton,v1:TransListBox"
                    // Legal separator is space and not comma
                    StackPanel1.SetAttribute("ProcessContent", mcns, "v1:TransButton,v1:TransListBox");
                    break;

                case "Repeatition_In_Value":
                    // mc:ProcessContent="v1:TransButton v1:TransListBox v1:TransButton"
                    // v1:TransButton is repeated. This could be a typo.
                    StackPanel1.SetAttribute("ProcessContent", mcns, "v1:TransButton v1:TransListBox v1:TransButton");
                    break;

                case "ProcessContent_Without_Ignorable":
                    // mc:ProcessContent on an element that doesn't carry an mc:Ignorable
                    // That's illegal.
                    StackPanel1.RemoveAttribute("Ignorable", mcns);
                    StackPanel1.SetAttribute("ProcessContent", mcns, "v1:TransButton v1:TransListBox");
                    break;

                case "Non_Ignorable_Namespace_In_Value":
                    // mc:ProcessContent="v1:TransButton v2:TransButton"
                    // The value references v2 namespace, however v2 has not
                    // been declared Ignorable. That's illegal.
                    StackPanel1.SetAttribute("ProcessContent", mcns, "v1:TransButton v2:TransButton");
                    break;
            }

            // Save the DOM tree into a Xaml file 
            doc.Save(testXamlFile);

            string errorDatafile = "Errors_CompatReaderModel_TestProcessContent.xml";
            bool expectedErrorThrown = ErrorVerifier.Verify(testXamlFile, errorDatafile);

            File.Delete(testXamlFile); //Delete the newly created test file
            if (!expectedErrorThrown)
            {
                throw new Microsoft.Test.TestValidationException("Expected error not thrown");
            }
            return true;
        }
        #endregion TestProcessContent

        /// <summary>
        /// Finds an Xml element in the given DOM tree that has the given name ("Name" attribute)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private XmlElement FindElementByName(XmlDocument doc, string nodeName)
        {
            XmlElement rootElement = doc.DocumentElement;
            XmlElement node = (XmlElement)rootElement.SelectSingleNode("//*[@Name='" + nodeName + "']");
            return node;
        }

        /// <summary>
        /// From the given DOM tree, returns the first XmlElement of the given type. 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementType"></param>
        /// <param name="elementNS"></param>
        /// <returns></returns>
        private XmlElement FindElementByType(XmlDocument doc, string elementType, string elementNS)
        {
            XmlElement rootElement = doc.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            // Add a mapping from a prefix to the given namespaces
            nsmgr.AddNamespace("ns", elementNS);
            XmlElement node = (XmlElement)rootElement.SelectSingleNode("//ns:" + elementType, nsmgr);
            return node;
        }

        private void FindElementAndSetAttribute(XmlDocument doc, string nodeName, string attrLocalName, string attrNS, string attrValue)
        {
            XmlElement node = FindElementByName(doc, nodeName);
            node.SetAttribute(attrLocalName, attrNS, attrValue);
        }
    }

}

//This file was generated using MDE on: Wednesday, June 22, 2005 4:20:30 PM
