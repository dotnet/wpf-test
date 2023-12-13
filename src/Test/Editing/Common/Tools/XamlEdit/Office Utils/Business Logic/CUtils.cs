// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace O12FFVC
{
	/// <summary>
	/// Misc utilities - string processing, file io, ect.
	/// </summary>
	public class CUtils
	{
		public static string RemoveFileExtension(string fileFullName)
		{
			string fileNameWithoutExtension;

			try
			{
				fileNameWithoutExtension = 
					fileFullName.Remove(fileFullName.LastIndexOf('.'), 
					fileFullName.Substring(fileFullName.LastIndexOf('.')).Length);
			}
			catch (Exception ex)
			{
				Log.Fail("Cannot remove file extension from file '" + fileFullName + "'", ex.Message);
				return null;
			}

			return fileNameWithoutExtension;
		}

		public static string RemoveFilePath(string fileFullName)
		{
			Log.Assert(fileFullName != null, "Cannot remove file path from null name.");

			return new FileInfo(fileFullName).Name;
		}

		public static string RemoveFileExtAndPath(string fullFileName)
		{
			return RemoveFileExtension(RemoveFilePath(fullFileName));
		}

		/// <summary>
		/// Gets the file extension including the '.' (eg. '.docx')
		/// </summary>		
		public static string GetFileExtension (string fileFullName)
		{
			try
			{
			
				if (fileFullName.IndexOf('.') == -1)
				{
					return "{ERROR}";
				}

				return fileFullName.Substring(fileFullName.LastIndexOf('.')).ToLower();			
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to get extension for '" + fileFullName + "'", ex.Message);
			}
			return null;
		}

		/// <summary>
		/// Attempts to delete a directory. Returns true if the dir does not exist, false otherwise.
		/// </summary>
		/// <param name="dirFullPath"></param>
		/// <returns></returns>
		public static bool DeleteDir(string dirFullPath)
		{
			bool dirExists = Directory.Exists(dirFullPath);
			Log.Assert(dirExists, "Cannot delete non existent directory '" + dirFullPath + "'. It may have already been deleted.");
			if (!dirExists)
			{
				return true;
			}

			try
			{
				Directory.Delete(dirFullPath, true);
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to deleted dir '" + dirFullPath + "'", ex.Message);
				return false;
			}

			return true;
		}

		public static bool AttemptToKillProc(Process proc)
		{
			string szProcName = "{ProcName}";
			try
			{
				szProcName = proc.ProcessName;
				proc.Kill();
				proc.Dispose();
				return true;
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to kill the process '" + szProcName + "' .", ex.Message);
				return false;
			}	
		}

		/// <summary>
		/// Returns true if the process is running
		/// </summary>		
		public static bool IsProcRunning(string procName)
		{
			return Process.GetProcessesByName(procName).Length != 0;		
		}

		public static bool CreateOrUseExistingDir(string dirFullPath)
		{
			// If the dir already exists, we have no work to do here.
			if (Directory.Exists(dirFullPath))
			{
				return true;
			}

			// The dir does not exist, try to create
			try
			{
				Directory.CreateDirectory(dirFullPath);
			}
			catch (Exception ex)
			{
				Log.Fail("Could not create dir '" + dirFullPath + "'", ex.Message);
				return false;
			}
			
			return true;
		}

        public static bool CreateOrUseExistingFile(string fileFullName)
        {
            // If the file already exists, we have no work to do here.
            if (File.Exists(fileFullName))
            {
                return true;
            }

            // The file does not exist, try to create
            try
            {
                FileStream fs = File.Create(fileFullName);
                fs.Close();
            }
            catch (Exception ex)
            {
                Log.Fail("Could not create file '" + fileFullName + "'", ex.Message);
                return false;
            }

            return true;
        }
		
		public static bool DeleteFile(string szFileFullName)
		{
			bool fFileExists = File.Exists(szFileFullName);
			if (!fFileExists)
			{
				return true;
			}
			
			try
			{
				File.Delete(szFileFullName);
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to delete the file '" + szFileFullName + "'", ex.Message);
				return false;
			}

			return true;
		}

		public static void BrowseToUrl(string szUrl)
		{
			try
			{	
				Process proc = new Process();
				proc.StartInfo.UseShellExecute = true;
				proc.StartInfo.FileName = "\"" + szUrl + "\"";				
				proc.Start();
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to open file '" + szUrl + "' via Process start.", ex.Message);
			}
		}

		public static void OpenFileInIE(string szFileFullName)
		{	
			if (!File.Exists(szFileFullName))
			{
				Log.Fail("File does not exist: " + szFileFullName);
				return;
			}

			try
			{
				Process proc = new Process();
				proc.StartInfo.UseShellExecute = true;
				proc.StartInfo.FileName = "iexplore";
				proc.StartInfo.Arguments = Quotify(szFileFullName);
				proc.Start();
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to open file '" + szFileFullName + "' via Process start.", ex.Message);
			}
		}

		public static void OpenFile(string szFileFullName)
		{
			// If this is an XML file (not Office file), open in IE, otherwise shell decides
			if (!CAppInfo.IsOfficeFile(szFileFullName) && CAppInfo.IsXmlFile(szFileFullName))
			{
				OpenFileInIE(szFileFullName);
				return;
			}
		
			if (!File.Exists(szFileFullName))
			{
				Log.Fail("File does not exist: " + szFileFullName);
				return;
			}

			try
			{
				Process proc = new Process();
				proc.StartInfo.UseShellExecute = true;
				proc.StartInfo.FileName = Quotify(szFileFullName);
				proc.Start();
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to open file '" + szFileFullName + "' via Process start.", ex.Message);
			}
		}


		/// <summary>
		/// Puts quotes around a string if it contains spaces. This is useful to be used 
		/// on filenames passed in via command line - because DOS needs quoted strings with spaces.
		/// </summary>
		/// <param name="str">string to quotify</param>
		/// <returns>A string that is enclosed in quotes if it contains spaces</returns>
		public static string Quotify(string str)
		{
			if (str.IndexOf(" ") != -1)
			{
				str = "\"" + str + "\"";
			}

			return str;
		}

		/// <summary>
		/// Creates a simple list in XML format. For example: {Files}{File}c:\foo.doc{/File}...{/Files}
		/// </summary>
		/// <param name="rgszListValues"></param>
		/// <param name="listItemName"></param>
		/// <returns></returns>
		public static XmlDocument CreateSimpleXmlList(string[] rgszListValues, string listItemName)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.LoadXml("<?xml version=\"1.0\" ?><" + listItemName + "s/>");

			XmlNode node = null;
			foreach (string szListValue in rgszListValues)
			{
				node = xdoc.CreateElement(listItemName);
				node.InnerText = szListValue;
				xdoc.DocumentElement.AppendChild(node);
			}

			return xdoc;
		}

		/// <summary>
		/// Kills a proc if it exists
		/// </summary>		
		public static void KillProc(string procName)
		{
			try
			{
				Process[] processes = Process.GetProcessesByName(procName);							

				foreach (Process proc in processes)
					AttemptToKillProc(proc);
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to kill process '" + procName + "'", ex.Message);
			}
		}

		public static void ShowSimpleMsgBoxError(string msg)
		{
			MessageBox.Show(msg, "File Format Activity Tool", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static void ShowSimpleMsgBoxWarning(string msg)
		{
			MessageBox.Show(msg, "File Format Activity Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		public static void ShowSimpleMsgBoxInfo(string msg)
		{
			MessageBox.Show(msg, "File Format Activity Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		
		public static string GetPrettyPrintedXmlFromFile(string szXmlFullName)
		{
			if (!File.Exists(szXmlFullName))
			{
				Log.Fail("XML file '" + szXmlFullName + "' does not exist. Cannot get its xml.");
				return string.Empty;
			}

			String szResult = string.Empty;
			MemoryStream MS = new MemoryStream();
			XmlTextWriter W = new XmlTextWriter(MS, System.Text.Encoding.Unicode);
			XmlDocument D   = new XmlDocument();

			try
			{
				// Load the XmlDocument with the XML.
				D.Load(szXmlFullName);

				W.Formatting = Formatting.Indented;

				// Write the XML into a formatting XmlTextWriter
				D.WriteContentTo(W);
				W.Flush();
				MS.Flush();

				// Have to rewind the MemoryStream in order to read
				// its contents.
				MS.Position = 0;

				// Read MemoryStream contents into a StreamReader.
				StreamReader SR = new StreamReader(MS);

				// Extract the text from the StreamReader.
				String FormattedXML = SR.ReadToEnd();

				szResult = FormattedXML;
			}
			catch (Exception ex)
			{
                Log.Fail("Failed to get pretty printed XML from file '" + szXmlFullName + "'", ex.Message);
			}

			MS.Close();
			W.Close();

			return szResult;
		}
	}
}
