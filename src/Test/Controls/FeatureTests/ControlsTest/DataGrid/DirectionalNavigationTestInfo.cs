
namespace Microsoft.Test.Controls
{
    /// <summary>
    /// DataGrid directional navigation test info.
    /// You can use it to build a collection of direction navigation test data.
    /// </summary>
    public struct DirectionalNavigationTestInfo
    {
        private CellCoordinate cellCoordinate;
        private DirectionalNavigationKey directionalNavigationKey;

        public DirectionalNavigationTestInfo(CellCoordinate cellCoordinate, DirectionalNavigationKey directionalNavigationKey)
        {
            this.cellCoordinate = cellCoordinate;
            this.directionalNavigationKey = directionalNavigationKey;
        }

        public CellCoordinate CellCoordinate
        {
            get
            {
                return cellCoordinate;
            }
        }

        public DirectionalNavigationKey DirectionalNavigationKey
        {
            get
            {
                return directionalNavigationKey;
            }
        }
    }
}
