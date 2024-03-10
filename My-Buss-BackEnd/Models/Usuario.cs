namespace API.Models
{
    public class Usuario (int id_usuario, string nombre, string correoElectronico, string teléfono, string? rol, string? contraseña, string? fotoPerfil, string? dirección)
    {
            
        public int ID_Usuario { get; set; } = id_usuario; 
        public string Nombre { get; set; } = nombre;
        public string? Rol { get; set; } = rol;
        public string CorreoElectronico { get; set; } = correoElectronico;
        public string? Contraseña { get; set; } = contraseña;
        public string? FotoPerfil { get; set; } = fotoPerfil; 
        public string? Dirección { get; set; } = dirección;
        public string Teléfono { get; set; } = teléfono;
         
    }
}
