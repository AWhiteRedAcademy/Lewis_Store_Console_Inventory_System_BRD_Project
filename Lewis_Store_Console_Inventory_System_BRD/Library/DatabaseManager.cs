using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Lewis_Store_Console_Inventory_System_BRD.Library
{
    public class DatabaseManager
    {
        private SqlConnection Connection { get; }

        public DatabaseManager()
        {
            try
            {
                string connectionstring = "Server=localhost;Database=LEWIS_STORE_STOCK;Trusted_Connection=True;TrustServerCertificate=True;";
                Connection = new SqlConnection(connectionstring);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection to Database Failed: " + ex.Message);
                Console.ReadKey();
            }
        }

        public static void EnsureDatabaseExists()
        {
            string masterConnectionString =
                "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

            using SqlConnection connection = new SqlConnection(masterConnectionString);
            connection.Open();

            string createDatabaseQuery = @"
            IF DB_ID('LEWIS_STORE_STOCK') IS NULL
            BEGIN
            CREATE DATABASE LEWIS_STORE_STOCK;
        END";

            using SqlCommand createDatabaseCommand = new SqlCommand(createDatabaseQuery, connection);
            createDatabaseCommand.ExecuteNonQuery();

            connection.ChangeDatabase("LEWIS_STORE_STOCK");

            string createSalesTableQuery = @"
        IF OBJECT_ID('Sales', 'U') IS NULL
        BEGIN
            CREATE TABLE Sales(
                SaleID int IDENTITY(1,1) PRIMARY KEY,
                Subtotal decimal(10,2) NOT NULL,
                VATAmount decimal(10,2) NOT NULL,
                TotalAmount decimal(10,2) NOT NULL,
                SalesDate DATETIME NOT NULL
            );
        END";

            using SqlCommand createSalesCommand = new SqlCommand(createSalesTableQuery, connection);
            createSalesCommand.ExecuteNonQuery();

            string createProductsTableQuery = @"
        IF OBJECT_ID('Products', 'U') IS NULL
        BEGIN
            CREATE TABLE Products(
                ProductID int IDENTITY(1,1) PRIMARY KEY,
                ProductName varchar(100) NOT NULL,
                Description varchar(255),
                QuantityInStock int NOT NULL,
                PriceExcludingVAT decimal(10, 2) NOT NULL,
                ProductActive bit NOT NULL DEFAULT 1
            );
        END";

            using SqlCommand createProductsCommand = new SqlCommand(createProductsTableQuery, connection);
            createProductsCommand.ExecuteNonQuery();

            string createSaleItemsTableQuery = @"
        IF OBJECT_ID('SaleItems', 'U') IS NULL
        BEGIN
            CREATE TABLE SaleItems(
                SaleItemID int IDENTITY(1,1) PRIMARY KEY,
                SaleID int,
                ProductID int,
                Quantity int,
                SalePrice DECIMAL(10,2),
                FOREIGN KEY (SaleID) REFERENCES Sales(SaleID),
                FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
            );
        END";

            using SqlCommand createSaleItemsCommand = new SqlCommand(createSaleItemsTableQuery, connection);
            createSaleItemsCommand.ExecuteNonQuery();

            string seedProductsQuery = @"
    IF NOT EXISTS (SELECT 1 FROM Products)
    BEGIN
        INSERT INTO Products 
            (ProductName, Description, QuantityInStock, PriceExcludingVAT, ProductActive)
        VALUES
            ('Samsung 55 Inch Smart TV', '55 inch UHD smart television', 12, 6999.99, 1),
            ('Defy Fridge Freezer', 'Double door fridge freezer', 8, 8499.99, 1),
            ('LG Washing Machine', 'Front loader washing machine', 10, 5999.99, 1),
            ('Russell Hobbs Microwave', '30L silver microwave oven', 15, 1899.99, 1),
            ('KIC Chest Freezer', 'Large white chest freezer', 6, 3999.99, 1),
            ('Hisense Soundbar', 'Bluetooth TV soundbar', 20, 1299.99, 1),
            ('HP Laptop', '15 inch laptop for home and office use', 7, 8999.99, 1),
            ('Canon Printer', 'Wireless colour inkjet printer', 9, 2499.99, 1);
    END";

            using SqlCommand seedProductsCommand = new SqlCommand(seedProductsQuery, connection);
            seedProductsCommand.ExecuteNonQuery();
        }
        public void SaleHistory(Table SalesTable, List<Table> SaleItemHist)
        {
            Connection.Open();
            try
            {
                SqlCommand commandSale = new SqlCommand("SELECT COUNT(*) FROM Sales", Connection);
                SqlDataReader readerSale = commandSale.ExecuteReader();

                int[] SaleIDCount = new int[readerSale.Read() ? (int)readerSale[0] : 0];
                readerSale.Close();

                commandSale = new SqlCommand("SELECT * FROM Sales", Connection);
                readerSale = commandSale.ExecuteReader();
                int Count = 0;

                while (readerSale.Read())
                {
                    string saleDate = readerSale["SalesDate"]?.ToString() ?? DateTime.Now.ToShortDateString();
                    SalesTable.AddRow(
                        "Sale " + (Count + 1).ToString(),
                        readerSale["SaleID"]?.ToString(),
                        "R" + (readerSale["Subtotal"]?.ToString()),
                        "R" + (readerSale["VATAmount"]?.ToString()),
                        "R" + (readerSale["TotalAmount"]?.ToString()),
                        DateTime.Parse(saleDate).ToShortDateString()
                    );

                    int SaleID = int.Parse(readerSale["SaleID"].ToString());
                    SaleIDCount[Count] = SaleID;
                    Count++;
                }

                readerSale.Close();
                commandSale.Dispose();

                foreach (int SaleID in SaleIDCount)
                {

                    Table SaleItemsTable = new Table()
                        .RoundedBorder()
                        .BorderColor(Color.Grey)
                        .ShowRowSeparators();

                    SaleItemsTable.AddColumn("[yellow]Sale Item ID[/]");
                    SaleItemsTable.AddColumn("[yellow]Product ID[/]");
                    SaleItemsTable.AddColumn("[yellow]Quantity[/]");
                    SaleItemsTable.AddColumn("[yellow]Price[/]");

                    SqlCommand commandItems = new SqlCommand("SELECT * FROM SaleItems WHERE SaleID = @SaleID", Connection);
                    commandItems.Parameters.AddWithValue("@SaleID", SaleID);

                    SqlDataReader readerItems = commandItems.ExecuteReader();

                    while (readerItems.Read())
                    {
                        SaleItemsTable.AddRow(
                            readerItems["SaleItemID"]?.ToString(),
                            readerItems["ProductID"]?.ToString(),
                            readerItems["Quantity"]?.ToString(),
                            "R" + (readerItems["SalePrice"]?.ToString())
                        );
                    }
                    readerItems.Close();
                    commandItems.Dispose();

                    SaleItemHist.Add(SaleItemsTable);
                }
            }
            finally
            {
                Connection.Close();
            }
        }
        public (List<string> ProductList, List<IRenderable> StockWarn) DisplayStock(Table DisplayTable)
        {

            Connection.Open();

            List<string> productlist = new List<string>();
            List<IRenderable> stockwarn = new List<IRenderable>();

            stockwarn.Add(new Markup("[red]Warning: [/]"));

            int Count = 0;

            SqlCommand command = new SqlCommand("SELECT * FROM Products WHERE ProductActive = 1", Connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DisplayTable.AddRow(reader["ProductName"].ToString(), reader["Description"].ToString(), reader["QuantityInStock"].ToString(), "R" + reader["PriceExcludingVAT"].ToString());
                productlist.Add($"ID: {reader["ProductID"].ToString()}){Count + 1}. " + reader["ProductName"].ToString());

                if (int.Parse(reader["QuantityInStock"].ToString()) == 0)
                {
                    stockwarn.Add(new Markup($"[red bold]{reader["ProductName"].ToString()} is out of stock![/]"));
                }
                else if (int.Parse(reader["QuantityInStock"].ToString()) < 10)
                {
                    stockwarn.Add(new Markup($"[red bold]{reader["ProductName"].ToString()} is low in stock! Only {reader["QuantityInStock"].ToString()} left![/]"));
                }

                Count++;
            }

            if (stockwarn.Count < 2)
            {
                stockwarn.Clear();
                stockwarn.Add(new Markup("[green bold]All Stock Levels Are Sufficient![/]"));
            }

            reader.Close();
            Connection.Close();

            return (productlist, stockwarn);

        }

        public Product? PullProduct(int ItemID)
        {
            Connection.Open();
            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Products WHERE ProductID = @ID AND NOT ProductActive = 0", Connection);
                command.Parameters.AddWithValue("ID", ItemID);

                SqlDataReader reader = command.ExecuteReader();

                if (!reader.Read())
                {
                    reader.Close();
                    return null;  // Product not found
                }

                Product Item = new Product(
                    ItemID,
                    reader["ProductName"]?.ToString() ?? "Unknown",
                    reader["Description"]?.ToString() ?? "N/A",
                    int.Parse(reader["QuantityInStock"]?.ToString() ?? "0"),
                    decimal.Parse(reader["PriceExcludingVAT"]?.ToString() ?? "0"),
                    bool.Parse(reader["ProductActive"]?.ToString() ?? "true")
                );

                reader.Close();
                return Item;
            }
            finally
            {
                Connection.Close();
            }
        }

        public void AddProduct(string ProductName, string Description, int QuantityInStock, decimal PriceExcludingVAT)
        {
            Connection.Open();

            string query = $"INSERT INTO Products (ProductName,Description,QuantityInStock,PriceExcludingVAT) " +
                           $"VALUES (@Name,@Desc,@Qty,@Price)";
            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@Name", ProductName);
            command.Parameters.AddWithValue("@Desc", Description);
            command.Parameters.AddWithValue("@Qty", QuantityInStock);
            command.Parameters.AddWithValue("@Price", PriceExcludingVAT);

            command.ExecuteNonQuery();

            Connection.Close();

        }

        public void UpdateProduct(int id, string Name, string Description, int Quantity, decimal Price, bool Active)
        {
            Connection.Open();

            string query = $"UPDATE Products SET ProductName = @Name, Description = @Description, QuantityInStock = @Qty, PriceExcludingVAT = @Price, ProductActive = @Active WHERE ProductID = @ID";
            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@Name", Name);
            command.Parameters.AddWithValue("@Description", Description);
            command.Parameters.AddWithValue("@Qty", Quantity);
            command.Parameters.AddWithValue("@Price", Price);
            command.Parameters.AddWithValue("@Active",Active);

            command.ExecuteNonQuery();

            Connection.Close();
        }

        public void DeleteProduct(int id)
        {
            try
            {
                Connection.Open();

                string query = "DELETE FROM Products WHERE ProductID = @ID";
                SqlCommand command = new SqlCommand(query, Connection);
                command.Parameters.AddWithValue("@ID", id);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                AnsiConsole.MarkupLine("[red]Cannot delete this product because it has already been sold.[/]");
                AnsiConsole.MarkupLine("[yellow]Rather update the quantity to 0 or mark it as inactive.[/]");
                Console.ReadKey();
            }
            finally
            {
                if (Connection.State == System.Data.ConnectionState.Open)
                    Connection.Close();
            }
        }
        public int AddSale(decimal subtotal, decimal vatamount, decimal totalamount, DateTime saledate)
        {
            Connection.Open();

            string query = $"INSERT INTO Sales (Subtotal,VATAmount,TotalAmount,SalesDate) " +
                           $"VALUES (@subtotal, @vatamount, @totalamount, @saledate); SELECT SCOPE_IDENTITY();";
            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@subtotal", subtotal);
            command.Parameters.AddWithValue("@vatamount", vatamount);
            command.Parameters.AddWithValue("@totalamount", totalamount);
            command.Parameters.AddWithValue("@saledate", saledate);

            // Get the ID of the newly inserted sale
            object result = command.ExecuteScalar();
            int saleID = 0;

            if (result != null && result != DBNull.Value)
            {
                saleID = Convert.ToInt32(result);
            }

            Connection.Close();

            return saleID;
        }

        public void AddSaleItem(int saleID, int productID, int quantity, decimal saleprice)
        {
            Connection.Open();

            // Validate that the ProductID still exists in the database
            string validateQuery = "SELECT COUNT(*) FROM Products WHERE ProductID = @productID";
            SqlCommand validateCommand = new SqlCommand(validateQuery, Connection);
            validateCommand.Parameters.AddWithValue("@productID", productID);

            int productExists = (int)validateCommand.ExecuteScalar();

            if (productExists == 0)
            {
                Connection.Close();
                throw new Exception($"Product with ID {productID} no longer exists in the database. It may have been deleted.");
            }

            string query = $"INSERT INTO SaleItems (SaleID, ProductID, Quantity, SalePrice) " +
                           $"VALUES (@saleID, @productID, @quantity, @saleprice)";
            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@saleID", saleID);
            command.Parameters.AddWithValue("@productID", productID);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@saleprice", saleprice);

            command.ExecuteNonQuery();

            Connection.Close();
        }
    }
}
