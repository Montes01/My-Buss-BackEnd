namespace API.Models
{
    public class Usuario
        (string foto, string nombre, string apellido, int edad, string telefono, string contraseña, string correo, string documento, string rol) 
        : Persona(foto, nombre, apellido, edad, telefono, contraseña, correo)
    {
        public string Documento { get; set; } = documento;
        public string Rol { get; set; } = rol;
    }
}
