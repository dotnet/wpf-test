// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Resources;
using System.Collections;

namespace resgenx
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
            //args = new string [] { "filter.xsl", "filter-txt2.xsl", "-o", "UIVerify" };

            int cArgs = args.Length;

            // No args
            if (cArgs < 1)
            {
                ShowUsage ();
                return 1;
            }

            // Parse the arguments
            bool fOut = false;
            ArrayList alFiles = new ArrayList ();
            string sOut = "resgenx.resources";

            for (int i = 0; i < cArgs; i++)
            {
                if (fOut)
                {
                    sOut = args [i];
                    fOut = false;
                }
                else if (args [i] == "-o")
                {
                    fOut = true;
                }
                else
                {
                    alFiles.Add (args [i]);
                }
            }

            if (sOut.IndexOf (".resources") < 0)
            {
                sOut = sOut + ".resources";
            }

            try
            {
                if (File.Exists (sOut))
                {
                    File.Delete (sOut);
                }

                ResourceWriter rw = new ResourceWriter (sOut);

                for (int c = alFiles.Count, i = 0; i < c; i++)
                {
                    string sIn = (string) alFiles [i];
                    FileStream fs = new FileStream (sIn, FileMode.Open, FileAccess.Read);
                    byte [] aData = new byte [(int) fs.Length];
                    fs.Read (aData, 0, aData.Length);
                    fs.Close ();

                    // remove the path in the resources
                    int iBackSlash = sIn.LastIndexOf ('\\');
                    if (iBackSlash >= 0)
                    {
                        sIn = sIn.Substring (iBackSlash + 1, sIn.Length - (iBackSlash + 1));
                    }
                    rw.AddResource (sIn, aData);
                }

                rw.Close ();
            }
            catch (Exception e)
            {
                Console.WriteLine ("Error: " + e.Message);
                return 1;
            }
            return 0;
		}

        static void ShowUsage ()
        {
            Console.WriteLine ("Usage: resgenx [-o output_file] input_file1 [input_file2]\n\nCompile \"input_file\" into a resources files.\n  If no \".resources\" extension for \"output_file\" is given, the extension \".resources\" is automatically added\n  If no \"output_file\" is given \"resgenx.resources\" is created");
        }
	}
}
