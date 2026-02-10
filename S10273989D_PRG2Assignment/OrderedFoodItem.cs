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


        public OrderedFoodItem(string itemName,string itemDesc, double itemPrice ,int qtyordered): base(itemName,itemDesc,itemPrice)
        {
            this.QtyOrdered = qtyordered;
        }

        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customize, int qtyordered) : base(itemName, itemDesc, itemPrice,customize)
        {
            this.QtyOrdered = qtyordered;
        }

        public double CalculateSubtotal(List<SpecialOffer> sp, Dictionary<string,SpecialOffer>spDict)
        {
            SpecialOffer bogo = spDict["BOGO"];

            if (sp.Any(o => o.OfferCode == bogo.OfferCode))
            {
                QtyOrdered = QtyOrdered * 2;
                SubTotal = ItemPrice * (QtyOrdered/2);
            }
            else
            {
                SubTotal = ItemPrice * QtyOrdered;
            }


            return SubTotal;
        }

        public double CalculateSubtotal()
        {
            SubTotal = ItemPrice * QtyOrdered;

            return SubTotal;
        }
    }
}
