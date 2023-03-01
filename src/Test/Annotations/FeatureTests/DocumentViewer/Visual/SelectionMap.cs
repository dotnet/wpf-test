// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Threading;


using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;

using Annotations.Test;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Collections;
using Annotations.Test.Reflection;
using System.Windows.Resources;					// TestSuite.

namespace Avalon.Test.Annotations
{
	public enum SelectionType
	{
		// Document:
		StartOfDocument,		
		EndOfDocDocument,				
		WholeDocument,

        // Page 0:
        Page0_10_to_30,
        Page0_250_to_215,
        Page0_574_to_599,        
        Page0_805_to_825,
        Page0_1208,
        Page0_1200_to_1215,
		Page0_to_Page1,
		Page0_End,

        // Page 1:
		Page1_End,
        Page1_End_Long,
		Page1_19_to_42,
		Page1_20_to_25,
		Page1_20_to_27,
		Page1_20_to_30,
		Page1_25_to_30,
		Page1_25_to_37,
		Page1_25_to_40,
		Page1_20_to_37,
        Page1_20_to_40,
		Page1_30_to_35,
		Page1_30_to_40,
		Page1_35_to_40,
		Page1_38_to_40,
		Page1_90_to_120,
		Page1_90_to_140,        
        Page1_100_to_150,
		Page1_105_to_140,
		Page1_110_to_125,
		Page1_110_to_135,
		Page1_120_to_135,		
		Page1_2215_to_2305,
		Page1_3590_to_3602,
        Page1_End_Page2_Start,
		Page1_EndMinus50_to_EndMinus25,
		Page1_EndMinus25_to_End,
		
		// Page 2:
        Page2,
		Page2_Start,
        Page2_10_to_25,
		Page2_End,
		Page2_End_into_Page_3,
		Page2_End_Page3_Start,
        Page2_End_to_Page_3_30,
		Page2_1067_to_1261,

		// Page 3:
        Page3_Start,
        Page3_End,
		Page3_0_to_50,
		Page3_0_to_53,		
        Page3_8_to_30,
        Page3_250_to_300,

        // Page 4:
        Page4_Start,
		
		// Multi-page:
		Page1_to_Page2,
		Page1_to_Page3,
        Page2_to_Page3,
		Page3_To_Page4,			
	}

	/// <summary>
	/// Module that maps SelectionType to specific text selection in a Fixed or Flow document.
	/// </summary>
	public class SelectionMap 
	{
		static SelectionMap()
		{
			SetupSelectionMappings();
		}

		public static void SetupSelectionMappings()
		{
			SelectionType type;

            #region Page0

            type = SelectionType.Page0_10_to_30;
            AddSelectionPosition(type, new object[] { 0, 10, 20 });
            
            type = SelectionType.Page0_250_to_215;
            AddSelectionPosition(type, new object[] { 0, 250, 15 });
            
            type = SelectionType.Page0_574_to_599;
            AddSelectionPosition(type, new object[] { 0, 574, 25 });
            
            type = SelectionType.Page0_805_to_825;
            AddSelectionPosition(type, new object[] { 0, 805, 20 });
            
            type = SelectionType.Page0_1208;
            AddSelectionPosition(type, new object[] { 0, 1200, 0 });
            
            type = SelectionType.Page0_1200_to_1215;
            AddSelectionPosition(type, new object[] { 0, 1200, 15 });

			type = SelectionType.Page0_to_Page1;
			AddSelectionPosition(type, new object[] { 0, PagePosition.End, -100, 1, PagePosition.Beginning, 100 });

			type = SelectionType.Page0_End;
			AddSelectionPosition(type, new object[] { 0, PagePosition.End, -100 });

            #endregion

			#region Page1

			type = SelectionType.Page1_100_to_150;
			AddSelectionPosition(type, new object[] { 1, 100, 50 });
			
			type = SelectionType.Page1_110_to_125;
			AddSelectionPosition(type, new object[] { 1, 110, 10 });
			
			// Adjacent to the previous selection
			type = SelectionType.Page1_120_to_135;
			AddSelectionPosition(type, new object[] { 1, 120, 15 });
			
			// Merge of the previous two selections
			type = SelectionType.Page1_110_to_135;
			AddSelectionPosition(type, new object[] { 1, 110, 25 });

			type = SelectionType.Page1_20_to_25;
			AddSelectionPosition(type, new object[] { 1, 20, 5 });

			type = SelectionType.Page1_20_to_27;
			AddSelectionPosition(type, new object[] { 1, 20, 7 });

			type = SelectionType.Page1_20_to_30;
			AddSelectionPosition(type, new object[] { 1, 20, 10 });
			
			type = SelectionType.Page1_20_to_37;
			AddSelectionPosition(type, new object[] { 1, 20, 17 });
			
			type = SelectionType.Page1_20_to_40;
			AddSelectionPosition(type, new object[] { 1, 20, 20 });
			
			type = SelectionType.Page1_30_to_35;
			AddSelectionPosition(type, new object[] { 1, 30, 5 });
			
			type = SelectionType.Page1_30_to_40;
			AddSelectionPosition(type, new object[] { 1, 30, 10 });
			
			type = SelectionType.Page1_25_to_30;
			AddSelectionPosition(type, new object[] { 1, 25, 5 });
			
			type = SelectionType.Page1_25_to_37;
			AddSelectionPosition(type, new object[] { 1, 25, 12 });
			
            type = SelectionType.Page1_25_to_40;
			AddSelectionPosition(type, new object[] { 1, 25, 15 });

			type = SelectionType.Page1_35_to_40;
			AddSelectionPosition(type, new object[] { 1, 35, 5 });

			type = SelectionType.Page1_38_to_40;
			AddSelectionPosition(type, new object[] { 1, 37, 3 });

			// contains Page1_20_to_30 and Page1_35_to_40
			type = SelectionType.Page1_19_to_42;
			AddSelectionPosition(type, new object[] { 1, 19, 23 });
			
			type = SelectionType.Page1_90_to_120;
			AddSelectionPosition(type, new object[] { 1, 90, 30 });
			
			type = SelectionType.Page1_105_to_140;
			AddSelectionPosition(type, new object[] { 1, 105, 35 });
	
            // covers the previous two selections
			type = SelectionType.Page1_90_to_140;
			AddSelectionPosition(type, new object[] { 1, 90, 50 });

			type = SelectionType.Page1_End;
			AddSelectionPosition(type, new object[] { 1, PagePosition.End, -50 });

            type = SelectionType.Page1_End_Long;
            AddSelectionPosition(type, new object[] { 1, PagePosition.End, -1443 });

            type = SelectionType.Page1_2215_to_2305;
			AddSelectionPosition(type, new object[] { 1, 2215, 90 });

            type = SelectionType.Page1_3590_to_3602;
			AddSelectionPosition(type, new object[] { 1, 3590, 17 });

            /// Covers Page1_3590_to_3602 and Page2_10_to_25
            type = SelectionType.Page1_End_Page2_Start;
            AddSelectionPosition(type, new object[] { 1, PagePosition.End, -96, 2, PagePosition.Beginning, 100 });
			type = SelectionType.Page1_EndMinus50_to_EndMinus25;
			AddSelectionPosition(type, new object[] { 1, PagePosition.End, -50, 1, PagePosition.End, -25 });

			type = SelectionType.Page1_EndMinus25_to_End;
			AddSelectionPosition(type, new object[] { 1, PagePosition.End, -25 });

			#endregion Page1

			#region Page2

			type = SelectionType.Page2_Start;
			AddSelectionPosition(type, new object[] { 2, PagePosition.Beginning, 50 });

            type = SelectionType.Page2_10_to_25;
            AddSelectionPosition(type, new object[] { 2, 10, 15 });

			type = SelectionType.Page2_End;
			AddSelectionPosition(type, new object[] { 2, PagePosition.End, -52 });

			type = SelectionType.Page2_End_into_Page_3;
			AddSelectionPosition(type, new object[] { 2, PagePosition.End, 101 });

			/// Page2_End merged with Page2_End_into_Page3
			type = SelectionType.Page2_End_Page3_Start;
			AddSelectionPosition(type, new object[] { 2, PagePosition.End, -52, 3, PagePosition.Beginning, 100 });

            type = SelectionType.Page2_End_to_Page_3_30;
			AddSelectionPosition(type, new object[] { 2, PagePosition.End, -55, 3, PagePosition.Beginning, 30 });
			
			type = SelectionType.Page2_1067_to_1261;
            AddSelectionPosition(type, new object[] { 2, 1067, 193 });

            type = SelectionType.Page2;
            AddSelectionPosition(type, new object[] { 2, PagePosition.Beginning, 0, 2, PagePosition.End, 0 });

			#endregion Page2

			#region Page3

            type = SelectionType.Page3_Start;
            AddSelectionPosition(type, new object[] { 3, PagePosition.Beginning, 100 });

			type = SelectionType.Page3_0_to_50;
			AddSelectionPosition(type, new object[] { 3, PagePosition.Beginning, 50 });

			type = SelectionType.Page3_0_to_53;
			AddSelectionPosition(type, new object[] { 3, PagePosition.Beginning, 54 });
	
            type = SelectionType.Page3_8_to_30;
            AddSelectionPosition(type, new object[] { 3, 8, 22 });

            type = SelectionType.Page3_End;
            AddSelectionPosition(type, new object[] { 3, PagePosition.End, -33 });

            type = SelectionType.Page3_250_to_300;
            AddSelectionPosition(type, new object[] { 3, 250, 300 });

			#endregion Page3

            #region Page4

            type = SelectionType.Page4_Start;
            AddSelectionPosition(type, new object[] { 4, PagePosition.Beginning, 71 });

            #endregion Page4

            /*
             * Note: for fixed content when making multi-page selections "\r\n" is added for each page.  This is not a "valid" selection, therefore
			 * it should not affect the expected length of the selection.
             */ 
			#region Multi-Page

			type = SelectionType.Page1_to_Page2;
            AddSelectionPosition(type, new object[] { 1, PagePosition.End, -50, 2, PagePosition.Beginning, 50 });

            type = SelectionType.Page1_to_Page3;
			AddSelectionPosition(type, new object[] { 1, PagePosition.End, -50, 3, PagePosition.Beginning, 100 });

            type = SelectionType.Page2_to_Page3;
            AddSelectionPosition(type, new object[] { 2, PagePosition.End, -52, 3, PagePosition.Beginning, 100 });

            type = SelectionType.Page3_To_Page4;
            AddSelectionPosition(type, new object[] { 3, PagePosition.End, -33, 4, PagePosition.Beginning, 70 });

			#endregion Multi-Page

            #region Document

            type = SelectionType.StartOfDocument;
            AddSelectionPosition(type, new object[] { 0, PagePosition.Beginning, 20 });

            type = SelectionType.EndOfDocDocument;
            AddSelectionPosition(type, new object[] { 4, PagePosition.End, -50 });

            type = SelectionType.WholeDocument;
            AddSelectionPosition(type, new object[] { 0, PagePosition.Beginning, 0, 4, PagePosition.End, 0 });

            #endregion Document
        }

        public static object[] SelectionData(TestMode mode, SelectionType type)
        {
			return _selectionData[type];
		}

		public static FlowDocument FlowContent
		{
			get
			{
				return (FlowDocument)AnnotationTestHelper.LoadContent(_flowDocFile);
			}
		}

		public static FixedDocument EmptyFixedContent
		{
			get { return new FixedDocument(); }
		}

		public static FixedDocument FixedContent
		{
			get
			{
				return (FixedDocument)AnnotationTestHelper.LoadContent(_fixedDocFile); ;
			}
		}

		private static string _fixedDocFile = "fixed_simple.xaml";
		private static string _flowDocFile = "simple_flow.xaml";


		private static void AddSelectionPosition(SelectionType type, object[] selectionData)
		{
			_selectionData.Add(type, selectionData);
		}


		/// <summary>
		/// Selection definitions are the same for both fixed and flow. 
		/// </summary>
		static IDictionary<SelectionType, object[]> _selectionData = new Dictionary<SelectionType, object[]>();
	}
}	

