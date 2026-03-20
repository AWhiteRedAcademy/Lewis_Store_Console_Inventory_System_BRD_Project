using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using Spectre.Console;

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

        public static (string Name, string Desc, int Qty, decimal Price) ItemAdd()
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
            if (Name.ToUpper() == "CANCEL")
            {
                return ("", "", 0, 0);
            }

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
          
            AnsiConsole.MarkupLine("\n[green]Item Added Successfully[/]");
            Console.ReadKey();
            Console.Clear();

            return (Name, Desc, Qty, Price);
        }

        public static void DisplayStock (string[] ItemN, string[] ItemD, int[] ItemQ, decimal[] ItemP, int itemCount)
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

            for (int item = 0; item < itemCount; item++)
            {
                DisplayTable.AddRow(ItemN[item], ItemD[item], ItemQ[item].ToString(), "R" + ItemP[item].ToString("0.00"));
            }

            var DisplayPanel = new Panel(DisplayTable)
                .Header("[lightgreen bold]View Product Stock[/]", Justify.Center)
                .RoundedBorder()
                .BorderColor(Color.Blue)
                .Padding(2, 1);
            AnsiConsole.Write(Align.Center(DisplayPanel));
        }

        public static (decimal FinalAmount, decimal FinalAmountVAT, int ItemIndex, int Qty) SellItem(string[] ItemN, string[] ItemD, int[] ItemQ, decimal[] ItemP, int itemCount)
        {
            string? ItemName;

            decimal FinalAmount = 0;
            decimal FinalAmountVAT = 0;
            DisplayStock(ItemN, ItemD, ItemQ, ItemP, itemCount);

            

            ItemName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select Item Name To Add To Cart")
                .PageSize(10)
                .EnableSearch()
                .SearchPlaceholderText("Type to search Items")
                .AddChoices("Cancel")
                .AddChoices(ItemN.Where(item => !string.IsNullOrWhiteSpace(item)).ToArray()));

            if (ItemName == "" || ItemName == null) { Console.WriteLine("Error Item Does Not Exist"); Console.ReadKey(); Console.Clear(); return (0, 0, 0, 0); }

            if (ItemName.ToUpper() == "CANCEL") { Console.WriteLine("CANCELLING"); return (1, 0, 0, 0); }


            int? ItemIndex = ItemSearch(ItemName, ItemN);
            if (ItemIndex == null) { Console.WriteLine("Error Item Does Not Exist"); Console.ReadKey(); Console.Clear(); return (0, 0, 0, 0); }       

            Console.WriteLine("How Many Do You Wish To Buy?: ");
            if (!int.TryParse(Console.ReadLine(), out int Qty))
            {
                Console.WriteLine("Error Item Does Not Exist"); Console.ReadKey(); Console.Clear(); return (0, 0, 0, 0);
            }else if (Qty > ItemQ[ItemIndex.Value])
            {
                Console.WriteLine("Warning you cannot buy more than whats available in stock."); Console.ReadKey(); Console.Clear(); return (0, 0, 0, 0);
            }
 

            FinalAmount = (ItemP[ItemIndex.Value] * Qty);
            FinalAmountVAT = VATCAL(FinalAmount);


            return (FinalAmount, FinalAmountVAT, ItemIndex.Value, Qty);
        }

        public static decimal VATCAL(decimal Price)
        {
            decimal VAT = 0.15m;
            return (Price + (Price * VAT));
        }

        static void Main(string[] args)
        {
            string[] ItemN = new string[100];   //item name
            string[] ItemD = new string[100];   //item description
            int[] ItemQ = new int[100];         //item quantity
            decimal[] ItemP = new decimal[100];   //item price

            int itemCount = 0;
            bool Continue = true;

            decimal TotalPrice, TotalPriceVAT;

            while (Continue)
            {

                AnsiConsole.Write(new Rule("[lightgreen bold]The Lewis Store Inventory Management System[/]")
                { Justification = Justify.Center,
                   Border = BoxBorder.Double });

                var Menu = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("\n Please Select An Option")
                .AddChoices("Add Item", "View Stock", "Sell Items", "Exit")
                .WrapAround());

                switch (Menu)
                {
                    case "Add Item":
                        {

                            if (itemCount < 100)
                            {
                                var Add = ItemAdd();
                                if (Add.Name == "" && Add.Desc == "" && Add.Qty == 0 && Add.Price == 0)
                                {
                                    Console.WriteLine("Adding Item Cancelled");
                                    Console.ReadKey();
                                    Console.Clear();
                                    break;
                                }

                                ItemN[itemCount] = Add.Name; ItemD[itemCount] = Add.Desc; ItemQ[itemCount] = Add.Qty; ItemP[itemCount] = Add.Price; //Adding item to parallel arrays
                                itemCount++;
                            }
                            else Console.WriteLine("Error Maximium Item Count Reached No More May Be Added"); 
                            
                            Console.Clear();

                            break;
                        }

                    case "View Stock":
                        {
                            Console.Clear();
                            DisplayStock(ItemN, ItemD, ItemQ, ItemP, itemCount);

                            AnsiConsole.Prompt(new SelectionPrompt<string>().Title("\n[lightgreen bold]Back To Menu?[/]").AddChoices("Back"));

                            Console.Clear();

                            break;
                        }
                    case "Sell Items":
                        {
                            TotalPrice = 0.00m; TotalPriceVAT = 0.00m;

                            Console.Clear();

                            List<int> ItemsOrdered = new List<int>(itemCount);

                            var CheckTable = new Table()
                                .RoundedBorder()
                                .BorderColor(Color.Grey)
                                .Title("[yellow bold] Checkout Items[/]")
                                .Expand();

                            CheckTable.AddColumn("[yellow bold] Item[/]", col => col.Centered());
                            CheckTable.AddColumn("[yellow bold] Qty[/]", col => col.Centered());
                            CheckTable.AddColumn("[yellow bold] Price.Excl VAT[/] ", col => col.RightAligned());

                            do
                            {
                            ErrorStart:
                                Console.Clear();
                                AnsiConsole.Write(new Rule("[blue]Selling Stock[/]") { Justification = Justify.Center });

                                var SellItems = SellItem(ItemN, ItemD, ItemQ, ItemP, itemCount);
                                if (SellItems.ToString() == "(0, 0, 0, 0)") { goto ErrorStart; } //Error Code 0000
                                else if (SellItems.ToString() == "(1, 0, 0, 0)") { goto CancelItem; } //Cancelled Item

                                SellItems.ItemIndex += 1;

                                if (!ItemsOrdered.Contains(SellItems.ItemIndex))
                                {
                                    SellItems.ItemIndex -= 1;
                                    ItemQ[SellItems.ItemIndex] -= SellItems.Qty;

                                    TotalPrice += SellItems.FinalAmount;
                                    TotalPriceVAT += SellItems.FinalAmountVAT;

                                    CheckTable.AddRow(ItemN[SellItems.ItemIndex], SellItems.Qty.ToString(), "R" + ItemP[SellItems.ItemIndex].ToString("0.00"));

                                    ItemsOrdered.Add(SellItems.ItemIndex += 1);
                                }
                                else { Console.WriteLine("Already have this item in Cart"); }

                            CancelItem:;
                                Console.WriteLine();

                            } while (AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Add More Items Or Checkout?").AddChoices("Continue Shopping?", "Checkout")) != "Checkout");

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

                            var CheckLayout = new Layout("Root")
                                .SplitRows(
                                    new Layout("ItemDisplay")
                                    .Size(20),
                                    new Layout("Totals"));

                            CheckLayout["ItemDisplay"].Update(CheckItem);
                            CheckLayout["Totals"].Update(CheckTotal);

                            //CheckTable.AddRow("","Total [Incl.VAT(15%)]: ", "R"+TotalPriceVAT.ToString());

                            AnsiConsole.Write(Align.Center(CheckLayout));

                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                    case "Exit":
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
