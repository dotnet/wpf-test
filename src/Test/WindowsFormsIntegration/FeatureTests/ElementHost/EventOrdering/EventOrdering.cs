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

//
// Testcase:    EventOrdering
// Description: Verify that a Avalon control recieves all the expected during it's lifecycle
// Author:      bogdanbr
//
public class EventOrdering : ReflectBase
{
    private StringBuilder m_eventsTrace = null;

    #region Testcase setup
    public EventOrdering(string[] args) : base(args) { }

    [STAThread]
    public static void Main(string[] args)
    {
        Application.Run(new EventOrdering(args));
    }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);
    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        m_eventsTrace = new StringBuilder();

        return base.BeforeScenario(p, scenario);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify Events and Order when a Avalon control hosted in a EH and an Winform window is Created, Shown and Closed.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create an elementHost and listen for events 
        ElementHost elementHost = new ElementHost();
        EventListener elementHostListener = new EventListener(elementHost, "ParentChanged", "HandleCreated", "HandleDestroyed", "Disposed");
        elementHostListener.EventFired += new EventHandler<EventFiredEventArgs>(eventListener_EventFired);

        //create a button and listen for event
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();
        EventListener avBtnEventListener = new EventListener(avBtn, "Initialized", "Loaded", "Unloaded");
        avBtnEventListener.EventFired += new EventHandler<EventFiredEventArgs>(eventListener_EventFired);     
 
        //put the elementHost on a new Form, host the WPF button and show/close/dispose the form
        Form secondForm = new Form();
        secondForm.Controls.Add(elementHost);
        elementHost.Child = avBtn;
        secondForm.Show();
        Application.DoEvents();
        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();
        
        //validate the order of events (for both ElemnentHost and WPF Button)
        sr.IncCounters( m_eventsTrace.ToString() == "ParentChanged:Initialized:HandleCreated:Loaded:HandleDestroyed:Disposed:Unloaded:", 
                        "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(), 
                        p.log );

        elementHostListener.Clear();
        avBtnEventListener.Clear();
        elementHostListener.Dispose();
        avBtnEventListener.Dispose();

        return sr;
    }

    [Scenario("Verify Events and Order when an Avalon control is added and removed from a EH")]
    public ScenarioResult Scenario2(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create the needed controls 
        ElementHost elementHost = new ElementHost();
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();

        //put the elementHost on a new Form and show form
        Form secondForm = new Form();
        secondForm.Controls.Add(elementHost);
        Application.DoEvents();
        secondForm.Show();
        Application.DoEvents();
        
        //now start to listen for events 
        // - Exclude Mouse related Events 
        EventListener elementHostEventListener = GetEventsListener(elementHost, "MouseEnter", "MouseMove", "MouseLeave", "Paint");
        EventListener avBtnEventListener = GetEventsListener(avBtn, "MouseEnter", "QueryCursor", "PreviewMouseMove", "MouseMove", "MouseLeave", "LayoutUpdated");

        //host the WPF button into the elementHost
        elementHost.Child = avBtn;
        Application.DoEvents();

        //remove the WPF button from the elementHost. 
        elementHost.Child = null;
        Application.DoEvents();

        //validate the order of events (for both ElemnentHost and WPF Button)
        sr.IncCounters( m_eventsTrace.ToString() == "Initialized:IsVisible:ChildChanged:SizeChanged:Loaded:IsVisible:ChildChanged:Unloaded:",
                        "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(), 
                        p.log);

        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        elementHostEventListener.Clear();
        avBtnEventListener.Clear();
        elementHostEventListener.Dispose();
        avBtnEventListener.Dispose();

        //Failing this scenario due to a suggestion bug. If the bug it's approved, just return the sr - else, change the expected string. 
        return sr;
    }

    [Scenario("Verify Events and Order when a  EH with a Avalon control is added and removed from a Winform Window.")]
    public ScenarioResult Scenario3(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create the needed controls 
        ElementHost elementHost = new ElementHost();
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();

        //host the WPF button 
        elementHost.Child = avBtn;

        //create and show a new form
        Form secondForm = new Form();
        secondForm.Show();
        Application.DoEvents();

        //now start to listen for events 
        // - Exclude Mouse related Events 
        EventListener elementHostEventListener = GetEventsListener(elementHost, "MouseEnter", "MouseMove", "MouseHover", "MouseLeave", "Paint");
        EventListener avBtnEventListener = GetEventsListener(avBtn, "MouseEnter", "QueryCursor", "PreviewMouseMove", "MouseMove", "LayoutUpdated", "MouseLeave");
        EventListener formEventListener = GetEventsListener(secondForm, "MouseEnter", "MouseMove", "MouseHover", "MouseLeave", "Paint");
 
        //add the elementHost to the form 
        secondForm.Controls.Add(elementHost);
        Application.DoEvents();

        //validate the order of events(helps debugging and review if we do this after each action and then reset the m_eventsTrace)
        sr.IncCounters(m_eventsTrace.ToString() == "ParentChanged:IsVisible:SizeChanged:HandleCreated:BindingContextChanged:BindingContextChanged:BindingContextChanged:Layout:ControlAdded:Loaded:",
                       "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(), 
                       p.log );
        m_eventsTrace.Remove(0, m_eventsTrace.Length);

        //remove the elementHost from the form 
        secondForm.Controls.Remove(elementHost);
        Application.DoEvents();

        //validate the order of events
        sr.IncCounters( m_eventsTrace.ToString() == "IsVisible:VisibleChanged:IsVisible:VisibleChanged:ParentChanged:BindingContextChanged:Layout:ControlRemoved:",
                        "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(),
                        p.log);

        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        elementHostEventListener.Clear();
        avBtnEventListener.Clear();
        formEventListener.Clear();
        elementHostEventListener.Dispose();
        avBtnEventListener.Dispose();
        formEventListener.Dispose();

        return sr;
    }

    [Scenario("Verify Events and Order when a Winform window with a EH and Avalon control, is moved around")]
    public ScenarioResult Scenario4(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create the needed controls 
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();
        ElementHost elementHost = new ElementHost();
        elementHost.Child = avBtn;

        //create and show a new form
        Form secondForm = new Form();
        secondForm.Show();
        Application.DoEvents();

        //add the elementHost to the form 
        secondForm.Controls.Add(elementHost);
        Application.DoEvents();

        //now start to listen for events 
        // - Exclude Mouse related Events 
        EventListener elementHostEventListener = GetEventsListener(elementHost, "MouseEnter", "MouseMove");
        EventListener avBtnEventListener = GetEventsListener(avBtn, "MouseEnter", "QueryCursor", "PreviewMouseMove", "MouseMove", "MouseLeave");
        
        //move the form 
        secondForm.Location = new System.Drawing.Point(secondForm.Location.X + 2, secondForm.Location.Y + 2);
        Application.DoEvents();

        //validate the order of events (no events expected)
        sr.IncCounters( m_eventsTrace.ToString() == "",
                        "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(),
                        p.log);

        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        elementHostEventListener.Clear();
        avBtnEventListener.Clear();
        elementHostEventListener.Dispose();
        avBtnEventListener.Dispose();

        return sr;
    }

    [Scenario("Verify Events and Order when a Winform window with a EH and  Avalon control, is resized")]
    public ScenarioResult Scenario5(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create the needed controls 
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();
        ElementHost elementHost = new ElementHost();
        elementHost.Child = avBtn;

        //create and show a new form
        Form secondForm = new Form();
        secondForm.Show();
        Application.DoEvents();

        //add the elementHost to the form 
        secondForm.Controls.Add(elementHost);
        Application.DoEvents();

        //now start to listen for events 

        // - Exclude Mouse related Events 
        EventListener elementHostEventListener = GetEventsListener(elementHost, "MouseEnter", "MouseMove");
        EventListener avBtnEventListener = GetEventsListener(avBtn, "MouseEnter", "QueryCursor", "PreviewMouseMove", "MouseMove", "MouseLeave");

        //resize the form
        secondForm.Size = new System.Drawing.Size(secondForm.Size.Width - 5, secondForm.Size.Height - 5);
        Application.DoEvents();

        //validate the order of events
        sr.IncCounters(m_eventsTrace.ToString() == "",
                        "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(),
                        p.log);

        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        elementHostEventListener.Clear();
        avBtnEventListener.Clear();
        elementHostEventListener.Dispose();
        avBtnEventListener.Dispose();

        return sr;
    }

    [Scenario("Verify Events and Order when a Winform window with a EH and Avalon control, is maximized and minimized")]
    public ScenarioResult Scenario6(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create the needed controls 
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();
        ElementHost elementHost = new ElementHost();
        elementHost.Child = avBtn;

        //create and show a new form
        Form secondForm = new Form();
        secondForm.Show();
        Application.DoEvents();

        //add the elementHost to the form 
        secondForm.Controls.Add(elementHost);
        Application.DoEvents();

        //now start to listen for events 
        // - Exclude Mouse related Events 
        EventListener elementHostEventListener = GetEventsListener(elementHost, "MouseEnter", "MouseMove", "MouseLeave");
        EventListener avBtnEventListener = GetEventsListener(avBtn, "MouseEnter", "QueryCursor", "PreviewMouseMove", "MouseMove", "MouseLeave", "LayoutUpdated");

        //minimize/maximize
        secondForm.WindowState = FormWindowState.Minimized;
        Application.DoEvents();
        secondForm.WindowState = FormWindowState.Maximized;
        Application.DoEvents();

        //validate the order of events
        string expectedEventTrace = "PreviewGotKeyboardFocus:Enter:GotFocus:IsKeyboardFocusWithin:IsKeyboardFocused:RequestBringIntoView:GotFocus:GotKeyboardFocus:LostFocus:IsKeyboardFocusWithin:IsKeyboardFocused:LostKeyboardFocus:GotFocus:PreviewGotKeyboardFocus:PreviewGotKeyboardFocus:IsKeyboardFocusWithin:IsKeyboardFocused:GotKeyboardFocus:Paint:";
        sr.IncCounters(m_eventsTrace.ToString() == expectedEventTrace,
            
                        "Unexpected events were raised. Expected: "+expectedEventTrace+
			"Raised events were: " + m_eventsTrace.ToString(),
                        p.log);
        m_eventsTrace.Remove(0, m_eventsTrace.Length);
        
        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        elementHostEventListener.Clear();
        avBtnEventListener.Clear();
        elementHostEventListener.Dispose();
        avBtnEventListener.Dispose();

        return sr;
    }

    [Scenario("Verify Events and Order when a EH and Avalon control, is covered and not covered")]
    public ScenarioResult Scenario7(TParams p)
    {
        ScenarioResult sr = new ScenarioResult(true);

        //create the needed controls 
        System.Windows.Controls.Button avBtn = new System.Windows.Controls.Button();
        ElementHost elementHost = new ElementHost();
        elementHost.Child = avBtn;

        //create and show a new form
        Form secondForm = new Form();
        secondForm.Show();
        Application.DoEvents();

        //add the elementHost to the form 
        secondForm.Controls.Add(elementHost);
        Application.DoEvents();

        //now start to listen for events 
        // - Exclude Mouse related Events 
        EventListener elementHostEventListener = GetEventsListener(elementHost, "MouseEnter", "MouseMove", "Paint");
        EventListener avBtnEventListener = GetEventsListener(avBtn, "MouseEnter", "QueryCursor", "PreviewMouseMove", "MouseMove", "LayoutUpdated", "MouseLeave");

        //move this form over the secondForm 
        this.Location = new System.Drawing.Point(secondForm.Location.X, secondForm.Location.Y);
        this.BringToFront();
        Application.DoEvents();

        //validate the order of events
        sr.IncCounters(m_eventsTrace.ToString() == "",
                        "Unexpected events were raised. Raised events were: " + m_eventsTrace.ToString(),
                        p.log);
        m_eventsTrace.Remove(0, m_eventsTrace.Length);

        //bring to front the second form
        secondForm.BringToFront();
        Application.DoEvents();

        //validate the order of events
        string expectedEventTrace = "PreviewGotKeyboardFocus:Enter:GotFocus:IsKeyboardFocusWithin:IsKeyboardFocused:RequestBringIntoView:GotFocus:GotKeyboardFocus:";
        sr.IncCounters(m_eventsTrace.ToString() == expectedEventTrace,
                        "Unexpected events were raised. Expected: "+expectedEventTrace+ 
			"Raised events were: " + m_eventsTrace.ToString(),
                        p.log);

        secondForm.Close();
        secondForm.Dispose();
        Application.DoEvents();

        elementHostEventListener.Clear();
        avBtnEventListener.Clear();
        elementHostEventListener.Dispose();
        avBtnEventListener.Dispose();

        return sr;
    }

    #endregion
    
    #region Helper methods

    //Helper method that will create and return an EventListener which listens to all the events raised by the given source
    //Use the second param if you want to exclude some events. 
    private EventListener GetEventsListener(object source, params string[] excludedEvents)
    {
        //Because WPF uses DependencyPropertyChangedEventArgs in System.Windows.DependencyPropertyChangedEventHandler, 
        //and this doesn't derive from EventArgs, we cannot use only EventListener. We'll use EventListener for all but DependencyPropertyChangedEventHandler
        
        Type sourceType = source.GetType();
        EventInfo[] sourceEvents = sourceType.GetEvents();

        //attach DependencyPropertyChangedEventHandler handlers to this type of events and count those events 
        int notListenedCount = 0;
        for (int i = 0; i < sourceEvents.Length; i++)
        {
            if (sourceEvents[i].EventHandlerType == typeof(System.Windows.DependencyPropertyChangedEventHandler))
            {
                sourceEvents[i].AddEventHandler(source, new System.Windows.DependencyPropertyChangedEventHandler(dependencyPropertyChangedEventHandler));
                notListenedCount++;
            }
        }

        //create the array with the events that will be listened by our EventListener 
        EventInfo[] listenedEvents = new EventInfo[sourceEvents.Length - notListenedCount - excludedEvents.Length];
        int index = 0;
        for (int i = 0; i < sourceEvents.Length; i++)
        {
            if (sourceEvents[i].EventHandlerType != typeof(System.Windows.DependencyPropertyChangedEventHandler))
            {
                bool bExcluded = false;
                for (int j = 0; j < excludedEvents.Length; j++)
                {
                    if (excludedEvents[j] == sourceEvents[i].Name)
                    {
                        bExcluded = true;
                        break;
                    }
                }
                if (!bExcluded)
                {
                    listenedEvents[index++] = sourceEvents[i];
                }
            }
        }

        EventListener eventListener = new EventListener(source, listenedEvents);
        eventListener.EventFired += new EventHandler<EventFiredEventArgs>(eventListener_EventFired);

        return eventListener;
    }

    void eventListener_EventFired(object sender, EventFiredEventArgs e)
    {
        //we handle this because we want to merge the events for elementHost and for the hosted control. 
        m_eventsTrace.Append( e.EventInfo.EventName + ":");
    }
    void dependencyPropertyChangedEventHandler(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
        //for this type of events we need a handler cause we cannot use the EventListener
        m_eventsTrace.Append(e.Property.Name + ":");
    }
    
    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify Events and Order when a Avalon control hosted in a EH and an Winform window is Created, Shown and Closed.
//@ Verify Events and Order when an Avalon control is added and removed from a EH
//@ Verify Events and Order when a  EH with a Avalon control is added and removed from a Winform Window.
//@ Verify Events and Order when a Winform window with a EH and Avalon control, is moved around
//@ Verify Events and Order when a Winform window with a EH and  Avalon control, is resized
//@ Verify Events and Order when a Winform window with a EH and Avalon control, is maximized and minimized
//@ Verify Events and Order when a EH and Avalon control, is covered and not covered