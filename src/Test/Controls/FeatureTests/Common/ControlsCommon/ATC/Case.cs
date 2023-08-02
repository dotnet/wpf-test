using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;

namespace Avalon.Test.ComponentModel.Utilities
{
    [ContentProperty("DTemp")]
    public class Case : IAddChild
    {
        private PropertyPath _property = new PropertyPath("");

        private object _value = null;

        private object _dtemp;

        public object DTemp
        {
            get { return _dtemp; }
            set
            {
                if (_dtemp == null)
                    _dtemp = value;
                else
                    throw new ArgumentException("Only one item allowed");
            }
        }

        public PropertyPath Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public object Content
        {
            get
            {
                return _dtemp;
            }
        }

        public bool CanApplyTo(object item)
        {
            object value = ValueGetter.GetValue(item, Property);
            if (value == Value || (value != null && value.Equals(Value)))
                return true;

            if (value != null && Value is string)
            {
                Type type = value.GetType();
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    object expected = converter.ConvertFrom(Value);
                    if (value.Equals(expected))
                        return true;
                }
            }

            return false;
        }

        #region IAddChild Members

        public void AddChild(object value)
        {
            if (_dtemp != null)
                throw new ArgumentException("Only one item allowed");

            _dtemp = value;
        }

        public void AddText(string text)
        {
            AddChild(text);
        }

        #endregion
    }
}
