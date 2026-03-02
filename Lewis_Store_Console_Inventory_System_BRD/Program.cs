using System;
using System.ComponentModel;
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
            Console.WriteLine("Selling Stock\n===============================================\n");
            Console.Clear();
            Console.WriteLine("Item Add\t(*Cancel)\n====================\nItem Name: ");
            string Name = Console.ReadLine();

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

            Console.WriteLine("Item Description: ");
            string Desc = Console.ReadLine();
            if (Desc == "")
            {
                Console.WriteLine("Error Invalid Description Please Try Again");
                Console.ReadKey();
                goto ErrorStart;

            }

            Console.WriteLine("Item Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int Qty) || Qty < 0)
            {
                Console.WriteLine("Error Invalid Quantity, Has to Be A Valid Number And Cannot Be Less Than 0");
                Console.ReadKey();
                goto ErrorStart;
            }

            Console.WriteLine("Item Price 'Excl.VAT': ");
            string sPrice = Console.ReadLine();

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

            Console.WriteLine("Item Added Successfully");
            Console.ReadKey();
            Console.Clear();

            return (Name, Desc, Qty, Price);
        }

        public static void DisplayStock (string[] ItemN, string[] ItemD, int[] ItemQ, decimal[] ItemP, int itemCount)
        {

            //{ for (int i = 0; i < itemCount; i++)
            //{

              //  Console.WriteLine($"{i}. |Item Name: {ItemN[i]}|\t|Quantity: {ItemQ[i]}|\t|Price Per Item'Excl.VAT': R{ItemP[i]}|\n|Description: {ItemD[i]}|\n" +
              //      $"===============================================\n");

            //}    

            var DisplayTable = new Table();

            DisplayTable.AddColumn("Item");
            DisplayTable.AddColumn("Description");
            DisplayTable.AddColumns("Qty");
            DisplayTable.AddColumns("Price.Excl VAT");

            for (int item = 0; item < itemCount; item++)
            {
                DisplayTable.AddRow(ItemN[item], ItemD[item], ItemQ[item].ToString(), "R" + ItemP[item].ToString());
            }

            AnsiConsole.Write(DisplayTable);
        }

        public static (decimal FinalAmount, decimal FinalAmountVAT, int ItemIndex, int Qty) SellItem(string[] ItemN, string[] ItemD, int[] ItemQ, decimal[] ItemP, int itemCount)
        {
            string ItemName;

            decimal FinalAmount = 0;
            decimal FinalAmountVAT = 0;
            DisplayStock(ItemN, ItemD, ItemQ, ItemP, itemCount);

            Console.WriteLine("Enter Item Name To Add To Cart, Item Name: ");
            ItemName = Console.ReadLine();
            if (ItemName == "") { Console.WriteLine("Error Item Does Not Exist"); return (0, 0, 0, 0); }


            int? ItemIndex = ItemSearch(ItemName, ItemN);
            if (ItemIndex == null) { Console.WriteLine("Error Item Does Not Exist"); return (0, 0, 0, 0); }       

            Console.WriteLine("How Many Do You Wish To Buy?: ");
            if (!int.TryParse(Console.ReadLine(), out int Qty))
            {
                Console.WriteLine("Error Item Does Not Exist"); return (0, 0, 0, 0);
            }else if (Qty > ItemQ[ItemIndex.Value])
            {
                Console.WriteLine("Warning you cannot buy more than whats available in stock."); return (0, 0, 0, 0);
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
            MenuStart:

                Console.WriteLine("The Lewis Store Inventory Management System\n-------------------------------------------------");
                Console.WriteLine("Please select an option:\n1. Add Item\n2. View Inventory\n3. Sell Items\n4. Exit");

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

                    case 2:
                        {
                            Console.Clear();
                            Console.WriteLine("View Product Stock\n===============================================\n");
                            DisplayStock(ItemN, ItemD, ItemQ, ItemP, itemCount);

                            Console.WriteLine("Press Any Key To Continue");
                            Console.ReadKey();
                            Console.Clear();

                            break;
                        }
                    case 3:
                        {
                            TotalPrice = 0; TotalPriceVAT = 0;

                            Console.Clear();
                            
                            Console.WriteLine("Selling Stock\n===============================================\nAdd Order To Cart? Y/N: ");
                            if (Console.ReadLine().ToUpper() == "N") { Console.Clear(); break; }

                            List<int> ItemsOrdered = new List<int>(itemCount);

                            var CheckTable = new Table();

                            CheckTable.AddColumn("Item");
                            CheckTable.AddColumns("Qty");
                            CheckTable.AddColumns("Price.Excl VAT");

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("Selling Stock\n===============================================\n");

                                var SellItems = SellItem(ItemN, ItemD, ItemQ, ItemP, itemCount);

                                if (!ItemsOrdered.Contains(SellItems.ItemIndex))
                                {

                                    ItemQ[SellItems.ItemIndex] -= SellItems.Qty;

                                    TotalPrice += SellItems.FinalAmount;
                                    TotalPriceVAT += SellItems.FinalAmountVAT;

                                    CheckTable.AddRow(ItemN[SellItems.ItemIndex], SellItems.Qty.ToString(), "R" + ItemP[SellItems.ItemIndex].ToString());

                                    ItemsOrdered.Add(SellItems.ItemIndex);

                                }
                                else { Console.WriteLine("Already have this item in checkout, Type 'Checkout' to Checkout");}

                            } while (Console.ReadLine().ToUpper() != "CHECKOUT");


                            Console.Clear();
                            Console.WriteLine("CHECKOUT\n===============================================\n");

                            CheckTable.AddRow("","","");
                            CheckTable.AddRow("","Total Excl.VAT: ", "R"+TotalPrice.ToString());
                            CheckTable.AddRow("","Total Incl.VAT(15%): ", "R"+TotalPriceVAT.ToString());

                            AnsiConsole.Write(CheckTable);
                            
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
