// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.XamlReaderWriterTests.XamlDocuments
{
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Xaml.Common.XamlOM;

    public class SampleDocuments
    {
        public static NodeList SampleDoc = new NodeList()
                                            {
                                                new StartObject(typeof(Point)),
                                                    new StartMember(typeof(Point), "X"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, true}} },
                                                        new ValueNode(42),
                                                    new EndMember(),
                                                    new StartMember(typeof(Point), "Y"){ TestMetadata = {{NodeListXmlWriter.WriteAsAttributeProperty, false}} },
                                                        new ValueNode(3),
                                                    new EndMember(),
                                                new EndObject(),
                                            };
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static Point CreatePoint()
        {
            return new Point
                       {
                           X = 42,
                           Y = 3
                       };
        }

        public static Point CreatePoint(int a)
        {
            return new Point
                       {
                           X = a,
                           Y = a
                       };
        }

        public static Point CreatePoint(int x, int y)
        {
            return new Point
                       {
                           X = x,
                           Y = y
                       };
        }

        public Point()
        {
            X = 42;
            Y = 3;
        }

        public Point(int a)
        {
            X = a;
            Y = a;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
