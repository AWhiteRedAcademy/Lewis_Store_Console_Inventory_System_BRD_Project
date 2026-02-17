namespace Lewis_Store_Console_Inventory_System_BRD
{
    internal class Program
    {
        static string[] itemnames = new string[100];
        static int[] itemquantities = new int[100];
        static double[] itemprices = new double[100];
        static int itemcount = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("The Lewis Store Inventory Management System");

        start:
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. View Inventory");
            Console.WriteLine("3. Exit");
            string choice = Console.ReadLine();
            goto start;
            while (true)
            {

                switch (choice)
                {
                    case "1":
                        if (itemcount < 100)
                        {
                            Console.WriteLine("Enter item name:");
                            itemnames[itemcount] = Console.ReadLine();
                            Console.WriteLine("Enter item quantity:");
                            itemquantities[itemcount] = int.Parse(Console.ReadLine());
                            Console.WriteLine("Enter item price:");
                            itemprices[itemcount] = double.Parse(Console.ReadLine());
                            itemcount++;
                            Console.WriteLine("Item added successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Inventory is full. Cannot add more items.");
                        }
                        break;
                    case "2":
                        Console.WriteLine("Current Inventory:");
                        for (int i = 0; i < itemcount; i++)
                        {
                            Console.WriteLine($"Item: {itemnames[i]}, Quantity: {itemquantities[i]}, Price: ${itemprices[i]:F2}");
                        }
                        break;
                    case "3":
                        Console.WriteLine("Exiting the program. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
