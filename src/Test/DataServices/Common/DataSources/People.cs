// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.Test.DataServices
{
	public class People : ObservableCollection<Person>
	{
		public People()
		{
			Add(new Person("Beatriz", "portuguese"));
			Add(new Person("Radu", "romanian"));
			Add(new Person("James", "canadian"));
			Add(new Person("Vincent", "belgian"));
			Add(new Person("David", "spanish"));
			Add(new Person("Stuart", "scotish"));
			Add(new Person("Marisa", "indonesian"));
			Add(new Person("Lilly", "taiwanese"));
			Add(new Person("Dennis", "greek"));
			Add(new Person("Michael", "american"));
		}
	}

	public class Person
	{
		private string _name;

		private string _nationality;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Nationality
		{
			get { return _nationality; }
			set { _nationality = value; }
		}

		public Person(string name, string nationality)
		{
			this._name = name;
			this._nationality = nationality;
		}
	}
}
