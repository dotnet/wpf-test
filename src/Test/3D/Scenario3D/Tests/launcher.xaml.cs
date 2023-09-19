// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Reflection;
using System.Resources;
using System.Windows.Resources;
using System.Collections;

namespace Microsoft.Test.Graphics
{
    public partial class ScenarioLauncher
    {
        public void OnLoaded( object sender, EventArgs args )
        {
        }

        // 
        public void OnContentRendered( object sender, EventArgs args )
        {
            ListBox lb = MAINPANEL;
            try
            {
                ResourceManager rm = ResourceManager.CreateFileBasedResourceManager(
                        "Scenario3D.g",
                        System.IO.Directory.GetCurrentDirectory(),
                        null );
                ResourceSet rs = rm.GetResourceSet( System.Globalization.CultureInfo.InvariantCulture, false, true );
                IDictionaryEnumerator ide = rs.GetEnumerator();
                foreach ( DictionaryEntry entry in rs )
                {
                    string resourceName = entry.Key.ToString();
                    if ( resourceName.EndsWith( ".baml" ) )
                    {
                        ListBoxItem li = new ListBoxItem();
                        TextBlock tx = new TextBlock();
                        tx.Text = resourceName.Replace( ".baml", ".xaml" );
                        li.Content = tx;
                        ( (System.Windows.Markup.IAddChild)lb ).AddChild( li );
                    }
                }
            }
            catch ( System.Exception )
            {
                ListBoxItem li = new ListBoxItem();
                TextBlock tx = new TextBlock();
                tx.Text = "Scenario3D.g.resources file not found.\nYou will need to have this in the same directory for resource introspection.";
                li.Content = tx;
                ( (System.Windows.Markup.IAddChild)lb ).AddChild( li );
            }

        }

        public void OnDoubleClickLoad( object sender, MouseButtonEventArgs args )
        {
            ListBox lb = MAINPANEL;
            ListBoxItem selected = lb.SelectedItem as ListBoxItem;
            string url = ( selected.Content as TextBlock ).Text;
            ScenarioUtility.NavigateToPage( url );
        }
    }
}
