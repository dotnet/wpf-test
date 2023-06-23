using System;
using System.Windows.Navigation;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// What does it do? 
    ///    Responsible to host wpf controls automation peer test scenario such as Button, Calendar, etc…
    /// How does it work? 
    ///    Use NavigationWindow to navigate to a page.  You can have code behind for the page.  All pages are compiled to assembly resource.
    ///    Requirement  - you need to pass page relative path in assembly resource; exception will be throw otherwise.
    ///Suggestion
    ///    Consider to create a name for the targetElement that you want to test because it enables automation test code to find the targetElement.
    /// </summary>
    public partial class Window1 : NavigationWindow
    {
        public Window1()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 2)
            {
                throw new ArgumentOutOfRangeException("Fail: args.Length != 2.");
            }

            string pageName = args[1];

            Navigate(new Uri(pageName, UriKind.Relative));
        }
    }
}
