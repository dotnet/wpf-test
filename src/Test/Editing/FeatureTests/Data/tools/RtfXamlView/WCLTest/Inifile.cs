// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace Inifile
{
	using Common;

	// Class INIFILE - Implementation of .ini file input
	class CInifile
	{
		StreamReader _stream;
		string _sfnInifile;
		int _nLine;

		// Constructor: initialized ini file reader 
		public CInifile (string _sfnInifile)
		{
			this._sfnInifile = _sfnInifile;
			_nLine = 0;

			try
			{
                _stream = File.OpenText (this._sfnInifile);
			}
			catch (Exception) { RaiseError ("Unable to open file for reading"); }
		}

		// Finalizer: releases system resources
		public void Close ()
		{
			_stream.Close ();
		}

		// Read next statement, return false if end of stream
		public bool FGetNext (string strKey, out string strValue)
		{
			string s1, s2;
			bool fFoundNextStatement;

			s1 = null;
			s2 = null;
			fFoundNextStatement = false;

			while (_stream.Peek () != -1 && !fFoundNextStatement)
			{
				string help = _stream.ReadLine ().Trim ();

				_nLine ++; /* Increase line counter for error messages */

				if (help != "" && help [0] != ';')
				{
					/* Not empty line and not a comment line */
					if (!Common.FSplitString (help, '=', out s1, out s2))
					{
						RaiseError ("Statement must include '='");
					}

					s1 = s1.Trim ();
					s2 = s2.Trim ();
					fFoundNextStatement = true;
				};
			};
			
			if (fFoundNextStatement) 
			{
				if (Common.CompareStr (s1, strKey) == 0)
				{
					/* Found correct key */
					strValue = s2;
					return true;
				}
				else
				{
					RaiseError ("Incorrect key '" + s1 + "', expected: '" + strKey + "'");
					strValue = ""; // will not be executed
					return false;
				}														 
			}
			else
			{
				/* End of stream */
				strValue = "";
				return false;
			};
		}

		// Report error in ini file
		void RaiseError (string strMessage)
		{
			W11Messages.RaiseError ( "Error in configuation file " + _sfnInifile + ", line " + _nLine.ToString () + "\n\n" +
				strMessage );
		}

		// Read next statement, fire exception is not equal to Key
		public string GetNext (string strKey)
		{
			string strValue;

			if (!FGetNext (strKey, out strValue))
			{
				RaiseError ("Can not find key '" + strKey + "'");
				return "";
			}
			else return strValue;
		}
	}
}