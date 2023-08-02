// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Implements the EventRecord as a structure to be used
 *          for filtering events.
 * 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Common base for EventRecord and EventRecordGroup.
    /// </summary>
    public class EventRecordBase
    {
    }
    /// <summary>
    /// Represents an instance of an event listener called.
    /// </summary>
    public class EventRecord : EventRecordBase
    {
        /// <summary>
        /// </summary>
        public EventRecord()
        {
            _name = "";
            _senderName = "";
            _propertyRecords = new List<PropertyRecord>();
            _downKeys = new KeyList();
            _eventArgs = new EventArgsRecord();
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            string str = Name + " " + SenderName;

            string downKeys = DownKeys.ToString();
            if(!String.IsNullOrEmpty(downKeys))
                str += " - '" + downKeys + "'";

            return str;
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = this.Name.GetHashCode() ^ this.SenderName.GetHashCode() ^ this.DownKeys.GetHashCode();

            for (int i = 0; i < this.PropertyRecords.Count; i++)
            {
                hashCode ^= this.PropertyRecords[i].GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is EventRecord))
                return false;

            EventRecord record = (EventRecord)obj;

            if (!this.Name.Equals(record.Name))
                return false;

            if (!this.SenderName.Equals(record.SenderName))
                return false;

            if (!this.PropertyRecords.Count.Equals(record.PropertyRecords.Count))
                return false;

            for (int i = 0; i < this.PropertyRecords.Count; i++)
            {
                if (!this.PropertyRecords[i].Equals(record.PropertyRecords[i]))
                    return false;
            }

            if (this.DownKeys != null && !this.DownKeys.Equals(record.DownKeys))
                return false;
            else if (this.DownKeys == null && record.DownKeys != null)
                return false;

            if (this.EventArgs != null && !this.EventArgs.Equals(record.EventArgs))
                return false;
            else if(this.EventArgs == null && record.EventArgs != null)
                return false;

            return true;
        }

        /// <summary>
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// </summary>
        public string SenderName
        {
            get { return _senderName; }
            set { _senderName = value; }
        }

        /// <summary>
        /// </summary>
        public List<PropertyRecord> PropertyRecords
        {
            get { return _propertyRecords; }
        }

        /// <summary>
        /// </summary>
        public KeyList DownKeys
        {
            get { return _downKeys; }
            set { _downKeys = value; }
        }

        /// <summary>
        /// </summary>
        public EventArgsRecord EventArgs
        {
            get { return _eventArgs; }
            set { _eventArgs = value; }
        }

        private string _name;
        private string _senderName;
        private List<PropertyRecord> _propertyRecords;
        private KeyList _downKeys = null;
        private EventArgsRecord _eventArgs = null;
    }
    /// <summary>
    /// </summary>
    public class EventArgsRecord
    {
        /// <summary>
        /// </summary>
        public EventArgsRecord()
        {
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = this.Type.GetHashCode();

            foreach (string key in this.Args.Keys)
            {
                hashCode ^= this.Args[key].GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is EventArgsRecord))
                return false;

            EventArgsRecord argsRecord = (EventArgsRecord)obj;

            if (this.Type != null && !this.Type.Equals(argsRecord.Type))
                return false;
            else if (this.Type == null && argsRecord.Type != null)
                return false;

            if (this.Args.Count != argsRecord.Args.Count)
                return false;

            foreach (string key in this.Args.Keys)
            {
                object val1 = this.Args[key];
                object val2 = argsRecord.Args[key];

                val1 = val1 == null ? "null" : val1.ToString();
                val2 = val2 == null ? "null" : val2.ToString();

                if (val1 != null && !val1.Equals(val2))
                    return false;
                else if (val1 == null && val2 != null)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// </summary>
        public Type Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// </summary>
        public Dictionary<string, object> Args
        {
            get { return _args; }
        }
        /// <summary>
        /// </summary>
        public object GetArg(string name)
        {
            if (_args.ContainsKey(name))
            {
                return _args[name];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// </summary>
        public void SetArg(string name, object value)
        {
            _args[name] = value;
        }

        private Type _type = null;
        private Dictionary<string, object> _args = new Dictionary<string, object>();
    }
    /// <summary>
    /// </summary>
    [TypeConverter(typeof(KeyListConverter))]
    public class KeyList : List<Key>
    {
        /// <summary>
        /// </summary>
        public KeyList()
        {
        }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < this.Count; i++)
            {
                Key key = this[i];
                if (i > 0)
                {
                    builder.Append("+");
                }
                builder.Append(Enum.GetName(typeof(Key), key));
            }

            return builder.ToString();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = 0;

            for (int i = 0; i < this.Count; i++)
            {
                hashCode ^= this[i].GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is KeyList))
                return false;

            KeyList records = (KeyList)obj;

            if (!this.Count.Equals(records.Count))
                return false;

            for (int i = 0; i < this.Count; i++)
            {
                if (!this[i].Equals(records[i]))
                    return false;
            }

            return true;
        }
    }
    /// <summary>
    /// Converter class for converting between a string and the Type of a KeyList
    /// </summary>
    /// <ExternalAPI/> 
    public class KeyListConverter : TypeConverter
    {
        /// <summary>
        /// CanConvertFrom()
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            // We can only handle string.
            return sourceType == typeof(string);
        }

        /// <summary>
        /// TypeConverter method override. 
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return false;
        }

        /// <summary>
        /// ConvertFrom()
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (source is string)
            {
                string[] keyArray = ((string)source).Split('+');

                KeyList keys = new KeyList();
                foreach (string keystr in keyArray)
                {
                    keys.Add((Key)Enum.Parse(typeof(Key), keystr));
                }

                return keys;
            }

            throw new ArgumentException("Can only convert from string type.", "source");
        }

        /// <summary>
        /// ConvertTo()
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new NotSupportedException("Cannot convert to any type.");
        }
    }
    /// <summary>
    /// Represents a repeating group of instances of event listeners called.
    /// </summary>
    public class EventRecordGroup : EventRecordBase
    {
        /// <summary>
        /// </summary>
        public EventRecordGroup() : base()
        {
            _eventRecords = new List<EventRecord>();
        }

        /// <summary>
        /// </summary>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();

            hashCode ^= this.RepeatCount.GetHashCode();

            for (int i = 0; i < this.EventRecords.Count; i++)
            {
                hashCode ^= this.EventRecords[i].GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is EventRecordGroup))
                return false;

            EventRecordGroup record = (EventRecordGroup)obj;

            if (!base.Equals(obj))
                return false;

            if (!this.RepeatCount.Equals(record.RepeatCount))
                return false;

            if (!this.EventRecords.Count.Equals(record.EventRecords.Count))
                return false;

            for (int i = 0; i < this.EventRecords.Count; i++)
            {
                if (!this.EventRecords[i].Equals(record.EventRecords[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// </summary>
        public int RepeatCount
        {
            get
            {
                return _repeatCount;
            }
            set
            {
                _repeatCount = value;
            }
        }

        /// <summary>
        /// </summary>
        public List<EventRecord> EventRecords
        {
            get
            {
                return _eventRecords;
            }
        }

        private List<EventRecord> _eventRecords;
        private int _repeatCount = -1;
    }
}

