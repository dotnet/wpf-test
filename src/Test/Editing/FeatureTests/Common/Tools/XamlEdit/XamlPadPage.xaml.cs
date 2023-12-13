// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections;
using System.Windows.Controls.Primitives;
using O12FFVC;
using System.Threading;
using System.Windows.Media.Imaging;



namespace XamlPadEdit
{
    public partial class XamlPadPage : Page
    {
        public static RoutedCommand SpellerReform = new RoutedCommand("SpellerReform", typeof(XamlPadPage), null);
        public static RoutedCommand CharacterFormatting = new RoutedCommand("CharacterFormatting", typeof(XamlPadPage), null);
        public static RoutedCommand OpenFind = new RoutedCommand("Find", typeof(XamlPadPage), null);
        public static RoutedCommand FindNext = new RoutedCommand("FindNext", typeof(XamlPadPage), null);
        public static RoutedCommand OpenGoTo = new RoutedCommand("GoTo", typeof(XamlPadPage), null);
        public static RoutedCommand ExecuteIndent = new RoutedCommand("ExecuteIndent", typeof(XamlPadPage), null);

        #region Constructors

        public XamlPadPage()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Interop Definitions

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int PostMessage(IntPtr hWnd, uint msg, uint lParam, uint wParam);

        private static uint MAKELPARAM(int low, int high)
        {
            return (uint)((high << 16) | (low & 0xffff));
        }

        #endregion Interop Definitions

        #region Private Fields

        private int _mouseCaptureEventHandlerCount = 0;
        private ColorPallette _pallete = null;
        private Dispatcher _mainDispatcher= null;
        private Process _p = null;
        private GoToLineDialog _gotoLineDialog= null;
        private FindAndReplace _findWindow = null;
        private bool _htmlFile = false;
        private Form1 _webform = null;
        private NoSelectionDialog _dialogOnLoading;
        private bool _wasContentEverRendered;
        private int _errorLineNumber;
        private int _errorPosition;
        private static DispatcherTimer s_dispatcherTimer;
        private bool _saveContentOnSuccessfulRender;
        private bool _contentRenderedSuccessfully;
        private Trackball _trackball;
        private bool _isTrackballEnabled;
#if !XamlPadExpressApp
        private SnippetManager _snippetManager;
#endif
        private bool _isPageLoaded;
        private string _lastFontName;
        private string _lastFontSize;
        private string _lastZoomFactor;
        private GridLength _lastEditorHeight;
        private static GridLength s_dividerHeight = new GridLength(5);
        private GridLength _lastVisualWidth = new GridLength(260);
        private static GridLength s_dividerWidth = new GridLength(5);

        private Window _commandInterpreter = null;
        private Interpreter _interpreterGUI = null;
        private TextBox _interpreterResultsBox = null;
        private TextBox _interpreterComboBox = null;
        private string _commandString = "";
        private CommandParser _commandParser = null;
        private UIElement _mainRoot;
        private RichTextBox _rtbBackup = null;
        private bool _containsRichTextBox = false;
        private static string s_cantDisplayWindowMessage = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ><Italic>Window content does not run in the XamlPad pane; click Refresh or press F5 to run in a separate window</Italic></TextBlock>";
        private int _lineCount = 0;


        #endregion Private Fields


        #region Private Properties

        private object RenderedContent
        {
            get
            {
                return (contentRenderArea != null) ? contentRenderArea.Content : null;
            }
        }

        #endregion Private Properties

        #region Private Methods


        // This tries to parse the text and regenerates the 3d and 2d views if the text is valid Xaml
        private void AttemptUpdate(object sender, EventArgs args)
        {
            if (_commandParser != null)
            {
                _commandParser.ClearObjectArrHashTable();
            }
            ClearDispatcherTimer();
            _containsRichTextBox = false;

            ParseResult r = XamlHelper.ParseXaml(editableTextBox.Text);
            string str = editableTextBox.Text.Replace("\r\n","");
            str = str.Replace(" ","");
            if ((str.Contains("<RichTextBox>") && (str.Contains("</RichTextBox>"))) || (str.Contains("<RichTextBox")))
            {
                _containsRichTextBox = true;
            }
            if (r.Root != null)
            {
                NormalRender(r.Root);
            }
            else
            {
                if (!_wasContentEverRendered)
                {
                    // The saved text was invalid, so fall back to the default
                    editableTextBox.Text = XamlHelper.InitialXaml;
                    r = XamlHelper.ParseXaml(editableTextBox.Text);
                    Debug.Assert(r.Root != null);
                    SetStatusText("Saved file was not successfully parsed. Default content created.");
                    NormalRender(r.Root);
                }
                else
                {
                    //Make red and return
                    editableTextBox.Foreground = Brushes.Red;
                    SetStatusText(r.ErrorMessage);
                    _errorLineNumber = r.ErrorLineNumber;
                    _errorPosition = r.ErrorPosition;
                    ((UIElement)goToErrorHyperlink.Parent).Visibility = Visibility.Visible;
                    goToErrorHyperlink.Inlines.Clear();
                    goToErrorHyperlink.Inlines.Add(new Run("Jump To: line " + _errorLineNumber + " col " + _errorPosition));
                }
            }
        }

        private static void ClearDispatcherTimer()
        {
            if (s_dispatcherTimer != null)
            {
                s_dispatcherTimer.Stop();
                s_dispatcherTimer = null;
            }
        }

        private void DisplayValidationError(string element)
        {
            string message = String.Format("Please enter a valid value for {0}.", element);
            MessageBox.Show(message, "XamlPad", MessageBoxButton.OK, MessageBoxImage.Error);
        }

#if !XamlPadExpressApp
        private void HelpCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationWindow helpWindow;    // Window with help contents.
            double workAreaWidth;           // Width of desktop work area (unobscured by taskbar/toolbars).
            bool overlap;                   // Whether the help window should overlap with main window.

            helpWindow = new NavigationWindow();
            Window appWindow = Application.Current.MainWindow;

            //
            // Set the size and position of helpWindow and appWindow to 
            // emulate an "on-the-right" help pane. 
            //
            if (appWindow.WindowState != WindowState.Normal)
            {
                appWindow.WindowState = WindowState.Normal;
            }
            workAreaWidth = SystemParameters.WorkArea.Width;
            helpWindow.Top = appWindow.Top;
            helpWindow.Height = appWindow.Height;
            helpWindow.Width = 240d;
            overlap = false;

            // If there isn't enough room, we'll either shrink the main window or overlap.
            if (appWindow.Left + appWindow.Width + helpWindow.Width > workAreaWidth)
            {
                // If we can adjust the width, let's shrink the main window a bit;
                // otherwise, we'll just overlap with it.
                if (workAreaWidth - appWindow.Left - helpWindow.Width > appWindow.MinWidth)
                {
                    appWindow.Width = workAreaWidth - appWindow.Left - helpWindow.Width;
                }
                else
                {
                    overlap = true;
                }
            }

            // Set the help window to the side or overlapping the main window.
            if (overlap)
            {
                helpWindow.Left = SystemParameters.WorkArea.Right - helpWindow.Width;
            }
            else
            {
                helpWindow.Left = appWindow.Left + appWindow.Width;
            }

            helpWindow.Navigate(new Uri("XamlPadHelp.xaml", UriKind.RelativeOrAbsolute));
            helpWindow.Title = "XamlPad Help";
            helpWindow.ShowInTaskbar = false;
            helpWindow.Owner = appWindow;
            helpWindow.Show();
        }
#endif

        private void InitializeMainWindow()
        {
            Window mainWindow = Application.Current.MainWindow;
            mainWindow.StateChanged += new EventHandler(mainWindow_StateChanged);
            _mainDispatcher = this.Dispatcher;
           // mainWindow.Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Alert 17.ico", UriKind.RelativeOrAbsolute)).Stream);
            //mainWindow.Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("XamlEdit.ico", UriKind.RelativeOrAbsolute)).Stream);
            mainWindow.MinWidth = 400;
            textBoxRow.Height = new GridLength(mainWindow.ActualHeight / 2);

            //TabControl.Height = mainWindow.ActualHeight / 2;
#if !XamlPadExpressApp
            mainWindow.Title = "XamlEdit";
            ResourceDictionary resources = new ResourceDictionary();
            resources.Source = new Uri("Resources.xaml", UriKind.Relative);
            Resources.MergedDictionaries.Add(resources);
#endif

#if XamlPadExpressApp
            showVisualTreeButton.Visibility = Visibility.Collapsed;
            ((NavigationWindow)mainWindow).ShowsNavigationUI = false;
#endif

            mainWindow.Background = SystemColors.ControlBrush;

            // Set up custom commands
            try
            {
                SetupCustomCommands(mainWindow);
            }
            catch (System.Security.SecurityException)
            {
                SetStatusText("Security restrictions prevent F5 support.");
            }

            // Load saved content and display in text box
            editableTextBox.Text = XamlHelper.LoadSavedXamlContent();
        }

        System.Windows.Forms.NotifyIcon _nfyico = null;
        void mainWindow_StateChanged(object sender, EventArgs e)
        {
            if (systrayButton.IsChecked == true)
            {
                if (((Window)sender).WindowState == WindowState.Minimized)
                {
                    ((Window)sender).ShowInTaskbar = false;
                    if (_nfyico == null)
                    {
                        Dispatcher.CurrentDispatcher.ShutdownStarted += new EventHandler(CurrentDispatcher_ShutdownStarted);
                        System.ComponentModel.IContainer cnt = new System.ComponentModel.Container();
                        _nfyico = new System.Windows.Forms.NotifyIcon(cnt);
                        _nfyico.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("XamlEdit.ico", UriKind.RelativeOrAbsolute)).Stream);
                        _nfyico.Text = "XamlEdit";
                        _nfyico.MouseUp += new System.Windows.Forms.MouseEventHandler(nfyico_MouseUp);
                        _nfyico.Visible = true;

                        System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
                        System.Windows.Forms.MenuItem item = new System.Windows.Forms.MenuItem("Open");
                        item.Click += new EventHandler(item_Click);
                        System.Windows.Forms.MenuItem item1 = new System.Windows.Forms.MenuItem("Exit");
                        item1.Click += new EventHandler(item1_Click);
                        cm.MenuItems.Add(item);
                        cm.MenuItems.Add(item1);
                        _nfyico.ContextMenu = cm;
                    }
                }
                else
                {
                    ((Window)sender).ShowInTaskbar = true;
                    editableTextBox.Focus();
                    scroll.ScrollToHorizontalOffset(0);
                    // ((Window)sender).Show();
                }
            }
        }

        void CurrentDispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (_nfyico != null)
            {
                _nfyico.Visible = false;
            }
        }

        void nfyico_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    Application.Current.MainWindow.WindowState = WindowState.Normal;   
                }
        }

        void item1_Click(object sender, EventArgs e)
        {
            _nfyico.Visible = false;
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        void item_Click(object sender, EventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void NormalRender(object root)
        {
            // Render the content in the Frame
            _saveContentOnSuccessfulRender = true;
            RenderTree(root);

            // Update the status bar
            if (!String.IsNullOrEmpty(XamlHelper.SavedXamlLocation))
                SetStatusText("Done. Markup saved to " + XamlHelper.SavedXamlLocation + ".");
            else
                SetStatusText("Done.");

            // Make the TextBox content black to indicate no errors
            editableTextBox.Foreground = Brushes.Black;

            // Collapse the "Go to error" link
            ((UIElement)goToErrorHyperlink.Parent).Visibility = Visibility.Collapsed;

            _wasContentEverRendered = true;
        }

        private void PrintCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();

            object content = RenderedContent;
            if ((pd.ShowDialog() == true) && (content != null))
            {
                string msg = "XamlPad - " + content.GetType().ToString();

                // note, IDocumentPaginatorSource takes priority
                // To print as a visual, wrap your IDocumentPaginatorSource implementer in a Canvas
                // or other visual not implementing IDP.

                if (content is IDocumentPaginatorSource)
                {
                    pd.PrintDocument(((IDocumentPaginatorSource)content).DocumentPaginator, msg);
                    SetStatusText("Printed using IDocumentPaginatorSource interface");
                }
                else if (content is Visual)
                {
                    pd.PrintVisual((Visual)content, msg);
                    SetStatusText("Printed current view of Visual");
                }
                else
                {
                    SetStatusText("Cannot print element if it is not descended from Visual and does not implement IDocumentPaginator");
                }
            }
            else
            {
                SetStatusText("User cancelled printing");
            }
        }

        private void RefreshCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            // Force a parse and render of the XAML content
            AttemptUpdate(this, null);

            if (XamlHelper.LastParsedContent != null)
            {
                // If the content is a Window, create a new window and render it in that
                Window window = XamlHelper.LastParsedContent as Window;
                if (window != null)
                {
                    window.Show();
                }
            }
        }

        private void RenderTree(object root)
        {
            if (root is Window)
            {
                // We can run a Window in the XamlPad pane; it has to run in a separate Window.
                // Show an error message that you have to press <F5> to see it.
                ParseResult t = XamlHelper.ParseXaml(s_cantDisplayWindowMessage, true); // "Window content does not run in the XamlPad pane; press F5 to run in a separate window"
                contentRenderArea.Content = t.Root;
            }
            else
            {
                // Show the content in the XamlPad pane
                _mainRoot = root as UIElement;
                contentRenderArea.Content = root;
                if (_containsRichTextBox)
                {
                    ShowTheEditingTabs(null, null);
                }
                else
                {
                    EditingExtras.Visibility = Visibility.Hidden;
                }
            }

            // Potentially enable the trackball
            Trackball newTrackball = new Trackball();

            // Did it find any Viewport3Ds?
            if ((root is FrameworkElement) && (newTrackball.AttachAndFindViewports((FrameworkElement)root)))
            {
                newTrackball.Enabled = _isTrackballEnabled;
                _trackball = newTrackball;
                enableCameraButton.Visibility = Visibility.Visible;
            }
            // If not, clear out any old Trackball which may be present and hide the button.
            else
            {
                _trackball = null;
                enableCameraButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SerializeCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            XmlWriter xmlWriter = new XmlTextWriter(writer);

            XamlWriter.Save(RenderedContent, xmlWriter);
            xmlWriter.Close();
            writer.Close();
            string s = sb.ToString();

            // Save serialized xaml markup to file
            StreamWriter sr = File.CreateText("XamlPad_Serialized.xaml");
            sr.WriteLine(s);
            sr.Close();
        }

        private void SetStatusText(string value)
        {
            string s = "";
            if (!String.IsNullOrEmpty(value))
            {
                s = value.Replace('\r', ' ').Replace('\n', ' ');
            }

            statusText.Text = s;
        }

        private void SetupCustomCommand(Window window, string name,
            KeyGesture gesture, ExecutedRoutedEventHandler handler)
        {
            RoutedCommand command;
            CommandBinding binding;

            command = new RoutedCommand(name, this.GetType());
            command.InputGestures.Add(gesture);
            binding = new CommandBinding(command, handler);
            window.CommandBindings.Add(binding);
        }

        private void SetupCustomCommands(Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            SetupCustomCommand(window, "Refresh", new KeyGesture(Key.F5), RefreshCommandExecute);
            SetupCustomCommand(window, "Serialize", new KeyGesture(Key.F6), SerializeCommandExecute);
            SetupCustomCommand(window, "Print", new KeyGesture(Key.F7), PrintCommandExecute);

#if !XamlPadExpressApp
            SetupCustomCommand(window, "Help", new KeyGesture(Key.F1), HelpCommandExecute);
            SetupCustomCommand(window, "ToggleRtf", new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift),
                ToggleRtfCommandExecute);
#endif
        }

#if !XamlPadExpressApp
        private void SetupSnippets(TextBox textbox)
        {
            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(SetupSnippetsTask), textbox);
        }

        private object SetupSnippetsTask(object arg)
        {
            TextBox textbox = (TextBox)arg;
            ContextMenu menu;

            if (_snippetManager == null)
            {
                _snippetManager = new SnippetManager();
                if (!_snippetManager.LoadSnippets())
                {
                    _snippetManager.SetDefaultSnippets();
                }
            }

            menu = new ContextMenu();
            menu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(ClickMenuItem),true);
            foreach (object o in _snippetManager.ContextMenuItems)
            {
                menu.Items.Add(o);
            }

            MenuItem styleItem = new MenuItem();
            styleItem.Header = "Custom Styles";
            styleItem.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(ClickMenuItem), true);
            menu.Items.Add(styleItem);
            foreach (object o in _snippetManager.ContextMenuStyleItems)
            {
                styleItem.Items.Add(o);
            }

            MenuItem brushItem = new MenuItem();
            brushItem.Header = "Gradiants";
            brushItem.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(ClickMenuItem), true);
            menu.Items.Add(brushItem);
            foreach (object o in _snippetManager.ContextMenuBrushItems)
            {
                brushItem.Items.Add(o);
            }

            MenuItem sampleItem = new MenuItem();
            sampleItem.Header = "Samples";
            sampleItem.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(ClickMenuItem), true);
            menu.Items.Add(sampleItem);
            foreach (object o in _snippetManager.ContextMenuSampleItems)
            {
                sampleItem.Items.Add(o);
            }

            MenuItem findReplaceItem = new MenuItem();
            findReplaceItem.Header = "Find/Replace (Ctrl F/H)";
            findReplaceItem.Click += new RoutedEventHandler(findReplaceItem_Click);
            menu.Items.Add(findReplaceItem);

            MenuItem gotoItem = new MenuItem();
            gotoItem.Header = "Goto Line # (Ctrl G)";
            gotoItem.Click += new RoutedEventHandler(gotoItem_Click);
            menu.Items.Add(gotoItem);

            MenuItem indentItem = new MenuItem();
            indentItem.Header = "Indent Xaml (Ctrl I)";
            indentItem.Click += new RoutedEventHandler(indentItem_Click);
            menu.Items.Add(indentItem);

            textbox.ContextMenu = menu;

            return null;
        }

        void indentItem_Click(object sender, RoutedEventArgs e)
        {
            editableTextBox.Text = XamlHelper.IndentXaml(editableTextBox.Text);
        }

        void findReplaceItem_Click(object sender, RoutedEventArgs e)
        {
            DoOpenFindCommand(sender,  (ExecutedRoutedEventArgs)null);
        }

        void gotoItem_Click(object sender, RoutedEventArgs e)
        {
            DoOpenGoToCommand(sender, (ExecutedRoutedEventArgs)null);
        }

        private void ClickMenuItem(object sender, RoutedEventArgs e)
        {
            object item = sender;
            if (sender as ContextMenu == null)
            {
                item = ((MenuItem)sender).ItemContainerGenerator.ItemFromContainer(
                (DependencyObject)e.OriginalSource);
            }
            else
            {
                item = ((ContextMenu)sender).ItemContainerGenerator.ItemFromContainer(
                 (DependencyObject)e.OriginalSource);
            }
            Snippet snippet = item as Snippet;
            if (snippet != null)
            {
                using (this.editableTextBox.DeclareChangeBlock())
                {
                    this.editableTextBox.SelectedText = snippet.Body
                        .Replace("|", this.editableTextBox.SelectedText);
                }
            }
            else
            {
                SnippetAction action = item as SnippetAction;
                if (action != null)
                {
                    action.Execute(this.editableTextBox, _snippetManager);
                }
            }
        }
#endif

#if !XamlPadExpressApp
        private void ToggleRtfCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            Assembly assembly;
            Type editorType;
            PropertyInfo converterPropertyInfo;
            bool enabled;

            // Read the current property value for the TextEditor in this AppDomain through reflection.
            assembly = System.Reflection.Assembly.GetAssembly(typeof(System.Windows.FrameworkElement));
            editorType = assembly.GetType("System.Windows.Documents.TextEditor");
            converterPropertyInfo = editorType.GetProperty("IsXamlRtfConverterEnabled",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            enabled = (bool)converterPropertyInfo.GetValue(null, null);

            // Toggle the converter enabled value.
            enabled = !enabled;
            SetStatusText("RTF/XAML converter is being " + ((enabled) ? "enabled." : "disabled."));
            converterPropertyInfo.SetValue(null, enabled, null);
        }
#endif


        private void UpdateXamlFont()
        {
            FontFamily newFont = new FontFamily(fontNameCombo.Text);
            if (newFont == null)
            {
                DisplayValidationError("font name");
            }
            else
            {
                editableTextBox.FontFamily = newFont;

                if (fontSizeCombo.Text.Length > 0)
                {
                    try
                    {
                        double newFontSize = Double.Parse(fontSizeCombo.Text);
                        editableTextBox.FontSize = newFontSize;
                    }

                    catch (FormatException)
                    {
                        DisplayValidationError("font size");
                    }

                    catch (OverflowException)
                    {
                        DisplayValidationError("font size");
                    }
                }

                _lastFontName = fontNameCombo.Text;
                _lastFontSize = fontSizeCombo.Text;
            }
        }

        private void UpdateZoomFactor()
        {
            if (zoomCombo.Text.Length > 0)
            {
                // Try to parse the value
                string zoomText = zoomCombo.Text.Replace('%', '\0');
                try
                {
                    double zoomValue = Double.Parse(zoomText);
                    zoomValue /= 100.0;

                    // Update the ScaleTransform
                    frameScaleTransform.ScaleX = zoomValue;
                    frameScaleTransform.ScaleY = zoomValue;
                }
                catch (FormatException)
                {
                    DisplayValidationError("zoom factor");
                }
                catch (OverflowException)
                {
                    DisplayValidationError("zoom factor");
                }

                _lastZoomFactor = zoomCombo.Text;
            }
        }

        #endregion Private Methods

        #region Event Handlers

        private void ContentFrameInitialized(object sender, EventArgs e)
        {
            // Now that the Frame is initialized, we can set the current index
            // on the Zoom dropdown.  Doing so will trigger an event that
            // will act on the Frame, which is why we don't want to do this
            // earlier.
            zoomCombo.SelectedIndex = 3;
        }

        private void ControlGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
//            FocusManager.SetFocusedElement((Window)this.Parent, (UIElement)sender);
        }

        private void EnableCameraButtonClicked(object sender, RoutedEventArgs e)
        {
            // The button should only be visible if the Trackball is present
            Debug.Assert(_trackball != null);

            _isTrackballEnabled = !_isTrackballEnabled;
            _trackball.Enabled = _isTrackballEnabled;
            enableCameraButton.Content = (_isTrackballEnabled ? "Dis" : "En") + "able 3D Camera Control";
        }

        private void FontNameInitialized(object sender, EventArgs e)
        {
            // Populate with font names
            ICollection<FontFamily> fonts = Fonts.SystemFontFamilies;
            foreach (FontFamily font in fonts)
            {
                fontNameCombo.Items.Add(font.ToString());
            }
        }

        private void FontNameLostFocus(object sender, RoutedEventArgs e)
        {
            if (fontSizeCombo.Text != _lastFontSize)
            {
                UpdateXamlFont();
            }
        }

        private void FontNameSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (fontNameCombo.SelectedItem != null)
            {
                fontNameCombo.Text = fontNameCombo.SelectedItem.ToString();
                UpdateXamlFont();
            }
        }

        private void FontSizeLostFocus(object sender, RoutedEventArgs e)
        {
            if (fontSizeCombo.Text != _lastFontSize)
            {
                UpdateXamlFont();
            }
        }

        private void FontSizeSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (fontSizeCombo.SelectedItem != null)
            {
                fontSizeCombo.Text = ((ComboBoxItem)fontSizeCombo.SelectedItem).Content.ToString();
                UpdateXamlFont();
            }
        }

        private void FrameContentRendered(object sender, EventArgs args)
        {
            // The rendering of the frame was completed successfully, so
            // go ahead and save the XAML to disk as long as we're not
            // rolling back the Frame content because the XAML in the
            // TextBox was invalid.
            if (_saveContentOnSuccessfulRender && _isPageLoaded)
            {
                try
                {
                    XamlHelper.SaveXamlContent(editableTextBox.Text);
                }
                catch (ApplicationException e)
                {
                    SetStatusText(e.Message);
                }
            }

            _contentRenderedSuccessfully = true;
        }

        private void ShowEditor(object sender, RoutedEventArgs e)
        {
            // Show the editor
            if (textBoxRow != null)
            {
                textBoxRow.Height = _lastEditorHeight;
                dividerRow.Height = s_dividerHeight;
            }
        }

        private void HideEditor(object sender, RoutedEventArgs e)
        {
            // Hide the editor
            if (textBoxRow != null)
            {
                _lastEditorHeight = textBoxRow.Height;
                dividerRow.Height = new GridLength(0);
                textBoxRow.Height = new GridLength(0);
            }
        }

        private void ShowTheEditingTabs(object sender, RoutedEventArgs e)
        {
            if (_mainRoot is RichTextBox)
            {
                SetupRtbEventsHelper(_mainRoot as RichTextBox);
                return;
            }
            else
            {
                GetFirstRichTextBox(_mainRoot);
            }
        }

        private void GetFirstRichTextBox(UIElement root)
        {
            IEnumerable eleColl = LogicalTreeHelper.GetChildren(root);
            IEnumerator eleCollEnumerator = eleColl.GetEnumerator();
            while (eleCollEnumerator.MoveNext())
            {
                if (eleCollEnumerator.Current.GetType().ToString().Contains("RichTextBox"))
                {
                    RichTextBox rtb = eleCollEnumerator.Current as RichTextBox;
                    SetupRtbEventsHelper(rtb);
                    return ;
                }
                else
                {
                    GetFirstRichTextBox(eleCollEnumerator.Current as UIElement);
                }
            }
            return;
        }

        private void SetupRtbEventsHelper(RichTextBox rtb)
        {
            _rtbBackup = rtb;
            _rtbBackup.TextChanged += new TextChangedEventHandler(rtb_TextChanged);
            _rtbBackup.SelectionChanged += new RoutedEventHandler(rtb_SelectionChanged);
            TabControl.SelectionChanged += new SelectionChangedEventHandler(TabControl_SelectionChanged);

            DocumentTreeTab.Visibility = Visibility.Visible;
            SelectionXamlTab.Visibility = Visibility.Visible;
            DocSerializedXamlTab.Visibility = Visibility.Visible;
            EditingExtras.Visibility = Visibility.Visible;
        }

        private void HideEditingTabs(object sender, RoutedEventArgs e)
        {
            DocumentTreeTab.Visibility = Visibility.Collapsed;
            SelectionXamlTab.Visibility = Visibility.Collapsed;
            DocSerializedXamlTab.Visibility = Visibility.Collapsed;

            if (_rtbBackup!=null)
            {
                _rtbBackup.TextChanged -= new TextChangedEventHandler(rtb_TextChanged);
                _rtbBackup.SelectionChanged -= new RoutedEventHandler(rtb_SelectionChanged);
            }
            TabControl.SelectionChanged -= new SelectionChangedEventHandler(TabControl_SelectionChanged);
            _rtbBackup = null;
            EditingExtras.Visibility = Visibility.Collapsed;
            staticXamlTab.Focus();
            editableTextBox.Focus();
            scroll.ScrollToHorizontalOffset(0);
            return;
        }

        void DoOpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog o = new System.Windows.Forms.OpenFileDialog();
            o.Filter = "Word files (*.docx,*.doc)|*.doc;*.docx|Rtf files (*.rtf)|*.rtf|Web files (*.htm,*.html,*.mht)|*.htm;*.html;*.mht|Text files (*.txt)|*.txt|XamlPackage files (*.xamlzip)|*.xamlzip|Xaml files (*.xaml)|*.xaml";
            o.FileOk += new System.ComponentModel.CancelEventHandler(o_FileOk);
            o.ShowDialog();
        }

        void o_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog o = sender as System.Windows.Forms.OpenFileDialog;
            
            try
            {
                string filePath = o.FileName;
                char[] separator = new char[1];
                separator[0] = '\\';
                string[] nameArr = filePath.Split(separator);
                string fileName = nameArr[nameArr.Length - 1];
                string destinationFilePath = filePath.Replace(fileName, "_temporaryRtfFileToBeDeleted");
                if(fileName.Contains(".xamlzip"))
                {
                    FileLoadHelper(filePath, DataFormats.XamlPackage);
                    return;
                }
                if (fileName.Contains(".txt"))
                {
                    FileLoadHelper(filePath, DataFormats.Text);
                    return;
                }
                if (fileName.Contains(".xaml"))
                {
                    FileLoadHelper(filePath, DataFormats.Xaml);
                    return;
                }
                if (fileName.Contains(".rtf"))
                {
                    FileLoadHelper(filePath, DataFormats.Rtf);
                    return;
                }

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 2);
                timer.IsEnabled = true;
                timer.Start();
                timer.Tick += new EventHandler(timer_Tick);

                _dialogOnLoading = new NoSelectionDialog();
                _dialogOnLoading.RemoveButton();
                _dialogOnLoading.Width = 500;
                _dialogOnLoading.SetText("Conversion may take some time");
                _dialogOnLoading.Topmost = true;
                _dialogOnLoading.Show();

                if ((fileName.Contains(".htm")) || (fileName.Contains(".mht")))
                {
                    _htmlFile = true;
                    _webform = new Form1();
                    _webform.browser.ScriptErrorsSuppressed = true;
                    //webform.Show();
                    //webform.Hide();
                    _webform.Navigate(filePath);
                    _webform.browser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
                    return;
                }

                if (COfficeUtils.IsOfficeVersionInstalled(12) || COfficeUtils.IsOfficeVersionInstalled(11))
                {
                    if (File.Exists(destinationFilePath + ".rtf"))
                    {
                        File.Delete(destinationFilePath);
                    }
                    if (!COfficeUtils.OpenAndSaveWordFileViaAppPool(o.FileName, destinationFilePath, COfficeUtils.ETestableWordSaveFormats.RTF))
                    {
                    }
                    else
                    {
                        FileLoadHelper(destinationFilePath+".rtf", DataFormats.Rtf);
                        File.Delete(destinationFilePath + ".rtf");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
                else
                {
                    NoSelectionDialog dialog = new NoSelectionDialog();
                    dialog.SetText("No Office Version Detected");
                    dialog.ShowDialog();
                }
            }
            catch (Exception)
            {
                _dialogOnLoading = new NoSelectionDialog();
                _dialogOnLoading.Width = 500;
                _dialogOnLoading.SetText("Document could not be loaded");
                _dialogOnLoading.Topmost = true;
                _dialogOnLoading.ShowDialog();
            }
        }

        void browser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if ((_webform.browser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete) && (_webform.browser.IsBusy == false))
            {
                //webform.browser.Document.ActiveElement.Enabled = false;
                _webform.browser.Document.Body.Focus();
                _webform.browser.Document.ExecCommand("SelectAll", false, null);
                _webform.browser.Document.ExecCommand("copy", false, null);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _dialogOnLoading.Close();
            DispatcherTimer timer = sender as DispatcherTimer;
            timer.IsEnabled = false;
            if (_htmlFile)
            {
                if ((_webform.browser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete) && (_webform.browser.IsBusy == false))
                {
                    //webform.browser.Document.ActiveElement.Enabled = false;
                    _webform.browser.Document.Body.Focus();
                    _webform.browser.Document.ExecCommand("SelectAll", false, null);
                    _webform.browser.Document.ExecCommand("copy", false, null);
                }
                _rtbBackup.Document.Blocks.Clear();
                _rtbBackup.Paste();
                _htmlFile = false;
            }
        }

        private void FileLoadHelper(string destinationFilePath, string format)
        {
            FileStream file = File.OpenRead(destinationFilePath);
          //  StreamReader reader = new StreamReader(file);
            TextRange tr = new TextRange(_rtbBackup.Document.ContentStart, _rtbBackup.Document.ContentEnd);
            tr.Load(file, format);
            file.Close();
        }

        void DoSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Rtf files (*.rtf)|*.rtf|Text files (*.txt)|*.txt|XamlPackage files (*.xamlzip)|*.xamlzip|Xaml files (*.xaml)|*.xaml";
            saveDialog.FileOk += new System.ComponentModel.CancelEventHandler(saveDialog_FileOk);
            saveDialog.ShowDialog();
        }

        void saveDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog o = sender as System.Windows.Forms.SaveFileDialog;
            try
            {
                string filePath = o.FileName;
                char[] separator = new char[1];
                separator[0] = '\\';
                string[] nameArr = filePath.Split(separator);
                string fileName = nameArr[nameArr.Length - 1];
                string destinationFilePath = filePath.Replace(fileName, "_temporaryRtfFileToBeDeleted");
                fileName = fileName.ToLower();
                switch (o.FilterIndex)
                {
                    case 1:
                        filePath = (fileName.Contains(".rtf")) ? filePath : (filePath.Trim() + ".rtf");
                        FileSaveHelper(filePath, DataFormats.Rtf);
                        break;

                    case 2:
                        filePath = (fileName.Contains(".txt")) ? filePath : (filePath.Trim() + ".txt");
                        FileSaveHelper(filePath, DataFormats.Text);
                        break;

                    case 3:
                        filePath = (fileName.Contains(".xamlzip")) ? filePath : (filePath.Trim() + ".XamlZip");
                        FileSaveHelper(filePath, DataFormats.XamlPackage);
                        break;

                    case 4:
                        filePath = (fileName.Contains(".xaml")) ? filePath : (filePath.Trim() + ".Xaml");
                        FileSaveHelper(filePath, DataFormats.Xaml);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception)
            {
                _dialogOnLoading = new NoSelectionDialog();
                _dialogOnLoading.Width = 500;
                _dialogOnLoading.SetText("Rtb content could not be saved");
                _dialogOnLoading.Topmost = true;
                _dialogOnLoading.ShowDialog();
            }
        }

        void FileSaveHelper(string filePath, string format)
        {
            TextRange range;
            FileStream fStream;

            range = new TextRange(_rtbBackup.Document.ContentStart, _rtbBackup.Document.ContentEnd);

            fStream = new FileStream(filePath, FileMode.Create);
            range.Save(fStream, format);
            fStream.Close();
        }

        void DoSpellingReformCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Process p = new Process();
                if (File.Exists("SpellingReformTest.exe") == false)
                {
                    CheckDllsExistHelper();
                }
                p.StartInfo = new ProcessStartInfo("SpellingReformTest.exe");
                p.Start();
        }

        void DoCharacterFormattingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Process p = new Process();
                if (File.Exists("FontDialogSample.exe") == false)
                {
                    CheckDllsExistHelper();
                }
                p.StartInfo = new ProcessStartInfo("FontDialogSample.exe");
                p.Start();
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDisplayTabs();
        }

        void rtb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (DocumentTreeTab.IsSelected == false)
            {
                UpdateDisplayTabs();
            }
        }

        void rtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDisplayTabs();
        }

        /// <summary>
        /// This methods updates the information of the Editinging area.
        /// it is called when the following event fires.
        /// 1. RichTextBox.TextChanged event fires. (the MainEditor for editing)
        /// 3. The TabControl.SelectionChanged event fires on the Bottom TabControl.
        /// 4. TextRange.Changed event fires when the selection in the Main editor (RichTextBox) is changed.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="o1"></param>
        void UpdateDisplayTabs()
        {
            if (_rtbBackup == null) return;

            Exception exception = null;
            try
            {
                TabItem selectedItem = TabControl.SelectedItem as TabItem;

                switch (selectedItem.Name)
                {
                    case "staticXamlTab":
                        break;

                    case "ClipboardViewerTab":
                        //ClipBoardContent.Document = new FlowDocument();
                        break;

                    case "CoreXamlTab":
                        dynamicTextBox.Text = XamlHelper.IndentXaml(XamlWriter.Save(_mainRoot));
                        break;

                    case "DocumentTreeTab":
                        TreeViewhelper.SetupTreeView(TextTreeView, _rtbBackup.Document);
                        break;

                    case "SelectionXamlTab":
                        if (_rtbBackup != null)
                        {
                            TextRange tr = new TextRange(_rtbBackup.Selection.Start, _rtbBackup.Selection.End);
                            SelectionXaml.Text = XamlHelper.IndentXaml(XamlHelper.TextRange_GetXaml(tr));
                        }
                        break;

                    case "DocSerializedXamlTab":
                        if (_rtbBackup != null)
                        {
                            TextRange tr = new TextRange(_rtbBackup.Document.ContentStart, _rtbBackup.Document.ContentEnd);
                            DocSerializedXaml.Text = XamlHelper.IndentXaml(XamlHelper.TextRange_GetXaml(tr));
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

        void TreeViewItemSelectEvent(object sender, MouseButtonEventArgs e)
        {
            object o = TextTreeView.SelectedItem;
            if (o != null)
            {
                if (_interpreterResultsBox == null)
                {
                    ShowCommandInterpreter(null, null);
                }
                _interpreterResultsBox.Text+= "Use Object Name:["+ _commandParser.CreateObjectForTreeItem(((TreeViewItem)o).Tag)+"] for selected item\r\n";
                _interpreterResultsBox.Text += "---------------------------------------------\r\n";
                ShowInterpreterButton.IsChecked = true;
                _commandInterpreter.Show();
            }
        }

        void VisualTreeViewItemSelected()
        {
            if (_interpreterResultsBox == null)
            {
                ShowCommandInterpreter(null, null);
            }
             //   InterpreterResultsBox.Text += "Use Object Name:[" + commandParser.CreateObjectForTreeItem(((TreeViewItem)o).Tag) + "] for selected item\r\n";
             //   InterpreterResultsBox.Text += "---------------------------------------------\r\n";
            ShowInterpreterButton.IsChecked = true;
            _commandInterpreter.Show();
        }

        void OnError(Exception e)
        {
            if (e != null)
            {
                statusText.Text = e.Message + "\r\n" + e.StackTrace;
            }
        }

        private void ShowPalletteWindow(object sender, RoutedEventArgs e)
        {
            if (_pallete == null)
            {
                _pallete = new ColorPallette();
                _pallete.Closed += new EventHandler(pallete_Closed);
            }
            _pallete.Show();
        }

        void pallete_Closed(object sender, EventArgs e)
        {
            ShowPallette.IsChecked = false;
            _pallete = null;
        }

        private void HidePalletteWindow(object sender, RoutedEventArgs e)
        {
            if (_pallete != null)
            {
                _pallete.Close();
            }
        }

        private void ShowStyleWindow(object sender, RoutedEventArgs e)
        {
            if (File.Exists("ControlStyles.exe") == false)
            {
                CheckDllsExistHelper();
            }
            _p = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo("ControlStyles.exe");
            _p.StartInfo = startinfo;
            _p.Exited += new EventHandler(p_Exited);
            _p.EnableRaisingEvents = true;
            _p.Start();
        }

        private static void CheckDllsExistHelper()
        {
            try
            {
                if (File.Exists("unzip.exe") == false)
                {
                    File.WriteAllBytes("unzip.exe", Properties.Resources.unzip);
                }
                if (File.Exists("zipResource.zip") == false)
                {
                    File.WriteAllBytes("zipResource.zip", Properties.Resources.zipResource);
                }
            }
            catch (Exception)
            {
                NoSelectionDialog dialog = new NoSelectionDialog();
                dialog.SetText( "Extracting Resources failed");
                dialog.Width = 500;
                dialog.Height = 80;
                dialog.ShowDialog();
            }
            UnZipHelper();
        }

        private static void UnZipHelper()
        {

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("unzip.exe");
            startInfo.Arguments = "-o -q zipResource.zip";
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.UseShellExecute = true;
            p.StartInfo = startInfo;
            
            p.Start();
            p.WaitForExit();
        }

        private delegate void SimpleDelegate();
        void p_Exited(object sender, EventArgs e)
        {
            Dispatcher mainDispatcher = _mainDispatcher;
            mainDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new SimpleDelegate(UnCheckControlStyleButton));
        }

        private void UnCheckControlStyleButton()
        {
            _p = null;
            ShowControlStyles.IsChecked = false;
        }

        private void HideStyleWindow(object sender, RoutedEventArgs e)
        {
            if (_p != null)
            {
                _p.CloseMainWindow();
                _p = null;
            }
        }

        private void ShowCommandInterpreter(object sender, RoutedEventArgs e)
        {
            if (_commandInterpreter == null)
            {
                _commandInterpreter = new Window();
                //commandInterpreter.Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("Alert 17.ico", UriKind.RelativeOrAbsolute)).Stream);
                _commandInterpreter.Title = "Command Interpreter";
                _commandInterpreter.Height = _commandInterpreter.Width = 400;

                _interpreterGUI = new Interpreter();

                _interpreterResultsBox = LogicalTreeHelper.FindLogicalNode(_interpreterGUI, "InterpreterResults") as TextBox;
                _interpreterComboBox = LogicalTreeHelper.FindLogicalNode(_interpreterGUI, "CommandInputBox") as TextBox;
                _commandInterpreter.Content = _interpreterGUI;
                _commandInterpreter.Loaded += new RoutedEventHandler(commandInterpreter_Loaded);
                _commandInterpreter.Closed += new EventHandler(commandInterpreter_Closed);
            }
            ShowInterpreterButton.IsChecked = true;
            _commandInterpreter.Show();
            _interpreterComboBox.Focus();
            _interpreterResultsBox.Focus();
            _commandInterpreter.Focus();
        }

        void commandInterpreter_Loaded(object sender, RoutedEventArgs e)
        {
            _interpreterComboBox.Text = "Type command here";
            _interpreterComboBox.Foreground = Brushes.Blue;
            //InterpreterComboBox.PreviewKeyUp += new KeyEventHandler(InterpreterComboBox_PreviewKeyUp);
            _interpreterComboBox.PreviewKeyDown += new KeyEventHandler(InterpreterComboBox_PreviewKeyDown);
            _commandParser = new CommandParser(_mainRoot, _interpreterResultsBox);
            _interpreterGUI.InterpreterCommandParser = _commandParser;
        }

        void InterpreterComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _commandParser.MainRoot = _mainRoot;
            if (e.Key == Key.Return) 
            {
                _commandString = _interpreterComboBox.Text;
                _commandString = _commandString.Replace(";", "");
                _interpreterComboBox.Text = "";
                _interpreterResultsBox.Text += "[" + _commandString.Replace(" ", "") + "]\r\n";
                _commandParser.ParseCommand(_commandString);
            }
        }

        void commandInterpreter_Closed(object sender, EventArgs e)
        {
            Interpreter interpreterPage = _commandInterpreter.Content as Interpreter;
            if (interpreterPage.helpWindow != null)
            {
                interpreterPage.helpWindow.Close();
            }
            _commandInterpreter = null;
            _interpreterResultsBox = null;
            _interpreterComboBox = null;
            ShowInterpreterButton.IsChecked = false;
        }

        void HideCommandInterpreter(object sender, EventArgs e)
        {
            if (_commandInterpreter != null)
            {
                _commandInterpreter.Visibility = Visibility.Hidden;
                Interpreter interpreterPage = _commandInterpreter.Content as Interpreter;
                _interpreterGUI.myPopup.IsOpen = _interpreterGUI.popupOpen = false;
                if (interpreterPage.helpWindow != null)
                {
                    interpreterPage.helpWindow.Visibility = Visibility.Hidden;
                }
            }
        }

        void NewWindowForContent(object sender, RoutedEventArgs e)
        {
            Window w = new Window();
            w.Title = "Clipboard Viewer";
            StackPanel sp = new StackPanel();
            FlowDocumentPageViewer fd = new FlowDocumentPageViewer();
            fd.Background = Brushes.Wheat;
            fd.Document = new FlowDocument();
            TextRange rangeDest = new TextRange(((FlowDocument)fd.Document).ContentEnd, ((FlowDocument)fd.Document).ContentStart);

            if (ClipBoardContent.Document != null)
            {
                TextRange rangeSource = new TextRange(((FlowDocument)ClipBoardContent.Document).ContentEnd, ((FlowDocument)ClipBoardContent.Document).ContentStart);
                rangeDest.Text = rangeSource.Text;
            }
            else
            {
                rangeDest.Text = "";
            }
            w.Content = fd;
            w.Show();

        }

        void ClearClipboard(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
        }
        
        void GetClipboardFormats(object sender, MouseButtonEventArgs e)
        {
            MenuItem root = sender as MenuItem;
            if (root.Items != null)
            {
                root.Items.Clear();
            }
            if (Clipboard.GetDataObject() != null)
            {
                IDataObject data = Clipboard.GetDataObject();
                string[] formats = data.GetFormats();
                if (formats.Length > 0)
                {
                    foreach (string format in formats)
                    {
                        MenuItem m = new MenuItem();
                        m.Header = format;
                        m.Click += new RoutedEventHandler(m_Click);
                        m.Tag = root.Name;
                        root.Items.Add(m);
                    }
                }
                else
                {
                    MenuItem m = new MenuItem();
                    m.Header = "Clipboard Empty";
                    m.Click += new RoutedEventHandler(m1_Click);
                    root.Items.Add(m);
                }
            }
        }

        void m1_Click(object sender, RoutedEventArgs e)
        {
            if (((FlowDocument)(ClipBoardContent.Document))!= null)
            {
                ((FlowDocument)(ClipBoardContent.Document)).Blocks.Clear();
            }
        }

        void m_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clickedMenuItem = sender as MenuItem;
            string dataFormat = clickedMenuItem.Header.ToString();

            if (clickedMenuItem.Tag.ToString().Contains(AvailableFormats.Name) == true)
            {
                if (ClipBoardContent.Document == null)
                {
                    ClipBoardContent.Document = new FlowDocument();
                }
                else
                {
                    ((FlowDocument)(ClipBoardContent.Document)).Blocks.Clear();
                }

                if (Clipboard.ContainsData(dataFormat))
                {
                    object o = Clipboard.GetData(dataFormat);
                    TextRange range = new TextRange(((FlowDocument)ClipBoardContent.Document).ContentEnd, ((FlowDocument)ClipBoardContent.Document).ContentStart);
                    range.Text = o.ToString();
                }
            }
            else
            {
                if (Clipboard.ContainsData(dataFormat))
                {
                    object o = Clipboard.GetData(dataFormat);
                    Clipboard.Clear();
                    Clipboard.SetData(dataFormat, o);
                }
            }
            e.Handled = true;
        }
        

        private void ShowVisualTree(object sender, RoutedEventArgs e)
        {
#if !XamlPadExpressApp
            if (visualColumn != null)
            {
                if (visualTreeViewContainer.Content == null)
                {
                    // Create visual tree view
                    VisualTreeView visualTreeView = new VisualTreeView();
                    visualTreeView.Margin = new Thickness(2, 2, 2, 2);
                    Binding visualBinding = new Binding("Content");
                    visualBinding.ElementName = "contentRenderArea";
                    BindingOperations.SetBinding(visualTreeView, VisualTreeView.TargetProperty, visualBinding);
                    visualTreeViewContainer.Content = visualTreeView;

                    // Create property tree view
                    PropertyTreeView propertyTreeView = new PropertyTreeView();
                    propertyTreeView.Margin = new Thickness(2, 2, 2, 2);
                    Binding propertyBinding = new Binding("Current");
                    propertyBinding.Source = visualTreeView;
                    BindingOperations.SetBinding(propertyTreeView, PropertyTreeView.TargetProperty, propertyBinding);
                    propertyTreeViewContainer.Content = propertyTreeView;
                }

                // Show the visual tree
                visualColumn.Width = _lastVisualWidth;
                dividerColumn.Width = s_dividerWidth;
            }
#endif
        }

        private void HideVisualTree(object sender, RoutedEventArgs e)
        {
#if !XamlPadExpressApp
            if (visualColumn != null)
            {
                // Hide the visual tree
                _lastVisualWidth = visualColumn.Width;
                dividerColumn.Width = new GridLength(0);
                visualColumn.Width = new GridLength(0);
            }
#endif
        }

        private void JumpToErrorLinkClicked(object sender, RoutedEventArgs e)
        {
            // Set the caret index of the TextBox to the line/column of the error
            if (_errorLineNumber > 0)
            {
                editableTextBox.CaretIndex = editableTextBox.GetCharacterIndexFromLineIndex(_errorLineNumber - 1) + _errorPosition;
            }
            else
            {
                editableTextBox.CaretIndex = 0;
            }

            // Scroll to the line indicated by the error information
            editableTextBox.ScrollToLine((_errorLineNumber > 0) ? (_errorLineNumber - 1) : 0);
            editableTextBox.Focus();
            editableTextBox.SelectedText = " ";
            editableTextBox.Undo();
        }

        private void PageClosed(object sender, RoutedEventArgs e)
        {
            Application.Current.DispatcherUnhandledException -= new DispatcherUnhandledExceptionEventHandler(UnhandledExceptionHandler);
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            
            InitializeMainWindow();
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(UnhandledExceptionHandler);
            // Load saved content and display in text box
            editableTextBox.Text = XamlHelper.LoadSavedXamlContent();
            _isPageLoaded = true;
            editableTextBox.Focus();
            editableTextBox.GotMouseCapture += new MouseEventHandler(editableTextBox_GotMouseCapture);
            _mouseCaptureEventHandlerCount++;
            editableTextBox.PreviewTextInput += new TextCompositionEventHandler(editableTextBox_PreviewTextInput);
            scroll.ScrollToHorizontalOffset(0);
            
            this.Unloaded += new RoutedEventHandler(XamlPadPage_Unloaded);
            this.PreviewKeyUp += new KeyEventHandler(XamlPadPage_PreviewKeyUp);
#if !XamlPadExpressApp
            XamlPadMain.CloseSplashScreen();
#endif
            CheckDllsExistHelper();
        }

        void editableTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ">")
            {
                string firstPart = editableTextBox.Text.Substring(0, editableTextBox.CaretIndex);
                string[] tagArr = firstPart.Split(new char[] { '<' });
                if (tagArr.Length > 0)
                {
                    string tag = tagArr[tagArr.Length - 1].Trim();
                    string temp = tag.Replace("'", "\"");
                    int count = 0;
                    int index=0;
                    while (index!=-1)
                    {
                        try{
                            index = temp.IndexOf("\"",index);
                            if (index == -1) break;
                            index++;
                            count++;
                        }
                        catch(Exception){
                            break;
                        }
                    }

                    if ((tag.Length>0)&&(tag[tag.Length - 1] != '/')&&(tag.Contains(">") == false)&&(count%2 ==0))
                    {
                        string endTag = "></" + (tag.Contains(" ") ? tag.Substring(0, tag.IndexOf(' ')) : tag) + ">";
                        string secPart = editableTextBox.Text.Substring(editableTextBox.CaretIndex);
                        int tempCaretIndex = editableTextBox.CaretIndex;
                        editableTextBox.Text = firstPart + endTag + secPart;
                        editableTextBox.Select(tempCaretIndex + 1, endTag.Length - 1);

                        e.Handled = true;
                    }
                }
            }
        }

        void editableTextBox_LayoutUpdated(object sender, EventArgs e)
        {
            if (_lineCount != editableTextBox.LineCount)
            {
                string str = "";
                for (int i = 1; i <= editableTextBox.LineCount; i++)
                {
                    str += i.ToString() + "\r\n";
                }
                LineNumberBox.Text = str;
                _lineCount = editableTextBox.LineCount;
            }
        }


        void editableTextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        void editableTextBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            editableTextBox.GotMouseCapture += new MouseEventHandler(editableTextBox_GotMouseCapture);
            _mouseCaptureEventHandlerCount++;
        }

        void editableTextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
                Point p = e.GetPosition(editableTextBox);
                if (((int)p.X < (int)(scroll.ViewportWidth - LineNumberBox.ActualWidth))
                    && ((int)scroll.HorizontalOffset <= (int)LineNumberBox.ActualWidth))
                {
                    int selectTextCount = editableTextBox.SelectedText.Length;
                    scroll.ScrollToHorizontalOffset(0);
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (SimpleDelegate)delegate { editableTextBox.Select(editableTextBox.SelectionStart, selectTextCount); });
                }
            while (_mouseCaptureEventHandlerCount > 0)
            {
                editableTextBox.GotMouseCapture -= new MouseEventHandler(editableTextBox_GotMouseCapture);
                _mouseCaptureEventHandlerCount--;
            }
        }

        void XamlPadPage_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_findWindow != null)
                {
                    _findWindow.Close();
                }
            }
        }

        void XamlPadPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_findWindow != null)
            {
                _findWindow.Close();
            }
        }

        void DoOpenFindCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_findWindow == null)
            {
                _findWindow = new FindAndReplace(editableTextBox);
                _findWindow.Closed += new EventHandler(findWindow_Closed);
            }
            _findWindow.ShowWindow();
        }

        void DoFindNextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_findWindow == null)
            {
                return;
            }
            else
            {
                _findWindow.ShowWindow();
                _findWindow.ok_Click(sender, null);
            }
        }

        void findWindow_Closed(object sender, EventArgs e)
        {
            _findWindow = null;
        }

        void DoOpenGoToCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (_gotoLineDialog == null)
            {
                _gotoLineDialog = new GoToLineDialog(editableTextBox);
                _gotoLineDialog.Closed += new EventHandler(gotoLineDialog_Closed);
            }
            _gotoLineDialog.ShowWindow();
        }

        void gotoLineDialog_Closed(object sender, EventArgs e)
        {
            _gotoLineDialog = null;
        }

        private void PrintButtonClicked(object sender, RoutedEventArgs e)
        {
            PrintCommandExecute((Window)this.Parent, null);
        }

        private void RefreshButtonClicked(object sender, RoutedEventArgs e)
        {
            RefreshCommandExecute((Window)this.Parent, null);
        }

        private void TextBoxInitialized(object sender, EventArgs e)
        {
            // Set the initial selected items for the drop-down lists 
            // that affect the TextBox.  We wait to do this until the TextBox
            // is initialized, because setting these values will trigger
            // events that will act on the TextBox.
            int i = fontNameCombo.Items.IndexOf("Courier New");
            fontNameCombo.SelectedIndex = (i > -1) ? i : 0;
            fontSizeCombo.SelectedIndex = 3;

            // Save the current height of the text box area so we can 
            // restore it if hidden.
            _lastEditorHeight = textBoxRow.Height;
           
#if !XamlPadExpressApp
            // Set up snippets
            SetupSnippets(editableTextBox);
            editableTextBox.ContextMenuOpening += new ContextMenuEventHandler(editableTextBox_ContextMenuOpening);
#endif
        }

        void editableTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            SetupSnippetsTask(editableTextBox);
        }

        void CoreXamlClick(object sender, RoutedEventArgs e)
        {
            dynamicTextBox.Text = XamlHelper.IndentXaml( XamlWriter.Save(contentRenderArea.Content));
        }

        void ParseCoreXamlClick(object sender, RoutedEventArgs e)
        {
            if (dynamicTextBox.Text != "")
            {
                editableTextBox.Text = dynamicTextBox.Text;
                staticXamlTab.Focus();
            }
        }

        void ParseDocXaml(object sender, RoutedEventArgs e)
        {
            if ((DocSerializedXaml.Text != "") && (_rtbBackup != null))
            {
                TextRange tr = new TextRange(_rtbBackup.Document.ContentStart, _rtbBackup.Document.ContentEnd);
                try
                {
                    XamlHelper.TextRange_SetXml(tr, XamlHelper.RemoveIndentation(DocSerializedXaml.Text));
                    statusText.Text= "Done";
                }
                catch (Exception e1)
                {
                    statusText.Text = e1.Message;
                }
            }
        }

        private void TextBoxTextChanged(object sedner, TextChangedEventArgs e)
        {
            if (autoParseButton.IsChecked == true)
            {
                ClearDispatcherTimer();
                long msecTimeout = 5 * 1000000; // ~1 sec ;)
                s_dispatcherTimer = new DispatcherTimer(new TimeSpan(msecTimeout), DispatcherPriority.ApplicationIdle, new EventHandler(AttemptUpdate), Dispatcher.CurrentDispatcher);
            }
        }

        private void UnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Exception e = args.Exception;

            // Color the text in the TextBox red to indicate an error with the XAML
            editableTextBox.Foreground = Brushes.Red;

            // Set the error information in the status bar
            SetStatusText(e.Message);
            _errorLineNumber = 0;
            _errorPosition = 0;
            ((UIElement)goToErrorHyperlink.Parent).Visibility = Visibility.Visible;
            goToErrorHyperlink.Inlines.Clear();
            
            goToErrorHyperlink.Inlines.Add(new Run("Jump To: line " + _errorLineNumber + " col " + _errorPosition));

            // If we're hitting this exception handler as a result of resetting the content
            // to attempt to recover from a previous exception, just clear out the Frame.
            // We have no known valid state at this point, since the XAML on disk was also bad,
            // and this will at least prevent from entering an infinite loop.
            if (!_contentRenderedSuccessfully)
            {
                contentRenderArea.Content = null;

                // Hack around a bug with Frame; it doesn't actually let the Content property
                // go to null until it has asynchronously processed the navigation.
                while( contentRenderArea.Content != null )
                {
                    DoEvents();
                    System.Threading.Thread.Sleep( 100 );
                }
            }
            else
            {
                // Load the saved XAML from disk and render that instead, but leave the TextBox unchanged
                string savedXaml = XamlHelper.LoadSavedXamlContent();

                // Parse the saved XAML and render it in the Frame
                _saveContentOnSuccessfulRender = false;
                ParseResult result = XamlHelper.ParseXaml(savedXaml);
                if (result == null)
                {
                    // This shouldn't ever happen because LoadSavedXamlContent() should've already
                    // parsed the XAML once and validated it, but just in case...
                    // In this case, just empty out the Frame because we've exhausted our options
                    // for finding valid content.
                    contentRenderArea.Content = null;
                }
                else
                {
                    _contentRenderedSuccessfully = false;
                    RenderTree(result.Root);
                }
            }

#if !XamlPadExpressApp

            // Note: This workaround is necessary in Beta 1 to allow the app to recover after 
            //       an exception is thrown during the rendering process.
            //       Posting two WM_SIZE notifications to the main window will effectively
            //       force it to invalidate and render again.
            // 
            // 0x0005 = WM_SIZE, 0 = SIZE_RESTORED
            IntPtr mainWindowHandle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            uint windowSize = MAKELPARAM((int)((Window)this.Parent).Width, (int)((Window)this.Parent).Height);
            PostMessage(mainWindowHandle, 0x0005, 0, windowSize);
            PostMessage(mainWindowHandle, 0x0005, 0, windowSize);
#endif

            args.Handled = true;
        }

        static public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, 
                        (DispatcherOperationCallback) delegate (object o)
                        {
                                    DispatcherFrame f = (DispatcherFrame)o;
                                    f.Continue = false;
                                    return null;
                        }, frame);

            Dispatcher.PushFrame(frame);
        }

        private void ZoomLostFocus(object sender, RoutedEventArgs e)
        {
            if (zoomCombo.Text != _lastZoomFactor)
            {
                UpdateZoomFactor();
            }
        }

        private void ZoomSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (zoomCombo.SelectedItem != null)
            {
                zoomCombo.Text = ((ComboBoxItem)zoomCombo.SelectedItem).Content.ToString();
                UpdateZoomFactor();
            }
        }

        #endregion

        #region publicProperties.

        public Window CommandInterpreter
        {
            set
            {
                _commandInterpreter = value;
            }
            get
            {
                return _commandInterpreter;
            }
        }

        public CommandParser Parser
        {
            get
            {
                return _commandParser;
            }
            set
            {
                _commandParser = value;
            }
        }

        public UIElement RootElement
        {
            get
            {
                return _mainRoot;
            }
            set
            {
                _mainRoot = value;
            }
        }

        #endregion publicProperties.

    }
}