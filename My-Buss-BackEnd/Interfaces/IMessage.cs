namespace My_Buss_BackEnd.Interfaces
{
    public interface IMessage
    {
        void EmailConfig(string to, string subject, string body);
    }
}
