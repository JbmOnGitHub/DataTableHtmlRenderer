using System;
using System.Data;
using System.Web;

namespace DataTableHtmlRenderer.AspNetFx
{
    /// <summary>
    /// Extension methods for ASP.NET Framework integration.
    /// </summary>
    public static class DataTableHtmlRendererExtensions
    {
        /// <summary>
        /// Renders the DataTable as an IHtmlString for use in ASP.NET Framework Razor views.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <returns>An IHtmlString containing the HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static IHtmlString ToMvcHtmlString(this DataTable table)
        {
            return ToMvcHtmlString(table, null);
        }

        /// <summary>
        /// Renders the DataTable as an IHtmlString for use in ASP.NET Framework Razor views with the specified options.
        /// </summary>
        /// <param name="table">The DataTable to render.</param>
        /// <param name="options">The rendering options.</param>
        /// <returns>An IHtmlString containing the HTML representation of the table.</returns>
        /// <exception cref="ArgumentNullException">Thrown when table is null.</exception>
        public static IHtmlString ToMvcHtmlString(this DataTable table, DataTableHtmlRendererOptions options)
        {
            string html = global::DataTableHtmlRenderer.DataTableHtmlRenderer.Render(table, options);
            return new HtmlString(html);
        }
    }
}
