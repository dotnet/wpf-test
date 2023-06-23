 using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    public static class InfoTextBoxTestValidation
    {
        public static bool VerifyOpacity(InfoTextBox TargetElement, string ExpectedState, double ExpectedOpacity)
        {
            if (TargetElement == null)
            {
                throw new ArgumentNullException("TargetElement",
                    "Trouble with XTC file - \"TargetElement\" must be an instantiated InfoTextBox control.");
            }

            InfoTextBoxTestState? wantState = Enum.Parse(typeof(InfoTextBoxTestState),ExpectedState,true) as InfoTextBoxTestState?;
            
            if (wantState == null) 
            {
                throw new ArgumentException("Trouble with XTC ExpectedState argument -- unrecognized value", "ExpectedState");
            }

            if (wantState == InfoTextBoxTestState.UT || wantState == InfoTextBoxTestState.ST)
            {
                if (TargetElement.HasText != true)
                {
                    throw new ArgumentException("Trouble with XTC -- for ExpectedState == UT or ExpectedState == ST, " +
                                                "expected TextBoxInfo property not empty");
                }
            }
            else
            {
                if (TargetElement.HasText == true)
                {
                    throw new ArgumentException("Trouble with XTC -- for ExpectedState == UF or ExpectedState == SF, " +
                                                "expected empty TextBoxInfo property");

                }
            }

            DependencyObject animatedDO = null;

            animatedDO = FindFirstTextBlockInVisualTree((DependencyObject)TargetElement);
            UIElement uiElement = (UIElement)animatedDO;
            
            if (uiElement.Opacity != ExpectedOpacity)
            {
                TestLog.Current.LogEvidence("!!!ERROR: Actual TextInfoBox.InfoBoxText opacity not the expected value.");
                TestLog.Current.LogEvidence("..Detail: Actual opacity = " + uiElement.Opacity.ToString() +
                                                    "; Expected opacity = " + ExpectedOpacity.ToString());
                return false;
            }
            return true;
        }

        internal static DependencyObject FindFirstTextBlockInVisualTree(DependencyObject obj)
        {
            DependencyObject retObj = null;
            if (obj.GetType() == typeof(TextBlock))
            {
                retObj = obj;
            }
            else
            {
                int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(obj);
                for (int i = 0; i < count; i++)
                {
                    retObj = FindFirstTextBlockInVisualTree(System.Windows.Media.VisualTreeHelper.GetChild(obj, i));
                    if (retObj != null) break;
                }
            }
            return retObj;
        }
    }
}
