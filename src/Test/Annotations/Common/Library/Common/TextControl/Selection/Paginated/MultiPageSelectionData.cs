// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

using System;
using System.Windows;
using System.Windows.Documents;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Object that describes a selection that is defined by 2 differ
	/// </summary>
	public class MultiPageSelectionData : PaginatedSelectionData
	{
		public MultiPageSelectionData(int pageA, PagePosition positionA, int lengthA, int pageB, PagePosition positionB, int lengthB)
		{
			_pageA = pageA;
			_positionA = positionA;
			_lengthA = lengthA;
			_pageB = pageB;
			_positionB = positionB;
			_lengthB = lengthB;
		}

        protected override TextRange DoSetSelection(ADocumentViewerBaseSelector selectionModule)
        {
            return selectionModule.SetSelection(_pageA, _positionA, _lengthA, _pageB, _positionB, _lengthB);
        }

        public ISelectionData[] GetPageBasedSelection()
        {
            if (_pageA == _pageB)
            {
                return new ISelectionData[] { this };
            }

            ISelectionData[] selection = new ISelectionData[_pageB - _pageA + 1];
            int i = 0;
            
            selection[i++] = new MultiPageSelectionData(_pageA, _positionA, _lengthA, _pageA, PagePosition.End, 0);

            for (int k = _pageA + 1; k < _pageB; k++)
            {
                selection[i++] = new MultiPageSelectionData(k, PagePosition.Beginning, 0, k, PagePosition.End, 0);
            }

            selection[i] = new MultiPageSelectionData(_pageB, PagePosition.Beginning, 0, _pageB, _positionB, _lengthB);

            return selection;
        }

        private int _pageA;
		private PagePosition _positionA;
		private int _lengthA;
		private int _pageB;
		private PagePosition _positionB;
		private int _lengthB;
	}
}
