using Microsoft.AspNetCore.Mvc;
using API.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using static My_Buss_BackEnd.Helpers.Constants;

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
            string q = $"EXECUTE usp_iniciarSesion '{request.Documento}', '{request.contraseña}'";

            SqlDataAdapter da = new(q, _conn);
            var dt = new DataTable();
            Usuario usuario;
            try
            {
                da.Fill(dt);
                if (dt.Rows.Count < 1) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "Usuario no encontrado"));
                usuario = new Usuario(
                            dt.Rows[0]["Foto"].ToString()!, dt.Rows[0]["Nombre"].ToString()!,
                            dt.Rows[0]["Apellido"].ToString()!, int.Parse(dt.Rows[0]["Edad"].ToString()!),
                            dt.Rows[0]["Telefono"].ToString()!, dt.Rows[0]["Contraseña"].ToString()!,
                            dt.Rows[0]["Correo"].ToString()!, dt.Rows[0]["Documento"].ToString()!,
                            dt.Rows[0]["Rol"].ToString()!
                            );

            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }

            Claim[] claim = [
                new("Foto", usuario.Foto ?? ""),
                new("Nombre", usuario.Nombre),
                new("Apellido", usuario.Apellido),
                new("Edad", usuario.Edad.ToString()),
                new("Telefono", usuario.Telefono),
                new("Correo", usuario.Correo),
                new("Documento", usuario.Documento),
                new("Rol", usuario.Rol)
                ];
            string token = Utils.GenerateToken(claim, keyBytes);

            return Ok(new Response(STATUS_MESSAGES.OK, token));
        }

        [HttpPost]
        [Route("Registrar")]
        public IActionResult Registrar([FromBody] Usuario request) 
        {
            string q = $"EXECUTE usp_registrarUsuario '{request.Foto}', '{request.Nombre}', '{request.Apellido}', {request.Edad}, '{request.Telefono}', '{request.Contraseña}', '{request.Correo}', '{request.Documento}', '{request.Rol}'";

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
    }
}

