// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Test.Xaml.Types.Security
{
    public class TypeWithModifiers
    {
        internal int InternalProp { get; set; }
        private int PrivateProp { get; set; }
        protected int ProtectedProp { get; set; }

        internal static TypeWithModifiers Create()
        {
            return new TypeWithModifiers();
        }

        public static TypeWithModifiers Create1()
        {
            return new TypeWithModifiers();
        }


        internal TCProp TCProp { get; set; }
    }

    [TypeConverter(typeof(PropTypeConverter))]
    public class TCProp { }

    class PropTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(TCProp))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return new TCProp();
        }
    }

    public class TypeWithEvent
    {
        public int EventCount { get; set; }
        public delegate void EventDelegate(object source, EventArgs args);

        public event EventDelegate PublicEvent;

        public void RaisePublicEvent()
        {
            if (PublicEvent != null)
            {
                EventArgs args = new EventArgs();
                PublicEvent(this, args);
            }
        }

        private void PrivateHandler(object source, EventArgs args)
        {
            EventCount++;
        }

        internal void InternalHandler(object source, EventArgs args)
        {
            EventCount++;
        }
    }
}
