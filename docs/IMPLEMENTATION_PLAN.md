# Implementation Plan

This document tracks the implementation progress of the DataTableHtmlRenderer project.

## 1. Repository Setup

- [x] Create solution structure (`DataTableHtmlRenderer.sln`)
- [x] Add `.editorconfig` with consistent coding style
- [x] Add `.gitignore` for build artifacts and IDE files
- [x] Add MIT `LICENSE` file
- [x] Add `Directory.Build.props` with common build settings
- [x] Create project directories (`src/`, `tests/`, `docs/`)

## 2. Core Package (`DataTableHtmlRenderer`)

- [x] Create `DataTableHtmlRenderer.csproj` targeting `netstandard2.0`
- [x] Enforce C# 7.0 language version
- [x] Add NuGet package metadata
- [x] Implement `HtmlEncoder` static class for safe HTML encoding
- [x] Implement `HtmlAttributes` class with fluent API
- [x] Implement render contexts (`TableRenderContext`, `HeaderRowRenderContext`, `HeaderCellRenderContext`, `BodyRowRenderContext`, `BodyCellRenderContext`)
- [x] Implement `DataTableHtmlRendererOptions` with all required properties
- [x] Implement column include/exclude behavior
- [x] Implement culture-aware formatting support
- [x] Implement `DataTableHtmlRenderer` class with `Render` method
- [x] Implement `DataTableHtmlRendererExtensions` with `ToHtmlTable` extension methods
- [x] Add XML documentation comments for all public APIs

## 3. ASP.NET Framework Adapter (`DataTableHtmlRenderer.AspNetFx`)

- [x] Create `DataTableHtmlRenderer.AspNetFx.csproj` targeting `net462`
- [x] Add project reference to core package
- [x] Add reference to `System.Web`
- [x] Add NuGet package metadata
- [x] Implement `DataTableHtmlRendererExtensions` with `ToMvcHtmlString` returning `IHtmlString`
- [x] Add XML documentation comments

## 4. ASP.NET Adapter (`DataTableHtmlRenderer.AspNet`)

- [x] Create `DataTableHtmlRenderer.AspNet.csproj` targeting `netstandard2.0`
- [x] Add project reference to core package
- [x] Add NuGet package reference to `Microsoft.AspNetCore.Html`
- [x] Add NuGet package metadata
- [x] Implement `DataTableHtmlRendererExtensions` with `ToHtmlContent` returning `IHtmlContent`
- [x] Add XML documentation comments

## 5. Tests (`DataTableHtmlRenderer.Tests`)

- [x] Create `DataTableHtmlRenderer.Tests.csproj` targeting `netcoreapp3.1`
- [x] Add project reference to core package
- [x] Add xUnit test framework references
- [x] Create `HtmlAttributesTests` class
  - [x] Test empty instance
  - [x] Test AddClass/Set/Id/Style/Title/Lang
  - [x] Test Data and Aria methods
  - [x] Test Merge and Clone
  - [x] Test attribute name validation
  - [x] Test forbidden attribute names
  - [x] Test HTML encoding in attributes
- [x] Create `HtmlEncoderTests` class
  - [x] Test null and empty handling
  - [x] Test encoding of special characters (&, <, >, ", ')
  - [x] Test XSS attack vectors
- [x] Create `DataTableHtmlRendererTests` class
  - [x] Test null table throws ArgumentNullException
  - [x] Test simple table rendering (2 columns / 2 lines)
  - [x] Test empty table
  - [x] Test DBNull.Value handling
  - [x] Test null value handling
  - [x] Test HTML encoding in values
  - [x] Test HTML encoding in headers
  - [x] Test DataColumn.Caption
  - [x] Test UseCaptionForHeaders
  - [x] Test IncludedColumns
  - [x] Test ExcludedColumns
  - [x] Test column order preservation
  - [x] Test table attributes
  - [x] Test header row attributes
  - [x] Test header cell attributes
  - [x] Test body row attributes
  - [x] Test body cell attributes
  - [x] Test CSS class merging
  - [x] Test data-* attributes
  - [x] Test aria-* attributes
  - [x] Test invalid attribute names
  - [x] Test CellTextSelector
  - [x] Test CellHtmlSelector
  - [x] Test CellHtmlSelector priority over CellTextSelector
  - [x] Test FormatValue with DateTime, decimal, bool
  - [x] Test FormatCellValue with per-column formatting
  - [x] Test culture-specific formatting with en-US
  - [x] Test culture-specific formatting with fr-FR
  - [x] Test explicit InvariantCulture for technical formats

## 6. Documentation

- [x] Create comprehensive `README.md` in English
  - [x] Document the problem addressed
  - [x] Document the migration context
  - [x] Explain why the component exists despite HtmlTableHelper
  - [x] Document the limitations
  - [x] Provide usage examples
  - [x] Document compatibility
  - [x] Document security model
  - [x] Document available packages
  - [x] Explain differences between string, IHtmlString, and IHtmlContent
  - [x] Document culture and formatting with multiple DateTime examples
  - [x] Document contribution guidelines
  - [x] Document license
- [x] Add link to implementation plan in README

## 7. Packaging and Release

- [x] Add NuGet metadata to all `.csproj` files
- [ ] Create GitHub Actions CI workflow
  - [ ] Run on push and pull_request
  - [ ] Restore packages
  - [ ] Build solution
  - [ ] Run tests
- [ ] Create GitHub Actions release workflow
  - [ ] Run only on tag `v*`
  - [ ] Restore packages
  - [ ] Build in Release configuration
  - [ ] Run tests
  - [ ] Package all projects
  - [ ] Publish `.nupkg` and `.snupkg` to nuget.org
  - [ ] Use GitHub secret `NUGET_API_KEY`
  - [ ] Never expose NuGet key in code or logs

## 8. Additional Documentation

- [ ] Create `CONTRIBUTING.md`
- [ ] Create `docs/ROADMAP.md` (optional)
- [ ] Create `docs/RELEASE_CHECKLIST.md` (optional)
- [ ] Create `docs/SECURITY_CHECKLIST.md` (optional)
- [ ] Create `docs/COMPATIBILITY_CHECKLIST.md` (optional)

## Acceptance Criteria

Before considering the component complete, verify:

- [x] Core package targets `netstandard2.0`
- [x] Core package does not reference ASP.NET
- [x] Values are encoded by default
- [x] Attributes are encoded
- [x] Attribute names are validated
- [x] XSS tests pass
- [x] Columns are auto-generated
- [x] DataTable column order is respected
- [x] Delegates allow customization of table, rows, columns, cells, and value conversion
- [x] All C# code and public examples are C# 7 compatible
- [x] Culture handling is explicit, tested, and documented
- [x] README contains English examples of DateTime formatting with multiple cultures and formats
- [ ] Repository contains implementation plan Markdown with checkable items
- [ ] Implementation plan reflects actual file state
- [x] ASP.NET Framework/ASP.NET adapters are separated
- [x] README clearly explains limitations
- [x] README clearly explains this is not a GridView replacement
- [x] README clearly explains this is not a JavaScript grid
- [ ] Package can be published from free GitHub account
- [x] No secrets are committed

## Known Risks and Assumptions

1. **C# 7 Compatibility**: All code must be compatible with C# 7.0. This limits the use of newer language features.
2. **No ASP.NET Dependencies in Core**: The core package must not reference any ASP.NET assemblies to maintain broad compatibility.
3. **Security First**: HTML encoding is always on by default. Users must explicitly opt-in to unencoded HTML via `CellHtmlSelector`.
4. **No JavaScript**: The component intentionally does not generate any JavaScript or integrate with client-side libraries.
5. **Minimal Features**: The component intentionally lacks many features found in full grid controls (sorting, filtering, pagination, etc.).

## Build and Test Commands

```bash
# Restore packages
dotnet restore

# Build in Debug
dotnet build

# Build in Release
dotnet build --configuration Release

# Run tests
dotnet test

# Run tests in Release
dotnet test --configuration Release

# Package all projects
dotnet pack --configuration Release
```

## Manual Publishing

```bash
# Publish to NuGet (manual)
dotnet nuget push "**/*.nupkg" \
  --api-key YOUR_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

## Tag-Based Publishing

```bash
# Create and push a tag
git tag v0.1.0
git push origin v0.1.0
```

The GitHub Actions workflow will automatically publish the packages when a tag matching `v*` is pushed.

## Notes

- Update this file as tasks are completed
- Keep the checklist in sync with actual implementation
- Do not commit secrets or API keys
- All public-facing documentation must be in English
