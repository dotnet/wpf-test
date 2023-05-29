// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model StyleModel.
 *
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 7 $
 
********************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Markup;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Xaml.Markup;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.StyleModel
{

    /// <summary>
    /// StyleModel Model class
    /// </summary>  
    [Model(@"FeatureTests\ElementServices\StyleModel_Pairwise.xtc", 0, @"PropertyEngine\StyleModel", TestCaseSecurityLevel.FullTrust, "StyleModel", SupportFiles = @"FeatureTests\ElementServices\StyleModel_empty.xaml,FeatureTests\ElementServices\StyleModel_elements.xaml",ExpandModelCases = true)]
    [Model(@"FeatureTests\ElementServices\StyleModel_Threewise.xtc", 2, @"PropertyEngine\StyleModel", TestCaseSecurityLevel.FullTrust, "StyleModel", SupportFiles = @"FeatureTests\ElementServices\StyleModel_empty.xaml,FeatureTests\ElementServices\StyleModel_elements.xaml", ExpandModelCases = true)]
    public class StyleModel : CoreModel
    {
        /// <summary>
        /// Creates a Style Model instance
        /// </summary>
        public StyleModel(): base()
        {
            Name = "StyleModel";
            Description = "StyleModel Model";
            
            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
        }

        /// <summary>
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
            _modelState = new StyleModelState(inParams);

            StyleModelState.Persist(_modelState);

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
            // Xaml-based variations go through either round-tripping
            // or compile-and-run.
            // The trees and verification routines should be identical
            // to the code-only variations.

            StyleModelXamlHelper xamlHelper = new StyleModelXamlHelper();
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
                    helper.RoundTripTest(stream, XamlWriterMode.Expression, true /*display*/);
                }
                else
                {
                    // compile-and-run.
                    XamlTestRunner runner = new XamlTestRunner();
                    runner.RunCompileCase(tempFileName, "Application");
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
        StyleModelState _modelState = null;

        // Filenames for saving serialized xaml.
        static string s_tempXamlFile = "___SerializationTempFile.xaml";
        static string s_tempXamlFile2 = "___SerializationTempFile2.xaml";
    }

    [Serializable]
    class StyleModelState  : CoreState
    {
        public StyleModelState(CoreState coreState)
        {
            Dictionary = (PropertyBag)coreState.Dictionary.Clone();
        }

        public StyleModelState(State state) : base(state)
        {          
        }

        public void ParseState(PropertyBag state)
        {           
        }

        public override void LogState()
        {
            CoreLogger.LogStatus("  Kind: " + Kind +
               "\r\n  StyleLocation: " + StyleLocation +
               "\r\n  StyledItem: " + StyledItem +
               "\r\n  StyleReference: " + StyleReference +
               "\r\n  StyleHasKey: " + StyleHasKey + 
               "\r\n  StyleHasTargetType: " + StyleHasTargetType +
               "\r\n  BaseStyleLocation: " + BaseStyleLocation +
               "\r\n  BaseHasKey: " + BaseHasKey +
               "\r\n  BaseHasTargetType: " + BaseHasTargetType +
               "\r\n  HasSetter: " + HasSetter +
               "\r\n  FreezableSetter: " + FreezableSetter + 
               "\r\n  HasEventTrigger: " + HasEventTrigger +
               "\r\n  RoutedEvent: " + RoutedEvent + 
               "\r\n  HasPropertyTrigger: " + HasPropertyTrigger +
               "\r\n  FreezableTriggerSetter: " + FreezableTriggerSetter +
               "\r\n  HasTemplate: " + HasTemplate +
               "\r\n  TemplateType: " + TemplateType +
               "\r\n  HasDataTrigger: " + HasDataTrigger +
               "\r\n  FreezableSetterValue: " + FreezableSetterValue +
               "\r\n  FreezableResourceLocation: " + FreezableResourceLocation +
               "\r\n  StyleResources: " + StyleResources);
        }

        public string Kind
        {
            get { return Dictionary["Kind"];  }
        }

        public string StyleLocation
        {
            get { return Dictionary["StyleLocation"]; }
        }

        public string StyledItem
        {
            get { return Dictionary["StyledItem"]; }
        }

        public string StyleReference
        {
            get { return Dictionary["StyleReference"]; }
        }

        public bool StyleHasKey
        {
            get { return Dictionary["StyleHasKey"].ToLower().Equals("true"); }
        }

        public bool StyleHasTargetType
        {
            get { return Dictionary["StyleHasTargetType"].ToLower().Equals("true"); }
        }

        public string BaseStyleLocation
        {
            get { return Dictionary["BaseStyleLocation"]; }
        }

        public bool BaseHasTargetType
        {
            get { return Dictionary["BaseHasTargetType"].ToLower().Equals("true"); }
        }

        public bool BaseHasKey
        {
            get { return Dictionary["BaseHasKey"].ToLower().Equals("true"); }
        }

        public string HasSetter
        {
            get { return Dictionary["HasSetter"]; }
        }

        public bool FreezableSetter
        {
            get { return Dictionary["FreezableSetter"].ToLower().Equals("true"); }
        }

        public string HasEventTrigger
        {
            get { return Dictionary["HasEventTrigger"]; }
        }

        public string RoutedEvent
        {
            get { return Dictionary["RoutedEvent"]; }
        }

        public string HasPropertyTrigger
        {
            get { return Dictionary["HasPropertyTrigger"]; }
        }

        public bool FreezableTriggerSetter
        {
            get { return Dictionary["FreezableTriggerSetter"].ToLower().Equals("true"); }
        }

        public string HasTemplate
        {
            get { return Dictionary["HasTemplate"]; }
        }

        public string TemplateType
        {
            get { return Dictionary["TemplateType"]; }
        }

        public string HasDataTrigger
        {
            get { return Dictionary["HasDataTrigger"]; }
        }

        public string FreezableSetterValue
        {
            get { return Dictionary["FreezableSetterValue"]; }
        }

        public string FreezableResourceLocation
        {
            get { return Dictionary["FreezableResourceLocation"]; }
        }

        public string StyleResources
        {
            get { return Dictionary["StyleResources"]; }
        }

    }
}

