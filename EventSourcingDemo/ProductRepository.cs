namespace EventSourcingDemo
{
    public class ProductRepository
    {
        private readonly IDictionary<string, IList<IEvent>> _productStreams;

        public ProductRepository()
        {
            _productStreams = new Dictionary<string, IList<IEvent>>();
        }

        public Product GetProduct(string sku)
        {
            var product = new Product(sku);

            if (_productStreams.ContainsKey(sku))
            {
                foreach (var productEvent in _productStreams[sku])
                {
                    product.AddEvent(productEvent);
                }
            }

            return product;
        }

        public void SaveProduct(Product product)
        {
            _productStreams[product.Sku] = product.GetEvents();
        }

    }
}
