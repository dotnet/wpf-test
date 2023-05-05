// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;

namespace Microsoft.Test.DataServices
{
    public class MyDynamicObject : DynamicObject, INotifyPropertyChanged
    {
        #region DynamicObject overrides

        public MyDynamicObject()
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _members.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _members[binder.Name] = value;
            OnPropertyChanged(binder.Name);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _members.Keys;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            int index = (int)indexes[0];
            result = _items[index];
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            int index = (int)indexes[0];
            _items[index] = value;
            OnPropertyChanged(System.Windows.Data.Binding.IndexerName);
            return true;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            if (_members.ContainsKey(binder.Name))
            {
                _members.Remove(binder.Name);
                return true;
            }
            return false;
        }

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            int index = (int)indexes[0];
            _items.RemoveAt(index);
            return true;
        }

        #endregion DynamicObject overrides

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged

        #region Public methods

        // the toolset compiler doesn't tell the runtime that the result will be
        // thrown away, and the runtime now crashes.
        public object AddItem(object item)
        {
            _items.Add(item);
            OnPropertyChanged(System.Windows.Data.Binding.IndexerName);
            return null;
        }

        // the toolset compiler doesn't tell the runtime that the result will be
        // thrown away, and the runtime now crashes (as of build 20814).
        public object DeleteProperty(string propertyToRemove)
        {
            _members.Remove(propertyToRemove);
            OnPropertyChanged("TempProperty");
            return null;
        }

        #endregion Public methods

        #region Private data

        Dictionary<string, object> _members = new Dictionary<string, object>();
        ObservableCollection<object> _items = new ObservableCollection<object>();

        #endregion Private data
    }
}
