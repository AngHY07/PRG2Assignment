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
    internal class Order
    {

        public int OrderID { get; set; }
        public DateTime OrderDateTime { get; set; }

        public double OrderTotal { get; set; }

        public double OrderStatus { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public string DeliveryAddress { get; set; }

        public string OrderPaymentMethod { get; set; }

        public bool OrderPaid { get; set; }

        public Customer Customer { get; set; }

        public Restaurant Restaurant { get; set; }

        public List<OrderedFoodItem> OrderedFoodItem { get; set; }

        public SpecialOffer SpecialOffer { get; set; }

        public Order(Customer customer, Restaurant restraurant, SpecialOffer specialOffer, int orderid, DateTime orderDateTime, double orderstatus, DateTime deliverydatetime, string deliveryaddress, string orderpaymentmethod, bool orderpaid)
        {
            
            this.Customer = customer;
            this.Restaurant = restraurant;
            this.SpecialOffer = specialOffer;
            this.OrderID = orderid;
            this.OrderDateTime = orderDateTime;
            this.OrderStatus = orderstatus;
            this.DeliveryDateTime = deliverydatetime;
            this.DeliveryAddress = deliveryaddress;
            this.OrderPaymentMethod = orderpaymentmethod;
            this.OrderPaid = orderpaid;
            OrderedFoodItem = new List<OrderedFoodItem>();

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
