


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























































void CustomerInit()
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
        Console.WriteLine(customerCount + " customers loaded!");
    }
}

void OrderInit()
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
        Console.WriteLine(orderCount + " orders loaded!");
    }
}

void MainMenu()
{
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4.Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
    int choice = Console.ReadLine();
    if (choice == 0)
    {
        ListAllRestaurantsAndMenuItems();
    }
}

void ListAllRestaurantsAndMenuItems()
{
    foreach (Restaurant res in restaurantsObj)
    {
        res.DisplayMenu();
    }
}

















































































































