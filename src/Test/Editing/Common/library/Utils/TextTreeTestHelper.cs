// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for public API of the TextRange class.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Text;
    using System.Collections;
    using System.Threading; using System.Windows.Threading;
    using System.Reflection;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Test.Uis.Data;

    #endregion Namespaces.

    /// <summary>
    /// The TextContainer type needs to be built.
    /// </summary>
    public enum TextRangeType
    {
        /// <summary>
        /// Return TextRange covering from TextContainer.Start to TextContainer.End
        /// </summary>
        WholeTextContainer,

        /// <summary>
        /// Return TextRange at TextContainer.Start
        /// </summary>
        CollapsedAtStart,

        /// <summary>
        /// Return TextRange at TextContainer.End
        /// </summary>
        CollapsedAtEnd
    }

    /// <summary>
    /// Verifies that when the end of a range is set, no asserts are fired.
    /// </summary>
    [TestOwner("Microsoft")]
    public class TextTreeTestHelper
    {
        /// <summary>
        /// Property name to be excluded from comparing.
        /// </summary>
        private static string[] s_propertyNamesFilter = 
            {
                "Text"
            };
        
        /// <summary>
        /// Obtain TextRange in representing a range in TextContainer supplied.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="textRangeTypeStr"></param>
        /// <returns></returns>
        public static TextRange GetTextRange(TextPointer start, TextPointer end, string textRangeTypeStr)
        {
            Debug.Assert(start != null);
            Debug.Assert(end != null);

            TextRangeType textRangeType = (TextRangeType)Enum.Parse(typeof(TextRangeType), textRangeTypeStr);
            TextRange textRange;

            switch (textRangeType)
            {
                case TextRangeType.WholeTextContainer:
                    textRange = new TextRange(start, end);
                    break;

                case TextRangeType.CollapsedAtStart:
                    textRange = new TextRange(start, start);
                    break;

                case TextRangeType.CollapsedAtEnd:
                    textRange = new TextRange(end, end);
                    break;

                default:
                    throw new InvalidOperationException("Unknown TextRangeType");
            }
            return textRange;
        }

        /// <summary>
        /// This method creates a number of TextPointers in the supplied TextRange so
        /// that the node can be split.
        /// </summary>
        /// <param name="textRange"></param>
        /// <param name="splitByHowMany"></param>
        /// <returns></returns>
        public static int SplitTextNode(TextRange textRange, int splitByHowMany)
        {
            TextPointer pointer;
            Debug.Assert(splitByHowMany > 0);

            if (splitByHowMany == 1)
            {
                return 1;
            }

            // Get an idea what it is in the TextRange
            TextPointer currentNavigator = textRange.Start;
            TextPointerContext symbolType = currentNavigator.GetPointerContext(LogicalDirection.Forward);

            if (symbolType != TextPointerContext.Text)
            {
                throw new InvalidOperationException("This method is designed to split TextNode only");
            }

            int textLength = textRange.Text.Length;

            if (splitByHowMany > textLength)
            {
                splitByHowMany = textLength;
            }

            int [] splitArray = new int[splitByHowMany];
            int remainingNumberCharToSplit = textLength;
            int necessarySplits = splitByHowMany;
            int possibleNumberOfChar = 0;

            while (necessarySplits > 0)
            {
                if (remainingNumberCharToSplit % necessarySplits == 0)
                {
                    int averageNumberOfChar = remainingNumberCharToSplit / necessarySplits;

                    for (int i = splitByHowMany - necessarySplits; i < splitByHowMany; i++)
                    {
                        splitArray[i] = averageNumberOfChar;
                    }

                    break;
                }
                else
                {
                    possibleNumberOfChar = remainingNumberCharToSplit / necessarySplits;
                    splitArray[splitByHowMany - necessarySplits] = ++possibleNumberOfChar;
                    necessarySplits--;
                    remainingNumberCharToSplit -= possibleNumberOfChar;
                }
            }

            int currentOffset = 0;
            for (int i = 0; i < splitArray.Length - 1; i++)
            {
                currentOffset += splitArray[i];

                pointer = textRange.Start;

                pointer = pointer.GetPositionAtOffset(currentOffset);
                s_arrayListTextPointers.Add(pointer);
            }

            return splitByHowMany;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static DependencyProperty[] GetListOfPropertiesToBeCompared(Type type)
        {
            DependencyProperty[] propertyList;

            if (type ==typeof( Hyperlink))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.HyperlinkPropertyData) ;
            }
            else if (type ==typeof( Paragraph))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.ParagraphPropertyData);
            }
            else if (type == typeof(List))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.ListPropertyData);
            }
            else if (type ==typeof( ListItem))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.ListItemPropertyData);
            }
            else if (type ==typeof(Table))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.TablePropertyData);
            }
            else if (type == typeof(TableRowGroup))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.TableRowGroupPropertyData);
            }
            else if (type  ==typeof( TableColumn))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.TableColumnPropertyData);
            }
            else if (type  ==typeof( TableRow))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.TableRowPropertyData);
            }
            else if (type  ==typeof( TableCell))
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.TableCellPropertyData);
            }
            else if (type.IsAssignableFrom(typeof( Block)))
            {
                //this should be after all the existing block element.
                //blockUIContainer does not have local defiened properties.
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.BlockPropertyData);
            }
            else if (type.IsAssignableFrom(typeof(Inline)))
            {
                //This should be after all the Inline element.
                //Run, InlineUIContainer, Span etc don't have local defined properties. so treat them as Inline
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.InlinePropertyData);
            }
            else
            {
                propertyList = DependencyPropertyData.GetPropertiesFromPropertyData(DependencyPropertyData.TextElementPropertyData);
            }

            return propertyList;
        }

        /// <summary>
        /// function to loop through the list of propertyList (in this file)
        /// array. Return if the type passed is found in the array, false otherwise
        /// </summary>
        /// <param name="propertyList">DependencyProperties that need to be compared.</param>
        /// <param name="info">Property info of this property</param>
        /// <returns>true if the type is found, false otherwise</returns>
        private static bool IsPropertyToBeCompared(DependencyProperty [] propertyList, 
            PropertyInfo info)
        {
            bool shouldCompare;

            shouldCompare = false;

            // See if this property is the one we want to compare
            foreach (DependencyProperty property in propertyList)
            {
                if (info.Name == property.Name)
                {
                    shouldCompare = true;
                    break;
                }
            }

            return shouldCompare;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textBuffer1"></param>
        /// <param name="textBuffer2"></param>
        /// <returns></returns>
        private static bool Compare(char[] textBuffer1, char[] textBuffer2)
        {
            int i;
            bool equal;
            
            equal = true;

            for (i = 0; i < textBuffer1.Length && i < textBuffer2.Length; i++)
            {
                if (textBuffer1[i] != textBuffer2[i])
                {
                    equal = false;
                    break;
                }
            }

            if (i < textBuffer1.Length - 1
                || i < textBuffer2.Length - 1)
            {
                equal = false;
            }

            return equal;
        }

        /// <summary>
        /// CompareTextRangeContents comapres two TextRanges for restricted object equality.
        /// The function return true if the two TextRanges:
        /// 1. have the same Text, ElementStart, ElementEnd and EmbeddedElement
        /// 2. the same values of the hardcoded list of attached properties of TextElement 
        /// false otherwise.
        /// </summary>
        /// <param name="start1">TextPointer start for source</param>
        /// <param name="end1">TextPointer end for source</param>
        /// <param name="start2">TextPointer start for target</param>
        /// <param name="end2">TextPointer end for target</param>
        /// <param name="unmatchReason">if the function returns false, this is the reason of unmatch in the form of string</param>
        /// <returns></returns>
        public static bool CompareTextRangeContents(TextPointer start1,
            TextPointer end1,
            TextPointer start2,
            TextPointer end2,
            out string unmatchReason)
        {
            TextPointerContext contextType1;
            TextPointerContext contextType2;

            TextPointer navigator1;
            TextPointer navigator2;
            LogicalDirection direction;
            TextElement textElement1;
            TextElement textElement2;
            PropertyInfo[] propertyInfo;
            object object1;
            object object2;
            bool done;
            bool equal;
            int numberOfCharsInThisRun;
            char[] textBuffer1;
            char[] textBuffer2;
            DependencyProperty[] propertyList;
            TextPointer tempPointer1;
            TextPointer tempPointer2;
            Type typeTextElement1;
            Type typeTextElement2;

            done = false;
            equal = true;
            unmatchReason = String.Empty;

            direction = LogicalDirection.Forward;
            Debug.Assert(start1.CompareTo(end1) <= 0 && start2.CompareTo(end2) <= 0);

            navigator1 = start1;
            navigator2 = start2;

            // we loop until done is true
            while (!done
                && navigator1.CompareTo(end1) < 0
                && navigator2.CompareTo(end2) < 0)
            {
                contextType1 = navigator1.GetPointerContext(direction);
                contextType2 = navigator2.GetPointerContext(direction);

                switch (contextType1)
                {
                    case TextPointerContext.Text:
                        // compare if it is TextBlock for both of the TextRange                        
                        equal = contextType2 == TextPointerContext.Text;

                        // if this is equal, we compare the text from current pointer
                        // up to TextRange.End if TextRange.End is before the end of current
                        // text run, otherwise we compare the current Text run
                        if (equal)
                        {
                            numberOfCharsInThisRun = navigator1.GetTextInRun(direction).Length;

                            if (numberOfCharsInThisRun > navigator1.GetOffsetToPosition(end1))
                            {
                                numberOfCharsInThisRun = navigator1.GetOffsetToPosition(end1);
                                textBuffer1 = new char[numberOfCharsInThisRun];
                                textBuffer2 = new char[numberOfCharsInThisRun];
                                navigator1.GetTextInRun(direction, textBuffer1, 0, numberOfCharsInThisRun);
                                navigator2.GetTextInRun(direction, textBuffer2, 0, numberOfCharsInThisRun);
                                equal = Compare(textBuffer1, textBuffer2);
                            }
                            else
                            {
                                equal = navigator1.GetTextInRun(direction) == navigator2.GetTextInRun(direction);
                            }
                        }

                        // they are not equal, set unmatchReason and return
                        if (!equal)
                        {
                            unmatchReason = "Text is not the same on both TextTree";
                        }
                        break;
                    case TextPointerContext.ElementStart:
                        // compare if it is ElementStart for both of the TextRanges
                        equal = contextType2 == TextPointerContext.ElementStart;

                        // then compare the type of the TextElement
                        if (equal)
                        {
                            tempPointer1 = navigator1;
                            tempPointer2 = navigator2;

                            tempPointer1 = tempPointer1.GetNextContextPosition(direction);
                            tempPointer2 = tempPointer2.GetNextContextPosition(direction);
                            
                            textElement1 = tempPointer1.Parent as TextElement;
                            textElement2 = tempPointer2.Parent as TextElement;

                            // Convert textElement1 and textElement2 to be
                            // some comparable types (e.g. Bold and Italic will be 
                            // converted to Inlines with different property values set
                            // and then the converted types are compared
                            typeTextElement1 = GetCompatabileTextElementType(textElement1.GetType());
                            typeTextElement2 =GetCompatabileTextElementType(textElement2.GetType());

                            equal = typeTextElement1.Equals(typeTextElement2);
                        }

                        if (!equal)
                        {
                            unmatchReason = "TextRange1 hits on ElementStart but TextRange2 doesn't";
                        }
                        break;
                    case TextPointerContext.ElementEnd:

                        // compare if it is ElementEnd for both of the TextRanges
                        equal = contextType2 == TextPointerContext.ElementEnd;
                        if (equal)
                        {
                            // it should be TextElement
                            // if this type cast fails, suspeciously a corruption in TextContainer
                            textElement1 = (TextElement)navigator1.Parent;
                            textElement2 = (TextElement)navigator2.Parent;

                            // retrieve comparable type for TextElement1 and textElement2
                            typeTextElement1 = GetCompatabileTextElementType(textElement1.GetType());
                            typeTextElement2 = GetCompatabileTextElementType(textElement2.GetType());

                            // textElement1 / 2 should not be null
                            equal = typeTextElement2.Equals(typeTextElement2);
                            if (equal)
                            {
                                // get the public attached property list for comparable type
                                // now typeTextElement1 is the same as typeElement2
                                propertyInfo = typeTextElement1.GetProperties();

                                propertyList = GetListOfPropertiesToBeCompared(typeTextElement1);

                                // go through each of the items in the list 
                                // to see if they match the type we want to compare.
                                foreach (PropertyInfo info in propertyInfo)
                                {
                                    if (IsPropertyToBeCompared(propertyList, info))
                                    {
                                        // we want to compare this property, get
                                        // the respective from two TextElements
                                        object1 = ReflectionUtils.GetProperty(textElement1,
                                            info.Name);
                                        object2 = ReflectionUtils.GetProperty(textElement2,
                                            info.Name);

                                        // if both object1 and object2 are null
                                        // they are treated as equal
                                        if (object1 == null && object2 == null)
                                        {
                                            equal = true;
                                        }
                                        else if (object1 != null)
                                        {
                                            // if object1 is not null,
                                            // check for their object eqaulity
                                            equal = object1.Equals(object2);
                                        }

                                        if (!equal 
                                            && object1 != null
                                            && object2 != null)
                                        {
                                            equal = object1.ToString() == object2.ToString();
                                        }

                                        if (!equal)
                                        {
                                            unmatchReason = "Property "
                                                + info.Name
                                                + " do not match on both TextRanges";
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                unmatchReason = "Element type is not the same on both TextRanges";
                            }
                        }
                        else
                        {
                            unmatchReason = "TextRange1 hits on ElementEnd but TextRange2 doesn't";
                        }
                        break;
                    case TextPointerContext.EmbeddedElement:
                        // compare if it is EmbeddedElement for both TextRanges
                        equal = contextType2 == TextPointerContext.EmbeddedElement;
                        if (equal)
                        {
                            // compare the type of two embedded elements.
                            object1 = navigator1.GetAdjacentElement(direction).GetType();
                            object2 = navigator2.GetAdjacentElement(direction).GetType();

                            // we treat them equal if the type of two embedded objects are equal
                            // this should be enough for TextRange serialization testing
                            equal = object1.Equals(object2);

                            if (!equal)
                            {
                                unmatchReason = "EmbededElement type do not match on both TextTrees";
                            }
                        }
                        else
                        {
                            unmatchReason = "TextRange1 hits on EmbeddedElement but TextRange2 doesn't";
                        }
                        break;
                    case TextPointerContext.None:

                        // check if both TextRanges reach the end
                        equal = contextType2 == TextPointerContext.None;

                        if (!equal)
                        {
                            unmatchReason = "TextRange1 hits on TextPointerContext.None but TextRange2 doesn't";
                        }
                        done = true;
                        break;
                }

                // if there is inequality detected we don't need to move any further
                // get ready to exit.
                if (!equal)
                {
                    done = true;
                }

                navigator1 = navigator1.GetNextContextPosition(direction);
                navigator2 = navigator2.GetNextContextPosition(direction);
            }

            return equal;
        }

        private static Type GetCompatabileTextElementType(Type type)
        {
            Type t;

            t = null;
            if(type == typeof(Paragraph) ||
                type == typeof(Hyperlink) ||
                type == typeof(List) ||
                type == typeof(ListItem) ||
                type == typeof(Table) ||
                type == typeof(TableRowGroup) ||
                type == typeof(TableCell) ||
                 type == typeof(TableRow) ||
                type == typeof(Run) ||
                type == typeof(Span) ||
                type == typeof(InlineUIContainer) ||
                type == typeof(BlockUIContainer))
            {
                return type;
            }

            else if (typeof(Span).IsAssignableFrom(type))
            {
                //Must be after all other standard inline elements such as Hyperlink, Bold, etc.
                t = typeof(Span);
            }
            else if (typeof(Inline).IsAssignableFrom(type))
            {
                t = typeof(Inline);
            }
            else if (typeof(Block).IsAssignableFrom(type))
            {
                // This check must be the last in sequence to not miss Paragraphs, ListItems etc.
                t = typeof(Block);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("WARNING: Tag " + type.ToString() + " UNKNOWN. Mapping this to Inline");
                t = typeof(Inline);
            }

            return t;
        }        



        /// <summary>
        /// The method walks TextRanges and call Delegates when it hits on different 
        /// TextContext, empty TextRange will not call into any of the delegates
        /// </summary>
        /// <param name="start">TextPointer start</param>
        /// <param name="end">TextPointer end</param>
        /// <param name="args">argument to be passed to the delegates</param>
        /// <param name="startTreeWalkDelegate">Delegate to be called when tree walk starts</param>
        /// <param name="textContextProcessor">Del egate to be called when it is TextPointerContent.Text, can be null</param>
        /// <param name="elementStartContextProcessor">Delegate to be called when it is TextPointerContent.ElementStart, can be null</param>
        /// <param name="elementEndContextProcessor">Delegate to be called when it is TextPointerContent.ElementEnd, can be null</param>
        /// <param name="embeddedElementContextProcessor">Delegate to be called when it is TextPointerContent.EmbeddedElement, can be null</param>
        /// <param name="finishTreeWalkProcessor">Delegate to be called when Tree walk is done</param>
        public static void WalkTextRange(TextPointer start,
            TextPointer end,
            object[] args,
            Delegate startTreeWalkDelegate,
            Delegate textContextProcessor,
            Delegate elementStartContextProcessor,
            Delegate elementEndContextProcessor,
            Delegate embeddedElementContextProcessor,
            Delegate finishTreeWalkProcessor)
        {
            TextPointer currentPointer;
            bool done;
            TextPointerContext pointerContext;
            object[] realArgs;
            TextPointer tempPointer;
            int textLengthInThisRun;
            char[] buffer;
            LogicalDirection dir;
            DependencyObject dObj;
            int temp;

            Debug.Assert(start.CompareTo(end) <= 0);

            dir = end.CompareTo(start) >= 0
                ? LogicalDirection.Forward
                : LogicalDirection.Backward;

            // get the starting pointer
            currentPointer = start;

            // if TextRange is not an empty range, we need to walk the tree,
            // and we need to fire start tree walk delegate
            if (startTreeWalkDelegate != null && 
                (dir == LogicalDirection.Forward && currentPointer.CompareTo(end) < 0)
                || (dir == LogicalDirection.Backward && currentPointer.CompareTo(start) > 0))
            {
                realArgs = new object[2];
                realArgs[0] = currentPointer.GetPositionAtOffset(0, dir);                
                realArgs[1] = args;
                startTreeWalkDelegate.DynamicInvoke(realArgs);
            }

            done = false;
            realArgs = new object[3];
            textLengthInThisRun = 0;

            realArgs[2] = args;


            while (!done &&
                ((dir == LogicalDirection.Forward && currentPointer.CompareTo(end) < 0)
                || (dir == LogicalDirection.Backward && currentPointer.CompareTo(start) > 0)))
            {
                pointerContext = currentPointer.GetPointerContext(dir);

                realArgs[0] = currentPointer.GetPositionAtOffset(0, dir);                
                
                switch (pointerContext)
                {
                    case TextPointerContext.Text:
                        if (textContextProcessor != null)
                        {
                            // the tempPointer is trying to identify the end point
                            // of current context. If the end point is beyond TextRange.End
                            // it is getting only portion of text
                            tempPointer = currentPointer;

                            tempPointer = tempPointer.GetNextContextPosition(dir);

                            if ((tempPointer.CompareTo(end) >= 0 && dir == LogicalDirection.Forward)
                                || (tempPointer.CompareTo(start) <= 0 && dir == LogicalDirection.Backward))
                            {
                                textLengthInThisRun = currentPointer.GetOffsetToPosition(dir == LogicalDirection.Forward
                                    ? end
                                    : start);

                                buffer = new char[textLengthInThisRun];

                                temp = currentPointer.GetTextInRun(dir,
                                    buffer,
                                    0,
                                    textLengthInThisRun);

                                Debug.Assert(temp == textLengthInThisRun);

                                // set realArgs[1] to be the string
                                realArgs[1] = new string(buffer);

                                // we have got to the end
                                done = true;
                            }
                            else
                            {
                                // set realArgs[1] to be the string
                                realArgs[1] = currentPointer.GetTextInRun(dir);
                            }

                            //invoke delegate
                            done = (bool)textContextProcessor.DynamicInvoke(realArgs) || done;
                        }
                        break;
                    case TextPointerContext.ElementStart:
                        if (elementStartContextProcessor != null)
                        {
                            // now currentPointer is still outside the element block
                            // get tempPointer and move into the block so that we can get
                            // the name of the block
                            tempPointer = currentPointer;

                            tempPointer = tempPointer.GetNextContextPosition(dir);

                            // it should not be zero since GetPointerContext returns ElementStart
                            // which means it is not the end of the TextTree.
                            Debug.Assert(tempPointer.GetOffsetToPosition(currentPointer) != 0);

                            dObj = tempPointer.Parent;

                            Debug.Assert(dObj is TextElement);

                            realArgs[1] = dObj as TextElement;

                            done = (bool)elementStartContextProcessor.DynamicInvoke(realArgs);
                        }
                        break;
                    case TextPointerContext.ElementEnd:
                        if (elementEndContextProcessor != null)
                        {
                            dObj = currentPointer.Parent;

                            Debug.Assert(dObj is TextElement);

                            realArgs[1] = dObj as TextElement;

                            done = (bool)elementEndContextProcessor.DynamicInvoke(realArgs);
                        }
                        break;
                    case TextPointerContext.EmbeddedElement:
                        if (embeddedElementContextProcessor != null)
                        {
                            dObj = currentPointer.GetAdjacentElement(dir);
                            realArgs[1] = dObj as DependencyObject;
                            done = (bool)embeddedElementContextProcessor.DynamicInvoke(realArgs);
                        }
                        break;
                    case TextPointerContext.None:
                        done = true;
                        break;
                }

                if (!done)
                {
                    currentPointer = currentPointer.GetNextContextPosition(dir);
                }
                else
                {
                    if (finishTreeWalkProcessor != null)
                    {
                        realArgs = new object[2];
                        realArgs[0] = currentPointer.GetPositionAtOffset(0, dir);                        
                        realArgs[1] = args;
                        finishTreeWalkProcessor.DynamicInvoke(realArgs);
                    }
                }
            }
        }

        /// <summary>
        /// Delegate to be called to start walking the tree
        /// </summary>
        /// <param name="frozenTextPointer">Frozen Start TextPointer of the TextRange</param>
        /// <param name="args"></param>
        public delegate void StartTreeWalkDelegate(TextPointer frozenTextPointer, object[] args);

        /// <summary>
        /// delegate to be called when TextPointerContext.Text is met
        /// </summary>
        /// <param name="frozenTextPointer">starting TextPointer of this context</param>
        /// <param name="content">string in this context, please note that frozenTextPointer.GetText doesn't
        /// necessarily to be the same as content since TextRange.End can be somewhere in this TextContext</param>
        /// <param name="args">argument passed to WalkTextRange</param>
        /// <returns>return true if further traversal is desired, false otherwise</returns>
        public delegate bool ProcessTextContextDelegate(TextPointer frozenTextPointer, string content, object[] args);

        /// <summary>
        /// delegate to be called when TextPointerContext.ElementStart is met
        /// </summary>
        /// <param name="frozenTextPointer">starting TextPointer of this context</param>
        /// <param name="element">element object associated with this context</param>
        /// <param name="args">argument passed to WalkTextRange</param>
        /// <returns>return true if further traversal is desired, false otherwise</returns>
        public delegate bool ProcessElementStartContextDelegate(TextPointer frozenTextPointer, TextElement element, object[] args);

        /// <summary>
        /// delegate to be called when TextPointerContext.ElementEnd is met
        /// </summary>
        /// <param name="frozenTextPointer">starting TextPointer of this context</param>
        /// <param name="element">element object associated with this context</param>
        /// <param name="args">argument passed to WalkTextRange</param>
        /// <returns>return true if further traversal is desired, false otherwise</returns>
        public delegate bool ProcessElementEndContextDelegate(TextPointer frozenTextPointer, TextElement element, object[] args);

        /// <summary>
        /// delegate to be called when TextPointerContext.EmbeddedElement is met
        /// </summary>
        /// <param name="frozenTextPointer">starting TextPointer of this context</param>
        /// <param name="dObj">DependencyObject associated with this context</param>
        /// <param name="args">argument passed to WalkTextRange</param>
        /// <returns>return true if further traversal is desired, false otherwise</returns>
        public delegate bool ProcessEmbeddedElementContextDelegate(TextPointer frozenTextPointer, DependencyObject dObj, object[] args);

        /// <summary>
        /// delegate to be called when Tree walk is done
        /// </summary>
        /// <param name="frozenTextPointer">starting TextPointer of this context</param>
        /// <param name="args">argument passed to WalkTextRange</param>
        public delegate void FinishTreeWalkDelegate(TextPointer frozenTextPointer, object [] args);

        /// <summary>
        /// This ArrayList is to keep those instantiated TextPointers in memory
        /// When GC collects the dead TextPointers TextContainer merges the split TextNodes
        /// internally. For testing purposes we might want a TextRange spanning through a couple
        /// of different TextNodes, and by keeping the references of TextPointers we can achieve
        /// this purpose
        /// </summary>
        private static ArrayList s_arrayListTextPointers = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        private delegate bool CompareMethod(object object1, object object2);
    }

    /// <summary>
    /// Static class to help find text in TextRanges
    /// </summary>
    public static class TextContentFinder
    {
        /// <summary>
        /// Find text content in TextRange. This works even when it goes across TextElement boundary
        /// (but not EmbeddedElement boundary)
        /// The function does IndexOf for each TextContext, but when it goes across the Element boundary
        /// it stores the last pattern.Length - 1 characters in remainngTextToSearch. This is to workaround
        /// the case when the pattern text happens to go across element boundary. 
        /// </summary>
        /// <param name="start">TextPointer start</param>
        /// <param name="end">TextPointer end</param>
        /// <param name="pattern">string pattern to look for</param>
        /// <returns>TextPointer right before the first occurrance starts</returns>
        public static TextPointer FindString(TextPointer start, TextPointer end, string pattern)
        {
            TextPointer textPointer;
            TextPointer carryOverTextPointer;
            string remainingTextToSearch;
            object[] args;

            // TextPointer to return
            textPointer = null;

            // TextPointer to start searching for next Text context
            carryOverTextPointer = null;

            remainingTextToSearch = String.Empty;
            args = new object[] { textPointer, pattern, remainingTextToSearch, carryOverTextPointer };

            TextTreeTestHelper.WalkTextRange(start,
                end,
                args,
                null, /* Start Tree walk delegate */
                new TextTreeTestHelper.ProcessTextContextDelegate(ExamineText), /*process Text */
                new TextTreeTestHelper.ProcessElementStartContextDelegate(ExamineElementStart), /* Process ElementStart */
                new TextTreeTestHelper.ProcessElementEndContextDelegate(ExamineElementEnd), /* Process ElementEnd */
                new TextTreeTestHelper.ProcessEmbeddedElementContextDelegate(ExmainEmbeddedElement), /* Process EmbeddedElement */
                null /* Finish tree walk delegate */);

            return args[0] as TextPointer;
        }

        private static bool ExamineText(TextPointer frozenTextPointer, string content, object[] args)
        {
            string pattern;
            string text;
            TextPointer textPointer;
            TextPointer carryOverTextPointer;
            int index;
            bool done;

            textPointer = args[0] as TextPointer;
            pattern = args[1] as string;
            text = (string)args[2] + content;
            carryOverTextPointer = args[3] as TextPointer;

            // we need to start searching from here
            if (carryOverTextPointer == null)
            {
                carryOverTextPointer = frozenTextPointer;
            }

            done = false;

            Debug.Assert(textPointer == null);
            
            index = text.IndexOf(pattern);

            // the occurrance found
            // set textPointer as the value to be returned
            // return done = true so that we don't walk the TextRange anymore
            if (index != -1)
            {
                textPointer = carryOverTextPointer.GetPositionAtOffset(index);
                args[0] = textPointer;
                done = true;
            }              
            else
            {
                // the occurrance not found
                // now we need to prepare text for the next search
                // since it is possible that the pattern goes across element boundary 
                // the longest possible for the pattern to "extend" into this text context
                // is pattern.Length - 1
                if (text.Length >= pattern.Length)
                {
                    args[2] = (object)(text.Substring(text.Length - pattern.Length + 1, pattern.Length - 1));
                    carryOverTextPointer = carryOverTextPointer.GetPositionAtOffset(text.Length - pattern.Length + 1);
                    args[3] = (object)carryOverTextPointer;
                }
                // if text length is shorter than pattern length,
                // we concatenate the text and the remaining text
                else
                {
                    args[2] = (object)(text + (string)args[2]);
                }
            }

            return done;
        }

        private static bool ExamineElementStart(TextPointer frozenTextPointer, TextElement element, object[] args)
        {
            // if this is a Block(e.g. Paragraph)
            // we don't want to carry over the previous checked text           
            if (element is Block)
            {
                args[2] = (object)String.Empty;
            }

            // continue walking
            return false;
        }

        private static bool ExamineElementEnd(TextPointer frozenTextPointer, TextElement element, object[] args)
        {
            // if this is a Block(e.g. Paragraph)
            // we don't want to carry over the previous checked text           
            if (element is Block)
            {
                args[2] = (object)String.Empty;
            }

            // continue walking
            return false;
        }

        private static bool ExmainEmbeddedElement(TextPointer frozenTextPointer, DependencyObject dObj, object[] args)
        {

            // if we go across the embedded object, remainingTextToSearch should be reset
            // since we don't treat the string split up by the embedded element as one entity
            args[2] = (object)String.Empty;

            return false;
        }
    }
    /// <summary>
    /// Walk TextRange and dump the content in plain Text format
    /// </summary>
    public static class TextRangeDumper
    {
        /// <summary>
        /// Walk TextRange and dump the content in plain Text format
        /// </summary>
        /// <param name="start">TextPointer start</param>
        /// <param name="end">TextPointer end</param>
        /// <param name="convertTagsToSymbol">if true, all xaml tags (e.g. Bold tag will be converted to 0xFFFC codepoint</param>
        /// <param name="normalizeTags">There are cases where number of open and close tags are different,
        /// if normailizeTags is true, open and close tags are added</param>
        /// <returns></returns>
        public static string Dump(TextPointer start, TextPointer end, bool convertTagsToSymbol, bool normalizeTags)
        {
            StringBuilder sb;
            int levelOfTextElement;

            // prepare the string builder to hold the text
            sb = new StringBuilder();
            levelOfTextElement = 0;

            TextTreeTestHelper.WalkTextRange(start,
                end,
                new object[] { sb, convertTagsToSymbol, normalizeTags, levelOfTextElement },
                new TextTreeTestHelper.StartTreeWalkDelegate(OnStartTreeWalk),
                new TextTreeTestHelper.ProcessTextContextDelegate(DumpTextContext),
                new TextTreeTestHelper.ProcessElementStartContextDelegate(DumpElementStartContext),
                new TextTreeTestHelper.ProcessElementEndContextDelegate(DumpElementEndContext),
                new TextTreeTestHelper.ProcessEmbeddedElementContextDelegate(DumpEmbeddedElementContext),
                new TextTreeTestHelper.FinishTreeWalkDelegate(OnFinishTreeWalk));

            return sb.ToString();
        }

        /// <summary>
        /// Delegate to be called when tree walk happens.
        /// </summary>
        /// <param name="frozenTextPointer"></param>
        /// <param name="args"></param>
        private static void OnStartTreeWalk(TextPointer frozenTextPointer, object [] args)
        {
            StringBuilder sb;
            TextElement element;
            bool normalizeTags;
            int levelOfTextElement;

            normalizeTags = (bool)args[2];

            if (normalizeTags)
            {
                levelOfTextElement = (int)args[3];
                sb = args[0] as StringBuilder;
                element = frozenTextPointer.Parent as TextElement;

                while (element != null)
                {
                    levelOfTextElement++;
                    sb.Insert(0, "<");
                    sb.Insert(0, element.GetType().Name);
                    sb.Insert(0, ">");
                    element = element.Parent as TextElement;
                }

                args[3] = levelOfTextElement;
            }
        }

        /// <summary>
        /// Append content to StringBuilder
        /// </summary>
        /// <param name="frozenTextPointer"></param>
        /// <param name="content"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool DumpTextContext(TextPointer frozenTextPointer, string content, object[] args)
        {
            StringBuilder sb;

            sb = args[0] as StringBuilder;
            if (sb != null)
            {
                sb.Append(content);
            }

            return false;
        }

        /// <summary>
        /// Append Element start tag to StringBuilder, if convertTagsToSymbol is true it appends 0xFFFC instead of the
        /// tag
        /// </summary>
        /// <param name="frozenTextPointer"></param>
        /// <param name="element"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool DumpElementStartContext(TextPointer frozenTextPointer, TextElement element, object[] args)
        {
            StringBuilder sb;
            bool convertTagsToSymbol;
            int levelOfTextElement;

            sb = args[0] as StringBuilder;
            convertTagsToSymbol = (bool)args[1];
            if (sb != null)
            {
                levelOfTextElement = (int)args[3];
                levelOfTextElement++;
                if (convertTagsToSymbol)
                {
                    sb.Append(TagSymbol);
                }
                else
                {
                    sb.Append("<");
                    sb.Append(element.GetType().Name);
                    sb.Append(">");
                }
                args[3] = levelOfTextElement;
            }
            return false;
        }

        /// <summary>
        /// Append Element end tag to StringBuilder, if convertTagsToSymbol is true it appends 0xFFFC instead of the
        /// tag
        /// </summary>
        /// <param name="frozenTextPointer"></param>
        /// <param name="element"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool DumpElementEndContext(TextPointer frozenTextPointer, TextElement element, object[] args)
        {
            StringBuilder sb;
            bool convertTagsToSymbol;
            int levelOfTextElement;

            sb = args[0] as StringBuilder;
            convertTagsToSymbol = (bool)args[1];

            if (sb != null)
            {
                levelOfTextElement = (int)args[3];
                levelOfTextElement--;
                if (convertTagsToSymbol)
                {
                    sb.Append(TagSymbol);
                }
                else
                {
                    sb.Append("</");
                    sb.Append(element.GetType().Name);
                    sb.Append(">");
                }

                args[3] = levelOfTextElement;
            }
            return false;
        }

        /// <summary>
        /// Append object name tag to the string 
        /// </summary>
        /// <param name="frozenTextPointer"></param>
        /// <param name="dObj"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static bool DumpEmbeddedElementContext(TextPointer frozenTextPointer, DependencyObject dObj, object[] args)
        {
            StringBuilder sb;
            bool convertTagsToSymbol;

            sb = args[0] as StringBuilder;
            convertTagsToSymbol = (bool)args[1];

            if (sb != null)
            {
                if (convertTagsToSymbol)
                {
                    sb.Append(TagSymbol);
                }
                else
                {
                    sb.Append("<");
                    sb.Append(dObj.GetType().Name);
                    sb.Append("/>");
                }
            }
            return false;
        }

        /// <summary>
        /// The delegate to be called when the tree walk finishes
        /// </summary>
        /// <param name="frozenTextPointer"></param>
        /// <param name="args"></param>
        private static void OnFinishTreeWalk(TextPointer frozenTextPointer, object [] args)
        {
            TextElement element;
            StringBuilder sb;
            bool normalizeTags;
            int levelOfTextElement;

            sb = args[0] as StringBuilder;
            normalizeTags = (bool)args[2];

            if (normalizeTags)
            {
                levelOfTextElement = (int)args[3];

                element = frozenTextPointer.Parent as TextElement;

                // we walk till the scope of start TextPointer
                // if _levelOfTextElement is +ve, we are seeing
                // more open tags than close tags
                while (element != null && levelOfTextElement-- > 0)
                {
                    sb.Append("</");
                    sb.Append(element.GetType().Name);
                    sb.Append(">");
                    element = element.Parent as TextElement;
                }

                args[3] = levelOfTextElement;
            }
        }

        /// <summary>
        /// unicode character to substitute tags
        /// </summary>
        public static readonly char TagSymbol = (char)0xFFFC;
    }
}
