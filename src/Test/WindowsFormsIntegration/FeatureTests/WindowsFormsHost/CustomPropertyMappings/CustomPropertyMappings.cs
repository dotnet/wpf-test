// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms.Integration;


//
// Testcase:    CustomPropertyMappings
// Description: Test whether or not custom property mappings work
// Abstract:   On every scenario, we should verify the correct behavior for a mapping by changing the mapped property 
//             on the WFH and on an ancestor (for WFH properties that are inherited from the parent if not explicitly set) . 
//           - Were a delegate should be called (scenarios 1, 3, 4, 5, 7, 9) verify the parameters received (host, propertyName, value).
//
namespace WindowsFormsHostTests
{
    class PropertyTranslatorInfo
    {
        private object _host = null;
        private string _propName = null;
        private object _value = null;
        private bool _callFlag = false; //m_host==null doesn't necessarily mean that the PropertyTranslator was not called... so we need this.    

        public object Host
        {
            get { return _host; }
            set { _host = value; }
        }
        public string PropName
        {
            get { return _propName; }
            set { _propName = value; }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public bool CallFlag
        {
            get { return _callFlag; }
            set { _callFlag = value; }
        }  

        public void Reset()
        {
            _host = null;
            _propName = null;
            _value = null;
            _callFlag = false;
        }

        //singleton stuff
        private static PropertyTranslatorInfo s_instance = new PropertyTranslatorInfo();
        private PropertyTranslatorInfo()
        {
        }
        public static PropertyTranslatorInfo GetInstance()
        {
            return s_instance;
        }
    }

    public class CustomPropertyMappings : WPFReflectBase
    {
        // class vars
        private static DockPanel s_dp;
        private static Button s_avBtn;
        private static WindowsFormsHost s_wfh;
        private static System.Windows.Forms.Button s_wfBtn;
        
        #region Testcase setup
        
        public CustomPropertyMappings(string[] args) : base(args)
        { 
        }
        
        protected override void InitTest(TParams p)
        {
            // hacks to get window to show up !!!
            this.Topmost = true;
            this.Topmost = false;
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            // need to call base.BeforeScenario before accessing "this"
            bool b = base.BeforeScenario(p, scenario);

            // set up app for tests
            SetupMainWindow();
            this.Title = currentScenario.Name;

            return b;
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        [Scenario("Add a new property mapping to a WFH. Verify the correct behavior.")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            //add a property that is not mapped by default (using Add syntax)
            s_wfh.PropertyMap.Add("SnapsToDevicePixels", new PropertyTranslator(MyPropertyTranslator));

            //change the property on the Host's parent and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_dp.SnapsToDevicePixels = !s_dp.SnapsToDevicePixels;
            MyPause();
            if( !PropertyTranslatorInfo.GetInstance().CallFlag )
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "SnapsToDevicePixels", s_dp.SnapsToDevicePixels.ToString());
            }

            //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.SnapsToDevicePixels = !s_wfh.SnapsToDevicePixels;
            MyPause();
            if( !PropertyTranslatorInfo.GetInstance().CallFlag )
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "SnapsToDevicePixels", s_wfh.SnapsToDevicePixels.ToString());
            }

            //add a property that doesn't exist on the WindowsFormsHost and verify ArgumentException is thrown
            try
            {
                s_wfh.PropertyMap.Add("", new PropertyTranslator(MyPropertyTranslator));
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one. ");
            }

            //put a null in the first param for the AddMethod. 
            try
            {
                s_wfh.PropertyMap.Add(null, new PropertyTranslator(MyPropertyTranslator));
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentNullException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one. ");
            }

            //Try to map the ToolTip property to an null translator and verify that ArgumentNullException is thown
            try
            {
                s_wfh.PropertyMap.Add("ToolTip", null);
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentNullException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception e)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one. " + e.Message);
            }

            return sr;
        }

        [Scenario("Remove a property mapping. Try this with a previously added custom-mapping, and with a default mapping. Verify the correct behavior. ")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            // ---- try removing a custom mapping ---- //        
            
            //add a custom mapping for a property that hasn't got a default mapping and then remove-it
            s_wfh.PropertyMap.Add("SnapsToDevicePixels", new PropertyTranslator(MyPropertyTranslator));
            s_wfh.PropertyMap.Remove("SnapsToDevicePixels");

            //change the property on the Host's parent and verify the correct behaviour 
            PropertyTranslatorInfo.GetInstance().Reset();
            s_dp.SnapsToDevicePixels = !s_dp.SnapsToDevicePixels;
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after the mapping was removed");
    
            //change the property on the Host and verify the correct behaviour
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.SnapsToDevicePixels = !s_wfh.SnapsToDevicePixels;
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after the mapping was removed");

            // ---- Now try the same things with an default mapped property ---- //

            //remove the default mapping for the FontFamily-property
            s_wfh.PropertyMap.Remove("FontFamily");

            //change the property on the Host's parent and verify the correct behaviour
            PropertyTranslatorInfo.GetInstance().Reset();
            this.FontFamily = new FontFamily("Garamond");   //!! this will persist in the next scenarios. 
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name != "Garamond", 
                                    "Default PropertyTranslator was called after the mapping was removed");
            
            //change the property on the Host and verify the correct behaviour
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.FontFamily = new FontFamily("Tahoma");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name != "Tahoma",
                                    "Default PropertyTranslator was called after the mapping was removed");

            // ---- Try to pass some invalid params into the Remove method ---- //

            try
            {
                s_wfh.PropertyMap.Remove("");
                WPFMiscUtils.IncCounters(sr, p.log, true, "");
            }
            catch (Exception e)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Remove should not throw exceptions when the PropertyName is invalid. " + e.Message);
            }

            try
            {
                s_wfh.PropertyMap.Remove(null);
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentNullException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one. " + ex.Message);
            }

            return sr;
        }

        [Scenario("Try to Remove a property that wasn't mapped. Nothing should happen, no exception should be thrown.")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);
                
            try
            {
                s_wfh.PropertyMap.Remove("ToolTip");
                WPFMiscUtils.IncCounters(sr, p.log, true, "");
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Remove shouldn't throw an exception");
            }

            return sr;
        }

        [Scenario("Replace an existing property mapping. Verify the correct behavior.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            //replace the FontFamily property
            s_wfh.PropertyMap.Remove("FontFamily");
            s_wfh.PropertyMap.Add("FontFamily", new PropertyTranslator(MyPropertyTranslator));

            //change the property on the Host's parent and verify the correct behaviour (MyPropertyTranslator is called with the proper params and the default translator is not invoked anymore)
            PropertyTranslatorInfo.GetInstance().Reset();
            this.FontFamily = new FontFamily("Garamond");
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "FontFamily", this.FontFamily);
            }
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name != "Garamond",
                                    "Default PropertyTranslator was called after the mapping was removed");


            //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.FontFamily = new FontFamily("Tahoma"); 
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "FontFamily", s_wfh.FontFamily);
            }
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name != "Tahoma",
                                    "Default PropertyTranslator was called after the mapping was removed");

            return sr;
        }

        [Scenario("Chain an additional mapping delegate. Verify the correct behavior. Verify that the additional delegate get's called after the existing delegates. ")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            // ---- Try to chain an additional mapping for an already mapped property ---- //

            //add an additional mapping for FontFamily property 
            s_wfh.PropertyMap["FontFamily"] += new PropertyTranslator(MyPropertyTranslator);

            //change the property on the Host's parent and verify the correct behaviour (the default translator and MyPropertyTranslator are called)
            PropertyTranslatorInfo.GetInstance().Reset();
            this.FontFamily = new FontFamily("Impact");
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "FontFamily", this.FontFamily);
            }
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name == "Impact",
                                    "Default PropertyTranslator wasn't called");


            //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.FontFamily = new FontFamily("Tahoma");
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "FontFamily", s_wfh.FontFamily);
            }
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name == "Tahoma",
                                    "Default PropertyTranslator wasn't called");

            // ---- Try to chain an additional mapping for a property that wasn't mapped before ---- //

            //add an additional mapping for a property that wasn't mapped previously (SnapsToDevicePixels)
            s_wfh.PropertyMap["SnapsToDevicePixels"] += new PropertyTranslator(MyPropertyTranslator);

            //change the property on the Host's parent and verify the correct behaviour (MyPropertyTranslator is called)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_dp.SnapsToDevicePixels = !s_dp.SnapsToDevicePixels;
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "SnapsToDevicePixels", s_dp.SnapsToDevicePixels.ToString());
            }
            
            //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.SnapsToDevicePixels = !s_wfh.SnapsToDevicePixels;
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "SnapsToDevicePixels", s_wfh.SnapsToDevicePixels.ToString());
            }
        
            
            // ---- Test the chaining syntax with weird params 
            
            //use a property that doesn't exist on the WindowsFormsHost and verify ArgumentException is thrown
            try
            {
                s_wfh.PropertyMap[""] += new PropertyTranslator(MyPropertyTranslator);
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one");
            }

            //put a null in the first param for the AddMethod and verify ArgumentNullException is thrown
            try
            {
                s_wfh.PropertyMap[null] += new PropertyTranslator(MyPropertyTranslator);
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentNullException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one");
            }

            //Try to map the tool-tip property to an null translator and verify that ArgumentNullException is thown
            try
            {
                s_wfh.PropertyMap["ToolTip"] += null;
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentNullException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one");
            }

            return sr;
        }

        [Scenario("Verify that you can map two properties on the same delegate.")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            //map the ToolTip and the FontFamily property to MyPropertyTranslator
            s_wfh.PropertyMap["ToolTip"] += new PropertyTranslator(MyPropertyTranslator);
            s_wfh.PropertyMap["FontFamily"] += new PropertyTranslator(MyPropertyTranslator);

            //change the ToolTip property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.ToolTip = "HostToolTip"; 
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "ToolTip", "HostToolTip");
            }

            //change the FontFamily property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params and the default mapping is called)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.FontFamily = new FontFamily("Impact");
            MyPause();
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "FontFamily", s_wfh.FontFamily);
            }
            WPFMiscUtils.IncCounters(sr, p.log,
                            s_wfBtn.Font.FontFamily.Name == "Impact",
                            "Default PropertyTranslator wasn't called");

            return sr;
        }

        [Scenario("Verify the return value for PropertyMap['SomeProperty'] when there's no mapping for the 'SomeProperty'")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            try
            {
                PropertyTranslator pt = s_wfh.PropertyMap["ToolTip"];
                WPFMiscUtils.IncCounters(sr, p.log, pt == null, "Invalid PropertyTranslator");
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Unexpected exception was thrown");
            }

            return sr;
        }

        [Scenario("Verify that the property-mapping is executed when it's added (and reseted for the default mappings)")]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            //change the ToolTip on the host
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.ToolTip = "HostToolTip";
            MyPause();

            //map the ToolTip property to my custom PropertyTranslator
            s_wfh.PropertyMap.Add("ToolTip", new PropertyTranslator(MyPropertyTranslator));
            MyPause();

            //verify if the custom PropertyTranslator was called (with the proper args)
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "ToolTip", "HostToolTip");
            }

            //remove the ToolTip mapping
            s_wfh.PropertyMap.Remove("ToolTip");

            //re-add the ToolTip mapping with += syntax - verify that the translator got called with the correct params 
            s_wfh.PropertyMap["ToolTip"] += new PropertyTranslator(MyPropertyTranslator);
            if (!PropertyTranslatorInfo.GetInstance().CallFlag)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "MyPropertyTranslator wasn't called");
            }
            else
            {
                VerifyMapping(p, sr, s_wfh, "ToolTip", "HostToolTip");
            }

            //remove a default-mapping 
            s_wfh.PropertyMap.Remove("FontFamily");
            
            //change the font
            this.FontFamily = new FontFamily("Tahoma");
            
            //reset the default mapping and verify that the default translator got called 
            s_wfh.PropertyMap.Reset("FontFamily");
            WPFMiscUtils.IncCounters(sr, p.log,
                            s_wfBtn.Font.FontFamily.Name == "Tahoma",
                            "Default PropertyTranslator wasn't called after the mapping was reseted");

            return sr;
        }

        [Scenario("Clear all mappings and verify the correct behavior.")]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            //add a custom mapping for the ToolTip property 
            s_wfh.PropertyMap.Add("ToolTip", new PropertyTranslator(MyPropertyTranslator));

            //clear all mappings 
            s_wfh.PropertyMap.Clear();

            //verify that the custom mapping got removed 
            PropertyTranslatorInfo.GetInstance().Reset();
            s_dp.ToolTip = "ParentToolTip";
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after the mapping was removed");
        
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.ToolTip = "HostToolTip";
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after the mapping was removed");

            //verify that the default mapping for FontFamily was removed
            this.FontFamily = new FontFamily("Garamond");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.FontFamily.Name != "Garamond",
                    "Default PropertyTranslator was called after it was removed");

            s_wfh.FontFamily = new FontFamily("Tahoma");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.FontFamily.Name != "Tahoma",
                    "Default PropertyTranslator was called after it was removed");

            //try to reset the FontFamily (to see if reset works after ClearAll)
            s_wfh.PropertyMap.Reset("FontFamily");
            
            s_wfh.FontFamily = new FontFamily("Times New Roman");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.FontFamily.Name == "Times New Roman",
                    "Default PropertyTranslator wasn't called after it was restored");

            return sr;
        }

        [Scenario("Reset a property-mapping and verify that the default mapping is restored.")]
        public ScenarioResult Scenario10(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            // --- try to reset a default property mapping that was Removed --- //

            //remove the property-mapping for the FontFamily and after that - reset
            s_wfh.PropertyMap.Remove("FontFamily");
            s_wfh.PropertyMap.Reset("FontFamily");

            //verify if the property got reseted
            this.FontFamily = new FontFamily("Impact");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.FontFamily.Name == "Impact",
                    "Default PropertyTranslator wasn't called after it was restored");

            s_wfh.FontFamily = new FontFamily("Tahoma");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.FontFamily.Name == "Tahoma",
                    "Default PropertyTranslator wasn't called after it was restored");

            // --- try to reset a default property mapping on which we've chained a custom mapping --- //

            //chain our translator to the FontFamily mapping and reset-it
            s_wfh.PropertyMap["FontFamily"] += new PropertyTranslator(MyPropertyTranslator);
            s_wfh.PropertyMap.Reset("FontFamily");

            //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.FontFamily = new FontFamily("Comic Sans MS");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was call after a Reset ");
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name == "Comic Sans MS",
                                    "Default PropertyTranslator wasn't called after it was reseted ");

            // ---- Try to pass some invalid strings into the Reset method ---- //

            try
            {
                s_wfh.PropertyMap.Reset("AAAA");
                WPFMiscUtils.IncCounters(sr, p.log, true, "");
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Reset should not throw exceptions when the PropertyName is invalid");
            }

            try
            {
                s_wfh.PropertyMap.Reset(null);
                WPFMiscUtils.IncCounters(sr, p.log, false, "Did not get expected exception");
            }
            catch (ArgumentNullException ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Got exception, but not right one");
            }

            return sr;
        }

        [Scenario("Try to Reset a property that has no default mapping. Verify the correct behaviour.")]
        public ScenarioResult Scenario11(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);
            try
            {
                s_wfh.PropertyMap.Reset("ToolTip");
                WPFMiscUtils.IncCounters(sr, p.log, true, "");
            }
            catch (Exception)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Reset shouldn't throw an exception");
            }
            
            return sr;
        }

        [Scenario("ResetAll property mappings. Verify correct behavior.")]
        public ScenarioResult Scenario12(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);
            
            // - remove the default mapping for the FontSize
            // - chain a new delegate to the default-mapped FontFamily
            // - replace the default mapping for IsEnabled property
            // - add a custom mapping for ToolTip (not mapped by default)
            s_wfh.PropertyMap.Remove("FontSize");
            s_wfh.PropertyMap["FontFamily"] += new PropertyTranslator(MyPropertyTranslator);
            s_wfh.PropertyMap.Remove("IsEnabled");
            s_wfh.PropertyMap.Add("IsEnabled", new PropertyTranslator(MyPropertyTranslator));
            s_wfh.PropertyMap.Add("ToolTip", new PropertyTranslator(MyPropertyTranslator));
            
            //Now reset all
            s_wfh.PropertyMap.ResetAll();

            //verify the correct behaviour for the FontSize (the default translator should be invoked)
            float wfBtnFontSize = s_wfBtn.Font.Size;
            this.FontSize = this.FontSize + 1;
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.Size > wfBtnFontSize,
                    "Default PropertyTranslator wasn't called after it was restored");
            wfBtnFontSize = s_wfBtn.Font.Size;

            s_wfh.FontSize = s_wfh.FontSize + 1;
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log,
                    s_wfBtn.Font.Size > wfBtnFontSize,
                    "Default PropertyTranslator wasn't called after it was restored");

            //verify the correct behaviour for the FontFamily (the default formater should be invoked and the custom shouldn't)
            PropertyTranslatorInfo.GetInstance().Reset();
            this.FontFamily = new FontFamily("Impact");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after a Reset");
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name == "Impact",
                                    "Default PropertyTranslator wasn't called after it was reseted");

            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.FontFamily = new FontFamily("Tahoma");
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was call after a Reset ");
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Font.FontFamily.Name == "Tahoma",
                                    "Default PropertyTranslator wasn't called after it was reseted ");

            //verify the correct behaviour for the IsEnabled property (the default formater should be invoked and the custom shouldn't)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_dp.IsEnabled = false;
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after a Reset");
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Enabled == false,
                                    "Default PropertyTranslator wasn't called after it was reseted");
            s_dp.IsEnabled = true;

            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.IsEnabled = false;
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was call after a Reset ");
            WPFMiscUtils.IncCounters(sr, p.log,
                                    s_wfBtn.Enabled == false,
                                    "Default PropertyTranslator wasn't called after it was reseted ");
            s_wfh.IsEnabled = true;

            //verify the correct behaviour for the ToolTip property (my custom shouldn't get called)
            PropertyTranslatorInfo.GetInstance().Reset();
            s_dp.ToolTip = "ParentToolTip";
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was called after a Reset");
            
            PropertyTranslatorInfo.GetInstance().Reset();
            s_wfh.ToolTip = "HostToolTip";
            MyPause();
            WPFMiscUtils.IncCounters(sr, p.log, !PropertyTranslatorInfo.GetInstance().CallFlag, "MyPropertyTranslator was call after a Reset ");
            
            return sr;
        }

        [Scenario("Map a property to a delegate that throws an exception and verify the correct behavior.")]
        public ScenarioResult Scenario13(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);
            
            //add the property-mapping - this will trigger the evil delegate
            try
            {
                //new behaviour - propertyMap will raise and event when the translator throws.
                s_wfh.PropertyMap.PropertyMappingError += new EventHandler<PropertyMappingExceptionEventArgs>(PropertyMap_PropertyMappingError);

                //map the ToolTip property to the delegate that throws an exception
                s_wfh.PropertyMap["ToolTip"] += new PropertyTranslator(MyEvilPropertyTranslator);

                MyPause();
                //if we are here - the exception that was thrown from the delegate vanished
                WPFMiscUtils.IncCounters(sr, p.log, false, "What happened with the exception that was throw inside the delegate??");
            }
            catch (Exception ex)
            {
                WPFMiscUtils.IncCounters(sr, p.log, true, ex.Message);
            }
            return sr;
        }

        #endregion


        #region Property Translator Functions

        // simple Property Translator that simply records that it was called
        public void MyPropertyTranslator(object host, string propName, object value)
        {
            //write down info into the PropertyTranslatorInfo
            PropertyTranslatorInfo singleInstance = PropertyTranslatorInfo.GetInstance();
            singleInstance.CallFlag = true;
            singleInstance.Host = host;
            singleInstance.PropName = propName;
            singleInstance.Value = value;
        }

        // simple Property Translator that throws an exception
        public void MyEvilPropertyTranslator(object host, string propName, object value)
        {
            throw new Exception("Have a Nice Day");
        }

        #endregion

        #region Helper functions

        // set up the (Avalon) main window by creating new Dockpanel, WFH and other controls
        private void SetupMainWindow()
        {
            //reset the font on the form - the scenarios will alter the fontFamily and we want to start with a fresh dialog
            this.FontFamily = new FontFamily("Verdana");

            // create dockpanel, add to window
            s_dp = new DockPanel();
            this.Content = s_dp;

            // add Avalon button for comparison
            s_avBtn = new Button();
            s_avBtn.Content = "Avalon Button";
            s_avBtn.Width = 300;
            s_dp.Children.Add(s_avBtn);

            // create WFH control, add to panel
            s_wfh = new WindowsFormsHost();
            s_dp.Children.Add(s_wfh);

            // create hosted WF control, add to WFH
            s_wfBtn = new System.Windows.Forms.Button();
            s_wfBtn.Text = "WinForms Button";
            s_wfBtn.Size = new System.Drawing.Size(200, 100);
            s_wfh.Child = s_wfBtn;

            MyPause();
        }

        //Helper function that Compares the values of PropertyTranslatorInfo-properties against the expected values and 
        //decides if the mapping mechanism behaved as expected
        // We'll have overloads for different types of Values. 
        private void VerifyMapping(TParams p, ScenarioResult sr, object expectedHost, string expectedPropName, string expectedValue)
        {
            if (PropertyTranslatorInfo.GetInstance().Host == null)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Invalid Host in PropertyTranslator");
                return;
            }
            if (PropertyTranslatorInfo.GetInstance().Value == null)
            {
                //for the cases when Value should be null .. add an overload for this without the expectedValue-param in the signature. 
                WPFMiscUtils.IncCounters(sr, p.log, false, "Invalid Value-param in PropertyTranslator");
                return;
            }

            WPFMiscUtils.IncCounters(sr, PropertyTranslatorInfo.GetInstance().Host, expectedHost,
                                    "Invalid host parameter in PropertyTranslator", p.log);
            WPFMiscUtils.IncCounters(sr, PropertyTranslatorInfo.GetInstance().PropName, expectedPropName,
                                    "Invalid PropName parameter in PropertyTranslator", p.log);
            WPFMiscUtils.IncCounters(sr, PropertyTranslatorInfo.GetInstance().Value.ToString(), expectedValue,
                                    "Invalid Value parameter in PropertyTranslator", p.log);
        }
        private void VerifyMapping(TParams p, ScenarioResult sr, object expectedHost, string expectedPropName, FontFamily expectedValue)
        {
            if (PropertyTranslatorInfo.GetInstance().Host == null)
            {
                WPFMiscUtils.IncCounters(sr, p.log, false, "Invalid Host in PropertyTranslator");
                return;
            }
            if (PropertyTranslatorInfo.GetInstance().Value == null)
            {
                //for the cases when Value should be null .. add an overload for this without the expectedValue-param in the signature. 
                WPFMiscUtils.IncCounters(sr, p.log, false, "Invalid Value-param in PropertyTranslator");
                return;
            }

            FontFamily receivedValue = PropertyTranslatorInfo.GetInstance().Value as FontFamily;

            WPFMiscUtils.IncCounters(sr, PropertyTranslatorInfo.GetInstance().Host, expectedHost,
                                    "Invalid host parameter in PropertyTranslator", p.log);
            WPFMiscUtils.IncCounters(sr, PropertyTranslatorInfo.GetInstance().PropName, expectedPropName,
                                    "Invalid PropName parameter in PropertyTranslator", p.log);
            WPFMiscUtils.IncCounters(sr, receivedValue, expectedValue,
                                    "Invalid Value parameter in PropertyTranslator", p.log);
        }

        void PropertyMap_PropertyMappingError(object sender, PropertyMappingExceptionEventArgs e)
        {
            e.ThrowException = true;
        }

        private static void MyPause()
        {
            // comment out following line if get "Invalid window handle" exception !!!
            WPFReflectBase.DoEvents();
            //System.Threading.Thread.Sleep(500);
        }
        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Add a new property mapping to a WFH. Verify the correct behavior.
//@ Remove a property mapping. Try this with a previously added custom-mapping, and with a default mapping. Verify the correct behavior. 
//@ Try to Remove a property that wasn't mapped. Nothing should happen, no exception should be thrown.
//@ Replace an existing property mapping. Verify the correct behavior.
//@ Chain an additional mapping delegate. Verify the correct behavior. Verify that the additional delegate get's called after the existing delegates. 
//@ Verify that you can map two properties on the same delegate.
//@ Verify the return value for PropertyMap['SomeProperty'] when there's no mapping for the 'SomeProperty'
//@ Verify that the property-mapping is executed when it's added (and reseted for the default mappings) 
//@ Clear all mappings and verify the correct behavior.
//@ Reset a property-mapping and verify that the default mapping is restored.
//@ Try to Reset a property that has no default mapping. Verify the correct behaviour.
//@ ResetAll property mappings. Verify correct behavior.
//@ Map a property to a delegate that throws an exception and verify the correct behavior.