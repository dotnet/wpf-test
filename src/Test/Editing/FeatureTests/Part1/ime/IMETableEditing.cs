// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Validates IME input in combination with editing opeations on a Table
//  inside a RichTextBox

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.TestTypes;
using Test.Uis.Utils;

namespace Test.Uis.TextEditing
{
    [Test(0, "IME", "IMETableEditing_Korean", MethodParameters = "/TestCaseType:IMETableEditing /locale=Korean", Timeout = 120, Keywords = "KoreanIME", Disabled = true)]
    [Test(0, "IME", "IMETableEditing_Japanese", MethodParameters = "/TestCaseType:IMETableEditing /locale=Japanese", Timeout = 120, Keywords = "JapaneseIME", Disabled = true)]
    [Test(0, "IME", "IMETableEditing_ChinesePinyin", MethodParameters = "/TestCaseType:IMETableEditing /locale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME", Disabled = true)]
    [Test(1, "IME", "IMETableEditing_ChineseQuanPin", MethodParameters = "/TestCaseType:IMETableEditing /locale=ChineseQuanPin", Timeout = 120, Keywords = "ChineseQuanPinIME", Disabled = true)]
    [Test(1, "IME", "IMETableEditing_ChineseNewPhonetic", MethodParameters = "/TestCaseType:IMETableEditing /locale=ChineseNewPhonetic", Timeout = 120, Keywords = "ChineseNewPhoneticIME", Disabled = true)]
    public class IMETableEditing : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            if (_rtb == null)
            {
                _rtb = new RichTextBox();
                _rtb.Height = 200;
                _rtb.FontSize = 24;
            }
            if (_testTextBox == null)
            {
                _testTextBox = new TextBox();
                _testTextBox.Height = 100;
                _testTextBox.FontSize = 24;
            }
            if (_panel == null)
            {
                _panel = new StackPanel();
                _panel.Children.Add(_rtb);
                _panel.Children.Add(_testTextBox);
                MainWindow.Content = _panel;
            }

            SetTestVariables();
            QueueDelegate(PerformTestSetup);
        }

        private void PerformTestSetup()
        {
            if (!_isIMESetupDone)
            {
                Log("Load IME keyboard");
                IMEHelper.SetUpIMEKeyboardLayout(_locale, _testTextBox, MainWindow);
                _isIMESetupDone = true;
            }

            ConstructTable();
            QueueDelegate(PerformTestActions);
        }

        private void PerformTestActions()
        {
            // Select complete table
            PerformSelectionAndSendInput(0, 0, 2, 2, 1, 0, 0);
            // Select a first cell in first row
            PerformSelectionAndSendInput(0, 0, 0, 0, 2, 0, 0);
            // Select a last cell in last row
            PerformSelectionAndSendInput(2, 2, 2, 2, 3, 2, 2);
            // Select a something in the middle
            PerformSelectionAndSendInput(1, 1, 1, 1, 4, 1, 1);
            // Select partial table
            PerformSelectionAndSendInput(0, 0, 1, 1, 5, 0, 0);
            // Select partial table
            PerformSelectionAndSendInput(1, 1, 2, 2, 6, 1, 1);
            // Select complete row
            PerformSelectionAndSendInput(0, 0, 0, 2, 7, 0, 0);
            // Select complete column
            PerformSelectionAndSendInput(0, 1, 2, 1, 8, 0, 1);
            // Insert into an empty table cell
            VerifyInsertInEmptyTableCell();
        }

        private void PerformSelectionAndSendInput(int startRow, int startColumn, int endRow, int endColumn, int variation, int effectedRow, int effectedColumn)
        {
            _rtb.Document.Blocks.Clear();
            ConstructTable();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            _rtb.Selection.Select(_rowGroup.Rows[startRow].Cells[startColumn].ContentStart, _rowGroup.Rows[endRow].Cells[endColumn].ContentEnd);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            _rtb.Focus();
            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            VerifyTableContent(variation, effectedRow, effectedColumn);
        }

        private void VerifyTableContent(int variation, int modifiedRow, int modifiedColumn)
        {
            string expectedString = null;
            string actualString = null;

            Verifier.Verify(_rtb.Document.Blocks.Count == 1, "Verify block count is 1 for the rich text box", true);
            Verifier.Verify(_rtb.Document.Blocks.FirstBlock == _table, "Verify table is the first block element in the rich text box", true);

            Verifier.Verify(_rowGroup.Rows.Count == 3, "Verify number of rows = 3", true);

            Verifier.Verify(_rowGroup.Rows[0].Cells.Count == 3, "Verify cell count in row 0 = 3", true);
            Verifier.Verify(_rowGroup.Rows[1].Cells.Count == 3, "Verify cell count in row 1 = 3", true);
            Verifier.Verify(_rowGroup.Rows[2].Cells.Count == 3, "Verify cell count in row 2 = 3", true);

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    actualString = GetTextFromTableCell(row, column);
                    if (row == modifiedRow && column == modifiedColumn)
                    {
                        expectedString = _composedStringByIME;
                    }
                    else
                    {
                        expectedString = _tableContent[row, column];
                    }
                    Verifier.Verify(expectedString == actualString, "Verify Actual content [" + actualString + "] of Table [" + row + "][" + column + "]= " + expectedString, true);
                }
            }
        }

        private void VerifyInsertInEmptyTableCell()
        {
            TableCell cell = new TableCell();
            Table table = new Table();

            TableRowGroup rowGroup = new TableRowGroup();
            TableRow row = new TableRow();

            row.Cells.Add(cell);
            rowGroup.Rows.Add(row);
            table.RowGroups.Add(rowGroup);

            _rtb.Document.Blocks.Add(table);
            _rtb.Selection.Select(cell.ContentStart, cell.ContentStart);

            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            _rtb.Focus();
            KeyboardInput.TypeString(_contentToTypeInIME);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEHelper.CiceroWaitTimeMs);

            Verifier.Verify(_rtb.Document.Blocks.Count == 1, "Verify block count = 1", true);
            Verifier.Verify(cell.Blocks.Count == 1, "Verify cell count = 1", true);
            Paragraph paragraph = (Paragraph)cell.Blocks.FirstBlock;
            Verifier.Verify(paragraph.Inlines.Count == 1, "Verify inline count = 1", true);
            Run run = (Run)paragraph.Inlines.FirstInline;
            Verifier.Verify(run.Text == _composedStringByIME, "Verify run.Text Actual = " + run.Text, true);
        }

        private string GetTextFromTableCell(int row, int column)
        {
            TableRowGroup rowGroup = _table.RowGroups[0];
            TableRow tableRow = rowGroup.Rows[row];
            TableCell tableCell = tableRow.Cells[column];
            Paragraph paragraph = tableCell.Blocks.FirstBlock as Paragraph;
            Run run = paragraph.Inlines.FirstInline as Run;
            return run.Text;
        }

        private void ConstructTable()
        {
            _table = new Table();
            _rowGroup = new TableRowGroup();

            for (int rowCount = 0; rowCount < 3; rowCount++)
            {
                _row = new TableRow();
                for (int columnCount = 0; columnCount < 3; columnCount++)
                {
                    _cell = new TableCell();
                    _cell.Blocks.Add(new Paragraph(new Run(_tableContent[rowCount, columnCount])));
                    _row.Cells.Add(_cell);
                }
                _rowGroup.Rows.Add(_row);
            }

            _table.RowGroups.Add(_rowGroup);
            _rtb.Document.Blocks.Add(_table);
        }

        private void SetTestVariables()
        {
            switch (_locale)
            {
                case IMELocales.Korean:
                    _contentToTypeInIME = "rkskek";
                    _composedStringByIME = "가나다";
                    break;
                case IMELocales.Japanese:
                    _contentToTypeInIME = "hiragana{SPACE}{ENTER}";
                    _composedStringByIME = "ひらがな"; 
                    break;
                case IMELocales.ChinesePinyin:
                    _contentToTypeInIME = "nihao{SPACE}{ENTER}";
                    _composedStringByIME = "你好";
                    break;
                case IMELocales.ChineseQuanPin:
                    _contentToTypeInIME = "nihao{ENTER}";
                    _composedStringByIME = "你好";
                    break;
                case IMELocales.ChineseNewPhonetic:
                    _contentToTypeInIME = "su3cl3a87{ENTER}";
                    _composedStringByIME = "你好嗎";
                    break;
            }
            _rtb.Document.Blocks.Clear();
        }

        #endregion Main flow

        #region Private fields

        // Combinatorial engine variables; set to default values
        private IMELocales _locale = IMELocales.Korean;
        private string[,] _tableContent = new string[,] { { "a", "b", "c" }, { "1", "2", "3" }, { "a1", "b2", "c3" } };

        private StackPanel _panel = null;
        private RichTextBox _rtb = null;
        private TextBox _testTextBox = null; // Used just to set the appropriate IME mode
        private bool _isIMESetupDone = false;
        private Table _table = null;
        private TableRowGroup _rowGroup = null;
        private TableRow _row = null;
        private TableCell _cell = null;

        private string _contentToTypeInIME = string.Empty;
        private string _composedStringByIME = string.Empty;
        private string _existingContent = string.Empty;

        #endregion Private fields
    }
}