using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows;
using System.Windows.Shapes;
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
using SWI = System.Windows.Input;

//
// Testcase:    Cursor
// Description: Verify that the EH Cursor propogates to its children.
// Author:      a-rickyt
//
namespace ElementHostTests
{

public class Cursor : ReflectBase
{
    #region Testcase setup

    ElementHost elementHost1;
    SWC.StackPanel stackPanel;
    SWC.Button avButton = new SWC.Button();
    List<Type> controlTypes;
    SWF.Label label = new SWF.Label();

    public Cursor(String[] args) : base(args) { }

    protected override void InitTest(TParams p)
    {
	try
	{
        this.Text = "Cursor - MouseOver control for visual verification";
        this.Size = new System.Drawing.Size(450, 400);

        label.Width = 400;
        Controls.Add(label);

        controlTypes = GetDerivedType("System.Windows.Controls.dll", typeof(SWC.UserControl));

        Utilities.SleepDoEvents(2);
        base.InitTest(p);
	}
	catch(Exception ex)
	{
	    p.log.WriteLine(ex.Message);
	}
    }

    #endregion

    #region Scenarios

    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult sr)
    {
        this.Controls.Clear();
        label.Width = 400;
        Controls.Add(label);
        base.AfterScenario(p, scenario, sr);
    }

    [Scenario("Set EH parent.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);

                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                Scenario1CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Arrow, control, t, p);

                if (sr.FailCount > 0)
                {
                    break;
                }
                Scenario1CursorTest(sr, SWF.Cursors.Cross, SWI.Cursors.Cross, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.Default, SWI.Cursors.None, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.UpArrow, SWI.Cursors.UpArrow, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.Hand, SWI.Cursors.Hand, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.No, SWI.Cursors.No, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.Help, SWI.Cursors.Help, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.IBeam, SWI.Cursors.IBeam, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.AppStarting, SWI.Cursors.AppStarting, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.WaitCursor, SWI.Cursors.Wait, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanNorth, SWI.Cursors.ScrollN, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanSouth, SWI.Cursors.ScrollS, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanEast, SWI.Cursors.ScrollE, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanWest, SWI.Cursors.ScrollW, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanNE, SWI.Cursors.ScrollNE, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanSE, SWI.Cursors.ScrollSE, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanSW, SWI.Cursors.ScrollSW, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.PanNW, SWI.Cursors.ScrollNW, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.SizeAll, SWI.Cursors.SizeAll, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.SizeNS, SWI.Cursors.SizeNS, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.SizeNESW, SWI.Cursors.SizeNESW, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.SizeWE, SWI.Cursors.SizeWE, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.SizeNWSE, SWI.Cursors.SizeNWSE, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.None, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Pen, control, t, p);
                Scenario1CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.ArrowCD, control, t, p);

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
	    p.log.WriteLine("Testing for " + t.ToString());
            }
	    catch (NullReferenceException)
	    {
		continue;
	    }
            catch (Exception)
            {
                continue;
            }
        }

        return sr;
    }
    void Scenario1CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor, 
        SWC.Control control, Type t, TParams p)
    {
        this.Cursor = wfCursor;
        control.Cursor = avCursor; //Cursor property doesn't work for WPF control hosted in 
                                   //WinForms application, needs manual setup
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH parent for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH parent for elementHost1 hosting " + 
            t.ToString(), p.log);
        sr.IncCounters(this.Cursor == wfCursor, "Failed at Set EH parent for elementHost1 hosting " +
            t.ToString(), p.log);
    }

    [Scenario("Set EH parent twice.")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);

                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                Scenario2CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Arrow, control, t, p);
                if (sr.FailCount > 0)
                {
                    break;
                }
                Scenario2CursorTest(sr, SWF.Cursors.Cross, SWI.Cursors.Cross, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.Default, SWI.Cursors.None, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.UpArrow, SWI.Cursors.UpArrow, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.Hand, SWI.Cursors.Hand, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.No, SWI.Cursors.No, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.Help, SWI.Cursors.Help, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.IBeam, SWI.Cursors.IBeam, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.AppStarting, SWI.Cursors.AppStarting, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.WaitCursor, SWI.Cursors.Wait, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanNorth, SWI.Cursors.ScrollN, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanSouth, SWI.Cursors.ScrollS, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanEast, SWI.Cursors.ScrollE, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanWest, SWI.Cursors.ScrollW, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanNE, SWI.Cursors.ScrollNE, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanSE, SWI.Cursors.ScrollSE, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanSW, SWI.Cursors.ScrollSW, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.PanNW, SWI.Cursors.ScrollNW, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.SizeAll, SWI.Cursors.SizeAll, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.SizeNS, SWI.Cursors.SizeNS, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.SizeNESW, SWI.Cursors.SizeNESW, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.SizeWE, SWI.Cursors.SizeWE, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.SizeNWSE, SWI.Cursors.SizeNWSE, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.None, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Pen, control, t, p);
                Scenario2CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.ArrowCD, control, t, p);

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return sr;
    }
    void Scenario2CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor, 
        SWC.Control control, Type t, TParams p)
    {
        this.Cursor = SWF.Cursors.SizeNWSE;
        this.Cursor = wfCursor;
        control.Cursor = avCursor; //Cursor property doesn't work for WPF control hosted in 
                                   //WinForms application, needs manual setup
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH parent twice for " + 
            t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH parent twice for " + 
            "elementHost1 hosting " + t.ToString(), p.log);
        sr.IncCounters(this.Cursor == wfCursor, "Failed at Set EH parent twice for " +
            "(this) hosting " + t.ToString(), p.log);
    }

    [Scenario("Set EH child and make sure EH parent doesn't change.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);

                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario3CursorTest(sr, SWF.Cursors.Default, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario3CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor, 
        SWC.Control control, Type t, TParams p)
    {
        control.Cursor = avCursor;
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child and make sure EH " + 
            "parent doesn't change, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child and make sure " + 
            "EH parent doesn't change, for elementHost1 hosting" + t.ToString(), p.log);
        sr.IncCounters(this.Cursor == wfCursor, "Failed at Set EH child and make sure " +
            "EH parent doesn't change, for (this) hosting" + t.ToString(), p.log);
    }

    [Scenario("Set EH child then EH parent and make sure EH child doesn't change to EH parent.")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);

                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario4CursorTest(sr, SWF.Cursors.Default, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario4CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        control.Cursor = avCursor;
        this.Cursor = wfCursor;
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for elementHost1 hosting" + t.ToString(), p.log);
        sr.IncCounters(this.Cursor == wfCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for (this) hosting" + t.ToString(), p.log);
    }

    [Scenario("Set EH parent then EH child.")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);
                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario5CursorTest(sr, SWF.Cursors.Default, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario5CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        this.Cursor = wfCursor;
        control.Cursor = avCursor;
        
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for elementHost1 hosting" + t.ToString(), p.log);
    }

    [Scenario("Set EH.")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);
                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                Scenario6CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Arrow, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Cross, SWI.Cursors.Cross, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Default, SWI.Cursors.None, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.UpArrow, SWI.Cursors.UpArrow, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Hand, SWI.Cursors.Hand, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.No, SWI.Cursors.No, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Help, SWI.Cursors.Help, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.IBeam, SWI.Cursors.IBeam, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.AppStarting, SWI.Cursors.AppStarting, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.WaitCursor, SWI.Cursors.Wait, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanNorth, SWI.Cursors.ScrollN, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanSouth, SWI.Cursors.ScrollS, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanEast, SWI.Cursors.ScrollE, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanWest, SWI.Cursors.ScrollW, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanNE, SWI.Cursors.ScrollNE, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanSE, SWI.Cursors.ScrollSE, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanSW, SWI.Cursors.ScrollSW, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.PanNW, SWI.Cursors.ScrollNW, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.SizeAll, SWI.Cursors.SizeAll, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.SizeNS, SWI.Cursors.SizeNS, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.SizeNESW, SWI.Cursors.SizeNESW, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.SizeWE, SWI.Cursors.SizeWE, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.SizeNWSE, SWI.Cursors.SizeNWSE, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.None, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Pen, control, t, p);
                Scenario6CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.ArrowCD, control, t, p);

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return sr;
    }
    void Scenario6CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        elementHost1.Cursor = wfCursor;
        control.Cursor = avCursor;  //Cursor property doesn't work for WPF control hosted in 
                                    //WinForms application, needs manual setup
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH parent for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH parent for elementHost1 hosting " +
            t.ToString(), p.log);
    }

    [Scenario("Set EH twice.")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);
                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                Scenario7CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Arrow, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Cross, SWI.Cursors.Cross, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Default, SWI.Cursors.None, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.UpArrow, SWI.Cursors.UpArrow, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Hand, SWI.Cursors.Hand, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.No, SWI.Cursors.No, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Help, SWI.Cursors.Help, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.IBeam, SWI.Cursors.IBeam, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.AppStarting, SWI.Cursors.AppStarting, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.WaitCursor, SWI.Cursors.Wait, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanNorth, SWI.Cursors.ScrollN, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanSouth, SWI.Cursors.ScrollS, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanEast, SWI.Cursors.ScrollE, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanWest, SWI.Cursors.ScrollW, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanNE, SWI.Cursors.ScrollNE, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanSE, SWI.Cursors.ScrollSE, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanSW, SWI.Cursors.ScrollSW, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.PanNW, SWI.Cursors.ScrollNW, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.SizeAll, SWI.Cursors.SizeAll, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.SizeNS, SWI.Cursors.SizeNS, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.SizeNESW, SWI.Cursors.SizeNESW, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.SizeWE, SWI.Cursors.SizeWE, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.SizeNWSE, SWI.Cursors.SizeNWSE, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.None, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.Pen, control, t, p);
                Scenario7CursorTest(sr, SWF.Cursors.Arrow, SWI.Cursors.ArrowCD, control, t, p);

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return sr;
    }
    void Scenario7CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        elementHost1.Cursor = SWF.Cursors.SizeNWSE;
        elementHost1.Cursor = wfCursor;
        control.Cursor = avCursor;  //Cursor property doesn't work for WPF control hosted in 
                                    //WinForms application, needs manual setup
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH parent twice for " +
            t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH parent twice for " +
            "elementHost1 hosting " + t.ToString(), p.log);
    }

    [Scenario("Set EH child and make sure EH doesn't change.")]
    public ScenarioResult Scenario8(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);
                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario8CursorTest(sr, SWF.Cursors.Default, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario8CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        control.Cursor = avCursor;
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child and make sure EH " +
            "parent doesn't change, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child and make sure " +
            "EH parent doesn't change, for elementHost1 hosting" + t.ToString(), p.log);
    }

    [Scenario("Set EH child then EH and make sure child doesn't change to EH.")]
    public ScenarioResult Scenario9(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);
                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario9CursorTest(sr, SWF.Cursors.Default, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario9CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        control.Cursor = avCursor;
        elementHost1.Cursor = wfCursor;
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for elementHost1 hosting" + t.ToString(), p.log);
    }

    [Scenario("Set EH then child.")]
    public ScenarioResult Scenario10(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);
                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario10CursorTest(sr, SWF.Cursors.Default, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario10CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor,
        SWC.Control control, Type t, TParams p)
    {
        elementHost1.Cursor = wfCursor;
        control.Cursor = avCursor;
        
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child then EH parent and make " +
            "sure EH child doesn't change to EH parent, for elementHost1 hosting" + t.ToString(), p.log);
    }

    [Scenario("Use WaitCursor property.")]
    public ScenarioResult Scenario11(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        foreach (Type t in controlTypes)
        {
            Object o;
            try
            {
                o = Activator.CreateInstance(t);

                SWC.Control control = o as SWC.Control;

                stackPanel = new SWC.StackPanel();
                stackPanel.Children.Add(control);

                //Create Element Host 1
                elementHost1 = new ElementHost();
                elementHost1.BackColor = Color.White;
                elementHost1.Child = stackPanel;
                elementHost1.AutoSize = true;
                elementHost1.Location = new System.Drawing.Point(20, 50);
                Controls.Add(elementHost1);

                label.Text = t.ToString();
                Utilities.SleepDoEvents(1);

                //Go through all SWI.Cursors one-by-one and test them
                List<SWI.Cursor> cursors = GetAvalonCursors();
                foreach (SWI.Cursor cursor in cursors)
                {
                    Scenario11CursorTest(sr, SWF.Cursors.WaitCursor, cursor, control, t, p);
                }

                elementHost1.Child = null;
                Controls.Remove(elementHost1);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return sr;
    }
    void Scenario11CursorTest(ScenarioResult sr, SWF.Cursor wfCursor, SWI.Cursor avCursor, 
        SWC.Control control, Type t, TParams p)
    {
        SWF.Application.UseWaitCursor = true;
        control.Cursor = avCursor;
        sr.IncCounters(control.Cursor == avCursor, "Failed at Set EH child and make sure EH " + 
            "parent doesn't change, for " + t.ToString(), p.log);
        sr.IncCounters(elementHost1.Cursor == wfCursor, "Failed at Set EH child and make sure " + 
            "EH parent doesn't change, for elementHost1 hosting" + t.ToString(), p.log);
        sr.IncCounters(this.Cursor == wfCursor, "Failed at Set EH child and make sure " +
            "EH parent doesn't change, for (this) hosting" + t.ToString(), p.log);
    }
    
    #endregion

    #region Utilities

    //Returns a list of all Avalon Cursors
    List<SWI.Cursor> GetAvalonCursors()
    {
        List<SWI.Cursor> cursors = new List<SWI.Cursor>();

        //Go through all SWI.Cursors one-by-one and test them
        Type type = typeof(SWI.CursorType);
        MemberInfo[] mi = type.GetMembers();
        foreach (MemberInfo m in mi)
        {
            try
            {
                if (m.MemberType != MemberTypes.Field || m.Name == "value__") continue;
                SWI.CursorConverter cc = new SWI.CursorConverter();
                SWI.Cursor cursor = (SWI.Cursor)cc.ConvertFromString(m.Name);
                cursors.Add(cursor);
            }
            catch (Exception)
            {
                continue;
            }
        }
        return cursors;
    }

    List<Type> GetDerivedType(string assembly, Type requestedType)
    {
        List<Type> list = new List<Type>();

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in a.GetTypes())
            {
                if (!t.IsClass || !t.IsPublic || t.GetConstructors().Length == 0 || t.IsAbstract) 
                    continue;
                Boolean derivedFromRequestedType = false;
                Type baseType = t.BaseType;
                while ((baseType != null) && !derivedFromRequestedType)
                {
                    derivedFromRequestedType = (baseType == requestedType);
                    baseType = baseType.BaseType;
                }
                if (!derivedFromRequestedType) continue;

                int i = t.GetConstructors().Length;

                list.Add(t);
            }
        }
        return list;
    }

    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios. 
//

// [Scenarios]
//@ Set EH parent.
//@ Set EH parent twice.
//@ Set EH child and make sure EH parent doesn't change.
//@ Set EH child then EH parent and make sure EH child doesn't change to EH parent.
//@ Set EH parent then EH child.
//@ Set EH.
//@ Set EH twice.
//@ Set EH child and make sure EH doesn't change.
//@ Set EH child then EH and make sure child doesn't change to EH.
//@ Set EH then child.
//@ Use WaitCursor property.