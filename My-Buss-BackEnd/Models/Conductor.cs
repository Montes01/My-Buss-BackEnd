namespace API.Models
{
    public class Conductor
        (string foto, string nombre, string apellido, int edad, string telefono, string contraseña, string correo, 
        string cedulaC, bool estado, string fkPlacaBus, string horaEntrada, string horaSalida) 
        : Persona(foto, nombre, apellido, edad, telefono, contraseña, correo)
    {
        public string CedulaC { get; set; } = cedulaC;
        public bool Estado { get; set; } = estado;
        public string FkPlacaBus { get; set; } = fkPlacaBus;
        public string HoraEntrada { get; set; } = horaEntrada;
        public string HoraSalida { get; set; } = horaSalida;
    }
}
