using System;
using System.ComponentModel;
using System.Threading.Channels;

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
            Console.WriteLine("Item Add\n====================\nItem Name: ");
            string Name = Console.ReadLine();

            Console.WriteLine("Item Description: ");
            string Desc = Console.ReadLine();

            Console.WriteLine("Item Quantity: ");
            int Qty = int.Parse(Console.ReadLine());

            Console.WriteLine("Item Price 'Excl.VAT': ");
            decimal Price = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Item Added Successfully");
            Console.ReadKey();
            Console.Clear();

            return (Name, Desc, Qty, Price);
        }

        public static void DisplayStock (string[] ItemN, string[] ItemD, int[] ItemQ, decimal[] ItemP, int itemCount)
        {

            for (int i = 0; i < itemCount; i++)
            {

                Console.WriteLine($"{i}. |Item Name: {ItemN[i]}|\t|Quantity: {ItemQ[i]}|\t|Price Per Item'Excl.VAT': R{ItemP[i]}|\n|Description: {ItemD[i]}|\n" +
                    $"===============================================\n");

            }
        }

        public static void SellItem(string[] ItemN, string[] ItemD, int[] ItemQ, decimal[] ItemP, int itemCount)
        {
            string ItemName;
            DisplayStock(ItemN, ItemD, ItemQ, ItemP, itemCount);

            Console.WriteLine("Enter Item Name To Add To Cart, Item Name: ");
            ItemName = Console.ReadLine();
            if (ItemName == "") { Console.WriteLine("Error Item Does Not Exist"); goto IfError; }
        

            int? ItemIndex = ItemSearch(ItemName, ItemN);
            if (ItemIndex == null) { Console.WriteLine("Error Item Does Not Exist"); goto IfError; }

            IfError:
            Console.WriteLine("To Checkout Type: Checkout");
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
                            Console.Clear();
                            if (itemCount < 100)
                            {
                                var Add = ItemAdd();
                                ItemN[itemCount] = Add.Name; ItemD[itemCount] = Add.Desc; ItemQ[itemCount] = Add.Qty; ItemP[itemCount] = Add.Price; //Adding item to parallel arrays

                                itemCount++;
                            }
                            else Console.WriteLine("Error Maximium Item Count Reached No More May Be Added"); Console.Clear();

                            break;
                        }

                    case 2:
                        {
                            Console.Clear();
                            Console.WriteLine("View Product Stock\n===============================================\n");
                            DisplayStock(ItemN, ItemD, ItemQ, ItemP, itemCount);
                            break;
                        }
                    case 3:
                        {
                            Console.Clear();
                            
                            Console.WriteLine("Selling Stock\n===============================================\nAdd Order To Cart? Y/N: ");
                            if (Console.ReadLine().ToUpper() == "N") { Console.Clear(); break; }

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("Selling Stock\n===============================================\n");

                                SellItem(ItemN, ItemD, ItemQ, ItemP, itemCount);

                            } while (Console.ReadLine().ToUpper() != "CHECKOUT");

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
