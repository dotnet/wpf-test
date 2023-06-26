// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: MainWindow.Xaml.cs implement the immediate window with the following
 * features:
 *      1. define the main window class.
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
    public partial class MainWindow : Window
    {
        System.Windows.Forms.Form _form;
        System.Windows.Forms.RichTextBox _winformsRTB;
        const string _fileName = "EditingExaminer_Saved.zip";
        DebugWindow _ImmediateWindow = null;

        /// <summary>
        /// Font comand
        /// </summary>
        public static RoutedCommand FontCommand = new RoutedCommand("FontDialog", typeof(MainWindow), null);
        
        /// <summary>
        /// Command to launch a Winform window.
        /// </summary>
        public static RoutedCommand LaunchWin32RTBCommand = new RoutedCommand("Win32RTB", typeof(MainWindow), null);
        
        /// <summary>
        /// Enable/Disable spellcheck command
        /// </summary>
        public static RoutedCommand SpellCheckCommand = new RoutedCommand("SpellCheck", typeof(MainWindow), null);
        
        /// <summary>
        /// Command to Toggle between the RichTextMode or panel mode.
        /// </summary>
        public static RoutedCommand RichTextModeCommand = new RoutedCommand("RichTextMode", typeof(MainWindow), null);
        
        /// <summary>
        /// Command to launch help document.
        /// </summary>
        public static RoutedCommand HelpTopicsCommand = new RoutedCommand("HelpTopics", typeof(MainWindow), null);
        
        /// <summary>
        /// command to enable/disable the analyzing window.
        /// </summary>
        public static RoutedCommand DebugWindowCommand = new RoutedCommand("DebugWindow", typeof(MainWindow), null);
        
        /// <summary>
        /// Command to close the application
        /// </summary>
        public static RoutedCommand ExitCommand = new RoutedCommand("exit", typeof(MainWindow), null);
        
        /// <summary>
        /// command to launch the immediate window.
        /// </summary>
        public static RoutedCommand ImmediateCommand = new RoutedCommand("Immediate", typeof(MainWindow), null);

        /// <summary>
        /// Margines struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        /// <summary>
        /// import the DWM stuff.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="pMarInset"></param>
        /// <returns></returns>
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
        /// defualt constructor
        /// </summary>
        public MainWindow()
        {
        }
        
        private void DoHelpTopicsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationWindow w = new NavigationWindow();
            w.Source = new Uri("File://\\\\avperfhog\\scratch\\Microsoft\\EditingExaminer\\editingExaminer.htm");
            w.Show();
        }

        void EnableDisableMenuItems(bool enable)
        {
            //The following menuItems need to be disable or enabled
            NewMenuItem.IsEnabled = enable;
            OpenMenuItem.IsEnabled = enable;
            SaveAsMenuItem.IsEnabled = enable;
            SaveMenuItem.IsEnabled = enable;
            FontMenuItem.IsEnabled = enable;
            SelectAllMenuItem.IsEnabled = enable;
            CopyMenuItem.IsEnabled = enable;
            CutMenuItem.IsEnabled = enable;
            PasteMenuItem.IsEnabled = enable;
            UndoMenuItem.IsEnabled = enable;
            RedoMenuItem.IsEnabled = enable;
            DeleteMenuItem.IsEnabled = enable;
        }

        private void DoExitCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void DoNewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MainEditor.Document.Blocks.Clear();
        }

        private void DoSpellCheckCommand(object sender, ExecutedRoutedEventArgs e)
        {
            MainEditor.SpellCheck.IsEnabled = SpellCheckMenuItem.IsChecked;
        }

        void DoFontCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Panel1.Children.Count > 0 && Panel1.Children[0] is RichTextBox)
            {
                FontChooser fontChooser = new FontChooser(MainEditor);
                fontChooser.Owner = this;
                fontChooser.ShowDialog();
            }
        }
        
        private void DoRichTextModeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            this.Focus();
            Panel1.Children.Clear();
            Panel1.Background = Brushes.White;
            if (RichEditMenuItem.IsChecked)
            {
                //RichTextBox is shown.
                Panel1.Children.Add(MainEditor);
                EnableDisableMenuItems(true);
                MainEditor.Focus();
            }
            else
            {
                EnableDisableMenuItems(false);
                //If the focus is in the RichTextBox that is being removed removed, the menu items are going to be disabled.
                //to walkaround this, set the focus to TableCoreXaml
                window_SizeChanged(null, null);
            }
            if (_ImmediateWindow != null && !_ImmediateWindow.IsClosed)
            {
                _ImmediateWindow.TargetChanged();
            }
        }

        private void DoLaunchWin32RTBCommand(object sender, ExecutedRoutedEventArgs e)
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

        private void DoImmediateCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (ImmediateMenuItem.IsChecked)
            {
                if (_ImmediateWindow == null || _ImmediateWindow.IsClosed)
                {
                    _ImmediateWindow = new DebugWindow(Panel1, this);
                    _ImmediateWindow.Closed += new EventHandler(_ImmediateWindow_Closed);
                    _ImmediateWindow.Show();
                }
            }
            else if(_ImmediateWindow !=null)
            {
                _ImmediateWindow.Close();
            }
        }

        void _ImmediateWindow_Closed(object sender, EventArgs e)
        {
            _ImmediateWindow.Closed -= new EventHandler(_ImmediateWindow_Closed);
            ImmediateMenuItem.IsChecked = false;
            _ImmediateWindow = null; 
        }

        private void DoFileOpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ShowError(Documentconvertion.LoadFileIntoRichTextBox(MainEditor, string.Empty));
        }
        
        private void DoFileSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ShowError(Documentconvertion.SaveRichTextBoxContentToFile(MainEditor, string.Empty));
        }

        void WindowLoaded(object o, RoutedEventArgs args)
        {
            PresentationSource p = PresentationSource.FromVisual(this);
            HwndSource s_hwndSource = p as HwndSource;

            if (s_hwndSource != null)
            {
                s_hwndSource.AddHook(new HwndSourceHook(ApplicationMessageFilter));

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
                catch (Exception e)
                {
                    ShowError(e);
                }
            }

            DoImmediateCommand(null, null);
            Documentconvertion.LoadFileIntoRichTextBox(MainEditor, _fileName);
            window_SizeChanged(null, null);
        }

        void ShowError(Exception e)
        {
            if (_ImmediateWindow != null)
            {
                _ImmediateWindow.OnError(e);
            }
        }
        
        void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                TopPanel.Width = this.ActualWidth -10;
                TopPanel.Height = this.ActualHeight;
                Panel1.Width = TopPanel.Width;
                Panel1.Height = TopPanel.Height - MainMenu.ActualHeight - 54;
                MainEditor.Width = Panel1.Width;
                MainEditor.Height = Panel1.Height;
            }
            catch (Exception exception)
            {
                ShowError(exception);
            }
        }

        void WindowClosed(object o, EventArgs args)
        {
            Documentconvertion.SaveRichTextBoxContentToFile(MainEditor, _fileName);
            
            //When the main window is closed, we should close the analyzing window if it is not closed.
            if (_ImmediateWindow != null && !_ImmediateWindow.IsClosed)
            {
                _ImmediateWindow.Close();
            }
            _ImmediateWindow = null; 
        }

      
        void _form_ResizeEnd(object sender, EventArgs e)
        {
            _winformsRTB.Height = _form.Height;
            _winformsRTB.Width = _form.Width;
        }
    }
}