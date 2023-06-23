using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace WpfControlToolkit
{
    public class ChartItemControl
    {
        public ChartItemControl()
            : this(0, string.Empty)
        {
        }

        public ChartItemControl(double value, string name)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                throw new ArgumentOutOfRangeException("value");
            }

            _value = value;
            _name = name;
        }

        public double Value
        {
            get { return _value; }
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                else
                {
                    _value = value;
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private double _value;
        private string _name;
    }
}
