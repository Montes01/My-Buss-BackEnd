using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Interfaces;
using My_Buss_BackEnd.Models;
using System.Data.SqlClient;
using static My_Buss_BackEnd.Helpers.Constants;
namespace My_Buss_BackEnd.Controllers
{
    [Route("Ticket")]
    [ApiController]
    public class TicketController(IConfiguration _config, IMessage message) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);

        private readonly IMessage _message = message;



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


                _message.EmailConfig(Email, "Ticket agregado", EmailHTML.Replace("Price", ticket.Precio + "$"));
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

        [HttpDelete]
        [Route("Eliminar")]
        [Authorize]
        public IActionResult DeleteTicket([FromQuery] int ID_Ticket)
        {
            string? IDUsuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (IDUsuario == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para eliminar un ticket"));
            string q = $"EXECUTE EliminarTicket {ID_Ticket}, {IDUsuario}";

            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
            finally
            {
                Utils.CloseConnection(_conn);
            }

            return Ok(new Response(STATUS_MESSAGES.OK, "Ticket eliminado correctamente"));
        }

        private readonly string EmailHTML = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title></title>\r\n</head>\r\n\r\n<style>\r\n\r\n    body {\r\n        font-family: Arial, sans-serif;\r\n        margin: 0;\r\n        padding: 20px;\r\n        background-color: #a23;\r\n        border-radius: 20px;\r\n        display: flex;\r\n        flex-direction: column;\r\n        gap: 20px;\r\n    }\r\n    \r\n    h1 {\r\n        color: white;\r\n        margin: 0;\r\n        padding: 0;\r\n        text-align: center;\r\n        font-size: 24px;\r\n        margin-top: 20px;\r\n    }\r\n\r\n    p {\r\n        color: #fff;\r\n        margin: 0;\r\n        padding: 0;\r\n        font-size: 16px;\r\n        margin-top: 20px;\r\n    }\r\n\r\n    img {\r\n        display: block;\r\n        margin: 0 auto;\r\n        border-radius: 15px;\r\n        border: 1px solid black;\r\n        box-shadow: 0 0 10px black;\r\n        margin-top: 20px;\r\n    }\r\n</style>\r\n<body>\r\n    <!-- ! this will be the html that the user will see when he buys a ticket  -->\r\n\r\n    <h1>Has comprado tu ticket satisfactoriamente</h1>\r\n    <p>Apreciado usuario, su ticked ha sido comprado por un valor de <strong>Price</strong></p>\r\n    <img src=\"https://th.bing.com/th/id/OIP.VDg9i102wcgTZ6GoVgMRMwHaE0?rs=1&pid=ImgDetMain\" />\r\n    <p>Gracias por comprar nuestro ticket</p>\r\n    <p>Feliz semana, My Buss Team</p>\r\n\r\n</body>\r\n</html>";
    }
}
