# ProStore E-commerce Platform

## Overview

ProStore is a full-stack **ASP.NET Core MVC** e-commerce web application designed for managing products, categories, brands, and orders. The application provides full **CRUD (Create, Read, Update, Delete)** functionality for all major entities, along with a secure role-based user authentication system. This project was developed as a key component of my intensive training program at the **Information Technology Institute (ITI)**.

## Project Structure

This project follows a standard ASP.NET Core MVC structure, organized into logical components:

### Controllers

* `HomeController.cs` - Handles the main landing page and customer-facing views.
* `ProductController.cs` - Manages product details and catalog views.
* `CartController.cs` - Handles all shopping cart operations.
* `AccountController.cs` - Manages user registration, login, and profile actions.
* `DashboardController.cs` - Main controller for the admin dashboard, providing analytics.
* *Admin CRUD Controllers* - Separate controllers for managing Products, Categories, Brands, Orders, and Users from the admin panel.

### Data

* `ApplicationDbContext.cs` - Entity Framework Core DbContext for database operations.
* `Migrations/` - Contains database migration files for schema changes.

### Models

* **Entity Models**: `Product.cs`, `Category.cs`, `Brand.cs`, `Order.cs`, `User.cs`, etc.
* **ViewModels**: `DashboardViewModel.cs`, `LoginViewModel.cs`, `RegisterViewModel.cs`, and others for passing data to views.

### Views

* **Home**: Contains the main `Index.cshtml` view for the storefront.
* **Product**: Houses views for `Details.cshtml` and product listings.
* **Cart**: Includes the `Index.cshtml` view for the shopping cart.
* **Admin**: A dedicated area with views for the dashboard and all CRUD operations.
* **Shared**: Includes common layout files (`_Layout.cshtml`, `_AdminLayout.cshtml`), partials, and validation scripts.

### wwwroot

* Contains all static assets, including CSS, JavaScript, and images.

## Prerequisites

Before you can set up and run this project, ensure you have the following installed:

* **.NET 8 SDK**
* **Microsoft SQL Server**
* **Entity Framework Core tools** (can be installed via NuGet or .NET CLI)

## Setup Instructions

Follow these steps to get the project up and running on your local machine:

1.  **Clone the repository**
    ```bash
    git clone [https://github.com/abdalahsalah/Products](https://github.com/abdalahsalah/Products)
    ```

2.  **Restore dependencies**
    Navigate to the project's root directory in your terminal and run:
    ```bash
    dotnet restore
    ```
    This command downloads all necessary NuGet packages.

3.  **Configure the Database**
    * Open the `appsettings.json` file.
    * Update the `DefaultConnection` string with your local SQL Server credentials.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ProStoreDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```

4.  **Apply database migrations**
    After restoring dependencies, apply the database migrations to create your database schema:
    ```bash
    dotnet ef database update
    ```

5.  **Run the application**
    Finally, start the application:
    ```bash
    dotnet run
    ```
    The application will be accessible via `https://localhost:xxxx` or `http://localhost:xxxx` in your web browser.

## Features

### Customer-Facing Features

* **Product Catalog:** Browse all products with a clean, modern grid layout.
* **Dynamic Filtering:** Filter products by category without a page reload.
* **Product Details:** View detailed information, specifications, and stock availability.
* **Shopping Cart:** A fully functional cart to add, update quantities, and remove items.
* **User Authentication:** Secure user registration and login system.
* **Wishlist:** Save favorite products to a wishlist that persists in the browser's local storage.
* **Checkout Process:** A simple, multi-step process to enter shipping information and place an order.
* **Order History:** Registered users can view their past orders and check their status.
* **Responsive Design:** A mobile-first design that works seamlessly on all devices.

### Admin Dashboard Features

* **Analytics Dashboard:** At-a-glance view of key metrics like total users, products, orders, and revenue.
* **Data Visualization:** Interactive charts displaying monthly sales and product distribution by category.
* **Product Management (CRUD):** Full control to Create, Read, Update, and Delete products.
* **Category & Brand Management (CRUD):** Easily manage product categories and brands.
* **Order Management:** View all customer orders and update their status (e.g., Pending, Shipped, Delivered).
* **User Management:** View all registered users and manage their roles (Admin/Customer).


## Configuration

Database connection strings and other environment-specific settings can be configured in the **`appsettings.json`** file.

## Migration Notes

The project includes several Entity Framework Core migrations to manage the database schema's evolution, covering initial creation and subsequent updates for all necessary tables and relationships.
