// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Query
{
	using Common;

	public class CQuery
	{
		CQueryNode _qnodeTop;
		public string ErrorMessage;

		// CQuery
		public CQuery ()
		{
			_qnodeTop = null;
		}

		// CreateQuery
		public bool CreateQuery (string [] rgstrColumnNames, string strQuery)
		{
			return CQueryCompiler.FCompile ( rgstrColumnNames, strQuery, 
											 out _qnodeTop, out ErrorMessage );
		}

		// ExecuteQuery: returns true if record corresponds to query
		public bool ExecuteQuery (string [] rgstrRecord, out bool fResult)
		{
			try
			{
				if (_qnodeTop == null) fResult = false;
				else
				{
					fResult = ExecuteQueryBool (_qnodeTop, rgstrRecord);
				}

				return true; /* Successful */
			}
			catch 
			{
				fResult = false;

				return false;
			}
		}

		// QueryError
		void QueryError (string strMsg)
		{
			ErrorMessage = strMsg;
			throw new QueryException ();
		}

		// ExecuteQueryBool: match, convert result to Bool
		bool ExecuteQueryBool (CQueryNode qnode, string [] rgstrRecord)
		{
			if (qnode.kn == KNode.knAnd)
			{
				return ExecuteQueryBool (qnode.qnode1, rgstrRecord) && 
					ExecuteQueryBool (qnode.qnode2, rgstrRecord);
			}
			else if (qnode.kn == KNode.knOr)
			{
				return ExecuteQueryBool (qnode.qnode1, rgstrRecord) ||
					ExecuteQueryBool (qnode.qnode2, rgstrRecord);
			}
			else if (qnode.kn == KNode.knNot)
			{
				Common.Assert (qnode.qnode2 == null);
				return ! ExecuteQueryBool (qnode.qnode1, rgstrRecord);
			}
			else if (qnode.kn == KNode.knEqual)
			{
				string str1 = ExecuteQueryString (qnode.qnode1, rgstrRecord);
				string str2 = ExecuteQueryString (qnode.qnode2, rgstrRecord);

				return String.Compare (str1, str2, true) == 0;
			}
			else if (qnode.kn == KNode.knContains)
			{
				string str1 = ExecuteQueryString (qnode.qnode1, rgstrRecord);
				string str2 = ExecuteQueryString (qnode.qnode2, rgstrRecord);

				return SubstrIndexInsensitive (str2, str1) != -1;
			}
			else if (qnode.kn == KNode.knUnicode)
			{
				string str1 = ExecuteQueryString (qnode.qnode1, rgstrRecord);

				return Common.FContainsUnicodeCharacter (str1);
			}
			else if ( qnode.kn == KNode.knGreater || 
				qnode.kn == KNode.knGreaterOrEqual || 
				qnode.kn == KNode.knLess || 
				qnode.kn == KNode.knLessOrEqual)
			{
				string str1 = ExecuteQueryString (qnode.qnode1, rgstrRecord);
				string str2 = ExecuteQueryString (qnode.qnode2, rgstrRecord);

				return DoIntegerCompare (str1, str2, qnode.kn);
			}
			else 
			{
				/* This is a string convertion to bool */
				/* Empty string means false, otherwise, true */

				string str;

				Common.Assert (qnode.kn == KNode.knColumn || qnode.kn == KNode.knString);

				/* Get the string firts */
				str = ExecuteQueryString (qnode, rgstrRecord);

				return str != ""; 
			}
		}

		// ExecuteQueryString: match, convert result to String
		string ExecuteQueryString (CQueryNode qnode, string [] rgstrRecord)
		{
			if (qnode.kn == KNode.knString)
			{
				return qnode.str;
			}
			else if (qnode.kn == KNode.knColumn)
			{
				Common.Assert (qnode.icolumn < rgstrRecord.Length);
				return rgstrRecord [qnode.icolumn];
			}
			else 
			{
				/* This is a boolean operation! */
				/* Review (why this is allowed at all? */
				/* Get the boolean first, then convert to string */

				if (ExecuteQueryBool (qnode, rgstrRecord)) return "true";
			    else return "false";
			}
		}

		// SubstrIndexInsensitive:
		static int SubstrIndexInsensitive (string strSubstr, string strMain)
		{
			return strMain.ToUpper ().IndexOf (strSubstr.ToUpper ());
		}
		
		// DoIntegerCompare: performs integer compare operator
		bool DoIntegerCompare (string str1, string str2, KNode kn)
		{
			int value1, value2;
			
			if (!Common.ParseStrToInt (str1, out value1))
			{
				QueryError ("Can not convert " + str1 + " to integer");
			}

			if (!Common.ParseStrToInt (str2, out value2))
			{
				QueryError ("Can not convert " + str2 + " to integer");
			}

			if (kn == KNode.knGreater) return value1 > value2;
			else if (kn == KNode.knGreaterOrEqual) return value1 >= value2;
			else if (kn == KNode.knLess ) return value1 < value2;
			else if (kn == KNode.knLessOrEqual ) return value1 <= value2;
			else 
			{
				Common.Assert (false, "Wrong kn");
				return false;
			}
		}
	}

	// QueryCompiler class

	class CQueryCompiler 
	{
		string [] _rgstrColumnNames;
		string _strzQuery;
		int _cp;

		/* Current lexema information */
		KLex _kLexema;
		string _strCurrentValue;				// for string value lexema
		int _iCurrentColumn;					// for column lexema

		string _strErrorMessage;

		// Compiler top entry
		public static bool FCompile (string [] rgstrColumnNames, string strQuery, out CQueryNode qnode,
									 out string strErrorMessage)
		{
			CQueryCompiler compiler = new CQueryCompiler ();
			bool fSuccessful;

			try
			{
				qnode = compiler.CompileCore (rgstrColumnNames, strQuery);
				fSuccessful = true;
				strErrorMessage = "";
			}
			catch 
			{
				// Compilation error
				qnode = null;
				fSuccessful = false;
				strErrorMessage = compiler._strErrorMessage;
			};

			return fSuccessful;
		}

		// CompileCore
		CQueryNode CompileCore (string [] rgstrColumnNames, string strQuery)
		{
			CQueryNode qnode;

			this._rgstrColumnNames = rgstrColumnNames;
			this._strzQuery = strQuery + KCharConst.zero;
			this._cp = 0;

			Next ();

			if (_kLexema == KLex.lexemaEnd) 
			{
				/* Empty query */
				return null;
			}
			else
			{
				qnode = ProcessLevelOr ();
				if (_kLexema != KLex.lexemaEnd) SyntaxError ("Expected end");
				return qnode ;
			};
		}

		// ProcessLevelOr (eat *or* sequence)
		CQueryNode ProcessLevelOr ()
		{
			CQueryNode qnode1 = ProcessLevelAnd (); // One level down
			CQueryNode qnode2;

			if (_kLexema == KLex.lexemaOr)
			{
				Next ();

				qnode2 = ProcessLevelOr (); // Same level
				return CQueryNode.NewOp (KNode.knOr, qnode1, qnode2);
			}
			else return qnode1;
		}

		// ProcessLevelAnd (eat *and* sequence)
		CQueryNode ProcessLevelAnd ()
		{
			CQueryNode qnode1 = ProcessLevelEQ (); // One level down
			CQueryNode qnode2;

			if (_kLexema == KLex.lexemaAnd)
			{
				Next ();

				qnode2 = ProcessLevelAnd (); // Same level
				return CQueryNode.NewOp (KNode.knAnd, qnode1, qnode2);
			}
			else return qnode1;
		}

		// ProcessLevelEQ (eat *eq* )
		CQueryNode ProcessLevelEQ ()
		{
			CQueryNode qnode1 = ProcessLevelFactor (); // One level down
			CQueryNode qnode2;

			if (_kLexema == KLex.lexemaEqual)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knEqual, qnode1, qnode2);
			}
			else if (_kLexema == KLex.lexemaNotEqual)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knNot, 
					CQueryNode.NewOp (KNode.knEqual, qnode1, qnode2), null);
			}
			else if (_kLexema == KLex.lexemaContains)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knContains, qnode1, qnode2);
			}
			else if (_kLexema == KLex.lexemaNotContains)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knNot, CQueryNode.NewOp (KNode.knContains, qnode1, qnode2), null);
			}
			else if (_kLexema == KLex.lexemaGreater)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knGreater, qnode1, qnode2);
			}
			else if (_kLexema == KLex.lexemaGreaterOrEqual)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knGreaterOrEqual, qnode1, qnode2);
			}
			else if (_kLexema == KLex.lexemaLess)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knLess, qnode1, qnode2);
			}
			else if (_kLexema == KLex.lexemaLessOrEqual)
			{
				Next ();
				qnode2 = ProcessLevelFactor (); // One level down
				return CQueryNode.NewOp (KNode.knLessOrEqual, qnode1, qnode2);
			}
			else return qnode1;
		}

		// ProcessLevelFactor (eat lowest level, including braces)
		CQueryNode ProcessLevelFactor ()
		{
			CQueryNode qnode;

			if (_kLexema == KLex.lexemaStringValue)
			{
				qnode = CQueryNode.NewString (_strCurrentValue);
				Next ();
			}
			else if (_kLexema == KLex.lexemaColumnName)
			{
				qnode = CQueryNode.NewColumn (_iCurrentColumn);
				Next ();
			}
			else if (_kLexema == KLex.lexemaNot)
			{
				Next ();
				qnode = ProcessLevelFactor ();
				qnode = CQueryNode.NewOp (KNode.knNot, qnode, null);
			}
			else if (_kLexema == KLex.lexemaUnicode)
			{
				Next ();

				if (_kLexema != KLex.lexemaOpenBrace) SyntaxError ("Expected: open brace");

				Next ();

				qnode = ProcessLevelOr ();
				qnode = CQueryNode.NewOp (KNode.knUnicode, qnode, null);

				if (_kLexema != KLex.lexemaCloseBrace) SyntaxError ("Expected: closing brace");

				Next ();
			}
			else if (_kLexema == KLex.lexemaOpenBrace)
			{
				Next ();
				qnode = ProcessLevelOr ();

				if (_kLexema != KLex.lexemaCloseBrace) SyntaxError ("Expected: closing brace");

				Next ();
			}
			else
			{
				SyntaxError ("Unexpected symbol, expected: value, open brace");
				qnode = null;
			};

			return qnode;
		}

		// Next - lexical analizer
		void Next ()
		{
			/* Skip blanks */
			while (_strzQuery [_cp] == ' ') _cp++;

			if (_strzQuery [_cp] == KCharConst.zero)
			{
				_kLexema = KLex.lexemaEnd;
			}
			else if (_strzQuery [_cp] == '&')
			{
				_cp ++; 
				if (_strzQuery [_cp] == '&') _cp++;
				_kLexema = KLex.lexemaAnd;
			}
			else if (_strzQuery [_cp] == '~')
			{
				_cp ++; 
				_kLexema = KLex.lexemaContains;
			}
			else if (_strzQuery [_cp] == '|')
			{
				_cp++;
				if (_strzQuery [_cp] == '|') _cp++;
				_kLexema = KLex.lexemaOr;
			}
			else if (_strzQuery [_cp] == '(')
			{
				_cp++;
				_kLexema = KLex.lexemaOpenBrace;
			}
			else if (_strzQuery [_cp] == ')')
			{
				_cp++;
				_kLexema = KLex.lexemaCloseBrace;
			}
			else if (_strzQuery [_cp] == '=')
			{
				_cp++;
				if (_strzQuery [_cp] == '=') _cp++;
				_kLexema = KLex.lexemaEqual;
			}
			else if (_strzQuery [_cp] == '!' && _strzQuery [_cp+1] == '=')
			{
				_cp += 2;
				_kLexema = KLex.lexemaNotEqual;
			}
			else if (_strzQuery [_cp] == '!' && _strzQuery [_cp+1] == '~')
			{
				_cp += 2;
				_kLexema = KLex.lexemaNotContains;
			}
			else if (_strzQuery [_cp] == '!' && _strzQuery [_cp+1] == '<' && _strzQuery [_cp+2] == '<')
			{
				_cp += 3;
				_kLexema = KLex.lexemaNotContains;
			}
			else if (_strzQuery [_cp] == '!')
			{
				_cp++;
				_kLexema = KLex.lexemaNot;
			}
			else if (_strzQuery [_cp] == '<' && _strzQuery [_cp+1] == '=')
			{
				_cp += 2;
				_kLexema = KLex.lexemaLessOrEqual;
			}
			else if (_strzQuery [_cp] == '<' && _strzQuery [_cp+1] == '<')
			{
				_cp += 2;
				_kLexema = KLex.lexemaContains;
			}
			else if (_strzQuery [_cp] == '<')
			{
				_cp++;
				_kLexema = KLex.lexemaLess;
			}
			else if (_strzQuery [_cp] == '>' && _strzQuery [_cp+1] == '=')
			{
				_cp += 2;
				_kLexema = KLex.lexemaGreaterOrEqual;
			}
			else if (_strzQuery [_cp] == '>')
			{
				_cp++;
				_kLexema = KLex.lexemaGreater;
			}
			else if (_strzQuery [_cp] >= '0' && _strzQuery [_cp] <= '9' || _strzQuery [_cp] == '-')
			{
				int cpEnd = _cp + 1;

				if (_strzQuery [_cp] == '-')
				{
					if (! (_strzQuery [_cp+1] >= '0' && _strzQuery [_cp+1] <= '9')) SyntaxError ("Expected: number");
					cpEnd = _cp + 2;
				};

				while (_strzQuery [cpEnd] >= '0' && _strzQuery [cpEnd] <= '9') cpEnd++;

				_kLexema = KLex.lexemaStringValue;
				_strCurrentValue = _strzQuery.Substring (_cp, cpEnd - _cp);

				_cp = cpEnd;
			}

			else if (FIdentifierFirstChar (_strzQuery [_cp]))
			{
				/* Indentified - must be column name */
				int cpEnd = _cp + 1;
				int ic;
				string strName;

				while (FIdentifierMiddleChar (_strzQuery [cpEnd])) cpEnd++;

				strName = _strzQuery.Substring (_cp, cpEnd - _cp);

				/* Check for special names */
				if (string.Compare (strName, "FUnicode", true) == 0)
				{
					_kLexema = KLex.lexemaUnicode;
					_cp = cpEnd;
				}
				else
				{
					/* Must be column name */
					ic = 0;
					while (ic < _rgstrColumnNames.Length)
					{
						if (string.Compare (strName, _rgstrColumnNames [ic], true) == 0) break;
						ic++;
					}

					if (ic == _rgstrColumnNames.Length) SyntaxError (strName + " is not a valid column name");

					_iCurrentColumn = ic;
					_kLexema = KLex.lexemaColumnName;
				}

				_cp = cpEnd;
			}
			else if (_strzQuery [_cp] == '"')
			{
				/* Just a search string */
				int cpEnd = _cp + 1;

				while (_strzQuery [cpEnd] != '"' && _strzQuery [cpEnd] != KCharConst.zero) cpEnd++;

				if (_strzQuery [cpEnd] == KCharConst.zero) SyntaxError ("Expected: closing quote \"");

				_strCurrentValue = _strzQuery.Substring (_cp + 1, cpEnd - _cp - 1);
				_kLexema = KLex.lexemaStringValue;

				_cp = cpEnd + 1;
			}
			else
			{
				SyntaxError ("Unexpected sympbol: " + _strzQuery [_cp]);
			}
		}

		// FIdentifierFirstChar
		static bool FIdentifierFirstChar (char chr) // Review: Use characters from real column names?
		{
			return chr >= 'a' && chr <= 'z' ||
				   chr >= 'A' && chr <= 'Z';
		}
		
		// FIdentifierMiddleChar
		static bool FIdentifierMiddleChar (char chr) // Review: Use characters from real column names?
		{
			return chr >= 'a' && chr <= 'z' ||
				chr >= 'A' && chr <= 'Z' ||
				chr >= '0' && chr <= '9';
		}

		// SyntaxError
		void SyntaxError (string strMessage)
		{
			_strErrorMessage = strMessage + " ( cp = " + _cp.ToString () + " )";
			throw new CompilerException ();
		}

	}

	//CQueryNode class
	class CQueryNode
	{
		public KNode kn;

		public CQueryNode qnode1; // First opearand
		public CQueryNode qnode2; // Second operant (only if binary)

		public string str;				// Only KNode.String

		public int icolumn;				// Only KNode.Column

		// A bunch of static constructors for the node

		public static CQueryNode NewString (string str)
		{
			CQueryNode qnode = new CQueryNode ();

			qnode.kn = KNode.knString;
			qnode.qnode1 = null;
			qnode.qnode2 = null;
			qnode.str = str;
			return qnode;
		}

		public static CQueryNode NewColumn (int icolumn)
		{
			CQueryNode qnode = new CQueryNode ();
			qnode.kn = KNode.knColumn;
			qnode.qnode1 = null;
			qnode.qnode2 = null;
			qnode.icolumn = icolumn;
			return qnode;
		}

		public static CQueryNode NewOp (KNode kn, CQueryNode qnode1, CQueryNode qnode2)
		{
			Common.Assert (kn != KNode.knString && kn != KNode.knColumn);

			CQueryNode qnode = new CQueryNode ();
			qnode.kn = kn;
			qnode.qnode1 = qnode1;
			qnode.qnode2 = qnode2;
			return qnode;
		}


	}

	// Kind of node
	enum KNode
	{
		knString,		// leaf
		knColumn,		// leaf
		knNot,			// unary
		knUnicode,		// unary
		knEqual,		// binary
		knOr,			// binary
		knContains,		// binary
		knAnd,			// binary
		knLess,			// binary
		knLessOrEqual,	// binary
		knGreater,		// binary
		knGreaterOrEqual// binary
	}

	// Kind of lexema
	enum KLex
	{
		lexemaStringValue,
		lexemaColumnName,
		lexemaAnd,
		lexemaOr,
		lexemaNot,
		lexemaEqual,
		lexemaUnicode,
		lexemaNotEqual,
		lexemaContains,
		lexemaNotContains,
		lexemaOpenBrace,
		lexemaCloseBrace,
		lexemaLess,
		lexemaLessOrEqual,
		lexemaGreater,
		lexemaGreaterOrEqual,
		lexemaEnd
	}

	// CompilerException
	public class CompilerException : Exception  {}; // Internal compiler error


	// CompilerException
	public class QueryException : Exception  {}; // Internal compiler error
}