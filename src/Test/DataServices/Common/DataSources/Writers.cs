// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Test.DataServices
{
    public class Writers : ObservableCollection<Writer>
    {
        public Writers()
        {
            Add(new Writer("Carl", "Sagan"));
            Add(new Writer("Stephen", "King"));
            Add(new Writer("Jules", "Verne"));
            Add(new Writer("J.R.R.", "Tolkien"));
        }
    }

    public class Writer
    {
        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public Writer()
        {
        }

        public Writer(string firstName, string lastName)
        {
		this.FirstName = firstName;
		this.LastName = lastName;
        }

    }
}
