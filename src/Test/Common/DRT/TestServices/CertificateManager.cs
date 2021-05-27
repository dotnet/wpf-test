// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <summary>

//  Class which manages certificate installation and removal for TestServices.

// </summary>



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace DRT
{
    /// <summary>
    /// Manages certificate installation and removal for TestServices.
    /// </summary>
    public class CertificateManager : IDisposable
    {
        #region Constructor
        //----------------------------------------------------------------------
        // Constructor
        //----------------------------------------------------------------------

        /// <summary>
        /// Will create a self signed certificate with the supplied name
        /// and place it in the current user's personal store.
        /// </summary>
        /// <param name="name">Subject name for the certificate.</param>
        public void Create(string name)
        {
            EnsureRootAuthority();
            CreateCertificate(name, BuildUserArguments(name));
        }
        #endregion Constructor

        #region IDisposable Members
        //----------------------------------------------------------------------
        // IDisposable Members
        //----------------------------------------------------------------------

        /// <summary>
        /// Will release unmanged resources and remove certificates.
        /// </summary>
        public void Dispose()
        {
            RemoveCertificates();

            if (_procs != null)
            {
                _procs.Dispose();
            }
        }
        #endregion IDisposable Members

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Creates the Root Authority certificate if needed.
        /// </summary>
        private void EnsureRootAuthority()
        {
            if (!_isRootInstalled)
            {
                CreateCertificate(_rootCert, BuildRootArguements(_rootCert));
                _isRootInstalled = true;
            }
        }

        /// <summary>
        /// Builds the command line arguments for Root Authority certificates.
        /// </summary>
        /// <param name="name">Name of the Root Authority.</param>
        /// <returns>Command line arguments.</returns>
        private CertificateArguments BuildRootArguements(string name)
        {
            return new CertificateArguments(
                string.Format(
                    "-n \"CN={0}\" -ss \"Root\" -a sha1 -len 2048 -r -cy authority -sr \"LocalMachine\" -sk \"D8C0990E-1B94-43cc-B37B-1C0FFFB5E3B4\"",
                    name),
                string.Format(
                    "-del -c -n \"{0}\" -r localMachine -s root",
                    name));
        }

        /// <summary>
        /// Builds the command line arguments for user certificates.
        /// </summary>
        /// <param name="name">Name of the user.</param>
        /// <returns>Command line arguments format string.</returns>
        private CertificateArguments BuildUserArguments(string name)
        {
            return new CertificateArguments(
                string.Format(
                    "-n \"CN={0}\" -a sha1 -cy end -is \"Root\" -in \"{1}\" -ir \"LocalMachine\" -ss \"My\" -sr \"CurrentUser\" -sk \"D8C0990E-1B94-43cc-B37B-1C0FFFB5E3B4\"",
                    name,
                    _rootCert),
                string.Format(
                    "-del -c -n \"{0}\" -s my",
                    name));
        }

        /// <summary>
        /// Will create a certificate using the arguments provied.
        /// </summary>
        /// <remarks>
        /// Uses the makecert.exe command.
        /// </remarks>
        /// <param name="name">Certificate name.</param>
        /// <returns>Command line arguments format string.</returns>
        private void CreateCertificate(
            string name, CertificateArguments arguments)
        {
            Process p = _procs.Create(
                "makecert.exe", arguments.Add, false, true);
            p.WaitForExit();
            if (p.ExitCode != 0)
            {
                TestServices.Log("Unable to create certificate {0}.", name);
            }
            else
            {
                _certs.Add(name, arguments);
            }
        }

        /// <summary>
        /// Will remove all certificates created.
        /// </summary>
        private void RemoveCertificates()
        {
            string[] names = new string[_certs.Keys.Count];
            _certs.Keys.CopyTo(names, 0);

            foreach (string name in names)
            {
                RemoveCertificate(name);
            }
        }

        /// <summary>
        /// Removes the specified certificate.
        /// </summary>
        /// <remarks>
        /// Uses the certmgr.exe command.
        /// </remarks>
        /// <param name="name">Name of certificate.</param>
        private void RemoveCertificate(string name)
        {
            ProcessStartInfo info = new ProcessStartInfo(
                "cmd.exe",
                string.Format(
                    CultureInfo.InvariantCulture, 
                    "/c echo 1 | certmgr.exe {0} >> certCleanup.log", 
                    _certs[name].Remove));

            info.WindowStyle = ProcessWindowStyle.Minimized;

            bool moreToDelete = true;
            while (moreToDelete)
            {
                Process p = _procs.Create(info, false);
                moreToDelete = p.WaitForExit(1000) && (p.ExitCode == 0);
            }

            _certs.Remove(name);
        }
        #endregion Private Methods

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        private bool _isRootInstalled = false;
        private string _rootCert = "DrxDigSig_Root";
        private ProcessManager _procs = new ProcessManager();
        private Dictionary<string, CertificateArguments> _certs =
            new Dictionary<string, CertificateArguments>();
        #endregion Private Fields

        /// <summary>
        /// A structure that pairs the add and remove arguements for a specific
        /// certificate.
        /// </summary>
        private struct CertificateArguments
        {
            #region Constructors
            //------------------------------------------------------------------
            // Constructor
            //------------------------------------------------------------------

            /// <summary>
            /// Will intialized the structure.
            /// </summary>
            /// <param name="add">The add certificate arguements.</param>
            /// <param name="remove">The remove certificate arguements.</param>
            public CertificateArguments(string add, string remove)
            {
                Add = add;
                Remove = remove;
            }
            #endregion Constructors

            #region Public Fields
            //------------------------------------------------------------------
            // Public Fields
            //------------------------------------------------------------------

            /// <summary>
            /// The add certificate arguements.
            /// </summary>
            public string Add;
            /// <summary>
            /// The remove certificate arguements.
            /// </summary>
            public string Remove;
            #endregion Public Fields
        }
    }
}
