// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.IO;
using System.IO.Packaging;
using MS.Internal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.Test.Discovery;
using Microsoft.Test.Xaml.Async;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Async
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncTest
    {
        #region TestCancelAsync
        /// <summary>
        /// In this test, we start async parsing a Xaml, and then cancel it after
        /// some time. We verify that XamlReader's LoadCompleted event is fired with
        /// args.Cancelled=true.
        /// </summary>        
        public void TestCancelAsync()
        {
            // Stream the async Xaml file over a simulated file server.
            long testFileSize = (new FileInfo(_validXamlFile)).Length;

            SimulatedServer server = new SimulatedServer(_validXamlFile);
            server.UseRandomChunkSize(10 /*seed*/, (int)testFileSize / 4 /*maxChunkSize*/);
            server.UseRandomSleepTime(10 /*seed*/, 500 /*maxSleepTime in milliseconds*/);
            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();

            // Start async parsing the Xaml
            ParserContext pc = new ParserContext();
            pc.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(LoadCompletedHandler);
            object asyncRoot = xamlReader.LoadAsync(stream, pc);

            // Create a timer for the cancel time out.
            // When the time passes, the Tick handler will be called,
            // which allows us to cancel the async parsing.
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
            timer.Tick += new EventHandler(OnTimerDispatched);
            timer.Interval = TimeSpan.FromMilliseconds(_cancelTimeout);
            timer.Tag = xamlReader;
            timer.Start();

            _asyncCancelled = false;

            // Display the tree as it builds. This call drains the dispatcher queue
            // before returning.
            (new SerializationHelper()).DisplayTree(asyncRoot);
            // Async parsing has been cancelled. Now serialize the partially built tree 
            // to ensure that it is not in corrupt state.
            SerializationHelper.SerializeObjectTree(asyncRoot);
            
            // Verify that XamlReader's LoadCompleted was fired with
            // args.Cancelled = true.
            if (!_asyncCancelled)
            {
                throw new Microsoft.Test.TestValidationException("We cancelled async, but XamlReader's LoadCompleted event was not fired with args.Cancelled = true.");
            }
        }
        #endregion TestCancelAsync

        // Handler for XamlReader's LoadCompleted event.
        private void LoadCompletedHandler(Object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _asyncCancelled = true;
            }

            if (e.Error != null)
            {
                _asyncError = e.Error;
            }
        }

        // Handler for DispatcherTimer's Tick event.
        private void OnTimerDispatched(object sender, EventArgs args)
        {
            // Stop the timer.
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();

            // Cancel async parsing 
            XamlReader xamlReader = (XamlReader)timer.Tag;
            xamlReader.CancelAsync();
        }

        private bool _asyncCancelled = false;
        private Exception _asyncError = null;

        #region TestAsyncError
        /// <summary>
        /// In this test, we async load an invalid Xaml, and make sure that 
        /// XamlReader's LoadCompleted event is fired with args.Error set to the 
        /// exception.
        /// </summary>
        public void TestAsyncError()
        {
            // Stream the async Xaml file over a simulated file server.
            long testFileSize = (new FileInfo(_invalidXamlFile)).Length;

            SimulatedServer server = new SimulatedServer(_invalidXamlFile);
            server.UseFixedChunkSize((int)testFileSize / 10);
            server.UseFixedSleepTime(100 /*milliseconds*/);
            GlobalLog.LogStatus("Async loading the xaml file from a file server.");
            Stream stream = server.ServeFile();

            // Start async parsing the Xaml
            ParserContext pc = new ParserContext();
            pc.BaseUri = PackUriHelper.Create(new Uri("siteoforigin://"));
            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(LoadCompletedHandler);
            _asyncError = null;
            object asyncRoot = xamlReader.LoadAsync(stream, pc);

            // Display the tree as it builds. This call drains the dispatcher queue
            // before returning.
            (new SerializationHelper()).DisplayTree(asyncRoot);

            // Async parsing should be terminated due to an error. Now serialize the partially 
            // built tree to ensure that it is not in corrupt state.
            SerializationHelper.SerializeObjectTree(asyncRoot);

            // Make sure that XamlReader's LoadCompleted event was fired with args.Error != null.
            if (_asyncError == null)
            {
                throw new Microsoft.Test.TestValidationException("Aync parsing was expected to throw an error, but it didn't");
            }
            else
            {
                GlobalLog.LogStatus("Async parsing reported the following error: " + _asyncError.Message);
            }
        }
        #endregion TestAsyncError

        private const string _validXamlFile = "TestCancelAsync.xaml";
        private const string _invalidXamlFile = "TestAsyncError.xaml";
        private const double _cancelTimeout = 3000; //milliseconds
    }
}

