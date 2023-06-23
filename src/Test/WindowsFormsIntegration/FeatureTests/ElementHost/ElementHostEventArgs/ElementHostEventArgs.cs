using System;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.Integration;
using System.ComponentModel;
using System.Text;

/// <summary>
/// Testcase:    ElementHostEventArgs
/// Description: Verify that WPF element host raises and interprets its events correctly.
/// Author:      gregfra
/// </summary>
public class ElementHostEventArgs : ReflectBase
{
    /// <summary>
    /// List of events raised by test.
    /// </summary>
    private StringBuilder eventsTrace = null;
    
    /// <summary>
    /// Flag - was the previous root null?
    /// </summary>
    private bool foundOneNull = false;
    
    /// <summary>
    /// Flag - was the previous root an actual element tree?
    /// </summary>
    private bool foundPreviousRoot = false;
    
    /// <summary>
    /// WPF button created to be available throughout class.
    /// </summary>
    private System.Windows.Controls.Button sharedWpfButton;

    #region Testcase setup
    public ElementHostEventArgs(string[] args) : base(args) { }

    [STAThread]
    public static void Main(string[] args)
    {
        Application.Run(new ElementHostEventArgs(args));
    }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        eventsTrace = new StringBuilder();
        sharedWpfButton = new System.Windows.Controls.Button();

        return base.BeforeScenario(p, scenario);
    }
    #endregion

    #region Scenarios

    [Scenario("Verify ChildChanged event and ChildChangedEventArgs event args when an EH changes its child twice")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult result = new ScenarioResult(true);

        //create an elementHost and listen for events 
        ElementHost elementHost = new ElementHost();
        EventListener elementHostListener = new EventListener(elementHost, "ChildChanged");
        elementHostListener.EventFired += new EventHandler<EventFiredEventArgs>(childChangedEventRaised);

        //create a second WPF button and listen for event
        System.Windows.Controls.Button wpfButton2 = new System.Windows.Controls.Button();

        //put the elementHost on a new Windows Form, host the WPF button, host the second WPF button
        Form secondForm = new Form();
        secondForm.Controls.Add(elementHost);
        elementHost.Child = sharedWpfButton;
        secondForm.Show();
        Application.DoEvents();
        
        elementHost.Child = wpfButton2;
        Application.DoEvents();
        
        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        //validate
        bool expected = (eventsTrace.ToString() == "ChildChanged:ChildChanged:") && foundOneNull && foundPreviousRoot;

        //validate the order of events (for both ElemnentHost and WPF Button)
        result.IncCounters(expected, 
                        "Unexpected events were raised. Raised events were: " + eventsTrace.ToString() + 
                            "; foundOneNull="+ foundOneNull + 
                            " (expected true);foundPreviousRoot="+foundPreviousRoot+" (expected true)",
                        p.log );

        elementHostListener.Clear();
        elementHostListener.Dispose();

        return result;
    }
    
    #endregion
    
    #region Helper methods

    void childChangedEventRaised(object sender, EventFiredEventArgs e)
    {
        //we handle this because we want to merge the events for elementHost and for the hosted control. 
        eventsTrace.Append(e.EventInfo.EventName + ":");

        if (e.EventInfo.EventName == "ChildChanged")
        {
            ChildChangedEventArgs args = (ChildChangedEventArgs)e.EventInfo.E;
            if (!foundOneNull)
            {
                if (args.PreviousChild == null)
                {
                    foundOneNull = true;
                }
            }
            foundPreviousRoot = (args.PreviousChild == sharedWpfButton);
        }
    }

    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify ChildChanged event and ChildChangedEventArgs event args when an EH changes its child twice
