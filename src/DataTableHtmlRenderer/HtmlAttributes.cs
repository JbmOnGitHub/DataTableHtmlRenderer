using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DataTableHtmlRenderer
{
    /// <summary>
    /// Represents a collection of HTML attributes with safe encoding and validation.
    /// </summary>
    public sealed class HtmlAttributes
    {
        private static readonly HashSet<string> ValidDataPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "data-",
            "aria-"
        };

        private static readonly HashSet<string> ForbiddenAttributeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "onclick",
            "ondblclick",
            "onmouseover",
            "onmouseout",
            "onmousemove",
            "onmousedown",
            "onmouseup",
            "onkeydown",
            "onkeyup",
            "onkeypress",
            "onload",
            "onunload",
            "onerror",
            "onabort",
            "onscroll",
            "onresize",
            "onfocus",
            "onblur",
            "onsubmit",
            "onreset",
            "onchange",
            "onselect",
            "oninput",
            "javascript:",
            "vbscript:"
        };

        private readonly SortedDictionary<string, string> _attributes = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _classes = new List<string>();

        /// <summary>
        /// Gets an empty HtmlAttributes instance.
        /// </summary>
        public static HtmlAttributes Empty => new HtmlAttributes();

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlAttributes"/> class.
        /// </summary>
        public HtmlAttributes()
        {
        }

        /// <summary>
        /// Adds a CSS class to the attributes.
        /// </summary>
        /// <param name="className">The CSS class name to add.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes AddClass(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return this;
            }

            _classes.Add(className.Trim());
            return this;
        }

        /// <summary>
        /// Adds multiple CSS classes to the attributes.
        /// </summary>
        /// <param name="classNames">The CSS class names to add.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes AddClasses(params string[] classNames)
        {
            if (classNames == null)
            {
                return this;
            }

            foreach (var className in classNames)
            {
                AddClass(className);
            }

            return this;
        }

        /// <summary>
        /// Adds multiple CSS classes to the attributes.
        /// </summary>
        /// <param name="classNames">The CSS class names to add.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes AddClasses(IEnumerable<string> classNames)
        {
            if (classNames == null)
            {
                return this;
            }

            foreach (var className in classNames)
            {
                AddClass(className);
            }

            return this;
        }

        /// <summary>
        /// Sets an attribute with the specified name and value.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The current instance for method chaining.</returns>
        /// <exception cref="ArgumentException">Thrown when the attribute name is invalid or forbidden.</exception>
        public HtmlAttributes Set(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return this;
            }

            ValidateAttributeName(name);

            if (string.IsNullOrEmpty(value))
            {
                _attributes.Remove(name);
            }
            else
            {
                ValidateAttributeValue(value);
                _attributes[name] = value;
            }

            return this;
        }

        /// <summary>
        /// Sets an attribute with the specified name and integer value.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Set(string name, int value)
        {
            return Set(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Sets an attribute with the specified name and boolean value.
        /// </summary>
        /// <param name="name">The attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Set(string name, bool value)
        {
            return Set(name, value.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
        }

        /// <summary>
        /// Adds a data attribute (data-*) with the specified name and value.
        /// </summary>
        /// <param name="name">The data attribute name (without 'data-' prefix).</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Data(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return this;
            }

            return Set("data-" + name.ToLowerInvariant(), value);
        }

        /// <summary>
        /// Adds an aria attribute (aria-*) with the specified name and value.
        /// </summary>
        /// <param name="name">The aria attribute name (without 'aria-' prefix).</param>
        /// <param name="value">The attribute value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Aria(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                return this;
            }

            return Set("aria-" + name.ToLowerInvariant(), value);
        }

        /// <summary>
        /// Sets the id attribute.
        /// </summary>
        /// <param name="value">The id value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Id(string value)
        {
            return Set("id", value);
        }

        /// <summary>
        /// Sets the style attribute.
        /// </summary>
        /// <param name="value">The style value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Style(string value)
        {
            return Set("style", value);
        }

        /// <summary>
        /// Sets the title attribute.
        /// </summary>
        /// <param name="value">The title value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Title(string value)
        {
            return Set("title", value);
        }

        /// <summary>
        /// Sets the lang attribute.
        /// </summary>
        /// <param name="value">The lang value.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Lang(string value)
        {
            return Set("lang", value);
        }

        /// <summary>
        /// Merges another HtmlAttributes instance into this one.
        /// </summary>
        /// <param name="other">The other HtmlAttributes instance to merge.</param>
        /// <returns>The current instance for method chaining.</returns>
        public HtmlAttributes Merge(HtmlAttributes other)
        {
            if (other == null)
            {
                return this;
            }

            foreach (var kvp in other._attributes)
            {
                _attributes[kvp.Key] = kvp.Value;
            }

            _classes.AddRange(other._classes);
            return this;
        }

        /// <summary>
        /// Validates that the attribute value does not contain a dangerous URI scheme.
        /// Strips whitespace and control characters before checking, matching browser normalization behavior.
        /// </summary>
        /// <param name="value">The attribute value to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the value uses a forbidden URI scheme.</exception>
        private static void ValidateAttributeValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (!char.IsWhiteSpace(c) && !char.IsControl(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                }
            }

            string normalized = sb.ToString();

            if (normalized.StartsWith("javascript:", StringComparison.Ordinal) ||
                normalized.StartsWith("vbscript:", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Attribute values using javascript: or vbscript: URI schemes are not allowed for security reasons.",
                    "value");
            }
        }

        /// <summary>
        /// Validates that the attribute name is safe to use.
        /// </summary>
        /// <param name="name">The attribute name to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the attribute name is invalid or forbidden.</exception>
        private void ValidateAttributeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string lowerName = name.ToLowerInvariant();

            // Check for forbidden attribute names
            if (ForbiddenAttributeNames.Contains(lowerName))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Attribute name '{0}' is not allowed for security reasons.", name),
                    nameof(name));
            }

            // Check if it starts with a valid prefix
            foreach (var prefix in ValidDataPrefixes)
            {
                if (lowerName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            // Check if it's a valid HTML attribute name (letters, digits, hyphens, underscores)
            foreach (char c in name)
            {
                if (!IsValidAttributeNameChar(c))
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture, "Attribute name '{0}' contains invalid characters.", name),
                        nameof(name));
                }
            }
        }

        /// <summary>
        /// Determines whether a character is valid for an HTML attribute name.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>True if the character is valid; otherwise, false.</returns>
        private static bool IsValidAttributeNameChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == ':' || c == '.';
        }

        /// <summary>
        /// Renders the attributes as an HTML attribute string.
        /// </summary>
        /// <returns>The rendered HTML attribute string.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            // Render classes
            if (_classes.Count > 0)
            {
                sb.Append(" class=\"");
                for (int i = 0; i < _classes.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(HtmlEncoder.Encode(_classes[i]));
                }
                sb.Append("\"");
            }

            // Render other attributes
            foreach (var kvp in _attributes)
            {
                sb.Append(' ');
                sb.Append(kvp.Key);
                sb.Append("=\"");
                sb.Append(HtmlEncoder.Encode(kvp.Value));
                sb.Append("\"");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a deep copy of this HtmlAttributes instance.
        /// </summary>
        /// <returns>A new HtmlAttributes instance with the same attributes and classes.</returns>
        public HtmlAttributes Clone()
        {
            var clone = new HtmlAttributes();
            foreach (var kvp in _attributes)
            {
                clone._attributes[kvp.Key] = kvp.Value;
            }
            clone._classes.AddRange(_classes);
            return clone;
        }
    }
}
