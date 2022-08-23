using System;
using System.Collections;
using System.Globalization;
using System.IO;

using System.Net;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;

namespace DrtNavError
{
    public partial class DrtNavErrorApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //DispatcherUnhandledException    += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
            _navWin = new NavigationWindow();
            _navWin.NavigationFailed += new NavigationFailedEventHandler(OnNavigationFailed);
            _navWin.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
            // navigate to a nonexsist web page. First web exception should be thrown.
            _navWin.Navigate(_firstNavSource);            
            _myState = State.FirstNavigation;

            // Start timeout timer for 120 secs (2 mins)
            // In case the drt fails, when timeout, the drt will shut down
            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Tick += new EventHandler(OnTick);
            _timer.Interval = TimeSpan.FromMilliseconds(120000);
            _timer.Start();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (e.ApplicationExitCode == 0)
            {
                Log("Test PASSED");
            }
            else
            {
                Log("Test FAILED");
            }

            _timer.Stop();

            base.OnExit(e);
        }

        private void OnLoadCompleted(Object sender, NavigationEventArgs e)
        {
            if (_myState != State.FourthNavigation)
            {
                Log("LoadCompleted should not be fired unless it is the fourth navigation");
                Shutdown(-1);
            }
            if (! e.Uri.ToString().EndsWith(_fourthNavSource.ToString(), true, CultureInfo.InvariantCulture))
            {
                Log("The Uri in EventArgs is wrong");
                Shutdown(-1);
            }
            if (!(e.Content is Page))
            {
                Log("The Content is wrong");
                Shutdown(-1);
            }
            Shutdown(0);
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (e.Exception is WebException)
            {                
                if (_myState == State.FirstNavigation)
                {
                    // Verify source
                    if (!e.Uri.ToString().EndsWith(_firstNavSource.ToString(), true, CultureInfo.InvariantCulture))
                        return;

                    Log(e.WebRequest.Headers.ToString());

                    e.Handled = true;
                    _myState = State.SecondNavigation;
                    // This should cause IOException
                    _navWin.Navigate(_secondNavSource);
                    return;
                }
            }

            if (e.Exception is IOException)
            {
                if (_myState == State.SecondNavigation)
                {
                    if (!e.Uri.ToString().EndsWith(_secondNavSource.ToString(), true, CultureInfo.InvariantCulture))
                        return;

                    e.Handled = true;
                    _myState = State.ThirdNavigation;
                    // frame.xaml contains a frame that navigates to a nonexsist page.
                    // Should cause another IOException.
                    _navWin.Navigate(_thirdNavSource);
                    return;
                }

                if (_myState == State.ThirdNavigation)
                {
                    if (!e.Uri.ToString().EndsWith(_thirdNavFrameSource.ToString(), true, CultureInfo.InvariantCulture))
                        return;

                    e.Handled = true;
                    _myState = State.FourthNavigation;
                    // This one should be successful and fire LoadCompleted
                    _navWin.Navigate(_fourthNavSource);
                }
            }
        }
       
        // Timeout. Shut down drt
        private void OnTick(object sender, EventArgs e)
        {
            Log("Drt timed out. ContextExceptionHandler was not called");
            Shutdown(-1);
        }

        private void Log(string message, params object[] args)
        {
            _logger.Log(message, args);
        }

        private NavigationWindow _navWin  = null;

        private DispatcherTimer _timer;

        private DRT.Logger _logger  = new DRT.Logger("DrtNavError", "Microsoft", "Testing Navigation Error behavior");

        private State _myState;

        private static Uri _firstNavSource = new Uri("http://DoesNotExist.DrtNavError.com/test.xaml");
        private static Uri _secondNavSource = new Uri("DoesNotExistTopLevel.xaml", UriKind.Relative);
        private static Uri _thirdNavSource = new Uri("frame.xaml", UriKind.Relative);
        private static Uri _thirdNavFrameSource = new Uri("DoesNotExist.xaml", UriKind.Relative);
        private static Uri _fourthNavSource = new Uri("frame2.xaml", UriKind.Relative);
        
        enum State
        {
            FirstNavigation,
            SecondNavigation,
            ThirdNavigation,
            FourthNavigation
        }
    }
}
