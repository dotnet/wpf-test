using System;
using System.Windows.Forms;

namespace WindowsOSBug934077Program
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Application app = new System.Windows.Application();
            app.Run(new WindowsFormsHostTests.WindowsOSBug934077(new string[0]));
        }
    }
}
