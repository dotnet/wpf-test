// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for serializing and deserializing a TextBox control.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;

    using Test.Uis.Data;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    #region enums.

    enum FlowDirectionInputChoices
    {
        Keyboard,
        Programmatical,
        Style,
    };

    #endregion enums.

    /// <summary>
    /// Verifies that the parent of the text content can be serialized
    /// to a string.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("539"), TestBugs("638"),
     TestSample("Parser.GetInnerXml"), TestSample("TextBox.Text")]
    [Test(3, "TextBox", "TextBoxSimpleSerialization", MethodParameters = "/TestCaseType=TextBoxSimpleSerialization /Text=!AD:index=0")]
    public class TextBoxSimpleSerialization : TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            if (TestControl is TextBox)
            {
                SetTextBoxProperties(TestTextBox);
            }

            TestWrapper.Text = Text;

            Log("Text: " + TestWrapper.Text);

            Log("Serializing TextBox...");

            TextRange textRange = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox));

            string serialization = XamlUtils.TextRange_GetXml(textRange);

            Log("Serialization: " + serialization);

            Verifier.Verify(serialization.IndexOf(TestWrapper.Text) != -1,
                "Text [" + TestWrapper.Text + "] found in serialization.", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verify that a TextBox, RichTextBox, and their subclass can be round-tripped when its properties have different values
    /// (serialized out and parsed back in).
    /// </summary>
    [Test(1, "TextBox", "TextBoxOMSerializationDeSerialization", MethodParameters = "/TestCaseType=TextBoxOMSerializationDeSerialization")]
    [TestOwner("Microsoft"), TestTactics("540"), TestWorkItem("91,92"), WindowlessTest(true), TestBugs("905"), TestLastUpdatedOn("May 21, 2006")]
    public class TextBoxOMSerializationDeSerialization : CustomTestCase
    {
        #region Main flow.
        private CombinatorialEngine _combinatorialEngine;
        private TextEditableType _textEditableType;
        private DependencyPropertyData[] _dpDataArray;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Hashtable combination;

            // Set up a combinatorial engine with the relevant dimensions.
            _combinatorialEngine = CombinatorialEngine.FromDimensions(new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.Values),
            });

            // Loop through all interesting combinations.
            combination = new Hashtable();
            while (_combinatorialEngine.Next(combination))
            {
                _textEditableType = (TextEditableType)combination["TextEditableType"];
                DoAction();
            }
            Logger.Current.ReportSuccess();
        }
        private void DoAction()
        {
            Log("Testing " + _textEditableType.Type.Name + "...");

            _dpDataArray = DependencyPropertyData.GetDPDataForControl(_textEditableType.Type);
            foreach (DependencyPropertyData dpData in _dpDataArray)
            {
                FrameworkElement textBox = _textEditableType.CreateInstance();                

                // Set TextBoxOM property
                textBox.SetValue(dpData.Property, dpData.TestValue);

                string serializedText;       // Serialized version of object.
                object parsedObject;         // Object returned from parser.

                Log("Serializing " + _textEditableType.Type + " with " + dpData.Property);
                serializedText = XamlWriter.Save(textBox);
                Log("Xaml " + serializedText);

                Log("Parsing text back...");
                parsedObject = XamlReader.Load(new Test.Uis.IO.StringStream(serializedText));

                // verify property value for parsedObject match with original value
                Log("\nExpect [" + dpData.TestValue.ToString() + "]\nActual [" +
                    ((FrameworkElement)parsedObject).GetValue(dpData.Property).ToString() + "]");
                if ((dpData.TestValue.ToString() == "NoWrap") && (textBox is RichTextBox))
                {
                    Verifier.Verify(((FrameworkElement)parsedObject).GetValue(dpData.Property).ToString() == "Wrap",
                        "Property value is not matched.\nThe original serialized string is [" + serializedText + "]");
                }
                else
                {
                    Verifier.Verify(((FrameworkElement)parsedObject).GetValue(dpData.Property).ToString() == dpData.TestValue.ToString(),
                        "Property value is not matched.\nThe original serialized string is [" + serializedText + "]");
                }
            }
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verify that a TextBox and RichTextBox can be declared as a resource and used to instantiate and edit a control.
    /// Xaml syntax as follow:
    /// <DockPanel xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    ///     <DockPanel.Resources>
    ///         <TextBox x:Key="myBtn" Background="Green" />
    ///     </DockPanel.Resources>
    ///     <Button Content="{DynamicResource myBtn}" />
    /// </DockPanel>
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("541"), TestWorkItem("91")]
    [Test(3, "TextOM", "TextBoxRichTextBoxResource", MethodParameters = "/TestCaseType=TextBoxRichTextBoxResource")]
    public class TextBoxRichTextBoxResource : CustomTestCase
    {
        #region Main flow.
        private DockPanel _dp;
        private TextBox _tb;
        private RichTextBox _rtb;
        private TextBoxSubClass _tbS;
        private RichTextBoxSubClass _rtbS;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            // Creat TextBox inside a DcokPanel.Resource
            _dp = new DockPanel();
            ResourceDictionary rd = new ResourceDictionary();

            _tb = new TextBox();
            _tb.Background = System.Windows.Media.Brushes.Green;

            _rtb = new RichTextBox();
            _rtb.Background = System.Windows.Media.Brushes.Red;
            _rtb.Width = 100;

            _tbS = new TextBoxSubClass();
            _tbS.Background = System.Windows.Media.Brushes.Yellow;
            _tbS.Width = 100;

            _rtbS = new RichTextBoxSubClass();
            _rtbS.Background = System.Windows.Media.Brushes.Blue;
            _rtbS.Width = 100;

            rd.Add("myTB", _tb);
            rd.Add("myRTB", _rtb);
            rd.Add("myTBS", _tbS);
            rd.Add("myRTBS", _rtbS);

            _dp.Resources = rd;

            object oTB = _dp.Resources["myTB"];
            object oRTB = _dp.Resources["myRTB"];
            object oTBS = _dp.Resources["myTBS"];
            object oRTBS = _dp.Resources["myRTBS"];

            TextBox newtb = (TextBox)oTB;
            RichTextBox newrtb = (RichTextBox)oRTB;
            TextBoxSubClass newtbS = (TextBoxSubClass)oTBS;
            RichTextBoxSubClass newrtbS = (RichTextBoxSubClass)oRTBS;

            _dp.Children.Add(newtb);
            _dp.Children.Add(newrtb);
            _dp.Children.Add(newtbS);
            _dp.Children.Add(newrtbS);

            MainWindow.Content = _dp;
            QueueDelegate(TestInTextBox);
        }
        private void TestInTextBox()
        {
            MouseInput.MouseClick(_tb);
            KeyboardInput.TypeString("abc");
            QueueDelegate(TestInRichTextBox);
        }
        private void TestInRichTextBox()
        {
            MouseInput.MouseClick(_rtb);
            KeyboardInput.TypeString("def^a");
            QueueDelegate(TestInTextBoxSubClass);
        }
        private void TestInTextBoxSubClass()
        {
            MouseInput.MouseClick(_tbS);
            KeyboardInput.TypeString("123^a");
            QueueDelegate(TestInRichTextBoxSubClass);
        }
        private void TestInRichTextBoxSubClass()
        {
            MouseInput.MouseClick(_rtbS);
            KeyboardInput.TypeString("987^a");
            QueueDelegate(VerifyResult);
        }
        private void VerifyResult()
        {
            Verifier.Verify(_tb.Text == "abc", "\nExpect text in TB: [abc]\nActual text [" + _tb.Text + "]");
            Verifier.Verify(_rtb.Selection.Text == "def\r\n", "\nExpect text in RTB: [def\r\n]\nActual text [" + _rtb.Selection.Text + "]");
            Verifier.Verify(_tbS.Text == "123", "\nExpect text in TBS: [123]\nActual text [" + _tbS.Text + "]");
            Verifier.Verify(_rtbS.Selection.Text == "987\r\n", "\nExpect text in RTBS: [987\r\n]\nActual text [" + _rtbS.Selection.Text + "]");
            Logger.Current.ReportSuccess();
        }
        #endregion Main flow.
    }

    /// <summary>
    /// Serializing deserializing flowdirection
    /// </summary>
    [Test(0, "TextOM", "RichTextBoxFlowDirectionSerialization", MethodParameters = "/TestCaseType=RichTextBoxFlowDirectionSerialization", Timeout=120)]
    [TestOwner("Microsoft"), TestTactics("542"), TestWorkItem("90"), TestLastUpdatedOn("June 12, 2006")]
    public class RichTextBoxFlowDirectionSerialization : ManagedCombinatorialTestCase
    {
        /// <summary>Starts the combinatorial engine </summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            _controlWrapper = new UIElementWrapper(_element);

            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>DoFocus</summary>
        private void DoFocus()
        {
            _element.Focus();
            if (_element is PasswordBox)
            {
                ((PasswordBox)_element).Password = "PASSWORD";
            }
            else
            {
                KeyboardInput.TypeString("Text");
            }
            QueueDelegate(ExecuteTrigger);
        }

        /// <summary>ExecuteTrigger</summary>
        private void ExecuteTrigger()
        {
            switch (_flowDirectionInputChoicesSwitch)
            {
                case FlowDirectionInputChoices.Programmatical:
                    if (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft)
                    {
                        ((Control)_element).FlowDirection = FlowDirection.RightToLeft;
                    }
                    break;

                case FlowDirectionInputChoices.Keyboard:
                    if (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft)
                    {
                        if(KeyboardInput.IsBidiInputLanguageInstalled())
                        {
                            KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.ControlShiftRight);
                            data[0].PerformAction(_controlWrapper, null, true);
                        }
                        else
                        {

                            QueueDelegate(NextCombination);
                        }
                    }
                    break;

                case FlowDirectionInputChoices.Style:
                    if (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft)
                    {
                        ((Control)_element).Style = GetStyleForEditableType(_element.GetType());
                    }
                    break;

                default:
                    break;
            }
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0,0,1), new SimpleHandler(GetSerializedXaml),null);
        }

        /// <summary>GetSerializedXaml </summary>
        private void GetSerializedXaml()
        {
            _serializedXaml = XamlWriter.Save(_element);
            bool val;
            if (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft)
            {
                val = (_flowDirectionInputChoicesSwitch == FlowDirectionInputChoices.Style) ? _serializedXaml.Contains("FlowDirection.RightToLeft") :
                    _serializedXaml.Contains("FlowDirection=\"RightToLeft\"");
            }
            else
            {
                val = !(_serializedXaml.Contains("FlowDirection=\"LeftToRight\"")) || !(_serializedXaml.Contains("FlowDirection.LeftToRight"));
            }
            if (val == false)
            {
                Log("Serialized XAML Content [" + _serializedXaml + "]");
            }
            Verifier.Verify(val, "Expected to contain FlowDirection [" + _flowDirectionChoicesSwitch.ToString() + "] if expected is LTR, it should not be present in XAML", true);

            if (_element is PasswordBox)
            {
                Verifier.Verify(_serializedXaml.Contains("PASSWORD") == false, "Pass word should not get serialized", false);
            }
            if (_element is RichTextBox)
            {
                RichTextBox _rtb = _element as RichTextBox;
                string _selectionXaml = XamlUtils.TextRange_GetXml(new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd));
                if (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft)
                {
                    val = _selectionXaml.Contains("FlowDirection=\"RightToLeft\"");
                }
                else
                {
                    val = (_selectionXaml.Contains("FlowDirection=\"LeftToRight\""));
                }
                if (val == false)
                {
                    Log("Serialized Selection XAML Content [" + _selectionXaml + "]");
                }
                Verifier.Verify(val, "Expected Selection to contain FlowDirection [" + val.ToString() + "] Selection XAML should contain LTR flow which is default", true);
            }
            QueueDelegate(DeSerializeXaml);
        }

        /// <summary>DeSerializeXaml </summary>
        private void DeSerializeXaml()
        {
            Control _deserializedControl = XamlReader.Load(new Test.Uis.IO.StringStream(_serializedXaml)) as Control;
            bool val;
            if (_element is RichTextBox)
            {
                RichTextBox _deserializedRtb = _deserializedControl as RichTextBox;
                val = (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft) ? (_deserializedRtb.Document.Blocks.FirstBlock.FlowDirection == FlowDirection.RightToLeft) : (_deserializedRtb.Document.Blocks.FirstBlock.FlowDirection == FlowDirection.LeftToRight);
            }
            else
            {
                val = (_flowDirectionChoicesSwitch == FlowDirection.RightToLeft) ? (((Control)_deserializedControl).FlowDirection == FlowDirection.RightToLeft) : (((Control)_deserializedControl).FlowDirection == FlowDirection.LeftToRight);
            }
            Verifier.Verify(val, "Expected FlowDirection [" + _flowDirectionChoicesSwitch.ToString() + "]" , true);
            QueueDelegate(NextCombination);
        }

        /// <summary>GetStyleForEditableType </summary>
        private Style GetStyleForEditableType(Type editableType)
        {
            Style newStyle = new Style(editableType);
            newStyle.Setters.Add(new Setter(RichTextBox.FlowDirectionProperty, FlowDirection.RightToLeft));
            newStyle.Setters.Add(new Setter(RichTextBox.BorderThicknessProperty, new Thickness(14)));
            return newStyle;
        }

        #region private data.

        private TextEditableType _editableType = null;
        private UIElementWrapper _controlWrapper;
        private FrameworkElement _element;

        private FlowDirectionInputChoices _flowDirectionInputChoicesSwitch = 0;
        private FlowDirection _flowDirectionChoicesSwitch = 0;

        string _serializedXaml = "";

        #endregion private data.
    }
}
