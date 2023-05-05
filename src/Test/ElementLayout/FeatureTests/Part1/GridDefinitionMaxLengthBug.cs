// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using ElementLayout.TestLibrary;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Layout;

namespace ElementLayout.FeatureTests.Part1
{
    /// <summary>
    /// Regression coverage: Auto sized Grid Rows/Columns max Height/Width are not observed when the initial content is larger than the max specification.
    /// </summary>
    [Test(0, "Part1", "GridDefinitionMaxLengthBug", Variables = "Area=ElementLayout")]
    public class GridDefinitionMaxLengthBug : CodeTest
    {       
        private ColumnDefinition _testColumnDefinition;
        private RowDefinition _testRowDefinition;
        private static double s_maxLengthValue = 100;
        private static double s_setLengthValue = 150;

        public GridDefinitionMaxLengthBug()
        { }

        /// <summary>
        /// Set Window content.
        /// </summary>       
        public override void WindowSetup()
        {
            this.window.Content = this.TestContent();                                                               
        }

        /// <summary>
        /// Creates a couple of StackPanels with width/height greater than their containing auto sized Row/Column Definition's max length.
        /// </summary>
        /// <returns>FrameworkElement</returns>
        public override FrameworkElement TestContent()
        {
            TextBlock topTextBlock = new TextBlock(new Run("Top TextBlock."));
            topTextBlock.Background = Brushes.LightBlue;
            topTextBlock.Height = s_setLengthValue;

            TextBlock bottomTextBlock = new TextBlock(new Run("Bottom TextBlock.  This TextBlock should not be visible."));
            bottomTextBlock.Background = Brushes.LightGreen;
            bottomTextBlock.Height = s_setLengthValue;

            StackPanel heightStackPanel = new StackPanel();
            heightStackPanel.Children.Add(topTextBlock);
            heightStackPanel.Children.Add(bottomTextBlock);          

            TextBlock leftTextBlock = new TextBlock(new Run("Left TextBlock."));
            leftTextBlock.Background = Brushes.LightYellow;
            leftTextBlock.Width = s_setLengthValue;

            TextBlock rightTextBlock = new TextBlock(new Run("Right TextBlock. This TextBlock should not be visible."));
            rightTextBlock.Background = Brushes.Orange;
            rightTextBlock.Width = s_setLengthValue;

            StackPanel widthStackPanel = new StackPanel();
            widthStackPanel.Orientation = Orientation.Horizontal;
            widthStackPanel.Children.Add(leftTextBlock);
            widthStackPanel.Children.Add(rightTextBlock);           
            widthStackPanel.SetValue(Grid.RowProperty, 1);
            widthStackPanel.SetValue(Grid.ColumnProperty, 1);
            
            _testColumnDefinition = new ColumnDefinition();
            _testColumnDefinition.Width = new GridLength(1, GridUnitType.Auto);
            _testColumnDefinition.MaxWidth = s_maxLengthValue;
            ColumnDefinition col2 = new ColumnDefinition();

            _testRowDefinition = new RowDefinition();
            _testRowDefinition.Height = new GridLength(1, GridUnitType.Auto);
            _testRowDefinition.MaxHeight = s_maxLengthValue;
            RowDefinition row2 = new RowDefinition();

            Grid parentGrid = new Grid();           
            parentGrid.RowDefinitions.Add(_testRowDefinition);
            parentGrid.RowDefinitions.Add(row2);
            parentGrid.ColumnDefinitions.Add(col2);
            parentGrid.ColumnDefinitions.Add(_testColumnDefinition);
            parentGrid.Children.Add(heightStackPanel);
            parentGrid.Children.Add(widthStackPanel);
                       
            return parentGrid;
        }

        /// <summary>
        /// Verifies that a Row/Column Definition's actual height/width is == the max length even though the definition is 
        /// auto sized and the child content is larger than this size.
        /// </summary>        
        public override void TestVerify()
        {           
            if (_testColumnDefinition.ActualWidth == s_maxLengthValue && _testRowDefinition.ActualHeight == s_maxLengthValue)
            {
                Helpers.Log("Actual lengths were as expected.  Test passes.");
                this.Result = true;
            }
            else
            {                
                Helpers.Log(string.Format("Row/Column definition is auto sized with a max length of {0}, ActualWidth/ActualHeight should == {1}", s_maxLengthValue, s_maxLengthValue));
                Helpers.Log(string.Format("RowDefinition.ActualHeight = {0}; ColumnDefinition.ActualWidth = {1}", _testRowDefinition.ActualHeight, _testColumnDefinition.ActualWidth));
                this.Result = false;
            }
        }
    }
}
