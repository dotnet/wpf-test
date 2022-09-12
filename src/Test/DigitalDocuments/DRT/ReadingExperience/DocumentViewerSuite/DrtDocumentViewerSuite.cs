// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description:  DRX's Suite of DocumentViewer DRTS
//

using DRT;
using System;
using System.ComponentModel;
using System.IO;    //FileInfo, Path
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Markup;     //ParserContext, XamlReader
using System.Windows.Xps.Packaging;     //XpsDocument

namespace DRTDocumentViewerSuite
{
    
    /// <summary>
    /// Stress tests DocumentViewer
    /// </summary>
    public sealed class DrtDocumentViewerSuite : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            return (new DrtDocumentViewerSuite()).Run(args);
        }

        private DrtDocumentViewerSuite()
        {
            DrtName = "DocumentViewerSuite";
            WindowSize = new Size(720, 800);
            WindowTitle = "DocumentViewer Suite";
            TeamContact = "WPF";
            Contact = "Microsoft";
            Suites = new DrtTestSuite[] {
                new BringIntoViewSuite(),
                new CommandingSuite(),
                new StressSuite(),                                
                new StylingSuite(),
                };
        }      
    }           
}

