# WebHike

An ASP.NET Core MVC e-commerce project with product categories, accounts, a shopping cart and an admin area for managing the catalogue.

## Tech stack

C#, .NET 9, ASP.NET Core MVC, Entity Framework Core, PostgreSQL, Razor views.

## Run locally

1. Install the .NET 9 SDK and create an empty PostgreSQL database.
2. From the `WebHike` directory, configure the connection string using an environment variable or .NET user secrets:
   `dotnet user-secrets set "ConnectionStrings:MyWebHikeConnection" "Host=localhost;Database=webhike;Username=postgres;Password=YOUR_LOCAL_PASSWORD"`
3. Apply the existing migrations: `dotnet ef database update`.
4. Start the application: `dotnet run`.

Keep passwords outside version control. The connection string is intentionally absent from committed appsettings files. Never reuse a previously exposed password.

## Features

- Registration and sign-in with session-based state
- Product browsing and details
- Category and item administration
- Session-backed basket with quantity controls
- PostgreSQL persistence through EF Core

## Administration

Set `WebHike:AdminEmail` in user secrets to the email of your registered admin account. Other users cannot access the category and item administration routes.

```bash
dotnet user-secrets set "WebHike:AdminEmail" "you@example.com"
```

Existing SHA-256 account passwords are migrated to ASP.NET Core PasswordHasher hashes on successful sign-in. Before any real deployment, add login rate limiting, reset flows, proper role management and upload validation.

## Current limitations

This is a development project, not a production shop. Payment processing, checkout fulfilment, email confirmation, stock control and a production-grade authentication system still need implementation and testing. No real payment data should be entered.
