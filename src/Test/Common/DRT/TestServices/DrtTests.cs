// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Tests for TestServices.

// </summary>


using System;
using System.Collections;
using System.Security;
using System.Windows.Automation;

namespace DRT
{
/// <summary>
/// Test to verify WebServer.RegistryHelpers
/// </summary>
[TestAttribute("Verify WebServer.RegistryHelpers", "Microsoft")]
public class WebServerRegistryHelpersTest : MarshalByRefObject
{
    #region Public Methods
    //--------------------------------------------------------------------------
    // Public Methods
    //--------------------------------------------------------------------------

    /// <summary>
    /// Validates the funtion and fidelity of the SecurityZone methods.
    /// </summary>
    [TestStep("Validate Security Zone Methods", Async=true)]
    public void ValidateSecurityZoneMethods()
    {
        SecurityZone orginal = WebServer.SecurityZone;

        foreach (string current in Enum.GetNames(typeof(SecurityZone)))
        {
            SecurityZone zone = (SecurityZone)Enum.Parse(
                typeof(SecurityZone), current);

            WebServer.SecurityZone = zone;

            TestServices.Assert(
                WebServer.SecurityZone == zone,
                "Get after set for SecurityZone does not match: was {0}, expected {1}.",
                WebServer.SecurityZone,
                zone);
        }

        WebServer.RestoreSecurityZone();

        TestServices.Assert(
            WebServer.SecurityZone == orginal,
            "Restored zone {0} does not match orginal {1}.",
            WebServer.SecurityZone,
            orginal);

        SecurityZone forcedRestore = SecurityZone.MyComputer;

        WebServer.RestoreSecurityZone(forcedRestore);
        TestServices.Assert(
            WebServer.SecurityZone == forcedRestore,
            "Restore value {0} was not set.",
            forcedRestore);

        WebServer.RestoreSecurityZone(orginal);
    }

    /// <summary>
    /// Validates the funtion of GetContentTypes.
    /// </summary>
    [TestStep("Validate Get Content Types", Async = true)]
    public void ValidateGetContentTypes()
    {
        Type registryHelpers = ReflectionHelper.GetType(
            "DRT.WebServer+RegistryHelpers", this.GetType().Assembly);

        string contentType = (string)ReflectionHelper.InvokeMethod(
            registryHelpers,
            "GetContentType",
            new object[] { "something.txt" });
        TestServices.Assert(
            string.Equals("text/plain", contentType, StringComparison.OrdinalIgnoreCase),
            "Well known content type did not match.");

        contentType = (string)ReflectionHelper.InvokeMethod(
            registryHelpers,
            "GetContentType",
            new object[] { "something.unknown" });
        TestServices.Assert(
            string.Equals("application/octet-stream", contentType, StringComparison.OrdinalIgnoreCase),
            "Well unknown content type was not default.");
    }

    #endregion Public Methods
}

/// <summary>
/// Test to verify WebServer.Port In Use Scenario
/// </summary>
[TestAttribute("Verify WebServer.when Port In Use", "Microsoft")]
public class WebServerPortInUseTest : MarshalByRefObject
{
    #region Public Methods
    //--------------------------------------------------------------------------
    // Public Methods
    //--------------------------------------------------------------------------

    /// <summary>
    /// Validates the funtion and fidelity of the SecurityZone methods.
    /// </summary>
    [TestStep("Validate Port In Use")]
    public void ValidatePortInUse()
    {
        using (WebServer ws = new WebServer(new Uri[0], 135))
        {
            ws.Start();
            ws.Stop();

            TestServices.Assert(
                ws.BaseUri.Port != 135,
                "A random port number was not assigned.");
        }
    }

    /// <summary>
    /// Validates the funtion of GetContentTypes.
    /// </summary>
    [TestStep("Validate Get Content Types", Async = true)]
    public void ValidateGetContentTypes()
    {
        Type registryHelpers = ReflectionHelper.GetType(
            "DRT.WebServer+RegistryHelpers", this.GetType().Assembly);

        string contentType = (string)ReflectionHelper.InvokeMethod(
            registryHelpers,
            "GetContentType",
            new object[] { "something.txt" });
        TestServices.Assert(
            string.Equals("text/plain", contentType, StringComparison.OrdinalIgnoreCase),
            "Well known content type did not match.");

        contentType = (string)ReflectionHelper.InvokeMethod(
            registryHelpers,
            "GetContentType",
            new object[] { "something.unknown" });
        TestServices.Assert(
            string.Equals("application/octet-stream", contentType, StringComparison.OrdinalIgnoreCase),
            "Well unknown content type was not default.");
    }

    #endregion Public Methods
}
}
