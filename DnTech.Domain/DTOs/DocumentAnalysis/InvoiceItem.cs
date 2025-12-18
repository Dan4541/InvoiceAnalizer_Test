namespace DnTech.Domain.DTOs.DocumentAnalysis
{
    public class InvoiceItem
    {
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
