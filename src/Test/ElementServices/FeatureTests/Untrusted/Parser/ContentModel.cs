// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 4 $
 
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
using Avalon.Test.Xaml.Markup;
namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Model to test Xaml content.
    /// </summary>
    [Model(@"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderObjects.xtc", 1, 4, 0, @"Parser\ContentModel\Objects\FullTrust", TestCaseSecurityLevel.FullTrust, "",SupportFiles = @"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderObjects_Base.xaml,FeatureTests\ElementServices\ContentModel_TestObjectContentUnderProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderObjects.xtc", 5, 23, 1, @"Parser\ContentModel\Objects\FullTrust", TestCaseSecurityLevel.FullTrust, "",SupportFiles = @"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderObjects_Base.xaml,FeatureTests\ElementServices\ContentModel_TestObjectContentUnderProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderProperties.xtc", 1, 4, 0, @"Parser\ContentModel\Properties\FullTrust", TestCaseSecurityLevel.FullTrust, "",SupportFiles = @"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderObjects_Base.xaml,FeatureTests\ElementServices\ContentModel_TestObjectContentUnderProperties_Base.xaml")]
    [Model(@"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderProperties.xtc", 5, 66, 1, @"Parser\ContentModel\Properties\FullTrust", TestCaseSecurityLevel.FullTrust, "",SupportFiles = @"FeatureTests\ElementServices\ContentModel_TestObjectContentUnderObjects_Base.xaml,FeatureTests\ElementServices\ContentModel_TestObjectContentUnderProperties_Base.xaml")]    
    public class ContentModel : CoreModel 
    {
        /// <summary>
        /// Creates a ContentModel instance
        /// </summary>
        public ContentModel()
            : base()
        {
            Name = "ContentModel";

            //Add Action Handlers
            AddAction("TestObjectContentUnderObjects", new ActionHandler(TestObjectContentUnderObjects));
            AddAction("TestObjectContentUnderProperties", new ActionHandler(TestObjectContentUnderProperties));
        }

        #region TestObjectContentUnderObjects
        /// <summary>
        /// Handler for TestObjectContentUnderObjects.
        /// 
        /// Here we construct a Xaml file that tests object content under objects.
        /// The type of parent object is decided depending on the values of various
        /// inParams.
        /// The type and number of content objects will be decided by inParams, too.
        /// 
        /// The resultant Xaml file is parsed, and the tree built is verified by an
        /// independent verifier.
        /// Then the Xaml is compiled, to create an app. The app is run and the tree built 
        /// is again verified by the independent verifier.
        /// 
        /// We persist our state in a way that can be consumed by the verifier, so that
        /// it can decide what the tree created from Xaml should look like.
        /// </summary>
        /// <remarks>Handler for TestObjectContentUnderObjects</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestObjectContentUnderObjects(State endState, State inParams, State outParams)
        {
            // Read values of various inParams.
            string Type_of_parent = inParams["Type of parent"];
            string Parent_Interface = inParams["Parent Interface"];
            string CPA_property_type = inParams["CPA property type"];
            string CPA_property_readability = inParams["CPA property readability"];
            string Content = inParams["Content"];

            // Decide the type of Parent object
            string Parent_object;
            if ((CPA_property_type == "NotApplicable") || (CPA_property_readability == "NotApplicable"))
            {
                Parent_object = "Custom_" + Type_of_parent + "_With_" + Parent_Interface;
            }
            else
            {
                Parent_object = "Custom_" + Type_of_parent + "_With_" + Parent_Interface
                                + "_" + CPA_property_type + "_" + CPA_property_readability;
            }
            CoreLogger.LogStatus("Parent object: " + Parent_object);

            // Decide whether the type of Parent_object implement IDictionary.
            bool parentIDictionary = Parent_Interface.Contains("IDictionary");

            // Decide the number and types of content objects
            string[] Content_objects = null;
            switch (Content)
            {
                case "Single object of type T":
                    Content_objects = new string[1] { "Button" };
                    break;

                case "Single object not of type T":
                    Content_objects = new string[1] { "ListBox" };
                    break;

                case "Multiple objects of type T":
                    Content_objects = new string[3] { "Button", "Button", "Button" };
                    break;

                case "Multiple objects of mixed type":
                    Content_objects = new string[3] { "ListBox", "Button", "Image" };
                    break;
            }

            // Log the content objects.
            string ContentObjectsList = "";
            foreach(string Content_object in Content_objects)
            {
                ContentObjectsList = ContentObjectsList + Content_object + " , ";
            }
            CoreLogger.LogStatus("Content objects: " + ContentObjectsList);

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            GenerateXaml_ObjectContentUnderObjects(Parent_object, Content_objects, parentIDictionary);

            // Initialize model state instance to be used in verifiers.
            ContentModelState _modelState = new ContentModelState();
            _modelState.Parent_object = Parent_object;
            _modelState.Content_objects = Content_objects;
            ContentModelState.Persist(_modelState);

            // Make the Xaml go through both load or compile routes.
            // In both cases, verify the tree has the right content.

            // 



            
            
                // Load route.  
                // Parse the Xaml, display the tree, and call verification routine, if any.
                CoreLogger.LogStatus("Putting the Xaml through XamlLoad route");
                object root = ParserUtil.ParseXamlFile(_testXamlFile);                
                (new SerializationHelper()).DisplayTree(root as UIElement);

                // Compile route.
            string securityLevel = DriverState.DriverParameters["SecurityLevel"];
	
            bool isFullTrust = false;

            if (String.Compare(securityLevel,"FullTrust", true) == 0)
            {
                isFullTrust = true;
            }

                if (isFullTrust)
                {
                    CoreLogger.LogStatus("Putting the Xaml through XamlCompile route");
                    XamlTestRunner runner = new XamlTestRunner();
                    runner.RunCompileCase(_testXamlFile, "Application");
                }
                else
                {
                    CoreLogger.LogStatus("\nSkipping compilation, since we are in Partial trust.");
                }
                return true;
            

        }

        /// <summary>
        /// Generate a Xaml with the specified content objects under the specified
        /// parent object. It also generates keys for content objects if the parent 
        /// object implements IDictionary (i.e. if the third parameter is true).
        /// 
        /// This method starts with a base Xaml file (that contains the Mapping PIs, 
        /// specifies the verifier routine, etc.) and adds on to it.
        /// </summary>
        /// <param name="Parent_object"></param>
        /// <param name="Content_objects"></param>
        /// <param name="parentIDictionary"></param>
        private void GenerateXaml_ObjectContentUnderObjects(string Parent_object, string[] Content_objects, bool parentIDictionary)
        {
            // Read the base Xaml file, and add-on to it.
            XmlDocument doc = new XmlDocument();
            doc.Load(_testObjectContentUnderObjectsBaseXamlFile);

            XmlElement rootElement = doc.DocumentElement;
            // Create the parent element.
            XmlElement parentElement = doc.CreateElement("cc", Parent_object, _customNS);
            rootElement.AppendChild(parentElement);
            // Create children elements.
            foreach (string Content_object in Content_objects)
            {
                XmlElement childElement = doc.CreateElement("", Content_object, _avalonNS);
                if (parentIDictionary)
                {
                    // Set x:Key attribute on the child.
                    // For convenience, we set the type name of child as it's x:Key
                    // e.g. <Button x:Key="Button" />
                    childElement.SetAttribute("Key", _xamlNS, Content_object);
                }
                parentElement.AppendChild(childElement);
            }
            XmlTextWriter writer = new XmlTextWriter((new StreamWriter(_testXamlFile)));
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            doc.Save(writer);
            writer.Close();
        }
        #endregion TestObjectContentUnderObjects

        #region TestObjectContentUnderProperties
        /// <summary>
        /// Handler for TestObjectContentUnderProperties.
        /// 
        /// Here we construct a Xaml file that tests object content under (compound) properties.
        /// The type of parent object, type of the property, type and number of content 
        /// objects, etc. is decided depending on the values of various
        /// inParams.
        /// 
        /// The resultant Xaml file is parsed, and the tree built is verified by an
        /// independent verifier.
        /// Then the Xaml is compiled, to create an app. The app is run and the tree built 
        /// is again verified by the independent verifier.
        /// 
        /// We persist our state in a way that can be consumed by the verifier, so that
        /// it can decide what the tree created from Xaml should look like.
        /// </summary>
        /// <remarks>Handler for TestObjectContentUnderProperties</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestObjectContentUnderProperties(State endState, State inParams, State outParams)
        {
            // Read values of various inParams.
            string Type_of_parent = inParams["Type of parent"];
            string Property_kind = inParams["Property kind"];
            string Property_category = inParams["Property category"];
            string Type_of_property = inParams["Type of property"];
            string Property_readability = inParams["Property readability"];
            string Stored_property_value = inParams["Stored property value"];
            string Object_tag_under_property = inParams["Object tag under property"];
            string Content = inParams["Content"];

            // Determine the type of parent object from the parameters 
            string Parent_object;
            Parent_object = "Custom_" + Type_of_parent + "_With_Properties";
            CoreLogger.LogStatus("Parent object: " + Parent_object);

            // Is it going to be an attached property?
            bool isAttached = (Property_category == "Normal" ? false : true);

            // Create the name of the property from the parameters
            string propertyName; 
            if (Stored_property_value != "NotApplicable")
            {
                propertyName = Type_of_property + "_" + Property_kind + "_"
                    + Property_readability + "_" + Stored_property_value;
            }
            else
            {
                propertyName = Type_of_property + "_" + Property_kind + "_"
                    + Property_readability;
            }

            // Name of the class that holds the property
            string className;
            if (isAttached)
            {
                // Custom_ClrProp_Attacher or Custom_DP_Attacher 
                className = "Custom_" + Property_kind + "_Attacher";
            }
            else
            {
                className = Parent_object;
            }

            string qualifiedPropertyName = className + "." + propertyName;

            // Type of the property. It's needed if Object_tag_under_property == "Explicit", 
            // i.e. if we want to produce a markup like:
            // <ListBox.Items>
            //    <ItemsCollection>   <-- explicit object tag.
            //       <ListBoxItem>
            //       .....   

            string[] explicitTagName = null;
            switch(Type_of_property)
            {
                case "Singular":
                    explicitTagName = new string[3] { "", "Button", _avalonNS };
                    break;

                case "Array":
                    explicitTagName = new string[3] { "", "Button[]", _avalonNS };
                    break;

                case "IList":
                    explicitTagName = new string[3] { "sys", "ArrayList", _sysCollectionsNS };
                    break;

                case "IDictionary":
                    explicitTagName = new string[3] { "sys", "Hashtable", _sysCollectionsNS };
                    break;

                case "IListActualType":
                    explicitTagName = new string[3] { "sys", "IList", _sysCollectionsNS };
                    break;

                case "IListRestrictsTypes":
                    explicitTagName = new string[3] { "cc", "ArrayListRestrictsTypes", _customNS };
                    break;

                case "IDictionaryActualType":
                    explicitTagName = new string[3] { "sys", "IDictionary", _sysCollectionsNS };
                    break;

                case "IDictionaryRestrictsTypes":
                    explicitTagName = new string[3] { "cc", "HashtableRestrictsTypes", _customNS };
                    break;
            }

            // Determine whether the property type implement IDictionary.
            // If yes, we need to generate x:Key attributes.
            bool propertyIDictionary = Type_of_property.Contains("IDictionary");

            // Decide the number and types of content objects
            string[] Content_objects = null;
            switch (Content)
            {
                case "Single object of type T":
                    Content_objects = new string[1] { "Button" };
                    break;

                case "Single object not of type T":
                    Content_objects = new string[1] { "ListBox" };
                    break;

                case "Multiple objects of type T":
                    Content_objects = new string[3] { "Button", "Button", "Button" };
                    break;

                case "Multiple objects of mixed type":
                    Content_objects = new string[3] { "ListBox", "Button", "Image" };
                    break;
            }

            // Log the content objects.
            string ContentObjectsList = "";
            foreach (string Content_object in Content_objects)
            {
                ContentObjectsList = ContentObjectsList + Content_object + " , ";
            }
            CoreLogger.LogStatus("Content objects: " + ContentObjectsList);

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            bool explicitTag = (Object_tag_under_property == "Explicit");
            GenerateXaml_ObjectContentUnderProperties(Parent_object, qualifiedPropertyName, explicitTagName, Content_objects, propertyIDictionary, explicitTag, inParams);

            // Initialize model state instance to be used in verifiers.
            ContentModelState _modelState = new ContentModelState(inParams);
            _modelState.Parent_object = Parent_object;
            _modelState.Content_objects = Content_objects;
            _modelState.Dictionary["isAttached"] = isAttached;
            _modelState.Dictionary["propertyName"] = propertyName;
            _modelState.Dictionary["className"] = className;
            _modelState.Dictionary["qualifiedPropertyName"] = qualifiedPropertyName;
            ContentModelState.Persist(_modelState);

            // Make the Xaml go through both load or compile routes.
            // In both cases, verify the tree has the right content.

            // 



            
            
                // Load route.  
                // Parse the Xaml, display the tree, and call verification routine, if any.
                CoreLogger.LogStatus("Putting the Xaml through XamlLoad route");
                object root = ParserUtil.ParseXamlFile(_testXamlFile);
                (new SerializationHelper()).DisplayTree(root as UIElement);

                // Compile route.
            string securityLevel = DriverState.DriverParameters["SecurityLevel"];
	
            bool isFullTrust = false;

            if (String.Compare(securityLevel,"FullTrust", true) == 0)
            {
                isFullTrust = true;
            }


                if (isFullTrust)
                {
                    CoreLogger.LogStatus("Putting the Xaml through XamlCompile route");
                    XamlTestRunner runner = new XamlTestRunner();
                    runner.RunCompileCase(_testXamlFile, "Application");
                }
                else
                {
                    CoreLogger.LogStatus("\nSkipping compilation, since we are in Partial trust.");
                }
                return true;
        }

        private void GenerateXaml_ObjectContentUnderProperties(string Parent_object, string qualifiedPropertyName, string[] explicitTagName, string[] Content_objects, bool propertyIDictionary, bool explicitTag, State inParams)
        {
            // Read the base Xaml file, and add-on to it.
            XmlDocument doc = new XmlDocument();
            doc.Load(_testObjectContentUnderPropertiesBaseXamlFile);

            XmlElement rootElement = doc.DocumentElement;
            // Create the parent element.
            XmlElement parentElement = doc.CreateElement("cc", Parent_object, _customNS);
            rootElement.AppendChild(parentElement);

            // Add the compound property tag under the parent object,
            // and an (optional) explicit object tag under the compound property tag.
            XmlElement propertyElement = doc.CreateElement("cc", qualifiedPropertyName, _customNS);
            parentElement.AppendChild(propertyElement);

            XmlElement contentHolderElement; //element under which content will be placed.
            if (explicitTag)
            {
                XmlElement explicitTagElement = doc.CreateElement(explicitTagName[0], explicitTagName[1], explicitTagName[2]);
                // If it's a RO, Non-null return value, Explicit tag scenario with IDictionary,
                // it's a Dictionary-of-dictionaries, so the explicit tag needs an x:Key
                if ((inParams["Property readability"] == "RO") &&
                    (inParams["Stored property value"] == "NonNull") &&
                    propertyIDictionary)
                {
                    explicitTagElement.SetAttribute("Key", _xamlNS, "key0");
                }
                propertyElement.AppendChild(explicitTagElement);
                contentHolderElement = explicitTagElement;
            }
            else
            {
                contentHolderElement = propertyElement;
            }

            // Create children elements.
            int i = 0;
            foreach (string Content_object in Content_objects)
            {
                XmlElement childElement = doc.CreateElement("", Content_object, _avalonNS);
                if (propertyIDictionary)
                {
                    // Set x:Key attribute on the child to "key" + i,
                    // i.e. key0, key1, etc.
                    childElement.SetAttribute("Key", _xamlNS, "key"+i);
                }
                contentHolderElement.AppendChild(childElement);
                i++;
            }

            XmlTextWriter writer = new XmlTextWriter((new StreamWriter(_testXamlFile)));
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            doc.Save(writer);
            writer.Close();
        }
        #endregion TestObjectContentUnderProperties

        private const string _testXamlFile = "__ContentModelTempFile.xaml";
        private const string _testObjectContentUnderObjectsBaseXamlFile = "ContentModel_TestObjectContentUnderObjects_Base.xaml";
        private const string _testObjectContentUnderPropertiesBaseXamlFile = "ContentModel_TestObjectContentUnderProperties_Base.xaml";
        private const string _avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string _xamlNS = "http://schemas.microsoft.com/winfx/2006/xaml";
        private const string _customNS = "clr-namespace:Avalon.Test.CoreUI.Parser;assembly=CoreTestsUntrusted";
        private const string _sysCollectionsNS = "clr-namespace:System.Collections;assembly=mscorlib";
    }


}
//This file was generated using MDE on: Wednesday, June 22, 2005 4:20:30 PM
