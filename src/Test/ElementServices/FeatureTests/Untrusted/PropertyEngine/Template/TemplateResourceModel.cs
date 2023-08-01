// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model TemplateResourceModel.
 *          Calls the appropriate helpers to construct trees and
 *          verify them.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 7 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.TemplateResources
{
    /// <summary>
    /// TemplateResourceModel Model class
    /// </summary>  
    [Model(@"FeatureTests\ElementServices\TemplateResourceModel_Pairwise.xtc", 0, @"PropertyEngine\ResourceTemplate", TestCaseSecurityLevel.FullTrust, "TemplateResourceModel",
        ExpandModelCases=true,
        SupportFiles = @"FeatureTests\ElementServices\TemplateResourceModel_empty.xaml, FeatureTests\ElementServices\TemplateResourceModel_elements.xaml")]
    [Model(@"FeatureTests\ElementServices\TemplateResourceModel_Threewise.xtc", 1, @"PropertyEngine\ResourceTemplate", TestCaseSecurityLevel.FullTrust, "TemplateResourceModel",
        ExpandModelCases = true,
        SupportFiles = @"FeatureTests\ElementServices\TemplateResourceModel_empty.xaml, FeatureTests\ElementServices\TemplateResourceModel_elements.xaml")]
    public class TemplateResourceModel : CoreModel
    {
        /// <summary>
        /// Creates a TemplateResourceModel Model instance
        /// </summary>
        public TemplateResourceModel(): base()
        {
            Name = "TemplateResourceModel";
            Description = "TemplateResourceModel Model";
            
            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
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
        private bool RunTest(State endState, State inParams, State outParams)
        {
            // Initialize model state instance to be used in helpers
            // and verifiers.
            _modelState = new TemplateResourceModelState(inParams);

            TemplateResourceModelState.Persist(_modelState);

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

            TemplateResourceModelXamlHelper xamlHelper = new TemplateResourceModelXamlHelper();
            Stream stream = xamlHelper.GenerateXaml(_modelState);

            // Save the xml to a temporary file.
            
            string tempFileName = ".\\__" + Path.ChangeExtension(Path.GetRandomFileName(), ".xaml");
            IOHelper.SaveTextToFile(stream, tempFileName);
            CoreLogger.LogStatus("Saved generated xaml to " + tempFileName);

            // Move the mouse out of the way before running the test so it doesn't trip any events unexpectedly.
            MouseHelper.MoveOnVirtualScreenMonitor();
            DispatcherHelper.DoEvents(2000);            

            try
            {
                // Go through either round-tripping or compile-and-run.
                if (_modelState.Kind.Contains("Load"))
                {
                    // round-tripping.
                    SerializationHelper helper = new SerializationHelper();
                    helper.XamlSerialized += new XamlSerializedEventHandler(_OnXamlSerialized);
                    ParserContext pc = new ParserContext();
                    pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                    if (Environment.OSVersion.Version > new Version("6.0"))
                    {
                        //For Win 7 bypassing RoundTrip tests to avoid some tests timing out when virtual driver is installed on machine
                        //To Do: Once state based logic is implemented for Virtual driver remove this fix
                        CoreLogger.LogStatus("Inside WIn 7 only condition");
                        object firstTreeRoot = XamlReader.Load(stream, pc);
                        helper.DisplayTree(firstTreeRoot);
                        String outputXAML = XamlWriter.Save(firstTreeRoot);
                        object secondTreeRoot = XamlReader.Parse(outputXAML);
                        if (firstTreeRoot.GetType() != secondTreeRoot.GetType())
                        {
                            throw new ArgumentException("Tree root did not match on re-loading");
                        }
                    }
                    else
                    {
                        helper.RoundTripTest(stream, XamlWriterMode.Expression, true /*display*/);
                    }
                }
                else
                {
                    // compile-and-run.
                    // todo: XamlTestRunner runner = new XamlTestRunner();
                    // todo: runner.RunCompileCase(tempFileName, "Application");
                }

                // Delete the temp xaml file since the test passed.
                File.Delete(tempFileName);
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
        TemplateResourceModelState _modelState = null;

        // Filenames for saving serialized xaml.
        static string s_tempXamlFile = "___SerializationTempFile.xaml";
        static string s_tempXamlFile2 = "___SerializationTempFile2.xaml";
    }

    [Serializable()]
    class TemplateResourceModelState : CoreState
    {
        public TemplateResourceModelState(CoreState state)
        {
            this.Dictionary = (PropertyBag)state.Dictionary.Clone();
        }

        public TemplateResourceModelState(State state) : base(state)
        {
        }

        public override void LogState()
        {
            CoreLogger.LogStatus("  Kind: " + Kind +
                           "\r\n  TemplateType: " + TemplateType +
                           "\r\n  TemplatedControlType: " + TemplatedControlType +
                           "\r\n  TemplatePropertyName: " + TemplatePropertyName +
                           "\r\n  HowTemplateIsSet: " + HowTemplateIsSet +
                           "\r\n  TemplateLocation: " + TemplateLocation +
                           "\r\n  HasBrush:          " + HasBrush +
                           "\r\n  HasVisualBrush:    " + HasVisualBrush +
                           "\r\n  HasStoryboard:     " + HasStoryboard +
                           "\r\n  HasStyle:          " + HasStyle +
                           "\r\n  HasViewport3D:     " + HasViewport3D +
                           "\r\n  HasXmlDataSource:  " + HasXmlDataSource + 
                           "\r\n  HasPropertyTrigger:" + HasPropertyTrigger +
                           "\r\n  HasConflictingResourceName:" + HasConflictingResourceName +
                           "\r\n  HasExternalDictionary: " + HasExternalDictionary +
                           "\r\n  HasTemplate: " + HasTemplate +
                           "\r\n  HasStyleBasedOn: " + HasStyleBasedOn +
                           "\r\n  BaseStyleLocation: " + BaseStyleLocation +
                           "\r\n  ContentHasResources: " + ContentHasResources);
        }

        public string Kind { get { return Dictionary["Kind"]; } }
        public string TemplateType { get { return Dictionary["TemplateType"]; } }
        public string TemplatedControlType { get { return Dictionary["TemplatedControlType"]; } }
        public string TemplatePropertyName { get { return Dictionary["TemplatePropertyName"]; } }
        public string HowTemplateIsSet { get { return Dictionary["HowTemplateIsSet"]; } }
        public string TemplateLocation { get { return Dictionary["TemplateLocation"]; } }
        public string HasStoryboard { get { return Dictionary["HasStoryboard"]; } }
        public string HasBrush { get { return Dictionary["HasBrush"]; } }
        public string HasVisualBrush { get { return Dictionary["HasVisualBrush"]; } }
        public string HasStyle { get { return Dictionary["HasStyle"]; } }
        public string HasViewport3D { get { return Dictionary["HasViewport3D"]; } }
        public string HasXmlDataSource { get { return Dictionary["HasXmlDataSource"]; } }
        public string HasPropertyTrigger { get { return Dictionary["HasPropertyTrigger"]; } }
        public string HasConflictingResourceName { get { return Dictionary["HasConflictingResourceName"]; } }
        public string HasExternalDictionary { get { return Dictionary["HasExternalDictionary"]; } }
        public string HasTemplate { get { return Dictionary["HasTemplate"]; } }
        public string HasStyleBasedOn { get { return Dictionary["HasStyleBasedOn"]; } }
        public string BaseStyleLocation { get { return Dictionary["BaseStyleLocation"]; } }
        public string ContentHasResources { get { return Dictionary["ContentHasResources"]; } }
    }
}

