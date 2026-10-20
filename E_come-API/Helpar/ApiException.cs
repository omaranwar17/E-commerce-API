namespace E_come_API.Helpar
{
    public class ApiException : ResponseApi
    {
        public ApiException(int statusCode, string? message = null , string details = null) : base(statusCode, message)
        {
            Details = details;
        }
        public string Details { get; set; }
    }
}
