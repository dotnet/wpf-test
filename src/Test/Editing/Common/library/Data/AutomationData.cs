// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data about UI Automation and accessibiliy testing./

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/AutomationData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;
    using System.Threading;

    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Text;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Documents;

    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    using WinHorizontalAlignment = System.Windows.HorizontalAlignment;

    #endregion Namespaces.

    /// <summary>
    /// Provides information about interesting pieces of UI that
    /// should be accessible.
    /// </summary>
    public sealed class AccessibleVisualData
    {
        #region Constructors.

        /// <summary>Hides the constructor.</summary>
        private AccessibleVisualData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Evaluates whether the wrapped element matches the
        /// system value for this piece of visual UI.
        /// </summary>
        /// <param name='wrapper'>Wrapper for element to evaluate.</param>
        /// <returns>
        /// An empty string if the system value is matched, a description
        /// of the difference otherwise.
        /// </returns>
        public string MatchesSystemValue(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            if (_callback != null)
            {
                return _callback(wrapper);
            }
            else
            {
                return SystemColorMatch(wrapper);
            }
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>Dependency property that should be accessible.</summary>
        public DependencyProperty DependencyProperty
        {
            get { return this._property; }
        }

        /// <summary>Name of UI that should be accessible.</summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>Index of system color or metric to query.</summary>
        public int ValueIndex
        {
            get { return this._valueIndex; }
        }

        /// <summary>Interesting pieces of UI thatshould be accessible.</summary>
        public static AccessibleVisualData[] Values = new AccessibleVisualData[] {
            FromSysColor("Background", Control.BackgroundProperty, Win32.COLOR_WINDOW),

            //Regression_Bug24
            //For Classic theme, Editing controls foreground is mapped to SystemColors.WindowTextBrushKey
            //FromSysColor("Text", Control.ForegroundProperty, Win32.COLOR_WINDOWTEXT),
            //For Luna and Aero theme, Editing controls foreground is mapped to SystemColors.ControlTextBrushKey
            FromSysColor("Text", Control.ForegroundProperty, Win32.COLOR_BTNTEXT),

            FromCallback("ScrollBar", VerifyScrollBar),

            // These properties are currently not automatically supported by the system.
            //FromCallback("TextSize", VerifyTextSize),
            //FromCallback("Highlight Background", VerifyHighlightBackground),
            //FromCallback("Highlight Text", VerifyHighlightText),
            //FromCallback("3-D Colors", Verify3DColors)
        };

        #endregion Public properties.


        #region Private methods.

        /// <summary>
        /// Returns a short string with the RGB values of the specified color.
        /// </summary>
        private static string ColorToRgbString(Color color)
        {
            return "RGB=" + color.R + "," + color.G + "," + color.B;
        }

        /// <summary>Initializes a new AccessibleVisualData instance.</summary>
        private static AccessibleVisualData FromSysColor(string name,
            DependencyProperty property, int valueIndex)
        {
            AccessibleVisualData result;

            result = new AccessibleVisualData();
            result._name = name;
            result._property = property;
            result._valueIndex = valueIndex;

            return result;
        }

        /// <summary>Initializes a new AccessibleVisualData instance.</summary>
        private static AccessibleVisualData FromCallback(string name,
            VerifyCallback callback)
        {
            AccessibleVisualData result;

            result = new AccessibleVisualData();
            result._name = name;
            result._callback = callback;

            return result;
        }

        /// <summary>
        /// Checks whether the specified Brush property value matches
        /// the given system color.
        /// </summary>
        /// <param name='propertyValue'>Property value on element.</param>
        /// <param name='valueIndex'>Index of system color.</param>
        /// <returns>
        /// An empty string if the system value is matched, a description
        /// of the difference otherwise.
        /// </returns>
        private static string BrushEqualsSysColor(object propertyValue, int valueIndex)
        {
            SolidColorBrush propertyBrush;  // Property value, as a brush.
            uint systemRgb;                 // System-supplied color.
            Color systemColor;              // System-supplied color, as Color.

            propertyBrush = propertyValue as SolidColorBrush;
            systemRgb = Win32.SafeGetSysColor(valueIndex);
            systemColor = ColorUtils.ColorFromWin32(systemRgb);

            if (propertyBrush == null)
            {
                return "Property value is not a SolidColorBrush.";
            }
            else if (propertyBrush.Color.Equals(systemColor))
            {
                return "";
            }
            else
            {
                return "Property value is [" + ColorToRgbString(propertyBrush.Color) +
                    "], expected [" + ColorToRgbString(systemColor) + "].";
            }
        }

        /// <summary>
        /// Returns the first non-null value of the specified property on
        /// the object or one of its parents.
        /// </summary>
        /// <param name="visual">Visual to get value for.</param>
        /// <param name="property">Property to get value for.</param>
        /// <returns>
        /// The first non-null value of the specified property on
        /// the object or one of its parents; throws if not found.
        /// </returns>
        private static object GetAssignedValue(Visual visual, DependencyProperty property)
        {
            Visual original;

            original = visual;
            while (visual != null)
            {
                object result;

                result = visual.GetValue(property);
                if (result != null)
                {
                    return result;
                }
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            throw new Exception("There are no elements with a value assigned " +
                " for " + property + " in the tree for " + original);
        }

        private string SystemColorMatch(UIElementWrapper wrapper)
        {
            return BrushEqualsSysColor(
                wrapper.Element.GetValue(DependencyProperty), ValueIndex);
        }

        private static string VerifyScrollBar(UIElementWrapper wrapper)
        {
            TextBoxBase control;    // Control to be verified.
            string result;          // Overall verification result.
            string barResult;       // ScrollBar-specific result.

            control = wrapper.Element as TextBoxBase;
            if (control == null)
            {
                return "";
            }

            result = "";
            if (control.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible)
            {
                barResult = VerifyScrollBarPart(control, "//HorizontalScrollBar//RepeatButton[1]", true);
                barResult += VerifyScrollBarPart(control, "//HorizontalScrollBar//RepeatButton[2]", true);
                barResult += VerifyScrollBarPart(control, "//HorizontalScrollBar//Thumb", true);
                barResult += VerifyScrollBarPart(control, "//HorizontalScrollBar//RepeatButton[3]", false);
                barResult += VerifyScrollBarPart(control, "//HorizontalScrollBar//RepeatButton[4]", false);
                if (barResult.Length != 0)
                {
                    result = "Horizontal Scrollbar differences:\r\n" + barResult + "\r\n";
                }
            }
            if (control.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
            {
                barResult = VerifyScrollBarPart(control, "//VerticalScrollBar//RepeatButton[1]", true);
                barResult += VerifyScrollBarPart(control, "//VerticalScrollBar//RepeatButton[2]", true);
                barResult += VerifyScrollBarPart(control, "//VerticalScrollBar//Thumb", true);
                barResult += VerifyScrollBarPart(control, "//VerticalScrollBar//RepeatButton[3]", false);
                barResult += VerifyScrollBarPart(control, "//VerticalScrollBar//RepeatButton[4]", false);
                if (barResult.Length != 0)
                {
                    result += "Vertical Scrollbar differences:\r\n" + barResult + "\r\n";
                }
            }

            return result;
        }

        private static string VerifyScrollBarPart(TextBoxBase control, string buttonXpath,
            bool isButton)
        {
            Visual[] buttons;       // Result of visual tree query.
            string buttonResult;    // Description of result for a given button.
            string result;          // Complete result.
            int colorIndex;         // Index of color in system values.

            if (isButton)
            {
                colorIndex = Win32.COLOR_BTNFACE;
            }
            else
            {
                colorIndex = Win32.COLOR_SCROLLBAR;
            }

            result = "";
            buttons = XPathNavigatorUtils.ListVisuals(control, buttonXpath);
            foreach(Visual button in buttons)
            {
                buttonResult = BrushEqualsSysColor(
                    GetAssignedValue(button, Control.BackgroundProperty), colorIndex);                    
                if (buttonResult != "")
                {
                    result += "Mismatch for " + buttonXpath + ": " + buttonResult + "\r\n";
                }
            }
            return result;
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Dependency property checked.</summary>
        private DependencyProperty _property;

        /// <summary>Index of system color or metric to query.</summary>
        private int _valueIndex;

        /// <summary>Name of UI that should be accessible.</summary>
        private string _name;

        /// <summary>Delegate to perform verification.</summary>
        private VerifyCallback _callback;

        #endregion Private fields.

        #region Inner types.

        private delegate string VerifyCallback(UIElementWrapper wrapper);

        #endregion Inner types.
    }

    /// <summary>
    /// Provides information about interesting automation properties.
    /// </summary>
    public sealed class AutomationPropertyData
    {
        #region Constructors.

        /// <summary>Hides the constructor.</summary>
        private AutomationPropertyData()
        {
        }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Verifies the current value of this property on the given
        /// automation/native pairs.
        /// </summary>
        /// <param name='element'>Element with the value to verify.</param>
        /// <param name='wrapper'>Wrapper around the native element.</param>
        /// <param name='handler'>Handler for call back.</param>
        /// <param name='log'>Whether to always log verification messages.</param>
        public void VerifyCurrentValue(AutomationElement element,
            UIElementWrapper wrapper, SimpleHandler handler, bool log)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            _verificationArgs = new VerificationArguments(element, handler, log,
                QueueHelper.Current, wrapper);

            _aeThread = new Thread(new ParameterizedThreadStart(GetValueForProperty));
            _aeThread.SetApartmentState(System.Threading.ApartmentState.STA);
            _aeThread.Start(_verificationArgs);                                       
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>The encapsulated automation property.</summary>
        public AutomationProperty AutomationProperty
        {
            get { return this._automationProperty; }
        }

        /// <summary>Name for the automation property.</summary>
        public string Name
        {
            get { return this._name; }
        }

        /// <summary>Interesting properties.</summary>
        public static AutomationPropertyData[] Values = new AutomationPropertyData[] {
            // AutomationElement properties.
            FromProperty(AutomationElement.ClassNameProperty, "ClassName", ClassNameVerify),
            FromProperty(AutomationElement.AutomationIdProperty, "AutomationId", ""),
            FromProperty(AutomationElement.ControlTypeProperty, "ControlType", ControlTypeVerify),
            FromProperty(AutomationElement.IsContentElementProperty, "IsContentElement", IsContentElementVerify),
            FromProperty(AutomationElement.IsControlElementProperty, "IsControlElement", true),
            FromProperty(AutomationElement.IsDockPatternAvailableProperty, "IsDockPatternAvailable", false),
            FromProperty(AutomationElement.IsExpandCollapsePatternAvailableProperty, "IsExpandCollapsePatternAvailable", false),
            FromProperty(AutomationElement.IsGridItemPatternAvailableProperty, "IsGridItemPatternAvailable", false),
            FromProperty(AutomationElement.IsGridPatternAvailableProperty, "IsGridPatternAvailable", false),
            FromProperty(AutomationElement.IsInvokePatternAvailableProperty, "IsInvokePatternAvailable", false),
            FromProperty(AutomationElement.IsMultipleViewPatternAvailableProperty, "IsMultipleViewPatternAvailable", false),
            FromProperty(AutomationElement.IsPasswordProperty, "IsPassword", IsPasswordVerify),
            FromProperty(AutomationElement.IsRangeValuePatternAvailableProperty, "IsRangeValuePatternAvailable", false),
            FromProperty(AutomationElement.IsScrollPatternAvailableProperty, "IsScrollPatternAvailable", IsScrollPatternAvailableVerify),
            FromProperty(AutomationElement.IsSelectionItemPatternAvailableProperty, "IsSelectionItemPatternAvailable", false),
            FromProperty(AutomationElement.IsTableItemPatternAvailableProperty, "IsTableItemPatternAvailable", false),
            FromProperty(AutomationElement.IsTextPatternAvailableProperty, "IsTextPatternAvailable", IsTextPatternAvailableVerify),
            FromProperty(AutomationElement.IsTogglePatternAvailableProperty, "IsTogglePatternAvailable", false),
            FromProperty(AutomationElement.IsTransformPatternAvailableProperty, "IsTransformPatternAvailable", false),
            FromProperty(AutomationElement.IsValuePatternAvailableProperty, "IsValuePatternAvailable", IsValuePatternAvailableVerify),
            FromProperty(AutomationElement.IsWindowPatternAvailableProperty, "IsWindowPatternAvailable", false),
            FromProperty(AutomationElement.LabeledByProperty, "LabeledBy", null),
            FromProperty(AutomationElement.NativeWindowHandleProperty, "NativeWindowHandle", 0),

            // ValuePattern properties.
            FromProperty(ValuePattern.IsReadOnlyProperty, "IsReadOnly", IsReadOnlyVerify),
            FromProperty(ValuePattern.ValueProperty, "Value", ValueVerify),
        };

        #endregion Public properties.


        #region Private methods.

        /// <summary>Verifies AutomationElement.ClassNameProperty.</summary>
        private static void ClassNameVerify(object currentValue,
            UIElementWrapper wrapper, bool log)
        {
            string expectedValue; // Expected value for this type.

            TextEditableType.GetValueForType(wrapper.Element.GetType(),
                new Type[] { typeof(TextBoxSubClass), typeof(RichTextBoxSubClass), typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox) },
                new string[] { "TextBox", "RichTextBox", "TextBox", "RichTextBox", "PasswordBox" },
                out expectedValue);

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "ClassName", log);
        }

        /// <summary>Verifies AutomationElement.ControlTypeProperty.</summary>
        private static void ControlTypeVerify(object currentValue,
            UIElementWrapper wrapper, bool log)
        {
            ControlType expectedValue; // Expected value for this type.

            if ((wrapper.Element is TextBox) || (wrapper.Element is PasswordBox))
            {
                expectedValue = ControlType.Edit;
            }
            else 
            {
                //RichTextBox
                expectedValue = ControlType.Document;
            }            

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "ControlType", log);
        }

        /// <summary>Initializes a new AutomationPropertyData instance.</summary>
        private static AutomationPropertyData FromProperty(AutomationProperty property,
            string name, VerifierCallback verifierCallback)
        {
            AutomationPropertyData result;

            result = new AutomationPropertyData();
            result._automationProperty = property;
            result._name = name;
            result._verifierCallback = verifierCallback;

            return result;
        }

        /// <summary>Initializes a new AutomationPropertyData instance.</summary>
        private static AutomationPropertyData FromProperty(AutomationProperty property,
            string name, object constantValue)
        {
            AutomationPropertyData result;

            result = new AutomationPropertyData();
            result._automationProperty = property;
            result._name = name;
            result._constantValue = constantValue;

            return result;
        }        

        /// <summary>
        /// Gets the value for AutomationProperty from AutomationElement 
        /// and queues a call-back to the main queue.
        /// </summary>
        /// <param name="verificationArgs">Verification arguments</param>
        private void GetValueForProperty(object verificationArgs)
        {
            VerificationArguments arguments = (VerificationArguments)verificationArgs;
            try
            {
                _currentValue = arguments.Element.GetCurrentPropertyValue(
                    this.AutomationProperty);
            }
            catch (System.InvalidOperationException)
            {
                //InvalidOperation exception should be thrown for ValuePattern on PasswordBox
                if (arguments.Element.GetCurrentPropertyValue(AutomationElement.IsPasswordProperty).Equals(false))
                {
                    Verifier.Verify(false, "InvalidOperationException thrown for non PasswordBox control", true);
                }
                else
                {
                    if (this.AutomationProperty == ValuePattern.ValueProperty)
                    {
                        Logger.Current.Log("InvalidOperationException thrown as expected for PasswordBox on ValuePattern property");                        
                    }
                    else
                    {
                        Verifier.Verify(false, "InvalidOperationException thrown for non ValuePattern property on PasswordBox", true);
                    }
                }
            }

            arguments.MainQueueHelper.QueueDelegate(VerifyPropertyValue);           
        }

        /// <summary>Verifies AutomationElement.IsContentElementProperty.</summary>
        private static void IsContentElementVerify(object currentValue,
            UIElementWrapper wrapper, bool log)
        {
            bool expectedValue; // Expected value for this type.

            TextEditableType.GetValueForType(wrapper.Element.GetType(),
                new Type[] { typeof(TextBoxSubClass), typeof(RichTextBoxSubClass), typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox) },
                new bool[] { true, true, true, true, true },
                out expectedValue);

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "IsContentElement", log);
        }

        /// <summary>Verifies AutomationElement.IsPasswordProperty.</summary>
        private static void IsPasswordVerify(object currentValue, UIElementWrapper wrapper, bool log)
        {
            bool expectedValue; // Expected value for this type.

            TextEditableType.GetValueForType(wrapper.Element.GetType(),
                new Type[] { typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox) },
                new bool[] { false,           false,               true                },
                out expectedValue);

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "IsPassword", log);
        }

        /// <summary>Verifies ValuePattern.IsReadOnlyProperty.</summary>
        private static void IsReadOnlyVerify(
            object currentValue, UIElementWrapper wrapper, bool log)
        {
            if (wrapper.Element is TextBox)
            {
                VerifyCurrentExpectedValue(currentValue,
                    ((TextBoxBase)wrapper.Element).IsReadOnly, "IsReadOnly", log);
            }            
            if (wrapper.Element is PasswordBox)
            {
                VerifyCurrentExpectedValue(currentValue,
                    false, "IsReadOnly", log);
            }
            //RichTextBox doesnt have ValuePattern
        }

        /// <summary>Verifies AutomationElement.IsScrollPatternAvailableProperty.</summary>
        private static void IsScrollPatternAvailableVerify(object currentValue,
            UIElementWrapper wrapper, bool log)
        {
            bool expectedValue; // Expected value for this type.

            TextEditableType.GetValueForType(wrapper.Element.GetType(),
                new Type[] { typeof(TextBoxSubClass), typeof(RichTextBoxSubClass), typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox) },
                new bool[] { true, true, true, true, true },
                out expectedValue);

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "IsScrollPatternAvailable", log);
        }

        /// <summary>Verifies AutomationElement.IsTextPatternAvailableProperty.</summary>
        private static void IsTextPatternAvailableVerify(object currentValue, UIElementWrapper wrapper, bool log)
        {
            bool expectedValue; // Expected value for this type.
            bool isTextPatternAvailableForPasswordBox = false;

            // Test fix for Part1 Regression_Bug25 - TextPattern is now available for password box   
#if TESTBUILD_CLR40
                isTextPatternAvailableForPasswordBox = true;
#endif
            TextEditableType.GetValueForType(wrapper.Element.GetType(),
                new Type[] { typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox) },
                new bool[] { true,            true,                 isTextPatternAvailableForPasswordBox },
                out expectedValue);

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "IsTextPatternAvailable", log);
        }

        private static void IsValuePatternAvailableVerify(
            object currentValue, UIElementWrapper wrapper, bool log)
        {
            bool expectedValue; // Expected value for this type.

            TextEditableType.GetValueForType(wrapper.Element.GetType(),
                new Type[] { typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox) },
                new bool[] { true,            false,               true               },
                out expectedValue);

            VerifyCurrentExpectedValue(currentValue,
                expectedValue, "IsValuePatternAvailable", log);
        }

        /// <summary>Verifies the ValuePattern.ValueProperty.</summary>
        private static void ValueVerify(
            object currentValue, UIElementWrapper wrapper, bool log)
        {
            if (wrapper.Element is TextBox)
            {
                VerifyCurrentExpectedValue(currentValue,
                    ((TextBox)wrapper.Element).Text, "Value", log);
            }           
        }

        /// <summary>
        /// Verifies that the current value of the specified property
        /// for the given element matches an expected value.
        /// </summary>
        private static void VerifyCurrentExpectedValue(object currentValue, 
            object expectedValue, string name, bool log)
        {            
            bool match;             // Whether the values match.
            
            match = (currentValue == null && expectedValue == null) ||
                currentValue.Equals(expectedValue);

            Verifier.Verify(match,
                "Current value [" + currentValue + "] for " + name +
                " matches expected value [" + expectedValue + "]", log);
        }

        private void VerifyPropertyValue()
        {
            if (_verifierCallback == null)
            {
                VerifyCurrentExpectedValue(_currentValue,
                    _constantValue, _name, _verificationArgs.Log);
            }
            else
            {
                _verifierCallback(_currentValue, _verificationArgs.Wrapper, _verificationArgs.Log);
            }

            QueueHelper.Current.QueueDelegate(_verificationArgs.Handler);            
        }

        #endregion Private methods.


        #region Private fields.

        private AutomationProperty _automationProperty;
        private string _name;
        private object _constantValue;
        private VerifierCallback _verifierCallback;
        private object _currentValue;
        private Thread _aeThread;
        private VerificationArguments _verificationArgs;  
        #endregion Private fields.
        

        #region Inner types.

        private delegate void VerifierCallback(object currentValue, UIElementWrapper wrapper, bool log);

        private class VerificationArguments
        {
            private AutomationElement _element;
            private SimpleHandler _handler;
            private bool _log;
            private QueueHelper _mainQueueHelper;
            private UIElementWrapper _wrapper;

            /// <summary>Creates a ThreadArgumentObject</summary>
            /// <param name="element">AutomationElement</param>
            /// <param name="handler">Handler to be called after verification is done</param>
            /// <param name="log">whether to log the verification even if it passes</param>
            /// <param name="mainQueueHelper">QueueHelper used to queue the callback handler</param>
            /// <param name="wrapper">UIElementWrapper of the control</param>            
            public VerificationArguments(AutomationElement element, SimpleHandler handler,
                bool log, QueueHelper mainQueueHelper, UIElementWrapper wrapper)
            {
                _element = element;
                _handler = handler;
                _log = log;
                _mainQueueHelper = mainQueueHelper;
                _wrapper = wrapper;
            }

            /// <summary>AutomationElement</summary>
            public AutomationElement Element
            {
                get { return _element; }
            }

            /// <summary>Handler for callback</summary>
            public SimpleHandler Handler
            {
                get { return _handler; }
            }

            /// <summary>Whether to log the verification even if it passes</summary>
            public bool Log
            {
                get { return _log; }
            }

            /// <summary>QueueHelper used to queue the callback handler</summary>
            public QueueHelper MainQueueHelper
            {
                get { return _mainQueueHelper; }
            }

            /// <summary>UIElementWrapper of the control</summary>
            public UIElementWrapper Wrapper
            {
                get { return _wrapper; }
            }
        }

        #endregion Inner types.
    }

    /// <summary>
    /// Provides information about available AutomationTextAttribute values.
    /// </summary>
    public sealed class AutomationTextAttributeData
    {

        #region Constructors.

        /// <summary>Hides the constructor.</summary>
        private AutomationTextAttributeData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Finds the AutomationTextAttributeData instance for the
        /// specified attribute.
        /// </summary>
        /// <param name='attribute'>Attribute to get data for.</param>
        /// <returns>
        /// The AutomationTextAttributeData instance for the specified
        /// attribute. An exception is thrown if not found.
        /// </returns>
        public static AutomationTextAttributeData FindForAttribute(
            AutomationTextAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }

            foreach(AutomationTextAttributeData result in Values)
            {
                if (result.Attribute == attribute)
                {
                    return result;
                }
            }

            throw new InvalidOperationException("AutomationTextAttributeData not " +
                " found for attribute " + attribute);
        }

        /// <summary>
        /// Gets the expected value for the wrapped element.
        /// </summary>
        /// <param name='wrapper'>Wrapper around element to query.</param>
        /// <returns>The expected text attribute value.</returns>
        public object GetExpectedElementValue(UIElementWrapper wrapper)
        {
            UIElement element;  // Wrapped element.

            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            // Avalon does not support all text attributes.
            if (!IsSupported)
            {
                return AutomationElement.NotSupported;
            }

            element = wrapper.Element;

            if (this.Attribute == TextPattern.AnimationStyleAttribute)
            {
                return (element.HasAnimatedProperties)? AnimationStyle.Other : AnimationStyle.None;
            }

            throw new NotImplementedException(
                "GetExpectedPointerValue does not implement attribute " + Attribute);
        }

        /// <summary>
        /// Gets the expected value for the pointer.
        /// </summary>
        /// <param name='pointer'>Pointer on which to query.</param>
        /// <returns>The expected text attribute value.</returns>
        public object GetExpectedPointerValue(TextPointer pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            // Avalon does not support all text attributes.
            if (!IsSupported)
            {
                return AutomationElement.NotSupported;
            }

            if (this.Attribute == TextPattern.AnimationStyleAttribute)
            {
                DependencyObject parent;
                bool isAnimating;

                isAnimating = false;
                parent = pointer.Parent;
                if (parent == null)
                {
                    throw new InvalidOperationException("Pointer has no parent.");
                }
                if (parent is UIElement)
                {
                    isAnimating = ((UIElement)parent).HasAnimatedProperties;
                }
                else if (parent is ContentElement)
                {
                    isAnimating = ((ContentElement)parent).HasAnimatedProperties;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Unknown pointer parent type: " + parent);
                }
                return (isAnimating)? AnimationStyle.Other : AnimationStyle.None;
            }
            else if (this.Attribute == TextPattern.BackgroundColorAttribute)
            {
                Brush background;

                background = (Brush) pointer.Parent.GetValue(TextElement.BackgroundProperty);
                if (background == null)
                {
                    background = (Brush) pointer.Parent.GetValue(Control.BackgroundProperty);
                    if (background == null)
                    {
                        throw new Exception("Background is null on " + pointer.Parent);
                    }
                }
                return TextPattern.MixedAttributeValue;
            }
            else if (this.Attribute == TextPattern.CapStyleAttribute)
            {
                FontCapitals capitals;  // Capitals property for text.
                CharacterCasing casing; // Character casing.
                bool isAllCaps;         // Whether character casing is uppercase.

                casing = (CharacterCasing)pointer.Parent.GetValue(TextBox.CharacterCasingProperty);
                isAllCaps = casing == CharacterCasing.Upper;

                capitals = (FontCapitals)pointer.Parent.GetValue(Typography.CapitalsProperty);
                if (capitals == FontCapitals.AllSmallCaps ||
                    (isAllCaps && capitals == FontCapitals.SmallCaps) ||
                    capitals == FontCapitals.AllPetiteCaps ||
                    (isAllCaps && capitals == FontCapitals.PetiteCaps))
                {
                    return CapStyle.SmallCap;
                }
                else if (capitals == FontCapitals.Normal)
                {
                    return (isAllCaps)? CapStyle.AllCap : CapStyle.None;
                }
                else
                {
                    return CapStyle.Other;
                }
            }
            else if (this.Attribute == TextPattern.FontNameAttribute)
            {
                return ((FontFamily)pointer.Parent.GetValue(TextElement.FontFamilyProperty)).Source;
            }
            else if (this.Attribute == TextPattern.FontSizeAttribute)
            {
                double size;

                size = (double)pointer.Parent.GetValue(TextElement.FontSizeProperty);
                return size / 96.0 * 72.0;
            }
            else if (this.Attribute == TextPattern.FontWeightAttribute)
            {
                FontWeight weight;

                weight = (FontWeight)pointer.Parent.GetValue(TextElement.FontWeightProperty);
                return FontWeightToLogicalFontInt(weight);
            }
            else if (this.Attribute == TextPattern.ForegroundColorAttribute)
            {
                Brush foreground;

                foreground = (Brush) pointer.Parent.GetValue(TextElement.ForegroundProperty);
                if (foreground == null)
                {
                    throw new Exception("Foreground is null on " + pointer.Parent);
                }
                return BrushToColor(foreground);
            }
            else if (this.Attribute == TextPattern.HorizontalTextAlignmentAttribute)
            {
                WinHorizontalAlignment alignment;

                alignment = (WinHorizontalAlignment)pointer.Parent.GetValue(Block.TextAlignmentProperty);
                return HorizontalAlignmentToHorizontalTextAlignment(alignment);
            }
            else if (this.Attribute == TextPattern.IndentationFirstLineAttribute)
            {
                double indent;

                indent = (double)pointer.Parent.GetValue(Paragraph.TextIndentProperty);
                return PixelsToPoints(indent);
            }
            else if (this.Attribute == TextPattern.IndentationLeadingAttribute)
            {
                Thickness padding;

                // Padding.Left is 'Start' or 'Leading', so it works with
                // right-to-left as well.
                padding = (Thickness)pointer.Parent.GetValue(Block.PaddingProperty);
                return PixelsToPoints(padding.Left);
            }
            else if (this.Attribute == TextPattern.IndentationTrailingAttribute)
            {
                Thickness padding;

                // Padding.Left is 'Start' or 'Leading', so it works with
                // right-to-left as well (symmetrical for right).
                padding = (Thickness)pointer.Parent.GetValue(Block.PaddingProperty);
                return PixelsToPoints(padding.Right);
            }
            else if (this.Attribute == TextPattern.IsItalicAttribute)
            {
                FontStyle style;

                style = (FontStyle)pointer.Parent.GetValue(TextElement.FontStyleProperty);
                return style == FontStyles.Italic || style == FontStyles.Oblique;
            }

            throw new NotImplementedException(
                "GetExpectedPointerValue does not implement attribute " + Attribute);
        }

        /// <summary>
        /// Verifies that the specified range has the correct attribute
        /// value for the wrapped element.
        /// </summary>
        public void VerifyElementValue(TextPatternRange range,
            UIElementWrapper wrapper)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            InternalVerify(range, GetExpectedElementValue(wrapper));
        }

        /// <summary>
        /// Verifies that the specified range has the correct attribute
        /// value for the given TextPointer.
        /// </summary>
        public void VerifyPointerValue(TextPatternRange range,
            TextPointer pointer)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            InternalVerify(range, GetExpectedPointerValue(pointer));
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Attribute with data.</summary>
        public AutomationTextAttribute Attribute
        {
            get { return this._attribute; }
        }

        /// <summary>Whether the attribute is supported by Avalon.</summary>
        public bool IsSupported
        {
            get { return this._isSupported; }
        }

        /// <summary>Friendly name for the attribute.</summary>
        private string Name
        {
            get { return this._name; }
        }

        /// <summary>Type for values of this attribute.</summary>
        public Type ValueType
        {
            get { return this._valueType; }
        }

         /// <summary>Interestnig values for AutomationTextAttribute.</summary>
        public static AutomationTextAttributeData[] Values = new AutomationTextAttributeData[] {
            FromAttribute(TextPattern.AnimationStyleAttribute,              "AnimationStyle",               typeof(AnimationStyle)),
            FromAttribute(TextPattern.BackgroundColorAttribute,             "BackgroundColor",              typeof(uint)),
            NotSupported(TextPattern.BulletStyleAttribute,                 "BulletStyle"                 /*typeof(BulletStyle)*/),
            FromAttribute(TextPattern.CapStyleAttribute,                    "CapStyle",                     typeof(CapStyle)),
            NotSupported(TextPattern.CultureAttribute,                     "Culture"                     /*typeof(CultureInfo)*/),
            FromAttribute(TextPattern.FontNameAttribute,
            FromAttribute(TextPattern.ForegroundColorAttribute,  
            FromAttribute(TextPattern.IndentationLeadingAttribute,          "IndentationLeading",           typeof(double)),
            FromAttribute(TextPattern.IndentationTrailingAttribute,         "IndentationTrailing",          typeof(double)),
            NotSupported(TextPattern.IsHiddenAttribute,                    "IsHidden"                    /*typeof(bool)*/),
            FromAttribute(TextPattern.IsItalicAttribute,                    "IsItalic",                     typeof(bool)),
            FromAttribute(TextPattern.IsReadOnlyAttribute,                  "IsReadOnly",                   typeof(bool)),
            FromAttribute(TextPattern.IsSubscriptAttribute,                 "IsSubscript",                  typeof(bool)),
            FromAttribute(TextPattern.IsSuperscriptAttribute,               "IsSuperscript",                typeof(bool)),
            FromAttribute(TextPattern.MarginBottomAttribute,                "MarginBottom",                 typeof(double)),
            FromAttribute(TextPattern.MarginLeadingAttribute,               "MarginLeading",                typeof(double)),
            FromAttribute(TextPattern.MarginTopAttribute,                   "MarginTop",                    typeof(double)),
            FromAttribute(TextPattern.MarginTrailingAttribute,              "MarginTrailing",               typeof(double)),
            FromAttribute(TextPattern.OutlineStylesAttribute,               "OutlineStyles",                typeof(OutlineStyles)),
            FromAttribute(TextPattern.OverlineColorAttribute,               "OverlineColor",                typeof(uint)),
            FromAttribute(TextPattern.OverlineStyleAttribute,               "OverlineStyle",                typeof(TextDecorationLineStyle)),
            FromAttribute(TextPattern.StrikethroughColorAttribute,          "StrikeThroughColor",           typeof(uint)),
            FromAttribute(TextPattern.StrikethroughStyleAttribute,          "StrikeThroughStyle",           typeof(TextDecorationLineStyle)),
            NotSupported(TextPattern.TabsAttribute,                        "Tabs"                        /*typeof(double[])*/),
            FromAttribute(TextPattern.TextFlowDirectionsAttribute,          "TextFlowDirections",           typeof(FlowDirection)),
            FromAttribute(TextPattern.UnderlineColorAttribute,              "UnderlineColor",               typeof(uint)),
            FromAttribute(TextPattern.UnderlineStyleAttribute,              "UnderlineStyle",               typeof(TextDecorationLineStyle)),
        };

        #endregion Public properties.


        #region Private methods.

        /// <summary>
        /// Converts a System.Windows.Media.Brush to a color value acceptable
        /// to UIAutomation.
        /// </summary>
        private static object BrushToColor(Brush brush)
        {
            SolidColorBrush solidBrush;

            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            solidBrush = brush as SolidColorBrush;
            if (solidBrush == null)
            {
                return TextPattern.MixedAttributeValue;
            }
            else
            {
                return (int)Microsoft.Test.Imaging.ColorUtils.Win32ColorFromColor(solidBrush.Color);
            }
        }

        /// <summary>Maps a FontWeight value to the int value used in LOGFONT.</summary>
        private int FontWeightToLogicalFontInt(FontWeight weight)
        {
            // FontWeight has been defined to map directly.
            return weight.ToOpenTypeWeight();
        }

        /// <summary>Initializes a new AutomationTextAttributeData instance.</summary>
        private static AutomationTextAttributeData FromAttribute(
            AutomationTextAttribute attribute, string name,
            Type valueType)
        {
            AutomationTextAttributeData result;

            System.Diagnostics.Debug.Assert(attribute != null);
            System.Diagnostics.Debug.Assert(name != null);
            System.Diagnostics.Debug.Assert(valueType != null);

            result = new AutomationTextAttributeData();
            result._attribute = attribute;
            result._isSupported = true;
            result._name = name;
            result._valueType = valueType;

            return result;
        }

        /// <summary>Maps a HorizontalAlignment value to a HorizontalTextAlignment.</summary>
        private TextAlignment HorizontalAlignmentToHorizontalTextAlignment(WinHorizontalAlignment alignment)
        {
            switch (alignment)
            {
                case WinHorizontalAlignment.Left:
                    return TextAlignment.Left;
                case WinHorizontalAlignment.Right:
                    return TextAlignment.Right;
                case WinHorizontalAlignment.Center:
                    return TextAlignment.Center;
                //case WinHorizontalAlignment.Justify:
                //    return TextAlignment.Justified;
                default:
                    throw new Exception("Invalid horizontal alignment value: " + (int)alignment);
            }
        }

        /// <summary>Verifies the actual value on the specified range.</summary>
        /// <param name='range'>Range to query value on.</param>
        /// <param name='expectedValue'>Expected attribute value..</param>
        private void InternalVerify(TextPatternRange range,
            object expectedValue)
        {
            object actualValue;     // Actual value as reported by range.
            string errorMessage;    // Error message for mismatches.

            actualValue = range.GetAttributeValue(this.Attribute);
            errorMessage = "Actual value [";
            errorMessage += (actualValue == null)?
                "null" : actualValue.GetType().ToString() + " " + actualValue;
            errorMessage += "] is different from expected value [";
            errorMessage += (expectedValue == null)?
                "null" : expectedValue.GetType().ToString() + " " + expectedValue;
            errorMessage += "] for attribute " + Name;

            if ((actualValue == null) != (expectedValue == null))
            {
                throw new Exception(errorMessage);
            }
            if (actualValue != null && !actualValue.Equals(expectedValue))
            {
                throw new Exception(errorMessage);
            }
        }

        /// <summary>
        /// Initializes a new AutomationTextAttributeData instance for an
        /// attribute that is not supported by Avalon.
        /// </summary>
        private static AutomationTextAttributeData NotSupported(
            AutomationTextAttribute attribute, string name)
        {
            AutomationTextAttributeData result;

            System.Diagnostics.Debug.Assert(attribute != null);
            System.Diagnostics.Debug.Assert(name != null);

            result = new AutomationTextAttributeData();
            result._attribute = attribute;
            result._isSupported = false;
            result._name = name;

            return result;
        }

        /// <summary>
        /// Converts the specified value from Avalon virtual pixels to points.
        /// </summary>
        /// <remarks>The value is points = pixels * 72 / 96.</remarks>
        private double PixelsToPoints(double pixels)
        {
            return pixels;
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Attribute with data.</summary>
        private AutomationTextAttribute _attribute;

        /// <summary>Whether the attribute is supported by Avalon.</summary>
        private bool _isSupported;

        /// <summary>Friendly name for the attribute.</summary>
        private string _name;

        /// <summary>Type for values of this attribute.</summary>
        private Type _valueType;

        #endregion Private fields.
    }
}