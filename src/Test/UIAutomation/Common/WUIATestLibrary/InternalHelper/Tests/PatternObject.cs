// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
* Purpose: InternalHelper
* Owner: Microsoft
* Contributors:
*******************************************************************/
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Collections;
using Drawing = System.Drawing;
using System.CodeDom;
using System.Text;
using System.Threading;
using MS.Win32;

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

namespace InternalHelper.Tests
{
    using InternalHelper.Enumerations;
    using Microsoft.Test.WindowsUIAutomation;
    using Microsoft.Test.WindowsUIAutomation.Core;
    using Microsoft.Test.WindowsUIAutomation.Interfaces;
	using Microsoft.Test.WindowsUIAutomation.TestManager;

    /// -----------------------------------------------------------------------
    /// <summary></summary>
    /// -----------------------------------------------------------------------
    public class PatternObject : TestObject
    {
        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal const string NAMESPACE = IDSStrings.IDS_NAMESPACE_UIVERIFY + "." + IDSStrings.IDS_NAMESPACE_PATTERN;

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        ArrayList _cachedPatternList = new ArrayList();

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        Hashtable _seenPatterns = new Hashtable();

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        public PatternObject(AutomationElement element, string testSuite, TestPriorities priority, TypeOfControl typeOfControl, TypeOfPattern typeOfPattern, string dirResults, bool testEvents, IApplicationCommands commands)
            :
		base (element, testSuite, priority, typeOfControl, typeOfPattern, dirResults, testEvents, commands)
        {
            _testCaseSampleType = TestCaseSampleType.Pattern;
        }

        #region HierarchyItem

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal enum NumberCompareType
        {
            LessThan,
            LessThanEqual,
            Equal,
            GreaterThan,
            GreaterThanEqual
        }

        #endregion HierarchyItem

        #region Lib

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal enum ObjectTypes
        {
            Boolean = 0,
            Char = 1,
            SByte = 2,
            Byte = 3,
            Int16 = 4,
            Int32 = 5,
            UInt16 = 6,
            UInt32 = 7,
            Int64 = 8,
            UInt64 = 9,
            Single = 10,
            Decimal = 11,
            Double = 12,
            DateTime = 13,
            Date = 14,
            String = 15,
            ItemCheckState = 16,
            IPAddress = 17,
            Octet = 18,
            DateTimeArray = 19,
            Unknown = 20,
            Last = 20
            //Date = 16,
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ObjectTypes[] SupportedType(object currentObject)
        {
            Type type = currentObject.GetType();

            if (type == typeof(DateTime[]))
                return new ObjectTypes[] { ObjectTypes.DateTimeArray, ObjectTypes.Unknown };
            else if (type == typeof(System.Boolean))
                return new ObjectTypes[] { ObjectTypes.Boolean, ObjectTypes.Unknown };
            else if (type == typeof(System.Char))
                return new ObjectTypes[] { ObjectTypes.Char, ObjectTypes.Unknown };
            else if (type == typeof(System.Byte))
                return new ObjectTypes[] { ObjectTypes.Byte, ObjectTypes.SByte, ObjectTypes.Unknown };
            else if (type == typeof(System.Int16))
                return new ObjectTypes[] { ObjectTypes.Int16, ObjectTypes.Unknown };
            else if (type == typeof(System.Int32))
                return new ObjectTypes[] { ObjectTypes.Int16, ObjectTypes.Int32, ObjectTypes.Unknown };
            else if (type == typeof(System.Int64))
                return new ObjectTypes[] { ObjectTypes.Int16, ObjectTypes.Int32, ObjectTypes.Int64, ObjectTypes.Unknown };
            else if (type == typeof(System.UInt16))
                return new ObjectTypes[] { ObjectTypes.UInt16, ObjectTypes.Unknown };
            else if (type == typeof(System.UInt32))
                return new ObjectTypes[] { ObjectTypes.Int16, ObjectTypes.Int32, ObjectTypes.UInt16, ObjectTypes.UInt32, ObjectTypes.Unknown };
            else if (type == typeof(System.UInt64))
                return new ObjectTypes[] { ObjectTypes.Int16, ObjectTypes.Int32, ObjectTypes.Int64, ObjectTypes.UInt16, ObjectTypes.UInt32, ObjectTypes.UInt64, ObjectTypes.Unknown };
            else if (type == typeof(System.Single))
                return new ObjectTypes[] { ObjectTypes.Single, ObjectTypes.Int16, ObjectTypes.Int32, ObjectTypes.Unknown };
            else if (type == typeof(System.Decimal))
                return new ObjectTypes[] { ObjectTypes.Decimal, ObjectTypes.Unknown };
            else if (type == typeof(System.Double))
                return new ObjectTypes[] { ObjectTypes.Double, ObjectTypes.Unknown };
            else if (type == typeof(System.DateTime))
                return new ObjectTypes[] { ObjectTypes.DateTime, ObjectTypes.Date, ObjectTypes.Unknown };
            else if (type == typeof(System.String))
                return new ObjectTypes[] { ObjectTypes.Boolean, ObjectTypes.Char, ObjectTypes.SByte, ObjectTypes.Byte, ObjectTypes.Int16, ObjectTypes.Int32, ObjectTypes.UInt16, ObjectTypes.UInt32, ObjectTypes.Int64, ObjectTypes.UInt64, ObjectTypes.Single, ObjectTypes.Decimal, ObjectTypes.Double, ObjectTypes.DateTime, ObjectTypes.String, ObjectTypes.Octet, ObjectTypes.ItemCheckState, ObjectTypes.Date, ObjectTypes.IPAddress, ObjectTypes.Unknown };
            else
                throw new Exception("Inhandled type(" + type.ToString() + ")");
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_SupportsDataType(ObjectTypes objType, object CurrentValue, bool CanBeNull, CheckType checkType)
        {
            Comment("Determining if " + CurrentValue + " supports " + objType + " type");
            if (CanBeNull && (CurrentValue == null))
            {
                Comment("Object was equal to null and is allowed");
            }
            else
            {
                // This is a step, since we said it could not support null values if we get to this location
                if (CurrentValue == null)
                    ThrowMe(CheckType.Verification, "Cannot check to see if CurrentValue supports " + objType + " since CurrentValue = null so cannot call CurrentValue.GetType()");

                if (!SupportsDataType(objType, CurrentValue))
                    ThrowMe(checkType, CurrentValue.GetType() + " does not support " + objType);
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal bool SupportsDataType(ObjectTypes objType, object CurrentValue)
        {
            Comment(" Checking to see if " + CurrentValue.GetType() + " supports " + objType);

            ObjectTypes[] supportedDataType = SupportedType(CurrentValue);

            return (!Array.IndexOf(supportedDataType, objType).Equals(-1));
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_GetNonSupportedDataType(out ObjectTypes unSupported, object currentValue, CheckType checkType)
        {
            if (currentValue == null)
                ThrowMe(checkType, TestCaseCurrentStep + ": ValueAsObect == null, can't determine what datatype is unsupported without know the current valid datatype");

            ObjectTypes ot = Get_ObjectTypes(currentValue);

            unSupported = ObjectTypes.Unknown;

            ObjectTypes[] supportedForDataType = SupportedType(currentValue);

            do
            {
				unSupported = (ObjectTypes)Helpers.RandomValue(0, Convert.ToInt32(ObjectTypes.Last, CultureInfo.CurrentCulture));
            } while (!Array.IndexOf(supportedForDataType, unSupported).Equals(-1));

            Comment("Datatype of existing object is " + ot + ", returning datatype of " + unSupported);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TSC_GetClickablePoint(ref Point pt, bool expectedToHaveOne, string name, CheckType checkType)
        {
            try
            {
                pt = m_le.GetClickablePoint();
                Comment("GetClickablePoint returned (" + pt.X + ", " + pt.Y + ")");

                if (!expectedToHaveOne)
                {
                    ThrowMe(checkType, "GetClickablePoint() did not thow NoClickablePointException exception as expected");
                }
            }
            catch (System.Windows.Automation.NoClickablePointException)
            {
                if (expectedToHaveOne)
                    ThrowMe(checkType, "GetClickablePoint() threw the NoClickablePointException exception and did not expect this");
                else
                    Comment("GetClickablePoint() throw the NoClickablePointException exception as expected");
            }
            catch
            {
                throw; // not sure what happend so throw it
            }
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_Compare_AutomationElement(AutomationElement element, object compare, CheckType checkType)
        {
            if ((compare == null) && element != null)
                ThrowMe(checkType, Library.GetUISpyLook(element) + " != " + compare);

            if ((compare != null) && (element == null))
                ThrowMe(checkType, Library.GetUISpyLook(element) + " != " + compare);

            if ((compare != null) && (element != null))
                if (Automation.Compare((AutomationElement)compare, element).Equals(false))
                    ThrowMe(checkType, "{" + Library.GetUISpyLook((AutomationElement)compare) + "} does not equal tested Logical Element {" + Library.GetUISpyLook(element) + "}");
                else
                    Comment("Both elements are equal");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ObjectTypes Get_ObjectTypes(object obj)
        {
            Type type = obj.GetType(); //m_pattern.ValueAsObject.GetType();

            switch (type.FullName)
            {
                case "System.Boolean":
                    return ObjectTypes.Boolean;

                case "System.Char":
                    return ObjectTypes.Char;

                case "System.SByte":
                    return ObjectTypes.SByte;

                case "System.Byte":
                    return ObjectTypes.Byte;

                case "System.Int16":
                    return ObjectTypes.Int16;

                case "System.Int32":
                    return ObjectTypes.Int32;

                case "System.UInt16":
                    return ObjectTypes.UInt16;

                case "System.UInt32":
                    return ObjectTypes.UInt32;

                case "System.Int64":
                    return ObjectTypes.Int64;

                case "System.UInt64":
                    return ObjectTypes.UInt64;

                case "System.Single":
                    return ObjectTypes.Single;

                case "System.Decimal":
                    return ObjectTypes.Decimal;

                case "System.Double":
                    return ObjectTypes.Double;

                case "System.DateTime":
                    return ObjectTypes.DateTime;

                case "System.String":
                    return ObjectTypes.String;

                case "ItemCheckState":
                    return ObjectTypes.ItemCheckState;

                case "IPAddress":
                    return ObjectTypes.IPAddress;

                case "Unknown":
                    return ObjectTypes.Unknown;

                default:
                    throw new Exception("Unhandled switch value");
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_VerifyMinimumLessThanEqualValueAsObjectLessThanEqualMaximum(object Min, object Between, object Max, CheckType checkType)
        {
            Type BetweenType = Between.GetType();
            bool IsIt;

            // Special case for DateTime arrays
            if (Between.GetType().Equals(typeof(System.DateTime[])))
            {
                if (((System.DateTime[])Between)[0] < (System.DateTime)Min)
                    ThrowMe(checkType, "Date is less than min date");

                if (((System.DateTime[])Between)[1] > (System.DateTime)Max)
                    ThrowMe(checkType, "Date is more than max date");

                m_TestStep++;
                return;
            }

            // Make sure we have all the same data types
            if (!Min.GetType().Equals(BetweenType))
                ThrowMe(checkType, "Data types not the same: 1st Number(" + Min.GetType() + ") != 2nd Number(" + Between.GetType() + ")");

            if (!Max.GetType().Equals(BetweenType))
                ThrowMe(checkType, "Data types not the same: 2st Number(" + Between.GetType() + ") != 3nd Number(" + Max.GetType() + ")");

            switch (BetweenType.FullName)
            {
                case "System.Boolean":
                    throw new Exception("Cannot compare two booleans");

                case "System.Char":
                    IsIt = (System.Char)Min <= (System.Char)Max && (System.Char)Between <= (System.Char)Max; ;
                    break;

                case "System.SByte":
                    IsIt = (System.SByte)Min <= (System.SByte)Max && (System.SByte)Between <= (System.SByte)Max; ;
                    break;

                case "System.Byte":
                    IsIt = (System.Byte)Min <= (System.Byte)Max && (System.Byte)Between <= (System.Byte)Max; ;
                    break;

                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                    IsIt = Convert.ToInt64(Min, CultureInfo.CurrentCulture) <= Convert.ToInt64(Max, CultureInfo.CurrentCulture) && Convert.ToInt64(Between, CultureInfo.CurrentCulture) <= Convert.ToInt64(Max, CultureInfo.CurrentCulture);
                    break;

                case "System.UInt16":
                case "System.UInt32":
                case "System.UInt64":
                    IsIt = Convert.ToInt64(Min, CultureInfo.CurrentCulture) <= Convert.ToInt64(Max, CultureInfo.CurrentCulture) && Convert.ToInt64(Between, CultureInfo.CurrentCulture) <= Convert.ToInt64(Max, CultureInfo.CurrentCulture);
                    break;

                case "System.Single":
                    IsIt = (System.Single)Min <= (System.Single)Max && (System.Single)Between <= (System.Single)Max;
                    break;

                case "System.Decimal":
                    IsIt = (System.Decimal)Min <= (System.Decimal)Max && (System.Decimal)Between <= (System.Decimal)Max;
                    break;

                case "System.Double":
                    IsIt = (System.Double)Min <= (System.Double)Max && (System.Double)Between <= (System.Double)Max;
                    break;

                case "System.DateTime":
                    IsIt = (System.DateTime)Min <= (System.DateTime)Between && (System.DateTime)Between <= (System.DateTime)Max;
                    break;

                case "System.String":
                    throw new Exception("Cannot compare two strings");

                case "ItemCheckState":
                    throw new Exception("Cannot compare two ItemCheckState");

                case "Date":
                    IsIt = (System.DateTime)Min <= (System.DateTime)Between && (System.DateTime)Between <= (System.DateTime)Max;
                    break;

                case "IPAddress":
                case "Unknown":
                default:
                    throw new Exception("Inhandled type(" + BetweenType + ")");
            }

            if (IsIt.Equals(false))
                ThrowMe(checkType, "1st Number(" + Min + ") !< 2nd Number(" + BetweenType + ") !< 3rd Number(" + Max + ")");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_VerifyObjectType(object obj, ObjectTypes objType, bool CanBeNull, CheckType checkType)
        {
            bool CorrectType = false;

            if (obj == null) //& CanBeNull.Equals(false))
                ThrowMe(checkType, TestCaseCurrentStep + ": ValueAsObject returned a null value");

            switch (objType)
            {
                case ObjectTypes.Boolean:
                    CorrectType = obj.GetType().Equals(typeof(System.Boolean));
                    break;

                case ObjectTypes.Char:
                    CorrectType = obj.GetType().Equals(typeof(System.Char));
                    break;

                case ObjectTypes.SByte:
                    CorrectType = obj.GetType().Equals(typeof(System.SByte));
                    break;

                case ObjectTypes.Byte:
                    CorrectType = obj.GetType().Equals(typeof(System.Byte));
                    break;

                case ObjectTypes.Int16:
                    CorrectType = obj.GetType().Equals(typeof(System.Int16));
                    break;

                case ObjectTypes.Int32:
                    CorrectType = obj.GetType().Equals(typeof(System.Int32));
                    break;

                case ObjectTypes.UInt16:
                    CorrectType = obj.GetType().Equals(typeof(System.UInt16));
                    break;

                case ObjectTypes.UInt32:
                    CorrectType = obj.GetType().Equals(typeof(System.UInt32));
                    break;

                case ObjectTypes.Int64:
                    CorrectType = obj.GetType().Equals(typeof(System.Int64));
                    break;

                case ObjectTypes.UInt64:
                    CorrectType = obj.GetType().Equals(typeof(System.UInt64));
                    break;

                case ObjectTypes.Single:
                    CorrectType = obj.GetType().Equals(typeof(System.Single));
                    break;

                case ObjectTypes.Decimal:
                    CorrectType = obj.GetType().Equals(typeof(System.Decimal));
                    break;

                case ObjectTypes.Double:
                    CorrectType = obj.GetType().Equals(typeof(System.Double));
                    break;

                case ObjectTypes.Date:
                    CorrectType = obj.GetType().Equals(typeof(System.DateTime));
                    break;

                case ObjectTypes.DateTime:
                    CorrectType = obj.GetType().Equals(typeof(System.DateTime));
                    break;

                case ObjectTypes.String:
                    CorrectType = obj.GetType().Equals(typeof(System.String));
                    break;

                case ObjectTypes.ItemCheckState:
                    CorrectType = obj.GetType().Equals(typeof(string));
                    break;

                case ObjectTypes.IPAddress:
                    CorrectType = obj.GetType().Equals(typeof(System.Net.IPAddress));
                    break;

                default:
                    ThrowMe(checkType, TestCaseCurrentStep + ": ValueAsObject returned " + obj.GetType().ToString());
                    break;
            }
            if (!CorrectType)
                ThrowMe(checkType, "Current object is '" + obj.GetType().ToString() + "' and not '" + objType + "'");

            Comment("Object type is '" + obj.GetType() + "'");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_GetRandomValue(out object obj, object CurrentType, ObjectTypes dataType, bool SameType, bool MustBeDifferentValue, CheckType checkType)
        {
            ObjectTypes newObjectType = dataType;

            // If we pass in unknown, determine the correct data type
            if (dataType.Equals(ObjectTypes.Unknown))
            {
                dataType = this.Get_ObjectTypes(CurrentType);
                if (dataType.Equals(ObjectTypes.IPAddress))
                    ThrowMe(CheckType.IncorrectElementConfiguration, "Don't know how to get a random IPAddress");
            }

            // Get a new type if SameType == false
            if (!SameType)
            {
                do
                {
					newObjectType = (ObjectTypes)(int)Helpers.RandomValue(0, (int)ObjectTypes.Last);
                } while (newObjectType.Equals(ObjectTypes.Last) || newObjectType.Equals(ObjectTypes.IPAddress) || newObjectType.Equals(newObjectType));

                dataType = newObjectType;
            }

            // Try 10 times to find the random value
            int TryCount = 0;

            do
            {
                if (TryCount > 10)
                    ThrowMe(checkType, "Could not determine a random value different from the current value in 10 tries");

				obj = Helpers.GetRandomValidValue(m_le, null);
            } while (MustBeDifferentValue.Equals(true) && obj.Equals(CurrentType));

            Comment("Returning random value of (" + obj.GetType() + ")" + obj);
            m_TestStep++;
        }

        #endregion Lib

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void KillApp()
        {
            AutomationElement app = m_le;
            WindowPattern wp = null;

            do
            {
                wp = app.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
                System.Diagnostics.Trace.WriteLine(Library.GetUISpyLook(app));
                if (wp != null)
                    break;

                app = TreeWalker.RawViewWalker.GetParent(app);
            } while (app != AutomationElement.RootElement);

            if (wp == null)
                ThrowMe(CheckType.Verification, "There is no element to get rid of, found the RootElement");

            wp.Close();
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void SetPatterns(AutomationElement element)
        {
            if (element == null)
                return;
            foreach (AutomationPattern ap in element.GetSupportedPatterns())
            {
                Comment("Element supports pattern: " + Automation.PatternName(ap).ToString());
                _cachedPatternList.Add(element.GetCurrentPattern(ap));
            }
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal object GetPattern(AutomationPattern patternId, CheckType ct)
        {
            object pattern = null;

            string sPattern = sPattern = patternId.ToString().Substring(2);
            foreach (BasePattern Id in _cachedPatternList)
            {
                StringBuilder s = new StringBuilder(Id.ToString());

                s.Replace("System.Windows.Automation.", "");
                s.Replace("Pattern", "");
                if (s.ToString().CompareTo(sPattern) == 0)
                {
                    pattern = Id;
                    m_TestStep++;
                    return pattern;
                }
            }

            if (pattern == null)
                ThrowMe(ct, "Element does not support the " + sPattern + " pattern");

            throw new Exception("This should never happen");
        }

        #region Selection

        /// -------------------------------------------------------------------
        /// <summary>Move a window using the NativeMethods.MoveWindow() method</summary>
        /// -------------------------------------------------------------------
        internal void TS_MoveElementsWindow(AutomationElement windowElement, double newX, double newY, CheckType checkType)
        {

            object objHwnd = windowElement.Current.NativeWindowHandle;

            IntPtr ptr = IntPtr.Zero;

            if (objHwnd is Int32)
                ptr = (IntPtr)(Convert.ToInt32(objHwnd, CultureInfo.CurrentCulture));
            else if (objHwnd is Int64)
                ptr = new IntPtr(Convert.ToInt64(objHwnd, CultureInfo.CurrentCulture));
            else
                throw new ArgumentException("Hwnd returned with type that we don't know how to handle: " + objHwnd.GetType().ToString());

            if (ptr == IntPtr.Zero)
                ThrowMe(checkType, "Could not get the handle to the element(window)");
            
            int width = (int)windowElement.Current.BoundingRectangle.Width;
            int height = (int)windowElement.Current.BoundingRectangle.Height;

            Comment("Moving window to (" + newX + ", " + newY + ", " + width + ", " + height + ")");

            SafeNativeMethods.MoveWindow(ptr, (int)newX, (int)newY, width, height, 1);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary>Walk the tree up from the AutoamtionElement until we find
        /// an object that supports WindowPattern.  Then move is according to the 
        /// coordinates passed in</summary>
        /// -------------------------------------------------------------------
        internal void TS_MoveElementsWindow(AutomationElement element, double newX, double newY, out Rect orginalRect, out AutomationElement windowElement, CheckType checkType)
        {
            while (
                (element != null) &&
                (!(bool)element.GetCurrentPropertyValue(AutomationElement.IsWindowPatternAvailableProperty)) &&
                (null != (element = TreeWalker.ControlViewWalker.GetParent(element)))
                )
            {
            }

            if (element == null)
                ThrowMe(checkType, "Element does not have an Ancestor that supports WindowPattern");

            windowElement = element;
            orginalRect = windowElement.Current.BoundingRectangle;

            TS_MoveElementsWindow(windowElement, newX, newY, checkType);
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_AtLeastOneSelectionRequired(SelectionPattern sip, bool expected, CheckType checkType)
        {
            bool actual = sip.Current.IsSelectionRequired;
            if (!actual.Equals(expected))
                ThrowMe(checkType, "IsSelectionRequired = " + actual + " but requires " + expected + " for test");

            Comment("IsSelectionRequired = " + actual);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_SupportsMultipleSelection(SelectionPattern sip, bool expected, CheckType checkType)
        {
            bool actual = sip.Current.CanSelectMultiple;

            if (!actual.Equals(expected))
                ThrowMe(checkType, "CanSelectMultiple = " + actual + " but requires " + expected + " for test");

            Comment("CanSelectMultiple = " + actual);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TSC_SelectElement(AutomationElement le, string name, Type expectedException, CheckType checkType)
        {
            SelectionItemPattern sip = le.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;

            if (sip == null)
                ThrowMe(checkType, "Could not get the selection item pattern for " + Library.GetUISpyLook(le));
            else
                Comment("Call Select() on : " + Library.GetUISpyLook(le)); 
            sip.Select();

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_IsSelected(AutomationElement element, bool ShouldElementBeSelected, CheckType checkType)
        {
            SelectionItemPattern sip = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);

            // Check to make sure all elements in 
            if (!sip.Current.IsSelected.Equals(ShouldElementBeSelected))
                ThrowMe(checkType, "IsSelected returned " + !ShouldElementBeSelected + " but expected " + ShouldElementBeSelected);

            Comment("IsSelected(" + Library.GetUISpyLook(element) + ") = " + sip.Current.IsSelected.Equals(ShouldElementBeSelected));
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_IsSelected(ArrayList array, bool shouldBeSelected, CheckType checkType)
        {
            SelectionItemPattern sip1;

            AutomationElement container = ((SelectionItemPattern)(((AutomationElement)array[0]).GetCurrentPattern(SelectionItemPattern.Pattern))).Current.SelectionContainer;

            SelectionPattern sp = container.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;

            AutomationElement[] elements = sp.Current.GetSelection();

            // The ones that are in the array, should be the same as shouldBeSelected
            for (int i = 0; i < elements.Length; i++)
            {
                foreach (AutomationElement el in array)
                {
                    if (Automation.Compare(elements[i], el) == true)
                    {
                        sip1 = el.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                        if (sip1.Current.IsSelected != shouldBeSelected)
                            ThrowMe(checkType, "AutomationElement(" + Library.GetUISpyLook(el) + ").IsSelected should be " + shouldBeSelected);
                        elements[i] = null;
                        break;
                    }
                }
            }

            // The rest of the ones that are in the array, should be not be the same as shouldBeSelected
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] != null)
                {
                    sip1 = elements[i].GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                    if (sip1.Current.IsSelected == shouldBeSelected)
                    {
                        ThrowMe(checkType, "AutomationElement(" + Library.GetUISpyLook(elements[i]) + ").IsSelected should be not be " + shouldBeSelected);
                    }
                }
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_AtLeastSelectableChildCount(AutomationElement element, int expectedCount, CheckType checkType)
        {

            ArrayList statements = new ArrayList();
			int count = Helpers.GetSelectableItems(element).Count;

            if (count < expectedCount)
                ThrowMe(checkType, "Expected " + expectedCount + " items to be selected but childcount is only " + count);

            Comment("GetChildrenCount returned " + count);
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_IsSelectable(AutomationElement element, bool Selectable, CheckType checkType)
        {
            SelectionItemPattern sip = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);

            if (!((sip != null) == Selectable))
                ThrowMe(checkType, "Element(" + Library.GetUISpyLook(element) + ") does not support the SelectionItemPattern so cannot be selected");

            string does = "does " + (Selectable.Equals(true) ? "" : "not ");

            Comment("Element(" + Library.GetUISpyLook(element) + ") " + does + "supports the SelectionItemPattern");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ArrayList TSC_SelectRandomElements(out ArrayList arraySelect, AutomationElement selectionContainer, int selectAtLeast, int selectNoMoreThan, CheckType checkType)
        {
            arraySelect = new ArrayList();
            ArrayList aecList = HelperGetContainersSelectableItems(selectionContainer);

            foreach (AutomationElement ae in aecList)
                Comment("Selectable item(" + Library.GetUISpyLook(ae) + ") found");

            SelectionItemPattern sip;

            bool firstSelected = false;

            // Calendars are special, you need to select contiguous elements
            if (selectionContainer.Current.ControlType == ControlType.Calendar)
            {
                // Pick between one and seven days to select
                int numDaysDesired = (int)Helpers.RandomValue(1, 6);
                AutomationElement nextSelected = (AutomationElement)aecList[(int)Helpers.RandomValue(1, aecList.Count - 1)];
                ((SelectionItemPattern)nextSelected.GetCurrentPattern(SelectionItemPattern.Pattern)).Select();
                arraySelect.Add(nextSelected);
                Comment("Calling Select(" + Library.GetUISpyLook(nextSelected) + " )");
                int selectCount = 0;

                // Select in the positive way
                while (
                    (null != (nextSelected = TreeWalker.ContentViewWalker.GetNextSibling(nextSelected))) && 
                    (bool)nextSelected.GetCurrentPropertyValue(AutomationElement.IsSelectionItemPatternAvailableProperty) && 
                    (++selectCount < numDaysDesired)
                    )
                {
                    ((SelectionItemPattern)nextSelected.GetCurrentPattern(SelectionItemPattern.Pattern)).AddToSelection();
                    Comment("Calling AddToSelection(" + Library.GetUISpyLook(nextSelected) + " )");
                    arraySelect.Add(nextSelected);
                }

                nextSelected = m_le;

                // If we run out going forward, then decrement
                while (
                    (selectCount++ < numDaysDesired)
                    && 
                    (null != (nextSelected = TreeWalker.ContentViewWalker.GetPreviousSibling(nextSelected)))
                    )
                {
                    ((SelectionItemPattern)nextSelected.GetCurrentPattern(SelectionItemPattern.Pattern)).AddToSelection();
                    Comment("Calling AddToSelection(" + Library.GetUISpyLook(nextSelected) + " )");
                    arraySelect.Add(nextSelected);
                }

            }
            else
            {
                // Make sure we hav enough to patternSelect
                if (aecList.Count < selectAtLeast)
                    ThrowMe(checkType, "There are not enough elements to select");

                // Make sure we don't patternSelect too many
                selectNoMoreThan = Math.Min(aecList.Count, selectNoMoreThan);

                int selectCount = 0;
                int i = -1;

                // Select...
                int tryCount = 0;

                for (; selectCount < selectNoMoreThan; )
                {
                    // Fail safe break out if we just can't patternSelect them items...try 10X's
                    if (tryCount++ > (selectNoMoreThan * 10))
                        ThrowMe(checkType, "Tried to select " + selectNoMoreThan + " items but could not in " + tryCount + " tries");

                    // Get a random element
                    i = (int)Helpers.RandomValue(0, aecList.Count);

                    sip = ((AutomationElement)aecList[i]).GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;

                    // found something that is not selected
                    if (sip.Current.IsSelected == false)
                    {
                        Comment("Selecting " + Library.GetUISpyLook((AutomationElement)aecList[i]));
                        if (firstSelected == false)
                        {
                            sip.Select();
                            firstSelected = true;
                        }
                        else
                            sip.AddToSelection();

                        arraySelect.Add(aecList[i]);
                        selectCount++;
                    }
                }
            }

            m_TestStep++;
            return arraySelect;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_GetRandomSelectableItem(AutomationElement selectionContainer, out AutomationElement element, bool selectedState, CheckType checkType)
        {
            Comment("Calling LibraryGetSelectionItems()");

            element = null;
            AutomationElement element2 = null; ;

            AutomationElementCollection IsSelected;

            if (selectionContainer.Current.ControlType == ControlType.Calendar)
            {
                IsSelected = selectionContainer.FindAll(TreeScope.Descendants, new PropertyCondition(SelectionItemPattern.IsSelectedProperty, true));

                if (IsSelected.Count > 0)
                {
                    switch (selectedState)
                    {
                        case true:  // Find something that is selected
                            {
                                // Return the 1st or last element since you can only remove exterior elements from selection
								element = IsSelected[(bool)Helpers.RandomValue(true, true) == true ? 0 : IsSelected.Count - 1];
                                break;
                            }

                        case false: // Find something that is not selected yet
                            {
								switch ((bool)Helpers.RandomValue(true, true))
                                {
                                    case true:  // Looking for something at the beginning of the selection
                                        element2 = TreeWalker.RawViewWalker.GetPreviousSibling(IsSelected[0]);

                                        // If we are at the start, get something from the tail
                                        if (element2 == null)
                                        {
                                            element2 = TreeWalker.RawViewWalker.GetNextSibling(IsSelected[IsSelected.Count - 1]);
                                        }

                                        // If for some reason they are all selected, then error out
                                        if (element2 == null)
                                            ThrowMe(checkType, "Could not get any element that was unselected");

                                        break;
                                    case false: // Looking for something at the end of the selection
                                        element2 = TreeWalker.RawViewWalker.GetNextSibling(IsSelected[IsSelected.Count - 1]);

                                        // If we are at the end, get something from the start
                                        if (element2 == null)
                                        {
                                            element2 = TreeWalker.RawViewWalker.GetPreviousSibling(IsSelected[0]);
                                        }

                                        // If for some reason they are all selected, then error out
                                        if (element2 == null)
                                            ThrowMe(checkType, "Could not get any element that was unselected");

                                        break;
                                }

                                element = element2;

                                break;
                            }
                    }
                }
            }
            else
            {

                System.Windows.Automation.Condition condition = new AndCondition
                    (
                    new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true),
                    new PropertyCondition(SelectionItemPattern.IsSelectedProperty, selectedState)
                    );

                IsSelected = selectionContainer.FindAll(TreeScope.Descendants, condition);

                if (IsSelected.Count > 0)
                {   // Return any element
					element = IsSelected[(int)Helpers.RandomValue(0, IsSelected.Count - 1)];
                }
            }

            if (element == null)
                ThrowMe(checkType, "Could not find element who's SelectionItemPattern.IsSelected = " + selectedState);

            Comment("Found AutomationElement(" + Library.GetUISpyLook(element) + ")");
            m_TestStep++;

        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_GetRandomSelectableItem(AutomationElement selectionContainer, out AutomationElement element, CheckType checkType)
        {

            Comment("Calling Helpers.GetSelectableItems()");
            ArrayList list = Helpers.GetSelectableItems(selectionContainer);

			element = (AutomationElement)list[(int)Helpers.RandomValue((int)0, list.Count)];
            if (element == null)
                ThrowMe(checkType, "Could not find an SelectionItem");

            Comment("Found AutomationElement(" + Library.GetUISpyLook(element) + ")");
            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal ArrayList Library_ArrayListSubtract(ArrayList bigArray, ArrayList smallArray)
        {
            bool removed = false;
            for (int i = bigArray.Count - 1; i > -1; i--)
            {
                removed = false;
                foreach (AutomationElement le_2 in smallArray)
                {
                    if (removed.Equals(false))
                    {
                        if ((Automation.Compare((AutomationElement)bigArray[i], le_2).Equals(true)))
                        {
                            bigArray.RemoveAt(i);
                            removed = true;
                        }
                    }
                }
            }
            return bigArray;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_UnselectAll(AutomationElement container, CheckType checkType)
        {

            AutomationElementCollection aec = container.FindAll(TreeScope.Descendants, new PropertyCondition(SelectionItemPattern.IsSelectedProperty, true));
            Comment("Before removing selection, there are currently " + aec.Count + " items selected");

            foreach (AutomationElement element in aec)
            {
                ((SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern)).RemoveFromSelection();
            }

            Comment("After removing selection, there are  " + aec.Count + " items selected");

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_CurrentEqualsSelection(AutomationElement container, ArrayList array, CheckType checkType)
        {
            int count = 0;

            SelectionPattern sp = container.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
            if (sp == null)
                ThrowMe(checkType, "Element does not support SelectionPattern");

            AutomationElement[] les = sp.Current.GetSelection();
            Comment("There are " + les.Length + " selected items according to element.Current.GetSelection()");

            Comment("There should be " + array.Count + " items selected");
            if (!les.Length.Equals(array.Count))
                ThrowMe(checkType, "m_pattern.Selection == " + count + ", but there should be " + array.Count + " items selected");


            Comment("Now compare that Current.GetSelection() is what should be selected");
            foreach (AutomationElement le in les)
            {
                Comment(" - Verify: (" + Library.GetUISpyLook(le) + ").IsSelected = true");
                if (array.IndexOf(le).Equals(false))
                    ThrowMe(checkType, "The selected elements does not equal to what it should be");
            }

            // Now compare what array says
            SelectionItemPattern sip;

            Comment("Now compare what we expected to be selected (items we called Select() on");
            foreach (AutomationElement le in array)
            {
                Comment(" - Verify: (SelectionItemPattern)" + Library.GetUISpyLook(le) + ".GetCurrentPattern(SelectionItemPattern.Pattern).IsSelected should be true");
                sip = (SelectionItemPattern)le.GetCurrentPattern(SelectionItemPattern.Pattern);
                if (sip.Current.IsSelected.Equals(false))
                    ThrowMe(CheckType.Verification, "Element is not selected when it should be");
            }

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_AddElementToSelection(AutomationElement element, Type expectedException, CheckType checkType)
        {
            string call = "SelectionItemPattern.AddToSelection()";
            try
            {
                SelectionItemPattern sip = element.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern;
                if (sip == null)
                    ThrowMe(CheckType.Verification, "Could not get SelectionItemPattern for " + Library.GetUISpyLook(element));

                Comment("Before calling " + call + " on LE(" + Library.GetUISpyLook(element) + ").IsSelected = " + sip.Current.IsSelected);

                sip.AddToSelection();

                Thread.Sleep(1);
                Comment("After calling " + call + " on LE(" + Library.GetUISpyLook(element) + ").IsSelected = " + sip.Current.IsSelected);
            }
            catch (Exception actualException)
            {
				if (Library.IsCriticalException(actualException))
                    throw;

                Comment("Exception occured : " + actualException.GetType().ToString());
                TestException(expectedException, actualException.GetType(), call, checkType);
                m_TestStep++;
                return;
            }
            TestNoException(expectedException, call, checkType);

            m_TestStep++;
        }

        /// -------------------------------------------------------------------
        /// <summary></summary>
        /// -------------------------------------------------------------------
        internal void TS_RemoveElementFromSelection(AutomationElement element, Type expectedException, CheckType checkType)
        {

            string call = "SelectionItemPattern.RemoveFromSelection()";
            try
            {
                Comment("Trying to RemoveElementFromSelection(" + Library.GetUISpyLook(element) + ")");
                SelectionItemPattern sip = (SelectionItemPattern)element.GetCurrentPattern(SelectionItemPattern.Pattern);
                sip.RemoveFromSelection();
                Thread.Sleep(1);
                Comment("After calling RemoveElementFromSelection on LE(" + Library.GetUISpyLook(element) + ").IsSelected = " + sip.Current.IsSelected);
            }
            catch (Exception actualException)
            {
				if (Library.IsCriticalException(actualException))
                    throw;

                Comment("Exception occured : " + actualException.GetType().ToString());
                TestException(expectedException, actualException.GetType(), call, checkType);
                return;
            }
            TestNoException(expectedException, call, checkType);
            m_TestStep++;
        }

        #endregion Selection

    }
}

