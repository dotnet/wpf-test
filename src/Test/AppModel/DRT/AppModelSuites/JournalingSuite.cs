// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Threading;

namespace DRT
{
    /// <summary>
    /// </summary>
    class JournalingSuite : DrtTestSuite
    {
        public JournalingSuite()
            : base("Journaling")
        {
            TeamContact = "WPF";
            Contact = "Microsoft";         // if different from DRT
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{
                        new DrtTest( Run ),
			};
        }

        #region Test helpers (reflection)

        private string GetDisplayName(Uri uri, Uri siteOfOrigin)
        {
            Type t = typeof(JournalEntry);
            MethodInfo method = t.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic);

            return (string) method.Invoke(null, new object[] { uri, siteOfOrigin });
        }

        #endregion


        private void AssertName(string expectedDisplayString, string originalString, string message)
        {
            Uri uri = new Uri(originalString, UriKind.RelativeOrAbsolute);
            DRT.LogOutput("AssertName: " + message);
            DRT.LogOutput("SOO       : " + _siteOfOrigin.ToString());
            DRT.LogOutput("URI       : " + originalString);
            DRT.LogOutput("Expected  : " + expectedDisplayString);
            string output = GetDisplayName(uri, _siteOfOrigin);
            DRT.LogOutput("Actual    : " + output);
            DRT.AssertEqual(expectedDisplayString, output, message);

            if (uri.IsAbsoluteUri && !uri.IsFile && !_siteOfOrigin.IsFile)
            {
                uri = new Uri(originalString + s_fancy, UriKind.RelativeOrAbsolute);
                expectedDisplayString += s_fancy;
                message += " (with query and fragment)";

                DRT.LogOutput("AssertName: " + message);
                DRT.LogOutput("SOO       : " + _siteOfOrigin.ToString());
                DRT.LogOutput("URI       : " + originalString);
                DRT.LogOutput("Expected  : " + expectedDisplayString);
                output = GetDisplayName(uri, _siteOfOrigin);
                DRT.LogOutput("Actual    : " + output);
                DRT.AssertEqual(expectedDisplayString, output, message);
            }
        }

        private void Run()
        {
            _siteOfOrigin = new Uri("https://no.site.of.origin.uris.yet/so/there.xbap");
            string httpUri = "http://foo.com/dir1/dir2/Bar.xaml";

            AssertName(
                httpUri,
                httpUri,
                "An absolute http URI gets passed through.");


            string fileUri = "file:///c:/dir1/foo.xaml";

            AssertName(
                fileUri,
                fileUri,
                "An absolute file URI gets passed through.");

            string packApp = "pack://application:,,,/"; 

            string simple = "Dir1/Foo.xaml";
            string packAppSimple = packApp + simple;

            AssertName(
                simple,
                packAppSimple,
                "A simple pack URI should be stripped of the scheme and server");

            string soo = "http://www.foo.com/path1/";
            string packSoo = "pack://siteoforigin:,,,/";
            string fooJpg = "images/foo.jpg";
            string fooXbap = "foo.xbap";

            _siteOfOrigin = new Uri(soo + fooXbap);

            AssertName(
                soo + fooJpg,
                packSoo + fooJpg,
                "Site of origin should be replaced with actual site of origin");

            string fsoo = "file:///c:/work/";
            packSoo = "pack://siteoforigin:,,/";
 
            _siteOfOrigin = new Uri(fsoo + fooXbap);

            AssertName(
                fsoo + fooJpg,
                packSoo + fooJpg,
                "Site of origin should be replaced with actual site of origin");

            string component = "mydll;component/";

            AssertName(
                simple,
                packApp + component + simple,
                "Should not show component part of URI");

            string fragment = "#top";

            AssertName(
                fragment,
                fragment,
                "Should pass a fragment-only URI straight through");

            AssertName(
                fragment,
                packApp + fragment,
                "Should strip the scheme and host from a pack app URI with a fragment");
        }

        const string _query = "a=b";
        const string _fragment = "top";
        static readonly string s_fancy = "?" + _query + "#" + _fragment;
        private Uri _siteOfOrigin;
    }
}
