// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Office = Microsoft.Office.InteropLateBound.Office;
using Word = Microsoft.Office.InteropLateBound.Word;

namespace O12FFVC
{
	/// <summary>
	/// Represents the Office app pool. Clients can make a request to open a file and if the app is alive we will use that app
	/// instead of booting a new one. Useful for better perf. 
	/// If clients use the app pool, they should close the file after opening it. Also shutdown the app after completion of run.
	/// </summary>
	public class CAppPool
	{
		/// <summary>
		/// These fields hold references to the apps. This is the pool.
		/// </summary>
		private static Word.Application s_WordApp;

		/// <summary>
		/// These fields hold the number of opens for a particular app. We will use
		/// this number later to decide whether to reboot the app. This is important 
		/// because Office apps are not good with managing memory - if you open
		/// too many documents (even if you close right after), the memory required by
		/// the app will continue to grow and eventually may cause paging. It is also 
		/// good to reboot an app after a while to start on a "clean slate"
		/// </summary>
		private static int s_WordTotalOpens = 0;
		private static int s_XlTotalOpens = 0;
		private static int s_PptTotalOpens = 0;
		private static readonly int s_TotalNumberOfOpensBeforeReboot = 25;

		public static void ShutDownAllApps(bool force)
		{
			ShutDownWord(force);
		}

		public static void ShutDownAllApps()
		{
			ShutDownAllApps(false);		
		}

		public static void ShutDownWord(bool force)
		{
			// If we are not forcing the shutdown, attempt to gently quit
			if (s_WordApp != null && !force)
				COfficeUtils.QuitWordAndCleanUp(s_WordApp);				
			
			// No matter what, we should make sure no procs are alive
			s_WordApp = null;
			COfficeUtils.KillWordProcs();			
		}

		public static Word.Document OpenWordDocument(string szFullName)
		{		
			// Decide whether we need a reboot
			if (s_WordApp != null && (s_WordTotalOpens%s_TotalNumberOfOpensBeforeReboot) == 0)
			{
				ShutDownWord(false);
				Log.WriteLine("Rebooting Word because it has opened over " + s_TotalNumberOfOpensBeforeReboot + " files.");
			}			
			
			// Only instantiate Word if we have to.
			if (s_WordApp == null)
			{
				// Kill any other instance of WD if it is running
				ShutDownWord(true);

				// Now boot a new instance
				s_WordApp = COfficeUtils.BootWord();
			}

			// Make sure that if we have a ref to WD, that the app is actually running
			if (s_WordApp != null)
			{
				if (!COfficeUtils.IsAppRunning(CAppInfo.EAppName.Word))
				{
					Log.WriteLine("We are holding a ref to WD, yet WD proc is not running.");
					
					// Now boot a new instance
					s_WordApp = COfficeUtils.BootWord();
				}
			}

			// Ensure only one document can be opened at a time
			if (s_WordApp.Documents.Count > 0)
			{
				Log.Fail("There should never be more than one document opened at time. Closing all.");
				CloseWordDocuments();				
			}
            	                
			Word.Document wdDoc = COfficeUtils.OpenWordDocument(szFullName, s_WordApp);	
			s_WordTotalOpens++;
			return wdDoc;
		}

	
		public static void CloseWordDocuments()
		{
			int cDocuments;
			while ((cDocuments = s_WordApp.Documents.Count) > 0)
			{				
				// It appears there are documents to close, trying to close them
				try
				{
					s_WordApp.Documents.Item(1).Close(Word.WdSaveOptions.wdDoNotSaveChanges);
				}
				catch
				{
					// We failed to close the docs, shut down WD next time we open
					s_WordApp = null;
					break;
				}

				// If the cached count of documents still equals the current number of documents
				// then an error occured because we just closed one.
				if (cDocuments == s_WordApp.Documents.Count)
				{					
					Log.Fail("WD refused to close documents.");
					s_WordApp = null;
					break;
				}
			}
		}


	}
}
