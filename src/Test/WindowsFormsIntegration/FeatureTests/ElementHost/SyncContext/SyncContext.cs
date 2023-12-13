// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using SWF=System.Windows.Forms;
using SWC=System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

/// <TestCase>
/// SyncContext
/// </TestCase>
/// <summary>
/// Make sure that the sync context doesn't change when we add a host.
/// </summary>
public class SyncContext : ReflectBase
{
    #region Testcase setup
    public SyncContext(string[] args) : base(args) { }

    // class vars
    private ElementHost _eh1;
    private SWC.Button _ehBtn;
    List<Type> _panelTypes;
    Window _window;
    Form _f2 ;
    

    protected override void InitTest(TParams p)
    {
        this.Size = new System.Drawing.Size(500, 500);
        _panelTypes = GetDerivedType("System.Windows.Forms.dll", typeof(Panel));
        base.InitTest(p);
    }
    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
         Controls.Clear();
         this.Text = "";
         base.AfterScenario(p, scenario, result);
    }
    private void SleepUpdateUI(int seconds)
    {
        while (true)
        {
            System.Windows.Forms.Application.DoEvents();
            System.Threading.Thread.Sleep(1000);
            seconds--;
            if (seconds <= 0) 
                break;
        }
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("SynchronizationContext.Current shouldn't change when we add an EH. Test with different panels as container of EH.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        // update app title bar and log file
        try
        {
            foreach (Type t in _panelTypes)
            {
                int i = t.GetConstructors().Length;
                if (t == typeof(TabPage) || t.GetConstructors().Length == 0 || t.IsAbstract)
                    continue;

                Object o;
                try
                {
                    o = Activator.CreateInstance(t);
                }
                catch (MissingMethodException )
                {
                    continue;

                }

                string str = string.Format("Container type: {0}", t.ToString());
                str = str.Substring(str.LastIndexOf(".") + 1);
                this.Text = str;
                p.log.WriteLine(str);

                Panel panel = o as Panel;
                panel.Dock = DockStyle.Fill;
                panel.Visible = true;
                panel.BackColor = Color.Crimson;
                

                // save current context
                System.Threading.SynchronizationContext scBefore = System.Threading.SynchronizationContext.Current;
                p.log.WriteLine("Current context '{0}'", scBefore.ToString());
                
                _ehBtn = new SWC.Button();
                _ehBtn.Content = str;
                
                //Add the ElementHost control
                _eh1 = new ElementHost();
                _eh1.Child = _ehBtn;
                _eh1.Size = new System.Drawing.Size(100, 100);
                _eh1.BackColor = Color.BlanchedAlmond;
                panel.Controls.Add(_eh1);
                this.Controls.Add(panel);

                Utilities.SleepDoEvents(1, 500);
                // get current context after adding ElementHost.
                System.Threading.SynchronizationContext scAfter = System.Threading.SynchronizationContext.Current;
                p.log.WriteLine("Current context '{0}'", scAfter.ToString());

                // make sure contexts match
                bool b = scBefore.Equals(scAfter);
                p.log.WriteLine("Matches = {0}", b);
                sr.IncCounters(b, "Synchronization Context does not match after adding Element Host.", p.log);
                Utilities.SleepDoEvents(1, 1000);
                Controls.Clear();
            }

            
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        return sr;
    }
    
    [Scenario("@SynchronizationContext.Current shouldn't change when we add an EH. Test with form as container of EH.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            // save current context
            System.Threading.SynchronizationContext scBefore = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scBefore.ToString());

            _ehBtn = new SWC.Button();
            _ehBtn.Content = "I am a Button";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = Color.DarkSeaGreen;
            _eh1.Child = _ehBtn;
            this.Controls.Add(_eh1);
            
            // get current context after adding ElementHost.
            System.Threading.SynchronizationContext scAfter = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scAfter.ToString());

            // make sure contexts match
            bool b = scBefore.Equals(scAfter);
            p.log.WriteLine("Matches = {0}", b);
            sr.IncCounters(b, "Synchronization Context does not match after adding Element Host.", p.log);

        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        return sr;
    }

    
    [Scenario("AV control that starts a new Form (nested pump)")]
    public ScenarioResult Scenario3(TParams p)
    {
        
        
        
        ScenarioResult sr = new ScenarioResult();
        try
        {
            // save current context
            System.Threading.SynchronizationContext scBefore = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scBefore.ToString());

            _ehBtn = new SWC.Button();
            _ehBtn.Content = "I am a Button";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Child = _ehBtn;
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = Color.DarkSeaGreen;

            _ehBtn.Click += new RoutedEventHandler(_ehBtn_ClickScenario3);
            _ehBtn_ClickScenario3(_ehBtn, null);
            Utilities.SleepDoEvents(2, 500);
            // get current context after adding ElementHost.
            System.Threading.SynchronizationContext scAfter = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scAfter.ToString());

            // make sure contexts match
            bool b = scBefore.Equals(scAfter);
            p.log.WriteLine("Matches = {0}", b);
            sr.IncCounters(b, "Synchronization Context does not match after adding Element Host.", p.log);
            _f2.Close();
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        
        return sr;
    }

    void _ehBtn_ClickScenario3(object sender, RoutedEventArgs e)
    {
        ElementHost eh2 = new ElementHost();
        System.Windows.Controls.CheckBox chkBox = new System.Windows.Controls.CheckBox();
        chkBox.Content = "Check Me";
        eh2.Child = chkBox;
        _f2 = new Form();
        this.AddOwnedForm(_f2);
        _f2.Controls.Add(eh2);
        _f2.Text = "Form2";
        _f2.Show();
    }

    [Scenario("AV control that starts a new Window(nested pump)")]
    public ScenarioResult Scenario4(TParams p)
    {
        
        ScenarioResult sr = new ScenarioResult();
        try
        {
            // save current context
            System.Threading.SynchronizationContext scBefore = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scBefore.ToString());

            _ehBtn = new SWC.Button();
            _ehBtn.Content = "I am a Button";
            //Add the ElementHost control
            _eh1 = new ElementHost();
            _eh1.Child = _ehBtn;
            _eh1.Size = new System.Drawing.Size(200, 200);
            _eh1.BackColor = Color.DarkSeaGreen;
            this.Controls.Add(_eh1);

            _ehBtn.Click += new RoutedEventHandler(_ehBtn_ClickScenario4);
            _ehBtn_ClickScenario4(_ehBtn, null);
            Utilities.SleepDoEvents(2, 500);
            
            // get current context after adding ElementHost.
            System.Threading.SynchronizationContext scAfter = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scAfter.ToString());

            // make sure contexts match
            bool b = scBefore.Equals(scAfter);
            p.log.WriteLine("Matches = {0}", b);
            sr.IncCounters(b, "Synchronization Context does not match after adding Element Host.", p.log);
            Utilities.SleepDoEvents(1, 500);
            _window.Close();
        }
        catch (Exception ex)
        {
            log.WriteLine("Exception occured: " + ex.ToString());
        }
        
        return sr;
    }


    void _ehBtn_ClickScenario4(object sender, RoutedEventArgs e)
    {
        _window = new Window();
        _window.Show();
    }


    #endregion

    #region Helper Functions
    /// <summary>
    /// Helper function to set up app for particular Scenario
    /// </summary>
    /// <param name="p"></param>
    /// <param name="contType"></param>

    static void LoadAssemblies()
    {
        String[] assemblies = {"System.Windows.Forms,      PublicKeyToken={0}"};

        String EcmaPublicKeyToken = "b77a5c561934e089";
        String MSPublicKeyToken = "b03f5f7f11d50a3a";

        // Get the version of the assembly containing System.Object
        // We'll assume the same version for all the other assemblies
        Version version =
           typeof(System.Object).Assembly.GetName().Version;

        // Explicitly load the assemblies that we want to reflect over
        foreach (String a in assemblies)
        {
            String AssemblyIdentity =
               String.Format(a, EcmaPublicKeyToken, MSPublicKeyToken) +
               ", Culture=neutral, Version=" + version;

            Assembly.Load(AssemblyIdentity);
        }
    }


    List<Type> GetDerivedType(string assembly, Type requestedType)
    {
        List<Type> list = new List<Type>();

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName.Contains("System.Windows.Forms") == false)
                continue;

            foreach (Type t in a.GetTypes())
            {
                if (!t.IsClass || !t.IsPublic) continue;
                Boolean derivedFromRequestedType = false;
                Type baseType = t.BaseType;
                while ((baseType != null) && !derivedFromRequestedType)
                {
                    // Append the base type to the end of the string
                    //typeHierarchy.Append("-" + baseType);
                    
                    derivedFromRequestedType = (baseType == requestedType);
                    baseType = baseType.BaseType;
                }

                if (!derivedFromRequestedType) continue;
                list.Add(t);
            }
        }

        return list;

    }


    #endregion
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@SynchronizationContext.Current shouldn't change when we add an EH. Test with different panels as container of EH.
//@SynchronizationContext.Current shouldn't change when we add an EH. Test with form as container of EH.
//@AV control that starts a new Form (nested pump)
//@AV control that starts a new Window(nested pump)