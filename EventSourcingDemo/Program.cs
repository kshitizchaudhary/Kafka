// See https://aka.ms/new-console-template for more information
using EventSourcingDemo;

Console.WriteLine("Hello, World to Event Sourcing Demo!");
var productRepository = new ProductRepository();

var key = string.Empty;
string quantityStr;
int? quantity;

while (key != "X")
{
    Console.WriteLine("R: Receive Product");
    Console.WriteLine("S: Ship Product");
    Console.WriteLine("A: Adjust Product Inventory");
    Console.WriteLine("E: Product Events");
    Console.WriteLine("Q: Available Product Quantity");
    Console.WriteLine("PS: Product Statistics");
    Console.WriteLine("> ");
    key = Console.ReadLine()?.ToUpperInvariant();
    Console.WriteLine();

    Console.Write("Enter Product SKU: ");
    var sku = Console.ReadLine()?.ToUpperInvariant();
    Console.WriteLine();

    var product = productRepository.GetProduct(sku);

    switch (key)
    {
        case "R":
            Console.Write("Enter quantity = ");
            quantityStr = Console.ReadLine()?.ToUpperInvariant();
            quantity = quantityStr.ConvertToInt();

            if (!quantity.HasValue)
                Console.WriteLine("Invalid quantity");

            product.ReceiveProduct(quantity.Value);
            productRepository.SaveProduct(product);
            Console.WriteLine($"Product {sku} - {quantity.Value} quantity received.");
            break;

        case "S":
            Console.Write("Enter quantity = ");
            quantityStr = Console.ReadLine()?.ToUpperInvariant();
            quantity = quantityStr.ConvertToInt();

            if (!quantity.HasValue)
                Console.WriteLine("Invalid quantity");

            product.ShipProduct(quantity.Value);
            productRepository.SaveProduct(product);
            Console.WriteLine($"Product {sku} - {quantity.Value} quantity shipped.");
            break;

        case "A":
            Console.Write("Enter quantity = ");
            quantityStr = Console.ReadLine()?.ToUpperInvariant();
            quantity = quantityStr.ConvertToInt();

            Console.Write("Enter reason = ");
            var reason = Console.ReadLine();

            product.AdjustProductInventory(quantity.Value, reason);
            productRepository.SaveProduct(product);
            Console.WriteLine($"Product {sku} - {quantity.Value} quantity adjusted.");
            break;

        case "Q":
            Console.WriteLine($"Product {sku} Available quantity {product.GetAvailableQuantity()}");
            break;

        case "PS":
            Console.WriteLine(product.GetProductStatistics());
            break;

        case "E":
            Console.WriteLine($"Events for product {sku}");
            foreach (var evnt in product.GetEvents())
            {
                switch (evnt)
                {
                    case ProductShipped shipProduct:
                        Console.WriteLine($"{shipProduct.CreatedDateTime:u} Product {sku} Shipped : {shipProduct.Quantity}");
                        break;
                    case ProductReceived receiveProduct:
                        Console.WriteLine($"{receiveProduct.CreatedDateTime:u} Product {sku} Received : {receiveProduct.Quantity}");
                        break;
                    case ProductInventoryAdjusted adjustProductInventory:
                        Console.WriteLine($"{adjustProductInventory.CreatedDateTime:u} Product {sku} Adjusted : {adjustProductInventory.Quantity} Reason: {adjustProductInventory.Reason}");
                        break;
                }
            }
            break;

        default:
            break;
    }

    Console.WriteLine();
    Console.WriteLine();
}