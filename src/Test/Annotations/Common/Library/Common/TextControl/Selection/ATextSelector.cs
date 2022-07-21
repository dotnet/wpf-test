// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Reflection;
using System.Collections;
using System.Windows.Annotations;
using Annotations.Test.Reflection;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Security.Permissions;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations
{
	public enum PagePosition
	{
		Beginning,
		Middle,
		End
	}

	/// <summary>
	/// Module for making programmatic selections on a DocumentViewer.
	/// </summary>
	[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	public abstract class SelectionModule
	{
		#region Constructors

        public SelectionModule(Control target)
		{
            targetControl = target;

            BindingFlags FULL_ACCESS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            ITextRangeType = PresentationFramework.GetType("System.Windows.Documents.ITextRange");
            TextEditorType = PresentationFramework.GetType("System.Windows.Documents.TextEditor");
            ITextViewType = PresentationFramework.GetType("System.Windows.Documents.ITextView");
            ITextPointerType = PresentationFramework.GetType("System.Windows.Documents.ITextPointer");
            ITextContainerType = PresentationFramework.GetType("System.Windows.Documents.ITextContainer");
            TextSegmentType = PresentationFramework.GetType("System.Windows.Documents.TextSegment");

            // ITextRange.			
            ITextRange_Select = ITextRangeType.GetMethod("Select", FULL_ACCESS, null, new Type[] { ITextPointerType, ITextPointerType }, null);

            // TextEditor.			
            TextEditor_Selection = TextEditorType.GetMethod("GetTextSelection", FULL_ACCESS);

            // ITextView.			
            ITextView_GetTextPositionFromPoint = ITextViewType.GetMethod("GetTextPositionFromPoint",FULL_ACCESS, null, new Type[] { typeof(Point), typeof(bool) }, null);
            ITextView_TextContainer = ITextViewType.GetProperty("TextContainer", FULL_ACCESS);

            // ITextPointer.			
            ITextPointer_CreatePointer = PresentationFramework.GetType("System.Windows.Documents.ITextPointer").GetMethod("CreatePointer", FULL_ACCESS, null, new Type[] { typeof(int) }, null);			
            ITextPointer_Offset = ITextPointerType.GetProperty("Offset", FULL_ACCESS);
            ITextPointer_GetOffsetToPosition = ITextPointerType.GetMethod("GetOffsetToPosition", FULL_ACCESS, null, new Type[] { ITextPointerType }, null);
            ITextPointer_TextContainer = ITextPointerType.GetProperty("TextContainer", FULL_ACCESS);
            ITextPointer_GetCharacterRect = ITextPointerType.GetMethod("GetCharacterRect", FULL_ACCESS, null, new Type[] { typeof(LogicalDirection) }, null);
            ITextPointer_LogicalDirection = ITextPointerType.GetProperty("LogicalDirection", FULL_ACCESS);
            ITextPointer_MoveToNextInsertionPosition = ITextPointerType.GetMethod("MoveToNextInsertionPosition", FULL_ACCESS, null, new Type[] { typeof(LogicalDirection) }, null);
            ITextPointer_MoveToInsertionPosition = ITextPointerType.GetMethod("MoveToInsertionPosition", FULL_ACCESS, null, new Type[] { typeof(LogicalDirection) }, null);
            ITextPointer_IsAtInsertionPosition = ITextPointerType.GetProperty("IsAtInsertionPosition", FULL_ACCESS);

            // ITextContainer.			
            ITextContainer_Start = ITextContainerType.GetProperty("Start", FULL_ACCESS);
            ITextContainer_End = ITextContainerType.GetProperty("End", FULL_ACCESS);

            // TextSegment.
            ITextView_TextSegments = ITextViewType.GetProperty("TextSegments", FULL_ACCESS);
            Item = typeof(IList).GetProperty("Item", new Type[] { typeof(int) });
            TextSegment_Start = TextSegmentType.GetProperty("Start", FULL_ACCESS);
            TextSegment_End = TextSegmentType.GetProperty("End", FULL_ACCESS);
		}

		#endregion
       
        #region Public Methods

        /// <summary>
        /// Create selection defined in absolute character offsets from document start.
        /// </summary>
        /// <param name="startOffset">Char offest to begin selection.</param>
        /// <param name="endOffset">Char offset to end selection.</param>
        /// <returns>TextRange corresponding to selection.</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public TextRange SetSelection(int startOffset, int endOffset)
        {
            object start = CreatePointer(StartOfDocument, startOffset);
            object end = CreatePointer(StartOfDocument, endOffset);
            return Select(start, end);
        }

        /// <summary>
        /// Create selection defined in absolute character offsets.
        /// </summary>
        /// <param name="position">Position relative to document.</param>
        /// <param name="length">Offset from start postion to end selection.</param>
        /// <returns>TextRange corresponding to selection.</returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public TextRange SetSelection(PagePosition position, int length)
        {
            object start = PositionToPointer(position);
            object end = CreatePointer(start, length);
            return Select(start, end);
        }

        /// <summary>
        /// Create selection defined in absolute character offsets.
        /// </summary>
        /// <param name="startPosition">Position relative to document.</param>
        /// <param name="startOffset">Char offset from startPosition to begin selection.</param>
        /// <param name="endPosition">Position relative to document.</param>
        /// <param name="endOffset">Char offset from endPosition to end selection.</param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        public TextRange SetSelection(PagePosition startPosition, int startOffset, PagePosition endPosition, int endOffset)
        {
            object start = PositionToPointer(startPosition);
            start = CreatePointer(start, startOffset);
            object end = PositionToPointer(endPosition);
            end = CreatePointer(end, endOffset);
            return Select(start, end);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public void SetDirection(object pointer, LogicalDirection direction)
		{
            ITextPointerType.GetMethod("SetLogicalDirection").Invoke(pointer, new object[] { direction });
		}
	
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange CreateTextRange(object startPointer, object endPointer)
		{
			return typeof(TextRange).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
							null,
							new Type[] { ITextPointerType, ITextPointerType },
							null).Invoke(new object[] { startPointer, endPointer }) as TextRange;
		}

		/// <summary>
		/// Set the Selection property of DocumentViewer with the given ITextPointers.
		/// </summary>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public TextRange Select(object startPointer, object endPointer)
		{
			if (!WriteThrough)
			{
				TextRange range = CreateTextRange(startPointer, endPointer);
				return range;
			}
			else
			{
				TextSelection selection = TextSelection;
				ITextRange_Select.Invoke(selection, new object[] { startPointer, endPointer });
				return TextSelection;
			}
		}

		/// <summary>
		/// Create a new pointer that is relative to an existing one.
		/// </summary>
        /// <remarks>
        /// Current implementation is to iterate through all the valid insertion points from the 
        /// current point to the given character offset.  This is done becuase there is no
        /// api for moving by character offsets.  This approach is relatively performant for
        /// paginated content because the number of positions per page is small, however, for 
        /// bottomless content this method should be overriden.
        /// </remarks>
		[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
		public virtual object CreatePointer(object pointer, int offset)
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
		/// Make given string of a format that will be easily readable when printed to the screen.
		/// Does stuff like, escaping all the \n's so that they are printed as '\n' instead of an actual carriage return.
		/// </summary>
		public static string PrintFriendlySelection(string msg)
		{
			string friendly = msg.Replace("\r\n", "\\r\\n");
			return friendly;
		}

		#endregion

        #region Public Properties

        /// <summary>
        /// Get document that selection is being performed on.
        /// </summary>
        public abstract IDocumentPaginatorSource Document
        {
            get;
        }

        /// <summary>
        /// Get the first ITextPointer in document.
        /// </summary>
        public abstract object StartOfDocument
        {
            get;
        }

        /// <summary>
        /// Get the last ITextPointer in document.
        /// </summary>
        public abstract object EndOfDocument
        {
            get;
        }

        /// <summary>
        /// If true then when SetSelection is called it will actually cause a change to the 
        /// DocumentViewer's selection.  If this is false, it will return what the TextSelection
        /// would be, but without actually setting it.
        /// </summary>
        public bool WriteThrough
        {
            get
            {
                return _writeThrough;
            }
            set
            {
                _writeThrough = value;
            }
        }

        /// <summary>
        /// Publicly expose selection as a TextRange.
        /// </summary>		
        public TextRange Selection
        {
            get
            {
                return (TextRange)TextSelection;
            }
        }

        public Control Target
        {
            get { return targetControl; }
        }

        /// <summary>
        /// Get the module that is capable of selecting within Tables.
        /// </summary>
        public TableSelector Tables
        {
            get
            {          
                if (tableSelector == null)
                    tableSelector = new TableSelector(this);
                return tableSelector;
            }
        }

        /// <summary>
        /// Get the module that is capable of selecting within Figures or Floaters..
        /// </summary>
        public AnchoredBlockSelector AnchoredBlocks
        {
            get
            {
                if (anchoredBlockSelector == null)
                    anchoredBlockSelector = new AnchoredBlockSelector(this);
                return anchoredBlockSelector;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Return Target's TextSelection.
        /// </summary>        
        protected virtual TextSelection TextSelection
        {
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            get
            {
                return TextEditor_Selection.Invoke(null, new object[] { Target }) as TextSelection;
            }
        }

        /// <summary>
        /// Converts a PagePosition relative to document to a pointer.
        /// </summary>
        /// <param name="position">Position relative to document.</param>
        /// <returns>ITextPointer of position.</returns>
        protected object PositionToPointer(PagePosition position)
        {
            object pointer = null;

            switch (position)
            {
                case PagePosition.Beginning:
                    pointer = StartOfDocument;
                    break;
                case PagePosition.End:
                    pointer = EndOfDocument;
                    break;
                case PagePosition.Middle:
                    int midOffset = ((int)ITextPointer_GetOffsetToPosition.Invoke(StartOfDocument, new object[] { EndOfDocument })) / 2;
                    pointer = CreatePointer(StartOfDocument, midOffset);
                    break;
                default:
                    throw new NotImplementedException(position.ToString());
            }
            return pointer;
        }

        /// <summary>
        /// Find the last insertion position in the ITextView.
        /// </summary>
        /// <param name="textView">ITextView to search.</param>
        /// <returns>ITextPointer of the last InsertionPosition in view.</returns>
        protected object FindLastInsertionPosition(object textView)
        {
            object collection = ITextView_TextSegments.GetValue(textView, null);
            int count = (int)collection.GetType().GetProperty("Count").GetValue(collection, null);
            object textSegment = Item.GetValue(collection, new object[] { count - 1 });
            object endPoint = TextSegment_End.GetValue(textSegment, null);

            endPoint = ReflectionHelper.InvokeMethod(endPoint, "CreatePointer", new object[] { LogicalDirection.Backward });
            if (!((bool)ITextPointer_IsAtInsertionPosition.GetValue(endPoint, null)))
            {
                ITextPointer_MoveToNextInsertionPosition.Invoke(endPoint, new object[] { LogicalDirection.Backward });
            }

            return endPoint;
        }

		#endregion

		#region Protected Fields

        
        protected Control targetControl;

        // Module for selecting within tables.
        
        protected TableSelector tableSelector;

        // Module for selectin within Figures/Floaters.
        
        protected AnchoredBlockSelector anchoredBlockSelector;

		static protected Assembly PresentationFramework = typeof(AnnotationService).Assembly;
		
		// ITextView.
		static protected Type			ITextViewType;
		static protected MethodInfo		ITextView_GetTextPositionFromPoint;
        static protected PropertyInfo   ITextView_TextContainer;

		// ITextRange.
		static protected Type			ITextRangeType;
		static protected MethodInfo		ITextRange_Select;		
		
		// TextEditor.
		static protected Type			TextEditorType;
		static protected MethodInfo		TextEditor_Selection;

		// ITextContainer.
		static protected Type			ITextContainerType;
		static protected PropertyInfo	ITextContainer_Start;
        static protected PropertyInfo   ITextContainer_End;

		// ITextPointer.
		static protected Type           ITextPointerType;
		static protected MethodInfo		ITextPointer_CreatePointer;
		static protected PropertyInfo	ITextPointer_Offset;
		static protected MethodInfo		ITextPointer_GetOffsetToPosition;
		static protected PropertyInfo	ITextPointer_TextContainer;
        static protected MethodInfo     ITextPointer_GetCharacterRect;
        static protected PropertyInfo   ITextPointer_LogicalDirection;        
        static protected MethodInfo     ITextPointer_MoveToNextInsertionPosition;
        static protected MethodInfo     ITextPointer_MoveToInsertionPosition;
        static protected PropertyInfo ITextPointer_IsAtInsertionPosition;       

        // TextSegment
        static protected Type TextSegmentType;
        static protected PropertyInfo ITextView_TextSegments;
        static protected PropertyInfo Item;
        static protected PropertyInfo TextSegment_Start;
        static protected PropertyInfo TextSegment_End;
		
		#endregion

		#region private Fields

		bool _writeThrough = true;

		#endregion
	}
}
