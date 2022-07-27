// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DRT
{
    internal class myTextChange
    {
        #region Constructors

        internal myTextChange()
        {
        }

        #endregion Constructors

        //  Public Members
    
        #region Public Members

        /// <summary>
        /// 0-based character offset for this change
        /// </summary>
        public int Offset
        {
            get
            {
                return _offset;
            }
            internal set
            {
                _offset = value;
            }
        }

        /// <summary>
        /// Number of characters added
        /// </summary>
        public int AddedLength
        {
            get
            {
                return _addedLength;
            }
            internal set
            {
                _addedLength = value;
            }
        }

        /// <summary>
        /// Number of characters removed
        /// </summary>
        public int RemovedLength
        {
            get
            {
                return _removedLength;
            }
            internal set
            {
                _removedLength = value;
            }
        }

        #endregion Public Members

        //  Private Fields
        #region Private Fields

        private int _offset;
        private int _addedLength;
        private int _removedLength;

        #endregion Private Fields
    }

    // Regression tests for the TextChanged event.
    internal class TextChangedSuite : DrtTestSuite
    {
        //  Constructors
     
        #region Constructors

        // Creates a new instance.
        internal TextChangedSuite() : base("TextChanged")
        {
        }

        #endregion Constructors

        //  Public Methods
     
        #region Public Methods

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            _rtb = new RichTextBox();
            _rtb.TextChanged += new TextChangedEventHandler(OnTextChanged);

            return new DrtTest[] { new DrtTest(Change1),
                                    new DrtTest(Change2),
                                    new DrtTest(Change3),
                                    new DrtTest(Change4),
                                    new DrtTest(Change5),
                                    new DrtTest(Change6),
                                    new DrtTest(Change7),
                                    new DrtTest(Change8),
                                    new DrtTest(Change9),
                                    new DrtTest(Change10),
                                    new DrtTest(Change11),
                                    new DrtTest(Change12),
                                    new DrtTest(Change13),
                                    new DrtTest(Change14),
                                    new DrtTest(Change15),
                                    new DrtTest(Change16),
                                    new DrtTest(Change17),
                                 };
        }

        #endregion Public Methods

        //  Private Methods
     
        #region Private Methods

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_expectedChanges == null)
            {
                return;
            }
            
            DRT.Assert(e.Changes.Count == _expectedChanges.Count, "Incorrect number of changes: expected " + _expectedChanges.Count + ", got " + e.Changes.Count);
            int i = 0;
            foreach (TextChange change in e.Changes)
            {
                DRT.Assert(change.Offset == _expectedChanges[i].Offset, "Offsets differ in change #" + i + ": expected " + _expectedChanges[i].Offset + ", got " + change.Offset);
                DRT.Assert(change.AddedLength == _expectedChanges[i].AddedLength, "AddedLengths differ in change #" + i + ": expected " + _expectedChanges[i].AddedLength + ", got " + change.AddedLength);
                DRT.Assert(change.RemovedLength == _expectedChanges[i].RemovedLength, "RemovedLengths differ in change #" + i + ": expected " + _expectedChanges[i].RemovedLength + ", got " + change.RemovedLength);
                i++;
            }
        }

        // Add some text
        public void Change1()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 5;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            ptr.InsertTextInRun("best ");
            _rtb.EndChange();
        }

        // Delete some text
        public void Change2()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 26;
            change.RemovedLength = 5;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(26, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(5, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Replace some text
        public void Change3()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 6;
            change.RemovedLength = 4;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.Text = "buffet";
            _rtb.EndChange();
        }

        // Non-overlapping changes (replacements)
        public void Change4()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(2);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 6;
            change.RemovedLength = 4;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 28;
            change.AddedLength = 6;
            change.RemovedLength = 4;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.Text = "buffet";
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(28, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.Text = "hungry";
            _rtb.EndChange();
        }

        // Insert at the same location as previous change
        public void Change5()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 12;
            change.RemovedLength = 4;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.Text = "buffet";
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            ptr.InsertTextInRun("grand ");
            _rtb.EndChange();
        }

        // Overlapping changes
        public void Change6()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 22;
            change.RemovedLength = 8;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.Text = "buffet";
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(17, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(6, LogicalDirection.Forward));
            range.Text = "alo wing denied to";
            _rtb.EndChange();
        }

        // Deletion adjoins later change
        public void Change7()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 6;
            change.RemovedLength = 12;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(5, LogicalDirection.Forward));
            range.Text = "";
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(6, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(7, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Deletion spans earlier change
        public void Change8()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 2;
            change.RemovedLength = 10;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            ptr.InsertTextInRun("best ");
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(15, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(13, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Deletion spans later change
        public void Change9()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 6;
            change.AddedLength = 0;
            change.RemovedLength = 20;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(5, LogicalDirection.Forward));
            range.Text = "";
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(6, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(15, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Deletion spans earlier and later change
        public void Change10()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 6;
            change.RemovedLength = 9;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            ptr.InsertTextInRun("best ");
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(27, LogicalDirection.Forward);
            ptr.InsertTextInRun("finding ");
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(15, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(16, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Deletion adjoins earlier addition
        public void Change11()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(1);
            myTextChange change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 6;
            change.RemovedLength = 5;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            ptr.InsertTextInRun("party ");
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(5, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Property change
        public void Change12()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(4);
            myTextChange change = new myTextChange();
            change.Offset = 1;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 19;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 39;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            _rtb.EndChange();
        }

        // Same thing, smaller text
        public void Change13()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Testing";
            _expectedChanges = new List<myTextChange>(4);
            myTextChange change = new myTextChange();
            change.Offset = 1;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 4;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 9;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(4, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(3, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            _rtb.EndChange();
        }

        // Overlapping property changes
        public void Change14()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(4);
            myTextChange change = new myTextChange();
            change.Offset = 1;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 19;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 39;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(15, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            _rtb.EndChange();
        }

        // Two separate property changes
        public void Change15()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            _expectedChanges = new List<myTextChange>(6);
            myTextChange change = new myTextChange();
            change.Offset = 1;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 13;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 19;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 26;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 32;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 43;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(26, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            _rtb.EndChange();
        }

        // Deleting across a text element boundary
        public void Change16()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Now is the time for all good men.";
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(13, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            _expectedChanges = new List<myTextChange>(2);
            myTextChange change = new myTextChange();
            change.Offset = 9;
            change.RemovedLength = 4;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 11;
            change.RemovedLength = 2;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(9, LogicalDirection.Forward);
            range = new TextRange(ptr, ptr.GetPositionAtOffset(8, LogicalDirection.Forward));
            range.Text = "";
            _rtb.EndChange();
        }

        // Format text, then insert text at the same start position, then format that text
        public void Change17()
        {
            _expectedChanges = null;
            TextRange all = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            all.Text = "Testing";
            _expectedChanges = new List<myTextChange>(4);
            myTextChange change = new myTextChange();
            change.Offset = 1;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 4;
            change.AddedLength = 12;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 19;
            change.AddedLength = 2;
            _expectedChanges.Add(change);
            change = new myTextChange();
            change.Offset = 23;
            change.AddedLength = 1;
            change.RemovedLength = 1;
            _expectedChanges.Add(change);
            _rtb.BeginChange();
            TextPointer ptr = _rtb.Document.ContentStart.GetPositionAtOffset(4, LogicalDirection.Forward);
            TextRange range = new TextRange(ptr, ptr.GetPositionAtOffset(3, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            // Position between the end run and bold start
            ptr = _rtb.Document.ContentStart.GetPositionAtOffset(5, LogicalDirection.Forward);
            ptr.InsertTextInRun("Italic");
            range = new TextRange(ptr.GetPositionAtOffset(2, LogicalDirection.Forward), ptr.GetPositionAtOffset(4, LogicalDirection.Forward));
            range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);

            _rtb.EndChange();
        }

        #endregion Private Methods

        private RichTextBox _rtb;
        private List<myTextChange> _expectedChanges;
    }
}