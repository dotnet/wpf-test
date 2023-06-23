using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfControlToolkit
{
    public class SuggestComboBox : ComboBox
    {
        static SuggestComboBox()
        {
            IsTextSearchEnabledProperty.OverrideMetadata(typeof(SuggestComboBox), new FrameworkPropertyMetadata(false));
            IsEditableProperty.OverrideMetadata(typeof(SuggestComboBox), new FrameworkPropertyMetadata(true));
            IsDropDownOpenProperty.OverrideMetadata(typeof(SuggestComboBox), new FrameworkPropertyMetadata(OnIsDropDownOpenChanged));
        }

        private static void OnIsDropDownOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SuggestComboBox scb = (SuggestComboBox)sender;
            Suggest.SetIsEnabled(scb, !(bool)e.NewValue);
        }
        
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!Suggest.GetIsOpen(this))
                base.OnPreviewKeyDown(e);
        }
    }
}
