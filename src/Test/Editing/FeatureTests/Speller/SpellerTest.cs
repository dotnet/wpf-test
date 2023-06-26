// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Tests the TextBox/RichTextBox speller.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;
    using System.Runtime.CompilerServices;

    #endregion Namespaces.

    /// <summary>Tests the TextBox/RichTextBox speller. covers english and german</summary>
   // [Test(1, "Speller",  "SpellerTest", MethodName="DriverEntryPoint", MethodParameters = "/TestCaseType:SpellerTest", TestParameters="Class=EntryPointType")]
   [Test(1, "Speller", "SpellerTest", MethodParameters = "/TestCaseType:SpellerTest", Keywords = "Localization_Suite", Timeout = 200, Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]

    [TestOwner("Microsoft"), TestTitle("SpellerTest"), TestTactics("668"), TestWorkItem("133,134"), TestLastUpdatedOn("July 10, 2006")]
    public class SpellerTest : CustomTestCase
    {
        static SpellerTest()
        {
            s_states = new List<Callback>();

            for (int count = 0; count < s_misspelledSentences.Length; count++)
            {
                // For each sentence being tested, these series of actions would be repeated
                s_states.Add(new Callback(UpdateTestData));

                // TextBox
                s_states.Add(new Callback(AddTextBox));
                s_states.Add(new Callback(OnAdded));
                s_states.Add(new Callback(OnFirstInput));
                s_states.Add(new Callback(OnSecondInput));
                s_states.Add(new Callback(OnSpellCheckDisabled));
                s_states.Add(new Callback(OnSpellCheckEnabled));
                s_states.Add(new Callback(OnContextMenu));
                s_states.Add(new Callback(OnCorrection));
                // Testing IgnoreAll
                s_states.Add(new Callback(OnIgnoreAll));
                s_states.Add(new Callback(OnSelectIgnoreAll));
                s_states.Add(new Callback(OnIgnoreAllVerification));
                // RichTextBox
                s_states.Add(new Callback(AddRichTextBox));
                s_states.Add(new Callback(OnAdded));
                s_states.Add(new Callback(OnFirstInput));
                s_states.Add(new Callback(OnSecondInput));
                s_states.Add(new Callback(OnSpellCheckDisabled));
                s_states.Add(new Callback(OnSpellCheckEnabled));
                s_states.Add(new Callback(OnContextMenu));
                s_states.Add(new Callback(OnCorrection));
                // Testing IgnoreAll
                s_states.Add(new Callback(OnIgnoreAll));
                s_states.Add(new Callback(OnSelectIgnoreAll));
                s_states.Add(new Callback(OnIgnoreAllVerification));
            }
        }

        #region Main flow.

        /// <summary>
        /// Runs the test case.
        /// </summary>
        public override void RunTestCase()
        {
            Log("SpellerTest::RunTestCase");

            // Stale content in the clipboard sometimes interferes with these tests.
            Clipboard.Clear();

            // Move the mouse well away from the client area, so it doesn't interfere
            // with the context menu popup.

            MouseInput.MouseMove(0, 0);

            OnNextState(0);
        }

        // Runs the next action.
        private void OnNextState(int state)
        {
            Clipboard.SetData(DataFormats.Text, "Text");
            // Do the callback.
            ((Callback)s_states[state])(this);

            if (state + 1 < s_states.Count)
            {
                // Schedule the next callback.
                QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1.5), new QueueHelperCallback(OnNextState), new object[] { state + 1 });
            }
            else
            {
                // Report success and shutdown the test gracefully.
                Logger.Current.ReportSuccess();
            }
        }

        // State machine callback -- initializes a new TextBox.
        private static void AddTextBox(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::AddTextBox");
            TextBox tb = new TextBox();
            tb.FontSize = 20;
            tb.Height = 100;
            SetGermanLocale(tb);
            StackPanel sp = AddPanel(tb);
            thisTest.MainWindow.Content = sp;
        }

        private static StackPanel AddPanel(TextBoxBase tb)
        {
            StackPanel sp = new StackPanel();
            TextBox temp = new TextBox();

            sp.Children.Add(tb);
            sp.Children.Add(temp);
            return sp;
        }

        // State machine callback -- initializes a new RichTextBox.
        private static void AddRichTextBox(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::AddRichTextBox");
            RichTextBox rtb = new RichTextBox();
            rtb.FontSize = 20;
            rtb.Height = 100;
            SetGermanLocale(rtb);
            StackPanel sp = AddPanel(rtb);
            thisTest.MainWindow.Content = sp;
        }

        // State machine callback -- called after TextBox or RichTextBox is first rendered.
        // Takes keyboard focus and enters some intial text.
        private static void OnAdded(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnAdded");

            // Take keyboard focus.
            thisTest.Control.Focus();

            Assert(!thisTest.Control.SpellCheck.IsEnabled, "TextBoxBase.SpellCheck.IsEnabled does not default to false!");

            KeyboardInput.TypeString(thisTest._misspelledSentence);
        }

        // State machine callback -- called after first input sequence.
        // Creates a bitmap of the current content (which has no speller squigglies),
        // then clears the control, enables spelling, and re-inserts the content.
        private static void OnFirstInput(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnFirstInput");

            if (thisTest.Control.GetType() == typeof(RichTextBox))
            {
                Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence + "\r\n", "Unexpected control content!");
            }
            else
            {
                Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence, "Unexpected control content!");
            }


            Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(thisTest.Control);
            thisTest._noSpellerSnapShot = BitmapUtils.CreateBorderlessBitmap(bmp1, 3);

            NormalizeCaretRender(thisTest.Control, thisTest.WrappedControl.SelectionInstance, thisTest._noSpellerSnapShot);

            thisTest.Control.SpellCheck.IsEnabled = true;
            Assert((bool)thisTest.Control.GetValue(SpellCheck.IsEnabledProperty), "DP system does not reflect property setting!");

            if (thisTest.Control is TextBox)
            {
                ((TextBox)thisTest.Control).Clear();
            }
            else
            {
                ((RichTextBox)thisTest.Control).Document = new FlowDocument(new Paragraph(new Run()));
            }

            KeyboardInput.TypeString(thisTest._misspelledSentence);
        }

        // State machine callback -- called after second input sequence.
        // Verifies the current content has speller error squigglies, then
        // disables the speller.
        private static void OnSecondInput(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnSecondInput");

            if (thisTest.Control is RichTextBox)
            {
                Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence + "\r\n", "Unexpected control content!");
            }
            else
            {
                Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence, "Unexpected control content!");
            }

            if (s_isControlCultureSupported)
            {
                Assert(HasSpellerSquiggles(thisTest), "No speller squiggle detected!");
            }
            else
            {
                Logger.Current.Log("Skipping verification of Squiggles because current culture is not supported by speller");
            }

            thisTest.Control.SetValue(SpellCheck.IsEnabledProperty, false);
            Assert(!thisTest.Control.SpellCheck.IsEnabled, "CLR property value out of synch with DP value!");
        }

        // State machine callback -- called after the speller is shut down.
        // Verifies the speller squiggles have disappeared, then re-enables
        // the speller.
        private static void OnSpellCheckDisabled(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnSpellCheckDisabled");

            if (thisTest.Control.GetType() == typeof(RichTextBox))
            {
                Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence + "\r\n", "Unexpected control content!");
            }
            else
            {
                Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence, "Unexpected control content!");
            }

            if (s_isControlCultureSupported)
            {
                Assert(!HasSpellerSquiggles(thisTest), "Speller squiggle detected!");
            }
            else
            {
                Logger.Current.Log("Skipping verification of Squiggles because current culture is not supported by speller");
            }

            thisTest.Control.SpellCheck.IsEnabled = true;
        }

        // State machine callback -- called after the speller is re-enabled.
        // Selects a misspelled word and invokes the context menu.
        private static void OnSpellCheckEnabled(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnSpellCheckEnabled");
            if (s_isControlCultureSupported)
            {
                Assert(HasSpellerSquiggles(thisTest), "No speller squiggle detected!");
            }
            else
            {
                Logger.Current.Log("Skipping verification of Squiggles because current culture is not supported by speller");
            }

            // Move the caret to a misspelled word.
            thisTest.WrappedControl.Select(thisTest._misspelledWordOffset, 0);

            if (s_isControlCultureSupported)
            {
                // Bring up the context menu.
                KeyboardInput.TypeString("+{F10}");
            }
        }

        // State machine callback -- called after context menu opens.
        // Selects the first context menu spelling suggestion.
        private static void OnContextMenu(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnContextMenu");

            if (s_isControlCultureSupported)
            {
                KeyboardInput.TypeString("{DOWN}{ENTER}");
            }
        }

        // State machine callback -- called after context menu closes.
        // Verifies the misspelled word we previously corrected is corrected.
        private static void OnCorrection(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnCorrection");

            if (s_isControlCultureSupported)
            {
                if (thisTest.Control.GetType() == typeof(RichTextBox))
                {
                    Assert(thisTest.WrappedControl.Text == thisTest._correctedSentence + "\r\n", "RichTextBox content not corrected!");
                }
                else
                {
                    Assert(thisTest.WrappedControl.Text == thisTest._correctedSentence, "RichTextBox content not corrected!");
                }
                // Do Undo, then mouse click away from the word "forz" so the squiggle comes back
                KeyboardInput.TypeString("^z{LEFT 4}");
            }
            else
            {
                Logger.Current.Log("Skipping verification of correction because current culture is not supported by speller");
            }
        }

        // State machine callback -- called after undo
        // Selects a misspelled word and invokes the context menu.
        private static void OnIgnoreAll(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnIgnoreAll");

            // Move the caret to a misspelled word.
            thisTest.WrappedControl.Select(thisTest._misspelledWordOffset /*Inside "forz"*/, 0);

            if (s_isControlCultureSupported)
            {
                // Bring up the context menu.
                KeyboardInput.TypeString("+{F10}");
            }
        }

        // State machine callback -- called after context menu opens.
        // Selects Ignore All from context menu.
        private static void OnSelectIgnoreAll(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnSelectIgnoreAll");

            if (s_isControlCultureSupported)
            {
                KeyboardInput.TypeString("{UP}{UP}{ENTER}{TAB}asb");
            }
        }

        // State machine callback -- called after context menu closes.
        // Verifies the misspelled word is ignored.
        private static void OnIgnoreAllVerification(SpellerTest thisTest)
        {
            thisTest.Log("SpellerTest::OnIgnoreAllVerification");
            if (s_isControlCultureSupported)
            {
                Assert(!HasSpellerSquigglesStable(thisTest), "There should be no sqiggle after ignoreall.");

                if (thisTest.Control.GetType() == typeof(RichTextBox))
                {
                    Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence + "\r\n", "misspell word should remain after IgnoreAll in RTB.");
                }
                else
                {
                    Assert(thisTest.WrappedControl.Text == thisTest._misspelledSentence, "misspell word should remain after IgnoreAll in TB.");
                }
            }
            else
            {
                Logger.Current.Log("Skipping verification of correction because current culture is not supported by speller");
            }

            thisTest.Control.SpellCheck.IsEnabled = false;
        }

        private static void UpdateTestData(SpellerTest thisTest)
        {
            thisTest._testDataIndex++;
            Assert(thisTest._testDataIndex < SpellerTest.s_misspelledSentences.Length, "There is something wrong with List<Callback> _states collection's setup");

            thisTest._misspelledSentence = SpellerTest.s_misspelledSentences[thisTest._testDataIndex];
            thisTest._correctedSentence = SpellerTest.s_correctedSentences[thisTest._testDataIndex];
            thisTest._misspelledWordOffset = SpellerTest.s_misspelledWordOffsets[thisTest._testDataIndex];
        }

        #endregion Main flow.

        #region Helper methods.

        public static bool IsSpellerAvailableForLanguage(XmlLanguage language)
        {
            bool isTestBuildTargetedFor461OrGreater = false;
#if TESTBUILD_NET_ATLEAST_461
            // Starting .NET 4.6.1, we use ISpellChecker for spell-checking
            isTestBuildTargetedFor461OrGreater = true;
#endif
            if ((System.Environment.Version.Major < 4) && (Environment.OSVersion.Version.Major >= 6))
            {
                // In Vista or higher with Framework version < 4 (3.0/3.5), all wpf speller dlls are in the system and accessible by WPF.
                if ((language.IetfLanguageTag == "en-us") ||
                (language.IetfLanguageTag == "de-de") ||
                (language.IetfLanguageTag == "fr-fr") ||
                (language.IetfLanguageTag == "es-es"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (!Microsoft.Test.Utilities.OSVersion.IsWindows8Point1OrGreater() || !isTestBuildTargetedFor461OrGreater)
            {
                switch (language.IetfLanguageTag)
                {
                    case "en-us":
                        return true;
                    case "de-de":
                        if (Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture().LCID == 1031)
                        {
                            return true;
                        }
                        break;
                    case "fr-fr":
                        if (Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture().LCID == 1036)
                        {
                            return true;
                        }
                        break;
                    case "es-es":
                        if (Microsoft.Test.Globalization.LanguagePackHelper.CurrentWPFUICulture().LCID == 3082)
                        {
                            return true;
                        }
                        break;
                }
                return false;
            }
            else // Windows 8.1 or greater
            {
                MsSpellCheckLib.RCW.ISpellCheckerFactory spellCheckerFactory = null;
                try
                {
                    spellCheckerFactory = new MsSpellCheckLib.RCW.SpellCheckerFactoryClass();
                    return spellCheckerFactory.IsSupported(language.IetfLanguageTag) != 0;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    Marshal.ReleaseComObject(spellCheckerFactory);
                }
            }
        }

        /// <summary>return IetfLanguageTags</summary>
        public static string IetfLanguageTags(string language)
        {
            language = language.ToLower();
            switch (language)
            {
                case "japanese":
                    return "ja-jp";

                case "german":
                    return "de-de";

                case "spanish":
                    return "es-es";

                case "arabic":
                    return "ar-sa";

                case "french":
                    return "fr-fr";

                default:
                    return "en-us";
            }
        }

        // The current active control.  This will be TextBox or RichTextBox.
        private TextBoxBase Control
        {
            get
            {
                return ((StackPanel)(this.MainWindow.Content)).Children[0] as TextBoxBase;
            }
        }

        // The wrapped control.  Provides a number of useful helpers
        // that streamline using TextBox/RichTextBox interchangeably.
        private UIElementWrapper WrappedControl
        {
            get
            {
                return new UIElementWrapper(this.Control);
            }
        }

        // Verifies a condition is true.  If false, throws an ApplicationException.
        private static void Assert(bool condition, string errorMessage)
        {
            if (!condition)
            {
                throw new ApplicationException(errorMessage);
            }
        }

        // Returns true if this.Control does not match the _noSpellerSnapShot.
        private static bool HasSpellerSquiggles(SpellerTest thisTest)
        {
            ComparisonCriteria criteria;
            ComparisonOperation operation;
            ComparisonResult result;
            Bitmap snapShot;

            Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(thisTest.Control);
            snapShot = BitmapUtils.CreateBorderlessBitmap(bmp1, 3);

            //snapShot = BitmapCapture.CreateBitmapFromElement(thisTest.Control);
            Logger.Current.LogImage(snapShot, "IMAGE");

            NormalizeCaretRender(thisTest.Control, thisTest.WrappedControl.SelectionInstance, snapShot);

            criteria = new ComparisonCriteria();

            // We need to allow some wiggle room because successive renders
            // may have different anti-aliasing artifacts.  Fortunately the
            // squiggles themselves are higher constrast than the artifacts.
            criteria.MaxColorDistance = 0.08f;

            operation = new ComparisonOperation();
            operation.Criteria = criteria;
            operation.MasterImage = thisTest._noSpellerSnapShot;
            Logger.Current.LogImage(operation.MasterImage, "master IMAGE");

            operation.SampleImage = snapShot;
            ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(operation.MasterImage, snapShot, criteria, true);
            result = operation.Execute();
           // ((TextBoxBase)((StackPanel)(this.MainWindow.Content)).Children[0]).Focus();
            return !result.CriteriaMet;
        }

        // Returns true if this.Control does not match the _noSpellerSnapShot.
        private static bool HasSpellerSquigglesStable(SpellerTest thisTest)
        {
            while (((TextBoxBase)((StackPanel)(thisTest.Control.Parent)).Children[1]).IsFocused == false)
            {
                Logger.Current.Log("Inside Loop to remove focus from the TestElement");
                ((TextBoxBase)((StackPanel)(thisTest.Control.Parent)).Children[1]).Focus();
            }
            ComparisonCriteria criteria;
            ComparisonOperation operation;
            ComparisonResult result;
            Bitmap snapShot;
            Bitmap bmp1 = BitmapCapture.CreateBitmapFromElement(thisTest.Control);
            snapShot = BitmapUtils.CreateBorderlessBitmap(bmp1, 3);

            criteria = new ComparisonCriteria();

            // We need to allow some wiggle room because successive renders
            // may have different anti-aliasing artifacts.  Fortunately the
            // squiggles themselves are higher constrast than the artifacts.
            criteria.MaxErrorProportion = 0.1f;

            operation = new ComparisonOperation();
            operation.Criteria = criteria;
            operation.MasterImage = thisTest._noSpellerSnapShot;

            operation.SampleImage = snapShot;
            Logger.Current.LogImage(snapShot, "Current Image");
            Logger.Current.LogImage(operation.MasterImage, "Master Image");
            Bitmap diff;
            ComparisonOperationUtils.AreBitmapsEqual(operation.MasterImage, snapShot, out diff);
            if (diff != null)
            {
                Logger.Current.LogImage(diff, "Differences");
            }
            result = operation.Execute();
            ((TextBoxBase)((StackPanel)(thisTest.Control.Parent)).Children[0]).Focus();
            return !result.CriteriaMet;
        }

        // Obscures the region surrounding the caret with a uniform black brush.
        // This elimates non-determinism caused by the caret animation -- the output
        // bitmap has constant color around the caret, regardless of what the caret
        // animation is doing.
        //
        // visual - the Visual from which clientAreaBitmap was taken, usually a Control.
        // selection - the matching TextSelection.  Expected to be empty (ie, a caret).
        // clientAreaBitmap - a bitmap of visual's client area to normalize.
        private static void NormalizeCaretRender(Visual visual, TextSelection selection, Bitmap clientAreaBitmap)
        {
            if (!selection.IsEmpty)
            {
                // A non-empty selection means no caret.
                Assert(false, "Unexpected non-empty selection!");
            }

            if (!selection.Start.HasValidLayout)
            {
                // Can't get the render bounds without a valid layout.
                Assert(false, "Unexpected dirty layout!");
            }

            // Get the caret position.
            Rect rectLeft = selection.Start.GetCharacterRect(LogicalDirection.Backward);
            Rect rectRight = selection.Start.GetCharacterRect(LogicalDirection.Forward);

            // Transform the device units (pixels).
            PresentationSource source = PresentationSource.FromVisual(visual);
            System.Windows.Point deviceUpperLeft = source.CompositionTarget.TransformToDevice.Transform(new System.Windows.Point(rectLeft.X, rectLeft.Y));
            System.Windows.Point deviceLowerRight = source.CompositionTarget.TransformToDevice.Transform(new System.Windows.Point(rectRight.X + rectRight.Width, rectRight.Y + rectRight.Height));

            // Expand the area to account for any aliasing artifacts.
            int x = (int)deviceUpperLeft.X - 4;
            int y = (int)deviceUpperLeft.Y - 4;
            int width = (int)(deviceLowerRight.X - deviceUpperLeft.X) + 8;
            int height = (int)(deviceLowerRight.Y - deviceUpperLeft.Y) + 8;

            Rectangle rectangle = new Rectangle(x, y, width, height);

            // Overwrite with a solid black brush.
            Graphics.FromImage(clientAreaBitmap).FillRectangle(new SolidBrush(System.Drawing.Color.White), rectangle);

        }

        /// <summary>
        /// Returns true if current culture is supported by speller
        /// </summary>
        public static bool IsCurrentCultureSupported
        {
            get
            {
                CultureInfo currentCultureInfo;
                currentCultureInfo = CultureInfo.CurrentCulture;

                //Speller supported languages. White list taken from
                //windows\wcp\Framework\System\Windows\Documents\Speller.cs
                if ((currentCultureInfo.TwoLetterISOLanguageName == "en") ||
                    (currentCultureInfo.TwoLetterISOLanguageName == "de") ||
                    (currentCultureInfo.TwoLetterISOLanguageName == "fr") ||
                    (currentCultureInfo.TwoLetterISOLanguageName == "es"))
                {
                    return true;
                }

                Logger.Current.Log("CultureInfo.TwoLetterISOLanguageName: " +
                    currentCultureInfo.TwoLetterISOLanguageName.ToString());
                return false;
            }
        }

        private static void SetGermanLocale(UIElement _element)
        {
            if (IsSpellerAvailableForLanguage(XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german"))))
            {
                Loggers.Logger.Current.Log("setting german locale-------------------------------------");
                KeyboardInput.SetActiveInputLocale(InputLocaleData.German.Identifier);
                ((TextBoxBase)_element).Language = XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german"));
            }
        }

        #endregion Helper methods.

        #region Delegates

        // Delegate for QueueHelper callbacks.
        private delegate void QueueHelperCallback(int state);

        // Delegate for OnNextState callbacks.
        private delegate void Callback(SpellerTest thisTest);

        #endregion Delegates

        #region Private members

        // A bitmap of the control rending _misspelledSentence WITHOUT the speller enabled.
        // Used as a baseline to detect the precence or absence of squiggles.
        private Bitmap _noSpellerSnapShot;

        // Array of actions to run from OnNextState.
        private static List<Callback> s_states;
        // Initial TextBox/RichTextBox content.
        private string _misspelledSentence;

        // Expected corrected TextBox/RichTextBox content.
        private string _correctedSentence; //

        // Offset into the misspelled word this test corrects.
        private int _misspelledWordOffset; //

        // Index into test data array
        private int _testDataIndex = -1;

        private static bool s_isControlCultureSupported = true;

        #region Test Data

        // Something about the way this test is built requires an offset of roughly 7,
        // and an assumption that the first word spells like "The ". For e.g., a
        // reasonable misspelled sentence would be of the form "The <misspelled word> word word word".
        // This is likely an idiosyncracy that is the result of harcoded <BKSP> inputs
        // in the test that assumes how many characters to traverse back before reaching a
        // preceeding valid word.
        private static string[] s_misspelledSentences = new string[]
        {
            "The fortx a fortx spring on spring",
            "The Managment's job is managing"         // DevDiv:912: CLR 4.6 WPF - RichTextBox spellcheck error on possesives
        };

        private static string[] s_correctedSentences = new string[]
        {
            "The fort a fortx spring on spring",  // offset 7
            "The Management's job is managing"        // offset 7
        };

        // Index Inside "fortx" and "Managment's" etc. where a
        // right-click would be effected to bring up spelling suggestions
        private static int[] s_misspelledWordOffsets = new int[]
        {
            7,
            7
        };

        #endregion

        #endregion Private members
    }

    /// <summary>
    /// Test which posts a background item to remove TextBox/RichTextBox from tree
    /// when large amount of text is being spell checked. SpellCheck code runs in idle propriety.
    /// </summary>
    [Test(2, "Speller", "SpellerEditorTest", MethodParameters = "/TestCaseType:SpellerEditorTest", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"), TestTactics("667"), TestBugs(""), TestWorkItem("27")]
    public class SpellerEditorTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _textBoxBase = (TextBoxBase)_editableType.CreateInstance();
            _textBoxBase.Language = _controlLanguage;
            _textBoxBase.SpellCheck.IsEnabled = true;
            _wrapper = new UIElementWrapper(_textBoxBase);

            _panel = new StackPanel();
            _button = new Button();
            _panel.Children.Add(_button);
            _panel.Children.Add(_textBoxBase);

            TestElement = _panel;

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new SimpleHandler(AddText));
        }

        private void AddText()
        {
            //content size approximately 3KB
            _wrapper.Text = TextUtils.RepeatString("Somple text to repet.\r\n", 1024);
            //Post a background item to remove the TextControl from Tree
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new SimpleHandler(RemoveTextControlFromTree));
        }

        private void RemoveTextControlFromTree()
        {
            //Remove TextBox/RichTextBox from tree.
            //Excepted no crash or exception.
            _panel.Children.Remove(_textBoxBase);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new SimpleHandler(FinishTest));
        }

        private void FinishTest()
        {
            Log("FinishTest called");
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new SimpleHandler(NextCombination));
        }

        #region Data fields

        /// <summary>
        /// array of languages
        /// </summary>
        public static XmlLanguage[] XmlLanguages = { XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("english")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("french")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("spanish"))};
        private XmlLanguage _controlLanguage = null;
        private TextEditableType _editableType = null;
        private TextBoxBase _textBoxBase;
        private UIElementWrapper _wrapper;
        private StackPanel _panel;
        private Button _button;

        #endregion Data fields
    }

    /// <summary>Test all public speller APIs in TB and RTB. Includes negative test</summary>
    [Test(0, "Speller", "SpellerAPITest", MethodParameters = "/TestCaseType=SpellerAPITest", Keywords = "Localization_Suite", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"), TestTactics("665"), TestBugs(""), TestWorkItem("132"),TestLastUpdatedOn("Jan 25, 2007")]
    public class SpellerAPITest : ManagedCombinatorialTestCase
    {
        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            return result = result && !_editableType.IsPassword;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_wrapper.Element;

            // Testing SetIsEnabled with null for TextBoxBase
            try
            {
                SpellCheck.SetIsEnabled(null, true);
                throw new Exception("SetIsEnabled doesn't accept null value.");
            }
            catch (ArgumentNullException)
            {
                Log("Thrown as expected - setting null to SetIsEnabled throws an exception.");
            }

            // Testing default value (false) for SpellCheck.IsEnabled
            Verifier.Verify(!((TextBoxBase)_wrapper.Element).SpellCheck.IsEnabled, "SpellCheck.IsEnabled is false by default.");

            // Set true to SpellCheck.SetIsEnabled
            SpellCheck.SetIsEnabled((TextBoxBase)_wrapper.Element, true);

            // Testing get value (true) for SpellCheck.IsEnabled
            Verifier.Verify(((TextBoxBase)_wrapper.Element).SpellCheck.IsEnabled, "SpellCheck.IsEnabled is set to true.");

            ((TextBoxBase)(_wrapper.Element)).Language = XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("ENGLISH"));
            if (TestElement is RichTextBox)
            {
                _rtb = (RichTextBox)_wrapper.Element;
                _run = new Run(_content);
                _run.Language = _rtb.Language;
                _rtb.Document.Blocks.Clear(); // remove default Paragraph
                _rtb.Document.Blocks.Add(new Paragraph(_run)); // Add a new paragraph with a run
                QueueDelegate(GetSpellingErrorFromRichTextBox);
            }
            else if (TestElement is TextBox)
            {
                _wrapper.Text = _content;
                _tb = (TextBox)_wrapper.Element;
                QueueDelegate(GetSpellingErrorFromTextBox);
            }
        }

        #region SpellerInRichTextBox

        /// <summary>Test position that is not in misspell word as well</summary>
        private void GetSpellingErrorFromRichTextBox()
        {
            Log("Testing Speller in RichTextBox...");
            // Invalid position testing GetSpellingError, GetSpellingErrorRange
            _spellingError = _rtb.GetSpellingError(_rtb.Document.ContentEnd);
            Verifier.Verify(_spellingError == null, "Null expected for ContentEnd.");

            _spellingError = _rtb.GetSpellingError(_rtb.Document.ContentStart);
            Verifier.Verify(_spellingError == null, "Null expected for ContentStart.");

            Verifier.Verify(_rtb.GetSpellingErrorRange(_rtb.Document.ContentStart) == null, "Null expected.");
            Verifier.Verify(_rtb.GetSpellingErrorRange(_rtb.Document.ContentEnd) == null, "Null expected.");

            // Valid position
            _tp = _run.ContentStart.GetPositionAtOffset(_charIndexInside1stError);
            _spellingError = _rtb.GetSpellingError(_tp);

            _spellingError.Correct(s_correctString); //Correct the misspell word with a word that is not in the suggestion list.
            VerifyString(_correctContent, _run.Text, "Misspell word is corrected.");

            _rtb.Undo();
            VerifyString(_content, _run.Text, "Undo for corrected word is undone.");
            QueueDelegate(GetNextSpellingErrorPosition);
        }

        /// <summary>GetNextSpellingErrorPosition</summary>
        private void GetNextSpellingErrorPosition()
        {
            _tp = _run.ContentStart;
            _tp = _rtb.GetNextSpellingErrorPosition(_tp, LogicalDirection.Forward);
            VerifyString("isss a asdfsaf test.", _tp.GetTextInRun(LogicalDirection.Forward), "TextPointer is at first error.");

            Verifier.Verify(HasSuggestions(_rtb.GetSpellingError(_tp)), "This misspell word has suggestions.");
            Verifier.Verify(SuggestionCount(_rtb.GetSpellingError(_tp)) == 10, "10 suggestions count.");

            _tp = _run.ContentStart.GetPositionAtOffset(_charIndexInside2ntError);
            _tp = _rtb.GetNextSpellingErrorPosition(_tp, LogicalDirection.Forward);
            VerifyString("asdfsaf test.", _tp.GetTextInRun(LogicalDirection.Forward), "TextPointer is at 2nd error.");

            Verifier.Verify(!HasSuggestions(_rtb.GetSpellingError(_tp)), "This misspell word doesn't have suggestion.");
            VerifyInt(0, SuggestionCount(_rtb.GetSpellingError(_tp)), "0 suggestions count.");

            _tp = _run.ContentEnd;
            _tp = _rtb.GetNextSpellingErrorPosition(_tp, LogicalDirection.Backward);
            VerifyString("This isss a ", _tp.GetTextInRun(LogicalDirection.Backward), "TextPointer is moved to misspell word.");

            Verifier.Verify(!HasSuggestions(_rtb.GetSpellingError(_tp)), "This misspell word doesn't have any suggestion.");
            VerifyInt(0, SuggestionCount(_rtb.GetSpellingError(_tp)), "0 suggestions.");

            _tp = _run.ContentStart.GetPositionAtOffset(_charIndexInside2ntError);
            _rtb.GetSpellingError(_tp).IgnoreAll();
            VerifyString(_content, _run.Text, "IgnoreAll: Misspell word is Ignored.");

            // Since privous step call ignoreall, there is only 1 misspell word "isss"
            _tp = _rtb.GetNextSpellingErrorPosition(_tp, LogicalDirection.Backward);
            VerifyString("isss", _rtb.GetSpellingErrorRange(_tp).Text, "TextRange has moved to the misspell word.");

            QueueDelegate(TestIgnoreAllCommand);
        }

        #endregion SpellerInRichTextBox

        #region SpellerInTextBox
        /// <summary>GetSpellingError in TextBox</summary>
        private void GetSpellingErrorFromTextBox()
        {
            Log("Testing Speller in TextBox...");
            // Invalid charIndex testing in GetSpellingError
            try
            {
                _spellingError = _tb.GetSpellingError(-1);
                throw new Exception("SpellingError.GetSpellingError doesn't accept negative value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting negative to GetSpellingError throw exception.");
            }

            try
            {
                _spellingError = _tb.GetSpellingError(_tb.Text.Length + 1);
                throw new Exception("SpellingError.GetSpellingError doesn't accept max+1 value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting max+1 to GetSpellingError throw exception.");
            }

            _spellingError = _tb.GetSpellingError(_tb.Text.Length - 4); // charIndex is between t and e in test
            Verifier.Verify(_spellingError == null, "Null is expected when charIndex is in non-misspell word.");

            _spellingError = _tb.GetSpellingError(_charIndexInside1stError); // charIndex is between i and s in "isss"

            // Correct error with null, the error string will be replaced wiht string.Empty
            _spellingError.Correct(null);
            VerifyString("This  a asdfsaf test.", _wrapper.Text, "Misspell word is replaced with empty string.");
            _tb.Undo(); // bring back that misspell word.

            _spellingError.Correct(s_correctString);   //Correct the misspell word with a word that is not in the suggestion list.
            VerifyString(_correctContent, _wrapper.Text, "Misspell word is corrected.");

            _tb.Undo();
            VerifyString(_content, _wrapper.Text, "Undo is undone.");

            QueueDelegate(GetSpellingErrorStartLength);
        }

        /// <summary>Test GetSpellingErrorStart</summary>
        private void GetSpellingErrorStartLength()
        {
            //Negative test for -1 and max+1
            try
            {
                _tb.GetSpellingErrorStart(-1);
                throw new Exception("SpellingError.GetSpellingErrorStart doesn't accept negative value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting negative to GetSpellingErrorStart throw exception.");
            }

            try
            {
                _tb.GetSpellingErrorStart(_tb.Text.Length + 1);
                throw new Exception("SpellingError.GetSpellingErrorStart doesn't accept max+1 value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting max+1 to GetSpellingErrorStart throw exception.");
            }

            _comment = "tb.Text.Length => return -1";
            VerifyInt(-1, _tb.GetSpellingErrorStart(_tb.Text.Length), _comment);

            _comment = "charIndex in the misspell word => return charIndex infron of misspell word.";
            VerifyInt(_charIndexInfront1stError, _tb.GetSpellingErrorStart(_charIndexInside1stError), _comment);

            _comment = "charIndex in front of misspell word => return charIndex infron of misspell word.";
            VerifyInt(_charIndexInfront1stError, _tb.GetSpellingErrorStart(_charIndexInfront1stError), _comment);

            _comment = "charIndex = end of misspell word => return -1.";
            VerifyInt(-1, _tb.GetSpellingErrorStart(_charIndexAfter1stError), _comment);

            QueueDelegate(GetSpellingErrorLength);
        }

        /// <summary>Test GetSpellingErrorLength</summary>
        private void GetSpellingErrorLength()
        {
            //Negative test for -1 and max+1
            try
            {
                _tb.GetSpellingErrorLength(-1);
                throw new Exception("SpellingError.GetSpellingErrorLength doesn't accept negative value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting negative value to GetSpellingErrorLength throw exception.");
            }

            try
            {
                _tb.GetSpellingErrorLength(_tb.Text.Length + 1);
                throw new Exception("SpellingError.GetSpellingErrorLength doesn't accept max+1 value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting max+1 value to GetSpellingErrorLength throw exception.");
            }
            _comment = "tb.Text.Length => return 0.";
            VerifyInt(0, _tb.GetSpellingErrorLength(_tb.Text.Length), _comment);

            _comment = "charIndex inside misspell word => return lengh of misspell word.";
            VerifyInt(_firstErrorLength, _tb.GetSpellingErrorLength(_charIndexInside1stError), _comment);

            _comment = "charInex at infront of misspell word => return lengh of misspell word.";
            VerifyInt(_firstErrorLength, _tb.GetSpellingErrorLength(_charIndexInfront1stError), _comment);

            _comment = "charIndex at then end of misspell word => return 0.";
            VerifyInt(0, _tb.GetSpellingErrorLength(_charIndexAfter1stError), _comment);

            QueueDelegate(GetSpellingErrorCharIndex);
        }

        /// <summary>Test GetNextSpellingErrorCharacterIndex</summary>
        private void GetSpellingErrorCharIndex()
        {
            LogicalDirection direction = LogicalDirection.Forward;
            int charIndex = 0;

            //---GetNextSpellingErrorCharacterIndex(charIndex, Forward)
            try // negative test for charIndex<0
            {
                _tb.GetNextSpellingErrorCharacterIndex(-1, direction);
                throw new Exception("GetNextSpellingErrorCharacterIndex doesn't accept negative value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting negative to GetNextSpellingErrorCharacterIndex throw exception.");
            }

            _comment = "End of doc => return -1.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_tb.Text.Length, direction);
            VerifyInt(-1, charIndex, _comment);

            _comment = "charIndex infront of 1st misspell word => return charIndex infront of 1st misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexInfront1stError, direction);
            VerifyInt(_charIndexInfront1stError, charIndex, _comment);

            _comment = "charIndex inside 1st misspell word => return charIndex infront of 1st misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexInside1stError, direction);
            VerifyInt(_charIndexInfront1stError, charIndex, _comment);

            _comment = "charIndex after 1st misspell word but infront of second misspell word => return charIndex infront of 2nd misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexAfter1stError, direction);
            VerifyInt(_charIndexInfront2ntError, charIndex, _comment);

            _comment = "charIndex after last misspell word in doc => return -1.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexAfter2ntError, direction);
            VerifyInt(-1, charIndex, _comment);

            //---GetNextSpellingErrorCharacterIndex(charIndex, Backward)
            direction = LogicalDirection.Backward;
            try // negative test for charIndex>charCount
            {
                _tb.GetNextSpellingErrorCharacterIndex(_tb.Text.Length + 1, direction);
                throw new Exception("GetNextSpellingErrorCharacterIndex doesn't accept max+1 value");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("Thrown as expected - setting max+1 to GetNextSpellingErrorCharacterIndex throw exception.");
            }

            _comment = "charIndex after 2nd misspell word => return position infront of that 2nd misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexAfter2ntError, direction);
            VerifyInt(_charIndexInfront2ntError, charIndex, _comment);

            _comment = "charIndex inside 2nd misspell word => return position infront of that 2nd misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexInside2ntError, direction);
            VerifyInt(_charIndexInfront2ntError, charIndex, _comment);

            _comment = "charIndex infront of 2nd misspell word => return position infront of 1st misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexInfront2ntError, direction);
            VerifyInt(_charIndexInfront1stError, charIndex, _comment);

            _comment = "charIndex infront of 1st misspell word => return -1.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_charIndexInfront1stError, direction);
            VerifyInt(-1, charIndex, _comment);

            _comment = "charIndex at end of doc => return position infront of that 2nd misspell word.";
            charIndex = _tb.GetNextSpellingErrorCharacterIndex(_tb.Text.Length, direction);
            VerifyInt(_charIndexInfront2ntError, charIndex, _comment);

            QueueDelegate(TestSuggestionsAndIgnoreAll);
        }

        /// <summary>Test both Suggestions and IgnoreAll</summary>
        private void TestSuggestionsAndIgnoreAll()
        {
            _spellingError = _tb.GetSpellingError(_charIndexInside2ntError); // position inside second misspell word
            Verifier.Verify(!HasSuggestions(_spellingError), "This error doesn't have suggestion.");
            VerifyInt(0, SuggestionCount(_spellingError), "There is 0 of suggestions.");

            _spellingError.IgnoreAll();
            VerifyString(_content, _wrapper.Text, "IgnoreAll works.");

            _spellingError = _tb.GetSpellingError(_charIndexInside1stError); // charIndex is between i and s in "isss"
            Verifier.Verify(HasSuggestions(_spellingError), "This error has suggestion.");
            VerifyInt(10, SuggestionCount(_spellingError), "There are 10 of suggestions.");

            // calling GetNextSpellingErrorCharacterIndex with empty doc
            _wrapper.Text = "";
            VerifyInt(-1, _tb.GetNextSpellingErrorCharacterIndex(0, LogicalDirection.Forward), "Empty doc => return -1.");

            QueueDelegate(TestIgnoreAllCommand);
        }
        #endregion SpellerInTextBox

        /// <summary>IgnoreSpellingError command is test for both RTB and TB</summary>
        private void TestIgnoreAllCommand()
        {
            RoutedCommand ignore = EditingCommands.IgnoreSpellingError;
            if (TestElement is RichTextBox)
            {
                _rtb.Document.Blocks.Remove(_rtb.Document.Blocks.FirstBlock);
                _rtb.Document.Blocks.Add(new Paragraph(_run));
                _spellingError = _rtb.GetSpellingError(_run.ContentStart.GetPositionAtOffset(_charIndexInside2ntError));
                ignore.Execute(_spellingError, _rtb);
                Verifier.Verify(_rtb.GetSpellingError(_run.ContentStart.GetPositionAtOffset(_charIndexInside2ntError)) == null,
                    "Verifying that IgnoreAll command works in RTB");
                VerifyString(_run.Text, _content, "Verifying that content didnt change after IgnoreAll command in RTB.");
            }
            else
            {
                _wrapper.Text = _content;
                _spellingError = _tb.GetSpellingError(_charIndexInside2ntError);
                ignore.Execute(_spellingError, _tb);
                Verifier.Verify(_tb.GetSpellingError(_charIndexInside2ntError) == null,
                    "Verifying IgnoreAll command works in TB");
                VerifyString(_wrapper.Text, _content, "Verifying that content didnt change after IgnoreAll command in TB.");
            }
            QueueDelegate(TestCorrectCommand);
        }

        /// <summary>CorrectSpellingError command is test for both RTB and TB</summary>
        private void TestCorrectCommand()
        {
            RoutedCommand correct = EditingCommands.CorrectSpellingError;
            if (TestElement is RichTextBox)
            {
                _rtb.CaretPosition = _run.ContentStart.GetPositionAtOffset(_charIndexInside1stError);
                correct.Execute(s_correctString, _rtb);
                VerifyString(_correctContent + "\r\n", _wrapper.Text, "Correct command works in RTB.");
            }
            else
            {
                _tb.CaretIndex = _charIndexInside1stError;
                correct.Execute(s_correctString, _tb);
                VerifyString(_correctContent, _wrapper.Text, "Correct command works in TB.");
            }
            QueueDelegate(NextCombination);
        }

        #region private helper methods

        /// <summary>Return teh number of suggestions word</summary>
        private int SuggestionCount(SpellingError se)
        {
            ArrayList alist = new ArrayList();
            foreach (string suggestion in se.Suggestions)
            {
                alist.Add(suggestion);
            }
            return alist.Count;
        }

        /// <summary>Return true if there is suggestions, false otherwise</summary>
        private bool HasSuggestions(SpellingError se)
        {
            ArrayList alist = new ArrayList();
            bool addedSuggestion = false;
            foreach (string suggestion in se.Suggestions)
            {
                alist.Add(suggestion);
                addedSuggestion = true;
            }
            return addedSuggestion;
        }

        /// <summary>Verify 2 strings are equal.</summary>
        private void VerifyString(string expect, string actual, string msg)
        {
            Verifier.Verify(expect == actual, msg + "\nExpect[" + expect + "]\nActual[" + actual + "]");
        }

        /// <summary>Verify 2 integers are equal.</summary>
        private void VerifyInt(int expect, int actual, string msg)
        {
            Verifier.Verify(expect == actual, msg + "\nExpect[" + expect + "]\nActual[" + actual + "]");
        }

        #endregion private helper methods

        #region private field

        private UIElementWrapper _wrapper;
        private RichTextBox _rtb;
        private Run _run;
        private TextBox _tb;
        private SpellingError _spellingError;

        /// <summary>EditableType under test</summary>
        private TextEditableType _editableType = null;

        private string _content = "This isss a asdfsaf test.";
        private string _correctContent = "This " + s_correctString + " a asdfsaf test.";
        private static string s_correctString = "good";
        private string _comment = "";
        private TextPointer _tp;

        private int _charIndexInfront1stError = 5;
        private int _charIndexInside1stError = 6;
        private int _charIndexAfter1stError = 9;
        private int _charIndexInfront2ntError = 12;
        private int _charIndexInside2ntError = 16;
        private int _charIndexAfter2ntError = 19;
        private int _firstErrorLength = 4;

        #endregion private field
    }

    /// <summary>
    /// Test whether word breakers are taken into account for spellchecking for plain and rich text.
    /// </summary>
    [Test(0, "Speller", "SpellerWordBreakingTest", MethodParameters = "/TestCaseType=SpellerWordBreakingTest", Keywords = "Localization_Suite", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"), TestTactics("534"), TestBugs("668,385"), TestWorkItem(""), TestLastUpdatedOn("Jan 25,2007")]
    public class SpellerWordBreakingTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _textBoxBase = (TextBoxBase)_editableType.CreateInstance();
            _textBoxBase.Language = System.Windows.Markup.XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("english"));
            _textBoxBase.SpellCheck.IsEnabled = true;
            _wrapper = new UIElementWrapper(_textBoxBase);

            TestElement = _textBoxBase;

            QueueDelegate(TestSpellCheck);
        }

        private void TestSpellCheck()
        {
            string content;
            foreach (string breaker in _contentWithWordBreakerText)
            {
                _wrapper.Text = string.Empty;

                if (_editableType.SupportsParagraphs)
                {
                    content = breaker;
                    //content = word1 + "&" + breaker + ";" + word2 + lastWord;
                    Log("Testing for content: [" + content + "]");

                    Run runElement = new Run(content);
                    ((RichTextBox)_textBoxBase).Document.Blocks.Add(new Paragraph(runElement));
                    //wrapper.XamlText = content;
                    ((RichTextBox)_textBoxBase).CaretPosition = ((RichTextBox)_textBoxBase).Document.ContentEnd;
                    VerifyRTBSpelling((RichTextBox)_textBoxBase, true);
                }
                else
                {
                    content = breaker;
                    //content = word1 + breaker + word2 + lastWord;
                    Log("Testing for content: [" + content + "]");

                    _wrapper.Text = content;
                    ((TextBox)_textBoxBase).CaretIndex = content.Length;
                    VerifyTBSpelling((TextBox)_textBoxBase, true);
                }
            }

            if (_editableType.SupportsParagraphs)
            {
                foreach (string xamlContent in _contentWithWordBreakingXamlTags)
                {
                    _wrapper.Text = string.Empty;
                    Log("Testing for xaml content: [" + xamlContent + "]");

                    _wrapper.XamlText = xamlContent;
                    ((RichTextBox)_textBoxBase).CaretPosition = ((RichTextBox)_textBoxBase).Document.ContentEnd;
                    VerifyRTBSpelling((RichTextBox)_textBoxBase, true);
                }

                foreach (string xamlContent in _contentWithNonWordBreakingXamlTags)
                {
                    _wrapper.Text = string.Empty;
                    Log("Testing for xaml content: [" + xamlContent + "]");

                    _wrapper.XamlText = xamlContent;
                    ((RichTextBox)_textBoxBase).CaretPosition = ((RichTextBox)_textBoxBase).Document.ContentEnd;
                    VerifyRTBSpelling((RichTextBox)_textBoxBase, false);
                }
            }

            QueueDelegate(NextCombination);
        }

        private void VerifyTBSpelling(TextBox tb, bool expectNoSpellingErrors)
        {
            int nextErrorCharIndex;
            bool result;
            string errorMessage = "";

            nextErrorCharIndex = tb.GetNextSpellingErrorCharacterIndex(0, LogicalDirection.Forward);
            if (expectNoSpellingErrors)
            {
                result = (nextErrorCharIndex == -1) ? true : false;
                if (!result)
                {
                    errorMessage = "Spelling errors found when not expected";
                }
            }
            else
            {
                result = (nextErrorCharIndex != -1) ? true : false;
                if (!result)
                {
                    errorMessage = "Spelling errors not found when expected";
                }
            }

            Verifier.Verify(result, errorMessage, false);
        }

        private void VerifyRTBSpelling(RichTextBox rtb, bool expectNoSpellingErrors)
        {
            TextPointer nextErrorTextPosition;
            bool result;
            string errorMessage = "";

            nextErrorTextPosition = rtb.GetNextSpellingErrorPosition(rtb.Document.ContentStart,
                LogicalDirection.Forward);
            if (expectNoSpellingErrors)
            {
                result = (nextErrorTextPosition == null) ? true : false;
                if (!result)
                {
                    errorMessage = "Spelling errors found when not expected";
                }
            }
            else
            {
                result = (nextErrorTextPosition != null) ? true : false;
                if (!result)
                {
                    errorMessage = "Spelling errors not found when expected";
                }
            }

            Verifier.Verify(result, errorMessage, false);
        }

        #endregion Main flow.

        #region Private fields

        private TextEditableType _editableType = null;
        private TextBoxBase _textBoxBase;
        private UIElementWrapper _wrapper;

        private const string word1 = "cat";
        private const string word2 = "dog";
        private const string lastWord = " test";
        private string[] _contentWithWordBreakerText = new string[] {
            word1 + " " + word2 + lastWord,
            word1 + "\t" + word2 + lastWord,
            word1 + "\r" + word2 + lastWord,
            word1 + "\n" + word2 + lastWord,
            word1 + "\r\n" + word2 + lastWord,
            word1 + "\x000b" /*VT*/ + word2 + lastWord,
            word1 + "\x000d" /*CR*/ + word2 + lastWord,
        };

        private string[] _contentWithNonWordBreakingXamlTags = new string[] {
            "<Run>" + word1 + "</Run>" + "<Run>" + word2 + "</Run>" + lastWord,
            "<Run>" + word1 + "</Run>" + "<Bold>" + word2 + "</Bold>" + lastWord,
            "<Bold>" + word1 + "</Bold>" + "<Italic>" + word2 + "</Italic>" + lastWord,
            "<Run>" + word1 + "</Run>" + "<Run FontWeight='Bold'>" + word2 + "</Run>" + lastWord,
        };

        private string[] _contentWithWordBreakingXamlTags = new string[] {
            word1 + "<LineBreak/>" + word2 + lastWord,
            "<Paragraph>" + word1 + "<Button>Button</Button>" + word2 + lastWord + "</Paragraph>",
            "<Paragraph>" + word1 + "<Floater><Paragraph>Figure</Paragraph></Floater>" + word2 + lastWord + "</Paragraph>",
            "<Paragraph>" + word1 + "</Paragraph>" + "<Paragraph>" + word2 + lastWord + "</Paragraph>",
        };

        #endregion Private fields
    }

    /// <summary>
    /// Regression_Bug809 - First misspell word at the  begining of document doesn't have squiggle underline highlighted after loading xaml
    /// Regression_Bug554 - Test spell check with text selected.
    /// - If selection start is 256 or more symbols away from selection end (this includes TextElement edges), ignore spelling errors.
    /// - If selection.Text contains \n, \r, \v, \f, \u0085 /*NEL*/, \u2028/*LS*/, \u2029/*PS*/, ignore spelling errors.
    /// </summary>
    [Test(0, "Speller", "SpellerWordSelectionTest", MethodParameters = "/TestCaseType=SpellerWordSelectionTest", Timeout = 240, Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"), TestTactics("666"), TestBugs("809,554"), TestWorkItem("131")]
    public class SpellerWordSelectionTest : ManagedCombinatorialTestCase
    {
        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            return result = result && !_editableType.IsPassword;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_wrapper.Element;
            ((TextBoxBase)_wrapper.Element).FontSize = 20;

            CultureInfo currentCultureInfo;
            currentCultureInfo = CultureInfo.CurrentCulture;
            XmlLanguage language = System.Windows.Markup.XmlLanguage.GetLanguage(currentCultureInfo.IetfLanguageTag);
            SetText(s_content + _stringContent, language);

            if (SpellerTest.IsCurrentCultureSupported)
                QueueDelegate(TestBugRegression_Bug809);
            else
                Logger.Current.ReportResult(true, "Speller is not supported for this culture. No test was run.");
        }

        private void SetText(string text, XmlLanguage language)
        {
            UIElement _element = _wrapper.Element;
            if (_element is TextBox)
            {
                _wrapper.Text = text;
            }
            else
            {
                ((RichTextBox)_element).Document.Blocks.Clear();
                Run r = new Run(text);
                r.Language = language;
                Paragraph p = new Paragraph(r);
                ((RichTextBox)_element).Document.Blocks.Add(p);
            }
        }

        /// <summary>Start the test case with testing for bug Regression_Bug809</summary>
        private void TestBugRegression_Bug809()
        {
            // Taking snap shot without speller enabled
            _noSpellerSnapShot = BitmapCapture.CreateBitmapFromElement(_wrapper.Element);

            // Enable speller
            ((TextBoxBase)_wrapper.Element).SpellCheck.IsEnabled = true;

            QueueDelegate(TakeNextSnapShot);
        }

        /// <summary>Take snap shot with speller enabled</summary>
        private void TakeNextSnapShot()
        {
            // Taking snap shot witht speller enabled
            _spellerSnapShot = BitmapCapture.CreateBitmapFromElement(_wrapper.Element);

            ComparisonCriteria _criteria = new ComparisonCriteria();
            _criteria.MaxColorDistance = 0.005f;
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_noSpellerSnapShot, _spellerSnapShot, _criteria, false))
            {
                Log("Logging images: BeforeSpeller, AfterSpeller");
                Logger.Current.LogImage(_noSpellerSnapShot, "BeforeSpeller");
                Logger.Current.LogImage(_spellerSnapShot, "AfterSpeller");
                Logger.Current.ReportResult(false, "Squiggle underline before and after compare failed.", true);
            }

            QueueDelegate(TestRegression_Bug554); // Testing for Regression_Bug554
        }

        /// <summary>
        /// Testing for Regression_Bug554
        /// - If selection.Text contains \n, \r, \v, \f, \u0085 /*NEL*/, \u2028/*LS*/, \u2029/*PS*/, ignore spelling errors.
        /// - If selection start is 256 or more symbols away from selection end (this includes TextElement edges), ignore spelling errors.
        /// </summary>
        private void TestRegression_Bug554()
        {
            Clipboard.Clear(); // Clear clipboard
            _spellerSuggestion = false;

            // There should be speller suggestion for selection contains 255 or less
            // When do arrow down and enter in ContextMenu, it should select the suggetive word
            // Therefor clipboard is empty.
            if (_wrapper.Text == s_content + s_content + s_content + s_content + s_content)
                _spellerSuggestion = true;

            _wrapper.Element.Focus();
            ((TextBoxBase)_wrapper.Element).SelectAll();

            KeyboardInput.TypeString("+{F10}");
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 1), new SimpleHandler(CheckContextMenu), null);
        }

        private void CheckContextMenu()
        {
            KeyboardInput.TypeString("{DOWN}{ENTER}");
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 1), new SimpleHandler(VerifyResult), null);
        }

        private void VerifyResult()
        {
            if (_spellerSuggestion)
                Verifier.Verify(Clipboard.GetText() == string.Empty,
                    "Spelling errors is available for selection contains 255 chars.\nExpect[]\nActual[" + Clipboard.GetText() + "]", true);
            else
                Verifier.Verify(_wrapper.Text == string.Empty,
                    "Spelling error is ignored for this selection.\nExpect[]\nActual[" + _wrapper.Text + "]", true);

            QueueDelegate(NextCombination);
        }

        #region private field
        private UIElementWrapper _wrapper;
        private TextEditableType _editableType = null;
        private string _stringContent = string.Empty;

        private static string s_content = "Misspellword at beginning of content. Character 255";
        /// <summary>String content to be tested</summary>
        public static string[] content = new string[] { "\n", "\r", "\r\n", "\v", "\f", "\u0085", "\u2028", "\u2029",
                s_content+s_content+s_content+s_content+"6" /*256 chars*/, s_content+s_content+s_content+s_content+"67" /*257 chars*/,
                s_content+s_content+s_content+s_content/*255*/};

        private Bitmap _noSpellerSnapShot;
        private Bitmap _spellerSnapShot;
        private bool _spellerSuggestion;
        #endregion private field
    }

    /// <summary>
    /// Test that the speller degrades gracefully for unsupported languages
    /// </summary>
    [Test(2, "Speller", "SpellerFailureForNonSupportedLang", MethodParameters = "/TestCaseType=SpellerFailureForNonSupportedLang", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+", Keywords = "Localization_Suite")]
    [TestOwner("Microsoft"), TestTactics("664"), TestWorkItem("130"), TestLastUpdatedOn("Jan 25,2007")]
    public class SpellerFailureForNonSupportedLang : ManagedCombinatorialTestCase
    {
        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            return result = result && !_editableType.IsPassword;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _cultureInfo = System.Globalization.CultureInfo.CurrentCulture;

            Clipboard.Clear();
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            _element = _wrapper.Element;
            TestElement = (FrameworkElement)_wrapper.Element;
            ((TextBoxBase)_wrapper.Element).FontSize = 20;
            ((TextBoxBase)_wrapper.Element).SpellCheck.IsEnabled = true;
            ((TextBoxBase)_element).Language = System.Windows.Markup.XmlLanguage.GetLanguage(_cultureInfo.IetfLanguageTag);

            if (_element is TextBox)
            {
                _wrapper.Text = _incorrectSentence;
            }
            else
            {
                ((RichTextBox)_element).Document.Blocks.Clear();
                Run r = new Run(_incorrectSentence);
                r.Language = ((TextBoxBase)_element).Language;
                Paragraph p = new Paragraph(r);
                ((RichTextBox)_element).Document.Blocks.Add(p);
            }
            QueueDelegate(DoFocus);
        }

        /// <summary>ocuses and opened context menu.</summary>
        private void DoFocus()
        {
            _element.Focus();
            KeyboardInput.TypeString("+{F10}");
            QueueDelegate(SelectChoice);
        }

        /// <summary>Selects an option in the context menu.</summary>
        private void SelectChoice()
        {
            KeyboardInput.TypeString("{DOWN}{enter}{ESC}");
            if (IsCultureSupported(_cultureInfo))
            {
                QueueDelegate(VerifySpellerWorks);
            }
            else
            {
                QueueDelegate(VerifySpellerDoesntWork);
            }
        }

        /// <summary>Verifies that speller works correctly.</summary>
        private void VerifySpellerWorks()
        {
            string _newLine = (_element is RichTextBox) ? "\r\n" : "";
            Verifier.Verify(_wrapper.Text != (_incorrectSentence + _newLine), "Strings should be different because speller works Initial [" +
                _incorrectSentence + "] Actual[" + _wrapper.Text + "]", true);
            NextCombination();
        }

        /// <summary>Verifies that speller doesnt work.</summary>
        private void VerifySpellerDoesntWork()
        {
            string _newLine = (_element is RichTextBox) ? "\r\n" : "";
            Verifier.Verify(_wrapper.Text == (_incorrectSentence + _newLine), "Strings should be same because speller doesnt work Initial [" +
                _incorrectSentence + _newLine + "] Actual[" + _wrapper.Text + "]", true);
            NextCombination();
        }

        /// <summary>
        /// Returns true if current culture is supported by speller
        /// </summary>
        public bool IsCultureSupported(CultureInfo _cultureInfo)
        {
            Logger.Current.Log("CultureInfo.TwoLetterISOLanguageName: " + _cultureInfo.TwoLetterISOLanguageName.ToString());

            if (SpellerTest.IsSpellerAvailableForLanguage(XmlLanguage.GetLanguage(_cultureInfo.IetfLanguageTag)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region data.

        private UIElement _element = null;
        private UIElementWrapper _wrapper = null;
        private TextEditableType _editableType = null;

        private string _incorrectSentence = "zydp adasda asdad";
        // private int _count = 0;
        CultureInfo _cultureInfo = null;

        #endregion data.
    }

    /// <summary>
    /// Testing spelling reform in German, French, and other
    /// </summary>
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "SpellingReformTest1", MethodParameters = "/TestCaseType:SpellingReformTest /XbapName=EditingTestDeploy", Versions = "3.0SP1,3.0SP2,4.0,4.0Client")]
    [Test(0, "Speller", "SpellingReformTest", MethodParameters = "/TestCaseType=SpellingReformTest", Keywords = "Localization_Suite", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"), TestTactics("663"), TestBugs("805,806"), TestWorkItem("129"), TestLastUpdatedOn("Jan 25,2007")]
    public class SpellingReformTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            if (SpellerTest.IsSpellerAvailableForLanguage(XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german"))))
            {
                _runForGerman = true;
            }
            if (SpellerTest.IsSpellerAvailableForLanguage(XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("french"))))
            {
                _runForFrench = true;
            }

            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_wrapper.Element;
            _element = TestElement;
            _wordCount = 0;
            _wrapper.Element.SetValue(SpellCheck.IsEnabledProperty, true);

            if (_runForGerman)
            {
                QueueDelegate(StartTestInGerman);
            }
            else if (_runForFrench)
            {
                QueueDelegate(StartTestInFrench);
            }
            else
            {
                QueueDelegate(StartTestInEnglish);
            }
        }

        #region German

        /// <summary>Start test in German</summary>
        private void StartTestInGerman()
        {
            Log("*********************GERMAN SETTING ********************");
            Log("Setting spelling reform values on the control ...");
            ((TextBoxBase)_wrapper.Element).Language = System.Windows.Markup.XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german"));
            SettingSpellingReformPropertyInGerman();
            _wordCount = GermanMisspellWord.Length;

            QueueDelegate(TestGermanPostPreReform);
        }

        private void TestGermanPostPreReform()
        {
            _wordCount--;
            if (_wordCount < 0)
            {
                if (_runForFrench)
                {
                    QueueDelegate(StartTestInFrench);
                }
                else
                {
                    QueueDelegate(StartTestInEnglish);
                }
            }
            else
            {
                _misspellWord = GermanMisspellWord[_wordCount];
                SetText(_misspellWord, ((TextBoxBase)_element).Language);
                QueueDelegate(VerifyResultInGerman);
            }
        }

        private void SetText(string text, XmlLanguage language)
        {
            if (_element is TextBox)
            {
                _wrapper.Text = text;
            }
            else
            {
                ((RichTextBox)_element).Document.Blocks.Clear();
                Run r = new Run(text);
                r.Language = language;
                Paragraph p = new Paragraph(r);
                ((RichTextBox)_element).Document.Blocks.Add(p);
            }
        }

        /// <summary>Setting SpellingReform properties for German</summary>
        private void SettingSpellingReformPropertyInGerman()
        {
            Log("In SettingSpellingReformPropertyInGerman() ...");

            if (_spellingReform == SpellingReform.PreAndPostreform)
            {
                ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = _spellingReform;
            }
            else
            {
                ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = _spellingReform;
                GettingSpellCheckSpellingReformInGerman();
                ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = (_spellingReform == SpellingReform.Prereform) ? SpellingReform.Postreform : SpellingReform.Prereform; // Re-set to default German value

                SpellCheck.SetSpellingReform((TextBoxBase)_wrapper.Element, _spellingReform);
                GettingSpellCheckSpellingReformInGerman();
                ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = (_spellingReform == SpellingReform.Prereform) ? SpellingReform.Postreform : SpellingReform.Prereform; // Re-set to default German value

                _wrapper.Element.SetValue(SpellCheck.SpellingReformProperty, _spellingReform);
                GettingSpellCheckSpellingReformInGerman();
            }
        }

        /// <summary>Getting SpellCheck.SpellingReform for German</summary>
        private void GettingSpellCheckSpellingReformInGerman()
        {
            Verifier.Verify(((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform == _spellingReform, "Verifying SpellingReform correctly set on control" +
                " Expect[" + _spellingReform + "] Actual[" + ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform + "]", true);
        }

        /// <summary>Verify spelling in German</summary>
        private void VerifyResultInGerman()
        {
            Log("------------------------------------------------------------");
            Log("Verifying that spelling reform works for [" + _misspellWord + "]");
            _se = GetSpellingError();
            switch (_misspellWord)
            {
                // should always flag in German (Germany) Pre-Reform (=should never flag in German (Germany) Post-Reform)
                case "Missfallens�u�erungen ":
                    Log("should always flag in Pre-Reform NOT in  Post-Reform/PreAndPostreform)");
                    if (_spellingReform == SpellingReform.Prereform)
                        VerifyFlag(_se);
                    else
                        Verifier.Verify(_se == null, "This word only flag in Prereform in German.", false);
                    break;

                // should always flag in German (Germany) Post-Reform (=should never flag in German (Germany) Pre-Reform)
                case "Bewu�tseinszustandes ":
                    Log("Should always flag in PreAndPostreform/Postreform NOT in PreReform \r\n");
                    if (_spellingReform == SpellingReform.PreAndPostreform || _spellingReform == SpellingReform.Postreform)
                        VerifyFlag(_se);
                    else
                        Verifier.Verify(_se == null, "This word Should NOT flag in PreReform.", false);
                    break;

                // should always flag in ANY German reform setting
                case "Aufrichigkeit ":
                    Log("Should always flag in ANY German reform setting ");
                    VerifyFlag(_se);
                    break;

                // should never flag in ANY German reform setting
                case "Aufrichtigkeit ":
                    Verifier.Verify(_se == null, "This word is never flagged in ANY German reform setting.", true);
                    break;

                // should always flag in German (Germany) Post-Reform (=should never flag in German (Germany) Pre-Reform)
                //last 4 words in the list
                default:
                    Log("Should always flag in PreAndPostreform/Postreform NOT in PreReform \r\n");
                    if (_spellingReform == SpellingReform.PreAndPostreform || _spellingReform == SpellingReform.Postreform)
                        VerifyFlag(_se);
                    else
                        Verifier.Verify(_se == null, "This word Should NOT flag in PreReform.", false);
                    break;
            }
            QueueDelegate(TestGermanPostPreReform);
        }

        #endregion German

        #region French and other language

        /// <summary>Start test in French </summary>
        private void StartTestInFrench()
        {
            Log("*********************FRENCH SETTING ********************");
            Log("Setting spelling reform values on the control ...");
            ((TextBoxBase)_wrapper.Element).Language = System.Windows.Markup.XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("FRENCH"));
            SettingSpellingReformPropertyInOtherLanguage();
            _wordCount = FrenchMisspellWord.Length;
            QueueDelegate(TestFrenchPostPreReform);
        }

        private void TestFrenchPostPreReform()
        {
            _wordCount--;
            if (_wordCount < 0)
            {
                QueueDelegate(StartTestInEnglish);
            }
            else
            {
                _misspellWord = FrenchMisspellWord[_wordCount];
                SetText(_misspellWord, ((TextBoxBase)_element).Language);
                VerifyResultInFrenchAndOtherLanguage();
                QueueDelegate(TestFrenchPostPreReform);
            }
        }

        /// <summary>Setting SpellingReform properties for French and other langualge</summary>
        private void SettingSpellingReformPropertyInOtherLanguage()
        {
            ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = _spellingReform;
            GettingSpellCheckSpellingReformInOtherLanguage();
            ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = SpellingReform.PreAndPostreform; // Re-set to default

            SpellCheck.SetSpellingReform((TextBoxBase)_wrapper.Element, _spellingReform);
            GettingSpellCheckSpellingReformInOtherLanguage();
            ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform = SpellingReform.PreAndPostreform; // Re-set to default

            _wrapper.Element.SetValue(SpellCheck.SpellingReformProperty, _spellingReform);
            GettingSpellCheckSpellingReformInOtherLanguage();
        }

        /// <summary>Getting SpellCheck.SpellingReform for French and other langualge</summary>
        private void GettingSpellCheckSpellingReformInOtherLanguage()
        {
            Verifier.Verify(((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform == _spellingReform, "testing spelling reform on control" +
                "\nExpect[" + _spellingReform + "]\nActual[" + ((TextBoxBase)_wrapper.Element).SpellCheck.SpellingReform + "]", false);
        }

        /// <summary>Verify spelling in French and other language</summary>
        private void VerifyResultInFrenchAndOtherLanguage()
        {
            Log("Verifying spellingReform works for Word [" + _misspellWord + "]");
            _se = GetSpellingError();
            switch (_misspellWord)
            {
                // should always flag in French (France) PRE (=should never flag in POST)
                case "r�glemente ":
                    Log("should always flag in PRE NOT in POST/PreAndPost");
                    if (_spellingReform == SpellingReform.Prereform)
                        VerifyFlag(_se);
                    else
                        Verifier.Verify(_se == null, "This word only flag in Prereform in French.");
                    break;

                // should always flag in French (France) POST (=should never flag in PRE)
                case "toquade ":
                    Log("should always flag in POST/PreAndPost NOT in PRE");
                    if (_spellingReform == SpellingReform.Postreform)
                        VerifyFlag(_se);
                    else
                        Verifier.Verify(_se == null, "This word only flag in Postreform in French.");
                    break;

                // should always flag in French (France) BOTH
                case "all�guera ":
                    Log("should always flag in French (France) BOTH");
                    VerifyFlag(_se);
                    break;

                // should never flag in French (France) ANY (PRE/POST/BOTH)
                case "sommes ":
                    Log("should never flag");
                    Verifier.Verify(_se == null, "This word never flag in any reform in French.");
                    break;

                // should always flag when � Accented Caps in French � = ON and � Ignore Words in Uppercase � is OFF
                case "araCHNEEN ":
                    Log("should always flag in French (France) BOTH");
                    VerifyFlag(_se);
                    break;

                // should never flag when � Accented Caps in French � = ON and � Ignore Words in Uppercase � is OFF
                case "litre ":
                    Log("should never flag");
                    Verifier.Verify(_se == null, "This word never flag in any reform in French.");
                    break;

                // should always flag in English irrespective of the SpellingReform setting.
                case "Accessibilitys":
                    Log("should flag always");
                    VerifyFlag(_se);
                    break;
            }
        }

        /// <summary>Start test in English </summary>
        private void StartTestInEnglish()
        {
            Log("********************* ENGLISH SETTING ********************");
            Log("Setting spelling reform values on the control ...");
            ((TextBoxBase)_wrapper.Element).Language = System.Windows.Markup.XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("ENGLISH"));
            SettingSpellingReformPropertyInOtherLanguage();
            _wordCount = EnglishMisspellWord.Length;
            QueueDelegate(TestEnglishPostPreReform);
        }

        private void TestEnglishPostPreReform()
        {
            _wordCount--;
            if (_wordCount < 0)
            {
                QueueDelegate(NextCombination);
            }
            else
            {
                _misspellWord = EnglishMisspellWord[_wordCount];
                SetText(_misspellWord, ((TextBoxBase)_element).Language);
                VerifyResultInFrenchAndOtherLanguage();
                QueueDelegate(TestEnglishPostPreReform);
            }
        }

        #endregion French and other language

        #region Helper method

        /// <summary>Return SpellingError</summary>
        private SpellingError GetSpellingError()
        {
            if (_wrapper.Element is RichTextBox)
            {
                TextPointer tp = ((RichTextBox)_wrapper.Element).Document.ContentStart;
                tp = tp.GetPositionAtOffset(_charIndex); // Move pointer into misspell word

                return ((RichTextBox)_wrapper.Element).GetSpellingError(tp); // Get SpellingError
            }
            else
                return ((TextBox)_wrapper.Element).GetSpellingError(_charIndex); // Get SpellingError
        }

        /// <summary>Return the list of suggestion words</summary>
        private ArrayList SuggestionList(SpellingError se)
        {
            ArrayList alist = new ArrayList();
            foreach (string suggestion in se.Suggestions)
            {
                alist.Add(suggestion);
            }
            return alist;
        }

        /// <summary>Verify word falg</summary>
        private void VerifyFlag(SpellingError se)
        {
            _correctString = SuggestionList(se)[0].ToString();
            se.Correct(_correctString);
            _correctString += (_wrapper.Element is RichTextBox) ? " \r\n" : " ";
            Verifier.Verify(_wrapper.Text == _correctString, "\nExpect[" + _correctString + "]\nActual[" + _wrapper.Text + "]");
        }

        #endregion Helper method

        #region private fields

        private UIElementWrapper _wrapper;
        private TextEditableType _editableType = null;
        private UIElement _element = null;
        private SpellingReform _spellingReform = 0; // SpellingReform enum value
        private string _misspellWord = string.Empty; // Holds misspell words in German, French and other
        private int _charIndex = 4; // Char index in the misspell word
        private SpellingError _se = null;
        string _correctString = string.Empty;
        int _wordCount = 0;
        bool _runForGerman = false;
        bool _runForFrench = false;
        /// <summary>Holds misspell word for German </summary>
        public static string[] GermanMisspellWord =
            new string[] { "Missfallens�u�erungen ", "Bewu�tseinszustandes ", "Aufrichigkeit ", "Aufrichtigkeit ", "Bew��rungen ", "Ble�b�cke ", "Bl��h�hner ", "Bi�wunde " };
        /// <summary>Holds misspell word for  French</summary>
        public static string[] FrenchMisspellWord =
            new string[] { "litre ", "araCHNEEN ", "sommes ", "all�guera ", "toquade ", "r�glemente " };
        /// <summary>Holds misspell word for english</summary>
        public static string[] EnglishMisspellWord =
            new string[] { "Accessibilitys " };

        #endregion private fields
    }

    /// <summary>Tests the TextBox/RichTextBox speller.</summary>
    [Test(0, "Speller", "SpellerTestMultipleLangOnRTB", MethodParameters = "/TestCaseType=SpellerTestMultipleLangOnRTB", Keywords = "Localization_Suite", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"), TestTitle("SpellerTestMultipleLangOnRTB"), TestTactics("662"), TestLastUpdatedOn("May 18,2006")]
    public class SpellerTestMultipleLangOnRTB : CustomTestCase
    {
        public override void RunTestCase()
        {
            StackPanel sp = new StackPanel();
            _rtb = new RichTextBox();
            sp.Children.Add(_rtb);
            MainWindow.Content = sp;
            SetData();
            _rtb.SpellCheck.IsEnabled = true;
            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _rtb.Focus();
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(GetSpeller);
        }

        private void GetSpeller()
        {
            for (int i = 0; i < (_xmlLanguages.Length - _numberOfNonSupportedLanguages); i++)
            {
                if (SpellerTest.IsSpellerAvailableForLanguage(_xmlLanguages[i]))
                {
                    TextPointer tp = _runs[i].ContentStart.GetPositionAtOffset(0, LogicalDirection.Forward);
                    TextRange tr = _rtb.GetSpellingErrorRange(tp);
                    Log("================================");
                    string langPropertyValueOnRun = tr.GetPropertyValue(Run.LanguageProperty).ToString();
                    Verifier.Verify(langPropertyValueOnRun == _xmlLanguages[i].IetfLanguageTag, "Verifying the lang property on the range with error: " +
                        "Expected [" + _xmlLanguages[i].IetfLanguageTag + "] Actual [" + langPropertyValueOnRun + "]", true);
                    Log("Text [" + tr.Text + "]");

                    SpellingError _sperr = _rtb.GetSpellingError(tp);
                    Verifier.Verify(SuggestionCount(_sperr) > 0, "Number of suggestions >0 Actual [" + SuggestionCount(_sperr).ToString() + "]", true);
                }
            }

            for (int j = _xmlLanguages.Length - _numberOfNonSupportedLanguages; j < _xmlLanguages.Length; j++)
            {
                TextPointer tp = _runs[j].ContentStart.GetPositionAtOffset(0);
                TextRange tr1 = _rtb.GetSpellingErrorRange(tp);
                SpellingError _sperr = _rtb.GetSpellingError(tp);
                Log("================================");
                Log("Language [" + _runs[j].GetValue(Run.LanguageProperty).ToString() + "] Text [" + _runs[j].Text + "]");
                Verifier.Verify(_sperr == null, "No spelling error - should return null", true);
                Verifier.Verify(tr1 == null, "No spelling error - spelling error range should return null", true);
            }

            Logger.Current.ReportSuccess();
        }

        #region Helper.

        private void SetData()
        {
            string[] data = {"abcd", "efgh", "ijkl", "mnop", "qrst"};

            _rtb.Document.Blocks.Clear();
            Paragraph p = new Paragraph();
            _runs = new Run[_xmlLanguages.Length];
            for (int i = 0; i < _xmlLanguages.Length; i++)
            {
                _runs[i] = new Run(data[i] + " ");
                _runs[i].Language = _xmlLanguages[i];
                p.Inlines.Add(_runs[i]);
            }
            _rtb.Document.Blocks.Add(p);
        }

        private int SuggestionCount(SpellingError se)
        {
            int _count = 0;
            foreach (string suggestion in se.Suggestions)
            {
                _count++;
            }
            return _count;
        }

        #endregion Helper.

        #region data.

        private RichTextBox _rtb = null;
        private Run[] _runs = null;
        private XmlLanguage[] _xmlLanguages = { XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("english")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("french")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("spanish")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("arabic"))};

        int _numberOfNonSupportedLanguages = 1;

        #endregion data.
    }

    /// <summary>Tests the default TextBox speller works on switiching input language</summary>
    [Test(0, "Speller", "SpellerTestOnTB", MethodParameters = "/TestCaseType=SpellerTestOnTB", Keywords = "Localization_Suite", Versions = "3.0SP1,3.0SP2,4.0,4.0Client,4.5+")]
    [TestOwner("Microsoft"),  TestTitle("SpellerTestOnTB"), TestTactics("661"), TestLastUpdatedOn("Jan 25,2007")]
    public class SpellerTestOnTB : CustomTestCase
    {
        public override void RunTestCase()
        {
            _sp = new StackPanel();
            MainWindow.Content = _sp;
            _initialLocale = KeyboardInput.GetActiveInputLocaleString();
            QueueDelegate(TestSpellerBasedOnActiveInputLocaleOnTextBoxCreation);
        }

        // This method is to test the speller language changes by the active input locale when TextBox is created
        private void TestSpellerBasedOnActiveInputLocaleOnTextBoxCreation()
        {
            if (_currLangIndex < _langTags.Length)
            {
                if (SpellerTest.IsSpellerAvailableForLanguage(_xmlLanguages[_currLangIndex]))
                {
                    Log("+++Testing speller based on active input loacle on TextBox creation: " + _xmlLanguages[_currLangIndex].ToString());
                    KeyboardInput.SetActiveInputLocale(_langTags[_currLangIndex]);
                    QueueDelegate(CreateTextBox);
                }
                else
                {
                    Log("---Skip the language because speller is not supported in this configuration: " + _xmlLanguages[_currLangIndex].ToString());
                    _currLangIndex++;
                    QueueDelegate(TestSpellerBasedOnActiveInputLocaleOnTextBoxCreation);
                }
            }
            else
            {
                _currLangIndex = 0;
                if (KeyboardInput.GetActiveInputLocaleString() != _initialLocale)
                {
                    KeyboardInput.SetActiveInputLocale(_initialLocale);
                }
                QueueDelegate(TestSpellerBasedOnLanguageProperty);
            }
        }

        // This method is to test the speller language changes by the Language property on the TextBox
        private void TestSpellerBasedOnLanguageProperty()
        {
            if (_currLangIndex < _langTags.Length)
            {
                if (SpellerTest.IsSpellerAvailableForLanguage(_xmlLanguages[_currLangIndex]))
                {
                    Log("+++Testing speller based on language property: " + _xmlLanguages[_currLangIndex].ToString());
                    _tb.Language = _xmlLanguages[_currLangIndex];
                    QueueDelegate(VerifySpellerSwitchesOnLanguageProperty);
                }
                else
                {
                    Log("---Skip the language because speller is not supported in this configuration: " + _xmlLanguages[_currLangIndex].ToString());
                    _currLangIndex++;
                    QueueDelegate(TestSpellerBasedOnLanguageProperty);
                }
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        private void CreateTextBox()
        {
            _sp.Children.Clear();
            _tb = new TextBox();
            _tb.Text = "wht";
            _tb.SpellCheck.IsEnabled = true;
            _sp.Children.Add(_tb);
            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _tb.Focus();
            QueueDelegate(VerifySpellerSwitchesOnActiveInputLocaleOnTextBoxCreation);
        }

        private void VerifySpellerSwitchesOnActiveInputLocaleOnTextBoxCreation()
        {
            VerifyFirstSpellingChoiceMatches();
            QueueDelegate(TestSpellerBasedOnActiveInputLocaleOnTextBoxCreation);
        }

        private void VerifySpellerSwitchesOnLanguageProperty()
        {
            VerifyFirstSpellingChoiceMatches();
            QueueDelegate(TestSpellerBasedOnLanguageProperty);
        }

        #region Helper.

        private string FirstSuggestion(SpellingError se)
        {
            foreach (string suggestion in se.Suggestions)
            {
                return suggestion;
            }
            return "";
        }

        private void VerifyFirstSpellingChoiceMatches()
        {
            // Since the content is just one word, index of 1 should work here
            SpellingError spErr = _tb.GetSpellingError(1);
            string str = FirstSuggestion(spErr);
            Verifier.Verify(str.Contains(_spellerChoices[_currLangIndex]),
                "First speller choice Expected [" + _spellerChoices[_currLangIndex] + "] Actual [" + str + "]", true);
            _currLangIndex++;
        }

        #endregion Helper.

        #region Private fields

        private TextBox _tb = null;
        private StackPanel _sp = null;
        private string _initialLocale = "";
        private int _currLangIndex = 0;

        private string[] _langTags ={
            InputLocaleData.EnglishUS.Identifier,
            InputLocaleData.German.Identifier,
            InputLocaleData.French.Identifier,
            InputLocaleData.Spanish.Identifier
        };

        // Respective XmlLanguage instances for _langTags instances
        private XmlLanguage[] _xmlLanguages = {
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("english")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("french")),
            XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("spanish"))};

        private string[] _spellerChoices ={
            "what",
            "weht",
            "ht",
            "wat"
        };

        #endregion
    }

#region CompoundWordsTest
#if TESTBUILD_NET_ATLEAST_462 // CompoundWordsTest is needed only on .NET 4.6.2+

    /// <summary>
    /// On Win8.1 and above, when running on .NET 4.6.1+, WPF depends on ISpellChecker and
    /// Windows.Data.Text.WordsSegmenter for spell-checking. WordsSegmenter automatically decompounds
    /// words, which results in odd spell-checking results. For e.g., when encountering the word <i>Hausnummer</i>
    /// in German, WordsSegmenter automatically returns two tokens - <i>Haus</i> and <i>nummer</i>. Further
    /// spell-checking indicates that <i>nummer</i> has an error because this noun should be capitalized like <i>Nummer</i>.
    /// The end-result is an odd outcome where only a part of a word is indicated as a mis-spelling, like this:
    ///              Haus<u>nummer</u>
    ///
    /// In .NET 4.6.2, we took a bug fix that addresses this problem. The solution for this depends
    /// on the fact that using a neutral (as against a langauge specific) instance of WordsSegmenter will
    /// ensure that words would not be automatically decompounded during word-breaking.
    ///
    /// This test case exists to validate that decompounding of words does not happen and that WPF correctly
    /// spell-checks compound words. This test will also protect against any future regressions brought about
    /// by changes in the behavior of WordsSegmenter.
    /// </summary>
    [Test(priority:2, subArea:"Speller", Name = nameof(CompoundWordsTest), MethodParameters = "/TestCaseType=" + nameof(CompoundWordsTest), Keywords = "Localization_Suite", Versions = "4.6.2+")]
    public class CompoundWordsTest : CustomTestCase
    {
        private TextBox textBox;
        private StackPanel stackPanel;

        private static readonly XmlLanguage GermanXmlLanguage
            = XmlLanguage.GetLanguage(SpellerTest.IetfLanguageTags("german"));

        // We only support German for this test
        // If German keyboard is not enabled, we can not run this test
        private static readonly bool IsGermanSpellCheckingEnabled
            = SpellerTest.IsSpellerAvailableForLanguage(GermanXmlLanguage);

        public override void RunTestCase()
        {
            Log($"{nameof(CompoundWordsTest)}: Starting Test");

            // Intialize the StackPanel
            stackPanel = new StackPanel();
            MainWindow.Content = stackPanel;

            // Initialize the textbox and add it to the StackPanel
            QueueDelegate(() =>
            {
                if (IsGermanSpellCheckingEnabled)
                {
                    Log($"{nameof(CompoundWordsTest)}: German spell-checking is enabled. Proceeding with tests...");

                    textBox = new TextBox();
                    textBox.Text = "Hausnummer";
                    textBox.Language = GermanXmlLanguage;
                    textBox.SpellCheck.IsEnabled = true;

                    stackPanel.Children.Clear();
                    stackPanel.Children.Add(textBox);

                    Log($"{nameof(CompoundWordsTest)}: Created text-box with word 'Hausnummer'...");

                    // Set focus on the text box
                    QueueDelegate(()=>
                    {
                        textBox.Focus();

                        // Look for spelling erors and validate that no spelling errors are found
                        QueueDelegate(()=>
                        {
                            Log($"{nameof(CompoundWordsTest)}: Checking that there are no spelling errors reported...");

                            SpellingError spellingError = textBox.GetSpellingError(6); // index = 6 will indicate the letter 'm' in the test word 'Hausnummer'
                            Verifier.Verify(spellingError == null, "No spelling error should be found", true);

                            Logger.Current.ReportSuccess();
                        });
                    });
                }
                else // Test is not applicable
                {
                    Log($"{nameof(CompoundWordsTest)}: WARNING: SpellChecking in German is not available. Skipping this test...");
                    Logger.Current.ReportSuccess();
                }
            });
        }
    }

#endif // TESTBUILD_NET_ATLEAST_462
#endregion // CompoundWordsTest
}
