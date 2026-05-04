# Contributing to DataTableHtmlRenderer

Thank you for your interest in contributing to **DataTableHtmlRenderer**! We welcome contributions from the community.

## How to Contribute

### Reporting Issues

Before reporting an issue, please:

1. Check the [README](README.md) to ensure your issue is not already addressed
2. Check existing issues to avoid duplicates
3. Provide a clear, reproducible description of the issue
4. Include:
   - Steps to reproduce
   - Expected behavior
   - Actual behavior
   - Code samples (if applicable)
   - Environment details (.NET version, OS, etc.)

### Suggesting Features

We welcome feature suggestions! However, please note that **DataTableHtmlRenderer** is intentionally minimalistic. Features that add complexity (sorting, filtering, pagination, JavaScript generation, etc.) will likely be declined as they go against the project's design philosophy.

### Submitting Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Ensure all tests pass
5. Add tests for new functionality
6. Update documentation as needed
7. Commit your changes with clear, descriptive messages
8. Push to your fork (`git push origin feature/amazing-feature`)
9. Open a Pull Request

## Development Guidelines

### Code Style

- Follow the existing code style and patterns
- Use 4 spaces for indentation in C# files
- Use 2 spaces for indentation in XML, JSON, and Markdown files
- Use UTF-8 encoding for all files
- Keep line endings as LF (not CRLF)
- Use `var` where the type is obvious from the context
- Use explicit types for public API return types and parameters

### C# Version Compatibility

All code must be compatible with **C# 7.0**. Do NOT use:

- Switch expressions
- Records
- Nullable reference types
- File-scoped namespaces
- Global usings
- Top-level statements
- Target-typed `new`
- `using` declarations (C# 8+)
- Any other C# 8+ features

### Testing

- All new functionality must have corresponding unit tests
- Tests should cover edge cases and error conditions
- Use xUnit for testing
- Follow the existing test naming conventions
- Keep tests focused and fast

### Documentation

- All public APIs must have XML documentation comments
- Documentation should be clear and concise
- Examples in documentation must be C# 7 compatible
- Public-facing documentation (README, etc.) must be in English

### Security

- Never commit secrets, API keys, or credentials
- All user input must be properly encoded
- Security considerations should be documented
- If you find a security vulnerability, please report it privately

## Build and Test

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Build in Release configuration
dotnet build --configuration Release

# Run tests in Release configuration
dotnet test --configuration Release

# Package all projects
dotnet pack --configuration Release
```

## Project Structure

```
DataTableHtmlRenderer/
├── .github/
│   └── workflows/
│       ├── ci.yml          # CI workflow
│       └── release.yml     # Release workflow
├── .editorconfig           # Editor configuration
├── .gitignore              # Git ignore rules
├── DataTableHtmlRenderer.sln  # Solution file
├── Directory.Build.props   # Common build properties
├── LICENSE                 # MIT License
├── README.md               # Project documentation
├── CONTRIBUTING.md         # This file
├── docs/
│   └── IMPLEMENTATION_PLAN.md  # Implementation plan
├── src/
│   ├── DataTableHtmlRenderer/          # Core package
│   │   ├── DataTableHtmlRenderer.cs
│   │   ├── DataTableHtmlRenderer.csproj
│   │   ├── DataTableHtmlRendererExtensions.cs
│   │   ├── DataTableHtmlRendererOptions.cs
│   │   ├── HtmlAttributes.cs
│   │   ├── HtmlEncoder.cs
│   │   └── RenderContexts.cs
│   ├── DataTableHtmlRenderer.AspNetFx/     # ASP.NET Framework adapter
│   │   ├── DataTableHtmlRenderer.AspNetFx.csproj
│   │   └── DataTableHtmlRendererExtensions.cs
│   └── DataTableHtmlRenderer.AspNet/  # ASP.NET adapter
│       ├── DataTableHtmlRenderer.AspNet.csproj
│       └── DataTableHtmlRendererExtensions.cs
└── tests/
    └── DataTableHtmlRenderer.Tests/    # Unit tests
        ├── DataTableHtmlRenderer.Tests.csproj
        ├── DataTableHtmlRendererTests.cs
        ├── HtmlAttributesTests.cs
        └── HtmlEncoderTests.cs
```

## Code of Conduct

By participating in this project, you agree to abide by the following code of conduct:

- Be respectful and inclusive
- Focus on constructive discussion
- Accept that design decisions may differ from your preferences
- Follow the project's design philosophy and limitations

## License

By contributing to this project, you agree that your contributions will be licensed under the [MIT License](LICENSE).
