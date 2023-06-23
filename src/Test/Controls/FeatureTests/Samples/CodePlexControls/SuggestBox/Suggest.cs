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
    public static class Suggest
    {
        // Determines if Suggestions are enabled on the element
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(Suggest),
                new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return (bool)element.GetValue(IsEnabledProperty);
        }
        public static void SetIsEnabled(UIElement element, bool value)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(IsEnabledProperty, value);
        }

        private static readonly KeyboardFocusChangedEventHandler _focusHandler = new KeyboardFocusChangedEventHandler(OnKeyboardFocusChanged);
        private static readonly TextChangedEventHandler _textHandler = new TextChangedEventHandler(OnTextChanged);
        
        private static void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)sender;
            
            if ((bool)e.NewValue)
            {
                element.AddHandler(UIElement.LostKeyboardFocusEvent, _focusHandler, true);
                element.AddHandler(TextBox.TextChangedEvent, _textHandler, true);
            }
            else
            {
                element.RemoveHandler(UIElement.LostKeyboardFocusEvent, _focusHandler);
                element.RemoveHandler(TextBox.TextChangedEvent, _textHandler);
            }

            element.CoerceValue(IsOpenProperty);
        }

        private static void OnKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((UIElement)sender).CoerceValue(IsOpenProperty);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UIElement element = (UIElement)sender;
            // Refresh the list even when it is closed to see if any suggestions are valid
            SuggestList list = GetSuggestList(element);
            if (list != null)
                list.OnTextChanged(sender, e);

            SetIsOpen(element, true);
        }

        
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.RegisterAttached(
                "IsOpen",
                typeof(bool),
                typeof(Suggest),
                new FrameworkPropertyMetadata(false, OnIsOpenChanged, CoerceIsOpen));

        public static bool GetIsOpen(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return (bool)element.GetValue(IsOpenProperty);
        }

        public static void SetIsOpen(UIElement element, bool value)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(IsOpenProperty, value);
        }

        private static void OnIsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)sender;

            SuggestList list = GetSuggestList(element);
            if (list != null)
            {
                list.IsOpen = (bool)e.NewValue;
            }
            else if ((bool)e.NewValue)
            {
                Suggest.SetSuggestList(element, new SuggestList(element));
                element.CoerceValue(IsOpenProperty);
            }
        }

        private static object CoerceIsOpen(object sender, object value)
        {
            UIElement element = (UIElement)sender;
            if ((bool)value && GetIsEnabled(element))
            {
                TextBox focused = Keyboard.FocusedElement as TextBox;
                if (focused != null && element.IsAncestorOf(focused))
                {
                    SuggestList list = GetSuggestList(element);
                    return list == null || list.Items.Count > 0;
                }
            }
            return false;
        }

        // A list of suggestions fo the text 
        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.RegisterAttached("Suggestions",
                typeof(IList),
                typeof(Suggest),
                new FrameworkPropertyMetadata(OnSuggestionsChanged));

        public static IList GetSuggestions(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return (IList)element.GetValue(SuggestionsProperty);
        }
        public static void SetSuggestions(UIElement element, IList value)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(SuggestionsProperty, value);
        }
        private static void OnSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)sender;
        
            SuggestList list = GetSuggestList(element);
            if (list != null)
                list.UpdateList();
            else
                element.CoerceValue(IsOpenProperty);
        }

        // Filters the Suggestions list when the TextBox's Text changes
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.RegisterAttached("Filter", 
                typeof(Predicate<object>), 
                typeof(Suggest),
                new FrameworkPropertyMetadata(OnFilterChanged));

        public static Predicate<object> GetFilter(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return (Predicate<object>)element.GetValue(FilterProperty);
        }
        public static void SetFilter(UIElement element, Predicate<object> value)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(FilterProperty, value);
        }

        private static void OnFilterChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SuggestList list = GetSuggestList((UIElement)sender);
            if (list != null)
            {
                list.UpdateFilter();
            }
        }

        // Caches the SuggestList associated with the element
        private static readonly DependencyProperty SuggestListProperty =
            DependencyProperty.RegisterAttached(
                "SuggestList",
                typeof(SuggestList),
                typeof(Suggest));

        private static SuggestList GetSuggestList(UIElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return (SuggestList)element.GetValue(SuggestListProperty);
        }

        private static void SetSuggestList(UIElement element, SuggestList value)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(SuggestListProperty, value);
        }
    }
}
