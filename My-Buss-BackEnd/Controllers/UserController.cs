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
                return BadRequest(new Response("Debes enviar una contraseña", null));
            }
            /*
                 @Nombre NVARCHAR(100),
    @Rol NVARCHAR(50) = NULL,
    @CorreoElectronico NVARCHAR(100),
    @Contraseña NVARCHAR(50),
    @FotoPerfil NVARCHAR(MAX) = NULL,
    @Dirección NVARCHAR(200) = NULL,
    @Teléfono NVARCHAR(20)
             */
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
    }
}

