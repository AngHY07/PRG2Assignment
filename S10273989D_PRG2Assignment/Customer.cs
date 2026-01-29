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
    internal class Customer
    {

        public string EmailAddress { get; set; }

        public string CustomerName { get; set; }

        public List<Order> Orders { get; set; }

        public Customer (string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
            Orders = new List<Order>();
        }

        public Customer()
        {

        }

        public void AddOrder(Order or)
        {
            Orders.Add(or);
        }

        public void DisplayAllOrders()
        {
            Console.WriteLine("All Orders");
            Console.WriteLine("==========");
            Console.WriteLine("Order ID    Customer    Restaurant    Delivery Date / Time    Amount    Status");
            Console.WriteLine("--------    ----------    -------------    ------------------    ------     ---------");
            foreach (Order order in Orders)
            {
                Console.WriteLine($"{order.OrderID}    {CustomerName}    {order.Restaurant.RestaurantName}    {order.DeliveryDateTime}    {order.DeliveryDateTime}    {order.OrderTotal}    {order.OrderStatus}");
            }
        }

        public bool RemoveOrder(Order or) 
        {
            return Orders.Remove(or);
        }

        public override string ToString()
        {
            return "Customer: " + CustomerName +
                    ", Email Address: " + EmailAddress;
        }
           
    }
}
