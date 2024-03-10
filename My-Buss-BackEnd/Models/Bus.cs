namespace API.Models
{
    public class Bus
    {
        public int ID_Bus { get; set; }
        public int ID_Empresa { get; set; }
        public string Placa { get; set; }
        public string? Modelo { get; set; }
        public int? Capacidad { get; set; }
        public string? Estado { get; set; }
    }
}
