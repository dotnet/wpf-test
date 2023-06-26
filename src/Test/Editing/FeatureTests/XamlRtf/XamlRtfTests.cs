// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Tests the Xaml-Rtf converter.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Winforms = System.Windows.Forms;

    using Test.Uis.Data;
    using Microsoft.Test.Imaging;
    using Test.Uis.IO;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Reflection;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>operations for XamlRtf coverter tests.</summary>
    internal enum XamlRtfOperations
    {
        Copy,
        Cut,
        Drag,
    }

    /// <summary>
    /// Tests XamlRtf converter by performing Cut/Copy/Paste and Drag/Drop across Avalon's
    /// RichTextBox and Winforms RichTextBox.
    /// </summary>
    [Test(0, "XamlRtf", "XamlToRtfTest1", MethodParameters = "/TestCaseType=XamlToRtfTest /Opr:CutCopyPaste")]

    // DISABLEDUNSTABLETEST:
    // TestName:XamlToRtfTest2
    // Area: Editing SubArea: XamlRtf
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    [Test(2, "XamlRtf", "XamlToRtfTest2", MethodParameters = "/TestCaseType=XamlToRtfTest /Opr:DragDrop", Timeout = 240,Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("361,362"), TestBugs(""), TestWorkItem("")]
    public class XamlToRtfTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);
            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _wrapper = new UIElementWrapper(_rtb);
            _wrapper.XamlText = _xamlContent;
            _rtb.Height = 300;
            _rtb.Width = 400;

            _rtb.DragOver += new DragEventHandler(_rtb_DragOver);

            _panel = new StackPanel();
            _panel.HorizontalAlignment = HorizontalAlignment.Left;
            _panel.Children.Add(_rtb);

            MainWindow.Height = _mainWindowHeight;
            MainWindow.Width = _mainWindowWidth;
            MainWindow.Top = 0d;
            MainWindow.Left = _mainWindowLeft;

            TestElement = _panel;

            MainWindow.Closed += new EventHandler(MainWindow_Closed);

            if (_form == null)
            {
                SetUpWinformsWindow();
            }
            else
            {
                QueueDelegate(DoFocus);
            }
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            _form.Close();
        }

        private void SetUpWinformsWindow()
        {
            _form = new System.Windows.Forms.Form();
            _form.Text = "Winforms RichTextBox";

            _winformsRTB = new System.Windows.Forms.RichTextBox();
            _winformsRTB.Height = 300;
            _winformsRTB.Width = 400;
            _winformsRTB.AllowDrop = true;
            _winformsRTB.EnableAutoDragDrop = true;

            _form.Controls.Add(_winformsRTB);
            _form.Shown += new EventHandler(_form_Shown);

            Winforms.Application.Run(_form);
        }

        private void _form_Shown(object sender, EventArgs e)
        {            
            UIElementWrapper.HighDpiScaleFactors(out _xFactor, out _yFactor);
            int xPosition = (int)((float)MainWindow.Left * _xFactor);
            int yPosition = (int)((float)MainWindow.Height * _yFactor);
            _form.DesktopLocation = new System.Drawing.Point(xPosition, yPosition);            

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            Clipboard.Clear();
            _winformsRTB.Clear();

            _rtb.Focus();
            QueueDelegate(PerformOperationAtXamlSource);            
        }

        private void PerformOperationAtXamlSource()
        {
            _rtb.SelectAll();
            _sourceContent = _rtb.Selection.Text;
            _originalXamlString = XamlUtils.TextRange_GetXml(_rtb.Selection);

            Log("Performing Copy/Cut/Drag operation in RichTextBox");
            if (_xamlRtfOperation == XamlRtfOperations.Copy)
            {
                _rtb.Copy();                
            }
            else if (_xamlRtfOperation == XamlRtfOperations.Cut)
            {
                _rtb.Cut();                
            }
            else if (_xamlRtfOperation == XamlRtfOperations.Drag)
            {
                Rect xamlRect = _wrapper.GetGlobalCharacterRect(_wrapper.Text.Length / 2, LogicalDirection.Backward);

                _xamlPoint = new System.Windows.Point(xamlRect.Left + xamlRect.Width / 2,
                    xamlRect.Top + xamlRect.Height / 2);
                int formsTitleBarHeight = _form.Height - _form.ClientRectangle.Height;

                //buffer part might have to be changed if there is any UI change in terms of
                //padding, margin etc. If drag/drop operation fails, try adjusting buffer
                _rtfPoint = new System.Windows.Point(_xamlPoint.X, (MainWindow.Height*_yFactor)
                    + formsTitleBarHeight + 9 /*buffer*/);

                MouseInput.MouseDragInOtherThread(_xamlPoint, _rtfPoint, true,
                    TimeSpan.FromMilliseconds(500), VerifyTargetRtfContent, Dispatcher.CurrentDispatcher);

                return;
            }

            QueueDelegate(DoTargetRtfFocus);
        }

        private void DoTargetRtfFocus()
        {
            if ((_xamlRtfOperation == XamlRtfOperations.Copy) || (_xamlRtfOperation == XamlRtfOperations.Cut))
            {
                VerifyRtfClipboardData();
            }

            //_winformsRTB.Focus();

            QueueDelegate(PerformOperationAtRtfTarget);
        }

        private void PerformOperationAtRtfTarget()
        {
            Log("Perfoming Paste operation in Winforms_RichTextBox");
            _winformsRTB.Paste();

            QueueDelegate(VerifyTargetRtfContent);
        }

        private void VerifyTargetRtfContent()
        {
            //Rtf has only \n as new line. so replace it to \r\n before comparison.
            _targetContent = _winformsRTB.Text.Replace("\n", "\r\n");

            Verifier.Verify(_sourceContent.Contains(_targetContent),
            "Verifying the contents in Winforms_RichTextBox. Actual [" + _winformsRTB.Text +
            "] expected to be contained in [" + _sourceContent + "]", true);

            VerifyRtfData(_winformsRTB.Rtf);

            //Now copy from Winforms and paste into Avalon TextBox.
            Log("Performing Copy/Cut/Drag operation in Winforsm_RichTextBox");
            _winformsRTB.SelectAll();
            _rtfString = _winformsRTB.SelectedRtf;
            if (_xamlRtfOperation == XamlRtfOperations.Copy)
            {
                _winformsRTB.Copy();
            }
            else if (_xamlRtfOperation == XamlRtfOperations.Cut)
            {
                _winformsRTB.Cut();
            }
            else if (_xamlRtfOperation == XamlRtfOperations.Drag)
            {
                _rtb.Document.Blocks.Clear();

                MouseInput.MouseDragInOtherThread(_rtfPoint, _xamlPoint, true,
                    TimeSpan.FromMilliseconds(500), VerifyTargetXamlContent, Dispatcher.CurrentDispatcher);

                return;
            }

            QueueDelegate(DoTargetXamlFocus);
        }

        private void DoTargetXamlFocus()
        {
            VerifyXamlClipboardData();
            _rtb.Document.Blocks.Clear();
            _rtb.Focus();

            QueueDelegate(PerformOperationAtXamlTarget);            
        }

        private void PerformOperationAtXamlTarget()
        {
            Log("Performing Paste operation in RichTextBox");
            _rtb.Paste();            

            QueueDelegate(VerifyTargetXamlContent);
        }

        private void VerifyTargetXamlContent()
        {
            _rtb.SelectAll();

            //Rtf has only \n as new line. so replace it to \r\n before comparison.
            _sourceContent = _winformsRTB.Text.Replace("\n", "\r\n");
            _targetContent = _wrapper.SelectionInstance.Text;

            Verifier.Verify(_targetContent.Contains(_sourceContent),
                "Verifying the contents in RichTextBox. Actual [" + _targetContent +
                "] expected to contain [" + _sourceContent + "]", true);

            VerifyXamlData(XamlUtils.TextRange_GetXml(_rtb.Selection));

            //Verify round-trip xaml: Xaml->Rtf->Xaml
            //Verifier.Verify(_rtb.Selection.Xml == _originalXamlString,
            //    "Verifying new Xaml with original Xaml (Roundtrip)", true);

            QueueDelegate(NextCombination);
        }

        void _rtb_DragOver(object sender, DragEventArgs e)
        {
            string rtfData;

            DataObject data = (DataObject)e.Data;

            Log("Verifying Rtf data in DataObject from DragOver event...");

            Verifier.Verify(data.GetDataPresent(DataFormats.Rtf),
                "Verifying that RTF data format is available when XamlRtf coverter is enabled", true);

            rtfData = (string)data.GetData(DataFormats.Rtf);
            VerifyRtfData(rtfData);
        }

        #endregion Main flow

        #region Verification helpers

        private void VerifyRtfData(string rtfData)
        {
            Log("Verifying RTF Data...");
            if (_originalXamlString.Contains("Bold"))
            {
                Verifier.Verify(rtfData.Contains(@"\b ") || rtfData.Contains(@"\b\"),
                    "Verifying that bold tag is present in RTF", true);
            }

            if (_originalXamlString.Contains("Italic"))
            {
                Verifier.Verify(rtfData.Contains(@"\i ") || rtfData.Contains(@"\i\"),
                    "Verifying that italic tag is present in RTF", true);
            }

            if (_originalXamlString.Contains("Underline"))
            {
                Verifier.Verify(rtfData.Contains(@"\ul ") || rtfData.Contains(@"\ul\"),
                    "Verifying that underline tag is present in RTF", true);
            }
        }

        private void VerifyXamlData(string xamlData)
        {
            Log("Verifying Xaml Data...");
            if (_rtfString.Contains(@"\b ") || _rtfString.Contains(@"\b\"))
            {
                Verifier.Verify(xamlData.Contains("Bold"),
                    "Verifying that bold tag is present in Xaml", true);
            }

            if (_rtfString.Contains(@"\i ") || _rtfString.Contains(@"\i\"))
            {
                Verifier.Verify(xamlData.Contains("Italic"),
                    "Verifying that italic tag is present in Xaml", true);
            }

            if (_rtfString.Contains(@"\ul ") || _rtfString.Contains(@"\ul\"))
            {
                Verifier.Verify(xamlData.Contains("Underline"),
                    "Verifying that underline tag is present in Xaml", true);
            }
        }

        private void VerifyXamlClipboardData()
        {
            DataObject data = (DataObject)Clipboard.GetDataObject();

            Verifier.Verify(data.GetDataPresent(DataFormats.Rtf),
                "Verifying that RTF data format is available when copied from Winforms_RTB", true);
            Verifier.Verify(!data.GetDataPresent(DataFormats.Xaml),
                "Verifying that Xaml data format is NOT available when copied from Winforms_RTB", true);
        }

        private void VerifyRtfClipboardData()
        {
            string rtfData;

            DataObject data = (DataObject)Clipboard.GetDataObject();

            Log("Verifying Rtf data in Clipboard...");

            Verifier.Verify(data.GetDataPresent(DataFormats.Rtf),
                "Verifying that RTF data format is available when XamlRtf coverter is enabled", true);

            rtfData = (string)data.GetData(DataFormats.Rtf);
            VerifyRtfData(rtfData);
        }

        #endregion Verification helpers

        #region Private fields

        private XamlRtfOperations _xamlRtfOperation = 0;
        private string _xamlContent ="";
        private RichTextBox _rtb;
        private UIElementWrapper _wrapper;
        private StackPanel _panel;

        private Winforms.Form _form;
        private Winforms.RichTextBox _winformsRTB;

        private string _sourceContent, _targetContent;
        private string _originalXamlString, _rtfString;

        private System.Windows.Point _xamlPoint, _rtfPoint;
        private const double _mainWindowLeft = 50d;
        private const double _mainWindowHeight = 400d;
        private const double _mainWindowWidth = 500d;

        private float _xFactor,_yFactor;

        #endregion Private fields
    }

    /// <summary>
    /// XAML RTF tool
    /// </summary>
    public class XAML_RTFConverterTool : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _topPanel = new StackPanel();
            _topPanel.Background = System.Windows.Media.Brushes.Black;
            _rtb = new RichTextBox();
            _rtb.Height = 200;
            _rtb.BorderBrush = System.Windows.Media.Brushes.Red;
            _rtb.BorderThickness = new Thickness(2);
            _rtb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _rtb.AcceptsTab = true;
            _rtb.Name = "rtb";

            _status = new TextBox();

            _getRTF = new Button();
            _getRTF.Content = "RTF from ClipBoard";
            _getRTF.Click += delegate
            {
                DisplayRTF();
            };

            _getXAML = new Button();
            _getXAML.Content = "XAML from RTB";
            _getXAML.Click += delegate
            {
                DisplayXAML();
            };

            _pasteXaml = new Button();
            _pasteXaml.Content = "Paste XAML";
            _pasteXaml.Click += delegate
            {
                PasteXaml();
            };

            _displayForm = new Button();
            _displayForm.Content = "Paste RTF";
            _displayForm.Click += delegate
            {
                ViewRTF();
            };

            Grid _grid = new Grid();
            SetupGrid(_grid);

            _status.IsEnabled = false;
            _status.Text = "RTF Converter disabled";

            _xamlLabel = new Label();
            _xamlLabel.Content = _label;
            SetColorForLabel(_xamlLabel);

            _xamlResources = new TextBox();
            _xamlResources.Text = "<!-- Resources can be added here --> \r\n";
            _xamlResources.MaxHeight = 100;
            TextPadSettings(_xamlResources);

            _rtbLabel = new Label();
            _rtbLabel.Content = _xamlRTB;
            SetColorForLabel(_rtbLabel);

            _xamlTextPad = new TextBox();
            _xamlTextPad.Text = "MaxHeight=\"200\" BorderBrush=\"Red\" >\r\n<FlowDocument>\r\n\r\n\r\n\r\n\r\n\r\n\r\n </FlowDocument>";
            _xamlTextPad.Height = 120;
            TextPadSettings(_xamlTextPad);

            _xamlClose = new Label();
            _xamlClose.Content = _closingTags;
            SetColorForLabel(_xamlClose);

            _topPanel.Children.Add(_rtb);
            _topPanel.Children.Add(_grid);
            _topPanel.Children.Add(_status);
            _topPanel.Children.Add(_xamlLabel);
            _topPanel.Children.Add(_xamlResources);
            _topPanel.Children.Add(_rtbLabel);
            _topPanel.Children.Add(_xamlTextPad);
            _topPanel.Children.Add(_xamlClose);

            MainWindow.Content = _topPanel;
            MainWindow.Height = 500;
            MainWindow.Width = 1100;
            MainWindow.Show();

            ShowRTFwindow();
            ShowXAMLwindow();
            SetUpWinformsWindow();
        }

        /// <summary>DisplayRTF in window</summary>
        private void DisplayRTF()
        {
            string _dataFormat = DataFormats.Rtf.ToString();
            string _tbName = "rtfBox";

            SetTextInWindow(_dataFormat, _tbName, _rtfWin);
        }

        /// <summary>DisplayXAML in window</summary>
        private void DisplayXAML()
        {
            string _dataFormat = DataFormats.Xaml.ToString();
            string _tbName = "xamlBox";
            SetTextInWindow(_dataFormat, _tbName, _xamlWin);
        }

        /// <summary>PasteXaml</summary>
        private void PasteXaml()
        {
            object parsedObject;
            FrameworkElement element;

            string str = _xamlLabel.Content.ToString() + _xamlResources.Text + _rtbLabel.Content.ToString() + _xamlTextPad.Text + _xamlClose.Content.ToString();
            try
            {
                parsedObject = XamlUtils.ParseToObject(str);
            }
            catch (XamlParseException exception)
            {
                MessageBox.Show(exception.ToString());
                return;
            }
            element = parsedObject as FrameworkElement;
            if (element == null)
            {
                MessageBox.Show(
                    "Topmost element must be a FrameworkElement-derived type.");
                return;
            }
            _topPanel.Children.RemoveAt(0);
            _topPanel.Children.Insert(0, element);
        }

        /// <summary>ViewRTF</summary>
        private void ViewRTF()
        {
            TextBox tb = (TextBox)(LogicalTreeHelper.FindLogicalNode(_rtfWin, "rtfBox"));
            _winformsRTB.Rtf = tb.Text;
        }

        #region helpers.

        /// <summary>PasteXaml</summary>
        private void SetUpWinformsWindow()
        {
            _form = new System.Windows.Forms.Form();
            _form.Text = "Winforms RichTextBox";

            _winformsRTB = new System.Windows.Forms.RichTextBox();
            _winformsRTB.Height = 300;
            _winformsRTB.Width = 400;
            _winformsRTB.AllowDrop = true;
            _winformsRTB.EnableAutoDragDrop = true;

            _form.Controls.Add(_winformsRTB);
          //  _form.Shown += new EventHandler(_form_Shown);

            _form.Show();
        }

        /// <summary>SetColorForLabel</summary>
        private void SetColorForLabel(Label _passedLabel)
        {
            _passedLabel.Background = System.Windows.Media.Brushes.Black;
            _passedLabel.Foreground = System.Windows.Media.Brushes.Cyan;
        }

        /// <summary>SetTextIn RTF and XAML Window</summary>
        private void SetTextInWindow(string _dataFormat, string _tbName, Window _win)
        {
            TextBox tb = (TextBox)(LogicalTreeHelper.FindLogicalNode(_win, _tbName));
            tb.Text = "";
            if (_dataFormat == DataFormats.Rtf.ToString())
            {
                if (Clipboard.ContainsData(_dataFormat))
                {
                    string str = Clipboard.GetData(DataFormats.Rtf).ToString();
                    str = str.Replace("}{", "}\r\n{");
                    tb.Text = str;
                }
            }
            else
            {
                RichTextBox _rtb1 = (RichTextBox)(LogicalTreeHelper.FindLogicalNode(MainWindow, "rtb"));
                TextRange tr = new TextRange(_rtb1.Document.ContentStart, _rtb1.Document.ContentEnd);
                string str = XamlUtils.TextRange_GetXml(tr);
                str = str.Replace("><", ">\r\n<");
                tb.Text = str;
            }
        }

        /// <summary>SetupGrid for buttons</summary>
        private void SetupGrid(Grid _grid)
        {
            ColumnDefinition cd1 = new ColumnDefinition();
            _grid.ColumnDefinitions.Add(cd1);
            ColumnDefinition cd2 = new ColumnDefinition();
            _grid.ColumnDefinitions.Add(cd2);
            ColumnDefinition cd3 = new ColumnDefinition();
            _grid.ColumnDefinitions.Add(cd3);
            ColumnDefinition cd4 = new ColumnDefinition();
            _grid.ColumnDefinitions.Add(cd4);
            ColumnDefinition cd5 = new ColumnDefinition();
            _grid.ColumnDefinitions.Add(cd5);

            RowDefinition rd1 = new RowDefinition();
            _grid.RowDefinitions.Add(rd1);

            Grid.SetRow(_getRTF, 0);
            Grid.SetColumn(_getRTF, 1);
            Grid.SetRow(_getXAML, 0);
            Grid.SetColumn(_getXAML, 2);
            Grid.SetRow(_pasteXaml, 0);
            Grid.SetColumn(_pasteXaml, 3);
            Grid.SetRow(_displayForm, 0);
            Grid.SetColumn(_displayForm, 4);

            _grid.Children.Add(_getRTF);
            _grid.Children.Add(_getXAML);
            _grid.Children.Add(_pasteXaml);
            _grid.Children.Add(_displayForm);
        }

        /// <summary>ShowRTFwindow</summary>
        private void ShowRTFwindow()
        {
            _rtfWin = new Window();
            _rtfWin.Background = System.Windows.Media.Brushes.Wheat;
            StackPanel _rtfStackPanel = new StackPanel();
            WindowSettings(_rtfStackPanel, _rtfBox, _rtfWin, "rtfBox");
            _rtfWin.Title = "RTF STREAM";
            _rtfWin.Left = 0;
            _rtfWin.Show();
        }

        /// <summary>ShowXAMLwindow</summary>
        private void ShowXAMLwindow()
        {
            _xamlWin = new Window();
            _xamlWin.Background = System.Windows.Media.Brushes.SkyBlue;
            StackPanel _xamlStackPanel = new StackPanel();
            WindowSettings(_xamlStackPanel, _xamlBox, _xamlWin, "xamlBox");
            _xamlWin.Title = "XAML STREAM";
            _xamlWin.Left = _rtfWin.Width;
            _xamlWin.Show();
        }

        /// <summary>TextPadSettings</summary>
        private void TextPadSettings(TextBox _pad)
        {
            _pad.AcceptsReturn = true;
            _pad.Background = System.Windows.Media.Brushes.Black;
            _pad.Foreground = System.Windows.Media.Brushes.Lime;
            _pad.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        /// <summary>WindowSettings for additional XAML and RTF windows</summary>
        private void WindowSettings(StackPanel _stackPanel, TextBox _tb, Window _win, string _name)
        {
            _tb = new TextBox();
            _tb.AcceptsReturn = true;
            _tb.Background = _win.Background;
            _tb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _tb.Height = 400;
            _tb.Name = _name;
            _tb.FontSize = 15;
            _tb.IsReadOnly = true;
            _tb.TextWrapping = TextWrapping.Wrap;
            _stackPanel.Children.Add(_tb);
            _win.Content = _stackPanel;
            _win.Top = MainWindow.Height;
            _win.Width = MainWindow.Width / 2;
            _win.Height = 450;

        }

        #endregion helpers.

        #region private data.

        private StackPanel _topPanel;
        private RichTextBox _rtb;
        private TextBox _status;
        private Button _getRTF;
        private Button _getXAML;
        private Button _pasteXaml;
        private Window _rtfWin;
        private Window _xamlWin;
        private TextBox _rtfBox = null;
        private TextBox _xamlBox = null;
        private Label _xamlLabel;
        private string _label = "<StackPanel xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \r\n xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" > ";
        private TextBox _xamlTextPad;
        private Label _xamlClose;
        private string _closingTags = "</RichTextBox> \r\n</StackPanel>";
        private TextBox _xamlResources;
        private Label _rtbLabel;
        private string _xamlRTB = "<RichTextBox Name=\"rtb\" ";
        private Winforms.Form _form;
        private Winforms.RichTextBox _winformsRTB;
        private Button _displayForm;

        #endregion private data.
    }

    /// <summary>
    /// Runs some predicate-based rtf-to-xaml converter cases, for regression
    /// coverage.
    /// </summary>
    [Test(0, "XamlRtf", "RtfToXamlPredicates", MethodParameters = "/TestCaseType=RtfToXamlPredicates")]
    [TestOwner("Microsoft"), TestBugs("651"), TestTactics("360")]
    public class RtfToXamlPredicates : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _box = new RichTextBox();
            _currentIndex = 0;
            RunCase();
        }

        private void RunCase()
        {
            if (_currentIndex < s_testData.Length)
            {
                _data = s_testData[_currentIndex];

                Log("Verifying checks for:");
                Log(_data.RtfToLoad);
                Clipboard.SetText(_data.RtfToLoad, TextDataFormat.Rtf);

                QueueDelegate(VerifyRtfToXamlConversion);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        private void VerifyRtfToXamlConversion()
        {
            _box.Document.Blocks.Clear();
            _box.Paste();
            Log("Contents after paste [:" +
               XamlUtils.TextRange_GetXml(new TextRange(_box.Document.ContentStart, _box.Document.ContentEnd)) + "]");
            foreach (DocumentCheckBase check in _data.Checks)
            {
                check.Check(_box.Document);
            }

            _currentIndex++;
            QueueDelegate(RunCase);
        }

        #endregion Main flow.

        #region Private fields.

        private int _currentIndex;
        private RichTextBox _box;
        private TestCaseData _data;

        private static string s_rtfTableString = @"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\froman\fprq2\fcharset0 Times New Roman;}{\f1\fswiss\fcharset0 Arial;}}
{\*\generator Msftedit 5.41.15.1507;}\viewkind4\uc1\trowd\trgaph115\trleft-239\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl115\trpaddr115\trpaddfl3\trpaddfr3
\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx2819\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx5822\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx8879\pard\intbl\f0\fs24 Abc\cell Def\cell Ghi\cell\row\trowd\trgaph115\trleft-239\trbrdrl\brdrs\brdrw10 \trbrdrt\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trpaddl115\trpaddr115\trpaddfl3\trpaddfr3
\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx2819\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx5822\clbrdrl\brdrw10\brdrs\clbrdrt\brdrw10\brdrs\clbrdrr\brdrw10\brdrs\clbrdrb\brdrw10\brdrs \cellx8879\pard\intbl Jkl\cell Mno\cell Pqr\cell\row\pard\f1\fs20\par
}";

        private static string s_rtfListString = @"{\rtf1\ansi\ansicpg1252\uc1\htmautsp\deff0{\fonttbl{\f0\fcharset0 Times New Roman;}}{\colortbl\red0\green0\blue0;\red255\green255\blue255;}
{\*\listtable
{\list\listtemplateid1\listhybrid
{\listlevel\levelnfc23\levelnfcn23\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid5\'01\'b7}{\levelnumbers;}\fi-360\li720\lin720\jclisttab\tx720}
{\listlevel\levelnfc23\levelnfcn23\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid6\'01\'b7}{\levelnumbers;}\fi-360\li1440\lin1440\jclisttab\tx1440}
{\listlevel\levelnfc23\levelnfcn23\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid7\'01\'b7}{\levelnumbers;}\fi-360\li2160\lin2160\jclisttab\tx2160}
{\listlevel\levelnfc0\levelnfcn0\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid8\'02\'03.;}{\levelnumbers\'01;}\fi-360\li2880\lin2880\jclisttab\tx2880}
{\listlevel\levelnfc0\levelnfcn0\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid9\'02\'04.;}{\levelnumbers\'01;}\fi-360\li3600\lin3600\jclisttab\tx3600}
{\listlevel\levelnfc0\levelnfcn0\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid10\'02\'05.;}{\levelnumbers\'01;}\fi-360\li4320\lin4320\jclisttab\tx4320}
{\listlevel\levelnfc0\levelnfcn0\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid11\'02\'06.;}{\levelnumbers\'01;}\fi-360\li5040\lin5040\jclisttab\tx5040}
{\listlevel\levelnfc0\levelnfcn0\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid12\'02\'07.;}{\levelnumbers\'01;}\fi-360\li5760\lin5760\jclisttab\tx5760}
{\listlevel\levelnfc0\levelnfcn0\leveljc0\leveljcn0\levelfollow0\levelstartat1\levelspace0\levelindent0{\leveltext\leveltemplateid13\'02\'08.;}{\levelnumbers\'01;}\fi-360\li6480\lin6480\jclisttab\tx6480}
{\listname ;}\listid1}}
{\*\listoverridetable
{\listoverride\listid1\listoverridecount0\ls1}
}
\loch\hich\dbch\pard\plain\ltrpar\itap0{\f0 {\listtext \'B7\tab}\ls1 ListItem1\li1440\ri0\sa0\sb0\jclisttab\tx1440\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1 ListItem2\li1440\ri0\sa0\sb0\jclisttab\tx1440\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1\ilvl1 ListItem2a\li2880\ri0\sa0\sb0\jclisttab\tx2880\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1\ilvl1 ListItem2b\li2880\ri0\sa0\sb0\jclisttab\tx2880\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1 ListItem3\li1440\ri0\sa0\sb0\jclisttab\tx1440\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1\ilvl1 ListItem3a\li2880\ri0\sa0\sb0\jclisttab\tx2880\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1\ilvl2 ListItem3aa\li4320\ri0\sa0\sb0\jclisttab\tx4320\fi-360\par}
{\f0 {\listtext \'B7\tab}\ls1\ilvl2 ListItem3ab\li4320\ri0\sa0\sb0\jclisttab\tx4320\fi-360\par}
{\fs24 \li0\ri0\sa0\sb0\fi0\par}
}";

        private static TestCaseData[] s_testData = new TestCaseData[] {
            new TestCaseData(@"{\rtf1\b text}", new DocumentCheckBase[] {
                new TextMatchCheck("//Run", "text"),
                new PropertyMatchCheck("//Run", Run.FontWeightProperty, FontWeights.Bold),
            }),
            new TestCaseData(s_rtfTableString, new DocumentCheckBase[] {
                new TextMatchCheck("(.//Table//TableRow[1]//TableCell//Run)[1]", "Abc"),
                new TextMatchCheck("(.//Table//TableRow[1]//TableCell//Run)[2]", "Def"),
                new TextMatchCheck("(.//Table//TableRow[1]//TableCell//Run)[3]", "Ghi"),
                new TextMatchCheck("(.//Table//TableRow[2]//TableCell//Run)[1]", "Jkl"),
                new TextMatchCheck("(.//Table//TableRow[2]//TableCell//Run)[2]", "Mno"),
                new TextMatchCheck("(.//Table//TableRow[2]//TableCell//Run)[3]", "Pqr"),
                new PropertyMatchCheck(".//TableCell", TableCell.BorderThicknessProperty, new Thickness(1d)),
                new PropertyMatchCheck(".//TableCell", TableCell.PaddingProperty, new Thickness(7.67,0,7.67,0)),
            }),
            new TestCaseData(s_rtfListString, new DocumentCheckBase[] {
                new TextMatchCheck("(.//List[1]/ListItem[1]//Run)[1]", "ListItem1"),
                new TextMatchCheck("(.//List[1]/ListItem[2]//Run)[1]", "ListItem2"),
                new TextMatchCheck("(.//List[1]/ListItem[2]/List/ListItem[1]//Run)", "ListItem2a"),
                new TextMatchCheck("(.//List[1]/ListItem[2]/List/ListItem[2]//Run)", "ListItem2b"),
                new TextMatchCheck("(.//List[1]/ListItem[3]//Run)[1]", "ListItem3"),
                new TextMatchCheck("(.//List[1]/ListItem[3]/List/ListItem[1]//Run)[1]", "ListItem3a"),
                new TextMatchCheck("(.//List[1]/ListItem[3]/List//List/ListItem[1]//Run)", "ListItem3aa"),
                new TextMatchCheck("(.//List[1]/ListItem[3]/List//List/ListItem[2]//Run)", "ListItem3ab"),
            }),
        };

        #endregion Private fields.

        #region Helper methods.

        private static void EvaluatePropertyMatch(FlowDocument document, string expression,
            DependencyProperty property, object value)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            TextElement[] elements;
            elements = XPathNavigatorUtils.ListTextElements(document, expression);
            foreach (TextElement element in elements)
            {
                Verifier.VerifyValue(property.Name + " on " + expression,
                    value, element.GetValue(property));
            }
        }

        private static void EvaluateTextMatch(FlowDocument document, string expression, string text)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            bool match;
            TextElement[] elements;
            elements = XPathNavigatorUtils.ListTextElements(document, expression);
            match = false;

            foreach (TextElement element in elements)
            {
                Run run = element as Run;
                if (run != null)
                {
                    Verifier.VerifyText("Run text", text, run.Text, false);
                    match = true;
                }
            }
            if (!match)
            {
                throw new Exception("No Run elements match " + expression);
            }
        }

        #endregion Helper methods.

        #region Inner types.

        abstract class DocumentCheckBase
        {
            public abstract void Check(FlowDocument document);
        }

        class PropertyMatchCheck : DocumentCheckBase
        {
            public PropertyMatchCheck(string expression, DependencyProperty property, object value)
            {
                this.Expression = expression;
                this.Property = property;
                this.Value = value;
            }

            public override void Check(FlowDocument document)
            {
                EvaluatePropertyMatch(document, Expression, Property, Value);
            }

            internal string Expression;
            internal DependencyProperty Property;
            internal object Value;
        }

        class TextMatchCheck : DocumentCheckBase
        {
            public TextMatchCheck(string expression, string text)
            {
                this.Expression = expression;
                this.Text = text;
            }

            public override void Check(FlowDocument document)
            {
                EvaluateTextMatch(document, Expression, Text);
            }

            internal string Expression;
            internal string Text;
        }

        class TestCaseData
        {
            public TestCaseData(string rtfToLoad, DocumentCheckBase[] checks)
            {
                this.RtfToLoad = rtfToLoad;
                this.Checks = checks;
            }

            public string RtfToLoad;
            public DocumentCheckBase[] Checks;
        }

        #endregion Inner types.
    }

    /// <summary>
    /// Verifies that XAML can be copied to RTF, based on an automated
    /// document generation engine.
    /// </summary>
    public class GeneratedXmlRtfTest : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            RichTextBox control;
            // FlowDocumentPageViewer control;
            TextBox logBox;
            DockPanel panel;

            panel = new DockPanel();
            logBox = new TextBox();
            // control = new FlowDocumentPageViewer();
            control = new RichTextBox();
            control.Document = new FlowDocument();
            MainWindow.Content = panel;

            panel.Children.Add(logBox);
            panel.Children.Add(control);

            logBox.FontFamily = new System.Windows.Media.FontFamily("Lucida Console");
            logBox.FontSize = 8d;
            logBox.MinLines = 16;
            logBox.AcceptsReturn = true;
            logBox.IsReadOnly = true;
            DockPanel.SetDock(logBox, Dock.Top);

            _logControl = logBox;

            _wrapper = new UIElementWrapper(control);
            _generator = new SelectedDocumentGenerator(_wrapper);

            // Modify the pruner to make a wide, shallow tree.
            DocumentContentPruner pruner = new DocumentContentPruner(new List<Type>(new Type[] {
                typeof(List), typeof(ListItem), typeof(Paragraph), typeof(Run),
            }));
            pruner.TypeBreadth.Add(typeof(FlowDocument), new List<int>(new int[] { 0, 1, 5 }));
            _generator.ContentPruner = pruner;

            QueueDelegate(NextSelection);
        }

        private void NextSelection()
        {
            if (!_generator.Next())
            {
                Logger.Current.ReportSuccess();
                return;
            }

            QueueDelegate(TestSelection);
        }

        private void TestSelection()
        {
            _logControl.Text = "Iteration " + ++_count + "\r\n" + Describe(_wrapper.Start.Parent as ContentElement);
            Logger.Current.Log(_logControl.Text);
            QueueDelegate(NextSelection);
        }

        private string Describe(ContentElement element)
        {
            StringBuilder sb = new StringBuilder(512);
            Describe(element, 0, sb);
            return sb.ToString();
        }

        private void Describe(ContentElement element, int depth, StringBuilder sb)
        {
            bool lineBroken = false;
            sb.Append(' ', depth);
            sb.Append(element.GetType().Name);
            IEnumerable e = LogicalTreeHelper.GetChildren(element);
            if (e != null)
            {
                foreach (object child in e)
                {
                    if (child is string)
                    {
                        sb.Append(": ");
                        sb.Append((string)child);
                    }
                    else
                    {
                        if (!lineBroken)
                        {
                            sb.AppendLine();
                        }
                        Describe((ContentElement)child, depth + 1, sb);
                    }
                }
            }
        }

        private int _count;
        private UIElementWrapper _wrapper;
        private TextBox _logControl;
        private SelectedDocumentGenerator _generator;
    }
}
