using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lewis_Store_Console_Inventory_System_BRD.Library
{
    public class Product
    {
        private int _ProductID;
        private string _Name;
        private string _Description;
        private int _Quantity;
        private decimal _PriceExclVAT;

        public int ProductID { get { return _ProductID; } set { _ProductID = value; } }
        public string Name { get { return _Name; } set { _Name = value; } }
        public string Description { get { return _Description; } set { _Description = value; } }
        public int Quantity
        {
            get { return _Quantity; }
            set
            {
                if (value >= 0)
                {
                    _Quantity = value;
                }
                else { Console.WriteLine("ERROR CANNOT HAVE A QUANTITY BELOW 0"); }
            }
        }
        public decimal PriceExclVAT
        {
            get { return _PriceExclVAT; }
            set
            {
                if (value > 0)
                {
                    _PriceExclVAT = value;
                }
                else { Console.WriteLine("ERROR CANNOT HAVE A PRICE OF ITEM BELOW 0"); }
            }
        }


        public Product(int productid, string name, string desc, int qty, decimal price)
        {
            ProductID = productid;
            Name = name;
            Description = desc;
            Quantity = qty;
            PriceExclVAT = price;
        }

        public void AddProduct()
        {
            DatabaseManager Add = new DatabaseManager();
            Add.AddProduct(_Name, _Description, _Quantity, _PriceExclVAT);
        }

        public void UpdateProduct()
        {
            Console.Clear();

            Table UpdateTable = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title("[yellow bold] Update Product Details[/]");

            UpdateTable.AddColumn("Product ID");
            UpdateTable.AddColumn("Product Name");
            UpdateTable.AddColumn("Description");
            UpdateTable.AddColumn("Quantity");
            UpdateTable.AddColumn("Price Excl VAT");

            UpdateTable.AddRow(ProductID.ToString(), Name, Description, Quantity.ToString(), "R" + PriceExclVAT.ToString());

            AnsiConsole.Write(Align.Center(UpdateTable));

            var Choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("\nPlease Select A Product To Update")
                    .PageSize(10)
                    .EnableSearch()
                    .SearchPlaceholderText("Type to search Products...")
                    .AddChoices("[red]Cancel[/]", "Product Name", "Description", "Quantity", "Price Excl.VAT")
                    .WrapAround());

            switch (Choice)
            {
                case "[red]Cancel[/]":
                    {
                        Console.Clear();
                        return;
                    }
                case "Product Name":
                    {
                        string UpdateName = AnsiConsole.Prompt(new TextPrompt<string>("[red](WARNING DO NOT USE THIS TO REPLACE/ADD NEW PRODUCTS)[/]\nEnter New Product Name:"));
                        Name = UpdateName;
                        break;
                    }

                case "Description":
                    {
                        string UpdateDescription = AnsiConsole.Prompt(new TextPrompt<string>("Enter New Product Description:"));
                        Description = UpdateDescription;
                        break;
                    }
                case "Quantity":
                    {
                        int UpdateQuantity = AnsiConsole.Prompt(new TextPrompt<int>("Enter New Product Quantity:")
                            .Validate(qty => qty > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Quantity must be greater than 0[/]")));
                        Quantity = UpdateQuantity;
                        break;
                    }
                case "Price Excl.VAT":
                    {
                        decimal UpdatePrice = AnsiConsole.Prompt(new TextPrompt<decimal>("Enter New Product Price:")
                            .Validate(price => price > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Price must be greater than 0[/]")));
                        PriceExclVAT = UpdatePrice;
                        break;
                    }
            }

            DatabaseManager Update = new DatabaseManager();
            Update.UpdateProduct(ProductID, Name, Description, Quantity, PriceExclVAT);

            AnsiConsole.MarkupLine("[green]Product Updated Successfully![/]");
            Console.ReadKey();
        }

        public void DeleteProduct()
        {
            DatabaseManager Delete = new DatabaseManager();
            Delete.DeleteProduct(ProductID);

            AnsiConsole.MarkupLine("[green]Product Deleted Successfully![/]");

            Console.ReadKey();
        }

        public void SellProduct(int quantity)
        {
            Quantity -= quantity;
            DatabaseManager Update = new DatabaseManager();
            Update.UpdateProduct(ProductID, Name, Description, Quantity, PriceExclVAT);
        }
    }
}
