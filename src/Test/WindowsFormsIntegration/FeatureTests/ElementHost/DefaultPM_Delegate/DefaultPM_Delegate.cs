// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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


// Testcase:    DefaultPM_Delegate
// Description: Test existing spec defined Mappings
public class DefaultPM_Delegate : ReflectBase
{
    class DefaultProp
    {
        public DefaultProp(string name)
        {
            this._name = name;
        }

        int _count = 0;
        public int Count
        {
            get { return _count; }
            set { _count = value; }
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


    ElementHost _eh;
    //private static System.Windows.Controls.TextBox avLbl;                        // our Avalon button
    private static System.Windows.Controls.Label s_avLbl;                        // our Avalon button
    private DefaultProp _currentProperty;
    
    protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
    {
        this.ClientSize = new System.Drawing.Size(300, 200);
        s_avLbl = new SWC.Label();
        
        s_avLbl.Content = "Avalon Button";
        _eh = new ElementHost();
        _eh.Child = s_avLbl;
        _eh.BackColor = System.Drawing.Color.Red;
        s_avLbl.Height = 50;
        s_avLbl.Width = 100;
        Controls.Add(_eh);
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
        PropertyMap dictionary = _eh.PropertyMap;
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
            _currentProperty = new DefaultProp(keys[i]);
            _eh.PropertyMap.ResetAll();
            _eh.PropertyMap.Remove((String)keys[i]);
            _eh.PropertyMap.Add((String)keys[i], new PropertyTranslator(OnPropChange));

            this.Text = keys[i]; 
            switch  (keys[i]) 
            {
                case "BackColor":
                    _eh.BackColor = System.Drawing.Color.Bisque;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackColor" ||  _currentProperty.Count != 2 || (System.Drawing.Color)_currentProperty.Val != System.Drawing.Color.Bisque)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString())); 
                        sr.IncCounters(false);
                    }
                    break;

                case "BackgroundImage":
                        bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                        _eh.BackgroundImage = bmp;
                        Utilities.SleepDoEvents(1, 100);
                        if (_currentProperty.Name != "BackgroundImage" || _currentProperty.Count != 2 ||   (System.Drawing.Bitmap)_currentProperty.Val != bmp)
                        {
                           p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                           sr.IncCounters(false);
                        }
                        _eh.BackgroundImage = null;
                    break;

                case "BackgroundImageLayout":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    _eh.BackgroundImage = bmp;
                    _eh.BackgroundImageLayout = ImageLayout.Stretch;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackgroundImageLayout" || _currentProperty.Count != 2 || (ImageLayout)_currentProperty.Val != ImageLayout.Stretch)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.BackgroundImage = null;
                    _eh.BackgroundImageLayout = ImageLayout.None;
                    break;

                case "Cursor":
                    _eh.Cursor = System.Windows.Forms.Cursors.Hand;
                    Utilities.SleepDoEvents(10);
                    if (_currentProperty.Name != "Cursor" || _currentProperty.Count != 2 || (System.Windows.Forms.Cursor)_currentProperty.Val != System.Windows.Forms.Cursors.Hand)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.Cursor = Cursors.Arrow;
                    break;

                case "Enabled":
                    _eh.Enabled = false;
                    Utilities.SleepDoEvents(10);
                    if (_currentProperty.Name != "Enabled" || _currentProperty.Count != 2 || (bool)_currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Enabled = true; //reset 
                    break;

                case "Font":
                    Utilities.SleepDoEvents(10);
                    _eh.Font = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                    Font fnt = (Font)_currentProperty.Val;
                    if (_currentProperty.Name != "Font" || _currentProperty.Count != 2)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Font = fnt; // Reset to original 
                    break;

                case "RightToLeft":
                    Utilities.SleepDoEvents(10);
                    _eh.RightToLeft = RightToLeft.Yes;
                    if (_currentProperty.Name != "RightToLeft" || _currentProperty.Count != 2 || (SWF.RightToLeft)_currentProperty.Val != RightToLeft.Yes)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.RightToLeft = RightToLeft.No;
                    break;

                case "Visible":
                    Utilities.SleepDoEvents(10);
                    _eh.Visible = false;
                    if (_currentProperty.Name != "Visible" || _currentProperty.Count != 2 || (bool)_currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Visible = true;
                    break;
            }
            Utilities.SleepDoEvents(1, 100);
            _currentProperty  = null;
        }
        sr.IncCounters(true);
      
        return sr;
    }


    [Scenario("Add a delegate and verify delegate function is called.")]
    public ScenarioResult AddDelegate(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropertyMap dictionary = _eh.PropertyMap;
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
            _currentProperty = new DefaultProp(keys[i]);
            _eh.PropertyMap.ResetAll();
            _eh.PropertyMap[keys[i]] += new PropertyTranslator(OnPropChange);

            this.Text = keys[i];
            switch (keys[i])
            {
                case "BackColor":
                    _eh.BackColor = System.Drawing.Color.Bisque;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackColor" || _currentProperty.Count != 2 || (System.Drawing.Color)_currentProperty.Val != System.Drawing.Color.Bisque)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }                    
                    break;

                case "BackgroundImage":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    _eh.BackgroundImage = bmp;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackgroundImage" || _currentProperty.Count != 2 || (System.Drawing.Bitmap)_currentProperty.Val != bmp)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.BackgroundImage = null;
                    break;

                case "BackgroundImageLayout":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    _eh.BackgroundImage = bmp;
                    _eh.BackgroundImageLayout = ImageLayout.Stretch;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackgroundImageLayout" || _currentProperty.Count != 2 || (ImageLayout)_currentProperty.Val != ImageLayout.Stretch)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.BackgroundImage = null;
                    _eh.BackgroundImageLayout = ImageLayout.None;
                    break;

                case "Cursor":
                    _eh.Cursor = System.Windows.Forms.Cursors.Hand;
                    Utilities.SleepDoEvents(10);
                    if (_currentProperty.Name != "Cursor" || _currentProperty.Count != 2 || (System.Windows.Forms.Cursor)_currentProperty.Val != System.Windows.Forms.Cursors.Hand)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.Cursor = Cursors.Arrow;
                    break;

                case "Enabled":
                    _eh.Enabled = false;
                    Utilities.SleepDoEvents(10);
                    if (_currentProperty.Name != "Enabled" || _currentProperty.Count != 2 || (bool)_currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Enabled = true; //reset 
                    break;

                case "Font":
                    Utilities.SleepDoEvents(10);
                    _eh.Font = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                    Font fnt = (Font)_currentProperty.Val;
                    
                    if (_currentProperty.Name != "Font" || _currentProperty.Count != 2)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Font = fnt; // Reset to original 
                    break;

                case "RightToLeft":
                    Utilities.SleepDoEvents(10);
                    _eh.RightToLeft = RightToLeft.Yes;
                    if (_currentProperty.Name != "RightToLeft" || _currentProperty.Count != 2 || (SWF.RightToLeft)_currentProperty.Val != RightToLeft.Yes)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.RightToLeft = RightToLeft.No;
                    break;

                case "Visible":
                    Utilities.SleepDoEvents(10);
                    _eh.Visible = false;
                    if (_currentProperty.Name != "Visible" || _currentProperty.Count != 2 || (bool)_currentProperty.Val != false)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Visible = true;
                    break;
            }
            _currentProperty = null;
            _eh.PropertyMap.ResetAll();
        }
        Utilities.SleepDoEvents(1, 100);
        sr.IncCounters(true);
        return sr;
    }


    [Scenario("Remove the  property and verify that PT is not called.")]
    public ScenarioResult RemovePropTranslator(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        PropertyMap dictionary = _eh.PropertyMap;
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
            _currentProperty = new DefaultProp(keys[i]);
            _eh.PropertyMap.ResetAll();
            _eh.PropertyMap.Remove(keys[i]);

            this.Text = keys[i]; 
            switch (keys[i])
            {
                case "BackColor":
                    _eh.BackColor = System.Drawing.Color.Bisque;
                    Utilities.SleepDoEvents(1, 100);
                    try
                    {
                        if (_currentProperty.Name != "BackColor" || _currentProperty.Count != 0 || _currentProperty.Val != null)
                        {
                            p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
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
                    _eh.BackgroundImage = bmp;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackgroundImage" || _currentProperty.Count != 0 || _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.BackgroundImage = null;
                    break;

                case "BackgroundImageLayout":
                    bmp = new System.Drawing.Bitmap("GreenStone.bmp");
                    _eh.BackgroundImage = bmp;
                    _eh.BackgroundImageLayout = ImageLayout.Stretch;
                    Utilities.SleepDoEvents(1, 100);
                    if (_currentProperty.Name != "BackgroundImageLayout" ||  _currentProperty.Count != 0 || _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.BackgroundImage = null;
                    _eh.BackgroundImageLayout = ImageLayout.None;
                    break;

                case "Cursor":
                    _eh.Cursor = System.Windows.Forms.Cursors.Hand;
                    Utilities.SleepDoEvents(10);
                    if (_currentProperty.Name != "Cursor" || _currentProperty.Count != 0  ||  _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1}", _currentProperty.Name, _currentProperty.Count.ToString()));
                        sr.IncCounters(false);
                    }
                    _eh.Cursor = Cursors.Arrow;
                    break;

                case "Enabled":
                    _eh.Enabled = false;
                    Utilities.SleepDoEvents(10);
                    if (_currentProperty.Name != "Enabled" || _currentProperty.Count != 0 || _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Enabled = true; //reset 
                    break;


                case "Font":
                    Utilities.SleepDoEvents(10);
                    _eh.Font = new Font(System.Drawing.FontFamily.GenericMonospace, 10);
                    Font fnt = (Font)_currentProperty.Val;
                    
                    if (_currentProperty.Name != "Font" || _currentProperty.Count != 0 || _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    _eh.Font = fnt; // Reset to original 
                    break;

                case "RightToLeft":
                    Utilities.SleepDoEvents(10);
                    _eh.RightToLeft = RightToLeft.Yes;
                    if (_currentProperty.Name != "RightToLeft" || _currentProperty.Count != 0 || _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        sr.IncCounters(false);
                    }
                    break;

                case "Visible":
                    Utilities.SleepDoEvents(10);
                    _eh.Visible = false;
                    if (_currentProperty.Name != "Visible" || _currentProperty.Count != 0 || _currentProperty.Val != null)
                    {
                        p.log.WriteLine(String.Format("Current Property Name: {0}, Count: {1} Value: ", _currentProperty.Name, _currentProperty.Count.ToString()), _currentProperty.Val.ToString());
                        _eh.RightToLeft = RightToLeft.No;
                    }
                    _eh.Visible = true;
                    break;
            }
            _currentProperty = null;
        }
        Utilities.SleepDoEvents(1, 100);
        sr.IncCounters(true);
        return sr;
    }
    #endregion

    public void OnPropChange(object host, String propertyName, object val)
    {
        _currentProperty.Name = propertyName;
        _currentProperty.Count++;
        _currentProperty.Val = val;
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Override and verify overridden function is called.
//@ Add a delegate and verify delegate function is called.
//@ Remove the  property and verify that PT is not called.
