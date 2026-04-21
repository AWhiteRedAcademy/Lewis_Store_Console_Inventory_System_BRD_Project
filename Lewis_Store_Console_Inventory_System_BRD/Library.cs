using Spectre.Console;
using Spectre.Console.Rendering;
using System; 
using System.Data.SqlClient; 
using System.Diagnostics; 
using System.IO.Pipelines; 
using System.Xml.Linq;

public class Product
{
    private int _ProductID;
    private string _Name;
    private string _Description;
    private int _Quantity;
    private decimal _PriceExclVAT;

    public int ProductID { get { return _ProductID; } set { _ProductID = value; } }
    public string Name { get { return _Name; } set { _Name = value; } }
    public string Description { get { return _Description; } set { _Description = value; } }
    public int Quantity
    {
        get { return _Quantity; }
        set
        {
            if (value >= 0)
            {
                _Quantity = value;
            }
            else { Console.WriteLine("ERROR CANNOT HAVE A QUANTITY BELOW 0"); }
        }
    }
    public decimal PriceExclVAT
    {
        get { return _PriceExclVAT; }
        set
        {
            if (value > 0)
            {
                _PriceExclVAT = value;
            }
            else { Console.WriteLine("ERROR CANNOT HAVE A PRICE OF ITEM BELOW 0"); }
        }
    }


    public Product(int productid, string name, string desc, int qty, decimal price)
    {
        ProductID = productid;
        Name = name;
        Description = desc;
        Quantity = qty;
        PriceExclVAT = price;
    }

    public void AddProduct()
    {
        DatabaseManager Add = new DatabaseManager();
        Add.AddProduct(_Name, _Description, _Quantity, _PriceExclVAT);
    }

    public void UpdateProduct()
    {
        Console.Clear();

        Table UpdateTable = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Title("[yellow bold] Update Product Details[/]");

        UpdateTable.AddColumn("Product ID");
        UpdateTable.AddColumn("Product Name");
        UpdateTable.AddColumn("Description");
        UpdateTable.AddColumn("Quantity");
        UpdateTable.AddColumn("Price Excl VAT");

        UpdateTable.AddRow(ProductID.ToString(), Name, Description, Quantity.ToString(), "R" + PriceExclVAT.ToString());

        AnsiConsole.Write(Align.Center(UpdateTable));

        var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\nPlease Select A Product To Update")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Products...")
                .AddChoices("[red]Cancel[/]", "Product Name", "Description", "Quantity", "Price Excl.VAT")
                .WrapAround());

        switch (Choice)
        {
            case "[red]Cancel[/]":
                {
                    Console.Clear();
                    return;
                }
            case "Product Name":
                {
                    string UpdateName = AnsiConsole.Prompt(new TextPrompt<string>("[red](WARNING DO NOT USE THIS TO REPLACE/ADD NEW PRODUCTS)[/]\nEnter New Product Name:"));
                    Name = UpdateName;
                    break;
                }

            case "Description":
                {
                    string UpdateDescription = AnsiConsole.Prompt(new TextPrompt<string>("Enter New Product Description:"));
                    Description = UpdateDescription;
                    break;
                }
            case "Quantity":
                {
                    int UpdateQuantity = AnsiConsole.Prompt(new TextPrompt<int>("Enter New Product Quantity:")
                        .Validate(qty => qty > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Quantity must be greater than 0[/]")));
                    Quantity = UpdateQuantity;
                    break;
                }
            case "Price Excl.VAT":
                {
                    decimal UpdatePrice = AnsiConsole.Prompt(new TextPrompt<decimal>("Enter New Product Price:")
                        .Validate(price => price > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Price must be greater than 0[/]")));
                    PriceExclVAT = UpdatePrice;
                    break;
                }
        }

        DatabaseManager Update = new DatabaseManager();
        Update.UpdateProduct(ProductID, Name, Description, Quantity, PriceExclVAT);

        AnsiConsole.MarkupLine("[green]Product Updated Successfully![/]");
        Console.ReadKey();
    }

    public void DeleteProduct()
    {
        DatabaseManager Delete = new DatabaseManager();
        Delete.DeleteProduct(ProductID);

        AnsiConsole.MarkupLine("[green]Product Deleted Successfully![/]");

        Console.ReadKey();
    }

    public void SellProduct(int quantity)
    {
        Quantity -= quantity;
        DatabaseManager Update = new DatabaseManager();
        Update.UpdateProduct(ProductID, Name, Description, Quantity, PriceExclVAT);
    }
}


public class Sale
{
    private int _SaleID;
    private decimal _Subtotal;
    private decimal _VatAmount;
    private decimal _TotalAmount;
    private DateTime _SaleDate;

    public int SaleID { get { return _SaleID; } set { _SaleID = value; } }
    public decimal Subtotal { get { return _Subtotal; } set { _Subtotal = value; } }
    public decimal VatAmount { get { return _VatAmount; } set { _VatAmount = value; } }
    public decimal TotalAmount { get { return _TotalAmount; } set { _TotalAmount = value; } }
    public DateTime SaleDate { get { return _SaleDate; } set { _SaleDate = value; } }

    public Sale(decimal subtotal, decimal vatamount, decimal totalamount, DateTime saledate)
    {
        Subtotal = subtotal;
        VatAmount = vatamount;
        TotalAmount = totalamount;
        SaleDate = saledate;
    }

    public int AddSale()
    {
        DatabaseManager Add = new DatabaseManager();
        SaleID = Add.AddSale(Subtotal, VatAmount, TotalAmount, SaleDate);
        return SaleID;
    }
}

public class SaleItem
{
    private int ProductID { get; set; }
    private int SaleID { get; set; }
    private decimal SalePrice { get; set; }
    private int Quantity { get; set; }

    public SaleItem(int saleID, int productID, int quantity, decimal salePrice)
    {
        SaleID = saleID;
        ProductID = productID;
        Quantity = quantity;
        SalePrice = salePrice;
    }

    public void AddSaleItem()
    {
        DatabaseManager Add = new DatabaseManager();
        Add.AddSaleItem(SaleID, ProductID, Quantity, SalePrice);
    }
}



public class Display
{
    public DatabaseManager display = new DatabaseManager();

    public Table StockTable;

    public Rows StockWarningRow;
    public Panel StockWarning;
    public List<string> ProductList;

    public Display()
    {
        StockTable = new Table()
                    .RoundedBorder()
                    .BorderColor(Color.Grey)
                    .ShowRowSeparators();
        StockTable.AddColumn("[yellow]Item[/]");
        StockTable.AddColumn("[yellow]Description[/]");
        StockTable.AddColumn("[yellow]Qty[/]");
        StockTable.AddColumn("[yellow]Price.Excl VAT[/]");

        var (productlist, stockwarn) = display.DisplayStock(StockTable);

        ProductList = productlist;

        StockWarningRow = new Rows(stockwarn);

        StockWarning = new Panel(StockWarningRow) {Width = 50 }
                    .Header("[red bold]Stock Warnings[/]", Justify.Center)
                    .RoundedBorder()
                    .BorderColor(Color.Red)
                    .Padding(1, 1);
    }

    public virtual void ViewStock()
    {

        var DisplayPanel = new Panel(StockTable)
            .Header("[lightgreen bold]View Product Stock[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
            {
                DisplayPanel,
                StockWarning
            });

        AnsiConsole.Write(DisplayLayout);
    }

    public void UpdateStock()
    {
        var DisplayPanel = new Panel(StockTable)
            .Header("[lightgreen bold]Update Product Stock[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
            {
                DisplayPanel,
                StockWarning
            });

        AnsiConsole.Write(DisplayLayout);

    }

    public void DeleteStock()
    {
        var DisplayPanel = new Panel(StockTable)
            .Header("[lightgreen bold]Delete Product[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
            {
                DisplayPanel
            });

        AnsiConsole.Write(DisplayLayout);
    }

    public void SellItemMenu(List<Product> CartList)
    {
        Panel SellItems = new Panel(StockTable)
            .Header("[lightgreen bold]Purchase[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        Table Cart = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Expand();

        Cart.AddColumn("[yellow]Items[/]", col => col.Centered());

            foreach (var item in CartList)
            {
                Cart.AddRow($"{item.Name} x{item.Quantity} - R{item.PriceExclVAT * item.Quantity}");
            }

        Panel CartPanel = new Panel(Cart)
            .Header("[lightgreen bold]Cart[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        Columns SellLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
            {
                SellItems,
                CartPanel});

        AnsiConsole.Write(new Rule("[lightgreen bold] SELL STOCK[/]"));
        AnsiConsole.Write(SellLayout);

    }

    public void SalesHistory()
    {
        List<Table> SaleItemHist = new List<Table>();

        Table SalesTable = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .ShowRowSeparators();
        SalesTable.AddColumn("[yellow]Sale ID[/]");
        SalesTable.AddColumn("[yellow]Subtotal[/]");
        SalesTable.AddColumn("[yellow]VAT Amount[/]");
        SalesTable.AddColumn("[yellow]Total Amount[/]");
        SalesTable.AddColumn("[yellow]Sale Date[/]");
        
        DatabaseManager SaleHist = new DatabaseManager();
        SaleHist.SaleHistory(SalesTable, SaleItemHist);

        Panel SalesPanel = new Panel(SalesTable) { Width = 70}
            .Header("[lightgreen bold]Sales History[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        Panel ItemPanel = new Panel(SaleItemHist[0]) { Width = 45 }
            .Header("[lightgreen bold]Items From Sale[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);

        Columns SalesHistMenu = new Columns(new Spectre.Console.Rendering.IRenderable[]{
              SalesPanel,
                ItemPanel
            });

        while (true)
        {
            AnsiConsole.Write(SalesHistMenu);
            
            var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\nSelect A Sale To View Items")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Sales...")
                .AddChoices("")
                .WrapAround()); 

            ItemPanel = new Panel(SaleItemHist[1]) {Width = 45}
            .Header("[lightgreen bold]Items From Sale[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1)
            .Expand();

            Console.Clear();
        }
    }
}

public class DatabaseManager
{
    private SqlConnection Connection { get; }

    public DatabaseManager()
    {
        try
        {
            string connectionstring = "Server=localhost;Database=LEWIS_STORE_STOCK;Trusted_Connection=True;";
            Connection = new SqlConnection(connectionstring);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Connection to Database Failed: " + ex.Message);
            Console.ReadKey();
        }
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

        SqlCommand command = new SqlCommand("SELECT * FROM Products", Connection);
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
            SqlCommand command = new SqlCommand("SELECT * FROM Products WHERE ProductID = @ID", Connection);
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
                decimal.Parse(reader["PriceExcludingVAT"]?.ToString() ?? "0")
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

    public void UpdateProduct(int id, string Name, string Description, int Quantity, decimal Price)
    {
        Connection.Open();

        string query = $"UPDATE Products SET ProductName = @Name, Description = @Description, QuantityInStock = @Qty, PriceExcludingVAT = @Price WHERE ProductID = @ID";
        SqlCommand command = new SqlCommand(query, Connection);

        command.Parameters.AddWithValue("@ID", id);
        command.Parameters.AddWithValue("@Name", Name);
        command.Parameters.AddWithValue("@Description", Description);
        command.Parameters.AddWithValue("@Qty", Quantity);
        command.Parameters.AddWithValue("@Price", Price);

        command.ExecuteNonQuery();

        Connection.Close();
    }

    public void DeleteProduct(int id)
    {
        Connection.Open();

        string query = $"DELETE FROM Products WHERE ProductID = @ID";
        SqlCommand command = new SqlCommand(query, Connection);

        command.Parameters.AddWithValue("@ID", id);

        command.ExecuteNonQuery();

        Connection.Close();
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
