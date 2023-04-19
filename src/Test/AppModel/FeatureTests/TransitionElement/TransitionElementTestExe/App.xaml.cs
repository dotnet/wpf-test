// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Navigation;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Test.Loaders;

namespace testapp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : System.Windows.Application
    {
        private static TestLog s_log = TestLog.Current;

        public static string hostIdentifier = "TransitionHost:";
        public enum TransitionHost { NavigationWindow, Frame, Window };
        private TransitionHost _host;
        private Uri _startUpUri;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (TestLog.Current == null)
            {
                new TestLog("TransitionElement");
                s_log = TestLog.Current;
            }

            if (e.Args.Length > 0)
            {
                foreach (string arg in e.Args)
                {
                    if (arg.StartsWith(hostIdentifier))
                    {
                        string h = arg.Substring(hostIdentifier.Length);
                        _host = (TransitionHost)Enum.Parse(typeof(TransitionHost), h);
                    }
                }
            }
            else
            {
                _host = TransitionHost.NavigationWindow;
            }

            s_log.LogEvidence("Host is " + _host.ToString());

            switch (_host)
            {
                case TransitionHost.NavigationWindow:
                    _startUpUri = new Uri("TransitionElement_NavWindow.xaml", UriKind.Relative);
                    break;
                case TransitionHost.Frame:
                    _startUpUri = new Uri("TransitionElement_Frame.xaml", UriKind.Relative);
                    break;
                case TransitionHost.Window:
                    _startUpUri = new Uri("TransitionElement_Window.xaml", UriKind.Relative);
                    break;
                default:
                    throw new ArgumentException("Unkown host specified");
            }

            this.StartupUri = _startUpUri;
            base.OnStartup(e);
        }
    }
}
