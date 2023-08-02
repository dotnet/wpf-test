// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives
using System;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI.UtilityHelper;
using Avalon.Test.CoreUI.PropertyEngine;
using Microsoft.Test.Modeling;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;
#endregion

namespace Avalon.Test.CoreUI.PropertyEngine.Precedence
{
    /// <summary>
    /// This class is used for unit tests. It has no effect in official test run.
    /// Use the command below to run this test using CoreTests.exe: 
    /// (The first and second line are for P0 and P2 respectively.)
    /// CoreTests.exe /Area="PropertyEngine\Precedence"
    /// CoreTests.exe /Area="PropertyEngine\StylePrecedence"
    /// CoreTests.exe /Area="PropertyEngine\P2Precedence"
    /// </summary>
    public class PETestCase
    {
        /// <summary>
        /// Entry point for unit tests
        /// </summary>
        public void LabRunAll()
        {
            PrecedenceModel test = new PrecedenceModel();
            Utilities.StartRunAllTests("PrecedenceModel");
            test.UnitTest();
            Utilities.StopRunAllTests();
        }
    }

    /// <summary>
    /// Test code section for model-based DependencyProperty Precedence test
    /// The xtc file, as well as the MDE project file, is checked into testdata depot
    /// </summary>
    [Model(@"FeatureTests\ElementServices\DependencyPropertyPrecedence.xtc", 0, @"PropertyEngine\Precedence", TestCaseSecurityLevel.FullTrust, "PrecedenceModel", ExpandModelCases = true)]
    [Model(@"FeatureTests\ElementServices\DependencyPropertyPrecedenceP2.xtc", 2, @"PropertyEngine\P2Precedence", TestCaseSecurityLevel.FullTrust, "PrecedenceModel", ExpandModelCases = true)]
    [Model(@"FeatureTests\ElementServices\StylePropertyPrecedence.xtc", 0, @"PropertyEngine\StylePrecedence", TestCaseSecurityLevel.FullTrust, "PrecedenceModel", ExpandModelCases = true)]
    public class PrecedenceModel : CoreModel
    {
        /// <summary>
        /// The constructor is called for each test case
        /// </summary>
        public PrecedenceModel()
            : base()
        {
            Name = "DependencyPropertyPrecedence";
            Description = "Dependency Property Precedence Model";

            //Add Action Handlers
            AddAction("SetStyleTestMode", new ActionHandler(SetStyleTestMode));
            AddAction("AssignElementTemplateTrigger", new ActionHandler(AssignElementTemplateTrigger));
            AddAction("AssignInheritance", new ActionHandler(AssignInheritance));
            AddAction("AssignLocalValue", new ActionHandler(AssignLocalValue));
            AddAction("AssignStyleSetter", new ActionHandler(AssignStyleSetter));
            AddAction("AssignStyleTemplateTrigger", new ActionHandler(AssignStyleTemplateTrigger));
            AddAction("AssignStyleTrigger", new ActionHandler(AssignStyleTrigger));
            AddAction("AssignTemplatedParentLocallySetValue", new ActionHandler(AssignTemplatedParentLocallySetValue));
            AddAction("AssignTemplatedParentTemplateTrigger", new ActionHandler(AssignTemplatedParentTemplateTrigger));
            AddAction("AssignImplicitReference", new ActionHandler(AssignImplicitReference));

            //DefaultValue is always assigned
            SetPrecedenceFactorsFlag(PrecedenceFactors.DefaultValue);
            PrintActionInfo("EntryPoint");
            SetTestElement(false, true);
        }

        /// <summary>
        /// Used for UnitTest. It has no effect in official test run
        /// </summary>
        internal void UnitTest()
        {
            SetStyleTestMode(null, null, null);

                //AssignElementTemplateTrigger(null, null, null);
                //AssignInheritance(null, null, null);
                //AssignLocalValue(null, null, null);
                //AssignStyleSetter(null, null, null);
                //AssignStyleTemplateTrigger(null, null, null);
                //AssignStyleTrigger(null, null, null);
                
            AssignImplicitReference(null, null, null);
            AssignTemplatedParentLocallySetValue(null, null, null);
                AssignTemplatedParentTemplateTrigger(null, null, null);
                AssignLocalValue(null, null, null);
        }

        #region Action Handlers
        private bool SetStyleTestMode(State endState, State inParams, State outParams)
        {
            _isStylePropertyTest = true;
            PrintActionInfo("Set Style Test Mode");
            SetTestElement(false, true);
            return true;
        }

        private bool AssignElementTemplateTrigger(State endState, State inParams, State outParams)
        {
            System.Diagnostics.Debug.Assert(!_isStylePropertyTest, "Should not get called in StyleProperty Test");
            SetPrecedenceFactorsFlag(PrecedenceFactors.ElementTemplateTrigger);
            PrintActionInfo("AssignElementTemplateTrigger");
            SetElementTemplate(true);
            return true;
        }

        private bool AssignInheritance(State endState, State inParams, State outParams)
        {
            System.Diagnostics.Debug.Assert(!_isStylePropertyTest, "Should not get called in StyleProperty Test");
            SetPrecedenceFactorsFlag(PrecedenceFactors.Inheritance);
            PrintActionInfo("AssignInheritance");
            SetInheritance(true);
            return true;
        }

        private bool AssignLocalValue(State endState, State inParams, State outParams)
        {
            SetPrecedenceFactorsFlag(PrecedenceFactors.LocalValue);
            PrintActionInfo("AssignLocalValue");
            SetLocalValue(true);
            return true;
        }

        private bool AssignStyleSetter(State endState, State inParams, State outParams)
        {
            System.Diagnostics.Debug.Assert(!_isStylePropertyTest, "Should not get called in StyleProperty Test");
            SetPrecedenceFactorsFlag(PrecedenceFactors.StyleSetter);
            PrintActionInfo("AssignStyleSetter");
            SetStyle(true);
            return true;
        }

        private bool AssignStyleTemplateTrigger(State endState, State inParams, State outParams)
        {
            System.Diagnostics.Debug.Assert(!_isStylePropertyTest, "Should not get called in StyleProperty Test");
            SetPrecedenceFactorsFlag(PrecedenceFactors.StyleTemplateTrigger);
            PrintActionInfo("AssignStyleTemplateTrigger");
            SetStyle(true);
            return true;
        }

        private bool AssignStyleTrigger(State endState, State inParams, State outParams)
        {
            System.Diagnostics.Debug.Assert(!_isStylePropertyTest, "Should not get called in StyleProperty Test");
            SetPrecedenceFactorsFlag(PrecedenceFactors.StyleTrigger);
            PrintActionInfo("AssignStyleTrigger");
            SetStyle(true);
            return true;
        }

        private bool AssignTemplatedParentLocallySetValue(State endState, State inParams, State outParams)
        {
            SetPrecedenceFactorsFlag(PrecedenceFactors.TemplatedParentLocallySetValue);
            PrintActionInfo("AssignTemplatedParentLocallySetValue");
            SetTestElement(true, true);
            return true;
        }

        private bool AssignTemplatedParentTemplateTrigger(State endState, State inParams, State outParams)
        {
            SetPrecedenceFactorsFlag(PrecedenceFactors.TemplatedParentTemplateTrigger);
            PrintActionInfo("AssignTemplatedParentTemplateTrigger");
            SetTestElement(true, true);
            return true;
        }

        private bool AssignImplicitReference(State endState, State inParams, State outParams)
        {
            SetPrecedenceFactorsFlag(PrecedenceFactors.ImplicitReference);
            PrintActionInfo("AssignImplicitReference");
            SetImplicitReference(true);
            return true;
        }

        #endregion


        #region Back-end functions that perform various actions and validation
        private void SetImplicitReference(bool shouldValidate)
        {
            System.Diagnostics.Debug.Assert(_isStylePropertyTest, "SetImplicitReference should only be called in StyleProperty Test");
            
            Canvas canvas = new Canvas();
            canvas.Resources.Add(typeof(TestControl), _style4ImplicitReference);
            if (_testElementTemplatedParent != null)
            {
                canvas.Children.Add(_testElementTemplatedParent);
            }
            else
            {
                System.Diagnostics.Debug.Assert(_testElement != null, "Check testElement logic");
                canvas.Children.Add(_testElement);
            }

            if (shouldValidate)
            {
                TriggertemplatedParentTemplateTriggerIfNeeded();
                Utilities.Assert(ValidatePrecedence(), "Validate Precedence within SetImplicitReference.");
            }
        }


        private void SetTestElement(bool withTemplatedParent, bool shouldValidate)
        {
            if (withTemplatedParent)
            {
                _testElementTemplatedParent = new Control();
                ControlTemplate ctrlTemplate = new ControlTemplate(typeof(Control));
                FrameworkElementFactory fefCanvas = new FrameworkElementFactory(typeof(Canvas), "backCanvas");
                FrameworkElementFactory fefTestFE = new FrameworkElementFactory(typeof(TestControl), "tc");
                fefCanvas.AppendChild(fefTestFE);
                ctrlTemplate.VisualTree = fefCanvas;

                if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.TemplatedParentLocallySetValue))
                {
                    if (_isStylePropertyTest)
                    {
                        fefTestFE.SetValue(TestControl.StyleProperty, _style4TemplatedParentLocallySetValue);
                    }
                    else
                    {
                        fefTestFE.SetValue(TestControl.MagicWordProperty, PrecedenceFactors.TemplatedParentLocallySetValue.ToString());
                    }
                }

                if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.TemplatedParentTemplateTrigger))
                {
                    Trigger trigger = new Trigger();
                    trigger.Property = Control.WidthProperty;
                    trigger.Value = 100.00;
                    if (_isStylePropertyTest)
                    {
                        trigger.Setters.Add(new Setter(TestControl.StyleProperty, _style4TemplatedParentTemplateTrigger, "tc"));
                    }
                    else
                    {
                        trigger.Setters.Add(new Setter(TestControl.MagicWordProperty, PrecedenceFactors.TemplatedParentTemplateTrigger.ToString(), "tc"));
                    }
                    ctrlTemplate.Triggers.Add(trigger);
                }
                _testElementTemplatedParent.Template = ctrlTemplate;
                _testElementTemplatedParent.ApplyTemplate();
                _testElement = (TestControl)ctrlTemplate.FindName("tc", _testElementTemplatedParent);
            }
            else
            {
                _testElement = new TestControl();
                _testElementTemplatedParent = null;
            }

            if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.ImplicitReference))
            {
                SetImplicitReference(false);
            }

            //Since new TestElement instance is created, we have to go through other Set..() methods
            //To ensure correct setup.
            SetLocalValue(false);
            SetInheritance(false);
            SetStyle(false);
            SetElementTemplate(false);

            if (shouldValidate)
            {
                TriggertemplatedParentTemplateTriggerIfNeeded();
                
                Utilities.Assert(ValidatePrecedence(), "Validate Precedence within SetTestElement.");

                if (!_isStylePropertyTest)
                {
                    Utilities.Assert(ValidateReadLocalValue(), "Validate ReadLocalValue within SetTestElement.");
                }
            }
        }

        private void TriggertemplatedParentTemplateTriggerIfNeeded()
        {
            if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.TemplatedParentTemplateTrigger))
            {
                _testElementTemplatedParent.Width = 100.00;
            }
        }

        private void SetInheritance(bool shouldValidate)
        {
            if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.Inheritance))
            {
                if (_testElementTemplatedParent == null)
                {
                    Canvas canvas = new Canvas();
                    canvas.Children.Add(_testElement);
                    canvas.SetValue(TestControl.MagicWordProperty, PrecedenceFactors.Inheritance.ToString());
                    if (shouldValidate)
                    {
                        Utilities.Assert(ValidatePrecedence(), "Validate precedence within SetInheritance (Without TemplatedParent)");
                        Utilities.Assert(ValidateReadLocalValue(), "Validate ReadLocalValue within SetInheritance (Without TemplatedParent)");
                    }
                }
                else
                {
                    _testElementTemplatedParent.SetValue(TestControl.MagicWordProperty, PrecedenceFactors.Inheritance.ToString());
                    if (shouldValidate)
                    {
                        Utilities.Assert(ValidatePrecedence(), "Validate precedence within SetInheritance (With TemplatedParent).");
                        Utilities.Assert(ValidateReadLocalValue(), "Validate ReadLocalValue within SetInheritance (With TemplatedParent.");
                    }
                }
            }
        }

        private void SetLocalValue(bool shouldValidate)
        {
            if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.LocalValue))
            {
                if (_isStylePropertyTest)
                {
                    _testElement.SetValue(TestControl.StyleProperty, _style4LocalValue);
                    if (shouldValidate)
                    {
                        TriggertemplatedParentTemplateTriggerIfNeeded();
                        Utilities.Assert(ValidatePrecedence(), "Validate precedence within SetLocalValue.");
                    }
                }
                else
                {
                    _testElement.SetValue(TestControl.MagicWordProperty, PrecedenceFactors.LocalValue.ToString());
                    if (shouldValidate)
                    {
                        TriggertemplatedParentTemplateTriggerIfNeeded();
                        Utilities.Assert(ValidatePrecedence(), "Validate precedence within SetLocalValue.");
                        Utilities.Assert(ValidateReadLocalValue(), "Validate ReadLocalValue within SetLocalValue.");
                    }
                }
            }
        }

        private void SetStyle(bool shouldValidate)
        {
            bool needsSetStyle = IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleSetter) 
                || IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleTemplateTrigger)
                || IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleTrigger);

            if (needsSetStyle)
            {
                Style style = new Style(typeof(TestControl));

                if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleTrigger))
                {
                    style.Triggers.Add(GetTrigger(PrecedenceFactors.StyleTrigger));
                }

                if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleSetter))
                {
                    style.Setters.Add(new Setter(TestControl.MagicWordProperty, PrecedenceFactors.StyleSetter.ToString()));
                }

                if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleTemplateTrigger))
                {
                    ControlTemplate template = new ControlTemplate(typeof(TestControl));
                    template.Triggers.Add(GetTrigger(PrecedenceFactors.StyleTemplateTrigger));
                    style.Setters.Add(new Setter(Control.TemplateProperty, template));
                }
                _testElement.Style = style;

                if (shouldValidate)
                {

                    if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleTrigger) || IsPrecedenceFactorsFlagSet(PrecedenceFactors.StyleTemplateTrigger))
                    {
                        _testElement.Width = 100.00;
                    }

                    Utilities.Assert(ValidatePrecedence(), "Validate precedence within SetStyle.");
                    Utilities.Assert(ValidateReadLocalValue(), "Validate ReadLocalValue within SetStyle.");
                }
            }
        }

        private void SetElementTemplate(bool shouldValidate)
        {
            if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.ElementTemplateTrigger))
            {
                ControlTemplate template = new ControlTemplate(typeof(TestControl));
                template.Triggers.Add(GetTrigger(PrecedenceFactors.ElementTemplateTrigger));

                _testElement.Template = template;

                if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.ElementTemplateTrigger))
                {
                    _testElement.Width = 100.00;
                }

                if (shouldValidate)
                {
                    Utilities.Assert(ValidatePrecedence(), "Validate precedence within SetElementTemplate.");
                    Utilities.Assert(ValidateReadLocalValue(), "Validate ReadLocalValue within SetElementTemplate.");
                }
            }
        }

        /// <summary>
        /// Compare current effective value with what the precedence rule prescribes. 
        /// </summary>
        /// <returns>true if validation succeeds (two matches). False otherwise.</returns>
        private bool ValidatePrecedence()
        {
            if (_isStylePropertyTest)
            {
                Style currentValue = (Style)_testElement.GetValue(TestControl.StyleProperty);
                Style expectedValue = (Style)GetExpectedValueBasedOnPrecedenceRule();

                return currentValue == expectedValue;
            }
            else
            {
                System.Diagnostics.Debug.Assert(_testElement != null, "ValidatePrecedence is called at wrong state.");
                string currentValue = (string)_testElement.GetValue(TestControl.MagicWordProperty);
                string expectedValue = (string)GetExpectedValueBasedOnPrecedenceRule();

                Utilities.PrintStatus(string.Format("/Expected/{0}, /Actual/{1}", expectedValue, currentValue));
                return currentValue == expectedValue;
            }
        }


        /// <summary>
        /// This model is on precedence, but it is also interesting to verify 
        /// the correctness of DependencyObject.ReadLocalValue() call for each test case
        /// </summary>
        /// <returns>true is validation succeeds. False otherwise</returns>
        private bool ValidateReadLocalValue()
        {
            System.Diagnostics.Debug.Assert(!_isStylePropertyTest, "Only call ValidateReadLocalValue for non StyleProperty test");

            System.Diagnostics.Debug.Assert(_testElement != null, "ValidateReadLocalValue is called at wrong state.");
            object currentValue = _testElement.ReadLocalValue(TestControl.MagicWordProperty);
            if (IsPrecedenceFactorsFlagSet(PrecedenceFactors.LocalValue))
            {
                return ((string)currentValue == PrecedenceFactors.LocalValue.ToString());
            }
            else
            {
                return (currentValue == DependencyProperty.UnsetValue);
            }
        }

        /// <summary>
        /// Used the Dependency Property Precedence Rule, get the expected Value for current state
        /// </summary>
        /// <returns>The expected Value</returns>
        private object GetExpectedValueBasedOnPrecedenceRule()
        {
            foreach (PrecedenceFactors pf in _precedences)
            {
                if (IsPrecedenceFactorsFlagSet(pf))
                {
                    if (_isStylePropertyTest)
                    {
                        switch (pf)
                        {
                            case PrecedenceFactors.ImplicitReference:
                                return _style4ImplicitReference;
                            case PrecedenceFactors.TemplatedParentLocallySetValue:
                                return _style4TemplatedParentLocallySetValue;
                            case PrecedenceFactors.TemplatedParentTemplateTrigger:
                                return _style4TemplatedParentTemplateTrigger;
                            case PrecedenceFactors.LocalValue:
                                return _style4LocalValue;
                            case PrecedenceFactors.DefaultValue:
                                return null;  //Default value for StyleProperty is null.
                            default:
                                System.Diagnostics.Debug.Fail("Programming logic error.");
                                return -1;
                        }
                    }
                    else
                    {
                        return pf.ToString();
                    }
                }
            }

            System.Diagnostics.Debug.Fail("GetExpectedValueBasedOnPrecedenceRule is called with wrong state!");
            return null;
        }

        [Flags]
        private enum PrecedenceFactors
        {
            LocalValue = 0x00000002,
            TemplatedParentTemplateTrigger = 0x00000008,
            TemplatedParentLocallySetValue = 0x00000010,
            ElementTemplateTrigger = 0x00000040,
            StyleTemplateTrigger = 0x00000100,
            StyleTrigger = 0x00000400,
            StyleSetter = 0x00000800,
            Inheritance = 0x00001000,
            DefaultValue = 0x00002000,
            ImplicitReference = 0x00004000
        }

        /// <summary>
        /// Make sure this is the right precedence order.
        /// Reference: EffectiveValueEntry.cs: internal enum ValueSource
        /// When The rule changes, update this routine
        /// </summary>
        private PrecedenceFactors[] _precedences = 
        {
            PrecedenceFactors.LocalValue,
            PrecedenceFactors.TemplatedParentTemplateTrigger,
            PrecedenceFactors.TemplatedParentLocallySetValue,
            PrecedenceFactors.ImplicitReference,
            PrecedenceFactors.StyleTrigger,
            PrecedenceFactors.ElementTemplateTrigger,
            PrecedenceFactors.StyleTemplateTrigger,
            PrecedenceFactors.StyleSetter,
            PrecedenceFactors.Inheritance,
            PrecedenceFactors.DefaultValue
        };

        #endregion

        #region Helper methods
        private Trigger GetTrigger(PrecedenceFactors pf)
        {
            Trigger trigger = new Trigger();
            trigger.Property = FrameworkElement.WidthProperty;
            trigger.Value = 100.00;
            trigger.Setters.Add(new Setter(TestControl.MagicWordProperty, pf.ToString()));

            return trigger;
        }

        private void PrintActionInfo(string actionName)
        {
            Utilities.PrintStatus(string.Format("Action {0}: {1}", _actionCounter, actionName));
            Utilities.PrintStatus(string.Format("Enum currently assigned: {0}", _testElementPrecedence));

            _actionCounter++;
        }

        private void SetPrecedenceFactorsFlag(PrecedenceFactors pf)
        {
            //In this model, we only switch from 'Not set' to 'Set'
            Debug.Assert(!IsPrecedenceFactorsFlagSet(pf), "In this model, we only switch from 'Not set' to 'Set'");

            _testElementPrecedence |= pf;
        }

        private bool IsPrecedenceFactorsFlagSet(PrecedenceFactors pf)
        {
            if ((_testElementPrecedence & pf) != pf)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region private fields
        private PrecedenceFactors _testElementPrecedence;
        private TestControl _testElement = null;
        private Control _testElementTemplatedParent = null;
        private int _actionCounter = 0;

        private bool _isStylePropertyTest = false;
        private Style _style4LocalValue = new Style(typeof(TestControl));
        private Style _style4TemplatedParentTemplateTrigger = new Style(typeof(TestControl));
        private Style _style4TemplatedParentLocallySetValue = new Style(typeof(TestControl));
        private Style _style4ImplicitReference = new Style(typeof(TestControl));
        #endregion
    }

    /// <summary>
    /// Test Control
    /// </summary>
    public class TestControl : Control
    {
        /// <summary>
        /// Test DP
        /// </summary>
        public static readonly DependencyProperty MagicWordProperty =
            DependencyProperty.RegisterAttached("MagicWord", typeof(string), typeof(TestControl),
            new FrameworkPropertyMetadata("DefaultValue", FrameworkPropertyMetadataOptions.Inherits));
    }

    internal class StringAnimation : StringAnimationBase
    {
        private string _returnValue;
        internal StringAnimation(string from, string to)
        {
            if (!from.Equals(to, StringComparison.InvariantCulture))
            {
                throw new ArgumentException("In this simple String Animation, from and to should be the same");
            }
            _returnValue = from;
        }

        protected override string GetCurrentValueCore(string defaultOriginValue, string defaultDestinationValue, AnimationClock animationClock)
        {
            return _returnValue;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new StringAnimation(_returnValue, _returnValue);
        }
}
}
