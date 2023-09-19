// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.Integration;


// Testcase:    CustomPropertyMappings
// Description: Test whether or not custom property mappings work
public class CustomPropertyMappings : ReflectBase
{
    private Panel _mainPanel = null;
    private ElementHost _elementHost = null;
    private System.Windows.Controls.Button _avButton = null;

    #region Testcase setup
    public CustomPropertyMappings(string[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        InitializeControls();

        return base.BeforeScenario(p, scenario);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    [Scenario("Add a new property mapping to an EH. Verify the correct behavior.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);
        
        //add a property that is not mapped by default
        _elementHost.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));

        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.CausesValidation = !_elementHost.CausesValidation;
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "CausesValidation", _elementHost.CausesValidation.ToString());
        }

        return sr;
    }

    [Scenario("Try to add a mapping for an invalid property. Verify ArgumentException/ArgumentNullExcepion is throw.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //add a property that doesn't exist on the Host and verify ArgumentException is thrown
        try
        {
            _elementHost.PropertyMap.Add("", new PropertyTranslator(MyPropertyTranslator));
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception e)
        {
            p.log.LogException(e);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        }

        //put a null in the first param for the Add-method. 
        try
        {
            _elementHost.PropertyMap.Add(null, new PropertyTranslator(MyPropertyTranslator));
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentNullException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception e)
        {
            p.log.LogException(e);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        }

        return sr;
    }

    [Scenario("Try map a property to a null translator. Verify ArgumentNullExcepion is throw.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //Try to map some property to an null translator and verify that ArgumentNullException is thown
        try
        {
            _elementHost.PropertyMap.Remove("Visible");
            _elementHost.PropertyMap.Add("Visible", null);
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentNullException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception e)
        {
            p.log.LogException(e);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        }

        return sr;
    }

    [Scenario("Remove a property mapping. Try this with a previously added custom-mapping, and with a default mapping. Verify the correct behavior.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        // ---- try removing a custom mapping ---- //        

        //add a custom mapping for a property that hasn't got a default mapping and then remove-it
        _elementHost.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));
        _elementHost.PropertyMap.Remove("CausesValidation");

        //change the property on the Host and verify the correct behaviour 
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.CausesValidation = !_elementHost.CausesValidation;
        Application.DoEvents();

        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after the mapping was removed", 
                        p.log );

        // ---- Now try the same things with an default mapped property ---- //

        //remove the default mapping for the Font-property
        _elementHost.PropertyMap.Remove("Font");

        //change the property on the Host's parent and verify the correct behaviour
        PropertyTranslatorInfo.GetInstance().Reset();
        _mainPanel.Font = new System.Drawing.Font("Impact", _mainPanel.Font.Size);
        Application.DoEvents();

        sr.IncCounters( _avButton.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after the mapping was removed", 
                        p.log );

        //change the property on the Host and verify the correct behaviour
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Tahoma", _elementHost.Font.Size);
        Application.DoEvents();

        sr.IncCounters( _avButton.FontFamily.ToString() != "Tahoma",
                        "Default PropertyTranslator was called after the mapping was removed", 
                        p.log );

        return sr;
    }

    [Scenario("Try to Remove an invalid property. Verify the correct behavior")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //remove a property that doesn't exist on the Host. Nothing should be thrown
        try
        {
            _elementHost.PropertyMap.Remove("");
            sr.IncCounters(true, "", p.log);
        }
        catch (Exception e)
        {
            sr.IncCounters(false, "Remove should not throw exceptions when the PropertyName is invalid. " + e.Message, p.log);
        }

        //Remove(null) should throw ArgumentNullException
        try
        {
            _elementHost.PropertyMap.Remove(null);
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentNullException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception ex)
        {
            p.log.LogException(ex);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        } 

        return sr;
    }

    [Scenario("Try to Remove a property that wasn't mapped. Nothing should happen, no exception should be thrown.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        try
        {
            _elementHost.PropertyMap.Remove("CausesValidation");
            sr.IncCounters(true, "", p.log);
        }
        catch( Exception ex )
        {
            sr.IncCounters(false, "Remove shouldn't throw an exception. " + ex.Message, p.log);
        }

        return sr;
    }

    [Scenario("Replace an existing property mapping. Verify the correct behavior.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //replace the Font property
        _elementHost.PropertyMap.Remove("Font");
        _elementHost.PropertyMap.Add("Font", new PropertyTranslator(MyPropertyTranslator));

        //change the property on the Host's parent and verify the correct behaviour (MyPropertyTranslator is called with the proper params and the default translator is not invoked anymore)
        PropertyTranslatorInfo.GetInstance().Reset();
        _mainPanel.Font = new System.Drawing.Font("Impact", _mainPanel.Font.Size);
        Application.DoEvents();
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "Font", _mainPanel.Font);
        }
        sr.IncCounters( _avButton.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after the mapping was removed", p.log);


        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Tahoma", _elementHost.Font.Size);
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "Font", _elementHost.Font);
        }
        sr.IncCounters( _avButton.FontFamily.ToString() != "Tahoma",
                        "Default PropertyTranslator was called after the mapping was removed", p.log);

        return sr;
    }

    [Scenario("Chain an additional mapping delegate. Verify the correct behavior.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        // ---- Try to chain an additional mapping for an already mapped (default) property ---- //

        //add an additional mapping for Font property 
        _elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);

        //change the property on the Host's parent and verify the correct behaviour (the default translator and MyPropertyTranslator are called)
        PropertyTranslatorInfo.GetInstance().Reset();
        _mainPanel.Font = new System.Drawing.Font("Impact", _mainPanel.Font.Size);
        Application.DoEvents();
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "Font", _mainPanel.Font);
        }
        sr.IncCounters( _avButton.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called", p.log);


        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Tahoma", _elementHost.Font.Size);
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log );
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "Font", _elementHost.Font);
        }
        sr.IncCounters( _avButton.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called", p.log);

        // ---- Try to chain an additional mapping for a property that wasn't mapped before ---- //

        //add an additional mapping for a property that wasn't mapped previously (SnapsToDevicePixels)
        _elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);

        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.CausesValidation = !_elementHost.CausesValidation;
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "CausesValidation", _elementHost.CausesValidation.ToString());
        }

        return sr;
    }

    [Scenario("Try to chain an additional mapping delegate to an invalid property. Verify the correct behavior.")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //use a property that doesn't exist on the ElementHost and verify ArgumentException is thrown
        try
        {
            _elementHost.PropertyMap[""] += new PropertyTranslator(MyPropertyTranslator);
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception e)
        {
            p.log.LogException(e);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        }

        //put a null in the param for the indexer and verify ArgumentNullException is thrown
        try
        {
            _elementHost.PropertyMap[null] += new PropertyTranslator(MyPropertyTranslator);
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentNullException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception e)
        {
            p.log.LogException(e);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        }

        return sr;
    }

    [Scenario("Verify that you can map two properties on the same delegate.")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //map the CausesValidation and the Font property to MyPropertyTranslator
        _elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);
        _elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);

        //change the CausesValidation property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.CausesValidation = !_elementHost.CausesValidation;
        Application.DoEvents();
        if(!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log );
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "CausesValidation", _elementHost.CausesValidation.ToString());
        }

        //change the Font property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params and the default mapping is called)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Impact", _elementHost.Font.Size);
        Application.DoEvents();
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "Font", _elementHost.Font);
        }
        sr.IncCounters( _avButton.FontFamily.ToString() == "Impact",
                         "Default PropertyTranslator wasn't called", p.log);

        return sr;
    }

    [Scenario("Verify that the property-mapping is executed when it's added (and reseted for the default mappings)")]
    public ScenarioResult Scenario11(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        PropertyTranslatorInfo.GetInstance().Reset();

        //map the CausesValidation property to my custom PropertyTranslator
        _elementHost.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));
        Application.DoEvents();

        //verify if the custom PropertyTranslator was called (with the proper args)
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "CausesValidation", _elementHost.CausesValidation.ToString());
        }

        //remove the CausesValidation mapping
        _elementHost.PropertyMap.Remove("CausesValidation");

        //re-add the CausesValidation mapping with += syntax - verify that the translator got called with the correct params 
        _elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log );
        }
        else
        {
            VerifyTranslatorParams(p, sr, _elementHost, "CausesValidation", _elementHost.CausesValidation.ToString());
        }

        //remove a default-mapping 
        _elementHost.PropertyMap.Remove("Font");

        //change the Font
        _mainPanel.Font = new System.Drawing.Font("Tahoma", _mainPanel.Font.Size);

        //reset the default mapping and verify that the default translator got called 
        _elementHost.PropertyMap.Reset("Font");
        sr.IncCounters( _avButton.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called after the mapping was reseted", p.log);

        return sr;
    }

    [Scenario("Clear all mappings and verify the correct behavior.")]
    public ScenarioResult Scenario12(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //add a custom mapping for the Dock property 
        _elementHost.PropertyMap.Add("Dock", new PropertyTranslator(MyPropertyTranslator));

        //clear all mappings 
        _elementHost.PropertyMap.Clear();

        //verify that the custom mapping got removed 
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Dock = DockStyle.Top;
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after the mapping was removed", p.log );

        //verify that the default mapping for Font was removed
        _mainPanel.Font = new System.Drawing.Font("Impact", _mainPanel.Font.Size);
        Application.DoEvents();
        sr.IncCounters( _avButton.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after it was removed", p.log);

        //try to reset the Font (to see if reset works after ClearAll)
        _elementHost.PropertyMap.Reset("Font");

        _elementHost.Font = new System.Drawing.Font("Times New Roman", _elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( _avButton.FontFamily.ToString() == "Times New Roman",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        return sr;
    }

    [Scenario("Reset a property-mapping and verify that the default mapping is restored.")]
    public ScenarioResult Scenario13(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        // --- try to reset a default property mapping that was Removed --- //

        //remove the property-mapping for the Font and after that - reset
        _elementHost.PropertyMap.Remove("Font");
        _elementHost.PropertyMap.Reset("Font");

        //verify if the property got reseted
        _mainPanel.Font = new System.Drawing.Font("Impact", _mainPanel.Font.Size);
        Application.DoEvents();
        sr.IncCounters( _avButton.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called after it was restored", p.log );

        _elementHost.Font = new System.Drawing.Font("Tahoma", _elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( _avButton.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        // --- try to reset a default property mapping on which we've chained a custom mapping --- //

        //chain our translator to the Font mapping and reset-it
        _elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);
        _elementHost.PropertyMap.Reset("Font");

        //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Comic Sans MS", _elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was call after a Reset", p.log );
        sr.IncCounters( _avButton.FontFamily.ToString() == "Comic Sans MS",
                        "Default PropertyTranslator wasn't called after it was reseted ", p.log);

        // --- try to reset a default property mapping that was replaced --- //

        //replace the default translator for Font-property with our translator 
        _elementHost.PropertyMap.Remove("Font");
        _elementHost.PropertyMap.Add( "Font", new PropertyTranslator(MyPropertyTranslator));
        _elementHost.PropertyMap.Reset("Font");

        //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Times New Roman", _elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was call after a Reset", p.log);
        sr.IncCounters(_avButton.FontFamily.ToString() == "Times New Roman",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);

        return sr;
    }

    [Scenario("Try to Reset a property that has no default mapping. Verify the correct behaviour.")]
    public ScenarioResult Scenario14(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);
        try
        {
            _elementHost.PropertyMap.Reset("Dock");
            sr.IncCounters(true, "", p.log);
        }
        catch (Exception ex)
        {
            sr.IncCounters( false, "Reset shouldn't throw an exception. " + ex.Message, p.log );
        }

        return sr;
    }

    [Scenario("Try to call Reset for an invalid property. Verify the correct behaviour.")]
    public ScenarioResult Scenario15(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);
       
        try
        {
            _elementHost.PropertyMap.Reset("  ");
            sr.IncCounters(true, "", p.log);
        }
        catch (Exception e)
        {
            sr.IncCounters(false, "Reset should not throw exceptions when the PropertyName is invalid. " + e.Message, p.log);
        }

        try
        {
            _elementHost.PropertyMap.Reset(null);
            sr.IncCounters(false, "Did not get expected exception", p.log);
        }
        catch (ArgumentNullException ex)
        {
            sr.IncCounters(true, ex.Message, p.log);
        }
        catch (Exception ex)
        {
            p.log.LogException(ex);
            sr.IncCounters(false, "Got exception, but not right one", p.log);
        }

        return sr;
    }

    [Scenario("ResetAll property mappings. Verify correct behavior.")]
    public ScenarioResult Scenario16(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        // - remove the default mapping for the Visible
        // - chain a new delegate to the default-mapped Font
        // - replace the default mapping for Enabled property
        // - add a custom mapping for Dock (not mapped by default)
        _elementHost.PropertyMap.Remove("RightToLeft");
        _elementHost.PropertyMap.Remove("Enabled");
        _elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);
        _elementHost.PropertyMap.Add("Enabled", new PropertyTranslator(MyPropertyTranslator));
        _elementHost.PropertyMap.Add("Dock", new PropertyTranslator(MyPropertyTranslator));

        //Now reset all
        _elementHost.PropertyMap.ResetAll();

        //verify the correct behaviour for the Visible (the default translator should be invoked)
        _elementHost.RightToLeft = RightToLeft.Yes;
        Application.DoEvents();
        sr.IncCounters( _avButton.FlowDirection == System.Windows.FlowDirection.RightToLeft,
                        "Default PropertyTranslator wasn't called after it was restored", p.log);
        _elementHost.RightToLeft = RightToLeft.No;

        //verify the correct behaviour for the Font (the default translator should be invoked and the custom shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Font = new System.Drawing.Font("Impact", _elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after a Reset", p.log );
        sr.IncCounters( _avButton.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log );

        //verify the correct behaviour for the IsEnabled property (the default formater should be invoked and the custom shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Enabled = false;
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after a Reset", p.log );
        sr.IncCounters( _avButton.IsEnabled == false,
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);
        _elementHost.Enabled = true;

        //verify the correct behaviour for the Dock property (my custom shouldn't get called)
        PropertyTranslatorInfo.GetInstance().Reset();
        _elementHost.Dock = DockStyle.Left;
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after a Reset", p.log );

        return sr;
    }

    [Scenario("Map a property to a delegate that throws an exception and verify the correct behavior.")]
    public ScenarioResult Scenario17(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        try
        {
            //new behaviour - propertyMap will raise and event when the translator throws.
            _elementHost.PropertyMap.PropertyMappingError += new EventHandler<PropertyMappingExceptionEventArgs>(PropertyMap_PropertyMappingError);

            //map the ToolTip property to the delegate that throws an exception
            _elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyEvilPropertyTranslator);
            Application.DoEvents();

            //if we are here - the exception that was thrown from the delegate vanished
            sr.IncCounters( false, "What happened with the exception that was throw inside the delegate??", p.log );
        }
        catch (Exception e)
        {
            sr.IncCounters( true, e.Message, p.log );
        }
        return sr;
    }

    #endregion

    #region Helper methods

    //helper method that creates&sets all the needed controls
    private void InitializeControls()
    {
        //Panel
        _mainPanel = new Panel();
        _mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Controls.Add(_mainPanel);

        //ElementHost
        _elementHost = new ElementHost();
        _elementHost.Dock = DockStyle.Fill;
        _mainPanel.Controls.Add(_elementHost);

        //WPF Button
        _avButton = new System.Windows.Controls.Button();
        _avButton.Content = "Hi there";
        _elementHost.Child = _avButton;
    }

    //Helper function that Compares the values of PropertyTranslatorInfo-properties against the expected values and 
    //decides if the mapping mechanism behaved as expected
    // We'll have overloads for different types of Values. 
    private void VerifyTranslatorParams(TParams p, ScenarioResult sr, object expectedHost, string expectedPropName, string expectedValue)
    {
        if (PropertyTranslatorInfo.GetInstance().Host == null)
        {
            sr.IncCounters(false, "Invalid Host in PropertyTranslator", p.log);
            return;
        }
        if (PropertyTranslatorInfo.GetInstance().Value == null)
        {
            //for the cases when Value should be null .. add an overload for this without the expectedValue-param in the signature. 
            sr.IncCounters(false, "Invalid Value-param in PropertyTranslator", p.log);
            return;
        }

        sr.IncCounters( PropertyTranslatorInfo.GetInstance().Host, expectedHost,
                        "Invalid host parameter in PropertyTranslator", p.log);
        sr.IncCounters( PropertyTranslatorInfo.GetInstance().PropName, expectedPropName,
                        "Invalid PropName parameter in PropertyTranslator", p.log);
        sr.IncCounters( PropertyTranslatorInfo.GetInstance().Value.ToString(), expectedValue,
                        "Invalid Value parameter in PropertyTranslator", p.log);
    }
    private void VerifyTranslatorParams(TParams p, ScenarioResult sr, object expectedHost, string expectedPropName, System.Drawing.Font expectedValue)
    {
        if (PropertyTranslatorInfo.GetInstance().Host == null)
        {
            sr.IncCounters(false, "Invalid Host in PropertyTranslator", p.log);
            return;
        }
        if (PropertyTranslatorInfo.GetInstance().Value == null)
        {
            //for the cases when Value should be null .. add an overload for this without the expectedValue-param in the signature. 
            sr.IncCounters(false, "Invalid Value-param in PropertyTranslator", p.log);
            return;
        }

        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Host, expectedHost,
                        "Invalid host parameter in PropertyTranslator", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().PropName, expectedPropName,
                        "Invalid PropName parameter in PropertyTranslator", p.log);
        sr.IncCounters(PropertyTranslatorInfo.GetInstance().Value, expectedValue,
                        "Invalid Value parameter in PropertyTranslator", p.log);
    }

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

    void PropertyMap_PropertyMappingError(object sender, PropertyMappingExceptionEventArgs e)
    {
        e.ThrowException = true;
    }

    #endregion
}

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

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Add a new property mapping to an EH. Verify the correct behavior.
//@ Try to add a mapping for an invalid property. Verify ArgumentException/ArgumentNullExcepion is throw.
//@ Try map a property to a null translator. Verify ArgumentNullExcepion is throw.
//@ Remove a property mapping. Try this with a previously added custom-mapping, and with a default mapping. Verify the correct behavior.
//@ Try to Remove an invalid property. Verify the correct behavior.
//@ Try to Remove a property that wasn't mapped. Nothing should happen, no exception should be thrown.
//@ Replace an existing property mapping. Verify the correct behavior.
//@ Chain an additional mapping delegate. Verify the correct behavior.
//@ Try to chain an additional mapping delegate to an invalid property. Verify the correct behavior.
//@ Verify that you can map two properties on the same delegate.
//@ Verify that the property-mapping is executed when it's added (and reseted for the default mappings)
//@ Clear all mappings and verify the correct behavior.
//@ Reset a property-mapping and verify that the default mapping is restored. 
//@ Try to Reset a property that has no default mapping. Verify the correct behaviour.
//@ Try to call Reset for an invalid property. Verify the correct behaviour.
//@ ResetAll property mappings. Verify correct behavior.
//@ Map a property to a delegate that throws an exception and verify the correct behavior.
