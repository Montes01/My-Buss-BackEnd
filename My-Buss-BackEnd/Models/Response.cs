namespace My_Buss_BackEnd.Models
{
    public class Response( string message, object? data )
    {
        public string Message { get; set; } = message;
        public object? Data { get; set; } = data;
    }
}
