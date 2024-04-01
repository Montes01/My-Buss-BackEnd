using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using static My_Buss_BackEnd.Helpers.Constants;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("Usuario")]
    [ApiController]
    public class UserController(IConfiguration _config) : ControllerBase
    {
        private readonly byte[] keyBytes = Encoding.ASCII.GetBytes(_config.GetSection("SecurityKey").ToString()!);
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);

        [HttpPost]
        [Route("Ingresar")]
        public IActionResult Validar([FromBody] InicioSesion request)
        {
            string q = $"EXECUTE IniciarSesion '{request.Correo}', '{request.Contraseña}'";
            try
            {
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                if (dt.Rows.Count < 1) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "Usuario no encontrado"));
                DataRow row = dt.Rows[0];
                Usuario usuario = new 
                    (
                        (int)row["ID_Usuario"],
                        row["Nombre"].ToString()!,
                        request.Correo,
                        row["Teléfono"].ToString()!,
                        row["Rol"].ToString(), 
                        null, 
                        row["FotoPerfil"].ToString(), 
                        row["Dirección"].ToString()
                    );



                Claim[] claim = [
                    new Claim("Nombre", usuario.Nombre),
                    new Claim("CorreoElectronico", usuario.CorreoElectronico),
                    new Claim("Teléfono", usuario.Teléfono),
                    new Claim("ID_Usuario", usuario.ID_Usuario.ToString()),
                    new Claim("FotoPerfil", usuario.FotoPerfil ?? "empty"),
                    new Claim("Rol", usuario.Rol!),
                    new Claim("Direccion", usuario.Dirección ?? "empty")
                ];
                string token = Utils.GenerateToken(claim, keyBytes);

                return Ok(new Response(STATUS_MESSAGES.OK, token));

            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }

        }

        [HttpPost]
        [Route("Registrar")]
        public IActionResult Registrar([FromBody] Usuario request) 
        {
            if (request.Contraseña == null)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, "Debes enviar una contraseña"));
            }
            string q = $"EXECUTE RegistrarUsuario '{request.Nombre}', '{request.Rol}', '{request.CorreoElectronico}', '{request.Contraseña}', '{request.FotoPerfil}', '{request.Dirección}', '{request.Teléfono}'";
            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            } finally
            {
               Utils.CloseConnection(_conn);
            }

            return Ok(new Response(STATUS_MESSAGES.OK, "Usuario registrado"));
        }

        [HttpPut]
        [Route("Actualizar")]
        [Authorize]
        public IActionResult Actualizar([FromBody] Usuario request)
        {
            string ID_Usuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (ID_Usuario == null)
            {
                return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));
            }
            string q = $"EXECUTE ActualizarUsuario {ID_Usuario}, '{request.Nombre}', '{request.CorreoElectronico}', '{request.Contraseña}', '{request.FotoPerfil}', '{request.Dirección}', '{request.Teléfono}'";

            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Usuario actualizado correctamente"));
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

        //solicitar ser conductor
        [HttpPost]
        [Route("Trabajo")]
        [Authorize]
        public IActionResult SolicitarConductor([FromBody] Conductor request)
        {
            string? ID_Usuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (ID_Usuario == null)
            {
                return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));
            }
            string q = $"EXECUTE SolicitarConductor {ID_Usuario}, '{null}', '{request.HorarioTrabajo}', '{request.Licencia}'";

            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Solicitud enviada correctamente"));
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

        //aprobar y rechazar trabajadores

        [HttpPut]
        [Route("Aprobar")]
        [Authorize]
        public IActionResult AprobarConductor([FromQuery] int ID_Conductor)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE AprobarConductor {ID_Conductor}";


            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Conductor aprobado correctamente"));
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

        [HttpPut]
        [Route("Rechazar")]
        [Authorize]
        public IActionResult RechazarConductor([FromQuery] int ID_Conductor)
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = $"EXECUTE RechazarConductor {ID_Conductor}";

            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Conductor rechazado correctamente"));
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

        //cancelar solicitud
        [HttpDelete]
        [Route("Cancelar")]
        [Authorize]
        public IActionResult CancelarSolicitud()
        {
            string? ID_Usuario = Utils.Token.GetClaim(HttpContext, "ID_Usuario") ?? null;
            if (ID_Usuario == null)
            {
                return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));
            }
            string q = $"EXECUTE CancelarSolicitud {ID_Usuario}";

            try
            {
                Utils.OpenConnection(_conn);
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Solicitud cancelada correctamente"));
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


        //listar aspirantes
        [HttpGet]
        [Route("Listar/Aspirantes")]
        [Authorize]
        public IActionResult GetAspirantes()
        {
            string? rol = Utils.Token.GetClaim(HttpContext, "Rol");
            if (rol != "admin") return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No tienes permisos para realizar esta acción"));

            string q = "EXECUTE ListarAspirantes";


            try
            {
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                List<Conductor> aspirantes = new();
                foreach (DataRow el in dt.Rows)
                {
                    Conductor aspirante = new()
                    {
                        ID_Conductor = (int)el["ID_Conductor"],
                        ID_Usuario = (int)el["ID_Usuario"],
                        FechaContrato = (DateTime)el["FechaContrato"],
                        HorarioTrabajo = el["HorarioTrabajo"].ToString(),
                        Licencia = el["Licencia"].ToString(),
                        Estado = (bool)el["Estado"]
                    };
                    aspirantes.Add(aspirante);
                }
                return Ok(new Response(STATUS_MESSAGES.OK, aspirantes));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

    }
}

