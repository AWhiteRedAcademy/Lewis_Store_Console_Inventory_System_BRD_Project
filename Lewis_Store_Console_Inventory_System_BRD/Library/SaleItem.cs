using System;
using System.Collections.Generic;
using System.Text;

namespace Lewis_Store_Console_Inventory_System_BRD.Library
{
    public class SaleItem
    {
        private int ProductID { get; set; }
        private int SaleID { get; set; }
        private decimal SalePrice { get; set; }
        private int Quantity { get; set; }

        public SaleItem(int saleID, int productID, int quantity, decimal salePrice)
        {
            SaleID = saleID;
            ProductID = productID;
            Quantity = quantity;
            SalePrice = salePrice;
        }

        public void AddSaleItem()
        {
            DatabaseManager Add = new DatabaseManager();
            Add.AddSaleItem(SaleID, ProductID, Quantity, SalePrice);
        }
    }
}
