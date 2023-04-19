// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Security.Permissions;
using System.Deployment.Application;
using System.Deployment;
using System.Windows.Interop;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Microsoft.Test.WPF.AppModel.Deployment
{
    public partial class WebOCXbap
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
                string actUri = System.Windows.Interop.BrowserInteropHelper.Source.ToString();
                if (actUri != "")
                {
                    if (actUri.IndexOf("?") >= 0)
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
                        DateTime rightNow = System.DateTime.Now;
                        rightNow = rightNow.AddSeconds(10);
                        while (System.DateTime.Compare(System.DateTime.Now, rightNow) < 0)
                            Thread.Sleep(500);
                        MainPage.WindowTitle = "Navigation Completed";
                    }
                    else // Assume we're looking for htmlmarkup.htm in the same dir, cos we can't call Directory.GetFiles()
                    {
                        string strippedDepUri = actUri.Remove(actUri.LastIndexOf(@"/") + 1);
                        string uriToNavigate = strippedDepUri + "deploy_htmlmarkup.htm";
                        txtBox.Text = uriToNavigate;
                        WebOCFrame.Source = new System.Uri(uriToNavigate);
                        DateTime rightNow = System.DateTime.Now;
                        rightNow = rightNow.AddSeconds(10);
                        while (System.DateTime.Compare(System.DateTime.Now, rightNow) < 0)
                            Thread.Sleep(500);
                        MainPage.WindowTitle = "Navigation Completed";
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
