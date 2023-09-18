// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Security;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    
    #endregion Namespaces.

    /// <summary>
    /// this class contains code for testing the passwordbox.
    ///</summary>
    [Test(0, "PasswordBox", "PasswordBoxBVTTest", MethodParameters = "/TestCaseType=PasswordBoxBVTTest /Case=NoPasswordLeak")]
    [TestOwner("Microsoft"), TestTactics("428")]
    public class PasswordBoxBVTTest : PasswordBoxBase
    {
        #region case - No Password leaked from public APIs 
        /// <summary>No Password leaked from public APIs.</summary>
        public void NoPasswordLeak()
        {
            PasswordBox pBox = _wraper1.Element as PasswordBox;
            pBox.PasswordChanged +=new RoutedEventHandler(PasswordChanged);
            pBox.Password = "MySpecialPasswordIsUnique";
        }

        void PasswordChanged(object o, RoutedEventArgs args)
        {
            LeakDetector(o, 0);
            LeakDetector(args, 0);
            QueueDelegate(EndTest);
        }

        void LeakDetector(object obj, int count)
        {
            //pass condition
            if (null==obj || count > 3 || (obj is string && !((string)obj).Contains("MySpecialPasswordIsUnique")))
                return;
            //failed condition.
            if(obj.ToString().Contains("MySpecialPasswordIsUnique"))
            {
                pass = false;
                MyLogger.Log(" - Failed: object " + obj.GetType().FullName + "Contains the password[MySpecialPasswordIsUnique]");
                return;
            }

            //Check the public  and static field
            System.Reflection.FieldInfo[] FInfos = obj.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (System.Reflection.FieldInfo fInfo in FInfos)
            {
                LeakDetector(fInfo.GetValue(obj), count + 1);
            }
            //check all the public properties.
            System.Reflection.PropertyInfo[] pInfos = obj.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo pInfo in pInfos)
            {
                if (pInfo.CanRead && pInfo.ReflectedType.IsPublic)
                {
                    try
                    {
                        //We will ingnore the Password Property at on passwordBox.
                        if (pInfo.Name != "Password")
                        {
                            LeakDetector(pInfo.GetValue(obj, null), count + 1);
                        }
                    }
                    //for those that need specific parameter, we ignore it.
                    catch (Exception e)
                    {
                        MyLogger.Log("We will ignored this excpetion: " + e.Message);
                        MyLogger.Log("Please ingore the above excpetion message!!!");
                    }
                }
            }
            //check all the returned value for public and static.
            //We only care about the DependencyObject. 
            if (obj is DependencyObject)
            {
                System.Reflection.MethodInfo[] MInfos = obj.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                foreach (System.Reflection.MethodInfo mInfo in MInfos)
                {
                    //If a method need parameter we don't know how to specify.
                    if (mInfo.GetParameters().Length == 0)
                    {
                        LeakDetector(mInfo.Invoke(obj, null), count + 1);
                    }
                }
            }
        }
        
        #endregion 
        
        #region case -UndoRedoBVT
        /// <summary>UndoRedoBVT</summary>
        public void UndoRedoBVT()
        {
            Verifier.Verify(false, "This cases is not implement since undo/redo is does not work yet");
        }
            #endregion 

        #region case - MouseSelection
        /// <summary>MouseSelection inside passwordbox</summary>
        public void MouseSelection()
        {
            Verifier.Verify(false, "This cases is not implement since commandsBVT is does not work yet");
        }
        #endregion 

        #region case - commandsBVT
        /// <summary>test commands in passwordBox. 1)copy, 2)Paste, 3)Cut </summary>
        public void commandsBVT_CaseStart()
        {
			EnterFuction("commandsBVT_CaseStart");
			Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper2.Element, "Password", BuildSecureString(""));
            MouseInput.MouseClick(_wraper2.Element);
           // PasswordChar = (char)Test.Uis.Utils.ReflectionUtils.GetProperty(_wraper2.Element, "PasswordChar");
            EndFunction();
           QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(commandsBVT_Paste));
        }

        /// <summary>Paste things into PasswordBox </summary>
        void commandsBVT_Paste()
        {
            EnterFuction("commandsBVT_Paste");
            System.Windows.Clipboard.SetDataObject("defg");
            KeyboardInput.TypeString("^v");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(commandsBVT_MakeSelection));
        }

        /// <summary>Make a selection for copy </summary>
        void commandsBVT_MakeSelection()
        {
            EnterFuction("commandsBVT_MakeSelection");
            KeyboardInput.TypeString("{END}+{LEFT 2}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(commandsBVT_Copy));
        }

        /// <summary>Copy</summary>
        void commandsBVT_Copy()
        {
            EnterFuction("commandsBVT_Copy");
            KeyboardInput.TypeString("^c");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(commandsBVT_Cut));
        }

        /// <summary>Cut</summary>
        void commandsBVT_Cut()
        {
            EnterFuction("commandsBVT_Cut");

            string str = System.Windows.Clipboard.GetDataObject().GetData(typeof(string)) as string;

            if (str != "defg")
            {
                MyLogger.Log(CurrentFunction + " - Failed: Clipboard text is changed!!! expected: [defg] Actual: [" + str + "]");
                pass = false;
            }
            KeyboardInput.TypeString("^x");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(commandsBVT_Done));
        }

        /// <summary>Done</summary>
        void commandsBVT_Done()
        {
            EnterFuction("commandsBVT_Done");

            string str = System.Windows.Clipboard.GetDataObject().GetData(typeof(string)) as string;

            //when Regression_Bug109 is fixed, we will enable the following line.
            if (str != "defg")
            {
                MyLogger.Log(CurrentFunction + " - Failed: Clipboard text is changed!!! expected: [defg] Actual: [" + str + "]");
                pass = false;
            }
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }

        #endregion 
        
        #region case - KeyBoardInputBVT
        /// <summary>
        /// This case tests the basic KeyBoard input for password.
        /// </summary>
        public void KeyBoardInputBVT_CaseStart()
        {
			EnterFuction("KeyBoardInputBVT_CaseStart");
			Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper2.Element, "Password", BuildSecureString(""));
            MouseInput.MouseClick(_wraper2.Element);
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_TypeSomething));
        }

        /// <summary>TypeSomething</summary>
        void KeyBoardInputBVT_TypeSomething()
        {
            EnterFuction("KeyBoardInputBVT_TypeSomething");

            //type things in.
            KeyboardInput.TypeString("xyz~~3239{backspace 2}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_KeySelect));
        }

        /// <summary>KeySelect</summary>
        void KeyBoardInputBVT_KeySelect()
        {
            EnterFuction("KeyBoardInputBVT_KeySelect");
            KeyboardInput.TypeString("+{LEFT 2}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_DeleteSelection));
        }

        /// <summary>DeleteSelection</summary>
        void KeyBoardInputBVT_DeleteSelection()
        {
            EnterFuction("KeyBoardInputBVT_DeleteSelection");
            KeyboardInput.TypeString("{Delete}{Left 2}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_DeleteKey));
        }

        /// <summary>DeleteKey</summary>
        void KeyBoardInputBVT_DeleteKey()
        {
            EnterFuction("KeyBoardInputBVT_DeleteKey");
            KeyboardInput.TypeString("{Delete 2}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_HomeKey));
        }

        /// <summary>HomeKey</summary>
        void KeyBoardInputBVT_HomeKey()
        {
            EnterFuction("KeyBoardInputBVT_HomeKey");
            KeyboardInput.TypeString("{HOME}+{LEFT}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_EndKey));
        }

        /// <summary>EndKey</summary>
        void KeyBoardInputBVT_EndKey()
        {
            EnterFuction("KeyBoardInputBVT_EndKey");
            KeyboardInput.TypeString("{END}+{RIGHT}+{Left 2}^i^b^u{end}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(KeyBoardInputBVT_EndCase));
        }

        /// <summary>EndCase</summary>
        void KeyBoardInputBVT_EndCase()
        {
            EnterFuction("KeyBoardInputBVT_EndCase");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }
        #endregion 

        #region case - PasswordBoxOMBVT
        /// <summary>
        /// This case test the basic funtionality of PasswordBoxOM.
        /// </summary>
        public void PasswordBoxOMBVT_CaseStart()
        {
			EnterFuction("PasswordBoxOMBVT_CaseStart");
            MouseInput.MouseClick(_wraper1.Element);
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_Password));
        }

        /// <summary>BVT test password property</summary>
        void PasswordBoxOMBVT_Password()
        {
            EnterFuction("PasswordBoxOMBVT_Password");
            ReflectionUtils.SetProperty(_wraper1.Element, "Password", BuildSecureString("abc"));
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_PasswordChar));
        }

        /// <summary> BVT test PasswordChar property</summary>
        void PasswordBoxOMBVT_PasswordChar()
        {
            EnterFuction("PasswordBoxOMBVT_PasswordChar");
            //PasswordChar = (char)ReflectionUtils.GetProperty(_wraper1.Element, "PasswordChar");
            //PasswordVerifier(_wraper1, 0, "abc");
            //PasswordChar = '?';
            ReflectionUtils.SetProperty(_wraper1.Element, "PasswordChar", '*');

            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_SelectAll));
        }

        /// <summary>BVT Test SelectAll() method</summary>
        void PasswordBoxOMBVT_SelectAll()
        {
            EnterFuction("PasswordBoxOMBVT_SelectAll");

            Test.Uis.Utils.ReflectionUtils.InvokePropertyOrMethod(null, "SelectAll", new object[] { _wraper1.Element }, InvokeType.InstanceMethod);

            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_Paste));
        }

        /// <summary>BVT test Paste() method </summary>
        void PasswordBoxOMBVT_Paste()
        {
            EnterFuction("PasswordBoxOMBVT_Paste");
            
            System.Windows.Clipboard.SetDataObject("defg");

            //we will directly access the API, the class become public
            Test.Uis.Utils.ReflectionUtils.InvokePropertyOrMethod(null, "Paste", new object[] { _wraper1.Element }, InvokeType.InstanceMethod);

            string str = ReflectionUtils.GetProperty(_wraper1.Element, "Password") as string;

            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_Select));
        }

        /// <summary>BVT test select(int, int) method</summary>
        void PasswordBoxOMBVT_Select()
        {
            EnterFuction("PasswordBoxOMBVT_Select");

            //we will directly access the API, the class become public
            Test.Uis.Utils.ReflectionUtils.InvokePropertyOrMethod(null, "Select", new object[] { _wraper1.Element, 1, 2}, InvokeType.InstanceMethod);
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_Clear));
        }

        /// <summary>Test clear() method </summary>
        void PasswordBoxOMBVT_Clear()
        {
            EnterFuction("PasswordBoxOMBVT_Clear");

            //PasswordVerifier(_wraper1, 2, "defg");

            Type tp = _wraper1.Element.GetType();
            System.Reflection.MethodInfo mInfo = tp.GetMethod("Clear");

            //we will directly access the API, the class become public
            Test.Uis.Utils.ReflectionUtils.InvokePropertyOrMethod(null, "Clear", new object[] { _wraper1.Element}, InvokeType.InstanceMethod);

            Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper1.Element, "MaxLength", 10);
            KeyboardInput.TypeString("abcdefghijklmn1234567890");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxOMBVT_MaxLength));
        }
        
        /// <summary>Test MaxLength property</summary>
        void PasswordBoxOMBVT_MaxLength()
        {
            int L = (int)Test.Uis.Utils.ReflectionUtils.GetProperty(_wraper1.Element, "MaxLength");
            if (L != 10)
            {
                MyLogger.Log(CurrentFunction + " - Failed: MaxLength should be: 10, Actual: " + L.ToString());
                pass = false;
            }
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }
        #endregion 

        #region case - PasswordBoxRenderingBVT

        /// <summary>
        /// This test will test the rendering of the PasswordBox control.
        /// it makes sure that the true password will not displayed.
        /// </summary>
        public void PasswordBoxRenderingBVT_UndoDevelpment()
        {
			EnterFuction("PasswordBoxRenderingBVT_UndoDevelpment");
            MouseInput.MouseClick(_wraper2.Element);
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxRenderingBVT_SetContent));
        }

        /// <summary>Set content to all boxes </summary>
        void PasswordBoxRenderingBVT_SetContent()
        {
            EnterFuction("PasswordBoxRenderingBVT_SetContent");
            Sleep();

            Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper1.Element, "PasswordChar", '*');
            Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper2.Element, "PasswordChar", '*');
            Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper1.Element, "Password", "abcdef");
            Test.Uis.Utils.ReflectionUtils.SetProperty(_wraper2.Element, "Password", "");

            //1)    type "abcdef" into passwordBox2 
            //2)    type "******" to textBox
            //3)    Set Caret to the tempTextBox.
            KeyboardInput.TypeString("abcdef{tab}" + RepeatString( "*", 6) + "{tab}");
            EndFunction();
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PasswordBoxRenderingBVT_ImageCompare));
        }

        /// <summary>take pictures and do comparing
        /// </summary>
        void PasswordBoxRenderingBVT_ImageCompare()
        {
//            EnterFuction("PasswordBoxRenderingBVT_ImageCompare");
//            Sleep();
//
//            System.Drawing.Bitmap Bmp1 = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_wraper1.Element);
//            System.Drawing.Bitmap Bmp2 = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(_wraper2.Element);
//            //System.Drawing.Bitmap Bmp3 = Microsoft.Test.Imaging.BitmapCapture.CreateBitmapFromElement(textBox);
//            System.Drawing.Bitmap bmp4 = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(Bmp1, new Rect(2, 2, 200, 68));
//            System.Drawing.Bitmap bmp5 = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(Bmp2, new Rect(2, 2, 200, 68));
//           // System.Drawing.Bitmap bmp6 = Microsoft.Test.Imaging.BitmapUtils.CreateSubBitmap(Bmp3, new Rect(2, 2, 200, 68));
//            System.Drawing.Bitmap bmp7, bmp8;
//            bool b1, b2;
//
//            b1 = Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(bmp4, bmp5, out bmp7);
//            b2 = Microsoft.Test.Imaging.ComparisonOperationUtils.AreBitmapsEqual(bmp4, bmp6, out bmp8);
//            MyLogger.LogImage(bmp4, "Passwordbox1Img");
//            MyLogger.LogImage(bmp5, "Passwordbox2Img");
//            MyLogger.LogImage(bmp6, "textBoxImg");
//            if (!b1)
//                MyLogger.LogImage(bmp7, "DiffP1AndP2Img");
//
//            if (!b2)
//                MyLogger.LogImage(bmp8, "DiffP1AndTextBoxImg");
//
//            Verifier.Verify(b1, CurrentFunction + " - Failed: Image difference between PasswordBox1 and PasswordBox2 is found!!! Please see file: DiffP1AndP2Img");
//            Verifier.Verify(b2, CurrentFunction + " - Failed: Image difference between PasswordBox1 and textBox is found!!! Please see file: DiffP1AndTextBoxImg");
//            EndFunction();
//            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(EndTest));
        }
        #endregion 
        
    #region case - Regression_Bug110 - check to see if caret is in the passwordbox when Listbox is in the page.
    /// <summary>
    /// Regression_Bug110
    /// </summary>
    public void Regression_Bug110_CaseStart()
    {
        EnterFunction("Regression_Bug110_CaseStart");
        _wraper1 = new Test.Uis.Wrappers.UIElementWrapper(new PasswordBox());
        ListBox listBox = new ListBox();
        Panel panel = new Canvas();

        panel.Background = Brushes.Violet;
        MainWindow.Content = panel;
        ((IAddChild)panel).AddChild(_wraper1.Element);
        ((IAddChild)panel).AddChild(listBox);
        ((PasswordBox)(_wraper1.Element)).Width = 250;
        ((PasswordBox)(_wraper1.Element)).Height = 70;
        listBox.Width = 150;
        listBox.Height = 100;
        listBox.Items.Add(new ListBoxItem());
        listBox.Items.Add(new ListBoxItem());
        Canvas.SetLeft(_wraper1.Element, 200);
        Canvas.SetTop(_wraper1.Element, 200);
        Canvas.SetLeft(listBox, 20);
        Canvas.SetTop(listBox, 180);
        ((PasswordBox)(_wraper1.Element)).FontSize = 50;
        _wraper1 = new Test.Uis.Wrappers.UIElementWrapper(_wraper1.Element);
        QueueDelegate(Regression_Bug110_MouseClick);
    }

    void Regression_Bug110_MouseClick()
    {
        MouseInput.MouseClick(_wraper1.Element);
        QueueDelegate(Regression_Bug110_Done);
    }

    void Regression_Bug110_Done()
    {
        System.Drawing.Bitmap elementBitmap;    // Colored image.
        System.Drawing.Bitmap bw;               // Black and white image.
        double caretWidth;                      // Expected width of caret.
        System.Drawing.Bitmap img;
        caretWidth = SystemParameters.CaretWidth;
        using (elementBitmap = BitmapCapture.CreateBitmapFromElement(_wraper1.Element))
        using (img = BitmapUtils.CreateBorderlessBitmap(elementBitmap, 2))
        using (bw = BitmapUtils.ColorToBlackWhite(img))
        {
            System.Drawing.Rectangle rectangle;
            pass = BitmapUtils.GetTextCaret(bw, out rectangle);
            if (!pass)
                MyLogger.LogImage(bw, "Regression_Bug110_img");
        }
        base.Init();
        QueueDelegate(EndTest);
    }
    #endregion
}
}
