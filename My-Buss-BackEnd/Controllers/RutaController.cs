using API.Models;
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
    public class RutaController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);

        [HttpGet]
        [Route("Lista")]
        public IActionResult GetAllRutes()
        {
            string q = "EXECUTE usp_ListarRutas";
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                List<Ruta> rutas = [];
                foreach (DataRow el in dt.Rows)
                {
                    Ruta newRute = new((int)el["NumeroR"], el["Inicio"]!.ToString()!, el["Fin"]!.ToString()!, (bool)el["EstadoR"], el["Empresa"].ToString() ?? "");
                    rutas.Add(newRute);
                }
                return Ok(new Response(STATUS_MESSAGES.OK, rutas));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("Agregar")]
        public IActionResult AddRute([FromBody] Ruta ruta)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "rol");
            if (string.IsNullOrEmpty(rol) || !(rol == "ADMIN" || rol == "SUPERADMIN"))
            {
                return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado a agregar una ruta"));
            }
            string q = $"EXECUTE usp_agregarRuta {ruta.NumeroR}, '{ruta.InicioR}', '{ruta.FinR}', '{ruta.EstadoR}'";


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

        //[HttpDelete]
        //[Authorize]
        //[Route("Eliminar")]
        //public IActionResult DeleteRute([FromQuery] int NumeroR)
        //{

        //    string rol = Token.GetUserRol(HttpContext);
        //    if (!(rol == "ADMIN" || rol == "SUPERADMIN")) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, $"No estas autorizado a eliminar una ruta"));
        //    string q = $"EXECUTE usp_eliminarRuta {NumeroR}";
        //    SqlCommand com = new(q, _conn);
        //    _conn.Open();
        //    try
        //    {
        //        com.ExecuteNonQuery();
        //        return Ok(new Response(STATUS_MESSAGES.OK, "Ruta eliminada correctamente, todos los buses que tenian esta ruta, pasaron a tener una ruta 0"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }
        //}

        //[HttpPut]
        //[Route("CambiarEstado")]
        //[Authorize]
        //public IActionResult UpdateRuteStatus([FromQuery] int NumeroR)
        //{

        //    string rol = Token.GetUserRol(HttpContext);
        //    if (rol != "ADMIN" || rol != "SUPERADMIN")
        //    {
        //        return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "Solo los usuarios con rol de ADMIN o SUPERADMIN pueden actualizar el estado de una ruta"));
        //    }
        //    string q = $"EXECUTE usp_cambiarEstadoRuta {NumeroR}";
        //    SqlCommand com = new(q, _conn);
        //    _conn.Open();
        //    try
        //    {
        //        com.ExecuteNonQuery();
        //        return Ok(new Response(STATUS_MESSAGES.OK, "Ruta Activada/Desactivada correctamente"));
        //    }
        //    catch (SqlException ex)
        //    {
        //        return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }

        //}

        //[HttpPut]
        //[Authorize]
        //[Route("Cambiar")]
        //public IActionResult UpdateRuteWay([FromQuery] int NumeroR, [FromBody] ChangeRute rute)
        //{

        //    string rol = Token.GetUserRol(HttpContext);
        //    if (!(rol == "ADMIN" || rol == "SUPERADMIN"))
        //        return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "Solo los usuarios con rol de ADMIN o SUPERADMIN pueden modificar el recorrido de una ruta"));

        //    string q = "EXECUTE usp_actualizarRecorridoRuta @numeroR, @inicio, @fin";
        //    SqlCommand com = new(q, _conn);
        //    com.Parameters.AddWithValue("@numeroR", NumeroR);
        //    com.Parameters.AddWithValue("@inicio", rute.InicioR);
        //    com.Parameters.AddWithValue("@fin", rute.FinR);
        //    _conn.Open();
        //    try
        //    {
        //        com.ExecuteNonQuery();
        //        return Ok(new Response(STATUS_MESSAGES.OK, "Recorrido de la ruta cambiados correctamente"));
        //    }
        //    catch (SqlException ex)
        //    {
        //        return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
        //    }
        //    finally
        //    {
        //        _conn.Close();
        //    }
        //}



        public class ChangeRute(string InicioR, string FinR)
        {
            public string InicioR { get; set; } = InicioR;
            public string FinR { get; set; } = FinR;
        }

    }
}
