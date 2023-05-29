// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Inheritance Behavior Test Suite
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

namespace Avalon.Test.CoreUI.PropertyEngine.InheritanceBehaviorModel
{
    /// <summary>
    /// Tests for InheritanceBehavior property on FrameworkElement.
    /// </summary>    
    [Model(@"FeatureTests\ElementServices\InheritanceBehavior.xtc", 0, @"PropertyEngine\InheritanceBehavior", TestCaseSecurityLevel.FullTrust, "InheritanceBehaviorModel",        
        ExpandModelCases = true)]
    public class InheritanceBehaviorModel : CoreModel
    {
        /// <summary>
        /// Creates a PropertyChangeTest Model instance.
        /// </summary>
        public InheritanceBehaviorModel()
            : base()
        {
            Name = "InheritanceBehavior";
            Description = "Model InheritanceBehavior";
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
            _modelState = new InheritanceBehaviorModelState(inParams);

            InheritanceBehaviorModelState.Persist(_modelState);

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

            //
            // Step 1: Create the Tree.
            //
                                   
            // Adding tree root.
            StackPanel root = new StackPanel();

            // Adding CustomFE Parent.
            StackPanel grandParentSP = new StackPanel();
            Grid grandParentG = new Grid();
            Border grandParentTB = new Border();
            
            // Creating the Custom Framework Element.
            CustomFE testFE = new CustomFE(_modelState.InheritanceBehavior);

            // Add Child
            Button childButton = new Button();

            // Property Lookup - Simple Tree.
            if (_modelState.LookupType == "PropertyValueLookup")
            {
                // Set the Value to be inherited.
                testFE.Test = "Parent Value";
            }

            // Resource Lookup - add Resources to appropriate Element in tree.
            else if (_modelState.LookupType == "ResourceLookup")
            {
                // We will add the resource to the
                // i) Parent CustomFrameworkElement
                ResourceDictionary rdTestFE;
                rdTestFE = new ResourceDictionary();
                rdTestFE.Add("TestResource", "CustomFrameworkElement Resource");
                testFE.Resources = rdTestFE;

                // ii) the GrandParent
                ResourceDictionary rdGrandParent;
                rdGrandParent = new ResourceDictionary();
                rdGrandParent.Add("TestResource", "GrandParent Resource");
                grandParentSP.Resources = rdGrandParent;
                grandParentG.Resources = rdGrandParent;
                grandParentTB.Resources = rdGrandParent;

                // iii) the Application resources 
                ResourceDictionary rdApp;
                rdApp = new ResourceDictionary();
                rdApp.Add("TestResource", "Application Resource");
                app.Resources = rdApp;

                // Set the Resource Reference on the Child element.
                childButton.SetResourceReference(CustomFE.TestProperty, "TestResource");
            }
            
            //
            // Link the tree together.
            //            
            root.Children.Add(grandParentSP);
            root.Children.Add(grandParentG);
            root.Children.Add(grandParentTB);

            if (_modelState.ParentElement == "StackPanel")
            {
                grandParentSP.Children.Add(testFE);
            }
            else if (_modelState.ParentElement == "Grid")
            {
                grandParentG.Children.Add(testFE);
            }
            else if (_modelState.ParentElement == "FlowDocument")
            {
                grandParentTB.Child = testFE;
            }

            // Adding Visual Parent if verifying multiple inheritance - This should not 
            if (_modelState.LookupType == "MultipleInheritance")
            {
                Border visualParent = new Border();

            }

            testFE.Children.Add(childButton);

            // If InheritanceBehavior was overriden than lookup should work as normal.
            if (_modelState.OverridesInheritanceBehavior == "True")
            {                
                if (_modelState.LookupType == "ResourceLookup")
                {
                    // Set the Resource Reference on the Child element.
                    childButton.SetResourceReference(CustomFE.Test2Property, "TestResource");
                }
                else
                {
                    testFE.Test2 = "CustomFE with overridden inheritancebehavior";
                }                

                // Verify that the lookup was correct.
                string nonOverridenValue = (string)childButton.GetValue(CustomFE.Test2Property);                
                CoreLogger.LogStatus("nonOverriden Value: " + nonOverridenValue, ConsoleColor.Green);

                if (nonOverridenValue == "Default Value")
                {
                   throw new Microsoft.Test.TestValidationException("Overrides Inheritance Behaviour not working correclty - failed to pick up resource/propertyvalue from parent");
                }
            }

            //
            // Step 2: Verify that the lookup was correct.
            //
            
            string actualValue = (string)childButton.GetValue(CustomFE.TestProperty);            
            
            CoreLogger.LogStatus("Actual Value: " + actualValue, ConsoleColor.Green);
            //CoreLogger.LogStatus("Excpected Value: " + actualValue, ConsoleColor.Green);
            if (_modelState.LookupType == "ResourceLookup")
            {
                if (_modelState.InheritanceBehavior == "Default")
                {
                    if (actualValue != "CustomFrameworkElement Resource")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected CustomFrameworkElement Resource : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipToAppNow")
                {
                    if (actualValue != "Application Resource")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected Application Resource : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipToAppNext")
                {
                    if (actualValue != "CustomFrameworkElement Resource")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected CustomFrameworkElement Resource : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipToThemeNow")
                {
                    if (actualValue != "Default Value")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected Default Value : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipToThemeNext")
                {
                    if (actualValue != "CustomFrameworkElement Resource")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected CustomFrameworkElement Resource : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipAllNow")
                {
                    if (actualValue != "Default Value")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected Default Value : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipAllNext")
                {
                    if (actualValue != "CustomFrameworkElement Resource")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected CustomFrameworkElement Resource : Got - " + actualValue);
                    }
                }

            }

            // In the case of property lookup
            else
            {
                if (_modelState.InheritanceBehavior == "Default" ||
                    _modelState.InheritanceBehavior == "SkipToAppNext" ||
                    _modelState.InheritanceBehavior == "SkipToThemeNext" ||
                    _modelState.InheritanceBehavior == "SkipAllNext" )
                {
                    if (actualValue != "Parent Value")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected Parent Value : Got - " + actualValue);
                    }
                }
                else if (_modelState.InheritanceBehavior == "SkipToAppNow" ||
                        _modelState.InheritanceBehavior == "SkipToThemeNow" ||
                        _modelState.InheritanceBehavior == "SkipAllNow")
                {
                    if (actualValue != "Default Value")
                    {
                        throw new Microsoft.Test.TestValidationException("Inheritance Behaviour not working correclty - Expected Default Value : Got - " + actualValue);
                    }
                }                
            }

            CoreLogger.LogStatus("Exiting Application", ConsoleColor.Cyan);

            // Shutdown App.
            AppShutDown(app);
        }       

        // Specifies the load type of the current run.
        InheritanceBehaviorModelState _modelState = null;
       
    }

    /// <summary>
    /// A custom FrameworkElement where the inheitanceBehavior can be modified and set as desired.
    /// </summary>
    public class CustomFE : FrameworkElement
    {
        /// <summary>
        /// Constructor for custom FrameworkElement. We can set the inheritanceBehavior property from here.
        /// </summary>
        public CustomFE (string iBehavior)
        {
            // Using model input to determine the inheritancebehavior property.
            if (iBehavior == "Default")       
            {
                this.InheritanceBehavior = InheritanceBehavior.Default;
            }
            else if (iBehavior == "SkipToAppNow")
            {
                this.InheritanceBehavior = InheritanceBehavior.SkipToAppNow;
            }
            else if (iBehavior == "SkipToAppNext")
            {
                this.InheritanceBehavior = InheritanceBehavior.SkipToAppNext;
            }
            else if (iBehavior == "SkipToThemeNow")
            {
                this.InheritanceBehavior = InheritanceBehavior.SkipToThemeNow;
            }
            else if (iBehavior == "SkipToThemeNext")
            {
                this.InheritanceBehavior = InheritanceBehavior.SkipToThemeNext;
            }
            else if (iBehavior == "SkipAllNow")
            {
                this.InheritanceBehavior = InheritanceBehavior.SkipAllNow;
            }
            else if (iBehavior == "SkipAllNext")
            {
                this.InheritanceBehavior = InheritanceBehavior.SkipAllNext;
            }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                if (_Children == null)
                {
                    _Children = new UIElementCollection(this, this);
                }
                
                return _Children;
            }
        }

        private UIElementCollection _Children = null;

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // otherwise, its logical children is its visual children                
                return this.Children.GetEnumerator();
            }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public static readonly DependencyProperty TestProperty
            = DependencyProperty.RegisterAttached(
                "Test",                  // Property name
                typeof(string),            // Property type
                typeof(CustomFE),
                new FrameworkPropertyMetadata("Default Value", FrameworkPropertyMetadataOptions.Inherits)
            );

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public static readonly DependencyProperty Test2Property
            = DependencyProperty.RegisterAttached(
                "Test2",                  // Property name
                typeof(string),            // Property type
                typeof(CustomFE),
                new FrameworkPropertyMetadata("Default Value2", FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior)
            );

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public string Test
        {
            get { return (string)GetValue(TestProperty); }
            set { SetValue(TestProperty, value); }
        }

        /// <summary>
        /// The Stretch property determines how the shape may be stretched to accommodate shape size
        /// </summary>
        public string Test2
        {
            get { return (string)GetValue(Test2Property); }
            set { SetValue(Test2Property, value); }
        }
    }




    [Serializable()]
    class InheritanceBehaviorModelState : CoreModelState
    {
        public InheritanceBehaviorModelState(State state)
        {
            InheritanceBehavior = state["InheritanceBehavior"];
            ParentElement = state["ParentElement"];
            MultipleInheritance = state["MultipleInheritance"];
            OverridesInheritanceBehavior = state["OverridesInheritanceBehavior"];
            LookupType = state["LookupType"];            
        }

        public override void LogState()
        {

            CoreLogger.LogStatus("  InheritanceBehavior: " + InheritanceBehavior +
                           "\r\n  ParentElement: " + ParentElement +
                           "\r\n  MultipleInheritance: " + MultipleInheritance +
                           "\r\n  OverridesInheritanceBehavior: " + OverridesInheritanceBehavior +
                           "\r\n  LookupType: " + LookupType );
        }

        public string InheritanceBehavior;
        public string ParentElement;        
        public string MultipleInheritance;
        public string OverridesInheritanceBehavior;
        public string LookupType;        
    }
}

