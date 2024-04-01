namespace My_Buss_BackEnd.Models
{
    public class Conductor
    {
        /*
         CREATE TABLE [dbo].[Conductor](
	[ID_Conductor] [int] IDENTITY(1,1) PRIMARY KEY,
	[ID_Usuario] [int] NOT NULL,
	[FechaContrato] [date] NULL,
	[HorarioTrabajo] [nvarchar](100) NULL,
	Licencia NVARCHAR(MAX) NOT NULL,
	Estado BIT DEFAULT 0
)
         */

        public int? ID_Conductor { get; set; }
        public int? ID_Usuario { get; set; }
        public DateTime? FechaContrato { get; set; }
        public string? HorarioTrabajo { get; set; }
        public string? Licencia { get; set; }
        public bool? Estado { get; set; }


    }
}
