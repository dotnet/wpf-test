// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Resources;
using System.Resources;
using System.Threading;

namespace DRT
{
    // NOTE: This test can't run in the SEE with default permission as it invokes the appropriate methods using reflection.

    class NewLoaderSuite : DrtTestSuite
    {
        Assembly _assembly          = null;
        Type _mimeObjectFactoryType = null;
        Type _contentTypeType       = null;
        bool _calledBack            = false;
        ManualResetEvent _callbackWaiter = new ManualResetEvent(true);

        void Initialize()
        {
            // get application to perform initialization
            object throwAway = System.Windows.Application.Current;

            _assembly = Assembly.GetAssembly(typeof(NavigationWindow)); // to get PresentationFramework.dll
            _mimeObjectFactoryType = _assembly.GetType("MS.Internal.AppModel.MimeObjectFactory");
        }

        public NewLoaderSuite() : base("NewLoader")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";
            Initialize();
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]
                    {
                        new DrtTest(TestLoadFromDefault),
                        new DrtTest(TestLoadWithAlternateName),
                        new DrtTest(TestNoManagerGiven),
                        new DrtTest(TestFileFallback),
                        new DrtTest(TestNoFileExists),
                        new DrtTest(TestMimeObjectFactorySync),
                        new DrtTest(TestMimeObjectFactoryLoadXaml),
                    };
        }

        #region helper methods        

        private object Call(Type type, object instance, string method, params object[] args)
        {
            // array of types is used to disambiguate overloaded calls.  I'm not really sure why
            // the framework doesn't do this part for us.  Maybe perf reasons.
            Type[] types = new Type[args.Length];
            for (int x = 0; x < args.Length; x++)
            {
                    types[x] = args[x].GetType();
            }

            MethodInfo m = type.GetMethod(method, types);
            if(m == null)
                m = type.GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
            if (m == null)
                m = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);

            try
            {
                return m.Invoke(instance, args);
            }
            catch (Exception e) // stop exception filter from hiding my exceptions
            {
                if (e.InnerException != null)
                    throw e.InnerException;
                else
                    throw;
            }
        }

        private void CallBack(IAsyncResult iar)
        {
            _calledBack = true;
            _callbackWaiter.Set();
        }

        private object CreateContentType(string mimeType)     
        {
            if (_contentTypeType == null)
            {
                Assembly windowsBase = typeof(DependencyObject).Assembly;
                _contentTypeType = windowsBase.GetType("MS.Internal.ContentType");
            }

            return Activator.CreateInstance(_contentTypeType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { mimeType }, null);
        }

        #endregion helper methods

        void TestLoadFromDefault()
        {
            // attempt to load resource.baml from the default loader should throw IOException
            bool exceptionThrown = false;
            try
            {
                StreamResourceInfo part = Application.GetResourceStream(new Uri("/resource.baml", UriKind.RelativeOrAbsolute));
            }
            catch (IOException)
            {
                exceptionThrown = true;
            }
            DRT.Assert(exceptionThrown == true, "Failed to throw for .baml.");
        }

        void TestLoadWithAlternateName()
        {
            // attempt to load resource.baml from the default loader
            StreamResourceInfo part = Application.GetResourceStream(new Uri("/resource.xaml", UriKind.RelativeOrAbsolute));
            DRT.Assert(part != null, "Failed to return default part.");
            Stream s = part.Stream;
            DRT.Assert(s != null, "Failed to load from the default resource manager using the .xaml extension");
            s.Close();
        }

        void TestNoManagerGiven()
        {
            Uri uri = new Uri("pack://application:,,,/newloadersuitehelperproject;component/thirdpartythemespage.xaml");

            LoadAssembly("newloadersuitehelperproject1/newloadersuitehelperproject.dll");
            StreamResourceInfo part = Application.GetResourceStream(new Uri("/newloadersuitehelperproject;component/thirdpartythemespage.xaml", UriKind.RelativeOrAbsolute));
            DRT.Assert(part != null, "Failed to return default part.");
            Stream s = part.Stream;
            DRT.Assert(s != null, "Failed to get a resource when not given a resource manager explicitly.");

            XamlReader asyncObjectConverter = new XamlReader();
            StackPanel data = Call(_mimeObjectFactoryType, null, "GetObjectAndCloseStream", s, CreateContentType("application/baml+xml"), uri, false, false, false, false, asyncObjectConverter) as StackPanel;
            DRT.Assert(data != null);
            DRT.Assert(data.Name == "root");

            LoadAssembly("newloadersuitehelperproject.dll");
            //Assembly assembly2 = Assembly.ReflectionOnlyLoad("newloadersuitehelperproject");
            StreamResourceInfo part2 = Application.GetResourceStream(new Uri("/newloadersuitehelperproject;component/thirdpartythemespage.xaml", UriKind.RelativeOrAbsolute));
            DRT.Assert(part2 != null, "Failed to return default part.");
            Stream s2 = part2.Stream;
            DRT.Assert(s2 != null, "Failed to get a resource when not given a resource manager explicitly.");

            StackPanel data2 = Call(_mimeObjectFactoryType, null, "GetObjectAndCloseStream", s2, CreateContentType("application/baml+xml"), uri, false, false, false, false, asyncObjectConverter) as StackPanel;
            DRT.Assert(data2 != null, "No StackPanel found");
            DRT.Assert(data2.Name == "testresourceloading");            
        }

        private Assembly LoadAssembly(string filename)
        {
            FileStream fin = new FileStream(filename, FileMode.Open, FileAccess.Read);
            byte[] bin = new byte[16384];
            long rdlen = 0;
            long total = fin.Length;
            int len;
            MemoryStream memStream = new MemoryStream((int)total);
            rdlen = 0;
            while (rdlen < total)
            {
                len = fin.Read(bin, 0, 16384);
                memStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }
            // done with input file
            fin.Close();
            return Assembly.Load(memStream.ToArray());
        }

        void TestFileFallback()
        {
            // attempt to load a file from DrtFiles, hopefully I'll pick one that isn't going away
            StreamResourceInfo part = Application.GetContentStream(new Uri("/DrtFiles/loader_tulip.jpg", UriKind.RelativeOrAbsolute));
            DRT.Assert(part != null, "Failed to return default part.");
            Stream s = part.Stream;
            DRT.Assert(s != null, "Failed to fall back to the file system to get a file");
            DRT.Assert(s is System.IO.FileStream, "Stream is of the wrong type.  Should be FileStream");
            s.Close();
        }

        void TestNoFileExists()
        {
            Stream s = null;
            try
            {
                StreamResourceInfo part = Application.GetResourceStream(new Uri("/foo/bar/baz/FileDoesNotExist.xaml", UriKind.RelativeOrAbsolute));
                DRT.Assert(part != null, "Failed to return default part.");
                s = part.Stream;
                DRT.Fail("PackagePart should have thrown an exception when ResourcePart gave it a null stream.");
            }
            catch (System.IO.IOException)
            {
                // expected code path
            }
            DRT.Assert(s == null, "Returned a stream when resource or file does not exist.");
        }

        void TestMimeObjectFactorySync()
        {
            // I need a stream, mime type, and Uri

            Uri uri = new Uri("pack://application:,,,/resource.baml");

            // re-use test case 1 'TestLoadFromDefault'
            StreamResourceInfo part = Application.GetResourceStream(new Uri("/resource.baml", UriKind.RelativeOrAbsolute));
            DRT.Assert(part != null, "Failed to return default part.");
            Stream s = part.Stream;
            DRT.Assert(s != null, "Failed to load from the default resource manager");

            // temporarily hard coding the baml mime type.  WeiBing's checkin will have part return the right mime type
            XamlReader asyncObjectConverter = new XamlReader();
            object data = Call(_mimeObjectFactoryType, null, "GetObjectAndCloseStream", s, CreateContentType("application/baml+xml"), uri, false, false, false, false, asyncObjectConverter);
            DRT.Assert(data != null, "The factory failed to produce an object for the baml stream");
        }

        void TestMimeObjectFactoryAsync()
        {
            Uri uri = new Uri("pack://application:,,,/resource.baml");

            // re-use test case 1 'TestLoadFromDefault'
            StreamResourceInfo part = Application.GetResourceStream(new Uri("/resource.baml", UriKind.RelativeOrAbsolute));
            DRT.Assert(part != null, "Failed to return default part.");
            Stream s = part.Stream;
            DRT.Assert(s != null, "Failed to load from the default resource manager");

            _calledBack = false;
            _callbackWaiter.Reset();

            IAsyncResult result = (IAsyncResult)Call(_mimeObjectFactoryType, null, "BeginGetObjectAndCloseStream", s, CreateContentType("application/baml+xml"), uri, false, false, false, false, new AsyncCallback(CallBack), new object());
            _callbackWaiter.WaitOne(5000, true); // wait for the callback for up to 5 seconds
            object data = Call(_mimeObjectFactoryType, null, "EndGetObjectAndCloseStream", result);

            DRT.Assert(_calledBack == true, "The asynchronous did not invoke the callback method");
            DRT.Assert(data != null, "EndGetObject did not return an object");
        }

        void TestMimeObjectFactoryLoadXaml()
        {
            Uri uri = new Uri("pack://application:,,,/DrtFiles/PageA.xaml"); // from file system

            StreamResourceInfo part = Application.GetContentStream(new Uri("/DrtFiles/PageA.xaml", UriKind.RelativeOrAbsolute));
            DRT.Assert(part != null, "Failed to return default part.");
            Stream s = part.Stream;
            DRT.Assert(s != null, "Failed to load pageA.xaml from the disk");
            
            XamlReader asyncObjectConverter = new XamlReader(); 
            object data = Call(_mimeObjectFactoryType, null, "GetObjectAndCloseStream", s, CreateContentType("application/xaml+xml"), uri, false, false, false, false, asyncObjectConverter);
            DRT.Assert(data != null, "The factory failed to produce an object for the xaml stream");
        }
    }
}

