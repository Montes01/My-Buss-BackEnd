namespace My_Buss_BackEnd.Models
{
    public class Ticket
    {
        public int ID_Ticket { get; set; }
        public int? ID_Usuario { get; set; }
        public int ID_Empresa { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Precio { get; set; }
        public string? TipoPago { get; set; }
        public string? Estado { get; set; }

        public string? PaymentId { get; set; }

        /*
         *{
            
        "ID_Ticket": 1,
            "ID_Usuario": 1,
            "ID_Empresa": 1,
            "FechaCompra": "2021-10-10T00:00:00",
            "Precio": 10.0,
            "TipoPago": "Efectivo",
            "Estado": "activo"
         *}
         * */
    }
}
