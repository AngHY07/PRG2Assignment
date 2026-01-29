using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//==========================================================
// Student Number : S10273989D
// Student Name : Ang Hao Yi
// Partner Name : Choo Yi Zehn
//==========================================================

namespace S10273989D_PRG2Assignment
{
    internal class OrderedFoodItem: FoodItem
    {

        public int QtyOrdered { get; set; }

        public double SubTotal { get; set; }


        public OrderedFoodItem(string itemName,string itemDesc, double itemPrice ,int qtyordered, double subtotal, FoodItem foodItem)
        {
            this.QtyOrdered = qtyordered;
            this.SubTotal = subtotal;
            FoodItem = foodItem;
        }

        public double CalculateSubtotal()
        {
            double subTotal = 0;

            subTotal += FoodItem.itemPrice * QtyOrdered;

            return subTotal;
        }
    }
}
