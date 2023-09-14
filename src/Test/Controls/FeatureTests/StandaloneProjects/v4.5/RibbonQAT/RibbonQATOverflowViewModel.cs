
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Ribbon QAT overflow viewmodel
    /// </summary>
    public class RibbonQATOverflowViewModel : DependencyObject
    {
        public RibbonQATOverflowViewModel()
        {
            this.PasteCommand = new RibbonQATOverflowCommand(this);
        }

        public ICommand PasteCommand { set; get; }

        public string ExecuteCommandStatus
        {
            get { return (string)GetValue(ExecuteCommandStatusProperty); }
            set { SetValue(ExecuteCommandStatusProperty, value); }
        }

        public static readonly DependencyProperty ExecuteCommandStatusProperty =
            DependencyProperty.Register("ExecuteCommandStatus", typeof(string), typeof(RibbonQATOverflowViewModel), new UIPropertyMetadata(""));
    }
}
