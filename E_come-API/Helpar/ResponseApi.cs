namespace E_come_API.Helpar
{
    public class ResponseApi
    {
        public ResponseApi(int statusCode, string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessage(statusCode);
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }

        private  string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Created successfully",
                204 => "No content",
                400 => "Bad request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not found",
                500 => "Server error",
               _=> "null",
            };
        }
    }
}