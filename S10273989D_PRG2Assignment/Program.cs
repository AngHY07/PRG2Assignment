
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
using System;



Stack<Order> Archive = new Stack<Order>();

Dictionary<string, Restaurant> restaurantsObj = new Dictionary<string, Restaurant>();

Dictionary<string, FoodItem> foodItemObj = new Dictionary<string, FoodItem>();

Dictionary<string, Customer> customerObj = new Dictionary<string, Customer>();

Dictionary<int, Order> orderObj = new Dictionary<int, Order>();

Dictionary<string, Menu> menuObj = new Dictionary<string, Menu>();
Dictionary <string,SpecialOffer>  specialOfferObj = new Dictionary<string, SpecialOffer>();
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

void OfferFileInit()
{
    using(StreamReader sr = new StreamReader("orderwithoffer.csv"))
    {
        while(true)
        {
            string line = sr.ReadLine();

            if (line == null)
            {
                break;
            }

            string[] offerinfo = line.Split(',');

            Order ord = orderObj[int.Parse(offerinfo[0])];

            for (int i = 1; i < offerinfo.Length; i++)
            {
                ord.SpecialOffer.Add(specialOfferObj[offerinfo[i]]);
            }
        }
    }
}

void SpecialOfferInit()
{
    using (StreamReader sr = new StreamReader("specialoffers.csv"))
    {
        string title = sr.ReadLine();


        while (true)
        {
            string line = sr.ReadLine();

            if (line == null)
            {
                break;
            }

            string[] specialOfferInfo = line.Split(',');

            string restaurantName = specialOfferInfo[0];
            string offerCode = specialOfferInfo[1];
            string description = specialOfferInfo[2];
            double discount = 0;
             
            if (specialOfferInfo[3] == "-")
            {
                discount = 0;
            }
            else
            {
                discount = double.Parse(specialOfferInfo[3]);
            }

            specialOfferObj[offerCode] = new SpecialOffer(offerCode, description, discount / 100);

            foreach (string resId in restaurantsObj.Keys)
            {
                if (restaurantsObj[resId].RestaurantName == restaurantName)
                {
                    Restaurant res = restaurantsObj[resId];

                    res.SpecialOffer.Add(specialOfferObj[offerCode]);
                }
            }
        }
    }
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
        Console.WriteLine($"{order.OrderID,-9}  {order.Customer.CustomerName,-13}  {order.Restaurant.RestaurantName,-15}  {order.DeliveryDateTime.ToString("dd'/'MM'/'yyyy H:mm"),-18}  {$"${order.OrderTotal.ToString("F2")}",-7}  {order.OrderStatus,-9}");
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
    string cEmail;
    while (true)
    {
        Console.Write("Enter Customer Email [X to Cancel]: ");
        cEmail = Console.ReadLine();

        if (cEmail.ToUpper() == "X")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(cEmail) && customerObj.ContainsKey(cEmail))
            break;

        Console.WriteLine("Invalid customer email.");
    }


    string rID;
    while (true)
    {
        Console.Write("Enter Restaurant ID [X to Cancel]: ");
        rID = Console.ReadLine();

        if (rID.ToUpper() == "X")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(rID) && restaurantsObj.ContainsKey(rID))
            break;

        Console.WriteLine("Invalid restaurant ID.");
    }

    DateTime dDate;
    while (true)
    {
        Console.Write("Enter Delivery Date (dd/mm/yyyy) [X to Cancel]: ");
        string input = Console.ReadLine();

        if (input.ToUpper() == "X")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (DateTime.TryParse(input, out dDate))
            break;

        Console.WriteLine("Invalid date format.");
    }

    DateTime dTime;
    while (true)
    {
        Console.Write("Enter Delivery Time (hh:mm) [X to Cancel]: ");
        string input = Console.ReadLine();

        if (input.ToUpper() == "X")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (DateTime.TryParse(input, out dTime))
            break;

        Console.WriteLine("Invalid delivery time format.");
    }

    string dAddress;
    while (true)
    {
        Console.Write("Enter Delivery Address [X to Cancel]: ");
        dAddress = Console.ReadLine();

        if (dAddress.ToUpper() == "X")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(dAddress))
            break;

        Console.WriteLine("Delivery address cannot be empty.");
    }

    DateTime deliveryDateTime = dDate.Date + dTime.TimeOfDay;
    if (restaurantsObj[rID].EnableOffers.Count > 0)
    {
        Console.WriteLine("\nAvailable Special Offers:");
        foreach (SpecialOffer offer in restaurantsObj[rID].EnableOffers)
        {
            Console.WriteLine($"Code: {offer.OfferCode} - {offer.OfferDesc}");
        }
    }
    Console.WriteLine("\nAvailable Food Items:");
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

    bool itemAdded = false;

    while (true)
    {

        Console.Write("Enter item number (0 to finish) : ");
        if (!int.TryParse(Console.ReadLine(), out int choice))
        {
            Console.WriteLine("Invalid item number.");
            continue;
        }
        if (choice == 0)
        {
            break;
        }
        if (choice < 1 || choice > res.Menu[0].FoodItems.Count)
        {
            Console.WriteLine("Item number out of range.");
            continue;
        }
        FoodItem selectedItem = res.Menu[0].FoodItems[choice - 1];

        Console.Write("Enter quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
        {
            Console.WriteLine("Quantity must be a positive number.");
            continue;
        }
        var bogo = specialOfferObj["BOGO"];

        if (restaurantsObj[rID].EnableOffers.Any(o => o.OfferCode == bogo.OfferCode))
        {


            Console.WriteLine($"\nBOGO (Buy one free one) offer applied! You get one free for each item bought! Item: {selectedItem.ItemName} Current Quantity: {quantity * 2}");
            newOrder.SpecialOffer.Add(bogo);
        }

        while (true)
        {

            Console.Write("Add special request? [Y/N] : ");
            string specialReqChoice = Console.ReadLine().ToUpper();

            if (specialReqChoice == "Y")
            {
                Console.Write("Enter special request: ");
                string specialRequest = Console.ReadLine();
                OrderedFoodItem orderedItem = new OrderedFoodItem(
                    selectedItem.ItemName,
                    selectedItem.ItemDesc,
                    selectedItem.ItemPrice,
                    specialRequest,
                    quantity
                );
                newOrder.AddOrderedFoodItem(orderedItem);
                itemAdded = true;
                Console.WriteLine();
                break;
            }
            if (specialReqChoice == "N")
            {
                OrderedFoodItem orderedItem = new OrderedFoodItem(
                    selectedItem.ItemName,
                    selectedItem.ItemDesc,
                    selectedItem.ItemPrice,
                    quantity
                );
                newOrder.AddOrderedFoodItem(orderedItem);
                itemAdded = true;
                Console.WriteLine();
                break;
            }
            if (specialReqChoice != "Y" && specialReqChoice != "N")
            {
                Console.WriteLine("Invalid choice. Enter Y or N\n");
                continue;
            }
        }
    }



    if (!itemAdded)
    {
        Console.WriteLine("Order must contain at least one item.");
        return;
    }

    bool applicableEarl = true;

    foreach(Order or in customerObj[cEmail].Orders)
    {
        if (or.Restaurant.RestaurantId == rID)
        {
            applicableEarl = false;
        }
    }
    newOrder.ApplicableEarl = applicableEarl;


    DayOfWeek dayofWeek = (DateTime.Now).DayOfWeek;

    bool weekday = true;

    if (dayofWeek == DayOfWeek.Saturday || dayofWeek == DayOfWeek.Sunday)
    {
        weekday = false;
    }

    newOrder.WeekDay = weekday;
        

    double totalPayment = newOrder.CalculateOrderTotal(restaurantsObj[rID].EnableOffers,specialOfferObj);
    
    var deli = specialOfferObj["DELI"];
    var fest = specialOfferObj["FEST"];
    var earl = specialOfferObj["EARL"];
    var week = specialOfferObj["WEEK"];
    var phol = specialOfferObj["PHOL"];



    if (restaurantsObj[rID].EnableOffers.Any(o => o.OfferCode == week.OfferCode) &&  weekday)
    {
        Console.WriteLine("WEEK (Weekday Discount) offer applied! 3 percent discount applied!");
        newOrder.SpecialOffer.Add(week);
    }

    if (restaurantsObj[rID].EnableOffers.Any(o => o.OfferCode == earl.OfferCode) && applicableEarl)
    {
        Console.WriteLine("EARL (Early Bird) offer applied! 5 percent discount applied!");
        newOrder.SpecialOffer.Add(earl);
    }

    if (restaurantsObj[rID].EnableOffers.Any(o => o.OfferCode == fest.OfferCode))
    {
        Console.WriteLine("FEST (Festive Season Discount) offer applied! 8 percent discount applied!");
        newOrder.SpecialOffer.Add(fest);
    }


    if (restaurantsObj[rID].EnableOffers.Any(o => o.OfferCode == phol.OfferCode))
    {
        Console.WriteLine("PHOL (Public Holiday Discount) offer applied! 10 percent discount applied!");
        newOrder.SpecialOffer.Add(phol);
    }

    if (restaurantsObj[rID].EnableOffers.Any(o => o.OfferCode == deli.OfferCode) && newOrder.FreeDelivery)
    {
        Console.WriteLine($"\nDELI (Free Delivery) offer applied! You get free delivery for this order!");
        Console.WriteLine($"\nOrder Total: ${(totalPayment):F2} + $0 (No delivery fee) = ${totalPayment:F2}");
        newOrder.SpecialOffer.Add(deli);
    }
    else
    {
        Console.WriteLine($"\nOrder Total: ${(totalPayment - 5):F2} + $5.00 (delivery) = ${totalPayment:F2}");
    }
    string paymentChoice;
    while (true)
    {
        Console.Write("Proceed to payment? [Y/N]: ");
        paymentChoice = Console.ReadLine().ToUpper();

        if (paymentChoice == "N")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (paymentChoice == "Y")
            break;

        Console.WriteLine("Invalid choice. Please enter Y, N\n");
    }



    string paymentMethod;
    while (true)
    {
        Console.WriteLine("\nPayment Method: ");
        Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery/ [X] Cancel: ");
        paymentMethod = Console.ReadLine().ToUpper();

        if (paymentMethod == "X")
        {
            Console.WriteLine("Order creation cancelled.");
            return;
        }

        if (paymentMethod == "CC" || paymentMethod == "PP" || paymentMethod == "CD")
            break;

        Console.WriteLine("Invalid payment method. Try again.");
    }

    newOrder.OrderPaymentMethod = paymentMethod;
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

    Console.WriteLine($"\nOrder {newOrder.OrderID} created successfully! Status: Pending");
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
            Console.Write("Enter Restaurant ID (X to Cancel): ");
            restaurantID = Console.ReadLine();

            if (restaurantID.ToUpper() == "X")
            {
                Console.WriteLine("Order processing cancelled by user.");
                return;
            }
            if (!restaurantsObj.ContainsKey(restaurantID))
            {
                Console.WriteLine("Restaurant ID not found. Please retry!\n");
                continue;
            }
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
                Console.WriteLine($"Delivery date/time: {ord.DeliveryDateTime.ToString("dd'/'MM'/'yyyy HH:mm")}");
                Console.WriteLine($"Total Amount: ${ord.OrderTotal.ToString("F2")}");
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
                            Console.WriteLine("\nOrder Status is not Preparing! Can't Deliver!\n");
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

    string cEmail;
    while (true)
    {
        Console.Write("Enter Customer Email [X to Cancel]: ");
        cEmail = Console.ReadLine();

        if (cEmail.ToUpper() == "X")
        {
            Console.WriteLine("Modification cancelled.");
            return;
        }

        if (customerObj.ContainsKey(cEmail))
            break;

        Console.WriteLine("Customer email not found.");
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

    int oID;
    while (true)
    {
        Console.Write("Enter Order ID [X to Cancel]: ");
        string input = Console.ReadLine();

        if (input.ToUpper() == "X")
        {
            Console.WriteLine("Modification cancelled.");
            return;
        }
        
        if (int.TryParse(input, out oID) && pendingOrders.Contains(oID))
            break;

        Console.WriteLine("Invalid Order ID.");
    }

    if (orderObj[oID].SpecialOffer.Count > 0)
    {
        Console.WriteLine("\nYou can't modify order that had special offer applied to it.");
        return;
    }

    Order order = orderObj[oID];

    Console.WriteLine("Order Items:");
    for (int i = 0; i < order.OrderedFoodItem.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {order.OrderedFoodItem[i].ItemName} - {order.OrderedFoodItem[i].QtyOrdered}");
    }

    Console.WriteLine($"Address:\n{order.DeliveryAddress}");
    Console.WriteLine($"Delivery Date/Time:\n{order.DeliveryDateTime:dd'/'MM'/'yyyy, HH:mm}");

    int modifyChoice;
    while (true)
    {
        Console.Write("\nModify: [1] Items [2] Address [3] Delivery Time [X to Cancel]: ");
        string input = Console.ReadLine();

        if (input.ToUpper() == "X")
        {
            Console.WriteLine("Modification cancelled.");
            return;
        }

        if (int.TryParse(input, out modifyChoice) && modifyChoice >= 1 && modifyChoice <= 3)
            break;

        Console.WriteLine("Invalid modification option.");
    }

    double oldTotal = order.OrderTotal;

    if (modifyChoice == 1)
    {
        int itemNo;
        while (true)
        {
            Console.Write("Enter item number to modify [X to Cancel]: ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "X")
            {
                Console.WriteLine("Modification cancelled.");
                return;
            }

            if (int.TryParse(input, out itemNo) &&
                itemNo >= 1 && itemNo <= order.OrderedFoodItem.Count)
                break;

            Console.WriteLine("Invalid item number.");
        }

        int newQty;
        
        while (true)
        {
            Console.Write("Enter new quantity [X to Cancel]: ");
            string input = Console.ReadLine();

            
            if (input.ToUpper() == "X")
            {
                Console.WriteLine("Modification cancelled.");
                return;
            }
            else if (int.TryParse(input, out newQty) && newQty > 0)
                break;
            else if (newQty == order.OrderedFoodItem[itemNo -1].QtyOrdered)
            {
                Console.WriteLine("The quantity amount is the same. Retry!");
            }

            Console.WriteLine("Quantity must be a positive number.");
        }

        int oldQty = order.OrderedFoodItem[itemNo - 1].QtyOrdered;

        if (newQty > order.OrderedFoodItem[itemNo - 1].QtyOrdered)
        {
            order.OrderedFoodItem[itemNo - 1].QtyOrdered = newQty;
            double newTotal = order.CalculateOrderTotal(order.Restaurant.EnableOffers, specialOfferObj);
            Console.WriteLine($"Additional payment required: ${(newTotal - oldTotal):F2}");

            string payChoice;
            while (true)
            {
                Console.Write("Proceed to payment? [Y/N]: ");
                payChoice = Console.ReadLine().ToUpper();

                if (payChoice == "N")
                {
                    order.OrderTotal = oldTotal;
                    Console.WriteLine("Payment cancelled. Changes reverted.");
                    order.OrderedFoodItem[itemNo - 1].QtyOrdered = oldQty;
                    order.CalculateOrderTotal(order.Restaurant.EnableOffers, specialOfferObj);
                    return;
                }

                if (payChoice == "Y")
                    break;

                Console.WriteLine("Invalid choice.");
            }

            string method;
            while (true)
            {
                Console.Write("Payment method [CC/PP/CD], X to Cancel: ");
                method = Console.ReadLine().ToUpper();

                if (method == "X")
                {
                    order.OrderTotal = oldTotal;
                    Console.WriteLine("Payment cancelled. Changes reverted.");
                    order.OrderedFoodItem[itemNo - 1].QtyOrdered = oldQty;
                    order.CalculateOrderTotal(order.Restaurant.EnableOffers, specialOfferObj);
                    return;
                }

                if (method == "CC" || method == "PP" || method == "CD")
                    break;

                Console.WriteLine("Invalid payment method.");
            }
            Console.WriteLine($"\nOrder {order.OrderID} updated. {order.OrderedFoodItem[itemNo - 1].ItemName} quantity changed to {newQty}");
            order.OrderPaymentMethod = method;
            order.OrderPaid = true;
        }
        else if (newQty < order.OrderedFoodItem[itemNo -1].QtyOrdered)
        {


            order.OrderedFoodItem[itemNo - 1].QtyOrdered = newQty;

            order.CalculateOrderTotal();

            Order refundobj = new Order(order.Customer, order.Restaurant, null, order.OrderID, order.OrderDateTime, order.OrderStatus, order.DeliveryDateTime, order.OrderTotal, order.DeliveryAddress);

            OrderedFoodItem refundqty = new OrderedFoodItem(order.OrderedFoodItem[itemNo - 1].ItemName, order.OrderedFoodItem[itemNo - 1].ItemDesc, order.OrderedFoodItem[itemNo - 1].ItemPrice, order.OrderedFoodItem[itemNo - 1].QtyOrdered);

            refundqty.QtyOrdered = oldQty - newQty;


            refundobj.OrderedFoodItem.Add(refundqty);
            refundobj.CalculateOrderTotal();
            refundobj.OrderTotal = refundobj.OrderTotal - 5;
            order.Restaurant.RefundStack.Push(refundobj);


            Archive.Push(refundobj);
            Console.WriteLine($"New Quantity of {order.OrderedFoodItem[itemNo - 1].ItemName}: {order.OrderedFoodItem[itemNo - 1].QtyOrdered}");
            Console.WriteLine($"Amount Refunded: ${refundobj.OrderTotal:F2}");
        }


            
    }
    else if (modifyChoice == 2)
    {
        string newAddress;
        while (true)
        {
            Console.Write("Enter new delivery address [X to Cancel]: ");
            newAddress = Console.ReadLine();

            if (newAddress.ToUpper() == "X")
            {
                Console.WriteLine("Modification cancelled.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(newAddress))
                break;

            Console.WriteLine("Delivery address cannot be empty.");
        }

        order.DeliveryAddress = newAddress;
        Console.WriteLine($"\nOrder {order.OrderID} updated. New Delivery Address:\n{newAddress}");
    }
    else if (modifyChoice == 3)
    {
        DateTime newTime;
        while (true)
        {
            Console.Write("Enter new delivery time (hh:mm) [X to Cancel]: ");
            string input = Console.ReadLine();

            if (input.ToUpper() == "X")
            {
                Console.WriteLine("Modification cancelled.");
                return;
            }

            if (DateTime.TryParse(input, out newTime))
                break;

            Console.WriteLine("Invalid time format.");
        }

        order.DeliveryDateTime = order.DeliveryDateTime.Date + newTime.TimeOfDay;
        Console.WriteLine($"\nOrder {order.OrderID} updated. New Delivery Time: {order.DeliveryDateTime:HH:mm}");
    }

    

    
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
            Console.Write("Enter Customer Email (X to Cancel): ");
            cEmail = Console.ReadLine();

            if (cEmail.ToUpper() == "X")
            {
                Console.WriteLine("Order deletion cancelled by user.");
                return;
            }

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
        Console.WriteLine($"Delivery date/time: {orderObj[oID].DeliveryDateTime:dd\\/MM\\/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${orderObj[oID].OrderTotal:F2}");
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

void DisplayTotalOrderAmount()
{
    Console.WriteLine("Total Order Amount Report");
    Console.WriteLine("=========================");

    double grandTotalOrders = 0;
    double grandTotalRefunds = 0;
    double gruberoEarnings = 0;

    foreach (Restaurant rest in restaurantsObj.Values)
    {
        double restaurantOrderTotal = 0;
        double restaurantRefundTotal = 0;

        foreach (Order ord in rest.Order)
        {
            if (ord.OrderStatus == "Delivered")
            {
                gruberoEarnings += (ord.OrderTotal * 0.3);

                restaurantOrderTotal += (ord.OrderTotal - 5);
            }

            
        }

        foreach (Order ord in rest.RefundStack)
        {
            restaurantRefundTotal += ord.OrderTotal;
        }


        Console.WriteLine($"\nRestaurant: {rest.RestaurantName}");
        Console.WriteLine($"Total Order Amount: ${restaurantOrderTotal:F2}");
        Console.WriteLine($"Total Refunds: ${restaurantRefundTotal:F2}");

        grandTotalOrders += restaurantOrderTotal;
        grandTotalRefunds += restaurantRefundTotal;
    }
    Console.WriteLine("\n=========================");
    Console.WriteLine($"Overall Total Orders: ${grandTotalOrders:F2}");
    Console.WriteLine($"Overall Total Refunds: ${grandTotalRefunds:F2}");
    Console.WriteLine($"Final Amount Earned by Gruberoo (30% of each order): ${gruberoEarnings:F2}");
}

void EnablingSpecialOffer()
{
    List<string> resIDs = new List<string>();
    Dictionary<int, SpecialOffer> offerDictRemove = new Dictionary<int, SpecialOffer>();
    Dictionary<int, SpecialOffer> offerDictAdd = new Dictionary<int, SpecialOffer>();


    foreach (Restaurant res in restaurantsObj.Values)
    {
        resIDs.Add(res.RestaurantId);
    }


    while (true)
    {
        int count = 1;
        int c = 1;
        int removeChoice;
        Console.WriteLine("\n=======================================================================================================");
        Console.WriteLine("                            Enable / Remove Special Offer for Restaurant");
        Console.WriteLine("=======================================================================================================");

        Console.Write("\nRestaurants ID [X to exit]: ");
        string res = Console.ReadLine();
        

        if (res.ToUpper() == "X")
        {
            break;
        }

        if (!resIDs.Contains(res))
        {
            Console.WriteLine("Restaurant ID not found. Please retry!\n");
            continue;
        }

        Console.WriteLine($"\n[OfferCodes of Restaurant {restaurantsObj[res].RestaurantName}, {restaurantsObj[res].RestaurantId}]");
        foreach (SpecialOffer sp in restaurantsObj[res].SpecialOffer)
        {
            Console.WriteLine($"* {sp.OfferCode} - {sp.OfferDesc}");
        }
        Console.WriteLine();



        while (true)
        {


            Console.WriteLine($"\n[Enabled OfferCodes ({restaurantsObj[res].RestaurantId})]");
            if (restaurantsObj[res].EnableOffers.Count == 0)
            {
                Console.WriteLine("No enabled offers!");
            }
            foreach (SpecialOffer sp in restaurantsObj[res].EnableOffers)
            {
                Console.WriteLine($"* {sp.OfferCode} - {sp.OfferDesc}");
            }

            Console.WriteLine("\n===== Enable / Disable Offers =====");
            Console.WriteLine("1. Remove Enabled Offers ");
            Console.WriteLine("2. Enable Offers ");
            Console.WriteLine("0. Exit ");
            Console.Write("Enter your choice: ");
            try
            {
                int choice = int.Parse(Console.ReadLine());
                

                if (choice == 0)
                {
                    break;
                }
                else if (choice == 1)
                {
                    if (restaurantsObj[res].EnableOffers.Count == 0)
                    {
                        Console.WriteLine("\nNo enabled offers to remove.\n");
                        continue;
                    }

                    Console.WriteLine("\nSelect offer to remove:");
                    foreach (SpecialOffer sp in restaurantsObj[res].EnableOffers)
                    {
                        offerDictRemove[count] = sp;

                        Console.WriteLine($"[{count}] {sp.OfferCode} - {sp.OfferDesc}");
                        count += 1;
                    }
                    while (true)
                    {
                        Console.Write("Enter the number of the offer to remove [0 : Exit]: ");
                        try
                        {
                            removeChoice = int.Parse(Console.ReadLine());

                            if (removeChoice < 0 || removeChoice > count)
                            {
                                Console.WriteLine("Invalid choice. Please retry!\n");
                                continue;
                            }
                            else if (removeChoice == 0)
                            {
                                offerDictRemove.Clear();
                                count = 1;
                                break;
                                
                            }

                            Console.WriteLine($"Offer {offerDictRemove[removeChoice].OfferCode} - {offerDictRemove[removeChoice].OfferDesc} has been removed!");
                            restaurantsObj[res].EnableOffers.Remove(offerDictRemove[removeChoice]);
                            offerDictRemove.Clear();
                            count = 1;
                            break;

                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input format. Please enter a valid number.\n");
                            continue;
                        }
                    }
                }
                else if (choice == 2)
                {
                    if (restaurantsObj[res].SpecialOffer.Count == 0)
                    {
                        Console.WriteLine("No Offers to Enable!");
                        continue;
                    }
                    int offerChoice;
                    Console.WriteLine("\nSelect offer to add: ");
                    foreach(SpecialOffer sp in restaurantsObj[res].SpecialOffer)
                    {
                        offerDictAdd[c] = sp;
                        Console.WriteLine($"[{c}] {sp.OfferCode} - {sp.OfferDesc}");
                        c += 1;
                    }

                    Console.WriteLine();
                    while (true)
                    {
                        try
                        {
                            Console.Write("Enter the number of the offer to add [0 : exit]: ");
                            offerChoice = int.Parse(Console.ReadLine());

                            if(offerChoice < 0  || offerChoice > c)
                            {
                                Console.WriteLine("Invalid choice. Please retry!\n");
                                continue;
                            }
                            else if (offerChoice == 0)
                            {
                                break;
                            }
                            else if (restaurantsObj[res].EnableOffers.Contains(offerDictAdd[offerChoice]))
                            {
                                Console.WriteLine("Offer already enabled. Please retry!\n");
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Invalid input format. Please enter a valid number.\n");
                            continue;
                        }
                    }

                    if (offerChoice == 0)
                    {
                        Console.WriteLine("Operation cancelled.\n");
                        offerDictAdd.Clear();
                        c = 1;
                        continue;
                    }

                    Console.WriteLine($"Offer {offerDictAdd[offerChoice].OfferCode} - {offerDictAdd[offerChoice].OfferDesc} has been enabled!");
                    restaurantsObj[res].EnableOffers.Add(offerDictAdd[offerChoice]);
                    c = 1;
                    offerDictAdd.Clear();

                }
                else
                {
                    Console.WriteLine("Invalid choice. Please retry!\n");
                    continue;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Format. Enter a value. Please Retry!\n");
                continue;
            }
        }
    }
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
    Console.WriteLine("8. Display total order amount ");
    Console.WriteLine("9. Enabling Special Offer (Restaurant)");
    
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
}


double gruberoEarnings = 0;
Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
RestaurantInit();
FoodItemInit();
CustomerInit();
OrderInit();
SpecialOfferInit();
OfferFileInit();


while (true)
{
    int inputChoice;
    MainMenu();
    try
    {
        inputChoice = int.Parse(Console.ReadLine());
    }
    catch (FormatException)
    {
        Console.WriteLine("Invalid Input Choice! Please enter a integer.");
        continue;
    }

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

        //Bonus
        using(StreamWriter sw = new StreamWriter("orderwithoffer.csv",false))
        {
            foreach(int ord in orderObj.Keys)
            {
                if (orderObj[ord].SpecialOffer.Count >0)
                {
                    List<string> codes = new List<string>();

                    foreach(SpecialOffer specialoffer in  orderObj[ord].SpecialOffer)
                    {
                        codes.Add(specialoffer.OfferCode);
                    }

                    sw.Write($"{orderObj[ord].OrderID}");
                    foreach(string code in codes)
                    {
                        sw.Write($",{code}");
                    }
                    sw.WriteLine();
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
    else if (inputChoice == 8)
    {

        DisplayTotalOrderAmount();
    }
    else if (inputChoice == 9)
    {
        EnablingSpecialOffer();
    }
    else
    {
        Console.WriteLine("Invalid Input Choice!");
    }
  
    
}    