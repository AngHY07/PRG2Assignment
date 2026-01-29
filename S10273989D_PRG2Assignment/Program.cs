


using S10273989D_PRG2Assignment;

List<Restaurant> restaurantsObj = new List<Restaurant>();
List <Customer> customerObj = new List<Customer>();
List<Order> orderObj = new List<Order>();


void RestaurantInit()
{
    using (StreamReader sr = new StreamReader("restaurants.csv"))
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
                Order ord = new Order(orderInfo[0], orderInfo[1], orderInfo[2], orderInfo[3], orderInfo[4], orderInfo[5], orderInfo[6], orderInfo[7], orderInfo[8], orderInfo[9]);
                orderObj.Add(ord);
                foreach (Order in orderObj)
                {
                    foreach (Customer cust in customerObj)
                    {
                        if (ord.CustomerEmail == cust.EmailAddress)
                        {
                            cust.AddOrder(ord);
                        }
                    }
                    foreach (Restaurant res in restaurantsObj)
                    {
                        if (ord.RestaurantID == res.RestaurantId)
                        {
                            res.Order.Add(ord);
                        }
                    }
                }

            }
        }
        return orderCount;
    }
}



















































































































