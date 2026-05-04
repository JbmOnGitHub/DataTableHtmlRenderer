using System;
using System.Data;
using Microsoft.AspNetCore.Html;

namespace DataTableHtmlRenderer.AspNet
{
    /// <summary>
    /// Extension methods for ASP.NET integration.
    /// </summary>
    public static class DataTableHtmlRendererExtensions
    {
        /// <summary>
        /// Renders the DataTable as an IHtmlContent for use in ASP.NET Razor views.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <returns>An IHtmlContent containing the HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static IHtmlContent ToHtmlContent(this DataTable table)
        {
            return ToHtmlContent(table, null);
        }

        /// <summary>
        /// Renders the DataTable as an IHtmlContent for use in ASP.NET Razor views with the specified options.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="options">The rendering options.</param>
        /// <returns>An IHtmlContent containing the HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static IHtmlContent ToHtmlContent(this DataTable table, DataTableHtmlRendererOptions options)
        {
            string html = global::DataTableHtmlRenderer.DataTableHtmlRenderer.Render(table, options);
            return new HtmlString(html);
        }
    }
}
