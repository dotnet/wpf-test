// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;        
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;    

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
        
    #endregion Namespaces.

    /// <summary>
    /// Data model suitable for simple (databinding) test cases.
    /// </summary>
    public class DataBindingModel : INotifyPropertyChanged
    {
        #region Public methods.
        /// <summary>
        /// Constructor which takes a string as argument
        /// </summary>
        public DataBindingModel(double inputFontSize, FontFamily inputFontFamily, bool inputAcceptsReturn, bool inputAcceptsTab, FontStyle inputFontStyle, 
            Thickness inputBorderThickness, TextWrapping inputTextWrapping, int inputMinLines, int inputMaxLines, TextAlignment inputTextAlignment)
        {
            _fontSize = inputFontSize;
            _fontFamily = inputFontFamily;
            _acceptsReturn = inputAcceptsReturn;
            _acceptsTab = inputAcceptsTab;
            _fontStyle = inputFontStyle;
            _borderThickness = inputBorderThickness;
            _textWrapping = inputTextWrapping;
            _minLines = inputMinLines;
            _maxLines = inputMaxLines;
            _textAlignment = inputTextAlignment;


        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataBindingModel()
        {
            

        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("FontSize"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return _fontFamily; }
            set
            {
                _fontFamily = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("FontFamily"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public bool AcceptsReturn
        {
            get { return _acceptsReturn; }
            set
            {
                _acceptsReturn = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("AcceptsReturn"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public bool AcceptsTab
        {
            get { return _acceptsTab; }
            set
            {
                _acceptsTab = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("AcceptsTab"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("FontStyle"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public Thickness BorderThickness
        {
            get { return _borderThickness; }
            set
            {
                _borderThickness = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("BorderThickness"));
                } 
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return _textWrapping; }
            set
            {
                _textWrapping = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("TextWrapping"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public int MinLines
        {
            get { return _minLines; }
            set
            {
                _minLines = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("MinLines"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public int MaxLines
        {
            get { return _maxLines; }
            set
            {
                _maxLines = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("MaxLines"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return _textAlignment; }
            set
            {
                _textAlignment = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("TextAlignment"));
                }
            }
        }

        /// <summary>
        /// Provides the current value of the data model.
        /// </summary>
        public string TextContent
        {
            get { return _textContent; }
            set
            {
                _textContent = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("TextContent"));
                }
            }
        }

        #endregion Public properties.

        #region INotifyPropertyChanged Members

        /// <summary>Fires when a property is changed.</summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged Members

        #region Private fields.

        private double _fontSize = 0;
        private FontFamily _fontFamily = null;
        private bool _acceptsReturn = false;
        private bool _acceptsTab = false;
        private FontStyle _fontStyle ;
        private Thickness _borderThickness;
        private TextWrapping _textWrapping;
        private int _minLines ;
        private int _maxLines;
        private TextAlignment _textAlignment;
        private string _textContent = "";

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the control can be databinded and its values can be cleared
    /// </summary>
    [Test(0, "TextBox", "TestClearValueForDataBoundValues1", MethodParameters = "/TestCaseType:TestClearValueForDataBoundValues")]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "TestClearValueForDataBoundValues2", MethodParameters = "/TestCaseType:TestClearValueForDataBoundValues /XbapName=EditingTestDeploy")]
    [TestOwner("Microsoft"), TestTactics("629,628"), TestWorkItem("118")]
    public class TestClearValueForDataBoundValues : ManagedCombinatorialTestCase
    {
        #region PrivateMembers

        private DataBindingModel _model;
        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;

        private double _defaultFontSize = 0;
        private FontFamily _defaultFontFamily = null;
        private bool _defaultAcceptsReturn = false;
        private bool _defaultAcceptsTab = false;
        private FontStyle _defaultFontStyle ;
        private Thickness _defaultBorderThickness;
        private TextWrapping _defaultTextWrapping;
        private int _defaultMinLines;
        private int _defaultMaxLines;
        private TextAlignment _defaultTextAlignment;

        #endregion

        #region MainFlow

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {

            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            _controlWrapper.Clear();
            TestElement = _element;
            QueueDelegate(GetDefaultValues);
        }

        private void GetDefaultValues()
        {
            _defaultFontSize = (_element is PasswordBox) ? ((PasswordBox)_element).FontSize : ((TextBoxBase)_element).FontSize;
            _defaultFontFamily = (_element is PasswordBox) ? ((PasswordBox)_element).FontFamily : ((TextBoxBase)_element).FontFamily;
            _defaultFontStyle = (_element is PasswordBox) ? ((PasswordBox)_element).FontStyle : ((TextBoxBase)_element).FontStyle;
            _defaultBorderThickness = (_element is PasswordBox) ? ((PasswordBox)_element).BorderThickness : ((TextBoxBase)_element).BorderThickness;
            if ((_element is PasswordBox) == false)
            {
                _defaultAcceptsReturn = ((TextBoxBase)_element).AcceptsReturn;
                _defaultAcceptsTab = ((TextBoxBase)_element).AcceptsTab;                
            }
            if(_element is TextBox)
            {
                _defaultTextWrapping = ((TextBox)_element).TextWrapping;
                _defaultMinLines = ((TextBox)_element).MinLines;
                _defaultMaxLines = ((TextBox)_element).MaxLines;
                _defaultTextAlignment = ((TextBox)_element).TextAlignment;
            }
            QueueDelegate(SetUpBinding);
        }

        private void SetUpBinding()
        {
            Log("Setting up data model...");
            _model = new DataBindingModel(60, new FontFamily("Arial"), true, true, FontStyles.Italic, new Thickness(4), TextWrapping.Wrap, 4,8, TextAlignment.Right);
            _element.DataContext = _model;

            BindingHelper("FontSize", TextElement.FontSizeProperty);
            BindingHelper("FontFamily", TextElement.FontFamilyProperty);

            if ((_element is PasswordBox) == false)
            {
                BindingHelper("AcceptsReturn", TextBoxBase.AcceptsReturnProperty);
                BindingHelper("AcceptsTab", TextBoxBase.AcceptsTabProperty);                
            }

            BindingHelper("FontStyle", TextElement.FontStyleProperty);
            if (_element is PasswordBox)
            {
                BindingHelper("BorderThickness", TextBoxBase.BorderThicknessProperty);
            }
            else
            {
                BindingHelper("BorderThickness", PasswordBox.BorderThicknessProperty);
            }

            if (_element is TextBox)
            {
                BindingHelper("TextWrapping", TextBox.TextWrappingProperty);
                BindingHelper("MinLines", TextBox.MinLinesProperty);
                BindingHelper("MaxLines", TextBox.MaxLinesProperty);
                BindingHelper("TextAlignment", TextBox.TextAlignmentProperty);
            }
            _controlWrapper.Text = "hello";
            QueueDelegate(VerifyBinding);
        }

        private void VerifyBinding()
        {
            Log("Verifying binding ");
            double _fontSize = (_element is PasswordBox) ? ((PasswordBox)_element).FontSize : ((TextBoxBase)_element).FontSize;
            Verifier.Verify(_fontSize == _model.FontSize, "data bound FontSize values should match Expected [" + _model.FontSize.ToString() +
                "] Actual [" + _fontSize.ToString() + "]", true);

            FontFamily _fontFamily = (_element is PasswordBox) ? ((PasswordBox)_element).FontFamily : ((TextBoxBase)_element).FontFamily;
            Verifier.Verify(_fontFamily == _model.FontFamily, "data bound FontFamily values should match Expected [" + _model.FontFamily.ToString() +
                "] Actual [" + _fontFamily.ToString() + "]", true);

            if ((_element is PasswordBox) == false)
            {
                bool _acceptsReturn = ((TextBoxBase)_element).AcceptsReturn;
                Verifier.Verify(_acceptsReturn == _model.AcceptsReturn, "data bound AcceptsReturn values should match Expected [" + _model.AcceptsReturn.ToString() +
                    "] Actual [" + _acceptsReturn.ToString() + "]", true);

                bool _acceptsTab = ((TextBoxBase)_element).AcceptsTab;
                Verifier.Verify(_acceptsTab == _model.AcceptsTab, "data bound AcceptsTab values should match Expected [" + _model.AcceptsTab.ToString() +
                    "] Actual [" + _acceptsTab.ToString() + "]", true);                
            }

            FontStyle _fontStyle = (_element is PasswordBox) ? ((PasswordBox)_element).FontStyle : ((TextBoxBase)_element).FontStyle;
            Verifier.Verify(_fontStyle == _model.FontStyle, "data bound FontStyle values should match Expected [" + _model.FontStyle.ToString() +
                "] Actual [" + _fontStyle.ToString() + "]", true);

            Thickness _borderThickness = (_element is PasswordBox) ? ((PasswordBox)_element).BorderThickness : ((TextBoxBase)_element).BorderThickness;
            Verifier.Verify(_borderThickness == _model.BorderThickness, "data bound BorderThickness values should match Expected [" + _model.BorderThickness.ToString() +
                "] Actual [" + _borderThickness.ToString() + "]", true);

            if (_element is TextBox)
            {
                TextWrapping _textWrapping = ((TextBox)_element).TextWrapping;
                Verifier.Verify(_textWrapping == _model.TextWrapping, "data bound TextWrapping values should match Expected [" + _model.TextWrapping.ToString() +
                    "] Actual [" + _textWrapping.ToString() + "]", true);

                int _minLines =  ((TextBox)_element).MinLines;
                Verifier.Verify(_minLines == _model.MinLines, "data bound MinLines values should match Expected [" + _model.MinLines.ToString() +
                    "] Actual [" + _minLines.ToString() + "]", true);

                int _maxLines = ((TextBox)_element).MaxLines;
                Verifier.Verify(_maxLines == _model.MaxLines, "data bound MaxLines values should match Expected [" + _model.MaxLines.ToString() +
                    "] Actual [" + _maxLines.ToString() + "]", true);

                TextAlignment _textAlignment = ((TextBox)_element).TextAlignment;
                Verifier.Verify(_textAlignment == _model.TextAlignment, "data bound TextAlignment values should match Expected [" + _model.TextAlignment.ToString() +
                    "] Actual [" + _textAlignment.ToString() + "]", true);
            }

            QueueDelegate(ClearValue);
            
        }


        private void ClearValue()
        {
            Log("Clearing Values");
            _element.ClearValue(TextElement.FontSizeProperty);
            _element.ClearValue(TextElement.FontFamilyProperty);

            if ((_element is PasswordBox) == false)
            {
                _element.ClearValue(TextBoxBase.AcceptsReturnProperty);
                _element.ClearValue(TextBoxBase.AcceptsTabProperty);                
            }

            _element.ClearValue(TextElement.FontStyleProperty);
            if (_element is PasswordBox)
            {
                _element.ClearValue(PasswordBox.BorderThicknessProperty);
            }
            else
            {
                _element.ClearValue(TextBoxBase.BorderThicknessProperty);
            }
            if (_element is TextBox)
            {
                _element.ClearValue(TextBox.TextWrappingProperty);
                _element.ClearValue(TextBox.MinLinesProperty);
                _element.ClearValue(TextBox.MaxLinesProperty);
                _element.ClearValue(TextBox.TextAlignmentProperty);
            }
            _element.Focus();
            QueueDelegate(VerifyClearedValues);
        }

        private void VerifyClearedValues()
        {
            Log("Verifying ClearValue ");
            double _fontSize = (_element is PasswordBox) ? ((PasswordBox)_element).FontSize : ((TextBoxBase)_element).FontSize;
            Verifier.Verify(_fontSize == _defaultFontSize, "Default FONTSIZE should match after clearing value Expected [" +
                _defaultFontSize.ToString() + "] Actual [" + _fontSize.ToString() + "]", true);

            FontFamily _fontFamily = (_element is PasswordBox) ? _element.GetValue(PasswordBox.FontFamilyProperty) as FontFamily : _element.GetValue(TextBoxBase.FontFamilyProperty) as FontFamily;
            Verifier.Verify(_fontFamily == _defaultFontFamily, "Default FontFamily should match after clearing value Expected [" +
                _defaultFontFamily.ToString() + "] Actual [" + _fontFamily.ToString() + "]", true);

            if ((_element is PasswordBox) == false)
            {
                bool _acceptsReturn = (bool)((TextBoxBase)_element).GetValue(TextBoxBase.AcceptsReturnProperty);
                Verifier.Verify(_acceptsReturn == _defaultAcceptsReturn, "Default AcceptsReturn values should match Expected [" + _defaultAcceptsReturn.ToString() +
                    "] Actual [" + _acceptsReturn.ToString() + "]", true);

                bool _acceptsTab = ((TextBoxBase)_element).AcceptsTab;
                Verifier.Verify(_acceptsTab == _defaultAcceptsTab, "Default AcceptsTab values should match Expected [" + _defaultAcceptsTab.ToString() +
                    "] Actual [" + _acceptsTab.ToString() + "]", true);                
            }

            FontStyle _fontStyle = (_element is PasswordBox) ? ((PasswordBox)_element).FontStyle : ((TextBoxBase)_element).FontStyle;
            Verifier.Verify(_fontStyle == _defaultFontStyle, "Default FontStyle values should match Expected [" + _defaultFontStyle.ToString() +
                "] Actual [" + _fontStyle.ToString() + "]", true);

            Thickness _borderThickness = (_element is PasswordBox) ? ((PasswordBox)_element).BorderThickness : ((TextBoxBase)_element).BorderThickness;
            Verifier.Verify(_borderThickness == _defaultBorderThickness, "Default BorderThickness values should match Expected [" + _defaultBorderThickness.ToString() +
                "] Actual [" + _borderThickness.ToString() + "]", true);

            if (_element is TextBox)
            {
                TextWrapping _textWrapping = ((TextBox)_element).TextWrapping;
                Verifier.Verify(_textWrapping == _defaultTextWrapping, "Default TextWrapping values should match Expected [" + _defaultTextWrapping.ToString() +
                    "] Actual [" + _textWrapping.ToString() + "]", true);

                int _minLines =(int) ((TextBox)_element).GetValue(TextBox.MinLinesProperty) ;
                Verifier.Verify(_minLines == _defaultMinLines, "Default MinLines values should match Expected [" + _defaultMinLines.ToString() +
                    "] Actual [" + _minLines.ToString() + "]", true);

                int _maxLines = ((TextBox)_element).MaxLines;
                Verifier.Verify(_maxLines == _defaultMaxLines, "Default MaxLines values should match Expected [" + _defaultMaxLines.ToString() +
                    "] Actual [" + _maxLines.ToString() + "]", true);

                TextAlignment _textAlignment = (TextAlignment)((TextBox)_element).GetValue(TextBox.TextAlignmentProperty)  ;
                Verifier.Verify(_textAlignment == _defaultTextAlignment, "Default TextAlignment values should match Expected [" + _defaultTextAlignment.ToString() +
                    "] Actual [" + _textAlignment.ToString() + "]", true);
            }

            QueueDelegate(NextCombination);

        }

        private void BindingHelper(string Path, DependencyProperty dp)
        {
            Binding bind = new Binding(Path);
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _element.SetBinding(dp, bind);
        }
        #endregion MainFlow
    }

    /// <summary>
    /// Verifies that the readonly controls can be databinded 2-way
    /// tests text on textbox and fontsize on all controls
    /// </summary>
    [Test(2, "TextBox", "TestDataBoundingOnReadOnlyControls", MethodParameters = "/TestCaseType:TestDataBoundingOnReadOnlyControls")]
    [TestOwner("Microsoft"), TestTactics("626"), TestWorkItem("117")]
    public class TestDataBoundingOnReadOnlyControls : ManagedCombinatorialTestCase
    {
        #region PrivateMembers

        private DataBindingModel _model;
        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;

        private string _initialData = "FROM DATA";
        private string _controlData = "FROM CONTROL";
        private double _dataFontSize = 30;
        private double _controlFontSize = 60;
      
        #endregion

        #region MainFlow

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is PasswordBox)
            { }
            else
            {
                ((TextBoxBase)_element).IsReadOnly = true;
            }
            _controlWrapper = new UIElementWrapper(_element);
            _controlWrapper.Clear();
            TestElement = _element;
            QueueDelegate(SetUpBinding);
        }

        private void SetUpBinding()
        {
            Log("Setting up data model...");
            _model = new DataBindingModel();
            _element.DataContext = _model;

            BindingHelper("FontSize", TextElement.FontSizeProperty);
            if (_element is TextBox)
            {
                BindingHelper("TextContent", TextBox.TextProperty);
            }
            _model.TextContent = _initialData;
            _model.FontSize = _dataFontSize;
            QueueDelegate(VerifyDataBindingFromData);
        }

        private void VerifyDataBindingFromData()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.Text == _initialData, "Data bound values should match Expected [" +
                    _initialData + "] Control data Actual [" + _controlWrapper.Text + "]", true);
            }

            double _fontSize = (_element is PasswordBox) ? ((PasswordBox)_element).FontSize : ((TextBoxBase)_element).FontSize;
            Verifier.Verify(_fontSize == _model.FontSize, "Data bound FontSize values should match Expected [" +
                _model.FontSize.ToString() + "]Control  FontSize Actual [" + _fontSize.ToString() + "]", true);

            _controlWrapper.Text = _controlData;
            if (_element is PasswordBox)
            {
                ((PasswordBox)_element).FontSize= _controlFontSize;
            }
            else
            {
                ((TextBoxBase)_element).FontSize = _controlFontSize;
            }

            QueueDelegate(VerifyBindingThroughControl);
        }

        private void VerifyBindingThroughControl()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.Text == _model.TextContent, "Data bound values should match Expected [" +
                    _controlWrapper.Text + "] Model Data Actual [" + _model.TextContent + "]", true);
            }
            double _fontSize = (_element is PasswordBox) ? ((PasswordBox)_element).FontSize : ((TextBoxBase)_element).FontSize;
            Verifier.Verify(_fontSize == _model.FontSize, "Data bound FontSize values should match Expected [" +
                _model.FontSize.ToString() + "] Model  FontSize Actual [" + _fontSize.ToString() + "]", true);

            QueueDelegate(NextCombination);
        }

        private void BindingHelper(string Path, DependencyProperty dp)
        {
            Binding bind = new Binding(Path);
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _element.SetBinding(dp, bind);
        }

        #endregion MainFlow
    }
}
