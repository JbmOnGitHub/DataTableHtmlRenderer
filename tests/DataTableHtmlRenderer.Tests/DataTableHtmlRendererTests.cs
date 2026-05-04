using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using DataTableHtmlRenderer;
using Xunit;

namespace DataTableHtmlRenderer.Tests
{
    public class DataTableHtmlRendererTests
    {
        private DataTable CreateTestTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("CreatedAt", typeof(DateTime));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("IsActive", typeof(bool));

            table.Rows.Add(1, "Alice", new DateTime(2023, 1, 15), 100.50m, true);
            table.Rows.Add(2, "Bob", new DateTime(2023, 2, 20), 200.75m, false);

            return table;
        }

        [Fact]
        public void ToHtmlTable_ShouldThrowForNullTable()
        {
            Assert.Throws<ArgumentNullException>(() => DataTableHtmlRenderer.Render(null, (DataTableHtmlRendererOptions)null));
        }

        [Fact]
        public void ToHtmlTable_ShouldRenderSimpleTable()
        {
            var table = CreateTestTable();
            string html = table.ToHtmlTable();

            Assert.Contains("<table>", html);
            Assert.Contains("</table>", html);
            Assert.Contains("<thead>", html);
            Assert.Contains("</thead>", html);
            Assert.Contains("<tbody>", html);
            Assert.Contains("</tbody>", html);
            Assert.Contains("<th>Id</th>", html);
            Assert.Contains("<th>Name</th>", html);
            Assert.Contains("<th>CreatedAt</th>", html);
            Assert.Contains("<th>Amount</th>", html);
            Assert.Contains("<th>IsActive</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldRenderDataRows()
        {
            var table = CreateTestTable();
            string html = table.ToHtmlTable();

            Assert.Contains("<td>1</td>", html);
            Assert.Contains("<td>Alice</td>", html);
            Assert.Contains("<td>2</td>", html);
            Assert.Contains("<td>Bob</td>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleEmptyTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id");
            table.Columns.Add("Name");

            string html = table.ToHtmlTable();
            Assert.Contains("<th>Id</th>", html);
            Assert.Contains("<th>Name</th>", html);
            Assert.Contains("<tbody></tbody>", html);
            Assert.DoesNotContain("<td>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleNullTableWithRenderEmptyTableFalse()
        {
            var options = new DataTableHtmlRendererOptions
            {
                RenderEmptyTable = false
            };

            var table = new DataTable();
            table.Columns.Add("Id");

            string html = table.ToHtmlTable(options);
            Assert.Equal("", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleDBNull()
        {
            var table = new DataTable();
            table.Columns.Add("Value");
            table.Rows.Add(DBNull.Value);

            string html = table.ToHtmlTable();
            Assert.Contains("<td></td>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleNullValues()
        {
            var table = new DataTable();
            table.Columns.Add("Value");
            table.Rows.Add((object)null);

            string html = table.ToHtmlTable();
            Assert.Contains("<td></td>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldEncodeHtmlInValues()
        {
            var table = new DataTable();
            table.Columns.Add("Value");
            table.Rows.Add("<script>alert(1)</script>");

            string html = table.ToHtmlTable();
            Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", html);
            Assert.DoesNotContain("<script>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldEncodeHtmlInHeaders()
        {
            var table = new DataTable();
            table.Columns.Add("<script>alert(1)</script>");

            string html = table.ToHtmlTable();
            Assert.Contains("&lt;script&gt;alert(1)&lt;/script&gt;", html);
            Assert.DoesNotContain("<script>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseCaptionForHeaders()
        {
            var table = new DataTable();
            var column = table.Columns.Add("Id");
            column.Caption = "Identifier";

            var options = new DataTableHtmlRendererOptions
            {
                UseCaptionForHeaders = true
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<th>Identifier</th>", html);
            Assert.DoesNotContain("<th>Id</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseColumnNameWhenCaptionEmpty()
        {
            var table = new DataTable();
            var column = table.Columns.Add("Id");
            column.Caption = "";

            var options = new DataTableHtmlRendererOptions
            {
                UseCaptionForHeaders = true
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<th>Id</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldIncludeColumns()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                IncludedColumns = new List<string> { "Id", "Name" }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<th>Id</th>", html);
            Assert.Contains("<th>Name</th>", html);
            Assert.DoesNotContain("<th>CreatedAt</th>", html);
            Assert.DoesNotContain("<th>Amount</th>", html);
            Assert.DoesNotContain("<th>IsActive</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldExcludeColumns()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                ExcludedColumns = new List<string> { "CreatedAt", "Amount", "IsActive" }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<th>Id</th>", html);
            Assert.Contains("<th>Name</th>", html);
            Assert.DoesNotContain("<th>CreatedAt</th>", html);
            Assert.DoesNotContain("<th>Amount</th>", html);
            Assert.DoesNotContain("<th>IsActive</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldPreserveColumnOrder()
        {
            var table = new DataTable();
            table.Columns.Add("Z");
            table.Columns.Add("A");
            table.Columns.Add("M");

            table.Rows.Add("z", "a", "m");

            string html = table.ToHtmlTable();

            int zIndex = html.IndexOf("<th>Z</th>");
            int aIndex = html.IndexOf("<th>A</th>");
            int mIndex = html.IndexOf("<th>M</th>");

            Assert.True(zIndex < aIndex);
            Assert.True(aIndex < mIndex);
        }

        [Fact]
        public void ToHtmlTable_ShouldApplyTableAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                TableAttributes = ctx => HtmlAttributes.Empty
                    .AddClass("table")
                    .AddClass("table-striped")
                    .Id("myTable")
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("class=\"table table-striped\"", html);
            Assert.Contains("id=\"myTable\"", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldApplyHeaderRowAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                HeaderRowAttributes = ctx => HtmlAttributes.Empty.AddClass("header-row")
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<tr class=\"header-row\">", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldApplyHeaderCellAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                HeaderCellAttributes = ctx =>
                {
                    if (ctx.ColumnName == "Id")
                    {
                        return HtmlAttributes.Empty.AddClass("id-column");
                    }
                    return HtmlAttributes.Empty;
                }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<th class=\"id-column\">Id</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldApplyBodyRowAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                BodyRowAttributes = ctx =>
                {
                    if (ctx.RowIndex == 0)
                    {
                        return HtmlAttributes.Empty.AddClass("first-row");
                    }
                    return HtmlAttributes.Empty;
                }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<tr class=\"first-row\">", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldApplyBodyCellAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                BodyCellAttributes = ctx =>
                {
                    if (ctx.ColumnName == "Amount" && ctx.Value is decimal amount && amount < 0)
                    {
                        return HtmlAttributes.Empty.AddClass("negative");
                    }
                    return HtmlAttributes.Empty;
                }
            };

            string html = table.ToHtmlTable(options);
            // Our test data has positive amounts, so no negative class should appear
            Assert.DoesNotContain("negative", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseHeaderTextSelector()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                HeaderTextSelector = ctx => "Col: " + ctx.ColumnName
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<th>Col: Id</th>", html);
            Assert.Contains("<th>Col: Name</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseCellTextSelector()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                CellTextSelector = ctx => "Value: " + ctx.Value
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<td>Value: 1</td>", html);
            Assert.Contains("<td>Value: Alice</td>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseCellHtmlSelector()
        {
            var table = new DataTable();
            table.Columns.Add("Link");
            table.Rows.Add("https://example.com");

            var options = new DataTableHtmlRendererOptions
            {
                CellHtmlSelector = ctx =>
                {
                    if (ctx.ColumnName == "Link")
                    {
                        return "<a href=\"" + HtmlEncoder.Encode(Convert.ToString(ctx.Value)) + "\">Link</a>";
                    }
                    return null;
                }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<a href=\"https://example.com\">Link</a>", html);
        }

        [Fact]
        public void ToHtmlTable_CellHtmlSelectorShouldHavePriority()
        {
            var table = new DataTable();
            table.Columns.Add("Value");
            table.Rows.Add("test");

            var options = new DataTableHtmlRendererOptions
            {
                CellHtmlSelector = ctx => "<b>HTML</b>",
                CellTextSelector = ctx => "Text"
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("<b>HTML</b>", html);
            Assert.DoesNotContain("Text", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseFormatValue()
        {
            var table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Rows.Add(new DateTime(2023, 1, 15));

            var culture = CultureInfo.GetCultureInfo("fr-FR");

            var options = new DataTableHtmlRendererOptions
            {
                FormatValue = value =>
                {
                    if (value is DateTime date)
                    {
                        return date.ToString("d", culture);
                    }
                    return Convert.ToString(value);
                }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("15/01/2023", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseFormatCellValue()
        {
            var table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add(new DateTime(2023, 1, 15), "Test");

            var culture = CultureInfo.GetCultureInfo("en-US");

            var options = new DataTableHtmlRendererOptions
            {
                FormatCellValue = ctx =>
                {
                    if (ctx.ColumnName == "Date" && ctx.Value is DateTime date)
                    {
                        return date.ToString("d", culture);
                    }
                    return null;
                }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("1/15/2023", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldUseFormatProvider()
        {
            var table = new DataTable();
            table.Columns.Add("Amount", typeof(decimal));
            table.Rows.Add(1234.56m);

            var culture = CultureInfo.GetCultureInfo("fr-FR");

            var options = new DataTableHtmlRendererOptions
            {
                FormatProvider = culture
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("1234,56", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldExcludeThead()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                IncludeThead = false
            };

            string html = table.ToHtmlTable(options);
            Assert.DoesNotContain("<thead>", html);
            Assert.DoesNotContain("</thead>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldExcludeTbody()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                IncludeTbody = false
            };

            string html = table.ToHtmlTable(options);
            Assert.DoesNotContain("<tbody>", html);
            Assert.DoesNotContain("</tbody>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleDataColumnCaption()
        {
            var table = new DataTable();
            var column = table.Columns.Add("Id");
            column.Caption = "Identifier";

            string html = table.ToHtmlTable();
            Assert.Contains("<th>Identifier</th>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleCustomEmptyTableHtml()
        {
            var table = new DataTable();
            table.Columns.Add("Id");

            var options = new DataTableHtmlRendererOptions
            {
                EmptyTableHtml = "<p>No data available</p>"
            };

            string html = table.ToHtmlTable(options);
            Assert.Equal("<p>No data available</p>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleBooleanValues()
        {
            var table = new DataTable();
            table.Columns.Add("IsActive", typeof(bool));
            table.Rows.Add(true);
            table.Rows.Add(false);

            string html = table.ToHtmlTable();
            Assert.Contains("<td>True</td>", html);
            Assert.Contains("<td>False</td>", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleDateTimeValues()
        {
            var table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Rows.Add(new DateTime(2023, 1, 15, 10, 30, 0));

            string html = table.ToHtmlTable();
            Assert.Contains("1/15/2023", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleDecimalValues()
        {
            var table = new DataTable();
            table.Columns.Add("Amount", typeof(decimal));
            table.Rows.Add(1234.56m);

            string html = table.ToHtmlTable();
            Assert.Contains("1234.56", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleInvariantCultureForTechnicalFormats()
        {
            var table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Rows.Add(new DateTime(2023, 1, 15, 10, 30, 0));

            var options = new DataTableHtmlRendererOptions
            {
                FormatCellValue = ctx =>
                {
                    if (ctx.ColumnName == "Date" && ctx.Value is DateTime date)
                    {
                        return date.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                    }
                    return null;
                }
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("2023-01-15T10:30:00", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleFrenchCultureFormatting()
        {
            var table = new DataTable();
            table.Columns.Add("Amount", typeof(decimal));
            table.Rows.Add(1234.56m);

            var culture = CultureInfo.GetCultureInfo("fr-FR");

            var options = new DataTableHtmlRendererOptions
            {
                FormatProvider = culture
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("1234,56", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleUSCultureFormatting()
        {
            var table = new DataTable();
            table.Columns.Add("Amount", typeof(decimal));
            table.Rows.Add(1234.56m);

            var culture = CultureInfo.GetCultureInfo("en-US");

            var options = new DataTableHtmlRendererOptions
            {
                FormatProvider = culture
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("1234.56", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleDataAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                TableAttributes = ctx => HtmlAttributes.Empty.Data("component", "grid")
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("data-component=\"grid\"", html);
        }

        [Fact]
        public void ToHtmlTable_ShouldHandleAriaAttributes()
        {
            var table = CreateTestTable();

            var options = new DataTableHtmlRendererOptions
            {
                TableAttributes = ctx => HtmlAttributes.Empty.Aria("label", "Data table")
            };

            string html = table.ToHtmlTable(options);
            Assert.Contains("aria-label=\"Data table\"", html);
        }
    }

    public class DataTableHtmlRendererFeatureTests
    {
        private DataTable CreateTestTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Rows.Add(1, "Alice", 100.50m);
            table.Rows.Add(2, (object)null, DBNull.Value);
            return table;
        }

        // --- Point 2 : <caption> ---

        [Fact]
        public void ShouldRenderCaption_WithCaptionText()
        {
            var table = CreateTestTable();
            var options = new DataTableHtmlRendererOptions { CaptionText = "My Table" };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<caption>My Table</caption>", html);
        }

        [Fact]
        public void ShouldRenderCaption_WithCaptionHtml()
        {
            var table = CreateTestTable();
            var options = new DataTableHtmlRendererOptions { CaptionHtml = "<strong>My Table</strong>" };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<caption><strong>My Table</strong></caption>", html);
        }

        [Fact]
        public void CaptionText_ShouldBeHtmlEncoded()
        {
            var table = CreateTestTable();
            var options = new DataTableHtmlRendererOptions { CaptionText = "<b>Title</b>" };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<caption>&lt;b&gt;Title&lt;/b&gt;</caption>", html);
        }

        [Fact]
        public void CaptionHtml_ShouldTakePriorityOverCaptionText()
        {
            var table = CreateTestTable();
            var options = new DataTableHtmlRendererOptions
            {
                CaptionHtml = "<em>HTML wins</em>",
                CaptionText = "Text loses"
            };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<em>HTML wins</em>", html);
            Assert.DoesNotContain("Text loses", html);
        }

        [Fact]
        public void ShouldNotRenderCaption_WhenBothNull()
        {
            var table = CreateTestTable();
            string html = table.ToHtmlTable();
            Assert.DoesNotContain("<caption>", html);
        }

        // --- Point 2 : <tfoot> ---

        [Fact]
        public void ShouldRenderTfoot_WithFooterHtml()
        {
            var table = CreateTestTable();
            var options = new DataTableHtmlRendererOptions
            {
                FooterHtml = "<tr><td>Total</td><td></td><td>100.50</td></tr>"
            };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<tfoot>", html);
            Assert.Contains("</tfoot>", html);
            Assert.Contains("Total", html);
            int tbodyClose = html.IndexOf("</tbody>", StringComparison.Ordinal);
            int tfootOpen = html.IndexOf("<tfoot>", StringComparison.Ordinal);
            Assert.True(tbodyClose < tfootOpen, "<tfoot> should appear after </tbody>");
        }

        [Fact]
        public void ShouldNotRenderTfoot_WhenFooterHtmlNull()
        {
            var table = CreateTestTable();
            string html = table.ToHtmlTable();
            Assert.DoesNotContain("<tfoot>", html);
        }

        // --- Point 3 : ColumnFormatters ---

        [Fact]
        public void ColumnFormatters_ShouldFormatSpecificColumn()
        {
            var table = CreateTestTable();
            var options = new DataTableHtmlRendererOptions
            {
                ColumnFormatters = new Dictionary<string, Func<BodyCellRenderContext, string>>
                {
                    { "Amount", ctx => ctx.Value is decimal d ? d.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-US")) : null }
                }
            };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<td>$100.50</td>", html);
        }

        [Fact]
        public void ColumnFormatters_ShouldTakePriorityOverFormatCellValue()
        {
            var table = new DataTable();
            table.Columns.Add("Val", typeof(int));
            table.Rows.Add(42);

            var options = new DataTableHtmlRendererOptions
            {
                ColumnFormatters = new Dictionary<string, Func<BodyCellRenderContext, string>>
                {
                    { "Val", ctx => "column-specific" }
                },
                FormatCellValue = ctx => "global"
            };
            string html = table.ToHtmlTable(options);
            Assert.Contains("column-specific", html);
            Assert.DoesNotContain("global", html);
        }

        [Fact]
        public void ColumnFormatters_ShouldNotAffectOtherColumns()
        {
            var table = new DataTable();
            table.Columns.Add("A", typeof(string));
            table.Columns.Add("B", typeof(string));
            table.Rows.Add("valA", "valB");

            var options = new DataTableHtmlRendererOptions
            {
                ColumnFormatters = new Dictionary<string, Func<BodyCellRenderContext, string>>
                {
                    { "A", ctx => "formatted" }
                }
            };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<td>formatted</td>", html);
            Assert.Contains("<td>valB</td>", html);
        }

        // --- Point 4 : ordre de colonnes explicite ---

        [Fact]
        public void IncludedColumns_ShouldPreserveExplicitOrder()
        {
            var table = new DataTable();
            table.Columns.Add("A");
            table.Columns.Add("B");
            table.Columns.Add("C");
            table.Rows.Add("a", "b", "c");

            var options = new DataTableHtmlRendererOptions
            {
                IncludedColumns = new[] { "C", "A" }
            };
            string html = table.ToHtmlTable(options);

            int posC = html.IndexOf("<th>C</th>", StringComparison.Ordinal);
            int posA = html.IndexOf("<th>A</th>", StringComparison.Ordinal);
            Assert.True(posC < posA, "C should appear before A as specified in IncludedColumns");
            Assert.DoesNotContain("<th>B</th>", html);
        }

        // --- Point 5 : null vs DBNull ---

        [Fact]
        public void DbNullCellText_ShouldRenderForDbNull()
        {
            var table = new DataTable();
            table.Columns.Add("Amount", typeof(decimal));
            table.Rows.Add(DBNull.Value);

            var options = new DataTableHtmlRendererOptions { DbNullCellText = "—" };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<td>—</td>", html);
        }

        [Fact]
        public void DbNullCellText_ShouldRenderForNullStringColumn()
        {
            // DataTable converts C# null to DBNull.Value internally
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add((object)null);

            var options = new DataTableHtmlRendererOptions { DbNullCellText = "N/A" };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<td>N/A</td>", html);
        }

        [Fact]
        public void DbNullCellText_ShouldNotAffectNonNullCells()
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add("Alice");
            table.Rows.Add(DBNull.Value);

            var options = new DataTableHtmlRendererOptions { DbNullCellText = "—" };
            string html = table.ToHtmlTable(options);
            Assert.Contains("<td>Alice</td>", html);
            Assert.Contains("<td>—</td>", html);
        }

        [Fact]
        public void DbNullCellText_ShouldDefaultToEmpty_WhenNotSet()
        {
            var table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add(DBNull.Value);

            string html = table.ToHtmlTable();
            Assert.Contains("<td></td>", html);
        }
    }

    public class DataTableHtmlRendererStreamingTests
    {
        private DataTable CreateTestTable()
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Rows.Add(1, "Alice");
            table.Rows.Add(2, "Bob");
            return table;
        }

        [Fact]
        public void Render_WithTextWriter_ShouldProduceSameOutputAsStringOverload()
        {
            var table = CreateTestTable();
            string expected = DataTableHtmlRenderer.Render(table, (DataTableHtmlRendererOptions)null);

            using (var sw = new StringWriter())
            {
                DataTableHtmlRenderer.Render(table, sw, null);
                Assert.Equal(expected, sw.ToString());
            }
        }

        [Fact]
        public void Render_WithTextWriter_ShouldThrowForNullTable()
        {
            using (var sw = new StringWriter())
            {
                Assert.Throws<ArgumentNullException>(() => DataTableHtmlRenderer.Render(null, sw, null));
            }
        }

        [Fact]
        public void Render_WithTextWriter_ShouldThrowForNullWriter()
        {
            var table = CreateTestTable();
            Assert.Throws<ArgumentNullException>(() => DataTableHtmlRenderer.Render(table, (System.IO.TextWriter)null, null));
        }

        [Fact]
        public void Render_WithTextWriter_ShouldWriteNothingForEmptyTableWhenNotRenderingEmpty()
        {
            var table = new DataTable();
            table.Columns.Add("Id");

            using (var sw = new StringWriter())
            {
                DataTableHtmlRenderer.Render(table, sw, new DataTableHtmlRendererOptions { RenderEmptyTable = false });
                Assert.Equal(string.Empty, sw.ToString());
            }
        }

        [Fact]
        public void Render_WithTextWriter_ShouldWriteEmptyTableHtmlWhenConfigured()
        {
            var table = new DataTable();
            table.Columns.Add("Id");

            var options = new DataTableHtmlRendererOptions
            {
                EmptyTableHtml = "<p>No data</p>"
            };

            using (var sw = new StringWriter())
            {
                DataTableHtmlRenderer.Render(table, sw, options);
                Assert.Equal("<p>No data</p>", sw.ToString());
            }
        }

        [Fact]
        public void WriteHtmlTo_Extension_ShouldProduceSameOutputAsToHtmlTable()
        {
            var table = CreateTestTable();
            string expected = table.ToHtmlTable();

            using (var sw = new StringWriter())
            {
                table.WriteHtmlTo(sw);
                Assert.Equal(expected, sw.ToString());
            }
        }
    }
}
