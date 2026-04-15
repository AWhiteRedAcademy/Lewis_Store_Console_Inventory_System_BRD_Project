using Spectre.Console;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Threading.Channels;
using System.Xml.Linq;


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
                .Validate(name =>
                {
                    if (string.IsNullOrWhiteSpace(name))
                        return ValidationResult.Error("[red]Name cannot be empty[/]");
                    else if (name.Length > 100)
                        return ValidationResult.Error("[red]Name cannot exceed 100 characters[/]");
                    else if (name.Contains("''") || name.Contains("\"\""))
                        return ValidationResult.Error("[red]Name cannot contain quotes[/]");
                    else
                        return ValidationResult.Success();
                })
                .PromptStyle(new Style(Color.Yellow));

            var DescP = new TextPrompt<string>("[yellow]Description: [/]")
                .Validate(desc =>
                {
                    if (string.IsNullOrWhiteSpace(desc))
                        return ValidationResult.Error("[red]Description cannot be empty[/]");
                    else if (desc.Length > 255)
                        return ValidationResult.Error("[red]Description cannot exceed 255 characters[/]");
                    else if (desc.Contains("''") || desc.Contains("\"\""))
                        return ValidationResult.Error("[red]Name cannot contain quotes[/]");
                    else
                        return ValidationResult.Success();
                })
                .PromptStyle(new Style(Color.Yellow));

            var QtyP = new TextPrompt<int>("[yellow]Qty: [/]")
                .Validate(qty =>
                {
                    if (qty < 0)
                        return ValidationResult.Error("[red]Quantity cannot be negative[/]");
                    else if (qty > 10000)
                        return ValidationResult.Error("[red]Quantity cannot exceed 10,000[/]");
                    else
                        return ValidationResult.Success();
                })
                .PromptStyle(new Style(Color.Yellow));

            var PriceP = new TextPrompt<decimal>("[yellow]Price Excl.VAT: [/]")
                .Validate(price =>
                {
                    if (price < 0)
                        return ValidationResult.Error("[red]Price cannot be negative[/]");
                    else if (price > 1000000)
                        return ValidationResult.Error("[red]Price cannot exceed 1,000,000[/]");
                    else
                        return ValidationResult.Success();
                })
                .PromptStyle(new Style(Color.Yellow));

            string Name = AnsiConsole.Prompt(NameP);

            string Desc = AnsiConsole.Prompt(DescP);

            int Quantity = AnsiConsole.Prompt(QtyP);

            decimal Price = AnsiConsole.Prompt(PriceP);

            Product Add = new Product(0, Name.ToString(), Desc.ToString(), Quantity, Price);
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
                string ProductID = Choice.Substring(Choice.IndexOf("ID: ") + 4, Choice.IndexOf(")") - (Choice.IndexOf("ID: ") + 4));

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

                string ProductID = Choice.Substring(Choice.IndexOf("ID: ") + 4, Choice.IndexOf(")") - (Choice.IndexOf("ID: ") + 4));

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
    

