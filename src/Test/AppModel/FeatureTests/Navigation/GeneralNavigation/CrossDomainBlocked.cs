// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Controls;                  // Frame, Button
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum CrossDomainBlocked_State
        {
            InitialNav, 
            Navigated,
            CrossNavigated
        }

        CrossDomainBlocked_State _crossDomainBlocked_curState = CrossDomainBlocked_State.InitialNav;

        private void CrossDomainBlocked_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("CrossDomainBlocked");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 8;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"";

            //NavigationHelper.Output("Registering application-level eventhandlers.");
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);
            #region cross-domain behaviour change
            //this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Catch_Navigation_Error);
            #endregion

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri(@"CrossDomainBlocked_a.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }

        #region cross-domain behaviour change
        //// this event handler will get attached twice
        //public void Catch_Navigation_Error (object source, DispatcherUnhandledExceptionEventArgs e)
        //{
        //    NavigationHelper.Output("** In Context Exception Event Handler");
        //    NavigationHelper.Output("Type of source is: " + source.GetType().ToString());            
            
        //    if ((e.Exception.Source.Equals("PresentationFramework")) && (this.CrossDomainBlocked_curState == CrossDomainBlocked_State.CrossNavigated) && (source is Dispatcher) && e.Exception.GetType().ToString() == "System.NotSupportedException")
        //    {
        //        // exception will be NotSupportedException
        //        NavigationHelper.Output("CrossDomainBlocked successfully");
        //        e.Handled = true;
                
        //        NavigationHelper.Output("Handling context exception error");
        //        NavigationHelper.FinishTest(true);
        //    }
        //    else 
        //    {
        //        NavigationHelper.ActualTestCount++;
        //        NavigationHelper.Output("Source is: " + e.Exception.Source.ToString());
        //        NavigationHelper.Output("Message is: " + e.Exception.Message.ToString());
        //        NavigationHelper.Output("Error is: " + e.Exception.ToString());
        //        NavigationHelper.Output("StackTrace is: " + e.Exception.StackTrace);
        //        if (e.Exception.InnerException != null)
        //        {
        //            NavigationHelper.Output("InnerException is: " + e.Exception.InnerException.ToString());
        //        }

        //        NavigationHelper.Output("Handling context exception error");
                
        //        // in this case, we don't want to catch the exception
        //        e.Handled = false;
        //    }
        //}
        #endregion

        public void CrossDomainBlocked_Navigating_Frame(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired on frame");
            NavigationHelper.ActualTestCount++;
        }

        public void CrossDomainBlocked_LoadCompleted_Frame(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired on frame");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
        }

        public void CrossDomainBlocked_LoadCompleted(object source, NavigationEventArgs e)
        {
            Frame f;
            FrameworkElement el;
            Hyperlink h1, h2;
            Frame frameSource = e.Navigator as Frame;

            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.ActualTestCount++;
            if ((frameSource != null) && (this._crossDomainBlocked_curState == CrossDomainBlocked_State.InitialNav))
                return;

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw == null)
                NavigationHelper.Fail("Could not get NavigationWindow o");

            el = nw.Content as FrameworkElement;
            if (el == null)
                NavigationHelper.Fail("Could not get NavigationWindow.Content as FrameworkElement - it is null");

            f = LogicalTreeHelper.FindLogicalNode(el, "frame1") as Frame;
            if (f == null)
                NavigationHelper.Fail("Could not get frame on startup page");

            switch (this._crossDomainBlocked_curState)
            {
                case CrossDomainBlocked_State.InitialNav : 
                    _crossDomainBlocked_curState = CrossDomainBlocked_State.Navigated;
                    f.Navigating += new NavigatingCancelEventHandler(CrossDomainBlocked_Navigating_Frame);
                    f.LoadCompleted += new LoadCompletedEventHandler(CrossDomainBlocked_LoadCompleted_Frame);
                    #region cross-domain behaviour change
                    ///-                Dispatcher ctx = Dispatcher.CurrentDispatcher;
                    ///-                ctx.UnhandledException += new DispatcherUnhandledExceptionEventHandler(Catch_Navigation_Error);
                    #endregion

                    NavigationHelper.Output("Clicking HyperLink1");

                    // get button & navigate frame
                    h1 = LogicalTreeHelper.FindLogicalNode(el, "navigatefirst") as Hyperlink;
                    if (h1 == null)
                        NavigationHelper.Fail("Could not get HyperLink to navigate frame");

                    h1.DoClick();
                    break;

                case CrossDomainBlocked_State.Navigated : 
                    _crossDomainBlocked_curState = CrossDomainBlocked_State.CrossNavigated;
                    NavigationHelper.Output("Clicking HyperLink2");
                    h2 = LogicalTreeHelper.FindLogicalNode(el, "navigatesecond") as Hyperlink;
                    if (h2 == null)
                        NavigationHelper.Fail("Could not get HyperLink to navigate frame");

                    h2.DoClick();
                    break;

                case CrossDomainBlocked_State.CrossNavigated :
                    NavigationHelper.Output("Cross-domain contents loaded into Frame");
                    if (f.Source.ToString().ToLowerInvariant().Equals("http://www.televisionwithoutpity.com/"))
                        NavigationHelper.FinishTest(true);
                    else
                        NavigationHelper.Fail("frame1 did not end up at the correct cross-domain site");

                    #region cross-domain behaviour change
                    // throw an error here - should not have fired loadcompleted
                    //NavigationHelper.Fail("LoadCompleted should not have fired for this State");
                    #endregion
                    break;
            }
        }
    }
}

