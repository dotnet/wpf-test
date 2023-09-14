using System;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// MenuBase test interface. We can use it to run concrete MenuBase controls tests.
    /// </summary>
    public interface IMenuBaseTest
    {
        void Run(MenuBase menubase);
    }
}
