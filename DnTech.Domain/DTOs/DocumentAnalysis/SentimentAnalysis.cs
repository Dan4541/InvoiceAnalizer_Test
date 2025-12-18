namespace DnTech.Domain.DTOs.DocumentAnalysis
{
    public class SentimentAnalysis
    {
        public string Sentiment { get; set; } = "Neutral"; // Positive, Negative, Neutral
        public double ConfidenceScore { get; set; }
        public string? Details { get; set; }
    }
}
