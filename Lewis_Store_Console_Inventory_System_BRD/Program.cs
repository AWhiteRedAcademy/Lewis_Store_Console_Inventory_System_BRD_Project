using System;
using System.ComponentModel;
using System.Threading.Channels;
using Spectre.Console;
using System.Data.SqlClient;


namespace Lewis_Store_Console_Inventory_System_BRD
{


    internal class Program
    {
        public static void DisplayStock()
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

            DatabaseManager display = new DatabaseManager();
            display.DisplayStock(DisplayTable);

            var DisplayPanel = new Panel(DisplayTable)
                .Header("[lightgreen bold]View Product Stock[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);
            AnsiConsole.Write(Align.Center(DisplayPanel));

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

            Product Add = new Product(0, Name.ToString(), Desc.ToString(), Qty,Price);
            Add.AddProduct();

            AnsiConsole.MarkupLine("\n[green]Item Added Successfully[/]");
            Console.ReadKey();
            Console.Clear();
        }

        public static void SellItem() 
        {

            var CheckTable = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title("[yellow bold] Checkout Items[/]")
                .Expand();


            CheckTable.AddColumn("Item", col => col.Centered());
            CheckTable.AddColumn("Qty", col => col.Centered());
            CheckTable.AddColumn("Price.Excl VAT", col => col.Centered());

            do 
            { 
        

                Console.WriteLine("Enter Item Name To Purchase Or Type 'CHECKOUT' To Finish Sale");
            } while (Console.ReadLine().ToUpper() != "CHECKOUT");


            var CheckItem = new Panel(CheckTable)
                .Border(BoxBorder.None)
                .Header("CHECKOUT", Justify.Center)
                .Expand();

            var Totals = new Table()
                .BorderColor(Color.Grey)
                .Expand()
                .AddColumn("")
                .AddColumn("");


            Totals.Columns[0].Header = new Text("Total [Excl.VAT]", new Style(decoration: Decoration.Bold)).RightJustified();
            Totals.Columns[1].Header = new Text("R" + "", new Style(Color.Green, decoration: Decoration.Bold)).RightJustified();

            Totals.Columns[0].Footer = new Text("Total [Incl.VAT]", new Style(decoration: Decoration.Bold)).RightJustified();
            Totals.Columns[1].Footer = new Text("R" + "", new Style(Color.Green, decoration: Decoration.Bold)).RightJustified();

            var CheckTotal = new Panel(Totals)
                .Border(BoxBorder.None)
                .Expand();

            //CheckTable.AddRow("","Total [Incl.VAT(15%)]: ", "R"+TotalPriceVAT.ToString());

            AnsiConsole.Write(Align.Center(CheckItem));

            AnsiConsole.Write(Align.Center(CheckTotal));

        }


        static void Main(string[] args)
        {

            bool Continue = true;

            decimal TotalPrice, TotalPriceVAT;

            while (Continue)
            {
            MenuStart:
                var MainScreen = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title("[lightgreen bold] The Lewis Store Inventory Management System[/]");

                MainScreen.AddColumn("Please select an option:");


                MainScreen.AddRow(
                    new Text("1. Add Item", new Style(foreground: Color.Yellow,decoration: Decoration.Bold))
                );
                MainScreen.AddRow(
                   new Text("2. View Inventory", new Style(foreground: Color.Yellow, decoration: Decoration.Bold))
                );
                MainScreen.AddRow(
                   new Text("3. Sell Items", new Style(foreground: Color.Yellow, decoration: Decoration.Bold))
                );
                MainScreen.AddRow(
                   new Text("4. Update Products", new Style(foreground: Color.Yellow, decoration: Decoration.Bold))
                );
                MainScreen.AddRow(
                   new Text("5. Exit", new Style(foreground: Color.Yellow, decoration: Decoration.Bold))
                );

                AnsiConsole.Write(Align.Center(MainScreen));

                if (!int.TryParse(Console.ReadLine(), out int MenuChoice) || MenuChoice > 4)
                {
                    Console.WriteLine("Invalid Character, Is Not A Integer or Out Of Bounds");
                    Console.ReadKey();
                    Console.Clear();
                    goto MenuStart;
                }

                switch (MenuChoice)
                {
                    case 1:
                        {
                            AddProduct();
                            Console.Clear();

                            break;
                        }

                    case 2:
                        {
                            Console.Clear();

                            DisplayStock();

                            Console.WriteLine("Press Any Key To Continue");
                            Console.ReadKey();
                            Console.Clear();

                            break;
                        }
                    case 3:
                        {
                            Console.Clear();

                            SellItem();

                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                    case 4: 
                        {
                            break;
                        }
                    case 5:
                        {
                            Console.Clear();
                            Console.WriteLine("Exiting now");
                            System.Environment.Exit(0);
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Invalid Choice, Option not available");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                }
            }
        }
    }
}
