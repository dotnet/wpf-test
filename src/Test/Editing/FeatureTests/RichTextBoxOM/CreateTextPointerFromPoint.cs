// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Display;
    
    #endregion Namespaces.

    /// <summary>
    /// This class intend to test RichTextBox.GetPositionFromPoint().
    /// </summary>
    [Test(0, "RichTextBox", "CreateTextPointerFromPoint", MethodParameters = "/TestCaseType=CreateTextPointerFromPoint")]
    [TestOwner("Microsoft"), TestTactics("687"), TestWorkItem("146"), TestLastUpdatedOn("June 14, 2006")]
    public class CreateTextPointerFromPoint : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            if (Monitor.Dpi.x == 120)
            {
                Logger.Current.ReportResult(true, "Case is 96 DPI dependent");
            }
            else
            {
                _rtb = new RichTextBox();
                _rtb.Width = _rtb.Height = 600;
                _controlWrapper = new UIElementWrapper(_rtb);
                _rtb.FontSize = 40;
                _rtb.Document.Blocks.Clear();
                string _str = "Tb";
                AddParas(_rtb, _str);
                _str = "Tde";
                AddParas(_rtb, _str);
                TestElement = _rtb;
                QueueDelegate(DoFocus);
            }
        }

        private void DoFocus()
        {
            _rtb.Focus();
            PopulatePointArray();

            _count = 0;
            QueueDelegate(ExecuteTrigger);
        }

        private void ExecuteTrigger()
        {
            _actualTp = _rtb.GetPositionFromPoint(_points[_count], _snapToText);
           // MouseInput.MouseMove(Points[_count]);    
            QueueDelegate(CheckValidity);
        }

        private void CheckValidity()
        {
            VerifyPointValidity(_actualTp, _count);
            _count++;
            if (_count < _points.Length)
            {
                QueueDelegate(ExecuteTrigger);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private void VerifyPointValidity(TextPointer _actualTp, int _index)
        {
            Log( "Point # ["+_index.ToString() + "] X["+ _points[_index].X.ToString() +"] Y ["+
                _points[_index].Y.ToString()+ "] SnapToText ["+ _snapToText.ToString()+"]");
            TextPointer _expectedPointer = null;
            if (_index < 16)
            {
                _expectedPointer = (_rtb.Document.Blocks.LastBlock as Paragraph).ContentEnd;
            }
            if (_index < 12)
            {
                _expectedPointer = (_rtb.Document.Blocks.LastBlock as Paragraph).ContentStart;
            }
            if (_index < 8)
            {
                _expectedPointer =(_rtb.Document.Blocks.LastBlock as Paragraph).ContentStart;
            }
            if (_index < 4)
            {
                _expectedPointer =  (_rtb.Document.Blocks.FirstBlock as Paragraph).ContentStart;
            }
            if (_snapToText == false)
            {
                //only the 3 and 7 ar points on the bounding rectangle of chars
                if ((_index % 4 == 3) && (_index  <8))
                {
                    VerifyPointerLocations(ref _actualTp, ref _expectedPointer, _index);
                }
                else
                {
                    Verifier.Verify(_actualTp == null, "Expected the Pointer to be null when SnapToText = false and the point is not within the bounding rectangle", true);
                }
            }
            else
            {
                VerifyPointerLocations(ref _actualTp, ref _expectedPointer, _index);
            }
        }

        private void VerifyPointerLocations(ref TextPointer _actualTp, ref TextPointer _expectedPointer, int _index)
        {
            LogicalDirection _direction = (_index < 12) ? LogicalDirection.Forward : LogicalDirection.Backward;
            while (_expectedPointer.IsAtInsertionPosition == false)
            {
                _expectedPointer = _expectedPointer.GetNextInsertionPosition(_direction);
            }

            while (_actualTp.IsAtInsertionPosition == false)
            {
                _actualTp = _actualTp.GetNextInsertionPosition(_direction);
            }
            Log("Actual Pointer Run [" + _actualTp.GetTextInRun(_direction) + "] Expected Pointer Run ["+
            _expectedPointer.GetTextInRun(_direction) + "]");

            Verifier.Verify(_expectedPointer.GetTextInRun(_direction).Contains(_actualTp.GetTextInRun(_direction)), "string should be equal", true);
        }

        private void PopulatePointArray()
        {
                Paragraph _p = _rtb.Document.Blocks.FirstBlock as Paragraph;
                _rtb.CaretPosition = _p.ContentStart;
                Point _point = _controlWrapper.GetElementRelativeCharacterRect(_rtb.CaretPosition, 0, LogicalDirection.Forward).TopLeft;
                _points = new Point[16];
                int _index = 0;
                //These points correspond to first char on first para
                InsertPoints(ref _point, ref _index);

                _p = _rtb.Document.Blocks.LastBlock as Paragraph;
                _rtb.CaretPosition = _p.ContentStart;
                _point = _controlWrapper.GetElementRelativeCharacterRect(_rtb.CaretPosition, 0, LogicalDirection.Forward).TopLeft;
                //These points correspond to first char on second/last para
                InsertPoints(ref _point, ref _index);

                //These points correspond to empty space to the bottom of first char of second/last para
                _point.Y = _point.Y + 100;
                InsertPoints(ref _point, ref _index);

                //These points correspond to empty space to right of second/last para
                _point.Y = _point.Y - 100;
                _point.X = _point.X + 100;
                InsertPoints(ref _point, ref _index);                
        }

        private void InsertPoints(ref Point _point, ref int _index)
        {
            double _incry = _controlWrapper.GetGlobalCharacterRect(_rtb.CaretPosition).Height * .5;
            double _incrx = _controlWrapper.GetGlobalCharacterRect(_rtb.CaretPosition).Width *.5;
            Log("Y increment added/subtracted [" + _incry.ToString());
            Log("X increment added/subtracted [" + _incrx.ToString());
            for (int i = 0; i < _pointChanges.Length; i++)
            {
                Point _p1 = new Point();
                _p1.X = ((i == 1) || (i == 3)) ? (_point.X + _incrx) : (_point.X - _incrx);
                _p1.Y = ((i == 2) || (i == 3)) ? (_point.Y + _incry) : (_point.Y - _incry);
                _points[_index] = _p1;
                _index++;
            }
        }

        private static void AddParas(RichTextBox rb, string str)
        {
            Paragraph p = new Paragraph();
            p.Padding = new Thickness(30);
            p.Margin = new Thickness(0);
            p.Inlines.Add(new Run(str));
            rb.Document.Blocks.Add(p);
        }

        #region data.

        private Point[] _points;
        private int[] _pointChanges = { 5, 5, 5, 5 };

        private UIElementWrapper _controlWrapper = null;
        private bool _snapToText = false;
        private int _count = 0;
        private TextPointer  _actualTp = null;
        private RichTextBox _rtb = null;

        #endregion data.
    }
}
