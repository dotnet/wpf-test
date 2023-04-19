// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.CrossProcess;
using System;
using System.Deployment;
using System.Deployment.Application;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.Test.WPF.AppModel.Deployment
{
    public partial class WebOCFTApp
    {

        void navWebOC(object sender, EventArgs e)
        {
            WebOCFrame.Source = new System.Uri(txtBox.Text);
        }

        void DoNothing(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            txtBox.Text = "Security Exception or otherwise!";
        }

        void CheckURLParams(object sender, EventArgs e)
        {
            try
            {
                string actUri = "";

                try
                {
                    actUri = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.ActivationUri.ToString();
                }
                catch (System.NullReferenceException)
                {
                    actUri = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
                }
                catch (System.Deployment.Application.InvalidDeploymentException)
                {
                    actUri = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
                }

                // For cross-process property bag.
                DictionaryStore.StartClient();

                if ((actUri != "") && (actUri.IndexOf("?") >= 0))
                {
                    string uriToNavigate = actUri.Substring(actUri.IndexOf("?") + 1);
                    string onlyDepUri = actUri.Substring(0, actUri.IndexOf("?"));
                    string strippedDepUri = onlyDepUri.Remove(onlyDepUri.LastIndexOf(@"/") + 1);
                    if ((uriToNavigate.Length > 8) && (uriToNavigate.StartsWith("samedir:")))
                    {
                        uriToNavigate = strippedDepUri + uriToNavigate.Substring(8);
                    }
                    txtBox.Text = uriToNavigate;
                    WebOCFrame.Source = new System.Uri(uriToNavigate);
                    MainPage.WindowTitle = "Navigation Completed";
                }
                else if (DictionaryStore.Current["WebOCTestUrl"] != null)
                {
                    string uriToNavigate = DictionaryStore.Current["WebOCTestUrl"];

                    if ((uriToNavigate.Length > 8) && (uriToNavigate.StartsWith("samedir:")))
                    {
                        uriToNavigate = AppDomain.CurrentDomain.BaseDirectory + uriToNavigate.Substring(8);
                    }
                    txtBox.Text = uriToNavigate;
                    WebOCFrame.Source = new System.Uri(uriToNavigate);

                    MainPage.WindowTitle = "Navigation Completed";
                }
                else
                {
                    string[] availableHTML = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.htm");
                    if (availableHTML.Length > 0)
                    {
                        txtBox.Text += availableHTML[0] + "\n";
                        WebOCFrame.Source = new System.Uri(availableHTML[0]);
                        MainPage.WindowTitle = "Navigation Completed";
                    }
                    else
                    {
                        throw new Exception("This particular FT WebOC app must be launched from directory containing at least one .htm file!");
                    }
                }
            }
            catch (Exception except)
            {
                MainPage.WindowTitle = "EXCEPTION THROWN!";
                txtBox.Height = 640;
                panel.Height = 640;
                txtBox.Text += except.ToString();
            }
        }
    }
}
