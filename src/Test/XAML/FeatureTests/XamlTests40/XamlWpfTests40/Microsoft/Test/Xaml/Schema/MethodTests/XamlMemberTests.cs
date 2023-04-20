// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Xaml.Schema.MethodTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xaml;
    using System.Xaml.Schema;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Types;

    /// <summary>
    /// Tests for the XamlMember API
    /// </summary>
    public class XamlMemberTests
    {
        /// <summary>
        /// Schema context that is shared by all tests 
        /// </summary>
        private XamlSchemaContext _context = new XamlSchemaContext();

        /// <summary>
        /// Keep track of the test result 
        /// </summary>
        private bool _testFailed = false;

        #region XamlMember tests

        /// <summary>
        ///   public property with getter and setter (P0)
        /// </summary>
        public void PublicPropertyWithGetterSetter()
        {
            PropertyInfo propertyInfo = typeof(MemberContainer).GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember = new XamlMember(propertyInfo, this._context);
            this.ValidateState(GetState(propertyInfo), GetState(xamlMember));
        }

        /// <summary>
        ///  Public property with no setter (P0)
        /// </summary>
        public void StringPropertyWithNoSetter()
        {
            PropertyInfo propertyInfo = typeof(MemberContainer).GetProperty("StringPropertyWithNoSetter");
            XamlMember xamlMember = new XamlMember(propertyInfo, this._context);
            this.ValidateState(GetState(propertyInfo), GetState(xamlMember));
        }

        /// <summary>
        /// property is an event (P0)
        /// </summary>
        public void PublicEventProperty()
        {
            EventInfo eventInfo = typeof(MemberContainer).GetEvent("PublicEventProperty");
            XamlMember xamlMember = new XamlMember(eventInfo, this._context);
            this.ValidateState(GetState(eventInfo), GetState(xamlMember));
        }

        /// <summary>
        /// Member that doesn’t exist on the XamlType (P0)
        /// </summary>
        public void UnknownXamlMember()
        {
            XamlMember xamlMember = new XamlMember("unknown", this._context.GetXamlType(typeof(MemberContainer)), false /*isAttachable*/);
            Dictionary<object, object> expected = GetState("unknown", typeof(MemberContainer));
            expected["Type"] = "Object";
            this.ValidateState(expected, GetState(xamlMember));
        }

        /// <summary>
        ///  Use unknown constructor – but matches existing member (P0)
        ///  Should this become known ... no XamlMember does not perform any resolution
        /// </summary>
        public void UnknownXamlMemberThatExists()
        {
            XamlMember xamlMember = new XamlMember("StringPropertyWithGetterSetter", this._context.GetXamlType(typeof(MemberContainer)), false /*isAttachable*/);
            Dictionary<object, object> expected = GetState("StringPropertyWithGetterSetter", typeof(MemberContainer));
            expected["Type"] = "Object";
            this.ValidateState(expected, GetState(xamlMember));
        }

        /// <summary>
        /// XamlMember.TypeConverter.ConverterInstance on an event
        /// should get back a converter
        /// </summary>
        public void EventConverter()
        {
            EventInfo eventInfo = typeof(MemberContainer).GetEvent("PublicEventProperty");
            XamlMember xamlMember = new XamlMember(eventInfo, this._context);
            if (xamlMember.TypeConverter.ConverterInstance == null)
            {
                GlobalLog.LogDebug("xamlMember.TypeConverter.ConverterInstance on Event is null");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogDebug("xamlMember.TypeConverter.ConverterInstance is: " + xamlMember.TypeConverter.ConverterInstance);
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        #endregion

        #region XamlAttachableProperty tests

        /// <summary>
        /// Create Attachable member with getter and setter (P0)
        /// </summary>
        public void KnownAttachableProperty()
        {
            Type containerType = typeof(MemberContainer);
            MethodInfo getter = containerType.GetMethod("GetStringProp");
            MethodInfo setter = containerType.GetMethod("SetStringProp");
            XamlMember xamlMember = new XamlMember("SetString", getter, setter, this._context);

            this.ValidateState(GetState(getter, setter), GetState(xamlMember));
        }

        /// <summary>
        ///  Property is an Event (P1)
        /// </summary>
        public void KnownAttachablePropertyEvent()
        {
            Type containerType = typeof(MemberContainer);
            MethodInfo setter = containerType.GetMethod("SetEventProp");
            XamlMember xamlMember = new XamlMember("SetEvent", setter, this._context);

            this.ValidateState(GetState(null, setter), GetState(xamlMember));
        }

        /// <summary>
        /// Create Attachable property with invalid getter and setter (does not have the correct signature)  (P2)
        /// Bug: This should throw 
        /// </summary>
        public void KnownAttachablePropertyInvalid()
        {
            Type containerType = typeof(MemberContainer);
            MethodInfo setter = containerType.GetMethod("SetInvalidProp");
            try
            {
                XamlMember xamlMember = new XamlMember("SetInvalid", null, setter, this._context);
            }
            catch (Exception)
            {
                TestLog.Current.Result = TestResult.Pass;
                return;
            }

            GlobalLog.LogEvidence("Did not throw as expected");
            TestLog.Current.Result = TestResult.Fail;
        }

        /// <summary>
        /// Create attachable property with unknown constructor (P0)
        /// Bug: getting target type on unknown throws null ref
        /// </summary>
        public void UnknownAttachableMember()
        {
            XamlMember xamlMember = new XamlMember("unknown", _context.GetXamlType(typeof(MemberContainer)), true /*isAttachable*/);
            Dictionary<object, object> expectedState = GetState("unknown", typeof(MemberContainer));
            expectedState.Remove("UnderlyingMember");
            expectedState["isAttachable"] = true;
            expectedState["isEvent"] = false;
            expectedState.Add("TargetType", "Object");
            expectedState.Add("UnderlyingGetter", null);
            expectedState.Add("UnderlyingSetter", null);
            expectedState["Type"] = "Object";
            this.ValidateState(expectedState, GetState(xamlMember));
        }

        #endregion

        #region XamlDirective tests

        /// <summary>
        /// Get XamlDirective from XamlLanguage and verify (P0)
        /// </summary>
        public void KnownXamlDirective()
        {
            XamlDirective directive = XamlLanguage.Key;

            XamlDirective createdDirective = new XamlDirective(directive.GetXamlNamespaces(), directive.Name, this._context.GetXamlType(typeof(object)), null, AllowedMemberLocations.Any);
            Dictionary<object, object> expected = GetState(directive);

            this.ValidateState(expected, GetState(createdDirective));
        }

        /// <summary>
        /// Create unknown directive using xmlns and name (P0)
        /// </summary>
        public void UnknownDirective()
        {
            XamlDirective directive = new XamlDirective(XamlLanguage.Xaml2006Namespace, "unknown");
            if (directive.IsUnknown)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Call GetHashCode after creating a XamlDirective with a null 2nd parameter - regression test
        /// The test passes if no exception is thrown.
        /// </summary>
        public void XamlDirectiveNullParameter()
        {
            XamlDirective d = new XamlDirective(string.Empty, null);
            int i = d.GetHashCode();

            TestLog.Current.Result = TestResult.Pass;
        }

        #endregion

        #region Equality tests

        /// <summary>
        /// Two XamlMembers (that are the same) (P0)
        /// </summary>
        public void SameXamlMemberCompare()
        {
            PropertyInfo propertyInfo = typeof(MemberContainer).GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember1 = new XamlMember(propertyInfo, this._context);
            XamlMember xamlMember2 = new XamlMember(propertyInfo, this._context);

            IsTrue(xamlMember1 == xamlMember2, "== operation");
            IsFalse(xamlMember1 != xamlMember2, "!= operation");
            IsTrue(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        ///  Two XamlMembers (that are not the same) (P0)
        /// </summary>
        public void DifferentXamlMemberCompare()
        {
            PropertyInfo propertyInfo1 = typeof(MemberContainer).GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);
            PropertyInfo propertyInfo2 = typeof(MemberContainer).GetProperty("StringPropertyWithNoSetter");
            XamlMember xamlMember2 = new XamlMember(propertyInfo2, this._context);

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// XamlMember and XamlAttachableMember(P0)
        /// </summary>
        public void XamlMemberXamlAttachableMemberCompare()
        {
            Type containerType = typeof(MemberContainer);
            PropertyInfo propertyInfo1 = containerType.GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);

            MethodInfo getter = containerType.GetMethod("GetStringProp");
            MethodInfo setter = containerType.GetMethod("SetStringProp");
            XamlMember xamlMember2 = new XamlMember("GetString", getter, setter, this._context);

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        ///  XamlAttachableMember and XamlAttachableMember (same member) (P0)
        /// </summary>
        public void SameXamlAttachableMemberCompare()
        {
            Type containerType = typeof(MemberContainer);
            MethodInfo getter = containerType.GetMethod("GetStringProp");
            MethodInfo setter = containerType.GetMethod("SetStringProp");
            XamlMember xamlMember1 = new XamlMember("GetString", getter, setter, this._context);
            XamlMember xamlMember2 = new XamlMember("GetString", getter, setter, this._context);

            IsTrue(xamlMember1 == xamlMember2, "== operation");
            IsFalse(xamlMember1 != xamlMember2, "!= operation");
            IsTrue(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// XamlMember and XamlAttachableMember (not the same member) (P0)
        /// </summary>
        public void DifferentXamlAttachableMemberCompare()
        {
            Type containerType = typeof(MemberContainer);
            MethodInfo getter = containerType.GetMethod("GetStringProp");
            MethodInfo setter = containerType.GetMethod("SetStringProp");
            XamlMember xamlMember1 = new XamlMember("GetString", getter, setter, this._context);
            MethodInfo adder = containerType.GetMethod("SetEventProp");
            XamlMember xamlMember2 = new XamlMember("SetEvent", adder, this._context);

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// XamlAttachableMember and XamlMember (P0)
        /// </summary>
        public void XamlAttachableMemberXamlMemberCompare()
        {
            Type containerType = typeof(MemberContainer);

            MethodInfo getter = containerType.GetMethod("GetStringProp");
            MethodInfo setter = containerType.GetMethod("SetStringProp");
            XamlMember xamlMember1 = new XamlMember("GetString", getter, setter, this._context);

            PropertyInfo propertyInfo1 = containerType.GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember2 = new XamlMember(propertyInfo1, this._context);

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// XamlDirective and XamlDirective (same) (P0)
        /// </summary>
        public void SameXamlDirectiveCompare()
        {
            XamlDirective xamlMember1 = XamlLanguage.Name;
            XamlDirective xamlMember2 = XamlLanguage.Name;

            IsTrue(xamlMember1 == xamlMember2, "== operation");
            IsFalse(xamlMember1 != xamlMember2, "!= operation");
            IsTrue(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        ///  XamlDirective and XamlDirective (not the same) (P0)
        /// </summary>
        public void DifferentXamlDirectiveCompare()
        {
            XamlDirective xamlMember1 = XamlLanguage.Name;
            XamlDirective xamlMember2 = XamlLanguage.Key;

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        ///  XamlDirective and XamlAttachableMember (P0)
        /// </summary>
        public void XamlDirectiveXamlAttachableMemberCompare()
        {
            XamlDirective xamlMember1 = XamlLanguage.Name;
            Type containerType = typeof(MemberContainer);

            MethodInfo getter = containerType.GetMethod("GetStringProp");
            MethodInfo setter = containerType.GetMethod("SetStringProp");
            XamlMember xamlMember2 = new XamlMember("GetString", getter, setter, this._context);

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// XamlMember and XamlAttachableMember (with same name) (P2)
        /// </summary>
        public void XamlMemberXamlAttachableMemberSameNameCompare()
        {
            Type containerType = typeof(MemberContainer);

            PropertyInfo propertyInfo1 = containerType.GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);

            MethodInfo getter = containerType.GetMethod("GetStringPropertyWithGetterSetterProp");
            MethodInfo setter = containerType.GetMethod("SetStringPropertyWithGetterSetterProp");
            XamlMember xamlMember2 = new XamlMember("StringPropertyWithGetterSetter", getter, setter, this._context);

            IsFalse(xamlMember1 == xamlMember2, "== operation");
            IsTrue(xamlMember1 != xamlMember2, "!= operation");
            IsFalse(xamlMember1.Equals(xamlMember2), ".Equals operation");

            if (!this._testFailed)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Property with type converter (P1)
        /// </summary>
        public void TypeConverterOnProperty()
        {
            Type containerType = typeof(MemberContainer);

            PropertyInfo propertyInfo1 = containerType.GetProperty("StringPropertyWithGetterSetter");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);

            if (xamlMember1.TypeConverter.Name != "StringTypeConverter1")
            {
                GlobalLog.LogEvidence("Failed: Text syntax value is incorrect");
                GlobalLog.LogDebug(xamlMember1.TypeConverter.Name);
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }
        #endregion

        #region TypeConverter tests

        /// <summary>
        /// Type with type converter (P1)
        /// </summary>
        public void TypeConverterOnType()
        {
            Type containerType = typeof(MemberContainer1);

            PropertyInfo propertyInfo1 = containerType.GetProperty("CustomType");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);

            if (xamlMember1.TypeConverter.Name != "CustomTypeConverter")
            {
                GlobalLog.LogEvidence("Failed: Text syntax value is incorrect");
                GlobalLog.LogEvidence(xamlMember1.TypeConverter.Name);
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Property and type with type converter (should choose property over type) (P1)
        /// </summary>
        public void TypeConverterOnTypeAndProperty()
        {
            Type containerType = typeof(MemberContainer1);

            PropertyInfo propertyInfo1 = containerType.GetProperty("CustomType1");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);

            if (xamlMember1.TypeConverter.Name != "StringTypeConverter1")
            {
                GlobalLog.LogEvidence("Failed: Text syntax value is incorrect");
                GlobalLog.LogEvidence(xamlMember1.TypeConverter.Name);
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Property with no type converter (P1)
        /// </summary>
        public void NoTypeConverterProperty()
        {
            Type containerType = typeof(MemberContainer);

            PropertyInfo propertyInfo1 = containerType.GetProperty("StringPropertyWithNoSetter");
            XamlMember xamlMember1 = new XamlMember(propertyInfo1, this._context);

            if (xamlMember1.TypeConverter.Name != "StringConverter")
            {
                GlobalLog.LogEvidence("Failed: Text syntax was not StringConverter ");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }
        #endregion

        #region private helper methods

        /// <summary>
        /// Get state for a XamlDirective
        /// </summary>
        /// <param name="directive"> The XamlDirective </param>
        /// <returns> State of the XamlDirective </returns>
        private Dictionary<object, object> GetState(XamlDirective directive)
        {
            Dictionary<object, object> state = GetState((XamlMember)directive);
            state["isDirective"] = true;
            state["AllowedLocation"] = directive.AllowedLocation;

            return state;
        }

        /// <summary>
        /// Validate if the value is False
        /// Fail the test if not
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <param name="message">message to print</param>
        private void IsFalse(bool value, string message)
        {
            IsTrue(!value, message);
        }

        /// <summary>
        /// Validate if the value is true
        /// Fail the test if not
        /// </summary>
        /// <param name="value">value to validate</param>
        /// <param name="message">message to print</param>
        private void IsTrue(bool value, string message)
        {
            if (value)
            {
                GlobalLog.LogDebug(message + " passed");
            }
            else
            {
                GlobalLog.LogEvidence(message + " failed");
                TestLog.Current.Result = TestResult.Fail;
                this._testFailed = true;
            }
        }

        /// <summary>
        /// Build state for unknown XamlMember
        /// </summary>
        /// <param name="name"> Name of the member </param>
        /// <param name="declaringType"> Type declaring the member </param>
        /// <returns> state build for unknown XamlMember </returns>
        private Dictionary<object, object> GetState(string name, Type declaringType)
        {
            Dictionary<object, object> memberState = new Dictionary<object, object>()
            {
                { "Name", name },
                { "DeclaringType", declaringType.Name },
                { "Type", null },
                { "isUnknown", true },
                { "isReadPublic", true },
                { "isWritePublic", true },
                { "isReadOnly", false },
                { "isAttachable", false },
                { "isEvent", false },
                { "isDirective", false },
                { "UnderlyingMember", null },
            };

            return memberState;
        }

        /// <summary>
        /// Get state from a memberInfo
        /// </summary>
        /// <param name="memberInfo"> The clr member </param>
        /// <returns> state built from memberInfo </returns>
        private Dictionary<object, object> GetState(MemberInfo memberInfo)
        {
            bool isReadPublic = false;
            bool isWritePublic = false;
            bool isReadOnly = false;
            string type = null;

            PropertyInfo propInfo = memberInfo as PropertyInfo;
            if (propInfo != null)
            {
                MethodInfo getMethod = propInfo.GetGetMethod();
                MethodInfo setMethod = propInfo.GetSetMethod();
                isReadPublic = getMethod == null ? false : getMethod.IsPublic;
                isWritePublic = setMethod == null ? false : setMethod.IsPublic;
                isReadOnly = (propInfo.GetSetMethod() == null);
                type = propInfo.PropertyType.Name;
            }

            EventInfo eventInfo = memberInfo as EventInfo;
            if (eventInfo != null)
            {
                // events aren't readable
                isReadPublic = false;
                isWritePublic = eventInfo.GetAddMethod().IsPublic;
                isReadOnly = false;
                type = eventInfo.EventHandlerType.Name;
            }

            Dictionary<object, object> memberState = new Dictionary<object, object>()
            {
                { "Name", memberInfo.Name },
                { "DeclaringType", memberInfo.DeclaringType.Name },
                { "Type", type },
                { "isUnknown", false },
                { "isReadPublic", isReadPublic },
                { "isWritePublic", isWritePublic },
                { "isReadOnly", isReadOnly },
                { "isAttachable", false },
                { "isEvent", eventInfo != null },
                { "isDirective", false },
                { "UnderlyingMember", memberInfo.Name },
            };

            return memberState;
        }

        /// <summary>
        /// Get state for attachable member
        /// </summary>
        /// <param name="getter"> the getter method </param>
        /// <param name="setter"> the setter method </param>
        /// <returns> state built for attachable member </returns>
        private Dictionary<object, object> GetState(MethodInfo getter, MethodInfo setter)
        {
            Type memberType = setter.GetParameters()[1].ParameterType;
            bool isEvent = false;
            if (memberType == typeof(EventHandler))
            {
                isEvent = true;
            }

            Dictionary<object, object> memberState = new Dictionary<object, object>()
            {
                { "Name", setter.Name.Replace("Prop", string.Empty) },
                { "DeclaringType", setter.DeclaringType.Name },
                { "Type", (memberType == null) ? null : memberType.Name },
                { "isUnknown", false },
                { "isReadPublic", getter == null ? false : getter.IsPublic },
                { "isWritePublic", setter.IsPublic },
                { "isReadOnly", !setter.IsPublic },
                { "isAttachable", true },
                { "isEvent", isEvent },
                { "isDirective", false },
                { "TargetType", (getter == null) ? typeof(object).Name : getter.GetParameters()[0].ParameterType.Name },
                { "UnderlyingGetter", (getter == null) ? null : getter.Name },
                { "UnderlyingSetter", setter.Name },
            };

            return memberState;
        }

        /// <summary>
        /// Get state for provide xamlProperty
        /// </summary>
        /// <param name="xamlProperty"> The xamlProperty </param>
        /// <returns> state built from xamlProperty </returns>
        private Dictionary<object, object> GetState(XamlMember xamlProperty)
        {
            Dictionary<object, object> memberState = new Dictionary<object, object>()
            {
                { "Name", xamlProperty.Name },
                { "DeclaringType", (xamlProperty.DeclaringType == null) ? null : xamlProperty.DeclaringType.Name },
                { "Type", (xamlProperty.Type == null) ? null : xamlProperty.Type.Name },
                { "isUnknown", xamlProperty.IsUnknown },
                { "isReadPublic", xamlProperty.IsReadPublic },
                { "isWritePublic", xamlProperty.IsWritePublic },
                { "isReadOnly", xamlProperty.IsReadOnly },
                { "isAttachable", xamlProperty.IsAttachable },
                { "isEvent", xamlProperty.IsEvent },
                { "isDirective", xamlProperty.IsDirective },
            };

            if (xamlProperty.IsAttachable)
            {
                memberState.Add("TargetType", (xamlProperty.TargetType == null) ? null : xamlProperty.TargetType.Name);
                memberState.Add("UnderlyingGetter", (xamlProperty.Invoker.UnderlyingGetter == null) ? null : xamlProperty.Invoker.UnderlyingGetter.Name);
                memberState.Add("UnderlyingSetter", (xamlProperty.Invoker.UnderlyingSetter == null) ? null : xamlProperty.Invoker.UnderlyingSetter.Name);
            }
            else if (!xamlProperty.IsDirective)
            {
                memberState.Add("UnderlyingMember", (xamlProperty.UnderlyingMember == null) ? null : xamlProperty.UnderlyingMember.Name);
            }

            return memberState;
        }

        /// <summary>
        ///  Validate the expected and actual states
        /// </summary>
        /// <param name="expected"> the epected state </param>
        /// <param name="actual"> the actual state </param>
        private void ValidateState(Dictionary<object, object> expected, Dictionary<object, object> actual)
        {
            // unordered validation //
            if (expected.Keys.Count != actual.Keys.Count)
            {
                GlobalLog.LogEvidence("Count does not match");
                TraceStates(expected, actual);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            foreach (string key in expected.Keys)
            {
                if (!actual.ContainsKey(key))
                {
                    TraceStates(expected, actual);
                    GlobalLog.LogEvidence("Actual does not contain " + key);
                    TestLog.Current.Result = TestResult.Fail;
                    this._testFailed = true;
                    return;
                }

                if (expected[key] == null && actual[key] == null)
                {
                    continue;
                }

                if ((expected[key] == null || actual[key] == null)
                    || (expected[key].ToString() != actual[key].ToString()))
                {
                    GlobalLog.LogEvidence("Did not match. Failed Key - " + key);
                    TraceStates(expected, actual);
                    TestLog.Current.Result = TestResult.Fail;
                    this._testFailed = true;
                    return;
                }
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Trace out expected and actual states
        /// </summary>
        /// <param name="expected"> the expected state </param>
        /// <param name="actual"> the actual state observed </param>
        private void TraceStates(Dictionary<object, object> expected, Dictionary<object, object> actual)
        {
            GlobalLog.LogDebug("Expected:");
            TraceState(expected);
            GlobalLog.LogDebug("Actual:");
            TraceState(actual);
        }

        /// <summary>
        /// Trace the state
        /// </summary>
        /// <param name="state"> state to trace </param>
        private void TraceState(Dictionary<object, object> state)
        {
            foreach (string key in state.Keys)
            {
                GlobalLog.LogDebug("{0}:{1}", key, state[key]);
            }
        }

        #endregion
    }
}
