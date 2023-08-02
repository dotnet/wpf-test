using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UIAEvents : Page
    {
        public UIAEvents()
        {
            // increase the top-level window's size to be large enough for this page's content
            Window window = Application.Current.MainWindow;
            window.Width = 800;
            window.Height= 550;

            InitializeComponent();

            // AsyncContentLoaded
            _asyncContentCombo.ItemsSource = Enum.GetValues<AsyncContentLoadedState>();

            // Notification
            _notificationKindCombo.ItemsSource = Enum.GetValues<AutomationNotificationKind>();
            _notificationProcessingCombo.ItemsSource = Enum.GetValues<AutomationNotificationProcessing>();

            // ActiveTextPositionChanged
            List<DependencyObject> targets = new List<DependencyObject> { _textBox, _richTextBox, _fdsv };
            _atpcTargetCombo.ItemsSource = targets;
        }

        #region AsyncContentLoaded

        private void _asyncContentButton_Click(object sender, RoutedEventArgs e)
        {
            object o = _asyncContentCombo.SelectedItem;
            Button button = sender as Button;
            AutomationPeer peer = UIElementAutomationPeer.FromElement(button);
            if (o != null && peer != null)
            {
                AsyncContentLoadedState state = (AsyncContentLoadedState)o;
                double percent = 0;
                switch (state)
                {
                    case AsyncContentLoadedState.Beginning: percent = 0; break;
                    case AsyncContentLoadedState.Progress: percent = 50; break;
                    case AsyncContentLoadedState.Completed: percent = 100; break;
                }

                peer.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(state, percent));
            }
        }

        private void _asyncContentComboCycle_Click(object sender, RoutedEventArgs e)
        {
            CycleComboBox(_asyncContentCombo);
        }

        #endregion AsyncContentLoaded

        #region Notification

        private void _notificationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            AutomationPeer peer = UIElementAutomationPeer.FromElement(button);
            if (peer != null)
            {
                peer.RaiseNotificationEvent((AutomationNotificationKind)_notificationKindCombo.SelectedItem,
                                            (AutomationNotificationProcessing)_notificationProcessingCombo.SelectedItem,
                                            _notificationString.Text,
                                            _notificationActivity.Text);
            }
        }
        
        private void _notificationKindComboCycle_Click(object sender, RoutedEventArgs e)
        {
            CycleComboBox(_notificationKindCombo);
        }
        
        private void _notificationProcessingComboCycle_Click(object sender, RoutedEventArgs e)
        {
            CycleComboBox(_notificationProcessingCombo);
        }

        #endregion Notification

        #region ActiveTextPositionChanged

        private void _atpcButton_Click(object sender, RoutedEventArgs e)
        {
            TextPointer start = null, end = null;
            TextAutomationPeer textPeer = null;
            ContentTextAutomationPeer contentTextPeer = null;
            string search = "Nevertheless";
            DependencyObject target = _atpcTargetCombo.SelectedItem as DependencyObject;

            switch (target.GetValue(FrameworkElement.NameProperty) as string)
            {
                case "_textBox":
                    FindWord(ContentStartFromTextBox(_textBox), search, out start, out end);
                    textPeer = UIElementAutomationPeer.FromElement(_textBox) as TextAutomationPeer;
                    break;
                case "_richTextBox":
                    FindWord(_richTextBox.Document.ContentStart, search, out start, out end);
                    textPeer = UIElementAutomationPeer.FromElement(_richTextBox) as TextAutomationPeer;
                    break;
                case "_fdsv":
                    FindWord(_fdsv.Document.ContentStart, search, out start, out end);
                    contentTextPeer = ContentElementAutomationPeer.FromElement(_fdsv.Document) as ContentTextAutomationPeer;
                    break;
                default:
                    return;
            }

            if (_startEdge.IsChecked == true) start = null;
            if (_endEdge.IsChecked == true) end = null;

            textPeer?.RaiseActiveTextPositionChangedEvent(start, end);
            contentTextPeer?.RaiseActiveTextPositionChangedEvent(start, end);
        }
        
        private void _atpcTargetComboCycle_Click(object sender, RoutedEventArgs e)
        {
            CycleComboBox(_atpcTargetCombo);
        }

        void FindWord(TextPointer position, string word, out TextPointer start, out TextPointer end)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(word);
                    if (indexInRun >= 0)
                    {
                        position = position.GetPositionAtOffset(indexInRun);
                        break;
                    }
                }
                else
                {
                    position = position.GetNextContextPosition(LogicalDirection.Forward);
                }
            }

            if (position == null)
            {
                start = end = null;
            }
            else
            {
                start = position;
                end = start.GetPositionAtOffset(word.Length);
            }
        }

        TextPointer ContentStartFromTextBox(TextBox textbox)
        {
            // There seems to be no public way to get a TextPointer from
            // a TextBox, so ... reflection
            PropertyInfo piTextContainer = typeof(TextBox).GetProperty("TextContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            object textContainer = piTextContainer?.GetValue(textbox);
            if (textContainer == null)
                return null;

            PropertyInfo piStart = textContainer.GetType().GetProperty("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            return piStart?.GetValue(textContainer) as TextPointer;
        }

        #endregion ActiveTextPositionChanged

        #region Helpers

        void CycleComboBox(ComboBox combobox)
        {
            int index = combobox.SelectedIndex + 1;
            if (index == combobox.Items.Count)
                index = -1;
            combobox.SelectedIndex = index;
        }

        #endregion Helpers
    }
}
