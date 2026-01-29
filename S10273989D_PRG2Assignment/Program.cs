


using S10273989D_PRG2Assignment;

List<Restaurant> restaurantsObj = new List<Restaurant>();
List <Customer> customerObj = new List<Customer>();


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























































































































int CustomerInit()
{
    int customerCount = 0;
    using (StreamReader sr = new StreamReader("customers.csv"))
    {
        string title = sr.ReadLine();
        while (true)
        {
            string line = sr.ReadLine();
            if (line == null)
            {
                break;
            }
            else
            {
                customerCount++;
                string[] customerInfo = line.Split(',');
                Customer cust = new Customer(customerInfo[0], customerInfo[1]);
                customerObj.Add(cust);
            }
        }
        return customerCount;
    }
}

int OrderInit()
{
    int orderCount = 0;
    using (StreamReader sr = new StreamReader("orders.csv"))
    {
        string title = sr.ReadLine();
        while (true)
        {
            string line = sr.ReadLine();
            if (line == null)
            {
                break;
            }
            else
            {
                orderCount++;
                string[] orderInfo = line.Split(',');


            }
        }
        return orderCount;
    }
}