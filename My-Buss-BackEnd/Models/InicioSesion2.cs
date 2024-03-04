namespace API.Models
{
    public class InicioSesion2( string Correo, string contraseña)
    {
        public string Correo { get; set; } = Correo;
        public string Contraseña { get; set; } = contraseña;
    }
}
