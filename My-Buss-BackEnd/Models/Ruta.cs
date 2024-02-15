namespace API.Models
{
    public class Ruta (int numeroR, string inicioR, string finR, bool estadoR, string empresaR)
    {
        public int NumeroR { get; set; } = numeroR;
        public string InicioR { get; set; } = inicioR;
        public string FinR { get; set; } = finR;
        public bool EstadoR { get; set; } = estadoR;
        public string EmpresaR { get; set; } = empresaR;
    }
}
