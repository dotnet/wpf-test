// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for ChainedTriggers
 * 
 * 
 
  
 * Revision:         $Revision: 14 $
 
********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.Xaml.Markup;


namespace Avalon.Test.CoreUI.PropertyEngine.ChainedTriggers
{
    /// <summary>
    /// PropertyTriggerModel Model class
    /// </summary>      
    [Model(@"FeatureTests\ElementServices\ChainedTriggersModel.xtc", 0, @"PropertyEngine\ChainedTriggers", TestCaseSecurityLevel.FullTrust, "ChainedTriggersModel",
      SupportFiles = @"FeatureTests\ElementServices\ChainedTriggersModel_empty.xaml,FeatureTests\ElementServices\ChainedTriggersModel_elements.xaml,Common\TestRuntime.dll",
        ExpandModelCases = true)
    ]
    public class ChainedTriggersModel : CoreModel
    {
        /// <summary>
        /// Creates a ChainedTriggersModel Model instance
        /// </summary>
        public ChainedTriggersModel()
            : base()
        {
            Name = "ChainedTriggers";
            Description = "ChainedTriggersModel Model";

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
            _modelState = new ChainedTriggersModelState(inParams);

            ChainedTriggersModelState.Persist(_modelState);

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

            ChainedTriggersModelXamlHelper xamlHelper = new ChainedTriggersModelXamlHelper();
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
        ChainedTriggersModelState _modelState = null;

        // Filenames for saving serialized xaml.
        static string s_tempXamlFile = "___SerializationTempFile.xaml";
        static string s_tempXamlFile2 = "___SerializationTempFile2.xaml";
    }

    [Serializable()]
    class ChainedTriggersModelState : CoreState
    {        

        public ChainedTriggersModelState(CoreState coreState)
        {
            Dictionary = (PropertyBag)coreState.Dictionary.Clone();
        }

        public ChainedTriggersModelState(State state)
            : base(state)
        {          
        }

        public void ParseState(PropertyBag state)
        {           
        }

        public override void LogState()
        {

            CoreLogger.LogStatus(
                            "  Kind: " + Kind +
                        "\r\n  Context: " + Context +
                        "\r\n  First " + First +
                        "\r\n  Second: " + Second +
                        "\r\n  Third: " + Third +
                        "\r\n  Fourth: " + Fourth
                           );
        }

        public string Kind
        {
            get { return Dictionary["Kind"]; }
        }
        public string Context
        {
            get { return Dictionary["Context"]; }
        }
        public string First
        {
            get { return Dictionary["First"]; }
        }
        public string Second
        {
            get { return Dictionary["Second"]; }
        }
        public string Third
        {
            get { return Dictionary["Third"]; }
        }
        public string Fourth
        {
            get { return Dictionary["Fourth"]; }
        }
    }
}
