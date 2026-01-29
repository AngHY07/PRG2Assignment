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
    internal class Restaurant
    {

        public string RestaurantId { get; set; }

        public string RestaurantName { get; set; }

        public string RestaurantEmail { get; set; }

        public List<Menu> Menu { get; set; }

        public List<Order> Order { get; set; }

        public List<SpecialOffer> SpecialOffer { get; set; }

        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            this.RestaurantId = restaurantId;
            this.RestaurantName = restaurantName;
            this.RestaurantEmail = restaurantEmail;
            Menu = new List<Menu>();
            Order = new List<Order>();
            SpecialOffer = new List<SpecialOffer>();

            Order = new List<Order>();
        }   

        public void DisplayOrders()
        {
            foreach (Order ord in Order)
            {
                Console.WriteLine($"{ord.OrderID,-8}  {ord.Customer.CustomerName}  {RestaurantName,-13}  {ord.DeliveryDateTime.ToString("dd/MM/yyyy hh/mm"),-18}  {$"${ord.OrderTotal}",-6}  {ord.OrderStatus,-9}  ");
            }
        }

        public void DisplaySpecialOffers()
        {
            foreach (SpecialOffer sp in SpecialOffer)
            {
                Console.WriteLine($"OfferCode: {sp.OfferCode}  OfferDesc: {sp.OfferDesc}  Discount: {sp.Discount}");
            }
        }

        public void DisplayMenu()
        {
            Console.WriteLine($"Restaurant: {RestaurantName} ({RestaurantId})");
            foreach (Menu m in Menu)
            {
                foreach(FoodItem fi in m.FoodItems)
                {
                    Console.WriteLine($"  - {fi.itemName}: {fi.itemDesc} - ${fi.itemPrice}");
                }
            }
        }

        public void AddMenu(Menu menu)
        {
            Menu.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            return Menu.Remove(menu);
        }

        public string ToString()
        {
            return $"RestaurantId : {RestaurantId}\nRestaurantName : {RestaurantName}\nRestaurantEmail : {RestaurantEmail}";
        }


    }
}
