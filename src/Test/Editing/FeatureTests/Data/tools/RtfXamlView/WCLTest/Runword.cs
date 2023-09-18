// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Runword
{
	using Common;
	using Location;

	// This class is responsible running Word
	class CRunword
	{
		System.Windows.Forms.Form	_formParent;
		Delegate					_delegateFinished;
		CLocation					_location;
		Process						_processWord;
		bool						_fRunning;		// if word is running
		bool						_fTerminating;	// in the process is being terminated and
													// finish delegate is already called
	
		public CRunword (System.Windows.Forms.Form formParent, CLocation Location)
		{
			this._formParent = formParent;
			this._location = Location;

			_fRunning = false;
			_fTerminating = false;

			_processWord = null;
		}

		// Check if Word already running
		public bool FWordRunning ()
		{
         //string fnameWordDebug = Path.GetFileNameWithoutExtension (Location.GetFnWordExe (KTarget.Debug));
         //string fnameWordShip  = Path.GetFileNameWithoutExtension (Location.GetFnWordExe (KTarget.Ship));

         //return Process.GetProcessesByName (fnameWordDebug).Length > 0 || 
         //      Process.GetProcessesByName (fnameWordShip).Length > 0 ;
         return false;
		}

		// Starts new Word process, opens document (optional)
		// Returns False if Word was already running and the document (if any)
		// was opening in the existing Word
		public bool FStartWordOpenDoc ( Delegate delegateFinished,
										int kTarget, string sfnDocument, string strParams,
										bool fAllowUsingExistingProcess )
		{
         string sfnWinwordExe = _location.GetPathWordExe();//Location.GetFnWordExe (kTarget);
			Thread threadWaiting;

			// Set finish delegate notification for the Word process
			this._delegateFinished = delegateFinished;

			if (FWordRunning ())
			{
				// Use existing Word process, if there is a document to open
				if (fAllowUsingExistingProcess && sfnDocument != "")
				{
					if (!File.Exists (sfnDocument)) W11Messages.RaiseError ("Can not find document: " + sfnDocument);

					// Start process with the document name => should open in the existing Word
					// No waiting and no running state in this case 
					try { Process.Start (sfnDocument, strParams); 
						}
					catch { W11Messages.RaiseError ("Unable to start: " + sfnDocument); 
						  }
				}
				else W11Messages.RaiseError ("Another Word is already running");

				return false; // Inform caller that no Word process was created
			}
			else
			{
				Common.Assert (!_fRunning);

				if (!File.Exists (sfnWinwordExe)) W11Messages.RaiseError ("Can not find Word executable: " + sfnWinwordExe);

				// Start new Word process
				try { _processWord = Process.Start (sfnWinwordExe, (sfnDocument == "" ? "" : "\"" + sfnDocument + "\"") + 
																  (strParams != "" ? " " + strParams : "")); 
					}
				catch { W11Messages.RaiseError ("Unable to start " + sfnWinwordExe);
					  }

				/* Wait for exit in another thread */

				_fRunning = true;
				_fTerminating = false;

				threadWaiting = new Thread (new ThreadStart (WaitWordProcessToExit));
				threadWaiting.Start ();

				return true; // Inform caller that new Word process was created
							 // The caller will be notified when process ends via delegate
							 // Runword object can not be used until process ends
			}
		}

		// Async function waits until word process finishes and
		// informs main form through the delegate
		void WaitWordProcessToExit ()
		{
			// wait
			_processWord.WaitForExit ();

			if (_fTerminating)
			{
				/* Do we need to do anything? */
			};

			lock (this)
			{
				// Not running any more
				_fRunning = false;

				// tell main form about completed run
				_formParent.Invoke (_delegateFinished);
			}
		}

		
		// User clicks on kill button
		public void KillWord ()
		{	
			lock (this)
			{
				// Kill the process
				try
				{
					_processWord.Kill ();
				}
				catch {};

				_fTerminating = true;
			}
		}

		// User clicks on close button
		public void CloseWord ()
		{	
			lock (this)
			{
				try
				{
					_processWord.CloseMainWindow ();
				} 
				catch {};
				_fTerminating = true;
			}
		}
	}  // End of class CRunword

} // End of namespace CRunword