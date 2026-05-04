using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace DataTableHtmlRenderer
{
    /// <summary>
    /// Renders a DataTable as an HTML table with customizable options.
    /// </summary>
    public sealed class DataTableHtmlRenderer
    {
        private readonly DataTableHtmlRendererOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableHtmlRenderer"/> class with default options.
        /// </summary>
        public DataTableHtmlRenderer()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableHtmlRenderer"/> class with the specified options.
        /// </summary>
        /// <param name="options">The rendering options.</param>
        public DataTableHtmlRenderer(DataTableHtmlRendererOptions options)
        {
            _options = options ?? new DataTableHtmlRendererOptions();
        }

        /// <summary>
        /// Renders the specified DataTable as an HTML table string.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <returns>The HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public string Render(DataTable table)
        {
            return Render(table, _options);
        }

        /// <summary>
        /// Renders the specified DataTable as an HTML table and writes it to the specified writer.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="writer">The writer to write the HTML output to.</param>
        /// <exception cref="ArgumentNullException">Thrown when table or writer is null.</exception>
        public void Render(DataTable table, TextWriter writer)
        {
            Render(table, writer, _options);
        }

        /// <summary>
        /// Renders the specified DataTable as an HTML table string.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="options">The rendering options.</param>
        /// <returns>The HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static string Render(DataTable table, DataTableHtmlRendererOptions options)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            options = options ?? new DataTableHtmlRendererOptions();

            if (table.Rows.Count == 0 && !options.RenderEmptyTable)
            {
                return string.Empty;
            }

            if (table.Rows.Count == 0 && options.EmptyTableHtml != null)
            {
                return options.EmptyTableHtml;
            }

            using (var sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                RenderCore(table, sw, options);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Renders the specified DataTable as an HTML table and writes it to the specified writer.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="writer">The writer to write the HTML output to.</param>
        /// <param name="options">The rendering options.</param>
        /// <exception cref="ArgumentNullException">Thrown when table or writer is null.</exception>
        public static void Render(DataTable table, TextWriter writer, DataTableHtmlRendererOptions options)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            options = options ?? new DataTableHtmlRendererOptions();

            if (table.Rows.Count == 0 && !options.RenderEmptyTable)
            {
                return;
            }

            if (table.Rows.Count == 0 && options.EmptyTableHtml != null)
            {
                writer.Write(options.EmptyTableHtml);
                return;
            }

            RenderCore(table, writer, options);
        }

        private static void RenderCore(DataTable table, TextWriter writer, DataTableHtmlRendererOptions options)
        {
            var columnsToRender = options.GetColumnsToRender(table);

            if (columnsToRender.Count == 0)
            {
                writer.Write(options.EmptyTableHtml ?? "<table></table>");
                return;
            }

            // Build table attributes
            var tableContext = new TableRenderContext(table, table.Rows.Count, columnsToRender.Count);
            var tableAttributes = options.TableAttributes != null
                ? options.TableAttributes(tableContext)
                : HtmlAttributes.Empty;

            // Start table
            writer.Write("<table");
            writer.Write(tableAttributes.ToString());
            writer.Write(">");

            // Render caption
            if (options.CaptionHtml != null)
            {
                writer.Write("<caption>");
                writer.Write(options.CaptionHtml);
                writer.Write("</caption>");
            }
            else if (options.CaptionText != null)
            {
                writer.Write("<caption>");
                writer.Write(HtmlEncoder.Encode(options.CaptionText));
                writer.Write("</caption>");
            }

            // Render header
            if (options.IncludeThead)
            {
                RenderHeader(writer, table, columnsToRender, options);
            }

            // Render body
            if (options.IncludeTbody)
            {
                RenderBody(writer, table, columnsToRender, options);
            }
            else
            {
                // Render rows without tbody
                RenderRows(writer, table, columnsToRender, options);
            }

            // Render footer
            if (options.FooterHtml != null)
            {
                writer.Write("<tfoot>");
                writer.Write(options.FooterHtml);
                writer.Write("</tfoot>");
            }

            // End table
            writer.Write("</table>");
        }

        private static void RenderHeader(TextWriter writer, DataTable table, IList<string> columnsToRender, DataTableHtmlRendererOptions options)
        {
            writer.Write("<thead>");

            var headerRowContext = new HeaderRowRenderContext(table, 0, columnsToRender.Count);
            var headerRowAttributes = options.HeaderRowAttributes != null
                ? options.HeaderRowAttributes(headerRowContext)
                : HtmlAttributes.Empty;

            writer.Write("<tr");
            writer.Write(headerRowAttributes.ToString());
            writer.Write(">");

            for (int i = 0; i < columnsToRender.Count; i++)
            {
                string columnName = columnsToRender[i];
                DataColumn column = table.Columns[columnName];

                var headerCellContext = new HeaderCellRenderContext(
                    table,
                    column,
                    i,
                    columnName,
                    column.Caption ?? string.Empty,
                    column.DataType);

                var headerCellAttributes = options.HeaderCellAttributes != null
                    ? options.HeaderCellAttributes(headerCellContext)
                    : HtmlAttributes.Empty;

                writer.Write("<th");
                writer.Write(headerCellAttributes.ToString());
                writer.Write(">");

                // Get header text
                string headerText = GetHeaderText(headerCellContext, options);
                writer.Write(HtmlEncoder.Encode(headerText));

                writer.Write("</th>");
            }

            writer.Write("</tr>");
            writer.Write("</thead>");
        }

        private static string GetHeaderText(HeaderCellRenderContext context, DataTableHtmlRendererOptions options)
        {
            if (options.HeaderTextSelector != null)
            {
                return options.HeaderTextSelector(context) ?? string.Empty;
            }

            if (options.UseCaptionForHeaders && !string.IsNullOrEmpty(context.Caption))
            {
                return context.Caption;
            }

            return context.ColumnName;
        }

        private static void RenderBody(TextWriter writer, DataTable table, IList<string> columnsToRender, DataTableHtmlRendererOptions options)
        {
            writer.Write("<tbody>");
            RenderRows(writer, table, columnsToRender, options);
            writer.Write("</tbody>");
        }

        private static void RenderRows(TextWriter writer, DataTable table, IList<string> columnsToRender, DataTableHtmlRendererOptions options)
        {
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                DataRow row = table.Rows[rowIndex];

                var bodyRowContext = new BodyRowRenderContext(table, row, rowIndex);
                var bodyRowAttributes = options.BodyRowAttributes != null
                    ? options.BodyRowAttributes(bodyRowContext)
                    : HtmlAttributes.Empty;

                writer.Write("<tr");
                writer.Write(bodyRowAttributes.ToString());
                writer.Write(">");

                for (int colIndex = 0; colIndex < columnsToRender.Count; colIndex++)
                {
                    string columnName = columnsToRender[colIndex];
                    DataColumn column = table.Columns[columnName];
                    object value = row[columnName];
                    bool isDbNull = value == DBNull.Value;

                    var bodyCellContext = new BodyCellRenderContext(
                        table,
                        row,
                        column,
                        rowIndex,
                        colIndex,
                        columnName,
                        column.DataType,
                        value,
                        isDbNull);

                    var bodyCellAttributes = options.BodyCellAttributes != null
                        ? options.BodyCellAttributes(bodyCellContext)
                        : HtmlAttributes.Empty;

                    writer.Write("<td");
                    writer.Write(bodyCellAttributes.ToString());
                    writer.Write(">");

                    // Render cell content
                    RenderCellContent(writer, bodyCellContext, options);

                    writer.Write("</td>");
                }

                writer.Write("</tr>");
            }
        }

        private static void RenderCellContent(TextWriter writer, BodyCellRenderContext context, DataTableHtmlRendererOptions options)
        {
            // Check for CellHtmlSelector first (priority)
            if (options.CellHtmlSelector != null)
            {
                string html = options.CellHtmlSelector(context);
                if (html != null)
                {
                    writer.Write(html);
                    return;
                }
            }

            // Check for CellTextSelector
            if (options.CellTextSelector != null)
            {
                string text = options.CellTextSelector(context);
                if (text != null)
                {
                    writer.Write(HtmlEncoder.Encode(text));
                    return;
                }
            }

            // Check for per-column formatter (more specific than global FormatCellValue)
            if (options.ColumnFormatters != null)
            {
                Func<BodyCellRenderContext, string> colFormatter;
                if (options.ColumnFormatters.TryGetValue(context.ColumnName, out colFormatter) && colFormatter != null)
                {
                    string formatted = colFormatter(context);
                    if (formatted != null)
                    {
                        writer.Write(HtmlEncoder.Encode(formatted));
                        return;
                    }
                }
            }

            // Check for FormatCellValue
            if (options.FormatCellValue != null)
            {
                string formatted = options.FormatCellValue(context);
                if (formatted != null)
                {
                    writer.Write(HtmlEncoder.Encode(formatted));
                    return;
                }
            }

            // Check for FormatValue
            if (options.FormatValue != null)
            {
                string formatted = options.FormatValue(context.Value);
                if (formatted != null)
                {
                    writer.Write(HtmlEncoder.Encode(formatted));
                    return;
                }
            }

            // Check null/DBNull text overrides before default formatting
            if (context.IsDbNull && options.DbNullCellText != null)
            {
                writer.Write(HtmlEncoder.Encode(options.DbNullCellText));
                return;
            }

            if (context.Value == null && options.NullCellText != null)
            {
                writer.Write(HtmlEncoder.Encode(options.NullCellText));
                return;
            }

            // Default formatting
            string defaultText = FormatValue(context.Value, context.DataType, options.FormatProvider);
            writer.Write(HtmlEncoder.Encode(defaultText));
        }

        private static string FormatValue(object value, Type dataType, IFormatProvider formatProvider)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            // Use the provided format provider or the current culture
            IFormatProvider provider = formatProvider ?? CultureInfo.InvariantCulture;

            // Handle common types with special formatting
            if (value is IFormattable formattable)
            {
                return formattable.ToString(null, provider);
            }

            return Convert.ToString(value, provider);
        }
    }
}
