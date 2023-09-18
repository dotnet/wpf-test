// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;

namespace O12FFVC
{
	/// <summary>
	/// Summary description for CRegistryMan.
	/// </summary>
	public class CRegistryMan
	{
		public CRegistryMan()
		{
		}

		private enum EKeyValueNames
		{
			RunPendingCompletion,
			LastResultsFolder,
			HostedSiteURL,
			Item_1
		}

		private enum EKeyNames
		{
			Office12FileFormatTool,
			WorkingDirHistory
		}

		public static string[] GetAllWorkingFolderPaths()
		{
			RegistryKey rkWorkingDirHistory = GetWorkingFolderHistoryKey();

			string[] workingFolderPaths = null;

			try
			{
				// No need to waste space, just replace each name in the array with its value.
				workingFolderPaths = rkWorkingDirHistory.GetValueNames();
				for (int i = 0; i < workingFolderPaths.Length; i++)				
				{
					workingFolderPaths[i] = rkWorkingDirHistory.GetValue(workingFolderPaths[i]).ToString();
				}
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to get all working folder paths.", ex.Message);
			}			

			return workingFolderPaths;
		}

		public static string GetMostRecentWorkingFolderPath()
		{
			RegistryKey rkWorkingDirHistory = GetWorkingFolderHistoryKey();

			object oMRWorkingFolderPath = null;


			try
			{
				oMRWorkingFolderPath = rkWorkingDirHistory.GetValue(EKeyValueNames.Item_1.ToString());
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to get most recent working folder path.", ex.Message);
			}			

			if (oMRWorkingFolderPath != null)
				return oMRWorkingFolderPath.ToString();
			else
				return null;
		}


		public static void SetKey_PushWorkingFolderName(string szFolderPath)
		{
			// First check if the working folder has already been added, no dupes allowed!
			foreach (string szStoredFolderPath in GetAllWorkingFolderPaths())
			{
				if (szStoredFolderPath == szFolderPath)
				{
					Log.Fail("Cannot add a duplicate working folder to registry.");
					return;
				}
			}			
			
			const int MAX_STORAGE = 15;
			RegistryKey rkWorkingDirHistory = GetWorkingFolderHistoryKey();

			try
			{
				// Pretend this key is like a queue. Push the most recent folder path to the top (Item 1).
				// Shift all previous folder paths (Item (i+1)).
				string[] regValueNames = rkWorkingDirHistory.GetValueNames();
				ArrayList listWorkingDirPaths = new ArrayList();				
				foreach (string szRegValueName in regValueNames)
				{
					listWorkingDirPaths.Add(rkWorkingDirHistory.GetValue(szRegValueName));
					rkWorkingDirHistory.DeleteValue(szRegValueName, true);
				}

				rkWorkingDirHistory.SetValue(EKeyValueNames.Item_1.ToString(), szFolderPath);

				int currItem = 1;
				foreach(string szPrevWorkingDir in listWorkingDirPaths)
				{
					if (currItem > MAX_STORAGE)
					{
						Log.Fail("Lots of stored working dirs (over " + MAX_STORAGE + "). This indicates something is awry.");
						break;
					}

					rkWorkingDirHistory.SetValue("Item_" + ++currItem, szPrevWorkingDir);
				}
			}
			catch (Exception ex)
			{
				Log.Fail("An error occurred while pushing the most recent Working folder path.", ex.Message);
			}
		}

		/// <summary>
		/// Sets a key indicating a run is pending completion. After the run is complete, be sure
		/// to set this to false. Because if it is true on app start, we know the previous session
		/// went awry.
		/// </summary>
		/// <param name="fRunPendingCompletion"></param>
		public static void SetKey_RunPendingCompletion(bool fRunPendingCompletion)
		{
			RegistryKey workingKey = GetWorkingKey();

			try
			{
				workingKey.SetValue(EKeyValueNames.RunPendingCompletion.ToString(), (fRunPendingCompletion ? "1" : "0"));
				workingKey.Close();
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to set reg value 'RunPendingCompletion'", ex.Message);
			}
		}

		public static void SetKey_LastResultsFolderPath(string szFolderPath)
		{
			RegistryKey workingKey = GetWorkingKey();

			try
			{
				workingKey.SetValue(EKeyValueNames.LastResultsFolder.ToString(), szFolderPath);
				workingKey.Close();
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to set reg value '" + EKeyValueNames.LastResultsFolder.ToString() + "'", ex.Message);
			}
		}

		public static void SetKey_HostedSiteURL(string szHostedSiteURL)
		{
			RegistryKey workingKey = GetWorkingKey();

			try
			{
				workingKey.SetValue(EKeyValueNames.HostedSiteURL.ToString(), szHostedSiteURL);
				workingKey.Close();
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to set reg value '" + EKeyValueNames.HostedSiteURL.ToString() + "'", ex.Message);
			}
		}

		public static bool IsRunPendingCompletion()
		{
			RegistryKey workingKey = GetWorkingKey();

			bool fRunPendingCompletion = false;

			try
			{
				// If the key was never set before, then there is no pending runs, otherwise
				// check if the value is "1" - that means a previous run did not complete.
				if (workingKey.GetValue(EKeyValueNames.RunPendingCompletion.ToString()) == null)
					fRunPendingCompletion = false;
				else			
					fRunPendingCompletion = 
						(workingKey.GetValue(EKeyValueNames.RunPendingCompletion.ToString()).ToString() == "1");
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to check if run is pending completion.", ex.Message);
			}			

			return fRunPendingCompletion;
		}


		public static string GetLastResultsFolderPath()
		{
			RegistryKey workingKey = GetWorkingKey();
		
			string szLastResultsFolderPath = null;

			try
			{				
				object szRegValue = workingKey.GetValue(EKeyValueNames.LastResultsFolder.ToString());

				if (szRegValue != null)
					szLastResultsFolderPath = szRegValue.ToString();
			
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to get last results folder path", ex.Message);
			}			

			return szLastResultsFolderPath;
		}

		public static string GetHostedSiteURL()
		{
			RegistryKey workingKey = GetWorkingKey();
		
			string szHostedSiteURL = null;

			try
			{				
				object szRegValue = workingKey.GetValue(EKeyValueNames.HostedSiteURL.ToString());

				if (szRegValue != null)
					szHostedSiteURL = szRegValue.ToString();			
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to get hosted site URL", ex.Message);
			}			

			return szHostedSiteURL;
		}

		/// <summary>
		/// Pings the registry to see if a key exists
		/// </summary>		
		public static bool RootClassKeyExists(string szKeyName)
		{			
			try
			{
				Registry.ClassesRoot.OpenSubKey(szKeyName).Close();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool LocalMachineSubKeyExists(string szKeyPath)
		{
			try
			{
				Registry.LocalMachine.OpenSubKey(szKeyPath).Close();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool DisableDWCrashDialog()
		{
			string szRegHklmDwPath = @"SOFTWARE\Microsoft\PCHealth\ErrorReporting\DW\Installed";
			if (!LocalMachineSubKeyExists(szRegHklmDwPath))
				return false;

			try
			{
				RegistryKey rkDwInstalled = Registry.LocalMachine.OpenSubKey(szRegHklmDwPath, true);
				rkDwInstalled.DeleteValue("DW0200");
				return true;
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to disabled DW crash dialog.", ex.Message);
				return false;
			}
		}

		public static bool EnableDWCrashDialog()
		{
			string szRegHklmDwPath = @"SOFTWARE\Microsoft\PCHealth\ErrorReporting\DW\Installed";
			if (!LocalMachineSubKeyExists(szRegHklmDwPath))
				return false;

			try
			{
				RegistryKey rkDwInstalled = Registry.LocalMachine.OpenSubKey(szRegHklmDwPath, true);
				rkDwInstalled.SetValue("DW0200", GetProgramFilesRootDrive() + @"PROGRA~1\COMMON~1\MICROS~1\DW\DW20.EXE");
				return true;
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to enable DW crash dialog.", ex.Message);
				return false;
			}
		}

		private static string GetProgramFilesRootDrive()
		{
			try
			{
				return Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			}
			catch (Exception ex)
			{
				Log.Fail("Failed to get program files drive name. Assuming c:\\", ex.Message);
				return @"C:\";
			}		
        }

		/// <summary>
		/// Gets the key that holds the string paths of previous working folders (the user can set the
		/// working folder in the app settings).
		/// </summary>
		/// <returns></returns>
		private static RegistryKey GetWorkingFolderHistoryKey()
		{
			RegistryKey workingKey = GetWorkingKey();
		
			RegistryKey rkWorkingDirHistory = workingKey.OpenSubKey(EKeyNames.WorkingDirHistory.ToString(), true);

			// If the reg key does not exist, attempt to create it
			if (rkWorkingDirHistory == null)
			{
				Log.WriteLine("'" + EKeyNames.WorkingDirHistory + @"' reg key does not exist in 'HKCU\Software\Microsoft\" + EKeyNames.Office12FileFormatTool + "', attempting to create it");
				try
				{
					workingKey.CreateSubKey(EKeyNames.WorkingDirHistory.ToString());					
				}
				catch (Exception ex)
				{
					Log.Fail(@"Failed to create reg key '" + EKeyNames.WorkingDirHistory.ToString() + @"' in 'HKCU\Software\Microsoft\" + EKeyNames.Office12FileFormatTool, ex.Message);
					return null;
				}

				Log.WriteLine("'" + EKeyNames.WorkingDirHistory.ToString() + "' reg key should exist now, attempting to point to it");
				try
				{
					rkWorkingDirHistory = workingKey.OpenSubKey(EKeyNames.WorkingDirHistory.ToString(), true);
				}
				catch (Exception ex)
				{
					Log.Fail("Failed to point to the newly created '" + EKeyNames.WorkingDirHistory.ToString() + "' reg key, it most likely does not exist.", ex.Message);
					return null;
				}
			}

			workingKey.Close();

			return rkWorkingDirHistory;
		}

		/// <summary>
		/// Gets the main working key. Should exist here: HKCU\Software\Microsoft\Office12FileFormatTools
		/// </summary>
		/// <returns></returns>
		private static RegistryKey GetWorkingKey()
		{
			RegistryKey rkMsft = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft", true);			
			if (rkMsft == null)
			{
				Log.Fail(@"Registry key at 'HKCU\Software\Microsoft' does not exist, bad install?");
				return null;
			}

			string szO12KeyName = EKeyNames.Office12FileFormatTool.ToString();
			RegistryKey rkO12FFT = rkMsft.OpenSubKey(szO12KeyName, true);

			// If the reg key does not exist, attempt to create it
			if (rkO12FFT == null)
			{
				Log.WriteLine("'" + szO12KeyName + @"' reg key does not exist in 'HKCU\Software\Microsoft', attempting to create it");
				try
				{
					rkMsft.CreateSubKey(szO12KeyName);					
				}
				catch (Exception ex)
				{
					Log.Fail(@"Failed to create reg key '" + szO12KeyName + @"' in 'HKCU\Software\Microsoft'", ex.Message);
					return null;
				}

				Log.WriteLine("'" + szO12KeyName + "' reg key should exist now, attempting to point to it");
				try
				{
					rkO12FFT = rkMsft.OpenSubKey(szO12KeyName, true);
				}
				catch (Exception ex)
				{
					Log.Fail("Failed to point to the newly created '" + szO12KeyName + "' reg key, it most likely does not exist.", ex.Message);
					return null;
				}
			}

			//Clean up
			rkMsft.Close();			

			return rkO12FFT;
		}
	}
}
