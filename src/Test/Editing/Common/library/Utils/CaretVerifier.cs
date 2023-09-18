// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides methods to test Avalon Caret in Editing Controls

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test.Imaging;
    using Test.Uis.Wrappers;
    using Test.Uis.Loggers;
    using System.Windows.Threading;
    using System.Threading;
    using Microsoft.Test.Threading;
    using Microsoft.Test.Logging;

    #endregion Namespaces.

    /// <summary>
    /// Provides various verification/testing for caret inside editing controls.
    /// </summary>
    public class CaretVerifier
    {
        #region Constructors.

        /// <summary>
        /// Creates an CaretVerifier associated with the specified UIElement.
        /// </summary>
        /// <param name="element">Element to associate the CaretVerifier with.</param>
        public CaretVerifier(FrameworkElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (!(element is TextBox
                || element is RichTextBox
                || element is PasswordBox))
            {
                throw new NotSupportedException(element.GetType().Name + " is not supported");
            }

            this._element = element;
            this._elementWrapper = new UIElementWrapper(_element);
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Stops the blinking of caret associated to the wrapper element
        /// </summary>
        public void StopCaretBlinking()
        {
            Adorner caretAdorner = ElementWrapper.CaretElement;
            ReflectionUtils.InvokeInstanceMethod(caretAdorner, "SetBlinking", new object[] { false });
        }

        /// <summary>
        /// Starts the blinking of caret associated to the wrapper element
        /// </summary>
        public void StartCaretBlinking()
        {
            Adorner caretAdorner = ElementWrapper.CaretElement;
            ReflectionUtils.InvokeInstanceMethod(caretAdorner, "SetBlinking", new object[] { true });
        }

        /// <summary>
        /// Gets the sub-bitmap of the caret rect from the snapshot of the control
        /// </summary>
        /// <returns>Caret rect bitmap</returns>
        public Bitmap GetCaretRectBitmap()
        {
            int caretBlinkTime = SystemCaretBlinkTimeMS;

            Logger.Current.Log("GetCaretRectBitmap::System Caret Blink Time in MilliSeconds" + caretBlinkTime);
            // Global state of caret is not affected by this operation.
            StopCaretBlinking();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(caretBlinkTime);
           
            Bitmap controlSnapshot = BitmapCapture.CreateBitmapFromElement(Element);
            if (ConfigurationSettings.Current.GetArgumentAsBool("debug"))
            {
                Logger.Current.LogImage(controlSnapshot, "controlSnapshot");
            }

            // Global state of caret is not affected by this operation.
            StartCaretBlinking();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(caretBlinkTime);

            Rect caretRect = GetExpectedCaretRect();
            // Scale rect to Bitmap's DPI
            caretRect = BitmapUtils.AdjustBitmapSubAreaForDpi(controlSnapshot, caretRect);
            Bitmap caretSnapshot = BitmapUtils.CreateSubBitmap(controlSnapshot, caretRect);            

            return caretSnapshot;
        }

        /// <summary>
        /// Captures Caret by taking captures with and without the caret when blinking and then 
        /// filtering the differences.
        /// </summary>
        /// <param name="callBackHandler">Call back handler which will be called in return</param>                        
        public void CaptureCaret(SimpleHandler callBackHandler)
        {
            _caretTestTrailCount = 0;
            _caretCaptureArray = new Bitmap[6];
            _caretCaptureResult = null;
            _caretCaptureRect = Rect.Empty;
            PrepareForCaretVerification();
            _originalCaretBlinkTime = Win32.GetCaretBlinkTime();
            Logger.Current.Log("CaptureCaret:: Win32.GetCaretBlinkTime" + _originalCaretBlinkTime);
            Win32.SetCaretBlinkTime(_originalCaretBlinkTime*4 );
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds((int)(_originalCaretBlinkTime * 1.5)),
                new CaretRenderHandler(CaptureCaretPrivate), callBackHandler);
        }

        /// <summary>
        /// Returns the caret rectangle Bitmap resulting from CaptureCaret() method. Call CaptureCaret
        /// before accessing this property. The value will empty Bitmap if CaptureCaret() fails to find caret.
        /// </summary>
        public Bitmap CaretCaptureResult
        {
            get { return _caretCaptureResult; }
        }

        /// <summary>
        /// Returns the caret rect resulting from CaptureCaret() method. Call CaptureCaret
        /// before accessing this property. The value will empty Rect if CaptureCaret() fails to find caret.
        /// </summary>
        public Rect CaretCaptureRect
        {
            get { return _caretCaptureRect; }
        }

        /// <summary>
        /// Gets the rectangle bounding the caret. The width of the Rect currently doesnt 
        /// reflect the actual Caret's width.
        /// </summary>
        /// <returns>rectangle bounding the caret</returns>
        public Rect GetCaretRectangleFromTransform()
        {
            Adorner caretElement;
            GeneralTransform caretTransform;
            Rect result;

            caretElement = ElementWrapper.CaretElement;
            if (caretElement == null)
            {
                throw new ApplicationException("Element has no caret");
            }
            caretTransform = caretElement.GetDesiredTransform(null);

            result = new Rect(new System.Windows.Point(0, 0), caretElement.RenderSize);
            result = caretTransform.TransformBounds(result);
            return result;
        }

        /// <summary>
        /// This method is called when performing smart verification. This method prepares 
        /// the Element for caret verification. Foreground color will be changed to Red. 
        /// This will help in doing Bitmap comparisons for caret so that text besides the 
        /// caret dont interfere. Original color will be restored after verification is done.
        /// </summary>
        private void PrepareForCaretVerification()
        {
            _doSmartVerification = true;
            Logger.Current.Log("Preparing for caret verification...");
            _originalBrush = (System.Windows.Media.Brush)Element.GetValue(TextElement.ForegroundProperty);
            Element.SetValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Red);
        }

        /// <summary>
        /// Verifies that caret is BiDi. Throws an exception if not.
        /// </summary>
        /// <param name="callBackHandler">Callback handler after verifying</param>
        /// <param name="doSmartVerification">If true, will do smart verification where foreground is
        /// changed to Red color and will be filtered out so that text besides caret doesnt interfere 
        /// while taking bitmap captures to detect caret</param>
        public void VerifyCaretBiDi(SimpleHandler callBackHandler, bool doSmartVerification)
        {
            if (doSmartVerification)
            {
                PrepareForCaretVerification();
            }

            _caretBiDi = false;
            _caretTestTrailCount = 0;
            QueueHelper.Current.QueueDelegate(new CaretRenderHandler(TestCaretBiDiPrivate), callBackHandler);
        }

        /// <summary>
        /// Verifies that caret is blinking. Throws an exception if not.
        /// </summary>
        /// <param name="callBackHandler">Callback handler after verifying</param>
        /// <param name="doSmartVerification">If true, will do smart verification where foreground is
        /// changed to Red color and will be filtered out so that text besides caret doesnt interfere 
        /// while taking bitmap captures to detect caret</param>
        public void VerifyCaretBlinking(SimpleHandler callBackHandler, bool doSmartVerification)
        {
            if (doSmartVerification)
            {
                PrepareForCaretVerification();
            }

            _caretTestTrailCount = 0;
            _caretBlinkingArray = new bool[6];

            QueueHelper.Current.QueueDelegate(new CaretRenderHandler(TestCaretBlinkingPrivate), callBackHandler);
        }

        /// <summary>
        /// Verifies that caret is Italic. Throws an exception if not.
        /// </summary>
        /// <param name="callBackHandler">Callback handler after verifying</param>
        /// <param name="doSmartVerification">If true, will do smart verification where foreground is
        /// changed to Red color and will be filtered out so that text besides caret doesnt interfere 
        /// while taking bitmap captures to detect caret</param>
        /// <remarks>
        /// This Italic caret testing is not stable for all Fonts. 
        /// Tahoma is one of the fonts where it is stable.
        /// </remarks>
        public void VerifyCaretItalic(SimpleHandler callBackHandler, bool doSmartVerification)
        {
            if (doSmartVerification)
            {
                PrepareForCaretVerification();
            }

            _caretItalic = false;
            _caretTestTrailCount = 0;
            QueueHelper.Current.QueueDelegate(new CaretRenderHandler(TestCaretItalicPrivate), callBackHandler);
        }

        /// <summary>
        /// Verifies that caret is rendered. Throws an exception if not.
        /// </summary>
        /// <param name="callBackHandler">Callback handler after verifying</param>
        /// <param name="doSmartVerification">If true, will do smart verification where foreground is
        /// changed to Red color and will be filtered out so that text besides caret doesnt interfere 
        /// while taking bitmap captures to detect caret</param>
        public void VerifyCaretRendered(SimpleHandler callBackHandler, bool doSmartVerification)
        {
            if (doSmartVerification)
            {
                PrepareForCaretVerification();
            }
            _originalCaretBlinkTime = Win32.GetCaretBlinkTime();
            Win32.SetCaretBlinkTime(_originalCaretBlinkTime*4);
              
            _caretFound = false;
            _caretTestTrailCount = 0;
            //delay inserted so that the caret is sure to be on screen
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds((int)(_originalCaretBlinkTime*1.5)),
                 new CaretRenderHandler(TestCaretRenderPrivate), callBackHandler);
        }


        #endregion Public methods.


        #region Public properties.

        /// <summary>Height of the caret bitmap captured.</summary>
        /// <remarks>This doesnt verify that caret is rendered.</remarks>
        public int CaretBitmapHeight
        {
            get
            {
                System.Drawing.Bitmap caretBitmap;
                caretBitmap = GetCaretBitmap();

                return caretBitmap.Height;
            }
        }

        /// <summary>Caret blink time in milli seconds</summary>
        public static int SystemCaretBlinkTimeMS
        {
            get { return System.Windows.Forms.SystemInformation.CaretBlinkTime; }
        }

        /// <summary>Caret width in double</summary>
        public static double SystemCaretWidthInDouble
        {
            get { return SystemParameters.CaretWidth; }
        }

        /// <summary>Caret width in pixels</summary>
        public static int SystemCaretWidthInPixels
        {
            get { return System.Windows.Forms.SystemInformation.CaretWidth; }
        }

        /// <summary>
        /// Gets the FrameworkElement associated with this CaretVerifier instance.
        /// </summary>
        public FrameworkElement Element
        {
            get { return this._element; }
        }

        /// <summary>
        /// Gets the UIElementWrapper associated with Element
        /// </summary>
        public UIElementWrapper ElementWrapper
        {
            get { return this._elementWrapper; }
        }

        #endregion Public properties.


        #region Private methods.

        private void CaptureCaretPrivate(SimpleHandler callBackHandler)
        {
            Bitmap differences, capture1, capture2;
            Rect resultRect;
            capture1 = capture2 = null;
            int arrayIndex = 0;
            if (_caretCaptureArray == null)
            {
                _caretCaptureArray = new Bitmap[4];
            }
            while (arrayIndex < _caretCaptureArray.Length)
            {
                _caretCaptureArray[arrayIndex] = GetCaretBitmap();
                //_caretCaptureArray[arrayIndex] = BitmapCapture.CreateBitmapFromElement(Element);
                //_caretCaptureArray[arrayIndex] = ConvertNonBlackPixelsToWhite(_caretCaptureArray[arrayIndex]);
                //_caretCaptureArray[arrayIndex] = BitmapUtils.ColorToBlackWhite(_caretCaptureArray[arrayIndex]);
                //Logger.Current.LogImage(_caretCaptureArray[arrayIndex], "CaretCapture_" + arrayIndex);
                arrayIndex++;
                DispatcherHelper.DoEvents(DispatcherPriority.ApplicationIdle);
            }
            Logger.Current.Log("CaptureCaretPrivate:: System Caret Blink Time" + _originalCaretBlinkTime);
            Win32.SetCaretBlinkTime(_originalCaretBlinkTime);
            int index = -1;
            int maxCount = 20;
            for (int i = 0; i < _caretCaptureArray.Length; i++)
            {
                //Logger.Current.Log("---------------" + BitmapUtils.CountColoredPixels(_caretCaptureArray[i], Colors.Black));
                if (BitmapUtils.CountColoredPixels(_caretCaptureArray[i], Colors.Black) > maxCount)
                {
                    maxCount = BitmapUtils.CountColoredPixels(_caretCaptureArray[i], Colors.Black);
                    index = i;
                }
            }
            if (index == -1)
            {
                if (!ComparisonOperationUtils.AreBitmapsEqual(_caretCaptureArray[0], _caretCaptureArray[1],
                    out differences))
                {
                    resultRect = BitmapUtils.GetDifferencesRect(_caretCaptureArray[0], _caretCaptureArray[1]);
                    _caretCaptureRect = resultRect;
                    capture1 = BitmapUtils.CreateSubBitmap(_caretCaptureArray[0], resultRect);
                    capture2 = BitmapUtils.CreateSubBitmap(_caretCaptureArray[1], resultRect);
                }
                else if (!ComparisonOperationUtils.AreBitmapsEqual(_caretCaptureArray[0], _caretCaptureArray[2],
                    out differences))
                {
                    resultRect = BitmapUtils.GetDifferencesRect(_caretCaptureArray[0], _caretCaptureArray[2]);
                    _caretCaptureRect = resultRect;
                    capture1 = BitmapUtils.CreateSubBitmap(_caretCaptureArray[0], resultRect);
                    capture2 = BitmapUtils.CreateSubBitmap(_caretCaptureArray[2], resultRect);
                }
                else if (!ComparisonOperationUtils.AreBitmapsEqual(_caretCaptureArray[1], _caretCaptureArray[2],
                    out differences))
                {
                    resultRect = BitmapUtils.GetDifferencesRect(_caretCaptureArray[1], _caretCaptureArray[2]);
                    _caretCaptureRect = resultRect;
                    capture1 = BitmapUtils.CreateSubBitmap(_caretCaptureArray[1], resultRect);
                    capture2 = BitmapUtils.CreateSubBitmap(_caretCaptureArray[2], resultRect);
                }
                DispatcherHelper.DoEvents();
                if ((capture1 != null) && (capture2 != null))
                {
                    if (BitmapUtils.CountColoredPixels(capture1, System.Windows.Media.Brushes.White.Color) < BitmapUtils.CountColoredPixels(capture2, System.Windows.Media.Brushes.White.Color))
                    {
                        _caretCaptureResult = capture1;
                    }
                    else
                    {
                        _caretCaptureResult = capture2;
                    }
                }
                else
                {
                    //Not able to find caret capture. Return empty bitmap
                    GlobalLog.LogStatus("Caret Capture was not successful");
                    _caretCaptureResult = new Bitmap(0, 0);
                    _caretCaptureRect = Rect.Empty;
                }
            }
            else
            {
                Rectangle rect = BitmapUtils.GetBoundingRectangle(_caretCaptureArray[index]);
                _caretCaptureResult = BitmapUtils.CreateSubBitmap(_caretCaptureArray[index], rect);
            }


            Logger.Current.LogImage(_caretCaptureResult, "CaretResult");

            RestoreOriginalState();
            if (callBackHandler != null)
            {
                QueueHelper.Current.QueueDelegate(callBackHandler);
            }
        }


        /// <summary>
        /// Replaces the pixels with red component value less than 128, with White pixels
        /// </summary>
        /// <param name="sourceBitmap">Input Bitmap</param>
        /// <returns>Output Bitmap</returns>
        public Bitmap ConvertNonBlackPixelsToWhite(Bitmap sourceBitmap)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted).Assert();
            return UnsafeConvertNonBlackPixelsToWhite(sourceBitmap);
        }

        public Rect GetExpectedCaretRect()
        {
            double spacing = 2; // Space before and after caret.
            System.Windows.FontStyle fontStyle;            
            double fontSize;
            double caretWidth = SystemCaretWidthInDouble; // Expected width of caret.
            Rect rect;
            
            if (ElementWrapper.SelectionLength != 0)
            {
                throw new InvalidOperationException("Selection is not empty.");
            }
            
            if (Element is TextBoxBase)
            {
                TextPointer caretPointer;
                bool isRightToLeftText;

                caretPointer = ElementWrapper.SelectionInstance.Start;

                // Figure out whether the caret should render in right-to-left fashion.
                isRightToLeftText = false;
                if ((Element.Language.GetSpecificCulture().TextInfo.IsRightToLeft) ||
                    (((XmlLanguage)ElementWrapper.SelectionInstance.GetPropertyValue(Block.LanguageProperty)).GetSpecificCulture().TextInfo.IsRightToLeft) ||
                    (System.Windows.Input.InputLanguageManager.Current.CurrentInputLanguage.TextInfo.IsRightToLeft))
                {
                    isRightToLeftText = true;
                }

                rect = ElementWrapper.GetElementRelativeCharacterRect(caretPointer, 0,
                    isRightToLeftText ? LogicalDirection.Backward : LogicalDirection.Forward);

                // For caret finding purpose we just need a zero-width rect
                rect.Width = 0.0;

                if ((isRightToLeftText && caretPointer.LogicalDirection == LogicalDirection.Forward)
                    || (!isRightToLeftText && caretPointer.LogicalDirection == LogicalDirection.Backward))
                {
                    rect.X += rect.Width;
                }

                if (caretPointer.GetTextInRun(LogicalDirection.Forward) != string.Empty
                    && caretPointer.GetTextInRun(LogicalDirection.Backward) == string.Empty)
                {
                    rect.X = rect.X - spacing;
                }
                else if (caretPointer.GetTextInRun(LogicalDirection.Forward) == string.Empty
                    && caretPointer.GetTextInRun(LogicalDirection.Backward) != string.Empty)
                {
                    rect.X = rect.X - rect.Width - spacing;
                }
                else
                {
                    rect.X = rect.X - caretWidth - spacing;
                }
                rect.Width = caretWidth + spacing * 2;

                //We need more spacing to the right if it is italic caret
                fontStyle = (System.Windows.FontStyle)ElementWrapper.SelectionInstance.GetPropertyValue(TextElement.FontStyleProperty);
                if (fontStyle == System.Windows.FontStyles.Italic)
                {
                    rect.Width += 4;     //Need more spacing for italic caret
                    fontSize = (double)ElementWrapper.SelectionInstance.GetPropertyValue(TextElement.FontSizeProperty);
                    if (fontSize > 12)
                    {
                        rect.Width += ((int)fontSize - 12) / 2; //Need even more spacing for big italic carets
                    }
                }
            }
            else if (Element is PasswordBox)
            {
                object selection;
                IList segments;
                object selectionStart;

                selection = ReflectionUtils.GetProperty(Element, "Selection");
                segments = (IList)ReflectionUtils.GetField(selection, "_textSegments");
                selectionStart = ReflectionUtils.GetProperty(segments[0], "Start");
                rect = (Rect)ReflectionUtils.InvokeInterfaceMethod(selectionStart,
                    "ITextPointer", "GetCharacterRect", new object[] { LogicalDirection.Backward });
                rect.X -= 1; //Adjustment
                rect.Width += 4; //Adjustment
            }
            else
            {
                throw new InvalidOperationException("Cannot get caret for element " + Element);
            }

            return rect;
        }

        private Bitmap GetCaretBitmap()
        {
            System.Drawing.Bitmap bitmap;         // Colored image of the element
            //System.Drawing.Bitmap bitmapDark;     // Colored image after removing red color(<128)
            System.Drawing.Bitmap bitmapBW;       // Black and white image.

            //System.Drawing.Bitmap borderless;
            //using (bitmap = BitmapCapture.CreateBitmapFromElement(Element))
            //using (borderless = BitmapUtils.CreateBorderlessBitmap(bitmap, 1))
            //using (bitmapBW = BitmapUtils.ColorToBlackWhite(borderless, 200))

            using (bitmap = BitmapCapture.CreateBitmapFromElement(Element))
            // using (bitmapDark = ConvertNonBlackPixelsToWhite(bitmap))
            using (bitmapBW = BitmapUtils.ColorToBlackWhite(bitmap, 200))
            {
                Rect rect;

                rect = GetExpectedCaretRect();
                //Adjust the rectangle co-ordinates for RightToLeft FlowDirection                
                if (Element.FlowDirection == FlowDirection.RightToLeft ||
                    (Element is RichTextBox))
                {
                    bool rtlFlowDirection = (Element.FlowDirection == FlowDirection.RightToLeft) ? true : false;
                    if (Element is RichTextBox)
                    {
                        if (ElementWrapper.SelectionInstance.Start.Paragraph != null)
                        {
                            rtlFlowDirection = (ElementWrapper.SelectionInstance.Start.Paragraph.FlowDirection == FlowDirection.RightToLeft) ? true : false;
                        }
                        else
                        {
                            rtlFlowDirection = (((Block)(ElementWrapper.SelectionInstance.Start.Parent)).FlowDirection == FlowDirection.RightToLeft) ? true : false;
                        }
                    }
                    if (rtlFlowDirection)
                    {
                        rect.X = bitmapBW.Width - rect.X - rect.Width;
                    }
                }

                //Scale rect to Bitmap's DPI
                rect = BitmapUtils.AdjustBitmapSubAreaForDpi(bitmapBW, rect);

                return BitmapUtils.CreateSubBitmap(bitmapBW, rect);
            }
        }

        /// <summary>
        /// Restores the Foreground color to original state which gets changed in PrepareForCaretVerification.
        /// </summary>
        private void RestoreOriginalState()
        {
            if ((_originalBrush != null) && (_doSmartVerification))
            {
                Element.SetValue(TextElement.ForegroundProperty, _originalBrush);
                _originalBrush = null; //set it back to null
                _doSmartVerification = false;
            }
        }

        private void TestCaretBiDiPrivate(SimpleHandler callBackHandler)
        {
            System.Drawing.Bitmap caretBitmap;

            caretBitmap = GetCaretBitmap();            
            Logger.Current.LogImage(caretBitmap, "BiDiCaretBitmap_" + _caretTestTrailCount);
            _caretBiDi = BitmapUtils.VerifyCaretType(caretBitmap, CaretType.BiDi);
            _caretTestTrailCount++;

            if (!_caretBiDi)
            {
                Logger.Current.Log("BiDi Caret testing Trail#" + _caretTestTrailCount + ": Not Found");
                if (_caretTestTrailCount > 2)
                {
                    RestoreOriginalState(); //Restore the original state before returning
                    throw new ApplicationException("BiDi Caret is not rendered.");
                }
                else
                {
                    QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(CaretVerifier.SystemCaretBlinkTimeMS / 2),
                        new CaretRenderHandler(TestCaretBiDiPrivate), callBackHandler);
                }
            }
            else
            {
                RestoreOriginalState(); //Restore the original state before returning
                Logger.Current.Log("BiDi Caret testing Trail#" + _caretTestTrailCount + ": Found");
                QueueHelper.Current.QueueDelegate(callBackHandler);
            }
        }

        private void TestCaretBlinkingPrivate(SimpleHandler callBackHandler)
        {
            System.Drawing.Bitmap caretBitmap;

            caretBitmap = GetCaretBitmap();
            Logger.Current.LogImage(caretBitmap, "BlinkingCaretBitmap_" + _caretTestTrailCount);            

            _caretCaptureResult = null;
            _originalCaretBlinkTime = Win32.GetCaretBlinkTime();
            Logger.Current.Log("TestCaretBlinkingPrivate::System Caret Blink Time" + _originalCaretBlinkTime);
            Win32.SetCaretBlinkTime(_originalCaretBlinkTime * 2);
            DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);
            CaptureCaretPrivate(null);

            _caretBlinkingArray[_caretTestTrailCount] = _caretCaptureResult == null ? false : true;
            Logger.Current.Log("Caret Blinking Test - Caret Found: " + _caretBlinkingArray[_caretTestTrailCount]);
            _caretTestTrailCount++;

            RestoreOriginalState(); //Restore the original state before returning
            bool blinking = false;
            for (int i = 1; i < _caretBlinkingArray.Length; i++)
            {
                if (_caretBlinkingArray[0] != _caretBlinkingArray[i])
                {
                    blinking = true;
                    break;
                }
            }
            //Check that there is atleast one instance of false (caret not found) and
            //atleast one instance of true (caret found)
            if (blinking)
            {
                Logger.Current.Log("Caret is Blinking");
                QueueHelper.Current.QueueDelegate(callBackHandler);
            }
            else
            {
                throw new ApplicationException("Caret is not Blinking.");
            }
        }

        private void TestCaretItalicPrivate(SimpleHandler callBackHandler)
        {
            System.Drawing.Bitmap caretBitmap;

            caretBitmap = GetCaretBitmap();
            Logger.Current.LogImage(caretBitmap, "ItalicCaretBitmap_" + _caretTestTrailCount);            
            _caretItalic = BitmapUtils.CheckCaretItalic(caretBitmap);
            _caretTestTrailCount++;

            if (!_caretItalic)
            {
                Logger.Current.Log("Italic Caret testing Trail#" + _caretTestTrailCount + ": Not Found");
                if (_caretTestTrailCount > 2)
                {
                    RestoreOriginalState(); //Restore the original state before returning
                    throw new ApplicationException("Italic Caret is not rendered.");
                }
                else
                {
                    QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(CaretVerifier.SystemCaretBlinkTimeMS * 3 / 4),
                        new CaretRenderHandler(TestCaretItalicPrivate), callBackHandler);
                }
            }
            else
            {
                RestoreOriginalState(); //Restore the original state before returning
                Logger.Current.Log("Italic Caret testing Trail#" + _caretTestTrailCount + ": Found");
                QueueHelper.Current.QueueDelegate(callBackHandler);
            }
        }

        private void TestCaretRenderPrivate(SimpleHandler callBackHandler)
        {
            System.Drawing.Bitmap caretBitmap;
            System.Drawing.Rectangle rectangle;
            caretBitmap = GetCaretBitmap();
            Logger.Current.LogImage(caretBitmap, "CaretBitmap_" + _caretTestTrailCount);            

            _caretFound = BitmapUtils.GetTextCaret(caretBitmap, out rectangle);
            _caretTestTrailCount++;

            if (!_caretFound)
            {
                Logger.Current.Log("Caret testing Trail#" + _caretTestTrailCount + ": Not Found");
                if (_caretTestTrailCount > 2)
                {
                    Win32.SetCaretBlinkTime(_originalCaretBlinkTime);
                    RestoreOriginalState(); //Restore the original state before returning                    
                    throw new ApplicationException("Caret not rendered after " + _caretTestTrailCount +
                        "attempts. See CaretBitmap_* for the latest snapshot.");
                }
                else
                {
                    QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(CaretVerifier.SystemCaretBlinkTimeMS / 2),
                        new CaretRenderHandler(TestCaretRenderPrivate), callBackHandler);
                }
            }
            else
            {
                Win32.SetCaretBlinkTime(_originalCaretBlinkTime);
                RestoreOriginalState(); //Restore the original state before returning
                Logger.Current.Log("Caret testing Trail#" + _caretTestTrailCount + ": Found");
                QueueHelper.Current.QueueDelegate(callBackHandler);
            }
        }

        private unsafe Bitmap UnsafeConvertNonBlackPixelsToWhite(Bitmap bitmap)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            BitmapData sourceData = null;
            BitmapData destData = BitmapUtils.LockBitmapDataWrite(result);
            try
            {
                sourceData = BitmapUtils.LockBitmapDataRead(bitmap);
                int width = PixelData.GetScanLineWidth(bitmap.Size);
                for (int y = 0; y < bitmap.Height; y++)
                {
                    PixelData* sourcePixels;
                    PixelData* destPixels;
                    sourcePixels = (PixelData*)
                        ((byte*)sourceData.Scan0.ToPointer() + y * width);
                    destPixels = (PixelData*)
                        ((byte*)destData.Scan0.ToPointer() + y * width);
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        if (sourcePixels ->red <= 128)
                        {
                            *destPixels = *sourcePixels;
                        }
                        else
                        {
                            destPixels ->red = 255;
                            destPixels ->blue = 255;
                            destPixels ->green = 255;
                        }

                        sourcePixels++;
                        destPixels++;
                    }
                }
            }
            finally
            {
                result.UnlockBits(destData);
                if (sourceData != null)
                    bitmap.UnlockBits(sourceData);
            }
            return result;
        }

        #endregion Private methods.
    
    #region Private fields.

        /// <summary>Reference to UIElement for which caret is tested.</summary>
        private FrameworkElement _element;
        /// <summary>Wrapper to the UIElement for which caret is tested</summary>
        private UIElementWrapper _elementWrapper;

        /// <summary>Original Foreground of the FrameworkElement</summary>
        private System.Windows.Media.Brush _originalBrush;

        private delegate void CaretRenderHandler(SimpleHandler callBackHandler);

        private bool _caretFound;
        private bool[] _caretBlinkingArray;
        private Bitmap[] _caretCaptureArray;
        private bool _caretItalic;
        private bool _caretBiDi;
        private bool _doSmartVerification;

        private Bitmap _caretCaptureResult;
        private Rect _caretCaptureRect;

        private int _caretTestTrailCount;
        private int _originalCaretBlinkTime;

        #endregion Private fields.
    }
}