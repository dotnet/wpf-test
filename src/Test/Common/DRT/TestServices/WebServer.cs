// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>
// </summary>
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Cryptography;
using System.Threading;

namespace DRT
{
/// <summary>
/// A simple web server implemented using HttpListener.
/// </summary>
public partial class WebServer : IDisposable
{
    #region Constructors
    //--------------------------------------------------------------------------
    // Constructors
    //--------------------------------------------------------------------------

    /// <summary>
    /// Will create a web server on a random port.
    /// Use the BaseUri property to get connection information.
    /// </summary>
    /// <param name="possibleRequests">The relative Uri's this server will handle.</param>
    public WebServer(Uri[] possibleRequests)
        : this(possibleRequests, new Random().Next(_minRandomPort, _maxPort))
    {
    }

    /// <summary>
    /// Will create the web server on the specified port. If the port is in use
    /// a random port number will automatically be used. Callers should always
    /// verify the port with the BaseUri property.
    /// </summary>
    /// <param name="possibleRequests">The relative Uri's this server will handle.</param>
    /// <param name="port">The port to listen on.</param>
    public WebServer(Uri[] possibleRequests, int port)
    {
        if (possibleRequests == null)
        {
            throw new ArgumentNullException("possibleRequests");
        }

        foreach (Uri possible in possibleRequests)
        {
            if (possible.IsAbsoluteUri)
            {
                throw new ArgumentOutOfRangeException("possibleRequests", "Uris provided must be relative.");
            }
        }

        if ((port < 0) || (port > _maxPort))
        {
            throw new ArgumentOutOfRangeException("port", string.Format("Value must be between 0 and {0}", _maxPort));
        }

        _possibleRequests = possibleRequests;

        bool foundValidPort = false;
        while (!foundValidPort)
        {
            _baseUri = new Uri(string.Format("{0}://{1}:{2}/Test/", Uri.UriSchemeHttp, _serverIpAddress, port));

            _listener = new System.Net.HttpListener();
            _listener.Prefixes.Add(_baseUri.OriginalString);
            try
            {
                // triggers port usage and validation
                _listener.Start();
                _listener.Stop();
                foundValidPort = true;
            }
            catch (HttpListenerException hle)
            {
                if (hle.ErrorCode == 32) // Magic Number: PortInUse Error
                {
                    // generate random port
                    port = new Random().Next(_minRandomPort, _maxPort);
                }
                else
                {
                    throw hle;
                }
            }
        }
    }
    #endregion Constructors

    #region Public Methods
    //--------------------------------------------------------------------------
    // Public Methods
    //--------------------------------------------------------------------------

    /// <summary>
    /// Restores the security zone to the orginal value.
    /// </summary>
    /// <remarks>
    /// This is automatically called by dispose so calling this method is not
    /// required.
    /// </remarks>
    public static void RestoreSecurityZone()
    {
        if (_originalZone.HasValue)
        {
            RestoreSecurityZone(_originalZone.Value);
        }
    }

    /// <summary>
    /// Restores the security zone to the value specified.
    /// </summary>
    /// <remarks>
    /// To set the value for testing please use the SecurityZone property this
    /// is exposed only for DRT Clean Up.
    /// </remarks>
    /// <param name="zone">
    /// The original security zone for this server.
    /// </param>
    public static void RestoreSecurityZone(SecurityZone zone)
    {
        RegistryHelpers.SetSecurityZone(zone);
        _originalZone = new Nullable<SecurityZone>();
    }

    /// <summary>
    /// Stops the web server.
    /// </summary>
    public void Start()
    {
        _listener.Start();
        _worker = new Thread(new ThreadStart(this.Run));
        _worker.Start();
        TraceMessage(Guid.Empty, "Listening at {0}...", _baseUri);
    }

    /// <summary>
    /// Starts the web server.
    /// </summary>
    public void Stop()
    {
        TraceMessage(Guid.Empty, "Stopping...");
        _listener.Stop();
        _stop = true;
        _worker.Join();
        TraceMessage(Guid.Empty, "Stopped.");
    }

    #endregion Public Methods

    #region IDisposable Members
    //--------------------------------------------------------------------------
    // IDisposable Members
    //--------------------------------------------------------------------------

    /// <summary>
    /// Restores the security zone and stops the server if not done already.
    /// </summary>
    public void Dispose()
    {
        RestoreSecurityZone();

        if (!_stop)
        {
            Stop();
        }
    }

    #endregion

    #region Public Properties
    //--------------------------------------------------------------------------
    // Public Properties
    //--------------------------------------------------------------------------

    /// <summary>
    /// Sets the authentication schemes to use. 
    /// (Anonymous, Basic, Digest, IntegratedWindowsAuthentication, Negotiate,
    ///  None, Ntlm)
    /// </summary>
    public AuthenticationSchemes AuthenticationSchemes
    {
        get { return _listener.AuthenticationSchemes; }
        set { _listener.AuthenticationSchemes = value; }
    }

    /// <summary>
    /// The base Uri for the web server.
    /// </summary>
    /// <remarks>
    /// Usefull for when the port is randomly allocated.
    /// </remarks>
    public Uri BaseUri
    {
        get { return _baseUri; }
    }

    /// <summary>
    /// Sets the rate in bytes per second to throttle a request at.
    /// </summary>
    /// <remarks>
    /// Default is WebServer.UnthrottledRate.
    /// </remarks>
    public float ByteRate
    {
        get { return _byteRate; }
        set { _byteRate = value; }
    }

    /// <summary>
    /// Gets or set the current Internet Explorer security zone for this server.
    /// </summary>
    public static SecurityZone SecurityZone
    {
        get
        {
            return RegistryHelpers.GetSecurityZone();
        }

        set
        {
            // store the orginal zone for clean up
            if (!_originalZone.HasValue)
            {
                _originalZone = new Nullable<SecurityZone>(
                    RegistryHelpers.GetSecurityZone());
            }

            RegistryHelpers.SetSecurityZone(value);
        }
    }

    #endregion Public Properties

    #region Public Events
    //--------------------------------------------------------------------------
    // Public Events
    //--------------------------------------------------------------------------

    /// <summary>
    /// Occurs when a request is made to the web server.
    /// </summary>
    public event EventHandler<ContextEventArgs> OnRequest;
    #endregion Public Events

    #region Private Methods
    //--------------------------------------------------------------------------
    // Private Methods
    //--------------------------------------------------------------------------

    /// <summary>
    /// Implementation that runs the actual web server.
    /// </summary>
    /// <remarks>
    /// This code handles dispatching the incomming requests to worker threads
    /// and will run until in a loop until the server is stopped.
    /// </remarks>
    private void Run()
    {
        HttpListenerContext context = null;

        while (!_stop)
        {
            try
            {
                // waits indefinately for next HttpRequest
                context = _listener.GetContext();
                ContextEventArgs args = new ContextEventArgs(context);
                if ((OnRequest != null) && IsKnown(context.Request.Url))
                {
                    OnRequest(this, args);
                }
                if (!args.Handled)
                {
                    ThreadPool.QueueUserWorkItem(
                        new WaitCallback(DefaultRequestWorker), args);
                }
            }
            catch (HttpListenerException hle)
            {
                TraceMessage(
                    context == null ? Guid.Empty : context.Request.RequestTraceIdentifier,
                    hle.Message);
            }
        }
    }

    /// <summary>
    /// Check to see if we are to handle this Uri.
    /// </summary>
    /// <param name="uri">Uri in question.</param>
    /// <returns>True if the Uri is in the list of Uris we are to handle.</returns>
    private bool IsKnown(Uri uri)
    {
        // ensure we are only checking requests for us
        if (!_baseUri.IsBaseOf(uri))
        {
            return false;
        }

        bool matchFound = false;
        Uri relative = _baseUri.MakeRelativeUri(uri);

        foreach (Uri possible in _possibleRequests)
        {
            if (string.Equals(
                GetPathFromRelativeUri(relative),
                GetPathFromRelativeUri(possible),
                StringComparison.OrdinalIgnoreCase))
            {
                matchFound = true;
                break;
            }

            // path is folder
            if (Directory.Exists(Path.GetFullPath(possible.OriginalString)))
            {
                // is a subfolder of
                if (Path.GetFullPath(relative.OriginalString).StartsWith(
                    Path.GetFullPath(possible.OriginalString),
                    StringComparison.OrdinalIgnoreCase))
                {
                    matchFound = true;
                    break;
                }
            }
        }

        return matchFound;
    }

    /// <summary>
    /// If no listener for the OnRequest event has handled the request
    /// then this one will be called.  It will stream files.
    /// </summary>
    /// <param name="state">A WebServer.ContextEventArgs.</param>
    private void DefaultRequestWorker(object state)
    {
        HttpListenerContext context = ((WebServer.ContextEventArgs)state).Context;
        Guid rid = context.Request.RequestTraceIdentifier;

        string file = GetPathFromRelativeUri(
            _baseUri.MakeRelativeUri(context.Request.Url));

        if (IsKnown(context.Request.Url) && File.Exists(file))
        {
            TraceMessage(
                rid,
                "Processing {0} {1}",
                context.Request.HttpMethod,
                context.Request.Url);

            TraceHeaders(rid, context.Request.Headers);

            SendFile(rid, context, file);
        }
        else
        {
            SendNotFound(rid, context);
        }
    }

    /// <summary>
    /// Will send 404 to the client.
    /// </summary>
    private void SendNotFound(Guid requestId, HttpListenerContext context)
    {
        TraceMessage(
            requestId,
            "Failing request for {0} because it is {1}.",
            context.Request.Url,
            !IsKnown(context.Request.Url) ? "unknown" : "not found");

        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.Close();
    }

    /// <summary>
    /// Will stream a file to the client.
    /// </summary>
    private void SendFile(Guid requestId, HttpListenerContext context, string file)
    {
        HttpListenerResponse response = context.Response;
        response.SendChunked = true;
        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentType = RegistryHelpers.GetContentType(file);

        using (Stream local = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            // 



            response.ContentLength64 = local.Length;
            Stream client = response.OutputStream;
            byte[] data = new byte[_chunkSize];
            int bytesRead = 0;
            DateTime chunkStart;
            DateTime start = DateTime.Now;

            while (local.Position < local.Length)
            {
                chunkStart = DateTime.Now;
                bytesRead = local.Read(data, 0, _chunkSize);
                try
                {
                    client.Write(data, 0, bytesRead);
                    client.Flush();
                    // balance of time to wait
                    ThrottleByteRate(chunkStart);
                }
                catch (HttpListenerException hle)
                {
                    TraceMessage(requestId,
                        "Error sending chunk {0}-{1}/{2}.  {3}",
                        local.Position - bytesRead,
                        local.Position,
                        local.Length,
                        hle.Message);
                    return; // will ab end
                }
            }

            response.Close();
            double elapsed = DateTime.Now.Subtract(start).TotalMilliseconds;

            TraceMessage(
                requestId,
                "Request of {1} bytes completed in {0}ms at {2} Kbps.",
                elapsed,
                local.Length,
                (float)local.Length * 8 / 1024 // bytes * (bits per byte) / Kilo
                    / (elapsed / 1000)); // divided by milliseconds / (milliseconds per second)
        }
    }

    /// <summary>
    /// Will throttle the byte rate based start time for sending a chunck.
    /// </summary>
    private void ThrottleByteRate(DateTime startedChunk)
    {
        // we perform this check first to minimize our time here
        // on unthrottled connections thus maximizing throughput
        if (_byteRate != UnthrottledRate)
        {
            // time elapsed since beginning of chunk
            int elapsed = (int)DateTime.Now.Subtract(startedChunk).TotalMilliseconds;
            // time required to send a chunk
            int timeWindow = (int)(((float)(_chunkSize / _byteRate)) * 1000);
            // balance of time to wait
            timeWindow -= elapsed;
            if (timeWindow > 0) { Thread.Sleep(timeWindow); }
        }
    }

    /// <summary>
    /// Gets the local file path from a relative Uri.
    /// </summary>
    /// <param name="uri">A relative Uri.</param>
    /// <returns>Local file path.</returns>
    private static string GetPathFromRelativeUri(Uri uri)
    {
        return uri.ToString().Split("?".ToCharArray())[0];
    }

    #region Trace Methods
    /// <summary>
    /// Writes the headers to our trace handler.
    /// </summary>
    private static void TraceHeaders(Guid requestId, NameValueCollection headers)
    {
        foreach (string key in headers.AllKeys)
        {
                TraceMessage(requestId, "{0}: {1}", key, headers[key]);
        }
    }
    
    /// <summary>
    /// Write a message to our trace handler.
    /// </summary>
    private static void TraceMessage(Guid requestId, string format, params object[] args)
    {
        TestServices.Trace("WebServer: {0}: {1}", requestId, string.Format(format, args));
    }
    #endregion Trace Methods
    #endregion Private Methods

    #region Public Fields
    //--------------------------------------------------------------------------
    // Public Fields
    //--------------------------------------------------------------------------
    public const int UnthrottledRate = int.MaxValue;
    #endregion Public Fields

    #region Private Fields
    //--------------------------------------------------------------------------
    // Private Fields
    //--------------------------------------------------------------------------
    private Uri _baseUri;
    private bool _stop;
    private HttpListener _listener;
    private Thread _worker;
    private float _byteRate = UnthrottledRate;
    private Uri[] _possibleRequests;

    /// <remarks>
    /// Using the loopback address of 127.0.0.1 will not trigger firewall
    /// rules; if another value is used and exception will need to be made
    /// for the port
    /// </remarks>
    private const string _serverIpAddress = "127.0.0.1";
    private const int _minRandomPort = 0x1000;
    private const int _maxPort = 0x2000;
    private const int _chunkSize = 1024;
    private static Nullable<SecurityZone> _originalZone = new Nullable<SecurityZone>();
    #endregion Private Fields
}
}
