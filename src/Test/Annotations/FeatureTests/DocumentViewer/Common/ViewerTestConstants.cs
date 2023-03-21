// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Place to put all constants used by DocumentViewer tests.

using System;
using System.Windows;

namespace Avalon.Test.Annotations
{
	public class ViewerTestConstants
	{  
        #region Fields

        public static string SimpleFixedContent = "fixed_simple.xaml";
        public static string FixedWithEmptyPage = "fixed_empty.xaml";
        public static string DocumentSequence = "DocumentSequence.xaml";

        public static string SimpleFlowContent = "simple_flow.xaml";
        public static string ComplexFlowContent = "complex_flow.xaml";
        public static string DrtFlowContent = "Flow_DRT.xaml";
        public static string NestedViewer_Simple = "Flow_SimpleNestedViewer.xaml";

        public class TableTests
        {
            public static string Filename = "Flow_Tables.xaml";
            public static string SimpleTableName = "SimpleTable";
            public static string SpanningTableName = "SpanningTable";
            public static string OuterNestedTableName = "OuterTable";
            public static string InnerNestedTablename = "InnerTable";
            public static string EmbeddedTableName = "EmbeddedTable";
        }

        public class AnchoredBlockTests
        {
            public static string Filename = "Flow_Figures.xaml";
            public static string Figure1 = "Figure1";
            public static string Floater1 = "Floater1";
            public static string FigureImage = "FigureImage";
            public static string FloaterImage = "FloaterImage";
        }

        #endregion
    }
}	

