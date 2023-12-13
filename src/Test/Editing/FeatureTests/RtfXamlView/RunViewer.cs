// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: Main module for RtfXamlView application.
//              Optional argument is a file name: rtf / xaml format

using System;
using System.IO;
using System.Windows.Forms;

namespace RtfXamlView
{
    static class RunViewer
    {
        [STAThread]
        static void Main(string[] args)
        {
            ////Process command line arguments
            //string fileNameArg = "";

            //if (args.Length > 0)
            //{
            //    fileNameArg = args[0];
            //}

            RtfXamlViewApp viewerApp = new RtfXamlViewApp(args);
            
            try
            {
                viewerApp.Run();
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString(), "Unexpected Error");
               StreamWriter sw = new StreamWriter(Path.GetDirectoryName(viewerApp._szLogFile) + "\\Exception.txt");
               sw.Write(e.ToString());
               sw.Flush();
               sw.Close();
            }
        }
    }
}