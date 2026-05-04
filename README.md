# DataTableHtmlRenderer

[![NuGet Version](https://img.shields.io/nuget/v/DataTableHtmlRenderer.svg)](https://www.nuget.org/packages/DataTableHtmlRenderer)
[![NuGet Downloads](https://img.shields.io/nuget/dt/DataTableHtmlRenderer.svg)](https://www.nuget.org/packages/DataTableHtmlRenderer)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

**DataTableHtmlRenderer** is a server-side HTML table renderer for `System.Data.DataTable`, designed for legacy ASP.NET migration scenarios and broad .NET compatibility.

## Problem Addressed

During progressive migration of ASP.NET WebForms applications to ASP.NET MVC 5, ASP.NET Core MVC, or other modern .NET environments, development teams often need to replace `GridView` controls with server-side HTML rendering. Many existing pages use `GridView` with `AutoGenerateColumns=true`, and the data is already prepared as `System.Data.DataTable` objects.

**DataTableHtmlRenderer** provides a simple, predictable, and highly compatible way to generate HTML tables from `DataTable` objects, making it an ideal tool for migration scenarios.

## Why This Component Exists

While there are existing packages like `HtmlTableHelper` that can convert objects or `DataTable` to HTML strings, **DataTableHtmlRenderer** addresses several limitations:

1. **Specialization**: Focused specifically on `System.Data.DataTable` and legacy ASP.NET migration scenarios
2. **Fine-grained Customization**: Provides delegate-based customization for every aspect of table rendering
3. **Rich Context API**: Offers strongly-typed contexts with access to `DataTable`, `DataColumn`, `DataRow`, indices, column names, data types, raw values, and `DBNull` status
4. **Explicit Security Model**: HTML encoding by default, attribute encoding, attribute name validation, with explicit API for safe HTML injection
5. **Framework Independence**: Core package has no ASP.NET dependencies, with optional adapters for MVC 5 and ASP.NET Core
6. **Clear Positioning**: Explicitly designed as a migration aid, not a full `GridView` replacement

## Features

- Automatic column generation from `DataTable.Columns`
- Respects `DataTable` column order
- Handles `DBNull.Value` and `null` values
- HTML encoding of headers and values by default
- Attribute encoding and validation
- Support for `DataColumn.Caption`
- Column include/exclude filtering
- CSS class merging
- Support for `data-*` and `aria-*` attributes
- Culture-aware formatting
- Delegate-based customization for all HTML elements
- Safe HTML injection via `CellHtmlSelector`

## What This Component Does NOT Do

**DataTableHtmlRenderer** is intentionally minimalistic. It does NOT provide:

- Sorting
- Filtering
- Pagination
- Row selection
- Inline editing
- ViewState management
- Postbacks
- Server events (WebForms-style)
- JavaScript generation
- DataTables.net integration
- Internal Razor templating
- SQL query generation
- Database binding
- Editable columns
- Command columns (Edit, Delete, Select)
- Form management

> **Important**: This component is a server-side HTML rendering tool, not a complete grid application. It is designed to help with migration scenarios where you need to replace `GridView` rendering with simple HTML table output.

## Installation

### Core Package

```bash
dotnet add package DataTableHtmlRenderer
```

### ASP.NET Framework Adapter

```bash
dotnet add package DataTableHtmlRenderer.AspNetFx
```

### ASP.NET Adapter

```bash
dotnet add package DataTableHtmlRenderer.AspNet
```

## Usage

### Minimal Usage

```csharp
using System.Data;
using DataTableHtmlRenderer;

// Create your DataTable (already populated with data)
DataTable dataTable = GetDataTable();

// Render as HTML
string html = dataTable.ToHtmlTable();
```

### Usage in ASP.NET MVC 5 (without adapter)

```csharp
@using DataTableHtmlRenderer

@{
    DataTable table = Model.Table;
}

@Html.Raw(table.ToHtmlTable())
```

### Usage in ASP.NET Framework (with adapter)

```csharp
@using DataTableHtmlRenderer.AspNetFx

@{
    DataTable table = Model.Table;
}

@table.ToMvcHtmlString()
```

### Usage in ASP.NET (with adapter)

```csharp
@using DataTableHtmlRenderer.AspNet

@{
    DataTable table = Model.Table;
}

@table.ToHtmlContent()
```

## Customization Examples

### Bootstrap Classes

```csharp
var options = new DataTableHtmlRendererOptions
{
    TableAttributes = ctx => HtmlAttributes.Empty
        .AddClass("table")
        .AddClass("table-sm")
        .AddClass("table-striped")
};

string html = dataTable.ToHtmlTable(options);
```

### Conditional Row Class

```csharp
var options = new DataTableHtmlRendererOptions
{
    BodyRowAttributes = ctx =>
    {
        var status = Convert.ToString(ctx.Row["Status"]);
        return status == "Error"
            ? HtmlAttributes.Empty.AddClass("table-danger")
            : HtmlAttributes.Empty;
    }
};
```

### Conditional Cell Class

```csharp
var options = new DataTableHtmlRendererOptions
{
    BodyCellAttributes = ctx =>
    {
        if (ctx.ColumnName == "Amount" && ctx.Value is decimal amount && amount < 0)
        {
            return HtmlAttributes.Empty.AddClass("text-danger");
        }
        return HtmlAttributes.Empty;
    }
};
```

### Custom Header Text

```csharp
var options = new DataTableHtmlRendererOptions
{
    HeaderTextSelector = ctx =>
    {
        // Use caption if available, otherwise column name
        if (!string.IsNullOrEmpty(ctx.Caption))
        {
            return ctx.Caption;
        }
        return ctx.ColumnName;
    }
};
```

### Custom Cell Formatting

```csharp
var options = new DataTableHtmlRendererOptions
{
    CellTextSelector = ctx =>
    {
        if (ctx.ColumnName == "Price" && ctx.Value is decimal price)
        {
            return "$" + price.ToString("N2");
        }
        return Convert.ToString(ctx.Value);
    }
};
```

## Culture and Formatting

**DataTableHtmlRenderer** follows .NET best practices for globalization and formatting:

- Uses `IFormatProvider` where appropriate
- Allows explicit `CultureInfo` specification
- Does NOT impose `CultureInfo.InvariantCulture` for user-facing values
- Reserves `InvariantCulture` for technical scenarios where stable, non-localized representation is explicitly desired

### Formatting a DateTime Column - Multiple Approaches

#### 1. Using Current Application Culture

```csharp
var options = new DataTableHtmlRendererOptions
{
    // Uses the current thread's culture (CultureInfo.CurrentCulture)
    // No explicit format provider needed
};
```

#### 2. Providing Explicit Culture

```csharp
var culture = CultureInfo.GetCultureInfo("fr-FR");

var options = new DataTableHtmlRendererOptions
{
    FormatProvider = culture
};
```

#### 3. Applying Short Date Format

```csharp
var culture = CultureInfo.GetCultureInfo("en-US");

var options = new DataTableHtmlRendererOptions
{
    FormatCellValue = ctx =>
    {
        if (ctx.ColumnName == "CreatedAt" && ctx.Value is DateTime)
        {
            return ((DateTime)ctx.Value).ToString("d", culture);
        }
        return null;
    }
};
```

#### 4. Applying Date/Time Format

```csharp
var culture = CultureInfo.GetCultureInfo("fr-FR");

var options = new DataTableHtmlRendererOptions
{
    FormatCellValue = ctx =>
    {
        if (ctx.ColumnName == "CreatedAt" && ctx.Value is DateTime)
        {
            return ((DateTime)ctx.Value).ToString("g", culture);
        }
        return null;
    }
};
```

#### 5. Applying ISO Format for Technical Use

```csharp
var options = new DataTableHtmlRendererOptions
{
    FormatCellValue = ctx =>
    {
        if (ctx.ColumnName == "CreatedAt" && ctx.Value is DateTime)
        {
            // ISO 8601 format using InvariantCulture for technical stability
            return ((DateTime)ctx.Value).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }
        return null;
    }
};
```

#### 6. Formatting a Specific Column Differently

```csharp
var culture = CultureInfo.GetCultureInfo("en-US");

var options = new DataTableHtmlRendererOptions
{
    FormatCellValue = ctx =>
    {
        if (ctx.ColumnName == "CreatedAt" && ctx.Value is DateTime)
        {
            return ((DateTime)ctx.Value).ToString("MMMM dd, yyyy", culture);
        }
        if (ctx.ColumnName == "Amount" && ctx.Value is decimal)
        {
            return ((decimal)ctx.Value).ToString("C", culture);
        }
        return null;
    }
};
```

> **Note**: Using `CultureInfo.InvariantCulture` is appropriate for technical identifiers, API responses, or any scenario where you need a stable, non-localized string representation. For user-facing display, prefer the user's culture.

## Safe HTML Injection

For scenarios where you need to inject HTML (e.g., links, icons), use `CellHtmlSelector`:

```csharp
var options = new DataTableHtmlRendererOptions
{
    CellHtmlSelector = ctx =>
    {
        if (ctx.ColumnName == "DetailsUrl")
        {
            var url = Convert.ToString(ctx.Value);
            return "<a href=\"" + HtmlEncoder.Encode(url) + "\">View</a>";
        }
        return null;
    }
};
```

> **Security Warning**: `CellHtmlSelector` should only be used with HTML that you control and have secured. The component assumes the returned HTML is safe and will NOT encode it. Always encode any user-provided data within your HTML.

## Column Filtering

### Include Specific Columns

```csharp
var options = new DataTableHtmlRendererOptions
{
    IncludedColumns = new List<string> { "Id", "Name", "Email" }
};
```

### Exclude Specific Columns

```csharp
var options = new DataTableHtmlRendererOptions
{
    ExcludedColumns = new List<string> { "Password", "SecretKey" }
};
```

### Include and Exclude (Include takes precedence, then Exclude is applied)

```csharp
var options = new DataTableHtmlRendererOptions
{
    IncludedColumns = new List<string> { "Id", "Name", "Email", "Password" },
    ExcludedColumns = new List<string> { "Password" }
};
// Result: Only Id, Name, Email columns are rendered
```

## Security Model

**DataTableHtmlRenderer** implements a security-first approach:

1. **HTML Encoding by Default**: All values and headers are HTML encoded by default
2. **Attribute Encoding**: All attribute values are encoded
3. **Attribute Name Validation**: Attribute names are validated to prevent dangerous attributes
4. **Forbidden Attributes**: Event handlers (`onclick`, `onload`, etc.) and `javascript:` URIs are blocked
5. **Explicit Safe HTML**: `CellHtmlSelector` is the only way to inject unencoded HTML, making it explicit when safe HTML is being used

### XSS Protection

The following inputs are automatically encoded and rendered safe:

```html
<script>alert(1)</script>
<img src=x onerror=alert(1)>
" onclick="alert(1)
& < > " '
```

These will be rendered as text, never as executable code.

## Packages Available

| Package | Description | Target Framework |
|--------|-------------|------------------|
| `DataTableHtmlRenderer` | Core rendering library | .NET Standard 2.0 |
| `DataTableHtmlRenderer.AspNetFx` | ASP.NET Framework adapter | .NET Framework 4.6.2 |
| `DataTableHtmlRenderer.AspNet` | ASP.NET adapter | .NET Standard 2.0 |

## Differences Between string, IHtmlString, and IHtmlContent

- **`string`**: Plain HTML string. In Razor views, must be used with `@Html.Raw()` to prevent double-encoding
- **`IHtmlString`**: ASP.NET MVC interface that tells Razor the string is safe HTML and should not be encoded
- **`IHtmlContent`**: ASP.NET Core interface that tells Razor the content is safe HTML and should not be encoded

The adapters return the appropriate type for each framework, allowing natural usage in Razor views.

## Compatibility

- **Core Package**: .NET Standard 2.0 (compatible with .NET Framework 4.6.2+, .NET Core 2.0+, .NET 5+)
- **ASP.NET Framework Adapter**: .NET Framework 4.6.2
- **ASP.NET Adapter**: .NET Standard 2.0
- **C# Version**: C# 7.0 (all code and examples are C# 7 compatible)

## Implementation Plan

The detailed implementation plan is available in [docs/IMPLEMENTATION_PLAN.md](docs/IMPLEMENTATION_PLAN.md).

## Contributing

Contributions are welcome! Please read our [contribution guidelines](CONTRIBUTING.md) before submitting pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
