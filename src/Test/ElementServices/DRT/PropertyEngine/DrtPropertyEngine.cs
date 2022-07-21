// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT for property engine.
//

using System;

// Loose resources should be declared in the .proj file.  Since this project does
// not have a .proj file the required attributes can be added manually.
// Note that these strings should all be lower case.
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"xmldatasource.xml")]


namespace DRT
{
    public sealed class DrtPropertyEngine : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            return new DrtPropertyEngine().Run(args);
        }

        private DrtPropertyEngine()
        {
            WindowTitle = "Property Engine DRT";
            Contact = "Microsoft";
            TeamContact = "WPF";
            DrtName = "DrtPropertyEngine";

            Suites = new DrtTestSuite[]{
                        new TestDependencyProperty(),
                        new TestInheritanceContext(),
                        new TestValueSource(),
                        new TestVisualTransforms(),
                        };
        }
    }
}
