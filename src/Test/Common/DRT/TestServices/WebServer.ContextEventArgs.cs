// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Event arguments for WebServer events.

// </summary>



using System;
using System.Net;
using System.Net.Sockets;

namespace DRT
{
public partial class WebServer
{
/// <summary>
/// Event arguments for WebServer events.
/// </summary>
public class ContextEventArgs : EventArgs
{
    #region Constructors
    //--------------------------------------------------------------------------
    // Constructors
    //--------------------------------------------------------------------------

    public ContextEventArgs(HttpListenerContext context)
    {
        _context = context;
    }

    #endregion Constructors

    #region Public Properties
    //--------------------------------------------------------------------------
    // Public Properties
    //--------------------------------------------------------------------------

    public HttpListenerContext Context
    {
        get { return _context; }
    }

    public bool Handled
    {
        get { return _handled; }
        set { _handled = value; }
    }
    #endregion Public Properties

    #region Private Fields
    //--------------------------------------------------------------------------
    // Private Fields
    //--------------------------------------------------------------------------

    private bool _handled;
    private HttpListenerContext _context;

    #endregion Private Fields
}
}
}
