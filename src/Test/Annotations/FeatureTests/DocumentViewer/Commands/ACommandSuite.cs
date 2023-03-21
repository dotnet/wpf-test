// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;

using System.Windows.Input;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Controls;

namespace Avalon.Test.Annotations
{
    [TestDimension("fixed,flow")]
	public abstract class ACommandSuite : ADefaultContentSuite
	{
		protected void DoSyncCommand(RoutedCommand command, IInputElement target, object parameter)
		{
			command.Execute(parameter, target);
			DispatcherHelper.DoEvents();
		}

		protected void DoSyncCommand(RoutedCommand command, IInputElement target)
		{
			command.Execute(null, target);
			DispatcherHelper.DoEvents();
		}

		/// <summary>
		/// Returns IList of AnnotationStateInfo objects reflecting the state of the currently
		/// visible annotations.
		/// </summary>
		protected IList<AnnotationStateInfo> GetCurrentState()
		{
			IList<AnnotationStateInfo> currentState = new List<AnnotationStateInfo>();
			IList highlights = AnnotationComponentFinder.GetVisibleHighlightComponents(ViewerBase);
			IList<StickyNoteControl> stickynotes = AnnotationComponentFinder.GetVisibleStickyNotes(ViewerBase);
			foreach(object highlight in highlights)
				currentState.Add(HighlightStateInfo.FromHighlightComponent(highlight));
			foreach (StickyNoteControl sn in stickynotes)
				currentState.Add(StickyNoteStateInfo.FromStickyNote(sn));
			
			return currentState;
		}
	}
}	

