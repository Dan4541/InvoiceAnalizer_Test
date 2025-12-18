namespace DnTech.Domain.DTOs.DocumentAnalysis
{
    public class InvoiceData
    {
        public string? InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public decimal? TotalAmount { get; set; }
        public List<InvoiceItem> Items { get; set; } = new();
    }
}
