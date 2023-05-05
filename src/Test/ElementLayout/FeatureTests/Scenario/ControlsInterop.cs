// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Collections.Generic;

namespace ElementLayout.FeatureTests.Scenario
{
    [Test(3, "Interop.Controls", "BorderAndButton", Variables="Area=ElementLayout")]
    public class BorderAndButton : PropertyDumpTest
    {
        public BorderAndButton() { this.DumpTest("Border_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndCheckbox", Variables="Area=ElementLayout")]
    public class BorderAndCheckbox : PropertyDumpTest
    {
        public BorderAndCheckbox() { this.DumpTest("Border_Checkbox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndComboBox", Variables="Area=ElementLayout")]
    public class BorderAndComboBox : PropertyDumpTest
    {
        public BorderAndComboBox() { this.DumpTest("Border_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndExpander", Variables="Area=ElementLayout")]
    public class BorderAndExpander : PropertyDumpTest
    {
        public BorderAndExpander() { this.DumpTest("Border_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndHorizontalScrollbar", Variables="Area=ElementLayout")]
    public class BorderAndHorizontalScrollbar : PropertyDumpTest
    {
        public BorderAndHorizontalScrollbar() { this.DumpTest("Border_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndLabel", Variables="Area=ElementLayout")]
    public class BorderAndLabel : PropertyDumpTest
    {
        public BorderAndLabel() { this.DumpTest("Border_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndListBox", Variables="Area=ElementLayout")]
    public class BorderAndListBox : PropertyDumpTest
    {
        public BorderAndListBox() { this.DumpTest("Border_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndMenu", Variables="Area=ElementLayout")]
    public class BorderAndMenu : PropertyDumpTest
    {
        public BorderAndMenu() { this.DumpTest("Border_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndProgressBar", Variables="Area=ElementLayout")]
    public class BorderAndProgressBar : PropertyDumpTest
    {
        public BorderAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {               
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;               
            }            
            this.DumpTest("Border_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml"));                
        }
    }

    [Test(3, "Interop.Controls", "BorderAndRadioButton", Variables="Area=ElementLayout")]
    public class BorderAndRadioButton : PropertyDumpTest
    {
        public BorderAndRadioButton() { this.DumpTest("Border_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndTabControl", Variables="Area=ElementLayout")]
    public class BorderAndTabControl : PropertyDumpTest
    {
        public BorderAndTabControl() { this.DumpTest("Border_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndThumb", Variables="Area=ElementLayout")]
    public class BorderAndThumb : PropertyDumpTest
    {
        public BorderAndThumb() { this.DumpTest("Border_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "BorderAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class BorderAndVerticalScrollBar : PropertyDumpTest
    {
        public BorderAndVerticalScrollBar() { this.DumpTest("Border_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndButton", Variables="Area=ElementLayout")]
    public class CanvasAndButton : PropertyDumpTest
    {
        public CanvasAndButton() { this.DumpTest("Canvas_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndCheckBox", Variables="Area=ElementLayout")]
    public class CanvasAndCheckBox : PropertyDumpTest
    {
        public CanvasAndCheckBox() { this.DumpTest("Canvas_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndComboBox", Variables="Area=ElementLayout")]
    public class CanvasAndComboBox : PropertyDumpTest
    {
        public CanvasAndComboBox() { this.DumpTest("Canvas_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndExpander", Variables="Area=ElementLayout")]
    public class CanvasAndExpander : PropertyDumpTest
    {
        public CanvasAndExpander() { this.DumpTest("Canvas_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class CanvasAndHorizontalScrollBar : PropertyDumpTest
    {
        public CanvasAndHorizontalScrollBar() { this.DumpTest("Canvas_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndLabel", Variables="Area=ElementLayout")]
    public class CanvasAndLabel : PropertyDumpTest
    {
        public CanvasAndLabel() { this.DumpTest("Canvas_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndListBox", Variables="Area=ElementLayout")]
    public class CanvasAndListBox : PropertyDumpTest
    {
        public CanvasAndListBox() { this.DumpTest("Canvas_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndMenu", Variables="Area=ElementLayout")]
    public class CanvasAndMenu : PropertyDumpTest
    {
        public CanvasAndMenu() { this.DumpTest("Canvas_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndProgressBar", Variables="Area=ElementLayout")]
    public class CanvasAndProgressBar : PropertyDumpTest
    {
        public CanvasAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("Canvas_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "CanvasAndRadioButton", Variables="Area=ElementLayout")]
    public class CanvasAndRadioButton : PropertyDumpTest
    {
        public CanvasAndRadioButton() { this.DumpTest("Canvas_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndTabControl", Variables="Area=ElementLayout")]
    public class CanvasAndTabControl : PropertyDumpTest
    {
        public CanvasAndTabControl() { this.DumpTest("Canvas_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndThumb", Variables="Area=ElementLayout")]
    public class CanvasAndThumb : PropertyDumpTest
    {
        public CanvasAndThumb() { this.DumpTest("Canvas_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "CanvasAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class CanvasAndVerticalScrollBar : PropertyDumpTest
    {
        public CanvasAndVerticalScrollBar() { this.DumpTest("Canvas_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndButton", Variables="Area=ElementLayout")]
    public class DecoratorAndButton : PropertyDumpTest
    {
        public DecoratorAndButton() { this.DumpTest("Decorator_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndCheckbox", Variables="Area=ElementLayout")]
    public class DecoratorAndCheckbox : PropertyDumpTest
    {
        public DecoratorAndCheckbox() { this.DumpTest("Decorator_Checkbox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndComboBox", Variables="Area=ElementLayout")]
    public class DecoratorAndComboBox : PropertyDumpTest
    {
        public DecoratorAndComboBox() { this.DumpTest("Decorator_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndExpander", Variables="Area=ElementLayout")]
    public class DecoratorAndExpander : PropertyDumpTest
    {
        public DecoratorAndExpander() { this.DumpTest("Decorator_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndHorizontalScrollbar", Variables="Area=ElementLayout")]
    public class DecoratorAndHorizontalScrollbar : PropertyDumpTest
    {
        public DecoratorAndHorizontalScrollbar() { this.DumpTest("Decorator_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndLabel", Variables="Area=ElementLayout")]
    public class DecoratorAndLabel : PropertyDumpTest
    {
        public DecoratorAndLabel() { this.DumpTest("Decorator_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndListBox", Variables="Area=ElementLayout")]
    public class DecoratorAndListBox : PropertyDumpTest
    {
        public DecoratorAndListBox() { this.DumpTest("Decorator_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndMenu", Variables="Area=ElementLayout")]
    public class DecoratorAndMenu : PropertyDumpTest
    {
        public DecoratorAndMenu() { this.DumpTest("Decorator_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndProgressBar", Variables="Area=ElementLayout")]
    public class DecoratorAndProgressBar : PropertyDumpTest
    {
        public DecoratorAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("Decorator_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "DecoratorAndRadioButton", Variables="Area=ElementLayout")]
    public class DecoratorAndRadioButton : PropertyDumpTest
    {
        public DecoratorAndRadioButton() { this.DumpTest("Decorator_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndTabControl", Variables="Area=ElementLayout")]
    public class DecoratorAndTabControl : PropertyDumpTest
    {
        public DecoratorAndTabControl() { this.DumpTest("Decorator_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndThumb", Variables="Area=ElementLayout")]
    public class DecoratorAndThumb : PropertyDumpTest
    {
        public DecoratorAndThumb() { this.DumpTest("Decorator_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DecoratorAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class DecoratorAndVerticalScrollBar : PropertyDumpTest
    {
        public DecoratorAndVerticalScrollBar() { this.DumpTest("Decorator_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndButton", Variables="Area=ElementLayout")]
    public class DockPanelAndButton : PropertyDumpTest
    {
        public DockPanelAndButton() { this.DumpTest("DockPanel_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndCheckbox", Variables="Area=ElementLayout")]
    public class DockPanelAndCheckbox : PropertyDumpTest
    {
        public DockPanelAndCheckbox() { this.DumpTest("DockPanel_Checkbox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndComboBox", Variables="Area=ElementLayout")]
    public class DockPanelAndComboBox : PropertyDumpTest
    {
        public DockPanelAndComboBox() { this.DumpTest("DockPanel_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndExpander", Variables="Area=ElementLayout")]
    public class DockPanelAndExpander : PropertyDumpTest
    {
        public DockPanelAndExpander() { this.DumpTest("DockPanel_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndHorizontalScrollbar", Variables="Area=ElementLayout")]
    public class DockPanelAndHorizontalScrollbar : PropertyDumpTest
    {
        public DockPanelAndHorizontalScrollbar() { this.DumpTest("DockPanel_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndLabel", Variables="Area=ElementLayout")]
    public class DockPanelAndLabel : PropertyDumpTest
    {
        public DockPanelAndLabel() { this.DumpTest("DockPanel_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndListBox", Variables="Area=ElementLayout")]
    public class DockPanelAndListBox : PropertyDumpTest
    {
        public DockPanelAndListBox() { this.DumpTest("DockPanel_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndMenu", Variables="Area=ElementLayout")]
    public class DockPanelAndMenu : PropertyDumpTest
    {
        public DockPanelAndMenu() { this.DumpTest("DockPanel_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndProgressBar", Variables="Area=ElementLayout")]
    public class DockPanelAndProgressBar : PropertyDumpTest
    {
        public DockPanelAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("DockPanel_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "DockPanelAndRadioButton", Variables="Area=ElementLayout")]
    public class DockPanelAndRadioButton : PropertyDumpTest
    {
        public DockPanelAndRadioButton() { this.DumpTest("DockPanel_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndTabControl", Variables="Area=ElementLayout")]
    public class DockPanelAndTabControl : PropertyDumpTest
    {
        public DockPanelAndTabControl() { this.DumpTest("DockPanel_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndThumb", Variables="Area=ElementLayout")]
    public class DockPanelAndThumb : PropertyDumpTest
    {
        public DockPanelAndThumb() { this.DumpTest("DockPanel_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "DockPanelAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class DockPanelAndVerticalScrollBar : PropertyDumpTest
    {
        public DockPanelAndVerticalScrollBar() { this.DumpTest("DockPanel_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndButton", Variables="Area=ElementLayout")]
    public class GridAndButton : PropertyDumpTest
    {
        public GridAndButton() { this.DumpTest("Grid_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndCheckBox", Variables="Area=ElementLayout")]
    public class GridAndCheckBox : PropertyDumpTest
    {
        public GridAndCheckBox() { this.DumpTest("Grid_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndComboBox", Variables="Area=ElementLayout")]
    public class GridAndComboBox : PropertyDumpTest
    {
        public GridAndComboBox() { this.DumpTest("Grid_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndExpander", Variables="Area=ElementLayout")]
    public class GridAndExpander : PropertyDumpTest
    {
        public GridAndExpander() { this.DumpTest("Grid_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class GridAndHorizontalScrollBar : PropertyDumpTest
    {
        public GridAndHorizontalScrollBar() { this.DumpTest("Grid_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndLabel", Variables="Area=ElementLayout")]
    public class GridAndLabel : PropertyDumpTest
    {
        public GridAndLabel() { this.DumpTest("Grid_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndListBox", Variables="Area=ElementLayout")]
    public class GridAndListBox : PropertyDumpTest
    {
        public GridAndListBox() { this.DumpTest("Grid_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndMenu", Variables="Area=ElementLayout")]
    public class GridAndMenu : PropertyDumpTest
    {
        public GridAndMenu() { this.DumpTest("Grid_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndProgressBar", Variables="Area=ElementLayout")]
    public class GridAndProgressBar : PropertyDumpTest
    {
        public GridAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("Grid_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "GridAndRadioButton", Variables="Area=ElementLayout")]
    public class GridAndRadioButton : PropertyDumpTest
    {
        public GridAndRadioButton() { this.DumpTest("Grid_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndTabControl", Variables="Area=ElementLayout")]
    public class GridAndTabControl : PropertyDumpTest
    {
        public GridAndTabControl() { this.DumpTest("Grid_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndThumb", Variables="Area=ElementLayout")]
    public class GridAndThumb : PropertyDumpTest
    {
        public GridAndThumb() { this.DumpTest("Grid_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "GridAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class GridAndVerticalScrollBar : PropertyDumpTest
    {
        public GridAndVerticalScrollBar() { this.DumpTest("Grid_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndButton", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndButton : PropertyDumpTest
    {
        public StackPanelHorizontalAndButton() { this.DumpTest("StackPanel_Horizontal_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndCheckBox", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndCheckBox : PropertyDumpTest
    {
        public StackPanelHorizontalAndCheckBox() { this.DumpTest("StackPanel_Horizontal_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndComboBox", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndComboBox : PropertyDumpTest
    {
        public StackPanelHorizontalAndComboBox() { this.DumpTest("StackPanel_Horizontal_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndExpander", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndExpander : PropertyDumpTest
    {
        public StackPanelHorizontalAndExpander() { this.DumpTest("StackPanel_Horizontal_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndHorizontalScrollBar : PropertyDumpTest
    {
        public StackPanelHorizontalAndHorizontalScrollBar() { this.DumpTest("StackPanel_Horizontal_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndLabel", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndLabel : PropertyDumpTest
    {
        public StackPanelHorizontalAndLabel() { this.DumpTest("StackPanel_Horizontal_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndListBox", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndListBox : PropertyDumpTest
    {
        public StackPanelHorizontalAndListBox() { this.DumpTest("StackPanel_Horizontal_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndMenu", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndMenu : PropertyDumpTest
    {
        public StackPanelHorizontalAndMenu() { this.DumpTest("StackPanel_Horizontal_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndProgressBar", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndProgressBar : PropertyDumpTest
    {
        public StackPanelHorizontalAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("StackPanel_Horizontal_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndRadioButton", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndRadioButton : PropertyDumpTest
    {
        public StackPanelHorizontalAndRadioButton() { this.DumpTest("StackPanel_Horizontal_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndTabControl", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndTabControl : PropertyDumpTest
    {
        public StackPanelHorizontalAndTabControl() { this.DumpTest("StackPanel_Horizontal_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndThumb", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndThumb : PropertyDumpTest
    {
        public StackPanelHorizontalAndThumb() { this.DumpTest("StackPanel_Horizontal_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelHorizontalAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class StackPanelHorizontalAndVerticalScrollBar : PropertyDumpTest
    {
        public StackPanelHorizontalAndVerticalScrollBar() { this.DumpTest("StackPanel_Horizontal_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndButton", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndButton : PropertyDumpTest
    {
        public StackPanelVerticalAndButton() { this.DumpTest("StackPanel_Vertical_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndCheckBox", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndCheckBox : PropertyDumpTest
    {
        public StackPanelVerticalAndCheckBox() { this.DumpTest("StackPanel_Vertical_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndComboBox", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndComboBox : PropertyDumpTest
    {
        public StackPanelVerticalAndComboBox() { this.DumpTest("StackPanel_Vertical_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndExpander", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndExpander : PropertyDumpTest
    {
        public StackPanelVerticalAndExpander() { this.DumpTest("StackPanel_Vertical_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndHorizontalScrollBar : PropertyDumpTest
    {
        public StackPanelVerticalAndHorizontalScrollBar() { this.DumpTest("StackPanel_Vertical_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndLabel", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndLabel : PropertyDumpTest
    {
        public StackPanelVerticalAndLabel() { this.DumpTest("StackPanel_Vertical_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndListBox", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndListBox : PropertyDumpTest
    {
        public StackPanelVerticalAndListBox() { this.DumpTest("StackPanel_Vertical_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndMenu", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndMenu : PropertyDumpTest
    {
        public StackPanelVerticalAndMenu() { this.DumpTest("StackPanel_Vertical_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndProgressBar", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndProgressBar : PropertyDumpTest
    {
        public StackPanelVerticalAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("StackPanel_Vertical_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndRadioButton", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndRadioButton : PropertyDumpTest
    {
        public StackPanelVerticalAndRadioButton() { this.DumpTest("StackPanel_Vertical_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndTabControl", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndTabControl : PropertyDumpTest
    {
        public StackPanelVerticalAndTabControl() { this.DumpTest("StackPanel_Vertical_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndThumb", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndThumb : PropertyDumpTest
    {
        public StackPanelVerticalAndThumb() { this.DumpTest("StackPanel_Vertical_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "StackPanelVerticalAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class StackPanelVerticalAndVerticalScrollBar : PropertyDumpTest
    {
        public StackPanelVerticalAndVerticalScrollBar() { this.DumpTest("StackPanel_Vertical_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndButton", Variables="Area=ElementLayout")]
    public class TransformAndButton : PropertyDumpTest
    {
        public TransformAndButton() { this.DumpTest("Transform_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndCheckBox", Variables="Area=ElementLayout")]
    public class TransformAndCheckBox : PropertyDumpTest
    {
        public TransformAndCheckBox() { this.DumpTest("Transform_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndComboBox", Variables="Area=ElementLayout")]
    public class TransformAndComboBox : PropertyDumpTest
    {
        public TransformAndComboBox() { this.DumpTest("Transform_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndExpander", Variables="Area=ElementLayout")]
    public class TransformAndExpander : PropertyDumpTest
    {
        public TransformAndExpander() { this.DumpTest("Transform_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class TransformAndHorizontalScrollBar : PropertyDumpTest
    {
        public TransformAndHorizontalScrollBar() { this.DumpTest("Transform_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndLabel", Variables="Area=ElementLayout")]
    public class TransformAndLabel : PropertyDumpTest
    {
        public TransformAndLabel() { this.DumpTest("Transform_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndListBox", Variables="Area=ElementLayout")]
    public class TransformAndListBox : PropertyDumpTest
    {
        public TransformAndListBox() { this.DumpTest("Transform_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndMenu", Variables="Area=ElementLayout")]
    public class TransformAndMenu : PropertyDumpTest
    {
        public TransformAndMenu() { this.DumpTest("Transform_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndProgressBar", Variables="Area=ElementLayout")]
    public class TransformAndProgressBar : PropertyDumpTest
    {
        public TransformAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("Transform_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "TransformAndRadioButton", Variables="Area=ElementLayout")]
    public class TransformAndRadioButton : PropertyDumpTest
    {
        public TransformAndRadioButton() { this.DumpTest("Transform_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndTabControl", Variables="Area=ElementLayout")]
    public class TransformAndTabControl : PropertyDumpTest
    {
        public TransformAndTabControl() { this.DumpTest("Transform_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndThumb", Variables="Area=ElementLayout")]
    public class TransformAndThumb : PropertyDumpTest
    {
        public TransformAndThumb() { this.DumpTest("Transform_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "TransformAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class TransformAndVerticalScrollBar : PropertyDumpTest
    {
        public TransformAndVerticalScrollBar() { this.DumpTest("Transform_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndButton", Variables="Area=ElementLayout")]
    public class ViewboxAndButton : PropertyDumpTest
    {
        public ViewboxAndButton() { this.DumpTest("Viewbox_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndCheckbox", Variables="Area=ElementLayout")]
    public class ViewboxAndCheckbox : PropertyDumpTest
    {
        public ViewboxAndCheckbox() { this.DumpTest("Viewbox_Checkbox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndComboBox", Variables="Area=ElementLayout")]
    public class ViewboxAndComboBox : PropertyDumpTest
    {
        public ViewboxAndComboBox() { this.DumpTest("Viewbox_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndExpander", Variables="Area=ElementLayout")]
    public class ViewboxAndExpander : PropertyDumpTest
    {
        public ViewboxAndExpander() { this.DumpTest("Viewbox_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndHorizontalScrollbar", Variables="Area=ElementLayout")]
    public class ViewboxAndHorizontalScrollbar : PropertyDumpTest
    {
        public ViewboxAndHorizontalScrollbar() { this.DumpTest("Viewbox_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndLabel", Variables="Area=ElementLayout")]
    public class ViewboxAndLabel : PropertyDumpTest
    {
        public ViewboxAndLabel() { this.DumpTest("Viewbox_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndListBox", Variables="Area=ElementLayout")]
    public class ViewboxAndListBox : PropertyDumpTest
    {
        public ViewboxAndListBox() { this.DumpTest("Viewbox_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndMenu", Variables="Area=ElementLayout")]
    public class ViewboxAndMenu : PropertyDumpTest
    {
        public ViewboxAndMenu() { this.DumpTest("Viewbox_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndProgressBar", Variables="Area=ElementLayout")]
    public class ViewboxAndProgressBar : PropertyDumpTest
    {
        public ViewboxAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("Viewbox_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "ViewboxAndRadioButton", Variables="Area=ElementLayout")]
    public class ViewboxAndRadioButton : PropertyDumpTest
    {
        public ViewboxAndRadioButton() { this.DumpTest("Viewbox_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndTabControl", Variables="Area=ElementLayout")]
    public class ViewboxAndTabControl : PropertyDumpTest
    {
        public ViewboxAndTabControl() { this.DumpTest("Viewbox_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndThumb", Variables="Area=ElementLayout")]
    public class ViewboxAndThumb : PropertyDumpTest
    {
        public ViewboxAndThumb() { this.DumpTest("Viewbox_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "ViewboxAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class ViewboxAndVerticalScrollBar : PropertyDumpTest
    {
        public ViewboxAndVerticalScrollBar() { this.DumpTest("Viewbox_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndButton", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndButton : PropertyDumpTest
    {
        public WrapPanelHorizontalAndButton() { this.DumpTest("WrapPanel_Horizontal_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndCheckBox", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndCheckBox : PropertyDumpTest
    {
        public WrapPanelHorizontalAndCheckBox() { this.DumpTest("WrapPanel_Horizontal_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndComboBox", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndComboBox : PropertyDumpTest
    {
        public WrapPanelHorizontalAndComboBox() { this.DumpTest("WrapPanel_Horizontal_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndExpander", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndExpander : PropertyDumpTest
    {
        public WrapPanelHorizontalAndExpander() { this.DumpTest("WrapPanel_Horizontal_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndHorizontalScrollBar : PropertyDumpTest
    {
        public WrapPanelHorizontalAndHorizontalScrollBar() { this.DumpTest("WrapPanel_Horizontal_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndLabel", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndLabel : PropertyDumpTest
    {
        public WrapPanelHorizontalAndLabel() { this.DumpTest("WrapPanel_Horizontal_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndListBox", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndListBox : PropertyDumpTest
    {
        public WrapPanelHorizontalAndListBox() { this.DumpTest("WrapPanel_Horizontal_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndMenu", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndMenu : PropertyDumpTest
    {
        public WrapPanelHorizontalAndMenu() { this.DumpTest("WrapPanel_Horizontal_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndProgressBar", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndProgressBar : PropertyDumpTest
    {
        public WrapPanelHorizontalAndProgressBar() 
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("WrapPanel_Horizontal_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndRadioButton", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndRadioButton : PropertyDumpTest
    {
        public WrapPanelHorizontalAndRadioButton() { this.DumpTest("WrapPanel_Horizontal_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndTabControl", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndTabControl : PropertyDumpTest
    {
        public WrapPanelHorizontalAndTabControl() { this.DumpTest("WrapPanel_Horizontal_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndThumb", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndThumb : PropertyDumpTest
    {
        public WrapPanelHorizontalAndThumb() { this.DumpTest("WrapPanel_Horizontal_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelHorizontalAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class WrapPanelHorizontalAndVerticalScrollBar : PropertyDumpTest
    {
        public WrapPanelHorizontalAndVerticalScrollBar() { this.DumpTest("WrapPanel_Horizontal_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndButton", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndButton : PropertyDumpTest
    {
        public WrapPanelVerticalAndButton() { this.DumpTest("WrapPanel_Vertical_Button.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndCheckBox", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndCheckBox : PropertyDumpTest
    {
        public WrapPanelVerticalAndCheckBox() { this.DumpTest("WrapPanel_Vertical_CheckBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndComboBox", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndComboBox : PropertyDumpTest
    {
        public WrapPanelVerticalAndComboBox() { this.DumpTest("WrapPanel_Vertical_ComboBox.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndExpander", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndExpander : PropertyDumpTest
    {
        public WrapPanelVerticalAndExpander() { this.DumpTest("WrapPanel_Vertical_Expander.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndHorizontalScrollBar", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndHorizontalScrollBar : PropertyDumpTest
    {
        public WrapPanelVerticalAndHorizontalScrollBar() { this.DumpTest("WrapPanel_Vertical_HorizontalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndLabel", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndLabel : PropertyDumpTest
    {
        public WrapPanelVerticalAndLabel() { this.DumpTest("WrapPanel_Vertical_Label.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndListBox", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndListBox : PropertyDumpTest
    {
        public WrapPanelVerticalAndListBox() { this.DumpTest("WrapPanel_Vertical_ListBox.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndMenu", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndMenu : PropertyDumpTest
    {
        public WrapPanelVerticalAndMenu() { this.DumpTest("WrapPanel_Vertical_Menu.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndProgressBar", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndProgressBar : PropertyDumpTest
    {
        public WrapPanelVerticalAndProgressBar()
        {
            if (Environment.Version.Major < 4)
            {
                this.Arguments.Name = "pre_v40_" + this.Arguments.Name;
            }  
            this.DumpTest("WrapPanel_Vertical_ProgressBar.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); 
        }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndRadioButton", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndRadioButton : PropertyDumpTest
    {
        public WrapPanelVerticalAndRadioButton() { this.DumpTest("WrapPanel_Vertical_RadioButton.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndTabControl", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndTabControl : PropertyDumpTest
    {
        public WrapPanelVerticalAndTabControl() { this.DumpTest("WrapPanel_Vertical_TabControl.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndThumb", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndThumb : PropertyDumpTest
    {
        public WrapPanelVerticalAndThumb() { this.DumpTest("WrapPanel_Vertical_Thumb.xaml", Helpers.LoadStyle("Aero.Theme.xaml")); }
    }

    [Test(3, "Interop.Controls", "WrapPanelVerticalAndVerticalScrollBar", Variables="Area=ElementLayout")]
    public class WrapPanelVerticalAndVerticalScrollBar : PropertyDumpTest
    {
        public WrapPanelVerticalAndVerticalScrollBar() { this.DumpTest("WrapPanel_Vertical_VerticalScrollBar.xaml", Helpers.LoadStyle("GenericControls.xaml")); }
    }
}
