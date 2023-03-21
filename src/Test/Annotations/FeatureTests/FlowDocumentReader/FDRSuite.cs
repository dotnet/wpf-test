// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;
using System.Windows.Annotations.Storage;
using System.Text.RegularExpressions;

namespace Avalon.Test.Annotations
{
    public class FDRSuite : ATextControlTestSuite
    {
        #region TestSuite Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);
            ReaderMode = DetermineReaderMode(args);
            printStatus("Reader Mode = '" + ReaderMode + "'.");
        }        

        [TestCase_Setup()]
        protected virtual void DoSetup() 
        {
            SetupTestWindow();
            SetContent(LoadContent(Simple));
            SetViewingMode();            
        }

        protected override ATextControlTestSuite.AnnotatableTextControlTypes DetermineTargetControlType(string[] args)
        {
            return AnnotatableTextControlTypes.FlowDocumentReader;
        }

        protected override object CreateWindowContents()
        {
            ContentControl toolbar = new AnnotationToolbar();
            DockPanel.SetDock(toolbar, Dock.Top);

            DockPanel mainPanel = new DockPanel();
            mainPanel.Children.Add(toolbar);
            mainPanel.Children.Add(TextControl);

            return mainPanel;
        }

        protected override AnnotationStore SetupAnnotationStore()
        {
            return new XmlStreamStore(AnnotationStream); // AutoFlush=false.
        }

        #endregion

        #region Protected Methods

        [TestCase_Helper()]
        protected void SetViewingMode()
        {
            ReaderWrapper.ViewingMode = ReaderMode;
        }
        [TestCase_Helper()]
        protected Match GetParameter(string[] args, Regex expression)
        {
            Match match;
            foreach (string arg in args)
            {
                if ((match = expression.Match(arg)).Success)
                    return match;
            }
            return null;
        }
        [TestCase_Helper()]
        protected virtual FlowDocumentReaderViewingMode DetermineReaderMode(string[] args)
        {
            FlowDocumentReaderViewingMode viewingMode = FlowDocumentReaderViewingMode.Page;
            Match match = GetParameter(args, new Regex("/fdrmode=(.*)"));
            if (match != null && match.Success)
            {
                string value = match.Groups[1].Value.ToLower();
                switch (value)
                {
                    case "page":
                        viewingMode = FlowDocumentReaderViewingMode.Page;
                        break;
                    case "twopage":
                        viewingMode = FlowDocumentReaderViewingMode.TwoPage;
                        break;
                    case "scroll":
                        viewingMode = FlowDocumentReaderViewingMode.Scroll;
                        break;
                    default:
                        throw new NotSupportedException(value);
                }
            }
            return viewingMode;
        }

        #endregion

        #region Properties 

        protected FlowDocumentReaderWrapper ReaderWrapper
        {
            get
            {
                return (FlowDocumentReaderWrapper)TextControlWrapper;
            }
        }

        #endregion

        #region Fields

        protected FlowDocumentReaderViewingMode ReaderMode;        

        protected static string Simple = "Simple.xaml";
        protected static string Article1 = "Article1.xaml";

        #endregion
    }
}	

