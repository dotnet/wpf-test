using System.Windows.Controls;

namespace Microsoft.Test.Controls.Actions
{
#if TESTBUILD_CLR40
    /// <summary>
    /// DataGrid action base.
    /// </summary>
    public abstract class DataGridAction
    {
        protected Control control;

        /// <summary>
        /// It assigns the control from Perform method parameter to the control is defined in the class.
        /// </summary>
        /// <param name="control">We need to pass in a control for mouse click action, but not every action needs the control object.</param>
        public virtual void Perform(Control control)
        {
            this.control = control;
        }
    }
#endif 
    }


