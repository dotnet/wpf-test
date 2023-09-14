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
    /// Represents a state of the Row- and Column- definitions of a Grid. Actual
    /// RowDefinition and ColumnDefinition references are not stored, but relevant values
    /// are copied into private readonly state.
    /// Instances of this class are used to represent the "before" and "after" states
    /// of a Grid's RowDefinitionCollection and ColumnDefinitionCollection.
    /// The class provides methods and types for comparing expected versus actual values
    /// for use when a test-specification is executed.
    /// </summary>
    public class GridDefinitionSnapshot
    {
        #region Private Members

        //region Constants

        // Anything less than 1.0 is sufficient because unit is pixel.
        private const Double diffTolerance = 0.1; // number of decimal places evaluated
        private const Int32 diffPrecision = 1;  // number of decimal places rounded

        /// <summary>
        /// Snapshot of interesting values from a RowDefinition or ColumnDefinition reference type
        /// </summary>
        private struct DefSurrogate
        {
            internal readonly Double ActualDimension;
            internal readonly Double Offset;
            internal readonly GridLength Dimension;
            internal DefSurrogate(Double actualDimension, Double offset, GridLength dimension)
            {
                ActualDimension = actualDimension; Offset = offset; Dimension = dimension;
            }
            public override String ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Actual={0:F3}, Offset={1:F3}, GridLength=<< {2} >>",
                    ActualDimension, Offset, GridSplitterGridLengthExtend.GetString(Dimension));
                return sb.ToString();
            }
        }

        // region Fields

        private readonly DefSurrogate[] columns;
        private readonly Double columnAggregateActualWidth;

        private readonly DefSurrogate[] rows;
        private readonly Double rowAggregateActualHeight;

        #endregion

        #region Public Constructor
        /// <summary>
        /// Construct this snapshot given a Grid.RowDefinitionCollection and
        /// Grid.ColumnDefinitionCollection The snapshot saves interesting
        /// Row definition information in each element of a RowDefSurrogate[],
        /// and interesting Column definition information in each element
        /// of a ColumnDefSurrogate[]
        /// </summary>
        public GridDefinitionSnapshot(
            ColumnDefinitionCollection c, RowDefinitionCollection r)
        {
            columns = new DefSurrogate[c.Count];
            rows = new DefSurrogate[r.Count];
            ColumnCount = c.Count;
            RowCount = r.Count;

            Int32 idx = 0;
            foreach (ColumnDefinition def in c)
            {
                columns[idx]
                    = new DefSurrogate(def.ActualWidth, def.Offset, def.Width);
                columnAggregateActualWidth += def.ActualWidth;
                ++idx;
            }
            idx = 0;
            foreach (RowDefinition def in r)
            {
                rows[idx]
                    = new DefSurrogate(def.ActualHeight, def.Offset, def.Height);
                rowAggregateActualHeight += def.ActualHeight;
                ++idx;
            }
        }

        #endregion

        #region Public Nested Types

        /// <summary>
        /// Abstract away the difference between Row.ActualHeight and Column.ActualWidth
        /// So that RowDiff and ColumnDiff can be used generically in some situations.
        /// </summary>
        public interface IDiff
        {
            Double ActualDimension { get; }
            Double? Dimension { get; }
        }

        /// <summary>
        /// Base class for RowDiff and ColumnDiff
        /// </summary>
        public abstract class DiffBase : IDiff
        {
            protected readonly Double ActualDimension;
            protected readonly Double? Dimension;
            public readonly Double Offset;
            public readonly Nullable<GridUnitType> UnitType;

            Double IDiff.ActualDimension
            {
                get { return ActualDimension; }
            }
            Double? IDiff.Dimension
            {
                get { return Dimension; }
            }

            protected DiffBase(Double actual, Double offset)
            {
                ActualDimension = actual;
                Offset = offset;
                Dimension = null; UnitType = null;
            }

            protected DiffBase(Double actual, Double offset, Double dimension, GridUnitType unit)
            {
                ActualDimension = actual;
                Offset = offset;
                Dimension = dimension;
                UnitType = unit;
            }

            // Only compare the non-nullable values
            protected static bool CompareArrays<T>(T[] dbX, T[] dbY) where T: DiffBase
            {
                if (dbX.Length != dbY.Length)
                {
                    throw new TestValidationException("DiffBase.CompareArrays requires both arrays of same length");
                }
                
                for (Int32 i = 0; i < dbX.Length; ++i)
                {
                    if (Math.Abs(dbX[i].ActualDimension - dbY[i].ActualDimension) > diffTolerance)
                    {
                        return false;
                    }
                    if (Math.Abs(dbX[i].Offset - dbY[i].Offset) > diffTolerance)
                    {
                        return false;
                    }
                }
                return true;
            }

            protected String GetString(String name, String actualDimension, String dimension)
            {
                if (Dimension.HasValue || UnitType.HasValue)
                {
                    return String.Format("{0}: {1}={2}, Offset={3}, {4}={5}, UnitType={6}",
                        name, actualDimension,
                        Math.Round(ActualDimension, diffPrecision).ToString(),
                        Math.Round(Offset, diffPrecision).ToString(),
                        dimension, Dimension.ToString(),
                        UnitType.ToString());
                }
                else
                {
                    return String.Format("{0}: {1}={2}, Offset={3}",
                        name, actualDimension,
                        Math.Round(ActualDimension, diffPrecision).ToString(),
                        Math.Round(Offset, diffPrecision).ToString());
                }
            }
        }

        /// <summary>
        /// Represents the differences in ActualHeight and Offset between two rows.
        /// This is a workhorse element of this test solution. Instances of this
        /// struct are used to specify expected test-case results. Expected values
        /// can be compared with the actual values returned by comparison of "before 
        /// and after" GridDefintionSnapshot instances.
        /// </summary>
        public class RowDiff : DiffBase
        {
            public Double ActualHeight { get { return ActualDimension; } }

            public Double? Height { get { return Dimension; } }

            public RowDiff(Double actual, Double offset) : base(actual, offset){}

            public RowDiff(Double actual, Double offset, Double height, GridUnitType unit)
                : base( actual, offset, height, unit){}

            public override String ToString()
            {
                return base.GetString("   RowDiff", "ActualHeight", "Height");
            }

            public static bool CompareArrays(RowDiff[] rdX, RowDiff[] rdY)
            {
                return DiffBase.CompareArrays<RowDiff>(rdX, rdY);
            }
        }

        /// <summary>
        /// Represents the differences in ActualWidth and Offset between two columns.
        ///  -- See summary for RowDiff --
        /// </summary>
        public class ColumnDiff : DiffBase
        {
            public Double ActualWidth { get { return ActualDimension; } }

            public Double? Width { get { return Dimension; } }

            public ColumnDiff(Double actual, Double offset) : base(actual, offset) { }

            public ColumnDiff(Double actual, Double offset, Double width, GridUnitType unit)
                : base(actual, offset, width, unit) { }

            public override String ToString()
            {
                return base.GetString("ColumnDiff", "ActualWidth", "Width");
            }

            public static bool CompareArrays(ColumnDiff[] cdX, ColumnDiff[] cdY)
            {
                return DiffBase.CompareArrays<ColumnDiff>(cdX, cdY);
            }
        }

        #endregion

        #region Public Properties

        public Int32 RowCount { get; private set; }
        public Int32 ColumnCount { get; private set; }

        #endregion

        #region Public Methods

        private static void CompareDefSurrogateArrays<T>(DefSurrogate[] alphas, DefSurrogate[] betas, out T[] diffs)
            where T : DiffBase
        {
            if (alphas.Length != betas.Length)
            {
                throw new TestValidationException(
                    "Can't compare GridDefinitionSnapshot instances with different number of rows or columns.");
            }

            ConstructorInfo constructor = 
                (typeof(T)).GetConstructor(
                    new Type[] { typeof(Double), typeof(Double), typeof(Double), typeof(GridUnitType) });

            diffs = new T[alphas.Length];
            for (Int32 i = 0; i < alphas.Length; ++i)
            {
                if (alphas[i].Dimension.GridUnitType != betas[i].Dimension.GridUnitType)
                {
                    throw new TestValidationException(
                        "Can't compare column or row GridLength instances with different GridUnitType.");
                }
                Double actual = betas[i].ActualDimension - alphas[i].ActualDimension;
                Double offset = betas[i].Offset - alphas[i].Offset;
                Double width = betas[i].Dimension.Value - alphas[i].Dimension.Value;
                GridUnitType unitType = alphas[i].Dimension.GridUnitType;
                diffs[i] = (T)(constructor.Invoke(new Object[] {actual, offset, width, unitType}));
            }
        }

        /// <summary>
        /// Compare column definitions held by two GridDefinitionSnapshot instances.
        /// Typically 'this' is the before instance and 'other' is the after 
        /// instance.
        /// -- see struct ColumnDiff summary --
        /// </summary>
        /// <returns></returns>
        public ColumnDiff[] CompareColumns(GridDefinitionSnapshot other)
        {
            ColumnDiff[] columnDiffs= null;
            CompareDefSurrogateArrays<ColumnDiff>(columns, other.columns, out columnDiffs);

            return columnDiffs;
        }

        /// <summary>
        /// Compare row definitions held by two GridDefinitionSnapshot instances.
        /// Typically 'this' is the before instance and 'other' is the after 
        /// instance.
        /// -- see struct RowDiff summary --
        /// </summary>
        /// <returns>RowDiff[] of Row differences between two snapshots</returns>
        public RowDiff[] CompareRows(GridDefinitionSnapshot other)
        {
            RowDiff[] rowDiffs = null;
            CompareDefSurrogateArrays<RowDiff>(rows, other.rows, out rowDiffs);

            return rowDiffs;
        }

        /// <summary>
        /// Return an array where the value at index i represents the ratio
        /// of Row[i].ActualHeight to the aggregate of all Row ActualHeights.
        /// </summary>
        public Double[] RowRatios(Byte precision)
        {
            Double[] ratios = new Double[RowCount];
            for (Int32 i = 0; i < RowCount; ++i)
            {
                ratios[i] = Math.Round(
                    (rows[i].ActualDimension / rowAggregateActualHeight), precision);
            }
            return ratios;
        }

        /// <summary>
        /// Return an array where the value at index i represents the ratio
        /// of Column[i].ActualWidth to the aggregate of all Column ActualWidths.
        /// </summary>
        public Double[] ColumnRatios(Byte precision)
        {
            Double[] ratios = new Double[ColumnCount];
            for (Int32 i = 0; i < ColumnCount; ++i)
            {
                ratios[i] = Math.Round(
                    (columns[i].ActualDimension / columnAggregateActualWidth), precision);
            }
            return ratios;
        }

        /// <summary>
        /// Simple predicate is true when 'this' and 'other' are compatible
        /// for CompareRows and CompareColumns operations.
        /// </summary>
        public Boolean IsCompareCompatible(GridDefinitionSnapshot other)
        {
            if (ColumnCount != other.ColumnCount
                || RowCount != other.RowCount)
            {
                return false;
            }
            for (Int32 i = 0; i < RowCount; ++i)
            {
                if (rows[i].Dimension.GridUnitType != other.rows[i].Dimension.GridUnitType)
                {
                    return false;
                }
            }
            for (Int32 j = 0; j < ColumnCount; ++j)
            {
                if (columns[j].Dimension.GridUnitType != other.columns[j].Dimension.GridUnitType)
                {
                    return false;
                }
            }
            return true;
        }
        
        #endregion

        #region (public) Debug-printing Support

        /// <summary>
        /// Pretty-printing debug support
        /// </summary>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("----- Columns::");
            Int32 idx = 0;
            foreach (DefSurrogate c in columns)
            {
                sb.AppendFormat("\t{0}: {1}", idx.ToString(), c.ToString());
                sb.AppendLine();
                idx++;
            }

            sb.AppendLine("----- Rows::");
            idx = 0;
            foreach (DefSurrogate r in rows)
            {
                sb.AppendFormat("\t{0}: {1}", idx.ToString(), r.ToString());
                sb.AppendLine();
                idx++;
            }
            return sb.ToString();
        }

        public static String RowDiffs_ToString(RowDiff[] rds)
        {
            StringBuilder sb = new StringBuilder("Row Differences:");
            sb.AppendLine();
            Int32 idx = 0;
            foreach (RowDiff rd in rds)
            {
                sb.AppendFormat("{0}: {1}", idx.ToString(), rd.ToString());
                sb.AppendLine();
                idx++;
            }
            return sb.ToString();
        }

        public static String ColumnDiffs_ToString(ColumnDiff[] cds)
        {
            StringBuilder sb = new StringBuilder("Column Differences:");
            sb.AppendLine();
            Int32 idx = 0;
            foreach (ColumnDiff cd in cds)
            {
                sb.AppendFormat("{0}: {1}", idx.ToString(), cd.ToString());
                sb.AppendLine();
                idx++;
            }
            return sb.ToString();
        }

        public static String RowRatios_ToString(Double[] ratios)
        {
            StringBuilder sb = new StringBuilder("   RowRatios: ");
            Boolean flag = false;
            foreach (Double val in ratios)
            {
                sb.Append((flag ? ",\t" : String.Empty) + val.ToString());
                flag = true;
            }
            return sb.ToString();
        }

        public static String ColumnRatios_ToString(Double[] ratios)
        {
            StringBuilder sb = new StringBuilder("ColumnRatios: ");
            Boolean flag = false;
            foreach (Double val in ratios)
            {
                sb.Append((flag ? ",\t" : String.Empty) + val.ToString());
                flag = true;
            }
            return sb.ToString();
        }

        #endregion
    }
}
