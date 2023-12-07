// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ReflectTools {
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Drawing.Printing;
	using System.Globalization;
	using System.IO;
	using System.Net;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Security.Permissions;
	using System.Threading;
	using System.Windows.Forms;
	using System.Windows.Forms.VisualStyles;
	//using System.Windows.Forms.Printing;
	using System.CodeDom.Compiler;
	using Microsoft.Win32;
	using System.Reflection.Emit;
	using System.Security;
	using System.Security.Principal;
    using System.Resources;

	public static class SafeMethods {

        [SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        public static void HideSecurityBubble(Form f)
        {
            if (null == f) { throw new ArgumentNullException("f"); }
            if (!f.Visible) { throw new ArgumentException("The form must be visible for the bubble to be hidden"); }
            //Bail out if there is no bubble to begin with
            if (!f.IsRestrictedWindow) { return; }

            FormBorderStyle prevStyle = f.FormBorderStyle;            
            try
            {
                SafeMethods.Activate(f);
                Application.DoEvents();
                //Ensure that it is sizable to allow the menu to be activated
                f.FormBorderStyle = FormBorderStyle.Sizable;

                Thread t = new Thread((ThreadStart)delegate
                {
                    Thread.Sleep(500);
                    SafeMethods.SendWait("{ESC}");
                });
                t.Start();
                SafeMethods.SendWait("% S");//Alt+space+s (size command)
                t.Join();
                Application.DoEvents();
                Thread.Sleep(500);
                Application.DoEvents();
            }
            finally
            { f.FormBorderStyle = prevStyle; }
        }

        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static void SetFormIcon(Form f, Icon ico)
        {
            System.Collections.Specialized.BitVector32 bv = (System.Collections.Specialized.BitVector32)typeof(Form).GetField("formState", ~BindingFlags.Public).GetValue(f);
            System.Collections.Specialized.BitVector32.Section sc = (System.Collections.Specialized.BitVector32.Section)typeof(Form).GetField("FormStateIsRestrictedWindow", ~BindingFlags.Public).GetValue(null);

            int prev = bv[sc];
            bv[sc] = 0;
            typeof(Form).GetField("formState", ~BindingFlags.Public).SetValue(f, bv);
            try
            {
                f.Icon = ico;
                Application.DoEvents();
            }
            finally
            {
                bv = (System.Collections.Specialized.BitVector32)typeof(Form).GetField("formState", ~BindingFlags.Public).GetValue(f);
                bv[sc] = prev;
                typeof(Form).GetField("formState", ~BindingFlags.Public).SetValue(f, bv);
            }
        }


        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static void SetWindowState(Form f, FormWindowState state)
        {
            System.Collections.Specialized.BitVector32 bv = (System.Collections.Specialized.BitVector32)typeof(Form).GetField("formState", ~BindingFlags.Public).GetValue(f);
            System.Collections.Specialized.BitVector32.Section sc = (System.Collections.Specialized.BitVector32.Section)typeof(Form).GetField("FormStateIsRestrictedWindow", ~BindingFlags.Public).GetValue(null);

            int prev = bv[sc];
            bv[sc] = 0;
            typeof(Form).GetField("formState", ~BindingFlags.Public).SetValue(f, bv);
            try
            {
                f.WindowState = state;
                Application.DoEvents();
            }
            finally
            {
                bv = (System.Collections.Specialized.BitVector32)typeof(Form).GetField("formState", ~BindingFlags.Public).GetValue(f);
                bv[sc] = prev;
                typeof(Form).GetField("formState", ~BindingFlags.Public).SetValue(f, bv);
            }
        }

        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static string WriteResourceToFile(string resourceName, object value)
        {
            string filePath = System.IO.Path.GetTempFileName();
            ResourceWriter rw = new ResourceWriter(filePath);
            rw.AddResource(resourceName, value);
            rw.Generate();
            rw.Close();
            return filePath;
        }
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static ResourceReader CreateResourceReader(string fileName)
        { return new ResourceReader(fileName); }

		[SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
		public static void InvokeCloseForm(Form f)
		{
			f.BeginInvoke(new FTCD(CloseForm), f);
		}
		private delegate void FTCD(Form f);
		private static void CloseForm(Form fToClose)
		{ fToClose.Close(); }

        private static string s_events;


		//
		// SECURITY: The following methods are here to allow tests to perform actions that
		//           might not be permitted without the appropriate permissions but are "safe"
		//           to use in a test.
		//
		// All properties and methods that are directly being tested should not use these methods
		// and should directly call the method or property being tested in order to ensure proper
		// security coverage.
		//


        public static void AddUrlToHistory(string targetUrl)
        {
            PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
            permissions.Assert();

            Form f = new Form();
            WebBrowser browser = new WebBrowser();
            f.Controls.Add(browser);
            f.Show();

            NavigateTo(browser, targetUrl, 10000);

            // it really takes that long on IA64 machines
            for(int i=0; i<100; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(200);
            }
                
            f.Close();
            f.Dispose();
            CodeAccessPermission.RevertAssert();

        }

        private static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            s_events += "DocumentCompleted ";
        }


        private static bool NavigateTo(WebBrowser wb, string url, int milliseconds)
        {
            wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);

            DateTime start = DateTime.Now;
            long stoptime = start.AddMilliseconds(milliseconds).Ticks;
            bool navigated = false;

            s_events = "";

            SafeMethods.Navigate(wb, url);
            navigated = false;
            while (DateTime.Now.Ticks < stoptime)
            {
                Application.DoEvents();
                if (s_events.Contains("DocumentCompleted"))
                {
                    navigated = true;
                    break;
                }
                Thread.Sleep(500);
            }
            return navigated;
        }


        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void AddRenderItemImageHandler(ToolStrip ts, ToolStripItemImageRenderEventHandler h)
        {
            ts.Renderer.RenderItemImage += new ToolStripItemImageRenderEventHandler(h);
        }


		//
		// Control members
		//
		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Control GetParent(Control c) {
			return c.Parent;
		}

        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void SetCapture(Control c, bool value)
        { c.Capture = value; }

	[System.Security.Permissions.RegistryPermission(System.Security.Permissions.SecurityAction.Assert, Unrestricted=true)]
	public static Microsoft.Win32.RegistryKey GetRegistryCurrentUserName()
	{ return Microsoft.Win32.Registry.CurrentUser; }


        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static AccessibleObject GetAccessibilityObject(Control c)
		{
			return c.AccessibilityObject;
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Form FindForm(Control c) {
			return c.FindForm();
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void Focus(Control c) {
			c.Focus();
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void Select(Control c) {
			c.Select();
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetRegion(Control c, Region r)
		{
			c.Region = r;
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetActiveControl(ContainerControl c, Control ctl) {
			c.ActiveControl = ctl;
		}

		[UIPermission(SecurityAction.Assert, Clipboard = UIPermissionClipboard.AllClipboard)]
		public static void Paste(TextBoxBase tb) {
			tb.Paste();
		}

		[UIPermission(SecurityAction.Assert, Clipboard = UIPermissionClipboard.AllClipboard)]
		public static void Clipboard_SetDataObject(object data)
		{
			Clipboard.SetDataObject(data);
		}

		[UIPermission(SecurityAction.Assert, Clipboard = UIPermissionClipboard.AllClipboard)]
		public static IDataObject Clipboard_GetDataObject()
		{
			return Clipboard.GetDataObject();
		}


        // If Clipboard does not contain string then this method returns
        // "no String.class data present"
        [UIPermission(SecurityAction.Assert, Clipboard = UIPermissionClipboard.AllClipboard)]
        public static string GetClipboardString()
        {
            IDataObject o = Clipboard.GetDataObject();
            String got;

            if (!o.GetDataPresent(typeof(String)))
                got = "no String.class data present";
            else
                got = (String)o.GetData(typeof(String));

            return got;
        }


		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static ContainerControl GetContainerControl(ErrorProvider p) {
			return p.ContainerControl;
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void RichTextBoxLoadFile(RichTextBox rtb, string filename) {
			rtb.LoadFile(filename);
		}


        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void SetAutoCompleteSource(ComboBox cb, AutoCompleteSource acs)
        {
            cb.AutoCompleteSource = acs;
        }


        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string GetEscapedCodeBase()
        {
            return Assembly.GetEntryAssembly().Location;
        }


		//
		// ControlPaint members
		//
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static IntPtr CreateHBitmapTransparencyMask(Bitmap bitmap) {
			return ControlPaint.CreateHBitmapTransparencyMask(bitmap);
		}

		//
		// Form members
		//
		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void Activate(Form f) {
			f.Activate();
		}

        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        public static FormCollection GetOpenForms()
        {
            return Application.OpenForms;
        }


		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Form GetMdiParent(Form f) {
			return f.MdiParent;
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Form GetOwner(Form f) {
			return f.Owner;
		}

		//
		// Environment members
		//
		[EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
		public static OperatingSystem GetOSVersion() {
			return Environment.OSVersion;
		}

        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string GetTempPath()
        {
            return System.IO.Path.GetTempPath();
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetCurrentDirectory() {
			return Environment.CurrentDirectory;
		}
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static void SetCurrentDirectory(string value)
        { Environment.CurrentDirectory = value; }

		[FileIOPermission(SecurityAction.Assert, AllLocalFiles = FileIOPermissionAccess.PathDiscovery)]
		public static string GetExecutablePath()
		{
			return Application.ExecutablePath;
		}

        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string GetEnvironmentVariable(string variable)
        {
            return Environment.GetEnvironmentVariable(variable);
        }


		[
		EnvironmentPermission(SecurityAction.Assert, Unrestricted = true),
		SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)
		]
		public static string GetMachineName() {
			return Environment.MachineName;
		}

		//
		// SendKeys members
		//
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void SendWait(string keys) {
			SendKeys.SendWait(keys);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void Send(string keys) {
			SendKeys.Send(keys);
		}

        //
        // EmulateKeyPress members
        //
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        // for info on "keybd_event", see:
        // "http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/keybd_event.asp"
        // "http://www.pinvoke.net/default.aspx/user32.keybd_event"
        // Notes:
        // Windows NT/2000/XP: The keybd_event function can toggle the NUM LOCK, CAPS LOCK, and SCROLL LOCK keys.
        // Windows 95/98/Me: The keybd_event function can toggle only the CAPS LOCK and SCROLL LOCK keys. It cannot toggle the NUM LOCK key.

        // simulate user pressing and releasing key on keyboard
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static void EmulateKeyPress(Keys keyVal)
        {
            // emulate user pressing and releasing the key, with typical pause between
            EmulateKeyPress(keyVal, true);          // press key down
            System.Threading.Thread.Sleep(100);
            EmulateKeyPress(keyVal, false);         // let key up
        }

        // simulate user pressing or releasing key on keyboard
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static void EmulateKeyPress(Keys keyVal, bool bKeydown)
        {
            const int KEYEVENTF_EXTENDEDKEY = 0x1;
            const int KEYEVENTF_KEYUP = 0x2;

            // make sure no high-order bits are set
            if (((int)keyVal & ~0xff) != 0)
            {
                throw new ArgumentException("EmulateKeyPress: only low-order bits 0xFF can be set in keyVal");
            }

            // NUMLOCK cannot be toggled in Windows 95/98/Me
            if (keyVal == Keys.NumLock)
            {
                // only incur hit of checking OS verison if need to (ie when using NUMLOCK)
                if (WFCTestLib.Util.Utilities.IsWin9x)
                {
                    throw new PlatformNotSupportedException("EmulateKeyPress: Cannot toggle NUMLOCK key in this OS version");
                }
            }

            byte keyCode = (byte)keyVal;        // convert Key enum into byte value needed by keybd_event

            if (bKeydown)
            {
                // simulate key down
                keybd_event(keyCode, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            }
            else
            {
                // simulate key up
                keybd_event(keyCode, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, (UIntPtr)0);
            }
        }

        //
		// Graphics members
		//
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Graphics GraphicsFromHdc(IntPtr h) {
			return Graphics.FromHdc(h);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void GraphicsReleaseHdc(Graphics g, IntPtr h) {
			g.ReleaseHdc(h);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static IntPtr BitmapGetHicon(Bitmap b) {
			return b.GetHicon();
		}

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static Bitmap BitmapFromHicon(IntPtr hIcon)
        {
            return Bitmap.FromHicon(hIcon);
        }

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Bitmap CreateBitmap(string filename) {
			return new Bitmap(filename);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Icon CreateIcon(string filename) {
			return new Icon(filename);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Metafile CreateMetafile(IntPtr hdc, Rectangle rect) {
			return new Metafile(hdc, rect);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static Metafile CreateMetafile(string filename) {
			return new Metafile(filename);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void ImageSave(Image i, string s) {
			i.Save(s);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void ImageSave(Image i, string s, ImageFormat f)
		{
			i.Save(s, f);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Font FontFromHfont(IntPtr hFont) {
			return Font.FromHfont(hFont);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static IntPtr ToHfont(Font font)
		{
			return font.ToHfont();
		}

		[ComVisible(false), StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
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
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
			public string lfFaceName;
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Font FontFromLogFont(LOGFONT lf, IntPtr hdc)
		{
			return Font.FromLogFont(lf, hdc);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Font FontFromLogFont(LOGFONT lf)
		{
			return Font.FromLogFont(lf);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void ToLogFont(Font f, LOGFONT lf, Graphics g)
		{
			f.ToLogFont(lf, g);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void ToLogFont(Font f, LOGFONT lf)
		{
			f.ToLogFont(lf);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Region RegionFromHrgn(IntPtr hrgn) {
			return Region.FromHrgn(hrgn);
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetCursorPosition(Point p) {
			Cursor.Position = p;
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static BufferedGraphics BufferedGraphicsAllocate(Graphics gfx, Rectangle bounds)
		{
			return BufferedGraphicsManager.Current.Allocate(gfx, bounds);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static BufferedGraphics BufferedGraphicsAllocate(BufferedGraphicsContext context, Graphics gfx, Rectangle bounds)
		{
			return context.Allocate(gfx, bounds);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static IntPtr GetHdc(Graphics gfx)
		{
			return gfx.GetHdc();
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void ReleaseHdc(Graphics gfx, IntPtr hdc)
		{
			gfx.ReleaseHdc(hdc);
		}

		//
		// Printing members
		//
		[PrintingPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetPrinterName(PrinterSettings ps, string name) {
			ps.PrinterName = name;
		}

		[PrintingPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetPrinterName(PrinterSettings ps) {
			return ps.PrinterName;
		}

		public static IntPtr GetHdevnames(PrinterSettings ps)
		{
			PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.AllPrinting);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.UnmanagedCode);

			permissions.Assert();
			IntPtr result = ps.GetHdevnames();
			CodeAccessPermission.RevertAssert();

			return result;
		}

		public static IntPtr GetHdevmode(PrinterSettings ps) 
		{
			PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.AllPrinting);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.UnmanagedCode);

			permissions.Assert();
			IntPtr result = ps.GetHdevmode();
			CodeAccessPermission.RevertAssert();

			return result;
		}

		public static void CopyToHdevmode(PageSettings ps, IntPtr ptr) 
		{
			PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.AllPrinting);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.UnmanagedCode);

			permissions.Assert();
			ps.CopyToHdevmode(ptr);
			CodeAccessPermission.RevertAssert();
		}

		public static void SetHdevmode(PageSettings ps, IntPtr ptr)
		{
			PermissionSet permissions = new PermissionSet(PermissionState.Unrestricted);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.AllPrinting);
			permissions.AddPermission(WFCTestLib.Util.LibSecurity.UnmanagedCode);

			permissions.Assert();
			ps.SetHdevmode(ptr);
			CodeAccessPermission.RevertAssert();
		}

		[PrintingPermission(SecurityAction.Assert, Unrestricted = true)]
		public static PrinterSettings.StringCollection GetInstalledPrinters() {
			return PrinterSettings.InstalledPrinters;
		}

		[PrintingPermission(SecurityAction.Assert, Level = PrintingPermissionLevel.SafePrinting)]
		public static PrintController GetPrintController(PrintDocument d) {
			return d.PrintController;
		}

		[PrintingPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void Print(PrintDocument d)
		{
			d.Print();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static void SetPrintFileName(PrinterSettings ps, string name)
		{
			ps.PrintFileName = name;
		}

		//
		// FileDialog members
		//
		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetFileDialogFileName(System.Windows.Forms.FileDialog d) {
			return d.FileName;
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetFileDialogFileName(System.Windows.Forms.FileDialog d, string filename) {
			d.FileName = filename;
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetFileDialogInitialDirectory(System.Windows.Forms.FileDialog d, string dir) {
			d.InitialDirectory = dir;
		}

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void SetFileDialogCheckFileExists(System.Windows.Forms.FileDialog d, bool flag)
        {
            d.CheckFileExists = flag;
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void SetFileDialogTitle(System.Windows.Forms.FileDialog d, string title)
        {
            d.Title = title;
        }

        //
		// Reflection members
		//
		// Need to assert full trust because if we don't have the permissions that
		// the target method demands, then we don't even see it through reflection.
		//
		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static ConstructorInfo[] GetConstructors(Type t)
		{
			return t.GetConstructors();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static ConstructorInfo[] GetConstructors(Type t, BindingFlags flags) {
			return t.GetConstructors(flags);
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static MethodInfo[] GetMethods(Type t)
		{
			return t.GetMethods();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static MethodInfo[] GetMethods(Type t, BindingFlags flags) {
			return t.GetMethods(flags);
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static FieldInfo[] GetFields(Type t) {
			return t.GetFields();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static FieldInfo[] GetFields(Type t, BindingFlags flags)
		{
			return t.GetFields(flags);
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static MemberInfo[] GetMembers(Type t) {
			return t.GetMembers();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static MemberInfo[] GetMembers(Type t, BindingFlags flags)
		{
			return t.GetMembers(flags);
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static EventInfo[] GetEvents(Type t)
		{
			return t.GetEvents();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static EventInfo[] GetEvents(Type t, BindingFlags flags)
		{
			return t.GetEvents(flags);
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static Type[] GetInterfaces(Type t)
		{
			return t.GetInterfaces();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static Type[] GetNestedTypes(Type t)
		{
			return t.GetNestedTypes();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static Type[] GetNestedTypes(Type t, BindingFlags flags)
		{
			return t.GetNestedTypes(flags);
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static PropertyInfo[] GetProperties(Type t)
		{
			return t.GetProperties();
		}

		[PermissionSetAttribute(SecurityAction.Assert, Name = "FullTrust")]
		public static PropertyInfo[] GetProperties(Type t, BindingFlags flags)
		{
			return t.GetProperties(flags);
		}

		//
		// Application members
		//
		public static string SafeTopLevelCaptionFormat {
			[UIPermission(SecurityAction.Assert, Unrestricted = true)]
			set {
				Application.SafeTopLevelCaptionFormat = value;
			}
		}

		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void AddThreadExceptionHandler(ThreadExceptionEventHandler h) {
			Application.ThreadException += h;
		}

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void Abort(Thread t)
        {
            t.Abort();
        }


		//NOTE: The permissions on this now are just screwy
		[SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void ApplicationRestart() {
			Application.Restart();
		}



		//
		// Miscellaneous methods
		//
		[SecurityPermission(SecurityAction.Assert, ControlThread = true)]
		public static void ThreadAbort(Thread t) {
			t.Abort();
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static FileStream CreateFileStream(string filename, FileMode mode) {
			return new FileStream(filename, mode);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static FileStream CreateFileStream(string filename, FileMode mode, FileAccess access) {
			return new FileStream(filename, mode, access);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static FileStream CreateFileStream(string filename, FileMode mode, FileAccess access, FileShare share) {
			return new FileStream(filename, mode, access, share);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static StreamReader CreateStreamReader(string path) {
			return new StreamReader(path);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static StreamReader CreateStreamReader(Stream s) {
			return new StreamReader(s);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static StreamWriter CreateStreamWriter(string path) {
			return new StreamWriter(path);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static StreamWriter CreateStreamWriter(string path, bool append) {
			return new StreamWriter(path, append);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static StreamWriter CreateStreamWriter(Stream s) {
			return new StreamWriter(s);
		}

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static DirectoryInfo DirectoryCreate(string path)
        {
            return Directory.CreateDirectory(path);
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void DirectoryDelete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }
        
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static FileStream FileCreate(string filename) {
			return File.Create(filename);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void FileDelete(string filename) {
			File.Delete(filename);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static bool FileExists(string filename) {
			return File.Exists(filename);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void FileCopy(string source, string dest, bool overwrite)
		{
			File.Copy(source, dest, overwrite);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string[] GetFiles(string directoryName) {
			return System.IO.Directory.GetFiles(directoryName);
		}

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string[] GetFiles(string directoryName, string searchPattern)
        {
            return System.IO.Directory.GetFiles(directoryName, searchPattern);
        }


        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string GetFullPath(string fileName)
        {
            return System.IO.Path.GetFullPath(fileName);
        }

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void MarshalWriteInt32(IntPtr ptr, int val) {
			Marshal.WriteInt32(ptr, val);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static int MarshalReadInt32(IntPtr ptr) {
			return Marshal.ReadInt32(ptr);
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static bool StartProcess(Process p) {
			return p.Start();
		}

	public static Process StartProcess(string exeName) 
        {
			return StartProcess(exeName, "");
		}

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static Process StartProcess(string exeName, string attributes)
        {
            return Process.Start(exeName, attributes);
        }

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void WaitForExit(Process p) {
			p.WaitForExit();
		}

		//
		// Process can't be used from semi-trusted code, so this is a
		// convenience method to create a process, wait for it to finish,
		// and return the exit code.
		//
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static int StartAndWaitForProcess(string exeName) {
			Process p = Process.Start(exeName);
			p.WaitForExit();
			return p.ExitCode;
		}

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static int StartAndWaitForProcess(string exeName, string arguments)
        {
            Process p = Process.Start(exeName, arguments);
            p.WaitForExit();
            return p.ExitCode;
        }

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void KillProcess(string process) {
			Process[] procs = Process.GetProcessesByName(process);
			foreach (Process proc in procs)
			{
				try
				{
					proc.Kill();
				}
				catch (Exception) { }
			}
		}

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static void KillProcess(Process proc)
        {
            proc.Kill();
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public static IntPtr MainHandle(Process proc)
        {
            return proc.MainWindowHandle;
        }


		[RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
		public static RegistryKey OpenSubKey(RegistryKey r, string name) {
			return r.OpenSubKey(name, true);//Writeable
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetFolderPath(Environment.SpecialFolder folder) {
			return Environment.GetFolderPath(folder);
		}

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		public static AssemblyName GetAssemblyName(Assembly a) {
			return a.GetName();
		}

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void SetCurrentInputLanguage(InputLanguage i) {
			InputLanguage.CurrentInputLanguage = i;
		}

		[SecurityPermission(SecurityAction.Assert, ControlThread = true)]
		public static void SetThreadCurrentCulture(CultureInfo ci)
		{
			Thread.CurrentThread.CurrentCulture = ci;
		}

//		[WebBrowserPermission(SecurityAction.Assert, Level = WebBrowserPermissionLevel.Default)]
		public static void Navigate(WebBrowser wb, string url) {
			wb.Navigate(new System.Uri(url));
		}

//		[WebBrowserPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetAllowWebBrowserDrop(WebBrowser wb, bool allow) {
			wb.AllowWebBrowserDrop = allow;
		}

//		[WebBrowserPermission(SecurityAction.Assert, Unrestricted = true)]
		public static WebBrowser NewWebBrowser()
		{
			return new WebBrowser();
		}


//		[WebBrowserPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetDocumentStream(WebBrowser wb, Stream stream) {
			wb.DocumentStream = stream;
		}

//		[WebBrowserPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetDocumentText(WebBrowser wb, string text) {
			wb.DocumentText = text;
		}

//		[WebBrowserPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetWebBrowserScrollBarsEnabled(WebBrowser wb, bool enabled) {
			wb.ScrollBarsEnabled = enabled;
		}

//		[WebPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetUrl(WebBrowser wb) {
			Uri ret = wb.Url;
			return (null == ret) ? string.Empty : ret.ToString();
			}

//		[WebPermission(SecurityAction.Assert, Unrestricted = true)]
		public static HtmlDocument GetDocument(WebBrowser wb) {
			return wb.Document;
		}

//		[WebPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetDocumentTitle(WebBrowser wb) {
			return wb.DocumentTitle;
		}

//		[WebPermission(SecurityAction.Assert, Unrestricted = true)]
		public static string GetDocumentText(WebBrowser wb) {
			return wb.DocumentText;
		}

//		[WebPermission(SecurityAction.Assert, Unrestricted = true)]
		public static HttpWebRequest GetHttpWebRequest(string url) {
			return (HttpWebRequest)WebRequest.Create(url);
		}

//		[WebPermission(SecurityAction.Assert, Unrestricted = true)]
		public static HttpWebResponse GetHttpWebResponse(HttpWebRequest request) {
			return (HttpWebResponse)request.GetResponse();
		}

		[UIPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void SetToolTipOwnerDraw(ToolTip tt, bool setting) {
			tt.OwnerDraw = setting;
		}

		[UIPermission(SecurityAction.Assert, Clipboard = UIPermissionClipboard.AllClipboard)]
		public static void SetAllowDrop(Control c, bool b) {
			c.AllowDrop = b;
		}

//		[WebBrowserPermission(SecurityAction.Assert, Unrestricted=true)]
		public static void SetUrl(WebBrowser b, string s) {
			b.Url = new System.Uri(s);
		}

		//
		// Visual Styles
		//
		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Region GetBackgroundRegion(VisualStyleRenderer vsr, Graphics g, Rectangle bounds)
		{
			return vsr.GetBackgroundRegion(g, bounds);
		}


		[RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
		public static object RegGetValue(RegistryKey key, string valName)
		{ return key.GetValue(valName); }

		[RegistryPermission(SecurityAction.Assert, Unrestricted = true)]
		public static void RegSetValue(RegistryKey key, string valName, object value)
		{ key.SetValue(valName, value); }

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static Rectangle GetAccessibleBounds(AccessibleObject ao)
		{ return ao.Bounds; }

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static AccessibleRole GetAccessibleRole(AccessibleObject ao)
		{ return ao.Role; }

		[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
		public static void DoAccessibleDefaultAction(AccessibleObject ao)
		{ ao.DoDefaultAction(); }

		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
       	public static void SetImageLocation(PictureBox pictureBox, string filename)
        {   pictureBox.ImageLocation = filename; }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void LoadAsyncFromImageLocation(PictureBox pictureBox, string imageLocation)
        { pictureBox.LoadAsync(imageLocation); }

        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void LoadFromImageLocation(PictureBox pictureBox, string imageLocation)
        { pictureBox.Load(imageLocation); }

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        public static void SetSyncContext(SynchronizationContext s)
        { SynchronizationContext.SetSynchronizationContext(s); }
 
        public static class Native
        {
            public static IntPtr GetDC(IntPtr hWnd)
            { return Actual.GetDC(hWnd); }

            public static int ReleaseDC(IntPtr hWnd, IntPtr hDC)
            { return Actual.ReleaseDC(hWnd, hDC); }

            public static IntPtr SelectObject(IntPtr hDC, IntPtr hgdiobj)
            { return Actual.SelectObject(hDC, hgdiobj); }

            [SuppressUnmanagedCodeSecurity()]
            private static class Actual
            {
                [DllImport("user32.dll")]
                public static extern IntPtr GetDC(IntPtr hWnd);

                [DllImport("user32.dll")]
                public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

                [DllImport("gdi32.dll")]
                public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
            }
        }

		[DllImport("advapi32.dll", SetLastError=true, ExactSpelling=true, CharSet=CharSet.Unicode)]
		private static extern bool LogonUserW(
			string userName,
			string domain,
			IntPtr password,
			LogonType logonType,
			LogonProvider logonProvider,
			[Out] out SafeUserToken token);
		public enum LogonType : int 
		{ 
			Interactive = 2,
			Network = 3,
/*			Batch=4,
			Service=5,
			Unlock=7,
			NetworkCleartext=8,
			NewCredentials=9*/
		}
		public enum LogonProvider:int{Default=0}

		[SuppressUnmanagedCodeSecurity]
		internal sealed class SafeUserToken : SafeHandle
		{
			[DllImport("kernel32.dll")]
			private static extern bool CloseHandle(IntPtr handle);

			private SafeUserToken()
				: base(IntPtr.Zero, true)
			{ }

			public override bool IsInvalid
			{
				get { return IntPtr.Zero == this.handle; }
			}

			protected override bool ReleaseHandle()
			{
				return CloseHandle(handle);
			}
		}

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public static void SetAppDomainData(AppDomain domain, string name, object data)
        {
            domain.SetData(name, data);
        }
        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public static object GetAppDomainData(AppDomain domain, string name)
        {
            return domain.GetData(name);
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public static AppDomain CreateAppDomainData(string name)
        {
            return AppDomain.CreateDomain(name);
        }
	}
}
