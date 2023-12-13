// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace Location
{
	using Common;

	// Class Location - This class is responsible for making paths and file names

	class CLocation
	{
		public SOptions options;

		public CLocation (/*string _pathBuilderExe, string _pathTestFolder, */SOptions _options)
		{
			//PathBuilderExe = _pathBuilderExe;
			//PathTestFolder = _pathTestFolder;
			options = _options;
		}

		/* Modify options on the fly */
		public void ChangeOptions (SOptions _options)
		{
			options = _options;
		}

		public string GetPathWordExe () 
		{ 
			return Path.Combine (options.szWordDir, "winword.exe");
		}

		/* Name of BVTTest.ini file */
		public string GetNameTestIni ()
		{
			return "BVTTest.ini";
		}

      public string GetRtfXamlExe()
      {
         return Path.Combine(options.szRTFXamlDir, "rtfxamlview.exe");
      }

      public string GetBVTDir()
      {
         return options.szBVTDir;
      }

      public string GetLogDir()
      {
         return options.szLogDir;
      }

      public string GetTempBVTLogFile()
      {
         return options.szLogDir +  "\\Temp.log";
      }

      /*Full path to the currently selected Log file*/
      public string GetLogFile()
      {
         return options.szLogDir + "\\" + options.szLogFile;
      }
      
      public string GetTempSumaryFile()
      {
         return options.szLogDir + "\\tempSumary.txt";
      }
      
      public string GetSumaryFile()
      {
         return options.szLogDir + "\\" + options.szSumaryFile;
      }

      public string CreateLogFileName(string szTest)
      {
         DateTime dt = DateTime.Now;

         options.szLogFile = szTest + " Results " + dt.Month.ToString() +
            "_" + dt.Day.ToString() + "_" + dt.Year.ToString() + ".log";
         options.szSumaryFile = szTest + " Sumary " + dt.Month.ToString() +
            "_" + dt.Day.ToString() + "_" + dt.Year.ToString() + ".txt";
         return options.szLogFile;
      }
	}


	// Options
	struct SOptions // Modify compare when changing this struct!!!
	{
      //Misc settings
      public bool fRestartDocs;              //Restart App for each test.
		public bool fNoAssert;                 //not currently implemented.
		public bool fUseXCVT;                  //Send Text directly to the converted or use copy paste
		public bool fPri0;                     //log only pri0 fails
      
      //BVT test flags
      public bool fRTRtf;
      public bool fRTXaml;
      public bool fRtfXaml;
      public bool fREAssert;
      public bool fWordAssert;
      
      //Paths
      public string szWordDir;				   // Directory word is installed in.
      public string szRTFXamlDir;		      // Directory that rtfxamlview is in.
      public string szBVTDir;	               // Directory with test files to run BVT test on.
      public string szLogDir;                // Directory to save log files in.
      public string szLogFile;               // the currently selected log file.
      public string szSumaryFile;            //sumary file for a given run.
		public static bool FEqual (SOptions o1, SOptions o2)
		{
         return o1.fRestartDocs == o2.fRestartDocs &&
               o1.fNoAssert == o2.fNoAssert &&
               o1.fUseXCVT == o2.fUseXCVT &&
               o1.fPri0 == o2.fPri0 &&
               o1.fRTRtf == o2.fRTRtf &&
               o1.fRTXaml == o2.fRTXaml &&
               o1.fRtfXaml == o2.fRtfXaml &&
               o1.fREAssert == o2.fREAssert &&
               o1.fWordAssert == o2.fWordAssert &&
               o1.szWordDir == o2.szWordDir &&
               o1.szRTFXamlDir == o2.szRTFXamlDir &&
               o1.szBVTDir == o2.szBVTDir &&
               o1.szLogDir == o2.szLogDir &&
               o1.szLogFile == o2.szLogFile;
		}
	}
}