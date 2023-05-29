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
using System.Collections.Generic;
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
    /// Model to test Source-able properties, like ResourceDictionary.Source, 
    /// Frame.Source etc.
    /// Currently we only cover ResourceDictionary.Source.
    /// </summary>
    [Model(@"FeatureTests\ElementServices\SourceablePropertyModel_TestSourceableProperties.xtc", 1, 4, 0, @"Parser\SourceablePropertyModel\FullTrust", TestCaseSecurityLevel.FullTrust, "",SupportFiles= @"FeatureTests\ElementServices\SourceablePropertyModel_TestSourceableProperties_Base.xaml,FeatureTests\ElementServices\ExternalRD1.xaml")]
    [Model(@"FeatureTests\ElementServices\SourceablePropertyModel_TestSourceableProperties.xtc", 5, 14, 1, @"Parser\SourceablePropertyModel\FullTrust", TestCaseSecurityLevel.FullTrust, "",SupportFiles= @"FeatureTests\ElementServices\SourceablePropertyModel_TestSourceableProperties_Base.xaml,FeatureTests\ElementServices\ExternalRD1.xaml")]    
    public class SourceablePropertyModel : CoreModel 
    {
        /// <summary>
        /// Creates a SourceablePropertyModel instance
        /// </summary>
        public SourceablePropertyModel()
            : base()
        {
            Name = "SourceablePropertyModel";

            //Add Action Handlers
            AddAction("TestSourceableProperties", new ActionHandler(TestSourceableProperties));
        }

        #region TestSourceableProperties
        /// <summary>
        /// Handler for TestSourceableProperties.
        /// 
        /// Here we construct a Xaml file that tests source-able properties.
        /// 
        /// The resultant Xaml file is either compiled (to create an app), or parsed,
        /// depending on the input parameters.
        /// In either case the resultant tree is verified by an independent verifier.
        /// 
        /// </summary>
        /// <remarks>Handler for TestSourceableProperties</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        public bool TestSourceableProperties(State endState, State inParams, State outParams)
        {
            // Read values of various input parameters.
            string Property = inParams["Property"];
            string Uri_form = inParams["Uri form"];
            string Uri_prefix = inParams["Uri prefix"];
            string App_type = inParams["App type"];
            string Build_action = inParams["Build action"];

            // Determine the Uri string depending on the Uri form and Uri prefix.
            // First determine the filePath.
            string filePath = null;
            string newDir = null;
            switch (Uri_form)
            {
                case "NonQualifiedUri":
                    filePath = _extResourceDictionaryFile;
                    break;

                case "RelativeUri":
                    // 

                    newDir = "SourceablePropertyModelFolder";
                    if (Directory.Exists(newDir))
                    {
                        CoreLogger.LogStatus("Found folder " + newDir + ". Deleting...");
                        Directory.Delete(newDir, true);
                    }
                    Directory.CreateDirectory(newDir);
                    File.Move(_extResourceDictionaryFile, newDir + "\\" + _extResourceDictionaryFile);
                    filePath = newDir + "/" + _extResourceDictionaryFile;
                    break;
            }

            string uriString = null;            
            switch(Uri_prefix)
            {
                case "SiteOfOrigin":
                    uriString = "pack://siteoforigin:,,/" + filePath;
                    break;

                case "Application":
                    uriString = "pack://application:,,/" + filePath;
                    break;

                // 




                case "Component":
                    string exeFilename = Path.GetFileNameWithoutExtension((new CompilerHelper()).CompiledExecutablePath);
                    uriString = exeFilename + ";component/" + filePath;
                    break;
            }

            string[] XamlPages = new string[1];
            XamlPages[0] = _testXamlFile;

            List<Content> Contents = null;
            List<Resource> Resources = null;

            // Page compiles the Xaml into Baml and embeds into the app.
            // Resource doesn't compile. It keeps the Xaml as it is,
            //  but the Xaml becomes a part of the app (it goes as part of .g.resources 
            //  stream).
            // Content doesn't make the Xaml a part of the app. It keeps it as a separate 
            //  file.
            // EmbeddedResource doesn't work. See 
            switch (Build_action)            
            {
                case "Page":
                    XamlPages = new string[2];
                    XamlPages[0] = _testXamlFile;
                    XamlPages[1] = filePath;
                    break;

                case "Content":
                    Contents = new List<Content>();
                    Contents.Add(new Content(filePath, "Always" /*copyToOutputDirectory*/));
                    break;

                case "Resource":
                    Resources = new List<Resource>();
                    Resources.Add(new Resource(filePath, "embedded", false /*localizable*/));
                    break;

                case "NotApplicable":
                    break;
            }

            CoreLogger.LogStatus("Generating Xaml file. Saving it to " + _testXamlFile);
            GenerateXaml(uriString);

            // Make the Xaml go through either load or compile route, 
            // depending on the value of App_type.
            // In either case, verify the tree has the right content.
            try
            {
                switch (App_type)
                {
                    case "FullTrustApp":
                        // Compile route.                    
                        CoreLogger.LogStatus("Putting the Xaml through XamlCompile route");
                    
                        // Cleanup old compile directories and files if necessary.
                        CompilerHelper compiler = new CompilerHelper();
                        compiler.CleanUpCompilation();
                        compiler.AddDefaults();

                        // Compile xaml into Avalon app and run the app.
                        // This will call the verification routine.
                        compiler.CompileApp(XamlPages, "Application", null, null, null, Languages.CSharp, null, Resources, Contents);

                        // If Uri_prefix is SiteOfOrigin, and Build_action is NotApplicable,
                        // then we have to manually copy the external ResourceDictionary file
                        // to bin/release/ since that's the site-of-origin for the generated
                        // app.
                        if ((Uri_prefix == "SiteOfOrigin") && (Build_action == "NotApplicable"))
                        {
                            string appSiteOfOrigin = Path.GetDirectoryName(compiler.CompiledExecutablePath);
                            switch (Uri_form)
                            {
                                case "NonQualifiedUri":
                                    File.Copy(_extResourceDictionaryFile, appSiteOfOrigin + "\\" + _extResourceDictionaryFile);
                                    break;

                                case "RelativeUri":
                                    Directory.CreateDirectory(appSiteOfOrigin + "\\" + newDir);
                                    File.Copy(newDir + "\\" + _extResourceDictionaryFile,
                                        appSiteOfOrigin + "\\" + newDir + "\\" + _extResourceDictionaryFile);
                                    break;
                            }
                        }

                        compiler.RunCompiledApp();
                        break;

                    case "LooseXaml":
                        // Load route.  
                        // Parse the Xaml, display the tree, and call verification routine.
                        CoreLogger.LogStatus("Putting the Xaml through XamlLoad route");
                        object root = ParserUtil.ParseXamlFile(_testXamlFile);
                        (new SerializationHelper()).DisplayTree(root as UIElement);
                        break;
                }
            }
            finally
            {
                // Move the external resource file back to where it was (if it was
                // moved in the first place).
                if (newDir != null)
                {
                    File.Move(newDir + "\\" + _extResourceDictionaryFile, _extResourceDictionaryFile);
                }
            }
            return true;
        }

        /// <summary>
        /// Generate a Xaml.
        /// 
        /// This method starts with a base Xaml file (that contains the namespace 
        /// declarations, specifies the verifier routine, etc.) and adds on to it.
        /// </summary>
        private void GenerateXaml(string sourcePropertyValue)
        {
            // Read the base Xaml file, and add-on to it.
            XmlDocument doc = new XmlDocument();
            doc.Load(_testSourceablePropertiesBaseXamlFile);

            // Find the element whose Source property is to be set.
            string targetId = "Target0";
            XmlElement sourceablePropertyTarget = doc.SelectSingleNode("//*[@ID='" + targetId + "']") as XmlElement;
            sourceablePropertyTarget.RemoveAttribute("ID");

            // Assign value to the Source property
            sourceablePropertyTarget.SetAttribute("Source", sourcePropertyValue);

            XmlTextWriter writer = new XmlTextWriter((new StreamWriter(_testXamlFile)));
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            doc.Save(writer);
            writer.Close();
        }
        #endregion TestSourceableProperties

        private const string _testXamlFile = "__SourceablePropertyModelTempFile.xaml";
        private const string _testSourceablePropertiesBaseXamlFile = "SourceablePropertyModel_TestSourceableProperties_Base.xaml";
        private const string _extResourceDictionaryFile = "ExternalRD1.xaml";
        private const string _avalonNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private const string _xamlNS = "http://schemas.microsoft.com/winfx/2006/xaml";
    }
}
