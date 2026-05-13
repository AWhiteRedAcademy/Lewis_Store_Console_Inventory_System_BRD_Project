# Lewis Store Console Inventory System

## Overview

The Lewis Store Console Inventory System is a C# console application for managing basic stock and sales records for a store environment.

The application allows the user to add products, view stock, sell items, update product details, delete products, and view sales history. It uses SQL Server to store product, sale, and sale item records.

The application uses Spectre.Console to make the console menus and tables easier to read.

## Main Features

* Add new stock items
* View active stock items
* Sell items and calculate VAT
* Update product name, description, quantity, and price
* Delete or deactivate products
* View sales history
* Automatically create the database if it does not already exist
* Automatically create the required database tables
* Automatically add seed products when the Products table is empty

## Technologies Used

* C#
* .NET 10.0
* SQL Server
* System.Data.SqlClient
* Spectre.Console

## Project Requirements

Before running the application, make sure the computer has SQL Server installed and running.

The application connects to SQL Server using Windows Authentication with this connection string:

```csharp
Server=localhost;Database=LEWIS\_STORE\_STOCK;Trusted\_Connection=True;TrustServerCertificate=True;
```

This means the application expects SQL Server to be available on the same computer.

The Windows user running the application must also have permission to create and access a SQL Server database.

## Database Setup

The application includes an automatic database setup method called:

```csharp
EnsureDatabaseExists()
```

This method runs when the application starts.

It performs the following steps:

1. Connects to the SQL Server `master` database.
2. Checks if the `LEWIS\_STORE\_STOCK` database exists.
3. Creates the database if it does not exist.
4. Switches to the `LEWIS\_STORE\_STOCK` database.
5. Creates the `Sales` table if it does not exist.
6. Creates the `Products` table if it does not exist.
7. Creates the `SaleItems` table if it does not exist.
8. Adds seed product data if the `Products` table is empty.

Because of this, the user does not normally need to run the SQL script manually before opening the application.

## Database Tables

The application uses three main tables.

### Products

Stores the stock items.

Main fields:

* ProductID
* ProductName
* Description
* QuantityInStock
* PriceExcludingVAT
* ProductActive

### Sales

Stores the overall sale records.

Main fields:

* SaleID
* Subtotal
* VATAmount
* TotalAmount
* SalesDate

### SaleItems

Stores the individual products sold in each sale.

Main fields:

* SaleItemID
* SaleID
* ProductID
* Quantity
* SalePrice

## Seed Data

When the application starts, it checks whether the `Products` table is empty.

If the table is empty, it inserts sample products such as:

* Samsung 55 Inch Smart TV
* Defy Fridge Freezer
* LG Washing Machine
* Russell Hobbs Microwave
* KIC Chest Freezer
* Hisense Soundbar
* HP Laptop
* Canon Printer

This gives the application sample stock to work with immediately.

The seed data is only inserted when the `Products` table is empty, so it will not duplicate products every time the app opens.

## How to Run the Application in Visual Studio

1. Open the project in Visual Studio.
2. Make sure SQL Server is installed and running.
3. Build the solution.
4. Run the project.
5. The application will create the database and tables automatically if they do not exist.
6. The main menu will open in the console.

## Main Menu Options

When the program opens, the following options are available:

```text
Add Item
View Stock
Sell Items
Update Products
Delete Products
View Sales History
Exit
```

## Publishing the Application for Presentation

For presentation purposes, it is better to publish the application instead of running it from:

```text
bin/Debug/net10.0
```

Open Command Prompt or Terminal in the project folder and run:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "%USERPROFILE%\\Desktop\\LewisStoreApp"
```

This creates a cleaner published version of the application on the Desktop.

The published folder can be moved to another location, but the application will still need SQL Server installed and running on the computer where it is used.

```

## Common Errors and Fixes

### Database setup failed

This usually means SQL Server is not running, SQL Server is not installed, or the current Windows user does not have permission to create the database.

Check SQL Server and try again.

### Cannot connect to localhost

The app is trying to connect to SQL Server on the same computer.

Make sure SQL Server is installed locally and that the service is running.

### Application works on one computer but not another

The app can be moved, but the new computer also needs SQL Server installed and running.

If the app was not published as self-contained, the other computer may also need the correct .NET runtime installed.

## Notes for Marking

This project demonstrates:

* Object-oriented programming using separate classes for products, sales, sale items, display logic, and database logic
* SQL Server database integration
* Input validation
* Menu-driven console interaction
* Automatic database creation
* Seed data insertion
* Basic stock and sales management

## Project Structure

```text
Lewis\_Store\_Console\_Inventory\_System\_BRD
│
├── Program.cs
├── Lewis\_Store\_Console\_Inventory\_System\_BRD.csproj
├── Lewis\_Store\_StockCreation.sql
├── Auto\_Increment\_Reset.sql
│
└── Library
    ├── DatabaseManager.cs
    ├── Display.cs
    ├── Product.cs
    ├── Sale.cs
    └── SaleItem.cs
```

## Final Note

The application should be run on a Windows machine with SQL Server available locally. The automatic database setup reduces the need for manual setup, but SQL Server itself still needs to be installed and accessible.

