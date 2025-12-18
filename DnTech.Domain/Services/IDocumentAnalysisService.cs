using DnTech.Domain.DTOs.DocumentAnalysis;

namespace DnTech.Domain.Services
{
    public interface IDocumentAnalysisService
    {
        Task<DocumentAnalysisResult> AnalyzeDocumentAsync(
        Stream documentStream,
        string fileName,
        CancellationToken cancellationToken = default);
    }
}
