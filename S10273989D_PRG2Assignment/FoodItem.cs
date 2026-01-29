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

        public string itemName { get; set; }
        public string itemDesc { get; set; }

        public double itemPrice { get; set; }

        public string customise { get; set; }

        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
        {
            this.itemName = itemName;
            this.itemDesc = itemDesc;
            this.itemPrice = itemPrice;
            this.customise = customise;
        }
    }
}
