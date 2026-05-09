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

Backend:

```bash
cd Back-end
dotnet restore "FragE-Commerce.sln"
dotnet run --project "FragranceECommerceApi/FragE-Commerce.csproj"
```

Configure the backend connection string in `Back-end/FragranceECommerceApi/appsettings.json` or through user secrets/environment variables before using a real database.

## Project Structure

- `Front-End-Vue/Front-end-Vue/`: Quasar storefront
- `Back-end/FragranceECommerceApi/`: ASP.NET Core API
- `Back-end/FragE-Commerce.sln`: backend solution
