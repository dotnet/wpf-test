// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using WFLog = WFCTestLib.Log;
using ReflectTools;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace WFCTestLib.Util
{
    /// <summary>
    ///  Provides miscellaneous utility methods.  Put any utility methods that might be useful in
    ///  other tests in this class.
    /// </summary>
    public static class Utilities
    {
        static Utilities()
        {
            s_configFile =  "ServerConfig.xml";
            DefaultTestFileShare = @"\\" + Utilities.GetServerName("ASPNET").ToLower() + @"\TestFiles";
            DefaultTestFileShareSsl = @"\\" + Utilities.GetServerName("ASPNET").ToLower() + @"\TestFilesSSL";
            DefaultTestFileShareUrl = @"http://" + Utilities.GetServerName("ASPNET").ToLower() + "/TestFiles";
            DefaultTestFileShareSslUrl = @"https://" + Utilities.GetServerName("ASPNET").ToLower() + "/TestFilesSSL";
        }
        private static Regex s_formatRegex = new Regex(@"\{[^\}]*\}", RegexOptions.Compiled);
        /// <summary>
        /// Formats the given object based on the input string.  
        /// 
        /// Property strings in angle brackets will be evaluated on the object passed in.
        /// Recursive evaluations are permitted.
        /// A Pseudo-property "GetType" is also supported which returns the type of any given object.
        /// 
        /// For example, if the passed in object is a control, I could use the string:
        /// "Control, type={GetType}, name={Name}, text={Text}, parentName={Parent.Name}"
        /// this might return something like
        /// "Control, type=System.Windows.Forms.Button, name=button1, text=button1, parentName=Form1"
        /// </summary>
        /// <param name="o">The object to format</param>
        /// <param name="format">The format string.</param>
        /// <returns>A formatted string</returns>
        public static string FormatObject(object o, string format)
        { return FormatObject(o, format, false); }

        /// <summary>
        /// Formats the specified object
        /// <see cref="FormatObject(object,string)"/>
        /// </summary>
        /// <param name="o">The object to format</param>
        /// <param name="format">The format string.</param>
        /// <param name="throwOnMissingProperty">If true, the method will throw when a missing property is specified, 
        /// otherwise that property value will be empty.</param>
        /// <returns>A formatted string</returns>
        public static string FormatObject(object o, string format, bool throwOnMissingProperty)
        {
            return s_formatRegex.Replace(format, delegate(Match m)
            {
                string value = m.Value.Trim('{', '}');
                string[] parts = value.Split('.');
                object oToConsider = o;
                for (int i = 0; i < parts.Length; ++i)
                {
                    if (null == oToConsider)
                    { return "[NULL]"; }
                    Type t = oToConsider.GetType();
                    if (parts[i].ToUpper() == "GETTYPE")
                    { oToConsider = t; }
                    else
                    {
                        PropertyInfo pi = t.GetProperty(parts[i], BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                        if (null == pi)
                        {
                            if (throwOnMissingProperty)
                            { throw new FormatException("Invalid Format string: " + value); }
                            else
                            { return ""; }
                        }
                        oToConsider = pi.GetValue(oToConsider, null);
                    }
                }
                if (null == oToConsider)
                { return "[NULL]"; }
                return oToConsider.ToString();
            });
        }
        /// <summary>
        ///  Name of resource bundle for Windows Forms
        /// </summary>
        public const string wfResourceBundle = "System.Windows.Forms";
        /// <summary>
        ///  Name of the Assembly where Windows Forms resource bundle comes from 
        /// </summary>
        public const string wfAssemblyName = "System.Windows.Forms.DLL";

        /// <summary>
        ///  ResourceManager for Windows Forms 
        /// </summary>
        private static ResourceManager s_wfResourceManager;

        //
        // Used by the AllControls property.
        //
        private static ArrayList s_allControls;

        // Cached value of the latest installed version of the CLR's system directory.
        private static string s_mostRecentClrRoot;

        // Cached value of the latest installed version of the CLR.
        private static string s_latestInstalledClrVersion;

        //public const string DefaultTestFileShare = @"\\aspnet-testweb\TestFiles";
        public static string DefaultTestFileShare = null; 

        //public const string DefaultTestFileShareSsl = @"\\aspnet-testweb\TestFilesSSL";
        public static string DefaultTestFileShareSsl = null; 

        //public const string DefaultTestFileShareUrl = "http://aspnet-testweb/TestFiles";
        public static string DefaultTestFileShareUrl = null; 

        //public const string DefaultTestFileShareSslUrl = "https://aspnet-testweb/TestFilesSSL";
        public static string DefaultTestFileShareSslUrl = null;

        public const string TestFileShareEnvVar = "TestFileShare";
        public const string TestFileShareSslEnvVar = "TestFileShareSSL";
        public const string TestFileShareUrlEnvVar = "TestFileShareURL";
        public const string TestFileShareSslUrlEnvVar = "TestFileShareSSLURL";

        /// <summary>
        /// Returns the UNC path of the file share used by tests.  This file share can be accessed
        /// by HREF at the URL returned by TestFileShareUrl.
        /// </summary>
        public static string TestFileShare
        {
            get { return GetTestFileShareInfo(TestFileShareEnvVar, DefaultTestFileShare); }
        }

        /// <summary>
        /// Returns the UNC path of the file share used by SSL tests.  This file share can be accessed
        /// by SSL HREF at the URL returned by TestFileShareSslUrl.
        /// </summary>
        public static string TestFileShareSsl
        {
            get { return GetTestFileShareInfo(TestFileShareSslEnvVar, DefaultTestFileShareSslUrl); }
        }

        /// <summary>
        /// Returns the URL from which files stored at TestFileShare can be accessed by HREF.
        /// </summary>
        public static string TestFileShareUrl
        {
            get { return GetTestFileShareInfo(TestFileShareUrlEnvVar, DefaultTestFileShareUrl); }
        }

        /// <summary>
        /// Returns the URL from which files stored at TestFileShareSsl can be accessed by SSL HREF.
        /// </summary>
        public static string TestFileShareSslUrl
        {
            get { return GetTestFileShareInfo(TestFileShareSslUrlEnvVar, DefaultTestFileShareSslUrl); }
        }

        [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
        private static string GetTestFileShareInfo(string envVarName, string defaultVal)
        {
            string envVar = Environment.GetEnvironmentVariable(envVarName);

            if (envVar == null)
                return defaultVal;
            else
                return envVar;
        }

        /// <summary>
        ///  Returns value of the resource from System.Windows.Forms.dll localized 
        ///  for the caller's current culture settings. 
        /// </summary>
        /// <param name="tokenString">
        ///  The token-string from corresponding resource bundle - 
        ///  Beta1: file \\urtdist\builds\SRC\2204.21\DNA\CompMod\System.ComponentModel.Framework.txt
        ///  Beta2: file \\urtdist\builds\SRC\build_number\DNA\Windows.Forms\Managed\System.Windows.Forms.SR.txt
        /// </param>
        /// <returns>
        ///  value of the resource from System.Windows.Forms.dll localized 
        ///  for the caller's current culture settings. If resource has not been localized -
        ///  closest match. If no match found - null.  
        /// </returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string GetResourceString(string tokenString)
        {
            if (s_wfResourceManager == null)
                s_wfResourceManager = new ResourceManager(wfResourceBundle, typeof(Button).Assembly);

            return s_wfResourceManager.GetString(tokenString);
        }

        /// <summary>
        ///  Returns value of the resource from given .dll localized 
        ///  for the caller's current culture settings. 
        /// </summary>
        /// <param name="tokenString">
        ///  The token-string from corresponding resource bundle - 
        ///  .txt file located \\urtdist\builds\SRC\2204.21\DNA\...
        ///  !!! Case sensitive !!! 
        /// </param>
        /// <param name="resourceBundle">
        ///  name of corresponding resource bundle - without ".txt" extention
        ///  bundle - .txt file located \\urtdist\builds\SRC\2204.21\DNA\... where 
        ///  token-string is located. 
        ///  !!! Case sensitive !!! 
        /// </param>
        /// <param name="assemblyName">
        ///  fully qualified name of dll where resource bundle comes from without ".dll" extention
        ///  like: "System.Drawing, Version=1.0.3200.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///  !!! Case sensitive !!! 
        /// </param>
        /// <returns>
        ///  value of the resource from given assemblyName localized 
        ///  for the caller's current culture settings. If resource has not been localized -
        ///  closest match. If no match found - null.       
        /// </returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string GetResourceString(string tokenString, string resourceBundle, string assemblyName)
        {
            ResourceManager rm = new ResourceManager(resourceBundle, Assembly.Load(assemblyName));
            return rm.GetString(tokenString);
        }

        private static Type s_resourceManagerType = null;
        private static Type ResourceManagerType
        {
            get
            {
                if (null == s_resourceManagerType)
                {
                    Assembly assm = null;
                    try
                    { assm = Assembly.Load("Maui.Core"); }
                    catch (FileNotFoundException fnfe)
                    { throw new ReflectBaseException("Unable to load Maui.Core.  Make sure that you have added the Winforms Runtime MAUI requirement group", fnfe); }

                    s_resourceManagerType = assm.GetType("Maui.Core.Resources.ResourceManager", true, false);
                }
                return s_resourceManagerType;
            }
        }
        private static object s_resourceManager = null;
        private static object ResourceManager
        {
            get
            {
                if (null == s_resourceManager)
                { s_resourceManager = Activator.CreateInstance(ResourceManagerType, new object[] { true }); }
                return s_resourceManager;
            }
        }
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static string GetNativeResourceString(string token)
        { return (string)ResourceManagerType.GetMethod("ExtractString", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null).Invoke(ResourceManager, new object[] { token }); }
        // <summary>
        //  Returns true if the calling code has been granted the specified permission, false
        //  otherwise.
        // </summary>
        public static bool HavePermission(IPermission p)
        {
            try
            {
                p.Demand();
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        // <summary>
        //  Returns true if the caller has full trust.
        // </summary>
        public static bool HasFullTrust()
        { return HavePermission(new SecurityPermission(PermissionState.Unrestricted)); }

        /// <summary>
        ///  Returns an ArrayList of an instance of each of the standard WinForms controls.
        ///  Doesn't include non-controls like CommonDialogs and ImageList. 
        /// </summary>
        public static ArrayList AllControls
        {
            get
            {
                // lazy initialization
                if (s_allControls == null)
                {
                    s_allControls = new ArrayList(new Control[] {
                        new Button(),
                        new CheckBox(),
                        new CheckedListBox(),
                        new ComboBox(),
                        new ContainerControl(),
                        new DateTimePicker(),
                        new DomainUpDown(),
                        new TextBox(),
                        new Form(),
                        new GroupBox(),
                        new HScrollBar(),
                        new Label(),
                        new LinkLabel(),
                        new ListBox(),
                        new ListView(),
                        new MonthCalendar(),
                        new NumericUpDown(),
                        new Panel(),
                        new PictureBox(),
                        new ProgressBar(),
                        new PrintPreviewControl(),
                        new RadioButton(),
                        new RichTextBox(),
                        new ScrollableControl(),
                        new Splitter(),
                        new TabControl(),
                        new TrackBar(),
                        new TreeView(),
                        new UserControl(),
                        new VScrollBar(),

                        //new ActiveDocumentHost(),
                        new DataGridView(),
                        new BindingNavigator(),
                        new FlowLayoutPanel(),
                        new MaskedTextBox(),
                        new MenuStrip(),
                        new PropertyGrid(),
                        new SplitContainer(),
                        new StatusStrip(),
                        new TableLayoutPanel(),
                        new ToolStrip(),
                        new WebBrowser(),
                    });

                    // Make this ArrayList read-only.
                    s_allControls = ArrayList.ReadOnly(s_allControls);
                }

                return s_allControls;
            }
        }

        /// <summary>
        /// Returns true if the current operating system is a flavor of Win9x.
        /// </summary>
        public static bool IsWin9x
        {
            [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32Windows;
            }
        }

        /// <summary>
        /// Returns true if the current operating system is Windows NT 4.0 Workstation or Server.
        /// </summary>
        public static bool IsWinNT
        {
            [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT;
            }
        }

        /// <summary>
        /// Returns true if the current operating system is Vist or LongHorn.
        /// </summary>
        public static bool IsVista
        {
            [EnvironmentPermission(SecurityAction.Assert, Unrestricted = true)]
            get
            {
                return Environment.OSVersion.Version.Major == 6;
            }
        }



        /// <summary>
        ///  returns true when called on Arabic/Hebrew OS
        ///  * additional BiDi OSes can be easily added when needed 
        /// </summary>
        public static bool IsOSBiDi
        {
            get
            {
                return ((((System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToString()).IndexOf("Arabic")) != -1) ||
                         (((System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToString()).IndexOf("Hebrew")) != -1));
            }
        }

        /// <summary>
        ///  returns true when called on Turkish OS 
        /// </summary>
        public static bool IsOSTurkish
        {
            get
            {
                return (((System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToString()).IndexOf("Turkish")) != -1);
            }
        }


        public static bool IsOSJapanese
        {
            get
            {
                return (((System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToString()).IndexOf("Japanese")) != -1);
            }
        }

        //NOTE: If we care about more than admin, we should make a help function & pass WindowsBuiltInRole.Administrator to it
        public static bool IsUserAdministrator
        {
            [System.Security.Permissions.SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }


        //
        // Methods for helping to visually debug UI-oriented tests.  Use these
        // convenience methods to draw visible lines on your control at borders,
        // specific points, etc.
        //
        public static void DrawHorizontal(Control c, Pen p, int y)
        {
            Console.WriteLine("y = " + y);
            Graphics g = c.CreateGraphics();
            g.DrawLine(p, 0, y, c.Width, y);
            g.Dispose();
        }

        public static void DrawHorizontal(Control c, int y)
        {
            DrawHorizontal(c, Pens.LightGreen, y);
        }

        public static void DrawVertical(Control c, Pen p, int x)
        {
            Console.WriteLine("x = " + x);
            Graphics g = c.CreateGraphics();
            g.DrawLine(p, x, 0, x, c.Height);
            g.Dispose();
        }

        public static void DrawVertical(Control c, int x)
        {
            DrawVertical(c, Pens.LightGreen, x);
        }

        public static void DrawRectangle(Control c, Pen p, Rectangle r)
        {
            Console.WriteLine("r = " + r);
            Graphics g = c.CreateGraphics();
            g.DrawRectangle(p, r);
            g.Dispose();
        }

        public static void DrawRectangle(Control c, Rectangle r)
        {
            DrawRectangle(c, Pens.LightGreen, r);
        }

        public static void MarkPoint(Control c, Pen p, Point pt)
        {
            DrawHorizontal(c, p, pt.Y);
            DrawVertical(c, p, pt.X);
        }

        public static void MarkPoint(Control c, Point pt)
        {
            MarkPoint(c, Pens.LightGreen, pt);
        }

        /// <summary>
        ///  Checks two bitmaps, pixel by pixel, for sameness.
        /// </summary>
        /// <param name="a">The first bitmap.</param>
        /// <param name="b">The second bitmap.</param>
        /// <returns>
        ///  True if the bitmaps are pixel-wise identical.
        /// </returns>
        public static bool BitmapsIdentical(Bitmap a, Bitmap b)
        {
            return BitmapsIdentical(a, b, false);
        }

        public static bool BitmapsIdentical(Bitmap a, Bitmap b, bool ignoreTransparency)
        {
            if ((a == null) || (b == null))
                return false;
            else if ((a.Width != b.Width) || (a.Height != b.Height))
                return false;

            for (int i = 0; i < a.Width; i++)
            {
                for (int j = 0; j < a.Height; j++)
                {
                    Color ac = a.GetPixel(i, j);
                    Color bc = b.GetPixel(i, j);

                    if (ignoreTransparency && (ac.A == 0 || bc.A == 0))
                        continue;
                    else if (ac.ToArgb() != bc.ToArgb())
                    {
                        Console.WriteLine("a({0}, {1}): {2}", i, j, a.GetPixel(i, j).ToString());
                        Console.WriteLine("b({0}, {1}): {2}", i, j, b.GetPixel(i, j).ToString());
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///  Returns a list of properties for a given object as a string of property-value pairs. 
        /// </summary>
        /// <param name="obj">The object for which to get a string of the properties.</param>
        /// <returns>
        ///  The return value is a block of text containing all of the non-indexed properties on
        ///  the object and their values. 
        /// </returns>
        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public static string ObjectProperties(object obj)
        {
            string retval = "";
            PropertyInfo[] prop = obj.GetType().GetProperties();
            retval += obj.GetType().FullName.ToString() + "\n";

            foreach (PropertyInfo pi in prop)
            {
                if (pi.GetIndexParameters().Length == 0)
                {
                    retval += "\t" + pi.Name + ": ";
                    if (pi.GetValue(obj, null) != null)
                        retval += pi.GetValue(obj, null).ToString() + "\n";
                    else
                        retval += "Null\n";
                }
            }
            return retval;
        }

        //Notes: this may not work for protected/private members.  If that's 
        //the case, and this functionality is required, you'll need to 
        //assert permissions on this method.  If you have a member that's
        //overloaded, you'll need to pass a MemberInfo instead (or else 
        //this will throw).
        public static bool MemberIsBrowsable(object o, string memberName)
        {
            PropertyInfo pi = o.GetType().GetProperty(memberName);
            MemberInfo mi;
            if (pi == null)
            {
                MemberInfo[] mis = o.GetType().GetMember(memberName);

                if (mis.Length > 1)
                    throw new AmbiguousMatchException("More than one overload.  Call version with MemberInfo");

                if (mis.Length == 0)
                    throw new MissingMemberException("Didn't find " + memberName);

                mi = mis[0];
            }
            else
            {
                mi = pi;
            }
            return MemberIsBrowsable(o, mi);
        }


        //Notes: this may not work for protected/private members.  If that's 
        //the case, and this functionality is required, you'll need to 
        //assert permissions on this method.  
        public static bool MemberIsBrowsable(object o, MemberInfo mi)
        {
            object[] attrs = mi.GetCustomAttributes(typeof(BrowsableAttribute), true);

            if (attrs.Length > 1)
                throw new InvalidOperationException("More than one Browsable attribute is on " + mi.Name);

            //if it's not marked Browsable(false), it's browsable
            if (attrs.Length == 0)
                return true;

            BrowsableAttribute browsable = (BrowsableAttribute)attrs[0];

            return browsable.Browsable;
        }


        //Notes: this may not work for protected/private members.  If that's 
        //the case, and this functionality is required, you'll need to 
        //assert permissions on this method.  If you have a member that's
        //overloaded, you'll need to pass a MemberInfo instead (or else 
        //this will throw).
        public static EditorBrowsableState MemberEditorBrowsable(object o, string memberName)
        {
            PropertyInfo pi = o.GetType().GetProperty(memberName);
            MemberInfo mi;
            if (pi == null)
            {
                MemberInfo[] mis = o.GetType().GetMember(memberName);

                if (mis.Length > 1)
                    throw new AmbiguousMatchException("More than one overload.  Call version with MemberInfo");

                if (mis.Length == 0)
                    throw new MissingMemberException("Didn't find " + memberName);

                mi = mis[0];
            }
            else
                mi = pi;
            return MemberEditorBrowsable(o, mi);
        }

        //Notes: this may not work for protected/private members.  If that's 
        //the case, and this functionality is required, you'll need to 
        //assert permissions on this method.  
        public static EditorBrowsableState MemberEditorBrowsable(object o, MemberInfo mi)
        {
            object[] attrs = mi.GetCustomAttributes(typeof(EditorBrowsableAttribute), true);

            if (attrs.Length > 1)
                throw new InvalidOperationException("More than one EditorBrowsable attribute is on " + mi.Name);

            //if it's not marked with an attribute, it's always EditorBrowsable
            if (attrs.Length == 0)
                return EditorBrowsableState.Always;

            EditorBrowsableAttribute editorBrowsable = (EditorBrowsableAttribute)attrs[0];

            return editorBrowsable.State;
        }

        public static WFLog.ScenarioResult VerifyMemberBrowsable(object o, string memberName, bool expectBrowsable, EditorBrowsableState expectEditorBrowsable, WFLog.Log log)
        {
            WFLog.ScenarioResult sr = new WFLog.ScenarioResult();
            VerifyMemberBrowsable(o, memberName, expectBrowsable, expectEditorBrowsable, sr, log);
            return sr;
        }


        public static void VerifyMemberBrowsable(object o, string memberName, bool expectBrowsable, EditorBrowsableState expectEditorBrowsable, WFLog.ScenarioResult sr, WFLog.Log log)
        {
            bool browsable = MemberIsBrowsable(o, memberName);
            sr.IncCounters(expectBrowsable == browsable, "Expected browsable " + expectBrowsable + ", but got " + browsable, log);
            EditorBrowsableState editorBrowsable = MemberEditorBrowsable(o, memberName);
            sr.IncCounters(expectEditorBrowsable == editorBrowsable, "Expected editorBrowsable " + expectEditorBrowsable + ", but got " + editorBrowsable, log);
        }


        public static WFLog.ScenarioResult VerifyMemberBrowsable(object o, MemberInfo mi, bool expectBrowsable, EditorBrowsableState expectEditorBrowsable, WFLog.Log log)
        {
            WFLog.ScenarioResult sr = new WFLog.ScenarioResult();
            VerifyMemberBrowsable(o, mi, expectBrowsable, expectEditorBrowsable, sr, log);
            return sr;
        }


        public static void VerifyMemberBrowsable(object o, MemberInfo mi, bool expectBrowsable, EditorBrowsableState expectEditorBrowsable, WFLog.ScenarioResult sr, WFLog.Log log)
        {
            bool browsable = MemberIsBrowsable(o, mi);
            sr.IncCounters(expectBrowsable == browsable, "Expected browsable " + expectBrowsable + ", but got " + browsable, log);
            EditorBrowsableState editorBrowsable = MemberEditorBrowsable(o, mi);
            sr.IncCounters(expectEditorBrowsable == editorBrowsable, "Expected editorBrowsable " + expectEditorBrowsable + ", but got " + editorBrowsable, log);
        }


        // This is the callback method for ActiveFreeze
        private static void ThreadCallBack()
        {
            MessageBox.Show("Active Testcase Pause");
        }

        /// <summary>
        ///  ActiveFreeze() will Pause test until the displayed MessageBox is
        ///  dismissed.  You will still be able to interact with the test though
        ///  so that you can debug it in the middle of a scenario. 
        /// </summary>
        public static void ActiveFreeze()
        {
            ActiveFreeze(new ThreadStart(ThreadCallBack));
        }

        /// <summary>
        /// Same as ActiveFreeze(), but allows you to specify the message that is
        /// displayed in the message box that appears.
        /// </summary>
        /// <param name="message">Message to display in the message box.</param>
        public static void ActiveFreeze(string message)
        {
            ActivePromptHelper hlp = new ActivePromptHelper(message);
            ActiveFreeze(new ThreadStart(hlp.ShowMessage));
        }

        /// <summary>
        /// Same as ActiveFreeze() but allows you to specify the ThreadStart object
        /// that will be invoked to initiate the "freeze".  The freeze will stop once
        /// the thread is complete.
        /// </summary>
        /// <param name="ts">ThreadStart object pointing to a method to invoke.</param>
        public static void ActiveFreeze(ThreadStart ts)
        {
            Thread thread = new Thread(ts);

            thread.Start();
            Thread.Sleep(500);
            while (thread.ThreadState == System.Threading.ThreadState.Running)
                Application.DoEvents();
        }

        /// <summary>
        /// Same as ActiveFreeze() except a message box with a question and Yes/No buttons
        /// is displayed.  Returns a bool based on whether the user clicked the Yes button.
        /// </summary>
        /// <param name="message">Message to be displayed in the prompt</param>
        /// <returns>true if the user clicked Yes, false if they clicked No.</returns>
        public static bool ActivePrompt(string message)
        {
            ActivePromptHelper hlp = new ActivePromptHelper(message);
            ActiveFreeze(new ThreadStart(hlp.ShowPrompt));
            return hlp.Success;
        }

        internal class ActivePromptHelper
        {
            private string _message;
            private bool _success;

            public ActivePromptHelper(string message)
            {
                this._message = message;
            }

            public string Message
            {
                get { return _message; }
            }

            public bool Success
            {
                get { return _success; }
            }

            public void ShowPrompt()
            {
                _success = (DialogResult.Yes == MessageBox.Show(null, "Is the following correct?\r\n" + Message, "Manual test interaction", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2));
            }

            public void ShowMessage()
            {
                MessageBox.Show(null, _message, "Testcase paused");
            }
        }

        // Yanked from BaseTestDriver.Utilities.GenericRoutines
        public static string GetSdkRoot(string version)
        {
            RegistryKey rk;
            string clrRoot;

            clrRoot = GetClrRoot(version);

            if (clrRoot.IndexOf("1.0.3705") != -1)
            {
                // RTM is the one we want
                rk = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework");
                return (string)rk.GetValue("sdkInstallRoot");
            }
            else if (clrRoot.IndexOf("1.1.4322") != -1)
            {
                // Everett
                rk = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework");
                return (string)rk.GetValue("sdkInstallRootv1.1");
            }
            else
            {
                // else assume Whidbey
                rk = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework");
                return (string)rk.GetValue("sdkInstallRootv2.0");
            }
        }

        public static string GetSdkRoot()
        {
            return GetSdkRoot(null);
        }

        /// <summary>
        /// Returns the location of the latest version of the .NET Framework.  We assume this won't change
        /// during the lifetime of a test so we cache it for efficiency.
        /// </summary>
        public static string GetClrRoot()
        {
            if (s_mostRecentClrRoot == null)
                s_mostRecentClrRoot = GetClrRoot(null);

            return s_mostRecentClrRoot;
        }

        /// <summary>
        /// Gets the location of the Framework version directory for a specified version.
        /// If null or an empty string is passed in, returns it for the most recent Framework version
        /// installed.
        /// </summary>
        /// <param name="version">The Version parameter should be of the form "v1.0.3500".</param
        public static string GetClrRoot(string version)
        {
            string keyName = @"SOFTWARE\Microsoft\.NETFramework";
            RegistryKey frameworkKey = Registry.LocalMachine.OpenSubKey(keyName);
            string installRoot;
            string path;

            if (frameworkKey == null)
                throw new Exception("Registry key '" + keyName + "' was not found");

            installRoot = (string)frameworkKey.GetValue("InstallRoot", "");

            if (installRoot == "")
                throw new Exception("InstallRoot value under '" + keyName + "' was not found");

            // Append version information to path
            if (version != null && version != "")
                path = System.IO.Path.Combine(installRoot, version);
            else
                path = System.IO.Path.Combine(installRoot, GetLatestInstalledClrVersion());

            if (!Directory.Exists(path))
                throw new Exception(path + ": Does not exist. Product was not installed or version argument was invalid.");

            return path;
        }

        /// <summary>
        /// Determines the latest CLR build we have installed.  We need this so if the driver is
        /// running on Everett, it will still use Whidbey to build testcases unless SBS flags
        /// are set.
        /// </summary>
        /// <returns>The string returned is of the form "v1.0.3500".</returns>
        public static string GetLatestInstalledClrVersion()
        {
            if (s_latestInstalledClrVersion != null)
                return s_latestInstalledClrVersion;

            string policyKeyName = @"SOFTWARE\Microsoft\.NETFramework\policy";
            RegistryKey policyKey = Registry.LocalMachine.OpenSubKey(policyKeyName);

            if (policyKey == null)
                throw new Exception("Registry key '" + policyKeyName + " was not found");

            string[] versionSubKeyNames = policyKey.GetSubKeyNames();
            int latestMajor = 0;
            int latestMinor = 0;

            // We expect all version subkeys to have names in the form "vX.Y" (e.g. v1.0, v1.1)
            // Here we loop thru all the version subkeys to determine the one with the highest version.
            for (int i = 0; i < versionSubKeyNames.Length; i++)
            {
                string name = versionSubKeyNames[i];
                int indexOfDot = name.IndexOf(".");

                // There's also a "Standards" key which we want to ignore, so we'll make sure to only
                // handle keys with a dot in them.
                if (indexOfDot != -1)
                {
                    int curMajor = int.Parse(name.Substring(1, indexOfDot - 1));	// the "X"
                    int curMinor = int.Parse(name.Substring(indexOfDot + 1));		// the "Y"

                    if (curMajor == latestMajor && curMinor > latestMinor)
                        latestMinor = curMinor;
                    else if (curMajor > latestMajor)
                    {
                        latestMajor = curMajor;
                        latestMinor = curMinor;
                    }
                }
            }

            // Now we need to determine the build number (i.e the ZZZZ in vX.Y.ZZZZ).
            string prefix = "v" + latestMajor + "." + latestMinor;
            string versionKeyName = policyKeyName + "\\" + prefix;
            RegistryKey versionKey = Registry.LocalMachine.OpenSubKey(versionKeyName);

            Debug.WriteLine("GetLatestInstalledClrBuild(): Getting version key from " + versionKeyName);

            if (versionKey == null)
                throw new Exception("Registry key '" + versionKeyName + "' was not found");

            // Append version information to path
            string[] values = versionKey.GetValueNames();

            if (values == null)
                throw new Exception("'" + versionKeyName + "' registry key contained no values");

            int latest = 0;

            for (int i = 0; i < values.Length; i++)
            {
                try
                {
                    int curVal = int.Parse(values[i]);

                    if (curVal > latest)
                        latest = curVal;
                }
                catch (Exception)
                {
                    Debug.WriteLine("* Error: \"" + values[i] + "\" is not a valid integer.  Ignoring.");
                }
            }

            if (latest == 0)
                throw new Exception("Couldn't determine latest installed CLR version");

            s_latestInstalledClrVersion = prefix + "." + latest;
            return s_latestInstalledClrVersion;
        }

        /// <summary>
        /// Diffs the two strings and returns a string showing exactly which characters differed.
        /// </summary>
        /// <param name="s1">First string.</param>
        /// <param name="s2">Second string.</param>
        /// <returns>A string showing which characters were different between s1 and s2.</returns>
        public static string[] DiffStrings(string s1, string s2)
        {
            ArrayList diffs = new ArrayList();
            if (s2 == null && null == s2)
            { return new string[] { }; }
            if (null == s1)
            { return new string[] { s2 }; }
            if (null == s2)
            { return new string[] { s1 }; }

            if (s1 != s2)
            {
                int len = Math.Max(s1.Length, s2.Length);

                for (int i = 0; i < len; i++)
                {
                    if (i >= s1.Length || i >= s2.Length || s1[i] != s2[i])
                    {
                        string s1Formatted;
                        string s2Formatted;

                        if (i >= s1.Length)
                            s1Formatted = "(none)";
                        else
                            s1Formatted = FormatChar("s1", i, s1[i]);

                        if (i >= s2.Length)
                            s2Formatted = "(none)";
                        else
                            s2Formatted = FormatChar("s2", i, s2[i]);

                        diffs.Add(string.Format("{0,20}{1,-20}{2,-20}", "", s1Formatted, s2Formatted));
                    }
                }
            }

            string[] retVal = new string[diffs.Count];
            diffs.CopyTo(retVal);
            return retVal;
        }

        private static string FormatChar(string stringName, int index, char c)
        {
            int width = stringName.Length + 5;
            return string.Format("{0,-" + width + "}{1}", stringName + "[" + index + "]", "= " + c + " (" + (int)c + ")");
        }

        /// <summary>
        /// Gets a Bitmap screenshot of the current state of the control.
        /// Caller is responsible for disposing the bitmap.
        /// </summary>
        /// <param name="ctrl">The control to be captured.</param>
        /// <returns>A Bitmap of the control.</returns>
        public static Bitmap GetBitmapOfControl(Control ctrl)
        {
            if (ctrl.Size.Width.Equals(0) || ctrl.Size.Height.Equals(0))
            {
                throw new InvalidOperationException("The size of the control cannot be zero");
            }
            else
            {
                return GetBitmapOfControl(ctrl, true);
            }

        }

        /// <summary>
        /// Gets a Bitmap screenshot of the current state of the control.
        /// Caller is responsible for disposing the bitmap.
        /// </summary>
        /// <param name="ctrl">The control to be captured.</param>
        /// <param name="doEventsFirst">If true, events will be performed prior to getting the bitmap</param>
        /// <returns>A Bitmap of the control.</returns>
        public static Bitmap GetBitmapOfControl(Control ctrl, bool doEventsFirst)
        {


            if (ctrl.Size.Width.Equals(0) || ctrl.Size.Height.Equals(0))
            {
                throw new InvalidOperationException("The size of the control cannot be zero");
            }
            else
            {
                return GetBitmapOfControl(ctrl, new Rectangle(0, 0, ctrl.ClientSize.Width, ctrl.ClientSize.Height), doEventsFirst);
            }

        }

        public static Bitmap GetBitmapOfControl(Control ctrl, Rectangle subRect, bool doEventsFirst)
        {
            if (ctrl == null) { throw new ArgumentNullException("ctrl"); }

            if (doEventsFirst) { Application.DoEvents(); }

            if (subRect.Size.Height.Equals(0) || subRect.Size.Width.Equals(0))
            {
                throw new InvalidOperationException("The size of the control cannot be zero");
            }
            else
            {
                Bitmap ret = new Bitmap(subRect.Width, subRect.Height);
                Point topLeftSrc = ctrl.PointToScreen(new Point(subRect.X, subRect.Y));
                Point topLeftDest = new Point(0, 0);

                using (Graphics retG = Graphics.FromImage(ret))
                {
                    LibSecurity.AllWindows.Assert();
                    try
                    { retG.CopyFromScreen(topLeftSrc, topLeftDest, subRect.Size, CopyPixelOperation.SourceCopy); }
                    finally { SecurityPermission.RevertAssert(); }
                }

                return ret;
            }
        }


        /// <summary>
        /// Gets screen shot for specified rectangle.
        /// </summary>
        public static Bitmap GetScreenBitmap(Rectangle rect)
        {
            if (rect.Size.Height.Equals(0) || rect.Size.Width.Equals(0))
            {
                throw new InvalidOperationException("The size of the rectangle cannot be zero");
            }
            else
            {
                Bitmap ret = new Bitmap(rect.Width, rect.Height);
                Point topLeftSrc = rect.Location;
                Point topLeftDest = new Point(0, 0);

                using (Graphics retG = Graphics.FromImage(ret))
                {
                    LibSecurity.AllWindows.Assert();
                    try
                    { retG.CopyFromScreen(topLeftSrc, topLeftDest, rect.Size, CopyPixelOperation.SourceCopy); }
                    finally { SecurityPermission.RevertAssert(); }
                }

                return ret;
            }
        }


        /// <summary>
        /// Returns true if the given bitmap contains any pixels with the same ARGB value
        /// as the given color.
        /// </summary>
        /// <param name="bmp">Bitmap to search.</param>
        /// <param name="c">Color to search for.</param>
        /// <returns>True if c is found in bmp, false otherwise.</returns>
        public static bool ContainsColor(Bitmap bmp, Color c)
        {
            return ContainsColor(bmp, c, 8);
        }

        /// <summary>
        /// Returns true if the given bitmap contains any pixels with the same ARGB value
        /// as the given color.
        /// </summary>
        /// <param name="bmp">Bitmap to search.</param>
        /// <param name="c">Color to search for.</param>
        /// <param name="tolerance">Acceptable absolute difference in ARGB values, which can result from different bit depths.</param>
        /// <returns>True if c is found in bmp, false otherwise.</returns>
        public static bool ContainsColor(Bitmap bmp, Color c, int tolerance)
        {
            if (bmp == null)
                throw new ArgumentNullException("bmp");

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color pixel = bmp.GetPixel(x, y);

                    if (ColorsMatch(pixel, c, tolerance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given bitmap contains any pixels with the same ARGB value
        /// as the given color.
        /// </summary>
        /// <param name="bmp">Bitmap to search.</param>
        /// <param name="c">Color to search for.</param>
        /// <param name="tolerance">Acceptable absolute difference in ARGB values, which can result from different bit depths.</param>
        /// <param name="subRect">A sub rectangle within the bitmap to search.</param>
        /// <returns>True if c is found in bmp, false otherwise.</returns>
        public static bool ContainsColor(Bitmap bmp, Color c, int tolerance, Rectangle subRect)
        {
            if (bmp == null)
            { throw new ArgumentNullException("bmp"); }
            if (subRect.Left < 0 || subRect.Top < 0 || subRect.Right > bmp.Width || subRect.Bottom > bmp.Height)
            { throw new ArgumentException("subRect"); }

            for (int x = subRect.Left; x < subRect.Right; x++)
            {
                for (int y = subRect.Top; y < subRect.Bottom; y++)
                {
                    Color pixel = bmp.GetPixel(x, y);

                    if (ColorsMatch(pixel, c, tolerance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given bitmap contains any pixels with the same ARGB value
        /// as the given color.
        /// </summary>
        /// <param name="bmp">Bitmap to search.</param>
        /// <param name="c">Color to search for.</param>
        /// <param name="subRect">A sub rectangle within the bitmap to search.</param>
        /// <returns>True if c is found in bmp, false otherwise.</returns>
        public static bool ContainsColor(Bitmap bmp, Color c, Rectangle subRect)
        { return ContainsColor(bmp, c, 8, subRect); }

        /// <summary>
        /// Returns true if the first color is "close enough" to the second (within tolerance)
        /// </summary>
        /// <param name="firstColor">First color to compare</param>
        /// <param name="secondColor">Second color to compare</param>
        /// <param name="tolerance">Acceptable absolute difference in ARGB values, which can result from different bit depths.</param>
        /// <returns>True if colors are within tolerance, false otherwise.</returns>
        public static bool ColorsMatch(Color firstColor, Color secondColor, int tolerance)
        {
            return ((Math.Abs(firstColor.A - secondColor.A) <= tolerance)
                && (Math.Abs(firstColor.R - secondColor.R) <= tolerance)
                && (Math.Abs(firstColor.G - secondColor.G) <= tolerance)
                && (Math.Abs(firstColor.B - secondColor.B) <= tolerance));
        }

        /// <summary>
        /// Returns true if the first color is "close enough" to the second (within tolerance)
        /// </summary>
        /// <param name="firstColor">First color to compare</param>
        /// <param name="secondColor">Second color to compare</param>
        /// <returns>True if colors are within default tolerance, false otherwise.</returns>
        public static bool ColorsMatch(Color firstColor, Color secondColor)
        {
            return ColorsMatch(firstColor, secondColor, 8);
        }


        /// <summary>
        /// Each iteration by default is 50 milliseconds.
        /// </summary>
        public static void SleepDoEvents(int iterationCount)
        {
            SleepDoEvents(iterationCount, 50);
        }

        public static void SleepDoEvents(int iterationCount, int millisecsPerIteration)
        {
            for (int i = 0; i < iterationCount; i++)
            {
                Application.DoEvents();
                Thread.Sleep(millisecsPerIteration);
            }
        }


        /// <summary>
        /// Returns true if the given bitmap contains any pixels with the same ARGB value
        /// as the given color.
        /// </summary>
        /// <param name="bmp">Bitmap to search.</param>
        /// <param name="c">Color to search for.</param>
        /// <param name="useDefaultTolerance">Uses default tolerance if true, or zero tolerance if false.</param>
        /// <returns>True if c is found in bmp, false otherwise.</returns>
        public static bool ContainsColor(Bitmap bmp, Color c, bool useDefaultTolerance)
        {
            if (useDefaultTolerance)
                return ContainsColor(bmp, c, 5);
            else
                return ContainsColor(bmp, c);
        }

        /// <summary>
        /// Sends a string in a separate thread after a delay
        /// </summary>
        /// <param name="s">The string to send.</param>
        /// <param name="delayInMilliseconds">Milliseconds to delay before sending.</param>
        public static void SendKeysInThread(string s, int delayInMilliseconds)
        {
            Thread th = new Thread(delegate() { InternalSendStringAfterDelay(s, delayInMilliseconds); });
            th.Start();
        }

        internal static void InternalSendStringAfterDelay(string s, int d)
        {
            Application.DoEvents();
            Thread.Sleep(d);
            SafeMethods.SendWait(s);
        }

        /// <summary>
        /// Returns the attribute of type attrType declared on mi, or null if it is
        /// not declared on mi.  If more than one attribute of that type is declared,
        /// throws an exception.
        ///
        /// If the attribute is not found and searchBaseClasses is true (default is true),
        /// we'll also check all parent classes.  We may want to do this because apparently
        /// the designer looks at attributes defined on parent class declarations of a
        /// member.
        /// </summary>
        /// <typeparam name="T">The type of the attribute to retrieve.</typeparam>
        /// <param name="mi">Member from which to get the attribute.</param>
        /// <param name="searchBaseClasses">Search up the inheritance chain for the attribute.</param>
        /// <returns>The attribute object of type T if present.  Otherwise, null.</returns>
        public static T GetAttribute<T>(MemberInfo mi, bool searchBaseClasses) where T : Attribute
        {
            object[] attrs = mi.GetCustomAttributes(typeof(T), true);

            if (attrs != null && attrs.Length != 0)
            {
                // Attribute found
                if (attrs.Length > 1)
                    throw new WinFormsTestLibException("More than one instance of " + typeof(T).Name + " was declared on " + mi.Name);
                else
                    return (T)attrs[0];
            }
            else
            {
                // Attribute not found--check base class
                Type baseType = mi.DeclaringType.BaseType;

                if (baseType == null || !searchBaseClasses)
                    return null;
                else
                {
                    mi = baseType.GetProperty(mi.Name, BindingFlags.Instance | BindingFlags.Public);

                    if (mi == null)
                        return null;

                    return GetAttribute<T>(mi, true);
                }
            }
        }

        // Overload of GetAttribute where searchBaseClasses defaults to true.
        public static T GetAttribute<T>(MemberInfo mi) where T : Attribute
        {
            return GetAttribute<T>(mi, true);
        }

        private static void AddSpread(List<int> target, int value)
        {
            unchecked
            {
                target.Add(value - 1);
                target.Add(value);
                target.Add(value + 1);
            }
        }
        public static IEnumerable<int> GetInterestingRangeValues(int min, int max, params int[] inflection)
        {
            List<int> l = new List<int>();
            AddSpread(l, min);
            AddSpread(l, max);
            AddSpread(l, 0);//Add zero as an assumed point of inflection
            foreach (int i in inflection)
            { AddSpread(l, i); }
            l = l.FindAll(delegate(int test) { return test >= min && test <= max; });
            l.Sort();
            //remove duplicates?
            for (int i = l.Count - 1; i > 0; --i)
            {
                if (l[i] == l[i - 1]) { l.RemoveAt(i); }
            }
            return l;
        }

        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static string GenerateObjectDiff(object left, object right)
        {
            if (null == left) { throw new ArgumentNullException("left"); }
            if (null == right) { throw new ArgumentNullException("right"); }

            Type type = left.GetType();
            if (type != right.GetType()) { throw new ArgumentException("left and right must have the same type"); }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Comparing objects of type {0}\r\n", type.Name);

            bool match = true;
            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                //Only pay attention to properties which can be read and which are not indexed
                if (pi.CanRead && pi.GetIndexParameters().Length == 0)
                {
                    object leftValue = null, rightValue = null;
                    try
                    { leftValue = pi.GetValue(left, null); }
                    catch { }

                    try
                    { rightValue = pi.GetValue(right, null); }
                    catch { }

                    if (
                        //One is null and the other isn't
                        object.ReferenceEquals(null, leftValue) != object.ReferenceEquals(null, rightValue)
                        || (!object.ReferenceEquals(null, leftValue) &&
                        !leftValue.Equals(rightValue))
                    )
                    {

                        sb.AppendLine(string.Format("  Property {0} didn't match \r\n\tleft\t[{1}]\r\n\tright\t[{2}]",
                            pi.Name, leftValue, rightValue));
                        match = false;
                    }
                }
            }
            if (match)
            { sb.AppendLine("  All public properties matched"); }
            return sb.ToString();
        }


        /// <summary>
        /// Returns the server name based on the token passed to it. This way the test case need
        /// not hard-code the server name.
        /// 
        /// Usage: Utilites.GetServerName("ASPNET");  // This will return aspnet-testweb as the server name.
        /// 
        /// The function will attempt to get the server name for a configuration file if available. 
        /// The configuration file should either be attached to the test case as a requirement or can 
        /// be copied down as a run-step.
        /// The schema of the config file is a follows
        ///<?xml version="1.0" encoding="utf-8" ?>
        ///<configuration>
        ///  <applicationSettings>
        ///     <Settings.Properties.Settings>
        ///         <setting name="DEFAULT" serializeAs="String">
        ///             <value>aspnet-testweb</value>
        ///             </setting>
        ///             ...
        ///             </Settings.Properties.Settings>
        ///             </applicationSettings>
        /// </configuration>
        /// </summary>
        /// <param name="token">The token for the actual server name</typeparam>

        
        private static string s_configFile = null; // Init to "ServerConfig.xml" in constructor
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static string GetServerName(string token)
        {
            //Hard-coded namve value pairs.
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("IPV6", "wf-ipv6");
            dictionary.Add("ASPNET", "aspnet-testweb");

            //Try reading from an Xml config file...
            //If it does not exists we will use values hard-coded in this function.
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(s_configFile);
                XmlNodeList serversList = doc.GetElementsByTagName(@"setting");
                foreach (XmlNode server in serversList)
                {
                    if (server.Attributes["name"].Value.Equals(token, StringComparison.CurrentCultureIgnoreCase))
                        return server.InnerText;
                }
            }
            catch (XmlException)
            {
                //Some XmlParser exception occured.
                throw;
            }
            catch (Exception)
            {
                //Console.WriteLine("No server Configuration file found. Using hard-coded values.");

                //We are not storing in an XML configurarion file. Just give back the hard-coded info 
                //avialalbe in the dictionary.
                foreach (String s in dictionary.Keys)
                {
                    if (s.Equals(token, StringComparison.CurrentCultureIgnoreCase))
                        return dictionary[s];
                }
            }
            return null;
        }
    }
}
