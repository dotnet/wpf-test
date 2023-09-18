// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    public class SolidColorVerifier : ScenarioTestVerifier
    {
        public SolidColorVerifier( Viewport3D vp, Color bg, Color expectedColor ) : base( vp, bg )
        {
            this.expectedColor = expectedColor;
        }

        public void Verify()
        {
            EnterVerificationLoop();
        }

        protected object VerifyTestCallback( object o )
        {
            int width = capture.GetLength(0);
            int height = capture.GetLength(1);
        
            // use renderbuffer for error difference estimation 
            result = new RenderBuffer( width, height, expectedColor );
            result.ClearToleranceBuffer( RenderTolerance.DefaultColorTolerance );
        
            // Use default verification
            VerifyAgainstCapture( "SolidColor", capture );

            return null;
        }

        protected override void OnTimerEvent( object sender, System.EventArgs args )
        {
            switch ( stageNumber++ )
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    // dummy
                    break;

                case 5:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback( CaptureScreenCallback ),
                            null );
                    break;

                case 6:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback( VerifyTestCallback ),
                            null );
                    break;

                case 7:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback( CleanUpCallback ),
                            null );
                    break;

                default:
                    break;
            }
        }
        
        protected Color expectedColor;
    }
}