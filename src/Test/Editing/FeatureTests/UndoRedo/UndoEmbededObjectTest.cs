// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Collections;

    using Test.Uis.Data;
    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    ///this class will test undo/redo on the embeded element.
    /// </summary>
    [Test(0, "UndoRedo", "UndoEmbededObjectTest", MethodParameters = "/TestCaseType=UndoEmbededObjectTest /Case=UndoSingleElement /ControlType=TextBox", Timeout=120)]
    [TestOwner("Microsoft"), TestTitle("UndoEmbededObjectTest"), TestTactics("366"), TestLastUpdatedOn("Jan 25, 2007")]
    public class UndoEmbededObjectTest : RichEditingBase
    {
        string _controlName;
        TextRange _range;
        ArrayList _undoSingleElementData;
        FrameworkElement _element;
        int _counter = 0; 

        /// <summary>override the base method for RichTextBox</summary>
        public override void Init()
        {
            base.Init();
            ((RichTextBox)TextControlWraper.Element).Document.Blocks.Clear();
            ((RichTextBox)TextControlWraper.Element).Document.Blocks.Add(new Paragraph(new Run("a")));

            _controlName = ConfigurationSettings.Current.GetArgument("ControlType");
            if (null == _controlName || _controlName == string.Empty)
            {
                _controlName = "Button";
            }
            _element = Test.Uis.Utils.ReflectionUtils.CreateInstanceOfType(_controlName, null) as FrameworkElement;
            _element.Name = _controlName;
            _element.Width = 100;
            _element.Height = 30;

            ((Paragraph)((RichTextBox)TextControlWraper.Element).Document.Blocks.FirstBlock).Inlines.Add(new InlineUIContainer(_element));
            ((Paragraph)((RichTextBox)TextControlWraper.Element).Document.Blocks.FirstBlock).Inlines.Add(new Run("c"));

            _range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
        }

        #region case - UndoSingleElement.
        /// <summary>perfrom Undo/redo form embeded element </summary>
        [TestCase(LocalCaseStatus.Ready, "Test for UndoSingleElement")]
        public void UndoSingleElement()
        {
            EnterFuction("UndoSingleElement");
            string str = "a" + " c\r\n" ;
            Verifier.Verify(null != _controlName, CurrentFunction + " - Don't know which control to be used!!!");

            KeyboardInput.TypeString("^{HOME}{RIGHT}+{RIGHT}");

            _undoSingleElementData = new ArrayList();
            _undoSingleElementData.Add(new UndoSingleObjectData("^x", "ac\r\n", "", 0, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^z", str, " ", 1, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^y", "ac\r\n", "", 0, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^v", "a c\r\n", "", 0, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^z^z", str, " ", 1, _controlName));
            ////UndoSingleElementData.Add(new UndoSingleObjectData("^y", str, "", 1, ControlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("{left}{delete}", "ac\r\n", "", 0, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^z", str, "", 1, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^y", "ac\r\n", "", 0, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^z{RIGHT}", str, "", 1, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("{backspace}", "ac\r\n", "", 0, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^z", str, "", 1, _controlName));
            _undoSingleElementData.Add(new UndoSingleObjectData("^y", "ac\r\n", "", 0, _controlName));
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(Perform_An_Action));
            EndFunction();
        }
        void Perform_An_Action()
        {
            MyLogger.Log(" ");
            MyLogger.Log("============== Case #" + _counter.ToString() + " ==============");
            _counter++;
            KeyboardInput.TypeString(((UndoSingleObjectData) _undoSingleElementData[0]).ActionString);
            QueueDelegate(Verify_UndoSingleElement);
        }
        void Verify_UndoSingleElement()
        {
            Sleep();

            string SelectedText = ((UndoSingleObjectData)_undoSingleElementData[0]).SelectedText;
            string RangeText = ((UndoSingleObjectData)_undoSingleElementData[0]).RangeContent;
            FailedIf(SelectedText != TextControlWraper.SelectionInstance.Text, CurrentFunction + " - Failed: Selected text is not correct!!! Expected[" + SelectedText + "] Actual[" + TextControlWraper.SelectionInstance.Text + "]");
            FailedIf(RangeText != _range.Text, CurrentFunction + " - Failed: Text in RichTextBox is wrong!!! Expected[" + RangeText + "] Actual[" + _range.Text + "]");
            int Controlcount = TextOMUtils.EmbeddedObjectCountInRange(_range);
            FailedIf(Controlcount != ((UndoSingleObjectData)_undoSingleElementData[0]).Controlcount, CurrentFunction + " - Failed: expected[" + ((UndoSingleObjectData)_undoSingleElementData[0]).Controlcount.ToString() + "]    Actual[" + Controlcount + "]");

            _undoSingleElementData.RemoveAt(0);

            if (_undoSingleElementData.Count > 0 && pass)
                QueueDelegate(Perform_An_Action);
            else
            {
               QueueDelegate(EndTest);
            }
        }
        #endregion
    }
    /// <summary>
    /// Class contains expected status of a RichTextbox.
    /// </summary>
    public class UndoSingleObjectData
    {
        private string _action;
        private string _rangecontent;
        private string _selectedText;
        private int _controlCount;
        private string _controlName;
        /// <summary>
        /// UndoSingleObjectData
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="RangeContent"></param>
        /// <param name="SelectedText"></param>
        /// <param name="ControlCount"></param>
        /// <param name="ControlName"></param>
        public UndoSingleObjectData(string Action, string RangeContent, string SelectedText, int ControlCount, string ControlName)
        {
            _action = Action;
            _rangecontent = RangeContent;
            _selectedText = SelectedText;
            _controlCount = ControlCount;
            _controlName = ControlName;
        }
        /// <summary>
        /// Return the action string of keyboard
        /// </summary>
        /// <value></value>
        public string ActionString
        {
            get
            {
                return _action;
            }
        }
        /// <summary>
        /// Return the whole content of the RichTextBox
        /// </summary>
        /// <value></value>
        public string RangeContent
        {
            get
            {
                return _rangecontent;
            }
        }
        /// <summary>
        /// Return the expected Selected Text
        /// </summary>
        /// <value></value>
        public string SelectedText
        {
            get
            {
                return _selectedText;
            }
        }
        /// <summary>
        /// Return number of controls in the RichTextbox.
        /// </summary>
        /// <value></value>
        public int Controlcount
        {
            get
            {
                return _controlCount;
            }
        }
        /// <summary>
        /// Return the control Name in the RichTextbox.
        /// </summary>
        /// <value></value>
        public string ControlName
        {
            get
            {
                return _controlName;
            }
        }

    }

    /// <summary>
    /// Verifies that undo and redo work correctly with embedded objects.
    /// </summary>
    /// <remarks>
    /// Test matrix:
    /// - Embedded object: Button, RichTextBox, FlowDocumentPageViewer, Image,
    ///   custom objects that are available [NIY], custom objects that are not
    ///   available in the loaded assmblies [NIY].
    /// - Command that may modify the control.
    /// - Whether the control is in an inline container or in a block container.
    /// - How the control was added: paste, parsed, inserted.
    ///
    /// There are two undo/redo cycles for operations, to ensure the
    /// object is not corrupted during this period.
    ///
    /// Note: FlowDocumentPageViewer was removed, it turns out not to be particularly interesting.
    /// </remarks>
    [Test(0, "UndoRedo", "EmbeddedObjectUndoRedo", MethodParameters = "/TestCaseType=EmbeddedObjectUndoRedo  /InputMonitorEnabled:False ", Timeout=600)]
    [TestOwner("Microsoft"), TestBugs("773,774,775"), TestWorkItem("39"), TestTactics("367")]
    public class EmbeddedObjectUndoRedo : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads a new combination of values for this test run.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);

            result = result &&
                _editingData.TestValue != KeyboardEditingTestValue.UndoCommandKeys &&
                _editingData.TestValue != KeyboardEditingTestValue.RedoCommandKeys;

            result = result &&
                !(_embeddedObjectType == typeof(System.Windows.Controls.Image) && _addition == AdditionKind.ProgrammaticInsertion);

            return result;
        }

        /// <summary>Tests the current combination.</summary>
        protected override void DoRunCombination()
        {
            _control = new RichTextBox();
            TestElement = _control;
            MainWindow.Width = 500d;
            MainWindow.Height = 200d;

            // Perform the undo/redo cycle once.
            _isSecondPass = false;

            QueueDelegate(RunAction);
        }

        private void RunAction()
        {
            CreateEmbeddedObject();

            _control.Focus();
            _control.SelectAll();
            _editingData.PerformAction(new UIElementWrapper(_control), PerformUndo, true);
        }

        private void PerformUndo()
        {
            // Programmatic insertion will always do an undo.
            if (InitialCreationGeneratesUndo)
            {
                Verifier.Verify(_control.Undo(), "Undo acted effectively.");
            }
            else
            {
                _control.Undo();
            }
            QueueDelegate(VerifyUndo);
        }

        private void VerifyUndo()
        {
            if (AdditionKind.Paste == this._addition)
            {
                VerifyElementPresent(false, "undo"); // UNDO UIElement is not supported.
            }
            else
            {
                if (IsKnownDestructive(_editingData) || !InitialCreationGeneratesUndo)
                {
                    VerifyElementPresent(true, "undo");
                }
                else if (IsKnownNonDestructive(_editingData))
                {
                    VerifyElementPresent(false, "undo");
                }
            }
            QueueDelegate(PerformRedo);
        }

        private void PerformRedo()
        {
            if (InitialCreationGeneratesUndo)
            {
                Verifier.Verify(_control.Redo(), "Redo acted effectively.");
            }
            else
            {
                _control.Redo();
            }
            QueueDelegate(VerifyRedo);
        }

        private void VerifyRedo()
        {
            if (IsKnownDestructive(_editingData))
            {
                VerifyElementPresent(false, "redo");
            }
            else if (IsKnownNonDestructive(_editingData))
            {
                if (AdditionKind.Paste == this._addition)
                    VerifyElementPresent(false, "redo"); // UNDO UIElement is not supported.
                else
                    VerifyElementPresent(true, "undo");
            }

            if (_isSecondPass)
            {
                QueueDelegate(NextCombination);
            }
            else
            {
                Log("Second pass for undo/redo...");
                _isSecondPass = true;
                QueueDelegate(PerformUndo);
            }
        }

        #endregion Main flow.

        #region Helper methods.

        private void AddEmbeddedObject(UIElement element)
        {
            switch (this._addition)
            {
                case AdditionKind.ProgrammaticInsertion:
                    using (_control.DeclareChangeBlock())
                    {
                        if (_inBlockContainer)
                        {
                            _control.Document.Blocks.Clear();
                            _control.Document.Blocks.Add(new Paragraph(new Run("text")));
                            _control.Document.Blocks.Add(new BlockUIContainer(element));
                            _control.Document.Blocks.Add(new Paragraph(new Run("text")));
                        }
                        else
                        {
                            _control.Document.Blocks.Clear();
                            _control.Document.Blocks.Add(new Paragraph(new InlineUIContainer(element)));
                        }
                    }
                    break;
                case AdditionKind.Parsing:
                    _control.Document = CreateDocumentForElement(element);
                    break;
                case AdditionKind.Paste:
                    RichTextBox generator = new RichTextBox();
                    generator.Document = CreateDocumentForElement(element);
                    generator.SelectAll();
                    generator.Copy();

                    _control.Paste();
                    break;
            }
        }

        private FlowDocument CreateDocumentForElement(UIElement element)
        {
            string documentMarkup;
            string elementMarkup;
            ParserContext context;

            documentMarkup = "<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>";
            elementMarkup = XamlWriter.Save(element);
            if (_inBlockContainer)
            {
                documentMarkup += "<BlockUIContainer>" + elementMarkup + "</BlockUIContainer>";
            }
            else
            {
                documentMarkup += "<Paragraph>text<InlineUIContainer>" + elementMarkup +
                    "</InlineUIContainer>text</Paragraph>";
            }
            documentMarkup += "</FlowDocument>";

            context = new ParserContext();
            context.BaseUri = new Uri(System.Environment.CurrentDirectory + "/");
            return (FlowDocument)XamlReader.Load(new StringStream(documentMarkup), context);
        }

        private void CreateEmbeddedObject()
        {
            FrameworkElement element = (FrameworkElement)Activator.CreateInstance(_embeddedObjectType);
            element.Name = ElementName;

            InitializeEmbeddedObjectContent(element);

            AddEmbeddedObject(element);
        }

        private void InitializeEmbeddedObjectContent(UIElement element)
        {
            Button button = element as Button;
            if (button != null)
            {
                button.Content = StringData.MixedScripts.Value;
                return;
            }

            FlowDocumentPageViewer viewer = element as FlowDocumentPageViewer;
            if (viewer != null)
            {
                FlowDocument document = new FlowDocument();
                document.Blocks.Add(new List(new ListItem(new Paragraph(new Run(StringData.CombiningCharacters.Value)))));
                viewer.Document = document;
                return;
            }

            RichTextBox box = element as RichTextBox;
            if (box != null)
            {
                box.Background = BrushData.GradientBrush.Brush;
                box.Document.Blocks.Add(new Paragraph(new Run("Inner RichTextBox.")));
                return;
            }

            System.Windows.Controls.Image image = element as System.Windows.Controls.Image;
            if (image != null)
            {
                System.Windows.Media.Imaging.BitmapImage bitmap =
                    new System.Windows.Media.Imaging.BitmapImage(new Uri("test.png", UriKind.Relative));
                image.Source = bitmap;
                return;
            }
        }

        private bool IsKnownDestructive(KeyboardEditingData editingValue)
        {
            KeyboardEditingTestValue value = editingValue.TestValue;
            return
                value == KeyboardEditingTestValue.Alphabetic ||
                value == KeyboardEditingTestValue.AlphabeticShift ||
                value == KeyboardEditingTestValue.Numeric ||
                value == KeyboardEditingTestValue.NumericShift ||
                // value == KeyboardEditingTestValue.AltNumpadKeys ||
                value == KeyboardEditingTestValue.Backspace ||
                value == KeyboardEditingTestValue.BackspaceControl ||
                value == KeyboardEditingTestValue.BackspaceShift ||
                value == KeyboardEditingTestValue.Delete ||
                value == KeyboardEditingTestValue.DeleteControl ||
                value == KeyboardEditingTestValue.CutCommandKeys;
        }

        private bool IsKnownNonDestructive(KeyboardEditingData editingValue)
        {
            return
                editingValue.IsNavigationAction ||
                editingValue.TestValue == KeyboardEditingTestValue.CopyCommandKeys;
        }

        private void VerifyElementPresent(bool expectedPresent, string operation)
        {
            DependencyObject o = LogicalTreeHelper.FindLogicalNode(_control, ElementName);
            bool isPresent = o != null;
            if (isPresent != expectedPresent)
            {
                Log("Error in tree:\n" + XamlWriter.Save(_control));
            }

            Verifier.Verify(expectedPresent == isPresent,
                "Expected element to be present [" + expectedPresent +
                "], but found it [" + isPresent + "] while checking operation " + operation);
        }

        private bool InitialCreationGeneratesUndo
        {
            get { return this._addition != AdditionKind.Parsing; }
        }

        #endregion Helper methods.

        #region Private fields.

        private const string ElementName = "EmbeddedObjectName";

        private bool _isSecondPass;
        private RichTextBox _control;

        private bool _inBlockContainer = false;
        private Type _embeddedObjectType = null;
        private KeyboardEditingData _editingData = null;

        /// <summary>Indicates how the embedded object is added.</summary>
        private AdditionKind _addition =0;

        internal static Type[] EmbeddedObjectTypes
        {
            get
            {
                return new Type[] {
                    typeof(Button),
                    // typeof(FlowDocumentPageViewer),
                    typeof(RichTextBox),
                    typeof(System.Windows.Controls.Image)
                };
            }
        }

        /// <summary>How embedded objects are added to the tree.</summary>
        public enum AdditionKind
        {
            /// <summary>Adds embedded objects through TOM API.</summary>
            ProgrammaticInsertion,
            /// <summary>Adds embedded objects by parsing a document.</summary>
            Parsing,
            /// <summary>Adds embedded objects by pasting from the clipboard.</summary>
            Paste,
        }

        #endregion Private fields.
    }
}
