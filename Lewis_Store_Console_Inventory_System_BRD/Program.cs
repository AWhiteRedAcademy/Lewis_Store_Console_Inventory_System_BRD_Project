using System;
using System.ComponentModel;
using System.Threading.Channels;
using Spectre.Console;
using System.Data.SqlClient;
using System.ComponentModel.Design;


namespace Lewis_Store_Console_Inventory_System_BRD
{


    internal class Program
    {
        public static (string[] ProductList, Table DisplayTable, Panel StockWarning) DisplayLoad()
        {
            var DisplayTable = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .ShowRowSeparators();

            DisplayTable.AddColumn("[yellow]Item[/]");
            DisplayTable.AddColumn("[yellow]Description[/]");
            DisplayTable.AddColumns("[yellow]Qty[/]");
            DisplayTable.AddColumns("[yellow]Price.Excl VAT[/]");

            var StockWarning = new Panel("[red bold]WARNING: LOW STOCK![/]")
                .Header("[yellow bold]Stock Warning[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Red)
                .Padding(1, 0);

            DatabaseManager display = new DatabaseManager();
            var ProductList = display.DisplayStock(DisplayTable, StockWarning);

            return (ProductList, DisplayTable, StockWarning);
        }

        public static (Columns DisplayFormat, Panel DisplayPanel) DisplayFormat()
        {
            var (ProductList, DisplayTable, StockWarning) = DisplayLoad();

            var DisplayPanel = new Panel(DisplayTable)
                .Header("[lightgreen bold]View Product Stock[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);

            var DisplayLayout = new Columns(new Spectre.Console.Rendering.IRenderable[]
                {
                DisplayPanel,
                StockWarning
                });

            return (DisplayLayout, DisplayPanel);
        }



        public static void AddProduct()
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

            Product Add = new Product(0, Name.ToString(), Desc.ToString(), Qty, Price);
            Add.AddProduct();

            AnsiConsole.MarkupLine("\n[green]Item Added Successfully[/]");
            Console.ReadKey();
            Console.Clear();
        }

        public static void SellItemMenu() 
        {
            Panel SellItems = DisplayFormat().DisplayPanel;

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

        public static void SellItem()
        {
            List<Product> CartItems = new List<Product>();
            
            while (true) 
            { 
                SellItemMenu();

                string[] ProductList = DisplayLoad().ProductList;

                var SelectItem = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\n[cyan]Select An Item To Sell[/]")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Products...")
                .AddChoices("[red]Cancel[/]")
                .AddChoices(ProductList.Where(p => p != null && p != "Null").ToArray())
                .AddChoices("[green]Proceed to Checkout?[/]")
                .WrapAround());


                if (SelectItem.Equals("[red]Cancel[/]"))
                {
                    Console.Clear();
                    return;
                } else if (SelectItem.Equals("[green]Proceed to Checkout?[/]"))
                {
                    Console.Clear();

                    Checkout(CartItems);

                    Console.Clear();
                    return;
                }
                else
                {   
                    int ProductID = int.Parse(SelectItem.Substring(SelectItem.IndexOf("ID: ") + 4, SelectItem.IndexOf(")")));

                    if (ProductID == CartItems.Find(x => x.ProductID == ProductID)?.ProductID) 
                    {
                        AnsiConsole.MarkupLine("[red]CANNOT ADD DUPLICATE ITEM TO CART[/]");
                        Console.ReadKey();
                    }
                    else 
                    {
                        DatabaseManager Pull = new DatabaseManager();
                        CartItems.Add(Pull.PullProduct(ProductID));

                    }

                }
            }
        }

        public static void Checkout(List<Product> CartItems)
        {
            AnsiConsole.MarkupLine("\n[cyan]Checkout is currently unavailable, Please contact the developer to enable this feature[/]");
            Console.ReadKey();
        }

        public static void UpdateProduct()
        {
            Columns DisplayLayout = DisplayFormat().DisplayFormat;

            var ProductList = DisplayLoad().ProductList;

            AnsiConsole.Write(DisplayLayout);
            AnsiConsole.MarkupLine("\n[cyan]ℹ Select a product below to update[/]");

            var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\nPlease Select A Product To Update")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Products...")
                .AddChoices("[red]Cancel[/]")
                .AddChoices(ProductList.Where(p => p != null && p != "Null").ToArray())
                .WrapAround());


            if (Choice.Equals("[red]Cancel[/]"))
            {
                Console.Clear();
                return;
            }
            else
            {
                string ProductID = Choice.Substring(Choice.IndexOf("ID: ") + 4, Choice.IndexOf(")"));

                DatabaseManager Pull = new DatabaseManager();
                Product CurrentItem = Pull.PullProduct(int.Parse(ProductID));

                CurrentItem.UpdateProduct();
               
            }

        }

        public static void DeleteProduct()
        {
            Columns DisplayLayout = DisplayFormat().DisplayFormat;

            var ProductList = DisplayLoad().ProductList;

            AnsiConsole.Write(DisplayLayout);

            var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\nPlease Select A Product To Delete")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Products...")
                .AddChoices("[red]Cancel[/]")
                .AddChoices(ProductList.Where(p => p != null && p != "Null").ToArray())
                .WrapAround());

            if (Choice.Equals("[red]Cancel[/]"))
            {
                Console.Clear();
                return;
            }
            else
            {
                var Confirmation = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\nARE YOU SURE YOU WISH TO DELETE THIS ITEM?")
                .AddChoices("[red]Cancel[/]", "[green]Yes[/]")
                .WrapAround());

                if (Confirmation.Equals("[red]Cancel[/]"))
                {
                    Console.Clear();
                    return;
                }

                string ProductID = Choice.Remove(Choice.IndexOf(")"));

                DatabaseManager Pull = new DatabaseManager();
                Product CurrentItem = Pull.PullProduct(int.Parse(ProductID));

                CurrentItem.DeleteProduct();
            }
            Console.Clear();
        }

        public static bool MainMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[lightgreen bold] The Lewis Store Inventory Management System[/]"));
            AnsiConsole.MarkupLine("\n[cyan]ℹ Scroll with Arrow Keys, Press Enter to Select[/]");

            var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("\nPlease Select An Option")
            .AddChoices("Add Item", "View Stock", "Sell Items", "Update Products", "Delete Products", "Exit")
            .WrapAround());

            switch (Choice)
            {
                case "Add Item":
                    {
                        AddProduct();
                        Console.Clear();
                        return true;
                    }

                case "View Stock":
                    {
                        Console.Clear();

                        Columns DisplayLayout = DisplayFormat().DisplayFormat;
                        AnsiConsole.Write(DisplayLayout);
                        AnsiConsole.MarkupLine("[yellow]Press Any Key To Continue[/]");

                        Console.ReadKey();
                        AnsiConsole.Clear();
                        return true;
                    }
                case "Sell Items":
                    {
                        Console.Clear();

                        SellItem();

                        Console.ReadKey();
                        Console.Clear();
                        return true;
                    }
                case "Update Products":
                    {
                        UpdateProduct();
                        Console.Clear();
                        return true;
                    }
                case "Delete Products":
                    {
                        DeleteProduct();
                        Console.Clear();
                        return true;
                    }
                case "Exit":
                    {
                        Console.Clear();
                        Console.WriteLine("Exiting now");
                        System.Environment.Exit(0);
                        return false;
                    }

                default:
                    {
                        Console.WriteLine("Invalid Choice, Option not available");
                        Console.ReadKey();
                        Console.Clear();
                        return true;
                    }
            }
        }



        static void Main(string[] args)
        {

            bool showMenu = true;

            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }
    }
}
    

