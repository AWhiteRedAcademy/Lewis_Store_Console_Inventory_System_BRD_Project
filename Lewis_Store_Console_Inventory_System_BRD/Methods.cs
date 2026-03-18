using Spectre.Console;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

public class Item
{

    private string Name     { get { return Name; } set { Name = value; } }
    private string Description     { get { return Description; } set { Description = value; } }
    private int Quantity    { get { return Quantity; } 
        set {
            if (value > 0)
            {
                Quantity = value;
            }
            else { Console.WriteLine("ERROR CANNOT HAVE A QUANTITY BELOW 0"); }
            } }
    private decimal Price   { get { return Price; } 
        set {
                if (value > 0)
                {
                    Price = value;
                }
                else { Console.WriteLine("ERROR CANNOT HAVE A PRICE OF ITEM BELOW 0"); }
            } }


    public Item(string name, string desc, int qty, decimal price)
	{
        Name = name;
        Description = desc;
        Quantity = qty;
        Price = price;
	}

}

public class DisplayStock
{
    public DisplayStock(SqlConnection connection)
    {
        var DisplayTable = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .ShowRowSeparators()
            .Expand();

        DisplayTable.AddColumn("[yellow]Item[/]");
        DisplayTable.AddColumn("[yellow]Description[/]");
        DisplayTable.AddColumns("[yellow]Qty[/]");
        DisplayTable.AddColumns("[yellow]Price.Excl VAT[/]");

        connection.Open();

        SqlCommand command = new SqlCommand("SELECT * FROM Item", connection);
        SqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            DisplayTable.AddRow(reader["Name"].ToString(), reader["Description"].ToString(), reader["Quantity"].ToString(), "R" + reader["Price"].ToString());
        }

        reader.Close();
        connection.Close();

        var DisplayPanel = new Panel(DisplayTable)
            .Header("[lightgreen bold]View Product Stock[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Blue)
            .Padding(2, 1);
        AnsiConsole.Write(Align.Center(DisplayPanel));

    }
}

public class ItemAdd
{
    public ItemAdd(SqlConnection connection)
    {
    ErrorStart:
        Console.Clear();

        var ItemAddPanel = new Panel("")
            .Header("[lightgreen bold]ADD ITEM[/]", Justify.Center)
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Padding(2, 1)
            .Expand();

        AnsiConsole.Write(Align.Center(ItemAddPanel));

        var NameP = new TextPrompt<string>("[yellow]Name Of Item: [/]")
            .PromptStyle(new Style(Color.Yellow));
        var DescP = new TextPrompt<string>("[yellow]Description: [/]")
            .PromptStyle(new Style(Color.Yellow));
        var QtyP = new TextPrompt<string>("[yellow]Qty: [/]")
            .PromptStyle(new Style(Color.Yellow));
        var PriceP = new TextPrompt<string>("[yellow]Price Excl.VAT: [/]")
            .PromptStyle(new Style(Color.Yellow));

        string Name = AnsiConsole.Prompt(NameP);

        if (Name == "")
        {
            Console.WriteLine("Error Invalid Name Please Try Again");
            Console.ReadKey();
            goto ErrorStart;

        }

        string Desc = AnsiConsole.Prompt(DescP);
        if (Desc == "")
        {
            Console.WriteLine("Error Invalid Description Please Try Again");
            Console.ReadKey();
            goto ErrorStart;

        }

        if (!int.TryParse(AnsiConsole.Prompt(QtyP), out int Qty) || Qty < 0)
        {
            Console.WriteLine("Error Invalid Quantity, Has to Be A Valid Number And Cannot Be Less Than 0");
            Console.ReadKey();
            goto ErrorStart;
        }

        string sPrice = AnsiConsole.Prompt(PriceP);

        if (sPrice.Contains("."))
        {
            sPrice = sPrice.Replace(".", ",");
        }

        if (!decimal.TryParse(sPrice, out decimal Price) || Price < 0)
        {

            Console.WriteLine("Error Invalid Price, Has to Be A Valid Number And Cannot Be Less Than R0");
            Console.ReadKey();
            goto ErrorStart;

        }

        connection.Open();

        string query = $"INSERT INTO Item (Name,Description,Quantity,Price) VALUES (@Name,@Desc,@Qty,@Price)";
        SqlCommand command = new SqlCommand(query,connection);

        command.Parameters.AddWithValue("@Name", Name);
        command.Parameters.AddWithValue("@Desc", Desc);
        command.Parameters.AddWithValue("@Qty", Qty);
        command.Parameters.AddWithValue("@Price", Price);

        command.ExecuteNonQuery();

        connection.Close();

        AnsiConsole.MarkupLine("\n[green]Item Added Successfully[/]");
        Console.ReadKey();
        Console.Clear();

    }
}



