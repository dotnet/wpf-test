// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace WFCTestLib.Util
{
	/// <summary>
	/// Keeps track of level of identation, as well as supplies the necessary
	/// spaces to ident as needed.
	/// </summary>
	public class Indenter
	{
		#region members
		private const int numSpaces = 4;
		private const string spaceChars = " ";
		private StringBuilder _spaces = new StringBuilder();
		private string _indentSpace;
		private int _level = 0;
		#endregion

		/// <summary>
		/// current level of indentation.
		/// </summary>
		public int Level
		{
			get { return _level; }
			set
			{
				_level = value;
				UpdateSpaces();
			}
		}

		/// <summary>
		/// constructor.
		/// </summary>
		public Indenter()
		{
			StringBuilder buf = new StringBuilder();
			for(int i = 0; i < numSpaces; i++) { buf.Append(spaceChars); }
			_indentSpace = buf.ToString();
		}

		/// <summary>
		/// returns string representing the number of spaces for indentation
		/// </summary>
		/// <returns>string representing the number of spaces for indentation</returns>
		public override string ToString()
		{
			return _spaces.ToString();
		}

		/// <summary>
		/// updates the value returned by ToString()
		/// </summary>
		private void UpdateSpaces()
		{
			_spaces.Length = 0;
			for(int i = 0; i < Level; i++) { _spaces.Append(_indentSpace); }
		}
	}
}
