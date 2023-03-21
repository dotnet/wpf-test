// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: General base class for tests of the StickyNoteControl hosted in a DocumentViewerBase.

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;
using System.Collections.Generic;
using Annotations.Test.Reflection;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Annotations.Storage;
using Proxies.System.Windows.Annotations;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace Avalon.Test.Annotations
{
    public abstract class AStickyNoteStylingSuite : AStickyNoteControlSuite
    {
        #region TestSuite Overrides

        protected override object CreateWindowContents()
        {
            Grid mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());

            DockPanel viewerPanel = new DockPanel();
            Grid.SetRow(viewerPanel, 0);
            viewerPanel.Children.Add(ViewerBase);
            mainGrid.Children.Add(viewerPanel);

            DockPanel stylePanel = new DockPanel();
            Grid.SetRow(stylePanel, 1);
            Button applyButton = new Button();
            DockPanel.SetDock(applyButton, Dock.Bottom);
            applyButton.Click += ApplyStyle;
            applyButton.Content = "Apply Style";
            stylePanel.Children.Add(applyButton);
            StyleBox = new TextBox();
            StyleBox.TextWrapping = TextWrapping.Wrap;
            StyleBox.AcceptsReturn = true;
            StyleBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            stylePanel.Children.Add(StyleBox);
            mainGrid.Children.Add(stylePanel);

            return mainGrid;
        }

        protected override void DoExtendedSetup()
        {
            ViewerBase.Resources = new ResourceDictionary();
            SetZoom(100);
            CreateAnnotation(new SimpleSelectionData(0, 20, 400));
            StyleBox.Text = GetResourceDictionaryText();
        }

        #endregion

        #region Protected Methods

        protected void UpdateStyleBox()
        {
            StyleBox.Text = GetResourceDictionaryText();
        }

        protected void SetResourceDictionary(ResourceDictionary resources)
        {            
            AnnotationService service = Service;
            AnnotationStore store = service.Store;
            service.Disable();
            DispatcherHelper.DoEvents();

            try
            {
                ViewerBase.Resources.Clear();
                if (resources != null)
                    ViewerBase.Resources = resources;
            }
            finally
            {
                service.Enable(store);
                DispatcherHelper.DoEvents();
            }
        }

        protected void LoadStyle()
        {
            LoadStyle(CaseNumber);
        }

        protected void LoadStyle(string nodeName) 
        {
            Stream stream = Application.GetResourceStream(new Uri(StylesFilename, UriKind.Relative)).Stream;
            StreamReader reader = new StreamReader(stream);
            string file = reader.ReadToEnd();
            reader.Close();

            file = file.Replace("\r\n", "");
            Regex expression = new Regex("<" + nodeName + ">(.*)</" + nodeName + ">");
            Match match = expression.Match(file);
            if (!match.Success)
                failTest("Could not find entry for '" + nodeName + "' in style file.");
            
            ResourceDictionary resources = CreateResourceDictionary(match.Groups[1].Value, true);
            SetResourceDictionary(resources);
            UpdateStyleBox();
        }

        #endregion

        #region Private Methods

        private void CreateTextNote(object sender, EventArgs e)
        {
            AnnotationHelper.CreateTextStickyNoteForSelection(Service, "foobar");
        }
        private void CreateInkNote(object sender, EventArgs e)
        {
            AnnotationHelper.CreateInkStickyNoteForSelection(Service, "foobar");
        }

        private string GetResourceDictionaryText()
        {
            string xaml = string.Empty;
            if (ViewerBase.Resources != null)
            {
                xaml = XamlWriter.Save(ViewerBase.Resources);
                Match match = ResourceDictionaryExpression.Match(xaml);
                xaml = match.Groups[2].Value;
            }
            return xaml;
        }

        private ResourceDictionary CreateResourceDictionary(string newXaml, bool throwExceptions)
        {
            ResourceDictionary resources = null;
            StringReader sr = new StringReader(newXaml);
            try
            {
                XmlTextReader xtr = new XmlTextReader(sr);
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                resources = (ResourceDictionary)XamlReader.Load(xtr);
            }
            catch (Exception e)
            {
                if (throwExceptions)
                    throw;
                else
                    MessageBox.Show(e.ToString());
            }
            finally
            {
                sr.Close();
            }
            return resources;
        }

        private void ApplyStyle(object s, RoutedEventArgs e)
        {
            string newXaml = "<ResourceDictionary xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:s=\"uri:microsoft-clr:Namespace=System;Assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" + StyleBox.Text + "</ResourceDictionary>";
            ResourceDictionary resources = CreateResourceDictionary(newXaml, false);
            SetResourceDictionary(resources);
        }        

        #endregion

        #region Protected Fields

        protected TextBox StyleBox;

        protected string StylesFilename = "Styles.xaml";
        protected Regex ResourceDictionaryExpression = new Regex(@"<ResourceDictionary(.+?)>(.*)</ResourceDictionary>");

        #endregion
    }
}	

