using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Data;
using System.Data.SqlClient;
using static My_Buss_BackEnd.Helpers.Constants;
namespace My_Buss_BackEnd.Controllers
{
    [Route("Paradero")]
    [ApiController]
    public class StopController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);

        [HttpGet]
        [Route("Listar")]
        public IActionResult GetStops()
        {
            string q = "EXECUTE ListarParaderos";
            try
            {
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                List<Paradero> paraderos = [];
                foreach (DataRow el in dt.Rows)
                {
                    paraderos.Add(new Paradero
                    {
                        ID_Paradero = (int)el["ID_Paradero"]!,
                        Nombre = el["Nombre"].ToString()!,
                        Ubicación = el["Ubicación"].ToString(),
                        Descripción = el["Descripción"].ToString(),
                        Foto = el["Foto"].ToString()
                    });
                }
                return Ok(new Response(STATUS_MESSAGES.OK, paraderos));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

         
        [HttpGet]
        [Route("ListarPorRuta")]
        public IActionResult GetStopsByRoute([FromQuery] int ID_Ruta)
        {
            string q = $"EXECUTE ListarParadasPorRuta {ID_Ruta}";
            try
            {
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                List<Paradero> paraderos = [];
                foreach (DataRow el in dt.Rows)
                {
                    paraderos.Add(new Paradero
                    {
                        ID_Paradero = (int)el["ID_Paradero"]!,
                        Nombre = el["Nombre"].ToString()!,
                        Ubicación = el["Ubicación"].ToString() ?? "empty",
                        Descripción = el["Descripción"].ToString() ?? "empty",
                        Foto = el["Foto"].ToString() ?? "empty"
                    });
                }
                return Ok(new Response(STATUS_MESSAGES.OK, paraderos));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }


        [HttpGet]
        [Route("Obtener")]
        public IActionResult GetStop([FromQuery] int ID_Paradero)
        {
            string q = $"EXECUTE ObtenerParadero {ID_Paradero}";
            try
            {
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new Response(STATUS_MESSAGES.ERROR, "El paradero que buscas no existe"));
                }
                DataRow el = dt.Rows[0];
                Paradero paradero = new ()
                {
                    ID_Paradero = (int)el["ID_Paradero"]!,
                    Nombre = el["Nombre"].ToString()!,
                    Ubicación = el["Ubicación"].ToString() ?? "empty",
                    Descripción = el["Descripción"].ToString() ?? "empty",
                    Foto = el["Foto"].ToString() ?? "empty"
                };
                return Ok(new Response(STATUS_MESSAGES.OK, paradero));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

        //agregar y eliminar, solo los administradores pueden

        [HttpPost]
        [Route("Agregar")]
        [Authorize]
        public IActionResult AddStop([FromBody] Paradero paradero)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE AgregarParadero '{paradero.Nombre}', '{paradero.Ubicación}', '{paradero.Descripción}', '{paradero.Foto}'";

            try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Paradero agregado correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

        [HttpDelete]
        [Route("Eliminar")]
        [Authorize]
        public IActionResult DeleteStop([FromQuery] int ID_Paradero)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE EliminarParadero {ID_Paradero}";
       try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Paradero eliminado correctamente"));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }
    }
}
