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
        DrtColorAPI.PrintLog(log);
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
/// Base class for ColorAPI' DRTs to set the owner.
/// </summary>
public abstract class ColorAPIDRTs : DrtBase
{
    public override string Owner { get { return "Microsoft"; } }
}

