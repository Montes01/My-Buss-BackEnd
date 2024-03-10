namespace My_Buss_BackEnd.Models
{
    public class Ruta
    {
        public int? ID_Ruta { get; set; }
        public int? ID_Empresa { get; set; }
        public string Nombre { get; set; }
        public string? Tipo { get; set; }
        public string? Descripción { get; set; }
        public string? Horario { get; set; }
        public double Tarifa { get; set; }

    }
}
