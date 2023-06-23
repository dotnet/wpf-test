using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{
    public class ConcreteMultiSelector : MultiSelector
    {
        public ConcreteMultiSelector(bool canSelectMultipleItems)
        {
            CanSelectMultipleItems = canSelectMultipleItems;
        }

        public bool IsUpdatingSelection
        {
            get { return IsUpdatingSelectedItems; }
        }

        public void BeginUpdateSelection()
        {
            BeginUpdateSelectedItems();
        }

        public void EndUpdateSelection()
        {
            EndUpdateSelectedItems();
        }
    }
}


