using System;
using System.Windows.Forms;

namespace CutAndPasteProgram
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
            app.Run(new WindowsFormsHostTests.CutAndPaste(new string[0]));
        }
    }
}
