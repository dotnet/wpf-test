// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: API and logic for making selections on subclasses of DocumentViewerBase.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Annotations;
using System.Security.Permissions;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{	
	[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    public abstract class ADocumentViewerBaseSelector : SelectionModule
	{
		#region Constructors

        public ADocumentViewerBaseSelector(DocumentViewerBase documentViewer)
            : base(documentViewer)
		{
			// Nothing.			
		}

		#endregion

        #region Abstract Methods

        /// <summary>
        /// Create a pointer at the first valid insertion point at the given page.
        /// </summary>
        public abstract object CreateStartPointer(DocumentPage page, object textView);

        /// <summary>
        /// Create a pointer at the last valid insertion point at the given page.
        /// </summary>
        public abstract object CreateEndPointer(DocumentPage page, object textView);

        #endregion

		#region Public Methods

        /// <summary>
        /// Returns the bounds of the character at the given position.  Values will
        /// be relative to the DocumentPage.
        /// </summary>
        /// <returns>Returns Point relative to page.</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public Rect CharacterRect(int page, int offset, LogicalDirection direction)
        {
            object pointer = CreatePointer(page, offset);
            return (Rect)ITextPointer_GetCharacterRect.Invoke(pointer, new object[] { direction });
        }

		/// <summary>
		/// Get the Symbol offset from the beginning of the document for the start of this page.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public int PageOffset(int page)
		{			
			object beginningOfPage = CreatePointer(page, PagePosition.Beginning);
			object textContainer = ITextPointer_TextContainer.GetValue(beginningOfPage, null);
			object beginningOfDoc= ITextContainer_Start.GetValue(textContainer, null);
			
			return (int)ITextPointer_GetOffsetToPosition.Invoke(beginningOfDoc, new object[] { beginningOfPage });
		}

		/// <summary>
		/// Get the total number of characters in the document.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public int DocumentLength()
		{
			object pointer = CreatePointer(Viewer.PageCount - 1, PagePosition.End);
			return (int)ITextPointer_Offset.GetValue(pointer, null);
		}

		/// <summary>
		/// Programmatically define the TextSelection of the DocumentViewer.
		/// </summary>
		/// <param name="pageNumber">Page boundry to use the startIdx relative to.</param>
		/// <param name="startIdx">Character offset from the beginning of page 'pageNumber' to begin selection.</param>
		/// <param name="length">Character offset from 'startIdx'.</param>
		/// <returns>TextSelection made.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int pageNumber, int startIdx, int length)
		{
			object startPointer = CreatePointer(pageNumber, startIdx);
			object endPointer = CreatePointer(startPointer, length);

			return Select(startPointer, endPointer);
		}

		/// <summary>
		/// Create a zero length selection at the given offset with a certain direction.
		/// </summary>
		/// <param name="pageNumber">Page boundry to use the startIdx relative to.</param>
		/// <param name="offset">Offset into page to create TextPointer.</param>
		/// <param name="direction">LogicalDirection to assign to the TextPointer.</param>
		/// <returns>TextSelection made.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int pageNumber, int offset, LogicalDirection direction)
		{
			object pointer = CreatePointer(pageNumber, offset);
			SetDirection(pointer, direction);
			return Select(pointer, pointer);
		}

		/// <summary>
		/// Create a zero length selection at the given position with a certain direction.
		/// </summary>
		/// <param name="pageNumber">Page boundry to use the startIdx relative to.</param>
		/// <param name="offset">Offset into page to create TextPointer.</param>
		/// <param name="direction">LogicalDirection to assign to the TextPointer.</param>
		/// <returns>TextSelection made.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int pageNumber, PagePosition position, LogicalDirection direction)
		{
			object pointer = CreatePointer(pageNumber, position);
			SetDirection(pointer, direction);
			return Select(pointer, pointer);
		}

		/// <summary>
		/// Set the selection of DocumentViewer.
		/// </summary>
		/// <param name="startPageNum">Page selections starts on.</param>
		/// <param name="startPagePos">Character offset from beginning of page to begin selection.</param>
		/// <param name="endPageNum">Page selection ends on.</param>
		/// <param name="endPagePos">Character offset from beginning of page to end selection.</param>
		/// <returns></returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int startPageNum, int startOffset, int endPageNum, int endOffset)
		{
			object startPointer = CreatePointer(startPageNum, startOffset);
			object endPointer = CreatePointer(endPageNum, endOffset);

			return Select(startPointer, endPointer);
		}

		/// <summary>
		/// Set the selection of DocumentViewer.
		/// </summary>
		/// <param name="startPageNum">Page selections starts on.</param>
		/// <param name="startPagePos">Location on page where selection begins.</param>
		/// <param name="endPageNum">Page selection ends on.</param>
		/// <param name="endPagePos">Location on page where selection ends.</param>
		/// <returns></returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int startPageNum, PagePosition startPagePos, int endPageNum, PagePosition endPagePos)
		{
			object startPointer = CreatePointer(startPageNum, startPagePos);
			object endPointer = CreatePointer(endPageNum, endPagePos);

			return Select(startPointer, endPointer);
		}

		/// <summary>
		/// Set the selection of DocumentViewer.
		/// </summary>
		/// <param name="startPageNum">Page selections starts on.</param>
		/// <param name="startPagePos">Location on page where selection begins.</param>
		/// <param name="startOffset">Offset from startPagePos to set beginning of selection.</param>
		/// <param name="endPageNum">Page selection ends on.</param>
		/// <param name="endPagePos">Location on page where selection ends.</param>
		/// <param name="endOffset">Offset from endPagePos to set end of selection.</param>
		/// <returns></returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int startPageNum, PagePosition startPagePos, int startOffset,
										  int endPageNum, PagePosition endPagePos, int endOffset)
		{
			object startPointer = CreatePointer(startPageNum, startPagePos);
			startPointer = CreatePointer(startPointer, startOffset);
			object endPointer = CreatePointer(endPageNum, endPagePos);
			endPointer = CreatePointer(endPointer, endOffset);

			return Select(startPointer, endPointer);
		}

		/// <summary>
		/// Set selection of DocumentViewer.
		/// </summary>
		/// <param name="pageNum">Page number to start selection on.</param>
		/// <param name="startPos">Location on page to begin selection.</param>
		/// <param name="offset">Relative offset from start location to end selection (negative ok).</param>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange SetSelection(int pageNum, PagePosition startPos, int offset)
		{
			object startPointer = CreatePointer(pageNum, startPos);
			object endPointer = CreatePointer(startPointer, offset);

			return Select(startPointer, endPointer);
		}

		/// <summary>
		/// Create a TextRange within a DocumentViewer.
		/// </summary>
		/// <param name="pageNumber">Page boundry to use the startIdx relative to.</param>
		/// <param name="startIdx">Character offset from the beginning of page 'pageNumber' to begin selection.</param>
		/// <param name="length">Character offset from 'startIdx'.</param>
		/// <returns>TextSelection made.</returns>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange MakeTextRange(int pageNumber, int startIdx, int length)
		{
			object startPointer = CreatePointer(pageNumber, startIdx);
			object endPointer = CreatePointer(startPointer, length);
			return CreateTextRange(startPointer, endPointer);
		}

		/// <summary>
		/// Determine the length of the given page in symbol offsets.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public int PageLength(int pageNumber)
		{
			object startPointer = CreatePointer(pageNumber, PagePosition.Beginning);
			object endPointer = CreatePointer(pageNumber, PagePosition.End);
			return (int) ITextPointer_GetOffsetToPosition.Invoke(startPointer, new object[] { endPointer });			
		}

		/// <summary>
		/// Create a pointer that is 'offset' characters from the beginning of the given page.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public object CreatePointer(int pageNum, int offset)
		{
			DocumentPage page = Viewer.Document.DocumentPaginator.GetPage(pageNum);
            object textView = GetTextView(page);

			object startOfPagePointer = CreateStartPointer(page, textView);
			return CreatePointer(startOfPagePointer, offset);
		}

        /// <summary>
        /// Create a new pointer that is relative to an existing one.
        /// </summary>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public override object CreatePointer(object pointer, int offset)
        {
            // Don't use this api, because it won't necessarily produce a selection of 'offset' length,
            //
            //return ITextPointerCreatePointerMethod.Invoke(pointer, new object[] { offset });
            //
            // instead starting with the given pointer, move 'offset' valid insertion points forward.
            //
            LogicalDirection direction = (offset >= 0) ? LogicalDirection.Forward : LogicalDirection.Backward;
            object newPointer = ITextPointer_CreatePointer.Invoke(pointer, new object[] { 0 }); // clone
            for (int i = 0; i < Math.Abs(offset); i++)
            {
                bool result = (bool)ITextPointer_MoveToNextInsertionPosition.Invoke(newPointer, new object[] { direction });
                if (!result)
                    throw new InvalidOperationException("Cannot create a pointer that is beyond the valid insteration range.");
            }
            return newPointer;
        }

		/// <summary>
		/// Create a pointer that is either at the beginning or the end of a page.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public object CreatePointer(int pageNum, PagePosition position)
		{
			DocumentPage page = Viewer.Document.DocumentPaginator.GetPage(pageNum);
            object textView = GetTextView(page);

			object pointer;
			if (position == PagePosition.Beginning)
			{
				pointer = CreateStartPointer(page, textView);
			}
			else if (position == PagePosition.End)
			{
				pointer = CreateEndPointer(page, textView);
			}
			else if (position == PagePosition.Middle)
			{
				object start = CreateStartPointer(page, textView);
				object end = CreateEndPointer(page, textView);
				pointer = CreatePointer(pageNum, CreateTextRange(start, end).Text.Length / 2);
			}
			else
				throw new NotSupportedException("CreatePointer for PagePosition '" + position + "' is not supported.");

			return ITextPointer_CreatePointer.Invoke(pointer, new object[] { 0 });
		}

        /// <summary>
        /// Make given string of a format that will be easily readable when printed to the screen.
        /// Does stuff like, escaping all the \n's so that they are printed as '\n' instead of an actual carriage return.
        /// </summary>
        new public static string PrintFriendlySelection(string msg)
        {
            string friendly = msg.Replace("\r\n", "\\r\\n");
            return friendly;
        }

        /// <summary>
        /// Get the TextView associated with the given page.
        /// </summary>
        /// <returns>ITextView or null if none exists.</returns>
        public object GetTextView(DocumentPage page)
        {
            return ((IServiceProvider)page).GetService(ITextViewType);
        }

		#endregion

        #region Public Properties

        public DocumentViewerBase Viewer
        {
            get 
            { 
                return Target as DocumentViewerBase; 
            }
        }

        public override IDocumentPaginatorSource Document
        {
            get
            {
                return Viewer.Document;
            }
        }

        public override object StartOfDocument
        {
            get
            {
                DocumentPage startPage = Document.DocumentPaginator.GetPage(0);
                return CreateStartPointer(startPage, GetTextView(startPage));
            }
        }

        public override object EndOfDocument
        {
            get
            {
                DocumentPage endPage = Document.DocumentPaginator.GetPage(Document.DocumentPaginator.PageCount - 1);
                return CreateEndPointer(endPage, GetTextView(endPage));
            }
        }

        #endregion           
   
    }
}
