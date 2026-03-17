using System;
using System.ComponentModel;
using System.Threading.Channels;
using Spectre.Console;
using System.Data.SqlClient;


namespace Lewis_Store_Console_Inventory_System_BRD
{


    internal class Program
    {


        public static int? ItemSearch(string ItemName, string[] ItemN) 
        {
            int ItemIndex;
            for (int i = 0; i < ItemN.Length; i++)
            {   
                if (ItemName.ToUpper() == ItemN[i].ToUpper())
                {
                    ItemIndex = i;
                    return ItemIndex;
                }
            }
            return null;
        }

        public static decimal VATCAL(decimal Price)
        {
            decimal VAT = 0.15m;
            return (Price + (Price * VAT));
        }

        static void Main(string[] args)
        {
            string connectionstring = "Server=localhost;Database=LEWIS_STORE_STOCK;Trusted_Connection=True;";
            SqlConnection connection = new SqlConnection(connectionstring);

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
                   new Text("4. Exit", new Style(foreground: Color.Yellow, decoration: Decoration.Bold))
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
                            new ItemAdd(connection);
                            Console.Clear();

                            break;
                        }

                    case 2:
                        {
                            Console.Clear();
                            new DisplayStock(connection);

                            Console.WriteLine("Press Any Key To Continue");
                            Console.ReadKey();
                            Console.Clear();

                            break;
                        }
                    case 3:
                        {
                            TotalPrice = 0; TotalPriceVAT = 0;

                            Console.Clear();

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
                            ErrorStart:
                                Console.Clear();
                                AnsiConsole.Write(new Rule("[blue]Selling Stock[/]") { Justification = Justify.Center });



                            CancelItem:
                                Console.WriteLine("Type 'Checkout' to Checkout Or Press Any Key To Add Another Item");

                            } while (Console.ReadLine().ToUpper() != "CHECKOUT");



                            Console.Clear();

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
                            Totals.Columns[1].Header = new Text("R" + TotalPrice.ToString(), new Style(Color.Green, decoration: Decoration.Bold)).RightJustified();

                            Totals.Columns[0].Footer = new Text("Total [Incl.VAT]", new Style(decoration: Decoration.Bold)).RightJustified();
                            Totals.Columns[1].Footer = new Text("R" + TotalPriceVAT.ToString(), new Style(Color.Green, decoration: Decoration.Bold)).RightJustified();

                            var CheckTotal = new Panel(Totals)
                                .Border(BoxBorder.None)
                                .Expand();

                            //CheckTable.AddRow("","Total [Incl.VAT(15%)]: ", "R"+TotalPriceVAT.ToString());

                            AnsiConsole.Write(Align.Center(CheckItem));

                            AnsiConsole.Write(Align.Center(CheckTotal));

                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                    case 4:
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
