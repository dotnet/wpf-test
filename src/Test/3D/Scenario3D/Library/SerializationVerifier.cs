// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Utility to test serialization visually
    /// </summary>
    public class SerializationVerifier : ScenarioTestVerifier
    {
        public SerializationVerifier( Viewport3D vp, Color bg )
            : base( vp, bg )
        {
        }

        public void Verify( int verificationTime )
        {
            times = new int[] { verificationTime };
            if ( verificationTime > 0 )
            {
                captureTime = true;
            }
            EnterVerificationLoop();
        }

        protected override void OnTimerEvent( object sender, System.EventArgs args )
        {
            switch ( stageNumber++ )
            {

                case 0:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( SetTimeCallback ), times[ 0 ] );
                    break;

                case 1:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( RenderFrameCallback ), null );
                    break;

                case 2:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( CaptureScreenOriginalCallback ), null );
                    break;

                case 3:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( SerializeCallback ), null );
                    break;

                case 4:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( SetTimeCallback ), times[ 0 ] );
                    break;

                case 5:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( CaptureScreenSerializedCallback ), null );
                    break;

                case 6:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( VerifyTestCallback ), null );
                    break;

                case 7:
                    Dispatcher.CurrentDispatcher.Invoke( DispatcherPriority.Normal, new DispatcherOperationCallback( CleanUpCallback ), null );
                    break;

                default:
                    break;
            }

        }

        protected object CaptureScreenOriginalCallback( object o )
        {
            CaptureScreenCallback( o );
            _originalCapture = capture;
            capture = null;
            return null;
        }

        protected object CaptureScreenSerializedCallback( object o )
        {
            CaptureScreenCallback( o );
            _serializedCapture = capture;
            capture = null;
            return null;
        }

        protected object VerifyTestCallback( object o )
        {
            VerifyAgainstCapture( "Original", _originalCapture );
            VerifyAgainstCapture( "Serialized", _serializedCapture );
            return null;
        }

        protected object SerializeCallback( object o )
        {
            Visual parent = (Visual)VisualTreeHelper.GetParent( viewport );

            // Serialze
            _serialization = System.Windows.Markup.XamlWriter.Save( viewport );
            LogSerialization(_serialization);

            // Roundtrip
            UIElement replacement = (UIElement)System.Windows.Markup.XamlReader.Load(
                    ScenarioUtility.GetStreamFromString( _serialization ) );

            // Re-attach
            if ( parent is Window )
            {
                ( parent as Window ).Content = replacement;
            }
            else if ( parent is Panel )
            {
                ( parent as Panel ).Children.Clear();
                ( parent as Panel ).Children.Add( replacement );
            }

            return null;
        }

        private void LogSerialization(string serialization)
        {
            string fullFileName = ScenarioUtility.CurrentTestPrefix + "_Serialization.txt";
            string fileName = System.IO.Path.GetFileName(fullFileName);

            LogStatus(string.Format("**** DUMPING SERIALIZED XAML TO LOG FILE : {0} ****", fileName));

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fullFileName, true))
            {
                sw.WriteLine("**** BEGIN DUMPING SERIALIZED XAML ****" + Environment.NewLine);
                sw.WriteLine(serialization + Environment.NewLine);
                sw.WriteLine("**** END DUMPING SERIALIZED XAML ****" + Environment.NewLine);
            }

            Microsoft.Test.Logging.GlobalLog.LogFile(fullFileName);
        }

        Color[ , ] _originalCapture = null;
        Color[ , ] _serializedCapture = null;
        string _serialization = null;
    }
}