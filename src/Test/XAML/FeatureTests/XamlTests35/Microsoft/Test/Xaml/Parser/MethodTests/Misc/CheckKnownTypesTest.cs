// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Misc
{
    /// <summary>
    /// XamlTestType for the ParserLoadTest
    /// </summary>
    public class CheckKnownTypesTest
    {
        #region Data

        /// <summary> Assembly List </summary>
        private Assembly[] _assemblies;

        /// <summary> Known Types </summary>
        private Type _knownTypes;

        /// <summary> Known Elements </summary>
        private Type _knownElements;

        /// <summary> Type Indexer </summary>
        private Type _typeIndexer;

        /// <summary> Instance using the known type ID </summary>
        private object _element;

        /// <summary> List of excluded types </summary>
        private List<string> _excludedTypes = new List<string>() 
                                                    {
                                                        "System.Windows.Markup.WhitespaceSignificantCollectionAttribute", 
                                                        "System.Windows.Markup.ContentPropertyAttribute",
                                                        "System.Windows.Markup.XamlReader" 
                                                    };

        /// <summary> KnownTypes.CreateKnownElement function </summary>
        private MethodInfo _createKnownElement;

        #endregion

        /// <summary>
        ///  Runs a ParserLoadTest which consists of:
        ///     Loading the xaml file and it's associated verifier
        ///     Verifying the object tree
        ///     Reloading the xaml for each mode and comparing the object trees
        /// Each test produces multiple variations, the number of which is determined by the number of ways
        /// to load a xaml file for the parser being tested
        /// </summary>
        public void VerifyKnownTypes()
        {
            GlobalLog.LogStatus("DisplayName: " + DriverState.TestName);

            _assemblies = new Assembly[]
            {
                typeof(FrameworkElement).Assembly,
                typeof(UIElement).Assembly,
                typeof(DependencyObject).Assembly
            };

            _knownTypes = GetType("System.Windows.Markup.KnownTypes");
            _knownElements = GetType("System.Windows.Markup.KnownElements");
            _typeIndexer = GetType("System.Windows.Markup.TypeIndexer");

            Array knownElementsArray = Enum.GetValues(_knownElements);

            foreach (object obj in knownElementsArray)
            {
                DoCheckKnownTypesAction(obj);
                TestLog.Current.LogStatus("\r\n");
            }
        }

        #region PrivateMembers

        /// <summary>
        /// Verifies that have parseable type has an Id in KnownElements.
        /// Verifies that KnownTypes.Types[&lt;known element id&gt;] returns the correct type.
        /// Verifies that KnownTypes.createKnownElement() returns an element of the correct type.
        /// Verifies that KnownTypes.KnownTypeConverterId() returns an Id if the type has a TypeConverter.
        /// Verifies that KnownTypes.createKnownElement() returns an instance of the correct TypeConverter.
        /// </summary>
        /// <param name="elementId">the element's id</param>
        private void DoCheckKnownTypesAction(object elementId)
        {
            TestLog.Current.LogStatus("CheckKnownTypes: " + elementId.ToString());

            TestLog.Current.Result = TestResult.Pass;
            XamlReader c = new XamlReader();

            if ((elementId.ToString().Equals("UnknownElement") == false) &&
                (elementId.ToString().Equals("MaxElement") == false))
            {
                // KnownTypes.Types[]
                // Verify that KnownTypes.Types[KnownElements.<typeName>] returns the  type.
                if (CheckKnownTypesIndexing(elementId) == null) return;
                Type type = CheckKnownTypesIndexing(elementId) as Type;

                TestLog.Current.LogStatus("Type found: " + type.Name);

                if ((type.GetConstructor(new Type[0] { }) != null) && (_excludedTypes.Contains(type.ToString()) == false))
                {
                    // KnownTypes.createKnownElement()
                    // Create an instance using the known type ID.
                    // Verify the instance type returned is the correct type.
                    if (!CheckKnownTypesCreateKnownElement(elementId, type)) return;
                    TestLog.Current.LogStatus("---- Passed KnownTypes.createKnownElement ----");

                    FixSpecialBehaviorElements();

                    // The issue with the following types is that the converter from TypeDescriptor and KnownTypes do not match
                    // This has been there since 3.0 and since KnownTypes.cs is only used in the Loc path, this is a Wont Fix (unlikely to be fixed in future versions)
                    if (!type.Name.Equals("Object") && !type.Name.Equals("StaticExtension") && !type.Name.Equals("TypeExtension"))
                    {
                        // KnownTypes.GetKnownTypeConverterId()
                        // Get TypeConverter for element via KnownTypes.
                        // Get TypeConverter for element via TypeDescriptor.
                        // Verify they are the same.
                        if (CheckKnownTypesGetKnownTypeConverterId(elementId, type) == false) return;
                        TestLog.Current.LogStatus("---- Passed KnownTypes.GetKnownTypeConverterId ----");

                        // KnownTypes.GetKnownTypeConverterIdForProperty()
                        // For each property, try to get a TypeConverter.
                        // If we get one, compare it against the one that PropertyDescriptor returns.
                        if (CheckKnownTypesGetKnownTypeConverterIdForProperty(elementId, type) == false) return;
                        TestLog.Current.LogStatus("---- Passed KnownTypes.GetKnownTypeConverterIdForProperty ----");
                    }
                }
            }

            TestLog.Current.Result = TestResult.Pass;
            return;
        }

        /// <summary>
        /// Checks the known types indexing.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <returns>knowntype object</returns>
        private object CheckKnownTypesIndexing(object elementId)
        {
            // KnownTypes.Types[]
            // Verify that KnownTypes.Types[KnownElements.<typeName>] returns the  type.
            object obj = _knownTypes.GetProperty("Types", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
            PropertyInfo item = _typeIndexer.GetProperty("Item");
            object knownType = item.GetValue(obj, new object[] { elementId });

            if (knownType == null)
            {
                TestLog.Current.LogEvidence("[FAIL] KnownTypes.Types[KnownElements." + elementId.ToString() + "] returned null.");
                TestLog.Current.Result = TestResult.Fail;
            }

            return knownType;
        }

        /// <summary>
        /// Checks the known types create known element.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <param name="indexedType">Type of the indexed.</param>
        /// <returns>returns bool value</returns>
        private bool CheckKnownTypesCreateKnownElement(object elementId, Type indexedType)
        {
            // KnownTypes.createKnownElement()
            // Create an instance using the known type ID.
            // Verify the instance type returned is the correct type.
            _createKnownElement = _knownTypes.GetMethod("CreateKnownElement", BindingFlags.NonPublic | BindingFlags.Static);
            _element = _createKnownElement.Invoke(null, BindingFlags.Static | BindingFlags.NonPublic, null, new object[] { elementId }, System.Globalization.CultureInfo.InvariantCulture);

            if (_element == null)
            {
                TestLog.Current.LogEvidence("[FAIL] KnownTypes.createKnownElement(KnownElements." + elementId.ToString() + ") returned null.");
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            Type elementType = _element.GetType();
            if (!elementType.Equals(indexedType))
            {
                TestLog.Current.LogEvidence("[FAIL] KnownTypes.createKnownElement(KnownElements." + elementId.ToString() + ") returned the wrong type of object: '" + elementType + "'.");
                TestLog.Current.Result = TestResult.Fail;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the known types get known type converter id.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <param name="type">The type value.</param>
        /// <returns>returns bool value</returns>
        private bool CheckKnownTypesGetKnownTypeConverterId(object elementId, Type type)
        {
            // KnownTypes.GetKnownTypeConverterId()
            // Get TypeConverter for element via KnownTypes.
            // Get TypeConverter for element via TypeDescriptor.
            // Verify they are the same.
            TypeConverter expectedConverter = TypeDescriptor.GetConverter(_element, true);

            MethodInfo getKnownTypeConverterId = _knownTypes.GetMethod("GetKnownTypeConverterId", BindingFlags.NonPublic | BindingFlags.Static);

            object converterId = getKnownTypeConverterId.Invoke(null, new object[] { elementId });

            TypeConverter knownConverter = _createKnownElement.Invoke(null, new object[] { converterId }) as TypeConverter;

            Type expectedConverterType = expectedConverter.GetType();

            if (knownConverter != null)
            {
                if (expectedConverter == null || knownConverter.GetType() != expectedConverter.GetType())
                {
                    TestLog.Current.LogEvidence("[FAIL] Could not get the expected converter for '" + type.Name + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: '" + knownConverter.GetType().Name + "'");
                    TestLog.Current.Result = TestResult.Fail;
                    return false;
                }
            }
            else
            {
                if (!IsDefaultConverter(expectedConverter))
                {
                    TestLog.Current.LogEvidence("[FAIL] Could not get the expected converter for '" + type.Name + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: 'null'");
                    TestLog.Current.Result = TestResult.Fail;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks the known types get known type converter id for property.
        /// </summary>
        /// <param name="elementId">The element id.</param>
        /// <param name="type">The type value.</param>
        /// <returns>returns bool value</returns>
        private bool CheckKnownTypesGetKnownTypeConverterIdForProperty(object elementId, Type type)
        {
            // KnownTypes.GetKnownTypeConverterIdForProperty()
            // For each property, try to get a TypeConverter.
            // If we get one, compare it against the one that PropertyDescriptor returns.
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(_element, true);
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            MethodInfo getKnownTypeConverterIdForProperty = _knownTypes.GetMethod("GetKnownTypeConverterIdForProperty", BindingFlags.NonPublic | BindingFlags.Static);
            PropertyInfo info = null;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                info = propertyInfos[i];

                if (String.Compare(info.Name, "Item", true) == 0)
                {
                    continue;
                }

                // Get converter from KnownTypes.
                object converterId = getKnownTypeConverterIdForProperty.Invoke(null, new object[] { elementId, info.Name });

                TypeConverter knownConverter = _createKnownElement.Invoke(null, new object[] { converterId }) as TypeConverter;

                TypeConverter expectedConverter = properties[info.Name].Converter;

                // If converter was found, compare it against the one from PropertyDescriptor.
                if (knownConverter != null)
                {
                    if (expectedConverter == null || knownConverter.GetType() != expectedConverter.GetType())
                    {
                        TestLog.Current.LogEvidence("[FAIL] Could not get the expected converter for '" + type.Name + "." + info.Name + "' using KnownTypes. Expected: '" + expectedConverter.GetType().Name + "', Actual: '" + knownConverter.GetType().Name + "'");
                        TestLog.Current.Result = TestResult.Fail;
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Helpers.

        /// <summary>
        /// Fixes the special behavior elements.
        /// </summary>
        private void FixSpecialBehaviorElements()
        {
            string typeString = _element.GetType().ToString();
            if (typeString.Equals("System.Windows.Media.GlyphTypeface")) // Empty constructor doesnt call initialize but GetHashCode specifically checks this
            {
                string fontPath = Path.Combine(Environment.SystemDirectory, "..\\Fonts\\Arial.ttf");
                _element = new System.Windows.Media.GlyphTypeface(new Uri(fontPath));
            }
        }

        /// <summary>
        /// is default converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        /// <returns>returns bool value</returns>
        private bool IsDefaultConverter(TypeConverter converter)
        {
            Type type = converter.GetType();

            string assemblyName = type.Assembly.GetName().Name;

            if (type.FullName.IndexOf("System.ComponentModel") != -1 &&
                (assemblyName.IndexOf("mscorlib") != -1
                || assemblyName.IndexOf("System") != -1))
            {
                return true;
            }

            if (type.Equals(typeof(System.ComponentModel.CollectionConverter))
                || type.Equals(typeof(System.ComponentModel.StringConverter))
                || type.Equals(typeof(System.ComponentModel.TypeConverter))
                || type.Equals(typeof(System.ComponentModel.BaseNumberConverter))
                || type.Equals(typeof(System.ComponentModel.BooleanConverter))
                || type.Equals(typeof(System.ComponentModel.CharConverter))
                || type.Equals(typeof(System.ComponentModel.CultureInfoConverter))
                || type.Equals(typeof(System.ComponentModel.DateTimeConverter))
                || type.Equals(typeof(System.ComponentModel.EnumConverter))
                || type.Equals(typeof(System.ComponentModel.ExpandableObjectConverter))
                || type.Equals(typeof(System.ComponentModel.GuidConverter))
                || type.Equals(typeof(System.ComponentModel.ReferenceConverter))
                || type.Equals(typeof(System.ComponentModel.StringConverter))
                || type.Equals(typeof(System.ComponentModel.TimeSpanConverter))
                || type.Equals(typeof(System.ComponentModel.TypeListConverter))
                || type.Equals(typeof(System.Drawing.ColorConverter))
                || type.Equals(typeof(System.Drawing.FontConverter))
                || type.Equals(typeof(System.Drawing.ImageConverter))
                || type.Equals(typeof(System.Drawing.ImageFormatConverter))
                || type.Equals(typeof(System.Drawing.PointConverter))
                || type.Equals(typeof(System.Drawing.RectangleConverter))
                || type.Equals(typeof(System.Drawing.SizeConverter))
                || type.Equals(typeof(System.Resources.ResXFileRef.Converter))
                || type.Equals(typeof(System.Windows.Forms.CursorConverter))
                || type.Equals(typeof(System.Windows.Forms.KeysConverter))
                || type.Equals(typeof(System.Windows.Forms.ListBindingConverter))
                || type.Equals(typeof(System.Windows.Forms.OpacityConverter))
                || type.Equals(typeof(System.Windows.Forms.SelectionRangeConverter))
                || type.Equals(typeof(System.Windows.Forms.TreeNodeConverter)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets The type value.
        /// </summary>
        /// <param name="typeName">Name of The type value.</param>
        /// <returns>returns type.</returns>
        private Type GetType(string typeName)
        {
            Type type = null;

            for (int i = 0; type == null && i < _assemblies.Length; i++)
            {
                type = _assemblies[i].GetType(typeName, false, false);
            }

            // type = Utility.FindType(typeName, true);
            if (type == null)
            {
                throw new Exception("Could not find type '" + typeName + "'.");
            }

            return type;
        }

        #endregion
    }
}
