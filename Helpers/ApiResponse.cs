namespace ASPDOTNETDEMO.Helpers
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse() { }

        public ApiResponse(int status, bool success, string message, T? data)
        {
            Status = status;
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
