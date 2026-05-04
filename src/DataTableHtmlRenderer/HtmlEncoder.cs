using System;
using System.Net;

namespace DataTableHtmlRenderer
{
    /// <summary>
    /// Provides HTML encoding functionality for safe HTML output.
    /// Delegates to <see cref="System.Net.WebUtility.HtmlEncode(string)"/> and additionally encodes single quotes.
    /// </summary>
    public static class HtmlEncoder
    {
        /// <summary>
        /// Encodes a string for safe HTML output.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <returns>The HTML-encoded string, or an empty string if <paramref name="value"/> is null or empty.</returns>
        public static string Encode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            string encoded = WebUtility.HtmlEncode(value);
            return encoded.IndexOf('\'') >= 0 ? encoded.Replace("'", "&#39;") : encoded;
        }

        /// <summary>
        /// Encodes an object for safe HTML output.
        /// </summary>
        /// <param name="value">The object to encode. <see cref="DBNull.Value"/> and <c>null</c> produce an empty string.</param>
        /// <returns>The HTML-encoded string.</returns>
        public static string Encode(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            return Encode(value.ToString());
        }

        /// <summary>
        /// Encodes a string for safe use in HTML attribute values.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <returns>The HTML-encoded string, or an empty string if <paramref name="value"/> is null or empty.</returns>
        public static string EncodeAttribute(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            string encoded = WebUtility.HtmlEncode(value);
            return encoded.IndexOf('\'') >= 0 ? encoded.Replace("'", "&#39;") : encoded;
        }
    }
}
