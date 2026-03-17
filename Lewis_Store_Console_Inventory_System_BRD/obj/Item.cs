using System;

public class Item
{

    private string Name     { get { return Name; } set { Name = value; } };
    private string Desc     { get { return Desc; } set { Desc = value } };
    private int Quantity    { get { return Quantity; } set { Quantity = value } };
    private decimal Price   { get { return Price; } set { Price = value } };


    public Item(string Name, string Desc, int Qty, decimal Price)
	{

	}
}
