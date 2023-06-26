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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;

namespace XamlPadEdit 
{
    /// <summary>
    /// Interaction logic for Interpreter.xaml
    /// </summary>

    public partial class Interpreter : Page
    {
        public Window helpWindow=null;
        TextBox _interpreterComboBox = null;
        public bool popupOpen = false;
        CommandParser _copyOfcommandParser = null;

        public Interpreter()
        {
            InitializeComponent();
        }

        public CommandParser InterpreterCommandParser
        {
            set
            {
                _copyOfcommandParser = value;
            }

            get
            {
                return _copyOfcommandParser;
            }
        }

        void Interpreter_Loaded(object sender, RoutedEventArgs e)
        {
            _interpreterComboBox = LogicalTreeHelper.FindLogicalNode(this, "CommandInputBox") as TextBox;
            _interpreterComboBox.PreviewKeyUp += new KeyEventHandler(InterpreterComboBox_PreviewKeyUp);
            myPopup.StaysOpen = false;
        }


        void InterpreterComboBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Key.LeftCtrl == e.Key)
            {
                if (_interpreterComboBox.Text[_interpreterComboBox.Text.Length - 1] == '.')
                {
                    PropertiesBox.Items.Clear();
                    if (PopulateListBox())
                    {   
                        myPopup.IsOpen = true;
                        popupOpen = true;
                        PropertiesBox.PreviewKeyUp += new KeyEventHandler(PropertiesBox_PreviewKeyUp);
                        PropertiesBox.PreviewMouseDoubleClick += new MouseButtonEventHandler(PropertiesBox_PreviewMouseDoubleClick);
                        PropertiesBox.ScrollIntoView(((ListBoxItem)(PropertiesBox.Items[0])));
                        PropertiesBox.Focus();
                    }
                }
                e.Handled = true;
            }
        }

        void PropertiesBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox listbox = sender as ListBox;
            ListBoxItem lbi = listbox.SelectedItem as ListBoxItem;
            if (lbi != null)
            {
                _interpreterComboBox.Text += lbi.Content.ToString().Replace("()", "").Trim();
                myPopup.IsOpen = false;
                _interpreterComboBox.Focus();
                _interpreterComboBox.CaretIndex = _interpreterComboBox.GetLineLength(0);
                e.Handled = true;
            }
        }

        void PropertiesBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListBox listbox = sender as ListBox;
                ListBoxItem lbi = listbox.SelectedItem as ListBoxItem;
                _interpreterComboBox.Text += lbi.Content.ToString().Replace("()","").Trim();
                myPopup.IsOpen = false;
                _interpreterComboBox.Focus();
                _interpreterComboBox.CaretIndex = _interpreterComboBox.GetLineLength(0);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                myPopup.IsOpen = false;
                //popupOpen = false;
                _interpreterComboBox.Focus();
                e.Handled = true;
                return;
            }
            else
            {
                myPopup.IsOpen = true;
                popupOpen = true;
            }
        }

        private bool PopulateListBox()
        {
            string cmd = _interpreterComboBox.Text;
            cmd = ObtainCurrentCmd(cmd).Trim();
            try
            {
                if (cmd.Length > 1)
                {
                    object o = InterpreterCommandParser.ParsePropertyCommand(cmd.Substring(0, cmd.Length - 1));
                    if (o != null)
                    {
                        ArrayList listArr = o as ArrayList;  
                        foreach (object obj in listArr)
                        {
                            string str = obj as string;
                            ListBoxItem l = new ListBoxItem();
                            if (str.Contains("()"))
                            {
                                l.Foreground = Brushes.Crimson;
                            }
                            l.Content = str;
                            PropertiesBox.Items.Add(l);
                        }
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        private string ObtainCurrentCmd(string cmd)
        {
            int tail = cmd.Length - 1;
            while (tail > 0)
            {
                if (char.IsLetterOrDigit(cmd[tail]) || (cmd[tail] == '.') || (cmd[tail]=='_'))
                {
                    tail--;
                }
                else
                {
                    return cmd.Substring(tail+1);
                }
            }
            return cmd;
        }


        void CommandInputBox_FocusEvent(object sender, RoutedEventArgs e)
        {
            if (popupOpen) 
            {
                if (e.RoutedEvent.Name == "GotFocus")
                {
                    myPopup.IsOpen = popupOpen = false;
                    e.Handled = true;
                }
                else
                {   
                }
            }
            else
            if (e != null && e.RoutedEvent.Name == "GotFocus")
            {
                CommandInputBox.Foreground = Brushes.Black;
                CommandInputBox.Text = "";

            }
            else
            {
                CommandInputBox.Foreground = Brushes.Blue;
                CommandInputBox.Text = "Type Command here!";
            }
        }

        void helpButton_Click(object sender, RoutedEventArgs e)
        {
            if(helpWindow == null)
            {
                helpWindow = new Window();
                //helpWindow.Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Alert 17.ico", UriKind.RelativeOrAbsolute)).Stream);
                helpWindow.SizeToContent = SizeToContent.Height;
                helpWindow.MaxHeight = 1000;
                helpWindow.Title = "Command Interpreter Instruction Manual";
                helpWindow.Closed += new EventHandler(helpWindow_Closed);
                InterpreterHelpPage helpPage = new InterpreterHelpPage();
                helpWindow.Content = helpPage;
            }
            helpWindow.Show();
        }

        void helpWindow_Closed(object sender, EventArgs e)
        {
            helpWindow = null;
        }
    }
}