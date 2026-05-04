using System;
using System.Data;

namespace DataTableHtmlRenderer
{
    /// <summary>
    /// Provides context information for table rendering.
    /// </summary>
    public sealed class TableRenderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRenderContext"/> class.
        /// </summary>
        /// <param name="table">The DataTable being rendered.</param>
        /// <param name="rowCount">The number of rows in the rendered table.</param>
        /// <param name="columnCount">The number of columns in the rendered table.</param>
        public TableRenderContext(DataTable table, int rowCount, int columnCount)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        /// <summary>
        /// Gets the DataTable being rendered.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the number of rows in the rendered table.
        /// </summary>
        public int RowCount { get; }

        /// <summary>
        /// Gets the number of columns in the rendered table.
        /// </summary>
        public int ColumnCount { get; }
    }

    /// <summary>
    /// Provides context information for header row rendering.
    /// </summary>
    public sealed class HeaderRowRenderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderRowRenderContext"/> class.
        /// </summary>
        /// <param name="table">The DataTable being rendered.</param>
        /// <param name="rowIndex">The index of the header row.</param>
        /// <param name="columnCount">The number of columns in the rendered table.</param>
        public HeaderRowRenderContext(DataTable table, int rowIndex, int columnCount)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            RowIndex = rowIndex;
            ColumnCount = columnCount;
        }

        /// <summary>
        /// Gets the DataTable being rendered.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the index of the header row.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets the number of columns in the rendered table.
        /// </summary>
        public int ColumnCount { get; }
    }

    /// <summary>
    /// Provides context information for header cell rendering.
    /// </summary>
    public sealed class HeaderCellRenderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderCellRenderContext"/> class.
        /// </summary>
        /// <param name="table">The DataTable being rendered.</param>
        /// <param name="column">The DataColumn for this header cell.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="caption">The caption of the column.</param>
        /// <param name="dataType">The data type of the column.</param>
        public HeaderCellRenderContext(
            DataTable table,
            DataColumn column,
            int columnIndex,
            string columnName,
            string caption,
            Type dataType)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Column = column ?? throw new ArgumentNullException(nameof(column));
            ColumnIndex = columnIndex;
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Caption = caption ?? string.Empty;
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        }

        /// <summary>
        /// Gets the DataTable being rendered.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the DataColumn for this header cell.
        /// </summary>
        public DataColumn Column { get; }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the caption of the column.
        /// </summary>
        public string Caption { get; }

        /// <summary>
        /// Gets the data type of the column.
        /// </summary>
        public Type DataType { get; }
    }

    /// <summary>
    /// Provides context information for body row rendering.
    /// </summary>
    public sealed class BodyRowRenderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyRowRenderContext"/> class.
        /// </summary>
        /// <param name="table">The DataTable being rendered.</param>
        /// <param name="row">The DataRow for this body row.</param>
        /// <param name="rowIndex">The index of the row.</param>
        public BodyRowRenderContext(DataTable table, DataRow row, int rowIndex)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Row = row ?? throw new ArgumentNullException(nameof(row));
            RowIndex = rowIndex;
        }

        /// <summary>
        /// Gets the DataTable being rendered.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the DataRow for this body row.
        /// </summary>
        public DataRow Row { get; }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int RowIndex { get; }
    }

    /// <summary>
    /// Provides context information for body cell rendering.
    /// </summary>
    public sealed class BodyCellRenderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyCellRenderContext"/> class.
        /// </summary>
        /// <param name="table">The DataTable being rendered.</param>
        /// <param name="row">The DataRow for this body cell.</param>
        /// <param name="column">The DataColumn for this body cell.</param>
        /// <param name="rowIndex">The index of the row.</param>
        /// <param name="columnIndex">The index of the column.</param>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="dataType">The data type of the column.</param>
        /// <param name="value">The value of the cell.</param>
        /// <param name="isDbNull">Whether the value is DBNull.</param>
        public BodyCellRenderContext(
            DataTable table,
            DataRow row,
            DataColumn column,
            int rowIndex,
            int columnIndex,
            string columnName,
            Type dataType,
            object value,
            bool isDbNull)
        {
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Row = row ?? throw new ArgumentNullException(nameof(row));
            Column = column ?? throw new ArgumentNullException(nameof(column));
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
            Value = value;
            IsDbNull = isDbNull;
        }

        /// <summary>
        /// Gets the DataTable being rendered.
        /// </summary>
        public DataTable Table { get; }

        /// <summary>
        /// Gets the DataRow for this body cell.
        /// </summary>
        public DataRow Row { get; }

        /// <summary>
        /// Gets the DataColumn for this body cell.
        /// </summary>
        public DataColumn Column { get; }

        /// <summary>
        /// Gets the index of the row.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets the index of the column.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the data type of the column.
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// Gets the value of the cell.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets whether the value is DBNull.
        /// </summary>
        public bool IsDbNull { get; }
    }
}
