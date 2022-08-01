// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DRT
{
    // Regression tests for TextElement SetValue/ClearValue undo support.
    internal class TextElementUndoSuite : DrtTestSuite
    {
        //  Constructors

        #region Constructors

        // Creates a new instance.
        internal TextElementUndoSuite() : base("TextElementUndo")
        {
        }

        #endregion Constructors

        #region Public Methods

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            return new DrtTest[] { new DrtTest(RunTest) };
        }

        #endregion Public Methods

        #region Private Methods

        // Prepares an empty RichTextBox for the visual tree.
        private UIElement CreateTree()
        {
            Canvas canvas = new Canvas();
            _richTextBox = new RichTextBox();
            Canvas.SetLeft(_richTextBox, 5);
            Canvas.SetTop(_richTextBox, 5);
            _richTextBox.Width = 200;
            _richTextBox.Height = 200;

            canvas.Children.Add(_richTextBox);

            return canvas;
        }

        // Entry point for test logic.
        // Creates a new Paragraph and sets and clear property values on an inner Run.
        private void RunTest()
        {
            Run run = new Run("Hello world!");

            ((Paragraph)_richTextBox.Document.Blocks.FirstBlock).Inlines.Add(run);

            DRT.Assert((bool)run.GetValue(Typography.StandardLigaturesProperty) == true);

            run.SetValue(Typography.StandardLigaturesProperty, false);

            _richTextBox.Undo();

            DRT.Assert((bool)run.GetValue(Typography.StandardLigaturesProperty) == true);

            _richTextBox.Redo();

            DRT.Assert((bool)run.GetValue(Typography.StandardLigaturesProperty) == false);

            run.ClearValue(Typography.StandardLigaturesProperty);
            DRT.Assert((bool)run.GetValue(Typography.StandardLigaturesProperty) == true);

            _richTextBox.Undo();

            DRT.Assert((bool)run.GetValue(Typography.StandardLigaturesProperty) == false);
        }

        #endregion Private Methods

        #region Private Fields

        // RichTextBox used by tests.
        private RichTextBox _richTextBox;

        #endregion Private Fields
    }
}