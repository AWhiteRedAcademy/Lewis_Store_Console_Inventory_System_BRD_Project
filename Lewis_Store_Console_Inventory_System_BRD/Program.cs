using System;

namespace Lewis_Store_Console_Inventory_System_BRD
using System;

namespace Lewis_Store_Console_Inventory_System_BRD
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string[] Sitemnames = new string[100];
            int[] Iitemquantities = new int[100];
            double[] Ditemprices = new double[100];
            int itemcount = 0;
            bool Continue = true;

            while (Continue)
            {

                Console.WriteLine("The Lewis Store Inventory Management System");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Add Item");
                Console.WriteLine("2. View Inventory");
                Console.WriteLine("3. Sell items");
                Console.WriteLine("4. Exit");

                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        {
                            if (itemcount < 100)
                            {
                                Console.WriteLine("Enter item name:");
                                Sitemnames[itemcount] = Console.ReadLine();
                                Console.WriteLine("Enter item quantity:");
                                Iitemquantities[itemcount] = int.Parse(Console.ReadLine());
                                Console.WriteLine("Enter item price:");
                                Ditemprices[itemcount] = double.Parse(Console.ReadLine());
                                itemcount++;
                                Console.WriteLine("Item added successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Inventory is full. Cannot add more items.");
                            }
                            break;
                        }

                    case 2:
                        {
                            Console.WriteLine("Current Inventory:");
                            for (int i = 0; i < itemcount; i++)
                            {
                                Console.WriteLine("Item: " + Sitemnames[i] + " Quantity: " + Iitemquantities[i] + " Price: R" + Ditemprices[i]);
                                Console.ReadKey();
                            }
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("Enter item name to sell:");
                            string SitemNameToSell = Console.ReadLine();
                            int index = Array.IndexOf(Sitemnames, SitemNameToSell);
                            if (index != -1)
                            {
                                Console.WriteLine("Enter quantity to sell:");
                                int quantityToSell = int.Parse(Console.ReadLine());
                                if (quantityToSell <= Iitemquantities[index])
                                {
                                    Iitemquantities[index] -= quantityToSell;
                                    double totalPrice = quantityToSell * Ditemprices[index];
                                    Console.WriteLine("Sold " + quantityToSell + " of " + Sitemnames[index] + " for a total of $" + totalPrice);
                                }
                                else
                                {
                                    Console.WriteLine("Not enough stock to sell. Available quantity: " + Iitemquantities[index]);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Item not found in inventory.");
                                Console.ReadKey();
                                Console.Clear();
                            }
                            break;

                        }
                    case 4:
                        {
                            Continue = false;
                            Console.WriteLine("Exiting the program. Goodbye!");
                            Console.ReadKey();
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Invalid option. Please try again.");
                            Console.ReadKey();
                            Console.Clear();
                            break;
                        }
                }
            }
        }
    }
}