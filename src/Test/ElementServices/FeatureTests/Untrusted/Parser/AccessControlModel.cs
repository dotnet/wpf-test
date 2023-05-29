// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
********************************************************************/

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Modeling;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser.Error;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Model to test parser access control.
    /// </summary>
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestCPA.xtc", 1, 2, 0, @"Parser\AccessControlModel\TestCPA\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestCPA1", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestCPA.xtc", 3, 4, 1, @"Parser\AccessControlModel\TestCPA\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestCPA2", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestCPA.xtc", 1, @"Parser\AccessControlModel\TestCPA\PartialTrust", TestCaseSecurityLevel.PartialTrust, "AccessControlModel_TestCPA3", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestEvents.xtc", 1, 4, 0, @"Parser\AccessControlModel\TestEvents\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestEvents1", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestEvents.xtc", 5, 8, 1, @"Parser\AccessControlModel\TestEvents\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestEvents2", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions.xtc", 0, @"Parser\AccessControlModel\TestMarkupExtensions\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestMarkupExtensions1", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions.xtc", 1, @"Parser\AccessControlModel\TestMarkupExtensions\PartialTrust", TestCaseSecurityLevel.PartialTrust, "AccessControlModel_TestMarkupExtensions2", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestProperties.xtc", 1, 7, 0, @"Parser\AccessControlModel\TestProperties\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestProperties1", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestProperties.xtc", 8, 14, 1, @"Parser\AccessControlModel\TestProperties\FullTrust", TestCaseSecurityLevel.FullTrust, "AccessControlModel_TestProperties2", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\AccessControlModel_TestProperties.xtc", 1, @"Parser\AccessControlModel\TestProperties\PartialTrust", TestCaseSecurityLevel.PartialTrust, "AccessControlModel1_TestProperties3", SupportFiles = @"FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestEvents_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestMarkupExtensions_Base.xaml,FeatureTests\ElementServices\AccessControlModel_TestProperties_Base.xaml")]
    public class AccessControlModel : CoreModel 
    {
        /// <summary>
        /// Creates a AccessControlModel instance
        /// </summary>
        public AccessControlModel()
            : base()
        {
            Name = "AccessControlModel";

            //Add Action Handlers
            AddAction("TestCPA", new ActionHandler(TestCPA));
            AddAction("TestEvents", new ActionHandler(TestEvents));
            AddAction("TestMarkupExtensions", new ActionHandler(TestMarkupExtensions));
            AddAction("TestProperties", new ActionHandler(TestProperties));
        }

        #region TestCPA
        /// <summary>
        /// Handler for TestCPA.
        /// 
        /// Here we create a Xaml file, in such a way that its root element is a class
        /// with inaccessible content properties (private, internal, public with non-public
        /// setter etc, depending on the input parameters). Then we add content under
        /// the root element (so that the CPA will be used).
        /// 
        /// The Xaml file is then parsed and compiled, and we make sure that expected 
        /// errors are thrown (since the content properties are always inaccessible, 
        /// we expect all Xamls to fail).
        /// 
        /// </summary>
        /// <remarks>Handler for TestCPA</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestCPA(State endState, State inParams, State outParams)
        {
            // Read the values of various parameters
            CoreLogger.LogStatus("Reading input parameter values");
            string ContentProperty_access = inParams["ContentProperty access"];

            // Create the name of the custom class (used as root element) from the parameters
            string className = "Custom_Class_With_" + ContentProperty_access + "_ContentProperty";

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            // Generate the Xaml file from scratch.
            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            StreamWriter writer = new StreamWriter(File.OpenWrite(_testXamlFile));
            writer.Write(
                @"<cc:" + className + @"
	                xmlns='" + _avalonNS + @"'      
	                xmlns:x='" + _xamlNS + @"'
                    xmlns:cc='" + _customNS + @"' 
                >
                    <cc:Custom_Clr />
                </cc:" + className + @">
            ");
            writer.Close();

            // Decide how should the error messages look like, 
            // when we parse and compile the Xaml
            string[] errPieces = new string[4];
            errPieces[0] = "Content";
            errPieces[1] = "property";
            errPieces[2] = className;
            errPieces[3] = "not";


            // Process the Xaml and verify that the errors thrown contain 
            // the expected pieces.
            ProcessXamlAndVerifyErrors(_testXamlFile, true, errPieces);

            return true;
        }
        #endregion TestCPA

        #region TestProperties
        /// <summary>
        /// Handler for TestProperties.
        /// 
        /// Here we read a base Xaml file, set different types of non-public properties  
        /// (depending on the input parameters) on its root element, and then save a new Xaml.
        /// 
        /// The resultant Xaml file is parsed and compiled, and we make sure that expected 
        /// errors are thrown (since we always set non-public properties, we expect all Xamls
        /// to fail.
        /// 
        /// </summary>
        /// <remarks>Handler for TestProperties</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestProperties(State endState, State inParams, State outParams)
        {
            // Read the values of various parameters
            CoreLogger.LogStatus("Reading input parameter values");
            string Property_type = inParams["Property type"];
            string Property_access = inParams["Property access"];
            string Xaml_manifestation = inParams["Xaml manifestation"];

            // Is it going to be an attached property?
            bool isAttached = (Property_type == "Normal" ? false : true);

            // Create the name of the property from the parameters
            string propertyName = (isAttached ? "Attached" : "") + Property_access + "Property";

            // Name of the class that holds the property
            string className = (isAttached ? "Custom_DP_Attacher" : "Custom_DO_With_Properties");

            // Property value. An arbitrary string
            string propertyValue = "Test";

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            // Read the base Xaml file, and add-on to it.
            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(_testPropertiesBaseXamlFile);

            // Get the root element. We will set the property on it.
            XmlElement rootElement = doc.DocumentElement;

            switch (Xaml_manifestation)            
            {
                case "Attribute":
                    // We need to set the property as an attribute. 
                    string attrName;
                    if (isAttached)
                    {
                        attrName = className + "." + propertyName;
                    }
                    else
                    {
                        attrName = propertyName;
                    }
                    rootElement.SetAttribute(attrName, _customNS, propertyValue);
                    break;

                case "Element":
                    // We need to set the property as a property element.
                    string propElementName = className + "." + propertyName;
                    XmlElement propElement = doc.CreateElement("cc", propElementName, _customNS);
                    propElement.InnerText = propertyValue;
                    rootElement.AppendChild(propElement);
                    break;
            }
            // Save the Xaml.
            doc.Save(_testXamlFile);

            // Decide how should the error messages look like, 
            // when we parse and compile the Xaml
            string[] errPieces = null;
            
            if(Property_access == "Private" || Property_access == "Internal")
            {
                
                if(Xaml_manifestation == "Attribute") 
                {
                    errPieces = new string[3];
                    errPieces[0] = (isAttached ? (className + "." + propertyName) : propertyName);
                    errPieces[1] = "property";
                    errPieces[2] = "does not";
                }
                else if(Xaml_manifestation == "Element")
                {
                    errPieces = new string[2];
                    errPieces[0] = className + "." + propertyName;
                    errPieces[1] = "does not";
                }
            }
            else if (Property_access == "NonPublicSetter")
            {
                errPieces = new string[3];
                errPieces[0] = className + "." + propertyName;
                errPieces[1] = "cannot be set";
                errPieces[2] = "does not have an accessible set accessor";
            }
            else if (Property_access == "InternalTypeConverter")
            {
                errPieces = new string[3];
                errPieces[0] = propertyValue;
                errPieces[1] = "not a valid value for";
                errPieces[2] = propertyName;
            }

            // Process the Xaml and verify that the errors thrown contain 
            // the expected pieces.
            ProcessXamlAndVerifyErrors(_testXamlFile, true, errPieces);
            return true;
        }
        #endregion TestProperties

        #region TestEvents
        /// <summary>
        /// Handler for TestEvents.
        /// 
        /// Here we read a base Xaml file, set different types of non-public events  
        /// (depending on the input parameters) on its root element, and then save a new Xaml.
        /// 
        /// The resultant Xaml file is compiled, and we make sure that expected 
        /// errors are thrown (since we always set non-public events, we expect all Xamls
        /// to fail.
        /// 
        /// </summary>
        /// <remarks>Handler for TestEvents</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestEvents(State endState, State inParams, State outParams)
        {
            // Read the values of various parameters
            CoreLogger.LogStatus("Reading input parameter values");
            string Event_type = inParams["Event type"];
            string Event_access = inParams["Event access"];
            string Xaml_manifestation = inParams["Xaml manifestation"];

            // Is it going to be an attached event?
            bool isAttached = (Event_type == "Normal" ? false : true);

            // Create the name of the event from the parameters
            string eventName = (isAttached ? "Attached" : "") + Event_access + "Event";

            // Name of the class that holds the Event
            string className = (isAttached ? "Custom_Event_Attacher" : "Custom_Class_With_Events");

            // Event value. Name of the event handler.
            string eventValue = "OnEvent";

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            // Read the base Xaml file, and add-on to it.
            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(_testEventsBaseXamlFile);

            // Get the root element. We will set the event on it.
            XmlElement rootElement = doc.DocumentElement;

            switch (Xaml_manifestation)
            {
                case "Attribute":
                    // We need to set the event as an attribute. 
                    string attrName;
                    if (isAttached)
                    {
                        attrName = className + "." + eventName;
                    }
                    else
                    {
                        attrName = eventName;
                    }
                    rootElement.SetAttribute(attrName, _customNS, eventValue);
                    break;

                case "Element":
                    // We need to set the event as an Event element.
                    string eventElementName = className + "." + eventName;
                    XmlElement eventElement = doc.CreateElement("cc", eventElementName, _customNS);
                    eventElement.InnerText = eventValue;
                    rootElement.PrependChild(eventElement);
                    break;
            }
            // Save the Xaml.
            doc.Save(_testXamlFile);

            // Decide how should the error messages look like, 
            // when we compile the Xaml (we shouldn't parse the Xaml,
            // since it has events).
            string[] errPieces = null;

            if (Event_access == "Private" || Event_access == "Internal")
            {

                if (Xaml_manifestation == "Attribute")
                {
                    errPieces = new string[4];
                    errPieces[0] = (isAttached ? (className + "." + eventName) : eventName);
                    errPieces[1] = "cannot be set";
                    errPieces[2] = "does not have";
                    errPieces[3] = "accessible";
                }
                else if (Xaml_manifestation == "Element")
                {
                    errPieces = new string[2];
                    errPieces[0] = className + "." + eventName;
                    errPieces[1] = "does not";
                }
            }

            // Process the Xaml (compile only) and verify that the errors thrown contain 
            // the expected pieces.
            ProcessXamlAndVerifyErrors(_testXamlFile, false, errPieces);

            return true;
        }
        #endregion TestEvents

        #region TestMarkupExtensions
        /// <summary>
        /// Handler for TestMarkupExtensions.
        /// 
        /// Here we read a base Xaml file, set the value of a property on the root 
        /// element to MarkupExtensions having non-public (private and internal)  
        /// constructors, and then save a new Xaml.
        /// 
        /// The resultant Xaml file is parsed and compiled, and we make sure that expected 
        /// errors are thrown (since we always use MarkupExtensions with non-public ctors, 
        /// we expect all Xamls to fail.
        /// 
        /// </summary>
        /// <remarks>Handler for TestMarkupExtensions</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestMarkupExtensions(State endState, State inParams, State outParams)
        {
            // Read the values of various parameters
            CoreLogger.LogStatus("Reading input parameter values");
            string Constructor_access = inParams["Constructor access"];

            // Create the name of the MarkupExtension from the parameters
            string markupExtensionName = "Custom_MarkupExt_With_" + Constructor_access + "_Ctor"; 

            // MarkupExtension's positional (constructor) parameters. 
            string param1 = "Foo";
            string param2 = "Bar";

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            // Read the base Xaml file, and add-on to it.
            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(_testMarkupExtensionsBaseXamlFile);

            // Get the root element. We will set the MarkupExtension on it.
            XmlElement rootElement = doc.DocumentElement;
            string attrValue = "{cc:" + markupExtensionName + " " + param1 + "," + param2 + "}";
            rootElement.SetAttribute("Content", _avalonNS, attrValue);

            // Save the Xaml.
            doc.Save(_testXamlFile);

            // Decide how should the error messages look like, 
            // when we parse and compile the Xaml
            string[] errPieces = new string[3];
            errPieces[0] = "Cannot find";
            errPieces[1] = "public constructor";
            errPieces[2] = markupExtensionName;

            // Process the Xaml and verify that the errors thrown contain 
            // the expected pieces.
            ProcessXamlAndVerifyErrors(_testXamlFile, true, errPieces);

            return true;
        }
        #endregion TestMarkupExtensions

        //#region ProcessXamlAndVerifyErrors
        //// Compile and optionally load the Xaml, catch the error, and verify that
        //// the error message contains the strings specified.
        //private void ProcessXamlAndVerifyErrors(string xamlFile, bool shouldLoad, string[] errPieces)
        //{
        //    TestExecutor testExecutor;
        //    Hashtable expectedErrData, actualErrData;
        //    string errMesg;
        //    Exception parseException = null, compileException = null;


        //    string securityLevel = DriverState.DriverParameters["SecurityLevel"];
	
        //    bool isFullTrust = false;

        //    if (String.Compare(securityLevel,"FullTrust", true) == 0)
        //    {
        //        isFullTrust = true;
        //    }

        //    #region Parse mode
        //    if (shouldLoad)
        //    {
        //        CoreLogger.LogStatus("\n Parsing the Xaml and getting the error information.");

        //        testExecutor = new XamlLoadTestExecutor();
        //        // Populate expectedErrData, in order to pass it to TestExecutor.
        //        // Generally, expectedErrData contains the name of the Xaml file, the expected error message, line,
        //        // position, etc. But in this case we will cheat a little bit, and only populate it
        //        // with the name of the Xaml file, since TestExecutor just needs that.
        //        expectedErrData = new Hashtable();
        //        expectedErrData["XamlFileName"] = xamlFile;

        //        // Parse the Xaml and get the actual error info.
        //        actualErrData = testExecutor.Run(expectedErrData);
        //        // Get the error message string.
        //        errMesg = actualErrData["ErrorMessage"] as string;
        //        CoreLogger.LogStatus("Xaml parse error: " + errMesg);
        //        try
        //        {
        //            VerifyErrorContainsPieces(errMesg, errPieces);
        //        }
        //        catch (Exception e) { parseException = e; }
        //    }
        //    #endregion Parse mode

        //    #region Compile mode
        //    // 







        //        testExecutor = new XamlCompileTestExecutor();
        //        // Populate expectedErrData just with the name of the Xaml file, 
        //        // like we did in Parse mode above
        //        expectedErrData = new Hashtable();
        //        expectedErrData["XamlFileName"] = xamlFile;

        //        // Compile the Xaml and get the actual error info.
        //        actualErrData = testExecutor.Run(expectedErrData);
        //        // Get the error message string.
        //        errMesg = actualErrData["ErrorMessage"] as string;
        //        CoreLogger.LogStatus("\n Build error: " + errMesg);
        //        try
        //        {
        //            VerifyErrorContainsPieces(errMesg, errPieces);
        //        }
        //        catch (Exception e) { compileException = e; }
        //    }
        //    else
        //    {
        //        CoreLogger.LogStatus("\n Skipping compilation, since we are in Partial trust.");
        //    }
        //    #endregion Compile mode

        //    if (shouldLoad)
        //    {
        //        if (parseException != null)
        //        {
        //            CoreLogger.LogStatus("\n Parsing the Xaml didn't throw expected exception. " + parseException.Message, ConsoleColor.Red);
        //        }
        //        else
        //        {
        //            CoreLogger.LogStatus("\n Parsing the Xaml threw expected exception.");
        //        }
        //    }

        //    if (isFullTrust) //We compile only in Full trust mode
        //    {
        //        if (compileException != null)
        //        {
        //            CoreLogger.LogStatus("\n Compiling the Xaml didn't throw expected exception. " + compileException.Message, ConsoleColor.Red);
        //        }
        //        else
        //        {
        //            CoreLogger.LogStatus("\n Compiling the Xaml threw expected exception.");
        //        }
        //    }

        //    // Fail the test if either parsing or compilation didn't throw expected exceptions.
        //    if (parseException != null || compileException != null)
        //    {
        //        throw new Microsoft.Test.TestValidationException("Test failed.");
        //    }
        //}
        //#endregion ProcessXamlAndVerifyErrors

        // Checks that the given error message contains all the given substrings (not necessarily in that order)
        private void VerifyErrorContainsPieces(string errMesg, string[] errPieces)
        {
            bool containsAll = true;
            int i;
            for (i = 0; i < errPieces.Length; i++)
            {
                if (errMesg.IndexOf(errPieces[i]) == -1)
                {
                    containsAll = false;
                    break;
                }
            }

            if (!containsAll)
            {
                throw new Microsoft.Test.TestValidationException("Error message was expected to contain, " +  
                        "but doesn't contain the following string: " + errPieces[i]);
            }
        }

        private const string _testXamlFile = "__AccessControlModelTempFile.xaml";
        private const string _testEventsBaseXamlFile = "AccessControlModel_TestEvents_Base.xaml";
        private const string _testMarkupExtensionsBaseXamlFile = "AccessControlModel_TestMarkupExtensions_Base.xaml";
        private const string _testPropertiesBaseXamlFile = "AccessControlModel_TestProperties_Base.xaml";
        private const string _avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string _xamlNS = "http://schemas.microsoft.com/winfx/2006/xaml";
        private const string _customNS = "clr-namespace:Avalon.Test.CoreUI.Parser;assembly=CoreTestsUntrusted";
    }
}
//This file was generated using MDE on: Wednesday, June 22, 2005 4:20:30 PM
