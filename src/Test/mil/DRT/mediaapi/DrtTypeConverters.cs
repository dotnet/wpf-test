// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains a set of DRTs designed to test our TypeConverters.
//
// Notes: Includes a function that will use reflection to grab all of
//        the TypeConverter classes.  However, this isn't currently
//        being used.  Instead it tests a selection of typeconverters.
//        The primary difficulty with testing all TypeConverters is
//        that we need an instance of an object that can be Converted
//        "From" and that is not easily obtainable in all cases.
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using MS.Internal;

/// <summary>
/// Test for primitive opertations
/// </summary>
public class DrtTypeConverters : DrtBase
{
    /// <summary>
    /// Run DRT.
    /// </summary>
    public override bool Run(out string results)
    {
        bool succeeded = true;
        results = "";

        object nn = new Point(0,0);
        
        _converters = new List<TypeConverter>();

        // Run our tests on .net Int32Converter which is assumed to be valid.
        _converters.Add( new Int32Converter() );
        AddSomeTypeConverters( _converters );

        // Get an object for each type convert to convert "from".
        
        InitializeConvertObjectTable();
        
        string badargs;

        // Note that because of the order of the operands to &&, all tests will run
        // even if one failed.  This is intentional, and ensures that all regressions 
        // are noted, not just the initial failure.

        succeeded = TestNullArguments(out badargs) && succeeded;
        if (!succeeded)
        {
            results = badargs;
        }
        else
        {
            results = "SUCCEEDED";
        }

        return succeeded;
    }

    public void AddAllTypeConverters( List<TypeConverter> list )
    {
        List<Assembly> assemblies = new List<Assembly>();
        assemblies.Add(Assembly.GetAssembly(typeof(System.Windows.Media.Media3D.Point3D))); // Core

        foreach (Assembly assembly in assemblies)
        {
            Module[] ms = assembly.GetModules();
            foreach (Module m in ms)
            {
                Type[] ts = m.GetTypes();
                foreach (Type t in ts)
                {
                    if (t.IsSubclassOf(typeof(TypeConverter)))
                    {
                        if (!t.IsAbstract && t.IsPublic)
                        {
                            TypeConverter tc = (TypeConverter) Activator.CreateInstance(t);
                            list.Add( tc );
                        }
                    }
                }
            }
        }
    }

    public void AddSomeTypeConverters( List<TypeConverter> list )
    {
        list.Add( new ColorConverter() );
        list.Add( new PointConverter() );
        list.Add( new RectConverter() );
        list.Add( new SizeConverter() );
        list.Add( new VectorConverter() );
        list.Add( new Point3DConverter() );
        list.Add( new Int32Converter() );
        list.Add( new TextDecorationCollectionConverter() );
        list.Add( new KeySplineConverter() );
        list.Add( new PointCollectionConverter() );
        list.Add( new Point3DCollectionConverter() );
        list.Add( new VectorCollectionConverter() );
        list.Add( new Int32CollectionConverter() );
        list.Add( new Vector3DCollectionConverter() );
        list.Add( new Point4DConverter() );
        list.Add( new QuaternionConverter() );
        list.Add( new Rect3DConverter() );
        list.Add( new Size3DConverter() );
        list.Add( new Vector3DConverter() );
        list.Add( new GeometryConverter() );
        list.Add( new DoubleCollectionConverter() );
        list.Add( new TransformConverter() );
    }

    public override string Owner { get { return "DanWo"; } }

    private Dictionary<Type,object> _convertersObjects;
    private List<TypeConverter> _converters;

    private bool InitializeConvertObjectTable()
    {
        _convertersObjects = new Dictionary<Type,object>();
        
        // This function initializes the table _convertersObjects so
        // that for each type converter type there is an object that
        // the type converter should be able to convert.  The "easy"
        // types are types that have a default constructor and whose
        // type converter can be grabbed using
        // TypeDescriptor.GetConverter()
        Type[] easyTypes = new Type[]{
            typeof(Color), typeof(Point), typeof(Point3D), typeof(Int32), typeof(TextDecorationCollection),
            typeof(Key), typeof(MouseAction), typeof(KeySpline),
            typeof(PointCollection), typeof(Point3DCollection), typeof(VectorCollection), typeof(Int32Collection), typeof(Vector3DCollection),
            typeof(Point4D), typeof(Quaternion), typeof(Rect3D), typeof(Size3D), typeof(Vector3D), typeof(PathGeometry), typeof(SolidColorBrush),
            typeof(DoubleCollection), typeof(PathFigure), typeof(PixelFormat),
            typeof(Rect), typeof(Vector), typeof(Size)
        };

        foreach (Type type in easyTypes)
        {
            try
            {
                object instance = Activator.CreateInstance(type);
                TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                
                _convertersObjects[typeConverter.GetType()] = instance;
            }
            catch( MissingMethodException )
            {
                Console.WriteLine( "Couldn't instantiate " + type + " using CreateInstance!" );
                return false;
            }
        }

        // Special cases, mostly objects without parameterless constructors.
        _convertersObjects[typeof(KeyGestureConverter)] = new KeyGesture(Key.F1);
        _convertersObjects[typeof(CursorConverter)] = Cursors.None;
        _convertersObjects[typeof(TransformConverter)] = new TranslateTransform();
        _convertersObjects[typeof(BrushConverter)] = new SolidColorBrush();
        //_convertersObjects[typeof(BitmapSourceConverter)] = new BitmapImage(100,100,100.0,100.0,PixelFormats.Default);
        
        return true;
    }

    // Test various combinations of null arguments and ensure that 
    
    private bool TestNullArguments(out string results)
    {
        results = "";
        bool succeeded = true;

        string testcase = "";
        
        foreach (TypeConverter converter in _converters)
        {
            object instance = null;
            try
            {
                instance = _convertersObjects[converter.GetType()];
            }
            catch( KeyNotFoundException )
            {
                results += "Object not found for type converter " + converter.GetType().ToString() + "!  Update _convertersObjects table.\n";
                succeeded = false;
            }
            
            bool converterGood = true;
            testcase = "null1"; // ConvertFrom with a null input object should return NotSupportedException
            try
            {
                converter.ConvertFrom( null, System.Globalization.CultureInfo.InvariantCulture, null );
            }
            catch( Exception e )
            {
                if (e.GetType() != typeof(NotSupportedException))
                {
                    if (converterGood)
                    {
                        results += converter.ToString() + " has failures.\n";
                        converterGood = false;
                    }
                    results += "  Failed case: " + testcase + ". Returned " + e.GetType() + "\n";
                    succeeded = false;
                }
            }

            testcase = "null2"; // ConvertTo with a null output type should throw ArgumentNullException
            try
            {
                converter.ConvertTo( null, System.Globalization.CultureInfo.InvariantCulture, instance, null );
            }
            catch( Exception e )
            {
                if (e.GetType() != typeof(ArgumentNullException))
                {
                    if (converterGood)
                    {
                        results += converter.ToString() + " has failures.\n";
                        converterGood = false;
                    }
                    results += "  Failed case: " + testcase + ". Returned " + e.GetType() + "\n";
                    succeeded = false;
                }
            }

            testcase = "null3"; // ConvertTo with a null input value should throw NotSupportedException
            try
            {
                converter.ConvertTo( null, System.Globalization.CultureInfo.InvariantCulture, null, typeof(string) );
            }
            catch( Exception e )
            {
                if (e.GetType() != typeof(NotSupportedException))
                {
                    if (converterGood)
                    {
                        results += converter.ToString() + " has failures.\n";
                        converterGood = false;
                    }
                    results += "  Failed case: " + testcase + ". Returned " + e.GetType() + "\n";
                    succeeded = false;
                }
            }

        }
        
        return succeeded;
    }
}


