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
    internal class Menu
    {
        public string MenuID { get; set; }

        public string MenuName { get; set; }

        public List<FoodItem> FoodItems { get; set; }   

        public Menu(string menuID, string menuName)
        {
            MenuID = menuID;
            MenuName = menuName;
            FoodItems = new List<FoodItem>();
        }

        public Menu() 
        {
            FoodItems = new List<FoodItem>();
        }

        public void AddFoodItem(FoodItem item)
        {
            FoodItems.Add(item);
        }

        public void RemoveFoodItem(FoodItem item)
        {
            FoodItems.Remove(item);
        }
        
        public void DisplayFoodItems()
        {
            Console.WriteLine($"Menu: {MenuName} (ID: {MenuID})");
            foreach (var item in FoodItems)
            {
                Console.WriteLine($"- {item.ItemName}: {item.ItemDesc} - {item.ItemPrice}");
            }
        }

        public override string ToString()
        {
            return $"MenuID: {MenuID}, MenuName: {MenuName}, FoodItems Count: {FoodItems.Count}";
        }
    }
}
