// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Markup;
    using System.Xaml;
    using Microsoft.Test.CDFInfrastructure;
    using Microsoft.Test.Globalization;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;
    using Microsoft.Test.Xaml.Driver;
    using Microsoft.Test.Xaml.Types.InstanceReference;
    using Microsoft.Test.Xaml.Types.Integration;
    using Microsoft.Test.Xaml.Utilities;

    public class InstanceReferenceTests
    {
        public static Dictionary<string, string> namespaces = new Dictionary<string, string>()
        {
            {"x", "http://schemas.microsoft.com/winfx/2006/xaml"},
            {"x2", "http://schemas.microsoft.com/netfx/2008/xaml"},
            {"xx", "clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"},
            {"p", "http://schemas.microsoft.com/netfx/2008/xaml/schema"},
        };

        //private static string localNamespace = "clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes";

        [TestCase]
        public void SimpleBackwardReference()
        {
            SimpleRefBar bar = new SimpleRefBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "Hello"
                                   };
            SimpleRefFoo foo = new SimpleRefFoo();
            foo.bar = bar;
            foo.bar2 = bar;

            XamlTestDriver.RoundTripCompareExamineXaml(foo,
                                                new string[]
                                                    {
                                                        @"/xx:SimpleRefFoo[@*='{x:Reference __ReferenceID0}']"
                                                    }, namespaces);
        }

        [TestCase]
        public void SimpleForwardReference()
        {
            SimpleRefBar bar = new SimpleRefBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "Hello"
                                   };
            SimpleRefFoo foo = new SimpleRefFoo();
            foo.bar = bar;
            foo.bar2 = bar;

            //  @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            //        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
            //        xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //      <xi:SimpleRefFoo.bar>
            //        <x2:Reference>__ReferenceID0</x2:Reference>
            //      </xi:SimpleRefFoo.bar>
            //      <xi:SimpleRefFoo.bar2>
            //        <xi:SimpleRefBar x:Name=""__ReferenceID0"" IntProperty=""5"" StringProperty=""Hello"" />
            //      </xi:SimpleRefFoo.bar2>
            //    </xi:SimpleRefFoo>";
            NodeList xamlDoc = new NodeList()
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                        NodeListFactory.CreateReference("__Reference_ID_0", false),
                    new EndMember(),
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        new StartObject(typeof(SimpleRefBar)),
                            NodeListFactory.CreateXName("__Reference_ID_0", true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "IntProperty", 5, true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "StringProperty", "Hello", true),              
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void MultipleReferences()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:MultipleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
            //xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
            //xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:MultipleRefFoo.barForward>
            //    <x2:Reference>__ReferenceID0</x2:Reference>
            //  </xi:MultipleRefFoo.barForward>                          
            //  <xi:MultipleRefFoo.bar>
            //    <xi:MultipleRefBar x:Name=""__ReferenceID0"" IntProperty=""15"" StringProperty=""Hello world"" />
            //  </xi:MultipleRefFoo.bar>
            //  <xi:MultipleRefFoo.barBackward>
            //    <x2:Reference>__ReferenceID0</x2:Reference>
            //  </xi:MultipleRefFoo.barBackward>
            //</xi:MultipleRefFoo>";

            NodeList xamlDoc = new NodeList()
            {
                new StartObject(typeof(MultipleRefFoo)),
                    new StartMember(typeof(MultipleRefFoo), "barForward"),
                        NodeListFactory.CreateReference("__Reference_ID_0", false),
                    new EndMember(),
                    new StartMember(typeof(MultipleRefFoo), "bar"),
                        new StartObject(typeof(MultipleRefBar)),
                            NodeListFactory.CreateXName("__Reference_ID_0", true),
                            NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                            NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                        new EndObject(),
                    new EndMember(),
                    new StartMember(typeof(MultipleRefFoo), "barBackward"),
                        NodeListFactory.CreateReference("__Reference_ID_0", false),
                    new EndMember(),
                new EndObject(),
            };

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void TreeReferencesLeafForward()
        {
            TreeFoo root = new TreeFoo()
                               {
                                   IntProperty = 100,
                                   StringProperty = "root"
                               };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };
            TreeFoo child1 = new TreeFoo()
                                 {
                                     IntProperty = 102,
                                     StringProperty = "child1"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            // child11 points to  child01
            TreeFoo child11 = child01;

            root.child0 = child0;
            root.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;
            child1.child0 = child10;
            child1.child1 = child11;

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <xi:TreeFoo IntProperty=""100"" StringProperty=""root"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
                xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
              <xi:TreeFoo.child1>
                <xi:TreeFoo IntProperty=""102"" StringProperty=""child1"">
                  <xi:TreeFoo.child0>
                    <xi:TreeFoo IntProperty=""105"" StringProperty=""child10"">
                      <xi:TreeFoo.child0>
                        <x:Null />
                      </xi:TreeFoo.child0>
                      <xi:TreeFoo.child1>
                        <x:Null />
                      </xi:TreeFoo.child1>
                    </xi:TreeFoo>
                  </xi:TreeFoo.child0>
                  <xi:TreeFoo.child1>
                    <x:Reference>__Reference_ID_0</x:Reference>
                  </xi:TreeFoo.child1>
                </xi:TreeFoo>
              </xi:TreeFoo.child1>                           
              <xi:TreeFoo.child0>
                <xi:TreeFoo IntProperty=""101"" StringProperty=""child0"">
                  <xi:TreeFoo.child0>
                    <xi:TreeFoo IntProperty=""103"" StringProperty=""child00"">
                      <xi:TreeFoo.child0>
                        <x:Null />
                      </xi:TreeFoo.child0>
                      <xi:TreeFoo.child1>
                        <x:Null />
                      </xi:TreeFoo.child1>
                    </xi:TreeFoo>
                  </xi:TreeFoo.child0>
                  <xi:TreeFoo.child1>
                    <xi:TreeFoo x:Name=""__Reference_ID_0"" IntProperty=""104"" StringProperty=""child01"">
                      <xi:TreeFoo.child0>
                        <x:Null />
                      </xi:TreeFoo.child0>
                      <xi:TreeFoo.child1>
                        <x:Null />
                      </xi:TreeFoo.child1>
                    </xi:TreeFoo>
                  </xi:TreeFoo.child1>
                </xi:TreeFoo>
              </xi:TreeFoo.child0>
            </xi:TreeFoo>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, root);
        }

        [TestCase]
        public void TreeReferencesLeafBackward()
        {
            TreeFoo root = new TreeFoo()
                               {
                                   IntProperty = 100,
                                   StringProperty = "root"
                               };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };
            TreeFoo child1 = new TreeFoo()
                                 {
                                     IntProperty = 102,
                                     StringProperty = "child1"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            // child11 points to  child01
            TreeFoo child11 = child01;

            root.child0 = child0;
            root.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;
            child1.child0 = child10;
            child1.child1 = child11;

            XamlTestDriver.RoundTripCompare(root);
        }

        [TestCase]
        public void TreeReferencesLeafRefsNonLeafForward()
        {
            TreeFoo root = new TreeFoo()
                               {
                                   IntProperty = 100,
                                   StringProperty = "root"
                               };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };
            TreeFoo child1 = new TreeFoo()
                                 {
                                     IntProperty = 102,
                                     StringProperty = "child1"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            // child11 points to  child1 (non leaf)
            TreeFoo child11 = child1;

            root.child0 = child0;
            root.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;
            child1.child0 = child10;
            child1.child1 = child11;

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <xi:TreeFoo IntProperty=""100"" StringProperty=""root"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                                xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
                                xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
                              <xi:TreeFoo.child1>
                                <xi:TreeFoo x:Name=""__Reference_ID_0"" IntProperty=""102"" StringProperty=""child1"">
                                  <xi:TreeFoo.child0>
                                    <xi:TreeFoo IntProperty=""105"" StringProperty=""child10"">
                                      <xi:TreeFoo.child0>
                                        <x:Null />
                                      </xi:TreeFoo.child0>
                                      <xi:TreeFoo.child1>
                                        <x:Null />
                                      </xi:TreeFoo.child1>
                                    </xi:TreeFoo>
                                  </xi:TreeFoo.child0>
                                  <xi:TreeFoo.child1>
                                    <x:Reference>__Reference_ID_0</x:Reference>
                                  </xi:TreeFoo.child1>
                                </xi:TreeFoo>
                              </xi:TreeFoo.child1>
                              <xi:TreeFoo.child0>
                                <xi:TreeFoo IntProperty=""101"" StringProperty=""child0"">
                                  <xi:TreeFoo.child0>
                                    <xi:TreeFoo IntProperty=""103"" StringProperty=""child00"">
                                      <xi:TreeFoo.child0>
                                        <x:Null />
                                      </xi:TreeFoo.child0>
                                      <xi:TreeFoo.child1>
                                        <x:Null />
                                      </xi:TreeFoo.child1>
                                    </xi:TreeFoo>
                                  </xi:TreeFoo.child0>
                                  <xi:TreeFoo.child1>
                                    <xi:TreeFoo IntProperty=""104"" StringProperty=""child01"">
                                      <xi:TreeFoo.child0>
                                        <x:Null />
                                      </xi:TreeFoo.child0>
                                      <xi:TreeFoo.child1>
                                        <x:Null />
                                      </xi:TreeFoo.child1>
                                    </xi:TreeFoo>
                                  </xi:TreeFoo.child1>
                                </xi:TreeFoo>
                              </xi:TreeFoo.child0>
                            </xi:TreeFoo>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, root);
        }

        [TestCase]
        public void TreeReferencesLeafRefsNonLeafBackward()
        {
            TreeFoo root = new TreeFoo()
                               {
                                   IntProperty = 100,
                                   StringProperty = "root"
                               };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };
            TreeFoo child1 = new TreeFoo()
                                 {
                                     IntProperty = 102,
                                     StringProperty = "child1"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            // child11 points to  child1 (non leaf)
            TreeFoo child11 = child1;

            root.child0 = child0;
            root.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;
            child1.child0 = child10;
            child1.child1 = child11;

            XamlTestDriver.RoundTripCompare(root);
        }

        [TestCase]
        public void TreeReferencesNonLeafRefsLeafForward()
        {
            TreeFoo root = new TreeFoo()
                               {
                                   IntProperty = 100,
                                   StringProperty = "root"
                               };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            // non leaf references leaf //
            TreeFoo child1 = child00;

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            TreeFoo child11 = new TreeFoo()
                                  {
                                      IntProperty = 106,
                                      StringProperty = "child11"
                                  };

            root.child0 = child0;
            root.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <xi:TreeFoo IntProperty=""100"" StringProperty=""root"" 
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                            xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
                            xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
                              <xi:TreeFoo.child1>        
                                <x:Reference>__Reference_ID_0</x:Reference>
                              </xi:TreeFoo.child1>
                              <xi:TreeFoo.child0>
                                <xi:TreeFoo IntProperty=""101"" StringProperty=""child0"">
                                  <xi:TreeFoo.child0>
                                    <xi:TreeFoo x:Name=""__Reference_ID_0"" IntProperty=""103"" StringProperty=""child00"">
                                      <xi:TreeFoo.child0>
                                        <x:Null />
                                      </xi:TreeFoo.child0>
                                      <xi:TreeFoo.child1>
                                        <x:Null />
                                      </xi:TreeFoo.child1>
                                    </xi:TreeFoo>
                                  </xi:TreeFoo.child0>
                                  <xi:TreeFoo.child1>
                                    <xi:TreeFoo IntProperty=""104"" StringProperty=""child01"">
                                      <xi:TreeFoo.child0>
                                        <x:Null />
                                      </xi:TreeFoo.child0>
                                      <xi:TreeFoo.child1>
                                        <x:Null />
                                      </xi:TreeFoo.child1>
                                    </xi:TreeFoo>
                                  </xi:TreeFoo.child1>
                                </xi:TreeFoo>
                              </xi:TreeFoo.child0>                           
                             </xi:TreeFoo>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, root);
        }

        [TestCase]
        public void TreeReferencesNonLeafRefsLeafBackward()
        {
            TreeFoo root = new TreeFoo()
                               {
                                   IntProperty = 100,
                                   StringProperty = "root"
                               };

            TreeFoo child0 = new TreeFoo()
                                 {
                                     IntProperty = 101,
                                     StringProperty = "child0"
                                 };

            TreeFoo child00 = new TreeFoo()
                                  {
                                      IntProperty = 103,
                                      StringProperty = "child00"
                                  };
            TreeFoo child01 = new TreeFoo()
                                  {
                                      IntProperty = 104,
                                      StringProperty = "child01"
                                  };

            // non leaf references leaf //
            TreeFoo child1 = child00;

            TreeFoo child10 = new TreeFoo()
                                  {
                                      IntProperty = 105,
                                      StringProperty = "child10"
                                  };
            TreeFoo child11 = new TreeFoo()
                                  {
                                      IntProperty = 106,
                                      StringProperty = "child11"
                                  };

            root.child0 = child0;
            root.child1 = child1;
            child0.child0 = child00;
            child0.child1 = child01;

            XamlTestDriver.RoundTripCompare(root);
        }

        // [DISABLED]
        // [TestCase]
        
        public void DifferentNames()
        {
            SimpleRefBar bar = new SimpleRefBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "Hello"
                                   };
            SimpleRefFoo foo = new SimpleRefFoo();
            foo.bar = bar;
            foo.bar2 = bar;

            // _ in name //
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>_myref</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""_myref"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            NodeList xamlDoc = GetNodeList("_myref");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);

            // name is _ //
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>_</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""_"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            xamlDoc = GetNodeList("_");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);

            // name has . //
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>name has . in it</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""name has . in it"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            xamlDoc = GetNodeList("name has . in it");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);

            // name has : //
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>name has : in it</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""name has : in it"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            xamlDoc = GetNodeList("name has : in it");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);

            // name has - //
            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>name has - in it</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""name has - in it"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            xamlDoc = xamlDoc = GetNodeList("name has - in it");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);

            // name has upper and lower case //
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>UPPER Both lower</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""UPPER Both lower"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            xamlDoc = GetNodeList("UPPER Both lower");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);

            // how to note <xml:space=preserve>
            // name has leading spaces //
            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
                xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
              <xi:SimpleRefFoo.bar>
                <x:Reference xml:space=""preserve""> name has leading spaces</x:Reference>
              </xi:SimpleRefFoo.bar>
              <xi:SimpleRefFoo.bar2>
                <xi:SimpleRefBar x:Name="" name has leading spaces"" IntProperty=""5"" StringProperty=""Hello"" />
              </xi:SimpleRefFoo.bar2>
            </xi:SimpleRefFoo>";
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo);

            // name has trailing spaces //
            xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                    xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
                    xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
              <xi:SimpleRefFoo.bar>
                <x:Reference xml:space=""preserve"">name has trailing spaces </x:Reference>
              </xi:SimpleRefFoo.bar>
              <xi:SimpleRefFoo.bar2>
                <xi:SimpleRefBar x:Name=""name has trailing spaces "" IntProperty=""5"" StringProperty=""Hello"" />
              </xi:SimpleRefFoo.bar2>
            </xi:SimpleRefFoo>";
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo);
        }

        private NodeList GetNodeList(string xNameValue)
        {
            return new NodeList()
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                      NodeListFactory.CreateReference(xNameValue, false),
                    new EndMember(),
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        new StartObject(typeof(SimpleRefBar)),
                            NodeListFactory.CreateXName(xNameValue, true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "IntProperty", 5, true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "StringProperty", "Hello", true),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };
        }

        [TestCase]
        public void ExternalReference()
        {
            SimpleRefBar bar = new SimpleRefBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "Hello"
                                   };
            SimpleRefFoo foo = new SimpleRefFoo();
            foo.bar = bar;
            foo.bar2 = bar;

            // @"<?xml version=""1.0"" encoding=""utf-8""?>
            //    <xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //      <xi:SimpleRefFoo.bar>
            //        <x2:Reference>externalRef</x2:Reference>
            //      </xi:SimpleRefFoo.bar>
            //      <xi:SimpleRefFoo.bar2>
            //        <x2:Reference>externalRef</x2:Reference>
            //      </xi:SimpleRefFoo.bar2>
            //    </xi:SimpleRefFoo>";

            NodeList xamlDoc = new NodeList()
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                        NodeListFactory.CreateReference("externalRef", false),
                    new EndMember(),
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        NodeListFactory.CreateReference("externalRef", false),
                    new EndMember(),
                new EndObject(),
            };

            NameScopeImpl nameScope = new NameScopeImpl();
            nameScope.RegisterName("externalRef", bar);
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo, nameScope);
        }

        [TestCase]
        public void ImplicitNaming()
        {
            ImplicitNamingBar bar = new ImplicitNamingBar()
                                        {
                                            IntProperty = 5,
                                            StringProperty = "Hello",
                                            MyImplicitName = "MyName"
                                        };
            ImplicitNamingFoo foo = new ImplicitNamingFoo();
            foo.bar = bar;
            foo.bar2 = bar;

            string xaml = XamlTestDriver.RoundTripCompare(foo);
            XamlTestDriver.RoundTripCompareExamineXaml(foo, new string[]
                                                         {
                                                             @"/xx:ImplicitNamingFoo[@bar2='{x:Reference MyName}']"
                                                         }, namespaces);
        }

        // 
        [TestCase]
        public void ReferenceNull()
        {
            SimpleRefFoo foo = new SimpleRefFoo();
            foo.bar = null;
            foo.bar2 = foo.bar;

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                    <mtxti:SimpleRefFoo 
                    xmlns:mtxti=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"" 
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                            <mtxti:SimpleRefFoo.bar>
                                    <x:Null x:Name=""myref""/>
                            </mtxti:SimpleRefFoo.bar>
                            <mtxti:SimpleRefFoo.bar2>
                                    <x:Reference>myref</x:Reference>
                            </mtxti:SimpleRefFoo.bar2>
                    </mtxti:SimpleRefFoo>
                            ";
            try
            {
                object obj = XamlTestDriver.Deserialize(xaml);
                throw new TestCaseFailedException("Referencing null is not allowed");
            }
            catch (XamlObjectWriterException ex)
            {
                Tracer.LogTrace("Referencing null should throw - got expected exception " + ex.Message);
            }
        }

        // 
        [TestCase]
        public void ForwardReferenceNull()
        {
            SimpleRefFoo foo = new SimpleRefFoo();
            foo.bar = null;
            foo.bar2 = foo.bar;

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                    <mtxti:SimpleRefFoo 
                    xmlns:mtxti=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"" 
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                            <mtxti:SimpleRefFoo.bar2>
                                    <x:Reference>myref</x:Reference>
                            </mtxti:SimpleRefFoo.bar2>
                            <mtxti:SimpleRefFoo.bar>
                                    <x:Null x:Name=""myref""/>
                            </mtxti:SimpleRefFoo.bar>
                    </mtxti:SimpleRefFoo>
                            ";
            try
            {
                object obj = XamlTestDriver.Deserialize(xaml);
                throw new TestCaseFailedException("Referencing null is not allowed");
            }
            catch (XamlObjectWriterException ex)
            {
                Tracer.LogTrace("Referencing null should throw - got expected exception " + ex.Message);
            }
        }

        [TestCase]
        public void ReferenceEmptyString()
        {
            SimpleRefBar bar1 = new SimpleRefBar()
                                    {
                                        IntProperty = 5,
                                        StringProperty = ""
                                    };
            SimpleRefBar bar2 = new SimpleRefBar()
                                    {
                                        IntProperty = 15,
                                        StringProperty = bar1.StringProperty
                                    };
            SimpleRefFoo foo = new SimpleRefFoo()
                                   {
                                       bar = bar1,
                                       bar2 = bar2
                                   };

            string xaml = XamlTestDriver.RoundTripCompare(foo);
            XamlTestDriver.RoundTripCompareExamineXaml(foo,
                                                new string[]
                                                    {
                                                        @"/xx:SimpleRefFoo/xx:SimpleRefFoo.bar/xx:SimpleRefBar[@StringProperty='']",
                                                        @"/xx:SimpleRefFoo/xx:SimpleRefFoo.bar2/xx:SimpleRefBar[@StringProperty='']",
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void MultipleRefForward()
        {
            object common = new SimpleRefBar()
                                {
                                    IntProperty = 100,
                                    StringProperty = "simplebar"
                                };

            A multipleRefA = new A()
                                 {
                                     C = common,
                                     D = common
                                 };

            string xaml = XamlTestDriver.RoundTripCompare(multipleRefA);
            XamlTestDriver.RoundTripCompareExamineXaml(multipleRefA,
                                                new string[]
                                                    {
                                                        @"/xx:A/xx:A.C/xx:SimpleRefBar",
                                                    },
                                                namespaces);
        }

        [TestCase]
        public void ReferenceNameWithinNamescope()
        {
            UnScopedBar ubar = new UnScopedBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "hello"
                                   };
            ScopedBar bar1 = new ScopedBar()
                                 {
                                     ubar = ubar,
                                     ubar2 = ubar
                                 };

            UnScopedFoo foo = new UnScopedFoo()
                                  {
                                      bar = bar1,
                                      bar2 = null
                                  };

            string xaml = XamlTestDriver.RoundTripCompare(foo);
        }

        //  
        [TestCase]
        public void ReferenceNameAboveNameScope()
        {
            UnScopedBar ubar = new UnScopedBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "hello"
                                   };
            UnScopedBar ubar2 = new UnScopedBar()
                                    {
                                        IntProperty = 10,
                                        StringProperty = "boho"
                                    };

            ScopedBar bar1 = new ScopedBar()
                                 {
                                     ubar = ubar,
                                     ubar2 = ubar2
                                 };

            UnScopedFoo foo = new UnScopedFoo()
                                  {
                                      bar = bar1,
                                      bar2 = null,
                                      bar3 = ubar2
                                  };

            string xaml = XamlTestDriver.RoundTripCompare(foo);
        }

        [TestCase]
        public void MultipleNamescopes()
        {
            UnScopedBar ubar1 = new UnScopedBar()
                                    {
                                        IntProperty = 5,
                                        StringProperty = "hello"
                                    };
            UnScopedBar ubar2 = new UnScopedBar()
                                    {
                                        IntProperty = 10,
                                        StringProperty = "boho"
                                    };

            ScopedBar bar1 = new ScopedBar()
                                 {
                                     ubar = ubar1,
                                     ubar2 = ubar1
                                 };
            ScopedBar bar2 = new ScopedBar()
                                 {
                                     ubar = ubar2,
                                     ubar2 = ubar2
                                 };

            UnScopedFoo foo = new UnScopedFoo()
                                  {
                                      bar = bar1,
                                      bar2 = bar2,
                                      bar3 = null
                                  };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                                <xi:UnScopedFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
                        xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
                                  <xi:UnScopedFoo.bar>
                                    <xi:ScopedBar>
                                      <xi:ScopedBar.ubar>
                                        <xi:UnScopedBar x:Name=""myref"" IntProperty=""5"" StringProperty=""hello"">
                                          <xi:UnScopedBar.bar4>
                                            <x:Null />
                                          </xi:UnScopedBar.bar4>
                                        </xi:UnScopedBar>
                                      </xi:ScopedBar.ubar>
                                      <xi:ScopedBar.ubar2>
                                        <x:Reference>myref</x:Reference>
                                      </xi:ScopedBar.ubar2>
                                    </xi:ScopedBar>
                                  </xi:UnScopedFoo.bar>
                                  <xi:UnScopedFoo.bar2>
                                    <xi:ScopedBar>
                                      <xi:ScopedBar.ubar>
                                        <xi:UnScopedBar x:Name=""myref"" IntProperty=""10"" StringProperty=""boho"">
                                          <xi:UnScopedBar.bar4>
                                            <x:Null />
                                          </xi:UnScopedBar.bar4>
                                        </xi:UnScopedBar>
                                      </xi:ScopedBar.ubar>
                                      <xi:ScopedBar.ubar2>
                                        <x:Reference>myref</x:Reference>
                                      </xi:ScopedBar.ubar2>
                                    </xi:ScopedBar>
                                  </xi:UnScopedFoo.bar2>
                                  <xi:UnScopedFoo.bar3>
                                    <x:Null />
                                  </xi:UnScopedFoo.bar3>
                                      </xi:UnScopedFoo>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, foo);
        }

        // 

        [TestCase]
        public void NestedNameScopes1()
        {
            UnscopedBar2 ubar1 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar1"
                                     };
            UnscopedBar2 ubar2 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar2"
                                     };
            // UnscopedBar2 ubar3 = new UnscopedBar2() { StringProperty = "hello ubar3" };

            NestedScope scope2 = new NestedScope()
                                     {
                                         scope1 = null,
                                         unscopedBar = ubar2
                                     };
            NestedScope scope1 = new NestedScope()
                                     {
                                         scope1 = scope2,
                                         unscopedBar = ubar2
                                     };
            UnscopedFoo1 foo = new UnscopedFoo1()
                                   {
                                       nestedScope = scope1,
                                       ubar2 = ubar1
                                   };

            string xaml = XamlTestDriver.RoundTripCompare(foo);
        }

        // scope2 references parent ; 
        [TestCase]
        public void NestedNameScopes2()
        {
            UnscopedBar2 ubar1 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar1"
                                     };
            UnscopedBar2 ubar2 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar2"
                                     };
            //UnscopedBar2 ubar3 = new UnscopedBar2() { StringProperty = "hello ubar3" };

            NestedScope scope2 = new NestedScope()
                                     {
                                         scope1 = null,
                                         unscopedBar = ubar1
                                     };
            NestedScope scope1 = new NestedScope()
                                     {
                                         scope1 = scope2,
                                         unscopedBar = ubar2
                                     };
            UnscopedFoo1 foo = new UnscopedFoo1()
                                   {
                                       nestedScope = scope1,
                                       ubar2 = ubar1
                                   };

            string xaml = XamlTestDriver.RoundTripCompare(foo);
        }

        // 
        [TestCase]
        public void NestedNamescopeExternalRef()
        {
            UnscopedBar2 ubar1 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar1"
                                     };
            UnscopedBar2 ubar2 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar2"
                                     };
            UnscopedBar2 ubar3 = new UnscopedBar2()
                                     {
                                         StringProperty = "hello ubar3"
                                     };

            NestedScope scope2 = new NestedScope()
                                     {
                                         scope1 = null,
                                         unscopedBar = ubar3
                                     };
            NestedScope scope1 = new NestedScope()
                                     {
                                         scope1 = scope2,
                                         unscopedBar = ubar2
                                     };
            UnscopedFoo1 foo = new UnscopedFoo1()
                                   {
                                       nestedScope = scope1,
                                       ubar2 = ubar1
                                   };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <xi:UnscopedFoo1 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
                        xmlns:x2=""http://schemas.microsoft.com/winfx/2006/xaml""
                        xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
                              <xi:UnscopedFoo1.nestedScope>
                                <xi:NestedScope>
                                  <xi:NestedScope.scope1>
                                    <xi:NestedScope>
                                      <xi:NestedScope.scope1>
                                        <x:Null />
                                      </xi:NestedScope.scope1>
                                      <xi:NestedScope.unscopedBar>
                                        <x:Reference>externalRef3</x:Reference>
                                      </xi:NestedScope.unscopedBar>
                                    </xi:NestedScope>
                                  </xi:NestedScope.scope1>
                                  <xi:NestedScope.unscopedBar>
                                    <x2:Reference>externalRef2</x2:Reference>
                                  </xi:NestedScope.unscopedBar>
                                </xi:NestedScope>
                              </xi:UnscopedFoo1.nestedScope>
                              <xi:UnscopedFoo1.ubar2>
                                <x:Reference>externalRef1</x:Reference>
                              </xi:UnscopedFoo1.ubar2>
                            </xi:UnscopedFoo1>";

            NameScopeImpl nameScope = new NameScopeImpl();
            nameScope.RegisterName("externalRef1", ubar1);
            nameScope.RegisterName("externalRef2", ubar2);
            nameScope.RegisterName("externalRef3", ubar3);
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, nameScope);
        }

        [TestCase]
        // 
        public void DesignerSerializationVisibility()
        {
            SimpleDSVFoo foo = new SimpleDSVFoo()
                                   {
                                       IntPropertyVisible = 10,
                                       IntListVisible = new List<int>()
                                                            {
                                                                1, 2, 3
                                                            },
                                       barVisible = new SimpleDSVBar()
                                                        {
                                                            IntProperty = 100,
                                                            StringProperty = "visible bar"
                                                        },
                                       IntPropertyContent = 100,
                                       IntListContent = new List<int>()
                                                            {
                                                                4, 5, 6
                                                            },
                                       barContent = new SimpleDSVBar()
                                                        {
                                                            IntProperty = 1001,
                                                            StringProperty = "content bar"
                                                        },
                                       IntPropertyHidden = 1000,
                                       IntListHidden = new List<int>()
                                                           {
                                                               7, 8, 9
                                                           },
                                       barHidden = new SimpleDSVBar()
                                                       {
                                                           IntProperty = 1002,
                                                           StringProperty = "hidden bar"
                                                       }
                                   };

            string xaml = XamlTestDriver.Serialize(foo);

            foo.IntPropertyHidden = default(int);
            foo.IntListHidden = null;
            foo.barHidden = null;

            XamlTestDriver.XamlFirstCompareObjects(xaml, foo);
        }

        [TestCase]
        // 
        public void ReferenceHiddenProperty()
        {
            SimpleDSVBar bar = new SimpleDSVBar()
                                   {
                                       IntProperty = 4,
                                       StringProperty = " My string property"
                                   };
            FooWithHiddenBar foo = new FooWithHiddenBar()
                                       {
                                           VisibleBar = bar,
                                           HiddenBar = bar
                                       };

            string xaml = XamlTestDriver.Serialize(foo);

            foo.HiddenBar = null;

            XamlTestDriver.XamlFirstCompareObjects(xaml, foo);
        }

        [TestCase]
        public void NonExistingReference()
        {
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference>Non Existant Reference</x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name=""__ReferenceID0"" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            NodeList xamlDoc = new NodeList()
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                        NodeListFactory.CreateReference("Non Existant Reference", false),
                    new EndMember(),    
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        new StartObject(typeof(SimpleRefBar)),
                            NodeListFactory.CreateXName("__Reference_ID_0", true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "IntProperty", 5, true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "StringProperty", "Hello", true),
                        new EndObject(),
                    new EndMember(),
                    
                new EndObject(),
            };

            try
            {
                string xaml = xamlDoc.NodeListToXml();
                XamlTestDriver.Deserialize(xaml, null);
            }
            catch (XamlObjectWriterException iop)
            {
                Console.WriteLine("Expected exception caught " + iop.Message);
            }
        }

        // 
        [TestCase]
        public void ReferenceNameisEmptyString()
        {
            //@"<?xml version=""1.0"" encoding=""utf-8""?>
            //<xi:SimpleRefFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" xmlns:xi=""clr-namespace:XamlTests.InstanceReference;assembly=XamlTests"">
            //  <xi:SimpleRefFoo.bar>
            //    <x2:Reference></x2:Reference>
            //  </xi:SimpleRefFoo.bar>
            //  <xi:SimpleRefFoo.bar2>
            //    <xi:SimpleRefBar x:Name="""" IntProperty=""5"" StringProperty=""Hello"" />
            //  </xi:SimpleRefFoo.bar2>
            //</xi:SimpleRefFoo>";
            NodeList xamlDoc = new NodeList()
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                        NodeListFactory.CreateReference("", false),
                    new EndMember(),
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        new StartObject(typeof(SimpleRefBar)),
                            NodeListFactory.CreateXName("", true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "IntProperty", 5, true),
                            NodeListFactory.CreateMember(typeof(SimpleRefBar), "StringProperty", "Hello", true),            
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };

            try
            {
                string xaml = xamlDoc.NodeListToXml();
                XamlTestDriver.Deserialize(xaml, null);
            }
            catch (XamlObjectWriterException iop)
            {
                Console.WriteLine("Expected exception caught" + iop.Message);
            }
        }

        // 
        [TestCase]
        public void ImplicitNameWithEmptyString()
        {
            ImplicitNamingBar bar = new ImplicitNamingBar()
                                        {
                                            IntProperty = 5,
                                            StringProperty = "Hello",
                                            MyImplicitName = ""
                                        };
            ImplicitNamingFoo foo = new ImplicitNamingFoo();
            foo.bar = bar;
            foo.bar2 = bar;

            // string serialized = XamlServices.Save(foo);
            string serialized = @"<ImplicitNamingFoo xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                                  <ImplicitNamingFoo.bar>
                                    <ImplicitNamingBar x:Name="""" IntProperty=""5"" StringProperty=""Hello"" />
                                  </ImplicitNamingFoo.bar>
                                  <ImplicitNamingFoo.bar2>
                                    <x:Reference></x:Reference>
                                  </ImplicitNamingFoo.bar2>
                                </ImplicitNamingFoo>";

            string message = Exceptions.GetMessage("NameScopeException", WpfBinaries.SystemXaml);

            ExceptionHelpers.CheckForException(typeof(XamlObjectWriterException), message, () => XamlTestDriver.Deserialize(serialized, null));
        }

        [TestCase]
        public void CircularReferences()
        {
            Microsoft.Test.Xaml.Types.InstanceReference.Node node1 = new Microsoft.Test.Xaml.Types.InstanceReference.Node()
                             {
                                 Bar = new SimpleRefBar()
                                           {
                                               IntProperty = 5,
                                               StringProperty = "hello"
                                           }
                             };
            Microsoft.Test.Xaml.Types.InstanceReference.Node node2 = new Microsoft.Test.Xaml.Types.InstanceReference.Node()
                             {
                                 Bar = new SimpleRefBar()
                                           {
                                               IntProperty = 15,
                                               StringProperty = "hello world"
                                           }
                             };

            node1.Next = node2;
            node2.Next = node1;

            string xaml = XamlTestDriver.RoundTripCompare(node1);
        }

        [TestCase]
        public void ReferenceParent()
        {
            Microsoft.Test.Xaml.Types.InstanceReference.Node node1 = new Microsoft.Test.Xaml.Types.InstanceReference.Node()
                             {
                                 Bar = new SimpleRefBar()
                                           {
                                               IntProperty = 5,
                                               StringProperty = "hello"
                                           }
                             };
            node1.Next = node1;

            string xaml = XamlTestDriver.RoundTripCompare(node1);
        }

        [TestCase]
        public void NegativeNameScope()
        {
            UnScopedBar ubar = new UnScopedBar()
                                   {
                                       IntProperty = 5,
                                       StringProperty = "hello"
                                   };
            SingleScopedBar bar = new SingleScopedBar()
                                      {
                                          ubar = ubar
                                      };
            SingleScopedFoo foo = new SingleScopedFoo()
                                      {
                                          bar = bar,
                                          ubar = ubar
                                      };

            string xaml = @"<?xml version=""1.0"" encoding=""utf-16""?>
                            <xi:SingleScopedFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml""
                        xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
                              <xi:SingleScopedFoo.bar>
                                <xi:SingleScopedBar>
                                  <xi:SingleScopedBar.ubar>
                                    <xi:UnScopedBar x:Name=""__Reference_ID_0"" IntProperty=""5"" StringProperty=""hello"">
                                  <xi:UnScopedBar.bar4>
                                    <x:Null />
                                  </xi:UnScopedBar.bar4>
                                </xi:UnScopedBar>
                                  </xi:SingleScopedBar.ubar>
                                </xi:SingleScopedBar>
                              </xi:SingleScopedFoo.bar>
                              <xi:SingleScopedFoo.ubar>
                                <x:Reference>__Reference_ID_0</x:Reference>
                              </xi:SingleScopedFoo.ubar>
                            </xi:SingleScopedFoo>";

            try
            {
                SingleScopedFoo dfoo = (SingleScopedFoo)XamlTestDriver.Deserialize(xaml, null);
            }
            catch (XamlObjectWriterException)
            {
                Console.WriteLine("Expected exception caught");
            }
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void DuplicateNameInScope()
        {
            ImplicitNamingBar bar1 = new ImplicitNamingBar
                                         {
                                             MyImplicitName = "name1"
                                         };
            ImplicitNamingBar bar2 = new ImplicitNamingBar
                                         {
                                             MyImplicitName = "name1"
                                         };
            object root = new List<ImplicitNamingBar>
                              {
                                  bar1, bar2
                              };

            string msg = Exceptions.GetMessage("ObjectReaderXamlNamedElementAlreadyRegistered", WpfBinaries.SystemXaml);
            XamlTestDriver.RoundTripCompare(root, msg);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void RuntimeNameSameAsContentProperty()
        {
            Persona p = new Persona()
                            {
                                Name = "foobar"
                            };
            p.Friends = new List<Persona>()
                            {
                                p
                            };

            XamlTestDriver.RoundTripCompare(p);
            string xaml = XamlServices.Save(p);

            if (!xaml.Contains(@"<x:Reference>foobar</x:Reference>"))
            {
                throw new TestCaseFailedException("Runtime name property is invalid");
            }
        }



        //    string xaml = XamlServices.Save(node1);

        //    XamlObjectDeserializer ds = new XamlObjectDeserializer();

        //    XamlObjectDeserializerContext context = new XamlObjectDeserializerContext()
        //    {

        //    };

        //    object deserialized = ds.Load(new StringReader(xaml), context);

        //    XamlObjectComparer.CompareObjects(context.RootNameScopeForOutput.FindName("node1"), node1);
        //    INameScope scope = (INameScope)context.TypeDescriptorContextForOutput.GetService(typeof(INameScope));
        //    XamlObjectComparer.CompareObjects(scope.FindName("node1"), node1);

        //}

        //        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        //        public void RootNamescopeScope()
        //        {
        //            UnScopedBar ubar1 = new UnScopedBar() { IntProperty = 5, StringProperty = "hello" };
        //            UnScopedBar ubar2 = new UnScopedBar() { IntProperty = 10, StringProperty = "boho" };

        //            ScopedBar bar1 = new ScopedBar() { ubar = ubar1, ubar2 = ubar1 };
        //            ScopedBar bar2 = new ScopedBar() { ubar = ubar2, ubar2 = ubar2 };

        //            UnScopedFoo foo = new UnScopedFoo() { bar = bar1, bar2 = bar2, bar3 = null };

        //            string xaml = @"<?xml version=""1.0"" encoding=""utf-8""?>
        //                                <xi:UnScopedFoo xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" 
        //                        xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"" 
        //                        xmlns:xi=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes"">
        //                                  <xi:UnScopedFoo.bar>
        //                                    <xi:ScopedBar>
        //                                      <xi:ScopedBar.ubar>
        //                                        <xi:UnScopedBar x:Name=""myref"" IntProperty=""5"" StringProperty=""hello"">
        //                                          <xi:UnScopedBar.bar4>
        //                                            <x:Null />
        //                                          </xi:UnScopedBar.bar4>
        //                                        </xi:UnScopedBar>
        //                                      </xi:ScopedBar.ubar>
        //                                      <xi:ScopedBar.ubar2>
        //                                        <x2:Reference>myref</x2:Reference>
        //                                      </xi:ScopedBar.ubar2>
        //                                    </xi:ScopedBar>
        //                                  </xi:UnScopedFoo.bar>
        //                                  <xi:UnScopedFoo.bar2>
        //                                    <xi:ScopedBar>
        //                                      <xi:ScopedBar.ubar>
        //                                        <xi:UnScopedBar x:Name=""myref"" IntProperty=""10"" StringProperty=""boho"">
        //                                          <xi:UnScopedBar.bar4>
        //                                            <x:Null />
        //                                          </xi:UnScopedBar.bar4>
        //                                        </xi:UnScopedBar>
        //                                      </xi:ScopedBar.ubar>
        //                                      <xi:ScopedBar.ubar2>
        //                                        <x2:Reference>myref</x2:Reference>
        //                                      </xi:ScopedBar.ubar2>
        //                                    </xi:ScopedBar>
        //                                  </xi:UnScopedFoo.bar2>
        //                                  <xi:UnScopedFoo.bar3>
        //                                    <x:Null />
        //                                  </xi:UnScopedFoo.bar3>
        //                                      </xi:UnScopedFoo>";

        //            string fileName = XamlTestDriver.WriteToFile(xaml, "output.xaml");
        //            XamlObjectDeserializer deserializer = new XamlObjectDeserializer();
        //            XamlObjectDeserializerContext context = new XamlObjectDeserializerContext();
        //            var deserialized = deserializer.LoadFromUri(new Uri(fileName), context);

        //            var object1 = context.RootNameScopeForOutput.FindName("myref");

        //            if (object1 != null)
        //                throw new TestCaseFailedException("Root namescope contains nested namescope values");

        //            deserialized = deserializer.LoadFromUri(new Uri(fileName), new XamlReaderSettings(), context);

        //            object1 = context.RootNameScopeForOutput.FindName("myref");

        //            if (object1 != null)
        //                throw new TestCaseFailedException("Root namescope contains nested namescope values");

        //        }

        //        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        //        public void RootNamescopeScopeRootIsINameScope()
        //        {
        //            _scoped scoped = new _scoped();
        //            scoped.MyProperty = new object();
        //            scoped.MyProperty1 = scoped.MyProperty;

        //            string xaml = @"<_scoped xmlns=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes""
        //                                xmlns:p=""http://schemas.microsoft.com/netfx/2008/xaml/schema"" 
        //                                xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
        //                              <_scoped.MyProperty>
        //                                <p:Object x:Name=""myref"" />
        //                              </_scoped.MyProperty>
        //                              <_scoped.MyProperty1>
        //                                <x2:Reference>myref</x2:Reference>
        //                              </_scoped.MyProperty1>
        //                            </_scoped>";

        //            string fileName = XamlTestDriver.WriteToFile(xaml, "output.xaml");
        //            XamlObjectDeserializer deserializer = new XamlObjectDeserializer();
        //            XamlObjectDeserializerContext context = new XamlObjectDeserializerContext();
        //            var deserialized = deserializer.LoadFromUri(new Uri(fileName), context);

        //            var object1 = context.RootNameScopeForOutput.FindName("myref");

        //            if (object1 != null)
        //                throw new TestCaseFailedException("Root namescope contains nested namescope values");
        //        }

        // [DISABLED]
        // [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        
        public void RTNPStrip()
        {
            RTNPClass node = new RTNPClass()
                                 {
                                 };
            node.MySelf = node;

            string xaml = XamlServices.Save(node);

            XamlTestDriver.RoundTripCompareExamineXaml(node,
                                                new string[]
                                                    {
                                                        @"/xx:RTNPClass[@MySelf]",
                                                        @"/xx:RTNPClass[@x:Name]",
                                                    },
                                                namespaces);
        }

        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void MEInReference()
        {
            UnNamedNode node = new UnNamedNode()
                                   {
                                       Id = "hello"
                                   };
            node.Next = node;

            string xaml = XamlServices.Save(node);

            XamlTestDriver.RoundTripCompareExamineXaml(node,
                                                new string[]
                                                    {
                                                        @"/xx:UnNamedNode[@Next]",
                                                        @"/xx:UnNamedNode[@x:Name]",
                                                    },
                                                namespaces);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void RTNPWithForwardReference()
        {
            NamedClass named1 = new NamedClass()
                                    {
                                        Name = "foo",
                                        StringProperty = "Hello world"
                                    };

            List<NamedClass> list = new List<NamedClass>()
                                        {
                                            named1,
                                            named1,
                                        };

            string xaml = @"<List x:TypeArguments=""mtxti:NamedClass"" Capacity=""4"" 
                            xmlns=""clr-namespace:System.Collections.Generic;assembly=mscorlib"" 
                            xmlns:mtxti=""clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes""
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                            xmlns:x2=""http://schemas.microsoft.com/netfx/2008/xaml"">
                              <x:Reference>foo</x:Reference>
                              <mtxti:NamedClass Name=""foo"" StringProperty=""Hello world"" />
                            </List>";

            XamlTestDriver.XamlFirstCompareObjects(xaml, list);
        }

        // 
        [TestCase(Owner = "Microsoft", Category = TestCategory.IDW)]
        public void NestedNameScopeTest()
        {
            UnscopedBar2 originalBar = new UnscopedBar2
                                           {
                                               StringProperty = "Some Value"
                                           };

            NestedScope original = new NestedScope
                                       {
                                           unscopedBar = originalBar,
                                           scope1 = new NestedScope
                                                        {
                                                            unscopedBar = originalBar
                                                        }
                                       };

            NodeList doc = new NodeList()
            {
                new StartObject(typeof(NestedScope)),
                    new StartMember(typeof(NestedScope), "unscopedBar"),
                        new StartObject(typeof(UnscopedBar2)),
                            new StartMember(typeof(UnscopedBar2), "StringProperty"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, true}}},
                                new ValueNode("Some Value"),
                            new EndMember(),
                            NodeListFactory.CreateXName("RefName", true),
                        new EndObject(),
                    new EndMember(),
                    new StartMember(typeof(NestedScope), "scope1"),
                        new StartObject(typeof(NestedScope)),
                            new StartMember(typeof(NestedScope), "unscopedBar"),
                                NodeListFactory.CreateReference("RefName", true),
                            new EndMember(),
                        new EndObject(),
                    new EndMember(),
                new EndObject(),
            };

            string xaml = doc.NodeListToXml();
            var scope = (NestedScope)XamlServices.Parse(xaml);

            if (scope.unscopedBar.GetHashCode() != scope.scope1.unscopedBar.GetHashCode())
            {
                throw new DataTestException("Reference equality not preserved");
            }

            XamlTestDriver.XamlFirstCompareObjects(xaml, original);
        }

        [TestCase]
        public void IxnrBackwardRefME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateIxnrDocument(
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject()
                },
                NodeListFactory.CreateReference("__Reference_ID_0", false),
                new NodeList()
                {
                    new StartObject(typeof(XamlNameResolverExtension)),
                        NodeListFactory.CreateMember(typeof(XamlNameResolverExtension), "Data", "null", true),
                    new EndObject(),
                },
                typeof(MultipleRefFoo));

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrForwardRefSimpleME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("null");
            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrForwardRefRecallWithSelfME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("this");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrForwardRefInvalidCallbackME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("other");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        // [DISABLED]
        // [TestCase]
        public void IxnrForwardRefSimpleNoNamesME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("nullnonames");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        // [DISABLED]
        // [TestCase]
        
        public void IxnrForwardRefSimpleNullNamesME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("nullnullnames");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrForwardRefRecallNoNamesME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("thisnonames");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        // [DISABLED]
        // [TestCase]
        public void IxnrForwardRefRecallNullNamesME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("thisnullnames");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrForwardRefRecallDiffMeME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("differentME");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        // [DISABLED]
        // [TestCase]        
        public void IxnrForwardRefRecallDiffTcME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("differentTC");

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrMultiplePresentME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefBar bar2 = new MultipleRefBar()
                                      {
                                          IntProperty = 16,
                                          StringProperty = "Hello world2"
                                      };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar2,
                                         barBackward = bar2,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateIxnrDocument(
                new NodeList()
                { 
                    new StartObject((typeof(MultipleRefBar))),
                        NodeListFactory.CreateXName("__Reference_ID_1", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                new NodeList()
                {
                    new StartObject(typeof(XamlNameResolverExtension)),
                        NodeListFactory.CreateMember(typeof(XamlNameResolverExtension), "Data", "multiplePresent", true),
                    new EndObject(),
                },
                new NodeList()
                {
                    new StartObject((typeof(MultipleRefBar))),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 16, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world2", true),
                    new EndObject(),
                },
                typeof(MultipleRefFoo));

            XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo);
        }

        [TestCase]
        public void IxnrMultipleNotPresentME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefBar bar2 = new MultipleRefBar()
                                      {
                                          IntProperty = 16,
                                          StringProperty = "Hello world2"
                                      };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar2,
                                         barBackward = bar2,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateIxnrDocument(
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_1", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                new NodeList()
                {
                    new StartObject(typeof(XamlNameResolverExtension)),
                        NodeListFactory.CreateMember(typeof(XamlNameResolverExtension), "Data", "multipleNotPresent", true),
                    new EndObject(),
                },
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 16, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world2", true),
                    new EndObject(),
                },
                typeof(MultipleRefFoo));

            ExceptionHelpers.CheckForException(
                typeof(XamlObjectWriterException),
                () => XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo));
        }

        [TestCase]
        public void IxnrForwardRequestDiffME()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFoo foo = new MultipleRefFoo()
                                     {
                                         bar = bar,
                                         barBackward = bar,
                                         barForward = bar
                                     };

            NodeList xamlDoc = CreateForwardRefME("requestDifferentRef");

            ExceptionHelpers.CheckForException(
                typeof(XamlObjectWriterException),
                () => XamlTestDriver.XamlFirstCompareObjects(xamlDoc, foo));
        }

        [TestCase]
        public void IxnrBackwardRefTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };
            NodeList xamlDoc = CreateIxnrDocument(
               new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
               NodeListFactory.CreateReference("__Reference_ID_0", false),
               new NodeList()
               {
                   new ValueNode("null"),
               },
               typeof(MultipleRefFooWTypeConverter));


            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRefSimpleTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("null");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRefRecallWithSelfTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("this");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRefInvalidCallbackTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("other");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        // [DISABLED]
        // [TestCase]
        public void IxnrForwardRefSimpleNoNamesTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("nullnonames");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        // [DISABLED]
        // [TestCase]
        public void IxnrForwardRefSimpleNullNamesTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("nullnullnames");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRefRecallNoNamesTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("thisnonames");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        // [DISABLED]
        // [TestCase]
        public void IxnrForwardRefRecallNullNamesTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("thisnullnames");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRefRecallDiffMeTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("differentME");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRefRecallDiffTcTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("differentTC");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrForwardRequestDiffTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateForwardRefTC("requestDifferentRef");

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrMultiplePresentTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefBar bar2 = new MultipleRefBar()
                                      {
                                          IntProperty = 16,
                                          StringProperty = "Hello world2"
                                      };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar2,
                                                       barBackward = bar2,
                                                       barForward = bar
                                                   };
            NodeList xamlDoc = CreateIxnrDocument(
              new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_1", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                new NodeList()
                {
                   new ValueNode("multiplePresent"),
                },
                new NodeList()
                {
                  new StartObject(typeof(MultipleRefBar)),
                     NodeListFactory.CreateXName("__Reference_ID_0", true),
                     NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 16, true),
                     NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world2", true),
                  new EndObject(),
                },
              typeof(MultipleRefFooWTypeConverter));

            string xaml = xamlDoc.NodeListToXml();
            XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null);
        }

        [TestCase]
        public void IxnrMultipleNotPresentTC()
        {
            MultipleRefBar bar = new MultipleRefBar()
                                     {
                                         IntProperty = 15,
                                         StringProperty = "Hello world"
                                     };
            MultipleRefBar bar2 = new MultipleRefBar()
                                      {
                                          IntProperty = 16,
                                          StringProperty = "Hello world2"
                                      };
            MultipleRefFooWTypeConverter foo = new MultipleRefFooWTypeConverter()
                                                   {
                                                       bar = bar,
                                                       barBackward = bar2,
                                                       barForward = bar
                                                   };

            NodeList xamlDoc = CreateIxnrDocument(
              new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_1", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                new NodeList()
                {
                   new ValueNode("multipleNotPresent"),
                },
                new NodeList()
                {
                  new StartObject(typeof(MultipleRefBar)),
                     NodeListFactory.CreateXName("__Reference_ID_0", true),
                     NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 16, true),
                     NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world2", true),
                  new EndObject(),
                },
              typeof(MultipleRefFooWTypeConverter));

            string xaml = xamlDoc.NodeListToXml();
            ExceptionHelpers.CheckForException(
                typeof(XamlObjectWriterException),
                () => XamlTestDriver.XamlFirstCompareObjects(xaml, foo, (INameScope)null));
        }

        /// <summary>
        /// Test objects in external name scope are hooked up properly and don't interact with root namescope
        /// </summary>
        [TestCase]
        public void ExternalNameScope()
        {
            NodeList xamlDoc = new NodeList
            {
                new StartObject(typeof(SimpleRefFoo)),
                    new StartMember(typeof(SimpleRefFoo), "bar"),
                        NodeListFactory.CreateReference("bar", false),
                    new EndMember(),
                    new StartMember(typeof(SimpleRefFoo), "bar2"),
                        NodeListFactory.CreateReference("bar2", false),
                    new EndMember(),
                new EndObject()                    
            };

            NameScopeImpl scope = new NameScopeImpl();
            var bar = new SimpleRefBar();
            var bar2 = new SimpleRefBar();
            scope.RegisterName("bar", bar);
            scope.RegisterName("bar2", bar2);

            var xows = new XamlObjectWriterSettings { ExternalNameScope = scope };
            var xxr = new XamlXmlReader(new StringReader(xamlDoc.NodeListToXml()));
            var xow = new XamlObjectWriter(xxr.SchemaContext, xows);

            XamlServices.Transform(xxr, xow);

            Assert.AreEqual(xow.RootNameScope.FindName("bar"), null);
            Assert.AreEqual(xow.RootNameScope.FindName("bar2"), null);
        }

        private NodeList CreateForwardRefME(string meValue)
        {
            return CreateIxnrDocument(
                NodeListFactory.CreateReference("__Reference_ID_0", false),
                new NodeList()
                {
                    new StartObject(typeof(XamlNameResolverExtension)),
                         NodeListFactory.CreateMember(typeof(XamlNameResolverExtension), "Data", meValue, true),
                    new EndObject(),
                },
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                typeof(MultipleRefFoo));
        }

        private NodeList CreateForwardRefTC(string tcValue)
        {
            return CreateIxnrDocument(
                NodeListFactory.CreateReference("__Reference_ID_0", false),
                new NodeList()
                {       
                    new ValueNode(tcValue),
                },
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                typeof(MultipleRefFooWTypeConverter));
        }

        private NodeList CreateIxnrDocument(NodeList barForwardChild, NodeList barChild, NodeList barBackwardChild, Type type)
        {
            return new NodeList()
            {
                new StartObject(type),
                    new StartMember(type, "barForward"),
                        barForwardChild,
                    new EndMember(),
                    new StartMember(type, "bar"),
                        barChild,
                    new EndMember(),
                    new StartMember(type, "barBackward"),
                        barBackwardChild,
                    new EndMember(),
                new EndObject(),
            };
        }

        private void NameScopePropertyAttributeTestBase(Type type, Func<object, INameScope> getNameScope)
        {
            NodeList xamlDoc = CreateIxnrDocument(
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_0", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
                new NodeList()
                {
                    new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_1", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                    new EndObject(),
                },
               new NodeList()
               {
                   new StartObject(typeof(MultipleRefBar)),
                        NodeListFactory.CreateXName("__Reference_ID_2", true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "IntProperty", 15, true),
                        NodeListFactory.CreateMember(typeof(MultipleRefBar), "StringProperty", "Hello world", true),
                   new EndObject(),
               },
               type);

            string xaml = xamlDoc.NodeListToXml();

            var instance = XamlTestDriver.Deserialize(xaml, null);
            INameScope scope = getNameScope(instance);
            if (scope == null)
            {
                throw new DataTestException("Namescope is null.");
            }

            foreach (string name in new string[]
                                        {
                                            "__Reference_ID_0", "__Reference_ID_1", "__Reference_ID_2"
                                        })
            {
                if (scope.FindName(name) == null)
                {
                    throw new DataTestException("Unable to find refernce for " + name);
                }
            }
        }

        [TestCase]
        public void NameScopePropertyAttrRefersInstanceMemberTest()
        {
            NameScopePropertyAttributeTestBase(typeof(NameScopePropOnType), (instance) => NameScopePropOnType.GetNameScope(instance));
        }

        [TestCase]
        public void CustomAttachableNameScopePropertyAttrTest()
        {
            NameScopePropertyAttributeTestBase(typeof(CustomAttachedNameScope), (instance) => CustomAttachableNameScope.GetNameScopeProp(instance));
        }

        [TestCase]
        public void ReferenceDictionaryKey()
        {
            ReferenceDictionaryKey(new object());
        }

        [TestCase]
        public void ReferenceNullDictionaryKey()
        {
            ExceptionHelpers.CheckForException(typeof(ArgumentNullException),
                () => ReferenceDictionaryKey(null));
        }

        private static void ReferenceDictionaryKey(object key)
        {
            var item = new TypeWithDictionaryKeyProperty()
            {
                ItemProperty = new object(),
                KeyProperty = key,
            };

            var root = new Container()
            {
                Content = new Dictionary<object, object>() 
                {
                    { key, item }
                },
                SecondContent = key,
            };

            string xaml = XamlTestDriver.RoundTripCompare(root);
            if (xaml.Contains("x:Key"))
            {
                throw new TestCaseFailedException("Key expected to be implicit but x:Key was written out");
            }
        }

        /// <summary>
        /// Add to a dictionary where the hash depends on a
        /// deferred property of the key (key is referenced and
        /// hash code is that of the key).
        /// </summary>
        [TestCase]
        public void ReferenceKeyWithHash()
        {
            var key = new object();
            var item = new ItemWithKeyPropertyAndHash()
            {
                ItemProperty = new object(),
                KeyProperty = key,
            };

            var root = new Container()
            {
                Content = new Dictionary<object, object>() 
                {
                    { key, item }
                },
                SecondContent = key,
            };

            string xaml = XamlTestDriver.RoundTripCompare(root);
            if (xaml.Contains("x:Key"))
            {
                throw new TestCaseFailedException("Key expected to be implicit but x:Key was written out");
            }
        }

        /// <summary>
        /// Add to a collection that validates based on
        /// a deferred property of the item. (key is a reference)
        /// </summary>
        [TestCase]
        public void AddToValidatingCollection()
        {
            var key = new object();
            var collection = new ValidatingCollection()
            {
                new TypeWithDictionaryKeyProperty()
                {
                    ItemProperty = new object(),
                    KeyProperty = key,
                }
            };

            var container = new Container()
            {
                Content = key,
                SecondContent = collection,
            };
            XamlTestDriver.RoundTripCompare(container);
        }

        [TestCase]
        public void ForwardRefToForwardRefTest()
        {
            string xaml = @"<scg:List xmlns='clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes' xmlns:scg='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:TypeArguments='IntComposite'>
                              <IntComposite>( 42 ( 3 4 5 ) ref2 )</IntComposite>
                              <IntComposite x:Name='ref2'>( ref3 ( ( 43 44 45 ) ) )</IntComposite>
                              <IntComposite x:Name='ref3'>( -1 -2 -3 )</IntComposite>
                            </scg:List>";

            var x = (List<IntComposite>)XamlServices.Parse(xaml);
            string expected = "( 42 ( 3 4 5 ) ( ( -1 -2 -3 ) ( ( 43 44 45 ) ) ) )";
            if (x[0].ToString() != expected)
            {
                Tracer.LogTrace("Expected: " + expected + "Actual: " + x[0].ToString());
                throw new Exception("Result did not match expected.");
            }
        }

        [TestCase]
        public void BackwardRefToForwardRefTest()
        {
            string xaml = @"<scg:List xmlns='clr-namespace:Microsoft.Test.Xaml.Types.InstanceReference;assembly=XamlClrTypes' xmlns:scg='clr-namespace:System.Collections.Generic;assembly=mscorlib' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' x:TypeArguments='IntComposite'>
                              <IntComposite x:Name='ref1'>( 42 ( 3 4 5 ) ref3 )</IntComposite>
                              <IntComposite>( ref1 ( ( 43 44 45 ) ) )</IntComposite>
                              <IntComposite x:Name='ref3'>( -1 -2 -3 )</IntComposite>
                            </scg:List>";

            var x = (List<IntComposite>)XamlServices.Parse(xaml);
            string expected = "( ( 42 ( 3 4 5 ) ( -1 -2 -3 ) ) ( ( 43 44 45 ) ) )";
            if (x[1].ToString() != expected)
            {
                Tracer.LogTrace("Expected: " + expected + "Actual: " + x[1].ToString());
                throw new Exception("Result did not match expected.");
            }
        }

        [TestCase]
        public void MultipleCircularReferencesInCollection()
        {
            var node1 = new SimpleNode();
            var node2 = new SimpleNode { Next = node1 };
            var node3 = new SimpleNode();
            var node4 = new SimpleNode { Next = node3 };
            var node5 = new SimpleNode { Next = node4 };
            node3.Next = node5;
            node1.Next = node2;

            var nodes = new List<SimpleNode>
            {
                new SimpleNode { Next = new SimpleNode { Next = node1 } },
                node1,
                node5,
                new SimpleNode { Next = node4 }
            };

            XamlTestDriver.RoundTripCompare(nodes);
        }
    }
}
