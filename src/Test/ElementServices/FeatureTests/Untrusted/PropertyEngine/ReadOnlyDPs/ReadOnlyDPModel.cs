// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: ReadOnly DP / Inheritance Behavior Test Suite
 * 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;

using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Trusted.Utilities;
#endregion

namespace Avalon.Test.CoreUI.PropertyEngine.ReadOnlyDPModel
{
    /// <summary>
    /// Tests for InheritanceBehavior property on FrameworkElement.
    /// </summary>
    [Model(@"FeatureTests\ElementServices\ReadOnlyDPs.xtc", 0, @"PropertyEngine\ReadOnlyDPs", TestCaseSecurityLevel.FullTrust, "ReadOnlyDP Verification.",
        ExpandModelCases = true)]
    public class ReadOnlyDPModel : CoreModel
    {
        /// <summary>
        /// Creates a ReadOnlyDP Model instance.
        /// </summary>
        public ReadOnlyDPModel()
            : base()
        {
            Name = "ReadOnlyDP";
            Description = "Model ReadOnlyDP";
            ModelPath = "MODEL_PATH_TOKEN";

            // Add Action Handlers.
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
            _modelState = new ReadOnlyDPModelState(inParams);

            ReadOnlyDPModelState.Persist(_modelState);

            _modelState.LogState();

            // Create new Application            
            Application app = new Application();
            
            // Launch the Application.            
            app.Startup += new StartupEventHandler(app_Startup);
            app.Run();            
            
            return true;
        }

        /// <summary>
        /// ShutDown the App.
        /// </summary>
        public void AppShutDown(Application app)
        {
            app.Shutdown();
        }
        
        // Handles code-based variations.
        private void app_Startup(object sender, StartupEventArgs e)
        {
            CoreLogger.LogStatus("Entering Application...", ConsoleColor.Cyan);

            Application app;
            app = (Application)sender;

            // Create an instance of the Custom Franework Elment and select the appropriate DP.
            _cFE = new CustomFE(_modelState.Action);

            string DPName = _modelState.RegistrationType + "Property";
            CoreLogger.LogStatus("Selected DP Name: " + DPName);

            FieldInfo fi = typeof(CustomFE).GetField(DPName, BindingFlags.Public | BindingFlags.Static);
            _DP_to_use = (DependencyProperty)fi.GetValue(_cFE);

            string defaultValue = "";

            if (_modelState.RegistrationType == "Regular")
            {
                defaultValue = "Default Value";
            }
            else if (_modelState.RegistrationType == "Attached")
            {
                defaultValue = "Default Value2";
            }

            // Create an instance of the second framework element that inherits.
            _inheritsFE = new InheritsCustomFE(_modelState.Action);

            //
            // Step 1: Create the Tree.
            //
            StackPanel parent = new StackPanel();
            //Button visualParent = new Button();
            //Context Menu = new ContextMenu();
            //Rectangle rect = new Rectangle();
            Decorator dec = new Decorator();
            //Brush rectFill = Brushes.Green;
            
            // Constructing and linking up a LOGICAL Tree.
            if (_modelState.ElementTree == "Logical")
            {
                parent.Children.Add(_cFE);
                _cFE.Children.Add(_inheritsFE);
            }

            // Constructing and linking up a VISUAL Tree.
            else if (_modelState.ElementTree == "Visual")
            {
                parent.Children.Add(_cFE);
                _cFE.Children.Add(dec);
                //visualParent.Background = rectFill;
                dec.Child = _inheritsFE;
            }
            

            //
            // Step 2: Verify that the value lookup is correct / inheritance works as expected.
            //            
            
            string cFEValue = (string)_cFE.GetValue(_DP_to_use);
            CoreLogger.LogStatus("Getting value worked (original FE): " + cFEValue);

            if (cFEValue != defaultValue)
            {
                throw new Microsoft.Test.TestValidationException("Correct Value not looked up for original FE");
            }


            string inheritsFEValue = (string)_inheritsFE.GetValue(_DP_to_use);
            CoreLogger.LogStatus("Getting value worked: " + inheritsFEValue);

            if (inheritsFEValue != defaultValue)
            {
                throw new Microsoft.Test.TestValidationException("Correct Value not looked up for inherited DP in FE");
            }


            //
            // Step 3: Verify that modifying / overriding property value does not work.
            //
            
            // Set the value.
            string cFESetEx = "";

            CoreLogger.LogStatus("Attempting to set value of DP on original FE", ConsoleColor.Green);
            try
            {
                _cFE.SetValue(_DP_to_use, "BOO");
            }
            catch (InvalidOperationException ex)
            {
                cFESetEx = ex.Message;
            }

            if (cFESetEx == "" || cFESetEx == null)
            {
                throw new Microsoft.Test.TestValidationException("No Exception thrown when attempting to set value of readonly DP (original FE)");
            }
            else
            {
                CoreLogger.LogStatus("Got Expected Exception: " + cFESetEx, ConsoleColor.Green);
            }


            string inheritsFESetEx = "";

            CoreLogger.LogStatus("Attempting to set value of DP on FE that inherits", ConsoleColor.Green);
            try
            {
                _inheritsFE.SetValue(_DP_to_use, "BOO");
            }
            catch (InvalidOperationException ex)
            {
                inheritsFESetEx = ex.Message;
            }

            if (inheritsFESetEx == "" || inheritsFESetEx == null)
            {
                throw new Microsoft.Test.TestValidationException("No Exception thrown when attempting to set value of readonly DP");
            }
            else
            {
                CoreLogger.LogStatus("Got Expected Exception: " + inheritsFESetEx, ConsoleColor.Green);
            }            


            // Clear the value.
            string cFEClearEx = "";

            CoreLogger.LogStatus("Attempting to clear value of DP on origianl FE", ConsoleColor.Green);
            try
            {
                _cFE.ClearValue(_DP_to_use);
            }
            catch (InvalidOperationException ex)
            {
                cFEClearEx = ex.Message;
            }

            if (cFEClearEx == "" || cFEClearEx == null)
            {
                throw new Microsoft.Test.TestValidationException("No Exception thrown when attempting to clear value of readonly DP (original FE)");
            }
            else
            {
                CoreLogger.LogStatus("Got Expected Exception: " + cFEClearEx, ConsoleColor.Green);
            } 


            string inheritsFEClearEx = "";

            CoreLogger.LogStatus("Attempting to clear value of DP on FE that inherits", ConsoleColor.Green);
            try
            {
                _inheritsFE.ClearValue(_DP_to_use);
            }
            catch (InvalidOperationException ex)
            {
                inheritsFEClearEx = ex.Message;
            }

            if (inheritsFEClearEx == "" || inheritsFEClearEx == null)
            {
                throw new Microsoft.Test.TestValidationException("No Exception thrown when attempting to clear value of readonly DP");
            }
            else
            {
                CoreLogger.LogStatus("Got Expected Exception: " + inheritsFEClearEx, ConsoleColor.Green);
            }

            
            CoreLogger.LogStatus("Exiting Application", ConsoleColor.Cyan);

            // Shutdown App.
            AppShutDown(app);
        }       

        // Specifies the load type of the current run.
        ReadOnlyDPModelState _modelState = null;
        private DependencyProperty _DP_to_use = null;
        private CustomFE _cFE = null;
        private InheritsCustomFE _inheritsFE = null;

    }



    #region Model Handlers
    [Serializable()]
    class ReadOnlyDPModelState : CoreModelState
    {
        public ReadOnlyDPModelState(State state)
        {
            RegistrationType = state["RegistrationType"];
            InheritanceContext = state["InheritanceContext"];
            InheritanceBehavior = state["InheritanceBehavior"];
            ElementTree = state["ElementTree"];
            Action = state["Action"];            
        }

        public override void LogState()
        {

            CoreLogger.LogStatus("  RegistrationType: " + RegistrationType +
                           "\r\n  InheritanceContext: " + InheritanceContext +
                           "\r\n  InheritanceBehavior: " + InheritanceBehavior +
                           "\r\n  ElementTree: " + ElementTree +
                           "\r\n  Action: " + Action);
        }

        public string RegistrationType;
        public string InheritanceContext;
        public string InheritanceBehavior;
        public string ElementTree;
        public string Action;
    }
    #endregion
}

