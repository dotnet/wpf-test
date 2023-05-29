// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* As far as understand, from webpages like
 * http://msdn.microsoft.com/library/en-us/cpguide/html/cpconhowruntimelocatesassemblies.asp
 * the runtime locates an assembly in the following order:
 * 
 * Before doing the following steps, the runtime figures out the correct version of the 
 * assembly. This can be dictated by the code itself (which provides the full name), or 
 * this can be specified in the app config file, publisher policy file, and machine config
 * file.
 * 1. Checks whether the assembly name has been bound to before and, if so, uses 
 * the previously loaded assembly. 
 * 2. Check the GAC.
 * 3.
 * 4. 
 * ....
 * ....
*/ 
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using System.Windows.Markup;
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.IO;
using System.Xml;
using System.Security;
using System.Security.Permissions;

using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.Common;


namespace Avalon.Test.CoreUI.Parser
{
	/// <summary>
	/// Verify that assemblies are handled correctly by the parser.
	/// </summary>
	public class AssemblyUsageTest
	{
		/// <summary>
		/// Test case entry point
		/// </summary>
		public void RunTest()
		{
			Setup();
			TestAssemblyWithDifferentMajorVersion();
		}

		/// <summary>
		/// Common setup for all the tests.
		/// Here we compile the XAML and create an executable.
		/// </summary>
		private void Setup()
		{
			string currDir = Directory.GetCurrentDirectory();
			// Copy the original assembly with a generic name
			File.Copy(Path.Combine(currDir, _orig_Assembly_Name), Path.Combine(currDir, _generic_Assembly_Name), true);
			// Compile the XAML
			ParserUtil.CompileXamlToBaml(_xaml_With_Short_Assembly_Name);
			// Copy the exe to the current folder
			String objPath = Path.Combine(Path.Combine(currDir, "obj"), "release");
			File.Copy(Path.Combine(objPath, _exeName), Path.Combine(currDir, _exeName), true);
		}

		/// <summary>
		/// Try running the exe and loading the XAML with a different assembly
		/// in place of the original assembly. They differ in major version no.
		/// </summary>
		private void TestAssemblyWithDifferentMajorVersion()
		{
			string currDir = Directory.GetCurrentDirectory();
			// Copy the new assembly to the generic assembly
			File.Copy(Path.Combine(currDir, _assembly_Name_Different_Major_Version), Path.Combine(currDir, _generic_Assembly_Name), true);
			// Run the executable.
			CoreTestsSingleRunServices.RunTestCaseProcess(_exeName, 60);
		}

		private String _exeName = "Xaml_With_Short_Assembly_Name.exe";
		private String _xaml_With_Short_Assembly_Name = "Xaml_With_Short_Assembly_Name.xaml";
		// private String Xaml_With_Long_Assembly_Name = "Xaml_With_Long_Assembly_Name.xaml".
		private String _generic_Assembly_Name = "AssemblyVerTest.dll";
		private String _orig_Assembly_Name = "AssemblyVerTest1_0.0.0.0.dll";
		private String _assembly_Name_Different_Major_Version = "AssemblyVerTest_1.0.0.0.dll";
	}	
}
