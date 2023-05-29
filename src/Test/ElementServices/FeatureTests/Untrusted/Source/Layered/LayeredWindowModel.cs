// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: LayeredWindow model class and model state.
 * 
 *
 
  
 * Revision:         $Revision: 7 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/Source/LayerdWindowModel.cs $
********************************************************************/
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;

using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Source.LayeredWindow
{
    /// <summary>
    /// LayeredWindowModel Model class
    /// </summary>  
    [Model(@"FeatureTests\ElementServices\LayeredWindowModel_Pairwise.xtc", 0, @"Source\LayeredWindow", TestCaseSecurityLevel.FullTrust, "LayeredWindowModel", ExpandModelCases = true, Area = "AppModel")]
    public class LayeredWindowModel: CoreModel
    {
        /// <summary>
        /// Creates a LayeredWindowModel Model instance.
        /// </summary>
        public LayeredWindowModel(): base()
        {
            Name = "LayeredWindowModel";
            Description = "LayeredWindowModel Model";
            
            // Add Action Handlers.
            AddAction("Go", new ActionHandler(RunTest));
        }

        /// <summary>
        /// Single action for this model.  
        /// </summary>
        /// <remarks>Handler for RunTest</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RunTest(State endState, State inParams, State outParams)
        {            
            // Initialize model state instance to be used in helpers and verifiers.
            _state = new LayeredWindowModelState(inParams);
            _state.LogState();

            // Create windows.
            _lwh = new LayeredWindowModelHelper(_state);
            _backgroundWindow = _lwh.BuildBackgroundWindow(500, 500);
            _layeredSource = _lwh.BuildLayeredWindow();
            _button = _lwh.Button;

            //// Validate layered window behavior.
            Validation(null);
            
            // Helpful pause.
            DispatcherHelper.DoEvents(2000);
            return true;
        }

        /// <summary>
        /// Perform validation of mouse entry, exit and clicks on layered window and it's button element.
        /// </summary>
        object Validation(object o)
        {
            //VerifyStyle();

            CoreLogger.LogStatus("Verifying button click with mouse.");
            VerifyButtonClick();
            CoreLogger.LogStatus("Pass", ConsoleColor.Green);

            //if (_state.OpacityMask == true)
            //{
            //    CoreLogger.LogStatus("Verifying opacity mask");
            //    VerifyOpacityMask();
            //}

            // Validate animation.
            // Animation will either animate from or to opacity 0.0, trigger animation and verify
            // mouse stuff.
            VerifyAnimation();

            // Move target
            VerifyMove();

            
            // Resize target
            VerifyResize();

            VerifyShowWindow();
            return null;
        }


        private void VerifyAnimation()
        {
            if (_state.Animate == "None")
                return;

            // Right clicking starts opacity animation.
            MouseHelper.Click(MouseButton.Right, _button, MouseLocation.Center);

            DispatcherHelper.DoEvents(2500);

            // 

            // Make sure button still clicks (or doesn't).
            VerifyButtonClick();
        }

        private void VerifyMove()
        {
            _lwh.ArrangeTestWindows();

            CoreLogger.LogStatus("Moving button");
            
            // Move button right a distance equal to its width.
            _lwh.MoveButton(_button.Width, 0);

            DispatcherHelper.DoEvents(250);
            VerifyButtonClick();

            // 
        }

        private void VerifyResize()
        {
            CoreLogger.LogStatus("Resize button");

            // Half button size.
            _button.Width = _button.Width * 0.5;
            _button.Height = _button.Height * 0.5;

            DispatcherHelper.DoEvents(250);

            // Clicking center right will catch resized area.
            VerifyButtonClick();
        }

        /// <summary>
        /// 
        /// </summary>
        private void VerifyOpacityMask()
        {
            VerifyButtonClick(MouseLocation.CenterLeft);
            VerifyButtonClick(MouseLocation.Center);
            VerifyButtonClick(MouseLocation.CenterRight);
        }

        /// <summary>
        /// Execute ShowWindow call with SW_HIDE, SW_MAXIMIZE, SW_MINIMIZE, SW_RESTORE or SW_SHOW,
        /// call SW_RESTORE, and verify window still clicks as expected.
        /// </summary>
        private void VerifyShowWindow()
        {
            CoreLogger.LogStatus("Calling ShowWindow(" + _state.ShowWindow + ")");
            switch (_state.ShowWindow)
            {
                case "SW_HIDE":
                    _lwh.LayeredShowWindowCall(NativeConstants.SW_HIDE);
                    break;
                case "SW_MAXIMIZE":
                    _lwh.LayeredShowWindowCall(NativeConstants.SW_MAXIMIZE);
                    break;
                case "SW_MINIMIZE":
                    _lwh.LayeredShowWindowCall(NativeConstants.SW_MINIMIZE);
                    break;
                case "SW_RESTORE":
                    _lwh.LayeredShowWindowCall(NativeConstants.SW_RESTORE);
                    break;
                case "SW_SHOW":
                    _lwh.LayeredShowWindowCall(NativeConstants.SW_SHOW);
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ShowWindow " + _state.ShowWindow + " not supported.");                    
            }

            CoreLogger.LogStatus("Calling ShowWindow(SW_RESTORE)");
            _lwh.LayeredShowWindowCall(NativeConstants.SW_RESTORE);

            CoreLogger.LogStatus("Validating click after ShowWindow calls");
            DispatcherHelper.DoEvents(250);

            VerifyButtonClick();
        }

        /// <summary>
        /// Verify button click when button is clicked by the mouse. Verify clicks that should
        /// go to the background window.
        /// </summary>
        private void VerifyButtonClick()
        {
            VerifyButtonClick(MouseLocation.Center);
        }

        /// <summary>
        /// Verify button click when button is clicked by the mouse. Verify clicks that should go to 
        /// background window.
        /// </summary>
        private void VerifyButtonClick(MouseLocation where)
        {
            // Reset click counts to zero.
            _lwh.LayeredClickCount = 0;
            _lwh.BackgroundClickCount = 0;

            // Click button
            MouseHelper.Click(MouseButton.Left, _button, where);

            CoreLogger.LogStatus("  Layered window clicks:    " + _lwh.LayeredClickCount);
            CoreLogger.LogStatus("  Background window clicks: " + _lwh.BackgroundClickCount);
            
            if ((_lwh.LayeredClickCount == 0) && PredictLayeredWindowClick())
            {
                throw new Microsoft.Test.TestValidationException("Layered window did not receive expected click.");
            }

            if ((_lwh.LayeredClickCount > 0) && !PredictLayeredWindowClick())
            {
                throw new Microsoft.Test.TestValidationException("Layered window received unexpected click.");
            }

            if ((_lwh.BackgroundClickCount == 0) && PredictBackgroundWindowClick())
            {
                throw new Microsoft.Test.TestValidationException("Background window did not receive expected click");
            }

            if ((_lwh.BackgroundClickCount > 0) && !PredictBackgroundWindowClick())
            {
                throw new Microsoft.Test.TestValidationException("Background window received unexpected click");
            }        
        }

        /// <summary>
        /// Return true if button in layered window should receive click.
        /// </summary>
        bool PredictLayeredWindowClick()
        {
            return PredictLayeredWindowClick(MouseLocation.Center);
        }

        /// <summary>
        /// Return true if button in layered window should receive click.
        /// </summary>
        bool PredictLayeredWindowClick(MouseLocation where)
        {
            //
            // We expect the button in the layered window to receive a click except when...
            //

            // Style
            if (_state.Style == Styles.WS_DISABLED)
            {
                CoreLogger.LogStatus("    Layered window has style WS_DISABLED and should not receive click.");
                return false;
            }
            if (_state.Style == Styles.WS_MINIMIZE)
            {
                CoreLogger.LogStatus("    Layered window has style WS_MINIMIZED and should not receive click.");
                return false;
            }



            // Extended Style
            if (_state.ExStyle == ExStyles.WS_EX_TRANSPARENT)
            {
                CoreLogger.LogStatus("    Layered window has extended style WS_EX_TRANSPARENT and should not receive click.");
                return false;
            }

            // Element Visibility
            if (_state.ElementVisibility != Visibility.Visible)
            {
                CoreLogger.LogStatus("    Layered window element has visibility hidden or collapsed and should not receive click.");
                return false;
            }

            if (_state.Opacity <= _alphaTolerance)
            {
                CoreLogger.LogStatus("    Layered window element has opacity below alpha tolerance and should not receive click.");
                return false;
            }

            if (_state.IsEnabled == false)
            {
                CoreLogger.LogStatus("    Layered window element is disabled and should not receive click.");
                return false;
            }

            if (_state.IsHitTestVisible == false)
            {
                CoreLogger.LogStatus("    Layered window element is not hit test visible");
                return false;
            }

            // Horizontally, OpacityMask is 1/3 solid, 1/3 semitransparent, 1/3 transparent.
            // The transparent section on the right should not produce click events.
            //if (_state.OpacityMask && (where == MouseLocation.CenterRight))
            //    return false;

            //if (_state.Animate == "ToTransparent" && _hasAnimated)
            //    return false;

            //if (_state.Animate == "FromTransparent" && !_hasAnimated)
            //    return false;

            // Default, layered window receives click.
            CoreLogger.LogStatus("    Layered window element should receive click.");
            return true;
        }

        /// <summary>
        /// Return true if background window should receive click.
        /// </summary>
        bool PredictBackgroundWindowClick()
        {
            return PredictBackgroundWindowClick(MouseLocation.Center);
        }

        /// <summary>
        /// Return true if background window should receive click.
        /// </summary>
        bool PredictBackgroundWindowClick(MouseLocation where)
        {

            if (_state.Style == Styles.WS_MINIMIZE)
            {
                CoreLogger.LogStatus("    Layered window has style WS_MINIMIZED, background shouldn't receive click (nothing to click)");
                return false;
            }

            if (_state.ElementVisibility == Visibility.Collapsed)
            {
                CoreLogger.LogStatus("    Layered window element is collapsed, background should not receive click (nothing to click).");
                return false;
            }


            //if (_state.ExStyle == ExStyles.WS_EX_NOACTIVATE)
            //{
            //    CoreLogger.LogStatus("    Layered window has extend style WS_EX_NOACTIVATE, background shouldn't receive click (nothing to click)");
            //    return false;
            //}


            // The layered window has extended window style WS_EX_TRANSPARENT (transparent to input events).
            if (_state.ExStyle == ExStyles.WS_EX_TRANSPARENT)
            {
                CoreLogger.LogStatus("    Layered window has extended style WS_EX_TRANSPARENT, background should receive click.");
                return true;
            }

            // The layered window is hidden or collapsed.
            if (_state.ElementVisibility == Visibility.Hidden)
            {
                CoreLogger.LogStatus("    Layered window element is hidden or collapsed, background should receive click.");
                return true;
            }
          

            // The layered window has pixels that are below the alpha tolerance.
            if (_state.Opacity <= _alphaTolerance)
            {
                CoreLogger.LogStatus("    Pixels are below alpha tolerance, background should receive click.");
                return true;
            }

            //if (_state.IsHitTestVisible == false)
            //{
            //    return true;
            //}

            // The layered window has window style WS_DISABLED.
            if (_state.Style == Styles.WS_DISABLED)
            {
                CoreLogger.LogStatus("    Layered window has style WS_DISABLED, background should not receive click.");
                return false;
            }

            // Default, background window doesn't receive click.
            CoreLogger.LogStatus("    Background window should not receive click.");
            return false;
        }

        // private bool _hasAnimated = false;

        // Pixels with alpha values less than this tolerance will not receive mouse events.
        // todo: figure out what this value is supposed to be.
        private const double _alphaTolerance = 0.0001;

        private Window _backgroundWindow = null;
        private HwndSource _layeredSource = null;
        private Button _button = null;

        private LayeredWindowModelHelper _lwh = null;
        private LayeredWindowModelState _state;
    }

    /// <summary>
    /// Parses State into fields of the correct type.
    /// </summary>
    internal class LayeredWindowModelState
    {
        State _state;
        public LayeredWindowModelState(State state)
        {
            // Save state logging.
            _state = state;

            Style = GetWindowsConstant(state["Style"], "Styles");
            ExStyle = GetWindowsConstant(state["ExStyle"], "ExStyles");
            ElementVisibility = (Visibility)Enum.Parse(ElementVisibility.GetType(), state["Visibility"], true);

            // Because MDE doesn't handle floating point numbers well:
            switch (state["Opacity"].ToLower())
            {
                case "transparent":
                    Opacity = 0.0;
                    break;
                case "semitransparent":
                    Opacity = 0.5;
                    break;
                case "opaque":
                    Opacity = 1.0;
                    break;
            }
            
            IsEnabled = state["IsEnabled"].ToLower().Equals("true");
            IsHitTestVisible = state["IsHitTestVisible"].ToLower().Equals("true");
            Animate = state["Animate"];
            Resize = state["Resize"].ToLower().Equals("true");
            Move = state["Move"].ToLower().Equals("true");
            ShowWindow = state["ShowWindow"];
            IgnoredExStyle = GetWindowsConstant(state["IgnoredExStyle"], "ExStyles");
            StrippedExStyle = GetWindowsConstant(state["StrippedExStyle"], "ExStyles");
            OpacityMask = state["OpacityMask"].ToLower().Equals("true");
        }

        /// <summary>
        /// Get an a Windows constant value by name. Reflects on static LayeredWindow class typeKey
        /// to get value for static constant constant.
        /// </summary>
        private int GetWindowsConstant(string constant, string typeKey)
        {
            Type constantType = Type.GetType("Avalon.Test.CoreUI.Source.LayeredWindow." + typeKey);
            FieldInfo fi = constantType.GetField(constant);

            return (int)fi.GetValue(null);
        }
       
        public void LogState()
        {
            foreach (DictionaryEntry de in _state)
            {
                CoreLogger.LogStatus("  " + de.Key + " " + de.Value, ConsoleColor.Yellow);
            }
        }

        public int Style;
        public int ExStyle;
        public Visibility ElementVisibility;
        public double Opacity;
        public bool IsEnabled;
        public bool IsHitTestVisible;
        // 
        public string Animate;
        // 
        public bool Resize;
        public bool Move;
        public string ShowWindow;
        public int IgnoredExStyle;
        public int StrippedExStyle;
        public bool OpacityMask;
    }
}

