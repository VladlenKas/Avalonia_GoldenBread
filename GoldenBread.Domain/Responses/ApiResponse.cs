namespace GoldenBread.Domain.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;

        public static ApiResponse<T> Failure() => new() 
        {
            IsSuccess = false, 
            Data = default,
            Message = "Ответ от сервера не получен"
        };
    }
}
