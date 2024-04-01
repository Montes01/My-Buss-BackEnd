using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Text;
using System.Data.SqlClient;
using static My_Buss_BackEnd.Helpers.Constants;
using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace My_Buss_BackEnd.Controllers
{
    [Route("admin")]
    [Authorize]
    [ApiController]
    public class AdminController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);
        private readonly byte[] keyBytes = Encoding.ASCII.GetBytes(_config.GetSection("SecurityKey").ToString()!);

        [HttpDelete]
        [Route("Eliminar/Empresa")]
        public IActionResult DeleteCompany([FromQuery] int companyId)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE EliminarEmpresa {companyId}";
            Utils.OpenConnection(_conn);
            try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Empresa eliminada correctamente"));
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

        [HttpDelete]
        [Route("Eliminar/Ticket")]
        public IActionResult DeleteTicket([FromQuery] int ticketId)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE EliminarTicketAdmin {ticketId}";

            Utils.OpenConnection(_conn);
            try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Ticket eliminado correctamente"));
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
        [Route("Listar/Tickets")]
        public IActionResult GetTickets()
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = "EXECUTE ListarTicketsAdmin";

            Utils.OpenConnection(_conn);
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                List<Ticket> tickets = [];
                foreach (DataRow el in dt.Rows)
                {
                    tickets.Add(new Ticket
                    {
                        ID_Ticket = (int)el["ID_Ticket"],
                        ID_Usuario = (int)el["ID_Usuario"],
                        ID_Empresa = (int)el["ID_Empresa"],
                        Precio = (decimal)el["Precio"],
                        TipoPago = (string)el["TipoPago"],
                        Estado = (string)el["Estado"],
                        PaymentId = (string)el["PaymentId"],

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



        [HttpDelete]
        [Route("Eliminar/Usuario")]
        public IActionResult DeleteUser([FromQuery] int userId)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE EliminarUsuario {userId}";

            Utils.OpenConnection(_conn);
            try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Usuario eliminado correctamente"));
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
        [Route("Listar/Usuarios")]
        public IActionResult GetUsers()
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = "EXECUTE ListarUsuariosAdmin";



            Utils.OpenConnection(_conn);
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                List<Usuario> users = [];
                foreach (DataRow el in dt.Rows)
                {
                    users.Add(new Usuario
                    (
                        (int)el["ID_Usuario"],
                        (string)el["Nombre"],
                        (string)el["CorreoElectronico"],
                        (string)el["Teléfono"],
                        el["Rol"]?.ToString() ?? string.Empty, // Si "Rol" es null, se asigna "".
                        el["Contraseña"]?.ToString() ?? string.Empty, // Si "Contraseña" es null, se asigna "".
                        el["FotoPerfil"]?.ToString() ?? string.Empty, // Si "FotoPerfil" es null, se asigna "".
                        el["Dirección"]?.ToString() ?? string.Empty // Si "Dirección" es null, se asigna "".
                    ));
                }
                return Ok(new Response(STATUS_MESSAGES.OK, users));
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
    }
}
