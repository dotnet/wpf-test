// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit and functional testing for the TextPointer class.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Threading; using System.Windows.Threading;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.
    
    /// <summary>
    /// minic the behavior of people picker doing text navigation.
    /// The 
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("513"),TestBugs("358")]
    public class ShellPeoplePickerRegression_Bug358 : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            this._textBox = new TextBox();
            
            this._textBox.Text = ConfigurationSettings.Current.GetArgument("TestString");
            MainWindow.Content = this._textBox;

            QueueHelper.Current.QueueDelegate(new SimpleHandler(TestNavigateTextContent));
        }
        
        /// <summary>
        /// You might ask why we create those unused TextPointer, it is because we need to have 
        /// some more text node to repro the problem. Without the TextPointer (indeed TextPoosition)
        /// all text in the textbox is just one text node, and the bug will not happen
        /// </summary>
        private void TestNavigateTextContent()
        {
            TextPointer tp1 = this._textBox.StartPosition.CreateNavigator();

            tp1.MoveByDistance(3);

            TextPointer tp2 = this._textBox.StartPosition.CreateNavigator();

            tp2.MoveByDistance(4);

            TextPointer tp3 = this._textBox.StartPosition.CreateNavigator();

            tp3.MoveByDistance(5);

            TextPointer tp4 = this._textBox.StartPosition.CreateNavigator();

            tp4.MoveByDistance(6);

            MovePointerToPrevTextUnit(this._textBox.EndPosition.CreateNavigator(), this._textBox.StartPosition.CreateNavigator());
            Logger.Current.ReportSuccess();
        }

        private bool IsDelimiter(char c)
        {
            if (c == ';')
            {
                return true;
            }

            return false;
        }

        private void MovePointerToPrevTextUnit(TextPointer mp, TextPointer mpLimit)
        {
            // make a copy of the MP that will move backwards
            TextPointer mpCurrent = mp.CreateNavigator();

            while (mpCurrent > mpLimit)
            {
                if (mpCurrent.GetSymbolType(LogicalDirection.Backward) == TextPointerContext.Text)
                {
                    string strText = mpCurrent.GetTextInRun(LogicalDirection.Backward);
                    char[] chText = strText.ToCharArray();
                    int i = chText.Length - 1;

                    while (i >= 0 && !IsDelimiter(chText[i]))
                    {
                        i--;
                    }

                    if (i < 0)
                    {
                        // didn't hit a delimiter yet
                        // need to keep going to the next context
                        mpCurrent.Move(LogicalDirection.Backward);
                    }
                    else
                    {
                        // seek just past the text and break out
                        mpCurrent.MoveByDistance(-1 * (chText.Length - i));
                        break;
                    }
                }
                else
                {
                    mpCurrent.Move(LogicalDirection.Backward);
                }
            }

            mp = mpCurrent;
        }

        #endregion Main flow.

        #region Private member
        private TextBox _textBox = null;

        #endregion
    }
}
