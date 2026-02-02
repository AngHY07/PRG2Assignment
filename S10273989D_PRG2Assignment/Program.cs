


using S10273989D_PRG2Assignment;

Dictionary<string, Restaurant> restaurantsObj = new Dictionary<string, Restaurant>();

Dictionary<string, FoodItem> foodItemObj = new Dictionary<string, FoodItem>();

Dictionary<string, Customer> customerObj = new Dictionary<string, Customer>();

Dictionary<int, Order> orderObj = new Dictionary<int, Order>();

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

        Console.WriteLine($"{counter} food items loaded!");
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

            string[] customerInfo = line.Split(',');
            string name = customerInfo[0].Trim();
            string email = customerInfo[1].Trim();

            customerObj[email] = new Customer(name, email);

            customerCount++;
        }
        Console.WriteLine(customerCount + " customers loaded!");
    }
}

void OrderInit()
{
    List<OrderedFoodItem> orderedFoodItems = new List<OrderedFoodItem>();

    
    int orderCount = 0;

    using (StreamReader sr = new StreamReader("orders.csv"))
    {
        string title = sr.ReadLine(); // skip header

        while (true)
        {

            orderedFoodItems.Clear();

            string line = sr.ReadLine();
            if (line == null)
                break;

            string[] orderInfo = line.Split(',');
            string[] foodInfo = line.Split("\"");
            int orderId = int.Parse(orderInfo[0]);
            string customerEmail = orderInfo[1];
            string restaurantId = orderInfo[2];

            DateTime deliveryDate = DateTime.Parse(orderInfo[3]);
            TimeSpan deliveryTime = TimeSpan.Parse(orderInfo[4]);
            string deliveryAddress = orderInfo[5];

            DateTime createdDateTime = DateTime.Parse(orderInfo[6]);
            double totalAmount = double.Parse(orderInfo[7]);
            string status = orderInfo[8];
            string itemsline = foodInfo[1]; // not used yet

            string[] items = itemsline.Split("|");


            foreach (string item in items)
            {
                string[] individualItem = item.Split(",");
                string individualItemName = individualItem[0];
                int qtyordered = int.Parse(individualItem[1]);
                orderedFoodItems.Add(new OrderedFoodItem(individualItem[0],
                    foodItemObj[individualItemName].ItemDesc,
                    foodItemObj[individualItemName].ItemPrice,
                    qtyordered));

            }

            if (!customerObj.TryGetValue(customerEmail, out Customer customer))
            {
                Console.WriteLine($"Order {orderId} skipped — customer not found: {customerEmail}");
                continue;
            }

            if (!restaurantsObj.TryGetValue(restaurantId, out Restaurant restaurant))
            {
                Console.WriteLine($"Order {orderId} skipped — restaurant not found: {restaurantId}");
                continue;
            }
            SpecialOffer specialOffer = null;

            Order order = new Order(
                customer,
                restaurant,
                specialOffer,
                orderId,
                createdDateTime,
                status, 
                deliveryDate.Add(deliveryTime),
                totalAmount,
                deliveryAddress
            );
            foreach(OrderedFoodItem ofi in orderedFoodItems)
            {
                order.AddOrderedFoodItem(ofi);
            }
            orderObj[orderId] = order;
            orderCount++;
        }
    }

    Console.WriteLine($"{orderCount} orders loaded!");
}

void ListAllOrder()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine($"{"Order ID",-9}  {"Customer",-13}  {"Restaurant",-15}  {"Delivery Date/Time",-18}  {"Amount",-6}  {"Status",-9}");
    Console.WriteLine($"{"---------",-9}  {"----------",-13}  {"-------------",-15}  {"------------------",-18}  {"------",-6}  {"---------",-9}");
    
    foreach(int orderid in orderObj.Keys)
    {

        Order order = orderObj[orderid];
        Console.WriteLine($"{order.OrderID,-9}  {order.Customer.CustomerName,-13}  {order.Restaurant.RestaurantName,-15}  {order.DeliveryDateTime.ToString("dd/MM/yyyy H:mm"),-18}  {$"${order.OrderTotal.ToString("F2")}",-6}  {order.OrderStatus,-9}");
    }

}

RestaurantInit();
CustomerInit();
FoodItemInit();
OrderInit();
ListAllOrder();

Console.ReadLine();

//void MainMenu()
//{
//    Console.WriteLine("===== Gruberoo Food Delivery System =====");
//    Console.WriteLine("1. List all restaurants and menu items");
//    Console.WriteLine("2. List all orders");
//    Console.WriteLine("3. Create a new order");
//    Console.WriteLine("4.Process an order");
//    Console.WriteLine("5. Modify an existing order");
//    Console.WriteLine("6. Delete an existing order");
//    Console.WriteLine("0. Exit");
//    Console.Write("Enter your choice: ");}


//void ListAllRestaurantsAndMenuItems()
//{
//    foreach (Restaurant res in restaurantsObj)
//    {
//        res.DisplayMenu();
//    }
//