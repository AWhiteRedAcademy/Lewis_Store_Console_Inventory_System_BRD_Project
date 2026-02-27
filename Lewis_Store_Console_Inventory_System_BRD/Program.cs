using System;
using System.ComponentModel;
using System.Threading.Channels;

namespace Lewis_Store_Console_Inventory_System_BRD
{


    internal class Program
    {


        void ItemSearch(string ItemName, string[] ItemN) 
        {

        }

        public static (string Name, string Desc, int Qty, double Price) ItemAdd()
        {
            Console.WriteLine("Item Add\n====================\nItem Name: ");
            string Name = Console.ReadLine();
            Console.WriteLine("Item Description: ");
            string Desc = Console.ReadLine();
            Console.WriteLine("Item Quantity: ");
            int Qty = int.Parse(Console.ReadLine());
            Console.WriteLine("Item Price 'Excl.VAT': ");
            double Price = double.Parse(Console.ReadLine());

            Console.WriteLine("Item Added Successfully");
            Console.ReadKey();
            Console.Clear();

            return (Name, Desc, Qty, Price);


        }


        static void Main(string[] args)
        {
            string[] ItemN = new string[100];   //item name
            string[] ItemD = new string[100];   //item description
            int[] ItemQ = new int[100];         //item quantity
            double[] ItemP = new double[100];   //item price

            int itemCount = 0;
            bool Continue = true;

            while (Continue)
            {
            MenuStart:
                Console.WriteLine("The Lewis Store Inventory Management System");
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
                            var Add = ItemAdd();
                            ItemN[itemCount] = Add.Name;
                            ItemD[itemCount] = Add.Desc;
                            ItemQ[itemCount] = Add.Qty;
                            ItemP[itemCount] = Add.Price;

                            itemCount++;
                            break;
                        }

                    case 2:
                        {
                            Console.Clear();
                            
                            for (int i = 0; i < itemCount i++)
                            {

                            }

                            break;
                        }
                    case 3:
                        {
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

                            break;
                        }
                }
            }
        }
    }
}
