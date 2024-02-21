namespace My_Buss_BackEnd.Models
{
    public class Empresa( string IdEmpresa, string Nombre, string Ubicacion, string Telefono, string Correo_electronico, string? Imagen, string? Contraseña )
    {
        public string IdEmpresa { get; set; } = IdEmpresa;
        public string Nombre { get; set; } = Nombre;
        public string Ubicacion { get; set; } = Ubicacion;
        public string Telefono { get; set; } = Telefono;
        public string Correo_electronico { get; set; } = Correo_electronico;
        public string? Imagen { get; set; } = Imagen;
        public string? Contraseña { get; set; } = Contraseña;
    }

}
