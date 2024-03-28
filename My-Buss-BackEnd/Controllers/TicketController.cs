using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Interfaces;
using My_Buss_BackEnd.Models;
using System.Data;
using System.Data.SqlClient;
using Stripe;
using static My_Buss_BackEnd.Helpers.Constants;
namespace My_Buss_BackEnd.Controllers
{
    [Route("Ticket")]
    [ApiController]
    public class TicketController(IConfiguration _config, IMessage message) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);

        private readonly IMessage _message = message;



        [HttpPut]
        [Route("Pagar")]
        [Authorize]
        public IActionResult PayTicket([FromBody] Ticket ticket)
        {
            string? IDUsuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (IDUsuario == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para pagar un ticket"));
            string Email = Utils.Token.GetClaim(HttpContext, "CorreoElectronico")!;

            StripeConfiguration.ApiKey = "sk_test_51OyEqeITkeuqFU4OYMbjyaiT8zT4xELkMW6VCwMTr7361jwjWEvZSFsDcoaA9GVYKNw74byZhH22RKun4ykseQEy00xxyQtH4R";

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(ticket.Precio * 100),
                Currency = "usd",
                PaymentMethodTypes = ["card"],
                PaymentMethod = ticket.PaymentId,
                Confirm = true,
            };

            var service = new PaymentIntentService();
            string Nombre = Utils.Token.GetClaim(HttpContext, "Nombre")!;
            string q_01 = $"EXECUTE ObtenerEmpresa {ticket.ID_Empresa}";
            string q_02 = $"EXECUTE PagarTicket {ticket.ID_Ticket}, '{ticket.PaymentId}'";
            try
            {

                service.Create(options);
                Utils.OpenConnection(_conn);
                DataTable dt = Utils.GetTableFromQuery(q_01, _conn);
                DataRow row = dt.Rows[0];
                string Empresa = row["Nombre"]?.ToString() ?? "No encontrada";
                Utils.ExecuteQuery(q_02, _conn);
                _message.EmailConfig(Email, "Ticket pagado", PayHTML.Replace("[PRICE]", ticket.Precio + "$").Replace("[COMPANY]", Empresa).Replace("[DATE]", DateTime.Now.ToString()).Replace("[USERNAME]", Nombre));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
            finally
            {
                Utils.CloseConnection(_conn);
            }

            return Ok(new Response(STATUS_MESSAGES.OK, "Ticket pagado correctamente"));
        }




        [HttpGet]
        [Route("Obtener")]
        [Authorize]
        public IActionResult GetTicket([FromQuery] int ID_Ticket)
        {
            string? IDUsuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (IDUsuario == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para ver el ticket"));
            string q = $"EXECUTE ObtenerTicket {ID_Ticket}";

            try
            {
                Utils.OpenConnection(_conn);
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                if (dt.Rows.Count == 0) return NotFound(new Response(STATUS_MESSAGES.ERROR, "No se encontro el ticket"));
                DataRow row = dt.Rows[0];
                Ticket ticket = new()
                {
                    ID_Ticket = (int)row["ID_Ticket"],
                    ID_Usuario = (int)row["ID_Usuario"],
                    ID_Empresa = (int)row["ID_Empresa"],
                    Precio = (decimal)row["Precio"],
                    TipoPago = (string)row["TipoPago"],
                    Estado = (string)row["Estado"]
                };
                if (ticket.ID_Usuario != int.Parse(IDUsuario)) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para ver el ticket"));
                return Ok(new Response(STATUS_MESSAGES.OK, ticket));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
            finally
            {
                Utils.CloseConnection(_conn);
            }
        }

        [HttpGet]
        [Route("Listar")]
        [Authorize]
        public IActionResult GetTickets()
        {
            string? IDUsuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (IDUsuario == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para ver los tickets"));
            string q = $"EXECUTE ListarTickets {IDUsuario}";

            try
            {
                Utils.OpenConnection(_conn);
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                List<Ticket> tickets = [];
                foreach (DataRow row in dt.Rows)
                {
                    tickets.Add(new Ticket
                    {
                        ID_Ticket = (int)row["ID_Ticket"],
                        ID_Usuario = (int)row["ID_Usuario"],
                        ID_Empresa = (int)row["ID_Empresa"],
                        Precio = (decimal)row["Precio"],
                        TipoPago = (string)row["TipoPago"],
                        Estado = (string)row["Estado"]
                    });
                }
                return Ok(new Response(STATUS_MESSAGES.OK, tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
            finally
            {
                Utils.CloseConnection(_conn);
            }
        }

        [HttpPost]
        [Route("Agregar")]
        [Authorize]
        public IActionResult AddTicket([FromBody] Ticket ticket)
        {
            string? IDUsuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (IDUsuario == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para agregar un ticket"));
            string Email = Utils.Token.GetClaim(HttpContext, "CorreoElectronico")!;
            string q = $"EXECUTE AgregarTicket {IDUsuario}, {ticket.ID_Empresa},  {ticket.Precio}, '{ticket.TipoPago}', '{"pendiente"}'";

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

        private readonly string PayHTML = "<header\r\n    style=\"display=flex; width:100%; background: #903; padding: 5px 10px; justify-content: center; border-radius:15px; box-sizing: border-box;\">\r\n\r\n    <h1 style=\"width:100%; color:white; text-align:center;\">Gracias por tu compra [USERNAME]</h1>\r\n</header>\r\n<main>\r\n    <section style=\"display:flex; width:100%; justify-content: center; align-items:center; padding: 10px; gap: 10px;box-sizing:border-box;\">\r\n\r\n        <p>Tu tiquete fue pagado satisfactoriamente</p>\r\n        <img style=\"height: 50px; aspect-ratio: 1/1;\" src=\"https://www.pngall.com/wp-content/uploads/2016/07/Success-PNG-Image.png\" alt=\"\" srcset=\"\">\r\n    </section>\r\n    <ul>\r\n        <li>Precio: [PRICE]$</li>\r\n        <li>Empresa: [COMPANY]</li>\r\n        <li>Fecha de compra: [DATE]</li>\r\n    </ul>\r\n</main>";
    }
}
