// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Threading;

namespace CultureSwitcher
{
    static class Launcher
    {
	[STAThread]
	static int Main(string[] args)
        {
            if (args.Length == 1)
            {
                LaunchAppInSpecifiedCulture(args[0], "en-US");
            }
            else if (args.Length == 2)
            {
                LaunchAppInSpecifiedCulture(args[0], args[1]);
            }
            else
            {
                Console.WriteLine("Usage: Launcher.exe AppName [TargetCulture]");
                return 1;
            }
            return 0;
        }
	
        static void LaunchAppInSpecifiedCulture(string appPath, string uiCulture)
        {
            // Switch UI culture
            CultureInfo desiredUICulture = new CultureInfo(uiCulture);
            Thread.CurrentThread.CurrentUICulture = desiredUICulture;

            // Load app to test into a new AppDomain
            AppDomain childAppDomain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
            childAppDomain.ExecuteAssembly(appPath);
            AppDomain.Unload(childAppDomain);
        }
    }
}
