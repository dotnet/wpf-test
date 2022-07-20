// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains the base class for all MediaAPI drts.
//
//

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows;
using System.Threading;

using System.Windows.Media;
using System.Runtime.InteropServices;

/// <summary>
/// Base class for media DRTs.
/// </summary>
public abstract class DrtBase: IDisposable
{
    /// <summary>
    /// This is automatically called to run the DRT. Override this function and
    /// implement the test in it.
    /// Return false if the function failed and give a reasonable error message in the out argument.
    /// </summary>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    /// <param name="string"> The text to display if the test fails. </param>
    public abstract bool Run(out string results);

    /// <summary>
    /// Return your email alias with this property.
    /// </summary>
    public abstract string Owner { get; }

    /// <summary>
    /// Outputs a log.
    /// </summary>
    protected void Log(string log)
    {
        DrtMediaAPI.PrintLog(log);
    }

    /// <summary>
    /// Clean up.
    /// </summary>
    public void Dispose()
    {
    }

    // Add this back if/when it becomes necessary
#if NEVER
    /// <summary>
    /// Property to access the visual manager. This property is automatically initalized before
    /// Run is called.
    /// </summary>
    public VisualManager VisualManager
    {
        get
        {
            return m_visualManager;
        }
        set
        {
            m_visualManager = value;
        }
    }

    /// <summary>
    /// Accessor to the root visual.
    /// </summary>
    public Visual RootVisual
    {
        get
        {
            return m_visualManager.RootVisual;
        }
        set            
        {
            m_visualManager.RootVisual = value;
        }
    }

    /// <summary>
    /// Clean up the visual manager. Dispose MUST be called from the DRT runner.
    /// </summary>
    public void Dispose()
    {
        RootVisual = null;
    }
    
    private VisualManager m_visualManager;
#endif
}

/// <summary>
/// Base class for PrimitiveDRTs.
/// </summary>
public abstract class DrtPrimitiveBase : DrtBase
{
    /// <summary>
    /// This is Adam (who thinks that he is also "me")
    /// </summary>
    public override string Owner { get { return "Microsoft"; } }

    /// <summary>
    /// Helper method that given a type and the name of the empty property
    /// will verify the conversion to/from the empty instance, string,
    /// and instance descriptor.
    /// 
    /// The bool result indicates success (true) or failure (false).  In
    /// the case of failure, 
    /// </summary>
    protected bool EmptyTestHelper(Type t, string emptyName, ref string results)
    {
        bool success = true;

        PropertyInfo emptyProperty = t.GetProperty(emptyName);
        object emptyInstance = emptyProperty.GetValue(/* target = */ null, BindingFlags.Default | BindingFlags.Static, /* Binder = */ null, /* index = */ null, /* cultureInfo = */ null);

        // Verify that the ToString() method returns the emptyName
        string emptyToString = emptyInstance.ToString();
        if (emptyToString != emptyName)
        {
            results += String.Format("FAILED: {0}.ToString() return '{1}' instead of '{2}'",
                t.Name, emptyToString, emptyName);
            success = false;
        }

        TypeConverter converter = TypeDescriptor.GetConverter(emptyInstance);

        // Verify that converting from emptyName yields an instance whose
        // IsEmpty returns true.  Our tokenizer should strip extraneous
        // whitespace.  To test this, we add some.
        object o = converter.ConvertFrom(" " + emptyName + " \t ");
        if (!GetIsEmpty(o, emptyName))
        {
            results += String.Format("FAILED: {0} converter did not convert '{1}' into {0}.{2}",
                t.Name, emptyToString, emptyName);
            success = false;
        }

        return success;
    }

    // Invokes the IsEmpty property of the given object and returns
    // the result using late binding.
    private bool GetIsEmpty(object o, string emptyName)
    {
        PropertyInfo isEmptyProperty = o.GetType().GetProperty("Is" + emptyName);
        return (bool)isEmptyProperty.GetValue(o, /* index = */ null);
    }
}
