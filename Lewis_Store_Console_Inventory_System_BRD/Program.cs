using System;

namespace Lewis_Store_Console_Inventory_System_BRD
{


    internal class Program
    {

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
                Console.WriteLine("Please select an option:\n1. Add Item\n2. View Inventory\n3. Sell items\n4. Exit");

                if !(int.TryParse(Console.ReadLine(), out MenuChoice))
                {
                    Console.WriteLine("Invalid Character Is Not A Integer");
                    Console.ReadKey();
                    goto MenuStart;
                }

                switch (MenuChoice)
                {
                    case 1:
                        {


                            break;
                        }

                    case 2:
                        {


                            break;
                        }
                    case 3:
                        {


                            break;
                        }
                    case 4:
                        {

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
