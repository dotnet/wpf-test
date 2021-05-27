// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.VisualVerification;

namespace Microsoft.Test.Security.Wrappers
{
    /// <summary>Security Wrapper for Type: Snapshot</summary>
    [System.CLSCompliant(false)]
    public static class SnapshotSW : System.Object
    {
        /// <summary/>
        public static Snapshot FromFile(string filePath)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            Snapshot snap = Snapshot.FromFile(filePath);
            return snap;
        }

        /// <summary/>
        public static Snapshot FromRectangle(System.Drawing.Rectangle rectangle)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            Snapshot snap = Snapshot.FromRectangle(rectangle);
            return snap;
        }

        /// <summary/>
        public static void ToFile(Snapshot snap, string filePath, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            snap.ToFile(filePath, imageFormat);
        }

        /// <summary/>
        public static Snapshot CompareTo(Snapshot a, Snapshot b)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return a.CompareTo(b);
        }


    }

    /// <summary>Security Wrapper for Type: Histogram</summary>
    [System.CLSCompliant(false)]
    public static class HistogramSW : System.Object
    {
        /// <summary/>
        public static Histogram FromFile(string filePath)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            Histogram hist = Histogram.FromFile(filePath);
            return hist;
        }


    }

    /// <summary>Security Wrapper for Type: SnapshotHistogramVerifier</summary>
    [System.CLSCompliant(false)]
    public class SnapshotHistogramVerifierSW : System.Object
    {
        /// <summary/>
        public static SnapshotHistogramVerifier SnapshotHistogramVerifier(Histogram tolerance)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return new SnapshotHistogramVerifier(tolerance);
        }

        /// <summary/>
        public static VerificationResult Verify(SnapshotHistogramVerifier SHV, Snapshot diff)
        {
            new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return SHV.Verify(diff);
        }

    }


}
