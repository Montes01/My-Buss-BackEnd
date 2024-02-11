namespace API.Models
{
    public class Interaccion (int fkNumeroR, string fkDocumentoU, bool meGusta, string comentario, string horaInteraccion, string horarioSuceso)
    {
        public int FkNumeroR { get; set; } = fkNumeroR;
        public string FkDocumentoU { get; set; } = fkDocumentoU;
        public bool MeGusta { get; set; } = meGusta;
        public string Comentario { get; set; } = comentario;
        public string HoraInteraccion { get; set; } = horaInteraccion;
        public string HorarioSuceso { get; set; } = horarioSuceso;
    }
}
