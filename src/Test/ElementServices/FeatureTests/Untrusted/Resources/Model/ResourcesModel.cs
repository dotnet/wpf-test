// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.IO;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
//using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.Xaml.Markup;
#endregion

namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// Resources Resolution Model
    /// </summary>

    // [DISABLE WHILE PORTING]
    // [Model(@"FeatureTests\ElementServices\Resources.xtc", 0, "Resources.Model", TestCaseSecurityLevel.FullTrust, "ResourceModel",
    // SupportFiles = @"FeatureTests\ElementServices\ResourcesModel_empty.xaml,FeatureTests\ElementServices\ResourcesModel_elements.xaml,Common\TestRuntime.dll",
    // ExpandModelCases = true)]
    public class ResourcesModel : CoreModel
    {
        /// <summary>
        /// The ctor is called for each test code
        /// </summary>
        public ResourcesModel()
        {
            Name = "Resources";
            Description = "Resources";

            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
        }

        /// <summary>
        /// Used for UnitTest. It has no effect in official test run
        /// </summary>
        internal void UnitTest()
        {
            //ResourcesModelState unitTestState = new ResourcesModelState();
            //_modelState = unitTestState;
            //ValidateAndAdjustState(ref _modelState);
            //ResourcesModelState.Persist(_modelState);
            //RunTestBasedOnModelState(unitTestState);
            //ResourcesModelVerifiers.StartVerify(null);
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
            _modelState = new ResourcesModelState(inParams);
            ValidateAndAdjustState(ref _modelState);
            ResourcesModelState.Persist(_modelState);

            return RunTestBasedOnModelState(_modelState);
        }

        private bool RunTestBasedOnModelState(ResourcesModelState modelState)
        {
            ResourcesModelState.Persist(modelState);

            modelState.LogState();

            switch (modelState.Kind)
            {
                case "CodeOnly":
                    RunCodeOnlyTest();
                    break;
                case "XamlCompile":
                    RunXamlCompileTest();
                    break;
                case "XamlLoad":
                    RunXamlLoadTest();
                    break;
                default:
                    CoreLogger.LogStatus("Unknown Kind Value of " + modelState.Kind, ConsoleColor.Yellow);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Ideally, Resources Model always produce valid model state for testing. 
        /// This function adds additional check to ensure it.
        /// 
        /// Unfortunately some restrictions involve more than 2 parameters. It can only be adjusted 
        /// with code logic. This function doubles up as adjustment. 
        /// 
        /// In the code, all Assert() are for the validation purpose. And if the code changes value for 
        /// modelState, it is for adjustment. 
        /// </summary>
        /// <param name="modelState"></param>
        private void ValidateAndAdjustState(ref ResourcesModelState modelState)
        {
            if (modelState.Kind != "CodeOnly")
            {
                CoreLogger.LogStatus("Currently we do not support CodeOnly", ConsoleColor.Yellow);
            }

            if (modelState.ResourceDefinitionLocation == "ApplicationRD")
            {
                if (modelState.Kind != "XamlLoad")
                {
                    CoreLogger.LogStatus("ApplicationRD not working with XamlLoad", ConsoleColor.Yellow);
                }
                if (modelState.ApplicationResourcePreferred == "true")
                {
                    CoreLogger.LogStatus("ApplicationResourcePreferred must be true for ApplicationRD", ConsoleColor.Yellow);
                }
                if (modelState.SameRDPreferred == "true") 
                {
                    CoreLogger.LogStatus("SameRDPreferred must be true for ApplicationRD", ConsoleColor.Yellow);
                }
            }

            _sameRDPreferred = modelState.SameRDPreferred;

            if (modelState.ResolutionSteps >= 0 && modelState.ResolutionSteps <= 3)
            {
                CoreLogger.LogStatus("ResolutionSteps is currently between 1 and 3", ConsoleColor.Yellow);
            }

            if (modelState.ResolutionSteps > 1 && modelState.SameRDPreferred == "false")
            {
                if (modelState.ResourceDefinitionLocation == "ApplicationRD")
                {
                    CoreLogger.LogStatus("Has to adjust SameRDPreferred");
                    _sameRDPreferred = "true";
                }
            }
            if (modelState.ResolutionSteps > 2 && modelState.SameRDPreferred == "false")
            {
                if (modelState.ResourceDefinitionLocation == "RootRD")
                {
                    CoreLogger.LogStatus("Has to adjust SameRDPreferred");
                    _sameRDPreferred = "true";
                }
            }
            //If the model is extended to have Resolution steps larger than 3, additional 
            //adjustment may become necessary.

        }

        private void RunCodeOnlyTest()
        {
        }

        private void RunXamlCompileTest()
        {
            GenerateXaml();

            try
            {               

                XamlTestRunner runner = new XamlTestRunner();
                runner.RunCompileCase(_tempFileName, "Application", ResourcesModelXamlHelper.AdditionalAppMarkup);


                // Delete the temp xaml file since the test passed.
                File.Delete(_tempFileName);
            }
            catch
            {
                // Save the xaml file for future analysis.
                TestLog.Current.LogFile(_tempFileName);
                throw;
            }
            finally
            {
                _stream.Close();
            }
        }

        private void RunXamlLoadTest()
        {
            if (_modelState.ApplicationResourcePreferred != "true")
            {
                CoreLogger.LogStatus("Cannot run LoadTest while ApplicationResourcePreferred", ConsoleColor.Yellow);
            }
            
            GenerateXaml();
            MouseHelper.MoveOnVirtualScreenMonitor();
            DispatcherHelper.DoEvents(2000);
            // round-tripping.            
            try
            {                
                SerializationHelper helper = new SerializationHelper();
                helper.XamlSerialized += new XamlSerializedEventHandler(_OnXamlSerialized);
                ParserContext pc = new ParserContext();
                pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
                if (Environment.OSVersion.Version > new Version("6.0"))
                {
                    //For Win 7 bypassing RoundTrip tests to avoid some tests timing out when virtual driver is installed on machine
                    //To Do: Once state based logic is implemented for Virtual driver remove this fix
                    CoreLogger.LogStatus("Inside WIn 7 only condition");
                    object firstTreeRoot = XamlReader.Load(_stream, pc);
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
                    helper.RoundTripTest(_stream, XamlWriterMode.Expression, true /*display*/);
                }

                // Delete the temp xaml file since the test passed.
                File.Delete(_tempFileName);

            }
            catch
            {
                // Save the xaml file for future analysis.
                TestLog.Current.LogFile(_tempFileName);
                throw;
            }
            finally
            {
                _stream.Close();
            }

        }

        private void GenerateXaml()
        {
            _stream = ResourcesModelXamlHelper.GenerateXaml(_modelState);

            _tempFileName = ".\\__" + Path.ChangeExtension(Path.GetRandomFileName(), ".xaml");
            IOHelper.SaveTextToFile(_stream, _tempFileName);
            CoreLogger.LogStatus("Saved generated xaml to " + _tempFileName);
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

        private ResourcesModelState _modelState = null;
        private string _tempFileName;
        private Stream _stream;
        private string _sameRDPreferred;

        // Filenames for saving serialized xaml.
        static string s_tempXamlFile = "___ResourcesTempFile.xaml";
        static string s_tempXamlFile2 = "___ResourcesTempFile2.xaml";

    }
}
