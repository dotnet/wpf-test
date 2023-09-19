// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace Microsoft.Test.Graphics
{
    public partial class XamlPadPage : Window
    {
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
        private string _lastFontName;
        private string _lastFontSize;
        private string _lastZoomFactor;
        private GridLength _lastEditorHeight;
        private static GridLength s_dividerHeight = new GridLength(5);

        private static string s_cantDisplayWindowMessage = "<TextBlock xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" ><Italic>Window content does not run in the XamlPad pane; click Refresh or press F5 to run in a separate window</Italic></TextBlock>";

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
            ClearDispatcherTimer();

            ParseResult r = XamlHelper.ParseXaml(editableTextBox.Text);

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
                    if (r.Root == null)
                    {
                        XamlTestHelper.LogFail("XamlHelper unexpectedly parsed out a null root.");
                    }
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
            Window mainWindow = (Window)Application.Current.MainWindow;

#if !XamlPadExpressApp
            mainWindow.Title = "XamlPad";
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

        private void NormalRender(object root)
        {
            // Render the content in the Frame
            _saveContentOnSuccessfulRender = true;
            RenderTree(root);

            // Update the status bar
            SetStatusText("Done. Markup saved to " + XamlHelper.SavedXamlLocation + ".");

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
                contentRenderArea.Content = root;
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

        private void ConvertXTCCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            object o = CreateWCPObjFromXTC();
            if (o != null)
            {
                xamlTB.Text = SerializeWCPObject(o);
                SetStatusText("Serialization is fully successful!");
                TC1.SelectedIndex = 2;
            }
        }

        private void SaveContentAsBitmap(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                RenderTargetBitmap render = new RenderTargetBitmap((int)contentRenderArea.ActualWidth, (int)contentRenderArea.ActualHeight, 96, 96, PixelFormats.Default);
                render.Render(contentRenderArea);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(render));

                FileStream fs = new System.IO.FileStream("XamlImage.bmp", System.IO.FileMode.Create);
                encoder.Save(fs);
                fs.Flush();
                fs.Close();
            }
            catch (Exception ei)
            {
                SetStatusText(ei.Message, Brushes.Red);
            }

            SetStatusText("Successuflly saved content into " + System.IO.Directory.GetCurrentDirectory() + "\\XamlImage.bmp");
        }

        private void SetStatusText(string value)
        {
            SetStatusText(value, Brushes.Black);
        }

        private void SetStatusText(string value, Brush b)
        {
            string s = "";
            if (!String.IsNullOrEmpty(value))
            {
                s = value.Replace('\r', ' ').Replace('\n', ' ');
            }

            statusText.Text = s;
            statusText.Foreground = b;
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
            SetupCustomCommand(window, "ConvertXTC", new KeyGesture(Key.F2), ConvertXTCCommandExecute);
            SetupCustomCommand(window, "SaveContentAsBitmap", new KeyGesture(Key.F3), SaveContentAsBitmap);

#if !XamlPadExpressApp
            SetupCustomCommand(window, "Help", new KeyGesture(Key.F1), HelpCommandExecute);
            SetupCustomCommand(window, "ToggleRtf", new KeyGesture(Key.R, ModifierKeys.Control | ModifierKeys.Shift),
                ToggleRtfCommandExecute);
#endif
        }

#if !XamlPadExpressApp
        private void SetupSnippets(TextBox textbox)
        {
            ContextMenu menu;
            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }

            _snippetManager = new SnippetManager();
            if (!_snippetManager.LoadSnippets())
            {
                _snippetManager.SetDefaultSnippets();
            }

            menu = new ContextMenu();
            menu.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
            {
                object item = menu.ItemContainerGenerator.ItemFromContainer(
                    (DependencyObject)e.OriginalSource);
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
            }), true);
            menu.ItemsSource = _snippetManager.ContextMenuItems;
            textbox.ContextMenu = menu;
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
        private void CBGotFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb != null)
            {
                cb.Text = string.Empty;
            }
        }

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
            FocusManager.SetFocusedElement((Window)this.Parent, (UIElement)sender);
        }

        private void EnableCameraButtonClicked(object sender, RoutedEventArgs e)
        {
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
            if (_saveContentOnSuccessfulRender)
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

        private void HideEditorButtonClicked(object sender, RoutedEventArgs e)
        {
            if (hideEditorButtonText.Text == "Hide Editor")
            {
                // Hide the editor
                _lastEditorHeight = textBoxRow.Height;
                dividerRow.Height = new GridLength(0);
                textBoxRow.Height = new GridLength(0);
                hideEditorButtonText.Text = "Show Editor";
            }
            else
            {
                // Show the editor
                textBoxRow.Height = _lastEditorHeight;
                dividerRow.Height = s_dividerHeight;
                hideEditorButtonText.Text = "Hide Editor";
            }
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
#endif
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

                while (contentRenderArea.Content != null)
                {
                    DoEvents();
                    System.Threading.Thread.Sleep(100);
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
            uint windowSize = MAKELPARAM((int)this.Width, (int)this.Height);
            PostMessage(mainWindowHandle, 0x0005, 0, windowSize);
            PostMessage(mainWindowHandle, 0x0005, 0, windowSize);
#endif

            args.Handled = true;
        }


        static public void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle,
                        (DispatcherOperationCallback)delegate(object o)
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

    }
}
