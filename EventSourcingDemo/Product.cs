namespace EventSourcingDemo
{
    public class CurrentState
    {
        public int QuantityAvailable { get; set; }
    }

    public class ProductStats
    {
        public int TotalQuantityReceived { get; set; }
        public int TotalQuantityShipped { get; set; }
        public int TotalQuantityAdjusted { get; set; }
        public DateTime LastReceived { get; set; }
        public DateTime LastShipped { get; set; }
        public DateTime LastAdjusted { get; set; }
    }

    public class Product
    {
        public string Sku { get; private set; }
        private IList<IEvent> _events;
        private CurrentState _currentState;
        private ProductStats _productStats;

        public Product(string sku)
        {
            Sku = sku;
            _events = new List<IEvent>();
            _currentState = new CurrentState();  // Projection (Current State)
            _productStats = new ProductStats();  // Projection (Current State)
        }

        public IList<IEvent> GetEvents() => _events;

        public void ShipProduct(int quantity)
        {
            if (quantity > _currentState.QuantityAvailable)
            {
                throw new InvalidDataException("We don't have available quantity to ship");
            }

            AddEvent(new ProductShipped(Sku, quantity, DateTime.Now));
        }

        public void ReceiveProduct(int quantity)
        {
            AddEvent(new ProductReceived(Sku, quantity, DateTime.Now));
        }

        public void AdjustProductInventory(int quantity, string reason)
        {
            if (quantity + _currentState.QuantityAvailable < 0)
            {
                throw new InvalidDataException("We can't adjust to negative quantity");
            }

            AddEvent(new ProductInventoryAdjusted(Sku, quantity, reason, DateTime.Now));
        }

        public void AddEvent(IEvent productEvent)
        {
            switch (productEvent)
            {
                case ProductShipped shipProduct:
                    Apply(shipProduct);
                    break;
                case ProductReceived receiveProduct:
                    Apply(receiveProduct);
                    break;
                case ProductInventoryAdjusted adjustProductInventory:
                    Apply(adjustProductInventory);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported Product event");
            }
        }

        public int GetAvailableQuantity() => _currentState.QuantityAvailable;

        public string GetProductStatistics()
        {
            var lastReceivedDate = "Not received yet";
            var lastShippedDate = "Not shipped yet";
            var lastAdjustedDate = "Not adjusted yet";

            if (_productStats.LastReceived != DateTime.MinValue)
                lastReceivedDate = _productStats.LastReceived.ToString();

            if (_productStats.LastShipped != DateTime.MinValue)
                lastShippedDate = _productStats.LastShipped.ToString();

            if (_productStats.LastAdjusted != DateTime.MinValue)
                lastAdjustedDate = _productStats.LastAdjusted.ToString();

            return $"----------------------------------------------------------{Environment.NewLine}Product SKU = {Sku}{Environment.NewLine}TotalQuantityReceived = {_productStats.TotalQuantityReceived}{Environment.NewLine}TotalQuantityShipped = {_productStats.TotalQuantityShipped}{Environment.NewLine}TotalQuantityAdjusted = {_productStats.TotalQuantityAdjusted}{Environment.NewLine}Last Product Received On = {lastReceivedDate}{Environment.NewLine}Last Product Shipped On = {lastShippedDate}{Environment.NewLine}Last Product Inventory Adjusted On = {lastAdjustedDate}{Environment.NewLine}----------------------------------------------------------";
        }

        private void Apply(ProductShipped productEvent)
        {
            _events.Add(productEvent);
            _currentState.QuantityAvailable -= productEvent.Quantity;
            _productStats.TotalQuantityShipped += productEvent.Quantity;
            _productStats.LastShipped = productEvent.CreatedDateTime;
        }

        private void Apply(ProductReceived productEvent)
        {
            _events.Add(productEvent);
            _currentState.QuantityAvailable += productEvent.Quantity;
            _productStats.TotalQuantityReceived += productEvent.Quantity;
            _productStats.LastReceived = productEvent.CreatedDateTime;
        }

        private void Apply(ProductInventoryAdjusted productEvent)
        {
            _events.Add(productEvent);
            _currentState.QuantityAvailable += productEvent.Quantity;
            _productStats.TotalQuantityAdjusted += productEvent.Quantity;
            _productStats.LastAdjusted = productEvent.CreatedDateTime;
        }
    }
}