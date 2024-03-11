using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Data.SqlClient;
using static My_Buss_BackEnd.Helpers.Constants;
namespace My_Buss_BackEnd.Controllers
{
    [Route("Ticket")]
    [ApiController]
    public class TicketController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);


        [HttpPost]
        [Route("Agregar")]
        public IActionResult AddTicket([FromBody] Ticket ticket)
        {
            string? IDUsuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (IDUsuario == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para agregar un ticket"));
            string Email = Utils.Token.GetClaim(HttpContext, "CorreoElectronico")!;
            string q = $"EXECUTE AgregarTicket {IDUsuario}, {ticket.ID_Empresa},  {ticket.Precio}, '{ticket.TipoPago}', '{ticket.Estado ?? "activo"}'";

            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
                //Utils.SendEmail(Email, "Se ha comprado un ticket", "has comprado un ticket");  //Pendiente
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
            finally
            {
                Utils.CloseConnection(_conn);
            }

            return Ok(new Response(STATUS_MESSAGES.OK, "Ticket agregado correctamente"));
        }
    }
}
