// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Globalization.Sample.StringTable
{
    using System;
    using System.Windows;
    using System.Windows.Navigation;
	using System.Globalization;
	using System.Resources;
	using System.Reflection;


     public partial class MyApp
     {
        
	protected override void OnStartup(StartupEventArgs e)
        {
             	NavigationWindow window = new NavigationWindow();
                
		ResourceManager rm = new ResourceManager ("stringtable", Assembly.GetExecutingAssembly());
		
		String str = rm.GetString("Title");
			
		window.Title = str;
	
                
                window.Show();

               
                window.Navigate(new Uri("strtbl.xaml", UriKind.RelativeOrAbsolute));
	
            
	}

     }
}
