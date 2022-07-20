// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//
using System;
using System.Windows;
using Annotations.Test.Framework;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Controls;
using Annotations.Test.Reflection;
using Proxies.MS.Internal.Annotations.Component;

using StickyNoteControl = System.Windows.Controls.StickyNoteControl;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Defines that state of a DocumentPaginator at some fixed point in time.
	/// </summary>
    public class DocumentStateInfo
    {
		#region Public Methods

		public void AddState(int page, IList<StickyNoteControl> stickynotes)
		{
			foreach (StickyNoteControl sn in stickynotes)
				InfoList(page).Add(StickyNoteStateInfo.FromStickyNote(sn));
		}

		public void AddState(int page, IList<HighlightComponent> highlights)
		{
			foreach (HighlightComponent highlight in highlights)
				InfoList(page).Add(HighlightStateInfo.FromHighlightComponent(highlight));
		}

		public IList<AnnotationStateInfo> AnnotationState(int pageNum)
		{
			if (!_stateDict.Keys.Contains(pageNum))
				return new List<AnnotationStateInfo>();
			return _stateDict[pageNum];
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Get info list from Dictionary or create one if it doesn't exist.
		/// </summary>
		protected IList<AnnotationStateInfo> InfoList(int page)
		{
			IList<AnnotationStateInfo> infoList;
			if (!_stateDict.Keys.Contains(page))
			{
				infoList = new List<AnnotationStateInfo>();
				_stateDict.Add(page, infoList);
			}
			else
			{
				infoList = _stateDict[page];
			}
			return infoList;
		}

		#endregion

		#region Private Fields

		IDictionary<int, IList<AnnotationStateInfo>> _stateDict = new Dictionary<int, IList<AnnotationStateInfo>>();

		#endregion
	}
}	
