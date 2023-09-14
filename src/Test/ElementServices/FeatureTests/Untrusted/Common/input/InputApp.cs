// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.CrossProcess;

namespace Avalon.Test.CoreUI.CoreInput
{
    /******************************************************************************
    * CLASS:          InputApp
    ******************************************************************************/
    [Test(1, "Input", TestCaseSecurityLevel.FullTrust, "Var", SupportFiles=@"FeatureTests\ElementServices\Controller*.*,FeatureTests\ElementServices\*.cur,FeatureTests\ElementServices\*.ani,FeatureTests\ElementServices\CoreInput_GridPanelCursors.xaml")]
    public class InputApp : AvalonTest
    {
        #region Private Data
        private string              _testName        = "";
        private string              _applicationType = "";
        private string              _testHost        = "";
        #endregion


        #region Constructor

        //NOTE: there are four mutually exclusive groups of Input tests: Pri0, Pri1, NonBrowser, and Pri1Browser,
        //that are placed with different combinations of ApplicationType and HostType.

        //**********************************************************************************************************
        //SET 1:  ApplicationType(WpfApplication/ClrExe/WinFormsApplication) x HostType(HwndSource/Window/NavigationWindow/WindowsFormSource)
        //        (Pri0 group)
        //**********************************************************************************************************
        //Pri0 group:
        [Variation("CaptureClickApp_ClrExe_HwndSource",Priority=0)]
        [Variation("CaptureClickApp_ClrExe_Window",Priority=0)]
        [Variation("CaptureClickApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("CaptureClickApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("CaptureClickApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("CaptureClickApp_WinFormsApplication_Window",Priority=0)]
        [Variation("CaptureClickApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("CaptureClickApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("CaptureClickApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("CaptureClickApp_WpfApplication_Window",Priority=0)]
        [Variation("CaptureClickApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("CaptureClickApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("CaptureMoveApp_ClrExe_HwndSource",Priority=0)]
        [Variation("CaptureMoveApp_ClrExe_Window",Priority=0)]
        [Variation("CaptureMoveApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("CaptureMoveApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("CaptureMoveApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("CaptureMoveApp_WinFormsApplication_Window",Priority=0)]
        [Variation("CaptureMoveApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("CaptureMoveApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("CaptureMoveApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("CaptureMoveApp_WpfApplication_Window",Priority=0)]
        [Variation("CaptureMoveApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("CaptureMoveApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("IsFocusWithinApp_ClrExe_HwndSource",Priority=0)]
        [Variation("IsFocusWithinApp_ClrExe_Window",Priority=0)]
        [Variation("IsFocusWithinApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("IsFocusWithinApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("IsFocusWithinApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("IsFocusWithinApp_WinFormsApplication_Window",Priority=0)]
        [Variation("IsFocusWithinApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("IsFocusWithinApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("IsFocusWithinApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("IsFocusWithinApp_WpfApplication_Window",Priority=0)]
        [Variation("IsFocusWithinApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("IsFocusWithinApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("KeyDownApp_ClrExe_HwndSource",Priority=0)]
        [Variation("KeyDownApp_ClrExe_Window",Priority=0)]
        [Variation("KeyDownApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("KeyDownApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("KeyDownApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("KeyDownApp_WinFormsApplication_Window",Priority=0)]
        [Variation("KeyDownApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("KeyDownApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("KeyDownApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("KeyDownApp_WpfApplication_Window",Priority=0)]
        [Variation("KeyDownApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("KeyDownApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("KeyStateApp_ClrExe_HwndSource",Priority=0)]
        [Variation("KeyStateApp_ClrExe_Window",Priority=0)]
        [Variation("KeyStateApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("KeyStateApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("KeyStateApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("KeyStateApp_WinFormsApplication_Window",Priority=0)]
        [Variation("KeyStateApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("KeyStateApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("KeyStateApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("KeyStateApp_WpfApplication_Window",Priority=0)]
        [Variation("KeyStateApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("KeyStateApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("MouseButtonApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseButtonApp_ClrExe_Window",Priority=0)]
        [Variation("MouseButtonApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseButtonApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseButtonApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseButtonApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseButtonApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseButtonApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseButtonApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseButtonApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseButtonApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseButtonApp_WpfApplication_WindowsFormSource", Priority = 0, Keywords = "MicroSuite")]

        [Variation("MouseCursorApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseCursorApp_ClrExe_Window",Priority=0)]
        [Variation("MouseCursorApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseCursorApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseCursorApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseCursorApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseCursorApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseCursorApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseCursorApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseCursorApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseCursorApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseCursorApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("MouseDownApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseDownApp_ClrExe_Window",Priority=0)]
        [Variation("MouseDownApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseDownApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseDownApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseDownApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseDownApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseDownApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseDownApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseDownApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseDownApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseDownApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("MouseEnterApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseEnterApp_ClrExe_Window",Priority=0)]
        [Variation("MouseEnterApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseEnterApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseEnterApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseEnterApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseEnterApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseEnterApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseEnterApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseEnterApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseEnterApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseEnterApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("MouseMoveApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseMoveApp_ClrExe_Window",Priority=0)]
        [Variation("MouseMoveApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseMoveApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseMoveApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseMoveApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseMoveApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseMoveApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseMoveApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseMoveApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseMoveApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseMoveApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("MouseSetCursorApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseSetCursorApp_ClrExe_Window",Priority=0)]
        [Variation("MouseSetCursorApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseSetCursorApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseSetCursorApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseSetCursorApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseSetCursorApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseSetCursorApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseSetCursorApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseSetCursorApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseSetCursorApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseSetCursorApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("MouseWheelApp_ClrExe_HwndSource",Priority=0)]
        [Variation("MouseWheelApp_ClrExe_Window",Priority=0)]
        [Variation("MouseWheelApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("MouseWheelApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("MouseWheelApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("MouseWheelApp_WinFormsApplication_Window",Priority=0)]
        [Variation("MouseWheelApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("MouseWheelApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("MouseWheelApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("MouseWheelApp_WpfApplication_Window",Priority=0)]
        [Variation("MouseWheelApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("MouseWheelApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("TextInputApp_ClrExe_HwndSource",Priority=0)]
        [Variation("TextInputApp_ClrExe_Window",Priority=0)]
        [Variation("TextInputApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("TextInputApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("TextInputApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("TextInputApp_WinFormsApplication_Window",Priority=0)]
        [Variation("TextInputApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("TextInputApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("TextInputApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("TextInputApp_WpfApplication_Window",Priority=0)]
        [Variation("TextInputApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("TextInputApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementCaptureApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementCaptureApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementCaptureApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementCaptureApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementCaptureApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementCaptureApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementCaptureApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementCaptureApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementCaptureApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementCaptureApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementCaptureApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementCaptureApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementCaptureToSubtreeApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementCaptureToSubtreeApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementFocusApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementFocusApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementFocusApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementFocusApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementFocusApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementFocusApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementFocusApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementFocusApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementFocusApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementFocusApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementFocusApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementFocusApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementIsEnabledApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementIsEnabledApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementIsEnabledApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementIsEnabledApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementIsEnabledApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementIsEnabledApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementIsEnabledApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementIsEnabledApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementIsEnabledApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementIsEnabledApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementIsEnabledApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementIsEnabledApp_WpfApplication_WindowsFormSource",Priority=0)]

        [Variation("UIElementLostFocusApp_ClrExe_HwndSource",Priority=0)]
        [Variation("UIElementLostFocusApp_ClrExe_Window",Priority=0)]
        [Variation("UIElementLostFocusApp_ClrExe_NavigationWindow",Priority=0)]
        [Variation("UIElementLostFocusApp_ClrExe_WindowsFormSource",Priority=0)]
        [Variation("UIElementLostFocusApp_WinFormsApplication_HwndSource",Priority=0)]
        [Variation("UIElementLostFocusApp_WinFormsApplication_Window",Priority=0)]
        [Variation("UIElementLostFocusApp_WinFormsApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementLostFocusApp_WinFormsApplication_WindowsFormSource",Priority=0)]
        [Variation("UIElementLostFocusApp_WpfApplication_HwndSource",Priority=0)]
        [Variation("UIElementLostFocusApp_WpfApplication_Window",Priority=0)]
        [Variation("UIElementLostFocusApp_WpfApplication_NavigationWindow",Priority=0)]
        [Variation("UIElementLostFocusApp_WpfApplication_WindowsFormSource",Priority=0)]

        //**********************************************************************************************************
        //SET 2:  ApplicationType=WpfApplication / HostType=Window (Pri1 group and NonBrowser group)
        //**********************************************************************************************************
        //Pri1 group:
        //These commented out tests were not successfully ported from v1 or randomly fail.
        //[Variation("ContentElementLostFocusApp")]
        //[Variation("ContentElementForceCursorApp")]
        //[Variation("CursorRelativePathStringApp")]
        //[Variation("CursorConverterConvertFromRelativePathApp")]
        //[Variation("FrameworkContentElementCursorApp")]
        //[Variation("FocusAfterWinFormsMessageBoxApp")]
        //[Variation("FocusAfterRemoveElementApp")]
        //[Variation("FocusAfterRemoveElementApp")]
        //[Variation("KeyConverterConvertFromAllKeysApp")]
        //[Variation("MultiWindowFocusActiveElementApp")]
        //[Variation("MultiCursorMoveToAndClickApp")]
        //[Variation("TextInputAltShiftCharApp")]
        //[Variation("UIElementPreviewFocusApp")]
        //[Variation("UIElementMouseEnterBorderImmediateMoveApp")]
        //[Variation("UIElementMouseDownOutsideCapturedElementCanvasApp")]
        //[Variation("UIElementMouseDownOutsideCapturedElementApp")]
        //[Variation("GetIntermediatePointsContentElementApp")]
        //[Variation("GetIntermediatePointsUIElementApp")]
        //[Variation("KeyboardFocusedPageNavigateApp")]
        //[Variation("InputManagerMultipleProcessEventsApp")]
        //[Variation("KeyboardFocusedPageNavigateBackApp")]
        [Variation("UIElementIsMouseDirectlyOverApp_WpfApplication_Window")]
        [Variation("CaptureAfterMessageBoxApp_WpfApplication_Window")]
        [Variation("CaptureAfterRemoveElementApp_WpfApplication_Window")]
        [Variation("CaptureAfterRemoveElementEnabledFalseApp_WpfApplication_Window", Disabled=true)] // Failing regularly on 3.5 and 4.0
        [Variation("CaptureAfterRemoveElementRecaptureApp_WpfApplication_Window")]
        [Variation("CaptureAfterWinFormsMessageBoxApp_WpfApplication_Window")]
        [Variation("CaptureDoubleAnimationApp_WpfApplication_Window")]
        [Variation("CaptureDoubleAnimationPositionApp_WpfApplication_Window")]
        [Variation("CaptureMoveNoSourceApp_WpfApplication_Window")]
        [Variation("CapturePositionApp_WpfApplication_Window")]
        [Variation("ContentElementCaptureApp_WpfApplication_Window")]
        [Variation("ContentElementCaptureToSubTreeApp_WpfApplication_Window")]
        [Variation("ContentElementCaptureToSubtreeMouseClickApp_WpfApplication_Window")]
        [Variation("ContentElementFocusApp_WpfApplication_Window")]
        [Variation("ContentElementIsEnabledApp_WpfApplication_Window")]
        [Variation("ContentElementIsFocusWithinApp_WpfApplication_Window")]
        [Variation("ContentElementIsMouseDirectlyOverApp_WpfApplication_Window")]
        [Variation("ContentElementIsMouseOverApp_WpfApplication_Window")]
        [Variation("ContentElementIsMouseOverChangedApp_WpfApplication_Window")]
        [Variation("ContentElementKeyDownApp_WpfApplication_Window")]
        [Variation("ContentElementMouseButtonApp_WpfApplication_Window")]
        [Variation("ContentElementMouseButtonImmediateMoveApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorAniFileApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorAniStreamApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorCurFileApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorCurStreamApp_WpfApplication_Window")]
        [Variation("ContentElementMouseDirectlyOverApp_WpfApplication_Window")]
        [Variation("ContentElementMouseDownOutsideCapturedElementApp_WpfApplication_Window")]
        [Variation("ContentElementMouseDownOutsideCapturedElementCanvasApp_WpfApplication_Window")]
        [Variation("ContentElementMouseEnterApp_WpfApplication_Window")]
        [Variation("ContentElementMouseEnterBorderApp_WpfApplication_Window")]
        [Variation("ContentElementMouseEnterBorderImmediateMoveApp_WpfApplication_Window")]
        [Variation("ContentElementMouseEnterImmediateMoveApp_WpfApplication_Window")]
        [Variation("ContentElementMouseLeftButtonDownApp_WpfApplication_Window")]
        [Variation("ContentElementMouseMoveApp_WpfApplication_Window")]
        [Variation("ContentElementMouseRightButtonDownApp_WpfApplication_Window")]
        [Variation("ContentElementMouseWheelApp_WpfApplication_Window")]
        [Variation("ContentElementPreviewFocusApp_WpfApplication_Window")]
        [Variation("ContentElementQueryCursorApp_WpfApplication_Window")]
        [Variation("ContentElementReleaseMouseCaptureApp_WpfApplication_Window")]
        [Variation("CursorTypeConverterCanConvertFromApp_WpfApplication_Window")]
        [Variation("CursorTypeConverterCanConvertToApp_WpfApplication_Window")]
        [Variation("CursorTypeConverterCanConvertToNullContextApp_WpfApplication_Window")]
        [Variation("CursorTypeConverterConvertToApp_WpfApplication_Window")]
        [Variation("FocusAfterMessageBoxApp_WpfApplication_Window")]
        [Variation("FocusAfterModalWindowApp_WpfApplication_Window")]
        [Variation("FocusAfterNonModalWindowApp_WpfApplication_Window")]
        [Variation("FocusAfterRemoveElementRefocusApp_WpfApplication_Window")]
        [Variation("FocusInvalidInputElementApp_WpfApplication_Window")]
        [Variation("ForceCursorApp_WpfApplication_Window")]
        [Variation("FrameworkContentElementFocusableApp_WpfApplication_Window")]
        [Variation("FrameworkContentElementIsFocusWithinApp_WpfApplication_Window")]
        [Variation("FrameworkContentElementIsMouseOverApp_WpfApplication_Window")]
        [Variation("FrameworkContentElementIsMouseOverChangedApp_WpfApplication_Window")]
        [Variation("FrameworkContentElementQueryCursorMouseMoveApp_WpfApplication_Window")]
        [Variation("FrameworkElementForceCursorElementMoveApp_WpfApplication_Window")]
        [Variation("FrameworkElementInputHitTestDisabledApp_WpfApplication_Window")]
        [Variation("FrameworkElementInputHitTestInvisibleApp_WpfApplication_Window")]
        [Variation("FrameworkElementIsFocusWithinApp_WpfApplication_Window")]
        [Variation("FrameworkElementIsMouseOverChangedApp_WpfApplication_Window")]
        [Variation("FrameworkElementMouseOverVisibilityPropertyTriggerApp_WpfApplication_Window")]
        [Variation("FrameworkElementQueryCursorElementMoveApp_WpfApplication_Window")]
        [Variation("FrameworkElementQueryCursorMouseMoveApp_WpfApplication_Window")]
        [Variation("FrameworkElementQueryCursorNullCursorApp_WpfApplication_Window")]
        [Variation("FrameworkElementQueryCursorSendMessageApp_WpfApplication_Window")]
        [Variation("FrameworkElementQueryCursorUpdateCursorApp_WpfApplication_Window")]
        [Variation("KeyAfterMessageBoxApp_WpfApplication_Window")]
        [Variation("KeyAfterWinFormsMessageBoxApp_WpfApplication_Window")]
        [Variation("KeyboardDeviceIsKeyToggledApp_WpfApplication_Window")]
        [Variation("KeyboardFocusedApp_WpfApplication_Window")]
        [Variation("KeyboardFocusedFrameApp_WpfApplication_Window")]
        [Variation("KeyboardStateApp_WpfApplication_Window")]
        [Variation("KeyConverterCanConvertFromApp_WpfApplication_Window")]
        [Variation("KeyConverterCanConvertToApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertFromApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertFromInvalidKeyApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertFromSpecificCultureApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertToApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertToInvalidDestinationTypeApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertToInvalidKeyApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertToKeyDownApp_WpfApplication_Window")]
        [Variation("KeyConverterConvertToKeyNoneApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterCanConvertFromApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterCanConvertToApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertFromApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertFromCombination2KeyApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertFromCombination3KeyApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertFromSpecificCultureApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertFromWindowsKeyApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertToApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertToInvalidDestinationTypeApp_WpfApplication_Window")]
        [Variation("ModifierKeysConverterConvertToInvalidKeyApp_WpfApplication_Window")]
        [Variation("MostRecentInputDeviceApp_WpfApplication_Window")]
        [Variation("MostRecentInputDeviceMouseMoveApp_WpfApplication_Window")]
        [Variation("MouseButtonAfterMessageBoxApp_WpfApplication_Window")]
        [Variation("MouseButtonAfterWinFormsMessageBoxApp_WpfApplication_Window")]
        [Variation("MouseButtonOnContentElementApp_WpfApplication_Window")]
        [Variation("MouseButtonOnFrameWithContentElementApp_WpfApplication_Window")]
        [Variation("MouseButtonOnFrameWithFrameworkElementApp_WpfApplication_Window")]
        [Variation("MouseButtonOnFrameworkElementApp_WpfApplication_Window")]
        [Variation("MouseCaptureButtonDownApp_WpfApplication_Window")]
        [Variation("MouseCapturedApp_WpfApplication_Window")]
        [Variation("MouseClassButtonApp_WpfApplication_Window")]
        [Variation("MouseClassButtonUpApp_WpfApplication_Window")]
        [Variation("MouseCursorRightBorderApp_WpfApplication_Window")]
        [Variation("MouseCursorToStringApp_WpfApplication_Window")]
        [Variation("MouseCursorToStringCursorTypeApp_WpfApplication_Window")]
        [Variation("MouseDirectlyOverApp_WpfApplication_Window")]
        [Variation("MouseDirectlyOverDrawingVisualApp_WpfApplication_Window")]
        [Variation("MouseDirectlyOverPointAnimationVisualApp_WpfApplication_Window")]
        [Variation("MouseDirectlyOverRemoveWindowContentApp_WpfApplication_Window")]
        [Variation("MouseDirectlyOverVisualApp_WpfApplication_Window")]
        [Variation("MouseDoubleClickApp_WpfApplication_Window")]
        [Variation("MouseDoubleClickButtonStateApp_WpfApplication_Window")]
        [Variation("MouseEnterImmediateMoveApp_WpfApplication_Window")]
        [Variation("MouseGetPositionApp_WpfApplication_Window")]
        [Variation("MouseGetPositionInvalidInputElementApp_WpfApplication_Window")]
        [Variation("MouseLeaveDoubleAnimationApp_WpfApplication_Window")]
        [Variation("MouseLeaveDoubleAnimationRepeatApp_WpfApplication_Window")]
        [Variation("MouseRightButtonDownApp_WpfApplication_Window")]
        [Variation("MouseTripleClickApp_WpfApplication_Window")]
        [Variation("MouseUpdateCursorApp_WpfApplication_Window")]
        [Variation("PreProcessEventPeekInputApp_WpfApplication_Window")]
        [Variation("PreProcessEventPopInputApp_WpfApplication_Window")]
        [Variation("PreviewMouseButtonOnContentElementApp_WpfApplication_Window")]
        [Variation("PreviewMouseButtonOnFrameworkElementApp_WpfApplication_Window")]
        [Variation("TextInputAltCharApp_WpfApplication_Window")]
        [Variation("TextInputControlCharApp_WpfApplication_Window")]
        [Variation("TextInputShiftCharApp_WpfApplication_Window")]
        [Variation("UIElementCaptureToSubtreeContentHostMouseClickApp_WpfApplication_Window")]
        [Variation("UIElementCaptureToSubtreeMouseClickApp_WpfApplication_Window")]
        [Variation("UIElementInputHitTestApp_WpfApplication_Window")]
        [Variation("UIElementInputHitTestContentHostApp_WpfApplication_Window")]
        [Variation("UIElementInputHitTestTransparentBrushApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorAniFileApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorAniStreamApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorCurFileApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorCurStreamApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorOverrideApp_WpfApplication_Window")]
        [Variation("UIElementMouseEnterBorderApp_WpfApplication_Window", Disabled=true)] // Failing regularly on 3.5 and 4.0
        [Variation("UIElementQueryCursorApp_WpfApplication_Window")]
        [Variation("UIElementReleaseMouseCaptureLostCaptureApp_WpfApplication_Window")]

        //NonBrowser:
        [Variation("CaptureAfterNonModalWindowApp_WpfApplication_Window")]
        [Variation("CaptureAfterModalWindowApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorAniHandleApp_WpfApplication_Window")]
        [Variation("ContentElementMouseCursorCurHandleApp_WpfApplication_Window")]
        [Variation("CursorTypeConverterConvertFromApp_WpfApplication_Window",Disabled=true)]
        [Variation("InputManagerEventsApp_WpfApplication_Window")]
        [Variation("InputManagerProcessEventPeekInputApp_WpfApplication_Window")]
        [Variation("InputManagerProcessEventPushInputNullApp_WpfApplication_Window")]
        [Variation("InputManagerProcessEventPushInputPromoteInvalidApp_WpfApplication_Window")]
        [Variation("InputManagerProcessEventPushInputPromoteNullApp_WpfApplication_Window")]
        [Variation("MultiSourceFocusableApp_WpfApplication_Window")]
        [Variation("MultiWindowCaptureApp_WpfApplication_Window")]
        [Variation("MultiWindowCaptureMouseWheelApp_WpfApplication_Window")]
        [Variation("MultiWindowFocusableApp_WpfApplication_Window")]
        [Variation("MultiWindowFocusApp_WpfApplication_Window")]
        [Variation("SourceChangedEventTest_WpfApplication_Window")]
        [Variation("TextInputAltNumKeyApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorCurHandleApp_WpfApplication_Window")]
        [Variation("UIElementMouseCursorAniHandleApp_WpfApplication_Window")]

        //**********************************************************************************************************
        //SET 3:  ApplicationType=WpfApplication / HostType=WindowsFormSource  (Pri1 Group)
        //**********************************************************************************************************
        //Pri1 group:
        // [Variation("UIElementIsMouseDirectlyOverApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        [Variation("CaptureAfterMessageBoxApp_WpfApplication_WindowsFormSource")]
        [Variation("CaptureAfterRemoveElementApp_WpfApplication_WindowsFormSource")]
        [Variation("CaptureAfterRemoveElementEnabledFalseApp_WpfApplication_WindowsFormSource", Disabled = true)] // Failing regularly on 3.5 and 4.0
        [Variation("CaptureAfterRemoveElementRecaptureApp_WpfApplication_WindowsFormSource")]
        [Variation("CaptureAfterWinFormsMessageBoxApp_WpfApplication_WindowsFormSource")]
        // [Variation("CaptureDoubleAnimationApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        // [Variation("CaptureDoubleAnimationPositionApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        [Variation("CaptureMoveNoSourceApp_WpfApplication_WindowsFormSource")]
        [Variation("CapturePositionApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementCaptureApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementCaptureToSubTreeApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementCaptureToSubtreeMouseClickApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementFocusApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementIsEnabledApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementIsFocusWithinApp_WpfApplication_WindowsFormSource")]
        // [Variation("ContentElementIsMouseDirectlyOverApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        [Variation("ContentElementIsMouseOverApp_WpfApplication_WindowsFormSource")]
        // [Variation("ContentElementIsMouseOverChangedApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        [Variation("ContentElementKeyDownApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseButtonApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseButtonImmediateMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseCursorAniFileApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseCursorAniStreamApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseCursorCurFileApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseCursorCurStreamApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseDirectlyOverApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseDownOutsideCapturedElementApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseDownOutsideCapturedElementCanvasApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseEnterApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseEnterBorderApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseEnterBorderImmediateMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseEnterImmediateMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseLeftButtonDownApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseRightButtonDownApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementMouseWheelApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementPreviewFocusApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementQueryCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("ContentElementReleaseMouseCaptureApp_WpfApplication_WindowsFormSource")]
        [Variation("CursorTypeConverterCanConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("CursorTypeConverterCanConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("CursorTypeConverterCanConvertToNullContextApp_WpfApplication_WindowsFormSource")]
        [Variation("CursorTypeConverterConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("FocusAfterMessageBoxApp_WpfApplication_WindowsFormSource")]
        [Variation("FocusAfterModalWindowApp_WpfApplication_WindowsFormSource")]
        [Variation("FocusAfterNonModalWindowApp_WpfApplication_WindowsFormSource")]
        [Variation("FocusAfterRemoveElementRefocusApp_WpfApplication_WindowsFormSource")]
        [Variation("FocusInvalidInputElementApp_WpfApplication_WindowsFormSource")]
        [Variation("ForceCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkContentElementFocusableApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkContentElementIsFocusWithinApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkContentElementIsMouseOverApp_WpfApplication_WindowsFormSource")]
        // [Variation("FrameworkContentElementIsMouseOverChangedApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        // [Variation("FrameworkContentElementQueryCursorMouseMoveApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        // [Variation("FrameworkElementForceCursorElementMoveApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        [Variation("FrameworkElementInputHitTestDisabledApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkElementInputHitTestInvisibleApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkElementIsFocusWithinApp_WpfApplication_WindowsFormSource")]
        // [Variation("FrameworkElementIsMouseOverChangedApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        // [Variation("FrameworkElementMouseOverVisibilityPropertyTriggerApp_WpfApplication_WindowsFormSource")] // [DISABLE WHILE PORTING]
        // [Variation("FrameworkElementQueryCursorElementMoveApp_WpfApplication_WindowsFormSource")]
        // [Variation("FrameworkElementQueryCursorMouseMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkElementQueryCursorNullCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("FrameworkElementQueryCursorSendMessageApp_WpfApplication_WindowsFormSource")]
        // [Variation("FrameworkElementQueryCursorUpdateCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyAfterMessageBoxApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyAfterWinFormsMessageBoxApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyboardDeviceIsKeyToggledApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyboardFocusedApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyboardFocusedFrameApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyboardStateApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterCanConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterCanConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertFromInvalidKeyApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertFromSpecificCultureApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertToInvalidDestinationTypeApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertToInvalidKeyApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertToKeyDownApp_WpfApplication_WindowsFormSource")]
        [Variation("KeyConverterConvertToKeyNoneApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterCanConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterCanConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertFromApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertFromCombination2KeyApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertFromCombination3KeyApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertFromSpecificCultureApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertFromWindowsKeyApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertToApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertToInvalidDestinationTypeApp_WpfApplication_WindowsFormSource")]
        [Variation("ModifierKeysConverterConvertToInvalidKeyApp_WpfApplication_WindowsFormSource")]
        [Variation("MostRecentInputDeviceApp_WpfApplication_WindowsFormSource")]
        [Variation("MostRecentInputDeviceMouseMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseButtonAfterMessageBoxApp_WpfApplication_WindowsFormSource")]
        // [Variation("MouseButtonAfterWinFormsMessageBoxApp_WpfApplication_WindowsFormSource")] [DISABLE WHILE PORTING]
        [Variation("MouseButtonOnContentElementApp_WpfApplication_WindowsFormSource")]
        // [Variation("MouseButtonOnFrameWithContentElementApp_WpfApplication_WindowsFormSource")] [DISABLE WHILE PORTING]
        // [Variation("MouseButtonOnFrameWithFrameworkElementApp_WpfApplication_WindowsFormSource")] [DISABLE WHILE PORTING]
        // [Variation("MouseButtonOnFrameworkElementApp_WpfApplication_WindowsFormSource")] [DISABLE WHILE PORTING]
        // [Variation("MouseCaptureButtonDownApp_WpfApplication_WindowsFormSource")] [DISABLE WHILE PORTING]
        [Variation("MouseCapturedApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseClassButtonApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseClassButtonUpApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseCursorRightBorderApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseCursorToStringApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseCursorToStringCursorTypeApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDirectlyOverApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDirectlyOverDrawingVisualApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDirectlyOverPointAnimationVisualApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDirectlyOverRemoveWindowContentApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDirectlyOverVisualApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDoubleClickApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseDoubleClickButtonStateApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseEnterImmediateMoveApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseGetPositionApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseGetPositionInvalidInputElementApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseLeaveDoubleAnimationApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseLeaveDoubleAnimationRepeatApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseRightButtonDownApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseTripleClickApp_WpfApplication_WindowsFormSource")]
        [Variation("MouseUpdateCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("PreProcessEventPeekInputApp_WpfApplication_WindowsFormSource")]
        [Variation("PreProcessEventPopInputApp_WpfApplication_WindowsFormSource")]
        [Variation("PreviewMouseButtonOnContentElementApp_WpfApplication_WindowsFormSource")]
        [Variation("PreviewMouseButtonOnFrameworkElementApp_WpfApplication_WindowsFormSource")]
        [Variation("TextInputAltCharApp_WpfApplication_WindowsFormSource")]
        [Variation("TextInputControlCharApp_WpfApplication_WindowsFormSource")]
        [Variation("TextInputShiftCharApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementCaptureToSubtreeContentHostMouseClickApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementCaptureToSubtreeMouseClickApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementInputHitTestApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementInputHitTestContentHostApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementInputHitTestTransparentBrushApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementMouseCursorAniFileApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementMouseCursorAniStreamApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementMouseCursorCurFileApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementMouseCursorCurStreamApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementMouseCursorOverrideApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementMouseEnterBorderApp_WpfApplication_WindowsFormSource", Disabled = true)] // Failing regularly on 3.5 and 4.0
        [Variation("UIElementQueryCursorApp_WpfApplication_WindowsFormSource")]
        [Variation("UIElementReleaseMouseCaptureLostCaptureApp_WpfApplication_WindowsFormSource")]

        /******************************************************************************
        * Function:          InputApp Constructor
        ******************************************************************************/
        public InputApp(string arg)
        {
            char[] delimiters = new char[] { '_' };
            String[] argArray = arg.Split(delimiters);
            if (argArray.Length != 3)
            {
                throw new Microsoft.Test.TestSetupException("Three parameters delimited by underscores must be specified.");
            }

            _testName = argArray[0];
            _applicationType = argArray[1];
            _testHost = argArray[2];

            RunSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps


        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult Initialize()
        {
            #pragma warning disable 618
            Assembly asmTestRuntime = Assembly.LoadWithPartialName("TestRunTime");
            if (asmTestRuntime == null)
            {
                throw (new ArgumentException("Could not load TestRunTime assembly."));
            }

            Assembly asmCoreTestsTrusted = Assembly.LoadWithPartialName("CoreTestsTrusted");
            if (asmCoreTestsTrusted == null)
            {
                throw (new ArgumentException("Could not load CoreTestsTrusted assembly."));
            }

            Assembly asmElementServicesTest = Assembly.LoadWithPartialName("ElementServicesTest");
            if (asmElementServicesTest == null)
            {
                throw (new ArgumentException("Could not load ElementServicesTest assembly."));
            }
            #pragma warning restore 618

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            DictionaryStore.StartServer();
            GlobalLog.LogStatus("In InputApp.RunTest...");

            GlobalLog.LogStatus("*******************************************");
            GlobalLog.LogStatus("ApplicationType: " + _applicationType);
            GlobalLog.LogStatus("Host:            " + _testHost);
            GlobalLog.LogStatus("Running:         " + _testName);
            GlobalLog.LogStatus("*******************************************");

            ApplicationType appType = (ApplicationType)Enum.Parse(typeof(ApplicationType), _applicationType);

            CommonStorage.CleanAll();
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), _testHost);

            ContainerVariationItem cvi = new ContainerVariationItem();
            cvi.Execute(appType, hostType, "Input", _testName);
            //A test failure will be handled by an Exception thrown during Verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
