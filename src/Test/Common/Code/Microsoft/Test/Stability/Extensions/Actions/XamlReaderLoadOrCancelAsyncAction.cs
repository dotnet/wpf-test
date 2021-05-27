// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Stability.Extensions.Actions
{
    public class XamlReaderLoadOrCancelAsyncAction : SimpleDiscoverableAction
    {
        #region Public Members

        [InputAttribute(ContentInputSource.GetFromLogicalTree)]
        public DependencyObject Target { get; set; }

        public bool IsCanceled { get; set; }

        #endregion

        #region Override Members

        public override void Perform()
        {
            Stream stream = SerializeToStream();
            if (stream == null)
            {
                return;
            }

            XamlReader xamlReader = new XamlReader();
            xamlReader.LoadCompleted += new AsyncCompletedEventHandler(LoadCompleted);
            try
            {
                xamlReader.LoadAsync(stream);
            }
            catch (XamlParseException xpe)
            {
                bool isIgnorableMessage = xpe.Message.Contains("No matching constructor");

                //Fix 
                isIgnorableMessage = isIgnorableMessage | xpe.Message.Contains("CancelPrint");

                //Work around 
                isIgnorableMessage = isIgnorableMessage | xpe.Message.Contains("invalid character");

                // Ignoring XamlParseException with specific message, as Xaml cannot load types with no default constructor
                if (!isIgnorableMessage)
                {
                    // Context is lost for the current exception because of the catch
                    // Calling the method again so that debugger breaks at the throwing location
                    stream.Seek(0, SeekOrigin.Begin);
                    XamlReader.Load(stream);
                }
            }

            if (IsCanceled)
            {
                xamlReader.CancelAsync();
            }
        }

        #endregion

        #region Private Members

        private void LoadCompleted(object o, AsyncCompletedEventArgs args)
        {
            Trace.WriteLine("XamlReader load xaml completed.");
        }

        private Stream SerializeToStream()
        {
            //Save Target or window.Content to xamlString
            string xamlString;
            if (Target is Window)//Filter Window since it will open a new Window when load it.
            {
                FrameworkElement content = ((Window)Target).Content as FrameworkElement;
                if (content == null)
                {
                    return null;
                }

                xamlString = XamlWriter.Save(content);
            }
            else
            {
                xamlString = XamlWriter.Save(Target);
            }

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(xamlString);
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        #endregion
    }
}
