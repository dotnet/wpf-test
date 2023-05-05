// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.DataServices
{
	public class NameAgeDataSource
	{
		private string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private int _age;

		public int Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public NameAgeDataSource()
		{
			Name = "";
			Age = 0;
		}
	}
}
