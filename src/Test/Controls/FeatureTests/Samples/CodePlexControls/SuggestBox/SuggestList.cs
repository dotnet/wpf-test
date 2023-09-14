using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ComponentModel;

namespace WpfControlToolkit
{
    public class SuggestList : ListBox
    {
        static SuggestList()
        {
            ScrollViewer.HorizontalScrollBarVisibilityProperty.OverrideMetadata(typeof(SuggestList), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto));
        }

        internal SuggestList(UIElement element)
        {
            // Hookup to element
            _element = element;
            _originalText = FocusedTextBox.Text;

            _popup = new Popup();
            _popup.PlacementTarget = element;
            _popup.Placement = PlacementMode.Bottom;
            _popup.Child = this;
            IsOpen = true;

            // Add a property?
            MaxHeight = SystemParameters.PrimaryScreenHeight / 3;

            UpdateList();
        }


        internal bool IsOpen
        {
            get { return _popup.IsOpen; }
            set
            {
                _popup.IsOpen = value;
                if (value)
                {
                    _element.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewElementKeyDown), true);
                    //_element.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnTextChanged), true);
                }
                else
                {
                    _element.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPreviewElementKeyDown));
                    //_element.RemoveHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(OnTextChanged));
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SuggestItem();
        }
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SuggestItem;
        }

        internal void UpdateList()
        {
            CollectionViewSource viewSource = new CollectionViewSource();
            viewSource.Source = Suggest.GetSuggestions(_element);
            _view = viewSource.View;
            UpdateFilter();
            ItemsSource = _view;
            SelectedIndex = -1;
            _element.CoerceValue(Suggest.IsOpenProperty);
        }

        internal void UpdateFilter()
        {
            if (_view == null) return;

            // Use the Default Filter if the Filter property has not been set
            ValueSource filterSource = DependencyPropertyHelper.GetValueSource(_element, Suggest.FilterProperty);
            if (filterSource.BaseValueSource == BaseValueSource.Default)
                _view.Filter = new Predicate<object>(DefaultFilter);
            else
                _view.Filter = Suggest.GetFilter(_element);
        }

        public bool DefaultFilter(object item)
        {
            string t = item != null ? item.ToString() : null;

            return t != null &&
                t.StartsWith(_originalText, StringComparison.CurrentCultureIgnoreCase) &&
                !t.Equals(_originalText, StringComparison.CurrentCultureIgnoreCase);
        }

        private void OnPreviewElementKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                // Increase index
                SelectedIndex = (SelectedIndex + 2) % (Items.Count + 1) - 1;
                UpdateText();
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                // Decrease index
                SelectedIndex = (SelectedIndex + (Items.Count + 1)) % (Items.Count + 1) - 1;
                UpdateText();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                Suggest.SetIsOpen(_element, false);
            }
            else if (e.Key == Key.Escape)
            {
                // Restore the original text and close
                SetText(_originalText);
                Suggest.SetIsOpen(_element, false);
            }
        }

        internal void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_settingText)
            {
                _originalText = FocusedTextBox.Text;
                if (_view != null)
                    _view.Refresh();
                //Refreshing seems to select an item
                SelectedIndex = -1;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            // If the items becomes empty, close the list
            _element.CoerceValue(Suggest.IsOpenProperty);
        }

        internal void UpdateText()
        {
            // Set the TextBox Text to the currently selected item
            SetText(SelectedIndex >= 0 ? SelectedValue.ToString() : _originalText);
        }

        private void SetText(string text)
        {
            try
            {
                _settingText = true;
                FocusedTextBox.Text = text;
                FocusedTextBox.CaretIndex = text.Length;
            }
            finally
            {
                _settingText = false;
            }
        }

        private TextBox FocusedTextBox
        {
            get
            {
                Debug.Assert(Keyboard.FocusedElement is TextBox && _element.IsAncestorOf((TextBox)Keyboard.FocusedElement));
                return (TextBox)Keyboard.FocusedElement;
            }
        }

        private UIElement _element;
        private Popup _popup;
        private string _originalText;
        private bool _settingText;

        private ICollectionView _view;
    }

    public class SuggestItem : ListBoxItem
    {
        static SuggestItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestItem), new FrameworkPropertyMetadata(typeof(SuggestItem)));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.IsSelected = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //base.OnMouseDown(e);
            if (e.ChangedButton == MouseButton.Left)
            {
                SuggestList list = (SuggestList)ItemsControl.ItemsControlFromItemContainer(this);
                list.UpdateText();
            }
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            BringIntoView();
        }
    }
}
