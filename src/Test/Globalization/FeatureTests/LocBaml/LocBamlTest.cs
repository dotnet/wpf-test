// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*  
*   LocBamlTestHelp.cs
*
*/


using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;

using System.Windows.Markup.Localizer;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;



namespace Microsoft.Test.Globalization
{

     [Test(0, "Localization.LocBaml", "LocBamlTestHelp", SupportFiles = @"FeatureTests\Globalization\LocBaml\locbaml.exe, FeatureTests\Globalization\Data\tree.baml", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
	public class LocBamlTestHelp : StepsTest
    {
         public LocBamlTestHelp()
         	{
		 	InitializeSteps += new TestStep(LoadLocBaml);
			RunSteps += new TestStep(CheckExit);
		       RunSteps += new TestStep(CheckOutput);
         	}
		 TestResult LoadLocBaml()
		 	        {
				     //Get the Working directory
			            _workDir = Environment.GetEnvironmentVariable("%WORKDIR%");
			            if (_workDir != null && _workDir.Length != 0)
			            {
			                _workDir = Environment.ExpandEnvironmentVariables(_workDir);
			                Directory.SetCurrentDirectory(_workDir);
			            }
			            else
			            {
			                _workDir = Directory.GetCurrentDirectory();
			            }
			            
			            // load the assembly
			            Assembly asm = Assembly.LoadFile(_workDir + "\\locbaml.dll");

			            // Get the stringtable 
			            _rm = new System.Resources.ResourceManager("Resources.stringtable", asm);
			            LogComment("LocBamlTest Started ..." + "\n");
				     return TestResult.Pass;
			        }
		 TestResult CheckExit()
		 	{
		 	
			            LogComment("Launching LocBaml with /? flag...");
			            Process p = new Process();
			            p.StartInfo.UseShellExecute = false;
			            p.StartInfo.RedirectStandardOutput = true;
			            p.StartInfo.RedirectStandardError = true;
			            p.StartInfo.CreateNoWindow = true;
			            p.StartInfo.FileName = "locbaml.exe";
			            LogComment("Parsing Process: " + p.StartInfo.FileName.ToString());
			            p.StartInfo.Arguments = "-?";
			            LogComment("Parsing Arg: " + p.StartInfo.Arguments.ToString());
			            p.StartInfo.WorkingDirectory = _workDir;
			            p.Start();
			            
			            
			            _output += p.StandardOutput.ReadToEnd() + "\r\n";

			            if (p.ExitCode == 100)
			            {
					  return TestResult.Pass;
					
			            }
			            else
			            {
			                LogComment("Does not exit LocBaml correctly...\n" + _output);
					  return TestResult.Fail;
			            }
		 	}
		 
		TestResult CheckOutput()
			{
				string error = _rm.GetString("Err_UnknownOption");
		              error = string.Format(error, "?");
              		string _error = _rm.GetString("ErrorMessage");
               		_error = string.Format(_error, error);
               	       string message = _rm.GetString("Msg_Copyright");
                		message = string.Format(message, CommonLib.GetAssemblyVersion(_workDir + "\\locbaml.dll"));
                		message += "\r\n" + _error + "\r\n\r\n";
                		message += _rm.GetString("Msg_Usage") + "\r\n\r\n";
                		LogComment(message);
                		if (String.Compare(_output, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                		{
                    			LogComment("Exit LocBaml correctly...\n");
		      			return TestResult.Pass;
                		}
                		else
                		{
                    			LogComment("Does not show help in LocBaml correctly...\n" + _output);
		      			return TestResult.Fail;
                		}
            
        		}
			private string _workDir;
			private string _output = null;
                     System.Resources.ResourceManager _rm;
        	}
}
		
    
    

    

