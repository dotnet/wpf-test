using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.ComponentModel;


namespace Avalon.Test.ComponentModel.Utilities
{
    [ContentProperty("Conditions")]
    public class GenericDataTemplateSelector : DataTemplateSelector, IAddChild
    {
        private Collection<Case> _conditions = new Collection<Case>();

        public Collection<Case> Conditions
        {
            get
            {
                return _conditions;
            }
        }

        public void AddChild(object o)
        {
            Case c = o as Case;
            if (c == null)
            {
                throw new ArgumentException("Only Case allowed");
            }
            _conditions.Add(c);
        }

        public void AddText(string o)
        {
            AddChild(o);
        }

        public DataTemplate Default
        {
            get { return _default; }
            set { _default = value; }
        }

        private DataTemplate _default = null;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            foreach (Case c in _conditions)
            {
                if (c.CanApplyTo(item))
                    return c.Content as DataTemplate;
            }

            return Default;
        }
    }
}
