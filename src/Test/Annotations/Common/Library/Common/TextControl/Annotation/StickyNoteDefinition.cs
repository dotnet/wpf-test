// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Documents;
using Proxies.System.Windows.Annotations;
using Proxies.System.Windows.Controls;
using System.Collections.Generic;
using System.Reflection;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Object that encapsulates the procedure of creating a StickyNote.
	/// </summary>
	public class StickyNoteDefinition : AnnotationDefinition
	{
		public StickyNoteDefinition(ISelectionData anchor, AnnotationMode mode)
			: this(anchor, true, false, mode)
		{
			// nothing.
		}

		public StickyNoteDefinition(ISelectionData anchor, bool isExpanded)
            : this(anchor, isExpanded, false, AnnotationMode.StickyNote)
		{
			// nothing.
		}

		/// <param name="anchor"></param>
		/// <param name="isExpanded"></param>
		/// <param name="sendToBack">If true, then SN will be created and moved behind all existing SNs (opposite of normal behavior).</param>
		public StickyNoteDefinition(ISelectionData anchor, bool isExpanded, bool sendToBack)
			: this(anchor, isExpanded, sendToBack, AnnotationMode.StickyNote)
		{
			// nothing.
		}

        public StickyNoteDefinition(ISelectionData anchor, bool isExpanded, bool sendToBack, AnnotationMode mode)
			: base(anchor)
		{
			_isExpanded = isExpanded;
			_sendToBack = sendToBack;

            if (mode != AnnotationMode.StickyNote && mode != AnnotationMode.InkStickyNote)
                throw new ArgumentException("Mode '" + mode + "' is invalid for StickyNoteDefinition.");
            _mode = mode;
		}

        public StickyNoteDefinition(ISelectionData anchor, AnnotationMode mode, bool isExpanded, string author)
            : this(anchor, isExpanded, false, mode)
        {
            _author = author;
        }

        public override void Create(ATextControlWrapper target)
		{
			Create(target, false);
		}

        public override void Create(ATextControlWrapper target, bool goToSelection)
		{
			if (Anchor != null)
				Anchor.SetSelection(target.SelectionModule);
			if (goToSelection)
				target.BringIntoView(Anchor);

			if (_mode == AnnotationMode.StickyNote)
				AnnotationHelper.CreateTextStickyNoteForSelection(target.Service, _author);
			else
				AnnotationHelper.CreateInkStickyNoteForSelection(target.Service, _author);
			DispatcherHelper.DoEvents();

			//
			// Indirectly get the AnnotationComponent that was just created.
			// NOTE: this method assumes that AnnotationComponents are stored in the order that
			// they were created, and therefore, that the AC for the annotation just created is the
			// last one in the List.
			//

			AnnotationComponentFinder finder = new AnnotationComponentFinder(target.Target);
			IList<StickyNoteControl> snComponents = finder.GetStickyNoteComponents();
			if (snComponents.Count == 0)
				throw new InvalidOperationException("No StickyNotes exist in the ComponentManager.");

			StickyNoteControl stickynote = snComponents[snComponents.Count - 1];
            
			// Set expanded state.
            StickyNoteWrapper wrapper = new StickyNoteWrapper(stickynote, "note");
            if (!_isExpanded)
                wrapper.MinimizeWithMouse();
			
			// Set z-order.
			if (_sendToBack)
			{				
				Assembly PresentationFramework = typeof(FrameworkElement).Assembly;

				// ZOrder = 0;
				ReflectionHelper.SetProperty(stickynote.Delegate, "ZOrder", 0);				
				// PresentationContext.UpdateComponentZOrder(sn);
				object presentationContext = ReflectionHelper.GetProperty(stickynote.Delegate, "PresentationContext");
				ReflectionHelper.InvokeMethod(presentationContext, "UpdateComponentZOrder", new object[] { stickynote.Delegate });

				// Wait for zorder to be updated.
				DispatcherHelper.DoEvents();
			}
		}

        public override void Delete(ATextControlWrapper target)
		{
			Anchor.SetSelection(target.SelectionModule);
			AnnotationHelper.DeleteTextStickyNotesForSelection(target.Service);
			DispatcherHelper.DoEvents();
		}

		#region Fields

		private bool _isExpanded;
		private bool _sendToBack = false;
        private AnnotationMode _mode = AnnotationMode.StickyNote;
        private string _author = null;

		#endregion
	}
}	
