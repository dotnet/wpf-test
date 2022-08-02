// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Windows.Media;
using System.Windows.Automation;

namespace DRT
{
    public class InputScopeTestSuite : DrtTestSuite
    {

        public InputScopeTestSuite() : base("InputScope DRT")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            DRT.LoadXamlFile(@"DrtFiles\DrtInputMethod\DrtInputScope.xaml");

            return new DrtTest[]{
                       new DrtTest( CheckInputScope),
                            null };        
        }

        private void CheckInputScope()
        {
            FrameworkElement r = (FrameworkElement)DRT.RootElement;

            InputScopeNameValue isNameRoot = ((InputScopeName)r.InputScope.Names[0]).NameValue;

            IEnumerator ienum = LogicalTreeHelper.GetChildren(r).GetEnumerator();
            while (ienum.MoveNext())
            {
                FrameworkElement child = (FrameworkElement)ienum.Current;
                if (null!=child)
                {
                    if(child.Name == "Combo")
                    {
                        // check if Combo has phrase list
                        if(((InputScopeName)child.InputScope.Names[0]).NameValue == InputScopeNameValue.PhraseList)
                        {
                            // then if it does match the list
                            for (int i = 0; i <child.InputScope.PhraseList.Count; i++)
                            {
                                // because generics is not CLS compliant at this moment, we need to cast what PhraseList returns
                                // this should be fixed when generics becomes CLS compliant.
                                if (((InputScopePhrase)child.InputScope.PhraseList[i]).Name != PhraseList[i])
                                {
                                    throw new ApplicationException("PhraseList doesn't match");
                                }
                            }
                        }
                        else
                        {
                            throw new ApplicationException("ComboBox doesn't have phraselist");
                        }
                    }
                    else if (child.Name == "TextBox1")
                    {
                        // check if TextBox has inherited Root's inputscope
                        if(((InputScopeName)child.InputScope.Names[0]).NameValue != isNameRoot)
                            throw new ApplicationException("TextBox1 doesn't inherit root's inputscope");
                    }
                    else if (child.Name == "TextBox2")
                    {
                        // check if TextBox has the expected inputscope
                        if(((InputScopeName)child.InputScope.Names[0]).NameValue != InputScopeNameValue.AddressCity)
                            throw new ApplicationException("TextBox2 doesn't have InputScope specified in xaml");
                    }
                }
                else
                    break;
           }
        }

        // This needs to remain synchronized with the list in drtinputscope.xaml - when you change either one and not other,
        // you'll break this DRT.
        static String[] PhraseList = new String[] {
                                    "Redmond",
                                    "Seattle",
                                    "Kirkland",
                                    "Bellevue",
                                    "Renton",
                                    "Woodenville",
                                    "Bothell",
                           };

    }
}

