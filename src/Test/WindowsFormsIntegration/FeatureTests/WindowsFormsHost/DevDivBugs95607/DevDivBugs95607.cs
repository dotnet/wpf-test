// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Threading;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using System.Windows.Media;


/// <Testcase>DevDivBugs95607</Testcase>
/// <summary>
/// Description: Ensure that setting windowsFormsHost.Background=Transparent does not prevent other
/// WPF elements content from rendering on startup.
/// </summary>
namespace WindowsFormsHostTests
{
    public class DevDivBugs95607 : WPFReflectBase
    {
        #region Testcase setup
        public DevDivBugs95607(string[] args) : base(args) { }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            return base.BeforeScenario(p, scenario);
        }

        protected override void InitTest(TParams p)
        {
            this.Width = 200;
            this.Height = 200;

            SWC.StackPanel stackPanel1 = new SWC.StackPanel();

            WindowsFormsHost windowsFormsHost1 = new WindowsFormsHost();
            windowsFormsHost1.Background = Brushes.Transparent;
            stackPanel1.Children.Add(windowsFormsHost1);

            SWC.Label label1 = new SWC.Label();
            label1.Content = "Label";
            label1.Foreground = Brushes.Red;
            stackPanel1.Children.Add(label1);

            SWC.TextBox textBox1 = new SWC.TextBox();
            textBox1.Text = "TextBox";
            textBox1.Foreground = Brushes.Yellow;
            stackPanel1.Children.Add(textBox1);

            this.AddChild(stackPanel1);
            Utilities.SleepDoEvents(20);

            base.InitTest(p);
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Ensure that setting windowsFormsHost.Background=Transparent does not prevent other WPF elements from rendering on startup.")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height); 
                System.Drawing.Bitmap bmp = Utilities.GetScreenBitmap(rect);
                //bmp.Save("rect.bmp");
                Utilities.SleepDoEvents(20);


                p.log.WriteLine("Check if the other WPF elements are rendered.");
                WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmp, System.Drawing.Color.Red), "The red WPF label content is not rendered.");
                WPFMiscUtils.IncCounters(sr, p.log, Utilities.ContainsColor(bmp, System.Drawing.Color.Yellow), "The yellow WPF textbox text is not rendered.");
                return sr;

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.ToString(), p.log);
            }
            WPFMiscUtils.IncCounters(sr, p.log, true, "The WPF elements are rendered.");
            return sr;
        }
        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Ensure that setting windowsFormsHost.Background=Transparent does not prevent other WPF elements from rendering on startup.
