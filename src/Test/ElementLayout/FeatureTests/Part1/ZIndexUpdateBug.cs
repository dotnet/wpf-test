// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Logging;

namespace ElementLayout.FeatureTests.Part1
{
    /// <summary>
    ///Regression coverage: ZIndex is not reflected in a Visual if updated after another element is removed.
    /// </summary>
    [Test(0, "Part1", "ZIndexUpdateBug", Variables = "Area=ElementLayout", Disabled = true )]
    public class ZIndexUpdateBug : CodeTest
    {              
        private Canvas _mainCanvas;
        private TextBlock _textBlockUpdateZIndex;
        private TextBlock _textBlockRemove;       

        public ZIndexUpdateBug() 
        {}

        /// <summary>
        /// Set Window content.
        /// </summary>       
        public override void WindowSetup()
        {
            this.window.Width = this.window.Height = 400;
            this.window.Content = this.TestContent();
        }

        /// <summary>
        /// Creates Window content.
        /// </summary>
        /// <returns>FrameworkElement</returns>
        public override FrameworkElement TestContent()
        {           
            TextBlock textBlockFirst = new TextBlock(new Run("This TextBlock should not be visible after Z order is updated!"));
            textBlockFirst.TextWrapping = TextWrapping.Wrap;
            textBlockFirst.Background = Brushes.Red;
            textBlockFirst.Width = textBlockFirst.Height = 100;
            textBlockFirst.SetValue(Canvas.ZIndexProperty, 1);

            _textBlockUpdateZIndex = new TextBlock();
            _textBlockUpdateZIndex.Background = Brushes.Blue;
            _textBlockUpdateZIndex.Width = _textBlockUpdateZIndex.Height = 110;

            _textBlockRemove = new TextBlock();
            _textBlockRemove.Background = Brushes.Green;
            _textBlockRemove.Width = _textBlockRemove.Height = 100;
            Canvas.SetLeft(_textBlockRemove, 100);
            Canvas.SetTop(_textBlockRemove, 100);
            
            _mainCanvas = new Canvas();
            _mainCanvas.Width = _mainCanvas.Height = 400;
            _mainCanvas.Children.Add(textBlockFirst);
            _mainCanvas.Children.Add(_textBlockUpdateZIndex);
            _mainCanvas.Children.Add(_textBlockRemove);

            return _mainCanvas;
        }

        /// <summary>
        /// Remove one of the TextBlocks and then update the ZIndex of one of the remaining TextBlocks.
        /// </summary>                     
        public override void TestActions()
        {
            _mainCanvas.Children.Remove(_textBlockRemove);
            Canvas.SetZIndex(_textBlockUpdateZIndex, 2);           
        }

        /// <summary>
        /// Verify by using Vscan image comparison
        /// </summary>                
        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this.window, DriverState.TestName, "FeatureTests\\ElementLayout\\Masters\\VSCAN");
            if (!tool.CompareImage())
            {
                TestLog.Current.LogEvidence("Visual verification has failed!");
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }           
        }
    }
}
