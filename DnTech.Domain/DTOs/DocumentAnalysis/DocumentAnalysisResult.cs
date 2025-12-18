namespace DnTech.Domain.DTOs.DocumentAnalysis
{
    public class DocumentAnalysisResult
    {
        public string DocumentType { get; set; } = string.Empty; // Factura, Información General
        public Dictionary<string, string> ExtractedData { get; set; } = new();
        public string FullText { get; set; } = string.Empty;
        public SentimentAnalysis? Sentiment { get; set; }
        public List<string> Errors { get; set; } = new();
        public bool IsSuccessful { get; set; }

        // Para facturas
        public InvoiceData? Invoice { get; set; }

        // Para información general
        public GeneralInformation? GeneralInfo { get; set; }
    }
}
