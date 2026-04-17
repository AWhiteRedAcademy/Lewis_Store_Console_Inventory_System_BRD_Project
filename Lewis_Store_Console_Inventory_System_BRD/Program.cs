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

            public static void AddProduct()
            {
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

            public static void SellItem()
            {
                List<Product> CartItems = new List<Product>();

                Display Sell = new Display();

                List<string> ProductList = Sell.ProductList;
                List<string> ProductInfo = new List<string>(ProductList.Where(p => p != null && p != "Null").Count());

                foreach (string Product in ProductList.Where(p => p != null && p != "Null"))
                {
                    ProductInfo.Add(Product.Substring(Product.IndexOf(")") + 1, Product.Length - (Product.IndexOf(")") + 1)));

                }

            while (true)
            {
                Console.Clear();
                Sell = new Display();

                Sell.SellItemMenu(CartItems);

                var SelectItem = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\n[cyan]Select An Item To Sell[/]")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Products...")
                .AddChoices("[red]Cancel[/]")
                .AddChoices(ProductInfo.Where(p => p != null && p != "Null"))
                .AddChoices("[green]Proceed to Checkout?[/]")
                .WrapAround());


                if (SelectItem.Equals("[red]Cancel[/]"))
                {
                    Console.Clear();
                    return;
                }
                else if (SelectItem.Equals("[green]Proceed to Checkout?[/]"))
                {
                    Console.Clear();

                    Checkout(CartItems);

                    Console.Clear();
                    return;
                }
                else
                {
                    string SProductID = ProductList.Where(p => p.Contains(SelectItem)).FirstOrDefault()?.Substring(0, ProductList.Where(p => p.Contains(SelectItem)).FirstOrDefault().IndexOf(")"));

                    SProductID = SProductID.Substring(SProductID.IndexOf("ID: ") + 4, SProductID.Length - (SProductID.IndexOf("ID: ") + 4));

                    int ProductID = int.Parse(SProductID);

                    if (ProductID == CartItems.Find(x => x.ProductID == ProductID)?.ProductID)
                    {
                        AnsiConsole.MarkupLine("[red]CANNOT ADD DUPLICATE ITEM TO CART[/]");
                        Console.ReadKey();
                    }
                    else
                    {
                        DatabaseManager Pull = new DatabaseManager();
                        CartItems.Add(Pull.PullProduct(ProductID));

                        var Quantity = new TextPrompt<int>("[yellow]Quantity To Purchase: [/]")
                            .Validate(Quantity =>
                            {
                                if (Quantity < 0)
                                {
                                    return ValidationResult.Error("[red]Quantity must be greater than 0[/]");
                                }

                                if (CartItems.Find(x => x.ProductID == ProductID).Quantity < Quantity)
                                {
                                    return ValidationResult.Error("[red]Quantity exceeds available stock[/]");
                                }

                                return ValidationResult.Success();
                            });

                        CartItems.Find(x => x.ProductID == ProductID).Quantity = AnsiConsole.Prompt(Quantity);
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
                Display Update = new Display();
                Update.UpdateStock();

                List<string> ProductList = Update.ProductList;
                List<string> ProductInfo = new List<string>(ProductList.Where(p => p != null && p != "Null").Count());

                foreach (string Product in ProductList.Where(p => p != null && p != "Null"))
                {
                    ProductInfo.Add(Product.Substring(Product.IndexOf(")") + 1, Product.Length - (Product.IndexOf(")") + 1)));

                }

                AnsiConsole.MarkupLine("\n[cyan]ℹ Select a product below to update[/]");

                var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("\nPlease Select A Product To Update")
                    .PageSize(10)
                    .EnableSearch()
                    .SearchPlaceholderText("Type to search Products...")
                    .AddChoices("[red]Cancel[/]")
                    .AddChoices(ProductInfo.Where(p => p != null && p != "Null"))
                    .WrapAround());


                if (Choice.Equals("[red]Cancel[/]"))
                {
                    Console.Clear();
                    return;
                }
                else
                {
                    string ProductID = ProductList.Where(p => p.Contains(Choice)).FirstOrDefault()?.Substring(0, ProductList.Where(p => p.Contains(Choice)).FirstOrDefault().IndexOf(")"));

                    ProductID = ProductID.Substring(ProductID.IndexOf("ID: ") + 4, ProductID.Length - (ProductID.IndexOf("ID: ") + 4));

                    DatabaseManager Pull = new DatabaseManager();
                    Product CurrentItem = Pull.PullProduct(int.Parse(ProductID));

                    CurrentItem.UpdateProduct();

                }

            }

            public static void DeleteProduct()
            {

                Display Delete = new Display();
                Delete.DeleteStock();

                List<string> ProductList = Delete.ProductList;
                List<string> ProductInfo = new List<string>(ProductList.Where(p => p != null && p != "Null").Count());

                foreach (string Product in ProductList.Where(p => p != null && p != "Null"))
                {
                    ProductInfo.Add(Product.Substring(Product.IndexOf(")") + 1, Product.Length - (Product.IndexOf(")") + 1)));
                }

                var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("\nPlease Select A Product To Delete")
                    .PageSize(10)
                    .EnableSearch()
                    .SearchPlaceholderText("Type to search Products...")
                    .AddChoices("[red]Cancel[/]")
                    .AddChoices(ProductInfo.Where(p => p != null && p != "Null"))
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

                    string ProductID = ProductList.Where(p => p.Contains(Choice)).FirstOrDefault()?.Substring(0, ProductList.Where(p => p.Contains(Choice)).FirstOrDefault().IndexOf(")"));

                    ProductID = ProductID.Substring(ProductID.IndexOf("ID: ") + 4, ProductID.Length - (ProductID.IndexOf("ID: ") + 4));

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

                            Display View = new Display();
                            View.ViewStock();
                            AnsiConsole.MarkupLine("[yellow]Press Any Key To Continue[/]");
                            Console.ReadKey();

                            AnsiConsole.Clear();
                            return true;
                        }
                    case "Sell Items":
                        {
                            Console.Clear();

                            SellItem();

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
    

