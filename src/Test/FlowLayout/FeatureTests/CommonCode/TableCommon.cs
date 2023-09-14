// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Threading; using System.Windows.Threading;


namespace Microsoft.Test.Layout
{
	/// <summary></summary>
	public class BasicTable : FrameworkContentElement
	{
		private Table _tbl;

		/// <summary></summary>
		/// <returns></returns>
		public Table Tbl
		{
			get
			{
				return _tbl;
			}
		}

		/// <summary></summary>
		public BasicTable()
		{
			_tbl = new Table();
		}
		
		/// <summary></summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public TableRowGroup CreateBody(Dispatcher context)
		{
                    TableRowGroup bdy = null;

                    context.Invoke(
                        DispatcherPriority.Normal,
                         new DispatcherOperationCallback(delegate {
                            bdy = new TableRowGroup();
			_tbl.RowGroups.Add(bdy);
                            return null;
                        }),null
                    );
                    return bdy;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="width"></param>
		/// <param name="background"></param>
		/// <returns></returns>
		public TableColumn CreateColumn(Dispatcher context, GridLength width, Brush background)
		{
                    TableColumn col = null;

                    context.Invoke(
                        DispatcherPriority.Normal,
                        new DispatcherOperationCallback(delegate {
                            col = new TableColumn();
			_tbl.Columns.Add(col);
			col.Width = width;
			col.Background = background;
                            return null;
                        }),null
                    );
                    return col;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="background"></param>
		/// <returns></returns>
		public TableColumn CreateColumn(Dispatcher context, Brush background)
		{
                    TableColumn col = null;

                    context.Invoke(
                        DispatcherPriority.Normal,
                        new DispatcherOperationCallback(delegate {
                            col = new TableColumn();
			_tbl.Columns.Add(col);
			col.Background = background;
                            return null;
                        }),null
                    );
                    return col;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="width"></param>
		/// <returns></returns>
		public TableColumn CreateColumn(Dispatcher context, GridLength width)
		{
                    TableColumn col = null;

                    context.Invoke(
                        DispatcherPriority.Normal,
                        new DispatcherOperationCallback(delegate {
                            col = new TableColumn();
			_tbl.Columns.Add(col);
			col.Width = width;
                            return null;
                        }),null
                    );
                    return col;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public TableColumn CreateColumn(Dispatcher context)
		{
                    TableColumn col = null;

                    context.Invoke(
                        DispatcherPriority.Normal,
                        new DispatcherOperationCallback(delegate {
                            col = new TableColumn();
			_tbl.Columns.Add(col);
                            return null;
                        }),null
                    );

                    return col;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="rowgroup"></param>
		/// <returns></returns>
		public TableRow CreateRow(Dispatcher context, TableRowGroup rowgroup)
		{
                    TableRow row = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                row = new TableRow();
			rowgroup.Rows.Add(row);
                                return null;
                        }),null
                    );

                    return row;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="rowgroup"></param>
		/// <param name="background"></param>
		/// <returns></returns>
		public TableRow CreateRow(Dispatcher context, TableRowGroup rowgroup, Brush background)
		{
                    TableRow row = null;

                    context.Invoke(
                        DispatcherPriority.Normal,
                        new DispatcherOperationCallback(delegate {
                                row = new TableRow();
			rowgroup.Rows.Add(row);
			row.Background = background;
                                return null;
                        }),null
                    );

                    return row;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="rowgroup"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public TableRow CreateRow(Dispatcher context, TableRowGroup rowgroup, double height)
		{
			TableRow row = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                row = new TableRow();
			rowgroup.Rows.Add(row);
                                return null;
                            }),null
                        );

			return row;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="row"></param>
		/// <param name="colspan"></param>
		/// <param name="rowspan"></param>
		/// <returns></returns>
		public TableCell CreateCell(Dispatcher context, TableRow row, int colspan, int rowspan)
		{
			TableCell cell = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                cell = new TableCell();
			row.Cells.Add(cell);
			cell.ColumnSpan = colspan;
			cell.RowSpan = rowspan;
			cell.BorderThickness = new Thickness(1);
			cell.BorderBrush = Brushes.Black;
                                return null;
                            }),null
                        );

			return cell;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="row"></param>
		/// <param name="colspan"></param>
		/// <param name="rowspan"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public TableCell CreateCell(Dispatcher context, TableRow row, int colspan, int rowspan, string text)
		{
            Paragraph para = new Paragraph(new Run(text));
            TableCell cell = null;
            context.Invoke(
                DispatcherPriority.Normal,
                new DispatcherOperationCallback(delegate {
                    cell = new TableCell(para);
                    row.Cells.Add(cell);
                    cell.ColumnSpan = colspan;
                    cell.RowSpan = rowspan;
                    cell.BorderThickness = new Thickness(1);
                    cell.BorderBrush = Brushes.Black;
                    return null;
                }), null);

			return cell;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="row"></param>
		/// <returns></returns>
		public TableCell CreateCell(Dispatcher context, TableRow row)
		{
			byte color = 0;
			TableCell cell = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                cell = new TableCell();
			row.Cells.Add(cell);
			cell.BorderThickness = new Thickness(1);
			cell.BorderBrush = Brushes.Black;
			color += 50;
                                return null;
                            }),null
                        );
			return cell;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="rowgroup"></param>
		/// <param name="cCellCnt"></param>
		/// <returns></returns>
		public TableRow CreateRow(Dispatcher context, TableRowGroup rowgroup, int cCellCnt)
		{
			TableRow row = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                row = new TableRow();
			rowgroup.Rows.Add(row);

			string rowGroupName = "Body";

			for (int i = 0; i < cCellCnt; i++)
			{
				CreateCell(context, row, rowGroupName + ":" + rowgroup.Rows.Count + ":" + i);
			}

                                return null;
                            }),null
                        );

                        return row;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="rowgroup"></param>
		/// <param name="background"></param>
		/// <param name="cCellCnt"></param>
		/// <returns></returns>
		public TableRow CreateRow(Dispatcher context, TableRowGroup rowgroup, Brush background, int cCellCnt)
		{
                        TableRow row = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                row = new TableRow();
			row.Background = background;
			rowgroup.Rows.Add(row);

			string rowGroupName = "Body";

			for (int i = 0; i < cCellCnt; i++)
			{
				CreateCell(context, row, rowGroupName + ":" + rowgroup.Rows.Count + ":" + i);
			}

                                return null;
                            }),null
                        );

                        return row;
		}

		/// <summary></summary>
		/// <param name="context"></param>
		/// <param name="row"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public TableCell CreateCell(Dispatcher context, TableRow row, string text)
		{
            Paragraph para = new Paragraph(new Run(text));
            TableCell cell = null;

                        context.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(delegate {
                                cell = new TableCell(para);
			row.Cells.Add(cell);
            cell.BorderThickness = new Thickness(1);
			cell.BorderBrush = Brushes.Black;
                                return null;
                            }),null
                        );

			return (cell);
		}

        /// <summary></summary>
        /// <returns></returns>
        public TableRowGroup CreateBody()
        {
            TableRowGroup bdy = new TableRowGroup();
                _tbl.RowGroups.Add(bdy);
                
            return bdy;
        }

        /// <summary></summary>
        /// <param name="width"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public TableColumn CreateColumn(GridLength width, Brush background)
        {
            TableColumn  col = new TableColumn();
                _tbl.Columns.Add(col);
                col.Width = width;
                col.Background = background;

            return col;
        }

        /// <summary></summary>
        /// <param name="background"></param>
        /// <returns></returns>
        public TableColumn CreateColumn(Brush background)
        {
            TableColumn  col = new TableColumn();
                _tbl.Columns.Add(col);
                col.Background = background;

            return col;
        }

        /// <summary></summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public TableColumn CreateColumn(GridLength width)
        {
            TableColumn col = new TableColumn();
                _tbl.Columns.Add(col);
                col.Width = width;

            return col;
        }

        /// <summary></summary>
        /// <returns></returns>
        public TableColumn CreateColumn()
        {
            TableColumn  col = new TableColumn();
                _tbl.Columns.Add(col);

            return col;
        }

        /// <summary></summary>
        /// <param name="rowgroup"></param>
        /// <returns></returns>
        public TableRow CreateRow(TableRowGroup rowgroup)
        {
            TableRow row = new TableRow();
                rowgroup.Rows.Add(row);


            return row;
        }

        /// <summary></summary>
       /// <param name="rowgroup"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public TableRow CreateRow(TableRowGroup rowgroup, Brush background)
        {
            TableRow row = new TableRow();
                rowgroup.Rows.Add(row);
                row.Background = background;


            return row;
        }

        /// <summary></summary>
        /// <param name="rowgroup"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public TableRow CreateRow(TableRowGroup rowgroup, double height)
        {
            TableRow row = new TableRow();
                rowgroup.Rows.Add(row);


            return row;
        }

        /// <summary></summary>
        /// <param name="row"></param>
        /// <param name="colspan"></param>
        /// <param name="rowspan"></param>
        /// <returns></returns>
        public TableCell CreateCell(TableRow row, int colspan, int rowspan)
        {
            TableCell cell = new TableCell(new Paragraph());
                row.Cells.Add(cell);
                cell.ColumnSpan = colspan;
                cell.RowSpan = rowspan;
                cell.BorderThickness = new Thickness(1);
                cell.BorderBrush = Brushes.Black;

            return cell;
        }

        /// <summary></summary>
        /// <param name="row"></param>
        /// <param name="colspan"></param>
        /// <param name="rowspan"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public TableCell CreateCell(TableRow row, int colspan, int rowspan, string text)
        {
            Paragraph para = new Paragraph(new Run(text));
            TableCell cell = new TableCell(para);

            cell.ColumnSpan = colspan;
            cell.RowSpan = rowspan;
            cell.BorderThickness = new Thickness(1);
            cell.BorderBrush = Brushes.Black;

            row.Cells.Add(cell);

            return cell;
        }

        /// <summary></summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public TableCell CreateCell(TableRow row)
        {
            byte color = 0;
            TableCell cell = new TableCell();
                row.Cells.Add(cell);
                cell.BorderThickness = new Thickness(1);
                cell.BorderBrush = Brushes.Black;
                color += 50;

            return cell;
        }

        /// <summary></summary>
        /// <param name="rowgroup"></param>
        /// <param name="cCellCnt"></param>
        /// <returns></returns>
        public TableRow CreateRow(TableRowGroup rowgroup, int cCellCnt)
        {
            TableRow row = new TableRow();
                rowgroup.Rows.Add(row);

                string rowGroupName = "Body";

                for (int i = 0; i < cCellCnt; i++)
                {
                    CreateCell(row, rowGroupName + ":" + rowgroup.Rows.Count + ":" + i);
                }
            return row;
        }

        /// <summary></summary>
        /// <param name="rowgroup"></param>
        /// <param name="background"></param>
        /// <param name="cCellCnt"></param>
        /// <returns></returns>
        public TableRow CreateRow(TableRowGroup rowgroup, Brush background, int cCellCnt)
        {
            TableRow row = new TableRow();
                row.Background = background;
                rowgroup.Rows.Add(row);

                string rowGroupName = "Body";

                for (int i = 0; i < cCellCnt; i++)
                {
                    CreateCell(row, rowGroupName + ":" + rowgroup.Rows.Count + ":" + i);
                }
            return row;
        }

        /// <summary></summary>
        /// <param name="row"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public TableCell CreateCell(TableRow row, string text)
        {
            Paragraph para = new Paragraph(new Run(text));
            TableCell cell = new TableCell(para);
                row.Cells.Add(cell);
                cell.BorderThickness = new Thickness(1);
                cell.BorderBrush = Brushes.Black;


            return (cell);
        }    

		double _cellSpacing;

		Brush _Background;

		/// <summary></summary>
		/// <returns></returns>
		public double CellSpacing
		{
			set
			{
				_cellSpacing = value;
				_tbl.CellSpacing = _cellSpacing;
			}
			get
			{
				return _cellSpacing;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		public Brush Background
		{
			set
			{
				_Background = value;
				_tbl.Background = _Background;
			}
			get
			{
				return _Background;
			}
		}
	}
}
