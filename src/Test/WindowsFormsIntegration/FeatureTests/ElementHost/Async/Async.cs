
using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using SWC = System.Windows.Controls;
using SWM = System.Windows.Media;
using System.Windows.Forms.Integration;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reflection;

///
/// <Testcase>
/// Async
/// </Testcase>    
/// 
/// <summary>
/// Verify that Async operations work on a Avalon control hosted in EHameerm
/// </summary>
/// <history>
///  [sameerm]   4/4/2006   Created
/// </history>


public class Async : ReflectBase 
{

    #region Testcase setup
    public Async(string[] args) : base(args) { }


    protected override void InitTest(TParams p) {

        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    protected override void AfterScenario(TParams p, MethodInfo scenario, ScenarioResult result)
    {
        Controls.Clear();
        avButton = null;
        elementHost = null;
        base.AfterScenario(p, scenario, result);
    }
    [Scenario("Host an Avalon Button in an EH. On a worker thread user call Dispatcher.BeginInvoke to update the Avalon control hosted in EH. Verify that you can change change the hosted controls props/methodsand the EH properties.")]
    public ScenarioResult Scenario1(TParams p) {
        ScenarioResult sr = new  ScenarioResult();
        
        elementHost = new ElementHost();
        avButton = new System.Windows.Controls.Button();
        useInvoke = false;  
        
        //Avalon Button
        elementHost.Child = avButton; //Host it on the ElementHost
        Controls.Add(elementHost);
        
        dispatcher = avButton.Dispatcher;  // Get the Buttons Dispatcher object
        t = new Thread(new ThreadStart(WorkerMethod)); //Start a new thread
        t.Start();
	
        Utilities.SleepDoEvents(2, 1000); //Wait for worker thread to complete

        if (elementHost.Size != newSize || avButton.Background != System.Windows.Media.Brushes.BlanchedAlmond)
        {
            p.log.WriteLine("Element Host Size after return from Async function: Expected: {0} Actual {1}: ", newSize.ToString(), elementHost.Size.ToString());
            p.log.WriteLine("AV Button Background after return from Async function: Expected: {0} Actual {1}: ",  System.Windows.Media.Brushes.BlanchedAlmond.ToString() ,avButton.Background.ToString());
            sr.IncCounters(false);
        }
        else
            sr.IncCounters(true);
        
        
        return sr; 
    }

    [Scenario("Host an Avalon Button in an EH. On a worker thread user call Dispatcher.Invoke to update the Avalon control hosted in EH. Verify that you can change change the hosted controls props/methods and the EH properties.")]
    public ScenarioResult Scenario2(TParams p) 
    {
        ScenarioResult sr = new  ScenarioResult();
        avButton = new System.Windows.Controls.Button();
        elementHost = new ElementHost();
        useInvoke = true;

        //Avalon Button
        elementHost.Child = avButton; //Host it on the ElementHost
        Controls.Add(elementHost);

        dispatcher = avButton.Dispatcher;  // Get the Buttons Dispatcher object
        t = new Thread(new ThreadStart(WorkerMethod)); //Start a new thread
        t.Start();
        Utilities.SleepDoEvents(1, 1000); //Wait for worker thread to complete

        if (elementHost.Size != newSize || avButton.Background != System.Windows.Media.Brushes.BlanchedAlmond)
        {
            p.log.WriteLine("Element Host Size after return from Async function: Expected: {0} Actual {1}: ", newSize.ToString(), elementHost.Size.ToString());
            p.log.WriteLine("AV Button Background after return from Async function: Expected: {0} Actual {1}: ",  System.Windows.Media.Brushes.BlanchedAlmond.ToString() ,avButton.Background.ToString());
            sr.IncCounters(false);
        }
        else
            sr.IncCounters(true);

        return sr;
    }

    #endregion

    #region DISPATCHER
    private delegate void ChangeProp();
    public void WorkerMethod()
    {
        ChangeProp ChangeContolProp = new ChangeProp(ChangeButtonProp); 
        if (useInvoke)
            dispatcher.Invoke(DispatcherPriority.Normal, ChangeContolProp);
        else
            dispatcher.BeginInvoke(DispatcherPriority.Normal, ChangeContolProp);
    }

    private void ChangeButtonProp()
    {
        this.avButton.Background = SWM.Brushes.BlanchedAlmond;
        this.elementHost.Size = newSize; // Change the size
    }

    #endregion

    Size newSize = new Size(100, 100);
    protected Dispatcher dispatcher;
    ElementHost elementHost ;
    SWC.Button avButton ; 
    bool useInvoke;
    private Thread t;
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Host an Avalon Button in an EH. On a worker thread user call Dispatacher.BeginInvoke to update the Avalon control hosted in EH. Verify that you can change change the hosted controls props/methods and the EH properties.
//@ Host an Avalon Button in an EH. On a worker thread user call Dispatacher.Invoke to update the Avalon control hosted in EH. Verify that you can change change the hosted controls props/methods and the EH properties.