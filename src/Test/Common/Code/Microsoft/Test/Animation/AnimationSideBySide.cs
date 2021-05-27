// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------------
//
// 
//
// Description: Provides side by side (rendering and values-only) animation verification
//
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Side by Side animation verification
    /// </summary>
    public class SideBySideVerifier
    {
        private Window aniWin;
        private string windowTitle;
        private AnimationValidator myValidator = new AnimationValidator();
        private ImageComparator imageCompare = new ImageComparator();
        private System.Drawing.Rectangle clientRect;
 
        private AnimatableAnimationProps[] registeredAnimatableObjs = new AnimatableAnimationProps[25];
        private UIElementAnimationProps[] registeredUIElementObjs = new UIElementAnimationProps[25];
        private int registeredAnimatableCount = 0;
        private int registeredUIElementCount = 0;
        private bool windowRegistered = false;
        private IntPtr hWnd;

        /// <summary>
        /// Side by Side verification verbose log
        /// </summary>
        public string verboseLog;

        /// <summary>
        /// Side by Side verificationtolerance
        /// </summary>
        public double toleranceInPercent = .2;

        /// <summary>
        /// Private Side by Side AnimatableAnimationProps
        /// </summary>
        private struct AnimatableAnimationProps
        {
            public Animatable animObj;
            public DependencyProperty animProp;
            public AnimationClock animClock;
            public object baseValue;
        }

        /// <summary>
        /// Private Side by Side UIElementAnimationProps
        /// </summary>
        private struct UIElementAnimationProps
        {
            public UIElement animObj;
            public DependencyProperty animProp;
            public AnimationClock animClock;
            public object baseValue;
        }

        // constructors
        /// <summary>
        /// Side by Side verifier constructor that takes a window. Create an internal client rectangle from the window
        /// </summary>
        /// <param name="win">Window to perform the snapshots on</param>
        public SideBySideVerifier(Window win) 
        { 
            if (win != null)
            {
                this.aniWin = win;
                windowTitle = win.Title;
                clientRect = new System.Drawing.Rectangle((int)aniWin.Left, (int)aniWin.Top, (int)aniWin.Width, (int)aniWin.Height);
                windowRegistered = true;

                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(win);
                this.hWnd = windowInteropHelper.Handle;
            }
        }

        /// <summary>
        /// Side by Side verifier constructor that doesn't take a window
        /// </summary>
        public SideBySideVerifier() 
        { 
            windowRegistered = false;
        }

        /// <summary>
        /// Side by Side verification call to register your animation with the verifier
        /// </summary>
        /// <param name="DO">UI Element Dependency Object</param>
        /// <param name="DP">Dependency Property</param>
        /// <param name="clock">AnimationClock to register</param>
        public void RegisterAnimation(UIElement DO, DependencyProperty DP, AnimationClock clock)
        {
            UIElementAnimationProps temp = new UIElementAnimationProps();
            temp.animObj = DO;
            temp.animProp = DP;
            temp.baseValue = DO.GetAnimationBaseValue(DP);

            temp.animClock = clock;
            registeredUIElementObjs[registeredUIElementCount] = temp;
            registeredUIElementCount ++;
        }

        /// <summary>
        /// Side by Side verification call to register your animation with the verifier
        /// </summary>
        /// <param name="DO">Animatable Dependency Object</param>
        /// <param name="DP">Dependency Property</param>
        /// <param name="clock">AnimationClock to register</param>
        public void RegisterAnimation(Animatable DO, DependencyProperty DP, AnimationClock clock)
        {
            AnimatableAnimationProps temp = new AnimatableAnimationProps();
            temp.animObj = DO;
            temp.animProp = DP;
            temp.baseValue = DO.GetAnimationBaseValue(DP);

            temp.animClock = clock;
            registeredAnimatableObjs[registeredAnimatableCount] = temp;
            registeredAnimatableCount ++;
        }
        

        /// <summary>
        /// Function to determine if the object is a UIElement or not. All of the DO's coming in for registration
        /// will be of either type UIElement or Animatable
        /// </summary>
        private bool IsUIElement(object testObject)
        {
            try
            {
                UIElement conversion = (UIElement)testObject;
            }
            catch (System.InvalidCastException e)
            {
                string temp = e.Message;
                return false;
            }

            return true;
        }

        /******************************************************************************
        * Function:         Verify
        ******************************************************************************/
        /// <summary>
        /// Side by Side verification call to perform verifcation for a specific time
        /// </summary>
        public bool Verify(double curTime)
        {
            // if there is no window registered, we can't proceede
            if (!windowRegistered)
            {
                verboseLog += "\n COULD NOT DO VISUAL VALIDATION!!!!!!!!!!! No Window was specified. Defaulting to ValuesOnlyVerify. For VisualValidation, use the following constructor: SideBySideVerifier(Window win) ";
                return ValuesOnlyVerify(curTime);
            }

            bool passResult = true;
            bool valuesCheckOK = true;
            // Snapshot

            System.Threading.Thread.Sleep(150);
            this.aniWin.Title = windowTitle;

            System.Drawing.Bitmap animatedCapture = ImageUtility.CaptureScreen(hWnd,true);

            // Set BaseValue to AnimationCalculator Value
            verboseLog = "\n Side by Side Verifier for time " + curTime;

            if ( (registeredAnimatableCount == 0) && (registeredUIElementCount == 0)) 
            { 
                verboseLog += " No animations could be registered - nothing to verify"; 
                return false; 
            }


            // walk all registered animatables, obtain expected and actual values and compare
            for (int i=0;i < registeredAnimatableCount; i++)
            {
                object expectedValue = myValidator.Verify(registeredAnimatableObjs[i].animClock,registeredAnimatableObjs[i].baseValue,curTime);
                DependencyObject current = registeredAnimatableObjs[i].animObj;
                object currentValue = current.GetValue(registeredAnimatableObjs[i].animProp);

                verboseLog += "\n" + current.GetType().ToString().Substring(current.GetType().ToString().LastIndexOf(".") +1);
                verboseLog += " " + registeredAnimatableObjs[i].animProp.ToString().Substring(registeredAnimatableObjs[i].animProp.ToString().LastIndexOf(".") +1) + " Anim:: prog: ";
                verboseLog += registeredAnimatableObjs[i].animClock.CurrentProgress.ToString() + "   exp : " + expectedValue.ToString() + " act: " + currentValue.ToString();

                if (!myValidator.WithinTolerance(currentValue,expectedValue,this.toleranceInPercent))
                { 
                    passResult = false; verboseLog += " <-----"; valuesCheckOK = false;
                }

                current.SetValue(registeredAnimatableObjs[i].animProp, expectedValue);
                ((System.Windows.Media.Animation.Animatable)current).ApplyAnimationClock(registeredAnimatableObjs[i].animProp,null);

            }


            // walk all registered UI Elements, obtain expected and actual values and compare
            for (int i=0;i < registeredUIElementCount; i++)
            {
                object expectedValue = myValidator.Verify(registeredUIElementObjs[i].animClock,registeredUIElementObjs[i].baseValue,curTime);
                object currentValue = registeredUIElementObjs[i].animObj.GetValue(registeredUIElementObjs[i].animProp);
                UIElement current = registeredUIElementObjs[i].animObj;


                verboseLog += "\n" + current.GetType().ToString().Substring(current.GetType().ToString().LastIndexOf(".") +1);
                verboseLog += registeredUIElementObjs[i].animProp.ToString().Substring(registeredUIElementObjs[i].animProp.ToString().LastIndexOf(".") +1) + " Anim:: prog: ";
                verboseLog += registeredUIElementObjs[i].animClock.CurrentProgress.ToString() + "   exp : " + expectedValue.ToString() + " act: " + currentValue.ToString();


                // check to make sure the values match. If they do, setting valuesCheckOk to 
                if (!myValidator.WithinTolerance(currentValue,expectedValue,this.toleranceInPercent))
                { 
                    passResult = false; verboseLog += " <-----"; valuesCheckOK = false;
                }

                current.SetValue(registeredUIElementObjs[i].animProp, expectedValue);
                current.ApplyAnimationClock(registeredUIElementObjs[i].animProp,null);

            }

            // obviously, if the calculated and actual values do not match, there is no need for visual validation
            if (valuesCheckOK)
            {
                // Snapshot
                System.Drawing.Bitmap staticCapture = ImageUtility.CaptureScreen(hWnd,true);
   
                // Visual Verification
                passResult = imageCompare.Compare(new ImageAdapter(staticCapture), new ImageAdapter(animatedCapture));
                if (!passResult) 
                {
                    verboseLog += "\n The comparison has failed. Anim_" + curTime.ToString() + ".bmp  and  Static_" + curTime.ToString() + ".bmp have been written out";
                    animatedCapture.Save("Anim_" + curTime.ToString() + ".bmp");
                    staticCapture.Save("Static_" + curTime.ToString() + ".bmp");
                }
            }
            else
            {
                verboseLog += "\n VISUAL VALIDATION SKIPPED!!!!!!!!!!! The values did not match, no sense comparing";
                passResult = false;
            }

            // Now lets put the animations back
            for (int i=0;i < registeredAnimatableCount; i++)
            {
                DependencyObject current = registeredAnimatableObjs[i].animObj;
                current.SetValue(registeredAnimatableObjs[i].animProp,registeredAnimatableObjs[i].baseValue);
                ((System.Windows.Media.Animation.Animatable)current).ApplyAnimationClock(registeredAnimatableObjs[i].animProp,registeredAnimatableObjs[i].animClock);
            }
            for (int i=0;i < registeredUIElementCount; i++)
            {
                UIElement current = registeredUIElementObjs[i].animObj;
                current.SetValue(registeredUIElementObjs[i].animProp,registeredUIElementObjs[i].baseValue);
                current.ApplyAnimationClock(registeredUIElementObjs[i].animProp,registeredUIElementObjs[i].animClock);
            }

            return passResult;
        }


        /// <summary>
        /// Values Only Verification 
        /// </summary>

        public bool ValuesOnlyVerify(double curTime)
        {
            AnimationValidator myValidator = new AnimationValidator();

            bool passResult = true;

            System.Threading.Thread.Sleep(2000);

            verboseLog += "\n Values Only Verifier for time " + curTime;

            if ( (registeredAnimatableCount == 0) && (registeredUIElementCount == 0)) 
            { 
                verboseLog += " No animations could be registered - nothing to verify"; 
                return false; 
            }

            // walk all registered animatables, obtain expected and actual values and compare
            for (int i=0;i < registeredAnimatableCount; i++)
            {
                object expectedValue = myValidator.Verify(registeredAnimatableObjs[i].animClock,registeredAnimatableObjs[i].baseValue,curTime);
                DependencyObject current = registeredAnimatableObjs[i].animObj.CloneCurrentValue();
                object currentValue = current.GetValue(registeredAnimatableObjs[i].animProp);

                verboseLog += "\n" + current.GetType().ToString().Substring(current.GetType().ToString().LastIndexOf(".") +1);
                verboseLog += " " + registeredAnimatableObjs[i].animProp.ToString().Substring(registeredAnimatableObjs[i].animProp.ToString().LastIndexOf(".") +1);
                verboseLog += ": Calculated : " + expectedValue.ToString() + " Actual: " + currentValue.ToString();

                //                   if (!currentValue.Equals(expectedValue)) 
                if (!myValidator.WithinTolerance(currentValue,expectedValue,this.toleranceInPercent))
                { 
                    passResult = false; verboseLog += " *********"; 
                }

            }

            // walk all registered UIElements, obtain expected and actual values and compare
            for (int i=0;i < registeredUIElementCount; i++)
            {
                object expectedValue = myValidator.Verify(registeredUIElementObjs[i].animClock,registeredUIElementObjs[i].baseValue,curTime);
                object currentValue = registeredUIElementObjs[i].animObj.GetValue(registeredUIElementObjs[i].animProp);
                UIElement current = registeredUIElementObjs[i].animObj;

                verboseLog += "\n" + current.GetType().ToString().Substring(current.GetType().ToString().LastIndexOf(".") +1);
                verboseLog += " " + registeredUIElementObjs[i].animProp.ToString().Substring(registeredUIElementObjs[i].animProp.ToString().LastIndexOf(".") +1);
                verboseLog += ": Calculated : " + expectedValue.ToString() + " Actual: " + currentValue.ToString();

                if (!myValidator.WithinTolerance(currentValue,expectedValue,this.toleranceInPercent))
                {
                    passResult = false; verboseLog += " *********";
                }
            }

            return passResult;
        }
    }
}
