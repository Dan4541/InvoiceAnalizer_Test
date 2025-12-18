namespace DnTech.Domain.DTOs.DocumentAnalysis
{
    public class GeneralInformation
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Summary { get; set; }
        public Dictionary<string, string> KeyValuePairs { get; set; } = new();
    }
}
