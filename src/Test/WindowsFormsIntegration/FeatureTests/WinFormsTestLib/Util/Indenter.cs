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
		private StringBuilder spaces = new StringBuilder();
		private string indentSpace;
		private int level = 0;
		#endregion

		/// <summary>
		/// current level of indentation.
		/// </summary>
		public int Level
		{
			get { return level; }
			set
			{
				level = value;
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
			indentSpace = buf.ToString();
		}

		/// <summary>
		/// returns string representing the number of spaces for indentation
		/// </summary>
		/// <returns>string representing the number of spaces for indentation</returns>
		public override string ToString()
		{
			return spaces.ToString();
		}

		/// <summary>
		/// updates the value returned by ToString()
		/// </summary>
		private void UpdateSpaces()
		{
			spaces.Length = 0;
			for(int i = 0; i < Level; i++) { spaces.Append(indentSpace); }
		}
	}
}
