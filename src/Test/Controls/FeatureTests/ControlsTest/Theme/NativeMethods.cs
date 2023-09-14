using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections;

//

namespace Avalon.Test.ComponentModel.UnitTests
{

    public  class NativeMethods
    {
        internal const int WM_THEMECHANGED = 0x031A;

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        internal static extern int IsThemeActive();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr FindWindow(string className, string windowName);
    }

}
