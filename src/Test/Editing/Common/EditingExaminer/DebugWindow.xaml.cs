// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: DebugWindow.Xaml.cs implement the immediate window with the following
 * features:
 *      1. Window docking to the main window. 
 *      2. Event hook up to monitor the main window's actions.
 *      3. Commandline parser input filtering.
 *
 *******************************************************************************/

using Winforms = System.Windows.Forms;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace EditingExaminer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class DebugWindow : Window
    {
        Window _mainWindow; 
        Hashtable _table;
        Panel _targetElement;
        bool _doDocking = false;
        bool _stateFollowing = false;
        ArrayList _commandHistory;
        int _historyCounter;
        int _dotCounter = 0;
        bool _isWindowDocked = true; 
        TextBox _inputTextBox;
        bool _windowClosed = false;
        System.Windows.Forms.Form _form;
        System.Windows.Forms.RichTextBox _winformsRTB;
        const string _fileName = "EditingExaminer_Saved.zip";

          [StructLayout(LayoutKind.Sequential)]
            public struct MARGINS
            {
                public int cxLeftWidth;
                    public int cxRightWidth;
                public int cyTopHeight;
                    public int cyBottomHeight;
            }


        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
            IntPtr hWnd,
            ref MARGINS pMarInset
            ); 
        
        private static IntPtr ApplicationMessageFilter(
            IntPtr hwnd,
            int message,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled)
        {

            return IntPtr.Zero;
        }       	

        /// <summary>
        /// constructor if the analyzing window
        /// </summary>
        /// <param name="element"></param>
        /// <param name="window"></param>
        public DebugWindow( Panel element, Window window)
        {
            this.InitializeComponent();
            _mainWindow = window;
            _targetElement = element;

            SetEventHanderToMainWindow(true);
        }

        /// <summary>
        /// Set the event handler when the target is changed.
        /// </summary>
        public void TargetChanged()
        {
            if (MainEditor != null)
            {
                MainEditor.Selection.Changed -= new EventHandler(UpdateDisplayTabs);
                MainEditor.TextChanged -= new TextChangedEventHandler(UpdateDisplayTabs);
                MainEditor.Selection.Changed += new EventHandler(UpdateDisplayTabs);
                MainEditor.TextChanged += new TextChangedEventHandler(UpdateDisplayTabs);
            }

            UpdateDisplayTabs(null, null);
        }

        /// <summary>
        /// preperty to show if the window is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _windowClosed;
            }
        }
        
        RichTextBox MainEditor
        {
            get
            {
                return TargetChild as RichTextBox; 
            }
        }

        UIElement TargetChild 
        {
            get
            {
                if (_targetElement != null && _targetElement.Children.Count > 0)
                {
                    return _targetElement.Children[0];
                }
                else
                {
                    return null; 
                }
            }
        }
        bool IsInAttachingRange
        {
            get
            {
                int range = 10; 
                double mainwindowBottom, MainwindowLeft;
                double immediatWindowTop, immediatWindowLeft;
                mainwindowBottom = _mainWindow.Top + _mainWindow.Height;
                MainwindowLeft = _mainWindow.Left;
                immediatWindowTop = this.Top;
                immediatWindowLeft = this.Left;
                if (immediatWindowTop <= mainwindowBottom + range
                    && immediatWindowTop >= mainwindowBottom - range
                    && immediatWindowLeft >= MainwindowLeft - range
                    && immediatWindowLeft <= MainwindowLeft + range)
                {
                    return true;
                }
                else
                {
                    return false; 
                }
           }
        }

        void SetEventHanderToMainWindow(bool addHandler)
        {
            if (addHandler)
            {
                Application.Current.Deactivated += new EventHandler(AvalonDeactivated);
                Application.Current.Activated += new EventHandler(AvalonActivated);
                _mainWindow.LocationChanged += new EventHandler(MainWindowLocationChanged);
                _mainWindow.SizeChanged += new SizeChangedEventHandler(MainWindowSizeChanged);
                _mainWindow.StateChanged += new EventHandler(MainWindowStateChanged);
            }
            else
            {
                Application.Current.Deactivated += new EventHandler(AvalonDeactivated);
                Application.Current.Activated -= new EventHandler(AvalonActivated);
                _mainWindow.LocationChanged -= new EventHandler(MainWindowLocationChanged);
                _mainWindow.SizeChanged -= new SizeChangedEventHandler(MainWindowSizeChanged);
                _mainWindow.StateChanged += new EventHandler(MainWindowStateChanged);
            }
        }

        void MainWindowStateChanged(object sender, EventArgs e)
        {
            if (_mainWindow.WindowState == WindowState.Minimized || _mainWindow.WindowState == WindowState.Normal)
            {
                _stateFollowing = true;
                this.WindowState = _mainWindow.WindowState;
                _stateFollowing = false;
                DoWindowDock();
            }
        }

        void ImmediateWindowLocationChanged(object sender, EventArgs e)
        {
            //if docking process is happening we should not do anything to interrupt it.
            if (_doDocking || _stateFollowing || _mainWindow.WindowState != this.WindowState)
            {
                return; 
            }
            if (IsInAttachingRange)
            {
                _isWindowDocked = true;
                DoWindowDock();
            }
            else
            {
                _isWindowDocked = false;
            }
        }

        void MainWindowLocationChanged(object sender, EventArgs e)
        {
            DoWindowDock();
        }

        void MainWindowSizeChanged(object sender, SizeChangedEventArgs arg)
        {
            DoWindowDock();
        }

        void DoWindowDock()
        {
            if (_isWindowDocked)
            {
                _doDocking = true;
                this.Width = _mainWindow.ActualWidth;
                this.Left = _mainWindow.Left;
                this.Top = _mainWindow.Top + _mainWindow.ActualHeight;
                _doDocking = false;
            }
        }

        void AvalonActivated(object sender, EventArgs e)
        {
            //this will make sure that the analying window is visible.
            this.Topmost = true;
            _mainWindow.Topmost = true;
        }

        void AvalonDeactivated(object sender, EventArgs e)
        {
            this.Topmost = false;
            _mainWindow.Topmost = false;
        }

        void WindowLoaded(object o, RoutedEventArgs args)
        {

            PresentationSource p = PresentationSource.FromVisual(this);
            HwndSource s_hwndSource = p as HwndSource;
            
            if (s_hwndSource != null)
            {
                s_hwndSource.AddHook(new HwndSourceHook(ApplicationMessageFilter));
            //	s_hwndSource.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
        
                try
                {
                    MARGINS margins = new MARGINS();

                    margins.cxLeftWidth = 0;
                    margins.cxRightWidth = 0;
                    margins.cyTopHeight = 45;
                    margins.cyBottomHeight = 0;

                    int hr = DwmExtendFrameIntoClientArea(
                            s_hwndSource.Handle,
                            ref margins
                         );
                    }
                catch
                {                			                			                		
                }
            }

            AddToolTips();
            _historyCounter = 0;
            _commandHistory = new ArrayList();
            UpdateDisplayTabs(null, null);

            if (MainEditor != null)
            {
                MainEditor.Selection.Changed += new EventHandler(UpdateDisplayTabs);
                MainEditor.TextChanged += new TextChangedEventHandler(UpdateDisplayTabs);
            }

             //Set up the Immediate window
            RefreshTable();
            CommandInputBox.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(InputBox_PreviewKeyUp);
            CommandInputBox_FocusEvent(CommandInputBox, null);
                     
            _inputTextBox = ReflectionUtils.GetProperty(CommandInputBox, "EditableTextBoxSite") as TextBox;
            _inputTextBox.TextChanged += new TextChangedEventHandler(_inputTextBox_TextChanged);
            DoWindowDock();
        }

        void WindowClosed(object o, EventArgs args)
        {
            SetEventHanderToMainWindow(false);
            
            //turn off all the events.
            if (MainEditor != null)
            {
                MainEditor.Selection.Changed -= new EventHandler(UpdateDisplayTabs);
                MainEditor.TextChanged -= new TextChangedEventHandler(UpdateDisplayTabs);
            }

            CommandInputBox.PreviewKeyUp -= new System.Windows.Input.KeyEventHandler(InputBox_PreviewKeyUp);
            CommandInputBox_FocusEvent(CommandInputBox, null);
            _inputTextBox.TextChanged -= new TextChangedEventHandler(_inputTextBox_TextChanged);

            _windowClosed = true; 
        }

        void RefreshTable()
        {
            if (_table == null)
            {
                _table = new Hashtable();
            }
            else
            {
                _table.Clear();
            }

            if (MainEditor != null)
            {
                _table.Add("RichTextBox", MainEditor);
                _table.Add("Selection", MainEditor.Selection);
                _table.Add("Document", MainEditor.Document);
            }

            _table.Add("StackPanel", _targetElement);
            _table.Add("null", null);
            _table.Add("True", true);
            _table.Add("False", false);
            DocumentTreeViewItemSelected(null, null);
        }

        void tb2_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _form = new System.Windows.Forms.Form();
            _form.Text = "Winforms RichTextBox";

            _winformsRTB = new System.Windows.Forms.RichTextBox();
            _winformsRTB.Height = _form.Height;
            _winformsRTB.Width = _form.Width;
            _winformsRTB.AllowDrop = true;
            _winformsRTB.EnableAutoDragDrop = true;

            _form.Controls.Add(_winformsRTB);
            _form.Show();
            _form.ResizeEnd += new EventHandler(_form_ResizeEnd);
        }

        void _form_ResizeEnd(object sender, EventArgs e)
        {
            _winformsRTB.Height = _form.Height;
            _winformsRTB.Width = _form.Width;
        }

     
        void w_LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationWindow w = sender as NavigationWindow;
            if (w != null)
            {
                w.Height = 800;
                w.Width = 950;
            }
        }

        void XamlCBChecked(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "XAML Content Displayed";
            XamlCB.IsChecked = false;
            RtfCB.IsChecked = false;
            ((FlowDocument)ClipBoardContent.Document).Blocks.Clear();
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                object o = Clipboard.GetData(DataFormats.Xaml);
                TextRange range = new TextRange(((FlowDocument)ClipBoardContent.Document).ContentEnd, ((FlowDocument)ClipBoardContent.Document).ContentStart);
                range.Text = o.ToString();
            }
        }

        void RtfCBChecked(object sender, RoutedEventArgs e)
        {
            StatusLabel.Content = "RTF Content Displayed";
            XamlCB.IsChecked = false;
            RtfCB.IsChecked = false;
            ((FlowDocument)ClipBoardContent.Document).Blocks.Clear();
            if (Clipboard.ContainsData(DataFormats.Rtf))
            {
                object o = Clipboard.GetData(DataFormats.Rtf);
                TextRange range = new TextRange(((FlowDocument)ClipBoardContent.Document).ContentEnd, ((FlowDocument)ClipBoardContent.Document).ContentStart);
                range.Text = o.ToString();
            }               
        }

        void _inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if the caret is at the beginning of the textBox, move it to the end.
            if (_inputTextBox.SelectionStart == 0 && _inputTextBox.SelectionLength == 0 && _inputTextBox.Text.Length > 0)
            {
                _inputTextBox.SelectionStart = _inputTextBox.Text.Length;
            }
        }
      
        void InputBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            CommandLine commandLine;
            string command;
            try
            {
                switch(e.Key)
                {
                    case Key.Return:
                                         
                        command = CommandInputBox.Text;
                        if (command.Length == 0)
                        {
                            return; 
                        }
                        for (int i = 0; i < _commandHistory.Count; i++ )
                        {
                            if (command == (string)_commandHistory[i])
                            {
                                _commandHistory.RemoveAt(i);
                                i = _commandHistory.Count;
                            }
                        }
                        _commandHistory.Insert(0, command);
                        commandLine = new CommandLine(command.Replace("\r\n", ""), _table);
                        command += "\r\n[" + commandLine.Result.ToString() + "]\r\n";
                        ImmediateWindow.Text += command;
                        ImmediateWindow.ScrollToEnd();
                        CommandInputBox.Text = "";                     
                        break;
                    case Key.Up:
                        if (_historyCounter < _commandHistory.Count)
                        {
                            CommandInputBox.Text = _commandHistory[_historyCounter] as string;
                            _historyCounter++;
                        }
                        break;
                    case  Key.Down:
                        if (_historyCounter > 0)
                        {
                            _historyCounter--;
                            CommandInputBox.Text = _commandHistory[_historyCounter] as string;
                        }
                        break;
                    default:
                     
                        _historyCounter=0;
                        break;
                }
                //if the count of Dots in the command line changed, we need to perfrom a new search for members.
                //Note: the commbobox is using the up/down key too for search the previous/next item. 
                if (!(e.Key == Key.Down || e.Key == Key.Up))
                {
                    CheckDots();
                }
                OnError(null);
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        void CheckDots()
        {
            int currentDotcounter; 
            string str = CommandInputBox.Text;
            currentDotcounter = 0;
        
            //Count the currently dot 
            while (str.Contains("."))
            {
                currentDotcounter++;
                str = str.Substring(str.IndexOf(".") + 1);
            }
            //if Dot count changed, perfrom a new search
            if (currentDotcounter != _dotCounter)
            {
                _dotCounter = currentDotcounter;
                if (currentDotcounter == 0)
                {
                    //no dot
                    SetDefaultItems();
                }
                else
                {
                    //more than one dots
                    SearchForMembers();
                }
            }
        }

        void SearchForMembers()
        {
            Type type;
            PropertyInfo[] pInfo;
            MethodInfo[] mInfos; 
            FieldInfo [] fInfos;
            BindingFlags bindingAttr;
            ArrayList strList;
            string tempStr;
            bindingAttr = BindingFlags.Public 
                | BindingFlags.Instance 
                | BindingFlags.Static 
                | BindingFlags.FlattenHierarchy;
            string str = CommandInputBox.Text.Substring(0, CommandInputBox.Text.LastIndexOf("."));
            tempStr = str + ".";
            try
            {
                if (str.Contains("="))
                {
                    str = str.Substring(str.LastIndexOf("=") + 1);
                }
                if (str.Contains(","))
                {
                    str = str.Substring(str.LastIndexOf(",") + 1);
                }
                else if (str.Contains("("))
                {
                    str = str.Substring(str.LastIndexOf("(") + 1);
                }
                str = str.Trim();
                CommandLine commandline = new CommandLine(str, _table);

                if (commandline.Result.Value != null)
                {
                    type = commandline.Result.Value.GetType();
                }
                else
                {
                    type = ReflectionUtils.FindType(str);
                }

                if (type != null)
                {
                    strList = new ArrayList();
                    pInfo = type.GetProperties(bindingAttr);
                    str = CommandInputBox.Text;
                    CommandInputBox.Items.Clear();
                    CommandInputBox.Text = str;
                    _inputTextBox.SelectionStart = str.LastIndexOf(".") + 1;
                    _inputTextBox.SelectionLength = str.Length - str.LastIndexOf(".") - 1;
                    foreach (PropertyInfo info in pInfo)
                    {
                        if (!strList.Contains(info.Name))
                        {
                            strList.Add(info.Name);
                            AddComboBoxitem(tempStr + info.Name);
                        }
                    }
                    mInfos = type.GetMethods(bindingAttr);
                    foreach (MethodInfo minfo in mInfos)
                    {

                        if (!strList.Contains(minfo.Name))
                        {
                            strList.Add(minfo.Name);
                            AddComboBoxitem(tempStr + minfo.Name);
                        }
                    }
                    fInfos = type.GetFields(bindingAttr);
                    foreach (FieldInfo fInfo in fInfos)
                    {
                        if (!strList.Contains(fInfo.Name))
                        {
                            strList.Add(fInfo.Name);
                            AddComboBoxitem(tempStr + fInfo.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        void AddComboBoxitem(string str)
        {
            ComboBoxItem item = new ComboBoxItem();
            item.Content = str;
            CommandInputBox.Items.Add(item);
        }

        void AddToolTips()
        {
            SetToolTip(CommandInputBox, "Command Line!");
            SetToolTip(TextTreeView, "TreeView of the rich document!");
            SetToolTip(SelectionXaml, "Xaml of the current selection!");
            SetToolTip(TextSerializedXaml, "Xaml of the document created by editing serializer!");
            SetToolTip(CoreXaml, "Xaml of the document created by the Core parser!");
            SetToolTip(ErrorMessageBox, "TextBox used to show errors!");
        }
       
        void CommandInputBox_FocusEvent(object sender, RoutedEventArgs e)
        {
            if (e != null && e.RoutedEvent.Name == "GotFocus")
            {
                CommandInputBox.Foreground = Brushes.Black;
                CommandInputBox.Text = "";
                CommandInputBox.Items.Clear();
                SetDefaultItems();  
            }
            else
            {
                CommandInputBox.Foreground = Brushes.Blue;
                CommandInputBox.Text = "Type Command here!";
                CommandInputBox.Items.Clear();
            }
        }

        void SetDefaultItems()
        {
            ComboBoxItem item;
            IDictionaryEnumerator enumerator = _table.GetEnumerator();
            string str = CommandInputBox.Text;
            CommandInputBox.Items.Clear();
            CommandInputBox.Text = str;
            _inputTextBox.SelectionStart = str.Length;
            while (enumerator.MoveNext())
            {
                item = new ComboBoxItem();
                item.Content = enumerator.Key;
                CommandInputBox.Items.Add(item);
            }
        }
       
        /// <summary>
        /// This methods updates the information of the Editinging area.
        /// it is called when the following event fires.
        /// 1. RichTextBox.TextChanged event fires. (the MainEditor for editing)
        /// 2. The TabControl.SelectionChanged event fires on the Top TabControl.
        /// 3. The TabControl.SelectionChanged event fires on the Bottom TabControl.
        /// 4. TextRange.Changed event fires when the selection in the Main editor (RichTextBox) is changed.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="o1"></param>
        void UpdateDisplayTabs(object o, object o1)
        {
            Exception exception=null;
            try
            {
                //When we try to ad-hoc in Panel, We need the CoreXaml to be selected.
                if (MainEditor == null)
                {
                    TabControl.SelectedItem = TabCoreXaml;
                }                    

                TabItem selectedItem = TabControl.SelectedItem as TabItem;
                
                //The TextRange.changed event is firing each time when the caret is moving. 
                //We don't need to do anything if the SelectionXaml TabItem is not selected.
                if (o is TextSelection && selectedItem.Header.ToString() != "SelectionXaml")
                {
                    return;
                }

                switch (selectedItem.Header.ToString())
                {
                    case "CoreXaml":
                        TextRange range = new TextRange(CoreXaml.Document.ContentEnd, CoreXaml.Document.ContentStart);  
                        if (MainEditor != null)
                        {
                            XamlHelper.TextRange_SetXml(range, XamlHelper.ColoringXaml(XamlHelper.IndentXaml(XamlWriter.Save(MainEditor.Document))));
                        }
                        else
                        {
                            XamlHelper.TextRange_SetXml(range, XamlHelper.ColoringXaml(XamlHelper.IndentXaml(XamlWriter.Save(_targetElement))));
                        }
                        
                        break;
                    case "SelectionXaml":
                        SelectionXaml.Text = XamlHelper.IndentXaml(XamlHelper.TextRange_GetXml(MainEditor.Selection)); break;
                    case "TextSerializedXaml":
                        TextSerializedXaml.Text = XamlHelper.IndentXaml(XamlHelper.TextRange_GetXml(new TextRange(MainEditor.Document.ContentEnd, MainEditor.Document.ContentStart))); break;
                    case "DocumentTree":
                        TreeViewhelper.SetupTreeView(TextTreeView, MainEditor.Document);
                        //Select the Document item by default
                        if (TextTreeView.Items.Count > 0)
                        {
                            ((TreeViewItem)TextTreeView.Items[0]).IsSelected = true; 
                        }
                        break;
                    default:
                        OnError(new Exception("Can't find specified TabItem!"));
                        break;
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            if (exception == null)
            {
                OnError(null);
            }
            else
            {
                OnError(exception);
            }
        }

        void DocumentTreeViewItemSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_table == null)
            {
                _table = new Hashtable();
            }
            if(_table.Contains("SelectedElement"))
            {
                _table.Remove("SelectedElement");
            }

            if (TextTreeView.SelectedItem != null)
            {
                _table.Add("SelectedElement", ((DocumentTreeVeiwItem)TextTreeView.SelectedItem).ItemObject);
            }
        }

        /// <summary>
        /// show the error to the message box.
        /// </summary>
        /// <param name="e"></param>
        public void OnError(Exception e)
        {
            if (e != null)
            {
                ErrorMessageBox.Text = e.Message + "\r\n" + e.StackTrace;
            }
            else
            {
                ErrorMessageBox.Text = "No exception!";
            }

        }

        void SetToolTip(Control element, object tip)
        {
            if (tip == null)
            {
                tip = "No tip";
            }
            element.ToolTip = new ToolTip();
            element.ToolTip = tip;    
        }

        void NewWindowForContent(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            StackPanel sp = new StackPanel();
            FlowDocumentPageViewer fd = new FlowDocumentPageViewer();
            fd.Background = Brushes.Wheat;
            fd.Document = new FlowDocument();
            ((FlowDocument)fd.Document).Background = Brushes.Beige;
            
             TextRange rangeDest = new TextRange(((FlowDocument)fd.Document).ContentEnd, ((FlowDocument)fd.Document).ContentStart);
             TextRange rangeSource = new TextRange(((FlowDocument)ClipBoardContent.Document).ContentEnd, ((FlowDocument)ClipBoardContent.Document).ContentStart);
             rangeDest.Text = rangeSource.Text;
            w.Content = fd;
            w.Show();
        }

        void ParsingXaml(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            try
            {
                string xaml=""; 
                switch (item.Header.ToString())
                {
                    case "SelectionXaml":
                        xaml =  XamlHelper.RemoveIndentation(TextSerializedXaml.Text); 
                        XamlHelper.TextRange_SetXml(MainEditor.Selection, xaml);
                        break;
                    case "TextSerializedXaml":
                        xaml =  XamlHelper.RemoveIndentation(TextSerializedXaml.Text); 
                        XamlHelper.TextRange_SetXml(new TextRange(MainEditor.Document.ContentEnd, MainEditor.Document.ContentStart), xaml);
                        break;
                    case "CoreXaml":
                        TextRange range = new TextRange(CoreXaml.Document.ContentEnd, CoreXaml.Document.ContentStart);
                        xaml = XamlHelper.RemoveIndentation(range.Text);
                        if (MainEditor != null)
                        {
                            MainEditor.Document = (FlowDocument)XamlHelper.ParseXaml(xaml);
                            MainEditor.Selection.Changed += new EventHandler(UpdateDisplayTabs);
                            RefreshTable();
                        }
                        else
                        {
                            _targetElement.Children.Clear();
                            _targetElement.Children.Add(XamlHelper.ParseXaml(xaml) as UIElement);
                            _table["StackPanel"] = _targetElement.Children[0];
                            OnError(null);
                        }
                        break;
                    default:
                        OnError(new Exception("Don't know how to parse the xaml"));
                        break;
                }
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }

        void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                TopPanel.Width = this.ActualWidth - 10;
                TopPanel.Height = this.ActualHeight - 26;

                ErrorMessageBox.Height = 100;
                Panel2.Width = TopPanel.Width;
                Panel2.Height = TopPanel.Height - ErrorMessageBox.Height;

                Row1.Height = new GridLength(Panel2.Height);
                Panel3.Width = Panel2.Width * 0.45;
                TabControl.Height = Panel2.Height;
                TabControl.Width = Panel2.Width * 0.55;
                TextTreeView.Height = Panel2.Height - CommandInputBox.ActualHeight - 7;
                ClipBoardContent.Height = (TextTreeView.Height - 40 > 200) ? TextTreeView.Height - 40 : (TextTreeView.Height - 60);

                Panel3.Height = Panel2.Height;
                ImmediateWindow.Height = Panel3.Height - CommandInputBox.ActualHeight - Label1.ActualHeight;
                CommandInputBox.Width = Panel3.Width;
                ImmediateWindow.Width = Panel3.Width;
                Label1.Width = Panel3.Width;
            }
            catch (Exception exception)
            {
                OnError(exception);
            }
        }
    }
}