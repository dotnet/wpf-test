// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WPF.Printing.Common.Helpers
{
    using System;
    using System.Management;
    using Microsoft.Test.Logging;

    /// <summary>
    /// Manages interaction with Printers
    /// </summary>
    public static class PrinterManager
    {
        /// <summary>
        /// Check to see if a printer is installed.
        /// </summary>
        /// <param name="printer">Name of printer</param>
        /// <returns>
        /// True if 'printer' is found in list of installed printers.
        /// Printers must have a unique name, so only one match will exist if any.
        /// </returns>
        public static bool IsPrinterInstalled(string printer)
        {
            lock (locker)
            {
                SelectQuery query = new SelectQuery("Win32_Printer");
                query.QueryString = string.Format("SELECT * FROM Win32_Printer WHERE Name = '{0}'", printer);
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection result = searcher.Get();
                return (result != null && result.Count == 1);
            }
        }

        /// <summary>
        /// Install a printer driver
        /// </summary>
        /// <param name="name">name of printer driver</param>
        /// <param name="infpath">path to inf</param>
        /// <param name="filepath">path to install files</param>
        /// <returns>True if Printer Driver was successfully installed</returns>
        public static bool InstallPrinterDriver(string name, string infpath, string filepath)
        {
            int returnvalue = 0;

            ManagementClass cls = new ManagementClass("Win32_PrinterDriver");
            ManagementObject printerDriver = cls.CreateInstance();
            printerDriver["Name"] = name;
            printerDriver["InfName"] = infpath;
            printerDriver["FilePath"] = filepath;

            ManagementBaseObject inParams = cls.GetMethodParameters("AddPrinterDriver");
            inParams["DriverInfo"] = printerDriver;
            try
            {
                ManagementBaseObject outParams = cls.InvokeMethod("AddPrinterDriver", inParams, null);
                uint retval = (uint)outParams["ReturnValue"];
                returnvalue = (int)retval;
            }
            catch (ManagementException e)
            {
                GlobalLog.LogEvidence(e);
                returnvalue = -1;
            }

            if (returnvalue != 0)
            {
                GlobalLog.LogStatus("Printer Driver was not installed.");

                switch (returnvalue)
                {
                    case 5: GlobalLog.LogStatus("Access denied."); break;
                    case 1797: GlobalLog.LogStatus("The printer driver is unknown."); break;
                    case -1: GlobalLog.LogStatus("Exception was thrown."); break;
                    default : GlobalLog.LogStatus("Unknown return value."); break;
                }
            }

            return (returnvalue == 0);
        }

        /// <summary>
        /// Install printer
        /// </summary>
        /// <param name="printer">Name of the printer</param>
        /// <param name="port">Printing Port</param>
        /// <returns>true if install is successful</returns>
        public static bool InstallPrinter(string printer, string port)
        {
            return InstallPrinter(printer, port, string.Format("{0}\\inf\\ntprint.inf", Environment.GetEnvironmentVariable("windir")));
        }

        /// <summary>
        /// Install printer
        /// </summary>
        /// <param name="printer">Name of the printer</param>
        /// <param name="port">Printing Port</param>
        /// <param name="inf">path to inf file</param>
        /// <returns>true if install is successful</returns>
        public static bool InstallPrinter(string printer, string port, string inf)
        {
            if (IsPrinterInstalled(printer))
            {
                return true;
            }

            string command = string.Format("printui.dll,PrintUIEntry /if /b \"{0}\" /f \"{2}\" /r \"{1}\" /m \"{0}\"", printer, port, inf);

            ProcessWatcher.ExecuteProcess("rundll32.exe", command);

            return IsPrinterInstalled(printer);
        }

        /// <summary>
        /// Delete printer
        /// </summary>
        /// <param name="printer">name of printer</param>
        public static bool DeletePrinter(string printer)
        {
            GlobalLog.LogStatus("Trying to delete printer. [\"{0}\"]", printer);

            string command = string.Format("printui.dll,PrintUIEntry /q /dl /n \"{0}\"", printer);

            ProcessWatcher.ExecuteProcess("rundll32.exe", command);

            return !IsPrinterInstalled(printer);
        }

        /// <summary>
        /// Get the current default printer
        /// </summary>
        public static string GetDefaultPrinter()
        {
            lock (locker)
            {
                string printer = string.Empty;

                SelectQuery query = new SelectQuery("Win32_Printer");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                foreach (ManagementObject result in searcher.Get())
                {
                    bool isDefault = (bool)result["Default"];
                    if (isDefault)
                    {
                        printer = (string)result["Name"];
                    }
                }

                return printer;
            }
        }

        /// <summary>
        /// Set default printer
        /// </summary>
        /// <param name="printer">name of printer</param>
        public static bool SetDefaultPrinter(string printer)
        {
            GlobalLog.LogStatus("Trying to set printer as default. [\"{0}\"]", printer);

            string command = string.Format("printui.dll,PrintUIEntry /q /y /n \"{0}\"", printer);

            ProcessWatcher.ExecuteProcess("rundll32.exe", command);

            return GetDefaultPrinter() == printer;
        }

        /// <summary>
        /// Check to see if a printer is the default
        /// </summary>
        /// <param name="printer">name of printer</param>
        public static bool IsDefaultPrinter(string printer)
        {
            return (GetDefaultPrinter() == printer);
        }

        private static object locker = new object();
    }
}
