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
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows;
using System.Windows.Markup;

using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Xaml.Markup;

using Microsoft.Test;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Model to test Loaded event handlers and event triggers.
    /// </summary>
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 1, 1, 0, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel1", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 2, 2, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.PartialTrust, @"StaticLoadedModel2", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 3, 3, 0, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel3", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 4, 4, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel4", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 5, 5, 0, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel5", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 6, 6, 0, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel6", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 7, 7, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel7", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 8, 8, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel8", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 9, 9, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel9", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 10, 10, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel10", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 11, 11, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel11", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 12, 12, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel12", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 13, 13, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel13", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 14, 14, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel14", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 15, 15, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel15", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    [Model(@"FeatureTests\ElementServices\StaticLoadedModel.xtc", 16, 16, 1, @"Events\Loaded\StaticModel", TestCaseSecurityLevel.FullTrust, @"StaticLoadedModel16", SupportFiles=@"FeatureTests\ElementServices\StaticLoadedModel_MarkupSnippets.xaml,FeatureTests\ElementServices\StaticLoadedModelBase.xaml", Timeout=180)]
    public class StaticLoadedModel : CoreModel 
    {
        /// <summary>
        /// Creates a StaticLoadedModel instance
        /// </summary>
        public StaticLoadedModel()
            : base()
        {
            Name = "StaticLoadedModel";

            //Add Action Handlers
            AddAction("TestLoaded", new ActionHandler(TestLoaded));
        }

        #region TestLoaded
        /// <summary>
        /// Handler for TestLoaded.
        /// 
        /// Here we construct various Xaml files that test Loaded event handlers
        /// and Loaded event triggers for FrameworkElements and FrameworkContentElements.
        ///
        /// The resultant Xaml file is parsed, and the tree built is verified by an
        /// independent verifier.
        /// Then the Xaml is compiled, to create an app. The app is run and the tree built 
        /// is again verified by the independent verifier.
        /// 
        /// We persist our state in a way that can be consumed by the verifier, so that
        /// it can decide what the tree created from Xaml should look like.
        /// </summary>
        /// <remarks>Handler for TestLoaded</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool TestLoaded(State endState, State inParams, State outParams)
        {
            // Read values of various inParams.
            string ElementType = inParams["ElementType"];
            string ElementUsageLocation = inParams["ElementUsageLocation"];
            string TreeHost = inParams["TreeHost"];
            string LoadedEventUsage = inParams["LoadedEventUsage"];
            
            if((ElementType == "FrameworkContentElement") && 
                (ElementUsageLocation == "ElementTree") &&
                (LoadedEventUsage == "EventTrigger"))
            {
                return true; //FrameworkContentElement doesn't have a Triggers property.
            }

            string element = null; // Element whose Loaded event will be used.
            // We have pre-chosen the elements to be used.
            switch (ElementType)
            {
                case "FrameworkElement":
                    element = "PasswordBox";
                    break;

                case "FrameworkContentElement":
                    element = "FlowDocument";
                    break;
            }

            // Replace the placeholder string in the markup snippets file with 
            // the actual element we want to use.
            string markupSnippetString = File.ReadAllText(_snippetFile);
            markupSnippetString = markupSnippetString.Replace("Placeholder", element);

            // If this is a Template case, replace the string "FrameworkTemplate" 
            // with the appropriate template (ControlTemplate/DataTemplate).
            switch (ElementUsageLocation)
            {
                case "ControlTemplate":
                case "StyleInControlTemplate":
                    markupSnippetString = markupSnippetString.Replace("FrameworkTemplate", "ControlTemplate");
                    break;

                case "DataTemplate":
                case "StyleInDataTemplate":
                    markupSnippetString = markupSnippetString.Replace("FrameworkTemplate", "DataTemplate");
                    break;                    
            }

            // Create main XmlDocument.
            // We start from a barebone base file - which just contains the root
            // "Page" tag - and then build on it.
            XmlDocument tempDoc = new XmlDocument();
            tempDoc.PreserveWhitespace = true;
            tempDoc.Load(_baseXamlFile);
            XmlDocument _mainDoc = tempDoc;
            XmlElement _mainDocRoot = _mainDoc.DocumentElement;

            XmlNode snippetNode; 
            if (ElementUsageLocation != "ElementTree")   //Style, ControlTemplate etc.
            {
                // Put the Style or Template definition under <Page.Resources>
                XmlNode resourcesNode = _mainDoc.CreateElement("", "Page.Resources", _avalonNS);
                snippetNode = GetSnippet(markupSnippetString, ElementUsageLocation + LoadedEventUsage, _mainDoc);
                resourcesNode.AppendChild(snippetNode);
                _mainDocRoot.AppendChild(resourcesNode);

                // Put the actual element (i.e. element using the Style or Template) under <Page>
                snippetNode = GetSnippet(markupSnippetString, ElementUsageLocation + "Target", _mainDoc);
                _mainDocRoot.AppendChild(snippetNode);
            }
            else
            {
                // No need for <Page.Resources>. Just put the actual element under <Page>
                snippetNode = GetSnippet(markupSnippetString, ElementUsageLocation + LoadedEventUsage, _mainDoc);
                _mainDocRoot.AppendChild(snippetNode);
            }

            // For LoadedEventUsage = EventHandler, we need to put in the <x:Code>
            // block containing the event handler
            if (LoadedEventUsage == "EventHandler")
            {
                snippetNode = GetSnippet(markupSnippetString, "LoadedHandlerCode", _mainDoc);
                _mainDocRoot.AppendChild(snippetNode);
            }

            CoreLogger.LogStatus("Deleting previously generated Xaml file, if any.");
            File.Delete(_testXamlFile);

            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            tempDoc.Save(_testXamlFile);

            // Save the inParams so that the verification routine can get at them.
            // However, State class is not serializable, so we first convert 
            // inParams into a CoreModelState instance, and then persist that instance.
            CoreModelState cms = new CoreModelState(inParams);
            CoreModelState.Persist(cms);

            // Make the Xaml go through load or compile or both routes, as appropriate
            // and verify the tree has the right content.

            // For LoadedEventUsage = EventHandler, Xaml doesn't support loading. It
            // has to be compiled.

            // 



            try
            {
                // Load route.  
                // Parse the Xaml, display the tree, and call verification routine, if any.
                if (LoadedEventUsage != "EventHandler")
                {
                    CoreLogger.LogStatus("Putting the Xaml through XamlLoad route");
                    object root = ParserUtil.ParseXamlFile(_testXamlFile);
                    (new SerializationHelper()).DisplayTree(root as UIElement);
                }
                else
                {
                    CoreLogger.LogStatus(@"Skipping XamlLoad route, since the Xaml contains" 
                            + " event handlers or style EventSetters.");
                }

                // Compile route.
                if (DriverState.DriverParameters["SecurityLevel"] == "FullTrust")
                {
                    CoreLogger.LogStatus("Putting the Xaml through XamlCompile route");
                    XamlTestRunner runner = new XamlTestRunner();
                    runner.RunCompileCase(_testXamlFile, "Application");
                }
                else
                {
                    CoreLogger.LogStatus("\n Skipping compilation, since we are in Partial trust.");
                }
            }
            catch(Exception)
            {
                throw;
            }
            return true;
        }

        private XmlElement GetSnippet(string markupString, string snippetName, XmlDocument destContext)
        {
            // Create a DOM from the snippets string. 
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(new StringReader(markupString));

            XmlElement snippet = doc.SelectSingleNode("//*[@SnippetID='" + snippetName + "']") as XmlElement;
            snippet.RemoveAttribute("SnippetID");
           
            return (XmlElement)destContext.ImportNode(snippet, true);
        }
        #endregion TestLoaded

        private const string _testXamlFile = "__StaticLoadedModelTempFile.xaml";
        private const string _snippetFile = "StaticLoadedModel_MarkupSnippets.xaml";
        private const string _baseXamlFile = "StaticLoadedModelBase.xaml";
        private const string _avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string _xamlNS = "http://schemas.microsoft.com/winfx/2006/xaml";
    }
}
//This file was generated using MDE on: Wednesday, June 22, 2005 4:20:30 PM
