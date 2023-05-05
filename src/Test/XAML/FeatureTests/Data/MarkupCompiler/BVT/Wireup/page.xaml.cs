// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test.ComponentModel // Namespace must be the same as what you set in project file
{

    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;
    using Microsoft.Test;
    using Microsoft.Test.Input;

    public partial class WireupClass
    {
        bool _failedTest = false;

        // Right now the verification structure is:
        // Start in OnLoaded handler and verify contents, then click third button, which clicks fourth button.
        // If a step along the way fails, won't follow path and will fail on timeout. So if you are analyzing for a
        // failure, look at the last stage it did get to.
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            CheckContents();

            // This is to verify that Button2 can be referenced by Name. (While Button4 is referenced inside the
            // third button handler when it calls for a click, Button2 never is since it has no handler.)
            if (Button2.Content as String == "Name, No Event Handler")
            {
                Console.WriteLine("Content of Button 2 as expected, able to reference by Name.");
            }
            else
            {
                Console.WriteLine("Content of Button 2 was '" + (Button2.Content as String) + "' did not match expectation of 'Name, No Event Handler'.");
                _failedTest = true;
            }

            Console.WriteLine("About to click on button 3.");
            IEnumerator i = Root.LogicalChildren;
            i.MoveNext();
            i.MoveNext();
            i.MoveNext();
            if (i.Current.GetType().FullName == "System.Windows.Controls.Button")
            {
                UserInput.MouseLeftClickCenter(i.Current as System.Windows.Controls.Button);
                Console.WriteLine("Just clicked on button 3.");
            }
            else
            {
                Console.WriteLine("Third UI Element was of type '" + i.Current.GetType().FullName + "' when it should have been of type System.Windows.Controls.Button.");
                _failedTest = true;
            }

        }

        void HandleButton3Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("We verified click handler on Button 3 works.");

            IEnumerator i = Root.LogicalChildren;
            i.MoveNext();
            i.MoveNext();
            i.MoveNext();
            if (sender == i.Current)
            {
                Console.WriteLine("Click event was sent by the correct button.");
            }
            else
            {
                Console.WriteLine("Click event was sent by some source other than the clicked button.");
                _failedTest = true;
            }

            Console.WriteLine("About to click on button 4.");
            UserInput.MouseLeftClickCenter(Button4);
            Console.WriteLine("Just clicked on button 4, able to reference by Name.");
        }

        void HandleButton4Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("We verified click handler on Button 4 works.");
            if (sender == Button4)
            {
                Console.WriteLine("Click event was sent by the correct button.");
            }
            else
            {
                Console.WriteLine("Click event was sent by some source other than the clicked button.");
                _failedTest = true;
            }

            if (_failedTest)
            {
                Application.Current.Shutdown(1);
            }
            else
            {
                Application.Current.Shutdown(0);
            }
        }
    }
}
