// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using System.Windows.Automation;
//    using Microsoft.Test.Internal; 
    using MTI = Microsoft.Test.Input;
    
    public class AutomationHelper
    {
        // public properties
        public static double ResizeGripPixel
        {
            get
            {
                if (Microsoft.Test.RenderingVerification.DisplayConfiguration.GetTheme().ToLower().Contains("classic"))
                {
                    return 1;
                }            
                else
                {
                    return 2;
                }
            }   
        }
        public delegate void AutomationHandler();
        AutomationHandler _dragDropCompleted;
        AutomationHandler _moveToCompleted;
        AutomationHandler _moveToAndClickCompleted;
        AutomationHandler _pauseCompleted;
        
        void DragDropProc(object DDArgs)
        {
            Point pointA = ((DragDropArgs)DDArgs).A;
            Point pointB = ((DragDropArgs)DDArgs).B;
            
            try
            {
                Logger.Status("  [DragDropProc] Sleep for 500 milliseconds");
                Thread.Sleep(500);
                
                Logger.Status("  [DragDropProc] Move mouse to " + pointA.ToString());
                MTI.Input.MoveTo(pointA);
                
                Logger.Status("  [DragDropProc] Sleep for 500 milliseconds");
                Thread.Sleep(500);

                Logger.Status("  [DragDropProc] Press left mouse button");
                MTI.Input.SendMouseInput(0, 0, 0, MTI.SendMouseInputFlags.LeftDown);

                Logger.Status("  [DragDropProc] Sleep for 500 milliseconds");
                Thread.Sleep(500);

                Logger.Status("  [DragDropProc] Move mouse to " + pointB.ToString());
                MTI.Input.MoveTo(pointB);
                
                Logger.Status("  [DragDropProc] Sleep for 500 milliseconds");
                Thread.Sleep(500);

                Logger.Status("  [DragDropProc] Release left mouse button");
                MTI.Input.SendMouseInput(0,0,0, MTI.SendMouseInputFlags.LeftUp);   

                Logger.Status("  [DragDropProc] Sleep for 500 milliseconds");
                Thread.Sleep(500);
                
                Logger.Status("  [DragDropProc] DragDrop Completed. Calling DragDrop Handler");


                if (_dragDropCompleted != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback) delegate (object o)
                    {
                        _dragDropCompleted();
                        return null;
                    },
                    null
                    );
                }
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("!!!EXCEPTION CAUGHT!!! during DragDrop Automation\n" + ex.ToString());
            }
        }

        public void DragDrop(Point PointA, Point PointB, AutomationHandler DragDropDelegate)
        {
            DragDropArgs DDArgs = new DragDropArgs(PointA, PointB);

            if (DragDropDelegate != null)
                _dragDropCompleted += DragDropDelegate;          
            
            Thread AutomationThread = new Thread(DragDropProc);
            AutomationThread.Start(DDArgs);
        }

        public void MoveTo(System.Windows.Point point)
        {
            Logger.Status("  [MoveTo] point " + point.ToString());
            MTI.Input.MoveTo(point);
        }
        
        public void WaitThenMoveTo(Point point, AutomationHandler MoveToDelegate)
        {
            if (_moveToCompleted != null)
                _moveToCompleted += MoveToDelegate;
                                
            Logger.Status("  [WaitThenMoveTo] point " + point.ToString());
            Thread AutomationThread = new Thread(WaitAndMoveToProc);
            AutomationThread.Start(point);
        }

        private void WaitAndMoveToProc(object point)
        {
            try
            {
                Logger.Status("  [WaitAndMoveToProc] Sleep for 1000 milliseconds");
                Thread.Sleep(1000);

                Logger.Status("  [WaitAndMoveToProc] Move Mouse To " + ((Point)point).ToString());                
                MTI.Input.MoveTo(((Point)point));

                Logger.Status("  [WaitAndMoveToProc] Sleep for 500 milliseconds. (Post wait)");
                Thread.Sleep(500);

                Logger.Status("  [WaitAndMoveToProc] Automation Completed");

                if (_moveToCompleted != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback) delegate (object o)
                    {
                        _moveToCompleted();
                        return null;
                    },
                    null
                    );
                }
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("!!!EXCEPTION CAUGHT!!! during WaitAndMoveToProc Automation\n" + ex.ToString());
            }

            
        }

        public void MoveToAndClick(Point point)
        {
            Logger.Status("  [MoveToAndClick] point " + point.ToString());
            MTI.Input.MoveToAndClick(point);
        }

        public void WaitThenMoveToAndClick(Point point, AutomationHandler MoveToAndClickDelegate)
        {
            if (MoveToAndClickDelegate != null)
                _moveToAndClickCompleted += MoveToAndClickDelegate;
                                
            Logger.Status("  [WaitThenMoveToAndClick] point " + point.ToString());
            Thread AutomationThread = new Thread(WaitThenMoveToAndClickProc);
            AutomationThread.Start(point);
        }

        private void WaitThenMoveToAndClickProc(object point)
        {               
            try
            {
                Logger.Status("  [WaitThenMoveToAndClickProc] Sleep for 1000 milliseconds");
                Thread.Sleep(1000);

                Logger.Status("  [WaitThenMoveToAndClickProc] Move Mouse To " + ((Point)point).ToString());
                MTI.Input.MoveToAndClick((Point)point);

                Logger.Status("  [WaitThenMoveToAndClickProc] Sleep for 500 milliseconds. (Post wait)");
                Thread.Sleep(500);

                Logger.Status("  [WaitThenMoveToAndClickProc] Automation Completed");

                if (_moveToAndClickCompleted != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback) delegate (object o)
                    {
                        _moveToAndClickCompleted();
                        return null;
                    },
                    null
                    );
                }
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("!!!EXCEPTION CAUGHT!!! during DragDrop Automation\n" + ex.ToString());
            }
        }

        public void Pause(int time, AutomationHandler PauseCompletedDelegate)
        {
            if (PauseCompletedDelegate != null)
                _pauseCompleted += PauseCompletedDelegate;
                                
            Thread AutomationThread = new Thread(PauseProc);
            AutomationThread.Start(time);
        }

        private void PauseProc(object time)
        {               
            try
            {
                Logger.Status("  [PauseProc] Pausing " + ((int)time).ToString() + " milliseconds");
                Thread.Sleep((int)time);
                Logger.Status("  [PauseProc] Pause Completed.");

                if (_pauseCompleted != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback) delegate (object o)
                    {
                        _pauseCompleted();
                        return null;
                    },
                    null
                    );
                }
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("!!!EXCEPTION CAUGHT!!! during Pause Operation\n" + ex.ToString());
            }
        }
        
    }
    public struct DragDropArgs
    {
        Point _pA;
        Point _pB;

        public DragDropArgs(Point A, Point B)
        {
            _pA = A;
            _pB = B;
        }

        public Point A
        {
            get 
            {
                return _pA;
            }
            set
            {
                _pA = value;
            }
        }

        public Point B
        {
            get 
            {
                return _pB;
            }
            set
            {
                _pB = value;
            }
        }
    }
    
}

