// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;

using System.Windows.Markup;

using Microsoft.Test.Serialization;
using Microsoft.Test;


namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A base class to handle serialization and its verification.
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// Revisions: Microsoft
    /// <para />
    /// FileName:  SerializationBaseCase.cs
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    public class SerializationBaseCase
    {
        /// <summary>
        /// Entry point for Testing via this class
        /// </summary>
        public void RunTestCase()
        {
            using (TestLog log = new TestLog(DriverState.TestName))
            {
                log.Result = TestResult.Pass;
                try
                {
                    InitializeCase();
                    DoTheTest("");
                }
                catch (Exception e)
                {
                    log.LogEvidence(e);
                    log.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// To initialize Logger, and to add _OnXamlSerilaized event handler.
        /// </summary>
        protected void InitializeCase()
        {
            _serhelper = new SerializationHelper();
            _serhelper.XamlSerialized += new XamlSerializedEventHandler(OnXamlSerialized);

            if (File.Exists(_tempXamlFile))
            {
                File.Delete(_tempXamlFile);
            }

            if (File.Exists(_tempXamlFile2))
            {
                File.Delete(_tempXamlFile2);
            }
        }


        /// <summary>
        /// Calls RoundTripTest() on SerializationHelper.
        /// </summary> 
        /// <param name="fileName">xaml file name</param>
        virtual protected void DoTheTest(String fileName)
        {
            _serhelper.RoundTripTestFile(fileName, XamlWriterMode.Expression, true);
        }

        /// <summary>
        /// Logs round trip status messages to CoreLogger.
        /// </summary>
        protected static void OnXamlSerialized(object sender, XamlSerializedEventArgs args)
        {
            string xamlFile = _tempXamlFile;

            // Save xaml to file for potential debugging.
            if (File.Exists(_tempXamlFile))
            {
                xamlFile = _tempXamlFile2;
            }

            IOHelper.SaveTextToFile(args.Xaml, xamlFile);
            GlobalLog.LogFile(xamlFile);
        }

        #region Variables
        /// <summary>
        /// Provides common serialization test functions.
        /// </summary>
        protected SerializationHelper _serhelper;

        /// <summary>
        /// File name for the xaml file serialized from the first tree.
        /// </summary>
        protected static string _tempXamlFile = "___SerializationTempFile.xaml";
        /// <summary>
        /// File name for the xaml file serialized from the second tree.
        /// </summary>
        protected static string _tempXamlFile2 = "___SerializationTempFile2.xaml";
        #endregion Variables
    }
}
