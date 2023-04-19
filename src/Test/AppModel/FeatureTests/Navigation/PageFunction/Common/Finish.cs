// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*      PageFunction return types test for pagefunctions of different types          *
*  Description:                                                                     *
*      Centralized location to test finish of pagefunctions of different types      *
*                                                                                   *
*      This testcase defines pagefunctions of 6 different classes including         *
*      reference and value object types. See comments below                         *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

#define use_tools
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Globalization;



namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{ 
    // NOTE: PageFunction <void> not allowed


    
    // Test for :
    // ref type generic pf 
    // value type generic pf
    
    // int 
    // bool
    // string
    // object
    // Your reference type
    // struct type (valuetype)
    // void pagefunction is not allowed, 
    //    so we piggyback it will object pf test, and return null
    
    // PageFunction PF_CLASS1 : PageFunction <int>
    // PageFunction PF_CLASS2 : PageFunction <bool>
    // PageFunction PF_CLASS3 : PageFunction <string>
    // PageFunction PF_CLASS4 : PageFunction <STRUCTTYPE_PFRET>
    // PageFunction PF_CLASS5 : new PageFunction <REFTYPE_PFRET>
    // PageFunction PF_CLASS6 : new PageFunction <object> and return null


//    public partial class {
//    
//    }
    
//    public enum PFType {
//        PF_CLASS1, //PageFunction <int>
//        PF_CLASS2, //PageFunction <bool>
//        PF_CLASS3, //PageFunction <string>
//        PF_CLASS4, //PageFunction <STRUCTTYPE_PFRET>
//        PF_CLASS5, //PageFunction <REFTYPE_PFRET>
//        PF_CLASS6  //PageFunction <object> // but really a void pf allowed
//    }
    
    public struct STRUCTTYPE_PFRET {
        public int intvalue;
        public string stringvalue;
        public Point objvalue;
    }
    
    public class REFTYPE_PFRET {
        private string _storedval;
        
        public string StoredVal {
            set 
            {
                _storedval = value;
            }
            
            get 
            {
                return _storedval;
            }
        }
    }
    
    class ElGenericPF 
    {
        //private const 
        public static FrameworkElement GeneratePage (int pftype) {
            Canvas cvsTop = new Canvas();
            double rectsize = 25;
            cvsTop.Name = "cvsTopGenericPF";
            cvsTop.Background = Brushes.White;
            Rectangle [] rects = new Rectangle[6];
            
            for (int i=0; i< rects.Length; ++i) {
                rects[i] = new Rectangle();
                rects[i].Width = rectsize;
                rects[i].Height = rectsize;
                Canvas.SetTop(rects[i], rectsize);
                Canvas.SetLeft(rects[i], 2 + (rectsize+10) * i );
                rects[i].Stroke = Brushes.DarkBlue;

                rects[i].StrokeThickness = 1;

                if (i == pftype) {
                    rects[i].Fill = Brushes.DarkBlue;
                } else {
                    rects[i].Fill = Brushes.White;
                }
                
                cvsTop.Children.Add(rects[i]);
            }
            
            return cvsTop;
            
        }
        
        public static bool VisualVerify(int pftype) 
        {
            bool retval = true;
            return retval;
        }
    }
    
    public partial class PageFunctionTestApp
    {
        NavigationWindow [] _thewindows;
        
        private void TestDiffPFTypes ()
        {            
            _thewindows = new NavigationWindow[6];
            Application.Current.LoadCompleted += 
                new LoadCompletedEventHandler(Load_TestDiffPFTypes);
            
            for (int i = 0; i < _thewindows.Length; ++i) 
            {
                // Position all the windows so they don't overlap
                // 2 rows, 3 each in a row
				_thewindows[i] = new NavigationWindow();
                _thewindows[i].Width = 250;
                _thewindows[i].Height = 250;
                _thewindows[i].Left = i % 3 * Math.Round(_thewindows[i].Width) + 10;
                _thewindows[i].Top = i / 3 * Math.Round(_thewindows[i].Height) + 10;
                _thewindows[i].Show();                
            }

            NavigationHelper.Output("Navigating all 6 windows to a default Launch PageFunction");
            
            _thewindows[0].Navigate(new FinishPFsLaunchParent());
            _thewindows[1].Navigate(new FinishPFsLaunchParent());
            _thewindows[2].Navigate(new FinishPFsLaunchParent());
            _thewindows[3].Navigate(new FinishPFsLaunchParent());
            _thewindows[4].Navigate(new FinishPFsLaunchParent());
            _thewindows[5].Navigate(new FinishPFsLaunchParent());
            
        }
        
        private void Load_TestDiffPFTypes(object sender, NavigationEventArgs e)
        {
            stage++;
            NavigationHelper.Output("Stage: " + stage.ToString());
            if (stage == 6) {
                NavigationHelper.Output("Navigating all 6 windows to child PageFunctions of different types");
                PF_CLASS1 pftype1child = new PF_CLASS1();
                PF_CLASS2 pftype2child = new PF_CLASS2();
                PF_CLASS3 pftype3child = new PF_CLASS3();
                PF_CLASS4 pftype4child = new PF_CLASS4();
                PF_CLASS5 pftype5child = new PF_CLASS5();
                PF_CLASS6 pftype6child = new PF_CLASS6();
                
                if (pftype1child == null || 
                    pftype2child == null || 
                    pftype3child == null || 
                    pftype4child == null || 
                    pftype5child == null || 
                    pftype6child == null)
                {
                    NavigationHelper.Fail("At least one of the 6 child pf types could not be created");
                }
                
                FinishPFsLaunchParent 
                    par1 = _thewindows[0].Content as FinishPFsLaunchParent, 
                    par2 = _thewindows[1].Content as FinishPFsLaunchParent,
                    par3 = _thewindows[2].Content as FinishPFsLaunchParent,
                    par4 = _thewindows[3].Content as FinishPFsLaunchParent,
                    par5 = _thewindows[4].Content as FinishPFsLaunchParent,
                    par6 = _thewindows[5].Content as FinishPFsLaunchParent;
                
                if (par1 == null || 
                    par2 == null || 
                    par3 == null || 
                    par4 == null || 
                    par5 == null || 
                    par6 == null)
                {
                    NavigationHelper.Fail("At least one of the 6 windows did not navigate to the " 
                        + "right parent pagefunction");
                }
                
                pftype1child.Return += 
                    new ReturnEventHandler<int> (par1.OnChildFinishClass1);
                pftype2child.Return += 
                    new ReturnEventHandler<bool> (par2.OnChildFinishClass2);
                pftype3child.Return += 
                    new ReturnEventHandler<string> (par3.OnChildFinishClass3);
                pftype4child.Return += 
                    new ReturnEventHandler<STRUCTTYPE_PFRET> (par4.OnChildFinishClass4);
                pftype5child.Return += 
                    new ReturnEventHandler<REFTYPE_PFRET> (par5.OnChildFinishClass5);
                pftype6child.Return += 
                    new ReturnEventHandler<object> (par6.OnChildFinishClass6);
                
                _thewindows[0].Navigate(pftype1child);
                _thewindows[1].Navigate(pftype2child);
                _thewindows[2].Navigate(pftype3child);
                _thewindows[3].Navigate(pftype4child);
                _thewindows[4].Navigate(pftype5child);
                _thewindows[5].Navigate(pftype6child);

                // we could ALSO have done                
//                  par1.NavChildWithinSameWindow(pftype1child);
//                  par2.NavChildWithinSameWindow(pftype2child);
//                  par3.NavChildWithinSameWindow(pftype3child);
//                  par4.NavChildWithinSameWindow(pftype4child);
//                  par5.NavChildWithinSameWindow(pftype5child);
//                  par6.NavChildWithinSameWindow(pftype6child);
            }
            
            if (stage == 12) {
                NavigationHelper.Output("Successfully navigated to the children pagefunctions of all six types");
                
                // post an item for visual verif
                NavigationHelper.Output("Currently foregoing the visual validation.");

                PF_CLASS1 pftype1child = _thewindows[0].Content as PF_CLASS1;
                PF_CLASS2 pftype2child = _thewindows[1].Content as PF_CLASS2;
                PF_CLASS3 pftype3child = _thewindows[2].Content as PF_CLASS3;
                PF_CLASS4 pftype4child = _thewindows[3].Content as PF_CLASS4;
                PF_CLASS5 pftype5child = _thewindows[4].Content as PF_CLASS5;
                PF_CLASS6 pftype6child = _thewindows[5].Content as PF_CLASS6;
                
                if (pftype1child == null || 
                    pftype2child == null || 
                    pftype3child == null || 
                    pftype4child == null || 
                    pftype5child == null || 
                    pftype6child == null)
                {
                    NavigationHelper.Fail("At least one of the 6 child pf types was not navigated to");
                }
                
                NavigationHelper.Output("Now Finishing the all the Child PageFunctions of different types");
                pftype1child.RequestFinish();                
                pftype2child.RequestFinish();                
                pftype3child.RequestFinish();                
                pftype4child.RequestFinish();                
                pftype5child.RequestFinish();                
                pftype6child.RequestFinish();                
                
            }
            
            if (stage == 18) 
            {
                FinishPFsLaunchParent 
                    par1 = _thewindows[0].Content as FinishPFsLaunchParent, 
                    par2 = _thewindows[1].Content as FinishPFsLaunchParent,
                    par3 = _thewindows[2].Content as FinishPFsLaunchParent,
                    par4 = _thewindows[3].Content as FinishPFsLaunchParent,
                    par5 = _thewindows[4].Content as FinishPFsLaunchParent,
                    par6 = _thewindows[5].Content as FinishPFsLaunchParent;
                
                if (par1 == null || 
                    par2 == null || 
                    par3 == null || 
                    par4 == null || 
                    par5 == null || 
                    par6 == null)
                {
                    NavigationHelper.Fail("At least one of the 6 windows did not navigate to the " 
                        + "right parent pagefunction after finishing the child pagefunction");
                }
                
//                if (par1.DoVerify_private_call_only_ID_12_1242_1_20_2004(typeof(PF_CLASS1)))
//                {
//                    NavigationHelper.Output("Finished testing return value of PF_CLASS1");
//                }
//                else
//                {
//                    NavigationHelper.Fail("Return value of PF_CLASS1 child was not correct");
//                }
                
                CheckRetVals_private_call_only_ID_12_1242_1_20_2004(par1, typeof(PF_CLASS1));
                CheckRetVals_private_call_only_ID_12_1242_1_20_2004(par2, typeof(PF_CLASS2));
                CheckRetVals_private_call_only_ID_12_1242_1_20_2004(par3, typeof(PF_CLASS3));
                CheckRetVals_private_call_only_ID_12_1242_1_20_2004(par4, typeof(PF_CLASS4));
                CheckRetVals_private_call_only_ID_12_1242_1_20_2004(par5, typeof(PF_CLASS5));
                CheckRetVals_private_call_only_ID_12_1242_1_20_2004(par6, typeof(PF_CLASS6));

                NavigationHelper.Pass(
                "Finished testing the return value of all the different " 
                + "classes of child PageFunctions");
                
                // Implicit shutdown
                for (int i = 0 ; i < _thewindows.Length; ++i) 
                {
                    _thewindows[i].Close();
                }

            }
            
        }
        
        
        private void CheckRetVals_private_call_only_ID_12_1242_1_20_2004 (
                                                            FinishPFsLaunchParent pf, 
                                                            Type t)
        {
            if (pf.DoVerify_private_call_only_ID_12_1242_1_20_2004 (t))
            {
                NavigationHelper.Output("Succeeded testing return value of " + t.ToString());
            }
            else
            {
                NavigationHelper.Fail("Return value of " + t.ToString() + " child was not correct");
            }        
        }
        
    }
    
    #region PageFunction Definitions for the Finish tests

	 public class PF_CLASS1 :  PageFunction <int>
     {
        public PF_CLASS1()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <int>");
            Content =  ElGenericPF.GeneratePage(0);           
        }
        
        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.ToString());
            ReturnEventArgs <int> retval = new ReturnEventArgs<int> ();
            retval.Result = 10;
            OnReturn(retval);
        }

        public override string ToString() 
        {
            return "PFCLASS1 [PageFunction <int>]";
        }
     }
     
	 public class PF_CLASS2 :  PageFunction <bool>
     {
        public PF_CLASS2()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <bool>");
            Content =  ElGenericPF.GeneratePage(1);           
        }
        
        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.ToString());
            ReturnEventArgs <bool> retval = new ReturnEventArgs<bool> ();
            retval.Result = true;
            OnReturn(retval);
        }

        public override string ToString() 
        {
            return "PFCLASS2 [PageFunction <bool>]";
        }

     }

	 public class PF_CLASS3 :  PageFunction <string>
     {
        public PF_CLASS3()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <string>");
            Content =  ElGenericPF.GeneratePage(2);           
        }

        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.ToString());
            ReturnEventArgs <string> retval = new ReturnEventArgs<string> ();
            retval.Result = "PF_MULTITYPE_FINISH_TEST";
            OnReturn(retval);
        }

        public override string ToString() 
        {
            return "PFCLASS3 [PageFunction <string>]";
        }
     }

	 public class PF_CLASS4 :  PageFunction <STRUCTTYPE_PFRET>
     {
        public PF_CLASS4()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <STRUCTTYPE_PFRET>");
            Content =  ElGenericPF.GeneratePage(3);           
        }

        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.ToString());

            ReturnEventArgs <STRUCTTYPE_PFRET> retval = new ReturnEventArgs<STRUCTTYPE_PFRET> ();
            STRUCTTYPE_PFRET returnarg = new STRUCTTYPE_PFRET();
            returnarg.intvalue = 5;
            returnarg.objvalue = new Point(15,15);
            returnarg.stringvalue = "RETURNED_STRUCT";
            retval.Result = returnarg;
            OnReturn(retval);
        }

        public override string ToString() 
        {
            return "PFCLASS4 [PageFunction <STRUCTTYPE_PFRET>]";
        }
     }

	 public class PF_CLASS5 :  PageFunction <REFTYPE_PFRET>
     {
        public PF_CLASS5()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <REFTYPE_PFRET>");
            Content =  ElGenericPF.GeneratePage(4);           
        }

        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.ToString());

            ReturnEventArgs <REFTYPE_PFRET> retval = new ReturnEventArgs<REFTYPE_PFRET> ();
            REFTYPE_PFRET returnarg = new REFTYPE_PFRET();
            returnarg.StoredVal = "RETURNED_REFTYPE";
            retval.Result = returnarg;
            OnReturn(retval);
        }

        public override string ToString() 
        {
            return "PFCLASS5 [PageFunction <REFTYPE_PFRET>]";
        }
     }

/* 
     **
     NOT ALLOWED 
     **
	 public class PF_CLASS6 :  PageFunction <void>
     {
        public PF_CLASS6()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <void>");
            Content =  ElGenericPF.GeneratePage(5);           
        }

        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.GetType().ToString());
        }

     }
*/    

     ///<summary>
     /// Since Void PageFunction is not allowed, we use an obj pf and return null
     ///</summary>
     public class PF_CLASS6 :  PageFunction <object>
     {
        public PF_CLASS6()
        {
            NavigationHelper.Output("Creating PF_CLASS of type PageFunction <void>");
            Content =  ElGenericPF.GeneratePage(5);           
        }

        public void RequestFinish()
        {
            NavigationHelper.Output("Finishing PageFunction of type: " 
            + this.ToString());

            ReturnEventArgs <object> retval = new ReturnEventArgs<object> ();
            retval.Result = null;
            OnReturn(retval);
        }
        
        public override string ToString() 
        {
            return "PFCLASS6 [PageFunction <object>]";
        }

     }
     
     public class FinishPFsLaunchParent :  PageFunction <int>
      {
         public FinishPFsLaunchParent()
         {
             NavigationHelper.Output("Creating FinishPFsLauncher ");
             DockPanel dp  = new DockPanel();
             dp.Background = Brushes.LightGreen;
             Content =  dp;           
         }

         public void RequestFinish()
         {
             NavigationHelper.Output("Cannot finish PageFunction of type: " 
             + this.ToString());
             
         }
         
         public void NavChildWithinSameWindow (FrameworkElement pfchild) 
         {
            if (pfchild is PF_CLASS1) 
            {
                ((PF_CLASS1)pfchild).Return += 
                    new ReturnEventHandler<int> (OnChildFinishClass1);                
            }
            else if (pfchild is PF_CLASS2) 
            {
                ((PF_CLASS2)pfchild).Return += 
                    new ReturnEventHandler<bool> (OnChildFinishClass2);
            }
            else if (pfchild is PF_CLASS3) 
            {
                ((PF_CLASS3)pfchild).Return += 
                    new ReturnEventHandler<string> (OnChildFinishClass3);
            }
            else if (pfchild is PF_CLASS4) 
            {
                ((PF_CLASS4)pfchild).Return += 
                    new ReturnEventHandler<STRUCTTYPE_PFRET> (OnChildFinishClass4);
            }
            else if (pfchild is PF_CLASS5) 
            {
                ((PF_CLASS5)pfchild).Return += 
                    new ReturnEventHandler<REFTYPE_PFRET> (OnChildFinishClass5);
            }
            else if (pfchild is PF_CLASS6) 
            {
                ((PF_CLASS6)pfchild).Return += 
                    new ReturnEventHandler<object> (OnChildFinishClass6);
            } 
            else 
            {
                NavigationHelper.Fail("The child element is not a pagefunction, which this test is not meant to handle.");
                return;
            }
            
            NavigationWindow _navigator = Window.GetWindow(this) as NavigationWindow;
            _navigator.Navigate(pfchild);
            
         }
         
         public void OnChildFinishClass1(object sender, ReturnEventArgs<int> args)
         {
            NavigationHelper.Output("Return event handler for child pf of type " + sender);
            _intreturnval = args.Result;
         }

         public void OnChildFinishClass2(object sender, ReturnEventArgs<bool> args)
         {
            NavigationHelper.Output("Return event handler for child pf of type "  + sender);
            _boolreturnval = args.Result;
         }

         public void OnChildFinishClass3(object sender, ReturnEventArgs<string> args)
         {
            NavigationHelper.Output("Return event handler for child pf of type "  + sender);
            _stringreturnval = args.Result;
         }

         public void OnChildFinishClass4(object sender, ReturnEventArgs<STRUCTTYPE_PFRET> args)
         {
            NavigationHelper.Output("Return event handler for child pf of type "  + sender);
            _structreturnval = args.Result;
         }

         public void OnChildFinishClass5(object sender, ReturnEventArgs<REFTYPE_PFRET> args)
         {
            NavigationHelper.Output("Return event handler for child pf of type "  + sender);
            _refreturnval = args.Result;
         }

         public void OnChildFinishClass6(object sender, ReturnEventArgs<object> args)
         {
            NavigationHelper.Output("Return event handler for child pf of type "  + sender);
            _objreturnval = args.Result;
         }

         public override string ToString() {
            return "Launcher PageFunction for Finish PF tests: Type FinishPFsLaunchParent [PageFunction <int>]"; 
         }
         
         internal bool DoVerify_private_call_only_ID_12_1242_1_20_2004(Type tChildType) 
         {
            if (tChildType == typeof(PF_CLASS1))
            {
                if (_intreturnval != 10)
                {
                    NavigationHelper.Fail("Return value from "  + tChildType.ToString() + " pagefunction was incorrect.");
                    return false;
                }
            }
            else if (tChildType == typeof(PF_CLASS2))
            {
                if (_boolreturnval != true)
                {
                    NavigationHelper.Fail("Return value from " + tChildType.ToString() + " pagefunction was incorrect.");
                }
                
            }
            else if (tChildType == typeof(PF_CLASS3))
            {
                if (String.Compare(
                        _stringreturnval,
                        "PF_MULTITYPE_FINISH_TEST",
                        false,                         // not case insensitive
                        CultureInfo.InvariantCulture) != 0
                   )
                {
                    NavigationHelper.Fail("Return value from " + tChildType.ToString() + " was incorrect.");
                }
                
            }
            else if (tChildType == typeof(PF_CLASS4))
            {
                if (_structreturnval.intvalue != 5 || 
                    String.Compare(
                        _structreturnval.stringvalue,"RETURNED_STRUCT",false,CultureInfo.InvariantCulture ) != 0 || 
                    _structreturnval.objvalue.X != 15 || 
                    _structreturnval.objvalue.Y != 15
                    )
                {
                    NavigationHelper.Fail("Return value from " + tChildType.ToString() + " pagefunction was incorrect.");
                }
                
            }
            else if (tChildType == typeof(PF_CLASS5))
            {
                if ( String.Compare(
                            _refreturnval.StoredVal,
                            "RETURNED_REFTYPE",
                            false,
                            CultureInfo.InvariantCulture) != 0 )
                {
                    NavigationHelper.Fail("Return value from " + tChildType.ToString() + " pagefunction was incorrect.");
                }
                
            }
            else if (tChildType == typeof(PF_CLASS6))
            {
                if (_objreturnval != null)
                {
                    NavigationHelper.Fail("Return value from " + tChildType.ToString() + " pagefunction was incorrect.");
                }
                
            }
            else
            {
                NavigationHelper.Fail("Incorrect call to this private method");
            }
            
            NavigationHelper.Output("Passed return value test from " + tChildType.ToString() );
            return true;
         }
         
         #region privates
            string _stringreturnval           = String.Empty;
            int    _intreturnval               = int.MinValue;
            bool   _boolreturnval             = false;
            STRUCTTYPE_PFRET _structreturnval;
            REFTYPE_PFRET    _refreturnval    = null;
            object           _objreturnval    = null;
         #endregion
     }
        
    #endregion

}
