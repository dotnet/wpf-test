
namespace Microsoft.Test.Controls
{
    /// <summary>
    /// An object to keep track of cell coordinate.
    /// </summary>
    public class CellCoordinate
    {
        public CellCoordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public int X { set; get; }
        public int Y { set; get; }
        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}
