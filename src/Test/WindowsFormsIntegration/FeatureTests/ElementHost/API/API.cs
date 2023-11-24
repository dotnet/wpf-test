using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using SWF = System.Windows.Forms;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

//
// Testcase:    API
// Description: Verify Property Mapping API for Element Host.
// Author:      a-rickyt
//
public class API : ReflectBase
{
    #region Test case setup

    ElementHost elementHost1;
    SWC.Button avButton1;

    public API(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        this.Text = "API Test";
        this.Size = new System.Drawing.Size(400, 400);
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        this.Controls.Clear();
        this.Text = "API Test";
        this.Font = null;

        avButton1 = new SWC.Button();
        avButton1.Content = "Avalon Button";

        //Create Element Host 1
        elementHost1 = new ElementHost();
        elementHost1.BackColor = Color.Red;
        elementHost1.Child = avButton1;
        Controls.Add(elementHost1);

        return base.BeforeScenario(p, scenario);
    }

    #endregion

    #region Scenarios

    [Scenario("PropertyMap returns ElementHostPropertyMap.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        Utilities.SleepDoEvents(20);

        sr.IncCounters("System.Windows.Forms.Integration.ElementHostPropertyMap", 
            elementHost1.PropertyMap.ToString(), "Failed at PropertyMap returns ElementHostPropertyMap.", p.log);

        return sr;
    }

    [Scenario("Can add new property with eh.PropertyMap.Add")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //add a property that is not mapped by default
        elementHost1.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));

        //change the property on the Host and verify the correct behaviour 
        //(MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.CausesValidation = !elementHost1.CausesValidation;
        Utilities.SleepDoEvents(20);

        //Verify that the new property was added and its translator called
        Verify(p, sr, elementHost1, "CausesValidation", elementHost1.CausesValidation.ToString());

        return sr;
    }

    [Scenario("Can add extend PT by using eH.PropertyMap")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        elementHost1.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);

        //change the property on the Host and verify the correct behaviour 
        //(MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.CausesValidation = !elementHost1.CausesValidation;
        Utilities.SleepDoEvents(20);

        //Verify that the property was extended and its translator called
        Verify(p, sr, elementHost1, "CausesValidation", elementHost1.CausesValidation.ToString());

        return sr;
    }

    [Scenario("Can replace PT eh.PropertyMap.Add")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        //replace the Font property
        elementHost1.PropertyMap.Remove("Font");
        elementHost1.PropertyMap.Add("Font", new PropertyTranslator(MyPropertyTranslator));

        //change the property on the Host and verify the correct behaviour 
        //(MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Font = new System.Drawing.Font("Tahoma", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);

        //Verify that the property was replaced and the right translator called
        Verify(p, sr, elementHost1, "Font", elementHost1.Font.ToString());

        return sr;
    }

    [Scenario("Can remove existing property with Remove method.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        // ---- Try removing a custom mapping ---- //        

        //add a custom mapping for a property that hasn't got a default mapping and then remove it
        elementHost1.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));
        elementHost1.PropertyMap.Remove("CausesValidation");

        //change the property on the Host and verify the correct behaviour 
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.CausesValidation = !elementHost1.CausesValidation;
        Utilities.SleepDoEvents(20);

        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after the mapping was removed", p.log);

        // ---- Now try the same things with an default mapped property ---- //

        //remove the default mapping for the Font-property
        elementHost1.PropertyMap.Remove("Font");

        //change the property on the Host's parent and verify the correct behaviour
        PropertyTranslatorInfo.GetInstance().Reset();
        this.Font = new System.Drawing.Font("Impact", this.Font.Size);
        Utilities.SleepDoEvents(20);

        sr.IncCounters(avButton1.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after the mapping was removed", p.log);

        //change the property on the Host and verify the correct behaviour
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Font = new System.Drawing.Font("Tahoma", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);

        sr.IncCounters(avButton1.FontFamily.ToString() != "Tahoma",
            "Default PropertyTranslator was called after the mapping was removed", p.log);
        return sr;
    }

    [Scenario("Calling Reset on non default property removes that property.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        elementHost1.PropertyMap.Remove("Font");
        elementHost1.PropertyMap.Add("Font", new PropertyTranslator(MyPropertyTranslator));
        elementHost1.PropertyMap.Reset("Font");

        //verify if the property got reset
        this.Font = new System.Drawing.Font("Impact", this.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(avButton1.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        elementHost1.Font = new System.Drawing.Font("Tahoma", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(avButton1.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        // --- Try to reset a default property mapping on which we've chained a custom mapping --- //

        //chain our translator to the Font mapping and reset-it
        elementHost1.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);
        elementHost1.PropertyMap.Reset("Font");

        //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Font = new System.Drawing.Font("Comic Sans MS", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after a Reset", p.log);
        sr.IncCounters(avButton1.FontFamily.ToString() == "Comic Sans MS",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);

        // --- Try to reset a default property mapping that was replaced --- //

        //replace the default translator for Font-property with our translator 
        elementHost1.PropertyMap.Remove("Font");
        elementHost1.PropertyMap.Add("Font", new PropertyTranslator(MyPropertyTranslator));
        elementHost1.PropertyMap.Reset("Font");

        //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Font = new System.Drawing.Font("Times New Roman", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after a Reset", p.log);
        sr.IncCounters(avButton1.FontFamily.ToString() == "Times New Roman",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);

        return sr;
    }

    [Scenario("Calling Reset on default property should not affect anything.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            elementHost1.PropertyMap.Reset("Dock");
            sr.IncCounters(true, "", p.log);
        }
        catch (Exception ex)
        {
            sr.IncCounters(false, "Reset shouldn't throw an exception. Exception: " + ex.Message, p.log);
        }
        return sr;
    }

    [Scenario("Calling ResetAll removes all non default properties.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // - remove the default mapping for the Visible
        // - chain a new delegate to the default-mapped Font
        // - replace the default mapping for Enabled property
        // - add a custom mapping for Dock (not mapped by default)
        elementHost1.PropertyMap.Remove("RightToLeft");
        elementHost1.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);
        elementHost1.PropertyMap.Remove("Enabled");
        elementHost1.PropertyMap.Add("Enabled", new PropertyTranslator(MyPropertyTranslator));
        elementHost1.PropertyMap.Add("Dock", new PropertyTranslator(MyPropertyTranslator));

        //Now reset all
        elementHost1.PropertyMap.ResetAll();

        //verify the correct behaviour for the Visible (the default translator should be invoked)
        elementHost1.RightToLeft = SWF.RightToLeft.Yes;
        Utilities.SleepDoEvents(20);
        sr.IncCounters(avButton1.FlowDirection == System.Windows.FlowDirection.RightToLeft,
                        "Default PropertyTranslator wasn't called after it was restored", p.log);
        elementHost1.RightToLeft = SWF.RightToLeft.No;

        //verify the correct behaviour for the Font (the default translator should be invoked and the custom shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Font = new System.Drawing.Font("Impact", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after a Reset", p.log);
        sr.IncCounters(avButton1.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);

        //verify the correct behaviour for the IsEnabled property (the default formater should be invoked and the custom shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Enabled = false;
        Utilities.SleepDoEvents(20);
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after a Reset", p.log);
        sr.IncCounters(avButton1.IsEnabled == false,
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);
        elementHost1.Enabled = true;

        //verify the correct behaviour for the Dock property (my custom shouldn't get called)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Dock = SWF.DockStyle.Left;
        Utilities.SleepDoEvents(20);
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after a Reset", p.log);

        return sr;
    }

    [Scenario("Calling Clear Removes all props.")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //add a custom mapping for the Dock property 
        elementHost1.PropertyMap.Add("Dock", new PropertyTranslator(MyPropertyTranslator));

        //clear all mappings 
        elementHost1.PropertyMap.Clear();

        //verify that the custom mapping got removed 
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Dock = SWF.DockStyle.Top;
        Utilities.SleepDoEvents(20);
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was called after the mapping was removed", p.log);

        //verify that the default mapping for Font was removed
        this.Font = new System.Drawing.Font("Impact", this.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(avButton1.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after it was removed", p.log);

        //try to reset the Font (to see if reset works after ClearAll)
        elementHost1.PropertyMap.Reset("Font");

        elementHost1.Font = new System.Drawing.Font("Times New Roman", elementHost1.Font.Size);
        Utilities.SleepDoEvents(20);
        sr.IncCounters(avButton1.FontFamily.ToString() == "Times New Roman",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        return sr;
    }

    [Scenario("BackColorTransparent true/false.")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        Controls.Clear();

        //Create Element Host 2
        ElementHost elementHost2 = new ElementHost();
        Controls.Add(elementHost2);

        //Verify initial state
        sr.IncCounters(false, elementHost2.BackColorTransparent, "Failed at BackColorTransparent", p.log);
        sr.IncCounters(System.Drawing.SystemColors.Control, elementHost2.BackColor, 
            "Incorrect initial state", p.log);
        Utilities.SleepDoEvents(20);

        this.BackColor = Color.Blue;

        //Verify after parent's backcolor is changed
        sr.IncCounters(false, elementHost2.BackColorTransparent, "Failed at BackColorTransparent", p.log);
        sr.IncCounters(Color.Blue, elementHost2.BackColor,
            "BackColor did not match parent's backcolor.", p.log);
        Utilities.SleepDoEvents(20);

        elementHost2.BackColorTransparent = true;
        sr.IncCounters(elementHost2.BackColorTransparent == true, p.log, BugDb.WindowsOSBugs, 1598431,
            "Setting BackColorTransparent to true does not work.");
        sr.IncCounters(Color.Blue, elementHost2.BackColor,
            "BackColorTransparent was not set to true.", p.log);
        Utilities.SleepDoEvents(20);

        elementHost2.BackColorTransparent = false;
        sr.IncCounters(false, elementHost2.BackColorTransparent, "Failed at BackColorTransparent", p.log);
        sr.IncCounters(Color.Blue, elementHost2.BackColor,
            "BackColorTransparent was not set to false.", p.log);
        Utilities.SleepDoEvents(20);

        return sr;
    }

    [Scenario("Virtual method OnPropertyChanged for properties that do not provide property change notification.")]
    public ScenarioResult Scenario11(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //add a property that that do not provide property change notification
        elementHost1.PropertyMap.Add("Name", new PropertyTranslator(MyPropertyTranslator));

        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost1.Name = "Element Host 1";
        Utilities.SleepDoEvents(20);

        //Verify initial behavior without OnPropertyChanged
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
            "MyPropertyTranslator was called.", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Host == null,
            "Host exists in PropertyTranslator", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Value == null,
            "Invalid Value-param in PropertyTranslator", p.log);

        elementHost1.OnPropertyChanged("Name", "Element Host 1");

        //Verify OnPropertyChanged works
        Verify(p, sr, elementHost1, "Name", elementHost1.Name.ToString());

        return sr;
    }

    [Scenario("Verify PropertyMappingError event.")]
    public ScenarioResult Scenario12(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        elementHost1.PropertyMap.PropertyMappingError += 
            new EventHandler<PropertyMappingExceptionEventArgs>(PropertyMap_PropertyMappingError);

        elementHost1.PropertyMap["BackColor"] += delegate 
            { throw new ArgumentOutOfRangeException("firstParam", "Don't set a ridiculous value"); };

        Utilities.SleepDoEvents(20);

        sr.IncCounters("PropertyMappingError", this.Text, "PropertyMappingError event was not fired.", p.log);

        return sr;
    }

    void PropertyMap_PropertyMappingError(object sender, PropertyMappingExceptionEventArgs e)
    {
        this.Text = "PropertyMappingError";
    }

    #endregion

    #region Utilities

    //Helper function that verifies the mapping mechanism behaves as expected.  The property values of
    //PropertyTranslatorInfo are compared against the expected values:
    //1.  Verify that the PropertyTranslator was called.
    //2.  Verify that the PropertyTranslator has the correct host (i.e. it’s not null).
    //3.  Verify that the PropertyTranslator's value is not null.
    //4.  Verify that the PropertyTranslator's host matches the expected host.
    //5.  Verify that the PropertyTranslator's name matches the expected property name.
    //6.  Verify that the PropertyTranslator's value matches the expected value.
    private void Verify(TParams p, ScenarioResult sr, object expectedHost, string expectedPropName, string expectedValue)
    {
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().CallFlag,
            "MyPropertyTranslator wasn't called", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Host != null,
            "Invalid Host in PropertyTranslator", p.log);

        //for the cases when Value should be null .. add an overload for this without the 
        //expectedValue-param in the signature. 
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Value != null,
            "Invalid Value-param in PropertyTranslator", p.log);

        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Host, expectedHost,
            "Invalid host parameter in PropertyTranslator", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Name, expectedPropName,
            "Invalid PropName parameter in PropertyTranslator", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Value.ToString(),
            expectedValue, "Invalid Value parameter in PropertyTranslator", p.log);
    }

    private void MyPropertyTranslator(object host, String propertyName, object value)
    {
        //write down info into the PropertyTranslatorInfo
        PropertyTranslatorInfo singleInstance = PropertyTranslatorInfo.GetInstance();
        singleInstance.CallFlag = true;
        singleInstance.Host = host;
        singleInstance.Name = propertyName;
        singleInstance.Value = value;

        this.Text = propertyName + " has been added.";
    }

    //PropertyTranslatorInfo is a helper class used to store the values of properties of the 
    //PropertyTranslator, so that they can be used to verify that the PropertyMapping mechanism
    //works correctly.  It uses the Singleton pattern to ensure that only one instance of the class
    //exists.
    class PropertyTranslatorInfo
    {
        private object host = null;
        private string name = null;
        private object val = null;
        private bool callFlag = false; //host==null doesn't necessarily mean that the PropertyTranslator was not called... so we need this.    

        //Singleton design pattern
        private static PropertyTranslatorInfo instance = new PropertyTranslatorInfo();
        private PropertyTranslatorInfo()
        {
        }
        public static PropertyTranslatorInfo GetInstance()
        {
            return instance;
        }

        public object Host
        {
            get { return host; }
            set { host = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public object Value
        {
            get { return val; }
            set { val = value; }
        }
        public bool CallFlag
        {
            get { return callFlag; }
            set { callFlag = value; }
        }

        public void Reset()
        {
            host = null;
            name = null;
            val = null;
            callFlag = false;
        }
    }

    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ PropertyMap returns ElementHostPropertyMap.
//@ Can add new property with eh.PropertyMap.Add("Clip", new PropertyTranslator(OnClipChange));
//@ Can add extend PT by using eH.PropertyMap["ExistingProperty"] += new PropertyTranslator(MyPT); Save existing translator and call it in the MyPT function.
//@ Can replace PT eh.PropertyMap.Add("Clip", new PropertyTranslator(OnClipChange));  Don't call existing PT.
//@ Can remove existing property with Remove method.
//@ Calling Reset on non default property removes that property.
//@ Calling Reset on default property should not affect anything.
//@ Calling ResetAll removes all non default properties.
//@ Calling Clear Removes all props.
//@ BackColorTransparent true/false.
//@ Virtual method OnPropertyChange for properties that do not provide property change notification.
//@ Verify PropertyMappingError event.