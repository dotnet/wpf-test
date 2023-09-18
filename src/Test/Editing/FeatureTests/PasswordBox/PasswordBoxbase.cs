// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Markup;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Collections;
    using System.Windows;
    using System.Security;
    using Test.Uis.Wrappers;


    #endregion Namespaces.

    /// <summary>
    /// this class contains code for testing the passwordbox.
    ///</summary>
    public class PasswordBoxBase : CommonTestCase
    {
        /// <summary> </summary>
        protected Test.Uis.Wrappers.UIElementWrapper _wraper1, _wraper2, _wraper3, _wraper4;
        /// <summary>PasswordChangedCounter</summary>
        protected int PasswordChangedCounter = 0;
        /// <summary>Engine generating combinations to test.</summary>
        protected CombinatorialEngine _engine;
        /// <summary>Hashtable to hold the values for each conbination.</summary>
        protected Hashtable values;
        /// <summary>Array list for holding TestData</summary>
        protected ArrayList TestDataArrayList;
		/// <summary>Default passwordChar</summary>
		protected char DefaultPasswordChar = (char)0x25CF; 
        
        /// <summary>Set up the test </summary>
        public override void Init()
        {
            EnterFuction("Init");
            PasswordBox passwordBox1 = new PasswordBox();
            PasswordBox passwordBox2 = new PasswordBox();
            TextBox textBox1 = new TextBox();
            TextBox textBox2 = new TextBox();
            Panel panel = new Canvas();

            panel.Background = Brushes.Violet;
            MainWindow.Content = panel;
            ((IAddChild)panel).AddChild(textBox2);
            ((IAddChild)panel).AddChild( passwordBox1);
            ((IAddChild)panel).AddChild(passwordBox2);
            ((IAddChild)panel).AddChild(textBox1);
            passwordBox1.Width = 250;
            passwordBox1.Height = 70;
            passwordBox2.Width = 250;
            passwordBox2.Height = 70;
            textBox1.Width = 250;
            textBox1.Height = 70;
            textBox2.Width = 250;
            textBox2.Height = 70;

            Canvas.SetLeft(passwordBox1, 20);
            Canvas.SetTop(passwordBox1, 20);
            Canvas.SetLeft(passwordBox2, 20);
            Canvas.SetTop(passwordBox2, 100);
            Canvas.SetLeft(textBox1, 20);
            Canvas.SetTop(textBox1, 180);
            Canvas.SetLeft(textBox2, 20);
            Canvas.SetTop(textBox2, 20);

            passwordBox1.FontSize = 50;
            passwordBox2.FontSize = 50;
            textBox1.FontSize = 50;
            textBox2.FontSize = 50;
            _wraper1 = new Test.Uis.Wrappers.UIElementWrapper(passwordBox1);
            _wraper2 = new Test.Uis.Wrappers.UIElementWrapper(passwordBox2);
            _wraper3 = new Test.Uis.Wrappers.UIElementWrapper(textBox1);
            _wraper4 = new Test.Uis.Wrappers.UIElementWrapper(textBox2);
            QueueDelegate(DefaultFocusToFirstPasswordbox);
            //textbox2 is used for helping calculating the location of Password Character
            //it is at the back of passwordBox1
            _wraper4.Text = "************************"; 
            EndFunction();
        }
        
        void DefaultFocusToFirstPasswordbox()
        {
            _wraper1.Element.Focus();
        }
        #region Shared methods
  
        /// <summary>
        /// Verify the values in a PasswordBox
        /// </summary>
        /// <param name="passwordBox"></param>
        /// <param name="expectedPasswordChar"></param>
        /// <param name="expectedPassword"></param>
        /// <param name="expectedSelectionStart"></param>
        /// <param name="expectedSelectionlength"></param>
        /// <param name="expectedMaxLength"></param>
        protected  void PasswordVerifier(PasswordBox passwordBox,  char expectedPasswordChar, string expectedPassword, int expectedSelectionStart, int expectedSelectionlength, int expectedMaxLength)
        {
            string actualPassword = PasswordString(passwordBox);
            if (actualPassword != expectedPassword)
            {
                MyLogger.Log(CurrentFunction + " - Failed: Password should be: [" + expectedPassword + "] Actual: [" + actualPassword + "]");
                pass = false;
            }
            if (expectedPasswordChar != passwordBox.PasswordChar)
            {
                pass = false;
                MyLogger.Log( CurrentFunction + " - Failed: PasswordChar should be: [" + expectedPasswordChar + "] Actual: [" + passwordBox.PasswordChar + "]") ;
            }
            if (passwordBox.MaxLength != expectedMaxLength)
            {
                MyLogger.Log(CurrentFunction + " - Failed: SelectionStart should be [" + expectedMaxLength + "] Actual: [" + passwordBox.MaxLength + "]");
                pass = false;
            }
        }

        /// <summary>
        /// build a securestring from a normal string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public SecureString BuildSecureString(string str)
        {
            int i = 0;
            SecureString sSring = new SecureString();
            while (i < str.Length)
            {
                sSring.AppendChar(str[i++]);
            }
            return sSring;
        }

        /// <summary>
        /// Event handler for PasswordChanged event
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        protected void PasswordChangedEventHandler(object o, RoutedEventArgs args)
        {
            PasswordChangedCounter++;
        }

        /// <summary>
        /// return the password as a string of a PasswordBox
        /// </summary>
        /// <param name="pBox"></param>
        /// <returns></returns>
        protected string PasswordString(PasswordBox pBox)
        {
            return pBox.Password;
        }
        
        /// <summary>
        /// Perfrom Password Input.
        /// </summary>
        protected void PasswordInputExecution()
        {
            EnterFuction("PasswordInputExecution");
            if (TestDataArrayList.Count > 0)
            {
                MyLogger.Log("");
                MyLogger.Log("*******************************Start a new combination - " + ((PasswordBoxInputData)TestDataArrayList[0]).CaseDescription + "*****************************");
                if (TestDataArrayList[0] is PasswordBoxKeyboardInputData)
                {
                    KeyboardInput.TypeString(((PasswordBoxKeyboardInputData)TestDataArrayList[0]).KeyboardInputString);
                }
                else if (TestDataArrayList[0] is PasswordBoxMouseDragData)
                {
                    int start = ((PasswordBoxMouseDragData)TestDataArrayList[0]).MouseStartIndex;
                    int end = ((PasswordBoxMouseDragData)TestDataArrayList[0]).MouseEndIndex;
                    UIElementWrapper.SelectionDirection dir = (start <= end) ? UIElementWrapper.SelectionDirection.LeftToRight : UIElementWrapper.SelectionDirection.RightToLeft;
                    Rect startRect = _wraper4.GetGlobalCharacterRect(start);
                    Rect endRect = _wraper4.GetGlobalCharacterRect(end);
                   
                    //Regression_Bug108 work around.
                    double StartX = (startRect.Left <= endRect.Left) ? startRect.Left + 2 : startRect.Left - 2;
                    double EndX = (endRect.Left <= startRect.Left) ? endRect.Left + 2 : endRect.Left - 2;
                    Point startPoint = new Point(StartX, startRect.Top + startRect.Height / 2);
                    Point endPoint = new Point(EndX, endRect.Top + endRect.Height / 2);
                    MouseInput.MouseDragPressed(startPoint, endPoint);
                }
                else if (TestDataArrayList[0] is PasswordBoxMouseDoubleClickData)
                {
                    int location = ((PasswordBoxMouseDoubleClickData)TestDataArrayList[0]).ClickIndex;
                    Rect rect = _wraper4.GetGlobalCharacterRect(location);
                    //Regression_Bug108 work around.
                    int X = (int)((location >0 )? rect.Left -2: rect.Left + 2);
                    MouseInput.MouseClick(X, (int)(rect.Top + rect.Height / 2));
                    MouseInput.MouseClick(X, (int)(rect.Top + rect.Height / 2));
                }
                QueueDelegate(Check_PasswordInputExecution);
            }
            else
                QueueDelegate(EndTest);
            EndFunction();
        }

        void Check_PasswordInputExecution()
        {
            EnterFunction("Check_PasswordInputExecution");
            PasswordBoxInputData pData = (PasswordBoxInputData)TestDataArrayList[0];
            PasswordBoxStatus fStatus =pData.FinalStatus;
            PasswordBoxStatus iStatus =pData.InitialStatus;
            PasswordVerifier(_wraper1.Element as PasswordBox, fStatus.PasswordChar, fStatus.Password, fStatus.SelectionStart, fStatus.SelectionLength, fStatus.MaxLength );
            
            if (!pass)
            {
                //we only print out the status values if a case fails.
                if (iStatus != null)
                {
                    MyLogger.Log(CurrentFunction + " - Initial Password[" + iStatus.Password + "]");
                    MyLogger.Log(CurrentFunction + " - Initial SelectionStart[" + iStatus.SelectionStart + "]");
                    MyLogger.Log(CurrentFunction + " - Initial SelectionLength[" + iStatus.SelectionLength + "]");
                    MyLogger.Log(CurrentFunction + " - Initial PasswordChar[" + iStatus.PasswordChar + "]");
                    MyLogger.Log(CurrentFunction + " - Initial MaxLength[" + iStatus.MaxLength + "]");
                }
                else
                    MyLogger.Log(CurrentFunction + " - Initial values are not set!!!");
                if (TestDataArrayList[0] is PasswordBoxKeyboardInputData)
                {
                    MyLogger.Log(CurrentFunction + " - KeyboardInputData[" + ((PasswordBoxKeyboardInputData)pData).KeyboardInputString + "]");
                }
                else if (TestDataArrayList[0] is PasswordBoxMouseDragData)
                {
                    MyLogger.Log(CurrentFunction + " - PasswordBoxMouseInputData.MouseStartIndex[" + ((PasswordBoxMouseDragData)pData).MouseStartIndex + "]");
                    MyLogger.Log(CurrentFunction + " - PasswordBoxMouseInputData.MouseEndIndex[" + ((PasswordBoxMouseDragData)pData).MouseEndIndex + "]");
                }
                else if (TestDataArrayList[0] is PasswordBoxMouseDoubleClickData)
                {
                    MyLogger.Log(CurrentFunction + " - PasswordBoxMouseInputData.ClickIndex[" + ((PasswordBoxMouseDoubleClickData)pData).ClickIndex + "]");

                }
                MyLogger.Log(CurrentFunction + " - Final Password[" + fStatus.Password + "]");
                MyLogger.Log(CurrentFunction + " - Final SelectionStart[" + fStatus.SelectionStart + "]");
                MyLogger.Log(CurrentFunction + " - Final SelectionLength[" + fStatus.SelectionLength + "]");
                MyLogger.Log(CurrentFunction + " - Final PasswordChar[" + fStatus.PasswordChar + "]");
                MyLogger.Log(CurrentFunction + " - Final MaxLength[" + fStatus.MaxLength + "]");

                QueueDelegate(EndTest);
                return; 
            }
            TestDataArrayList.RemoveAt(0);
            if (pData.Reset && TestDataArrayList.Count > 0)
            {
                SetInitValuesForNextLocalCase();
            }
            QueueDelegate(PasswordInputExecution);
            EndFunction();
        }
        
        /// <summary>
        /// Set up the inivalues for next case.
        /// </summary>
        protected void SetInitValuesForNextLocalCase()
        {
            EnterFuction("SetInitValuesForNextLocalCase");
            Init();
            PasswordBox pBox = _wraper1.Element as PasswordBox; 
            PasswordBoxStatus initialStatus = ((PasswordBoxInputData)TestDataArrayList[0]).InitialStatus ; 
            if (initialStatus != null)
            {
                pBox.Password = initialStatus.Password;
                pBox.PasswordChar = initialStatus.PasswordChar;
                pBox.MaxLength = initialStatus.MaxLength;
            }
            EndFunction();
        }
        #endregion 
    }

    # region Structs for Test Data and status
    
    /// <summary>
    /// PasswordKeyboardInputData
    /// </summary>
    public class PasswordBoxKeyboardInputData : PasswordBoxInputData
    {
        /// <summary>What to typing into PasswordBox</summary>
        public String KeyboardInputString;

        /// <summary>
        /// PasswordKeyboardInputData
        /// </summary>
        /// <param name="case_Description"></param>
        /// <param name="keybarodInputString"></param>
        /// <param name="Initial_Status"></param>
        /// <param name="Final_Status"></param>
        /// <param name="reset"></param>
        /// <param name="priority"></param>
        public PasswordBoxKeyboardInputData(string case_Description,  string keybarodInputString, PasswordBoxStatus Initial_Status,  PasswordBoxStatus Final_Status, bool reset, int priority):
            base(case_Description, Initial_Status, Final_Status, reset, priority)
        {
            KeyboardInputString = keybarodInputString;
        }
    }

    /// <summary>Data class for Double click</summary>
    public class PasswordBoxMouseDoubleClickData : PasswordBoxInputData
    {
        /// <summary>What to typing into PasswordBox</summary>
        public int ClickIndex;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="case_Description"></param>
        /// <param name="index"></param>
        /// <param name="Initial_Status"></param>
        /// <param name="Final_Status"></param>
        /// <param name="reset"></param>
        /// <param name="priority"></param>
        public PasswordBoxMouseDoubleClickData(string case_Description, int index, PasswordBoxStatus Initial_Status, PasswordBoxStatus Final_Status, bool reset, int priority)
            :base(case_Description, Initial_Status, Final_Status, reset, priority)
        {
            ClickIndex = index;
        }
    }

    /// <summary> Base Class for PasswordBox input data </summary>
    public class PasswordBoxInputData
    {
        /// <summary>Initial password set in the PasswordBox</summary>
        public PasswordBoxStatus InitialStatus;
        /// <summary>Final password status</summary>
        public PasswordBoxStatus FinalStatus;
        /// <summary>Description for each conbination</summary>
        public string CaseDescription;
        /// <summary>Will the PasswordBox be reset</summary>
        public bool Reset;
        /// <summary>Case priority</summary>
        public int Priority;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="case_Description"></param>
        /// <param name="Initial_Status"></param>
        /// <param name="Final_Status"></param>
        /// <param name="reset"></param>
        /// <param name="priority"></param>
        public PasswordBoxInputData(string case_Description, PasswordBoxStatus Initial_Status, PasswordBoxStatus Final_Status, bool reset, int priority)
        {
            InitialStatus = Initial_Status;
            FinalStatus = Final_Status;
            CaseDescription = case_Description;
            Reset = reset;
            Priority = priority;
        }
    }

    /// <summary>Mouse Input Data </summary>
    public class PasswordBoxMouseDragData : PasswordBoxInputData
    {
        /// <summary>Index for mouse down</summary>
        public int MouseStartIndex;
        /// <summary>Index for mosue up</summary>
        public int MouseEndIndex;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="case_Description"></param>
        /// <param name="StartIndex"></param>
        /// <param name="EndIndex"></param>
        /// <param name="Initial_Status"></param>
        /// <param name="Final_Status"></param>
        /// <param name="reset"></param>
        /// <param name="priority"></param>
        public PasswordBoxMouseDragData(string case_Description, int StartIndex, int EndIndex, PasswordBoxStatus Initial_Status, PasswordBoxStatus Final_Status, bool reset, int priority)
            : base(case_Description, Initial_Status, Final_Status, reset, priority)
        {
            MouseStartIndex = StartIndex;
            MouseEndIndex = EndIndex; 
        }
    }

    /// <summary>PasswordBox status</summary>
    public class PasswordBoxStatus
    {
        /// <summary>Password</summary>
        public string Password;
        /// <summary>Selection start position</summary>
        public int SelectionStart;
        /// <summary>Selection length</summary>
        public int SelectionLength;
        /// <summary>Password char</summary>
        public char PasswordChar;
        /// <summary>Max length of password can be inputed</summary>
        public int MaxLength; 
        /// <summary>
        /// PasswordBoxStatus constructor
        /// </summary>
        /// <param name="passwordValue"></param>
        /// <param name="selection_Start"></param>
        /// <param name="selection_Length"></param>
        /// <param name="password_Char"></param>
        /// <param name="max_Length"></param>
        public PasswordBoxStatus(string passwordValue, int selection_Start, int selection_Length, char password_Char, int max_Length)   
        {
            Password = passwordValue; 
            SelectionStart = selection_Start;
            SelectionLength = selection_Length;
            PasswordChar = password_Char;
            MaxLength = max_Length;
        }
    }

    #endregion
}