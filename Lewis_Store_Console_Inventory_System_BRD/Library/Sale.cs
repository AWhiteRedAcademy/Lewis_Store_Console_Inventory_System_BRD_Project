using System;
using System.Collections.Generic;
using System.Text;

namespace Lewis_Store_Console_Inventory_System_BRD.Library
{
    public class Sale
    {
        private int _SaleID;
        private decimal _Subtotal;
        private decimal _VatAmount;
        private decimal _TotalAmount;
        private DateTime _SaleDate;

        public int SaleID { get { return _SaleID; } set { _SaleID = value; } }
        public decimal Subtotal { get { return _Subtotal; } set { _Subtotal = value; } }
        public decimal VatAmount { get { return _VatAmount; } set { _VatAmount = value; } }
        public decimal TotalAmount { get { return _TotalAmount; } set { _TotalAmount = value; } }
        public DateTime SaleDate { get { return _SaleDate; } set { _SaleDate = value; } }

        public Sale(decimal subtotal, decimal vatamount, decimal totalamount, DateTime saledate)
        {
            Subtotal = subtotal;
            VatAmount = vatamount;
            TotalAmount = totalamount;
            SaleDate = saledate;
        }

        public int AddSale()
        {
            DatabaseManager Add = new DatabaseManager();
            SaleID = Add.AddSale(Subtotal, VatAmount, TotalAmount, SaleDate);
            return SaleID;
        }
    }
}
