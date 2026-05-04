using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DataTableHtmlRenderer
{
    /// <summary>
    /// Options for customizing the HTML table rendering behavior.
    /// </summary>
    public sealed class DataTableHtmlRendererOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableHtmlRendererOptions"/> class.
        /// </summary>
        public DataTableHtmlRendererOptions()
        {
            UseCaptionForHeaders = true;
            IncludeThead = true;
            IncludeTbody = true;
            RenderEmptyTable = true;
            EmptyTableHtml = null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use DataColumn.Caption for header text when available.
        /// When true and Caption is not empty, Caption is used; otherwise, ColumnName is used.
        /// </summary>
        public bool UseCaptionForHeaders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the thead element.
        /// </summary>
        public bool IncludeThead { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include the tbody element.
        /// </summary>
        public bool IncludeTbody { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to render an empty table.
        /// When false and the table has no rows, returns an empty string.
        /// </summary>
        public bool RenderEmptyTable { get; set; }

        /// <summary>
        /// Gets or sets the HTML to render for an empty table.
        /// Used when RenderEmptyTable is true and the table has no rows.
        /// </summary>
        public string EmptyTableHtml { get; set; }

        /// <summary>
        /// Gets or sets the list of column names to include in the rendered table.
        /// If set, only these columns will be rendered, in the order specified by this list.
        /// </summary>
        public IReadOnlyList<string> IncludedColumns { get; set; }

        /// <summary>
        /// Gets or sets the list of column names to exclude from the rendered table.
        /// Columns in this list will be excluded from rendering.
        /// </summary>
        public IReadOnlyList<string> ExcludedColumns { get; set; }

        /// <summary>
        /// Gets or sets a function to provide HTML attributes for the table element.
        /// </summary>
        public Func<TableRenderContext, HtmlAttributes> TableAttributes { get; set; }

        /// <summary>
        /// Gets or sets a function to provide HTML attributes for header row elements.
        /// </summary>
        public Func<HeaderRowRenderContext, HtmlAttributes> HeaderRowAttributes { get; set; }

        /// <summary>
        /// Gets or sets a function to provide HTML attributes for header cell elements.
        /// </summary>
        public Func<HeaderCellRenderContext, HtmlAttributes> HeaderCellAttributes { get; set; }

        /// <summary>
        /// Gets or sets a function to provide HTML attributes for body row elements.
        /// </summary>
        public Func<BodyRowRenderContext, HtmlAttributes> BodyRowAttributes { get; set; }

        /// <summary>
        /// Gets or sets a function to provide HTML attributes for body cell elements.
        /// </summary>
        public Func<BodyCellRenderContext, HtmlAttributes> BodyCellAttributes { get; set; }

        /// <summary>
        /// Gets or sets a function to select the text for header cells.
        /// The returned text will be HTML encoded.
        /// </summary>
        public Func<HeaderCellRenderContext, string> HeaderTextSelector { get; set; }

        /// <summary>
        /// Gets or sets a function to select the text for body cells.
        /// The returned text will be HTML encoded.
        /// </summary>
        public Func<BodyCellRenderContext, string> CellTextSelector { get; set; }

        /// <summary>
        /// Gets or sets a function to select the HTML for body cells.
        /// The returned HTML will NOT be encoded and is assumed to be safe.
        /// This takes priority over CellTextSelector when both are set.
        /// </summary>
        public Func<BodyCellRenderContext, string> CellHtmlSelector { get; set; }

        /// <summary>
        /// Gets or sets a function to format values for display.
        /// This function should return text (not HTML) which will be HTML encoded.
        /// </summary>
        public Func<object, string> FormatValue { get; set; }

        /// <summary>
        /// Gets or sets a function to format cell values for display.
        /// This function receives the full cell context and should return text (not HTML) which will be HTML encoded.
        /// This takes priority over FormatValue when both are set.
        /// </summary>
        public Func<BodyCellRenderContext, string> FormatCellValue { get; set; }

        /// <summary>
        /// Gets or sets the format provider to use for formatting values.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }

        /// <summary>
        /// Gets or sets the raw HTML to render inside the caption element.
        /// When set, a caption element is rendered immediately after the opening table tag.
        /// Takes priority over <see cref="CaptionText"/> when both are set.
        /// The value is written as-is without HTML encoding.
        /// </summary>
        public string CaptionHtml { get; set; }

        /// <summary>
        /// Gets or sets the plain text to render inside the caption element.
        /// When set, a caption element is rendered immediately after the opening table tag.
        /// The value is HTML encoded. Ignored when <see cref="CaptionHtml"/> is set.
        /// </summary>
        public string CaptionText { get; set; }

        /// <summary>
        /// Gets or sets the raw HTML to render inside the tfoot element.
        /// When set, a tfoot element is rendered after the tbody (or rows) and before the closing table tag.
        /// The value is written as-is without HTML encoding.
        /// </summary>
        public string FooterHtml { get; set; }

        /// <summary>
        /// Gets or sets per-column formatters keyed by column name.
        /// Each formatter receives a <see cref="BodyCellRenderContext"/> and returns a plain text string that is HTML encoded.
        /// Per-column formatters take priority over <see cref="FormatCellValue"/> when both match.
        /// </summary>
        public Dictionary<string, Func<BodyCellRenderContext, string>> ColumnFormatters { get; set; }

        /// <summary>
        /// Gets or sets the text to render when a cell value is C# null (not DBNull).
        /// The value is HTML encoded. When null (default), an empty string is rendered.
        /// </summary>
        public string NullCellText { get; set; }

        /// <summary>
        /// Gets or sets the text to render when a cell value is DBNull.Value.
        /// The value is HTML encoded. When null (default), an empty string is rendered.
        /// </summary>
        public string DbNullCellText { get; set; }

        /// <summary>
        /// Determines whether a column should be included in the rendered table.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>True if the column should be included; otherwise, false.</returns>
        internal bool ShouldIncludeColumn(string columnName)
        {
            // If IncludedColumns is set, only include columns in that list
            if (IncludedColumns != null && IncludedColumns.Count > 0)
            {
                return IncludedColumns.Contains(columnName);
            }

            // If ExcludedColumns is set, exclude columns in that list
            if (ExcludedColumns != null && ExcludedColumns.Count > 0)
            {
                return !ExcludedColumns.Contains(columnName);
            }

            // By default, include all columns
            return true;
        }

        /// <summary>
        /// Gets the columns to render in order.
        /// </summary>
        /// <param name="table">The DataTable being rendered.</param>
        /// <returns>An ordered list of column names to render.</returns>
        internal IList<string> GetColumnsToRender(DataTable table)
        {
            var result = new List<string>();

            if (table == null || table.Columns == null)
            {
                return result;
            }

            // If IncludedColumns is set, iterate in IncludedColumns order (explicit ordering)
            if (IncludedColumns != null && IncludedColumns.Count > 0)
            {
                var excludedSet = ExcludedColumns != null && ExcludedColumns.Count > 0
                    ? new HashSet<string>(ExcludedColumns, StringComparer.OrdinalIgnoreCase)
                    : null;

                foreach (string name in IncludedColumns)
                {
                    if (table.Columns.Contains(name) && (excludedSet == null || !excludedSet.Contains(name)))
                    {
                        result.Add(name);
                    }
                }
            }
            else
            {
                // Use DataTable column order, excluding any in ExcludedColumns
                foreach (DataColumn column in table.Columns)
                {
                    if (ShouldIncludeColumn(column.ColumnName))
                    {
                        result.Add(column.ColumnName);
                    }
                }
            }

            return result;
        }
    }
}
