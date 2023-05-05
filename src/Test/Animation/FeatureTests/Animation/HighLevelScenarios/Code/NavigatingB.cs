// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    public class NavigatingB : AvalonTest
    {
        private DispatcherTimer      _aTimer                  = null;  //Used for Timing method verification.
        private int                  _dispatcherTickCount     = 0;
        private NavigationWindow     _navWin                  = null;
        private AnimationClock       _clock1                  = null;
        private EllipseGeometry      _ellipseGeometry         = null;
        private PointAnimation       _pointAnim               = null;
        private Storyboard           _storyboard1             = null;
        private Storyboard           _storyboard2             = null;
        private Storyboard           _storyboard3             = null;
        private Storyboard           _storyInResource         = null;
        private string               _animationType           = "";
        private string               _eventType               = "";
        public  bool                 currentStateFired       = false;
        private Rectangle            _animElement             = null;
        private FrameworkElement     _rootElement             = null;
        private TimeSpan             _TIMER_INTERVAL          = new TimeSpan(0,0,0,0,1000);
        private bool                 _testPassed              = false;
        
        public NavigatingB()
        {
        }


        /******************************************************************************
           * Function:          ContinueTest
           ******************************************************************************/
        /// <summary>
        /// ContinueTest: set up event on the NavigationWindow and start a Dispatcher Timer to navigate.
        /// Objects are obtained from the Markup that has loaded.
        ///     animType:     the type of Animation carried out, e.g., double, rect
        ///     eventType:    the expected value of the Animation when completed
        /// </summary>
        /// <returns></returns>
        public bool ContinueTest(string animType, string winEvent, NavigationWindow nWin)
        {
            GlobalLog.LogStatus("-------------ContinueTest-------------");
            
            _animationType   = animType;
            _eventType       = winEvent;
            _navWin          = nWin;

            //Retrieve elements from the Markup page
            _rootElement = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"Root1");
            _animElement = (Rectangle)LogicalTreeHelper.FindLogicalNode((DependencyObject)_navWin.Content,"Animate");
            _animElement.Unloaded             += new RoutedEventHandler(OnUnloaded);
            
            GlobalLog.LogStatus("*************Test Parameters*************");
            GlobalLog.LogStatus("AnimationType:  " + _animationType);
            GlobalLog.LogStatus("Window Event :  " + _eventType);
            GlobalLog.LogStatus("*****************************************");

            _navWin.Navigating           += new NavigatingCancelEventHandler(OnNavigating);
            _navWin.Navigated            += new NavigatedEventHandler(OnNavigated);
            _navWin.Closing              += new CancelEventHandler(OnClosing);
            _navWin.Closed               += new EventHandler(OnClosed);
            
            //Setup the Animations, but don't start them.
            SetupAnimation();
            
            //Use a DispatcherTimer to initiate an Animation.
            _aTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _aTimer.Tick += new EventHandler(OnTick);
            _aTimer.Interval = _TIMER_INTERVAL;
            _aTimer.Start();

            WaitForSignal("TestFinished");
            
            return _testPassed;
        }
        
        /******************************************************************************
           * Function:          OnTick
           ******************************************************************************/
        /// <summary>
        /// OnTick: starts verification (only for cases in which Animation events are not bound).
        /// </summary>
        private void OnTick(object sender, EventArgs e)          
        {
            _dispatcherTickCount++;

            GlobalLog.LogStatus("-------------Tick #" + _dispatcherTickCount);
            
            if (_dispatcherTickCount == 1)
            {
                
                NavigateMarkup();
            }
            else
            {
                _aTimer.Stop();
                FinishTest(currentStateFired);
                Signal("TestFinished", TestResult.Pass);
            }
        }
        /******************************************************************************
           * Function:          CreateAnimation
           ******************************************************************************/
        /// <summary>
        /// CreateAnimation: create and return a PointAnimation
        /// </summary>
        /// <returns></returns>
        private PointAnimation CreateAnimation()          
        {
            GlobalLog.LogStatus("-------------CreateAnimation-------------");

            PointAnimation pointAnimation = new PointAnimation();
            pointAnimation.By             = new Point(50,50);
            pointAnimation.BeginTime      = TimeSpan.FromMilliseconds(0);
            pointAnimation.Duration       = new Duration(TimeSpan.FromSeconds(1));
            pointAnimation.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);

            return pointAnimation;
        }

        /******************************************************************************
           * Function:          OnCurrentStateInvalidated
           ******************************************************************************/
        /// <summary>
        /// OnCurrentStateInvalidated: Used to validate event firing.
        /// </summary>
        private void OnCurrentStateInvalidated(object sender, EventArgs e)
        {               
            GlobalLog.LogStatus("CurrentStateInvalidated:  " + ((Clock)sender).CurrentState);
            currentStateFired = true;
        }
        
        /******************************************************************************
           * Function:          NavigateMarkup
           ******************************************************************************/
        /// <summary>
        /// NavigateMarkup: 
        /// </summary>
        /// <returns></returns>
        private void NavigateMarkup()          
        {
            if (_eventType == "Closing" || _eventType == "Closed")
            {
                _navWin.Close();
            }
            else
            {
                //navWin.Navigate(new Uri("Navigating2.xaml", UriKind.RelativeOrAbsolute));
                _navWin.Content = (Page)XamlReader.Load(File.OpenRead("Navigating2.xaml"));
                _navWin.Show();
            }
        }
        /******************************************************************************
           * Function:          OnClosing
           ******************************************************************************/
        private void OnClosing(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnClosing---");
            if (_eventType == "Closing")
            {
                StartAnimation();
                FinishTest(true);  //Force a pass.
                Signal("TestFinished", TestResult.Pass);
            }
        }

        /******************************************************************************
           * Function:          OnClosed
           ******************************************************************************/
        private void OnClosed(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnClosed---");
            if (_eventType == "Closed")
            {
                StartAnimation();
                FinishTest(true);  //Force a pass.
                Signal("TestFinished", TestResult.Pass);
            }
        }

        /******************************************************************************
           * Function:          OnNavigated
           ******************************************************************************/
        private void OnNavigated(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnNavigated---");
            if (_eventType == "Navigated")
            {
                StartAnimation();
            }
        }

        /******************************************************************************
           * Function:          OnNavigating
           ******************************************************************************/
        private void OnNavigating(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnNavigating---");
            if (_eventType == "Navigating")
            {
                StartAnimation();
            }
        }

        /******************************************************************************
           * Function:          OnUnloaded
           ******************************************************************************/
        private void OnUnloaded(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("---EVENT---OnUnloaded---");
            if (_eventType == "Unloaded")
            {
                StartAnimation();
            }
        }
        
        /******************************************************************************
           * Function:          SetupAnimation
           ******************************************************************************/
        /// <summary>
        /// SetupAnimation: creates the Animations, Storyboards etc., but doesn't start them.
        /// </summary>
        /// <returns></returns>
        public void SetupAnimation()          
        {
            GlobalLog.LogStatus("-------------SetupAnimation-------------");

            if (_animElement == null)
            {
                GlobalLog.LogEvidence("ERROR: SetupAnimation: Animated element was not found in Markup.");
                _testPassed = false;
            }
            else
            {
                _ellipseGeometry = new EllipseGeometry();
                _ellipseGeometry.RadiusX         = 50d;
                _ellipseGeometry.RadiusY         = 50d;
                _ellipseGeometry.Center          = new Point(50,50);
                _animElement.Clip                = _ellipseGeometry;

                _pointAnim = CreateAnimation();

                switch (_animationType)
                {
                    case "BeginAnimation" :
                        //Do nothing.
                        break;

                    case "AnimationClock" :
                        _clock1 = _pointAnim.CreateClock();
                        break;

                    case "StoryboardBegin" :
                        _storyboard1 = new Storyboard();
                        _storyboard1.Name = "story1";
                        _storyboard1.BeginTime = TimeSpan.FromMilliseconds(0);
                        _storyboard1.Children.Add(_pointAnim);

                        PropertyPath path1 = new PropertyPath("(0).(1)", new DependencyProperty[] { Ellipse.ClipProperty, EllipseGeometry.CenterProperty });

                        Storyboard.SetTargetProperty(_pointAnim, path1);
                        Storyboard.SetTargetName(_pointAnim, _animElement.Name);
                        break;

                    case "BeginStoryboard" :
                        _storyboard2 = new Storyboard();
                        _storyboard2.Name = "story2";
                        _storyboard2.BeginTime = TimeSpan.FromMilliseconds(0);
                        _storyboard2.Children.Add(_pointAnim);

                        PropertyPath path2 = new PropertyPath("(0).(1)", new DependencyProperty[] { Ellipse.ClipProperty, EllipseGeometry.CenterProperty });

                        Storyboard.SetTargetProperty(_pointAnim, path2);
                        Storyboard.SetTargetName(_pointAnim, _animElement.Name);
                        break;

                    case "PropertyTrigger" :
                        _storyboard3 = new Storyboard();
                        _storyboard3.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(0).(1)", Ellipse.ClipProperty, EllipseGeometry.CenterProperty));
                        _storyboard3.Name = "story3";
                        _storyboard3.BeginTime = TimeSpan.FromMilliseconds(0);
                        _storyboard3.Children.Add(_pointAnim);
                        PropertyPath path3 = new PropertyPath("(0).(1)", new DependencyProperty[] { Ellipse.ClipProperty, EllipseGeometry.CenterProperty });
                        Storyboard.SetTargetProperty(_pointAnim, path3);

                        BeginStoryboard bs = new BeginStoryboard();
                        bs.Storyboard = _storyboard3;

                        Trigger trigger = AnimationUtilities.CreatePropertyTrigger(Rectangle.FillProperty, Brushes.Red, bs);

                        Style style = new Style();
                        style.TargetType = typeof(Rectangle);
                        style.Triggers.Add(trigger);

                        _animElement.Style = style;
                        break;

                    case "AnimationInMarkup" :
                        _storyInResource = (Storyboard)_rootElement.FindResource("StoryKey");
                        
                        if (_storyInResource == null)
                        {
                            GlobalLog.LogEvidence("ERROR!! SetupAnimation: StoryKey Resource was not found. \n");
                            _testPassed = false;
                        }
                        else
                        {
                            DoubleAnimation doubleAnim = (DoubleAnimation)_storyInResource.Children[0];
                            doubleAnim.CurrentStateInvalidated += new EventHandler(OnCurrentStateInvalidated);
                        }
                        break;

                    default:
                        GlobalLog.LogEvidence("ERROR: SetupAnimation: AnimationType was not found.");
                        _testPassed = false;
                        break;
                }
            }
        }        
        /******************************************************************************
           * Function:          StartAnimation
           ******************************************************************************/
        /// <summary>
        /// StartAnimation: 
        ///     animationType: specifies how the animation is to be started
        /// </summary>
        /// <returns></returns>
        public void StartAnimation()          
        {
            GlobalLog.LogStatus("-------------StartAnimation-------------");

            switch (_animationType)
            {
                case "BeginAnimation" :
                    _ellipseGeometry.BeginAnimation(EllipseGeometry.CenterProperty, _pointAnim);
                    break;

                case "AnimationClock" :
                    _ellipseGeometry.ApplyAnimationClock(EllipseGeometry.CenterProperty, _clock1);
                    break;

                case "StoryboardBegin" :
                    _storyboard1.Begin(_animElement);
                    break;

                case "BeginStoryboard" :
                    _animElement.BeginStoryboard(_storyboard2);
                    break;

                case "PropertyTrigger" :
                    _animElement.Fill   = Brushes.Red;
                    break;

                case "AnimationInMarkup" :
                    _storyInResource.Begin(_rootElement);
                    break;

                default:
                    GlobalLog.LogEvidence("ERROR: StartAnimation: AnimationType was not found.");
                    _testPassed = false;
                    break;
            }
        }
        
        /******************************************************************************
           * Function:          FinishTest
           ******************************************************************************/
        /// <summary>
        /// FinishTest: sets a global variable indicating Pass/Fail, and signals that the test
        /// is finished.
        /// </summary>
        /// <returns></returns>
        private void FinishTest(bool result)          
        {
            _testPassed = result;
        }
    }
}
