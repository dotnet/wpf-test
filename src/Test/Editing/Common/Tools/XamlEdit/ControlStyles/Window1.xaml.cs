// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Xml;


namespace StyleSnooper
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : System.Windows.Window
    {
        private Hashtable _elementArray = new Hashtable();

        public Window1()
        {
            InitializeComponent();
            this.typeComboBox.SelectionChanged += this.ShowStyle;
            this.Loaded += new RoutedEventHandler(StyleSnooper_Loaded);
            this.Closed += new EventHandler(Window1_Closed);
            // Start out by looking at Button.
            //CollectionViewSource.GetDefaultView(this.ElementTypes).MoveCurrentTo(typeof(ComboBox));
        }

        void Window1_Closed(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeShutdown();
        }

        void StyleSnooper_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> typeList = new List<string>();
            FrameworkElement element = null;
            Style style = null;
            foreach (Type type in typeof(FrameworkElement).Assembly.GetTypes())
            {
                if (type.IsPublic &&
                    !type.IsAbstract &&
                    !type.ContainsGenericParameters &&
                    typeof(FrameworkElement).IsAssignableFrom(type) &&
                    type.GetConstructor(Type.EmptyTypes) != null)
                {
                    element = (FrameworkElement)Activator.CreateInstance(type);
                    object defaultStyleKey = element.GetValue(Window1.DefaultStyleKeyProperty);
                    if (defaultStyleKey != null)
                    {
                        // Try to get the default style for the type.
                        style = Application.Current.TryFindResource(defaultStyleKey) as Style;
                        if (style != null)
                        {
                            _elementArray.Add(type.Name, style);
                            typeList.Add(type.Name);
                        }
                    }
                }
            }

            Array stringArr = typeList.ToArray();
            Array.Sort(stringArr);

            foreach (string str in stringArr)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = str;
                this.typeComboBox.Items.Add(item);
            }

        }

        private void ShowStyle(object sender, SelectionChangedEventArgs e)
        {
            // See which type is selected.

            string serializedStyle = "[Style not found]";
            try
            {
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;
                System.Windows.Markup.XamlWriter.Save(_elementArray[((ComboBoxItem)this.typeComboBox.SelectedValue).Content], xmlTextWriter);
                serializedStyle = stringWriter.ToString();
            }
            catch (Exception exception)
            {
                serializedStyle = "[Exception thrown while serializing style]" +
                    Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            // Show the style in a pageviewer.
            if (this.styleTextBox.Document == null)
            {
                this.styleTextBox.Document = new FlowDocument();
            }
            TextRange tr = new TextRange(this.styleTextBox.Document.ContentStart, this.styleTextBox.Document.ContentEnd);
            tr.Text = serializedStyle;
            //  this.styleTextBox.Text = serializedStyle;
        }
    }
}