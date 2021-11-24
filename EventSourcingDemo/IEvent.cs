namespace EventSourcingDemo
{
    public interface IEvent
    {
        string Sku { get; }
        int Quantity { get; }
        DateTime CreatedDateTime { get; }
    }

    public record ProductShipped(string Sku, int Quantity, DateTime CreatedDateTime) : IEvent;

    public record ProductReceived(string Sku, int Quantity, DateTime CreatedDateTime) : IEvent;

    public record ProductInventoryAdjusted(string Sku, int Quantity, string Reason, DateTime CreatedDateTime) : IEvent;
}
