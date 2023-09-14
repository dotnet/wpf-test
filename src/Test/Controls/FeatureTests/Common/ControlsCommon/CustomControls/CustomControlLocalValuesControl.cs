using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Test.Controls
{
    public class CustomControlLocalValueControl : ContentControl, INotifyPropertyChanged
    {
        private object _lastBaseValueFromCoercionCallback;
        private object _lastOldValueFromPropertyChangedCallback;
        private object _lastNewValueFromPropertyChangedCallback;
        private object _titleLocalValue;
        private bool _useCustomCoercion;
        private ValueSource _titleValueSource;

        #region WorkingTagProperty

        public static readonly DependencyProperty WorkingTagProperty = DependencyProperty.Register(
            "WorkingTag",
            typeof(object),
            typeof(CustomControlLocalValueControl),
            new UIPropertyMetadata("<working tag>"));

        public object WorkingTag
        {
            get { return GetValue(WorkingTagProperty); }
            set { SetValue(WorkingTagProperty, value); }
        }

        #endregion WorkingTagProperty

        #region TitleProperty

        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
           "Title", typeof(string),
           typeof(CustomControlLocalValueControl),
           new FrameworkPropertyMetadata(
               null,
               FrameworkPropertyMetadataOptions.Inherits,
               new PropertyChangedCallback(OnTitlePropertyChanged),
               new CoerceValueCallback(OnCoerceTitleProperty)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomControlLocalValueControl)
            {
                (d as CustomControlLocalValueControl).OnTitlePropertyChanged(e);
            }
        }

        private void OnTitlePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            LastNewValueFromPropertyChangedCallback = e.NewValue;
            LastOldValueFromPropertyChangedCallback = e.OldValue;

            TitleValueSource = DependencyPropertyHelper.GetValueSource(this, CustomControlLocalValueControl.TitleProperty);
            TitleLocalValue = this.ReadLocalValue(CustomControlLocalValueControl.TitleProperty);
        }

        private static object OnCoerceTitleProperty(DependencyObject d, object baseValue)
        {
            if (d is CustomControlLocalValueControl)
            {
                return (d as CustomControlLocalValueControl).OnCoerceTitleProperty(baseValue);
            }
            else
            {
                return baseValue;
            }
        }

        private object OnCoerceTitleProperty(object baseValue)
        {
            LastBaseValueFromCoercionCallback = baseValue;

            if (UseCustomCoercion)
            {
                return "custom coerced value";
            }
            else
            {
                return baseValue;
            }
        }

        public ValueSource TitleValueSource
        {
            get { return _titleValueSource; }
            private set
            {
                _titleValueSource = value;
                OnPropertyChanged("TitleValueSource");
            }
        }

        public bool UseCustomCoercion
        {
            get { return _useCustomCoercion; }
            set
            {
                _useCustomCoercion = value;
                OnPropertyChanged("UseCustomCoercion");
            }
        }

        public object TitleLocalValue
        {
            get { return _titleLocalValue; }
            set
            {
                _titleLocalValue = value;
                OnPropertyChanged("TitleLocalValue");
            }
        }

        public object LastBaseValueFromCoercionCallback
        {
            get { return _lastBaseValueFromCoercionCallback; }
            set
            {
                _lastBaseValueFromCoercionCallback = value;
                OnPropertyChanged("LastBaseValueFromCoercionCallback");
            }
        }

        public object LastNewValueFromPropertyChangedCallback
        {
            get { return _lastNewValueFromPropertyChangedCallback; }
            set
            {
                _lastNewValueFromPropertyChangedCallback = value;
                OnPropertyChanged("LastNewValueFromPropertyChangedCallback");
            }
        }

        public object LastOldValueFromPropertyChangedCallback
        {
            get { return _lastOldValueFromPropertyChangedCallback; }
            set
            {
                _lastOldValueFromPropertyChangedCallback = value;
                OnPropertyChanged("LastOldValueFromPropertyChangedCallback");
            }
        }

        #endregion TitleProperty

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged
    }
}
