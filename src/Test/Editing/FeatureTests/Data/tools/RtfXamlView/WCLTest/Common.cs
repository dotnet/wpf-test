// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Common
{
	/* Constants, enmus, structures */

   //Ripped from RTFXamlView
   public enum rxCommands
   {
      rxOpenFile = 0,
      rxRTRtf = 1,
      rxRTXaml = 2,
      rxRTRTFXAML = 3,
      rxRTREAssert = 4,
      rxBVTDir = 5,
      rxFileList = 6,
      rxNumFiles = 7,
      rxIndex = 8,
      rxLogFile = 9
   };

	class KCharConst // Some character constants
	{
		public static char eoln1 = '\x0D';
		public static char eoln2 = '\x0A';
		public static char tab = '\x09';
		public static char zero = '\x00';


		public static bool FIsCharSpaceEolnTab (char ch)
		{ return (ch == ' ' || ch == eoln1 || ch == eoln2 || ch== tab);}
	};	
	
	class KTarget // Build target (debug or ship)
	{			 
		// Review: I do not use enum, because it is not convenient with array indexes...
		public static int Debug = 0; 
		public static int Ship = 1; 
		public static int Range =2;

		public static string ToDebugShipString (int target)
		{
			return (target == KTarget.Debug ? "debug" : "ship");
		}

		public static int FromDebugShipString (string s)
		{
			s = s.ToLower ();
			if (s == "debug") return Debug;
			else if (s == "ship") return Ship;
			else return -1;
		}
	};

	class KLayoutOut // Kind of layout output for test
	{
		public static int Compare = 0;
		public static int Save = 1;
		public static int Disable = 2;

		public static string ToPresentationString (int k)
		{
			if (k==Compare) return "Save & compare";
			else if (k==Save) return "Save only";
			else if (k==Disable) return "Disable";
			else return "BAAAAD DAAATA";
		}

	}

	class KTestStatus
	{
		public static int Unknown = 0;
		public static int OK = 1;
		public static int Failed = 2;
		public static int Running = 3;
		public static int Terminated = 4;

		public static string ToPresentationString (int k)
		{
			if (k == Unknown) return "N/A";
			else if (k == OK) return "COMPLETED";
			else if (k == Failed) return "FAILED";
			else if (k == Running) return "RUNNING";
			else if (k == Terminated) return "TERMINATED";
			else return "BAAAAD DAAATA";
		}
	}

	class KWord
	{
		public static int w11 = 11;
		public static int w12 = 12;

		public static string ToPresentationString (int kword) {return kword.ToString ();}
		public static int FromPresentationString (string s)
		{
			s = s.ToLower ();
			if (s == "11") return w11;
			else if (s == "12") return w12;
			else return -1;
		}
			
	}

	class KPage
	{
		public static int GoodRegular = 0;
		public static int GoodOptimal = 1;
		public static int GoodBestFit = 2;
		public static int Word = 3;
		public static int WordCompareLrs = 4;
		public static int Old = 5; /* For running in the old rtm build */
		public static int Range = 6;

		public static string ToPresentationString (int kPage)
		{
			if (kPage == Word) return "Word";
			else if (kPage == WordCompareLrs) return "Word.CompareLrs"; 
			else if (kPage == GoodRegular) return "Good.Regular"; 
			else if (kPage == GoodOptimal) return "Good.Optimal"; 
			else if (kPage == GoodBestFit) return "Good.BestFit";
			else if (kPage == Old) return "Old";
			else
				Common.Assert (false, "Wrong kPage"); 
			return "";
		}

		public static int FromPresentationString (string strPage)
		{
			string strHelp = strPage.ToUpper ();

			if (strHelp == "WORD") return Word;
			if (strHelp == "WORD.COMPARELRS") return WordCompareLrs;
			else if (strHelp == "GOOD.REGULAR") return GoodRegular;
			else if (strHelp == "GOOD.OPTIMAL") return GoodOptimal;
			else if (strHelp == "GOOD.BESTFIT") return GoodBestFit;
			else if (strHelp == "OLD") return Old;
			else return -1;
		}

	} // End of KPage




	class KRunningTask
	{
		public static int Word = 0;
		public static int Tests = 1;
		public static int Build = 2;
		public static int TestList = 3;

		public static string ToPresentationString (int k)
		{
			if (k == Word) return "Word";
			else if (k == Tests) return "Tests";
			else if (k == Build) return "Build";
			else if (k == TestList) return "TestList";
			else return "BAAAAD DAAATA";
		}

	}

	class KBuildSuccess // Result of the build
	{
		public static int NotAvailable = 0;
		public static int Successful = 1;
		public static int Errors = 2;
		public static int Warnings = 3;
		public static int Running = 4;
		public static int Terminated = 5;
		public static int Range = 6;

		public static string ToPresentationString (int br)
		{
			if (br == NotAvailable) return "N/A";
			else if (br == Successful) return "SUCCESSFUL";
			else if (br == Errors)return "ERRORS";
			else if (br == Warnings)return "WARNINGS";
			else if (br == Running) return "RUNNING";
			else if (br == Terminated) return "TERMINATED";
			else return "BAAAAD DAAATA";
		}
	}

	/*************/
	/*			 */
	/* KLanguage */		// Kind of language
	/*			 */
	/*************/

	class KLanguage
	{
		public static int English = 0;
		public static int Japanese = 1;
		public static int Arabic = 2;
		public static int Other = 3; /* For all other languages */
		public static int Range = 4;

		public static string ToPresentationString (int kLanguage)
		{
			switch (kLanguage)
			{
			case 0: return "English";
			case 1: return "Japanese"; 
			case 2: return "Arabic"; 
			case 3: return "Other";
			default:
				Common.Assert (false, "Wrong kLanguage"); 
				return "";
			};
		}

		public static int FromPresentationString (string strLanguage)
		{
			string strHelp = strLanguage.ToUpper ();

			if (strHelp == "ENGLISH") return English;
			else if (strHelp == "JAPANESE") return Japanese;
			else if (strHelp == "ARABIC") return Arabic;
			else if (strHelp == "OTHER") return Other;
			else return -1;
		}

	} // End of KLanguage


	/*******************/
	/*				   */
	/* Class BuildInfo */
	/*		  	       */
	/*******************/

	// Debug or ship build information
	struct SBuildInfo 
	{
		public bool FInCurrentSession;	// If build was attempted during current session of W11Builder
		public DateTime DTime;			// Time of the last build
		public int Success;				// How last build was successful (BuildSuccess)
		public string StrConfigName;	// Name of configuration
	};

	/******************/
	/*				  */
	/* Error handling */
	/*				  */
	/******************/

	class W11Exception: Exception  {}; // Internal W11 error
	// Only to distinguish it form other exceptions

	class W11Messages // Static class for errors and information messages
	{
		static string s_strErrorMsg = "******************\n" +
									"   E R R O R\n" +
									"******************\n\n";

		static string s_strMessageHeader = "W11 Builder                            ";
		static System.Windows.Forms.Form s_mainForm = null;

		public static void Initialize (System.Windows.Forms.Form form)
		{
			// MainForm = form;
			s_mainForm = null;
		}

		public static void RaiseError ()
		{
			throw new W11Exception ();
		}

		public static void RaiseError (string msg)
		{
			MessageBox.Show ( s_mainForm, s_strErrorMsg + msg, s_strMessageHeader, MessageBoxButtons.OK, MessageBoxIcon.Error);
			throw new W11Exception ();
		}

		public static void RaiseOpenFileError (string sfn)
		{ RaiseError ("Unable to open file " + sfn + "\nMake it not in use by another application");}

		public static void ShowError (string msg)
		{ MessageBox.Show ( s_mainForm, s_strErrorMsg + msg, s_strMessageHeader, MessageBoxButtons.OK, MessageBoxIcon.Error); }

		public static void ShowWarning (string msg)
		{ MessageBox.Show ( s_mainForm, msg, s_strMessageHeader, MessageBoxButtons.OK, MessageBoxIcon.Warning); }

		public static void ShowMessage (string msg)
		{ MessageBox.Show ( s_mainForm, msg, s_strMessageHeader, MessageBoxButtons.OK, MessageBoxIcon.Information); }

		public static void ShowExceptionError (Exception ex)
		{ ShowError ("Unhandled exception: " + ex.ToString ()); }
	}

	/* Service (state-less) functions */	

	class Common // Static class with service functions
	{
		public static Random random = new Random ();

		// Assert 
		public static void Assert (bool f, string str)
		{
			if (!f) 
			{
				//				W11Messages.ShowError ( "Internal error (Assert) " + (str != "" ? ": " + str : "") + 
				//					"\n\nThe program will now terminate");

				Debug.Assert (f, str);
			
				//				Process.GetCurrentProcess ().Kill ();
			};
		}

		public static void Assert (bool f)
		{
			if (!f) Assert (f, "");
		}

		// FCompareStr
		// Compares strings without case
		public static int CompareStr (string s1, string s2)
		{
			return string.Compare (s1, s2, true);
		}

		// PackMultiLineString 
		// Compares strings without case
		public static string PackMultiLineString (string strM)
		{
			string strComp = "";;
			int i;

			for (i=0; i < strM.Length; i++)
			{
				if (strM [i] == KCharConst.eoln1) strComp += "^D";
				else if (strM [i] == KCharConst.eoln2) strComp += "^A";
				else strComp += strM [i];
			};

			return strComp;
		}


		// PackMultiLineString 
		// Compares strings without case
		public static string UnpackMultiLineString (string strComp)
		{
			string strM = "";;
			int i;

			for (i=0; i < strComp.Length; i++)
			{
				if (strComp [i] == '^' && i + 1 < strComp.Length && strComp [i+1] == 'D')
					strM += KCharConst.eoln1;
				else if (strComp [i] == '^' && i + 1 < strComp.Length && strComp [i+1] == 'A')
					strM += KCharConst.eoln2;
				else 
					strM += strComp [i];
			};

			return strM;
		}


		// Reads names of subfolders
		public static string [] ReadDirectotyNames (string spath, string sfnwildcard)
		{
			long i;

			string [] rgspathSub;

			if (!Directory.Exists (spath))
			{
				return new string [0];
			};

			rgspathSub = Directory.GetDirectories (spath, sfnwildcard);

			for (i=0; i < rgspathSub.Length; i++)
			{
				rgspathSub [i] = Path.GetFileName (rgspathSub [i]);
			}

			return rgspathSub;
		}

		// Reads names of files
		public static string [] ReadFileNames (string spath, string sfnwildcard)
		{
			long i;

			string [] rgsfn;

			if (!Directory.Exists (spath))
			{
				return new string [0];
			};

			rgsfn = Directory.GetFiles (spath, sfnwildcard);

			for (i=0; i < rgsfn.Length; i++)
			{
				rgsfn [i] = Path.GetFileName (rgsfn [i]);
			}

			return rgsfn;
		}

		// Delete folder, handle error
		static public void DeleteFolder (string spath)
		{
			try
			{
				if (Directory.Exists (spath))
				{
					Directory.Delete (spath, true);
				};
			}
			catch (Exception ex)
			{
				W11Messages.RaiseError ("Unable to delete folder " + spath +
					"\nProbably it is used by another process or has read-only files" +
					"\n\n" + ex.ToString () );
			}
		}


		// Delete folder, handle error
		static public void RenameFolder (string pathold, string pathnew)
		{
			try
			{
				Directory.Move (pathold, pathnew);
			}
			catch (Exception ex)
			{
				W11Messages.RaiseError ("Unable to rename folder " + pathold + " => " + pathnew +
					"\nProbably the folder does not exist or it is used by another process" +
					"\n\n" + ex.ToString () );
			}
		}


		// Reads given number of lines from stream
		public static string [] ReadLinesFromStream (StreamReader stream, long nlines)
		{
			int i = 0;
			string [] arrlines = new string [nlines];

			while (i < nlines && stream.Peek () != -1)
			{
				arrlines [i] = stream.ReadLine ();
				i++;
			}

			if (i == nlines) return arrlines;
			else
			{
				string [] arrlinesT = new string [i];
				int j;
				for (j=0; j < i; j++) arrlinesT [j] = arrlines [j];

				return arrlinesT;
			};
		}

		// Split string by given character (private)
		public static bool FSplitString (string s, char chr, out string s1, out string s2)
		{
			int i = 0;
			while (i < s.Length && s [i] != chr) i++;
			s1 = null;
			s2 = null;
			if (i == s.Length) return false;
			else
			{
				s1 = s.Substring (0, i);
				s2 = s.Substring (i+1, s.Length - i - 1);
				return true;
			}
		}

		// GetTemporaryDocumentName
		public static string GetTemporaryDocumentName (string spathDoc)
		{
			string strDocnameTemp;
			int index = random.Next ();

			do
			{
				strDocnameTemp = "~$-DBC-" + index.ToString () + ".doc";
				index ++;
			} while (File.Exists (Path.Combine (spathDoc, strDocnameTemp)));

			return strDocnameTemp;
		}

		// FTemporaryDocument
		public static bool FTemporaryDocument (string sfnDoc)
		{
			string strFileName = Path.GetFileName (sfnDoc);

			return (strFileName.Length >= 2 && strFileName [0] == '~' && strFileName [1] == '$');
		}

		// StrToInt with error check, but no check
		// for overflow
		public static bool ParseStrToInt (string s, out int lout)
		{
			int ich = 0;
			int len = s.Length;
			int lresult;
			bool fNegative = false;

			lout = System.Int32.MaxValue;

			/* Skip tabs and white spaces */
			while (ich < len && (s[ich] == ' ' || s [ich] == 9)) ich++;

			/* Must have content */
			if (ich == len) return false;

			if (s [ich] == '-') 
			{
				fNegative = true;
				ich++;
			};

			/* Number must follow */
			if (ich == len || s [ich] < '0' || s [ich] > '9') return false;

			lresult = 0;

			while (ich < len && (s [ich] >= '0' && s [ich] <= '9'))
			{
				lresult = lresult * 10 + (s [ich] - '0');
				ich++;
			};

			/* Skip tabs and white spaces */
			while (ich < len && (s[ich] == ' ' || s [ich] == 9)) ich++;

			/* Must be in the end */
			if (ich != len) return false;

			if (fNegative) lout = -lresult;
			else lout = lresult;

			return true;
		}

		// SortStringArray
		public static void SortStringArray (string [] rgstr)
		{
			int n = rgstr.Length;
			int step = n / 2;

			while (step > 0)
			{
				bool fContinue = true;

				while (fContinue)
				{	
					int i = 0;
					fContinue = false;
					while (i + step < n)
					{
						if (Common.CompareStr (rgstr [i], rgstr [i+step]) > 0)
						{
							string help = rgstr [i];
							rgstr [i] = rgstr [i+ step];
							rgstr [i+ step] = help;

							fContinue = true;
						}

						i = i + step;
					};
				}
				step = step / 2;
			}
		}

		// Find if string contains special uncode character 
		static public bool FContainsUnicodeCharacter (string s)
		{
			for (int i=0; i < s.Length; i++)
			{
				if ( (int) s[i] > 0xFF) return true;
			}

			return false;
		}


		// Finds string in given array (not sorted); 
		// returns -1 if such string is not found
		public static int FindStringInArray (string [] rgstrArray, string strKey)
		{
			int i = 0;
			while (i < rgstrArray.Length && Common.CompareStr (strKey, rgstrArray [i]) != 0) 
			{
				i++;
			};

			if (i == rgstrArray.Length) return -1;
			else return i;
		}

	} /* End of Service */

}
