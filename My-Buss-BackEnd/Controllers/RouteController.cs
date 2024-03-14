using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static My_Buss_BackEnd.Helpers.Constants;

namespace API.Controllers
{
    [Route("Ruta")]
    [ApiController]
    public class RouteController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);



        [HttpPost]
        [Authorize]
        [Route("Agregar")]
        public IActionResult AddRute([FromBody] Ruta ruta)
        {

            string? ID_EMPRESA = Utils.Token.GetClaim(HttpContext, "ID_Empresa") ?? null;
            if (ID_EMPRESA == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para agregar una ruta"));
            string q = $"EXECUTE AgregarRuta {ID_EMPRESA}, '{ruta.Nombre}', '{ruta.Tipo}', '{ruta.Descripción}', '{ruta.Horario}', {ruta.Tarifa}";

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

            return Ok(new Response(STATUS_MESSAGES.OK, "Ruta agregada correctamente"));
        }

        [HttpGet]
        [Route("ListaPorEmpresa")]
        public IActionResult GetRutesByCompany([FromQuery] int ID_Empresa)
        {
            string q = $"EXECUTE ListarRutasPorEmpresa {ID_Empresa}";
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                List<Ruta> rutas = [];
                foreach (DataRow item in dt.Rows)
                {
                    rutas.Add(new Ruta
                    {
                        ID_Ruta = (int)item["ID_Ruta"]!,
                        ID_Empresa = (int)item["ID_Empresa"]!,
                        Nombre = item["Nombre"].ToString()!,
                        Tipo = item["Tipo"].ToString() ?? "empty",
                        Descripción = item["Descripción"].ToString() ?? "empty",
                        Horario = item["Horario"].ToString() ?? "empty",
                        Tarifa = double.Parse(item["Tarifa"]?.ToString() ?? "0")
                    });
                }
                return Ok(new Response(STATUS_MESSAGES.OK, rutas));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("Eliminar")]
        public IActionResult DeleteRute([FromQuery] int ID_Ruta)
        {
            string? ID_EMPRESA = Utils.Token.GetClaim(HttpContext, "ID_Empresa") ?? null;
            if (ID_EMPRESA == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para eliminar una ruta"));
            string q = $"EXECUTE EliminarRuta {ID_Ruta}, {ID_EMPRESA}";

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
            return Ok(new Response(STATUS_MESSAGES.OK, "Ruta eliminada correctamente"));
        }
    }
}
