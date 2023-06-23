
namespace Microsoft.Test.Controls
{
    enum SelectionOption
    {
        AddItem,
        InsertItem,
        RemoveItem,
        RemoveAtItem,
        Refresh
    }

    interface ISelectionTest<T>
    {
        void Run();
    }
}
