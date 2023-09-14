using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.Helpers
{
    /// <summary>
    /// These methods are in lieu of "extension" methods featured in .Net 3.5,
    /// but not available in .Net 3.0. Without the "extension" feature these 
    /// these can still be scoped together.    /// </summary>
    public static class GridSplitterGridExtend
    {
        /// <summary>
        /// Instantiate a GridDefinitionSanpshot from a Grid.
        /// Note that the QueueHelper wait is necessary for reliable results.
        /// </summary>
        public static GridDefinitionSnapshot GetDefinitionSnapshot(Grid grid)
        {
            QueueHelper.WaitTillQueueItemsProcessed();
            return new GridDefinitionSnapshot(grid.ColumnDefinitions, grid.RowDefinitions);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CloneRowColumnDefinitions(Grid grid, Grid other)
        {
            CloneDefinitionsOfType<RowDefinition>(grid, other);
            CloneDefinitionsOfType<ColumnDefinition>(grid, other);
        }

        /// <summary>
        /// Clone the Grid.RowDefinitions or Grid.ColumnDefinitins of the other
        /// Grid to this Grid. Allows using the other Grid's Definitions as a
        /// template so as not to modify the other Grid by GridSplitter operations.
        /// </summary>
        public static void CloneDefinitionsOfType<T>(Grid grid, Grid other) where T : DefinitionBase, new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties
                (BindingFlags.Public |
                 BindingFlags.Instance);

            MethodInfo[] getters = new MethodInfo[properties.Length];
            MethodInfo[] setters = new MethodInfo[properties.Length];

            for (Int32 i = 0; i < properties.Length; ++i)
            {
                setters[i] = properties[i].GetSetMethod();
                if (null != setters[i])
                {
                    getters[i] = properties[i].GetGetMethod();
                }
            }

            Int32 count = (typeof(T) == typeof(RowDefinition)) ? other.RowDefinitions.Count : other.ColumnDefinitions.Count;

            for (Int32 j = 0; j < count; ++j)
            {
                T target = new T();

                for (Int32 k = 0; k < properties.Length; ++k)
                {
                    if (null != setters[k])
                    {
                        if (typeof(T) == typeof(ColumnDefinition))
                        {
                            (setters[k]).Invoke(target, new Object[] { (getters[k]).Invoke(other.ColumnDefinitions[j], null) });
                        }
                        else
                        {
                            (setters[k]).Invoke(target, new Object[] { (getters[k]).Invoke(other.RowDefinitions[j], null) });
                        }
                    }
                }
                if (typeof(T) == typeof(ColumnDefinition))
                {
                    grid.ColumnDefinitions.Add(target as ColumnDefinition);
                }
                else
                {
                    grid.RowDefinitions.Add(target as RowDefinition);
                }
            }
        }
    }
}
