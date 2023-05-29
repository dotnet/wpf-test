// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.UtilityHelper;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.PropertyEngine
{
    /// <summary>
    /// All property engine verifiers in this class
    /// </summary>
    public static partial class Verifiers
    {
        private static void PrintTitle(string tagForTestCase)
        {
            string title = string.Empty;
            switch (tagForTestCase)
            {
                case "DP0001":
                    title = "MultiPropertyTriggers triggered when When both IsKeyboardFocusWithin and IsMouseOver are true. (Style in Resources)";
                    break;
                case "DP0002":
                    title = "MultiPropertyTriggers triggered when When both IsKeyboardFocusWithin and IsMouseOver are true. (Style in FE.Style)";
                    break;
                case "DP0003":
                    title = "Two PropertyTrigger targes the same property. One is triggered by IsKeyboardFocusWithin and the other IsMouseOver. Sequence counts.";
                    break;
                case "DP0101":
                    title = "Style.Triggers Only; In Style.Triggers, only EnterActions; In EnterActions, only one BeginStoryboard; Style in Resources (key is TargetType)";
                    break;
                case "DP0102":
                    title = "Style.Triggers Only; In Style.Triggers, only EnterActions; In EnterActions, Three BeginStoryboard targeting the same property. (Last one win); Style in Resources (key is TargetType)";
                    break;
                case "DP0103":
                    title = "Style.Triggers Only; In Style.Triggers, only EnterActions; In EnterAction, Three BeginStoryboard targeting the different property. Style in Resources (key is TargetType)";
                    break;
                case "DP0106":
                    title = "Style.Triggers Only; In Style.Triggers, only EnterActions; In EnterActions, only one BeginStoryboard; Style in Resources (key is TargetType); MultiTrigger";
                    break;
                case "DP0107":
                    title = "Style.Triggers Only; In Style.Triggers, only EnterActions; In EnterActions, Three BeginStoryboard targeting the same property. (Last one win); Style in Resources (key is TargetType); MultiTrigger";
                    break;
                case "DP0108":
                    title = "Style.Triggers Only; In Style.Triggers, only EnterActions; In EnterAction, Three BeginStoryboard targeting the different property. Style in Resources (key is TargetType); MultiTrigger";
                    break;
                case "DP0111":
                    title = "Style.Triggers Only; In Style.Triggers, only ExitActions; In ExitActions, only one BeginStoryboard; Style in Resources (key is TargetType)";
                    break;
                case "DP0112":
                    title = "Style.Triggers Only; In Style.Triggers, only ExitActions; In ExitActions, Three BeginStoryboard targeting the same property. (Last one win); Style in Resources (key is TargetType)";
                    break;
                case "DP0113":
                    title = "Style.Triggers Only; In Style.Triggers, only ExitActions; In ExitActions, Three BeginStoryboard targeting the different property. Style in Resources (key is TargetType)";
                    break;
                case "DP0116":
                    title = "Style.Triggers Only; In Style.Triggers, only ExitActions; In ExitActions, only one BeginStoryboard; Style in Resources (key is TargetType); MultiTrigger ";
                    break;
                case "DP0117":
                    title = "Style.Triggers Only; In Style.Triggers, only ExitActions; In ExitActions, Three BeginStoryboard targeting the same property. (Last one win); Style in Resources (key is TargetType); MultiTrigger";
                    break;
                case "DP0118":
                    title = "Style.Triggers Only; In Style.Triggers, only ExitActions; In ExitActions, Three BeginStoryboard targeting the different property. Style in Resources (key is TargetType); MultiTrigger";
                    break;
                case "DP0121":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, only one BeginStoryboard. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType); ";
                    break;
                case "DP0122":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the same property. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0123":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the different property. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0126":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, only one BeginStoryboard. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType); MultiTrigger";
                    break;
                case "DP0127":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the same property. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType) MultiTrigger";
                    break;
                case "DP0128":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the different property. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType) MultiTrigger";
                    break;
                case "DP0131":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, only one BeginStoryboard. (EnterActions and ExitActions target DIFFERENT property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0132":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the same property. (EnterActions and ExitActions target Different property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0133":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Two BeginStoryboard targeting the different property. (EnterActions and ExitActions target Different property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0141":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, only one BeginStoryboard. (EnterActions and ExitActions target the same property. EnterAction specified after ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0142":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the same property. (EnterActions and ExitActions target the same property. EnterAction specified after ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0143":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the different property. (EnterActions and ExitActions target the same property. EnterAction specified after ExitAction.) Style in Resources (key is TargetType)";
                    break;
                case "DP0151":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, only one BeginStoryboard. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType); Use PropertyPath notation: Background.Color becomes (Button.Background).(SolidColorBrush.Color).";
                    break;
                case "DP0152":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the same property. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType); Use PropertyPath notation: Background.Color becomes (Button.Background).(SolidColorBrush.Color).";
                    break;
                case "DP0153":
                    title = "Style.Triggers Only; In Style.Triggers, Both EnterActions and ExitActions; In EnterActions and ExitActions, Three BeginStoryboard targeting the different property. (EnterActions and ExitActions target the same property. EnterAction specified prior to ExitAction.) Style in Resources (key is TargetType) ; Use PropertyPath notation: Background.Color becomes (Button.Background).(SolidColorBrush.Color).";
                    break;
                case "DP0161":
                    title = "";
                    break;
                case "DP0162":
                    title = "";
                    break;
                case "DP0163":
                    title = "";
                    break;
                case "DP0171":
                    title = "";
                    break;
                case "DP0172":
                    title = "";
                    break;
                case "DP0173":
                    title = "";
                    break;
                case "DP0201":
                    title = "";
                    break;
                case "DP0202":
                    title = "";
                    break;
                case "DP0203":
                    title = "";
                    break;
                case "DP0204":
                    title = "";
                    break;
                case "DP0205":
                    title = "";
                    break;
                case "DP0206":
                    title = "";
                    break;
                case "Resources0001":
                    title = "Resource Type: Non-DO CLR object, Freezable, FE, FCE, Style, Template, ResourceDictionary. x:Shared as default. SetResourceReference and FindResource. Chain of Resource Reference.";
                    break;
                case "Resources0002":
                    title = "Resource Type: Non-DO CLR object, Freezable, FE, FCE, Style, Template, ResourceDictionary. x:Shared as all true. SetResourceReference and FindResource. Chain of Resource Reference.";
                    break;
                case "Resources0003":
                    title = "Resource Type: Non-DO CLR object, Freezable, FE, FCE, Style, Template, ResourceDictionary. x:Shared as all true. SetResourceReference and FindResource. Chain of Resource Reference.";
                    break;
                case "Resources0004":
                    title = "Resource Type: Non-DO CLR object, Freezable, FE, FCE, Style, Template, ResourceDictionary. x:Shared as default. Original XAML file contains reference (DynamicResource and StaticResource) to resource.";
                    break;
                case "Resources0005":
                    title = "Negative Test Cases";
                    break;
                case "Resources0101":
                    title = "From Use vs. From Definition Lookup. PR.C.CR: Definition @ Parent.Resources, Use @ Child and Overriding Resources @ Child.Resources. ";
                    break;
                case "Resources0102":
                    title = "From Use vs. From Definition Lookup. GpR.Gc.GcR: Definition @ GrandParent.Resources, Use @ GrandChild and Overriding Resources @ GrandChild.Resources.";
                    break;
                case "Resources0103":
                    title = "From Use vs. From Definition Lookup. GpR.Gc.PR: Definition @ GrandParent.Resources, Use @ GrandChild and Overriding Resources @ Parent.Resources.";
                    break;
                case "Resources0104":
                    title = "From Use vs. From Definition Lookup.  GpR.Gc.CR&PR: Definition @ GrandParent.Resources, Use @ GrandChild and Overriding Resources @ Child.Resources & Parent.Resources.";
                    break;
                case "Resources0105":
                    title = "From Use vs. From Definition Lookup. StaticResource does not follow this rule. ";
                    break;
                case "Resources2000":
                    title = "Bug : Compiled page that uses ResourceDictionary.MergedDictionaries causes XamlParseException if that ResourceDictionary has at least one resource in iteslf.";
                    break;
                case "Resources2001":
                case "Resources2002":
                case "Resources2003":
                case "Resources2004":
                case "Resources2005":
                case "Resources2006":
                case "Resources2007":
                case "Resources2008":
                case "Resources2009":
                    title = "Resources.MergedDictionaries test cases";
                    break;
                default:
                    Debug.Fail("Please provide Title for " + tagForTestCase);
                    break;
            }
            Utilities.PrintTitle(title);
        }
    }
}
