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
            if (value > 0)
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
        if (quantity > 0 && quantity <= Quantity)
        {
            Quantity -= quantity;
        }
        else
        {
            Console.WriteLine("ERROR: Invalid Quantity. Please enter a quantity between 1 and " + Quantity);
        }
    }

    public class Sale
    {
        private int ProductID { get; set; }
        private int QuantitySold { get; set; }
        private decimal Subtotal { get; set; }
        private decimal VatAmount { get; set; }
        private decimal TotalAmount { get; set; }
        private DateOnly SaleDate { get; set; }
        public Sale(int productid, int quantity, decimal subtotal, decimal vatamount, decimal totalamount, DateOnly saledate)
        {
            ProductID = productid;
            QuantitySold = quantity;
            Subtotal = subtotal;
            VatAmount = vatamount;
            TotalAmount = totalamount;
            SaleDate = saledate;
        }

        public void AddSale()
        {
            DatabaseManager Add = new DatabaseManager();
            Add.AddSale(ProductID, QuantitySold, Subtotal, VatAmount, TotalAmount, SaleDate);
        }

    }
}

public class Display
{
    public DatabaseManager display = new DatabaseManager();

    public Table StockTable;

    public Rows StockWarningRow;
    public Panel StockWarning;
    public string[] ProductList;

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

        StockWarning = new Panel(StockWarningRow)
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

        public (string[]  ProductList, List<IRenderable> StockWarn) DisplayStock(Table DisplayTable)
        {

            Connection.Open();

            string[] productlist = new string[100];
            List<IRenderable> stockwarn = new List<IRenderable>();

        int Count = 0;

            SqlCommand command = new SqlCommand("SELECT * FROM Products", Connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                DisplayTable.AddRow(reader["ProductName"].ToString(), reader["Description"].ToString(), reader["QuantityInStock"].ToString(), "R" + reader["PriceExcludingVAT"].ToString());
                productlist[Count] = $"{Count + 1}.    ID: "+ reader["ProductID"].ToString() + ")" + reader["ProductName"].ToString();

                if (int.Parse(reader["QuantityInStock"].ToString()) < 10)
                {
                    stockwarn.Add(new Markup($"[red bold]WARNING: {reader["ProductName"].ToString()} is low in stock! Only {reader["QuantityInStock"].ToString()} left![/]"));
                }

            Count++;
            }

            reader.Close();
            Connection.Close();

            return (productlist, stockwarn);

        }

        public Product PullProduct(int ItemID)
        {
            Connection.Open();

            SqlCommand command = new SqlCommand("SELECT * FROM Products WHERE ProductID = @ID", Connection);

            command.Parameters.AddWithValue("ID", ItemID);

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();

            Product Item = new Product(ItemID, reader["ProductName"].ToString(), reader["Description"].ToString(), int.Parse(reader["QuantityInStock"].ToString()), decimal.Parse(reader["PriceExcludingVAT"].ToString()));

            reader.Close();
            Connection.Close();

            return Item;
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
        public void AddSale(int productid, int quantity, decimal subtotal, decimal vatamount, decimal totalamount, DateOnly saledate)
        {
            Connection.Open();

            string query = $"INSERT INTO Sales (Subtotal,VATAmount,TotalAmount,SalesDate) " +
                           $"VALUES (@subtotal, @vatamount, @totalamount, @saledate)";
            SqlCommand command = new SqlCommand(query, Connection);

            command.Parameters.AddWithValue("@subtotal", subtotal);
            command.Parameters.AddWithValue("@vatamount", vatamount);
            command.Parameters.AddWithValue("@totalamount", totalamount);
            command.Parameters.AddWithValue("@saledate", saledate);

            command.ExecuteNonQuery();

            Connection.Close();
        }

        public void AddSaleItem(int saleID, int productID, int quantity, decimal saleprice)
        {
            Connection.Open();

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
