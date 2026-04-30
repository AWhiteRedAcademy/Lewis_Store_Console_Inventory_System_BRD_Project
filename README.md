[README.md](https://github.com/user-attachments/files/27230981/README.md)
# Lewis Store Console Inventory System

## Project Overview

This is a C# console application created for managing stock in a Lewis Store inventory system. The program allows the user to add products, view current stock, sell items, update product details, delete products, and view sales history.

The system is designed to run in the console and uses Spectre.Console to make the menu and tables easier to read. Product and sales information is stored in a SQL Server database.

## Main Features

The application opens with a main menu where the user can choose what they want to do. The user can add a new item by entering the product name, description, quantity, and price excluding VAT. The system validates the information so that blank names, negative quantities, and invalid prices are not accepted.

The stock view shows all products currently saved in the database. It also shows stock warnings when an item is low in stock or out of stock.

The sell item section allows the user to select products, choose quantities, and complete a sale. The system calculates the subtotal, VAT at 15 percent, and final total. A receipt is then displayed on screen.

The update section allows the user to change a product name, description, quantity, or price. The delete section allows the user to remove a product after confirmation.

The sales history section displays previous sales and the items linked to each sale.

## Technologies Used

This project was built using C# and .NET. It uses SQL Server for database storage, System.Data.SqlClient for database communication, and Spectre.Console for the console interface.

## Database Information

The application connects to a local SQL Server database called LEWIS_STORE_STOCK. The current connection string is set inside the DatabaseManager class.

```csharp
Server=localhost;Database=LEWIS_STORE_STOCK;Trusted_Connection=True;
```

The database should contain tables for Products, Sales, and SaleItems. These tables are used to store stock records, completed sales, and the products sold in each sale.

## How To Run The Project

Open the project in Visual Studio. Make sure SQL Server is installed and that the database named LEWIS_STORE_STOCK exists on the local machine. Check that the required tables have been created before running the program.

Restore the NuGet packages if Visual Studio does not do this automatically. Build the project to check for errors, then run the application. The program will open in the console and show the main menu.

## Notes

This system is intended for basic stock management and sales tracking. It is suitable for a small inventory system where products need to be added, updated, sold, and reviewed through a simple console interface.
