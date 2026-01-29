


using S10273989D_PRG2Assignment;

Dictionary<string, Restaurant> restaurantsObj = new Dictionary<string, Restaurant>();

Dictionary<string, FoodItem> foodItemObj = new Dictionary<string, FoodItem>();




Dictionary<string, Menu> menuObj = new Dictionary<string, Menu>();

void RestaurantInit()
{
    using (StreamReader sr = new StreamReader("restaurants.csv"))
    {
        int counter = 0;
        string title = sr.ReadLine();

        while (true)
        {
            string line = sr.ReadLine();

            if (line == null)
            {
                break;
            }


            string[] restaurantInfo = line.Split(',');


            restaurantsObj[restaurantInfo[0]] = new Restaurant(restaurantInfo[0], restaurantInfo[1], restaurantInfo[2]);
            restaurantsObj[restaurantInfo[0]].Menu.Add(new Menu(counter.ToString(), "Main Menu"));
            counter += 1;
        }

        Console.WriteLine($"{counter} restaurants loaded!");
    }
}


void FoodItemInit()
{
    using (StreamReader sr = new StreamReader("fooditems.csv"))
    {

        int counter = 0;
        string title = sr.ReadLine();
        while (true)
        {
            string line = sr.ReadLine();
            if (line == null)
            {
                break;
            }

            string[] foodItemInfo = line.Split(',');

            foodItemObj[foodItemInfo[1]] = new FoodItem(foodItemInfo[1], foodItemInfo[2], double.Parse(foodItemInfo[3]));

            restaurantsObj[foodItemInfo[0]].Menu[0].AddFoodItem(foodItemObj[foodItemInfo[1]]);

            counter += 1;
        }

        Console.WriteLine($"{counter} Food Items loaded!");
    }
}


RestaurantInit();
FoodItemInit();
Console.ReadLine();