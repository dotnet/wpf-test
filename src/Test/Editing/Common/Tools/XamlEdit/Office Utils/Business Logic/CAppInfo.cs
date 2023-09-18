// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace O12FFVC
{
	/// <summary>
	/// Summary description for CAppInfo.
	/// </summary>
	public class CAppInfo
	{
		public static string s_extXmlFile = "*.xml";		
		public static string s_extWdFile = "*.doc;*.dot;*.rtf;*.docm;*.docx";
		public static string s_extXlFile = "*.xl;*.xls;*.xlt;*.xlsb;*.xlw;*.xlsx;*.xlsm";
		public static string s_extPptFile = "*.ppt;*.pps;*.pot;*.pptx;*.pptm";
		//public static string s_extNewFileFormats = "*.docm;*.docx;*.xlsx;*.xlsm;*.pptx;*.pptm";
		
		public enum EAppName
		{
			PowerPoint = 0,
			Word = 1,
			Excel = 2,
			Unknown = 3,			
			Outlook = 4,			
		}

		public static EAppName GetAppNameFromXml(string fileFullName)
		{
			try
			{
				XmlTextReader rdr = new XmlTextReader(fileFullName);			
				while (rdr.Read())
				{
					if (rdr.NodeType == XmlNodeType.ProcessingInstruction)
					{
						if (rdr.LocalName == "mso-application")
						{
							if (rdr.Value.IndexOf("Excel.Sheet") != -1)
								return EAppName.Excel;

							if (rdr.Value.IndexOf("Word.Document") != -1)
								return EAppName.Word;

							return EAppName.Unknown;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to read xml file '" + fileFullName + "' to get the Office app name.", ex.Message);
			}
				

			return EAppName.Unknown;
		}

		public static EAppName GetAppName(string fileFullName, bool checkNonUniqueFormats)
		{
			if (IsWdFile(fileFullName, checkNonUniqueFormats))
				return EAppName.Word;

			if (IsXlFile(fileFullName, checkNonUniqueFormats))
				return EAppName.Excel;

			if (IsPptFile(fileFullName, checkNonUniqueFormats))
				return EAppName.PowerPoint;

			return EAppName.Unknown;
		}

		public static string GetAppFriendlyName(EAppName app)
		{
			switch (app)
			{
				case EAppName.Excel : 
					return "Excel File";
				case EAppName.PowerPoint :
					return "PowerPoint Presentation";
				case EAppName.Word :
					return "Word Document";
			}
		
			return "Unknown File";
		}

		public static bool IsFileValid(string fileFullName)
		{
			if (!File.Exists(fileFullName))
				return false;

			string ext = CUtils.GetFileExtension(fileFullName);

			if (s_extXmlFile.IndexOf(ext) != -1) return true;			
			if (s_extWdFile.IndexOf(ext) != -1) return true;
			if (s_extXlFile.IndexOf(ext) != -1) return true;
			if (s_extPptFile.IndexOf(ext) != -1) return true;

			return false;
		}

		public static bool IsOfficeFile(string fileFullName)
		{
			if (!File.Exists(fileFullName))
				return false;

			string ext = CUtils.GetFileExtension(fileFullName);
			
			if (s_extWdFile.IndexOf(ext) != -1) return true;
			if (s_extXlFile.IndexOf(ext) != -1) return true;
			if (s_extPptFile.IndexOf(ext) != -1) return true;

			return false;
		}

		public static bool IsXmlFile(string fileFullName)
		{
			return s_extXmlFile.IndexOf(CUtils.GetFileExtension(fileFullName)) != -1;
		}

		public static bool IsNewFileFormatOfficeFile(string fileFullName)
		{
			string ext = CUtils.GetFileExtension(fileFullName);
			Log.Assert(ext.IndexOf('.') != -1, " Assuming the extension contains a '.'");

			switch (ext)
			{
				case ".docm" : return true;
				case ".docx" : return true;
				case ".xlsx" : return true;
				case ".xlsm" : return true;
				case ".pptx" : return true;
				case ".pptm" : return true;
			}

			return false;
		}

        public static string GetWdExtensionFromFormat(Microsoft.Office.InteropLateBound.Word.WdSaveFormat format)
        {
            switch (format)
            {
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatDocument: return "doc";                
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatFilteredHTML: return "html";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatHTML: return "html";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatRTF: return "rtf";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatTemplate: return "dot";                
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatText: return "txt";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatUnicodeText: return "txt";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatWebArchive: return "mht";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatXML: return "xml";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatXMLDocument: return "docx";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatXMLDocumentMacroEnabled: return "docm";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatXMLTemplate: return "dotx";
                case Microsoft.Office.InteropLateBound.Word.WdSaveFormat.wdFormatXMLTemplateMacroEnabled: return "dotm";                
            }

            Log.Fail("Unknown format type given. (" + format + ")");
            return "???";
        }                                

		public static bool IsWdFile(string fileFullName, bool checkNonUniqueFormats)
		{
			if (!File.Exists(fileFullName))
			{
				Log.Fail("File does not exist: " + fileFullName);
				return false;
			}

			string ext = CUtils.GetFileExtension(fileFullName);
			if (ext == String.Empty)
				return false;		

			if (s_extWdFile.IndexOf(ext) != -1)
				return true;

			if (checkNonUniqueFormats)
			{
				if (s_extXmlFile.IndexOf(ext) != -1)
					return (EAppName.Word == GetAppNameFromXml(fileFullName));					
			}

			return false;
		}

		public static bool IsXlFile(string fileFullName, bool checkNonUniqueFormats)
		{
			if (!File.Exists(fileFullName))
			{
				Log.Fail("File does not exist: " + fileFullName);
				return false;
			}
			
			string ext = CUtils.GetFileExtension(fileFullName);
			if (ext == String.Empty)
				return false;

			if (s_extXlFile.IndexOf(ext) != -1)
				return true;

			if (checkNonUniqueFormats)
			{
				if (s_extXmlFile.IndexOf(ext) != -1)
					return (EAppName.Excel == GetAppNameFromXml(fileFullName));				
				
			}

			return false;
		}

		public static bool IsPptFile(string fileFullName, bool checkNonUniqueFormats)
		{
			if (!File.Exists(fileFullName))
			{
				Log.Fail("File does not exist: " + fileFullName);
				return false;
			}
		
			string ext = CUtils.GetFileExtension(fileFullName);
			if (ext == String.Empty)
				return false;

			if (s_extPptFile.IndexOf(ext) != -1)
				return true;

			if (checkNonUniqueFormats)
			{
				return false;
			}

			return false;
		}   
	}
}
