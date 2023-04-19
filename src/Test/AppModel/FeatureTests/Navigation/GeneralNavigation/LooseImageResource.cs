// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections;
using Microsoft.Test.Logging;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        NavigationWindow _looseImageResource_navWin = null;
        Application _looseImageResource_navApp = null;

        enum LooseImageResource_State
        {
            UnInit,
            Init,
            Nav
        };

        LooseImageResource_State _looseImageResource_currentState = LooseImageResource_State.UnInit;

        private void LooseImageResource_Startup(object sender, StartupEventArgs e)
        {
            _looseImageResource_currentState = LooseImageResource_State.Init;
            NavigationHelper.CreateLog("LooseImageResource");

            _looseImageResource_navApp = System.Windows.Application.Current as Application;
            NavigationHelper.SetStage(TestStage.Run);

            this.StartupUri = new Uri("LooseImageResource_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        void LooseImageResource_Exit(object sender, ExitEventArgs e)
        {
            Microsoft.Test.Logging.LogManager.LogMessageDangerously("Shutting down");
        }

        void LooseImageResource_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_looseImageResource_currentState)
            {
                case LooseImageResource_State.Init:
                    _looseImageResource_navWin = _looseImageResource_navApp.MainWindow as NavigationWindow;
                    break;
            }
        }

        void LooseImageResource_LoadCompleted(object sender, NavigationEventArgs e)
        {
            LooseImageResource_ExecNextState();
        }

        void LooseImageResource_ExecNextState()
        {
            switch (_looseImageResource_currentState)
            {
                case LooseImageResource_State.Init:
                    _looseImageResource_currentState = LooseImageResource_State.Nav;
                    _looseImageResource_navWin.Navigate(new Uri(IMAGEPAGE, UriKind.RelativeOrAbsolute));            
                break;

                case LooseImageResource_State.Nav:
                    NavigationHelper.Output("NavWin src = " + _looseImageResource_navWin.Source);
                    if (_looseImageResource_navWin.Source.ToString().Equals(IMAGEPAGE))
                    {
                        NavigationHelper.Pass("Loose image resource test passed");
                    } 
                    else
                    {
                        NavigationHelper.Fail("Loose image resource test failed");
                    }
                break;
            }    
        }        
    }
}
