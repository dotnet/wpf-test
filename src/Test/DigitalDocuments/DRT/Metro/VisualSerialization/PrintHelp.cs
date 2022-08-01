// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
*
*  PrintHelp - Printing help functions
*
*
*
\***************************************************************************/
using System;
using System.Threading;

using System.IO;

using System.Windows;
using System.Runtime.InteropServices;
using System.Collections;

using System.Printing;
using System.Printing.Interop;
using System.Windows.Media;

using System.Windows.Forms;
using System.Drawing.Printing;
using DRT;

public class PrintHelp
{
    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    internal static extern IntPtr GlobalLock(HandleRef handle);

    [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    internal static extern int GlobalSize(HandleRef handle);

    /// <summary>
    /// SelectDevmode
    /// </summary>
    /// <returns></returns>
    public static byte[] SelectDevmode(out string printerName)
    {
        printerName = null;

        PrintDialog prndlg = new PrintDialog();

        prndlg.AllowSomePages = true;
        prndlg.PrinterSettings = new PrinterSettings();
        prndlg.PrinterSettings.MinimumPage = 1;
        prndlg.PrinterSettings.MaximumPage = 1;
        prndlg.PrinterSettings.FromPage = 1;
        prndlg.PrinterSettings.ToPage = 1;

        if (prndlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            IntPtr hDevMode = IntPtr.Zero;

            byte[] devmode = null;

            try
            {
                hDevMode = prndlg.PrinterSettings.GetHdevmode();

                IntPtr pDevMode = GlobalLock(new HandleRef(null, hDevMode));

                int size = GlobalSize(new HandleRef(null, hDevMode));

                devmode = new byte[size];

                Marshal.Copy(pDevMode, devmode, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(hDevMode);
            }

            printerName = prndlg.PrinterSettings.PrinterName;

            return devmode;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// SelectPrintQueue
    /// </summary>
    /// <returns></returns>
    public static PrintQueue SelectPrintQueue()
    {
        PrintDialog prndlg = new PrintDialog();

        prndlg.AllowSomePages              = true;
        prndlg.PrinterSettings             = new PrinterSettings();
        prndlg.PrinterSettings.MinimumPage = 1;
        prndlg.PrinterSettings.MaximumPage = 1;
        prndlg.PrinterSettings.FromPage    = 1;
        prndlg.PrinterSettings.ToPage      = 1;

        if (prndlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            return GetPrintQueue(prndlg.PrinterSettings);
        }
        else
        {
            return null;
        }
    }

    public static PrintQueue GetPrintQueue(PrinterSettings set)
    {
        IntPtr hDevMode = IntPtr.Zero;

        PrintTicket pt = null;

        try
        {
            hDevMode = set.GetHdevmode();

            IntPtr pDevMode = GlobalLock(new HandleRef(null, hDevMode));

            int size = GlobalSize(new HandleRef(null, hDevMode));

            byte[] devmode = new byte[size];

            Marshal.Copy(pDevMode, devmode, 0, size);

            PrintTicketConverter ptc = new PrintTicketConverter(set.PrinterName, 1);

            pt = ptc.ConvertDevModeToPrintTicket(devmode);
        }
        finally
        {
            Marshal.FreeHGlobal(hDevMode);
        }

        if (pt == null)
        {
            return null;
        }
        else
        {
            string fullname = set.PrinterName;
            string server = null;
            string printer = fullname;

            // \\server\printer
            if ((fullname.Length > 3) && (fullname[0] == '\\') && (fullname[1] == '\\'))
            {
                int i = fullname.IndexOf('\\', 2);

                if (i > 0)
                {
                    server = fullname.Substring(0, i);
                    printer = fullname.Substring(i + 1);
                }
            }

            PrintQueue queue = new PrintQueue(new PrintServer(server), printer);

            queue.UserPrintTicket = pt;

            return queue;
        }
    }

    public static PrintQueue GetDefaultPrintQueue(DrtBase drt)
    {
        PrintQueue queue = null;

        try
        {
            System.Drawing.Printing.PrintDocument doc = new System.Drawing.Printing.PrintDocument();

            queue = GetPrintQueue(doc.PrinterSettings);
        }
        catch (Exception e)
        {
            if (drt != null)
            {
                drt.ConsoleOut.WriteLine(e.ToString());
            }
            else
            {
                throw e;
            }
        }

        return queue;
    }
}
