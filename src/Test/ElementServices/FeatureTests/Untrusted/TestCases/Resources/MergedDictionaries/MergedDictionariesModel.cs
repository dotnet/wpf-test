// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for MergedDictionaries
 * 
 * 
 
  
 * Revision:         $Revision: 14 $
 
********************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;

namespace Avalon.Test.CoreUI.Resources.MergedDictionaries
{
    /// <summary>
    /// PropertyTriggerModel Model class
    /// </summary>      
    [Model(@"FeatureTests\ElementServices\MergedDictionariesModel.xtc", 0, @"Resources\MergedDictionaries", TestCaseSecurityLevel.FullTrust, "Merged Dictionaries Model",
    SupportFiles = @"FeatureTests\ElementServices\MergedDictionariesModel_empty.xaml,FeatureTests\ElementServices\MergedDictionariesModel_elements.xaml",
    ExpandModelCases = true)]
    public class MergedDictionariesModel : CoreModel
    {
        /// <summary>
        /// Creates a MergedDictionariesModel Model instance
        /// </summary>
        public MergedDictionariesModel()
            : base()
        {
            Name = "MergedDictionaries Model";
            Description = "MergedDictionariesModel Model";

            //Add Action Handlers
            AddAction("Setup", new ActionHandler(Setup));
        }

        /// <summary>
        /// Single action for this model.  Constructs a tree based on
        /// the parameter combination; loads the tree. the tree is
        /// verified while it's loaded.
        /// </summary>
        /// <remarks>Handler for RunTest</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool Setup(State endState, State inParams, State outParams)
        {
            // Initialize model state instance to be used in helpers
            // and verifiers.
            _modelState = new MergedDictionariesModelState(inParams);

            MergedDictionariesModelState.Persist(_modelState);

            _modelState.LogState();

            // Create the tree with xaml or code and run a dispatcher
            // and display the tree.
            if (_modelState.Kind.Contains("Xaml"))
            {
                _RunXamlBasedTest();
            }
            else
            {
                _RunCodeBasedTest();
            }

            return true;
        }

        // Handles code-based variations.
        private void _RunCodeBasedTest()
        {
            // Code-only variations aren't implemented yet.
            throw new NotSupportedException("Non-Xaml-based tests are not supported yet.");
        }

        // Handles xaml-based variations.
        private void _RunXamlBasedTest()
        {
            // Xaml-based variations always go through round-tripping
            // or compile-and-run.
            // The trees and verification routines should be identical
            // to the code-only variations.

            MergedDictionariesModelXamlHelper xamlHelper = new MergedDictionariesModelXamlHelper();
            Stream stream = xamlHelper.GenerateXaml(_modelState);

            // Save the xml to a temporary file.
            string tempFileName = ".\\__" + Path.ChangeExtension(Path.GetRandomFileName(), ".xaml");
            IOHelper.SaveTextToFile(stream, tempFileName);
            CoreLogger.LogStatus("Saved generated xaml to " + tempFileName);

            string externalRDFile1 = null;
            string externalRDFile2 = null;

            // Create a resource dictionary file if needed.
            if (Convert.ToInt32(_modelState.ExternalDicts) >= 1)
            {
                Stream stream1 = xamlHelper.GenerateExternalXaml(_modelState, 1);
                externalRDFile1 = ".\\" + Path.ChangeExtension("ExternalRD1", ".xaml");
                IOHelper.SaveTextToFile(stream1, externalRDFile1);
                CoreLogger.LogStatus("Saved External RD 1 generated xaml to " + externalRDFile1);
            }
            if (Convert.ToInt32(_modelState.ExternalDicts) == 2)
            {
                Stream stream2 = xamlHelper.GenerateExternalXaml(_modelState, 2);
                externalRDFile2 = ".\\" + Path.ChangeExtension("ExternalRD2", ".xaml");
                IOHelper.SaveTextToFile(stream2, externalRDFile2);
                CoreLogger.LogStatus("Saved External RD 2 generated xaml to " + externalRDFile2);
            }

            // Move the mouse out of the way before running the test so it doesn't trip any events unexpectedly.
            MouseHelper.MoveOnVirtualScreenMonitor();
            DispatcherHelper.DoEvents(2000);

            try
            {
                //NOTE: ROUND TRIPPING DISABLED AS MERGEDDICTIONARIES DO NOT SERIALIZE
                // Go through either round-tripping or compile-and-run.
                if (_modelState.Kind.Contains("Load"))
                {
                    // Parse and display XAML.
                    object root = ParserUtil.ParseXamlFile(tempFileName);
                    (new SerializationHelper()).DisplayTree(root as UIElement);
                }

                else
                {
                    //// Compile route.                    
                    CoreLogger.LogStatus("XamlCompile route disabled, returning");

                    //List<String> assemblyReferences = new List<String>();
                    //assemblyReferences.Add("CoreTestsUntrusted.dll");
                    //assemblyReferences.Add("TestRuntime.dll");

                    //// Compile xaml into Avalon app and run the app.
                    //// This will call the verification routine.
                    //string[] XamlPages = new string[] { tempFileName };

                    //List<Content> Contents = new List<Content>();
                    //if (Convert.ToInt32(_modelState.ExternalDicts) >= 1)
                    //{
                    //    Contents.Add(new Content(externalRDFile1, "Always" /*copyToOutputDirectory*/));
                    //}
                    //if (Convert.ToInt32(_modelState.ExternalDicts) == 2)
                    //{
                    //    Contents.Add(new Content(externalRDFile2, "Always" /*copyToOutputDirectory*/));
                    //}

                    //CompilerHelper compiler = new CompilerHelper();
                    //compiler.AddDefaults();
                    //compiler.CompileApp(XamlPages, "Application", null, null, assemblyReferences, Languages.CSharp, MergedDictionariesModelXamlHelper.AdditionalAppMarkup, null, Contents);
                    //compiler.RunCompiledApp();

                    //// Delete the temp xaml file since the test passed.
                    //File.Delete(tempFileName);
                }
            }
            catch
            {
                // Save the xaml file for future analysis.
                TestLog.Current.LogFile(tempFileName);
                throw;
            }
            finally
            {
                stream.Close();
            }
        }
        /// <summary>
        /// Logs round trip status messages to CoreLogger.
        /// </summary>
        private static void _OnXamlSerialized(object sender, XamlSerializedEventArgs args)
        {
            // Save xaml to file for potential debugging.
            if (File.Exists(s_tempXamlFile))
            {
                File.Copy(s_tempXamlFile, s_tempXamlFile2, true);
            }

            IOHelper.SaveTextToFile(args.Xaml, s_tempXamlFile);
        }

        // Specifies the load type of the current run.
        MergedDictionariesModelState _modelState = null;

        // Filenames for saving serialized xaml.
        static string s_tempXamlFile = "___SerializationTempFile.xaml";
        static string s_tempXamlFile2 = "___SerializationTempFile2.xaml";
    }

    [Serializable()]
    class MergedDictionariesModelState : CoreModelState
    {
        public MergedDictionariesModelState(State state)
        {
            Kind = state["Kind"];
            Lookup = state["Lookup"];
            Key = state["Key"];
            ExternalDicts = state["ExternalDicts"];
            InternalDicts = state["InternalDicts"];
            DuplicateKey = state["DuplicateKey"];
            ExternalKeys = state["ExternalKeys"];
            InternalKeys = state["InternalKeys"];
            ResourceLocation = state["ResourceLocation"];
            ResourceType = state["ResourceType"];
            KeyLookup = state["KeyLookup"];
        }

        public override void LogState()
        {

            CoreLogger.LogStatus(
                            "  Kind: " + Kind +
                        "\r\n  Lookup: " + Lookup +
                        "\r\n  Key: " + Key +
                        "\r\n  ExternalDicts: " + ExternalDicts +
                        "\r\n  InternalDicts: " + InternalDicts +
                        "\r\n  DuplicateKey: " + DuplicateKey +
                        "\r\n  ExternalKeys: " + ExternalKeys +
                        "\r\n  InternalKeys: " + InternalKeys +
                        "\r\n  ResourceLocation: " + ResourceLocation +
                        "\r\n  ResourceType: " + ResourceType +
                        "\r\n  KeyLookup: " + KeyLookup 
                           );
        }

        public string Kind;
        public string Lookup;
        public string Key;
        public string ExternalDicts;
        public string InternalDicts;
        public string DuplicateKey;
        public string ExternalKeys;
        public string InternalKeys;
        public string ResourceLocation;
        public string ResourceType;
        public string KeyLookup;
    }
}
