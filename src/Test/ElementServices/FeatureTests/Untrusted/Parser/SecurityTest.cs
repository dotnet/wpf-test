// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test.Threading;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Xml;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Parser
{
    #region Class SecurityTest
    /// <summary>
    /// This class contains Parser threat tests.
    /// </summary>
    [TestDefaults]
    public class SecurityTest
    {
        /// <summary>
        /// This method represents a test that tries to create 
        /// a type which it is not supposed to be able to create, 
        /// because this test runs in Partial trust, and it doesn't have
        /// the needed permission.
        /// 
        /// This test tries to circumvent the permission checking, by 
        /// putting that particular type in a Xaml, and then trying
        /// to async load the Xaml. This is with the hope that maybe the multi-
        /// threading used in async loading will loosen the permission checks
        /// 
        /// Verification is done by catching the SecurityException
        /// </summary>
        /// 
        public void CreateTypeUsingAsync()
        {
            CoreLogger.BeginVariation();
            Stream xamlFileStream = File.OpenRead("SecurityTest1.xaml");

            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(SignalAsyncCompleted);
            _asyncCompleted = false;

            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));

            object root = xamlReader.LoadAsync(xamlFileStream, pc);

            // This will make us wait till the async tree building is done.
            _dispatcherFrame = new DispatcherFrame(false);
            Dispatcher.PushFrame(_dispatcherFrame);

            // Preliminary verification. Detailed verification is done in SignalAsyncCompleted()
            // routine.
            if (!_asyncCompleted)
            {
                throw new Microsoft.Test.TestValidationException("Async parsing wasn't given a chance to complete.");
            }
            CoreLogger.EndVariation();
        }

        private void SignalAsyncCompleted(Object sender, AsyncCompletedEventArgs e)
        {
            CoreLogger.LogStatus("XamlReader's LoadCompleted event fired.");
            _asyncCompleted = true;

            // We didn't cancel async loading.
            if (e.Cancelled)
            {
                throw new Microsoft.Test.TestValidationException("LoadCompleted event's args say async loading " +
                    "was cancelled, but it wasn't.");
            }

            // e.Error stores the exception, if one has occurred.
            if (e.Error == null)
            {
                // No exception thrown. Fail the test.
                throw new Microsoft.Test.TestValidationException("Security breach!! Could create an instance of System.IO.FileSystemWatcher, without having FileIOPermission");
            }
            else
            {
                if (!(e.Error is XamlParseException))
                {
                    throw e.Error;
                }
                else
                {
                    XamlParseException xpe = e.Error as XamlParseException;
                    if ((!(xpe.InnerException is System.Security.SecurityException)
                        && !(xpe.InnerException.InnerException is System.Security.SecurityException))
                        || !(xpe.Message.Contains("FileSystemWatcher")))
                    {
                        throw new Microsoft.Test.TestValidationException("Possible security breach!! May be we could create an instance of System.IO.FileSystemWatcher, without having FileIOPermission. "
                            + "XamlParseException was thrown as expected, but probably for other reasons than we expected. Exception is: " + xpe);
                    }
                }
            }
            _dispatcherFrame.Continue = false;
        }

        private bool _asyncCompleted;
        private DispatcherFrame _dispatcherFrame;
    }
    #endregion Class SecurityTest
}
