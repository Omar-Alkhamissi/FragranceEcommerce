namespace FragEcom.Helpers
{
    public class OrderDetailsHelper
    {
        public int OrderId { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int QtyOrdered { get; set; }
        public int QtySold { get; set; }
        public int QtyBackOrdered { get; set; }
        public decimal SellingPrice { get; set; }
        public int CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
