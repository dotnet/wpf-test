// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a very straightforward, array-based implementation of a text container.

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Controls;

    #endregion Namespaces.

    /// <summary>
    /// Container for DependencyProperty/value pairs.
    /// </summary>
    public class DependencyPropertyBag : Dictionary<DependencyProperty, object>
    {

        #region Constructors.

        /// <summary>
        /// Initializes an empty DependencyPropertyBag instance.
        /// </summary>
        public DependencyPropertyBag()
        {
        }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Applies the values in the bag to the specified object.
        /// </summary>
        /// <param name="dependencyObject">Object to apply values to.</param>
        public void ApplyValues(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            foreach (KeyValuePair<DependencyProperty, object> entry in this)
            {
                dependencyObject.SetValue(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Initializes this bag from the local properties assigned to the 
        /// specified object.
        /// </summary>
        /// <param name="dependencyObject">Object to get local values from.</param>
        public void ReadLocalValues(DependencyObject dependencyObject)
        {
            LocalValueEnumerator enumerator;

            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            Clear();
            enumerator = dependencyObject.GetLocalValueEnumerator();
            while (enumerator.MoveNext())
            {
                this[enumerator.Current.Property] = enumerator.Current.Value;
            }
        }

        #endregion Public methods.
    }

    /// <summary>
    /// A text symbol in an ArrayTextContainer.
    /// </summary>
    public class ArrayTextSymbol
    {

        #region Constructors.

        /// <summary>
        /// Initializes an ArrayTextSymbol instance by cloning the
        /// specified symbol
        /// </summary>
        /// <param name="symbol">Symbol to clone.</param>
        public ArrayTextSymbol(ArrayTextSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }

            this._context = symbol._context;
            this._element = symbol._element;
            this._embeddedElement = symbol._embeddedElement;
            this._character = symbol._character;
        }

        /// <summary>
        /// Initializes an ArrayTextSymbol instance with the specified
        /// character text.
        /// </summary>
        /// <param name="c">Character text for symbol.</param>
        public ArrayTextSymbol(char c)
        {
            this._character = c;
            this._context = TextPointerContext.Text;
            this._localValues = new DependencyPropertyBag();
        }

        /// <summary>
        /// Initializes an ArrayTextSymbol instance with the specified
        /// TextElement.
        /// </summary>
        /// <param name="element">Element referenced by symbol.</param>
        /// <param name="isStart">Whether the element is the start edge.</param>
        public ArrayTextSymbol(TextElement element, bool isStart)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this._element = (TextElement)Activator.CreateInstance(element.GetType());
            this._context = (isStart) ? TextPointerContext.ElementStart
                : TextPointerContext.ElementEnd;
            this._localValues = new DependencyPropertyBag();
            this._localValues.ReadLocalValues(element);
            this._localValues.ApplyValues(this._element);
        }

        /// <summary>
        /// Initializes an ArrayTextSymbol instance with the specified
        /// UIElement or ContentElement.
        /// </summary>
        /// <param name="embeddedElement">Embedded element referenced by symbol.</param>
        public ArrayTextSymbol(DependencyObject embeddedElement)
        {
            if (embeddedElement == null)
            {
                throw new ArgumentNullException("embeddedElement");
            }
            if (embeddedElement is TextElement)
            {
                throw new InvalidOperationException("Use .ctor(TextElement, bool) for TextElement instances.");
            }

            this._embeddedElement = embeddedElement;
            this._context = TextPointerContext.EmbeddedElement;
        }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Verifies whether this symbol matches the specified symbol.
        /// </summary>
        /// <param name="symbol">Symbol to match against.</param>
        /// <returns>true if the symbols match, false otherwise.</returns>
        public bool Matches(ArrayTextSymbol symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }

            if (symbol.Context != this.Context)
            {
                return false;
            }
            if (symbol._character != this._character ||
                symbol._embeddedElement != this._embeddedElement)
            {
                return false;
            }
            if (this._element != null)
            {
                if (this._element.GetType() != symbol._element.GetType())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a System.String that represent the current symbol.
        /// </summary>
        /// <returns>As System.String that represent the current symbol.</returns>
        public override string ToString()
        {
            string result;

            result = "[Symbol " + Context + " ";
            switch (Context)
            {
                case TextPointerContext.Text:
                    result += EscapeCharacterConverter.ConvertToVisibleAnsi(Character);
                    break;
                case TextPointerContext.ElementEnd:
                case TextPointerContext.ElementStart:
                    result += Element.GetType().Name;
                    break;
            }
            return result + "]";
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// Character value for character symbols (identified
        /// by a Text context).
        /// </summary>
        public char Character
        {
            get
            {
                if (this.Context != TextPointerContext.Text)
                {
                    throw new InvalidOperationException(
                        "Cannot retrieve character from non-text symbol.");
                }
                return this._character;
            }
        }

        /// <summary>Context for symbol.</summary>
        public TextPointerContext Context
        {
            get
            {
                return this._context;
            }
        }

        /// <summary>
        /// Element referenced by element edge symbols (identified
        /// by an ElementEnd or ElementStart context).
        /// </summary>
        public TextElement Element
        {
            get
            {
                if (this.Context != TextPointerContext.ElementEnd &&
                    this.Context != TextPointerContext.ElementStart)
                {
                    throw new InvalidOperationException(
                        "Cannot retrieve element from non-element symbol.");
                }
                return this._element;
            }
        }

        /// <summary>
        /// UIElement or ContentElement referenced by embedded object symbols
        /// (identified by an EmbeddedElement context).
        /// </summary>
        /// <remarks>This may be a UIElement or a </remarks>
        public DependencyObject EmbeddedElement
        {
            get
            {
                if (this.Context != TextPointerContext.EmbeddedElement)
                {
                    throw new InvalidOperationException(
                        "Cannot retrieve embedded element from non-embedded symbol.");
                }
                return this._embeddedElement;
            }
        }

        /// <summary>Whether this symbol is an element start or edge symbol.</summary>
        public bool IsElementEdge
        {
            get
            {
                return this.Context == TextPointerContext.ElementStart || 
                    this.Context == TextPointerContext.ElementEnd;
            }
        }

        /// <summary>Dictionary of local values that apply to the symbol.</summary>
        public DependencyPropertyBag LocalValues
        {
            get
            {
                return this._localValues;
            }
        }

        #endregion Public properties.


        #region Private fields.

        /// <summary>Character value held by character symbols.</summary>
        private char _character;

        /// <summary>Context for symbol.</summary>
        private TextPointerContext _context;

        /// <summary>Element copy held by ElementStart or ElementEnd symbols.</summary>
        private TextElement _element;

        /// <summary>Local element values held by ElementStart, ElementEnd or embedded elements.</summary>
        private DependencyPropertyBag _localValues;

        /// <summary>Element held by EmbeddedObject symbols.</summary>
        private DependencyObject _embeddedElement;

        /*
        matching end/start ArrayTextSymbol
        index in list
        */

        #endregion Private fields.
    }

    /// <summary>Provides convenience values to work with a LogicalDirection.</summary>
    class DirectionHelper
    {
        /// <summary>Initializes a new DirectionHelper instance.</summary>
        /// <param name="direction">Direction for helper.</param>
        internal DirectionHelper(LogicalDirection direction)
        {
            this.Direction = direction;
            if (direction == LogicalDirection.Forward)
            {
                OppositeDirection = LogicalDirection.Backward;
                MoveDistance = 1;
                DirectionOffset = 0;
                EnterElementContext = TextPointerContext.ElementStart;
                ExitElementContext = TextPointerContext.ElementEnd;
            }
            else
            {
                OppositeDirection = LogicalDirection.Forward;
                MoveDistance = -1;
                DirectionOffset = -1;
                EnterElementContext = TextPointerContext.ElementEnd;
                ExitElementContext = TextPointerContext.ElementStart;
            }
        }

        /// <summary>Direction for this helper.</summary>
        internal readonly LogicalDirection Direction;

        /// <summary>Direction opposite to Direction.</summary>
        internal readonly LogicalDirection OppositeDirection;

        /// <summary>Helper for opposite direction.</summary>
        internal DirectionHelper OppositeHelper
        {
            get { return new DirectionHelper(OppositeDirection); }
        }

        /// <summary>Offset to move in Direction.</summary>
        internal readonly int MoveDistance;

        /// <summary>Offset to inspect a symbol in ArrayTextContainer, given Direction.</summary>
        internal readonly int DirectionOffset;

        /// <summary>Context found when entering an element in this direction.</summary>
        internal readonly TextPointerContext EnterElementContext;

        /// <summary>Context found when exiting an element in this direction.</summary>
        internal readonly TextPointerContext ExitElementContext;
    }

    /// <summary>
    /// Container for text symbols.
    /// </summary>
    public class ArrayTextContainer : List<ArrayTextSymbol>
    {

        #region Constructors.

        /// <summary>Initializes a new ArrayTextContainer instance.</summary>
        public ArrayTextContainer()
        {
        }

        /// <summary>
        /// Initializes a new ArrayTextContainer by cloning the specified
        /// contianer.
        /// </summary>
        /// <param name="container">Container to clone.</param>
        public ArrayTextContainer(ArrayTextContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            foreach (ArrayTextSymbol symbol in container)
            {
                Add(new ArrayTextSymbol(symbol));
            }
        }

        /// <summary>
        /// Initializes a new ArrayTextContainer from the TextContainer
        /// of the specified TextPointer.
        /// </summary>
        /// <param name="pointer">Pointer inside container to clone.</param>
        public ArrayTextContainer(TextPointer pointer)
        {
            TextPointer p;  // Working pointer value.

            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            p = pointer;

            // Move to the beginning of the container.
            //while (p.MoveByOffset(-1024) != 0) { }
            int offsetToStart = p.GetOffsetToPosition(p.DocumentStart);
            p = p.GetPositionAtOffset(offsetToStart);

            // Extract every symbol.
            while (p.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None)
            {
                string text;

                switch (p.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.ElementEnd:
                        Add(new ArrayTextSymbol((TextElement)p.Parent, false));
                        p = p.GetPositionAtOffset(1);
                        break;
                    case TextPointerContext.ElementStart:
                        p = p.GetPositionAtOffset(1);
                        Add(new ArrayTextSymbol((TextElement)p.Parent, true));
                        break;
                    case TextPointerContext.EmbeddedElement:
                        Add(new ArrayTextSymbol(p.GetAdjacentElement(LogicalDirection.Forward)));
                        p = p.GetPositionAtOffset(1);
                        break;
                    case TextPointerContext.None:
                        throw new Exception("Unexpected None pointer context.");
                    case TextPointerContext.Text:
                        text = p.GetTextInRun(LogicalDirection.Forward);
                        AppendText(text);
                        p = p.GetNextContextPosition(LogicalDirection.Forward);
                        break;
                }
            }
        }

        /// <summary>Initializes a new ArrayTextContainer instance.</summary>
        public ArrayTextContainer(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            AppendText(text);
        }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Deletes content between the specified start and
        /// end indexes, following text editing rules.
        /// </summary>
        /// <param name="startIndex">Index of first symbol to delete.</param>
        /// <param name="endIndex">Index of last symbol to delete.</param>
        /// <remarks>This currently works for plain text only.</remarks>
        public void DeleteContent(int startIndex, int endIndex)
        {
            VerifyIndices(startIndex, endIndex, false);

            for (int i = endIndex; i >= startIndex; i--)
            {
                this.RemoveAt(i);
            }
            VerifyConsistency();
        }

        /// <summary>
        /// Removes the base and combining characters at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the string text element starts.</param>
        /// <remarks>
        /// The .NET Framework defines a text element as a unit of text that 
        /// is displayed as a single character; that is, a grapheme. A text 
        /// element can be a base character, a surrogate pair, or a combining 
        /// character sequence. This is *not* an Avalon TextElement.
        /// </remarks>
        public void DeleteCombinedCharacters(int index)
        {
            string text;
            int lastIndex;

            if (index < 0 || index >= this.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (this[index].Context != TextPointerContext.Text)
            {
                throw new ArgumentException("Index " + index + " refers to a " +
                    this[index].Context + " symbol but must refer to a Text symbol.", "index");
            }

            // Get as much text as possible and calculate the last possible index.
            // Use a reasonable limit to keep this faster for longer runs.
            text = new string(this[index].Character, 1);
            lastIndex = index;
            while (lastIndex < this.Count && lastIndex < index + 12)
            {
                if (this[lastIndex + 1].Context == TextPointerContext.Text)
                {
                    text += this[lastIndex + 1].Character;
                    lastIndex++;
                }
                else
                {
                    break;
                }
            }

            // Get the real last-index of the text element.
            lastIndex = index + StringInfo.GetNextTextElement(text).Length - 1;

            // Delete the content.
            DeleteContent(index, lastIndex);
        }

        /// <summary>Returns a description of each symbol with multiple lines.</summary>
        /// <returns>A string describing each symbol.</returns>
        public string DescribeSymbols()
        {
            StringBuilder builder;

            builder = new StringBuilder(this.Count * 12);
            builder.AppendLine("Symbol count: " + this.Count);
            for (int i = 0; i < Count; i++)
            {
                builder.AppendFormat("{0,3} - {1}\r\n", i, this[i].ToString());
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the count of elements that are around the specified symbol.
        /// </summary>
        /// <param name="index">The index position to count from.</param>
        /// <returns>The count of elements that are around the specified symbol.</returns>
        /// <remarks>Edges don't count themselves.</remarks>
        public int GetDepth(int index)
        {
            int result;

            if (index == Count)
            {
                return 0;
            }

            result = 0;
            if (this[index].Context == TextPointerContext.ElementEnd)
            {
                result--;   // To account for future start.
            }
            index--;
            while (index >= 0)
            {
                if (this[index].Context == TextPointerContext.ElementEnd)
                {
                    result--;
                }
                else if (this[index].Context == TextPointerContext.ElementStart)
                {
                    result++;
                }
                index--;
            }

            if (result < 0)
            {
                throw new Exception("Internal error: unbalanced ArrayTextContainer");
            }
            return result;
        }

        /// <summary>
        /// Gets the closest insertion position in the given direction from index,
        /// possibly index itself.
        /// </summary>
        /// <param name="index">Starting index to start looking from.</param>
        /// <param name="direction">Direction in which to look.</param>
        /// <param name="includeSelectionOnly">Whether selection-only positions should be considered.</param>
        /// <returns>
        /// The closest insertion position in the given direction from index,
        /// possibly index itself.
        /// </returns>
        public int GetInsertionPosition(int index, LogicalDirection direction,
            bool includeSelectionOnly)
        {
            bool blockBoundaryCrossed;

            return GetInsertionPosition(index, direction, includeSelectionOnly,
                out blockBoundaryCrossed);
        }

        /// <summary>
        /// Gets the closest insertion position in the given direction from index,
        /// possibly index itself.
        /// </summary>
        /// <param name="index">Starting index to start looking from.</param>
        /// <param name="direction">Direction in which to look.</param>
        /// <param name="includeSelectionOnly">Whether selection-only positions should be considered.</param>
        /// <param name="blockBoundaryCrossed">
        /// Whether a block boundary was crossed to get to the resulting position.
        /// </param>
        /// <returns>
        /// The closest insertion position in the given direction from index,
        /// possibly index itself.
        /// </returns>
        public int GetInsertionPosition(int index, LogicalDirection direction,
            bool includeSelectionOnly, out bool blockBoundaryCrossed)
        {
            DirectionHelper helper;
            int originalIndex;

            if (index < 0 || index > Count)
            {
                throw new ArgumentException("Sought index out of range.", "index");
            }

            helper = new DirectionHelper(direction);
            blockBoundaryCrossed = false;
            originalIndex = index;

            // If we are completely outside of any structural tags,
            // we need to get inside one.
            if (!IsInParagraph(index))
            {
                // Consider after-last-paragraph condition.
                if (includeSelectionOnly && IsAfterLastParagraph(index))
                {
                    return index;
                }

                index = SkipBlockSymbols(index, helper.Direction, helper.EnterElementContext);
                if (!IsInParagraph(index))
                {
                    index = SkipBlockSymbols(index, helper.OppositeDirection, helper.ExitElementContext);
                }
                if (index != originalIndex)
                {
                    blockBoundaryCrossed = true;
                }
            }

            // If this is already an insertion position, return true.
            if (IsInsertionPosition(index))
            {
                return index;
            }

            // Start by skipping formatting tags in one direction.
            // If this puts us in a non-insertion position (outside a Run),
            // snap back in the other direction.
            while (!IsInsertionPosition(index) && IsFormattingSymbol(index + helper.DirectionOffset))
            {
                index += helper.MoveDistance;
            }
            if (!IsInsertionPosition(index))
            {
                helper = helper.OppositeHelper;
                while (!IsInsertionPosition(index) && IsFormattingSymbol(index + helper.DirectionOffset))
                {
                    index += helper.MoveDistance;
                }
            }

            return index;
        }

        /// <summary>
        /// Gets the element that is most local to the specified position.
        /// </summary>
        /// <param name="index">The index position to start searching from.</param>
        /// <returns>The closest scoping element; null if none is available.</returns>
        public TextElement GetParentElement(int index)
        {
            int parentIndex;

            parentIndex = GetParentIndex(index);
            if (parentIndex == -1)
            {
                return null;
            }
            else
            {
                return this[parentIndex].Element;
            }
        }

        /// <summary>
        /// Gets the index of the element that is most local to the specified 
        /// position.
        /// </summary>
        /// <param name="index">The index position to start searching backwards from.</param>
        /// <returns>
        /// The index of the closest ElementStart symbol before index, -1 if
        /// there is none.
        /// </returns>
        public int GetParentIndex(int index)
        {
            int closeTagsFound; // Used to balance tags.

            if (index < 0 || index > Count)
            {
                throw new ArgumentException("Sought index out of range.", "index");
            }
            if (index == Count || index == 0)
            {
                return -1;
            }

            closeTagsFound = 0;
            index--;
            while (index >= 0)
            {
                if (this[index].Context == TextPointerContext.ElementStart)
                {
                    if (closeTagsFound == 0)
                    {
                        return index; 
                    }
                    else
                    {
                        closeTagsFound--;
                    }
                }
                else if (this[index].Context == TextPointerContext.ElementEnd)
                {
                    closeTagsFound++;
                }
                index--;
            }

            return -1;
        }

        /// <summary>Gets the run of text in which the specified index is in.</summary>
        /// <param name="index">Index to get run from.</param>
        /// <param name="run">On return, the run of text the index is in.</param>
        /// <param name="runStartIndex">On return, the index of the run start.</param>
        public void GetRun(int index, out string run, out int runStartIndex)
        {
            StringBuilder runBuilder;

            if (index < 0 || index > Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            // Verify that the index is in a run of text.
            if (this[index].Context != TextPointerContext.Text)
            {
                if (index == 0 || this[index -1].Context != TextPointerContext.Text)
                {
                    throw new ArgumentException("index", "Index is not in a text run.");
                }
                index--;
            }

            // Go back as far as possible within the run.
            while (index > 0 && this[index - 1].Context == TextPointerContext.Text)
            {
                index--;
            }
            
            // Put everything inside a string.
            runBuilder = new StringBuilder();
            runStartIndex = index;
            while (index < this.Count && this[index].Context == TextPointerContext.Text)
            {
                runBuilder.Append(this[index].Character);
                index++;
            }
            run = runBuilder.ToString();
        }

        /// <summary>
        /// Gets the computed value for the specified property for the symbol
        /// at the specified symbol.
        /// </summary>
        /// <param name="index">Index of symbol to get value for.</param>
        /// <param name="property">DependencyProperty to get value for.</param>
        /// <returns>The value for the specified property.</returns>
        /// <remarks>
        /// Getting the value for an element edge is equivalent to the values
        /// found inside the element. There is currently no way to get
        /// the default values from the container or on an empty
        /// element.
        /// </remarks>
        public object GetValue(int index, DependencyProperty property)
        {
            if (index < 0 || index > this.Count)
            {
                throw new ArgumentException("Symbol index out of range.", "index");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (index < this.Count)
            {
                int queryIndex; // Index of symbol being queried.

                queryIndex = index;
                do
                {
                    if (this[queryIndex].LocalValues.ContainsKey(property))
                    {
                        return this[queryIndex].LocalValues[property];
                    }
                    else
                    {
                        queryIndex = GetParentIndex(queryIndex);
                    }
                } while (queryIndex != -1);
            }

            return property.DefaultMetadata.DefaultValue;
        }

        /// <summary>
        /// Inserts a text element between the specified indices.
        /// </summary>
        /// <param name="startIndex">The index position of the insertion for the element start.</param>
        /// <param name="endIndex">The index position of the insertion for the element end.</param>
        /// <param name="element">The element to insert.</param>
        public void InsertElement(int startIndex, int endIndex, TextElement element)
        {
            int edgeBalance;    // Count of element edges start/end found.

            VerifyIndices(startIndex, endIndex, true);
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            edgeBalance = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (this[i].Context == TextPointerContext.ElementStart)
                {
                    edgeBalance++;
                }
                else if (this[i].Context == TextPointerContext.ElementEnd)
                {
                    edgeBalance--;
                }
            }

            if (edgeBalance != 0)
            {
                throw new InvalidOperationException("Cannot insert element in indexes " +
                    startIndex + ";" + endIndex + " - these positions are unbalanced.");
            }

            Insert(endIndex, new ArrayTextSymbol(element, false));
            Insert(startIndex, new ArrayTextSymbol(element, true));
        }

        /// <summary>
        /// Inserts text at the specified index.
        /// </summary>
        /// <param name="index">The index position of the insertion.</param>
        /// <param name="text">The text to insert.</param>
        /// <remarks>The text is inserted before the this[index] element.</remarks>
        public void InsertText(int index, string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (index < 0 || index > Count)
            {
                throw new ArgumentException("Insertion index out of range.", "index");
            }

            for (int i = 0; i < text.Length; i++)
            {
                Insert(i + index, new ArrayTextSymbol(text[i]));
            }
            VerifyConsistency();
        }

        /// <summary>Checks whether the specified index is in the list bounds.</summary>
        /// <param name="index">Index to evaluate.</param>
        /// <returns>true if index can be used to access a list element; false otherwise.</returns>
        public bool IsInBounds(int index)
        {
            return index >= 0 && index < this.Count;
        }

        /// <summary>Checks whether the symbol at the specified index has a Paragraph ancestor.</summary>
        /// <param name="index">Index of symbol to check.</param>
        /// <returns>
        /// true if the symbol at <paramref name="index"/> has a Paragraph 
        /// ancestor; false otherwise.
        /// </returns>
        public bool IsInParagraph(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new ArgumentException("Index out of range.", "index");
            }

            while (index != -1)
            {
                int parentIndex;

                parentIndex = GetParentIndex(index);
                if (parentIndex != -1)
                {
                    if (this[parentIndex].Element is Paragraph)
                    {
                        return true;
                    }
                }
                index = parentIndex;
            }
            return false;
        }

        /// <summary>Returns whether the specified symbol is a paragraph end.</summary>
        /// <param name="symbol">Symbol to evaluate.</param>
        /// <returns>true if the specified symbol is a paragraph end, false otherwise.</returns>
        public static bool IsSymbolParagraphEnd(ArrayTextSymbol symbol)
        {
            return symbol != null &&
                symbol.Context == TextPointerContext.ElementEnd &&
                symbol.Element.GetType() == typeof(Paragraph);
        }

        /// <summary>Returns whether the specified symbol is a paragraph start.</summary>
        /// <param name="symbol">Symbol to evaluate.</param>
        /// <returns>true if the specified symbol is a paragraph start, false otherwise.</returns>
        public static bool IsSymbolParagraphStart(ArrayTextSymbol symbol)
        {
            return symbol != null &&
                symbol.Context == TextPointerContext.ElementStart &&
                symbol.Element.GetType() == typeof(Paragraph);
        }

        /// <summary>Returns a list of parent symbols from the specified index.</summary>
        /// <param name="symbolIndex">Index of symbol to start from.</param>
        /// <returns>
        /// A list of parent symbols from the specified index, from nearest
        /// to furthest.
        /// </returns>
        public List<ArrayTextSymbol> ListParents(int symbolIndex)
        {
            List<ArrayTextSymbol> result;
            int parentIndex;

            result = new List<ArrayTextSymbol>();
            parentIndex = GetParentIndex(symbolIndex);
            while (parentIndex >= 0)
            {
                result.Add(this[parentIndex]);
                parentIndex = GetParentIndex(parentIndex);
            }

            return result;
        }

        /// <summary>
        /// Checks whether the specified container matches this container.
        /// </summary>
        /// <param name="container">Container to compare with.</param>
        public bool Matches(ArrayTextContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (this.Count != container.Count)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                if (!this[i].Matches(container[i]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>Whether the contents are plain text.</summary>
        public bool IsPlainText
        {
            get
            {
                // If there is no content or if the first
                // element is a Character (rather than a block
                // or inline element), then this is plain text.
                return this.Count == 0 || this[0].Context == TextPointerContext.Text;
            }
        }

        /// <summary>
        /// Plain text representation of the contents of the
        /// container.
        /// </summary>
        public string Text
        {
            get
            {
                StringBuilder builder;

                builder = new StringBuilder(this.Count);
                foreach (ArrayTextSymbol symbol in this)
                {
                    if (symbol.Context == TextPointerContext.Text)
                    {
                        builder.Append(symbol.Character);
                    }
                }
                return builder.ToString();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.Clear();
                AppendText(value);
            }
        }

        #endregion Public properties.


        #region Private methods.

        private void AppendText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            foreach (char c in text)
            {
                Add(new ArrayTextSymbol(c));
            }
        }

        /// <summary>
        /// Checks whether the specified insertion index is after
        /// the last paragraph.
        /// </summary>
        private bool IsAfterLastParagraph(int insertionIndex)
        {
            return insertionIndex > 0 && 
                this[insertionIndex-1].Context == TextPointerContext.ElementEnd &&
                this[insertionIndex-1].Element is Paragraph && 
                insertionIndex == this.Count;
        }

        /// <summary>
        /// Checks whether the symbol at the specified index is used
        /// for formatting purposes only (not blocks or text).
        /// </summary>
        private bool IsFormattingSymbol(int index)
        {
            if (index < 0 && index >= Count)
            {
                return false;
            }
            else if (this[index].IsElementEdge && 
                (this[index].Element is Run || this[index].Element is Span))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the specified index is a valid insertion position.
        /// </summary>
        private bool IsInsertionPosition(int index)
        {
            if (IsPlainText)
            {
                return true;
            }
            else
            {
                return GetParentElement(index) is Run;
            }
        }

        /// <summary>
        /// Returns the index of a position after skipping block elements 
        /// of a specified start / end context in a given direction.
        /// </summary>
        private int SkipBlockSymbols(int index, LogicalDirection direction,
            TextPointerContext context)
        {
            DirectionHelper helper;

            helper = new DirectionHelper(direction);
            while (
                IsInBounds(index + helper.DirectionOffset) &&
                this[index + helper.DirectionOffset].Context == context &&
                this[index + helper.DirectionOffset].Element is Block)
            {
                index += helper.MoveDistance;
            }

            return index;
        }

        /// <summary>Verifies that all elements are balanced correctly.</summary>
        private void VerifyConsistency()
        {
            Stack<Type> elementStack;
            Stack<int> indexStack;

            elementStack = new Stack<Type>();
            indexStack = new Stack<int>();
            for (int i = 0; i < Count; i++)
            {
                ArrayTextSymbol symbol;

                symbol = this[i];
                if (symbol.Context == TextPointerContext.ElementStart)
                {
                    elementStack.Push(symbol.Element.GetType());
                    indexStack.Push(i);
                }
                else if (symbol.Context == TextPointerContext.ElementEnd)
                {
                    Type closingType;
                    Type openingType;
                    int openingIndex;
                    
                    openingType = elementStack.Pop();
                    openingIndex = indexStack.Pop();
                    closingType = symbol.Element.GetType();

                    if (openingType != closingType)
                    {
                        throw new Exception("Invalid ArrayTextContainer state. " +
                            "Element end for " + closingType + " found at " + i +
                            ", which is not balanced with " + openingType + " found at " +
                            openingIndex + ".\r\n" + this.DescribeSymbols());
                    }
                }
            }

            if (elementStack.Count != 0)
            {
                Type openingType;
                int openingIndex;

                openingType = elementStack.Pop();
                openingIndex = indexStack.Pop();

                throw new Exception("Invalid ArrayTextContainer state. " +
                    "Finished iteration before an element end was found for " +
                    openingType + " found at " + openingIndex + ".");
            }
        }

        /// <summary>
        /// Verifies that the specified start and end index arguments are
        /// within range.
        /// </summary>
        /// <param name="startIndex">Start index argument.</param>
        /// <param name="endIndex">End index argument.</param>
        /// <param name="lastIsValid">Whether the last position is valid.</param>
        private void VerifyIndices(int startIndex, int endIndex, bool lastIsValid)
        {
            if (startIndex < 0 || startIndex > Count || (startIndex == Count && !lastIsValid))
            {
                throw new ArgumentException("Start index out of range.", "startIndex");
            }
            if (endIndex < 0 || endIndex > Count || (endIndex == Count && !lastIsValid))
            {
                throw new ArgumentException("End index out of range.", "endIndex");
            }
            if (startIndex > endIndex)
            {
                throw new ArgumentException("Start index after end index.", "startIndex");
            }
        }

        #endregion Private methods.



        #region Private fields.

        #endregion Private fields.


        #region Tests.

        /*
        [STAThread]
        static void Main()
        {
            TestArrayTextSymbolConstructor();
            TestArrayTextSymbolMatches();

            TestConstructor();
            TestText();
            TestDeleteContent();
            TestMatches();
            TestInsertText();
            TestLocalValues();
            TestGetValue();
            TestGetDepth();
            TestGetParentIndex();
            TestInsertElement();

            TestDependencyPropertyBagConstructor();
        }

        private static void Assert(bool condition, string conditionDescription)
        {
            if (!condition) throw new ApplicationException("Assertion failed: " + conditionDescription);
        }

        private static void TestArrayTextSymbolConstructor()
        {
            ArrayTextSymbol symbol;
            TextElement element;
            UIElement embeddedElement;

            symbol = new ArrayTextSymbol('Q');
            Assert(symbol.Character == 'Q', "Character is set by constructor");
            Assert(symbol.Context == TextPointerContext.Text, "Context for character is Text");

            element = new Bold();
            symbol = new ArrayTextSymbol(element, true);
            Assert(symbol.Element.GetType() == element.GetType(), "Element is set by constructor");
            Assert(symbol.Context == TextPointerContext.ElementStart, "Context for element start is ElementStart");

            symbol = new ArrayTextSymbol(element, false);
            Assert(symbol.Element.GetType() == element.GetType(), "Element is set by constructor");
            Assert(symbol.Context == TextPointerContext.ElementEnd, "Context for element end is ElementEnd");

            embeddedElement = new Button();
            symbol = new ArrayTextSymbol(embeddedElement);
            Assert(symbol.EmbeddedElement == embeddedElement, "Embedded element is set by constructor");
            Assert(symbol.Context == TextPointerContext.EmbeddedElement, "Context for embedded element is EmbeddedElement");
        }

        private static void TestArrayTextSymbolMatches()
        {
            Assert(new ArrayTextSymbol('a').Matches(new ArrayTextSymbol('a')), "Characters match as expected.");
            Assert(!new ArrayTextSymbol('a').Matches(new ArrayTextSymbol('b')), "Characters differ as expected.");
            Assert(!new ArrayTextSymbol('a').Matches(new ArrayTextSymbol(new Button())), "Different contexts differ as expected.");
            Assert(new ArrayTextSymbol(new Inline(), true).Matches(new ArrayTextSymbol(new Inline(), true)), "Elements match as expected.");
            Assert(!new ArrayTextSymbol(new Inline(), true).Matches(new ArrayTextSymbol(new Bold(), true)), "Elements differ as expected.");
            Assert(!new ArrayTextSymbol(new Inline(), false).Matches(new ArrayTextSymbol(new Inline(), true)), "Element edges differ as expected.");
        }

        private static void TestConstructor()
        {
            ArrayTextContainer container;
            ArrayTextContainer clonedContainer;
            RichTextBox control;

            new ArrayTextContainer();
            new ArrayTextContainer(new RichTextBox().ContentStart);

            control = new RichTextBox();
            control.ContentStart.InsertText("foo");
            container = new ArrayTextContainer(control.ContentStart);
            Assert(container.Text == "foo", "Constructor initializes text from container.");
            
            new Bold().Reposition(control.ContentStart, control.ContentEnd);
            container = new ArrayTextContainer(control.ContentStart);
            Assert(container.Text == "foo", "Plain text discard rich content.");
            Assert(container[0].Context == TextPointerContext.ElementStart, "Constructor loads elements.");
            Assert(container[container.Count-1].Context == TextPointerContext.ElementEnd, "Constructor loads elements.");

            control.ContentStart.InsertEmbeddedElement(new Button());
            container = new ArrayTextContainer(control.ContentEnd);
            Assert(container[0].Context == TextPointerContext.EmbeddedElement, "Constructor loads embedded elements.");

            clonedContainer = new ArrayTextContainer(container);
            Assert(container[0].Context == clonedContainer[0].Context, "First element in container matches.");

            container = new ArrayTextContainer("baa");
            Assert(container.Text == "baa", "Constructor initializes valid text.");
        }

        private static void TestText()
        {
            ArrayTextContainer container;

            container = new ArrayTextContainer();
            Assert(container.Text == "", "Text is empty");

            container.Text = "My text";
            Assert(container.Text == "My text", "Text can be set");
        }

        private static void TestDeleteContent()
        {
            ArrayTextContainer container;

            container = new ArrayTextContainer();
            container.Text = "0123456789";
            container.DeleteContent(0, 0);
            Assert(container.Text == "123456789", "Delete works on first character.");
            container.DeleteContent(1, 3);
            Assert(container.Text == "156789", "Delete works on middle characters.");
        }

        private static void TestMatches()
        {
            ArrayTextContainer container;
            ArrayTextContainer otherContainer;

            container = new ArrayTextContainer();
            container.Text = "foo";

            otherContainer = new ArrayTextContainer(container);
            Assert(container.Matches(otherContainer), "Cloned containers match.");
            container[0] = new ArrayTextSymbol('Q');
            Assert(!container.Matches(otherContainer), "Different containers do not match.");
        }

        private static void TestInsertText()
        {
            ArrayTextContainer container;

            container = new ArrayTextContainer();
            container.InsertText(0, "foo");
            Assert(container.Text == "foo", "InsertText inserts at start.");
        }

        private static void TestLocalValues()
        {
            ArrayTextSymbol symbol;

            symbol = new ArrayTextSymbol('f');
            Assert(symbol.LocalValues.Count == 0, "Character symbols have no local values.");
        }

        private static void TestGetValue()
        {
            ArrayTextContainer container;
            Inline inline;

            container = new ArrayTextContainer("text");
            inline = new Inline();
            inline.SetValue(Inline.FontFamilyProperty, "Verdana");
            container.InsertElement(1, container.Count, inline);
            Assert(container.GetValue(0, Inline.FontFamilyProperty) == Inline.FontFamilyProperty.DefaultMetadata.DefaultValue,
                "Default values retrieved when there is no parent element.");
            Assert(container.GetValue(1, Inline.FontFamilyProperty).ToString() == "Verdana",
                "Correct FontFamily value retrieved from element edge.");
            Assert(container.GetValue(2, Inline.FontFamilyProperty).ToString() == "Verdana",
                "Correct FontFamily value retrieved from inside element.");
        }

        private static void TestGetDepth()
        {
            ArrayTextContainer container;

            container = new ArrayTextContainer("ab12cd");
            Assert(container.GetDepth(0) == 0, "Depth at start is zero.");

            container.InsertElement(0, 1, new Inline());
            Assert(container.GetDepth(0) == 0, "Depth at start is still zero.");
            Assert(container.GetDepth(1) == 1, "Depth in inline is one.");
            Assert(container.GetDepth(2) == 0, "Depth at end is zero.");
            Assert(container.GetDepth(3) == 0, "Depth after end is zero.");
        }

        private static void TestGetParentIndex()
        {
            ArrayTextContainer container;

            container = new ArrayTextContainer("ab12cd");
            Assert(container.GetParentIndex(0) == -1, "Parent at start is -1.");

            container.InsertElement(0, 1, new Inline());
            Assert(container.GetParentIndex(0) == -1, "Parent at start is still -1.");
            Assert(container.GetParentIndex(1) == 0, "Parent in inline is 0.");
            Assert(container.GetParentIndex(2) == -1, "Parent at end is -1.");
            Assert(container.GetParentIndex(3) == -1, "Parent after end is -1.");
        }

        private static void TestInsertElement()
        {
            ArrayTextContainer container;

            container = new ArrayTextContainer();
            container.InsertElement(0, 0, new Bold());
            Assert(container[0].Context == TextPointerContext.ElementStart, "Element start at beginning");
            Assert(container[1].Context == TextPointerContext.ElementEnd, "Element end at end");

            try
            {
                container.InsertElement(0, 1, new Inline());
                Assert(false, "Unbalanced InsertElement does not throw an exception.");
            }
            catch (SystemException)
            {
                Assert(container.Count == 2, "Unbalanced InsertElement rejected.");
            }
            container.InsertElement(0, 2, new Inline());
            Assert(container[0].Context == TextPointerContext.ElementStart, "Element start for Inline found");
            Assert(container[0].Element.GetType() == typeof(Inline), "Element start for Inline found");
            Assert(container[1].Context == TextPointerContext.ElementStart, "Element start for Bold found");
            Assert(container[1].Element.GetType() == typeof(Bold), "Element start for Bold found");
            Assert(container[2].Context == TextPointerContext.ElementEnd, "Element end for Bold found");
            Assert(container[2].Element.GetType() == typeof(Bold), "Element end for Bold found");
            Assert(container[3].Context == TextPointerContext.ElementEnd, "Element end for Inline found");
            Assert(container[3].Element.GetType() == typeof(Inline), "Element end for Inline found");

            // Direct case taken from test case.
            container = new ArrayTextContainer();
            container.InsertElement(0, 0, new Inline());
            container[0].LocalValues[Inline.FontFamilyProperty] = "my font";
            container.InsertText(1, "a");
            Assert(container.Count == 3, "Three symbols in the container.");
            Assert(container[0].Context == TextPointerContext.ElementStart, "Element start found.");
            Assert(container[1].Context == TextPointerContext.Text, "Character found.");
            Assert(container[2].Context == TextPointerContext.ElementEnd, "Element end found.");
        }

        private static void TestDependencyPropertyBagConstructor()
        {
            DependencyPropertyBag bag;
            Button button;

            button = new Button();
            button.FontFamily = "Verdana";
            bag = new DependencyPropertyBag();
            bag.ReadLocalValues(button);
            Assert(bag[Button.FontFamilyProperty].ToString() == "Verdana", "FromLocalValues gets local values.");

            button = new Button();
            bag[Button.AllowDropProperty] = true;
            bag.ApplyValues(button);
            Assert(button.FontFamily == "Verdana", "ApplyValues sets local values.");
            Assert(button.AllowDrop, "ApplyValues sets local values.");
        }
*/

        #endregion Tests.
    }
}