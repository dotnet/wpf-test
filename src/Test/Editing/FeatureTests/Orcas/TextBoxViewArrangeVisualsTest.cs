// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  This file contains TextBoxView regression coverage

using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using Microsoft.Test.Diagnostics;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Test.Uis.TextEditing
{
    /// <summary>
    /// Test coverage for following bugs which have exclusive repros.
    /// Regression_Bug104: InvariantAssert firing in TextBoxView.ArrangeVisuals()    
    /// </summary>
    [Test(0, "TextBox", "TextBoxViewArrangeVisualsTest", MethodParameters = "/TestCaseType=TextBoxViewArrangeVisualsTest")]
    [TestOwner("Microsoft"), TestBugs("104"), TestLastUpdatedOn("June 18th, 2007")]
    public class TextBoxViewArrangeVisualsTest : CustomTestCase
    {        
        #region Private fields        

        private static readonly string s_textBoxReproXaml =
    @"<Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:local='clr-namespace:Test.Uis.TextEditing;assembly=EditingOrcasTest'
        Height='300' Width='300'>
    <Canvas.Resources>
        <Style TargetType='{x:Type local:CustomTextBox}'>
            <Setter Property='Template'>
            <Setter.Value>
            <ControlTemplate TargetType='local:CustomTextBox'>
                <TextBox Text='{Binding Path=Text, RelativeSource={RelativeSource TemplatedParent}}'
                    TextWrapping='Wrap' FontFamily='Arial' FontSize='16'/>
            </ControlTemplate>
            </Setter.Value>
            </Setter>
        </Style>
    </Canvas.Resources>
    <Slider Width='150' Name='WrapWidthSlider' Minimum='50' Maximum='400' Value='250' />
    <local:CustomTextBox Canvas.Top='100' Canvas.Left='30'
        WrapWidth='{Binding ElementName=WrapWidthSlider, Path=Value}'
        Text='The quick brown fox jumps over the lazy dog.        .'/>
	</Canvas>";

        private double _testWidthValue = 75;  //Invariant.Assert fires at this value or below 

        #endregion Private fields

        /// <summary>Test case starts here.</summary>
        public override void RunTestCase()
        {            
            Canvas canvas = (Canvas)XamlUtils.ParseToObject(s_textBoxReproXaml);            
            MainWindow.Content = canvas;
            
            QueueDelegate(ChangeWidth);
        }

        private void ChangeWidth()
        {
            Canvas canvas = (Canvas)MainWindow.Content;
            Slider slider = (Slider)canvas.Children[0];
            slider.Value = _testWidthValue;

            //Wait for the render to happen.
            DispatcherHelper.DoEvents();

            //Regression_Bug104 should crash at this point by throwing an Invariant.Assert
            CustomTextBox customTextBox = (CustomTextBox)canvas.Children[1];
            Verifier.Verify(customTextBox.WrapWidth == _testWidthValue,
                "Verifying that width of the TextBox changed. Expected [" + _testWidthValue + "] Actual [" + customTextBox.WrapWidth + "]", true);
            
            Logger.Current.ReportSuccess();
        }
    }
}