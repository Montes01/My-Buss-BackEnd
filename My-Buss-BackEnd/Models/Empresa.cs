namespace My_Buss_BackEnd.Models
{
    public class Empresa
    {

        public int ID_Empresa { get; set; }
        public string Nombre { get; set; }
        public string CorreoElectronico { get; set; }
        public string? Contraseña { get; set; } 
        public string? Logo { get; set; } 
        public string? Dirección { get; set; } 
        public string Teléfono { get; set; } 
    }
}
