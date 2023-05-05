// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.DataServices
{
	public class Star
	{
		private bool _throwExceptionOnSetter;

		public bool ThrowExceptionOnSetter
		{
			get { return _throwExceptionOnSetter; }
			set
			{
				_throwExceptionOnSetter = value;
			}
		}

		private string _firstName;

		public string FirstName
		{
			get { return _firstName; }
			set
			{
				if (_throwExceptionOnSetter)
				{
					throw new Exception("Can not set FirstName");
				}
				else
				{
					_firstName = value;
				}
			}
		}

		private string _lastName;

		public string LastName
		{
			get { return _lastName; }
			set
			{
				if (_throwExceptionOnSetter)
				{
					throw new Exception("Can not set LastName");
				}
				else
				{
					_lastName = value;
				}
			}
		}

		private int _age;
		public int Age
		{
			get { return _age; }
			set
			{
				if (_throwExceptionOnSetter)
				{
					throw new Exception("Can not set Age");
				}
				else
				{
					_age = value;
				}
			}
		}

		public Star(string firstName, string lastName, int age)
		{
			this._firstName = firstName;
			this._lastName = lastName;
			this._age = age;
		}

		public Star()
		{
			this._firstName = "";
			this._lastName = "";
			this._age = 0;
		}
	}
}
