using Azure;
using Azure.AI.DocumentIntelligence;
using DnTech.Domain.DTOs.DocumentAnalysis;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace DnTech.Infrastructure.AI
{
    public class AzureDocumentIntelligenceService : IDocumentAnalysisService
    {
        private readonly DocumentIntelligenceClient _client;
        private readonly ILogger<AzureDocumentIntelligenceService> _logger;

        public AzureDocumentIntelligenceService(
            IConfiguration configuration,
            ILogger<AzureDocumentIntelligenceService> logger)
        {
            var endpoint = configuration["AzureDocumentIntelligence:Endpoint"];
            var apiKey = configuration["AzureDocumentIntelligence:ApiKey"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException(
                    "Azure Document Intelligence no está configurado correctamente. " +
                    "Verifica Endpoint y ApiKey en appsettings.json");
            }

            _client = new DocumentIntelligenceClient(
                new Uri(endpoint),
                new AzureKeyCredential(apiKey));

            _logger = logger;
        }

        public async Task<DocumentAnalysisResult> AnalyzeDocumentAsync(
        Stream documentStream,
        string fileName,
        CancellationToken cancellationToken = default)
        {
            var result = new DocumentAnalysisResult();

            try
            {
                _logger.LogInformation("Iniciando análisis de documento: {FileName}", fileName);

                // Determinar el tipo de análisis según la extensión
                var extension = Path.GetExtension(fileName).ToLower();

                // Reset stream position
                documentStream.Position = 0;

                // Convertir Stream a BinaryData
                var binaryData = await BinaryData.FromStreamAsync(documentStream, cancellationToken);

                // Analizar con modelo prebuilt-layout para extraer todo el contenido
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    "prebuilt-layout",
                    binaryData,
                    cancellationToken: cancellationToken);

                var analyzeResult = operation.Value;

                // Extraer texto completo
                if (analyzeResult.Content != null)
                {
                    result.FullText = analyzeResult.Content;
                }

                // Extraer pares clave-valor
                if (analyzeResult.KeyValuePairs != null)
                {
                    foreach (var kvp in analyzeResult.KeyValuePairs)
                    {
                        var key = kvp.Key?.Content ?? "Unknown";
                        var value = kvp.Value?.Content ?? string.Empty;
                        result.ExtractedData[key] = value;
                    }
                }

                // Intentar clasificar el documento
                result.DocumentType = ClassifyDocument(result.FullText, result.ExtractedData);

                // Si es una factura, intentar análisis específico
                if (result.DocumentType.Contains("Factura", StringComparison.OrdinalIgnoreCase))
                {
                    await AnalyzeInvoiceAsync(documentStream, result, cancellationToken);
                }
                else
                {
                    // Para información general
                    result.GeneralInfo = new GeneralInformation
                    {
                        Title = ExtractTitle(result.FullText),
                        Description = result.FullText.Length > 500
                            ? result.FullText.Substring(0, 500) + "..."
                            : result.FullText,
                        Summary = GenerateSummary(result.FullText),
                        KeyValuePairs = result.ExtractedData
                    };
                }

                // Análisis de sentimiento básico (puedes mejorarlo con Azure Text Analytics)
                result.Sentiment = AnalyzeSentiment(result.FullText);

                result.IsSuccessful = true;
                _logger.LogInformation("Análisis completado exitosamente para: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al analizar documento: {FileName}", fileName);
                result.IsSuccessful = false;
                result.Errors.Add($"Error en el análisis: {ex.Message}");
            }

            return result;
        }

        private async Task AnalyzeInvoiceAsync(
        Stream documentStream,
        DocumentAnalysisResult result,
        CancellationToken cancellationToken)
        {
            try
            {
                // Reset stream position
                documentStream.Position = 0;

                // Convertir Stream a BinaryData
                var binaryData = await BinaryData.FromStreamAsync(documentStream, cancellationToken);

                // Usar modelo prebuilt-invoice
                var operation = await _client.AnalyzeDocumentAsync(
                    WaitUntil.Completed,
                    "prebuilt-invoice",
                    binaryData,
                    cancellationToken: cancellationToken);

                var invoiceResult = operation.Value;

                if (invoiceResult.Documents?.Count > 0)
                {
                    var invoice = invoiceResult.Documents[0];
                    var invoiceData = new InvoiceData();

                    // Extraer campos de la factura
                    if (invoice.Fields.TryGetValue("InvoiceId", out var invoiceId))
                        invoiceData.InvoiceNumber = invoiceId.ValueString;

                    if (invoice.Fields.TryGetValue("InvoiceDate", out var invoiceDate) && invoiceDate.ValueDate.HasValue)
                        invoiceData.InvoiceDate = invoiceDate.ValueDate.Value.DateTime;

                    if (invoice.Fields.TryGetValue("CustomerName", out var customerName))
                        invoiceData.CustomerName = customerName.ValueString;

                    if (invoice.Fields.TryGetValue("CustomerAddress", out var customerAddress))
                        invoiceData.CustomerAddress = customerAddress.ValueString;

                    if (invoice.Fields.TryGetValue("InvoiceTotal", out var totalInvoice) && totalInvoice.ValueDouble != null)
                    {
                        try
                        {
                            var currency = totalInvoice.ValueCurrency;
                            invoiceData.TotalAmount = (decimal)totalInvoice.ValueDouble.Value;
                        }
                        catch (InvalidOperationException)
                        {
                            invoiceData.TotalAmount = 0;
                        }
                    }
                    else
                    {
                        invoiceData.TotalAmount = 0;
                    }

                    // Extraer items
                    if (invoice.Fields.TryGetValue("Items", out var items) && items.ValueList != null)
                    {
                        foreach (var item in items.ValueList)
                        {
                            if (item.ValueDictionary == null) continue;

                            var invoiceItem = new InvoiceItem();

                            if (item.ValueDictionary.TryGetValue("Description", out var desc))
                                invoiceItem.Description = desc.ValueString;

                            if (item.ValueDictionary.TryGetValue("Quantity", out var quantityField) && quantityField.ValueDouble != null)
                                invoiceItem.Quantity = (int)quantityField.ValueDouble.Value;

                            if (item.ValueDictionary.TryGetValue("UnitPrice", out var unitPrice) && unitPrice.ValueDouble != null)
                                invoiceItem.UnitPrice = (decimal)unitPrice.ValueDouble.Value;

                            if (item.ValueDictionary.TryGetValue("Amount", out var amount) &&
                                amount.ValueDouble != null)
                            {
                                invoiceItem.TotalPrice = (decimal)amount.ValueDouble.Value;
                            }

                            invoiceData.Items.Add(invoiceItem);
                        }
                    }
                    result.Invoice = invoiceData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo analizar como factura, continuando con análisis general");
            }
        }

        private string ClassifyDocument(string fullText, Dictionary<string, string> keyValuePairs)
        {
            // Lógica simple de clasificación basada en palabras clave
            var lowerText = fullText.ToLower();

            if (lowerText.Contains("factura") || lowerText.Contains("invoice") ||
                lowerText.Contains("total") || lowerText.Contains("subtotal"))
            {
                return "Factura";
            }

            return "Información General";
        }

        private string ExtractTitle(string text)
        {
            // Extraer las primeras líneas como título
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            return lines.Length > 0 ? lines[0].Trim() : "Sin título";
        }

        private string GenerateSummary(string text)
        {
            // Resumen simple - primeras 200 palabras
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var summaryWords = words.Take(200);
            return string.Join(" ", summaryWords) + (words.Length > 200 ? "..." : "");
        }

        private SentimentAnalysis AnalyzeSentiment(string text)
        {
            // Análisis de sentimiento básico (puedes integrarlo con Azure Text Analytics)
            var positiveWords = new[] { "excelente", "bueno", "positivo", "feliz", "satisfecho" };
            var negativeWords = new[] { "malo", "negativo", "problema", "error", "insatisfecho" };

            var lowerText = text.ToLower();
            var positiveCount = positiveWords.Count(w => lowerText.Contains(w));
            var negativeCount = negativeWords.Count(w => lowerText.Contains(w));

            var sentiment = positiveCount > negativeCount ? "Positive" :
                           negativeCount > positiveCount ? "Negative" : "Neutral";

            var confidence = positiveCount + negativeCount > 0
                ? Math.Max(positiveCount, negativeCount) / (double)(positiveCount + negativeCount)
                : 0.5;

            return new SentimentAnalysis
            {
                Sentiment = sentiment,
                ConfidenceScore = confidence,
                Details = $"Palabras positivas: {positiveCount}, Palabras negativas: {negativeCount}"
            };
        }

        //private string GetContentType(string extension)
        //{
        //    return extension switch
        //    {
        //        ".pdf" => "application/pdf",
        //        ".jpg" or ".jpeg" => "image/jpeg",
        //        ".png" => "image/png",
        //        _ => "application/octet-stream"
        //    };
        //}

    }
}
