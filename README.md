# FragranceECommerce

FragranceECommerce is a full-stack ecommerce application for browsing fragrance brands, managing a cart, viewing branch locations, and handling customer authentication.

## Features

- Quasar/Vue storefront for brands, products, cart, and order history
- ASP.NET Core API with Entity Framework Core
- Customer registration and login
- Branch and product data workflows
- Swagger/OpenAPI support for backend inspection

## Tech Stack

- Vue 3 and Quasar
- ASP.NET Core 8
- Entity Framework Core with SQL Server provider
- JWT bearer authentication package

## Getting Started

Frontend:

```bash
cd Front-End-Vue/Front-end-Vue
npm install
npm run dev
```

Run frontend tests and production build:

```bash
npm test
npm run build
```

Backend:

```bash
cd Back-end
dotnet restore "FragE-Commerce.sln"
dotnet run --project "FragranceECommerceApi/FragE-Commerce.csproj"
```

Configure the backend connection string in `Back-end/FragranceECommerceApi/appsettings.json` or through user secrets/environment variables before using a real database.

## Verification

- `npm test` runs Node's built-in test runner against storefront utility behavior: currency/date formatting, API path construction, JSON request bodies, bearer-token headers, and HTTP error handling.
- `npm run build` verifies the Quasar SPA production bundle.
- The ASP.NET Core API project builds under the installed .NET 8 SDK.

## Architecture

```text
Quasar/Vue storefront
  -> src/utils/apiutil.js
      -> ASP.NET Core controllers
          -> DAO/helper layer
              -> EF Core AppDbContext
                  -> SQL Server
```

## Demo Data

- Product, brand, branch, customer, cart, and order entities are represented in `Back-end/FragranceECommerceApi/DAL/DomainClasses`.
- EF Core migrations document the schema evolution for customer, order, branch, and Azure deployment iterations.

## Known Limitations

- Backend automated tests are still future work; current automated coverage is frontend utility/smoke coverage plus build verification.
- `appsettings.json` uses local placeholder configuration. Real connection strings and JWT secrets should be supplied through user secrets, environment variables, or a secret manager.
- Publish profiles are ignored by git and should not be treated as resume evidence for a currently live deployment unless a live URL is verified.

## Project Structure

- `Front-End-Vue/Front-end-Vue/`: Quasar storefront
- `Back-end/FragranceECommerceApi/`: ASP.NET Core API
- `Back-end/FragE-Commerce.sln`: backend solution
