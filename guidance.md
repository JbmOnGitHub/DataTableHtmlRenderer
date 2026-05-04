# Developer Guidance

This file provides developer guidance for working with code in this repository.

## Commands

```bash
dotnet restore                          # Restore dependencies
dotnet build                            # Build (Debug)
dotnet build --configuration Release    # Build (Release)
dotnet test                             # Run all tests
dotnet test --configuration Release     # Run tests in Release mode
dotnet pack --configuration Release     # Package all projects as NuGet packages
```

To run a single test by name:
```bash
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

## Architecture

This library renders `System.Data.DataTable` objects as HTML `<table>` strings. It ships as three NuGet packages: a core library and two thin adapters.

### Package layout

- **`src/DataTableHtmlRenderer/`** — Core library (targets `netstandard2.0`). All rendering logic lives here.
- **`src/DataTableHtmlRenderer.AspNetFx/`** — ASP.NET Framework adapter (`net462`). Adds `ToMvcHtmlString()` returning `IHtmlString`.
- **`src/DataTableHtmlRenderer.AspNet/`** — ASP.NET Core adapter (`netstandard2.0`). Adds `ToHtmlContent()` returning `IHtmlContent`.
- **`tests/DataTableHtmlRenderer.Tests/`** — xUnit test project (`net8.0`).

### Core files

| File | Purpose |
|---|---|
| `DataTableHtmlRenderer.cs` | Main engine. Static `Render(DataTable, options)` entry point plus instance methods that walk `DataTable` rows/columns. |
| `DataTableHtmlRendererOptions.cs` | ~20 properties: column filtering (`IncludedColumns`, `ExcludedColumns`), structural flags (`IncludeThead`, `IncludeTbody`, `RenderEmptyTable`), delegate callbacks for attributes and text, and formatting (`FormatValue`, `FormatCellValue`, `FormatProvider`). |
| `HtmlAttributes.cs` | Fluent builder for HTML attributes. Blocks forbidden attributes (event handlers, `javascript:`/`vbscript:` schemes). Deduplicates CSS classes. Supports `.Data()`, `.Aria()`, `.Merge()`, `.Clone()`. |
| `HtmlEncoder.cs` | Static utility. Encodes all values by default; handles `null`/`DBNull.Value`. Non-ASCII chars encoded as numeric entities. |
| `RenderContexts.cs` | Strongly-typed context objects passed to option callbacks (`TableRenderContext`, `HeaderCellRenderContext`, `BodyCellRenderContext`, etc.). |

### Security model

All cell values and header text are HTML-encoded by default via `HtmlEncoder`. `CellHtmlSelector` is the sole escape hatch for raw HTML and places responsibility on the caller. `HtmlAttributes.Set()` encodes attribute values and rejects event-handler attributes.

## Code standards (enforced via `Directory.Build.props` and `.editorconfig`)

- **C# 7.0** — no switch expressions, records, nullable reference types, file-scoped namespaces, global usings, or top-level statements.
- **Indentation:** 4 spaces in `.cs`; 2 spaces in `.xml`/`.csproj`/`.props`/`.json`/`.md`/`.yaml`.
- **Line endings:** LF everywhere.
- **Warnings as errors** in all configurations.
- All public APIs require XML documentation comments.
- All new features require xUnit tests.
