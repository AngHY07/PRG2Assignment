
//==========================================================
// Student Number : S10273989D
// Student Name : Ang Hao Yi
// Partner Name : Choo Yi Zehn
//==========================================================

//==========================================================
// Student Number : S10274355B
// Student Name : Choo Yi Zehn
// Partner Name : Ang Hao Yi
//==========================================================

using Microsoft.VisualBasic;
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
            string itemsline = foodInfo[1]; 

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

            restaurantsObj[restaurant.RestaurantId].Order.Enqueue(order);
            
            if (status == "Cancelled" || status == "Rejected")
            {
                restaurantsObj[restaurant.RestaurantId].RefundStack.Push(order);
            }
            foreach (OrderedFoodItem ofi in orderedFoodItems)
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
    Console.WriteLine($"{"Order ID",-9}  {"Customer",-13}  {"Restaurant",-15}  {"Delivery Date/Time",-18}  {"Amount",-7}  {"Status",-9}");
    Console.WriteLine($"{"---------",-9}  {"----------",-13}  {"-------------",-15}  {"------------------",-18}  {"------",-7}  {"---------",-9}");
    
    foreach(int orderid in orderObj.Keys)
    {

        Order order = orderObj[orderid];
        Console.WriteLine($"{order.OrderID,-9}  {order.Customer.CustomerName,-13}  {order.Restaurant.RestaurantName,-15}  {order.DeliveryDateTime.ToString("dd/MM/yyyy H:mm"),-18}  {$"${order.OrderTotal.ToString("F2")}",-7}  {order.OrderStatus,-9}");
    }
}

void ListAllRestaurantsAndMenuItems()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant res in restaurantsObj.Values)
    {
        res.DisplayMenu();
    }
}

void CreateNewOrder()
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    Console.Write("Enter Customer Email: ");
    string cEmail = Console.ReadLine();
    Console.Write("Enter Restaurant ID: ");
    string rID = Console.ReadLine();
    Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
    DateTime dDate = DateTime.Parse(Console.ReadLine());
    Console.Write("Enter Delivery Time (hh:mm): ");
    DateTime dTime = DateTime.Parse(Console.ReadLine());
    Console.Write("Enter Delivery Address: ");
    string dAddress = Console.ReadLine();
    DateTime deliveryDateTime = dDate.Date + dTime.TimeOfDay;

    Console.WriteLine("Available Food Items:");
    Restaurant res = restaurantsObj[rID];
    Order newOrder = new Order(
        customerObj[cEmail],
        res,
        null,
        1001 + orderObj.Count,
        DateTime.Now,
        "Pending",
        deliveryDateTime,
        0,
        dAddress
    );
    for (int i = 0; i < res.Menu[0].FoodItems.Count; i++)
        {
            FoodItem fi = res.Menu[0].FoodItems[i];
            Console.WriteLine($"{i + 1}. {fi.ItemName} - ${fi.ItemPrice:F2}");
        }

     while (true)
     {
         Console.Write("Enter item number (0 to finish) : ");
         int choice = int.Parse(Console.ReadLine());
         if (choice == 0)
         {
            break;
         }
         FoodItem selectedItem = res.Menu[0].FoodItems[choice - 1];
         Console.Write("Enter quantity: ");
         int quantity = int.Parse(Console.ReadLine());
         OrderedFoodItem orderedItem = new OrderedFoodItem(
         selectedItem.ItemName,
         selectedItem.ItemDesc,
         selectedItem.ItemPrice,
         quantity
         );
        newOrder.AddOrderedFoodItem(orderedItem);
    }

    Console.Write("Add special request? [Y/N] : ");
    string specialReqChoice = Console.ReadLine().ToUpper();

    if (specialReqChoice == "Y")
    {
        Console.Write("Enter special request: ");
        string request = Console.ReadLine();
    }

    double totalPayment = newOrder.CalculateOrderTotal();
    Console.WriteLine($"Order Total: ${(totalPayment - 5):F2} + $5.00 (delivery) = ${totalPayment:F2}");
    Console.Write("Proceed to payment? [Y/N] : ");
    string paymentChoice = Console.ReadLine().ToUpper();
    if (paymentChoice == "N")
        return;
    Console.WriteLine("Payment method:");
    Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
    newOrder.OrderPaymentMethod = Console.ReadLine().ToUpper();
    newOrder.OrderPaid = true;

    orderObj.Add(newOrder.OrderID, newOrder);
    res.Order.Enqueue(newOrder);
    Customer cust = customerObj[cEmail];
    cust.Orders.Add(newOrder);

    string itemsLine = string.Join("|", newOrder.OrderedFoodItem.Select(ofi => $"{ofi.ItemName},{ofi.QtyOrdered}"));
    DateTime now = DateTime.Now;
    string formattedDateTime = now.ToString("dd'/'MM'/'yyyy HH:mm");

    using (StreamWriter sw = new StreamWriter("orders.csv", true))
    {
        sw.WriteLine($"{newOrder.OrderID},{cEmail},{rID},{dDate:dd'/'MM'/'yyyy},{dTime:HH:mm},{dAddress},{formattedDateTime},{totalPayment},{newOrder.OrderStatus},\"{itemsLine}\"");
    }

    Console.WriteLine($"Order {newOrder.OrderID} created successfully! Status: Pending");
}



void MainMenu()
{
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
}

RestaurantInit();
CustomerInit();
FoodItemInit();
OrderInit();


while(true)
{
    MainMenu();
    int inputChoice = int.Parse(Console.ReadLine());

    if (inputChoice == 0)
    {
        break;
    } 
    else if (inputChoice == 1)
    {
        ListAllRestaurantsAndMenuItems();
    }
    else if (inputChoice == 2)
    {
        ListAllOrder();
    }
    else if (inputChoice == 3)
    {
        CreateNewOrder();
    }


}    




