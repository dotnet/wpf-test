// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Layout
{
	using Common;

	// Layout - Service for xml layout files
	class Layout
	{
		static string s_strCompareErrorReport;	// Error report 
		static int s_cpPrev;						// Previous found cp
		static string s_strPageParamPrev;			// Previous found ipgd
		static CLayoutXML [] s_rgLayoutXml;		// Array of two layout xml files
		static string [] s_rgstrFormatPage;		// If this layout file corresponds to Word format page
		static string [] s_rgstrLsVersion;		// LineServices version for this layout file
		static bool s_fIgnoreJusitficationYl;		// Ignore yl comparision in case of justification
		static bool s_fIgnoreNestedPositions;		// Ignore all x & y in nested layouts (inside tables, frames, etc.)
		static bool s_fFuzzyDupCompare;			// Fuzzy compare of presentation dups (allow few pix error)
		static int s_kCompare;					// Compare only page breaks

		static int s_iContentLevel;				// Nested level
		static bool s_fJustificationYlIgnored;	// If Yl was ignored due to vertical justification
		static bool s_fFootnotePositionIgnored;	// If footnote position was ignored
		static bool s_fEndnoteSeparatorIgnored;	// If endnote separator ihdt & cps were ignored
		static bool s_fDropcapHeightIgnored;		// Height of dropcaps is different, ignored
		static bool s_fFigurePositionIgnored;		// Figure position different, ignored
		static bool s_fLineBtwColumnsIgnored;		// Line between columns different, ignored

		static public string strW11Exception;			
												// Exception occured during compare

		static bool FIgnorePositions ()
		{
			return s_fIgnoreNestedPositions && s_iContentLevel > 1;
		}

		// AdvanceStringValue 
		// Parse string value
		static string AdvanceStringValue (int ixml, string strTag)
		{
			string strValue;
			CLayoutXML layoutxml = s_rgLayoutXml [ixml];

			if (layoutxml.CurToken != KToken.TagOpen || layoutxml.StrTag != strTag) CreateReportTagMatch ();
			layoutxml.Next ();

			if (layoutxml.CurToken != KToken.Value) CreateReportTagMatch ();
			strValue = layoutxml.StrValue;
			layoutxml.Next ();

			if (layoutxml.CurToken != KToken.TagClose || layoutxml.StrTag != strTag)  CreateReportTagMatch ();
			layoutxml.Next ();

			return strValue;
		}

		// AdvanceIntValue 
		// Parse integer value
		static int AdvanceIntValue (int ixml, string strTag)
		{
			string strValue;
			int ivalue;

			ivalue = 0;
			strValue = AdvanceStringValue (ixml, strTag);

			if (!Common.ParseStrToInt (strValue, out ivalue)) CreateReportTagMatch ();

			return ivalue;
		}

		// AdvanceLine 
		// Parse one line
		static void AdvanceLine ( int ixml, out int cp, out  int cpL, out  int yl, out int xl, 
								  out  int dyl, out int dxl, out string ihdt, out int upPropL,
								  out  int upLineL, out string endr)
		{
			AdvanceTag (ixml, KToken.TagOpen, "line");

			cp = AdvanceIntValue      (ixml, "cp");
			cpL = AdvanceIntValue     (ixml, "cpL");
			yl = AdvanceIntValue      (ixml, "yl");
			xl = AdvanceIntValue      (ixml, "xl");
			dyl= AdvanceIntValue      (ixml, "dyl");
			dxl = AdvanceIntValue     (ixml, "dxl");
			ihdt = AdvanceStringValue (ixml, "ihdt");
			upPropL = AdvanceIntValue (ixml, "upPropL");
			upLineL = AdvanceIntValue (ixml, "upLineL");
			endr = AdvanceStringValue (ixml, "endr");

			AdvanceTag (ixml, KToken.TagClose, "line");
		}

		// AdvanceLrdo
		// Parse one lrdo
		static void AdvanceLrdo (int ixml, out int cp, out int xlFull, out int ylFull, out int xpLeft,
			out int ypTop, out int xpRight, out int ypBottom, out string ihdt, 
			out int fIgnorePosition)
		{
			AdvanceTag (ixml, KToken.TagOpen, "lrdo");

			cp = AdvanceIntValue         (ixml, "cp");
			xlFull = AdvanceIntValue     (ixml, "xlFull");
			ylFull = AdvanceIntValue     (ixml, "ylFull");
			xpLeft = AdvanceIntValue     (ixml, "xpLeft");
			ypTop = AdvanceIntValue      (ixml, "ypTop");
			xpRight = AdvanceIntValue    (ixml, "xpRight");
			ypBottom = AdvanceIntValue   (ixml, "ypBottom");
			ihdt = AdvanceStringValue    (ixml, "ihdt");
			fIgnorePosition = AdvanceIntValue
										 (ixml, "fIgnorePosition");

			AdvanceTag (ixml, KToken.TagClose, "lrdo");
		}

		// AdvanceLrg 
		// Parse one lrg or line between columns
		static void AdvanceLrg (int ixml, string strTag, out int x, out int y, out int dx, out int dy)
		{
			AdvanceTag (ixml, KToken.TagOpen, strTag);

			x = AdvanceIntValue			(ixml, "xl");
			y = AdvanceIntValue			(ixml, "yl");
			dx = AdvanceIntValue		(ixml, "dxl");
			dy = AdvanceIntValue		(ixml, "dyl");

			AdvanceTag (ixml, KToken.TagClose, strTag);
		}

		// AdvanceDropap
		// Parse one dropcap: cp, rectangle, but stop where content starts
		static void AdvanceDropcapHeader ( int ixml, out int cp, out  int cpL, out  int yl, out int xl, 
			out  int dyl, out int dxl, out string ihdt)
		{
			CLayoutXML layoutxml = s_rgLayoutXml [ixml];

			AdvanceTag (ixml, KToken.TagOpen, "dropcap");

			cp = AdvanceIntValue      (ixml, "cp");
			cpL = AdvanceIntValue     (ixml, "cpL");
			yl = AdvanceIntValue      (ixml, "yl");
			xl = AdvanceIntValue      (ixml, "xl");
			dyl= AdvanceIntValue      (ixml, "dyl");
			dxl = AdvanceIntValue     (ixml, "dxl");
			ihdt = AdvanceStringValue (ixml, "ihdt");
		}


		// Varuious compare functions
		static void CompareValuesFuzzy (string strTag, int iv1, int iv2, int diAllowance)
		{
			if (Math.Abs (iv1-iv2) > diAllowance) CreateReportValuesMatch (strTag, iv1.ToString (), iv2.ToString ());
		}

		static void CompareValues (string strTag, int iv1, int iv2)
		{
			if (iv1 != iv2) CreateReportValuesMatch (strTag, iv1.ToString (), iv2.ToString ());
		}

		static void CompareValues (string strTag, string strv1, string strv2)
		{
			if (strv1 != strv2) CreateReportValuesMatch (strTag, strv1, strv2);
		}

		static void CompareLineBreakValues (string strTag, string strv1, string strv2)
		{
			if (strv1 != strv2) CreateReportLineBreakValuesMatch (strTag, strv1, strv2);
		}

		static void CompareLineBreakValues (string strTag, int iv1, int iv2)
		{
			if (iv1 != iv2) CreateReportLineBreakValuesMatch (strTag, iv1.ToString (), iv2.ToString ());
		}

		// ProcessTags
		// Advance on open/clos tag
		static void AdvanceTag (int ixml, int kTokenTag, string strTag)
		{
			if (s_rgLayoutXml [ixml].CurToken != kTokenTag || s_rgLayoutXml [ixml].StrTag != strTag)
			{
				CreateReportTagMatch ();
			}
			s_rgLayoutXml [ixml].Next ();
		}

		// ProcessDropcaps
		// Parse and compare two dropcaps
		static void ProcessDropcaps ()
		{
			int cp1, cpL1, yl1, xl1, dyl1, dxl1;
			int cp2, cpL2, yl2, xl2, dyl2, dxl2;
			string ihdt1;
			string ihdt2;

			AdvanceDropcapHeader (0, out cp1, out cpL1, out yl1, out xl1, out dyl1, out dxl1, out ihdt1);
			AdvanceDropcapHeader (1, out cp2, out cpL2, out yl2, out xl2, out dyl2, out dxl2, out ihdt2);

			CompareValues ("cp",  cp1,  cp2);
			CompareValues ("cpL", cpL1, cpL2);

			if (yl1 != yl2 && s_fIgnoreJusitficationYl)
			{
				s_fJustificationYlIgnored = true;
			}
			else if (!FIgnorePositions ()) CompareValues ("yl", yl1, yl2);

			if (!FIgnorePositions ()) CompareValues ("xl",  xl1,  xl2);
			CompareValues ("dxl", dxl1, dxl2);

			// We do not compare dropcap dyls for different formatpages
			if (s_rgstrFormatPage [0] == s_rgstrFormatPage [1])
			{
				CompareValues ("dyl", dyl1, dyl2);
			}
			else if (dyl1 != dyl2)
			{
				s_fDropcapHeightIgnored = true;
			}

			// Process content recursively, do not compare
			ProcessCompareSkip ();

			// Close dropcaps
			AdvanceTag (0, KToken.TagClose, "dropcap");
			AdvanceTag (1, KToken.TagClose, "dropcap");
		}


		// ProcessLines 
		// Parse and compare two lines
		static void ProcessLines ()
		{
			bool fIgnoreSeparatorDifference = false;
			bool fIgnoreFootnoteYl = false;
			int cp1, cpL1, yl1, xl1, dyl1, dxl1, upPropL1, upLineL1;
			int cp2, cpL2, yl2, xl2, dyl2, dxl2, upPropL2, upLineL2;

			string ihdt1, endr1;
			string ihdt2, endr2;

			AdvanceLine (0, out cp1, out cpL1, out yl1, out xl1, out dyl1, out dxl1, out ihdt1, out upPropL1, out upLineL1, out endr1);
			AdvanceLine (1, out cp2, out cpL2, out yl2, out xl2, out dyl2, out dxl2, out ihdt2, out upPropL2, out upLineL2, out endr2);

			s_cpPrev = cp1;

			if (ihdt1 != ihdt2)
			{
				/* Check for endnote */
				if ( s_rgstrFormatPage [0] != s_rgstrFormatPage [1] && 
					(ihdt1 == "9" || ihdt1 == "10" || ihdt1 == "11") &&
					(ihdt2 == "9" || ihdt2 == "10" || ihdt2 == "11") )
				{
					fIgnoreSeparatorDifference = true;
					s_fEndnoteSeparatorIgnored = true;
				}
				else CompareLineBreakValues ("ihdt", ihdt1, ihdt2);
			}

			if ((ihdt1.Length > 0 && ihdt1 [ihdt1.Length-1] == 'F') || 
				(ihdt1 == "6" || ihdt1 == "7" || ihdt1 == "8"))
			{
				// This is a footnote or footnote separator line
				// We ignore footnote y-position when comparing between Word and Pts formatpage
				fIgnoreFootnoteYl = s_rgstrFormatPage [0] != s_rgstrFormatPage [1];
				}

			if (!fIgnoreSeparatorDifference) CompareLineBreakValues ("cp",  cp1,  cp2);
			if (!fIgnoreSeparatorDifference) CompareLineBreakValues ("cpL", cpL1, cpL2);

			if (s_kCompare == KCompare.All)
			{
				if (yl1 != yl2 && s_fIgnoreJusitficationYl)
				{
					s_fJustificationYlIgnored = true;
				}
				else if (yl1 != yl2 && fIgnoreFootnoteYl)
				{
					s_fFootnotePositionIgnored = true;
				}
				else if (!FIgnorePositions ()) CompareValues ("yl", yl1, yl2);

				if (!FIgnorePositions ()) CompareValues ("xl",  xl1,  xl2);

				CompareValues ("dyl", dyl1, dyl2);
				CompareValues ("dxl", dxl1, dxl2);

				if (!fIgnoreSeparatorDifference)
				{
					if ((upPropL1 != upPropL2 || upLineL1 != upLineL2) && s_fFuzzyDupCompare)
					{
						// Fuzzy => do not compare at all for now
						// CompareValuesFuzzy ("upPropL", upPropL1, upPropL2, 5);
						// CompareValuesFuzzy ("upLineL", upLineL1, upLineL2, 5);
					}
					else 
					{
						CompareValues ("upPropL", upPropL1, upPropL2);
						CompareValues ("upLineL", upLineL1, upLineL2);
					};

					CompareValues ("endr", endr1, endr2);
				}
			}
		}


		// ProcessLrdos
		// Parse and compare two lrdos
		static void ProcessLrdos ()
		{
			int cp1, xlFull1, ylFull1, xpLeft1, ypTop1, xpRight1, ypBottom1, fIgnorePosition1;
			int cp2, xlFull2, ylFull2, xpLeft2, ypTop2, xpRight2, ypBottom2, fIgnorePosition2;
			string ihdt1;
			string ihdt2;

			AdvanceLrdo (0, out cp1, out xlFull1, out ylFull1, out xpLeft1, out ypTop1, out xpRight1, out ypBottom1, out ihdt1, out fIgnorePosition1);
			AdvanceLrdo (1, out cp2, out xlFull2, out ylFull2, out xpLeft2, out ypTop2, out xpRight2, out ypBottom2, out ihdt2, out fIgnorePosition2);

			CompareValues ("cp", cp1, cp2);
			CompareValues ("ihdt", ihdt1, ihdt2);
			CompareValues ("fIgnorePosition", fIgnorePosition1, fIgnorePosition2);

			if (fIgnorePosition1 != 0)
			{
				if (xlFull1 != xlFull2 || (!s_fIgnoreJusitficationYl && ylFull1 != ylFull2) ||
					xpLeft1 != xpLeft2 || ypTop1 != ypTop2 || xpRight1 != xpRight2 ||
					(!s_fIgnoreJusitficationYl && ypBottom1 != ypBottom2))
				{
					s_fFigurePositionIgnored = true;
				}
			}
			else
			{
				// Full compare
				if (!FIgnorePositions ()) CompareValues ("xlFull", xlFull1, xlFull2);

				if (s_fIgnoreJusitficationYl)
				{
					s_fJustificationYlIgnored |= ylFull1 != ylFull2 || ypTop1 != ypTop2 || ypBottom1 != ypBottom2;
				}
				else if (!FIgnorePositions ())
				{
					CompareValues ("ylFull", ylFull1, ylFull2);
					CompareValues ("ypTop", ypTop1, ypTop2);
					CompareValues ("ypBottom", ypBottom1, ypBottom2);
				}

				if (!FIgnorePositions ())
				{
					CompareValues ("xpLeft", xpLeft1, xpLeft2);
					CompareValues ("xpRight", xpRight1, xpRight2);
				}
			}
		}

		// ProcessCompareSkip
		// Recursive process without compare
		static void ProcessCompareSkip ()
		{
			string strTagOpen;

			// Check if both tags are of the same kind
			if (s_rgLayoutXml [0].CurToken != s_rgLayoutXml [1].CurToken) CreateReportTagMatch ();

			while (s_rgLayoutXml [0].CurToken == KToken.TagOpen)
			{
				// Open tag
				strTagOpen = s_rgLayoutXml [0].StrTag;

				if (strTagOpen != s_rgLayoutXml [1].StrTag) CreateReportTagMatch ();

				// Check if we have values, otherwise process recursively
				// First, make sure tags are of the same kind

				s_rgLayoutXml [0].Next ();
				s_rgLayoutXml [1].Next ();

				if (s_rgLayoutXml [0].CurToken != s_rgLayoutXml [1].CurToken) CreateReportTagMatch ();

				if (s_rgLayoutXml [0].CurToken == KToken.Value)
				{
					// Advance to the closing tag
					s_rgLayoutXml [0].Next ();
					s_rgLayoutXml [1].Next ();
				}
				else
				{
					// Process content recursively
					ProcessCompareSkip ();
				};

				// Make sure we have closing tags
				AdvanceTag (0, KToken.TagClose, strTagOpen);
				AdvanceTag (1, KToken.TagClose, strTagOpen);
			}
		}

		// SkipLinesBetweenColumns
		static void SkipLinesBetweenColumns ()
		{
			while (s_rgLayoutXml [0].CurToken != s_rgLayoutXml [1].CurToken &&
				 ( s_rgLayoutXml [0].CurToken == KToken.TagOpen && s_rgLayoutXml [0].StrTag == "linebtwcolumns" ||
				   s_rgLayoutXml [1].CurToken == KToken.TagOpen && s_rgLayoutXml [1].StrTag == "linebtwcolumns"))
			{
				int x, y, dx, dy;

				s_fLineBtwColumnsIgnored = true;

				if (s_rgLayoutXml [0].CurToken == KToken.TagOpen && s_rgLayoutXml [0].StrTag == "linebtwcolumns")
				{
					AdvanceLrg (0, "linebtwcolumns", out x, out y, out dx, out dy);
				}
				else
				{
					AdvanceLrg (1, "linebtwcolumns", out x, out y, out dx, out dy);
				}
			}

			if (s_rgLayoutXml [0].CurToken == KToken.TagOpen && s_rgLayoutXml [0].StrTag == "linebtwcolumns" &&
				s_rgLayoutXml [1].CurToken == KToken.TagOpen && s_rgLayoutXml [1].StrTag == "linebtwcolumns")
			{
				int x1, y1, dx1, dy1;
				int x2, y2, dx2, dy2;

				AdvanceLrg (0, "linebtwcolumns", out x1, out y1, out dx1, out dy1);
				AdvanceLrg (1, "linebtwcolumns", out x2, out y2, out dx2, out dy2);

				if (x1 != x2 || y1 != y2 || dx1 != dx2 || dy1 != dy2)
				{
					s_fLineBtwColumnsIgnored = true;
				};
			}
		}

		// SkipLinesBetweenColumns
		static void ProcessPages ()
		{
			int cpLim0, cpLim1;
			int cpLimFtn0, cpLimFtn1;
			int ixml;
			string strParamPage0 = s_rgLayoutXml [0].StrTagParam;
			string strParamPage1 = s_rgLayoutXml [1].StrTagParam;

			s_strPageParamPrev = s_rgLayoutXml [0].StrTagParam;
	
			AdvanceTag (0, KToken.TagOpen, "page");
			AdvanceTag (1, KToken.TagOpen, "page");

			cpLim0 = AdvanceIntValue (0, "cpL");
			cpLim1 = AdvanceIntValue (1, "cpL");

			/* cp lim of footnote - optional */

			if (cpLim0 != cpLim1)
			{
				CreateCustomErrorReport ("Page break " + strParamPage1 + " cpLim: " + cpLim0.ToString () + " != " + cpLim1.ToString ());
			};

			if (s_rgLayoutXml [0].CurToken == KToken.TagOpen && s_rgLayoutXml [0].StrTag == "cpLFtn")
			{
				cpLimFtn0 = AdvanceIntValue (0, "cpLFtn");
				cpLimFtn1 = AdvanceIntValue (1, "cpLFtn");

				if (cpLimFtn0 != cpLimFtn1)
				{
					CreateCustomErrorReport ("Page break " + strParamPage1 + " cpLimFootnote: " + cpLimFtn0.ToString () + " != " + cpLimFtn1.ToString ());
				};
			}

			if (s_kCompare == KCompare.PageBreaks)
			{
				/* Skip everything till the page closing tag or line */
				for (ixml =0; ixml < 2; ixml++)
				{
					/* Page inside page is not possible, so just search for page closing tag */
					while (s_rgLayoutXml [ixml].CurToken != KToken.Eof && 
						( s_rgLayoutXml [ixml].CurToken != KToken.TagClose || 
						s_rgLayoutXml [ixml].StrTag != "page"))
					{
						s_rgLayoutXml [ixml].Next ();
					}
				};
			}
			else if (s_kCompare == KCompare.PageAndLineBreaks)
			{
				bool fFoundLines;

				do 
				{
					/* Skip everything till the page closing tag or line */
					for (ixml =0; ixml < 2; ixml++)
					{
						/* Page inside page is not possible, so just search for page closing tag */
						while (s_rgLayoutXml [ixml].CurToken != KToken.Eof && 
							( s_rgLayoutXml [ixml].CurToken != KToken.TagClose || 
							s_rgLayoutXml [ixml].StrTag != "page") && 
							( s_rgLayoutXml [ixml].CurToken != KToken.TagOpen || 
							s_rgLayoutXml [ixml].StrTag != "line"))
						{
							s_rgLayoutXml [ixml].Next ();
						}
					};

					fFoundLines = s_rgLayoutXml [0].CurToken == KToken.TagOpen && s_rgLayoutXml [0].StrTag == "line" ||
								  s_rgLayoutXml [1].CurToken == KToken.TagOpen && s_rgLayoutXml [1].StrTag == "line";

					if (fFoundLines) ProcessLines ();

				} while (fFoundLines);
			}
			else
			{
				ProcessCompare ();
			}

			AdvanceTag (0, KToken.TagClose, "page");
			AdvanceTag (1, KToken.TagClose, "page"); 
		}

		// SkipLinesBetweenColumns
		static void ProcessContent ()
		{
			AdvanceTag (0, KToken.TagOpen, "content");
			AdvanceTag (1, KToken.TagOpen, "content");

			s_iContentLevel++;

			ProcessCompare ();

			s_iContentLevel --;

			AdvanceTag (0, KToken.TagClose, "content");
			AdvanceTag (1, KToken.TagClose, "content"); 
		}

		// ProcessCompare 
		// Main compare routine, recursive parse 
		static void ProcessCompare ()
		{
			string strTagOpen;

			SkipLinesBetweenColumns ();

			// Check if both tags are of the same kind
			if (s_rgLayoutXml [0].CurToken != s_rgLayoutXml [1].CurToken) CreateReportTagMatch ();

			while (s_rgLayoutXml [0].CurToken == KToken.TagOpen)
			{
				// Open tag
				strTagOpen = s_rgLayoutXml [0].StrTag;

				if (strTagOpen != s_rgLayoutXml [1].StrTag) CreateReportTagMatch ();

				// Handle all special known tags

				if (strTagOpen == "page")
				{
					/* We are comparing only page breaks - call special function */
					ProcessPages ();
				}

				else if (strTagOpen == "line")
				{
					// LINE
					ProcessLines ();
				}
				else if (strTagOpen == "lrdo")
				{
					// FIGURE
					ProcessLrdos ();
				}
				else if (strTagOpen == "dropcap")
				{
					// DROP CAP
					ProcessDropcaps ();
				}
				else if (strTagOpen == "content")
				{
					// Recursive content
					ProcessContent ();
				}
				
				else if (strTagOpen == "LineFormatter")
				{
					// LINE FORMATTER (LS VERSION)
					s_rgstrLsVersion [0] = AdvanceStringValue (0, "LineFormatter");
					s_rgstrLsVersion [1] = AdvanceStringValue (1, "LineFormatter");
				}
				else if (strTagOpen == "PageFormatter")
				{
					// PAGE FORMATTER (PTS or WORD)
					s_rgstrFormatPage [0] = AdvanceStringValue (0, "PageFormatter");
					s_rgstrFormatPage [1] = AdvanceStringValue (1, "PageFormatter");
				}
				else if (strTagOpen == "WordVerticalJustification")
				{
					string strvjust0 = AdvanceStringValue (0, "WordVerticalJustification");
					string strvjust1 = AdvanceStringValue (1, "WordVerticalJustification");

					/* Do not compare yls if one document has old Word vertical justification and another - not */
					s_fIgnoreJusitficationYl = strvjust1 != strvjust0 && 
						s_rgstrFormatPage [0] != s_rgstrFormatPage [1];											
				}
				else if ( strTagOpen == "yl")
				{
					// Various yls: must ignore difference in vertical justification

					int yl0 = AdvanceIntValue (0, strTagOpen);
					int yl1 = AdvanceIntValue (1, strTagOpen);

					if (yl0 != yl1 && s_fIgnoreJusitficationYl)
					{
						// Remember that yl was ignore (for the output flag)
						s_fJustificationYlIgnored = true;
					}
					else if (!FIgnorePositions ()) CompareValues (strTagOpen, yl0, yl1);
				}
				else if ( strTagOpen == "xl")
				{
					int xl0 = AdvanceIntValue (0, strTagOpen);
					int xl1 = AdvanceIntValue (1, strTagOpen);
					if (!FIgnorePositions ()) CompareValues (strTagOpen, xl0, xl1);
				}

				else
				{
					// Check if we have values, otherwise process recursively
					// First, make sure tags are of the same kind

					s_rgLayoutXml [0].Next ();
					s_rgLayoutXml [1].Next ();

					if (s_rgLayoutXml [0].CurToken != s_rgLayoutXml [1].CurToken) CreateReportTagMatch ();

					if (s_rgLayoutXml [0].CurToken == KToken.Value)
					{
						// Advance to the next tag
						// Compare values
						CompareValues (strTagOpen, s_rgLayoutXml [0].StrValue, s_rgLayoutXml [1].StrValue);

						// Advance to the closing tag
						s_rgLayoutXml [0].Next ();
						s_rgLayoutXml [1].Next ();
					}
					else
					{
						// Process content recursively
						ProcessCompare ();
					};

					// Make sure we have closing tags
					AdvanceTag (0, KToken.TagClose, strTagOpen);
					AdvanceTag (1, KToken.TagClose, strTagOpen);
				}

				SkipLinesBetweenColumns ();
			}
		}

		// COMPARE
		// Fuzzy compare of two xml layout files 
		static public bool FCompare ( string fnxmlOne, string fnxmlTwo, bool fFuzzyDupCompare,
									  int kCompare, bool fIgnoreNestedPositions, out string strReport,
									  out string strFlags )
		{
			int kCompareCur;
			string strReportCur;
			string strFlagsCur;
			bool fEqualCur;
			bool fResultEqual;
			string strReportPrev;
			int kCompareFirstError;

			strReport = "";
			strFlags = "";
			strReportPrev = "";
			fResultEqual = true;
			kCompareFirstError = -1;

			for (kCompareCur = 0; kCompareCur <= kCompare; kCompareCur++)
			{
				fEqualCur = FCompareCore ( fnxmlOne, fnxmlTwo, fFuzzyDupCompare, kCompareCur,
										   fIgnoreNestedPositions, out strReportCur, out strFlagsCur );

				strFlags = strFlagsCur; // Flags are always from the last compare

				if (!fEqualCur)
				{
					fResultEqual = false;

					if (strReport == "")
					{
						strReport = strReportCur;
					}
					else if (strReportCur != strReportPrev)
					{
						strReport += " +++++ " + strReportCur;
					}

					if (kCompareFirstError == -1) kCompareFirstError = kCompareCur;
				};

				strReportPrev = strReportCur;
			};

			if (!fResultEqual)
			{
				strReport = "Pri " + kCompareFirstError.ToString () + " " + strReport;
			};

			return fResultEqual;

		}


		// COMPARE
		// Fuzzy compare of two xml layout files 
		static bool FCompareCore ( string fnxmlOne,
								   string fnxmlTwo,
								   bool fFuzzyDupCompare,
								   int kCompare,
								   bool fIgnoreNestedPositions,
								   out string strReport,
								   out string strFlags )
		{

			strW11Exception = "Unhandled";

			try 
			{
				CLayoutXML layoutxmlOne = new CLayoutXML (fnxmlOne);
				CLayoutXML layoutxmlTwo = new CLayoutXML (fnxmlTwo);
				bool fCompareResult;

				s_strCompareErrorReport = "";
				s_cpPrev = -1;
				s_strPageParamPrev = "";
				s_fIgnoreJusitficationYl = false;
				Layout.s_fFuzzyDupCompare = fFuzzyDupCompare;
				Layout.s_kCompare = kCompare;
				Layout.s_fIgnoreNestedPositions = fIgnoreNestedPositions;

				s_iContentLevel = 0;
				s_fJustificationYlIgnored = false;
				s_fFootnotePositionIgnored = false;
				s_fEndnoteSeparatorIgnored = false;
				s_fDropcapHeightIgnored = false;
				s_fFigurePositionIgnored = false;
				s_fLineBtwColumnsIgnored = false;

				s_rgLayoutXml = new CLayoutXML [2] { layoutxmlOne, layoutxmlTwo };
				s_rgstrFormatPage  = new string [2] { "", ""}; // Will initialize later
				s_rgstrLsVersion = new string [2] { "", "" }; // Will initialize later

				fCompareResult = true;
				strReport = "";

				try
				{
					ProcessCompare ();

					if (s_rgLayoutXml [0].CurToken != KToken.Eof || s_rgLayoutXml [1].CurToken != KToken.Eof)
					{
						// Must end both documents here
						CreateReportTagMatch ();
					}
				} 
				catch (CompareErrorException) 
				{
					/* Not equal */
					strReport = s_strCompareErrorReport;
					fCompareResult = false;
				}

				/* Prepare output and return */
				strFlags = "";

				if (s_fJustificationYlIgnored) strFlags += "v";
				if (s_fFootnotePositionIgnored) strFlags += "t";
				if (s_fEndnoteSeparatorIgnored) strFlags += "e";
				if (s_fDropcapHeightIgnored) strFlags += "d";
				if (s_fFigurePositionIgnored) strFlags += "f";
				if (s_fLineBtwColumnsIgnored) strFlags += "c";

				return fCompareResult;
			}
			catch
			{
				/* Trying to catch all possible errors */

				strReport = "LayoutCompare exception: " + strW11Exception;
				strFlags = "";
				return false;
			}
		}

		// GetFlagsDescription
		// Returns string describing meaning of compare flags
		static public string GetFlagsDescription ()
		{
			string strLn = "" + (char) 0xD + (char) 0xA;
			return " v : LSPTS # 606 Different yl in vertical justification, ignored" + strLn +
				   " t : LSPTS # 176 Different footnote yl ignored" + strLn +
				   " e : LSPTS # 478 Different endnote separator ignored" + strLn +
				   " d : LSPTS # 654 Different dropcap height, ignored" + strLn +
				   " f :             Different figure position (usually inside wrapped or double nested table), ignored" + strLn +
				   " c :			 Different line between columns, ignored";
		}

		// CreateReportValuesMatch 
		// Creates infromation report in case values do not match 
		static void CreateReportValuesMatch ( string strOpenTag, string strValue1, string strValue2 )
		{
			s_strCompareErrorReport = (s_cpPrev == -1? "" : (s_strPageParamPrev != "" ? s_strPageParamPrev + " " : "") +
				   "CP=" + s_cpPrev.ToString () + ", ") +
				   (strOpenTag == null ? "Values do not match: " : "<" + strOpenTag + "> ") +
				   strValue1 + " != " + strValue2 + ", xml.line = " + s_rgLayoutXml [0].GetLineNumber ().ToString ();

			throw new CompareErrorException ();
		}

		// CreateReportValuesMatch 
		// Creates infromation report in case values do not match 
		static void CreateReportLineBreakValuesMatch ( string strOpenTag, string strValue1, string strValue2 )
		{
			s_strCompareErrorReport = "Line break " + (s_cpPrev == -1? "" : (s_strPageParamPrev != "" ? s_strPageParamPrev + " " : "") +
				"CP=" + s_cpPrev.ToString () + ", ") +
				(strOpenTag == null ? "Values do not match: " : "<" + strOpenTag + "> ") +
				strValue1 + " != " + strValue2 + ", xml.line = " + s_rgLayoutXml [0].GetLineNumber ().ToString ();

			throw new CompareErrorException ();
		}

		// CreateCustomErrorReport
		static void CreateCustomErrorReport ( string strMsg)
		{
			s_strCompareErrorReport = strMsg;
			throw new CompareErrorException ();
		}

		// CreateReportTagMatch 
		// Creates infromation report in case tags do not match
		static void CreateReportTagMatch ()
		{
			s_strCompareErrorReport = (s_cpPrev == -1 ? "" : "CP=" + s_cpPrev.ToString () + ", ") + 
				   "Tags do not match, xml.line = " + s_rgLayoutXml [0].GetLineNumber ().ToString ();

			throw new CompareErrorException ();
		}
	}


	// CLayoutXML - Parser of layout xml file
	class CLayoutXML
	{
		string _fn;
		string _str;
		int _ipos;

		public int CurToken;
		public string StrTag;
		public string StrTagParam;
		public string StrValue;

		// GetLineNumber
		public int GetLineNumber ()
		{
			int nlines = 1;

			for (int i=0; i < _ipos; i++)
			{
				if (_str [i] == 13) nlines++;
			};

			return nlines;
		}

		// ReportError
		void ReportError (string strMsg)
		{
			Layout.strW11Exception = ("Problem reading layout xml file: " + _fn + ", line " + 
								     GetLineNumber () + ": " + strMsg);
			W11Messages.RaiseError ();
		}

		public CLayoutXML (string fn)
		{
			this._fn = fn;
			_ipos = 0;
			_str = "";

			try
			{
				StreamReader stream = new StreamReader (fn);
				_str = stream.ReadToEnd ();
				stream.Close ();
			}
			catch
			{
				ReportError ("Can not open file");
			}

			Next ();
		}

		void SkipSpaces ()
		{
			while (_ipos < _str.Length && Char.IsWhiteSpace (_str [_ipos])) _ipos++;
		}

		string FetchTagName ()
		{
			int iposA = _ipos;

			while (_ipos < _str.Length && !Char.IsWhiteSpace (_str [_ipos]) && _str [_ipos] != '>') _ipos++;

			if (_ipos == iposA) ReportError ("Invalid tag");

			return _str.Substring (iposA, _ipos - iposA);
		}

		string FetchTagParam ()
		{
			int iposA, iposB;

			SkipSpaces ();

			iposA = _ipos;
			iposB = _ipos-1;

			while (_ipos < _str.Length && _str [_ipos] != '>') 
			{
				if (!Char.IsWhiteSpace (_str [_ipos])) iposB = _ipos;
				
				_ipos ++;
			}

			if (_ipos == _str.Length) ReportError ("Tag must be closed");

			/* Skip tag closing brace */
			_ipos++;

			if (iposB >= iposA) return _str.Substring (iposA, iposB - iposA + 1);
			else 
				return "";
		}

		string FetchValue ()
		{
			int iposA, iposB;

			SkipSpaces ();

			iposA = _ipos;
			iposB = _ipos-1;

			while (_ipos < _str.Length && _str [_ipos] != '<') 
			{
				if (!Char.IsWhiteSpace (_str [_ipos])) iposB = _ipos;
				
				_ipos ++;
			}

			if (iposB >= iposA) return _str.Substring (iposA, iposB - iposA + 1);
			else 
				return "";
		}

		public void Next ()
		{
			bool fFound = false;

			/* Set current strings to null */
			StrTag = null;
			StrTagParam = null;
			StrValue = null;

			while (!fFound)
			{
				SkipSpaces ();

				if (_ipos == _str.Length) 
				{
					CurToken = KToken.Eof;
					fFound = true;
				}
				else if (_str [_ipos] == '<')
				{
					/* Handle tag */
					bool fComment = false;
					bool fEndTag = false;

					_ipos++; if (_ipos == _str.Length) ReportError ("");

					if (_str [_ipos] == '?')  { fComment = true; _ipos++; }
					else if (_str [_ipos] == '/') { fEndTag = true; _ipos++; };

					StrTag = FetchTagName ();

					StrTagParam = FetchTagParam ();

					if (!fComment)
					{
						fFound = true;
						if (fEndTag) CurToken = KToken.TagClose;
						else CurToken = KToken.TagOpen;
					};
				}
				else
				{
					/* Process value */
					StrValue = FetchValue ();

					Common.Assert (StrValue != "");

					fFound = true;
					CurToken = KToken.Value;
				}
			}

			return;

		} // End of Next

	} // End of CLayoutXML

	class CompareErrorException: Exception {};

	// KToken - Kind of xml token
	class KToken
	{
		public static int TagOpen = 0;
		public static int TagClose = 1;
		public static int Value = 2;
		public static int Eof = 3;
	}

	// KCompare - Kind of compare
	class KCompare 
	{
		public static int PageBreaks = 0;
		public static int PageAndLineBreaks = 1;
		public static int All = 2;
	}
}