// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WFCTestLib.Util {

    using System;
    using System.Text;
	 using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
    public class InteropUtil {
        public static int MAKELONG(int low, int high) {
            return(high << 16) | (low & 0xffff);
        }

        public static int MAKELPARAM(int low, int high) {
            return(high << 16) | (low & 0xffff);
        }

        public static int HIWORD(int n) {
            return(n >> 16) & 0xffff;
        }

        public static int LOWORD(int n) {
            return n & 0xffff;
        }

        public static int SignedHIWORD(int n) {
            int i = (int)(short)((n >> 16) & 0xffff);

            i = i << 16;
            i = i >> 16;

            return i;
        }

        public static int SignedLOWORD(int n) {
            int i = (int)(short)(n & 0xFFFF);

            i = i << 16;
            i = i >> 16;

            return i;
        }

        /// <summary>
        ///     Computes the string size that should be passed to a typical Win32 call.
        ///     This will be the character count under NT, and the ubyte count for Win95.
        /// </summary>
        /// <param name='s'>
        ///     The string to get the size of.
        /// </param>
        /// <returns>
        ///     the count of characters or bytes, depending on what the jdirect
        ///     call wants
        /// </returns>
        public static int GetJDirectStringLength(String s) {
            if (s == null) {
                return 0;
            }

            if (System.Runtime.InteropServices.Marshal.SystemDefaultCharSize == 2) {
                return s.Length;
            } else {
                if (s.Length == 0) {
                    return 0;
                }
                if (s.IndexOf('\0') > -1) {
                    return GetEmbededNullStringLengthAnsi(s);
                } else {
                    return lstrlen(s);
                }
            }
        }

        private static int GetEmbededNullStringLengthAnsi(String s) {
            int n = s.IndexOf('\0');
            if (n > -1) {
                String left = s.Substring(0, n);
                String right = s.Substring(n+1);
                return GetJDirectStringLength(left) + GetEmbededNullStringLengthAnsi(right) + 1;
            } else {
                return GetJDirectStringLength(s);
            }
        }

        //
        // Native methods -- we need to declare these guys internally and just call them here for
        // security purposes.  Apparently, you can't assert unmanaged code permission on a native
        // declaration, so we assert it on these methods below, and they call the native versions
        // of the methods.
        //
        public static int lstrlen(String s) {
            return InternalDeclares.lstrlen(s);
        }

        public static int RegisterWindowMessage(String msg) {
            return InternalDeclares.RegisterWindowMessage(msg);
        }

        public static int GetModuleFileName(int hModule, StringBuilder buffer, int length) {
            return InternalDeclares.GetModuleFileName(hModule, buffer, length);
        }

        public static int PostMessage(int hwnd, int msg, int wparam, int lparam) {
            return InternalDeclares.PostMessage(hwnd, msg, wparam, lparam);
        }
    
        public static int GetSystemMetrics(int nIndex) {
            return InternalDeclares.GetSystemMetrics(nIndex);
        }

        public static int SendMessage(int hWnd, int Msg, int wParam, int lParam) {
            return InternalDeclares.SendMessage(hWnd, Msg, wParam, lParam);
        }

        public static int GetActiveWindow() {
            return InternalDeclares.GetActiveWindow();
        }

        public static int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount) {
            return InternalDeclares.GetWindowText(hWnd, lpString, nMaxCount);
        }

        public static int FindWindow(string lpClassName, string lpWindowName) {
            return InternalDeclares.FindWindow(lpClassName, lpWindowName);
        }

        public static int FindWindowEx(int hwndParent, int hwndChildAfter, string lpClassName, string lpWindowName) {
            return InternalDeclares.FindWindowEx(hwndParent, hwndChildAfter, lpClassName, lpWindowName);
        }

        public static int GetModuleHandle(string modName) {
            return InternalDeclares.GetModuleHandle(modName);
        }

        public static IntPtr SelectObject(IntPtr hDC, IntPtr hObject) {
            return InternalDeclares.SelectObject(hDC, hObject);
        }

        public static IntPtr LoadLibrary(string libFilename) {
            return InternalDeclares.LoadLibrary(libFilename);
        }

        public static int SHGetFolderPath(int hwndOwner, StringBuilder lpszPath, int nFolder, bool fCreate) {
            return InternalDeclares.SHGetFolderPath(hwndOwner, lpszPath, nFolder, fCreate);
        }

        public static int SHGetSpecialFolderLocation(int hwndOwner, int nFolder, int[] pidl) {
            return InternalDeclares.SHGetSpecialFolderLocation(hwndOwner, nFolder, pidl);
        }

        public static int SHGetPathFromIDList(int pidl, StringBuilder lpszPath) {
            return InternalDeclares.SHGetPathFromIDList(pidl, lpszPath);
        }
    
        public static bool SystemParametersInfo(int nAction, int nParam, RECT rc, int nUpdate) {
            return InternalDeclares.SystemParametersInfo(nAction, nParam, rc, nUpdate);
        }

        public static bool SystemParametersInfo(int nAction, int nParam, HIGHCONTRAST_I rc, int nUpdate) {
            return InternalDeclares.SystemParametersInfo(nAction, nParam, rc, nUpdate);
        }

		public static bool SystemParametersInfo(int nAction, int nParam, NONCLIENTMETRICS rc, int nUpdate) {
			return InternalDeclares.SystemParametersInfo(nAction, nParam, rc, nUpdate);
		}
		
		public static bool SystemParametersInfo(int nAction, int nParam, int[] rc, int nUpdate) {
            return InternalDeclares.SystemParametersInfo(nAction, nParam, rc, nUpdate);
        }
    
        public static bool GetComputerName(StringBuilder lpBuffer, int[] nSize) {
            return InternalDeclares.GetComputerName(lpBuffer, nSize);
        }

        public static bool GetUserName(StringBuilder lpBuffer, int[] nSize) {
            return InternalDeclares.GetUserName(lpBuffer, nSize);
        }
    
        public static int GetProcessWindowStation() {
            return InternalDeclares.GetProcessWindowStation();
        }

        public static bool LookupAccountName(string machineName, string accountName, byte[] sid, ref int sidLen, StringBuilder domainName, ref int domainNameLen, out int peUse) {
            return InternalDeclares.LookupAccountName(machineName, accountName, sid, ref sidLen, domainName, ref domainNameLen, out peUse);
        }

        public static int GetDoubleClickTime() {
            return InternalDeclares.GetDoubleClickTime();
        }

        public static IntPtr PostMessage(IntPtr hWnd, int msg, short wParam, short lParam) {
            return InternalDeclares.PostMessage(hWnd, msg, wParam, lParam);
        }

		  // added by t-tclif, 10july2003, to support scrollbar scrolling messages
        public static IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam){
            return InternalDeclares.SendMessage(hWnd, msg, wParam, lParam);
        }

        // Windows XP Theming APIs.  If called on pre-XP OS, will throw DllNotFoundException
        public static bool IsAppThemed() {
            try {
                return IsAppThemedHelper();
            }
            catch (DllNotFoundException) {
                return false;
            }
        }

        private static bool IsAppThemedHelper() {
            return InternalDeclares.IsAppThemed();
        }

        public static bool IsThemeActive() {
            return InternalDeclares.IsThemeActive();
        }

		public static int GetDeviceCaps(IntPtr hDC, int nIndex) {
			return InternalDeclares.GetDeviceCaps(hDC, nIndex);
		}		

		
        /// <summary>
        ///     To work around the problem that key sent via SendKeys.SendWait() 
		///  	doesn't always reach control, we will post KeyDown message for given key.            
		///
		///  	*** Need to call Application.DoEvents() after calling the method ***
		///
        /// </summary>
        /// <param name='hWnd'>
        ///     Handle to the control we want to send key to.
        /// </param>      
        /// <param name='keyValue'>
        ///     code of the key to be sent - code is the KeyValue from KeyEventArgs (KeyDown event).
        /// </param>
		///
		
		public const int WM_KEYDOWN = 0x0100;
		public const int WM_KEYUP = 0x0101;
		
		public static void ImitateSendKey(IntPtr hWnd, int keyValue) {
			InternalDeclares.PostMessage((int)hWnd, WM_KEYDOWN, keyValue, 0);
			// we only need to post KeyDown to send the given key
			// if we post KeyUp, the key will be sent second time
		   // InternalDeclares.PostMessage((int)hWnd, WM_KEYUP, keyValue, 0);
		    System.Windows.Forms.Application.DoEvents();
		}

        [ SuppressUnmanagedCodeSecurity() ]
        private class InternalDeclares {
            [DllImport("kernel32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int lstrlen(String s);

            [DllImport("user32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int RegisterWindowMessage(String msg);

            [DllImport("kernel32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int GetModuleFileName(int hModule, StringBuilder buffer, int length);

            [DllImport("user32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int PostMessage(int hwnd, int msg, int wparam, int lparam);
        
            [DllImport("user32", ExactSpelling=true, CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int GetSystemMetrics(int nIndex);

            [DllImport("User32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);
    
            [DllImport("User32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int GetActiveWindow();
    
            [DllImport("User32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);
    
            [DllImport("User32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int FindWindow(string lpClassName, string lpWindowName);

            [DllImport("User32", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string lpClassName, string lpWindowName);

            [DllImport("kernel32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern int GetModuleHandle(string modName);

            [DllImport("Gdi32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
            internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            [DllImport("Kernel32", CharSet=System.Runtime.InteropServices.CharSet.Auto, SetLastError=true)]
            internal static extern IntPtr LoadLibrary(string libFilename);

            // From XSystemInformation
            [DllImport("shell32", CharSet=CharSet.Auto)]
            internal static extern int SHGetFolderPath(int hwndOwner, StringBuilder lpszPath, int nFolder, bool fCreate);

            [DllImport("shell32", CharSet=CharSet.Auto)]
            internal static extern int SHGetSpecialFolderLocation(int hwndOwner, int nFolder, int[] pidl);

            [DllImport("shell32", CharSet=CharSet.Auto)]
            internal static extern int SHGetPathFromIDList(int pidl, StringBuilder lpszPath);
        
            [DllImport("user32", CharSet=CharSet.Auto)]
            internal static extern bool SystemParametersInfo(int nAction, int nParam, RECT rc, int nUpdate);

            [DllImport("user32", CharSet=CharSet.Auto)] 
            internal static extern bool SystemParametersInfo(int nAction, int nParam, HIGHCONTRAST_I rc, int nUpdate);


			[DllImport("user32", CharSet=CharSet.Auto)] 
			internal static extern bool SystemParametersInfo(int nAction, int nParam, [In,Out] NONCLIENTMETRICS rc, int nUpdate);

            [DllImport("user32", CharSet=CharSet.Auto)]
            internal static extern bool SystemParametersInfo(int nAction, int nParam, int[] rc, int nUpdate);
        
            [DllImport("kernel32", CharSet=CharSet.Auto)]
            internal static extern bool GetComputerName(StringBuilder lpBuffer, int[] nSize);

            [DllImport("advapi32", CharSet=CharSet.Auto)]
            internal static extern bool GetUserName(StringBuilder lpBuffer, int[] nSize);      
        
            [DllImport("user32")]
            internal static extern int GetProcessWindowStation();

            [DllImport("advapi32", CharSet=CharSet.Auto, SetLastError=true)]
            internal static extern bool LookupAccountName(string machineName, string accountName, byte[] sid, ref int sidLen, StringBuilder domainName, ref int domainNameLen, out int peUse);

            [DllImport("user32", CharSet=CharSet.Auto)]
            internal static extern int GetDoubleClickTime();

            [DllImport("user32", CharSet=CharSet.Auto)]
            internal static extern IntPtr PostMessage(IntPtr hWnd, int msg, short wParam, short lParam);

				// added by t-tclif, 10july2003, to support scrollbar scrolling messages
				[DllImport("user32", CharSet = CharSet.Auto)]
				public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

            // Windows XP Theming APIs.  If called on pre-XP OS, will throw DllNotFoundException
            [DllImport("uxtheme", CharSet=CharSet.Auto)]
            internal static extern bool IsAppThemed();       // Is this app themed?

            [DllImport("uxtheme", CharSet=CharSet.Auto)]
            internal static extern bool IsThemeActive();     // Is theming enabled?

			//
			//  For XScreen tests to verify BitsPerPixel.
			//
			[DllImport("Gdi32", CharSet=System.Runtime.InteropServices.CharSet.Auto)]
			internal static extern int GetDeviceCaps(IntPtr hDC, int nIndex);
		}
    }

    //
    // Types used by Win32 methods
    //
    public struct HIGHCONTRAST_I {
        public int cbSize;
        public int dwFlags;
        public int lpszDefaultScheme;
    }

    public struct RECT {
        public int left;
        public int top;
        public int right;
        public int bottom;
    
        public RECT(int left, int top, int right, int bottom) {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public static RECT FromXYWH(int x, int y, int width, int height) {
            return new RECT(x,
                            y,
                            x + width,
                            x + height);
        }
    }

	//  Added to support new SystemInformation members for Whidbey TopAPI feature set.
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public class LOGFONT
	{
		public int lfHeight;
		public int lfWidth;
		public int lfEscapement;
		public int lfOrientation;
		public int lfWeight;
		public byte lfItalic;
		public byte lfUnderline;
		public byte lfStrikeOut;
		public byte lfCharSet;
		public byte lfOutPrecision;
		public byte lfClipPrecision;
		public byte lfQuality;
		public byte lfPitchAndFamily;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
		public string   lfFaceName;
	}

	//  Added to support new SystemInformation members for Whidbey TopAPI feature set.
	[StructLayout(LayoutKind.Sequential)]
	public class NONCLIENTMETRICS
	{
		public int cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
		public int      iBorderWidth; 
		public int      iScrollWidth; 
		public int      iScrollHeight; 
		public int      iCaptionWidth; 
		public int      iCaptionHeight; 
		[MarshalAs(UnmanagedType.Struct)]
		public LOGFONT  lfCaptionFont; 
		public int      iSmCaptionWidth; 
		public int      iSmCaptionHeight; 
		[MarshalAs(UnmanagedType.Struct)]
		public LOGFONT  lfSmCaptionFont; 
		public int      iMenuWidth; 
		public int      iMenuHeight; 
		[MarshalAs(UnmanagedType.Struct)]
		public LOGFONT  lfMenuFont; 
		[MarshalAs(UnmanagedType.Struct)]
		public LOGFONT  lfStatusFont; 
		[MarshalAs(UnmanagedType.Struct)]
		public LOGFONT  lfMessageFont; 
	}
}
