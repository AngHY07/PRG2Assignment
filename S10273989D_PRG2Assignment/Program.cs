
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
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

Stack<Order> Archive = new Stack<Order>();

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

            customer.AddOrder(order);

            restaurant.Order.Enqueue(order);
            if (status == "Delivered")
            {
                Archive.Push(order);
            }
            if (status == "Cancelled" || status == "Rejected")
            {
                restaurant.RefundStack.Push(order);
                Archive.Push(order);
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

void ProcessOrder()
{
    string restaurantID;
    int processCount = 0;
    Queue<Order> orderQueuePop = new Queue<Order>();
    bool quit = false;
    while (true)
    {
        try
        {
            Console.WriteLine("Process Order");
            Console.WriteLine("=============");
            Console.Write("Enter Restaurant ID: ");
            restaurantID = Console.ReadLine();

            if (restaurantsObj[restaurantID].Order.Count == 0)
            {
                Console.WriteLine("No orders to process for this restaurant.\n");
                continue;
            }
            Console.WriteLine();


            foreach (Order ord in restaurantsObj[restaurantID].Order)
            {

                if (quit)
                {
                    break;
                }
                int count = 1;
                Console.WriteLine($"Order {ord.OrderID}:");
                Console.WriteLine($"Customer : {ord.Customer.CustomerName}");
                Console.WriteLine("Ordered Items: ");
                foreach (OrderedFoodItem ofi in ord.OrderedFoodItem)
                {
                    Console.WriteLine($"{count}. {ofi.ItemName} - {ofi.QtyOrdered}");
                    count += 1;
                }
                Console.WriteLine($"Delivery date/time: {ord.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm")}");
                Console.WriteLine($"Total Amount: ${ord.OrderTotal}");
                Console.WriteLine($"Order Status: {ord.OrderStatus}\n");
                while (true)
                {

                    Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver/ [Q] Quit: ");
                    string statusChoice = Console.ReadLine().ToUpper();


                    if (statusChoice == "C")
                    {
                        if (ord.OrderStatus != "Pending")
                        {
                            Console.WriteLine("\nOrder Status is not Pending! Can't Confirm!\n");
                            continue;
                        }
                        ord.OrderStatus = "Preparing";
                        Console.WriteLine($"\nOrder {ord.OrderID} confirmed. Status: {ord.OrderStatus}\n");
                        if (!(ord == restaurantsObj[restaurantID].Order.Last()))
                        {
                            processCount += 1;
                        }

                        orderQueuePop.Enqueue(ord);

                        break;


                    }
                    else if (statusChoice == "R")
                    {
                        if (ord.OrderStatus != "Pending")
                        {
                            Console.WriteLine("\nOrder Status is not Pending! Can't Reject!\n");
                            continue;
                        }
                        ord.OrderStatus = "Rejected";
                        Archive.Push(ord);
                        ord.Restaurant.RefundStack.Push(ord);
                        Console.WriteLine($"\nOrder {ord.OrderID} rejected. Status: Rejected\n");
                        if (!(ord == restaurantsObj[restaurantID].Order.Last()))
                        {
                            processCount += 1;
                        }
                        orderQueuePop.Enqueue(ord);


                        break;
                    }
                    else if (statusChoice == "S")
                    {

                        if (ord.OrderStatus == "Cancelled" || ord.OrderStatus == "Delivered" || ord.OrderStatus == "Rejected")
                        {
                            Console.WriteLine($"\nOrder {ord.OrderID} skipped!\n");
                            if (!(ord == restaurantsObj[restaurantID].Order.Last()))
                            {
                                processCount += 1;
                            }
                            orderQueuePop.Enqueue(ord);


                            break;
                        }
                        else
                        {
                            Console.WriteLine("\nOrder Status is not Cancelled, Delivered or Rejected. Can't Skip!\n");

                            continue;
                        }

                    }
                    else if (statusChoice == "D")
                    {
                        if (ord.OrderStatus != "Preparing")
                        {
                            Console.WriteLine("\nOrder Status is not Preparing! Can't Deiver!\n");
                            continue;
                        }
                        ord.OrderStatus = "Delivered";
                        Archive.Push(ord);
                        Console.WriteLine($"\nOrder {ord.OrderID} Delivered. Status {ord.OrderStatus}\n");
                        if (!(ord == restaurantsObj[restaurantID].Order.Last()))
                        {
                            processCount += 1;
                        }

                        orderQueuePop.Enqueue(ord);

                        break;
                    }
                    else if (statusChoice == "Q")
                    {
                        processCount -= 1;
                        quit = true;
                        break;

                    }
                    else
                    {
                        Console.WriteLine("Invalid Input! Try Again!");
                        continue;
                    }
                }
            }

            for (int i = 0; i <= processCount; i++)
            {
                restaurantsObj[restaurantID].Order.Dequeue();
            }
            foreach (Order ord in orderQueuePop)
            {
                restaurantsObj[restaurantID].Order.Enqueue(ord);
            }
            break;
        }
        catch (KeyNotFoundException)
        {
            Console.WriteLine("Restaurant ID not found. Please retry!\n");
            continue;
        }
    }
}

void ModifyOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");

    Console.Write("Enter Customer Email: ");
    string cEmail = Console.ReadLine();

    if (!customerObj.ContainsKey(cEmail))
    {
        Console.WriteLine("Customer email not found.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    List<int> pendingOrders = new List<int>();

    foreach (Order ord in customerObj[cEmail].Orders)
    {
        if (ord.OrderStatus == "Pending")
        {
            Console.WriteLine(ord.OrderID);
            pendingOrders.Add(ord.OrderID);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found.");
        return;
    }

    Console.Write("Enter Order ID: ");
    if (!int.TryParse(Console.ReadLine(), out int oID) || !pendingOrders.Contains(oID))
    {
        Console.WriteLine("Invalid Order ID.");
        return;
    }

    Order order = orderObj[oID];

    Console.WriteLine("\nOrder Items:");
    for (int i = 0; i < order.OrderedFoodItem.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {order.OrderedFoodItem[i].ItemName} - {order.OrderedFoodItem[i].QtyOrdered}");
    }

    Console.WriteLine($"Address:\n{order.DeliveryAddress}");
    Console.WriteLine($"Delivery Date/Time:\n{order.DeliveryDateTime:dd/MM/yyyy, HH:mm}");

    Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time: ");
    if (!int.TryParse(Console.ReadLine(), out int modifyChoice) || modifyChoice < 1 || modifyChoice > 3)
    {
        Console.WriteLine("Invalid modification option.");
        return;
    }

    double oldTotal = order.OrderTotal;

    if (modifyChoice == 1)
    {
        Console.Write("Enter item number to modify: ");
        if (!int.TryParse(Console.ReadLine(), out int itemNo) ||
            itemNo < 1 || itemNo > order.OrderedFoodItem.Count)
        {
            Console.WriteLine("Invalid item number.");
            return;
        }

        Console.Write("Enter new quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int newQty) || newQty <= 0)
        {
            Console.WriteLine("Quantity must be a positive number.");
            return;
        }

        order.OrderedFoodItem[itemNo - 1].QtyOrdered = newQty;
    }
    else if (modifyChoice == 2)
    {
        Console.Write("Enter new delivery address: ");
        string newAddress = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(newAddress))
        {
            Console.WriteLine("Delivery address cannot be empty.");
            return;
        }

        order.DeliveryAddress = newAddress;
    }
    else if (modifyChoice == 3)
    {
        Console.Write("Enter new delivery time (hh:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime newTime))
        {
            Console.WriteLine("Invalid time format.");
            return;
        }

        order.DeliveryDateTime = order.DeliveryDateTime.Date + newTime.TimeOfDay;
    }

    double newTotal = order.CalculateOrderTotal();

    if (newTotal > oldTotal)
    {
        Console.WriteLine($"Additional payment required: ${(newTotal - oldTotal):F2}");
        Console.Write("Proceed to payment? [Y/N]: ");

        if (Console.ReadLine().ToUpper() != "Y")
        {
            order.OrderTotal = oldTotal;
            Console.WriteLine("Payment cancelled. Changes reverted.");
            return;
        }

        Console.Write("Payment method [CC/PP/CD]: ");
        string method = Console.ReadLine().ToUpper();

        if (method != "CC" && method != "PP" && method != "CD")
        {
            Console.WriteLine("Invalid payment method.");
            return;
        }

        order.OrderPaymentMethod = method;
        order.OrderPaid = true;
    }

    Console.WriteLine("Order updated successfully!");
}
void DeleteExistingOrder()
{
    string cEmail;
    int count = 1;
    int oID;
    int pendingCount = 0;
    List <int> order = new List<int>();

    while (true)
    {

        try
        {
            Console.WriteLine("\nDelete Order");
            Console.WriteLine("=============");
            Console.Write("Enter Customer Email: ");
            cEmail = Console.ReadLine();
            Console.WriteLine("Pending Orders:");


            foreach (Order ord in customerObj[cEmail].Orders)
            {
                if (ord.OrderStatus == "Pending")
                {
                    Console.WriteLine($"{ord.OrderID}");
                    order.Add(ord.OrderID);
                    pendingCount += 1;
                }
            }

            if (pendingCount ==0)
            {
                Console.WriteLine("No pending orders found for this customer.");
                continue;
            }
        }
        catch (KeyNotFoundException)
        {
            Console.WriteLine("Customer Email not found. Please retry!");
            continue;
        }


        while (true)
        {
            try
            {
                Console.Write("Enter Order ID: ");
                oID = int.Parse(Console.ReadLine());

                if (!order.Contains(oID))
                {
                    Console.WriteLine("Order ID not found in pending orders. Please retry!");
                    continue;
                }
                else
                {
                    break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Order ID format. Please enter a valid integer.");
                continue;
            }
        }

      

        Console.WriteLine($"\nCustomer: {customerObj[cEmail].CustomerName}");
        Console.WriteLine("Ordered Items: ");
        foreach (OrderedFoodItem ofi in orderObj[oID].OrderedFoodItem)
        {
            Console.WriteLine($"{count}. {ofi.ItemName} - {ofi.QtyOrdered}");

            count += 1;
        }
        Console.WriteLine($"Delivery date/time: {orderObj[oID].DeliveryDateTime.ToString("dd/MM/yyyy  HH:mm")}");
        Console.WriteLine($"Total Amount: ${orderObj[oID].OrderTotal}");
        Console.WriteLine($"Order Status: {orderObj[oID].OrderStatus}");
                

        while (true)
        {
            Console.Write("Confirm deletion? [Y/N]: ");
            string choice = Console.ReadLine().ToUpper();


            if (choice == "Y")
            {
                orderObj[oID].OrderStatus = "Cancelled";
                Archive.Push(orderObj[oID]);
                orderObj[oID].Restaurant.RefundStack.Push(orderObj[oID]);
                Console.WriteLine($"\nOrder {orderObj[oID].OrderID} cancelled. Refund of ${orderObj[oID].OrderTotal.ToString("F2")} processed.");
                break;
            }
            else if (choice == "N")
            {
                Console.WriteLine("\nDeletion cancelled.");
                break;
            }
            else
            {
                Console.WriteLine("\nInvalid choice. Please retry!");
                        
            }
        }
        break;

    }
}

void BulkProcessUnprocessedOrders()
{

    double orderCount = 0;
    double processedCount = 0;
    double rejectCount = 0;
    double prepareCount = 0;
    double totalorders = 0;
    double individualRCount = 0;
    double individualPCount = 0;


    foreach (string restaurantID in restaurantsObj.Keys)
    {
        int rPrepareCount = 0;
        int rRejectCount = 0;
        int rProcessCount = 0;
        int rTotalOrder = 0;
        int rindividualRCount = 0;
        int rindividualPCount = 0;

        Restaurant rObj = restaurantsObj[restaurantID];
        foreach (Order ord in rObj.Order)
        {
            totalorders += 1;
            rTotalOrder += 1;
            if (ord.OrderStatus == "Preparing")
            {
                prepareCount += 1;
                rPrepareCount += 1;
            }
            else if (ord.OrderStatus == "Rejected")
            {
                rejectCount += 1;
                rRejectCount += 0;
            }

            if (ord.OrderStatus != "Pending")
            {
                continue;
            }

            TimeSpan deliverytime = ord.DeliveryDateTime - ord.OrderDateTime;

            int diffdays = deliverytime.Days;
            int hours = diffdays * 24;
            hours += deliverytime.Hours;

            if (hours < 1)
            {
                ord.OrderStatus = "Rejected";
                ord.Restaurant.RefundStack.Push(ord);
                Archive.Push(ord);
                rindividualRCount += 1;
                individualRCount += 1;
                rRejectCount += 1;
                rejectCount += 1;
                processedCount += 1;
                rProcessCount += 1;
            }
            else
            {
                ord.OrderStatus = "Preparing";
                rindividualPCount += 1;
                individualPCount += 1;
                rPrepareCount += 1;
                prepareCount += 1;
                processedCount += 1;
                rProcessCount += 1;
            }
        }
        Console.WriteLine($"\nSummary Statistics for Restaurant [{rObj.RestaurantId}]:");
        Console.WriteLine($"Number of Order Processed: {rProcessCount}");
        Console.WriteLine($"Number of Pending Status Before Processed: {rProcessCount}");
        Console.WriteLine($"Total Number of orders processed to Preparing status: {rindividualPCount}");
        Console.WriteLine($"Total Number of orders processed to Rejected status: {rindividualRCount}");
        Console.WriteLine($"Number of order with Preparing status: {rProcessCount}");
        Console.WriteLine($"Number of order with Rejected status: {rRejectCount}");
        Console.WriteLine($"Restaurant {rObj.RestaurantId} Total Orders: {rTotalOrder}\n");

    }
    Console.WriteLine("=======================================================================");
    Console.WriteLine("Summary Statistics for all restaurants:");
    Console.WriteLine($"Number of Order Processed: {processedCount}");
    Console.WriteLine($"Number of Pending Status Before Processed: {processedCount}");
    Console.WriteLine($"Total Number of orders processed to Preparing status: {individualPCount}");
    Console.WriteLine($"Total Number of orders processed to Rejected status: {individualRCount}");
    Console.WriteLine($"Total Number of order with Preparing status: {prepareCount}");
    Console.WriteLine($"Total Number of order with Rejected status: {rejectCount}");
    Console.WriteLine($"Percentage of automatically processed orders against all orders: {((processedCount / totalorders) * 100).ToString("F2")}%");
    Console.WriteLine("=======================================================================");

}
void MainMenu()
{
    Console.WriteLine("\n===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("7. Process all unprocessed orders ");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
}

Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
RestaurantInit();
FoodItemInit();
CustomerInit();
OrderInit();


while(true)
{
    try
    {
        MainMenu();
        int inputChoice = int.Parse(Console.ReadLine());

        if (inputChoice == 0)
        {
            using (StreamWriter sw = new StreamWriter("queue.csv", false))
            {
                foreach(int ord in orderObj.Keys)
                {
                    Order ordObj = orderObj[ord];

                    string itemsLine = string.Join("|", ordObj.OrderedFoodItem.Select(ofi => $"{ofi.ItemName},{ofi.QtyOrdered}"));
                    sw.WriteLine($"{ordObj.OrderID},{ordObj.Customer.EmailAddress},{ordObj.Restaurant.RestaurantId},{ordObj.DeliveryDateTime:dd'/'MM'/'yyyy},{ordObj.DeliveryDateTime:HH:mm},{ordObj.DeliveryAddress},{ordObj.OrderDateTime},{ordObj.OrderTotal},{ordObj.OrderStatus},\"{itemsLine}\"");
                    
                }
            }


            using (StreamWriter sw = new StreamWriter("stack.csv", false))
            {
                foreach (string resID in restaurantsObj.Keys)
                {
                    Restaurant resObj = restaurantsObj[resID];
                    foreach (Order ord in resObj.RefundStack)
                    {
                        string itemsLine = string.Join("|", ord.OrderedFoodItem.Select(ofi => $"{ofi.ItemName},{ofi.QtyOrdered}"));
                        sw.WriteLine($"{ord.OrderID},{ord.Customer.EmailAddress},{ord.Restaurant.RestaurantId},{ord.DeliveryDateTime:dd'/'MM'/'yyyy},{ord.DeliveryDateTime:HH:mm},{ord.DeliveryAddress},{ord.OrderDateTime},{ord.OrderTotal},{ord.OrderStatus},\"{itemsLine}\"");
                    }
                }
            }   

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
        else if (inputChoice == 4)
        {
            ProcessOrder();
        }
        else if (inputChoice ==5)
        {
            ModifyOrder();
        }
        else if (inputChoice == 6)
        {
            DeleteExistingOrder();
        }
        else if (inputChoice == 7)
        {

            BulkProcessUnprocessedOrders();
        }
        else
        {
            Console.WriteLine("Invalid Input Choice!");
        }
    }
    catch(FormatException)
    {
        Console.WriteLine("Invalid input format. Please enter an integer choice.");
        continue;
    }
    
}    




