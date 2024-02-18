namespace My_Buss_BackEnd.Models
{
    public class Company(string CompanyId, string Name, string Address, string Phone, string Email, string Password, string Image)
    {
        public string CompanyId { get; set; } = CompanyId;
        public string Name { get; set; } = Name;
        public string Address { get; set; } = Address;
        public string Phone { get; set; } = Phone;
        public string Email { get; set; } = Email;
        public string Password { get; set; } = Password;
        public string Image { get; set; } = Image;

    }

}
