namespace DnTech.Domain.Entities
{
    public class FileUpload
    {
        public Guid Id { get; private set; }
        public string OriginalFileName { get; private set; }
        public string StoredFileName { get; private set; }
        public string ContentType { get; private set; }
        public long FileSizeBytes { get; private set; }
        public string StorageLocation { get; private set; } // URL o path
        public Guid UploadedByUserId { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public FileStatus Status { get; private set; }
        public int TotalRows { get; private set; }
        public int ValidRows { get; private set; }
        public int InvalidRows { get; private set; }
        public List<FileValidation> Validations { get; private set; }

        // Parámetros adicionales del upload
        public string Parameter1 { get; private set; }
        public string Parameter2 { get; private set; }

        private FileUpload()
        {
            Validations = new List<FileValidation>();
        }

        public static FileUpload Create(
            string originalFileName,
            string storedFileName,
            string contentType,
            long fileSizeBytes,
            string storageLocation,
            Guid uploadedByUserId,
            string parameter1,
            string parameter2)
        {
            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("El nombre del archivo no puede estar vacío", nameof(originalFileName));

            if (fileSizeBytes <= 0)
                throw new ArgumentException("El tamaño del archivo debe ser mayor a 0", nameof(fileSizeBytes));

            if (uploadedByUserId == Guid.Empty)
                throw new ArgumentException("El ID del usuario no puede estar vacío", nameof(uploadedByUserId));

            return new FileUpload
            {
                Id = Guid.NewGuid(),
                OriginalFileName = originalFileName,
                StoredFileName = storedFileName,
                ContentType = contentType,
                FileSizeBytes = fileSizeBytes,
                StorageLocation = storageLocation,
                UploadedByUserId = uploadedByUserId,
                UploadedAt = DateTime.UtcNow,
                Status = FileStatus.Pending,
                Parameter1 = parameter1 ?? string.Empty,
                Parameter2 = parameter2 ?? string.Empty,
                Validations = new List<FileValidation>()
            };
        }

        public void MarkAsProcessing()
        {
            Status = FileStatus.Processing;
        }

        public void MarkAsCompleted(int totalRows, int validRows, int invalidRows)
        {
            Status = FileStatus.Completed;
            TotalRows = totalRows;
            ValidRows = validRows;
            InvalidRows = invalidRows;
        }

        public void MarkAsFailed()
        {
            Status = FileStatus.Failed;
        }

        public void AddValidation(FileValidation validation)
        {
            Validations.Add(validation);
        }


    }
}
