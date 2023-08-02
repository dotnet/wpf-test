using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;


namespace WpfControlToolkit
{
    public abstract class BrushSelector
    {
        public abstract Brush SelectBrush(object item, DependencyObject container);
    }
}
