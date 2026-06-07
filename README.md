## Fastinventory Sale

Base structure for the Sale API.

### Layers

- `Src/Application` for DTOs, interfaces, and services.
- `Src/Domain` for entities and shared utilities.
- `Src/Infraestructure` for persistence contracts, models, and repositories.
- `Src/Presentation` for controllers and middleware.

### Tooling

- Swagger is available in development through `Swashbuckle.AspNetCore`.
- `Husky.Net` is configured for commit and push hooks.
- `.editorconfig` defines code style and formatting rules.
- `Microsoft.CodeAnalysis.NetAnalyzers` provides build-time analysis.

### Manual setup

From the `fastinventorySale` folder, run these steps once:

```bash
dotnet tool restore
dotnet husky install
```

Useful checks:

```bash
dotnet format fastinventorySale.csproj --verify-no-changes
dotnet build fastinventorySale.csproj
```

The hooks live in `.husky/`:

- `commit-msg` validates Conventional Commits.
- `pre-commit` runs the format check.
- `pre-push` runs the build.

### Commit rules

Use Conventional Commits such as:

```text
feat(api): add sale endpoint
fix(db): handle null connection string
```