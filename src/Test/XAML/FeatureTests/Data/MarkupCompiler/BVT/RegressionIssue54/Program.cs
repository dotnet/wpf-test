// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Windows.Markup;

namespace RegressionIssue54
{
 class Program
  {
     const string xaml = "<Class1 xmlns='clr-namespace:ClassLibrary1;assembly=ClassLibrary1' />";
     const string xaml1 = "<Class1 xmlns='clr-namespace:ClassLibrary1;assembly=ClassLibrary1,Version=1.0.0.0' />";

     static void Main(string[] args)
     {
        bool caughtRightException = false;

        string exePath = Assembly.GetExecutingAssembly().Location;
        FileInfo file = new FileInfo(Path.Combine(exePath.Substring(0, exePath.LastIndexOf("\\")), @"..\..\ClassLibrary1\bin\Release\ClassLibrary1.dll"));
        Assembly.LoadFile(file.FullName);
        XamlReader.Parse(xaml);
        Console.WriteLine("Repro1: Load1 succeeded");
        try
        {
            XamlReader.Parse(xaml1);
        }
        catch (XamlParseException)
        {
            caughtRightException = true;
        }
        catch (Exception)
        {
            caughtRightException = false;
        }

        Console.WriteLine("CaughtRightException [" + caughtRightException + "]");
        if(!caughtRightException)
        {
            throw new Exception("Exception Not thrown in second case");
        }
    }
  }
}
