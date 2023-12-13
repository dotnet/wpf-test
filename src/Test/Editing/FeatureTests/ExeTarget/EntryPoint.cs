// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Defines the entry point for Text BVT test cases.

using System.Runtime.InteropServices;
using Microsoft.Test.Logging;
using System;
using Test.Uis.TextEditing;
using System.Diagnostics;
using Microsoft.Test.Loaders;
using Test.Uis.TestTypes;

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/ExeTarget/EntryPoint.cs $")]

class EntryPointType
{
    [System.STAThread]
    public static void Main(string[] args)
    {
        try
        {         
            LibraryDoMain(args);
        }
        catch(System.Exception e)
        {
            System.Console.WriteLine(
                "Unexpected exception caught at executable entry point.");
            System.Console.WriteLine(e.ToString());

            System.IO.FileNotFoundException fnf =
                e as System.IO.FileNotFoundException;
            bool missingLib = (fnf != null) &&
                (fnf.FileName.IndexOf("EditingTestLib") != -1);
            if (missingLib)
            {
                System.Console.WriteLine(
                    "Known cause: EditingTestLib.dll not found.");
            }
            else
            {
                ResetInput();
            }
        }
    }

    /// <summary>
    /// Entry Point of Sti.exe
    /// </summary>
    public static void DriverEntryPoint(string commandline)
    {
        Main(commandline.Split(' '));
    }

    /// <summary>
    /// Isolating this calls enables the test case to detect when the
    /// helper library has been incorrectly setup.
    /// </summary>
    private static void LibraryDoMain(string[] args)
    {
        bool _partialTrust = false;
        string commandline = "";
        foreach (string str in args)
        {
            if (str.ToLower() == "/pt")
            {
                _partialTrust = true;
                continue;
            }
            commandline += str+" ";
        }

        if (_partialTrust == true)
        {
            commandline = "EditingTestDeploy.xbap " + commandline+ " /pt";
            //  GlobalLog.LogEvidence("Executing Partial Trust");
            CustomTestCase.ExecutePartialTrust(commandline);
        }
        else
        {
            Test.Uis.Management.TestRunner.DoMain(args);
        }
    }

    /// <summary>
    /// Isolating these calls enable the test case to handle a missing
    /// library gracefully. This cleanup is required to avoid affecting
    /// the following test in automated environments, as ctrl, alt and shift
    /// may be left in a pressed state.
    /// </summary>
    private static void ResetInput()
    {
        Test.Uis.Utils.KeyboardInput.ResetKeyboardState();
        Test.Uis.Wrappers.Win32.SafeBlockInput(false);
    }
}
