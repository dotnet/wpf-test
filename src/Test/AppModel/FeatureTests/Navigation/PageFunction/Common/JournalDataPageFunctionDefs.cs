// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*                                                                                   *
*  Description:                                                                     *
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
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
   /*****************************************************\
   *                                                      *
   *  PAGEFUNCTION DEFINITIONS FOR JOURNALDATA TESTCASES  *
   *                                                      *
   \******************************************************/
   
   /*****************************************\
   \*****************************************/


   /****************************\
   REFTYPE_PFRET
   \****************************/   
   public class PersistedRefPageFuncObj: PageFunction<REFTYPE_PFRET> /*, IJnlData */
   {
        public PersistedRefPageFuncObj () {
            _nstartcount=0;
            _eventseq = "defctor";
            /////_nconstruction = 0;
            _persistdata1 = new Point(20,20);
            _persistdata2 = 14;
            _creationtime = DateTime.Now;
            
            DockPanel dproot = new DockPanel();
            dproot.Background = Brushes.LightGreen;
            _elem = new TextBlock();
            dproot.Children.Add(_elem);
            _elem.Text = _persistedelementdata 
                        + " >>> Created at: " + _creationtime.ToString();
            Content = dproot;
            NavigationHelper.Output ("Creating object of type PersistedRefPageFuncObj");
            
            if (_navapp.Properties[PageFunctionTestApp.SAPPSEQ] == null) {
                _navapp.Properties[PageFunctionTestApp.SAPPSEQ] = "";
            }
            
            string seq = (string)(_navapp.Properties[PageFunctionTestApp.SAPPSEQ]);
            seq += "*" + PageFunctionTestApp.APPSEQ_VPFDEFCTOR;
            _navapp.Properties[PageFunctionTestApp.SAPPSEQ] += seq;

        }
        
        
        public PersistedRefPageFuncObj(DateTime dt) 
        {
            _nstartcount=0;
            _eventseq = "ctor";
            /////_nconstruction = 0;
            _persistdata1 = new Point(20,20);
            _persistdata2 = 14;
            _creationtime = dt;
            
            DockPanel dproot = new DockPanel();
            dproot.Background = Brushes.LightGreen;
            _elem = new TextBlock();
            dproot.Children.Add(_elem);
            _elem.Text = _persistedelementdata 
                        + " >>> Created at: " + _creationtime.ToString();
            Content = dproot;
            NavigationHelper.Output("Creating object of type PersistedRefPageFuncObj");

            if (_navapp.Properties[PageFunctionTestApp.SAPPSEQ] == null) {
                _navapp.Properties[PageFunctionTestApp.SAPPSEQ] = "";
            }

            string seq = (string)(_navapp.Properties[PageFunctionTestApp.SAPPSEQ]);
            seq += "*" + PageFunctionTestApp.APPSEQ_VPFCTOR;
            _navapp.Properties[PageFunctionTestApp.SAPPSEQ] += seq;
            
        }
        
        protected override void Start() 
        {
            NavigationHelper.Output("Start called on Persisted PF");
            _nstartcount++;    
            _eventseq += "_start";
            
            string seq = (string)(_navapp.Properties[PageFunctionTestApp.SAPPSEQ]);
            seq += "*" + PageFunctionTestApp.APPSEQ_VPFSTART;
            _navapp.Properties[PageFunctionTestApp.SAPPSEQ] += seq;
        }
        
        /// <summary>
        /// Does visual verification, a call to this method 
        /// must be using the UiContext Item
        /// </summary>
        public bool CheckState() 
        {
             if (!Bag.VisualVerifyClientArea(
                                        "PersistedRefPageFuncObj_Loose.bmp",
                                        0,
                                        70,
                                        50,
                                        50,
                                        PageFunctionTestApp.CurrentPFTestApp.MainNavWindow
                                        )) 
            {
                NavigationHelper.Fail("Visual Verif of PersistedRefPageFuncObj (IJnlData implemented) failed");
                return false;
            }
                        
            return true;
        }
        
        public void NavChild(
                        NavigationWindow navigator, 
                        bool fRmvFromJnl /*, 
                        string discriminant */
                        ) 
        {
            if (navigator == null) 
            {
                NavigationHelper.Fail("Navigator is null");
                return;
            }
            
            //PageFunction<bool> pfChild = null;
            
            PersistedValPageFuncBool pfChild = new PersistedValPageFuncBool ();
            if (fRmvFromJnl) {
                pfChild.RemoveFromJournal = true;
            } else {
                pfChild.RemoveFromJournal = false;
            }
            
            NavigationHelper.Output("Navigating to child");
            navigator.Navigate (pfChild);
        }
        
        
        #region IJnlData implementation
        byte[] /*IJnlData.*/Save() 
        {

            NavigationHelper.Output("Persisting to Journal -- IJnlData.Save");
            NavigationHelper.Output("Persisting PF state on Navigation");

            int ncount = (int)_navapp.Properties[PageFunctionTestApp.VPFSAVECALLED];
            _navapp.Properties[PageFunctionTestApp.VPFSAVECALLED] = ++ncount;
            
            
            string seq = (string)(_navapp.Properties[PageFunctionTestApp.SAPPSEQ]);
            seq += "*" + PageFunctionTestApp.APPSEQ_VPFSAVE;
            _navapp.Properties[PageFunctionTestApp.SAPPSEQ] = seq;
            

            // Notice that we change the Text of the TextBlock Control when serializing.
            _elem.Text = "CHANGED_TEXT";
            
            object [] saveddata = new object[] {
                (object) this._creationtime, 
                (object) _persistdata1,
                (object) _persistdata2,
                (object) _elem.Text                
                };
                
            SavedPFData _store = new SavedPFData(saveddata);

            MemoryStream saveStream = new MemoryStream();
            new BinaryFormatter().Serialize(saveStream, _store);
            return saveStream.ToArray();
            
        }

        bool /*IJnlData.*/Load( byte [] bytes) 
        {
            NavigationHelper.Output("IJnlData.Load called on PF -- rehydrating");
            if (bytes != null) 
            {
                int ncount = (int)_navapp.Properties[PageFunctionTestApp.VPFLOADCALLED];
                _navapp.Properties[PageFunctionTestApp.VPFLOADCALLED] = ++ncount;
                
                //((string[])_navapp.Properties[PageFunctionTestApp.APPSEQ])[unsavedint++] = PageFunctionTestApp.APPSEQ_VPFLOAD;
                
                string seq = (string)(_navapp.Properties[PageFunctionTestApp.SAPPSEQ]);
                seq += "*" + PageFunctionTestApp.APPSEQ_VPFLOAD;
                _navapp.Properties[PageFunctionTestApp.SAPPSEQ] = seq;

                MemoryStream _ldstream = new MemoryStream(bytes);
                _ldstream.Seek(0,SeekOrigin.Begin);
                SavedPFData saveddata = (new BinaryFormatter()).Deserialize(_ldstream) as SavedPFData;
                
                if (saveddata == null) 
                {
                    NavigationHelper.Fail("Unable to retrieve serialized data from Journal");
                    return false;
                } else {
                }
                this._creationtime = (DateTime)(saveddata.PersistedData[0]);
                this._persistdata1 = (Point)(saveddata.PersistedData[1]);
                this._persistdata2 = (int)(saveddata.PersistedData[2]);
                
            } 
            else 
            {
                NavigationHelper.Fail("On call to Load, byte array to rehydrate from was null");
                return false;
            }
            return true;
        }

        ////public int PersistId 
        ////{
        ////    get 
        ////    {
        ////        return _persistid; // shouldn't be called
        ////    }
        ////    set 
        ////    {
        ////        _persistid = value;
        ////    }
        ////}
        #endregion
        
        ///// private int _nconstruction;
        private Point _persistdata1; // ref persisted data        
        private int _persistdata2;   // value persisted data
        private DateTime _creationtime;
        private const string _persistedelementdata = "PERSISTED D"; 
        private int _nstartcount;    
        private string _eventseq;
        private TextBlock _elem;
        ////private int _persistid;
        
        private Application _navapp = Application.Current as Application;
   }
   
   
   /****************************\
   \****************************/
   public class PersistedValPageFuncBool : PageFunction<bool> //, Journal Serialization implementation
   {
        public PersistedValPageFuncBool() {
            NavigationHelper.Output ("Creating object of type PersistedValPageFuncBool");
            DockPanel dproot = new DockPanel();
            dproot.Background = Brushes.Silver;
            this.Content = dproot;
        }
        
        public bool CheckState() {
            if (!(this.Content is DockPanel)) {
                NavigationHelper.Fail("Child property of PersistedValPageFuncBool is incorrect");
                return false;
            }
            
            return true;
            
        }
        
        /////private int _nconstruction;
        /////private Point _persistdata1; // ref persisted data
        /////private int _persistdata2;   // value persisted data
        /////private DateTime _creationtime;
        /////private const string _persistedelementdata = "PERSISTED D"; 
        /////private int _nstartcount;    
        /////private string _eventseq;   
   }
   
   
   
   [Serializable]
   public class SavedPFData 
   {
       private object[] _storedobjs;

       public SavedPFData (object [] objs_to_be_stored) 
       {
           _storedobjs = objs_to_be_stored; 
       }

       public object[] PersistedData 
       {
           get 
           {
               return _storedobjs;
           }
       }
    }
    
   
}
