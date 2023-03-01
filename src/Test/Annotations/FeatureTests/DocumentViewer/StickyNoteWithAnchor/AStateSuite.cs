// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: TestSuite for verifying the state of a StickyNoteWithAnchor, can run
//  in two modes: Maximized, Minimized.

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Collections;
using System.Collections.Generic;

namespace Avalon.Test.Annotations
{
	public enum StickyNoteMode
	{
		Minimized,
		Maximized
	}

    public abstract class AStateSuite : AStickyNoteWithAnchorSuite
    {
		public override void ProcessArgs(string[] args)
		{
			base.ProcessArgs(args);
			foreach (string arg in args)
			{
				if (arg.ToLower().Equals("minimized"))
					_stickynoteState = StickyNoteMode.Minimized;
			}
		}

		/// <summary>
		/// Set zoom to 100% so that we are zoomed in for performing mouse operations on FixedDocuments.
		/// </summary>
		protected override void  DoExtendedSetup()
		{
			SetZoom(100);
		}

		/// <summary>
		/// Standard single annotation state test:
		/// 1. Create annotation on selection.
		/// 2. Set active selection.
		/// 3. Verify state.
		/// </summary>
		protected StickyNoteWrapper TestState(ISelectionData anchor, ISelectionData selection, StickyNoteState expectedState)
		{
			anchor.SetSelection(DocViewerWrapper.SelectionModule);
			StickyNoteWrapper wrapper = CreateStickyNoteWithAnchor();
			AssertNotNull("Verify SN exists.", wrapper);

			ViewerBase.Focus(); // Unfocus SN.

			selection.SetSelection(DocViewerWrapper.SelectionModule);			
			AssertEquals("Verify State", expectedState, wrapper.State);
			return wrapper;
		}

		/// <summary>
		/// Standard multiple annotation state test:
		/// 1. Create annotationA on selectionA.
		/// 2. Create annotationB on selectionB.
		/// 3. Set active selection.
		/// 4. Verify stateA.
		/// 5. Verify stateB.
		/// </summary>
		protected StickyNoteWrapper[] TestState(ISelectionData anchorA, ISelectionData anchorB, ISelectionData selection, StickyNoteState expectedStateA, StickyNoteState expectedStateB)
		{
			StickyNoteWrapper [] wrappers = new StickyNoteWrapper[2];
						
			wrappers[0] = CreateStickyNoteWithAnchor(anchorA, "A");
			wrappers[1] = CreateStickyNoteWithAnchor(anchorB, "B");

			ViewerBase.Focus(); // Unfocus SN.

			selection.SetSelection(DocViewerWrapper.SelectionModule);

			AssertEquals("Verify State A", expectedStateA, wrappers[0].State);
			AssertEquals("Verify State B", expectedStateB, wrappers[1].State);
			return wrappers;
		}

		/// <summary>
		/// Will set the maximized/minimized state of the StickyNote created based _stickynoteState variable.
		/// </summary>
		new protected StickyNoteWrapper CreateStickyNoteWithAnchor()
		{
			StickyNoteWrapper wrapper = base.CreateStickyNoteWithAnchor();
			if (_stickynoteState == StickyNoteMode.Minimized)
				wrapper.Expanded = false;
			return wrapper;
		}

		protected void VerifyStates(string statusMsg, StickyNoteWrapper[] wrappers, StickyNoteState[] states)
		{
			printStatus(statusMsg);
			AssertEquals("Verify number of wrappers matches number of states.", states.Length, wrappers.Length);
			for (int i = 0; i < wrappers.Length; i++)
				AssertEquals("Verify state " + i.ToString() + ".", states[i], wrappers[i].State);
		}

		#region Private Variables

		StickyNoteMode _stickynoteState = StickyNoteMode.Maximized;

		#endregion
	}
}	

