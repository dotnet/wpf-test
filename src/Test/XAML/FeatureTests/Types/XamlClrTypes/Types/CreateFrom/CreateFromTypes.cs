// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Test.Xaml.Types.CreateFrom
{
    public class Foo
    {
        public Bar Bar { get; set; }
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    public class Bar
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    #region Nested Create From

    public class NestedRoot
    {
        public string StringProperty { get; set; }
        public Nest1 Nest1 { get; set; }
    }

    public class Nest1
    {
        public string StringProperty { get; set; }
        public Nest2 Nest2 { get; set; }
    }

    public class Nest2
    {
        public string StringProperty { get; set; }
    }

    #endregion

    #region Multiple CreateFroms

    public class MultipleRoot
    {
        public Child1 Child1 { get; set; }
        public Child2 Child2 { get; set; }
    }

    public class Child1
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    public class Child2
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    #endregion

    #region circular

    public class CircleChild1
    {
        public CircleChild2 Child2 { get; set; }
    }

    public class CircleChild2
    {
        public CircleChild1 Child1 { get; set; }
    }

    #endregion

    #region CreateFrom is like Namescope

    public class NameRoot
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public MyFoo Foo { get; set; }
    }

    public class Node
    {
        public MyFoo Foo1 { get; set; }
        public MyFoo Foo2 { get; set; }
    }

    public class MyFoo
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    #endregion

    #region derived types

    public class Base
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    public class Derived : Base
    {
        public int IntProperty2 { get; set; }
        public string StringProperty2 { get; set; }
    }

    public class NotDerived
    {
        public int IntProperty2 { get; set; }
        public string StringProperty2 { get; set; }
    }

    #endregion

    public class MyNode
    {
        public string Name { get; set; }
        public MyNode NextNode { get; set; }
    }
}
