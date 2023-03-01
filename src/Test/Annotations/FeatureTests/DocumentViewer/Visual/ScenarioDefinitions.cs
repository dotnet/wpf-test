// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Single file containing all Scenario definitions.  Shared between BVT and Pri1s.

using Annotations.Test;
using Annotations.Test.Framework;
using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Threading;
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
    /// <summary>
    /// Class that can be used to define the setup operations of a Scenario.
    /// </summary>
    public class ScenarioSetup
    {
        public int NumPageDown = 0;
        public int NumScrollUp = 0;
        public int NumScrollDown = 0;

        public object[] SelectionData = null;

        public int[] ExpectedVisiblePages = new int[0];
        public int ExpectedVisiblePage
        {
            set { ExpectedVisiblePages = new int[] { value }; }
        }
    }

    /// <summary>
	/// Base class for all Scenario based test cases.
	/// </summary>
	public abstract class AScenario
	{
		public abstract void AppendScript(ref AsyncTestScript script);

		// Use if you want to verify result with VScan.
		public string VScanModel = string.Empty;
	}

	#region Scenario1

	/// <summary>
	/// ----------------------------------------------------------------------------------------------------------
	/// Scenario 1: Verify annotation location.
	/// Actions										Verify
	/// 1. Pages A to B are visible. (A may == B)   X1 Annotations are loaded and X2 annotations are visible. Verify visuals with VScan.
	/// ----------------------------------------------------------------------------------------------------------
	/// </summary>
    public class Scenario1 : AScenario
	{
		public override void AppendScript(ref AsyncTestScript script)
		{
			script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors.Length, ExpectedAttachedAnchors });			
		}

		// Use if you want to verify multiple AttachedAnnotations.
		public string[] ExpectedAttachedAnchors = new string[0];

		// Use if you want to verify a single AttachedAnnotation.
		public string ExpectedAttachedAnchor
		{
			set
			{
				ExpectedAttachedAnchors = new string[] { value };
			}
		}
	}

	#endregion Scenario1

	#region Scenario2

    public abstract class Scenario2 : AScenario
	{
		protected int Direction = 0;

		public override void AppendScript(ref AsyncTestScript script)
		{
			script.Add("VerifyPagesAreVisible", new object[] { VisiblePages });
			script.Add("VerifyAnnotations", new object[] { InitialNumAttachedAnnotations, InitialAttachedAnchors });
			
			// Page up or down based on how the 'Direction' is set by subclass.
			//
			if (Direction > 0)
				script.Add("PageUp", new object[] { NumPageChanges });
			else
				script.Add("PageDown", new object[] { NumPageChanges });

			script.Add("VerifyPagesAreNotVisible", new object[] { VisiblePages });
			script.Add("VerifyAnnotations", new object[] { ExpectedNumAttachedAnnotations, ExpectedAttachedAnchors });
			
			// Return to original page(s).
			//
			if (VisiblePages.Length == 1)
				script.Add("GoToPage", new object[] { VisiblePages[0] });
			else
				script.Add("GoToPageRange", new object[] { VisiblePages[0], VisiblePages[1] });
			
			script.Add("VerifyPagesAreVisible", new object[] { VisiblePages });
			script.Add("VerifyAnnotations", new object[] { InitialNumAttachedAnnotations, InitialAttachedAnchors });
		}	

		public int VisiblePage
		{
			set
			{
				VisiblePages = new int[] { value };
			}
		}

		private int[] _visiblePages;
		public int[] VisiblePages
		{
			set
			{
				if (value.Length > 2)
					throw new ArgumentException("Must specify 1 or 2 visible pages.");
				_visiblePages = value;
			}
			get
			{
				return _visiblePages;
			}
		}

		public string InitialAttachedAnchor
		{
			set
			{
				InitialNumAttachedAnnotations = 1;
				InitialAttachedAnchors = new string[] { value };
			}
		}

		public int NumScrolls = 0;
		public int NumPageChanges = 1;

		public int InitialNumAttachedAnnotations;
		public string[] InitialAttachedAnchors;

		public int ExpectedNumAttachedAnnotations;
		public string[] ExpectedAttachedAnchors = new string[0];
	}

	/// <summary>
	/// ---------------------------------------------------------------------------------------------------------- 
	/// Scenario2a: Verify page up load/unload
	/// 
	/// Action											Verify
	/// 1. Pages A to B are visible. (A may == B)		X1 Annotations are loaded and X2 annotations are visible.
	/// 2. Page up twice* (Pages A-1 to B-1 visible)		Y1 annotations are loaded and Y2 annotations are visible.
	/// 3. Page down twice* (pages A to B visible)		X1 Annotations are loaded and X2 annotations are visible.
	/// ----------------------------------------------------------------------------------------------------------
	/// * Page up/down twice because DocumentViewer keeps 2 pages as visible for performance reasons.
	/// </summary>
    public class Scenario2a : Scenario2
	{
		public Scenario2a()
		{
			Direction = 1; // PageUp.
		}
	}

	/// <summary>
	/// ----------------------------------------------------------------------------------------------------------
	/// Scenario2b: Verify page down load/unload
	/// 
	/// Action											Verify
	/// 1. Pages A to B are visible. (A may == B		X1 Annotations are loaded and X2 annotations are visible.
	/// 2. Page down twice* (Pages A-1 to B-1 visible)	Y1 annotations are loaded and Y2 annotations are visible.
	/// 3. Page up twice* (pages A to B visible)		X1 Annotations are loaded and X2 annotations are visible.
	/// ----------------------------------------------------------------------------------------------------------
	/// * Page up/down twice because DocumentViewer keeps 2 pages as visible for performance reasons.
	/// </summary>
    public class Scenario2b : Scenario2
	{
		public Scenario2b()
		{
			Direction = -1; // PageDown.
		}
	}

	#endregion Scenario2

	#region Scenario3

    public abstract class Scenario3 : AScenario
	{
        protected AScenarioSuite _scenarioSuite;
        public Scenario3(AScenarioSuite suite)
        {
            _scenarioSuite = suite;
        }

        protected abstract void ResetContent(ref AsyncTestScript script);

        public override void AppendScript(ref AsyncTestScript script)
		{
            script.Add("GoToPageRange", new object[] { FirstVisiblePage, LastVisiblePage });
            for (int i=0; i < Selections.Length; i++)
                script.Add("CreateAnnotation", new object[] { Selections[i] });
			script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors.Length, ExpectedAttachedAnchors });
			ResetContent(ref script);
			if (ViewAsTwoPages)
			{
				script.Add("ViewAsTwoPages");
			}
			else
			{
				script.Add("SetZoom", new object[] { 100 });
				script.Add("WholePageLayout");
			}
			script.Add("GoToPageRange", new object[] { FirstVisiblePage, LastVisiblePage });
			script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors.Length, ExpectedAttachedAnchors });
		}

        public ISelectionData[] Selections;

		private string[] _anchors = null;
		private string[] ExpectedAttachedAnchors
		{
			get
			{
				if (_anchors == null)
				{
					_anchors = new string[Selections.Length];
					for (int i = 0; i < Selections.Length; i++)
					{
						_anchors[i] = _scenarioSuite.GetText(Selections[i]);
					}
				}
				return _anchors;
			}
		}

		public int FirstVisiblePage = 0;
        public int LastVisiblePage = 0;
		public bool ViewAsTwoPages = false;

		/// <summary>
		/// Zoom percentage to set DV after window is reloaded (default=100%).
		/// </summary>
		public double ZoomPercentage = 100.0;
	}

	/// <summary>
	/// ----------------------------------------------------------------------------------------------------------
	/// Scenario3: Verify closing and re-creating DocumentViewer
	/// ----------------------------------------------------------------------------------------------------------
	/// Action																Verify
	/// 1. Pages A to B are visible. (A may == B)							X1 Annotations are loaded and X2 annotations are visible.
	/// 2. Close window.	
	/// 3. Re-create window with same annotation stream and zoom levels.	X1 Annotations are loaded and X2 annotations are visible.
	/// ----------------------------------------------------------------------------------------------------------
	/// </summary>
    public class Scenario3a : Scenario3
	{
        public Scenario3a(AScenarioSuite suite) : base(suite)
        { }
		protected override void ResetContent(ref AsyncTestScript script)
		{			
			script.Add("CloseWindow");
			script.Add("SetupTestWindow");
			script.Add("SetDocumentViewerContent");			
		}
	}

	/// <summary>
	/// ----------------------------------------------------------------------------------------------------------
	/// Scenario4: Verify resetting DocumentViewer.Content
	/// ----------------------------------------------------------------------------------------------------------
	/// Action														Verify
	/// 1. Pages A to B of DocA are visible.						X1 Annotations are loaded and X2 annotations are visible.
	/// 2. Change DV content to DocB.								0 annotations.
	/// 3. Change DV content to DocA.  Pages A to B are visible.	X1 Annotations are loaded and X2 annotations are visible.
	/// ----------------------------------------------------------------------------------------------------------
	/// </summary>
    public class Scenario3b : Scenario3
	{
        public Scenario3b(AScenarioSuite suite) : base(suite)
        { }
		protected override void ResetContent(ref AsyncTestScript script)
		{
			script.Add("ClearDocumentViewerContent");
			script.Add("VerifyAnnotations", new object[] { 0, new string[0] });
			script.Add("SetDocumentViewerContent");
		}
	}

	#endregion

	#region Scenario4

    /// <summary>
	/// ----------------------------------------------------------------------------------------------------------
	/// Scenario4: Verify resize and zoom after selection but before annotating.
	/// ----------------------------------------------------------------------------------------------------------
	/// Action								Verify
	/// 1. Pages A to B are visible.	
	/// 2. Make a selection.	
	/// 3. Resize window.	
	/// 4. Zoom 200%	
	/// 5. Zoom 50%	
	/// 6. Create annotation.				Verify that it is in the correct location and anchored to the original selection.
	/// ----------------------------------------------------------------------------------------------------------
	/// </summary>
    public class Scenario4 : AScenario
	{
        public Scenario4(ADocumentViewerBaseWrapper dvWrapper)
		{
			_dvWrapper = dvWrapper;
		}

		public override void AppendScript(ref AsyncTestScript script)
		{
            script.Add("GoToPageRange", new object[] { FirstVisiblePage, LastVisiblePage });
            script.Add(SelectionData, "SetSelection", new object[] { _dvWrapper.SelectionModule } );
			script.Add("ResizeWindow", new object[] { ResizedWindowSize });
			script.Add("SetZoom", new object[] { ZoomInPercentage });
			script.Add("SetZoom", new object[] { ZoomOutPercentage });
            script.Add("CreateAnnotation", new object[] { true });
            script.Add("GoToSelection");
			if (ViewPreviousPage)
			{
				if (_dvWrapper is FlowDocumentPageViewerWrapper)
					script.Add("PageUp");
				else 
					script.Add("ScrollToPreviousPage");
			}
            script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors.Length, ExpectedAttachedAnchors });
		}

        public int FirstVisiblePage = 0;
        public int LastVisiblePage = 0;

        public bool ViewPreviousPage = false;

        public ISelectionData SelectionData = null;

        public Size ResizedWindowSize = new Size(200, 300);
		public double ZoomInPercentage = 200.0;
		public double ZoomOutPercentage = 50.0;

		// Use if you want to verify multiple AttachedAnnotations.
		public string[] ExpectedAttachedAnchors;

		// Use if you want to verify a single AttachedAnnotation.
		public string ExpectedAttachedAnchor
		{
			set
			{
				ExpectedAttachedAnchors = new string[] { value };
			}
		}

		private ADocumentViewerBaseWrapper _dvWrapper;
	}

	#endregion Scenario4

    #region Scenario5

    /// <summary>
    /// Scenario5: Verify annotation after scrolling.
    /// Action	                                Verify
    /// 1. Pages A to B are visible.	        X1 Annotations are attached.
    /// 2. Scroll down.	                        X1 Annotations still attached and are visually correct.
    /// 3. Scroll up.	                        X1 Annotations still attached and are visually correct.
    /// </summary>
    public class Scenario5 : AScenario
    {
        public override void AppendScript(ref AsyncTestScript script)
        {
            script.Add("GoToPageRange", new object[] { FirstVisiblePage, LastVisiblePage });
            script.Add("CreateAnnotation", SelectionData );
            script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors });
            script.Add("ScrollDown", new object[] { NumScrollsDown });
            script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors });
            script.Add("ScrollUp", new object[] { NumScrollsUp });
            script.Add("VerifyAnnotations", new object[] { ExpectedAttachedAnchors });
        }

        public int FirstVisiblePage = 0;
        public int LastVisiblePage = 0;

        public object[] SelectionData;

        public int NumScrollsDown = 10;
        public int NumScrollsUp = 7;

        // Use if you want to verify multiple AttachedAnnotations.
        public string[] ExpectedAttachedAnchors;

        // Use if you want to verify a single AttachedAnnotation.
        public string ExpectedAttachedAnchor
        {
            set
            {
                ExpectedAttachedAnchors = new string[] { value };
            }
        }
    }

    #endregion
}	

