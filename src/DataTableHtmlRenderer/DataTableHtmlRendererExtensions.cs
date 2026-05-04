using System;
using System.Data;
using System.IO;

namespace DataTableHtmlRenderer
{
    /// <summary>
    /// Extension methods for rendering DataTable as HTML.
    /// </summary>
    public static class DataTableHtmlRendererExtensions
    {
        /// <summary>
        /// Renders the DataTable as an HTML table string.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <returns>The HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static string ToHtmlTable(this DataTable table)
        {
            return DataTableHtmlRenderer.Render(table, (DataTableHtmlRendererOptions)null);
        }

        /// <summary>
        /// Renders the DataTable as an HTML table string with the specified options.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="options">The rendering options.</param>
        /// <returns>The HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static string ToHtmlTable(this DataTable table, DataTableHtmlRendererOptions options)
        {
            return DataTableHtmlRenderer.Render(table, options);
        }

        /// <summary>
        /// Renders the DataTable as an HTML table and writes it directly to the specified writer.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="writer">The writer to write the HTML output to.</param>
        /// <exception cref="ArgumentNullException">Thrown when table or writer is null.</exception>
        public static void WriteHtmlTo(this DataTable table, TextWriter writer)
        {
            DataTableHtmlRenderer.Render(table, writer, (DataTableHtmlRendererOptions)null);
        }

        /// <summary>
        /// Renders the DataTable as an HTML table with the specified options and writes it directly to the specified writer.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="writer">The writer to write the HTML output to.</param>
        /// <param name="options">The rendering options.</param>
        /// <exception cref="ArgumentNullException">Thrown when table or writer is null.</exception>
        public static void WriteHtmlTo(this DataTable table, TextWriter writer, DataTableHtmlRendererOptions options)
        {
            DataTableHtmlRenderer.Render(table, writer, options);
        }
    }
}
