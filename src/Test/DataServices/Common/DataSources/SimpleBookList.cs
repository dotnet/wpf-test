// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Test.DataServices
{
   public class SimpleBookList : ObservableCollection<SimpleBook>
    {
        public SimpleBookList()
        {
            Add(new SimpleBook("Book 1", SimpleBookStatus.InStock));
            Add(new SimpleBook("Book 2", SimpleBookStatus.Ordered));
            Add(new SimpleBook("Book 3", SimpleBookStatus.Ordered));
            Add(new SimpleBook("Book 4", SimpleBookStatus.InStock));
            Add(new SimpleBook("Book 5", SimpleBookStatus.OutOfPrint));
            Add(new SimpleBook("Book 6", SimpleBookStatus.InStock));
            Add(new SimpleBook("Book 7", SimpleBookStatus.InStock));
            Add(new SimpleBook("Book 8", SimpleBookStatus.OutOfPrint));
        }
    }

    public enum SimpleBookStatus { InStock, Ordered, OutOfPrint };

    public class SimpleBook
    {
        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private SimpleBookStatus _status;

        public SimpleBookStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public SimpleBook(string title, SimpleBookStatus status)
        {
            this.Title = title;
            this.Status = status;
        }
    }
}
