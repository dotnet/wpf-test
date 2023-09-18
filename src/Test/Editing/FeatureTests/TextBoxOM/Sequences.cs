// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Allows sequences of keystrokes and properties to be verified.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Sequences.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
        
    #endregion Namespaces.

    /// <summary>
    /// Data-based test case for keystroke sequences.
    /// </summary>
    /// <remarks>
    /// The key sequence is KeySend steps alternating with
    /// verification steps. KeySend steps are trimmed and
    /// ignored if they have only whitespace (use {SPACE} to
    /// add trailing or heading whitespace). Verification
    /// steps are not trimmed and specify a sequence
    /// of checks. Checks are separated by a semicolon and
    /// are in the format property-name=value. Use null
    /// to represent a null property value.
    /// <example><code>hello|Text=hello|{BS 2}|Text=hel;SelectionLength=0|</code></example>
    /// </remarks>
    [Test(0, "Editor", "TextBoxSequenceTest1", MethodParameters = "/TestCaseType=TextBoxSequenceTest /TestName=TextBoxType-ShiftBackspace")]
    [Test(0, "Editor", "TextBoxSequenceTest2", MethodParameters = "/TestCaseType=TextBoxSequenceTest /TestName=TextBoxAcceptsReturn-True")]
    [Test(3, "Editor", "TextBoxSequenceTest3", MethodParameters="/TestCaseType=TextBoxSequenceTest /TestName=TextBoxReproRegression_Bug897")]
    [TestOwner("Microsoft"), TestTactics("118,111"), TestArgument("Steps", "Sequence of input keystrokes and verifications.")]
    public class TextBoxSequenceTest: TextBoxTestCase
    {
        #region Private data.

        private string[] _steps;
        private int _currentStepIndex;

        private bool IsKeySendStep
        {
            get { return (_currentStepIndex % 2) == 0; }
        }

        private bool IsVerificationStep
        {
            get { return (_currentStepIndex % 2) == 1; }
        }

        private bool ShouldTrimStep(int index)
        {
            return (_currentStepIndex % 2) == 0;
        }

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            UIElement control = TestControl;

            MouseInput.MouseClick(control);
            TextBox textbox = control as TextBox;
            textbox.TextWrapping = TextWrapping.Wrap;
            if (control != null)
            {
                SetTextBoxProperties(textbox);
            }

            //textbox.Height = new Length(100, UnitType.Percent);

            ParseSteps();
            _currentStepIndex = 0;
            QueueDelegate(new SimpleHandler(SequenceStep));
        }

        private void SequenceStep()
        {
            if (_currentStepIndex > _steps.Length)
            {
                throw new Exception("Internal test case error." +
                    "Step index is greater than step array length.");
            }
            if (_currentStepIndex == _steps.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                string step = _steps[_currentStepIndex];
                Log("Step " + _currentStepIndex);
                if (IsVerificationStep)
                {
                    Log("Verifying [" + step + "]");
                    DoVerification(step);
                }
                if (IsKeySendStep)
                {
                    Log("Typing [" + step + "]");
                    KeyboardInput.TypeString(step);
                }
                _currentStepIndex++;
                QueueDelegate(new SimpleHandler(SequenceStep));
            }
        }

        /// <summary>Parses the keystroke and verification steps.</summary>
        private void ParseSteps()
        {
            string stepsValue = Settings.GetArgument("Steps");
            _steps = stepsValue.Split('|');
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Steps: ");
            sb.Append(stepsValue);
            sb.Append("\r\n");
            for (int i = 0; i < _steps.Length; i++)
            {
                string step = _steps[i].Trim();
                _steps[i] = step;
                sb.Append(step);
                sb.Append("\r\n");
            }
            Log(sb.ToString());
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Performs the specified verification on the control being tested.
        /// </summary>
        /// <param name='verification'>Verification specification.</param>
        private void DoVerification(string verification)
        {
            string[] checks = verification.Split(';');
            foreach(string check in checks)
            {
                if (check == null) continue;
                if (check.Trim().Length == 0) continue;
                string[] checkParts = check.Split('=');
                if (checkParts.Length == 0)
                {
                    throw new Exception("Invalid check " + check);
                }

                string propertyName = checkParts[0];
                string propertyValue = (checkParts.Length > 1)?
                    checkParts[1] : null;
                object actualValue = null;
                bool shouldCompare = true;

                if (!GetSpecialProperty(propertyName, propertyValue,
                    ref actualValue, ref shouldCompare))
                {
                    actualValue = Verifier.GetValue(TestControl, propertyName);
                }

                if (shouldCompare)
                {
                    if (actualValue == null)
                    {
                        actualValue = "null";
                    }

                    string msg = String.Format(
                        "Property {0} expected value: [{1}]; actual: [{2}]",
                        propertyName, propertyValue, actualValue);
                    if (actualValue.ToString() != propertyValue)
                    {
                        throw new Exception("Check failed for " + msg);
                    }

                    Log(msg);
                }
                else
                {
                    Log("Executed check command: " + check);
                }
            }
        }

        /// <summary>
        /// Gets the value for a property that is calculated somehow rather
        /// than taken directly off the tested control. Also gives the chance to
        /// customize logs or to do a custom comparison.
        /// </summary>
        /// <param name='propertyName'>Property to read.</param>
        /// <param name='propertyValue'>Property value.</param>
        /// <param name='val'>On return, the value of the property.</param>
        /// <param name='shouldCompare'>
        /// On return, whether a comparison operation should take place.
        /// true by default.
        /// </param>
        /// <returns>
        /// true if the property was calculated or command executed, false
        /// otherwise.
        /// </returns>
        private bool GetSpecialProperty(string propertyName,
            string propertyValue, ref object val, ref bool shouldCompare)
        {
            IDataObject DO;
            val = null;

            // ClipboardText will return the text in the clipboard, null if
            // it's not text.
            if (propertyName == "ClipboardText")
            {
                DO = Clipboard.GetDataObject();
                bool isPresent = (DO != null) && DO.GetDataPresent(DataFormats.Text);
                if (isPresent)
                {
                    val = DO.GetData("System.String", true).ToString();
                }
                else
                {
                    val = null;
                }
                return true;
            }

            if (propertyName == "ClipboardTextLength")
            {
                DO = Clipboard.GetDataObject();
                bool isPresent = (DO != null) && DO.GetDataPresent(DataFormats.Text);
                if (isPresent)
                {
                    val = DO.GetData("System.String", true).ToString().Length;
                }
                else
                {
                    val = 0;
                }
                return true;
            }

            // LineCount can be used to get an estimate of lines.
            if (propertyName == "LineCount")
            {
                using (Bitmap b = GetBlackWhiteContents())
                {
                    Bitmap b1 = BitmapUtils.CreateBorderlessBitmap(b, 4);
                    Logger.Current.LogImage(b1, "bitmap");
                    val = BitmapUtils.CountTextLines(b1);
                }
                return true;
            }

            // LogCommands will log the commands on the control.
            if (propertyName == "LogCommands")
            {
                Log(CommandLogger.DescribeUIElementCommands(TestControl));
                shouldCompare = false;
                return true;
            }

            // LogSelection will log the selection object; nothing is checked.
            if (propertyName == "LogSelection")
            {
                Log("Selection: " +
                    TextTreeLogger.Describe(TestWrapper.SelectionInstance));
                shouldCompare = false;
                return true;
            }

            // TextLength can be used to get the total length of the text.
            if (propertyName == "TextLength")
            {
                val = TestWrapper.Text.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a black and white snapshot of the tested control, without borders.
        /// </summary>
        private Bitmap GetBlackWhiteContents()
        {
            Bitmap b = BitmapCapture.CreateBitmapFromElement(TestControl);
            Bitmap noBorder = BitmapUtils.CreateBorderlessBitmap(b, 2);
            Bitmap result = BitmapUtils.ColorToBlackWhite(noBorder);

            noBorder.Dispose();
            b.Dispose();

            return result;
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies typing is possible when control is first intialised with large amount of text 
    /// </summary>
    [Test(2, "TextBox", "TextBoxSequenceTest1", MethodParameters = "/TestCaseType:TextBoxSequenceTest1")]
    [TestOwner("Microsoft"), TestTactics("538"), TestWorkItem(""), TestBugs("202")]
    public class TextBoxSequenceTest1 : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("PasswordBox"))
                return false;
            return true;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();

            //Remove the wrap once Regression_Bug202 is fixed
            if (_element is TextBox)
            {
                ((TextBox)_element).TextWrapping = TextWrapping.Wrap;
            }
            _element.Width = 150;

            _ControlWrapper = new UIElementWrapper(_element);

            _ControlWrapper.Text = "!:\x5cb8\x4efc\x4e67\x01ec\x5ba3\x8bdc\x7281\xccba\xff6d\xeff0\xab0d\xa8ae\x6afc\x1d9a\x6726\x493c\x098d\x6cc0\xbde1\x8622\xa14f\x6b4c\x1640\x4259\x6c1a\xfb4e\x5097\x8187\x62b7\xe9af\xd80b\xdc61\x3f2d\x0943\x54c3\xd0e2\x21a4\x5a3c\x4316\x92fc\xeb8d\xcb8e\x9614\xeef7\x5097\x0c38\x9838\x97e9\xd1a0\x8f76\xd2d1\x858c\xf3c2\x3ad1\x5b0e\xf16a\x38bc\x0c0e\x95d0\x6054\x3b44\xf0f9\x6a02\x15be\x138e\xa829\x6980\x61b8\xc8d6\x9642\x2797\xaf50\x29a9\x2ae5\x9a94\xd5bf\xd537\x2747\xf1c0\x5fe2\x6315\xb8ad\xafe5\xd33f\x16dd\x56d4\x0f20\x045c\xae33\x6357\x9825\x1596\xc46b\xe2c1\x57b7\xfa93\x618c\x8056\xdb68\xdea1\xa2b6\x3680\xcf12\x3b5d\x67df\x2381\x5be3\xc8dd\xa03c\x854e\x1c33\x1174\x1a4c\x35ee\xfd3e\x8296\x4113\x96c1\xfedf\xbcb8\x9909\x6523\xb383\x657e\xfe1c\x1200\xeae3\x46e7\xd32c\x9fff\x7433\x54e0\x4bde\x4952\xbd2a\x2c94\xe999\x7488\x6b60\xf35a\xfaf0\x4641\x641e\x28e5\x4723\x86b1\xfb48\x8e7c\xe581\xd520\xb980\xcac9\x8175\x1eaf\x0f64\x3d92\x82fb\x6993\x3d40\x55de\x389c\x14fc\xf5af\x003c\x111a\xc752\xc595\xd0f9\x78c2\xd0a9\x98fb\xcc8a\x2b60\x0b85\xc1c7\x52c7\x0105\x8a9d\x1e5b\x776a\x16b7\x5c67\x6164\xfe0b\xe67e\xa969\xd36a\x2d2e\x39ee\x7f97\xa998\x8005\x3747\x1582\xbabd\xe5f4\x5090\x63e1\x17cb\x7fcf\xc11b\x2a4f\x15b9\x14d8\x3c24\xecf4\x9f68\x75f0\x5ce7\xbc9b\x3c8d\xf85c\x4b37\xc5d4\x3f27\xdace\xdc34\xf7a3\x19bd\x67af\xf3e7\x9866\x970a\xf92a\x2711\x18f5\x9542\x15de\x50c6\xdbd2\xdc08\x9d22\x72d2\x9e8b\xb64e\xec66\x46ad\x4c8c\xc1e6\xf988\x0a00\x5d79\xd046\x7d51\x4309\xb13a\x34ce\x7172\xd65a\xd8c9\xde88\x58eb\x4a24\xb01b\x8be7\x28b4\x9343\x1c8e\xedc6\x232e\x57b2\x898a\x2529\x8113\xba64\x9f6a\x858a\xacab\x52bf\x9586\x670a\x35bd\x2033\x5daf\x966d\xc81e\x19b8\xb620\x75d5\xe426\x23cf\x3f83\x723d\xa974\xa94a\x52fe\xc2b6\x12a3\x8d99\x5922\x2a1f\x5ec5\x9eb7\xa1d6\x8075\x3850\x4f32\xc2ec\xa39e\x2baf\x8821\x61ec\x839a\x4901\xcb52\x232e\x29f0\x526b\xf578\x6095\x798b\x666c\x77f0\x3f07\x33e2\x4a07\xb2eb\xd79e\x111a\x4c6c\xc2d3\x9a07\x81eb\xf99b\x6b67\x0844\x9704\x7e5c\xd978\xdd39\x78eb\x56cb\x1281\x4a25\x5c04\xc1e2\xbbe7\x293b\xda18\xde20\x290d\x704a\x1d29\x2d04\xdb0e\xdf96\xe6d3\x5fb0\x6df3\x366e\x8563\x7793\xb1d2\x5732\x68db\xee18\xda27\xdfff\xdb0c\xdd98\x8c29\xab92\x7531\x975b\xe7a8\x22bf\x53eb\x2dca\xe301\x71fe\x8e1e\x89b0\xf97d\x8afd\x235e\x5289\x7cdd\x54e7\x1a04\xa7b5\x216f\x3754\x1069\xa6cb\xd8b9\xdc05\xa4f7\xbb4d\xe149\x6deb\xe1e2\x31e9\x4ba1\x1b12\xf9f1\x7d7a\xfb17\x85cd\x455a\xbcd6\x6fc5\xb909\x7cae\xfbf3\xa84e\xfbb2\x7e15\x26d4\x33d4\x1652\x713b\x8b17\x6ff9\x35e3\xa19d\x54d4\x9b29\xce65\xbea1\xe650\x7dc7\x989c\x4c7f\x7516\x901b\x5c35\x3e0e\xda44\xde6a\xa5e3\x5770\xf70f\x0f8d\x5d2d\x37ef\x6865\xbaa5\x1474\xfe7b\xda0d\xdd05\x7e23\x8779\xcaf7\xfcae\x56ca\xc1ee\x15be\x7973\xa51c\xe24f\x2cb2\xc72a\x5f08\x3f0f\xd728\x6c8a\x0797\x6bd8\x4c19\xbda3\x9fa9\x95c7\x8df0\xbee0\x7a2a\x7b8a\x12cc\xfdf2\x3938\x9a2e\x86b4\xcfe8\xe19a\x630e\xffa2\x1122\x8186\x7866\x3950\x9a45\x284f\x64f6\xebed\xc392\x2abd\x2fe4\xb084\x1e1d\x60c5\xfbda\xb30d\xa89b\xb262\x1f61\xe397\xe85a\xf988\x0c16\x8283\xdb3f\xdebc\x17ca\x403b\x0aee\x5b9a\x5cc8\xe58f\xfbf8\x3f6c\xc605\xeb02\x8f30\x3287\xb1d3\x9554\x3ab3\xa9d8\xca5c\x9423\x4a46\xcb04\xf4ad\x9d2d\x3d5d\xe71f\xd876\xde18\xc238\x7f76\x1747\x1799\x7570\xf5e1\x5e0f\xeb21\x1084\x24bb\xe0ff\x67f7\xcdf4\x4a54\xb48a\xda6a\xdeb0\x10d7\x23dd\x7613\x008f\x8a0c\xa8e3\x3e82\x2f2b\x77f2\x383d\x103a\xba73\x7a9c\x02dd\x23cd\x7d80\x3f42\x2356\x7c81\x2824\xae6c\x7592\x994d\xd477\xc6b0\x84cf\x15f8\xc8d7\x6264\xc62e\xfff1\x167a\x15fd\x026e\x2c85\xc342\x6805\x1cf7\x382c\xd692\xd8c4\xdfa1\xfd7c\xbda4\x4dd4\x30ad\x95e7\x21dd\xbd32\xea75\x8eb1\x26fd\x3808\xb68b\xec52\x9b44\x8a8e\xa19b\x39dd\x053d\x92eb\x75a9\xccc5\xb1c3\x384a\xf9bf\xa476\x782e\xd656\x6089\x157b\x224a\xeb29\xa5ee\x4f5f\xc5fe\x7814\xdba8\xde1a\x9602\xeee6\xf419\x0ba5\x77ed\x377c\xd8f4\xdf79\x5f70\x161b\x913f\x38b4\xc669\xe319\x32ee\x43a6\x631b\x1ba7\x4bb8\x8559\x5413\x8c36\x1db9\x4b85\x5ca9\xd4fa\x6c67\x3bd2\x9219\x672c\x2654\x2330\xaee5\x1af8\xa3d9\x1655\x9ed1\x6a04\x54d8\x7a47\x5f55\x1b4f\x4505\x6213\x4517\x27d5\x4f10\x3f30\xb83a\x6248\xec43\xaa56\x2c5c\x564e\x328e\x09cc\xd12d\xa893\xaefb\xa2f2\xcb13\x9d21\x4c57\xf843\xefc5\x6e0f\x89cd\xab70\x3f3f\x1c99\xa4d4\xf916\xc6ce\xd16f\x2604\x38c4\x4731\xbba4\x066e\x34d3\x85e9\x2d36\x8396\x2fd0\x7ae8\x7bfc\xf6d2\x5897\xe869\x9a0d\x4526\xf63c\xbb08\xb1e4\xaf32\xc232\xcef7\x4cc0\x724c\xd706\x9e07\xa39f\xfff0\x9ba0\xbd72\xf32b\xe387\x5aeb\x3049\xf9c9\xc29a\x1588\xa224\x7a27\x1d08\x9ddd\x198b\x1c86\x7d5b\x73c9\x773b\x3135\xc306\xa531\xd772\xae96\x3e0d\x14e9\x223c\x63d1\x69cb\xfa6f\x4958\x2f67\x96cb\xe249\x2d45\xe7f4\x725d\x87bb\x9873\x9be7\x284d\xee9f\xd772\x2f9d\x5417\x40e0\x94dc\x1155\xa8a6\xb271\xcf64\xfe81\x5fca\x6cd1\xe098\x5abf\xc42d\x0edb\xb51d\xce9e\x38ae\xcc77\x8ffd\xc829\xcc2f\x72bd\xe35a\x3abe\x7097\x3195\xaa28\xf59e\xdb55\xdf6d\x08e8\xd466\xcdbd\xa7d5\x5a7f\xfd2c\x800c\x10e7\xbb29\xb759\x2b0b\x4ad5\xcf9b\x29fa\x01b0\xd285\x23c7\x6381\xd29c\xc9d3\x6339\x5bd4\x5ea1\x0f48\x636c\xe158\x5d85\x5a1d\xa0bd\x7710\x0848\xd9c4\xdc8c\x8475\x63e8\x0c32\x8d00\x1c58\xb45b\xb7f0\x4e92\x27c7\x114e\xd8a3\xde74\x8159\xa320\xb960\x390d\x9e11\x0dce\x46a7\x2302\x1181\x781a\xad12\x75c4\xbe74\x4468\x7925\x9fa6\x25ee\x7028\x4418\xaf10\x5145\x27a9\x4b25\xc610\xf57d\x4584\x076f\xaf24\x1aab\x7b41\x3b71\x4a85\x85fc\x7229\xe211\x3e38\xa424\x2110\x02ab\x6942\xc1a0\xb6c2\x11ab\x0c5b\xb6d5\x966c\xce8a\xd7ee\x3b35\x1849\x28a4\xb79d\xcd35\x2993\x4bd9\x59b0\x57fa\xf34e\x438e\x588c\x065f\x9781\x0856\x963e\x3ca8\x628c\xefc7\x4c4b\x6255\x3aed\xfb81\x04dd\x6d7c\xdad5\xdfb5\x9a81\x15fe\x3eca\x0f3b\x5f11\x76f9\x2f7f\x73ee\x0261\x529d\x83d3\x7d4f\x5c69\x2650\x8861\xe63c\xb0d4\xdaa8\xdd80\x62e2\x2a1f\xae6c\x7b6b\xcfb2\x5448\xa6a3\x823e\x7598H\x1cc7\x23c6\x4a20\xf25f\x3c23\xb156\x43b2\x1930\xe413\xe47c\xe191\xd6df\x2392\x05f4\x43a1\xb8d3\xe53c\x935d\x25fb\xd9f3\xdd52\x4aad\x275c\xc35d\x77f2\xc34f\xec14\x9a91\x6f16\xbaf1\xb86c\xf4b9\xb9e5\x73a6\xe599\x2ed7\x39b2\x8aee\x2046\x74f8\x44ae\xcd0b\xccc0\xf903\xc88b\x8c02\x068c\xa877\x37ca\x16de\x6f0b\x1346\x5c43\x9ba4\xab97\xd218\xfc68\x86c2\x7a6c\x78d2\xc541\xa91f\xaa18\x2921\x2c0f\xecd7\x1cf9\xafeb\x205a\x14c9\x7f20\x5a4d\x7317\xb101\x9544\x87a3\x7deb\x2e57\xfad0\xebef\xbcc2\x439c\x62c7\x5838\x4be6\xa526\x9875\x1e41\xc80d\x1380\x326d\xb2ef\x1082\xa772\xafb5\x9b8e\x22f2\xa39f\xccf4\xdbb3\xddbc\x56a0\x881d\x2300\x97bc\x14bd\xa02e\xab41\x065f\x23f3\x542d\xce11\x8bf0\x8e7b\xbc0e\x81a5\x4658\x51e0\xda1b\xdd39\x5461\xfeb6\xe7dc\x0cd9\xe25b\xcc30\x49cb\xcba3\x014c\xf9b5\x64b0\xda4b\xdd85\x1f1c\x7ce7\x6622\xbb7e\x3fc7\xc07b\x3728\x04f7\xed33\x17e1\xa419\xbf52\x645b\x26ff\x8205\xeb63\x2e10\x5535\xd111\xc664\x460b\x8752\x7050\x6ec2\x7b43\x40a3\xcb8b\xcaf1\xd48a\xa9f4\x0ca9\xbf42\x79e0\x6cae\x6cd3\x1194\x55ec\xc625\x0691\x9163\xa611\x81f0\x672c\xe6d5\x43c2\x4d86\x7e00\xa530\xc7c5\xe03f\xd33a\xa47f\x939e\x13e7\x1b56\x97c9\x0c96\xf75e\x403b\xff22\xf4ef\x6c35\x306c\x433e\x0b38\xe4d6\x4571\xf7ab\xba2a\x7070\x9576\x67ea\x4ea4\x3fae\x2053\xc41a\x2026\x897a\x2b00\x2dbd\xc2a2\x265a\x032c\xf449\xd6b9\x6828\x2ba4\x65f9\x5157\xd509\x04fe\x5e8c\x85ea\x076e\x9c73\x39db\x5183\x23ed\xdb9c\xdd7b\x0814\x8653\xeb05\x5754\x4ac8\x6b50\x55db\x53f0\xf393\x573a\x77a2\x831b\xcc5e\x127d\x3c7f\xce94\x6909\x3c21\x6c84\xa30e\xb932\xda99\xdf77\xe51f\x6b72\x36ea\xe1fe\x4736\xa339\xe676\x7297\xfc37\xaddc\xcc13\x25a9\x3c4e\x3b53\xabd6\xa981\x6b69\x124c\xd7b3\x7265\xfa1c\x5d66\x81e1\x922c\x736c\xcaed\xcddd\xd0d0\x1561\xb767\x388e\xa5b7\x289b\xd817\xdffe\xebe1\x13de\x8950\x0ea5\xb0b5\x0d1c\xe4a2\x7b69\xd53eJ\xecd30\x9340\xbd31\x929f\x011b\x90c1\xe17e\x6d12\xac36\x87b8\xe98f\xa4bc\x6e92\x7c48\xd55a\x15a5\x5d35\x44cf\x754e\x265c\xfd0c\x5c4f\x356e\xbff4\x95a3\xe217\x03a7\x26fd\x6549\x1579\xe1fc\xaca2\x91e1\x8698\xcabc\x3a9c\x139e\x82c1\xb64c\xa7cb\xc438\xbb88\x33c8\x7e2b\x0252\x6f20\x1abe\x925c\xdb59\xdfdd\x876d\x3699\x5fef\xd9ed\xdfbf\xa3e0\x5dd2\xfd3c\xfcfa\x1f02\x8d1a\xba80\x07c9\x96bd\xa5bb\x3cec\x12da\xe7f9\xb18a\x9abd\x0207\xda73\xde3a\xcd81\x6222\x4184\x2887\xb741\xbda2\x2683\xc759\x714a\x4ba2\x6dd2\x8e0b\xab62\x4cb2\xb7f3\x96d1\x26db\xd859\xdcc9\x85c5\x974a\x1ab0\x09b7\xb3bf\x9d09\xd86f\xdf15\x5c45\x32c4\xe0d0\x409b\xf56e\xacf8\xa816\x926d\x77ca\xae41\x7b59\xa690\x3f9a\xd677\x57a8\x1bd0\x6edd\x99f5\x08b2\xfa58\xf038\x5ae6\x5127\x8aaf\xbdf3\xeba4\x54ad\xf736\xb2d1\x586b\x8dc4\x8b7c\xdbe0\xded1\xca3c\x9494\x9078\x0b06\x7863\xe111\x034b\xba44\x4028\xe88e\xab80\x3638\xd6c5\xaf4d\x3fa2\xfedf\x9ad8\x19c9\x9457\xdb54\xde15\x015f\xe19b\x5620\x82a7\x09c9\x584a\xb0df\xd87d\xdf9a\x207d\xefdc\xc7be\x475d\x0c3b\xc312\x8b56\x63d6\x2191\x279f\xf70d\x35f3\x1abd\x6898\xda1e\xdf2e\x14df\xa55f\xb794\xb3f0\xbd91\x73fa\xf724\x00bb\x2312\xc8dd\xb2f8\x3a57\x885e\x6e9a\x88c6\x526b\xdaa8\xdc83\xc811\xbba3\x6e78\x8f68\xa311\x7c8e\x7388\x3701\xf836\x6cb7\xe707\x9f36\xe6a0\x7903\x76f1\xfaea\xf4e9\xb2ea\xf949\x2baa\x61cc\xac81\xf8b7\xc703\x244a\x435d\x101a\x50fe\xdb76\xdecb\x9ed7\xa4a2\x5b4a\xf9f3\xa085\x238b\x1942\x8576\x024d\x3b05\x4068\x868f\x7bc3\x8a6c\x19b3\x83da\xe23b\x39f4\xc365\x8d73\x79b0\xd5db\x5920\xaefe\xbefb\x1b8f\xc2ea\xa774\x6b1d\x5fb4\x6c74\x2289\x5b89\x4540\xcdde\x4265\x43ec\xecac\xd87c\xdd4d\xe1a8\x6f72\xb09c\xbe43\xeb40\xdb3c\xdcc2\x6e4a\xad4f\xa06f\x6120\xd624\x8d98\x4e03\x3946\xc8fb\x4b81\xac4a\x3af7\x84f5\x609f\x71cc\x1a58\xa298\xce90\x1dde\x2b05\x3683\xbc8c\xd74d\x3fee\xf58d\x6177\x6ffe\xabca\x0a3d\x253f\x9adc\xc3bc\xe3be\xead0\x54a0\xfa24\xcaad\xfe93\x964f\x94ef\x0d86\x0bf9\x04e3\xf6e2\x97a1\xb1b5\x5386\xf2c6\x6fdb\x551a\x0e04\xefb2\xcd62\xb036\xfa3a\xb1bd\xd601\x6081\x6b91\x74ad\x1d99\xa238\x2f09\xa3bc\xb0a4\xe88c\x5737\x9a24\x0bff\x77a7\x4faa\xa403\x3841\x88ed\x1d7f\x2a89\xb7a8\xe069\xa84b\x43d8\x0df0\x7d37\x3bee\xb521\x173a\xab28\xf659\x3387\xf095\xa2e1\x2422\x6a2b\x9301\x21a1\x7756\x6b4c\xd984\xdcef\x463e\xaf14\x5a7d\xb961\xe6c7\xf832\x056f\x69ff\xb771\x4474\x92b7\xcfb0\xfa13\xf597\xb834\x27b9\x66bc\xa061\x6649\x79e6\x8c81\x057b\xf231\x23af\xa98d\x691c\x5384\xe57d\x1102\x169f\x1197\xb231\x4d99\x5682\x9a2a\xf936\xe95a\x548e\x966f\x43bb\xafb2\xad3a\xf3b6\xb1e5\xa0cf\x20e4\xa8cc\x2e8a\xb1f6\x6b48\xbae4\xd6f3\x0503\x6308\xd4a3\x40c3\xbce1\x36cd\x0fd3\x7daa\xa4ad\x1ff1\x58fc\xa0d2\x32dc\xe084\x8217\xa390\x5b6c\xbefc\x3e5d\x122e\x09f2\x228e\xca33\xd847\xdf46\x404c\x82df\x88a9\xc04e\x24fa\x3387\xa5b9\x5bb9\x3aa3\xad2e\xea8f\x81de\x5967\x3c54\xb28c\x44bb\x18c4\x9f0c\x8fc0\x543d\x52e3\x7f08\xc049\x9ecb\x053c\xd31d\xf2f8\x2cea\xa8b6\xcd01\xe273\x98d4\xf55c\x2efe\x7c95\xb3ed\x8728\xbd5b\x7fb3\xec68\xb342\x4518\xf838\x3356\x9786\x21b1\x0205\x82a7\x8bcf\xcd71\xf12d\x8381\x3a72\x8b08\xbedf\xc143\xc294\xe9dc\xbb12\x51dc\x408d\x78ce\xb302\x6da0\xcab9\x51bb\x8c81\x2a69\xbfbe\xfe9e\xbd91\x5b67\x1f58\xa356\xa0fa\x0dca\x86ce\x8cf3\x0744\xe389\xd118\x7050\xa11a\xdb43\xdf6f\x5ef2\x5e61\x6a53\x701d\xbb51\xf157\x9d4b\x0249\x2dd7\xabdb\x3a73\x9215\x8a97\x689c\x45cb\x952f\xd79a\xc2e8\x8d30\x0fe0\x95c5\x6428\x971a\xea0d\xb114\x3a74\x35a0\xe297\xd787\x80c3\xd03b\xd7b3\xd7bd\x91cc\x6bc6\xf358\x222e\xba4a\x046d\x0d46\x2045\x591e\xf180\xf77a\x6686\x7bb4\xfc35\x2457\xc178\x4e59\xf97c\xad67\x13ea\xcb62\x460d\xfac9\xc746\x8045\xbf07\x80dc\xbbb7\xbab2\x2ac0\xad13\xdb9e\xdd8a\xba57\xf8c9\xfcd4\x5272\x7301\x1d50\xbe7a\x7fe9\xef99\x3ca6\x72a7\x9f9e\x8386\x3560\x3e3d\x1149\x211f\x892d\x8745\x22d4\xc3c9\x0c5a\x4bbf\x70fc\x2c12\xa1e7\xfb42\x838f\x518e\x6592\x2e5e\x446c\x1bdc\x03c6\xc15b\x036b\x2781\x6f06\xdb57\xdf70\xeeeb\x93ff\xdbc7\xdc25\x8821\x27a7\xfcbd\x89a6\x429f\xaa6d\x9993\xa191\x25cd\xb8ca\x9ef5\xadfc\xad0a\x8bd8\x265f\xd118\x220d\x3aeb\x2e5a\x8a07\x0e47\x2e3a\x83c2\x7fbf\x7403\x3ad1\xe9c7\xb218\xadd5\xab19\x33e7\x2fc9\x3091\x4257\xe8d9\x046b\xa529\x719b\x40f0\xa720\xcbfd\x8ccc\x1e9f\x6310\x64d0\x135f\x5660\x9ba7\x48a6\x0a3e\xba1e\xb400\x65a4\x51c0\xfb1e\x59e6\xa3e3\x7cfd\x15a3\x07cd\xc0a4\xe77a\xf3bb\x7ab3\x84e2\x6f2b\x7d6a\x6ab2\xa2fe\x21f4\x2bef\xb071\xf9f9\x8739\xbe08\x817a\x20b1\x1aef\x60a4\xe470\x4e1f\x6971\xa396\xf0f9\x7fe6\xca23\xd91c\xded0\x8ef2\x6087\x282b\x5bf8\x3923\xe67a\xe482\x990f\xa3ea\xdbd6\xde2d\x95f3\xebac\xf8a8\x26b2\xd826\xde4e\xba05\xda4e\xde6a\x79a3\x4a4b\x8331\x620e\xb4fd\x235b\x7285\x7e09\x5024\x89b8\x04fb\xa506\x9e99\x237a\x140b\xc16c\x03c4\x5479\xc0d6\xa0bd\xd985\xdfa1\x6bb0\x4283\x254a\xcaaf\xf8c3\x5575\x42bf\xc547\xe74c\xc5e0\xebb8\x4b19\xcd94\x44a7\xdb18\xdd54\xa6f9\x15c8\x73f5\x6678\x48eb\x1a32\xd932\xdd42\xf758\xc832\xe49d\x6545\xda89\xded4\xf92e\x3dac\xba2f\x9138\xd902\xde9a\x3cc4\xea4d\x2a96\x1d0f\x3b49\x8adc\xa26b\x3f1a\xb94d\x537f\x55e4\xcf63\xe416\x3da7\xad7e\xab0d\x2cc8\x730c\x9f7e\x517d\x6950\xd4ab\xd355\x3091\x70d7\xdbaa\xdd78\x3219\xccb1\xae0b\x90e9\x3c5b\x660c\x9ca6\x8957\xbcab\xeb30\x56e5\x2b2e\xbe0d\x77c5\x9a17\x974f\xa3d9";
            MainWindow.Content = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focuses on element</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(TypeABC);

        }

        /// <summary>Types text</summary>
        private void TypeABC()
        {
            KeyboardInput.TypeString(_inputString);
            QueueDelegate(VerifyTextABC);
        }

        /// <summary>Verifies text </summary>
        private void VerifyTextABC()
        {
            string str = _ControlWrapper.Text;
            Verifier.Verify(str.Contains("A")||str.Contains("a"), " String does NOT contain [A]", false);
            Verifier.Verify(str.Contains("B")||str.Contains("b"), " String does NOT contain [B]", false);
            Verifier.Verify(str.Contains("C")||str.Contains("c"), " String does NOT contain [C]", false);
            KeyboardInput.TypeString(_emotyString);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private string _emotyString = "";
        private string _inputString = "ABC";

        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _ControlWrapper;

        #endregion Private fields.
    }
}