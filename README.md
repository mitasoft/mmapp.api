# mmapp.api
Mihai's template for Web APIs


# ef migrations

``` ps
dotnet ef dbcontext scaffold "Server=RO-L-1BLTGY3;Database=mm-modus-database;user id=sa;password=sa;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Data --context "ModusContext" --context-namespace "Modus.Api.Data" --no-build --force
```

```
dotnet tool install --global dotnet-ef
```

```
dotnet ef migrations add Initial -c MyAppIdentityContext -o .\Identity\Migrations
```

```
dotnet ef database update -c MyAppIdentityContext --connection "Server=RO-L-1BLTGY3;Database=api-template;user id=sa;password=sa;TrustServerCertificate=True;" 
```