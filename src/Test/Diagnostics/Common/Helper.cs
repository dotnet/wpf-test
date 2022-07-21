// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace XamlSourceInfoTest
{
    public static class Helper
    {
        private static string BaseUri
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0};component/RD.xaml", typeof(Helper).Assembly.GetName().Name);
            }
        }

        public static Uri BadRelativeResourceDictionaryUri
        {
            get
            {
                //   create relative URI without '/' at the beginning, e.g.
                //   BAD:   MyAssembly;comonent/RD.xaml
                //   GOOD:  /MyAssembly;comonent/RD.xaml
                return new Uri(Helper.BaseUri, UriKind.Relative);
            }
        }

        public static Uri GoodRelativeResourceDictionaryUri
        {
            get
            {
                return new Uri("/" + Helper.BaseUri, UriKind.Relative);
            }
        }
    }
}
