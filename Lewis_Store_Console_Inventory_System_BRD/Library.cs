using Spectre.Console;
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

    private int ProductID { get { return _ProductID; } set { _ProductID = value; } }
    private string Name     { get { return _Name; } set { _Name = value; } }
    private string Description     { get { return _Description; } set { _Description = value; } }
    private int Quantity    { get { return _Quantity; } 
        set {
            if (value > 0)
            {
                _Quantity = value;
            }
            else { Console.WriteLine("ERROR CANNOT HAVE A QUANTITY BELOW 0"); }
            } }
    private decimal PriceExclVAT   { get { return _PriceExclVAT; } 
        set {
                if (value > 0)
                {
                    _PriceExclVAT = value;
                }
                else { Console.WriteLine("ERROR CANNOT HAVE A PRICE OF ITEM BELOW 0"); }
            } }


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

    public void DisplayStock(Table DisplayTable)
    {

        Connection.Open();

        SqlCommand command = new SqlCommand("SELECT * FROM Products", Connection);
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            DisplayTable.AddRow(reader["ProductName"].ToString(), reader["Description"].ToString(), reader["QuantityInStock"].ToString(), "R" + reader["PriceExcludingVAT"].ToString());
        }

        reader.Close();
        Connection.Close();

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




