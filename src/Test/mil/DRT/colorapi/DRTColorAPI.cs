// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains a set of DRTs designed to test the MediaAPIs 
//              and public classes.
//
//

using System;
using System.Windows;
using System.Threading;

using System.Windows.Media;
using System.Runtime.InteropServices;

public class DrtColorAPI
{
    /// <summary>
    /// Instantiates all classes which are subclasses of BaseMediaDRT.
    /// </summary>
    /// <returns>
    /// Return true on success, false on failure.
    /// </returns>
    public static bool RunDRTs()
    {
        System.Type[] types = null;

        // All your types are belong to us ;)
        types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        
        PrintLog("============== Begin " + s_drtName + " ==============\n");

        bool succeeded = true;

        for (int i=0; i<types.Length; i++)
        {
            if ((types[i].IsSubclassOf(typeof(DrtBase))) &&
                (!types[i].IsAbstract))
            {
                DrtBase drt = (DrtBase)Activator.CreateInstance(types[i]); // Create an instance of the drt using the default constructor

                PrintLog(s_drtName + " Test: " + types[i].Name + " Owner: " + drt.Owner + ": ");
                
                string results = null;

                succeeded &= ((DrtBase)drt).Run(out results);                
                    
                drt.Dispose();

                if (succeeded)
                {
                    PrintLog("SUCCEEDED\n");
                }
                else
                {
                    PrintLog("FAILED\n");
                    PrintLog(results + "\n");
                }
            }
        }

        PrintLog("============== End " + s_drtName + " ==============\n");

        return succeeded;
    }

    /// <summary>
    /// Prints a log.
    /// </summary>
    /// <param name="s">The string to print to the log.</param>
    public static void PrintLog(string s)
    {
        s_logBuilder.Append(s);
    }

    [STAThread]
    public static int Main()
    {
        bool succeeded = RunDRTs();

        string statusString = succeeded ? "SUCCEEDED" : "FAILED";

        using (System.IO.StreamWriter output = new System.IO.StreamWriter(s_drtName + ".log", false /* do not append - re-write if the file exists */))
        {
            // Emit success or failure as the first line of the log
            output.WriteLine(s_drtName + " " + statusString + " Owner: " + s_drtOwner + "\n\nExtended log:\n");
            output.WriteLine(s_logBuilder);
        }

        System.Console.Out.WriteLine(s_drtName + " " + statusString + " Owner: " + s_drtOwner);

        // A return code of 0 indicates success, anything else is failure
        return succeeded ? 0 : 1;
    }

    // This StringBuilder is used to accumulate success and failure info for each test.
    static System.Text.StringBuilder s_logBuilder = new System.Text.StringBuilder();

    // This string is used to abstract the name of the DRT from the various places it's referenced.
    static string s_drtName = "DRTColorApi";

    // This string is used to abstract the name of the DRT owner from the various places it's referenced.
    static string s_drtOwner = "Microsoft";
}

