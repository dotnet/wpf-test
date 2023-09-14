// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Automation.Peers;


namespace Avalon.Test.ComponentModel
{
    public class MyButtonAutomationPeer : ButtonAutomationPeer
    {
        public MyButtonAutomationPeer(MyButton owner) : base(owner)
        { }

        protected override string GetClassNameCore()
        {
            return "MyButton";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }
    }

    public class MyButton : Button
    {
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new MyButtonAutomationPeer(this);
        }
    }
}
