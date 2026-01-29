//==========================================================
// Student Number : S10274355B
// Student Name : Choo Yi Zehn
// Partner Name : Ang Hao Yi
//==========================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10273989D_PRG2Assignment
{
    internal class FoodItem
    {

        public string ItemName { get; set; }
        public string ItemDesc { get; set; }

        public double ItemPrice { get; set; }

        public string Customise { get; set; }

        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
        {
            this.ItemName = itemName;
            this.ItemDesc = itemDesc;
            this.ItemPrice = itemPrice;
            this.Customise = customise;
        }

        public FoodItem()
        {

        }

        public override string ToString()
        {
            return "FoodItem: " + ItemName +
                    ", Description: " + ItemDesc +
                    ", Price: " + ItemPrice +
                    ", Customise: " + Customise;
        }

        public FoodItem(string itemName, string itemDesc, double itemPrice)
        {
            this.ItemName = itemName;
            this.ItemDesc = itemDesc;
            this.ItemPrice = itemPrice;
        }
    }
}
