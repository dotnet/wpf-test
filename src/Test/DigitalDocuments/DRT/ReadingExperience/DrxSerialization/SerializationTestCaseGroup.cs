// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Description:
//  This class represents a group of serialization test cases for a single type.
//
//

using System;
using System.Collections;               // For ArrayList
using System.ComponentModel;            // For DesignerSerializationVisibilityAttribute
using System.Reflection;
using System.Windows;                 // For DependencyProperty



namespace MS.Internal.WppDrt.EDocsUx
{
    /// <summary>
    ///   This class represents a group of serialization test cases for a single
    ///   type.
    /// </summary>
    internal class SerializationTestCaseGroup
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------
        #region Constructors
        
        /// <summary>
        ///   Constructor for the SerializationTestCaseGroup class.
        /// </summary>
        /// <param name="type">
        ///   The type of object to serialize.
        /// </param>
        /// <param name="knownPropertyNames">
        ///   Array of the names of the public read/write properties that this
        ///   type was known to have when this DRT was last updated.
        /// </param>
        /// <param name="testCases">
        ///   Array of test cases for this type.
        /// </param>
        internal SerializationTestCaseGroup(
            Type type,
            string[] knownPropertyNames,
            SerializationTestCase[] testCases
            )
        {
            if (type == null)
                throw new ArgumentNullException("type");
            _type = type;

            if (knownPropertyNames == null)
                throw new ArgumentNullException("knownPropertyNames");
            _knownPropertyNames = knownPropertyNames;

            if (testCases == null)
                throw new ArgumentNullException("testCases");
            _testCases = testCases;
        }

        #endregion Constructors

        //------------------------------------------------------
        //
        //  Internal methods
        //
        //------------------------------------------------------
        #region Internal methods

        /// <summary>
        ///   Execute this group of serialization test cases.
        /// </summary>
        /// <param name="host">
        ///   Object that provides services needed by serialization test cases.
        /// </param>
        /// <returns>
        ///   Returns 0 on success. On failure, returns the number of test cases
        ///   that failed.
        /// </returns>
        internal int Run(ISerializationTestCaseHost host)
        {
            int result = 0;
            SerializationTestCaseRunMode mode = host.Mode;            

            if (mode == SerializationTestCaseRunMode.RunDrt)
                Console.WriteLine("Testing " + _type.ToString() + ":");

            Console.WriteLine("Validating list of known properties...");
            if (ValidateProperties() == false)
                result = 1;
            
            for (int i = 0; i < _testCases.Length; ++i)
            {
                result += _testCases[i].Run(host);
            }

            return result;
        }

        #endregion Internal methods

        //------------------------------------------------------
        //
        //  Private methods
        //
        //------------------------------------------------------
        #region Private methods

        private bool ValidateProperties()
        {
            bool bResult = true;

            ArrayList arr = new ArrayList();    // The list of all public R/W properties
                                                //  that are not hidden from serialization.

            // Find all the public R/W CLR properties declared on this type (not on
            // its base types).
            PropertyInfo[] props = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (PropertyInfo prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                     // We have found a public R/W CLR property. Assume that it
                     // should be serialized unless reflection tells us otherwise.
                    bool serialized = true;

                    // Is the property marked with the attribute
                    // [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden]?
                    DesignerSerializationVisibilityAttribute visAttr =
                        Attribute.GetCustomAttribute(prop, typeof(DesignerSerializationVisibilityAttribute), false)
                        as DesignerSerializationVisibilityAttribute;
                    if (visAttr != null && visAttr.Visibility == DesignerSerializationVisibility.Hidden)
                    {
                        serialized = false;
                    }

                    if (serialized)
                    {
                        DependencyProperty dp = DependencyPropertyFromName(prop.Name + "Property", _type);
                        if (dp != null)
                        {
                            // There is a correspondingly named DP. Is it hidden from
                            // serialization because it's ReadOnly?
                            if (dp.ReadOnly)
                            {
                                serialized = false;
                            }
                        }
                    }
                    
                    if (serialized)
                        arr.Add(prop.Name);
                }
            }

            string[] currentProps = (string[])arr.ToArray(typeof(string));
            Array.Sort(currentProps);

            // Compare this to the list of known properties for this type.
            if (currentProps.Length == _knownPropertyNames.Length)
            {
                for (int i = 0; i < _knownPropertyNames.Length; ++i)
                {
                    if (currentProps[i] != _knownPropertyNames[i])
                    {
                        bResult = false;
                        break;
                    }
                }
            }
            else
                bResult = false;

            if (!bResult)
            {
                Console.WriteLine("");
                Console.WriteLine("ERROR: Type " + _type.ToString() + " has a different set of public read/writable properties than it did when this DRT was written.");
                Console.WriteLine("Known properties:");
                for (int i = 0; i < _knownPropertyNames.Length; ++i)
                    Console.WriteLine("    " + _knownPropertyNames[i]);
                Console.WriteLine("Current properties:");
                for (int i = 0; i < currentProps.Length; ++i)
                    Console.WriteLine("    " + currentProps[i]);
                Console.WriteLine("To fix this error, modify DRXSerialization.cs as follows:");
                Console.WriteLine("  1. Update this DRT to test any new properties by adding property assignments to method Set" + _type.Name + "Properties().");
                Console.WriteLine("  2. Modify the array _known" + _type.Name + "Properties to match the current set of properties.");
                Console.WriteLine("");
            }

            return bResult;
        }

        private DependencyProperty DependencyPropertyFromName(string propertyName, Type propertyType)
        {
            FieldInfo fi = propertyType.GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return (fi != null)?fi.GetValue(null) as DependencyProperty:null;
        }


        #endregion Private methods

        //------------------------------------------------------
        //
        //  Private fields
        //
        //------------------------------------------------------
        #region Private fields

        // The type of object to serialize.
        private readonly Type _type;

        // Array of the names of the public read/write properties that this
        // type was known to have when this DRT was last updated.
        private readonly string[] _knownPropertyNames;

        // Array of test cases for this type.
        private readonly SerializationTestCase[] _testCases;        

        #endregion Private fields
    }
}





