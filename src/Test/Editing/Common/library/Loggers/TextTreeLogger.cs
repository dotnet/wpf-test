// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides logging services to describe objects from a text tree.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Loggers/TextTreeLogger.cs $")]

namespace Test.Uis.Loggers
{
    #region Namespaces.

    using System;
    using System.Text;
    using System.Collections.Generic;

    using System.Windows;
    using System.Windows.Documents;

    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Provides methods to log information about text tree objects.
    /// </summary>
    public static class TextTreeLogger
    {
        #region Public methods.

        /// <summary>
        /// Describes a System.Windows.Design.TextTreeNavigator instance.
        /// </summary>
        /// <param name="navigator">Instance to describe.</param>
        /// <returns>A multiline description of the instance.</returns>
        public static string DescribeNavigator(TextPointer navigator)
        {
            if (navigator == null)
                throw new ArgumentNullException("navigator");
            string result = DescribePosition(navigator);
            return result;
        }

        /// <summary>
        /// Describes a System.Windows.Documents.TextPointer instance.
        /// </summary>
        /// <param name="position">Instance to describe.</param>
        /// <returns>A multiline description of the instance.</returns>
        public static string DescribePosition(TextPointer position)
        {
            if (position == null)
                throw new ArgumentNullException("position");

            /*
            int backwardRunLength = position.GetTextRunLength(LogicalDirection.Backward);
            int forwardRunLength = position.GetTextRunLength(LogicalDirection.Forward);

            char[] backwardCharacters = new char[backwardRunLength];
            char[] forwardCharacters = new char[forwardRunLength];

            int backwardActualCopyLength = position.GetTextInRun(LogicalDirection.Backward,
                backwardRunLength, null, backwardCharacters, 0);

            int forwardActualCopyLength = position.GetTextInRun(LogicalDirection.Forward,
                forwardRunLength, null, forwardCharacters, 0);
            */
            string result;
            string backwardText = GetPlainText(position, LogicalDirection.Backward);
            string forwardText = GetPlainText(position, LogicalDirection.Forward);

            result = "  Backward text [" +
                //new string(backwardCharacters) + "]";
                backwardText + "]";
            result += Environment.NewLine;
            result += "  Forward text [" +
                // new string(forwardCharacters) + "]";
                forwardText + "]";
            return result;
        }

        /// <summary>
        /// Describes a System.Windows.Documents.TextRange instance.
        /// </summary>
        /// <param name="range">Instance to describe.</param>
        /// <returns>A multiline description of the instance.</returns>
        public static string DescribeRange(TextRange range)
        {
            if (range == null)
                throw new ArgumentNullException("range");
            string result = range.ToString();
            result += ",IsEmpty:" + range.IsEmpty;
            result += ")" + Environment.NewLine;
            result += "  Text [" + range.Text + "]";
            result += Environment.NewLine;
            result += "  Range Start Position: " + DescribePosition(range.Start);
            result += Environment.NewLine;
            result += "  Range End Position: " + DescribePosition(range.Start);
            return result;
        }

        /// <summary>
        /// Describes a ViewLocation instance by dumping all properties.
        /// </summary>
        /// <param name='viewLocation'>
        /// Instance to describe, possibly null.
        /// </param>
        /// <returns>A single-line description of the instance.</returns>
        public static string DescribeViewLocation(object viewLocation)
        {
            if (viewLocation == null)
            {
                return "[null]";
            }
            else
            {
                string[] props =
                    ReflectionUtils.DescribeProperties(viewLocation);
                return "ViewLocation(" +
                    String.Join(",", props) + ")";
            }
        }

        /// <summary>
        /// Provides the most detailed description available for the object.
        /// </summary>
        /// <param name="o">Object to describe.</param>
        /// <returns>A string with the object description.</returns>
        public static string Describe(object o)
        {
            TextPointer navigator = o as TextPointer;
            if (navigator != null) return DescribeNavigator(navigator);

            TextPointer position = o as TextPointer;
            if (position != null) return DescribePosition(position);

            TextRange range = o as TextRange;
            if (range != null) return DescribeRange(range);

            return "Unknown object to describe: " + o;
        }

        /// <summary>
        /// Logs the whole container specified in the pointer, and
        /// additional pointers with their names.
        /// </summary>
        /// <param name='imageName'>Name of image to save.</param>
        /// <param name='containerPointer'>Pointer in container to save.</param>
        /// <param name='pointersAndNames'>TextPointer and string pairs to call out.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void LogRichTextBox(RichTextBox b) {
        ///   TextTreeLogger.LogContainer("my-box", b.ContentStart,
        ///     pointer1, "Interesting pointer", pointer2, "other pointer");
        /// }</code></example>
        public static void LogContainer(string imageName,
            TextPointer containerPointer, params object[] pointersAndNames)
        {
            TextPointer start;
            TextPointer end;
            TextPointer[] pointers;
            string[] names;

            if (containerPointer == null)
            {
                throw new ArgumentNullException("containerPointer");
            }
            start = GetContainerStart(containerPointer);
            end = GetContainerEnd(containerPointer);

            ExtractPointersAndNames(pointersAndNames, out pointers, out names);

            LogRange(imageName, start, end, pointers, names);
        }

        /// <summary>
        /// Logs the range between the specified pointers, and
        /// additional pointers with their names.
        /// </summary>
        /// <param name='imageName'>Name of image to save.</param>
        /// <param name='start'>Pointer from which to start logging.</param>
        /// <param name='end'>Pointer on which to stop logging.</param>
        /// <param name='pointersAndNames'>TextPointer and string pairs to call out.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void LogRange(TextRange range) {
        ///   TextTreeLogger.LogContainer("my-range", range.Start, range.End,
        ///     pointer1, "Interesting pointer", pointer2, "other pointer");
        /// }</code></example>
        public static void LogRange(string imageName, TextPointer start,
            TextPointer end, params object[] pointersAndNames)
        {
            TextPointer[] pointers;
            string[] names;

            ExtractPointersAndNames(pointersAndNames, out pointers, out names);

            LogRange(imageName, start, end, pointers, names);
        }

        /// <summary>
        /// Logs the range between the specified pointers, and
        /// additional pointers with their names.
        /// </summary>
        /// <param name='imageName'>Name of image to save.</param>
        /// <param name='start'>Pointer from which to start logging.</param>
        /// <param name='end'>Pointer on which to stop logging.</param>
        /// <param name='pointers'>TextPointers to call out.</param>
        /// <param name='pointerNames'>Labels for pointers to call out.</param>
        /// <remarks>
        /// Every element in pointers must be paired up with an element in
        /// pointerNames.
        /// </remarks>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void LogRange(TextRange range) {
        ///   TextTreeLogger.LogContainer("my-range", range.Start, range.End,
        ///     new TextPointer[] { pointer1, pointer2 },
        ///     new string[] { "Interesting pointer", "other pointer" });
        /// }</code></example>
        public static void LogRange(string imageName, TextPointer start, TextPointer end,
            TextPointer[] pointers, string[] pointerNames)
        {
            if (imageName == null)
            {
                throw new ArgumentNullException("imageName");
            }
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (end == null)
            {
                throw new ArgumentNullException("end");
            }
            if (pointers == null)
            {
                throw new ArgumentNullException("pointers");
            }
            if (pointerNames == null)
            {
                throw new ArgumentNullException("pointerNames");
            }
            if (pointers.Length != pointerNames.Length)
            {
                throw new ArgumentException(
                    "pointers length does not match pointerNames length", "pointers");
            }
            if (start.CompareTo(end) > 0)
            {
                throw new ArgumentException("start cannot be after end", "start");
            }

            PrivateLogRange(imageName, start, end, pointers, pointerNames);
        }

        #endregion Public methods.


        #region Private methods.

        private static System.Drawing.Bitmap CreateBitmapForLayouts(
            TextPointer start, TextPointer end,
            PointerNameLayoutList nameLayouts, SymbolLayoutList symbolLayouts)
        {
            System.Drawing.Bitmap bitmap;
            System.Drawing.Font font;
            System.Drawing.Pen pen;
            System.Drawing.Brush brush;
            System.Drawing.Brush symbolBrush;
            System.Drawing.Brush backBrush;

            font = new System.Drawing.Font("Arial", 10);
            brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            symbolBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            pen = new System.Drawing.Pen(System.Drawing.Color.Blue);

            // Measure in a small sample bitmap.
            bitmap = new System.Drawing.Bitmap(100, 100);

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                MeasureLayout(graphics, font, nameLayouts, symbolLayouts);
            }

            // Recreate the bitmap with the appropriate size.
            bitmap = new System.Drawing.Bitmap(
                symbolLayouts.Right + Padding, nameLayouts.Bottom + Padding);
            backBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new System.Drawing.Point(0, 0), new System.Drawing.Point(bitmap.Width, bitmap.Height),
                System.Drawing.Color.LightYellow, System.Drawing.Color.Tan);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Fill in the background.
                graphics.FillRectangle(backBrush, 0, 0, bitmap.Width, bitmap.Height);

                // Draw all pointer names.
                foreach(PointerNameLayout layout in nameLayouts)
                {
                    graphics.DrawString(layout.Name, font, brush, layout.Position);
                }

                // Draw all content (position ticks and symbols)
                foreach(SymbolLayout layout in symbolLayouts)
                {
                    float bottom;

                    bottom = layout.Position.Y + layout.Size.Height * (float)1.3;
                    graphics.DrawLine(pen,
                        new System.Drawing.PointF(layout.Position.X - Padding / 2, layout.Position.Y + layout.Size.Height * (float)0.5),
                        new System.Drawing.PointF(layout.Position.X - Padding / 2, bottom));
                    graphics.DrawString(layout.Content, font, symbolBrush, layout.Position);
                    graphics.DrawLine(pen,
                        new System.Drawing.PointF(layout.Right + Padding / 2, layout.Position.Y + layout.Size.Height * (float)0.5),
                        new System.Drawing.PointF(layout.Right + Padding / 2, bottom));
                    graphics.DrawLine(pen,
                        new System.Drawing.PointF(layout.Position.X - Padding / 2, bottom),
                        new System.Drawing.PointF(layout.Right + Padding / 2, bottom));
                }


                // Draw lines between names and pointers.
                foreach(PointerNameLayout nameLayout in nameLayouts)
                {
                    System.Drawing.PointF pointerPoint;
                    System.Drawing.PointF namePoint;

                    pointerPoint = symbolLayouts.GetPointerPoint(nameLayout.Pointer);
                    if (pointerPoint.X == 0)
                    {
                        Logger.Current.Log("Unable to find pointer " + nameLayout.Name +
                            " in specified range.");
                        Logger.Current.Log("Pointer is at " +
                            GetContainerStart(nameLayout.Pointer).GetOffsetToPosition(nameLayout.Pointer) +
                            " positions from start of container.");
                        continue;
                    }

                    namePoint = new System.Drawing.PointF(nameLayout.Right + Padding / 2, nameLayout.HalfVertical);

                    graphics.DrawLine(pen, namePoint,
                        new System.Drawing.PointF(pointerPoint.X, namePoint.Y));
                    graphics.DrawLine(pen, pointerPoint,
                        new System.Drawing.PointF(pointerPoint.X, namePoint.Y));
                }
            }

            return bitmap;
        }

        private static string GetPlainText(TextPointer textPointer, LogicalDirection direction)
        {
            TextPointerContext textPointerContext;
            TextPointer currentPointer;
            bool done;
            StringBuilder sb;

            done = false;
            currentPointer = textPointer;
            sb = new StringBuilder();

            while (!done)
            {
                textPointerContext = currentPointer.GetPointerContext(direction);

                switch (textPointerContext)
                {
                    case TextPointerContext.Text:
                        sb.Append(currentPointer.GetTextInRun(direction));
                        break;
                    case TextPointerContext.None:
                        done = true;
                        break;
                    case TextPointerContext.ElementStart:
                        sb.Append("<Element ");
                        sb.Append(ExtractTypeName(currentPointer.GetAdjacentElement(direction).GetType().ToString()));
                        sb.Append(">");
                        break;
                    case TextPointerContext.ElementEnd:
                        sb.Append("</Element>");
                        break;
                    case TextPointerContext.EmbeddedElement:
                        sb.Append("<EmbeddedObject>");
                        sb.Append(ExtractTypeName(currentPointer.GetAdjacentElement(direction).GetType().ToString()));
                        sb.Append("</EmbeddedObject>");
                        break;
                    default:
                        break;
                }
                currentPointer = currentPointer.GetNextContextPosition(direction);
            }

            return sb.ToString();
        }

        private static string ExtractTypeName(string fullQualifyTypeName)
        {
            int lastIndex;

            lastIndex = fullQualifyTypeName.LastIndexOf(".");

            if (lastIndex > -1 &&
                lastIndex < fullQualifyTypeName.Length - 1)
            {
                return fullQualifyTypeName.Substring(lastIndex + 1, fullQualifyTypeName.Length - lastIndex);
            }

            return String.Empty;
        }

        private static TextPointer GetContainerEnd(TextPointer containerPointer)
        {
            TextPointer result;

            result = containerPointer;
            while (result.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None)
            {
                result = result.GetNextContextPosition(LogicalDirection.Forward);
                //result = result.GetPositionAtOffset(1024);
            }

            return result;
        }

        private static TextPointer GetContainerStart(TextPointer containerPointer)
        {
            TextPointer result;

            result = containerPointer;
            while (result.GetPointerContext(LogicalDirection.Backward) != TextPointerContext.None)
            {
                result = result.GetNextContextPosition(LogicalDirection.Backward);
                //result = result.GetPositionAtOffset(-1024);
            }

            return result;
        }

        private static void ExtractPointersAndNames(object[] pointersAndNames,
            out TextPointer[] pointers, out string[] names)
        {
            if (pointersAndNames == null || pointersAndNames.Length == 0)
            {
                pointers = new TextPointer[0];
                names = new string[0];
            }
            else
            {
                int index;

                pointers = new TextPointer[pointersAndNames.Length / 2];
                names = new string[pointersAndNames.Length / 2];

                index = 0;
                while (index < pointersAndNames.Length)
                {
                    pointers[index / 2] = (TextPointer) pointersAndNames[index];
                    names[index / 2] = (string) pointersAndNames[index + 1];
                    index += 2;
                }
            }
        }

        private static void MeasureLayout(System.Drawing.Graphics graphics,
            System.Drawing.Font font, PointerNameLayoutList nameLayouts,
            SymbolLayoutList symbolLayouts)
        {
            float x;    // Horizontal offset value for next element.
            float y;    // Vertical offset value for next element.

            // Calculate size for names.
            nameLayouts.SortByPointers();
            foreach(PointerNameLayout layout in nameLayouts)
            {
                layout.Size = graphics.MeasureString(layout.Name, font);
            }

            // Calculate size for symbols.
            x = Padding + nameLayouts.MaxNameWidth + Padding + Padding;
            foreach(SymbolLayout symbolLayout in symbolLayouts)
            {
                symbolLayout.Size = graphics.MeasureString(symbolLayout.Content, font);
                symbolLayout.Position = new System.Drawing.PointF(x, Padding);
                x += Padding + symbolLayout.Size.Width + Padding;
            }

            // Lay out names.
            x = Padding;
            y = Padding + symbolLayouts.MaxContentHeight + Padding;
            foreach(PointerNameLayout layout in nameLayouts)
            {
                layout.Position = new System.Drawing.PointF(x, y);
                y += nameLayouts.MaxNameHeight;
                y += Padding;
            }
        }

        private static void PrivateLogRange(string imageName, TextPointer start, TextPointer end,
            TextPointer[] pointers, string[] pointerNames)
        {
            System.Drawing.Bitmap bitmap;
            PointerNameLayoutList nameLayouts;
            SymbolLayoutList symbolLayouts;
            TextPointer cursor;

            cursor = start;
            nameLayouts = new PointerNameLayoutList();
            symbolLayouts = new SymbolLayoutList();

            // Add all interesting pointers to their layout.
            for (int i = 0; i < pointers.Length; i++)
            {
                nameLayouts.Add(new PointerNameLayout(pointerNames[i], pointers[i]));
            }

            // Add the first pointer, then move until the end is found.
            symbolLayouts.Add(new SymbolLayout(cursor, LogicalDirection.Backward));
            symbolLayouts.Add(new SymbolLayout(cursor, LogicalDirection.Forward));
            while (cursor.CompareTo(end) < 0)
            {
                cursor = cursor.GetPositionAtOffset(1);
                symbolLayouts.Add(new SymbolLayout(cursor, LogicalDirection.Forward));
            }

            bitmap = CreateBitmapForLayouts(start, end, nameLayouts, symbolLayouts);
            Logger.Current.LogImage(bitmap, imageName);
        }

        #endregion Private methods.


        #region Private constants.

        /// <summary>Padding used between various elements.</summary>
        private const int Padding = 8;

        #endregion Private constants.

        #region Inner types.

        /// <summary>
        /// Layout information for the name of an interesting pointer.
        /// </summary>
        class PointerNameLayout
        {
            /// <summary>Initializes a new PointerNameLayout instance.</summary>
            internal PointerNameLayout(string name, TextPointer pointer)
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                if (pointer == null)
                {
                    throw new ArgumentNullException("pointer");
                }

                if (pointer.LogicalDirection == LogicalDirection.Backward)
                {
                    this.Name = "< " + name;
                }
                else
                {
                    this.Name = "> " + name;
                }
                this.Pointer = pointer;
                this.Position = new System.Drawing.PointF(0, 0);
            }

            #region Internal properties and fields.

            internal string Name;
            internal System.Drawing.PointF Position;
            internal System.Drawing.SizeF Size;
            internal TextPointer Pointer;

            internal float Right
            {
                get { return Position.X + Size.Width; }
            }

            internal float Bottom
            {
                get { return Position.Y + Size.Height; }
            }

            internal float HalfVertical
            {
                get { return Position.Y + Size.Height / 2; }
            }

            #endregion Internal properties and fields.
        }

        /// <summary>
        /// Represents a collection of PointerNameLayout objects that
        /// can be individually accessed by index.
        /// </summary>
        class PointerNameLayoutList: List<PointerNameLayout>
        {
            /// <summary>Initializes a new PointerNameLayoutList instance.</summary>
            internal PointerNameLayoutList(): base() { }

            /// <summary>Sorts the pointers in container order.</summary>
            internal void SortByPointers()
            {
                Sort(delegate(PointerNameLayout x, PointerNameLayout y) {
                    int result;
                    result = y.Pointer.GetOffsetToPosition(x.Pointer);
                    if (result == 0) result = x.Name.CompareTo(y.Name);
                    return result;
                });
            }

            #region Internal properties.

            /// <summary>Vertical distance to bottom of list.</summary>
            internal int Bottom
            {
                get
                {
                    if (Count == 0)
                    {
                        // Bogus value to avoid a zero-high space.
                        return 64;
                    }
                    else
                    {
                        return (int)(this[Count-1].Bottom + 1);
                    }
                }
            }

            /// <summary>Height of the tallest name.</summary>
            internal float MaxNameHeight
            {
                get
                {
                    float result;
                    result = 0;
                    foreach(PointerNameLayout p in this)
                    {
                        if (p.Size.Height > result)
                        {
                            result = p.Size.Height;
                        }
                    }
                    return result;
                }
            }

            /// <summary>Width of the widest name.</summary>
            internal float MaxNameWidth
            {
                get
                {
                    float result;
                    result = 0;
                    foreach(PointerNameLayout p in this)
                    {
                        if (p.Size.Width > result)
                        {
                            result = p.Size.Width;
                        }
                    }
                    return result;
                }
            }

            #endregion Internal properties.
        }

        /// <summary>Layout information for a symbol in the container.</summary>
        class SymbolLayout
        {
            #region Constructors.

            /// <summary>Initializes a new SymbolLayout instance.</summary>
            /// <param name='pointer'>Pointer next to this symbol.</param>
            /// <param name='direction'>
            /// Direction indicating how the pointer is related to this symbol,
            /// such that this symbol is in the [direction] direction of the
            /// pointer.
            /// </param>
            internal SymbolLayout(TextPointer pointer, LogicalDirection direction)
            {
                TextPointerContext context;
                bool isForward;

                if (pointer == null)
                {
                    throw new ArgumentNullException("pointer");
                }

                isForward = direction == LogicalDirection.Forward;
                context = pointer.GetPointerContext(direction);
                switch(context)
                {
                    case TextPointerContext.None:
                        this.Content = "[NONE]";
                        break;
                    case TextPointerContext.Text:
                        this.Content = pointer.GetTextInRun(direction);
                        if (isForward)
                        {
                            this.Content = this.Content.Substring(0, 1);
                        }
                        else
                        {
                            this.Content = this.Content.Substring(this.Content.Length - 1, 1);
                        }
                        if (!ShouldDisplayChar(this.Content[0]))
                        {
                            this.Content = TextUtils.ConvertToAnsi(this.Content);
                        }
                        break;
                    case TextPointerContext.EmbeddedElement:
                        this.Content = "<" + pointer.GetAdjacentElement(direction).GetType().Name +" />";
                        break;
                    case TextPointerContext.ElementStart:
                        if (isForward) pointer = pointer.GetPositionAtOffset(1);
                        this.Content = "<" + pointer.Parent.GetType().Name +">";
                        if (isForward) pointer = pointer.GetPositionAtOffset(-1);
                        break;
                    case TextPointerContext.ElementEnd:
                        if (!isForward) pointer = pointer.GetPositionAtOffset(-1);
                        this.Content = "</" + pointer.Parent.GetType().Name +">";
                        if (!isForward) pointer = pointer.GetPositionAtOffset(1);
                        break;
                    default:
                        this.Content = context.ToString();
                        break;
                }

                this.Position = new System.Drawing.PointF(0, 0);
                this.Size = new System.Drawing.SizeF(0, 0);
                this.Pointer = pointer;
                this.Direction = direction;
            }

            #endregion Constructors.

            #region Internal properties and fields.

            /// <summary>String representation of the symbol content.</summary>
            internal string Content;
            internal System.Drawing.PointF Position;
            internal System.Drawing.SizeF Size;

            /// <summary>Pointer next to this symbol.</summary>
            internal TextPointer Pointer;

            /// <summary>
            /// Direction indicating how Pointer is related to this symbol,
            /// such that this symbol is in the [direction] direction of the
            /// pointer.
            /// </summary>
            internal LogicalDirection Direction;

            internal float Right
            {
                get { return Position.X + Size.Width; }
            }

            internal float Bottom
            {
                get { return Position.Y + Size.Height; }
            }

            #endregion Internal properties and fields.

            #region Private methods.

            private static bool ShouldDisplayChar(char c)
            {
                return
                    (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    c == '.' || c == ',' || c == '-';
            }

            #endregion Private methods.
        }

        /// <summary>
        /// Represents a collection of SymbolLayout objects that
        /// can be individually accessed by index.
        /// </summary>
        class SymbolLayoutList: List<SymbolLayout>
        {
            #region Constructors.

            /// <summary>Initializes a new SymbolLayoutList instance.</summary>
            internal SymbolLayoutList(): base() { }

            #endregion Constructors.

            #region Internal properties.

            /// <summary>Height of the tallest symbol.</summary>
            internal float MaxContentHeight
            {
                get
                {
                    float result;
                    result = 0;
                    foreach(SymbolLayout p in this)
                    {
                        if (p.Size.Height > result)
                        {
                            result = p.Size.Height;
                        }
                    }
                    return result;
                }
            }

            /// <summary>Horizontal distance to the right of the list.</summary>
            internal int Right
            {
                get
                {
                    if (Count == 0)
                    {
                        return 128;  // Bogus value.
                    }
                    else
                    {
                        return (int)(this[Count-1].Right + 1);
                    }
                }
            }

            #endregion Internal properties.

            #region Internal methods.

            /// <summary>
            /// Gets the point at which the pointer should be represented
            /// in the laid out image, (0,0) if not found.
            /// </summary>
            internal System.Drawing.PointF GetPointerPoint(TextPointer pointer)
            {
                System.Drawing.PointF notFoundPoint;

                if (pointer == null)
                {
                    throw new ArgumentNullException("pointer");
                }

                notFoundPoint = new System.Drawing.PointF(0, 0);

                if (Count == 0)
                {
                    return notFoundPoint;
                }

                // The first symbol is the one before the start position,
                // and will always be LogicalDirection.Backward from its position.
                System.Diagnostics.Debug.Assert(this[0].Direction == LogicalDirection.Backward);
                if (this[0].Pointer.CompareTo(pointer) == 0)
                {
                    return new System.Drawing.PointF(this[0].Right + Padding, this[0].Bottom);
                }

                // Symbol with index 1 refers to the same position, only it's
                // in the forward direction from its position. Look for other
                // positions starting from index 2.
                for (int i = 1; i < Count; i++)
                {
                    System.Diagnostics.Debug.Assert(this[i].Direction == LogicalDirection.Forward);
                    if (this[i].Pointer.CompareTo(pointer) == 0)
                    {
                        return new System.Drawing.PointF(
                            this[i].Position.X - Padding, this[0].Bottom);
                    }
                }

                return notFoundPoint;
            }

            #endregion Internal methods.
        }

        #endregion Inner types.
    }
}
