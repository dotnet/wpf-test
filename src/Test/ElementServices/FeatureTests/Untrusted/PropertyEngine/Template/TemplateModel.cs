// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model TemplateModel.
 *          Calls the appropriate helpers to construct trees and
 *          verify them.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
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


namespace Avalon.Test.CoreUI.PropertyEngine.Template
{
    /// <summary>
    /// TemplateModel Model class
    /// </summary>  
    [Model(@"FeatureTests\ElementServices\TemplateModel_Pairwise.xtc", 0, @"PropertyEngine\Template", TestCaseSecurityLevel.FullTrust, "TemplateModel", 
        SupportFiles = @"FeatureTests\ElementServices\TemplateModel_empty.xaml,FeatureTests\ElementServices\TemplateModel_elements.xaml",
        ExpandModelCases=true)]
    [Model(@"FeatureTests\ElementServices\TemplateModel_ThreeTuple.xtc", 2, @"PropertyEngine\Template", TestCaseSecurityLevel.FullTrust, "TemplateModel", 
        SupportFiles = @"FeatureTests\ElementServices\TemplateModel_empty.xaml,FeatureTests\ElementServices\TemplateModel_elements.xaml",
        ExpandModelCases=true)]
    public class TemplateModel : CoreModel
    {
        /// <summary>
        /// Creates a TemplateModel Model instance.
        /// </summary>
        public TemplateModel(): base()
        {
            Name = "TemplateModel";
            Description = "TemplateModel Model";
            
            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
            GlobalLog.LogStatus("*******************************");
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
            _modelState = new TemplateModelState(inParams);

            TemplateModelState.Persist(_modelState);

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

            TemplateModelXamlHelper xamlHelper = new TemplateModelXamlHelper();
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
                    //List<String> assemblyReferences = new List<String>();
                    //assemblyReferences.Add("CoreTestsUntrusted.dll");
                    //assemblyReferences.Add("TestRuntime.dll");

                    //// compile-and-run.
                    //CompilerHelper runner = new CompilerHelper();
                    //runner.CompileApp(tempFileName, "Application", null, null, assemblyReferences);
                    //runner.RunCompiledApp();
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
        TemplateModelState _modelState = null;

        // Filenames for saving serialized xaml.
        static string s_tempXamlFile = "___SerializationTempFile.xaml";
        static string s_tempXamlFile2 = "___SerializationTempFile2.xaml";
    }

    [Serializable()]
    class TemplateModelState : CoreState
    {
        public TemplateModelState(CoreState state)
        {
            this.Dictionary = (PropertyBag)state.Dictionary.Clone();
        }

        public TemplateModelState(State state) : base (state)
        {
            //Kind = state["Kind"];
            //TemplateType = state["TemplateType"];
            //TemplatedControlType = state["TemplatedControlType"];
            //TemplatePropertyName = state["TemplatePropertyName"];
            //TargetTypeSet = state["TargetTypeSet"];
            //TemplateRootType = state["TemplateRootType"];
            //TemplateChildType = state["TemplateChildType"];
            //HasPropertyTrigger = state["HasPropertyTrigger"].ToLower().Equals("true");

            //HasFreezableTriggerSetter = state["HasFreezableTriggerSetter"].ToLower().Equals("true");
            //FreezableSetterValue = state["FreezableSetterValue"];

            //HasEventTrigger = state["HasEventTrigger"];
            //HasTemplateBind = state["HasTemplateBind"].ToLower().Equals("true");
            //DoesReferToStyle = state["DoesReferToStyle"].ToLower().Equals("true");
            //HasEventSet = state["HasEventSet"].ToLower().Equals("true");
            //IsKeySet = state["IsKeySet"].ToLower().Equals("true");
            //HowTemplateIsSet = state["HowTemplateIsSet"];
            //DataSourceType = state["DataSourceType"];
            //HasDataBinding = state["HasDataBinding"].ToLower().Equals("true");
            //TriggerTarget = state["TriggerTarget"];
            //TriggerSource = state["TriggerSource"];
        }

        public override void LogState()
        {

            CoreLogger.LogStatus("  Kind: " + Kind +
                           "\r\n  TemplateType: " + TemplateType +
                           "\r\n  TemplatedControlType: " + TemplatedControlType +
                           "\r\n  TemplatePropertyName: " + TemplatePropertyName +
                           "\r\n  TargetTypeSet: " + TargetTypeSet +
                           "\r\n  TemplateRootType: " + TemplateRootType +
                           "\r\n  TemplateChildType: " + TemplateChildType +
                           "\r\n  HasPropertyTrigger: " + HasPropertyTrigger +

                           "\r\n  HasFreezableTriggerSetter: " + HasFreezableTriggerSetter +
                           "\r\n  FreezableSetterValue: " + FreezableSetterValue +

                           "\r\n  HasEventTrigger: " + HasEventTrigger +
                           "\r\n  HasTemplateBind: " + HasTemplateBind +
                           "\r\n  DoesReferToStyle: " + DoesReferToStyle +
                           "\r\n  HasEventSet: " + HasEventSet +
                           "\r\n  IsKeySet: " + IsKeySet +
                           "\r\n  HowTemplateIsSet: " + HowTemplateIsSet +
                           "\r\n  DataSourceType: " + DataSourceType +
                           "\r\n  HasDataBinding: " + HasDataBinding + 
                           "\r\n  TriggerTarget: " + TriggerTarget + 
                           "\r\n  TriggerSource: " + TriggerSource);
        }

        public string Kind { get { return Dictionary["Kind"]; } }
        public string TemplateType { get { return Dictionary["TemplateType"]; } }
        public string TemplatedControlType { get { return Dictionary["TemplatedControlType"]; } }
        public string TemplatePropertyName { get { return Dictionary["TemplatePropertyName"]; } }
        public string TargetTypeSet { get { return Dictionary["TargetTypeSet"]; } }
        public string TemplateRootType { get { return Dictionary["TemplateRootType"]; } }
        public string TemplateChildType { get { return Dictionary["TemplateChildType"]; } }
        public bool HasPropertyTrigger { get { return Dictionary["HasPropertyTrigger"].ToLower().Equals("true"); } }

        public bool HasFreezableTriggerSetter { get { return Dictionary["HasFreezableTriggerSetter"].ToLower().Equals("true"); } }
        public string FreezableSetterValue { get { return Dictionary["FreezableSetterValue"]; } }

        public string HasEventTrigger { get { return Dictionary["HasEventTrigger"]; } }
        public bool HasTemplateBind { get { return Dictionary["HasTemplateBind"].ToLower().Equals("true"); } }
        public bool DoesReferToStyle { get { return Dictionary["DoesReferToStyle"].ToLower().Equals("true"); } }
        public bool HasEventSet { get { return Dictionary["HasEventSet"].ToLower().Equals("true"); } }
        public bool IsKeySet { get { return Dictionary["IsKeySet"].ToLower().Equals("true"); } }
        public string HowTemplateIsSet { get { return Dictionary["HowTemplateIsSet"]; } }
        public string DataSourceType { get { return Dictionary["DataSourceType"]; } }
        public bool HasDataBinding { get { return Dictionary["HasDataBinding"].ToLower().Equals("true"); } }
        public string TriggerTarget { get { return Dictionary["TriggerTarget"]; } }
        public string TriggerSource { get { return Dictionary["TriggerSource"]; } }
    }
}

