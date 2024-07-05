// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: 
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
using Microsoft.Test.WindowsUIAutomation.Core;
using System.Diagnostics.CodeAnalysis;  // Required for FxCop suppression attributes
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using System.CodeDom;
using System.Text;
using System.Resources;
using System.Diagnostics;
using System.Threading;
using Accessibility;
using MS.Win32;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace InternalHelper
{
    using InternalHelper.Enumerations;
    using InternalHelper.Tests;
    using Microsoft.Test.WindowsUIAutomation.TestManager;

    internal sealed class GenericMath
    {
        GenericMath() { }
        /// -------------------------------------------------------------------
        /// <summary>
        /// Since not all data types have a CompareTo(), we will wrap this
        /// and return the same as the CompareTo()
        /// </summary>
        /// -------------------------------------------------------------------
        public static Int32 CompareTo<T>(T item1, T item2)
            where T : IComparable
        {
            return item1.CompareTo(item2);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// -------------------------------------------------------------------
        public static bool AreEquals<T>(T item1, T item2)
            where T : IComparable
        {
            return item1.ToString().CompareTo(item2.ToString()) == 0;
        }
    }

    /// -----------------------------------------------------------------------
    /// <summary>
    /// Static methods to be used across the test cases
    /// </summary>
    /// -----------------------------------------------------------------------
    public class Helpers
    {
        /// -------------------------------------------------------------------
        /// <summary>
        /// Generic time out for SendMessages
        /// Most messages won't need anything near this - but there are a few - eg. WM_COMMAND/CBN_DROPDOWN
        /// when sent to IE's address combo for the first time causes it to populate iself, and that can
        /// take a couple of seconds on a slow machine.        
        /// </summary>
        /// -------------------------------------------------------------------
        private const int _sendMessageTimeoutValue = 10000;

        /// -------------------------------------------------------------------
        /// <summary>Generic flags for SendMessages</summary>
        /// -------------------------------------------------------------------
        private const int _sendMessageFlags = NativeMethods.SMTO_BLOCK;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Helpers() { }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static internal System.Random _rnd = new System.Random(~unchecked((int)DateTime.Now.Ticks));

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string PatternNotSupported = "Could not get pattern from AutomationElement";

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static Hashtable s_tableProperties = new Hashtable();

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static Hashtable s_tableControlTypes = new Hashtable();

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static Hashtable s_tableEvents = new Hashtable();

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static Hashtable s_tableTextAttributes = new Hashtable();

        /// -------------------------------------------------------------------
        /// <summary>ControlPath tags</summary>
        /// -------------------------------------------------------------------
        const string _IDSClassname = "Classname";

        /// -------------------------------------------------------------------
        /// <summary>ControlPath tags</summary>
        /// -------------------------------------------------------------------
        const string _IDSControlType = "ControlType";

        /// -------------------------------------------------------------------
        /// <summary>ControlPath tags</summary>
        /// -------------------------------------------------------------------
        const string _IDSAutomationId = "AutomationId";

        /// -------------------------------------------------------------------
        /// <summary>ControlPath tags</summary>
        /// -------------------------------------------------------------------
        const string _IDSName = "NameProperty";

        /// -------------------------------------------------------------------
        /// <summary>ControlPath tags</summary>
        /// -------------------------------------------------------------------
        const string _IDSElement = "AutomationElement";

        /// -------------------------------------------------------------------
        /// <summary>ControlPath tags</summary>
        /// -------------------------------------------------------------------
        const string _IDSControl = "ControlType";

        /// -------------------------------------------------------------------
        /// <summary>Uses reflection to get to the actual bug message associated with an BugIssue</summary>
        /// -------------------------------------------------------------------
        internal static string BugIssueToDescription(BugIssues bugNumber)
        {
            object[] bi = bugNumber.GetType().GetCustomAttributes(true);
            string buffer = string.Empty;
            foreach (FieldInfo enumValue in bugNumber.GetType().GetFields())
            {
                if ((BugIssues)enumValue.GetValue(bugNumber) == bugNumber)
                {
                    object attribute = Attribute.GetCustomAttribute(enumValue, typeof(BugDescriptionAttribute));
                    if (null != attribute && attribute is BugDescriptionAttribute)
                    {
                        BugDescriptionAttribute bd = (BugDescriptionAttribute)attribute;
                        buffer = string.Format("{0}", bd.Description);
                        break;
                    }
                }
            }
            return buffer;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Get an IAccessible from a window pointer
        /// </summary>
        /// -------------------------------------------------------------------
        internal static int GetIAccessibleFromWindow(IntPtr hwnd, int idObject, ref IAccessible accObject)
        {
            accObject = null;

            try
            {
                object obj = null;
                int hr = UnsafeNativeMethods.AccessibleObjectFromWindow(hwnd, idObject, ref UnsafeNativeMethods.IID_IUnknown, ref obj);

                accObject = obj as IAccessible;

                if (hr != NativeMethods.S_OK || accObject == null)
                {
                    return NativeMethods.S_FALSE;
                }

                return hr;
            }
            catch
            {
                return NativeMethods.S_FALSE;
            }
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets WindowHandle from an AutomationElement
        /// </summary>
        ///---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.InvalidOperationException.#ctor(System.String)")]
        public static IntPtr CastNativeWindowHandleToIntPtr(AutomationElement element)
        {
            Library.ValidateArgumentNonNull(element, "Automation Element cannot be null");

            object objHwnd = element.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
            IntPtr ptr = IntPtr.Zero;

            if (objHwnd is IntPtr)
                ptr = (IntPtr)objHwnd;
            else
                ptr = new IntPtr(Convert.ToInt64(objHwnd, CultureInfo.CurrentCulture));

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException("Could not get the handle to the element(window)");

            return ptr;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /// -------------------------------------------------------------------
        internal static IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr result;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, msg, wParam, lParam, _sendMessageFlags, _sendMessageTimeoutValue, out result);
            int lastWin32Error = Marshal.GetLastWin32Error();

            if (resultSendMessage == IntPtr.Zero)
            {
                //  EvaluateSendMessageTimeoutError(lastWin32Error);
            }

            return result;
        }


        /// -------------------------------------------------------------------
        /// <summary>
        /// On a 64-bit platform, the value of the IntPtr is too large to represent as a 32-bit signed integer.
        /// An int is a System.Int32.  When an explicit cast of IntPtr to int is done on a 64-bit platform an
        /// OverflowException will occur when the IntPtr value exceeds the range of int. In cases where using
        /// SendMessage to get back int (e.g. an item index or an enum value), this version safely truncates
        /// from IntPtr to int.
        /// </summary>
        /// -------------------------------------------------------------------
        internal static int SendMessageInt(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr result = SendMessage(hwnd, msg, wParam, lParam);
            return unchecked((int)(long)result);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Same as above but does not throw on timeout
        /// </summary>


        internal static IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, bool ignoreTimeout)
        {
            IntPtr result;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, msg, wParam, lParam, _sendMessageFlags, _sendMessageTimeoutValue, out result);
            int lastWin32Error = Marshal.GetLastWin32Error();

            if (resultSendMessage == IntPtr.Zero)
            {
                // EvaluateSendMessageTimeoutError(lastWin32Error, ignoreTimeout);
            }

            return result;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// On a 64-bit platform, the value of the IntPtr is too large to represent as a 32-bit signed integer.
        /// An int is a System.Int32.  When an explicit cast of IntPtr to int is done on a 64-bit platform an
        /// OverflowException will occur when the IntPtr value exceeds the range of int. In cases where using
        /// SendMessage to get back int (e.g. an item index or an enum value), this version safely truncates
        /// from IntPtr to int.
        /// </summary>
        /// -------------------------------------------------------------------
        internal static int SendMessageInt(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, bool ignoreTimeout)
        {
            IntPtr result = SendMessage(hwnd, msg, wParam, lParam, ignoreTimeout);
            return unchecked((int)(long)result);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /// -------------------------------------------------------------------
        internal static IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, StringBuilder sb)
        {
            IntPtr result;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, msg, wParam, sb, _sendMessageFlags, _sendMessageTimeoutValue, out result);
            int lastWin32Error = Marshal.GetLastWin32Error();

            if (resultSendMessage == IntPtr.Zero)
            {
                //  EvaluateSendMessageTimeoutError(lastWin32Error);
            }

            return result;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// On a 64-bit platform, the value of the IntPtr is too large to represent as a 32-bit signed integer.
        /// An int is a System.Int32.  When an explicit cast of IntPtr to int is done on a 64-bit platform an
        /// OverflowException will occur when the IntPtr value exceeds the range of int. In cases where using
        /// SendMessage to get back int (e.g. an item index or an enum value), this version safely truncates
        /// from IntPtr to int.
        /// </summary>
        /// -------------------------------------------------------------------
        internal static int SendMessageInt(IntPtr hwnd, int msg, IntPtr wParam, StringBuilder sb)
        {
            IntPtr result = SendMessage(hwnd, msg, wParam, sb);
            return unchecked((int)(long)result);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// -------------------------------------------------------------------
        internal static IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref NativeMethods.Win32Rect lParam)
        {
            IntPtr result;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, msg, wParam, ref lParam, _sendMessageFlags, _sendMessageTimeoutValue, out result);
            int lastWin32Error = Marshal.GetLastWin32Error();

            if (resultSendMessage == IntPtr.Zero)
            {
                // 
            }

            return result;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /// -------------------------------------------------------------------
        internal static IntPtr SendMessage(IntPtr hwnd, int msg, out int wParam, out int lParam)
        {
            IntPtr result;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, msg, out wParam, out lParam, _sendMessageFlags, _sendMessageTimeoutValue, out result);
            int lastWin32Error = Marshal.GetLastWin32Error();

            if (resultSendMessage == IntPtr.Zero)
            {
                // 
            }

            return result;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Wrapper for MapWindowPoints
        /// </summary>
        /// -------------------------------------------------------------------
        internal static bool MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref NativeMethods.Win32Rect rect, int cPoints)
        {
            // The failure case of this API may also be a valid return.  GetLastError() is need to determine a true failure state.
            // Clear the last error code to make sure we have a good starting condition to determine error state.
            //UnsafeNativeMethods.SetLastError(0);
            int mappingOffset = UnsafeNativeMethods.MapWindowPoints(hWndFrom, hWndTo, ref rect, cPoints);
            int lastWin32Error = Marshal.GetLastWin32Error();
            //if (mappingOffset == 0)
            //{
            //    // When mapping points to/from Progman and its children MapWindowPoints may fail with error code 1400
            //    // Invalid Window Handle.  Since Progman is the desktop no mapping is need.
            //    if ((IsProgmanWindow(hWndFrom) && hWndTo == IntPtr.Zero) ||
            //        (hWndFrom == IntPtr.Zero && IsProgmanWindow(hWndTo)))
            //    {
            //        lastWin32Error = 0;
            //    }

            //    ThrowWin32ExceptionsIfError(lastWin32Error);

            //    // If the coordinates is at the origin a zero return is valid.  
            //    // Use GetLastError() to check that. Error code 0 is "Operation completed successfull".  
            //    return lastWin32Error == 0;
            //}

            return true;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Obtains a resouce value within the assembly.
        /// </summary>
        /// -------------------------------------------------------------------
        static public string GetResourceString(string name)
        {
            ResourceManager rm;
            byte[] rawData = new byte[] { };
            bool found = false;

            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] names = assembly.GetManifestResourceNames();

            foreach (string assemblyName in names)
            {
                Console.WriteLine("Assembly Loaded : " + assemblyName.ToLower());

                if (assemblyName.ToLower().IndexOf("uiverify.resources") != -1)
                {
                    int iLoc = assemblyName.ToLower().LastIndexOf(".");

                    StringBuilder sb = new StringBuilder(assemblyName).Remove(iLoc, "resources".Length + 1);
                    Console.WriteLine("Attempting to load ResourceManager(" + sb.ToString() + ")");
                    rm = new ResourceManager(sb.ToString(), assembly);
                    try
                    {
                        rawData = (byte[])rm.GetObject(name);
                        found = true;
                        Console.WriteLine("Resouce loaded");
                    }
                    catch (MissingManifestResourceException)
                    {
                        Trace.WriteLine("Did not find resource in: " + sb.ToString());
                    }
                }
                if (found)
                    break;
            }

            if (!found)
                throw new InvalidOperationException("Could not get the resource");

            Decoder utf8Decoder = Encoding.UTF8.GetDecoder();
            int charCount = utf8Decoder.GetCharCount(rawData, 0, rawData.Length);
            char[] chars = new char[charCount];
            int charsDecodedCount = utf8Decoder.GetChars(rawData, 0, rawData.Length, chars, 0);

            return new string(chars);
        }

        ///--------------------------------------------------------------------
        /// <summary>
        /// Get the count of items that are selected in the Selection pattern
        /// </summary>
        ///--------------------------------------------------------------------
        static internal int SelectionCount(AutomationElement element)
        {

            AutomationElementCollection aec = element.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.IsSelectionPatternAvailableProperty, true));

            return aec.Count;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns the SelectionItems of a control that support Selection
        /// </summary>
        /// -------------------------------------------------------------------
        static internal ArrayList GetSelectableItems(AutomationElement element)
        {
            ArrayList list = new ArrayList();

            // Find all the children that support SelectionItemPattern
            AutomationElementCollection aec = element.FindAll(TreeScope.Subtree, new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true));

            foreach (AutomationElement l in aec)
            {
                // Calendar has a weird selection model.  If you select one of the greyed days that 
                // represent a next or previous day, or moves the calendar ahead or back one month
                // and then the day is selected which is not the same automation element as before.
                if (l.Current.AutomationId.IndexOf("Previous") == -1 && l.Current.AutomationId.IndexOf("Next") == -1)
                {
                    list.Add(l);
                }
            }

            return list;
        }

        /// ----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines if the element supports a given pattern
        /// </summary>
        /// ----------------------------------------------------------------------------------------------------
        static internal bool SupportsPattern(AutomationElement element, AutomationPattern pattern)
        {
            return (null == element.GetCurrentPattern(pattern));
        }


        #region Random methods

        /// -------------------------------------------------------------------
        /// <summary>Set the random seed</summary>
        /// -------------------------------------------------------------------
        static internal void RandomSeed(int seed)
        {
            _rnd = new Random(seed);
        }

        /// -------------------------------------------------------------------
        /// <summary>Get a random int between two values</summary>
        /// -------------------------------------------------------------------
        static internal int RandomValue(int val1, int val2)
        {
            int RetValue = (int)Random(val1, val2);

            return RetValue;
        }

        /// -------------------------------------------------------------------
        /// <summary>Get a random float bewteen two points</summary>
        /// -------------------------------------------------------------------
        static internal object RandomValue(object obj1, object obj2)
        {
            Type t = obj1.GetType();
            object RetValue;

            switch (t.FullName)
            {
                case "System.Boolean":
                    RetValue = RandomBool(); ;
                    break;

                case "System.Char":
                    throw new Exception("Have not implemented Random Chars");

                case "System.SByte":
                case "System.Byte":
                    RetValue = (byte)RandomByte((System.Byte)obj1, (System.Byte)obj2);
                    break;

                case "System.Int16":
                case "System.Int32":
                case "System.UInt16":
                case "System.UInt32":
                    RetValue = (int)Random(Convert.ToInt32(obj1, CultureInfo.CurrentCulture), Convert.ToInt32(obj2, CultureInfo.CurrentCulture));
                    break;

                case "System.Int64":
                case "System.UInt64":
                    throw new Exception("Have not implemented Random Int64/UInt64");

                case "System.Single":
                    RetValue = RandomDouble(obj1, obj2);
                    break;

                case "System.Decimal":
                    RetValue = RandomDouble(obj1, obj2);
                    break;

                case "System.Double":
                    RetValue = RandomDouble(obj1, obj2);
                    break;

                case "System.DateTime":
                    RetValue = RandomDate((System.DateTime)obj1, (System.DateTime)obj2);
                    break;

                case "System.String":
                    RetValue = RandomString(100, true);
                    break;

                case "ItemCheckState":
                    RetValue = RandomItemCheckState();
                    break;

                default:
                    throw new Exception("Inhandled type(" + t + ")");
            }
            return Convert.ChangeType(RetValue, t, CultureInfo.CurrentCulture);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Get a random float bewteen two points.
        /// </summary>
        /// -------------------------------------------------------------------
        static double RandomDouble(object min, object max)
        {
            double minimum = Convert.ToDouble(min, CultureInfo.CurrentCulture);
            double maximum = Convert.ToDouble(max, CultureInfo.CurrentCulture);

            // Neg or Pos number
            return minimum + (Convert.ToDouble(_rnd.NextDouble(), CultureInfo.CurrentCulture) * (maximum - minimum));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static System.DateTime RandomDate(DateTime min, DateTime max)
        {
            long minLong = min.Ticks;
            long maxLong = max.Ticks;
            long cur = (long)RandomDouble(minLong, maxLong);
            DateTime dt = new DateTime(cur);

            dt = new DateTime(dt.Year, dt.Month, dt.Day);
            return dt;
        }

        static int s_iCount = 0;

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns a random string
        /// </summary>
        /// -------------------------------------------------------------------
        static internal string RandomString(int maxLength, bool trueRandomLength)
        {
            string[] aRandom200 = new string[] { "01234567890", "ABCDEFGHIJKL" };

            if (maxLength == 2000)
                return @"'1ZNUL{FYI14`.'=h\b/_N3O7..BSDR^zG_,U.+}l82`q/`W62*}e^oZ},8G>Yq6uR9:MyfZRcBG<Q\(LU-B;Tw<V&-Jub_3E{i?uG*;JNK#=I>LN0ygMPzBZ{bHm=do$/\g=1&Lpvk)u}.f`zegvetGdycaM.iZ}/G}c1~t<[Y/9d|U`-B~),H-?@X7BiDi#Mwoeh{jfr[+<q.!{.!@kiRqe;^DWgRQ8o-jk.ov1W95P>pM2X;4i$}8z'*K+X3}@*rLdB4n>*05Q>AGJ22~M?g4ptoe$DlXD:tGKDgnEKu_rie\da!wF3QA^ZoRMkx=Wo8Hy{&JBZH)@(03|\ACs22gr&liw=v_{C<xN5jezN%WPX9>R`?LJUHF]OTs0;R%bALd%.`}6ZHF-Y8;lN88}K5Fyt<~_uV_{!t/,S:D~@6o.#9$7:(`$;&8,'j/qa??%gJ-d)/95s8^6E8w[8gEnt\`y]#&h+Q>!ujEW-?/+mL0#F|fmqRZxxP/,h2(&*P*By]@Yr@OxY;!HM^S[<8uWvD(E\:nz7)!>Bl(ha.zR$`@]Of]*]KiqYRJVVy|XPSaQ:b'_Vjy/=DT6%Iut[d=j?/coBQ2f0B|O~mDk_}SqQ[T6|5Q.%(|k7u3+~6>&Y4}yC&66%*6]2J:cZMr.~$a)E59*[9lJ`N9>]Yrw}<)yq?Zk[NPQC0&UQtU.k-Yk=j|c2{'!Zy0cyf7&ZLM-h<'PHbkg%d,pa=]'BuZ_HPuI!AZi^PR@_L$|dL;N0>{tn>#]*0bcqlc8E]P')#*S3k25:\Bu[-P+T!Za?|`NlmNqhj['{4F-5Hb#-lCLml)e=YcD$mg+BZfD=?LO<xHeCu,=WURY5`~egU/'7#/L;jY`?elAsJQs_~xcX'L96Q.yf]Ti=Hi9j{KX<J}pg1ei/,&h[KN1Vu7i\_y`1$$K`9)cTAG.)ZP%jDjZWk/qyYatd`z\P=`d.:)t$A5;%A(zE2}9t?)Ds2Q(W4}rEi\j\";

            if (maxLength == 1000) return @"'1ZNUL{FYI14`.'=h\b/_N3O7..BSDR^zG_,U.+}l82`q/`W62*}e^oZ},8G>Yq6uR9:MyfZRcBG<Q\(LU-B;Tw<V&-Jub_3E{i?uG*;JNK#=I>LN0ygMPzBZ{bHm=do$/\g=1&Lpvk)u}.f`zegvetGdycaM.iZ}/G}c1~t<[Y/9d|U`-B~),H-?@X7BiDi#Mwoeh{jfr[+<q.!{.!@kiRqe;^DWgRQ8o-jk.ov1W95P>pM2X;4i$}8z'*K+X3}@*rLdB4n>*05Q>AGJ22~M?g4ptoe$DlXD:tGKDgnEKu_rie\da!wF3QA^ZoRMkx=Wo8Hy{&JBZH)@(03|\ACs22gr&liw=v_{C<xN5jezN%WPX9>R`?LJUHF]OTs0;R%bALd%.`}6ZHF-Y8;lN88}K5Fyt<~_uV_{!t/,S:D~@6o.#9$7:(`$;&8,'j/qa??%gJ-d)/95s8^6E8w[8gEnt\`y]#&h+Q>!ujEW-?/+mL0#F|fmqRZxxP/,h2(&*P*By]@Yr@OxY;!HM^S[<";

            if (maxLength == 200)
                return aRandom200[s_iCount++ % 2];

            // Set the real random length
            int len = trueRandomLength ? maxLength : _rnd.Next(maxLength);
            char[] ach = new char[len];

            // May decide to increase this to cover more different characters.
            for (int i = 0; i < len; i++)
            {
                ach[i] = System.Convert.ToChar((int)Random(33, 127));
            }

            // Embedded \0 can lead to incorrect results / test cases that report flaws
            string results = new string(ach);
            while (results.LastIndexOf("\\0") > -1)
                results = results.Replace("\\0", "AO");

            return results;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal static string RandomItemCheckState()
        {
            int val = (int)Random(0, 3); switch (val)
            {
                case 0:
                    return "Checked";

                case 1:
                    return "Indeterminate";

                case 2:
                    return "Unchecked";

                default:
                    throw new Exception("Logic error!");
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>Random integer</summary>
        /// -------------------------------------------------------------------
        static int Random(int min, int max) { return _rnd.Next(min, max); }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        static byte RandomByte(byte b1, byte b2) { return Convert.ToByte((int)Random(b1, b2)); }

        /// -------------------------------------------------------------------
        /// <summary>Random Bool</summary>
        /// -------------------------------------------------------------------
        static bool RandomBool()
        {
            switch ((int)Random(0, 2))
            {
                case 0:
                    return false;

                case 1:
                    return true;

                default:
                    throw new ArgumentException();
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>Try to return some intelligent value based on the ControlType 
        /// or LocalizedControlTypeProperty</summary>
        /// -------------------------------------------------------------------
        internal static object GetRandomValidValue(AutomationElement element, object largeString)
        {
            const int OCTET_MAX = 128;
            object min = 0;
            object max = -1;
            object obj = null;
            ControlType controlType = element.Current.ControlType;

            if (controlType == null)
                controlType = ControlType.Custom;

            if (controlType == ControlType.Button)
            {
            }
            else if ((controlType == ControlType.Calendar) || (element.Current.ClassName.ToLower() == "datepicker"))
            {
                min = new System.DateTime(1753, 1, 1);
                max = new System.DateTime(9998, 12, 31);

                System.DateTime dt = (DateTime)Helpers.RandomValue((DateTime)min, (DateTime)max);

                obj = new DateTime(dt.Year, dt.Month, dt.Day);
            }
            else if (controlType == ControlType.CheckBox)
            {
            }
            else if (controlType == ControlType.ComboBox)
            {
            }
            else if (controlType == ControlType.Custom)
            {
                // Do specific controls we know about
                if (element.Current.ClassName.ToLower().IndexOf("sysdatetimepick32") != -1)
                {
                    min = new System.DateTime(1753, 1, 1);
                    max = new System.DateTime(9998, 12, 31);

                    obj = ((DateTime)Helpers.RandomValue((DateTime)min, (DateTime)max)).ToShortDateString();
                }
                else if (element.Current.ClassName.ToLower().IndexOf("sysipaddress32") != -1)
                {
                    ValuePattern vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                    string o = vp.Current.Value;
                    int iLoc = 0;
                    StringBuilder sb = new StringBuilder();

                    // Match 1:1 the fields in vp.Value ( there are 3 and 4 field IP address)
                    do
                    {
                        sb.Append("." + Helpers.RandomValue(0, OCTET_MAX).ToString());
                    } while ((iLoc = o.IndexOf('.', iLoc + 1)) != -1);

                    sb.Remove(0, 1);
                    obj = sb.ToString();
                }
                else if (element.Current.ClassName.ToLower().IndexOf("datagridcell") != -1)
                {
                    obj = Helpers.RandomString(10, true);
                }
                // else fallback and do subcontrols that we *might* know about for non-localization
                else
                {

                    object control = element.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty) as string;

                    if (control == null || control.ToString().Length == 0 || control == AutomationElement.NotSupported)
                    {
                        TestObject.ThrowMe(CheckType.Verification, "Cannot determine enough information about this control to set to a valid random value");
                    }

                    switch (control.ToString())
                    {
                        case "Calendar Year":
                            {
                                min = new System.DateTime(1753, 1, 1);
                                max = new System.DateTime(9998, 12, 31);

                                System.DateTime dt = (DateTime)Helpers.RandomValue((DateTime)min, (DateTime)max);

                                obj = dt.Year.ToString(CultureInfo.CurrentUICulture);
                                break;
                            }

                        case "Calendar Month":
                            {
                                min = new System.DateTime(1753, 1, 1);
                                max = new System.DateTime(9998, 12, 31);

                                System.DateTime dt = (DateTime)Helpers.RandomValue((DateTime)min, (DateTime)max);

                                obj = dt.Month.ToString(CultureInfo.CurrentUICulture);
                                break;
                            }

                        case "Calendar Day":
                            {
                                obj = (int)Helpers.RandomValue(1, 28);
                                break;
                            }

                        case "ListView Cell":
                            {
                                obj = Helpers.RandomString(20, true);
                                break;
                            }

                        default:
                            {
                                TestObject.ThrowMe(CheckType.Verification, "Need to handle this control type(" + control + ")");
                                break;
                            }
                    }
                }
            }
            else if (controlType == ControlType.Edit || controlType == ControlType.DataItem)
            {
                object control = element.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty) as string;

                switch (control.ToString())
                {
                    case "Octet":
                        {
                            obj = Helpers.RandomValue(0, OCTET_MAX).ToString();
                            break;
                        }

                    default:
                        {
                            // If we specify the large/small then do this
                            if (largeString != null)
                            {
                                if ((bool)largeString)
                                    obj = Helpers.RandomString(200, true);
                                else
                                    obj = Helpers.RandomString(3, true);
                            }
                            else
                            {   // else some standard value

                                // is it a true string or a number
                                ValuePattern vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

                                try
                                {
                                    int IsNum = Convert.ToInt32(vp.Current.Value, CultureInfo.CurrentCulture);   // its a number
                                    RangeValuePattern rvp = element.GetCurrentPattern(RangeValuePattern.Pattern) as RangeValuePattern;

                                    if (rvp != null)
                                    {
                                        obj = Helpers.RandomValue(rvp.Current.Minimum, rvp.Current.Maximum);
                                    }
                                    else
                                    {
                                        obj = Helpers.RandomValue(-100, 100);
                                    }
                                }
                                catch (FormatException)
                                {   // it's a string
                                    obj = Helpers.RandomString(30, true);
                                }
                            }

                            break;
                        }
                }
            }
            else if (controlType == ControlType.Hyperlink)
            {
            }
            else if (controlType == ControlType.Image)
            {
            }
            else if (controlType == ControlType.List)
            {
            }
            else if (controlType == ControlType.ListItem)
            {
            }
            else if (controlType == ControlType.Menu)
            {
            }
            else if (controlType == ControlType.MenuBar)
            {
            }
            else if (controlType == ControlType.MenuItem)
            {
            }
            else if (controlType == ControlType.ScrollBar)
            {
                ArrayList validLocations = new ArrayList();
                ValuePattern vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (vp == null)
                    throw new Exception("ScrollBar should have supported ValuePattern and did not");

                string curValue;

                // Get a bunch of locations...brain dead assuption, increment by .01;
                for (double val = 0; val < 100; )
                {
                    vp.SetValue(val.ToString(CultureInfo.CurrentUICulture));
                    curValue = vp.Current.Value;
                    if (validLocations.IndexOf(curValue) == -1)
                        validLocations.Add(curValue);
                    val += .01;
                }

                // return a location in the arraylist of valid locations
                obj = validLocations[(int)Helpers.RandomValue(0, validLocations.Count)];

            }
            else if (controlType == ControlType.ProgressBar || controlType == ControlType.Slider || controlType == ControlType.Spinner)
            {
                if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsRangeValuePatternAvailableProperty))
                {
                    RangeValuePattern rvp = element.GetCurrentPattern(RangeValuePattern.Pattern) as RangeValuePattern;
                    obj = Convert.ToInt32(Helpers.RandomValue((int)rvp.Current.Minimum, (int)rvp.Current.Maximum), CultureInfo.CurrentUICulture);
                }
                else if ((bool)element.GetCurrentPropertyValue(AutomationElement.IsValuePatternAvailableProperty))
                {
                    obj = "Some string";
                }
                else
                {
                    throw new InvalidOperationException("This is a spinner, but it does not support Value or RangeValue");
                }
            }
            else if (controlType == ControlType.RadioButton)
            {
            }
            else if (controlType == ControlType.StatusBar)
            {
            }
            else if (controlType == ControlType.TabItem)
            {
            }
            else if (controlType == ControlType.ToolBar)
            {
            }
            else if (controlType == ControlType.ToolTip)
            {
            }
            else if (controlType == ControlType.Tree)
            {
            }
            else if (controlType == ControlType.TreeItem)
            {
            }
            else
            {
                throw new Exception("Need to handle " + GetProgrammaticName(controlType));
            }

            if (obj == null)
                TestObject.ThrowMe(CheckType.IncorrectElementConfiguration, "Control(" + GetProgrammaticName(controlType) + ") does not support a random value");

            return obj;
        }

        #endregion random methods

        /// -------------------------------------------------------------------
        /// <summary>
        /// This will search the desktop for the AutomationElement defined
        /// by the pathXml.  Current implementation allows the threesome of 
        /// {ControlType/Name/AutomationId} to uniquly define an element.
        /// </summary>
        /// <param name="pathXml">Xml path that was created with the 
        /// Helpers.GetControlTypePath() method</param>
        /// <returns>AutomationElement that is defined by the pathXml</returns>
        /// -------------------------------------------------------------------
        static public AutomationElement GetAutomationElementFromXmlPath(string pathXml)
        {
            // Xml file should look like this.
            // Control
            //      Element
            //          Property
            //          Property
            //          Property...
            //          Element
            //              Property
            //              Property
            //              Property...

            XmlDocument doc = new XmlDocument();
            AutomationElement element = null;
            XmlNode xmlNode;
            TreeScope treeScope;
            Condition[] conditions;
            ArrayList list = new ArrayList();
            int index;

            doc.LoadXml(pathXml);
            xmlNode = doc.DocumentElement[_IDSElement];

            // Children is the default argument for a FindFirst() call.  If the arguments are empty to the 
            // specific level, then switch the next search on the next threesome to TreeScope.SubTree.
            treeScope = TreeScope.Children;

            while (xmlNode != null)
            {
                // Clear the list of property conditions each time we look at an element
                list.Clear();

                // Do all the properties of the element.  Some properties won't work since they need to be
                // special cased like the ControlType that we have here, but handle those on a need to have
                // situation.
                // Use the ArrayList vs. Condition[] since we really don't know how many of the sub-elements
                // are really property conditions, or another sub element that we need to drill down into.
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    // Don't do element, element means that there are sub elements below this element that will 
                    // need to be drilled down into.
                    if (node.Name != _IDSElement)
                    {
                        switch (node.Name)
                        {
                            // ControlType requires special handling since it is not a string, but an object type
                            case _IDSControlType:
                                {
                                    StringBuilder sb = new StringBuilder(node.InnerText);
                                    sb.Replace("C:", ""); // TODO$: Legacy, ControlTypes were for example: C:Windows
                                    sb.Replace("ControlType.", "");

                                    list.Add(new PropertyCondition(AutomationElement.ControlTypeProperty, Helpers.GetControlTypeByName(sb.ToString())));
                                    break;
                                }
                            case _IDSName:
                                {
                                    list.Add(new PropertyCondition(AutomationElement.NameProperty, node.InnerText));
                                    break;
                                }
                            case _IDSAutomationId:
                                {
                                    list.Add(new PropertyCondition(AutomationElement.AutomationIdProperty, node.InnerText));
                                    break;
                                }
                            default:
                                {
                                    throw new ArgumentException();
                                }
                        }
                    }
                }

                // If > 0, then there is a property that defines the object that we should be able to 
                // search on such as the AutomationId.
                if (list.Count > 0)
                {
                    index = 0;
                    conditions = new Condition[list.Count + 1];

                    // For simplicity, we used an ArrayList, but AndCondition 
                    // required Condition[], so move everthing over
                    foreach (PropertyCondition propCondition in list)
                    {
                        conditions[index++] = propCondition;
                    }

                    // Incase we only have one condition, just add a true condition so 
                    // we can use the AndCondition which requires 2+ or more conditions
                    conditions[index++] = PropertyCondition.TrueCondition;

                    AndCondition andCondition = new AndCondition(conditions);

                    // Find the element based on the XmlNode conditions
                    if (element == null)
                    {
                        element = AutomationElement.RootElement.FindFirst(treeScope, andCondition);
                    }
                    else
                    {
                        element = element.FindFirst(treeScope, andCondition);
                    }

                    // If we could not find anything, then bail out of here and return to the caller
                    if (element == null)
                        break;

                    // Reset this for the next time we go through the while loop
                    treeScope = TreeScope.Children;
                }
                else
                {
                    // There was nothing to query for on this child, so let it go to the 
                    // next sub level and then search the whole subtree, not just the child elements
                    treeScope = TreeScope.Subtree;
                }

                // Get the next sub element(control) from the xml path
                xmlNode = xmlNode[_IDSElement];
            }

            return element;

        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns a Xml path to an element.  This string can then be used for
        /// the call to GetAutomationElementFromXmlPath() at a later time which
        /// will then return the actual element.
        /// </summary>
        /// -------------------------------------------------------------------
        static public string GetXmlPathFromAutomationElement(AutomationElement element)
        {
            StringBuilder sb = new StringBuilder();
            Stream stream = new MemoryStream();
            XmlTextWriter tw = new XmlTextWriter(stream, Encoding.Default);
            object item;
            Stack stack = new Stack();

            tw.WriteStartDocument();

            // Place them on a stack so we can pop them off in reverse order
            while (element != null && element != AutomationElement.RootElement)
            {
                // Place the threesome on the stack
                stack.Push((string)element.GetCurrentPropertyValue(AutomationElement.ClassNameProperty));
                stack.Push((string)element.GetCurrentPropertyValue(AutomationElement.AutomationIdProperty));
                stack.Push((ControlType)element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty));
                stack.Push((string)element.GetCurrentPropertyValue(AutomationElement.NameProperty));

                // Walk up the tree to the next element until we get to the root.
                element = TreeWalker.ControlViewWalker.GetParent(element);
            }

            // Create an XML string of the element's path
            tw.WriteStartElement(_IDSControl);

            // Pop them off in reverse order and create the Xml string
            for (; stack.Count > 0; )
            {
                tw.WriteStartElement(_IDSElement);

                // Write AutomationElement.NameProperty
                item = stack.Pop();
                if ((string)item != string.Empty)
                {
                    tw.WriteStartElement(_IDSName);
                    tw.WriteString((string)item);
                    tw.WriteEndElement();
                }

                // Write AutomationElement.ControlTypeProperty
                item = stack.Pop();
                if (item != null)
                {
                    sb = new StringBuilder(((ControlType)item).ProgrammaticName);
                    // Legacy ControlType was prefixeed with "C:";
                    sb.Replace("C:", "");  // TODO$: Legacy stuff
                    tw.WriteStartElement(_IDSControlType);
                    tw.WriteString(sb.ToString());
                    tw.WriteEndElement();
                }

                // Write AutomationElement.AutomationIdProperty
                item = stack.Pop();
                if ((string)item != string.Empty)
                {
                    tw.WriteStartElement(_IDSAutomationId);
                    tw.WriteString((string)item);
                    tw.WriteEndElement();
                }
                // Write AutomationElement.ClassName
                item = stack.Pop();
                if ((string)item != string.Empty)
                {
                    tw.WriteStartElement(_IDSClassname);
                    tw.WriteString((string)item);
                    tw.WriteEndElement();
                }
            }

            tw.WriteFullEndElement();
            tw.WriteEndDocument();

            tw.Flush();

            // TODO$: I'm sure there is an easier way to convert the stream to a string, but
            // in the motor home and don't have access to help right now.  Later look into 
            // this and do it a cleaner way.
            byte[] buffer = new byte[tw.BaseStream.Length];
            tw.BaseStream.Seek(0, SeekOrigin.Begin);
#pragma warning disable CA2022 // Avoid inexact read
            tw.BaseStream.Read(buffer, 0, (int)tw.BaseStream.Length);
#pragma warning restore CA2022
            sb = new StringBuilder(buffer.Length);
            foreach (byte b in buffer)
                sb.Append(Convert.ToChar(b));

            return sb.Remove(0, sb.ToString().IndexOf("<C")).ToString();
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Based on the property name, return the AutomationProperty value
        /// </summary>
        /// -------------------------------------------------------------------
        static internal AutomationProperty GetPropertyByName(string name)
        {
            // Use refelction to get the AutomationProperty object
            Assembly assembly = Assembly.GetAssembly(typeof(System.Windows.Automation.AutomationElement));
            Type t = assembly.GetType("System.Windows.Automation." + name.Substring(0, name.LastIndexOf(".")));
            FieldInfo f = t.GetField(name.Substring(name.IndexOf(".") + 1));
            return (AutomationProperty)f.GetValue(null);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Return the ControlTpye associated with a name
        /// </summary>
        /// -------------------------------------------------------------------
        static internal ControlType GetControlTypeByName(string name)
        {
            // Use reflection to get the ControlType object
            return (ControlType)typeof(System.Windows.Automation.ControlType).GetField(name).GetValue(null);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// This will return all the TestSuite's that are supported by the 
        /// Logical Element.
        /// </summary>
        /// -------------------------------------------------------------------
        static internal ArrayList GetPatternSuitesForAutomationElement(AutomationElement element)
        {
            if (element == null)
                throw new ArgumentException();

            ArrayList al = new ArrayList();
            AutomationPattern[] patterns = element.GetSupportedPatterns();
            string[] AutomationPatternNames = new string[patterns.Length];
            int index = 0;

            foreach (AutomationPattern ap in patterns)
            {
                AutomationPatternNames[index++] = Automation.PatternName(ap);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            Type[] mytypes = assembly.GetTypes();

            foreach (Type type in mytypes)
            {
                FieldInfo suite = type.GetField("TestWhichPattern");

                if (suite != null)
                {
                    string TestWhichPattern = suite.GetValue(null) as string;

                    if (!Array.IndexOf(AutomationPatternNames, TestWhichPattern).Equals(-1))
                    {
                        suite = type.GetField("TestSuite");
                        if (suite != null)
                        {
                            string TestSuite = suite.GetValue(null) as string;

                            if ((!string.IsNullOrEmpty(TestSuite)) && (!CheckSuiteExclusions(type.Name, element.Current.ClassName)))
                                al.Add(TestSuite);
                        }
                    }
                }
            }

            return al;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// <param name="controlType"></param>
        /// <returns>Name without any additional information.  Example: Button
        /// This method is used by the reflection code to pull in the correct
        /// tests depending on the name.  Changing this has a potential of causing 
        /// the tests in the lab to fail.</returns>
        /// -------------------------------------------------------------------
        static internal string GetProgrammaticName(ControlType controlType)
        {
            StringBuilder buffer = new StringBuilder(ProgrammaticName(controlType.ProgrammaticName));

            // There are two versions out there right now, depending on which branch you are in, just be 
            // proactive and do them both.
            buffer.Replace("ControlTypes.", "");
            buffer.Replace("ControlType.", "");
            return buffer.ToString();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// <param name="eventId"></param>
        /// <returns>Name without any additional information. Example: InvokePattern.InvokedEvent
        /// This method is used only for textual display of the name within the log file</returns>
        /// -------------------------------------------------------------------
        static internal string GetProgrammaticName(AutomationEvent eventId)
        {
            return ProgrammaticName(eventId.ProgrammaticName);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// <param name="identifier"></param>
        /// <returns>Name without any additional information. Example: GridItem.ContainingGridProperty
        /// This method is used only for textual display of the name within the log file</returns>
        /// -------------------------------------------------------------------
        static internal string GetProgrammaticName(AutomationIdentifier identifier)
        {
            return ProgrammaticName(identifier.ProgrammaticName);
        }

        /// -------------------------------------------------------------------
        /// <summary>Helper for breaking changes</summary>
        /// -------------------------------------------------------------------
        static string ProgrammaticName(string buffer)
        {
            // TODO$: This code is not needed after the breaking changes.  Previoulsy names had
            // a prefix such as 'C:' which had to be removed.  This was added per Mic's comment to 
            // be proactive for the beaking change.  After this, this can be removed.
            int iLoc = buffer.IndexOf(":");
            if (iLoc != -1)
                return buffer.Substring(iLoc + 1);
            else
                return buffer;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Return the elements encompassing parent that has an Hwnd. This is 
        /// needed for elements such as a thumb on a list box.  In this case
        /// it will return ControlType.List
        /// </summary>
        /// -------------------------------------------------------------------
        internal static void GetParentHwndControl(AutomationElement le, out AutomationElement parent)
        {
            parent = le;
            while (parent.Current.NativeWindowHandle == 0)
                parent = TreeWalker.ControlViewWalker.GetParent(parent);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Returns the current listitem index of the element
        /// </summary>
        /// -------------------------------------------------------------------
        internal static int GetItemIndex(AutomationElement le)
        {
            int index = -1;
            if (le.Current.ControlType != ControlType.ListItem)
                throw new ArgumentException("Passed in a '" + le.Current.ControlType.ProgrammaticName + "' control when expecting a '" + ControlType.ListItem.ProgrammaticName + "'");

            // Assume that the order is same as list items
            while (le != null && le.Current.ControlType == ControlType.ListItem)
            {
                index++;
                le = TreeWalker.ControlViewWalker.GetPreviousSibling(le);
            }

            return index;
        }

        /// -------------------------------------------------------------------
        /// <summary>This will determine if a particular suite
        /// needs to be excluded</summary>
        /// <param name="suiteName"></param>
        /// <param name="controlName"></param>
        /// <returns>true if the suite corresponding to the control
        /// needs to be excluded.</returns>
        static internal bool CheckSuiteExclusions(string suiteName, string controlName)
        {
            //Some tests don't make sense for PasswordBox even though the pattern is supported. 
            //Text ones will be excluded but the Value ones might need to be tweaked and reenabled
            if ((controlName == "PasswordBox") && ((suiteName == "ValueTests")) || (suiteName == "TextTests"))
                return true;

            else if ((controlName == "Calendar") && (suiteName == "SelectionTests"))
                return true;

            else
                return false;
        }
    }
}

    
