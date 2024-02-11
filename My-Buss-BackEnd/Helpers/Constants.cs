
namespace My_Buss_BackEnd.Helpers
{
    internal static class Constants
    {
        public static readonly string HOME_MESSAGE = "Welcome to My Buss API";

        public static readonly StatusMessages STATUS_MESSAGES = new ()
        {
            OK = "OK",
            ERROR = "ERROR",
            DENIED = "DENIED"
        };
        public class StatusMessages
        {
            public string OK { get; set; }
            public string ERROR { get; set; }
            public string DENIED { get; set; }
        }
    }
}
