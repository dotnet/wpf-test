using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

//
// Testcase:    SyncContext
// Description: Make sure that the sync context doesn't change when we add a host.
// Author:      a-wboyde
//
namespace WindowsFormsHostTests
{

public class SyncContext : WPFReflectBase
{

    #region Testcase setup
    public SyncContext(string[] args) : base(args) { }

    // class vars
    private enum ContainerType { DockPanel, Grid, StackPanel, Canvas, WrapPanel };
    private DockPanel _dp;
    private Grid _grid;
    private StackPanel _stack;
    private Canvas _canvas;
    private WrapPanel _wrap;
    private WindowsFormsHost _wfh1;
    private System.Windows.Forms.Button _wfBtn;

    protected override void InitTest(TParams p)
    {
        // hacks to get window to show up !!!
        this.Topmost = true;
        this.Topmost = false;
        this.WindowState = System.Windows.WindowState.Maximized;
        this.WindowState = System.Windows.WindowState.Normal;

        base.InitTest(p);
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("SynchronizationContext.Current shouldn't change when we add a WFH.")]
    public ScenarioResult Scenario1(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        // iterate through each container type
        foreach (ContainerType contType in Enum.GetValues(typeof(ContainerType)))
        {
            // set up Avalon window
            TestSetup(p, contType);
            MyPause();

            // save current context
            System.Threading.SynchronizationContext scBefore = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scBefore.ToString());

            // add WFH to Window
            AddHostToWindow(p, contType);

            // let events get processed
            //WPFReflectBase.DoEvents();
            MyPause();

            // get current context
            System.Threading.SynchronizationContext scAfter = System.Threading.SynchronizationContext.Current;
            p.log.WriteLine("Current context '{0}'", scAfter.ToString());

            // make sure contexts match
            bool b = scBefore.Equals(scAfter);
            p.log.WriteLine("Matches = {0}", b);
            WPFMiscUtils.IncCounters(sr, p.log, scBefore.Equals(scAfter),
                "SynchronizationContexts should match");
        }

        return sr;
    }
    #endregion

    #region Helper Functions
    /// <summary>
    /// Helper function to set up app for particular Scenario
    /// </summary>
    /// <param name="p"></param>
    /// <param name="contType"></param>
    private void TestSetup(TParams p, ContainerType contType)
    {
        // update app title bar and log file
        string str = string.Format("Container type: {0}", contType.ToString());
        this.Title = str;
        p.log.WriteLine(str);

        // create WF host control
        _wfh1 = new WindowsFormsHost();
        _wfBtn = new System.Windows.Forms.Button();
        _wfBtn.Text = "WinForms";
        _wfh1.Child = _wfBtn;

        // create (Avalon) container, set as application content
        if (contType == ContainerType.Canvas)
        {
            _canvas = new Canvas();

            // have to set explicit locations for Canvas
            Canvas.SetLeft(_wfh1, 100);
            Canvas.SetTop(_wfh1, 100);

            this.Content = _canvas;
        }
        else if (contType == ContainerType.DockPanel)
        {
            _dp = new DockPanel();
            this.Content = _dp;
        }
        else if (contType == ContainerType.Grid)
        {
            _grid = new Grid();

            // have to define Columns/Rows for Grid
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.RowDefinitions.Add(new RowDefinition());
            Grid.SetColumn(_wfh1, 1);

            this.Content = _grid;
        }
        else if (contType == ContainerType.StackPanel)
        {
            _stack = new StackPanel();
            this.Content = _stack;
        }
        else if (contType == ContainerType.WrapPanel)
        {
            _wrap = new WrapPanel();
            this.Content = _wrap;
        }
        else
        {
            // unknown ContainerType?
            throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
        }
    }

    /// <summary>
    /// Helper function to add WFH to the Avalon panel we are currently using
    /// </summary>
    /// <param name="p"></param>
    /// <param name="contType"></param>
    private void AddHostToWindow(TParams p, ContainerType contType)
    {
        // Add WFH to (Avalon) container
        if (contType == ContainerType.Canvas)
        {
            _canvas.Children.Add(_wfh1);
        }
        else if (contType == ContainerType.DockPanel)
        {
            _dp.Children.Add(_wfh1);
        }
        else if (contType == ContainerType.Grid)
        {
            _grid.Children.Add(_wfh1);
        }
        else if (contType == ContainerType.StackPanel)
        {
            _stack.Children.Add(_wfh1);
        }
        else if (contType == ContainerType.WrapPanel)
        {
            _wrap.Children.Add(_wfh1);
        }
        else
        {
            // unknown ContainerType?
            throw new ArgumentException("Unknown ContainerType '{0}'", contType.ToString());
        }
    }

    private static void MyPause()
    {
        // does this still cause exception? !!!
        WPFReflectBase.DoEvents();
        System.Threading.Thread.Sleep(100);
    }
    #endregion
}
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ SynchronizationContext.Current shouldn't change when we add a WFH.
