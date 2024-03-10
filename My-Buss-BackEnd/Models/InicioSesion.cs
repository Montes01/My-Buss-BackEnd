namespace My_Buss_BackEnd.Models
{
    public class InicioSesion( string Correo, string contraseña)
    {
        public string Correo { get; set; } = Correo;
        public string Contraseña { get; set; } = contraseña;
    }
}
