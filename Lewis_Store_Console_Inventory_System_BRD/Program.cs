namespace Lewis_Store_Console_Inventory_System_BRD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool ContinueRun = true;
            string Name;
            int Age;
            bool Milk_Foam;
            Milk_Foam = false;
            Name = "";
            Age = 0;
            int Scof, Mcof, Lcof;
            Scof = 0;
            Mcof = 0;
            Lcof = 0;

            DetailsGrab();


            while (ContinueRun)
            {
            Start:
                Console.Clear();
                Console.WriteLine("Welcome To The Coffee Calculator");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Welcome \nMENU" + "\n1.Order Coffee" + "\n2.View Order" + "\n3.Create Payslip" + "\n4.Exit" + "\nPlease select a option");
                Console.WriteLine("-----------------------------------");



                if (int.TryParse(Console.ReadLine(), out int Input) && Input <= 4) { }
                else if (Input > 4)
                {
                    Console.WriteLine("That is not an available option please try again");
                    Console.ReadKey();
                    goto Start;
                }
                else
                {
                    Console.WriteLine("Invalid Number");
                    Console.ReadKey();
                    goto Start;
                }

                switch (Input)
                {
                    case 1:
                        bool KeepOrdering = true;
                        while (KeepOrdering)
                        {

                            Console.Clear();

                            Console.WriteLine("Currnt Order is: \n" + Scof + " Small Coffee\n" + Mcof + " Medium Coffee\n" + Lcof + " Large Coffee\n");
                            Console.WriteLine("Coffee Order" + "\n1.Small: R14" + "\n2.Medium: R17" + "\n3.Large R20" + "\n4.Finish Order");

                            if (int.TryParse(Console.ReadLine(), out int OrderChoice) && OrderChoice <= 4) { }
                            else if (OrderChoice > 4)
                            {
                                Console.WriteLine("That is not an available option please try again");
                                Console.ReadKey();

                            }
                            else
                            {
                                Console.WriteLine("Invalid Number");
                                Console.ReadKey();

                            }



                            if (OrderChoice == 1)
                            {

                                Console.Clear();
                                Console.WriteLine("How Many?: ");

                                if (int.TryParse(Console.ReadLine(), out int CofCount))
                                {
                                    Scof = Scof + CofCount;
                                    Console.WriteLine(CofCount + " Small Coffee Added");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Number");
                                    Console.ReadKey();

                                }

                            }
                            else if (OrderChoice == 2)
                            {
                                Console.Clear();
                                Console.WriteLine("How Many?: ");

                                if (int.TryParse(Console.ReadLine(), out int CofCount))
                                {
                                    Mcof = Mcof + CofCount;
                                    Console.WriteLine(CofCount + " Medium Coffee Added");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Number");
                                    Console.ReadKey();

                                }
                            }
                            else if (OrderChoice == 3)
                            {
                                Console.Clear();
                                Console.WriteLine("How Many?: ");

                                if (int.TryParse(Console.ReadLine(), out int CofCount))
                                {
                                    Lcof = Lcof + CofCount;
                                    Console.WriteLine(CofCount + " Large Coffee Added");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Number");
                                    Console.ReadKey();

                                }
                            }
                            else if (OrderChoice == 4)
                            {

                                Console.WriteLine("Would you like Warm/Foam Milk, R2.50 added to each cup \n1.Warm\n2.Milk");
                                if (int.TryParse(Console.ReadLine(), out int Choice) && Choice <= 2)
                                {
                                    if (Choice == 1)
                                    {
                                        Milk_Foam = true;
                                    }
                                    else if (Choice == 2)
                                    {
                                        Milk_Foam = false;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Choice, No Warm/Foam Milk by default");
                                }
                                Console.WriteLine("Order Submitted Please Proceed To Create Payslip");
                                Console.ReadKey();
                                KeepOrdering = false;
                            }
                        }
                        break;
                    case 2:
                        bool ExitOrderView = true;
                        Console.Clear();

                        while (ExitOrderView)
                        {
                            Console.WriteLine("Current Order" + "\n---------------------------\n" + Scof + " Small Coffee\n" + Mcof + " Medium Coffee\n" + Lcof + " Large Coffee");
                            Console.WriteLine("\n4.Return to menu? Y/N");
                            if (Console.ReadLine().ToUpper() == "Y")
                            {
                                ExitOrderView = false;
                            }
                            else
                            {
                                Console.Clear();
                            }
                        }
                        break;
                    case 3:
                        ExitOrderView = true;
                        while (ExitOrderView)
                        {
                            if (Milk_Foam)
                            {
                                Console.Clear();
                                double FinalAmount = Scof * 16.50 + Mcof * 19.50 + Lcof * 22.50;
                                Console.Write("Hello, " + Name);
                                Console.WriteLine("Your Current Total Amounts to: \n" + Scof + " Small Coffee  R" + Scof * 14 + "\n" + Mcof + " Medium Coffee R" + Mcof * 17 + "\n" + Lcof + " Large Coffee  R" + Lcof * 20 + "\nTotal Amount: R" + FinalAmount);
                                Console.WriteLine("4.Pay? Y/N");
                                if (Console.ReadLine().ToUpper() == "Y")
                                {
                                    ExitOrderView = false;
                                }
                                else if (Console.ReadLine().ToUpper() == "N")
                                {
                                    Console.Clear();
                                }
                            }
                            else
                            {
                                Console.Clear();
                                double FinalAmount = Scof * 16.50 + Mcof * 19.50 + Lcof * 22.50;
                                Console.WriteLine("Your Current Total Amounts to: \n" + Scof + " Small Coffee  R" + Scof * 14 + "\n" + Mcof + " Medium Coffee R" + Mcof * 17 + "\n" + Lcof + " Large Coffee  R" + Lcof * 20 + "\nTotal Amount: R" + FinalAmount);
                                Console.WriteLine("4.Pay? Y/N");
                                if (Console.ReadLine().ToUpper() == "Y")
                                {
                                    ExitOrderView = false;
                                }
                                else if (Console.ReadLine().ToUpper() == "N")
                                {
                                    Console.Clear();
                                }
                            }
                        }
                        break;
                    case 4:
                        Console.Clear();
                        Console.WriteLine("Thank You For The Purchase!");
                        ContinueRun = false;
                        Console.ReadKey();
                        break;
                    default:

                        break;
                }
            }
    }
}
