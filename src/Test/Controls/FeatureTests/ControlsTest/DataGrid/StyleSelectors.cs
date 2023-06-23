using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Test.Controls
{
    public class RowStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style NewItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {            
            ItemsControl itemsControl = ItemsControl.ItemsControlFromItemContainer(container);
            if (item == CollectionView.NewItemPlaceholder)
            {
                return NewItemStyle;
            }
            else
            {
                return DefaultStyle;
            }
        }
    }

    public class TemplateColumnDataTemplateSelector : DataTemplateSelector
    {
        public TemplateColumnDataTemplateSelector()
        {
            this.ItemsFromSelectTemplate = new List<object>();
        }

        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate CustomDataTemplate { get; set; }

        public List<object> ItemsFromSelectTemplate { get; private set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            this.ItemsFromSelectTemplate.Add(item);
            return DefaultDataTemplate;
        }
    }
}
