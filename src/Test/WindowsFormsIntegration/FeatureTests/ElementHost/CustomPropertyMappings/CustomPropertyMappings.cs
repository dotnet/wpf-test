using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.Integration;

//
// Testcase:    CustomPropertyMappings
// Description: Test whether or not custom property mappings work
// Author:      bogdanbr
//
public class CustomPropertyMappings : ReflectBase
{
    private Panel mainPanel = null;
    private ElementHost elementHost = null;
    private System.Windows.Controls.Button avButton = null;

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
        elementHost.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));

        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.CausesValidation = !elementHost.CausesValidation;
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "CausesValidation", elementHost.CausesValidation.ToString());
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
            elementHost.PropertyMap.Add("", new PropertyTranslator(MyPropertyTranslator));
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
            elementHost.PropertyMap.Add(null, new PropertyTranslator(MyPropertyTranslator));
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
            elementHost.PropertyMap.Remove("Visible");
            elementHost.PropertyMap.Add("Visible", null);
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
        elementHost.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));
        elementHost.PropertyMap.Remove("CausesValidation");

        //change the property on the Host and verify the correct behaviour 
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.CausesValidation = !elementHost.CausesValidation;
        Application.DoEvents();

        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after the mapping was removed", 
                        p.log );

        // ---- Now try the same things with an default mapped property ---- //

        //remove the default mapping for the Font-property
        elementHost.PropertyMap.Remove("Font");

        //change the property on the Host's parent and verify the correct behaviour
        PropertyTranslatorInfo.GetInstance().Reset();
        mainPanel.Font = new System.Drawing.Font("Impact", mainPanel.Font.Size);
        Application.DoEvents();

        sr.IncCounters( avButton.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after the mapping was removed", 
                        p.log );

        //change the property on the Host and verify the correct behaviour
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Tahoma", elementHost.Font.Size);
        Application.DoEvents();

        sr.IncCounters( avButton.FontFamily.ToString() != "Tahoma",
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
            elementHost.PropertyMap.Remove("");
            sr.IncCounters(true, "", p.log);
        }
        catch (Exception e)
        {
            sr.IncCounters(false, "Remove should not throw exceptions when the PropertyName is invalid. " + e.Message, p.log);
        }

        //Remove(null) should throw ArgumentNullException
        try
        {
            elementHost.PropertyMap.Remove(null);
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
            elementHost.PropertyMap.Remove("CausesValidation");
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
        elementHost.PropertyMap.Remove("Font");
        elementHost.PropertyMap.Add("Font", new PropertyTranslator(MyPropertyTranslator));

        //change the property on the Host's parent and verify the correct behaviour (MyPropertyTranslator is called with the proper params and the default translator is not invoked anymore)
        PropertyTranslatorInfo.GetInstance().Reset();
        mainPanel.Font = new System.Drawing.Font("Impact", mainPanel.Font.Size);
        Application.DoEvents();
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "Font", mainPanel.Font);
        }
        sr.IncCounters( avButton.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after the mapping was removed", p.log);


        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Tahoma", elementHost.Font.Size);
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "Font", elementHost.Font);
        }
        sr.IncCounters( avButton.FontFamily.ToString() != "Tahoma",
                        "Default PropertyTranslator was called after the mapping was removed", p.log);

        return sr;
    }

    [Scenario("Chain an additional mapping delegate. Verify the correct behavior.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        // ---- Try to chain an additional mapping for an already mapped (default) property ---- //

        //add an additional mapping for Font property 
        elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);

        //change the property on the Host's parent and verify the correct behaviour (the default translator and MyPropertyTranslator are called)
        PropertyTranslatorInfo.GetInstance().Reset();
        mainPanel.Font = new System.Drawing.Font("Impact", mainPanel.Font.Size);
        Application.DoEvents();
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "Font", mainPanel.Font);
        }
        sr.IncCounters( avButton.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called", p.log);


        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Tahoma", elementHost.Font.Size);
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log );
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "Font", elementHost.Font);
        }
        sr.IncCounters( avButton.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called", p.log);

        // ---- Try to chain an additional mapping for a property that wasn't mapped before ---- //

        //add an additional mapping for a property that wasn't mapped previously (SnapsToDevicePixels)
        elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);

        //change the property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.CausesValidation = !elementHost.CausesValidation;
        Application.DoEvents();
        if (!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "CausesValidation", elementHost.CausesValidation.ToString());
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
            elementHost.PropertyMap[""] += new PropertyTranslator(MyPropertyTranslator);
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
            elementHost.PropertyMap[null] += new PropertyTranslator(MyPropertyTranslator);
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
        elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);
        elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);

        //change the CausesValidation property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.CausesValidation = !elementHost.CausesValidation;
        Application.DoEvents();
        if(!PropertyTranslatorInfo.GetInstance().CallFlag)
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log );
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "CausesValidation", elementHost.CausesValidation.ToString());
        }

        //change the Font property on the Host and verify the correct behaviour (MyPropertyTranslator is called with the proper params and the default mapping is called)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Impact", elementHost.Font.Size);
        Application.DoEvents();
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "Font", elementHost.Font);
        }
        sr.IncCounters( avButton.FontFamily.ToString() == "Impact",
                         "Default PropertyTranslator wasn't called", p.log);

        return sr;
    }

    [Scenario("Verify that the property-mapping is executed when it's added (and reseted for the default mappings)")]
    public ScenarioResult Scenario11(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        PropertyTranslatorInfo.GetInstance().Reset();

        //map the CausesValidation property to my custom PropertyTranslator
        elementHost.PropertyMap.Add("CausesValidation", new PropertyTranslator(MyPropertyTranslator));
        Application.DoEvents();

        //verify if the custom PropertyTranslator was called (with the proper args)
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters(false, "MyPropertyTranslator wasn't called", p.log);
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "CausesValidation", elementHost.CausesValidation.ToString());
        }

        //remove the CausesValidation mapping
        elementHost.PropertyMap.Remove("CausesValidation");

        //re-add the CausesValidation mapping with += syntax - verify that the translator got called with the correct params 
        elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyPropertyTranslator);
        if( !PropertyTranslatorInfo.GetInstance().CallFlag )
        {
            sr.IncCounters( false, "MyPropertyTranslator wasn't called", p.log );
        }
        else
        {
            VerifyTranslatorParams(p, sr, elementHost, "CausesValidation", elementHost.CausesValidation.ToString());
        }

        //remove a default-mapping 
        elementHost.PropertyMap.Remove("Font");

        //change the Font
        mainPanel.Font = new System.Drawing.Font("Tahoma", mainPanel.Font.Size);

        //reset the default mapping and verify that the default translator got called 
        elementHost.PropertyMap.Reset("Font");
        sr.IncCounters( avButton.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called after the mapping was reseted", p.log);

        return sr;
    }

    [Scenario("Clear all mappings and verify the correct behavior.")]
    public ScenarioResult Scenario12(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //add a custom mapping for the Dock property 
        elementHost.PropertyMap.Add("Dock", new PropertyTranslator(MyPropertyTranslator));

        //clear all mappings 
        elementHost.PropertyMap.Clear();

        //verify that the custom mapping got removed 
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Dock = DockStyle.Top;
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after the mapping was removed", p.log );

        //verify that the default mapping for Font was removed
        mainPanel.Font = new System.Drawing.Font("Impact", mainPanel.Font.Size);
        Application.DoEvents();
        sr.IncCounters( avButton.FontFamily.ToString() != "Impact",
                        "Default PropertyTranslator was called after it was removed", p.log);

        //try to reset the Font (to see if reset works after ClearAll)
        elementHost.PropertyMap.Reset("Font");

        elementHost.Font = new System.Drawing.Font("Times New Roman", elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( avButton.FontFamily.ToString() == "Times New Roman",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        return sr;
    }

    [Scenario("Reset a property-mapping and verify that the default mapping is restored.")]
    public ScenarioResult Scenario13(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        // --- try to reset a default property mapping that was Removed --- //

        //remove the property-mapping for the Font and after that - reset
        elementHost.PropertyMap.Remove("Font");
        elementHost.PropertyMap.Reset("Font");

        //verify if the property got reseted
        mainPanel.Font = new System.Drawing.Font("Impact", mainPanel.Font.Size);
        Application.DoEvents();
        sr.IncCounters( avButton.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called after it was restored", p.log );

        elementHost.Font = new System.Drawing.Font("Tahoma", elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( avButton.FontFamily.ToString() == "Tahoma",
                        "Default PropertyTranslator wasn't called after it was restored", p.log);

        // --- try to reset a default property mapping on which we've chained a custom mapping --- //

        //chain our translator to the Font mapping and reset-it
        elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);
        elementHost.PropertyMap.Reset("Font");

        //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Comic Sans MS", elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was call after a Reset", p.log );
        sr.IncCounters( avButton.FontFamily.ToString() == "Comic Sans MS",
                        "Default PropertyTranslator wasn't called after it was reseted ", p.log);

        // --- try to reset a default property mapping that was replaced --- //

        //replace the default translator for Font-property with our translator 
        elementHost.PropertyMap.Remove("Font");
        elementHost.PropertyMap.Add( "Font", new PropertyTranslator(MyPropertyTranslator));
        elementHost.PropertyMap.Reset("Font");

        //verify if the property got reseted (default should be called and MyPropertyTranslator shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Times New Roman", elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters(!PropertyTranslatorInfo.GetInstance().CallFlag,
                        "MyPropertyTranslator was call after a Reset", p.log);
        sr.IncCounters(avButton.FontFamily.ToString() == "Times New Roman",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);

        return sr;
    }

    [Scenario("Try to Reset a property that has no default mapping. Verify the correct behaviour.")]
    public ScenarioResult Scenario14(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);
        try
        {
            elementHost.PropertyMap.Reset("Dock");
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
            elementHost.PropertyMap.Reset("  ");
            sr.IncCounters(true, "", p.log);
        }
        catch (Exception e)
        {
            sr.IncCounters(false, "Reset should not throw exceptions when the PropertyName is invalid. " + e.Message, p.log);
        }

        try
        {
            elementHost.PropertyMap.Reset(null);
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
        elementHost.PropertyMap.Remove("RightToLeft");
        elementHost.PropertyMap.Remove("Enabled");
        elementHost.PropertyMap["Font"] += new PropertyTranslator(MyPropertyTranslator);
        elementHost.PropertyMap.Add("Enabled", new PropertyTranslator(MyPropertyTranslator));
        elementHost.PropertyMap.Add("Dock", new PropertyTranslator(MyPropertyTranslator));

        //Now reset all
        elementHost.PropertyMap.ResetAll();

        //verify the correct behaviour for the Visible (the default translator should be invoked)
        elementHost.RightToLeft = RightToLeft.Yes;
        Application.DoEvents();
        sr.IncCounters( avButton.FlowDirection == System.Windows.FlowDirection.RightToLeft,
                        "Default PropertyTranslator wasn't called after it was restored", p.log);
        elementHost.RightToLeft = RightToLeft.No;

        //verify the correct behaviour for the Font (the default translator should be invoked and the custom shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Font = new System.Drawing.Font("Impact", elementHost.Font.Size);
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after a Reset", p.log );
        sr.IncCounters( avButton.FontFamily.ToString() == "Impact",
                        "Default PropertyTranslator wasn't called after it was reseted", p.log );

        //verify the correct behaviour for the IsEnabled property (the default formater should be invoked and the custom shouldn't)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Enabled = false;
        Application.DoEvents();
        sr.IncCounters( !PropertyTranslatorInfo.GetInstance().CallFlag, 
                        "MyPropertyTranslator was called after a Reset", p.log );
        sr.IncCounters( avButton.IsEnabled == false,
                        "Default PropertyTranslator wasn't called after it was reseted", p.log);
        elementHost.Enabled = true;

        //verify the correct behaviour for the Dock property (my custom shouldn't get called)
        PropertyTranslatorInfo.GetInstance().Reset();
        elementHost.Dock = DockStyle.Left;
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
            elementHost.PropertyMap.PropertyMappingError += new EventHandler<PropertyMappingExceptionEventArgs>(PropertyMap_PropertyMappingError);

            //map the ToolTip property to the delegate that throws an exception
            elementHost.PropertyMap["CausesValidation"] += new PropertyTranslator(MyEvilPropertyTranslator);
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
        mainPanel = new Panel();
        mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Controls.Add(mainPanel);

        //ElementHost
        elementHost = new ElementHost();
        elementHost.Dock = DockStyle.Fill;
        mainPanel.Controls.Add(elementHost);

        //WPF Button
        avButton = new System.Windows.Controls.Button();
        avButton.Content = "Hi there";
        elementHost.Child = avButton;
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
    private object m_host = null;
    private string m_propName = null;
    private object m_value = null;
    private bool m_callFlag = false; //m_host==null doesn't necessarily mean that the PropertyTranslator was not called... so we need this.    

    public object Host
    {
        get { return m_host; }
        set { m_host = value; }
    }
    public string PropName
    {
        get { return m_propName; }
        set { m_propName = value; }
    }
    public object Value
    {
        get { return m_value; }
        set { m_value = value; }
    }
    public bool CallFlag
    {
        get { return m_callFlag; }
        set { m_callFlag = value; }
    }

    public void Reset()
    {
        m_host = null;
        m_propName = null;
        m_value = null;
        m_callFlag = false;
    }

    //singleton stuff
    private static PropertyTranslatorInfo m_instance = new PropertyTranslatorInfo();
    private PropertyTranslatorInfo()
    {
    }
    public static PropertyTranslatorInfo GetInstance()
    {
        return m_instance;
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