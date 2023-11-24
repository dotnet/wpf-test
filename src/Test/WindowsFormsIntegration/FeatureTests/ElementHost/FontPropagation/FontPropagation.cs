using System;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.Integration;
using System.Drawing;
using Microsoft.Test.Display;

//
// Testcase:    FontPropagation
// Description: Verify that Fonts set on the EH parent get propogated to the EH and it's control
// Author:      bogdanbr
//

public class FontPropagation : ReflectBase
{
    private Panel ehParent = null;
    private ElementHost elementHost = null;
    private System.Windows.Controls.Label ehChild = null;

    #region Testcase setup
    public FontPropagation(string[] args) : base(args) { }


    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        //we'll initialize/refresh the form at the beginning of each scenario 
        if (this.Controls.Count != 0)
        {
            this.Controls.Clear();
        }

        ehChild = new System.Windows.Controls.Label();
        ehChild.Content = "Test String";

        elementHost = new ElementHost();
        elementHost.AutoSize = true;
        elementHost.Child = ehChild;

        ehParent = new Panel();
        ehParent.Location = new Point(0, 0);
        ehParent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        ehParent.AutoSize = true;
        ehParent.Controls.Add(elementHost);
        this.Controls.Add(ehParent);

        return base.BeforeScenario(p, scenario);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("1) Set Font on EH parent")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the parent
        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), "FontFamily failed", p.log);
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, 15), ehChild.FontSize, "FontSize failed", p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, "FontStyle failed", p.log);

        return sr;
    }

    [Scenario("2) Set Font on EH parent twice")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the parent
        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, 15), ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        //Set it again 
        ehParent.Font = new Font("Times New Roman", 18, FontStyle.Regular, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Times New Roman", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters((int)Monitor.ConvertScreenToLogical(Dimension.Width, 18), (int)ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Normal, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("3) Set Font on EH child and make sure EH parent doesn't change")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //get the font on the ehParent
        Font originalFont = ehParent.Font;

        //change the font on the ehChild
        ehChild.FontSize += 3;
        ehChild.FontFamily = new System.Windows.Media.FontFamily("Impact");
        ehChild.FontStyle = System.Windows.FontStyles.Italic;
        Application.DoEvents();

        //verify that the font on the ehParent wasn't changed. 
        sr.IncCounters(originalFont, ehParent.Font, p.log);

        return sr;
    }

    [Scenario("4) Set Font on EH child then EH parent and make sure EH child doesn't change to EH parent")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the child. 
        ehChild.FontStyle = System.Windows.FontStyles.Normal;
        ehChild.FontSize = 20;
        ehChild.FontFamily = new System.Windows.Media.FontFamily("Impact");
        Application.DoEvents();

        //set the font on the ehParent. 
        ehParent.Font = new Font("Times New Roman", 12, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font on the ehChild didn't change. (changes on the parent shoud not overwrite what was specifically set) 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(20.0, ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Normal, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("5) Set Font on EH parent then EH child")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the ehParent. 
        ehParent.Font = new Font("Times New Roman", 12, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //set the font on the child. 
        ehChild.FontStyle = System.Windows.FontStyles.Normal;
        ehChild.FontSize = 20;
        ehChild.FontFamily = new System.Windows.Media.FontFamily("Impact");
        Application.DoEvents();

        //verify that the font on the ehChild changed 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(20.0, ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Normal, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("6) Set Font on EH")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the elementHost
        elementHost.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, 15), ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("7) Set Font on EH twice")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the element host
        elementHost.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, 15), ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        //Set it again 
        elementHost.Font = new Font("Times New Roman", 18, FontStyle.Regular, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Times New Roman", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters((int)Monitor.ConvertScreenToLogical(Dimension.Width, 18), (int)ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Normal, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("8) Set Font on EH child and make sure EH doesn't change")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //get the font on the elementHost
        Font originalFont = elementHost.Font;

        //change the font on the ehChild
        ehChild.FontSize += 3;
        ehChild.FontFamily = new System.Windows.Media.FontFamily("Impact");
        ehChild.FontStyle = System.Windows.FontStyles.Italic;
        Application.DoEvents();

        //verify that the font on the ehParent wasn't changed. 
        sr.IncCounters(originalFont, elementHost.Font, p.log);

        return sr;
    }

    [Scenario("9) Set Font on EH child then EH and make sure child doesn't change to EH")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the child. 
        ehChild.FontStyle = System.Windows.FontStyles.Normal;
        ehChild.FontSize = 20;
        ehChild.FontFamily = new System.Windows.Media.FontFamily("Impact");
        Application.DoEvents();

        //set the font on the element host 
        elementHost.Font = new Font("Times New Roman", 12, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font on the ehChild didn't change. (changes on the element host shoud not overwrite what was specifically set) 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(20.0, ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Normal, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("10) Set Font on EH then child")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //set the font on the element-host. 
        elementHost.Font = new Font("Times New Roman", 12, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //set the font on the child. 
        ehChild.FontStyle = System.Windows.FontStyles.Normal;
        ehChild.FontSize = 20;
        ehChild.FontFamily = new System.Windows.Media.FontFamily("Impact");
        Application.DoEvents();

        //verify that the font on the ehChild changed 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(20.0, ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Normal, ehChild.FontStyle, p.log);

        return sr;
    }

    [Scenario("11) Set Font to a font with GraphicalUnit of Millimeter")]
    public ScenarioResult Scenario11(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        //set the font on the parent
        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Millimeter);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(56, (int)ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Document);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(4, (int)ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Inch);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(1440, (int)ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Point);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(20, (int)ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.World);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, 15), ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        ehParent.Font = new Font("Impact", 15, FontStyle.Italic, GraphicsUnit.Pixel);
        Application.DoEvents();

        //verify that the font was applied on the Avalon Label. 
        sr.IncCounters("Impact", ehChild.FontFamily.ToString(), p.log);
        sr.IncCounters(Monitor.ConvertScreenToLogical(Dimension.Width, 15), ehChild.FontSize, p.log);
        sr.IncCounters(System.Windows.FontStyles.Italic, ehChild.FontStyle, p.log);

        return sr;
    }

    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Set Font on EH parent

//@ 2) Set Font on EH parent twice

//@ 3) Set Font on EH child and make sure EH parent doesn't change

//@ 4) Set Font on EH child then EH parent and make sure EH child doesn't change to EH parent

//@ 5) Set Font on EH parent then EH child

//@ 6) Set Font on EH

//@ 7) Set Font on EH twice

//@ 8) Set Font on EH child and make sure EH doesn't change

//@ 9) Set Font on EH child then EH and make sure child doesn't change to EH

//@ 10) Set Font on EH then child

//@ 11) Set Font to a font with GraphicalUnit of Millimeter