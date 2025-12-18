namespace DnTech.Domain.Entities
{
    public class FileValidation
    {
        public Guid Id { get; private set; }
        public Guid FileUploadId { get; private set; }
        public int RowNumber { get; private set; }
        public string ColumnName { get; private set; }
        public string ErrorMessage { get; private set; }
        public ValidationSeverity Severity { get; private set; }
        public string? InvalidValue { get; private set; }
        public DateTime DetectedAt { get; private set; }

        private FileValidation() { }

        public static FileValidation Create(
            Guid fileUploadId,
            int rowNumber,
            string columnName,
            string errorMessage,
            ValidationSeverity severity,
            string? invalidValue = null)
        {
            return new FileValidation
            {
                Id = Guid.NewGuid(),
                FileUploadId = fileUploadId,
                RowNumber = rowNumber,
                ColumnName = columnName,
                ErrorMessage = errorMessage,
                Severity = severity,
                InvalidValue = invalidValue,
                DetectedAt = DateTime.UtcNow
            };
        }
    }
}
