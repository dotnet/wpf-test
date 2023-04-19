// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: GetNavigationService checks that an instance of 
//  NavigationService is returned after a call to GetNavigationService
//  on a valid DependencyObject (NavigationWindow or Frame or the contents
//  of either) 
//  The returned value should never be null.
//

using System;
using System.Reflection;                // Assembly
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;             // Brushes
using System.Windows.Navigation;
using Microsoft.Test.Logging;           // TestLog
using Microsoft.Windows.Test.Client.AppSec.Navigation;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {

        /// <summary>
        /// Sets up the TestLog in NavigationHelper, and 
        /// registers an eventhandler for LoadCompleted.
        /// </summary>
        private void GetNavigationService_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("GetNavigationService");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 5;
            NavigationHelper.ActualTestCount = 0;

            //NavigationHelper.Output("Registering application-level eventhandlers.");
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri("GetNavigationService_homePage.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }


        /// <summary>
        /// Once XAML page has been loaded, check that GetNavigationService
        /// returns a valid NavigationService on the DependencyObject passed in
        /// </summary>
        private void GetNavigationService_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Starting GetNavigationService BVTs.");

            if (e.Navigator is NavigationWindow)
            {
                NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                StackPanel nwContent = LogicalTreeHelper.FindLogicalNode(nw, "Root") as StackPanel;
                Frame nwFrame = LogicalTreeHelper.FindLogicalNode(nwContent, "RootFrame") as Frame;
                TextBlock nwFrameContent = LogicalTreeHelper.FindLogicalNode(((DependencyObject)nwFrame.Content), "RootTextBlock") as TextBlock;

                NavigationService ns = null;

                // [1] GetNavigationService on Frame
                NavigationHelper.Output("GetNavigationService on Frame");
                ns = NavigationService.GetNavigationService(nwFrame);
                GetNavigationService_CheckNavigationServiceReturned(ns, nwContent);

                // [2] GetNavigationService on NavigationWindow
                NavigationHelper.Output("GetNavigationService on NavigationWindow");
                ns = NavigationService.GetNavigationService(nw);
                GetNavigationService_CheckNavigationServiceReturned(ns, null);

                // [3] GetNavigationService on Frame contents
                NavigationHelper.Output("GetNavigationService on Frame contents");
                ns = NavigationService.GetNavigationService(nwFrameContent);
                GetNavigationService_CheckNavigationServiceReturned(ns, nwFrame);

                // [4] GetNavigationService on NavigationWindow contents
                NavigationHelper.Output("GetNavigationService on NavigationWindow contents");
                ns = NavigationService.GetNavigationService(nwContent);
                GetNavigationService_CheckNavigationServiceReturned(ns, nw);

                // [5] GetNavigationService on an object that has no NavigationService
                NavigationHelper.Output("GetNavigationService on non-Frame/NavigationWindow tree");
                ns = NavigationService.GetNavigationService(new NoNavigationServicePage());
                GetNavigationService_CheckNavigationServiceReturned(ns, null);
                NavigationHelper.FinishTest(ns == null);
            }
        }


        /// <summary>
        /// Checks that the NavigationService returned is not null and that it is the 
        /// same NavigationService enabled on the DependencyObject's closest Frame/
        /// NavigationWindow ancestor
        /// </summary>
        /// <param name="result">NavigationService returned by GetNavigationService</param>
        private void GetNavigationService_CheckNavigationServiceReturned(NavigationService result, 
            DependencyObject ancestor)
        {
            if (result == null)
            {
                // If there are no Frame/NavigationWindow ancestors, then
                // NavigationService should be null, so test passes.
                if (ancestor == null)
                    NavigationHelper.ActualTestCount++;
                else
                    NavigationHelper.Fail("GetNavigationService returned null");
            }
            else if (ancestor != null)
            {
                // Use AssemblyProxy (Reflection-based helper class) to grab the 
                // non-public NavigationService property on Frame and NavigationWindow
                AssemblyProxy ap = new AssemblyProxy();

                // Check that the NavigationService returned is the same as the 
                // NavigationService for the closest Frame/NavWindow ancestor
                NavigationService ns_ancestor = null;

                // If ancestor is a Frame:
                // Compare Frame content's NavigationService with Frame.NavigationService (internal)
                if (ancestor is Frame)
                {
                    Frame frAncestor = ancestor as Frame;
                    ap.Load(Assembly.GetAssembly(typeof(Frame)));
                    ns_ancestor = ap.Invoke(frAncestor, "get_NavigationService") as NavigationService;
                }

                // If ancestor is a NavigationWindow:
                // Compare NavWin content's NavigationService with NavWin.NavigationService (interal)
                else if (ancestor is NavigationWindow)
                {
                    NavigationWindow nwAncestor = ancestor as NavigationWindow;
                    ap.Load(Assembly.GetAssembly(typeof(NavigationWindow)));
                    ns_ancestor = ap.Invoke(nwAncestor, "get_NavigationService") as NavigationService;
                }

                // Otherwise, 
                // Compare with the NavigationService DP of element's closest Frame/NavWin ancestor
                else
                {
                    ns_ancestor = NavigationService.GetNavigationService(ancestor);
                }

                if (ns_ancestor != null && ns_ancestor.Equals(result))
                    NavigationHelper.ActualTestCount++;
                else
                    NavigationHelper.Fail("NavigationService does not equal the NavigationService of the closest Frame/NavWin ancestor");
            }
        }
    }


    /// <summary>
    /// Custom class that has no NavigationService set on it.
    /// Used to test that no NavigationService is returned when we call GetNavigationService
    /// on non-Frame/non-NavWin class/descendant.
    /// </summary>
    public class NoNavigationServicePage : Canvas
    {
        public NoNavigationServicePage() : base()
        {
            Log.Current.CurrentVariation.LogMessage("Inside NoNavigationServicePage constructor");
            this.Background = Brushes.Thistle;
            TextBox tb = new TextBox();
            tb.Text = "One fish, two fish, red fish, blue fish";
            this.Children.Add(tb);

            Log.Current.CurrentVariation.LogMessage("Exiting NoNavigationServicePage constructor");
        }
    }
}
