// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ControlLibrary;

namespace InputProblem
{
  public partial class Form1 : Form
  {    
    //  <summary>
    //  Creates ElementHost to put WPF control(s) in
    //  </summary>
    //  <param name="wpfControl"></param>
    //  <param name="placeholder"></param>
    //  <returns>Den Elementhost der das Control beinhaltet</returns>
    //  <remarks></remarks>
    public static System.Windows.Forms.Integration.ElementHost InstallWpfControl(System.Windows.UIElement wpfControl, System.Windows.Forms.Control placeholder)
    {
      //  Host Control container... 
      System.Windows.Forms.Integration.ElementHost elementHost = new System.Windows.Forms.Integration.ElementHost();

      elementHost.Dock = placeholder.Dock;
      elementHost.Anchor = placeholder.Anchor;
      elementHost.Location = placeholder.Location;
      elementHost.Size = placeholder.Size;
      elementHost.Child = wpfControl;
      // Assign Control to be the Wpf Control
      System.Windows.Forms.Control phParent ; 
      phParent  = placeholder.Parent; 
      // Remove wildcard 
      phParent .Controls.Remove (placeholder); 
      // Add the control... 
      phParent .Controls.Add(elementHost);
      return elementHost;
    }

    public Form1()
    {
      InitializeComponent();
      FunnyControl fc = new FunnyControl();
      InstallWpfControl(fc, pnlPlaceholder2);
    }
  }
}
