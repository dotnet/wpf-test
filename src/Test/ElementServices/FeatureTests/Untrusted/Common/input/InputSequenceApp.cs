// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.CrossProcess;

namespace Avalon.Test.CoreUI.InputSequence
{
    /******************************************************************************
    * CLASS:          InputSequenceApp
    ******************************************************************************/
    [Test(1, "InputSequence", TestCaseSecurityLevel.FullTrust, "Var", SupportFiles=@"FeatureTests\ElementServices\Controller*.*,FeatureTests\ElementServices\*.cur,FeatureTests\ElementServices\*.ani,FeatureTests\ElementServices\CoreInput_*.xaml,FeatureTests\ElementServices\AllInputScenarios.xtc,FeatureTests\ElementServices\CommonMouseScenarios*.xaml,FeatureTests\ElementServices\DialogOpen*.xaml,FeatureTests\ElementServices\CommonKeyboardScenarios*.xaml,FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\DynamicContentScenarios*.xaml")]
    public class InputSequenceApp : AvalonTest
    {
        #region Private Data
        private string              _testName        = "";
        private string              _applicationType = "";
        private string              _testHost        = "";
        #endregion


        #region Constructor

        //NOTE:  these names correspond to nodes in AllInputScenarios.xtc; a reference to this file is specified
        //in ActionSequenceTestEngine.RunTestAction().

        //NOTE: there are six mutually exclusive groups of Input tests that are placed with different
        //combinations of ApplicationType and HostType.

        //**********************************************************************************************************
        //SET 1:  All combinations of ApplicationType(ClrExe/WpfApplication/WinFormsApplication)
        //        x HostType(NavigationWindow/Window/WindowsFormSource) + Xbap-Browser
        //        for two tests (Keyboard and Mouse) taken from CommonKeyboardScenarios and CommonMouseScenarios.
        //        HostType=HwndSource is excluded; it is covered with SET 4.
        //**********************************************************************************************************
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-ClrExe-NavigationWindow", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-ClrExe-Window", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-ClrExe-WindowsFormSource", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-WpfApplication-NavigationWindow", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-WpfApplication-Window", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-WpfApplication-WindowsFormSource", Priority = 0, Keywords = "MicroSuite")]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-WinFormsApplication-NavigationWindow", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-WinFormsApplication-Window", Priority=0)]
        [Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-WinFormsApplication-WindowsFormSource", Priority=0)]

        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-ClrExe-NavigationWindow", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-ClrExe-Window", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-ClrExe-WindowsFormSource", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WpfApplication-NavigationWindow", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WpfApplication-Window", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WpfApplication-WindowsFormSource", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WinFormsApplication-NavigationWindow", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WinFormsApplication-Window", Priority=0)]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WinFormsApplication-WindowsFormSource", Priority=0)]

        //[Variation("Border_Border_Canvas_AllFocusable_HandledEventsToo-Xbap-Browser", Priority=0)]
        //[Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-Xbap-Browser", Priority=0)]

        //**********************************************************************************************************
        //SET 2:  ApplicationType(ClrExe) x HostType(NavigationWindow/WindowsFormSource)
        //        DynamicContentScenarios + CommonMouseScenarios_NonBrowser
        //**********************************************************************************************************
        //DynamicContentScenarios:
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_IsHitTestVisibleFalse-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_Opacity0-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_IsVisibleHidden-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_IsHitTestVisibleFalse-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_Opacity0-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_VisibilityHidden-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_IsHitTestVisibleFalse-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_Opacity0-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_VisibilityHidden-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_IsHitTestVisibleFalse-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_Opacity0-ClrExe-NavigationWindow")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_VisibilityHidden-ClrExe-NavigationWindow")]
        // [DISABLE WHILE PORTING]
        // [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse-ClrExe-NavigationWindow")]
        // [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_IsHitTestVisibleFalse-ClrExe-NavigationWindow")]
        // [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_Opacity0-ClrExe-NavigationWindow")]
        // [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_VisibilityHidden-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_Animated-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_ResizeButtonUnderMouse-ClrExe-NavigationWindow")]
        [Variation("Border_Canvas_Button_ResizeButtonUnderMouse_Animated-ClrExe-NavigationWindow")]

        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_IsHitTestVisibleFalse-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_Opacity0-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_IsVisibleHidden-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_IsHitTestVisibleFalse-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_Opacity0-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_VisibilityHidden-ClrExe-WindowsFormSource")]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse-ClrExe-WindowsFormSource")]  // [DISABLE WHILE PORTING]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_IsHitTestVisibleFalse-ClrExe-WindowsFormSource")] // [DISABLE WHILE PORTING]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_Opacity0-ClrExe-WindowsFormSource")]// [DISABLE WHILE PORTING]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_VisibilityHidden-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_IsHitTestVisibleFalse-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_Opacity0-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_VisibilityHidden-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_IsHitTestVisibleFalse-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_Opacity0-ClrExe-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_VisibilityHidden-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_Animated-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_ResizeButtonUnderMouse-ClrExe-WindowsFormSource")]
        [Variation("Border_Canvas_Button_ResizeButtonUnderMouse_Animated-ClrExe-WindowsFormSource")]

        
        //CommonMouseScenarios_NonBrowser
        // [Variation("Border_Button_Canvas_ContextMenu_ToolTip-ClrExe-NavigationWindow",Priority=2)] // [DISABLE WHILE PORTING]
        // [Variation("Border_Button_Canvas_ContextMenu_ToolTip2-ClrExe-NavigationWindow",Priority=2)]// [DISABLE WHILE PORTING]

        // [Variation("Border_Button_Canvas_ContextMenu_ToolTip-ClrExe-WindowsFormSource",Priority=2)] // [DISABLE WHILE PORTING]
        // [Variation("Border_Button_Canvas_ContextMenu_ToolTip2-ClrExe-WindowsFormSource",Priority=2)] // [DISABLE WHILE PORTING]

        //CommonMouseScenarios_Drag_Partial Trust --- inconsistent
        //[Variation("Border_TextBox_MouseDragOver_PT-ClrExe-NavigationWindow")]
        //[Variation("Border_Button_Canvas_MouseDragOver_PT-ClrExe-NavigationWindow")]
        //[Variation("Border_TextBox_MouseDragOver_PT-ClrExe-WindowsFormSource")]
        //[Variation("Border_Button_Canvas_MouseDragOver_PT-ClrExe-WindowsFormSource")]

        //**********************************************************************************************************
        //SET 3:  ApplicationType(WpfApplication) x HostType(WindowsFormSource)
        //        DynamicContentScenarios + CommonMouseScenarios_NonBrowser
        //        + CommonKeyboardScenarios + CommonMouseScenarios
        //**********************************************************************************************************
        //DynamicContentScenarios:
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_IsHitTestVisibleFalse-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_Opacity0-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_IsVisibleHidden-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_IsHitTestVisibleFalse-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_Opacity0-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonByRenderTransformUnderMouse_VisibilityHidden-WpfApplication-WindowsFormSource")]
        // [DISABLE WHILE PORTING]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse-WpfApplication-WindowsFormSource")]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_IsHitTestVisibleFalse-WpfApplication-WindowsFormSource")]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_Opacity0-WpfApplication-WindowsFormSource")]
        // [Variation("Border_StackPanel_Button_AddRemoveButtons_LayoutUnderMouse_VisibilityHidden-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_IsHitTestVisibleFalse-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_Opacity0-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_RemoveAndAddButtonUnderMouse_VisibilityHidden-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_IsHitTestVisibleFalse-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_Opacity0-WpfApplication-WindowsFormSource")]
        [Variation("Border_StackPanel_Button_MoveWindowFromUnderMouse_VisibilityHidden-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_MoveButtonCanvasPositionUnderMouse_Animated-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_ResizeButtonUnderMouse-WpfApplication-WindowsFormSource")]
        [Variation("Border_Canvas_Button_ResizeButtonUnderMouse_Animated-WpfApplication-WindowsFormSource")]

        // [DISABLE WHILE PORTING]
        //CommonMouse Scenarios_NonBrowser
        // [Variation("Border_Button_Canvas_ContextMenu_ToolTip-WpfApplication-WindowsFormSource",Priority=2)]
        // [Variation("Border_Button_Canvas_ContextMenu_ToolTip2-WpfApplication-WindowsFormSource",Priority=2)]

        //CommonKeyboardScenarios:
        [Variation("Border_TextBox_AllFocusable_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("Border_Border_Canvas_AllFocusable-WpfApplication-WindowsFormSource")]
        [Variation("Border_TextBox_AllFocusable-WpfApplication-WindowsFormSource")]
        [Variation("Border_Border_Canvas_AllFocusable2-WpfApplication-WindowsFormSource")]
        [Variation("Border_TextBox_AllFocusable2-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Canvas_AllFocusable_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Canvas_AllFocusable-WpfApplication-WindowsFormSource")]
        //[Variation("Border_TextBox_ModifierKeyCombinations-WpfApplication-WindowsFormSource", Disabled=true)]
        //[Variation("TextBlock_Bold_Canvas_ModifierKeyCombinations-WpfApplication-WindowsFormSource", Disabled=true)]
        //[Variation("Border_TextBox_ModifierKeyCombinations_HandledEventsToo-WpfApplication-WindowsFormSource", Disabled=true)]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:   Var(Border_TextBox_AltNumPadKeyCombinations_HandledEventsToo-WpfApplication-WindowsFormSource)
        // Area: ElementServices �� SubArea: InputSequence
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("Border_TextBox_AltNumPadKeyCombinations_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("Border_TextBox_ShiftF2KeyCombination_HandledEventsToo-WpfApplication-WindowsFormSource", Disabled=true)]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:  Var(Border_TextBox_RepeatKeyPress-WpfApplication-WindowsFormSource)
        // Area: ElementServices �� SubArea: InputSequence
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("Border_TextBox_RepeatKeyPress-WpfApplication-WindowsFormSource")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:  Var(TextBlock_Bold_Canvas_RepeatKeyPress-WpfApplication-WindowsFormSource)
        // Area: ElementServices �� SubArea: InputSequence
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("TextBlock_Bold_Canvas_RepeatKeyPress-WpfApplication-WindowsFormSource")]

        ////////////////////////////////////////////////////////////////////////////////////////////������� 
        // DISABLEDUNSTABLETEST:
        // TestName:  Var(Border_TextBox_RepeatModifierKeyPress-WpfApplication-WindowsFormSource)
        // Area: ElementServices �� SubArea: InputSequence
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        ////////////////////////////////////////////////////////////////////////////////////////////
//        [Variation("Border_TextBox_RepeatModifierKeyPress-WpfApplication-WindowsFormSource")]
        [Variation("Border_Border_Canvas_MarkPreviewKeyDownHandled-WpfApplication-WindowsFormSource", Disabled=true)]
        [Variation("Border_TextBox_MarkPreviewKeyDownHandled-WpfApplication-WindowsFormSource", Disabled=true)]
        [Variation("Border_TextBox_ModifierKeyCombinations_MarkPreviewKeyDownHandled-WpfApplication-WindowsFormSource", Disabled=true)]
        //[Variation("Border_Border_Canvas_PrintScreen_MarkPreviewKeyDownHandled-WpfApplication-WindowsFormSource")]

        //CommonMouseScenarios:
        [Variation("Border_Button_Canvas_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("TextFlow_Paragraph_Button_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Run_Canvas_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("Border_Button_Canvas_MouseMoveOverAndClick-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick-WpfApplication-WindowsFormSource")]
        [Variation("Border_Button_Canvas_MouseMoveOverAndClick2-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick2-WpfApplication-WindowsFormSource")]
        //[Variation("Border_Button_Canvas_MoveMouseWheel-WpfApplication-WindowsFormSource", Disabled=true)]
        //[Variation("TextBlock_Bold_Canvas_MoveMouseWheel-WpfApplication-WindowsFormSource", Disabled=true)]
        //[Variation("Border_Button_Canvas_MoveMouseWheel_UseStaticMethod-WpfApplication-WindowsFormSource", Disabled=true)]
        //[Variation("TextBlock_Bold_Canvas_MoveMouseWheel_UseStaticMethod-WpfApplication-WindowsFormSource", Disabled=true)]
        [Variation("TextBlock_Bold_Canvas_MouseCaptureAndClickOutsideElement_UseStaticMethod-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Canvas_MouseCaptureAndCickInsideElement_UseStaticMethod-WpfApplication-WindowsFormSource")]
        [Variation("TextBlock_Bold_Canvas_MouseCaptureAndClickOutsideElement-WpfApplication-WindowsFormSource")]
        [Variation("Border_Button_Canvas_MouseMoveImmediatelyOver_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("Border_Button_Canvas_MouseMoveAsyncOver_HandledEventsToo-WpfApplication-WindowsFormSource")]
        [Variation("Border_Button_Canvas_MouseMoveAsyncOver_HandledEventsToo_NoWait-WpfApplication-WindowsFormSource")]

//These three were not included in the .txt file.
        //CommonMouse Scenarios_Drag_FullTrust
        [Variation("Border_TextBox_MouseDragOver_FT-WpfApplication-WindowsFormSource", Disabled=true)]
        [Variation("Border_Button_Canvas_MouseDragOver_FT-WpfApplication-WindowsFormSource", Disabled=true)]
        [Variation("TextBlock_Bold_Run_Canvas_MouseMoveOverAndClick_FT-WpfApplication-WindowsFormSource", Disabled=true)]

        //DialogOpenMouseInputStateScenarios:  --- inconsistent
        //[Variation("DockPanel_DialogOpenOnPropertyChange_Canvas_MouseMoveCenterRight_HandledEventsToo-WpfApplication-WindowsFormSource")]
        //[Variation("DockPanel_DialogOpenOnPropertyChange_Canvas_MouseMoveCenter_HandledEventsToo-WpfApplication-WindowsFormSource")]
        //[Variation("DockPanel_DialogOpenOnPropertyChange_Canvas_MouseLeftButtonClickOnCenter_HandledEventsToo-WpfApplication-WindowsFormSource")]
        //[Variation("DockPanel_DialogOpenOnPropertyChange_Canvas_MouseRightButtonClickOnCenter_HandledEventsToo-WpfApplication-WindowsFormSource")]
        
        //DialogOpenBoundKeyboardInputStateScenarios:
        //[Variation("DockPanel_DialogOpenOnPropertyChange_Canvas_FocusAction_HandledEventsToo-WpfApplication-WindowsFormSource")]
        
        //DialogOpenBoundMouseInputStateScenarios:
        //[Variation("DockPanel_BoundDialogOpenOnPropertyChange_Canvas-WpfApplication-WindowsFormSource")]
        //[Variation("DockPanel_BoundDialogOpenOnPropertyChange_Canvas_MouseMoveCenter_HandledEventsToo-WpfApplication-WindowsFormSource")]
        //[Variation("DockPanel_BoundDialogOpenOnPropertyChange_Canvas_MouseLeftButtonClickOnCenter_HandledEventsToo-WpfApplication-WindowsFormSource")]
        //[Variation("DockPanel_BoundDialogOpenOnPropertyChange_Canvas_MouseRightButtonClickOnCenter_HandledEventsToo-WpfApplication-WindowsFormSource")]
       
       //DialogOpenKeyboardInputStateScenarios:
        //[Variation("DockPanel_DialogOpenOnPropertyChange_Canvas_HandledEventsToo-WpfApplication-WindowsFormSource")]

        //**********************************************************************************************************
        //SET 4:  ApplicationType(WpfApplication) x HostType(HwndSource)
        //        CommonKeyboardScenarios_HwndSource + CommonMouseScenarios
        //**********************************************************************************************************
        //Common Keyboard Scenarios_HwndSource
        [Variation("Hwnd_Border_Border_Canvas_AllFocusable_HandledEventsToo-WpfApplication-HwndSource", Disabled=true)]
        [Variation("Hwnd_Border_TextBox_AllFocusable_HandledEventsToo-WpfApplication-HwndSource", Disabled=true)]
        [Variation("Hwnd_Border_Border_Canvas_AllFocusable-WpfApplication-HwndSource", Disabled=true)]
        [Variation("Hwnd_Border_Border_Canvas_AllFocusable2-WpfApplication-HwndSource", Disabled=true)]
        [Variation("Hwnd_Border_TextBox_AllFocusable2-WpfApplication-HwndSource", Disabled=true)]
        [Variation("Hwnd_TextBlock_Bold_Canvas_AllFocusable_HandledEventsToo-WpfApplication-HwndSource", Disabled=true)]
        [Variation("Hwnd_TextBlock_Bold_Canvas_AllFocusable-WpfApplication-HwndSource", Disabled=true)]

        //CommonMouseScenarios:
        [Variation("Border_Button_Canvas_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-HwndSource")]
        [Variation("TextFlow_Paragraph_Button_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-HwndSource")]
        [Variation("TextBlock_Bold_Run_Canvas_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-HwndSource")]
        [Variation("Border_Button_Canvas_MouseMoveOverAndClick-WpfApplication-HwndSource")]
        [Variation("Border_Button_Canvas_MouseMoveOverAndClick2-WpfApplication-HwndSource")]
        [Variation("TextBlock_Bold_Canvas_MouseMoveOverAndClick2-WpfApplication-HwndSource")]
        //[Variation("Border_Button_Canvas_MoveMouseWheel-WpfApplication-HwndSource", Disabled=true)]
        //[Variation("TextBlock_Bold_Canvas_MoveMouseWheel-WpfApplication-HwndSource", Disabled=true)]
        //[Variation("Border_Button_Canvas_MoveMouseWheel_UseStaticMethod-WpfApplication-HwndSource", Disabled=true)]
        //[Variation("TextBlock_Bold_Canvas_MoveMouseWheel_UseStaticMethod-WpfApplication-HwndSource", Disabled=true)]
        [Variation("TextBlock_Bold_Canvas_MouseCaptureAndClickOutsideElement_UseStaticMethod-WpfApplication-HwndSource")]
        [Variation("TextBlock_Bold_Canvas_MouseCaptureAndCickInsideElement_UseStaticMethod-WpfApplication-HwndSource")]
        [Variation("TextBlock_Bold_Canvas_MouseCaptureAndClickOutsideElement-WpfApplication-HwndSource")]
        [Variation("Border_Button_Canvas_MouseMoveImmediatelyOver_HandledEventsToo-WpfApplication-HwndSource")]
        [Variation("Border_Button_Canvas_MouseMoveAsyncOver_HandledEventsToo-WpfApplication-HwndSource")]
        [Variation("Border_Button_Canvas_MouseMoveAsyncOver_HandledEventsToo_NoWait-WpfApplication-HwndSource")]

       
        //**********************************************************************************************************
        //SET 6:  ApplicationType(WpfApplication) x HostType(NavigationWindow)
        //        UIElement3DKeyboard + UIElement3DMouse
        //        NOTE: these tests were not fully implemented.
        //**********************************************************************************************************
/*
        //UIElement3DKeyboard:
        [Variation("3D_2dIn3dSetFocusAndTypeTwice-WpfApplication-NavigationWindow")]
        [Variation("3D_UIElement3DSetFocusAndTypeTwice-WpfApplication-NavigationWindow")]
        [Variation("3D_SetFocusAndTypeNoHandledEvents-WpfApplication-NavigationWindow")]

        //UIElement3DMouse:
        [Variation("3D_MoveOverEach3dElement-WpfApplication-NavigationWindow")]
        [Variation("3D_CaptureOn2Din3DAndMove-WpfApplication-NavigationWindow")]
        [Variation("3D_CaptureOnUIElement3DAndMove-WpfApplication-NavigationWindow")]
        [Variation("3D_ClickOnEachElement-WpfApplication-NavigationWindow")]
        [Variation("3D_Viewport3D_Viewport2DVisual3D_Button_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_TextFlow_Paragraph_Button_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_TextBlock_Bold_Run_Canvas_MouseMoveOverAndClick_HandledEventsToo-WpfApplication-NavigationWindow")]
        [Variation("3D_Border_Button_Canvas_MouseMoveOverAndClick-WpfApplication-NavigationWindow")]
        [Variation("3D_TextBlock_Bold_Canvas_MouseMoveOverAndClick-WpfApplication-NavigationWindow")]
        [Variation("3D_Border_Button_Canvas_MouseMoveOverAndClick2-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_TextBlock_Bold_Canvas_MouseMoveOverAndClick2-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_Border_Button_Canvas_MoveMouseWheel-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_TextBlock_Bold_Canvas_MoveMouseWheel-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_Border_Button_Canvas_MoveMouseWheel_UseStaticMethod-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_TextBlock_Bold_Canvas_MoveMouseWheel_UseStaticMethod", Disabled=true)]
        [Variation("3D_TextBlock_Bold_Canvas_MouseCaptureAndClickOutsideElement_UseStaticMethod-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_TextBlock_Bold_Canvas_MouseCaptureAndClickInsideElement_UseStaticMethod-WpfApplication-NavigationWindow")]
        [Variation("3D_TextBlock_Bold_Canvas_MouseCaptureAndClickOutsideElement-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_Border_Button_Canvas_MouseMoveImmediatelyOver_HandledEventsToo-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_Border_Button_Canvas_MouseMoveAsyncOver_HandledEventsToo-WpfApplication-NavigationWindow", Disabled=true)]
        [Variation("3D_Border_Button_Canvas_MouseMoveAsyncOver_HandledEventsToo_NoWait-WpfApplication-NavigationWindow", Disabled=true)]
*/

        /******************************************************************************
        * Function:          InputSequenceApp Constructor
        ******************************************************************************/
        public InputSequenceApp(string arg)
        {
            char[] delimiters = new char[] { '-' };
            String[] argArray = arg.Split(delimiters);
            if (argArray.Length != 3)
            {
                throw new Microsoft.Test.TestSetupException("Three parameters delimited by dashes must be specified.");
            }

            _testName = argArray[0];
            _applicationType = argArray[1];
            _testHost = argArray[2];

            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            DictionaryStore.StartServer();
            GlobalLog.LogStatus("In InputSequenceApp.RunTest...");

            GlobalLog.LogStatus("*******************************************");
            GlobalLog.LogStatus("ApplicationType: " + _applicationType);
            GlobalLog.LogStatus("Host:            " + _testHost);
            GlobalLog.LogStatus("Running:         " + _testName);
            GlobalLog.LogStatus("*******************************************");

            ApplicationType appType = (ApplicationType)Enum.Parse(typeof(ApplicationType), _applicationType);

            CommonStorage.CleanAll();
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), _testHost);

            ContainerVariationItem cvi = new ContainerVariationItem();
            cvi.Execute(appType, hostType, "ActionSequence", _testName);            
            //A test failure will be handled by an Exception thrown during Verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
