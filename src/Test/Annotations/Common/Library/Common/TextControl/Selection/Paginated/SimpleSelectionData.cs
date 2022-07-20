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
	/// Object that describes a selection that starts on some page, with and offset and a length.
	/// </summary>
	public class SimpleSelectionData : PaginatedSelectionData
	{
		public SimpleSelectionData(int page, int offset, int length)
		{
			_page = page;
			_offset = offset;
			_length = length;
		}

		public SimpleSelectionData(int page, int offset, LogicalDirection direction)
		{
			_directionSet = true;
			_page = page;
			_offset = offset;
			_direction = direction;
		}

		public SimpleSelectionData(int page, PagePosition position, int length)
		{
			_positionSet = true;
			_page = page;
			_position = position;
			_length = length;
		}

		public SimpleSelectionData(int page, PagePosition position, LogicalDirection direction)
		{
			_positionSet = true;
			_directionSet = true;
			_page = page;
			_position = position;
			_direction = direction;
		}

        protected override TextRange DoSetSelection(ADocumentViewerBaseSelector selectionModule)
        {
            TextRange result;
            if (_directionSet)
            {
                if (_positionSet)
                    result = selectionModule.SetSelection(_page, _position, _direction);
                else
                    result = selectionModule.SetSelection(_page, _offset, _direction);
            }
            else
            {
                if (_positionSet)
                    result = selectionModule.SetSelection(_page, _position, _length);
                else
                    result = selectionModule.SetSelection(_page, _offset, _length);
            }
            return result;
        }

		private int _page;
		private int _offset;
		private int _length;
		private bool _positionSet = false;
		private PagePosition _position;
		private bool _directionSet = false;
		private LogicalDirection _direction;
	}
}
