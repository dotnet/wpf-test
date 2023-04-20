// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Forms;

namespace Microsoft.Test.DataServices
{
    public class BookDataSource : DataSourceProvider
    {
        public BookDataSource()
        {
        }


        protected override void BeginQuery()
        {
            ObservableCollection<BookSource> data = new ObservableCollection<BookSource>();

            data.Add(new BookSource("0-7356-1448-2", "Microsoft C# Language Specifications"));
            data.Add(new BookSource("0-7356-1288-9", "Inside C#"));
            data.Add(new BookSource("0-7356-0562-9", "XML in Action"));
            OnQueryFinished(data);
        }

    }

    public class BookSource : INotifyPropertyChanged
    {
        private string _isbn;

        private string _title;

        public BookSource(string isbn, string title)
        {
            _isbn = isbn;
            _title = title;
        }

        public string ISBN
        {
            get { return _isbn; }
            set
            {
                _isbn = value;
                RaisePropertyChangedEvent("ISBN");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChangedEvent("Title");
            }
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
