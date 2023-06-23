using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Reflection;
using System.Threading;

using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWD = System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;
using SWS = System.Windows.Shapes;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MitaControl = MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

// Testcase:    SeparateThreads
// Description: Running  EH on seperate Form on seperate threads
// Author:      pachan
//
public class SeparateThreads : ReflectBase 
{
    #region Testcase setup
    SWF.Button WFButton;

    // Second WinForms window
    SWF.Form WFForm;
    ElementHost eh;
    SWC.Button AVButton;

    private const string AVButtonName = "AVButton";
    private const string WFFormName = "WFForm";
    private const string WFButtonName = "WFButton";
    private const string AppName = "SeparateThreads";


    public SeparateThreads(string[] args) : base(args) { }


    protected override void InitTest(TParams p) 
    {
        this.UseMita = true;
        this.Text = AppName;

        this.Size = new System.Drawing.Size(200, 200);
        this.Left = 0;
        this.Top = 200;

        WFButton = new SWF.Button();
        WFButton.Text = WFButton.Name = WFButtonName;
        WFButton.AutoSize = true;
        WFButton.Click += new EventHandler(WFButton_Click);
        this.Controls.Add(WFButton);
        base.InitTest(p);
    }
    #endregion

    void WFButton_Click(object sender, EventArgs e)
    {
        Thread t = new Thread(new ThreadStart(WFDialogThread));
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
    }

    private void WFDialogThread()
    {
        WFForm = new System.Windows.Forms.Form();
        WFForm.Text = WFFormName;
        WFForm.Size = new System.Drawing.Size(200, 200);
        WFForm.Left = 400;
        WFForm.Top = 200;

        eh = new ElementHost();
        AVButton = new System.Windows.Controls.Button();
        AVButton.Content = AVButton.Name = AVButtonName;
        eh.Child = AVButton;
        WFForm.Controls.Add(eh);

        AVButton.Click += new System.Windows.RoutedEventHandler(AVButton_Click);

        // start the WF Dialog Box on a separate thread
        WFForm.ShowDialog();
    }

    void AVButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        WFForm.Close();
    }

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    [Scenario("Verify that an Winform App can spawn a new thread with a new  Form that contains an EH with Avalon controls. We want to make sure that this scenario works i.e. no exception are thrown.")]
    public ScenarioResult Scenario1(TParams p) 
    {
        ScenarioResult sr = new ScenarioResult();
        try
        {
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(AppName));
            UIObject uiControl = uiApp.Descendants.Find(UICondition.CreateFromId(WFButtonName));
            // Click on the button to bring up the second WinForms on a separate thread
//            Utilities.ActiveFreeze("aaa");
            uiControl.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(20);

            UIObject uiSecondWF = UIObject.Root.Children.Find(UICondition.CreateFromName(WFFormName));
            UIObject uiEHBtn = uiSecondWF.Descendants.Find(UICondition.CreateFromId(AVButtonName));
            // Click on the AV Button to close the second WinForms
            uiEHBtn.Click(PointerButtons.Primary);
            Utilities.SleepDoEvents(10);
        }
        catch (Exception ee)
        {
            sr.IncCounters(false, ee.Message, p.log);
        }

        sr.IncCounters(true);
        return sr;
    }

    #endregion
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that an Winform App can spawn a new thread with a new  Form that contains an EH with Avalon controls. We want to make sure that this scenario works i.e. no exception are thrown.

