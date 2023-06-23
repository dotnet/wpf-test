using System;
using System.Windows.Forms;

namespace FontPropagationProgram
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new FontPropagation(new string[0]));
        }
    }
}
