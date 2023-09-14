// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the disjoint tree stateless MDE model.
 *  
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 14 $
 
********************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
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

namespace Avalon.Test.CoreUI.PropertyEngine.DisjointTree
{
    /// <summary>
    /// DisjointTreeModel Model class
    /// </summary>  
    [Model(@"FeatureTests\ElementServices\DisjointTree_Xaml.xtc", 0, @"PropertyEngine\DisjointTree\Model", TestCaseSecurityLevel.FullTrust, "DisjointTree full combo",
        SupportFiles = @"FeatureTests\ElementServices\DisjointTree_Empty.xaml,FeatureTests\ElementServices\DisjointTree_Elements.xaml,FeatureTests\ElementServices\DisjointTree_FailureOverride.xml",
        ExpandModelCases = true)]
    public class DisjointTreeModel : CoreModel
    {
        /// <summary>
        /// Creates a TemplateModel Model instance
        /// </summary>
        public DisjointTreeModel(): base()
        {
            Name = "DisjointTree";
            Description = "DisjointTreeModel Model";
            
            //Add Action Handlers
            AddAction("Go", new ActionHandler(Go));
        }

        /// <summary>
        /// Single action for this model.  Constructs a tree based on
        /// the parameter combination; loads the tree. the tree is
        /// verified after it's loaded.
        /// </summary>
        private bool Go(State endState, State inParams, State outParams)
        {
            // Initialize model state instance to be used in helpers and verifiers.
            _modelState = new DisjointTreeModelState(inParams);
            DisjointTreeModelState.Persist(_modelState);
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

        /// <summary>
        /// Handle code-based variations.
        /// </summary>
        private void _RunCodeBasedTest()
        {
            // Build and display the tree.
            DisjointTreeTreeHelper treeHelper = new DisjointTreeTreeHelper(_modelState);
            Window win = treeHelper.BuildTree();

            // Run verification.            
            DisjointTreeVerification.Verify(win);
        }

        /// <summary>
        /// Handles xaml-based variations. 
        /// </summary>
        private void _RunXamlBasedTest()
        {
            // Xaml-based variations always go through compile-and-run.
            // The trees and verification routines should be identical
            // to the code-only variations.

            DisjointTreeXamlHelper treeHelper = new DisjointTreeXamlHelper(_modelState);
            Stream stream = treeHelper.GenerateXaml();

            // Save the xml to a temporary file.

            string tempFileName = ".\\__" + Path.ChangeExtension(Path.GetRandomFileName(), ".xaml");
            IOHelper.SaveTextToFile(stream, tempFileName);
            CoreLogger.LogStatus("Saved generated xaml to " + tempFileName);

            // Move the mouse out of the way before running the test so it doesn't trip any events unexpectedly.
            MouseHelper.MoveOnVirtualScreenMonitor();
            DispatcherHelper.DoEvents(2000);

            try
            {
                // compile-and-run, verification will be called from xaml property on custom page.
                List<String> assemblyReferences = new List<String>();
                assemblyReferences.Add("CoreTestsUntrusted.dll");
                assemblyReferences.Add("TestRuntime.dll");

                // compile-and-run.
                CompilerHelper runner = new CompilerHelper();
                runner.AddDefaults();
                runner.CompileApp(tempFileName, "Application", null, null, assemblyReferences);
                runner.RunCompiledApp();

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

        DisjointTreeModelState _modelState = null;
    }

    [Serializable()]
    class DisjointTreeModelState : CoreModelState
    {
        public DisjointTreeModelState(State state)
        {
            Kind = state["Kind"];
            MentorItem = state["MentorItem"];
            Link = state["Link"];
            HowLinked = state["HowLinked"];
            Service = state["Service"];
            ContextOrientation = state["ContextOrientation"];

            this.Dictionary.Add("Kind", state["Kind"]);
            this.Dictionary.Add("MentorItem", state["MentorItem"]);
            this.Dictionary.Add("Link", state["Link"]);
            this.Dictionary.Add("HowLinked", state["HowLinked"]);
            this.Dictionary.Add("Service", state["Service"]);
            this.Dictionary.Add("ContextOrientation", state["ContextOrientation"]);
        }

        public override void LogState()
        {
            CoreLogger.LogStatus(
                "\n  Kind: " + Kind +
                "\n  MentorItem: " + MentorItem +
                "\n  Link: " + Link +
                "\n  HowLinked: " + HowLinked +
                "\n  Service: " + Service +
                "\n  ContextOrientation: " + ContextOrientation);
        }

        public string Kind;
        public string MentorItem;
        public string Link;
        public string HowLinked;
        public string Service;
        public string ContextOrientation;


        // These names are used in the test tree generation and verification 
        // routines so this is the best place for them.
        public const string DynamicResourceKey = "testBrush";
        
        public const string TestRootName = "TestRoot";

        public const string FirstParentName = "firstParent";
        public const string SecondParentName = "secondParent";

        public const string FirstMentorName = "firstMentor";
        public const string SecondMentorName = "secondMentor";

        public const string FirstMenteeName = "firstMentee";
        public const string SecondMenteeName = "secondMentee";
    }

}

