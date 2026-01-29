


using S10273989D_PRG2Assignment;

List<Restaurant> restaurantsObj = new List<Restaurant>();


void RestaurantInit()
{
    using (StreamReader sr = new StreamReader("resraurant.csv"))
    {

        string title = sr.ReadLine();

        while (true)
        {
            string line = sr.ReadLine();

            if (line == null)
            {
                break;
            }


            string[] restaurantInfo = line.Split(',');

            


        }
    }
}