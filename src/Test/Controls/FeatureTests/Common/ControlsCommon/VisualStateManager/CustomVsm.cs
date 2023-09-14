using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    public class CustomVsm : VisualStateManager
    {
        public delegate bool GoToStateDelegate(FrameworkElement control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions);

        public GoToStateDelegate GoToStateFunction { get; set; }

        protected override bool GoToStateCore(FrameworkElement control, FrameworkElement templateRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            if (GoToStateFunction != null)
            {
                return GoToStateFunction(control, templateRoot, stateName, group, state, useTransitions);
            }
            else if (group != null && state != null)
            {
                return base.GoToStateCore(control, templateRoot, stateName, group, state, useTransitions);
            }
            else
            {
                return false;
            }
        }
    }
}
