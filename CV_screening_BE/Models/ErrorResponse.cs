namespace CVToolApi.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; } = "";
        public string? Detail { get; set; }
        public int StatusCode { get; set; }
    }
}