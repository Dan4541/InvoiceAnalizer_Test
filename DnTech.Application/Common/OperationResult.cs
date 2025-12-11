namespace DnTech.Application.Common
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? Error { get; }
        public List<string> Errors { get; }

        private OperationResult(bool isSuccess, T? data, string? error, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        public static OperationResult<T> Success(T data) => new(true, data, null);

        public static OperationResult<T> Failure(string error) => new(false, default, error);

        public static OperationResult<T> Failure(List<string> errors) =>
            new(false, default, errors.FirstOrDefault(), errors);
    }

    public class OperationResult
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public List<string> Errors { get; }

        private OperationResult(bool isSuccess, string? error, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            Errors = errors ?? new List<string>();
        }

        public static OperationResult Success() => new(true, null);
        public static OperationResult Failure(string error) => new(false, error);
        public static OperationResult Failure(List<string> errors) =>
            new(false, errors.FirstOrDefault(), errors);
    }



}
