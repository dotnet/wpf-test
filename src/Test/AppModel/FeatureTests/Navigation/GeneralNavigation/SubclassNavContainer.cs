// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Controls;                  // Frame, Button
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum SubclassNavContainer_State
        {
            InitialNav, 
            Navigated,
            CrossNavigated
        }

        SubclassNavContainer_State _subclassNavContainer_curState = SubclassNavContainer_State.InitialNav;

        void SubclassNavContainer_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("SubclassNavContainer");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize); 

            NavigationHelper.ExpectedTestCount = 4;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"SubclassNavContainer_c.xaml";

            //NavigationHelper.Output("Registering application-level eventhandlers.");
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri(@"SubclassNavContainer_a.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }
    
        public void OnNavigatingTTT(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired on navcontainer");
            NavigationHelper.ActualTestCount++;
        }

        public void OnLoadCompletedTTT(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired on navcontainer");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
        }

        public void SubclassNavContainer_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationWindow nwSource = e.Navigator as NavigationWindow;
            MyNavFrameSubclass mySub = e.Navigator as MyNavFrameSubclass;
            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;

            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("Source is " + (e.Navigator.GetType()).ToString());
            if ((mySub != null) && (this._subclassNavContainer_curState == SubclassNavContainer_State.CrossNavigated))
            {
                NavigationHelper.Output("First subclass bonus string is: " + mySub.SubclassBonusString);
                if (nw == null)
                    NavigationHelper.Fail("Could not get nw");

                    // check subclass string property
                ProgrammaticTree pt = nw.Content as ProgrammaticTree;
                if (pt == null)
                    NavigationHelper.Fail("Could not get pt");

                NavigationHelper.Output("Getting MyNavFrameSubclass");
                MyNavFrameSubclass sc2 = pt.Children[2] as MyNavFrameSubclass;
                if (sc2 == null)
                    NavigationHelper.Fail("Could not get sc2");

                NavigationHelper.Output("Second subclass bonus string is: " + sc2.SubclassBonusString);

                // finish test
                NavigationHelper.FinishTest(NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri) && ((mySub.SubclassBonusString.Equals("part2Bonus - yeehaw!")) &&(sc2.SubclassBonusString.Equals("defaultStringValue"))));
            }

            if (nwSource == null)
                return;

            if (nw == null)
                NavigationHelper.Fail("Could not get NavigationWindow o");

            switch (this._subclassNavContainer_curState)
            {
                case SubclassNavContainer_State.InitialNav:
                    _subclassNavContainer_curState = SubclassNavContainer_State.Navigated;
                    NavigationHelper.Output("Navigating page to new Programmatic Tree");
                    nw.Navigate(new ProgrammaticTree());
                    break;

                case SubclassNavContainer_State.Navigated:
                    _subclassNavContainer_curState = SubclassNavContainer_State.CrossNavigated;
                    NavigationHelper.Output("Getting ProgrammaticTree");
                    ProgrammaticTree pt = nw.Content as ProgrammaticTree;
                    if (pt == null)
                        NavigationHelper.Fail("Could not get pt");

                    NavigationHelper.Output("Getting MyNavFrameSubclass");
                    MyNavFrameSubclass sc = pt.Children[1] as MyNavFrameSubclass;
                    if (sc == null)
                        NavigationHelper.Fail("Could not get sc");

                    NavigationHelper.Output("Subclass bonus string is: " + sc.SubclassBonusString);
                    NavigationHelper.Output("Setting bonus string to part2Bonus - yeehaw!");
                    sc.SubclassBonusString = "part2Bonus - yeehaw!";
                    MyNavFrameSubclass sc2 = new MyNavFrameSubclass();
                    pt.Children.Add(sc2);
                    NavigationHelper.Output("Navigating first subclass");
                    sc.Navigate(new Uri("SubclassNavContainer_c.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case SubclassNavContainer_State.CrossNavigated:
                    // error here...?
                    // NavigationHelper.FinishTest(NavigationHelper.CompareFileNames(NavigationHelper.expectedFileName, e.Uri) && (e.Content != null));
                    break;

            }
        }
    }

    public class ProgrammaticTree : Canvas
    {
        public ProgrammaticTree() : base()
        {
            Log.Current.CurrentVariation.LogMessage("Inside ProgrammaticTree constructor");
            this.Background=System.Windows.Media.Brushes.Red;
            Button b = new Button();
            b.Content = "I am a button on ProgrammaticTree";
            this.Children.Add(b);
            MyNavFrameSubclass newNC = new MyNavFrameSubclass();
            newNC.Navigate(new Uri("SubclassNavContainer_b.xaml", UriKind.RelativeOrAbsolute));
            this.Children.Add(newNC);
            Log.Current.CurrentVariation.LogMessage("Exiting ProgrammaticTree constructor");
        }
    }

    public class MyNavFrameSubclass : Frame
    {
        public MyNavFrameSubclass() : base()
        {
        }

        public String SubclassBonusString
        {
            get { return _bonusString; } 
            set { _bonusString = value; } 
        }

        String _bonusString = "defaultStringValue";
    }

}

