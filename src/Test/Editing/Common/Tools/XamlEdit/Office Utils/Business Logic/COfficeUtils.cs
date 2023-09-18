// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Office = Microsoft.Office.InteropLateBound.Office;
using Word = Microsoft.Office.InteropLateBound.Word;

namespace O12FFVC
{
	/// <summary>
	/// Office utilities
	/// </summary>
	public class COfficeUtils
	{
		public static object oMiss = Type.Missing;
		
		public enum ETestableWordSaveFormats
		{
			LegacyDoc97To2003 = (int)Word.WdSaveFormat.wdFormatDocument97,
            NewFileFormatMacroEnabled = (int)Word.WdSaveFormat.wdFormatXMLDocumentMacroEnabled,
			WordML = (int)Word.WdSaveFormat.wdFormatXML,
			RTF = (int)Word.WdSaveFormat.wdFormatRTF
		}


		public enum EOfficeProcNames
		{
			WINWORD,
			WINWORDD,
			POWERPNT,
			EXCEL,
			OUTLOOK,
			UKNOWN
		}

		public static EOfficeProcNames AppNameToProcName (CAppInfo.EAppName appName)
		{
			switch (appName)
			{
				case CAppInfo.EAppName.Excel : 
					return EOfficeProcNames.EXCEL;
				case CAppInfo.EAppName.PowerPoint :
					return EOfficeProcNames.POWERPNT;
				case CAppInfo.EAppName.Word :
					return EOfficeProcNames.WINWORD;
				case CAppInfo.EAppName.Outlook :
					return EOfficeProcNames.OUTLOOK;
			}

			Log.Fail("Unknown app name, cannot get proc name.");
			return EOfficeProcNames.UKNOWN;
		}

		public static bool IsAppRunning(CAppInfo.EAppName appName)
		{
			return CUtils.IsProcRunning(AppNameToProcName(appName).ToString());			
		}

		public static CAppInfo.EAppName GetNameOfAnyRunningOfficeApp()
		{
			if (IsAppRunning(CAppInfo.EAppName.Word)) return CAppInfo.EAppName.Word;
			if (IsAppRunning(CAppInfo.EAppName.PowerPoint)) return CAppInfo.EAppName.PowerPoint;			
			if (IsAppRunning(CAppInfo.EAppName.Excel)) return CAppInfo.EAppName.Excel;
			if (IsAppRunning(CAppInfo.EAppName.Outlook)) return CAppInfo.EAppName.Outlook;
		
			return CAppInfo.EAppName.Unknown;
		}

		public static bool IsOfficeVersionInstalled(int version)
		{
			return IsOfficeAppVersionInstalled(CAppInfo.EAppName.Word, version) &&
				IsOfficeAppVersionInstalled(CAppInfo.EAppName.PowerPoint, version) &&
				IsOfficeAppVersionInstalled(CAppInfo.EAppName.Excel, version);
		}
		
		public static bool IsOfficeAppVersionInstalled(CAppInfo.EAppName appName, int version)
		{
			string szClassId = appName + ".Application." + version;
			Log.WriteLine("Checking if Class ID exists: " + szClassId);
			return CRegistryMan.RootClassKeyExists(szClassId);
		}

		public static void KillWordProcs()
		{
			KillProc(EOfficeProcNames.WINWORD);
			KillProc(EOfficeProcNames.WINWORDD);			
		}

		public static void KillAllOfficeProcs()
		{
			KillWordProcs();
			KillProc(EOfficeProcNames.EXCEL);
			KillProc(EOfficeProcNames.POWERPNT);
		}			

		public static void KillProc(EOfficeProcNames procName)
		{
			CUtils.KillProc(procName.ToString());
		}		

		public static void QuitWordAndCleanUp(Word.Application wapp)
		{
			object oMiss = Type.Missing;
			object oDoNotSaveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;

			try
			{				
				// Restore Word's default settings
				wapp.ScreenUpdating = true;					
				wapp.DisplayAlerts = Word.WdAlertLevel.wdAlertsMessageBox;
				wapp.AutomationSecurity = Office.MsoAutomationSecurity.msoAutomationSecurityLow;
				SetAutoMacros(wapp, false);		
				
				// Quit
				wapp.Quit(Word.WdSaveOptions.wdDoNotSaveChanges);							
			}
			catch (Exception ex)
			{
				Log.Fail("Could not quit Word.", ex.Message);
			}		
			
			wapp = null;			
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}


		public static bool OpenAndSaveWordFileViaAppPool(string szFullNameToOpen, string szFullNameToSave, ETestableWordSaveFormats format)
		{
            return OpenAndSaveWordFileViaAppPool(szFullNameToOpen, szFullNameToSave, (int)format);
		}

        public static bool OpenAndSaveWordFileViaAppPool(string szFullNameToOpen, string szFullNameToSave, Word.WdSaveFormat format)
        {
            return OpenAndSaveWordFileViaAppPool(szFullNameToOpen, szFullNameToSave, (int)format);
        }

        public static bool OpenAndSaveWordFileViaAppPool(string szFullNameToOpen, string szFullNameToSave, int format)
        {
            Word.Document wdoc = CAppPool.OpenWordDocument(szFullNameToOpen);
            bool result = SaveWordDocument(szFullNameToSave, wdoc, format);
            CAppPool.CloseWordDocuments();
            return result;
        }



		public static Word.Application BootWord()
		{
			Word.Application wdApp = null;		
			try
			{			
				wdApp = new Word.Application();
				wdApp.Visible = false;
				wdApp.ScreenUpdating = false;
				wdApp.DisplayAlerts = Word.WdAlertLevel.wdAlertsNone;
				wdApp.AutomationSecurity = Office.MsoAutomationSecurity.msoAutomationSecurityForceDisable;
				SetAutoMacros(wdApp, true);
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to instantiate Word.", ex.Message);
			}

			return wdApp;
		}

		public static bool SaveWordDocument(string szFullName, Word.Document wdoc, int format)
		{
			if (wdoc == null)
			{
				Log.Fail("Given a null Word document object");
				return false;
			}

			if (File.Exists(szFullName))
			{
				Log.Fail("Warning: '" + szFullName + "' already exists. Saving over it.");
			}						
			
			try
			{
				wdoc.SaveAs(szFullName, format);
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to save document '" + szFullName + "' in Word.", ex.Message);	
				return false;
			}		

			return true;			
		}



		public static Word.Document OpenWordDocument(string fullFileName, Word.Application wapp)
		{
			if (!File.Exists(fullFileName))
			{
				Log.Fail("Non existent wd filename given to open: " + fullFileName);
				return null;
			}

			if (wapp == null)
			{
				Log.Fail("Given a null Word object");
				return null;
			}
	
			// Try to open - note this is where unexpected dialogs may pop up
			Word.Document wdoc = null;
			try
			{
				wdoc = wapp.Documents.Open(fullFileName, false, true, false, "foo", "foo", true); 
				
				// According to KB article #Regression_Bug893, must disable security immediatly after open "..to avoid malicious subversion."
				wapp.AutomationSecurity = Office.MsoAutomationSecurity.msoAutomationSecurityForceDisable;

				Log.Assert(wapp.Documents.Count == 1, "Only 1 document should be open at any time.");			
			}
			catch (Exception ex)
			{				
				Log.Fail("Failed to open document '" + fullFileName + "'", ex.Message);				
			}					

			return wdoc;
		}

		[Guid("00020400-0000-0000-c000-000000000046")]
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			public interface IDispatch
		{
			[PreserveSig] int GetTypeInfoCount();
			UCOMITypeInfo GetTypeInfo(
				[MarshalAs(UnmanagedType.U4)] int iTInfo,
				[MarshalAs(UnmanagedType.U4)] int lcid);
			[PreserveSig] int GetIDsOfNames(
				ref Guid riid,
				[MarshalAs(UnmanagedType.LPArray,
					 ArraySubType=UnmanagedType.LPWStr)]
				string[] rgsNames,
				int cNames,
				int lcid,
				[MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);
			[PreserveSig] int Invoke(
				int dispIdMember,
				ref Guid riid,
				[MarshalAs(UnmanagedType.U4)] int lcid,
				[MarshalAs(UnmanagedType.U4)] int dwFlags,
				ref DISPPARAMS pDispParams,
				[MarshalAs(UnmanagedType.LPArray)][Out] object[]
				pVarResult,
				ref EXCEPINFO pExcepInfo,
				[MarshalAs(UnmanagedType.LPArray)][Out] IntPtr[]
				pArgErr);
		}

		public struct VariantBool
		{
			public short vt /*VT_BOOL = 11*/;
			//short wReserved1;
			//short wReserved2;
			//short wReserved3;
			public short boolVal;
			public short dummy1;
			//IntPtr dummy2;
		}

		public static void SetAutoMacros(Word.Application wapp, bool fDisable)
		{

			try
			{
				object wb = wapp.WordBasic;
				int[] array = new int [1];
				System.Guid IID_NULL = System.Guid.Empty;
				IntPtr [] err = new IntPtr[2];
				EXCEPINFO except = new EXCEPINFO();
				DISPPARAMS dispparams = new DISPPARAMS();

				((IDispatch)wb).GetIDsOfNames(ref IID_NULL, new string [] {"DisableAutoMacros"}, 1, 1033, array);

				dispparams.cNamedArgs = 0;
				dispparams.cArgs = 1;

				int size = 16 /*sizeof(VariantBool)*/;
				IntPtr varArray = Marshal.AllocCoTaskMem(size);

				VariantBool var = new VariantBool();

				if (fDisable)
				{
					var.boolVal = -1 /*TRUE*/;				
				}
				else
				{
					var.boolVal = 0; /*FALSE*/
				}

				var.vt = 11 /*VT_BOOL*/;

				Marshal.StructureToPtr(var, varArray, false);
				dispparams.rgvarg = varArray;

				((IDispatch)wb).Invoke(array[0],ref IID_NULL, 1033,
					1/*DISPATCH_METHOD*/, ref dispparams, null, ref except, err);

				Marshal.FreeCoTaskMem(varArray);
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to disable auto macros", ex.Message);
			}

		}

	}
}
