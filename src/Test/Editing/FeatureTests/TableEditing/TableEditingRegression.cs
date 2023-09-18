// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
	#region Namespaces.

	using System;
	using Test.Uis.Loggers;
	using Test.Uis.Management;
	using Test.Uis.TestTypes;
	using System.Windows.Controls;
	using Test.Uis.Utils;
	using System.Threading; using System.Windows.Threading;
	using System.Windows.Documents;
	using System.Windows;
	using DrawingPoint = System.Drawing.Point;
	using Point = System.Windows.Point;
	using System.Drawing;
	using Test.Uis.Wrappers;
    using Microsoft.Test.Imaging;
	#endregion Namespaces.

	/// <summary>Regression test for Table Editing</summary>
	[TestOwner("Microsoft"), TestBugs("559, 560, 561, 558, 557"), TestTactics("657")]
	public class TableEditingRegressionTest : RichEditingBase
	{
		#region Regression case - Regression_Bug557: Table Editing: Not able to move caret to the previous line by using {backspace} in a table cell since it will select a whole cell.
		/// <summary>Regression_Bug557: Table Editing: Not able to move caret to the previous line by using {backspace} in a table cell since it will select a whole cell.</summary>
		[TestCase(LocalCaseStatus.Ready, "Regression_Bug557: Table Editing: Not able to move caret to the previous line by using {backspace} in a table cell since it will select a whole cell.")]
		public void Regression_Bug557()
		{
			EnterFunction("Regression_Bug557");
			SetInitValue("<Table><TableRowGroup><TableRow><TableCell BorderThickness=\"1px,1px,1px,1px\"></TableCell></TableRow></TableRowGroup></Table>");
			QueueDelegate(Regression_Bug557_Typing);
			EndFunction();
		}

		void Regression_Bug557_Typing()
		{
			EnterFunction("Regression_Bug557_Typing");
			Rect rect = ElementUtils.GetScreenRelativeRect(TextControlWraper.Element);
			MouseInput.MouseClick((int)(rect.Left + 30), (int)(rect.Top + 20));
			KeyboardInput.TypeString("a{enter}{backspace}");
			QueueDelegate(Regression_Bug557_End);
			EndFunction();
		}

		void Regression_Bug557_End()
		{
			EnterFunction("Regression_Bug557_End");
			FailedIf (TextControlWraper.SelectionInstance.Text.Length != 0, CurrentFunction + " - Failed: we don't expected any selection! Actual Selected text is [" + TextControlWraper.SelectionInstance.Text + "]");
			QueueDelegate(EndTest);
			EndFunction();
		}

		#endregion

		#region Regression case - Regression_Bug558: WCP: Not able to Cut the selection if it contains table(s) only
		/// <summary>Regression_Bug558: WCP: Not able to Cut the selection if it contains table(s) only </summary>
		[TestCase(LocalCaseStatus.Ready, "Regression_Bug558: WCP: Not able to Cut the selection if it contains table(s) only")]
		public void Regression_Bug558()
		{
			EnterFunction("Regression_Bug557");
			SetInitValue("<Table><TableRowGroup><TableRow><TableCell BorderThickness=\"1px,1px,1px,1px\"></TableCell></TableRow></TableRowGroup></Table>");
			QueueDelegate(Regression_Bug558_DoCut);
			EndFunction();
		}

		void Regression_Bug558_DoCut()
		{
			EnterFunction("Regression_Bug558_DoCut");
			string str = XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance);
			FailedIf(Occurency(str, "<Table")<1, CurrentFunction + " - Failed: Selection does not contain a table. Please check the xaml[" + str + "]");
			KeyboardInput.TypeString("^a^x");
			QueueDelegate(Regression_Bug558_End);
			EndFunction();
		}

		void Regression_Bug558_End()
		{
			EnterFuction("Regression_Bug558_End");
			FailedIf(TextControlWraper.SelectionInstance.Text.Length != 0, CurrentFunction + " - Failed: we don't expected any selection! Actual Selected text is [" + TextControlWraper.SelectionInstance.Text + "]");
			QueueDelegate(EndTest);
			EndFunction();
		}

		#endregion

		#region Regression case - Regression_Bug559: Table Editing: won't able to set caret outside of table in lexicon
		/// <summary>Regression_Bug559: Table Editing: won't able to set caret outside of table in lexicon </summary>
		[TestCase(LocalCaseStatus.Ready, "Table Editing: won't able to set caret outside of table in lexicon")]
		public void Regression_Bug559()
		{
			EnterFunction("Regression_Bug559");
			SetInitValue("<Table><TableRowGroup><TableRow><TableCell BorderThickness=\"1px,1px,1px,1px\"></TableCell></TableRow></TableRowGroup></Table>");
			QueueDelegate(Regression_Bug559_SetCaretInTableAndMoveItOut);
		}

		void Regression_Bug559_SetCaretInTableAndMoveItOut()
		{
			EnterFunction("Regression_Bug559_SetCaretInTableAndMoveItOut");
			string str = XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance);
			Rect rect = ElementUtils.GetScreenRelativeRect(TextControlWraper.Element);
			MyLogger.Log(CurrentFunction + ": Set caret in the table...");
			MouseInput.MouseClick((int)(rect.Left + 30), (int)(rect.Top + 20));
			KeyboardInput.TypeString("{RIGHT 5}abc{backspace 10}");
			QueueDelegate(Regression_Bug558_End);
			EndFunction();
		}

		void Regression_Bug559_End()
		{
			EnterFunction("Regression_Bug559_End");
			string str = XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance);
			MyLogger.Log(CurrentFunction + ": when caret is inside table, selection will not return xaml when it is not fully cross! This may not be the case when table editing feature is mature.");
			FailedIf(Occurency(str, "<Table")>0, CurrentFunction + " - Failed: We don't expected table in xaml. Please check the xaml[" + str + "]");
			QueueDelegate(EndTest);
			EndFunction();
		}

		#endregion	

		#region Regression case - Regression_Bug560: Table Editing: an exception is thrown when try to delete a table using {DELETE} when caret is just set before the table.
		/// <summary>Regression_Bug560: Table Editing: an exception is thrown when try to delete a table using {DELETE} when caret is just set before the table.</summary>
		[TestCase(LocalCaseStatus.Ready, "Regression_Bug560: Table Editing: an exception is thrown when try to delete a table using {DELETE} when caret is just set before the table.")]
		public void Regression_Bug560()
		{
			EnterFunction("Regression_Bug560");
			SetInitValue("<Paragraph>a</Paragraph><Table><TableRowGroup><TableRow><TableCell BorderThickness=\"1px,1px,1px,1px\"></TableCell></TableRow></TableRowGroup></Table><Paragraph>b</Paragraph>");
			QueueDelegate(Regression_Bug560_SetCaret);
			EndFunction();
		}

		void Regression_Bug560_SetCaret()
		{
			EnterFunction("Regression_Bug560_SetCaret");
			KeyboardInput.TypeString("{Up 3}{home}+{RIGHT}");
			QueueDelegate(Regression_Bug560_DoDelete);
			EndFunction();
		}

		void Regression_Bug560_DoDelete()
		{
			EnterFunction("Regression_Bug560_DoDelete");
			string str = TextControlWraper.SelectionInstance.Text;
			FailedIf(str !="a", CurrentFunction + " - Failed: wrong text is selected. Expected[a]  Actual:[" + str + "]");
			KeyboardInput.TypeString("{RIGHT}{DELETE 10}");
			MyLogger.Log(CurrentFunction + ": don't expected crash when pressing {delete} if caret is before the table");
			QueueDelegate(EndTest);
		}

		#endregion 
		
		#region Regression case - Regression_Bug561: WCP: When drag and drop multiple lines of text from a table, Border(s) are created after the text is dropped.
		/// <summary>Regression_Bug561: WCP: When drag and drop multiple lines of text from a table, Border(s) are created after the text is dropped.</summary>
		[TestCase(LocalCaseStatus.Ready, "Regression_Bug561: WCP: When drag and drop multiple lines of text from a table, Border(s) are created after the text is dropped.")]
		public void Regression_Bug561()
		{
			EnterFunction("Regression_Bug561");
			SetInitValue("<Paragraph></Paragraph><Table><TableRowGroup><TableRow><TableCell BorderThickness=\"1px,1px,1px,1px\"><Paragraph>acd</Paragraph><Paragraph>efg</Paragraph><Paragraph>hijklmnopqrstuvwxyz</Paragraph></TableCell></TableRow></TableRowGroup></Table><Paragraph>b</Paragraph>");
			TextPointer tp1 = TextControlWraper.Start;
			tp1 = tp1.GetPositionAtOffset(9);
			TextPointer tp2 = TextControlWraper.Start;
			tp2 = tp2.GetPositionAtOffset(25);
			//we are going to select between d to O
			TextControlWraper.SelectionInstance.Select(tp1, tp2);
			QueueDelegate(Regression_Bug561_DoDragDrop);
			EndFunction();
		}

		void Regression_Bug561_DoDragDrop()
		{
			//not we only move 20 here so that mouse will be in the selection.
			Rect rect = TextControlWraper.GetGlobalCharacterRect(TextControlWraper.Start, 20);
			//need to disable InputMonitorManager
			Test.Uis.Utils.InputMonitorManager.Current.IsEnabled = false;
			MouseInput.MouseDragPressed(new Point(rect.Left, rect.Top + 5), new Point(rect.Left, rect.Top + 200));
			QueueDelegate(Regression_Bug561_Done);
			EndFunction();
		}

		void Regression_Bug561_Done()
		{
			EnterFunction("Regression_Bug561_Done");
			string xaml = XamlUtils.TextRange_GetXml(TextControlWraper.SelectionInstance);
			string text = TextControlWraper.SelectionInstance.Text;
			FailedIf(Occurency(xaml.ToLower(), "border") != 0, CurrentFunction + " - Failed: we don't expect border in the selection xaml. Please check the xaml[" + xaml + "]");
			FailedIf(text != "d\r\nefg\r\nhijklmno\r\n", CurrentFunction + " - Failed: wrong selected text. Expected[d\r\nefg\r\nhijklmno]  Actual[" + text + "]");
			EndFunction();
			QueueDelegate(EndTest);
			EndFunction();
		}
		#endregion 
	}
}
