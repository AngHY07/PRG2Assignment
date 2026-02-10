using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


//==========================================================
// Student Number : S10273989D
// Student Name : Ang Hao Yi
// Partner Name : Choo Yi Zehn
//==========================================================
namespace S10273989D_PRG2Assignment
{
    internal class Order
    {

        public int OrderID { get; set; }
        public DateTime OrderDateTime { get; set; }

        public double OrderTotal { get; set; }

        public bool FreeDelivery { get; set; }

        public string OrderStatus { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public string DeliveryAddress { get; set; }

        public string OrderPaymentMethod { get; set; }

        public bool OrderPaid { get; set; }

        public bool ApplicableEarl { get; set; }

        public bool WeekDay { get; set; }
        public Customer Customer { get; set; }

        public Restaurant Restaurant { get; set; }

        public List<OrderedFoodItem> OrderedFoodItem { get; set; }

        public List<SpecialOffer> SpecialOffer { get; set; }

        public Order(Customer customer, Restaurant restraurant, SpecialOffer specialOffer, int orderid, DateTime orderDateTime, string orderstatus, DateTime deliverydatetime, double totalamount,string deliveryaddress)
        {
            
            this.Customer = customer;
            this.Restaurant = restraurant;
            this.SpecialOffer = new List<SpecialOffer>();
            this.OrderID = orderid;
            this.OrderDateTime = orderDateTime;
            this.OrderStatus = orderstatus;
            this.DeliveryDateTime = deliverydatetime;
            this.DeliveryAddress = deliveryaddress;
            this.OrderTotal = totalamount;
            OrderedFoodItem = new List<OrderedFoodItem>();

        }

        public double CalculateOrderTotal(List<SpecialOffer> sp, Dictionary<string, SpecialOffer> spDict)
           
        {
            
            SpecialOffer deli = spDict["DELI"];
            SpecialOffer phol = spDict["PHOL"];
            SpecialOffer fest = spDict["FEST"];
            SpecialOffer earl = spDict["EARL"];
            SpecialOffer week = spDict["WEEK"];
            
            double total = 0;
            foreach (OrderedFoodItem item in OrderedFoodItem)
            {
                total += item.CalculateSubtotal(sp,spDict);
            }

            OrderTotal = total;

            if(sp.Any(o => o.OfferCode == week.OfferCode) && WeekDay)
            {
                OrderTotal = OrderTotal * (1 - week.Discount);
            }

            if (sp.Any(o => o.OfferCode == earl.OfferCode) && ApplicableEarl)
            {
                OrderTotal = OrderTotal * (1 - earl.Discount);
            }

            if (sp.Any(o => o.OfferCode == fest.OfferCode))
            {
                OrderTotal = OrderTotal * (1 - fest.Discount);
            }

            if (sp.Any(o => o.OfferCode == phol.OfferCode))
            {
                OrderTotal = OrderTotal * (1 - phol.Discount);
            }

            if (sp.Any(o => o.OfferCode == deli.OfferCode) && total > 30)
            {
                
                FreeDelivery = true;
            }
            else
            {
                OrderTotal = OrderTotal + 5;
                FreeDelivery = false;
            }

            return OrderTotal;
        }

        public double CalculateOrderTotal()
        {
            double total = 0;
            foreach (OrderedFoodItem item in OrderedFoodItem)
            {
                total += item.CalculateSubtotal();
            }

            OrderTotal = total + 5;
            return OrderTotal;

        }

        public void AddOrderedFoodItem(OrderedFoodItem orderedFooditem)
        {
            OrderedFoodItem.Add(orderedFooditem);
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem foodItem)
        {
            return OrderedFoodItem.Remove(foodItem);

        }

        public void DisplayOrderedFoodItems()
        {
            foreach(OrderedFoodItem item in OrderedFoodItem)
            {
                Console.WriteLine($"{item.ItemName} - {item.QtyOrdered}");
            }
        }

        public string ToString()
        {
            return $"OrderID : {OrderID}\nOrderDateTime : {OrderDateTime.ToString("dd/MM/yyyy  hh:mm")}\nOrderTotal : {OrderTotal}\nOrderStatus : {OrderStatus}\nDeliveryDateTime : {DeliveryDateTime}\nDeliveryAddress : {DeliveryAddress}\nOrderPaymentMethod : {OrderPaymentMethod}\nOrderedPaid : {OrderPaid}";
        }
    }
}
