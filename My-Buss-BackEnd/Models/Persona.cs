namespace API.Models
{
    public abstract class Persona (string? foto, string nombre, string apellido, int edad, string telefono, string contraseña, string correo )
    {
        public string? Foto { get; set; } = foto;
        public string Nombre { get; set; } = nombre ;
        public string Apellido { get; set; } = apellido ;
        public int Edad { get; set; } = edad ;
        public string Telefono { get; set; } = telefono ;
        public string Contraseña { get; set; } = contraseña ;
        public string Correo{ get; set; } = correo;
    }
}
