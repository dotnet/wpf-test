using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using SD = System.Drawing;
using SWF = System.Windows.Forms;
using System.Windows.Controls;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;


//
// Testcase:    DefaultPM_Delegate
// Description: Test existing spec defined Mappings
// Author:      sameerm
//
public class DefaultPM_Delegate : ReflectBase
{

    class DefaultProp
    {
        public DefaultProp(string name)
        {
            this._name = name;
        }
        int count = 0;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        object _val;
        public object Val
        {
            get { return _val; }
            set { _val = value; }
        }
    }


    public DefaultPM_Delegate(string[] args) : base(args) { }


    protected override void InitTest(TParams p)
    {         
       
         base.InitTest(p);
    }


    ElementHost eh;
    //private static System.Windows.Controls.TextBox avLbl;                        // our Avalon button
    private static System.Windows.Controls.Label avLbl;                        // our Avalon button
    private DefaultProp currentProperty;
    
    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        this.ClientSize = new System.Drawing.Size(300, 200);
        avLbl = new SWC.Label();
        
        avLbl.Content = "Avalon Button";
        eh = new ElementHost();
        eh.Child = avLbl;
        eh.BackColor = System.Drawing.Color.Red;
        avLbl.Height = 50;
        avLbl.Width = 100;
        Controls.Add(eh);
        return base.BeforeScenario(p, scenario);
    }

    protected override void AfterScenario(TParams p, System.Reflection.MethodInfo scenario, ScenarioResult result)
    {
        //Controls.Clear();
        base.AfterScenario(p, scenario, result);
    }
 

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios

    [Scenario("Override and verify overridden function is called.")]
    public ScenarioResult OverrideDelegate(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropertyMap dictionary = eh.PropertyMap;
        if (dictionary == null)
        {
            sr.IncCounters(false, "Property Map Dictionary did not return prop map.", p.log);
            return sr;
        }
        Bitmap bmp = null;

        Array [] keysArray = new Array[dictionary.Keys.Count];
        String[] keys = new String[dictionary.Keys.Count];
        dictionary.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; i++)
        {
            currentProperty = new DefaultProp(keys[i]);
            eh.PropertyMap.ResetAll();
            eh.PropertyMap.Remove((String)keys[i]);
            eh.PropertyMap.Add((String)keys[i], new PropertyTranslator(OnPropChange));

            this.Text = keys[i]; 
            switch  (keys[i]) 
            {
                case "BackColor":
                    eh.BackColor = System.Drawing.Color.Bisque;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackColor" ||  currentProperty.Count != 2 || (System.Drawing.Color)currentProperty.Val != System.Drawing.Color.Bisque)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString())); 
                        sr.IncCounters(false);
                    }
                    break;


                case "BackgroundImage":
                        bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                        eh.BackgroundImage = bmp;
                        Utilities.SleepDoEvents(1, 100);
                        if (currentProperty.Name != "BackgroundImage" || currentProperty.Count != 2 ||   (System.Drawing.Bitmap)currentProperty.Val != bmp)
                        {
                           p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                           sr.IncCounters(false);
                        }
                        eh.BackgroundImage = null;
                    break;

                case "BackgroundImageLayout":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    eh.BackgroundImage = bmp;
                    eh.BackgroundImageLayout = ImageLayout.Stretch;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackgroundImageLayout" || currentProperty.Count != 2 || (ImageLayout)currentProperty.Val != ImageLayout.Stretch)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.BackgroundImage = null;
                    eh.BackgroundImageLayout = ImageLayout.None;

                    break;

                case "Cursor":
                    eh.Cursor = System.Windows.Forms.Cursors.Hand;
                    Utilities.SleepDoEvents(10);
                    if (currentProperty.Name != "Cursor" || currentProperty.Count != 2 || (System.Windows.Forms.Cursor)currentProperty.Val != System.Windows.Forms.Cursors.Hand)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.Cursor = Cursors.Arrow;
                    break;


                case "Enabled":
                    eh.Enabled = false;
                    Utilities.SleepDoEvents(10);
                    if (currentProperty.Name != "Enabled" || currentProperty.Count != 2 || (bool)currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Enabled = true; //reset 
                    break;


                case "Font":
                    Utilities.SleepDoEvents(10);
                    eh.Font = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                    Font fnt = (Font)currentProperty.Val;
                    if (currentProperty.Name != "Font" || currentProperty.Count != 2)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Font = fnt; // Reset to original 
                    break;

                case "RightToLeft":
                    Utilities.SleepDoEvents(10);
                    eh.RightToLeft = RightToLeft.Yes;
                    if (currentProperty.Name != "RightToLeft" || currentProperty.Count != 2 || (SWF.RightToLeft)currentProperty.Val != RightToLeft.Yes)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.RightToLeft = RightToLeft.No;
                    break;

                case "Visible":
                    Utilities.SleepDoEvents(10);
                    eh.Visible = false;
                    if (currentProperty.Name != "Visible" || currentProperty.Count != 2 || (bool)currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Visible = true;
                    break;
            }
            Utilities.SleepDoEvents(1, 100);
            currentProperty  = null;
        }
        sr.IncCounters(true);
      
        return sr;
    }



    [Scenario("Add a delegate and verify delegate function is called.")]
    public ScenarioResult AddDelegate(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropertyMap dictionary = eh.PropertyMap;
        Bitmap bmp = null;
        if (dictionary == null)
        {
            sr.IncCounters(false, "Property Map Dictionary did not return prop map.", p.log);
            return sr;
        }

        Array[] keysArray = new Array[dictionary.Keys.Count];
        String[] keys = new String[dictionary.Keys.Count];
        dictionary.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; i++)
        {
            currentProperty = new DefaultProp(keys[i]);
            eh.PropertyMap.ResetAll();
            eh.PropertyMap[keys[i]] += new PropertyTranslator(OnPropChange);

            this.Text = keys[i];
            switch (keys[i])
            {
                case "BackColor":
                    eh.BackColor = System.Drawing.Color.Bisque;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackColor" || currentProperty.Count != 2 || (System.Drawing.Color)currentProperty.Val != System.Drawing.Color.Bisque)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    
                    break;


                case "BackgroundImage":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    eh.BackgroundImage = bmp;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackgroundImage" || currentProperty.Count != 2 || (System.Drawing.Bitmap)currentProperty.Val != bmp)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.BackgroundImage = null;
                    break;

                case "BackgroundImageLayout":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    eh.BackgroundImage = bmp;
                    eh.BackgroundImageLayout = ImageLayout.Stretch;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackgroundImageLayout" || currentProperty.Count != 2 || (ImageLayout)currentProperty.Val != ImageLayout.Stretch)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.BackgroundImage = null;
                    eh.BackgroundImageLayout = ImageLayout.None;

                    break;

                case "Cursor":
                    eh.Cursor = System.Windows.Forms.Cursors.Hand;
                    Utilities.SleepDoEvents(10);
                    if (currentProperty.Name != "Cursor" || currentProperty.Count != 2 || (System.Windows.Forms.Cursor)currentProperty.Val != System.Windows.Forms.Cursors.Hand)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.Cursor = Cursors.Arrow;
                    break;


                case "Enabled":
                    eh.Enabled = false;
                    Utilities.SleepDoEvents(10);
                    if (currentProperty.Name != "Enabled" || currentProperty.Count != 2 || (bool)currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Enabled = true; //reset 
                    break;


                case "Font":
                    Utilities.SleepDoEvents(10);
                    eh.Font = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                    Font fnt = (Font)currentProperty.Val;
                    //SAM TODO
                    if (currentProperty.Name != "Font" || currentProperty.Count != 2)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Font = fnt; // Reset to original 
                    break;

                case "RightToLeft":
                    Utilities.SleepDoEvents(10);
                    eh.RightToLeft = RightToLeft.Yes;
                    if (currentProperty.Name != "RightToLeft" || currentProperty.Count != 2 || (SWF.RightToLeft)currentProperty.Val != RightToLeft.Yes)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.RightToLeft = RightToLeft.No;
                    break;

                case "Visible":
                    Utilities.SleepDoEvents(10);
                    eh.Visible = false;
                    if (currentProperty.Name != "Visible" || currentProperty.Count != 2 || (bool)currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Visible = true;
                    break;
            }
            currentProperty = null;
            eh.PropertyMap.ResetAll();
        }
        Utilities.SleepDoEvents(1, 100);
        sr.IncCounters(true);
        return sr;
    }



    [Scenario("Remove the  property and verify that PT is not called.")]
    public ScenarioResult RemovePropTranslator(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropertyMap dictionary = eh.PropertyMap;
        Bitmap bmp = null;

        if (dictionary == null)
        {
            sr.IncCounters(false, "Property Map Dictionary did not return prop map.", p.log);
            return sr;
        }
        Array[] keysArray = new Array[dictionary.Keys.Count];
        String[] keys = new String[dictionary.Keys.Count];
        dictionary.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; i++)
        {
            currentProperty = new DefaultProp(keys[i]);
            eh.PropertyMap.ResetAll();
            eh.PropertyMap.Remove(keys[i]);

            this.Text = keys[i]; 
            switch (keys[i])
            {
                case "BackColor":
                    eh.BackColor = System.Drawing.Color.Bisque;
                    Utilities.SleepDoEvents(1, 100);
                    try
                    {
                        if (currentProperty.Name != "BackColor" || currentProperty.Count != 0 || currentProperty.Val != null)
                        {
                            p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                            sr.IncCounters(false);
                        }
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                    break;


                case "BackgroundImage":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    eh.BackgroundImage = bmp;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackgroundImage" || currentProperty.Count != 0 || currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.BackgroundImage = null;
                    break;

                case "BackgroundImageLayout":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    eh.BackgroundImage = bmp;
                    eh.BackgroundImageLayout = ImageLayout.Stretch;
                    Utilities.SleepDoEvents(1, 100);
                    if (currentProperty.Name != "BackgroundImageLayout" ||  currentProperty.Count != 0 || currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.BackgroundImage = null;
                    eh.BackgroundImageLayout = ImageLayout.None;

                    break;

                case "Cursor":
                    eh.Cursor = System.Windows.Forms.Cursors.Hand;
                    Utilities.SleepDoEvents(10);
                    if (currentProperty.Name != "Cursor" || currentProperty.Count != 0  ||  currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", currentProperty.Name, currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    eh.Cursor = Cursors.Arrow;
                    break;


                case "Enabled":
                    eh.Enabled = false;
                    Utilities.SleepDoEvents(10);
                    if (currentProperty.Name != "Enabled" || currentProperty.Count != 0 || currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Enabled = true; //reset 
                    break;


                case "Font":
                    Utilities.SleepDoEvents(10);
                    eh.Font = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                    Font fnt = (Font)currentProperty.Val;
                    //SAM TODO
                    if (currentProperty.Name != "Font" || currentProperty.Count != 0 || currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    eh.Font = fnt; // Reset to original 
                    break;

                case "RightToLeft":
                    Utilities.SleepDoEvents(10);
                    eh.RightToLeft = RightToLeft.Yes;
                    if (currentProperty.Name != "RightToLeft" || currentProperty.Count != 0 || currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    break;

                case "Visible":
                    Utilities.SleepDoEvents(10);
                    eh.Visible = false;
                    if (currentProperty.Name != "Visible" || currentProperty.Count != 0 || currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", currentProperty.Name, currentProperty.Count.ToString()), currentProperty.Val.ToString());
                        eh.RightToLeft = RightToLeft.No;
                    }
                    eh.Visible = true;
                    break;
            }
            currentProperty = null;
        }
        Utilities.SleepDoEvents(1, 100);
        sr.IncCounters(true);
        return sr;
    }
    #endregion

    public void OnPropChange(object host, String propertyName, object val)
    {
        currentProperty.Name = propertyName;
        currentProperty.Count++;
        currentProperty.Val = val;
    }


}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Override and verify overridden function is called.
//@ Add a delegate and verify delegate function is called.
//@ Remove the  property and verify that PT is not called.