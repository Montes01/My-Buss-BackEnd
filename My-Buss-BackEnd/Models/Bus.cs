namespace API.Models
{
    public class Bus(string placaB, string fotoB, int numeroR, string modeloB, string colorB, int capacidadB, int cilindrajeB, string marcaB, double maximaVelocidad)
    {
        public string PlacaB { get; set; } = placaB;
        public string FotoB { get; set; } = fotoB;
        public int NumeroR { get; set; } = numeroR;
        public string ModeloB { get; set; } = modeloB;
        public string ColorB { get; set; } = colorB;
        public int CapacidadB { get; set; } = capacidadB;
        public int CilindrajeB { get; set; } = cilindrajeB;
        public string MarcaB { get; set; } = marcaB;
        public double MaximaVelocidad { get; set; } = maximaVelocidad;
    }
}
