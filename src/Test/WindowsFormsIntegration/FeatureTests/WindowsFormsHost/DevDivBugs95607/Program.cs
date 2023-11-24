using System;
using System.Windows.Forms;

namespace DevDivBugs95607Program
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
            app.Run(new WindowsFormsHostTests.DevDivBugs95607(new string[0]));
        }
    }
}
