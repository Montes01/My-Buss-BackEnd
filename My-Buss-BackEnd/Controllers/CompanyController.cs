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
    [Route("Empresa")]
    [ApiController]
    public class CompanyController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);
        private readonly byte[] keyBytes = Encoding.ASCII.GetBytes(_config.GetSection("SecurityKey").ToString()!);

        [HttpPost]
        [Route("Registrar")]
        public IActionResult RegisterCompany([FromBody] Empresa company)
        {
            string q = $"EXECUTE RegistrarEmpresa '{company.Nombre}', '{company.CorreoElectronico}', '{company.Contraseña}', '{company.Logo}', '{company.Dirección}', '{company.Teléfono}'";

            Utils.OpenConnection(_conn);
            try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Empresa registrada correctamente"));
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
        public IActionResult GetCompanies()
        {
            string q = "EXECUTE ListarEmpresas";
            Utils.OpenConnection(_conn);
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                List<Empresa> companies = [];
                foreach (DataRow el in dt.Rows)
                {
                    companies.Add(new Empresa
                    {
                        ID_Empresa = (int)el["ID_Empresa"]!,
                        Nombre = el["Nombre"].ToString()!,
                        CorreoElectronico = el["CorreoElectronico"].ToString()!,
                        Contraseña = null,
                        Logo = el["Logo"].ToString(),
                        Dirección = el["Dirección"].ToString(),
                        Teléfono = el["Teléfono"].ToString()!
                    });
                }
                return Ok(new Response(STATUS_MESSAGES.OK, companies));
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
        [Route("Ingresar")]
        public IActionResult LoginCompany([FromBody] InicioSesion request)
        {

            string q = $"EXECUTE IniciarSesionEmpresa '{request.Correo}', '{request.Contraseña}'";
            Utils.OpenConnection(_conn);
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new Response(STATUS_MESSAGES.ERROR, "Correo o contraseña incorrectos"));
                }
                DataRow row = dt.Rows[0];
                Empresa company = new()
                {
                    ID_Empresa = (int)row["ID_Empresa"],
                    Nombre = row["Nombre"].ToString()!,
                    CorreoElectronico = request.Correo,
                    Contraseña = null,
                    Logo = row["Logo"].ToString(),
                    Dirección = row["Dirección"].ToString(),
                    Teléfono = row["Teléfono"].ToString()!
                };

                Claim[] claim = [
                    new Claim("Nombre", company.Nombre),
                    new Claim("CorreoElectronico", company.CorreoElectronico),
                    new Claim("ID_Empresa", company.ID_Empresa.ToString()),
                    new Claim("Logo", company.Logo ?? "empty"),
                    new Claim("Direccion", company.Dirección ?? "empty"),
                    new Claim("Telefono", company.Teléfono!)
                ];
                string token = Utils.GenerateToken(claim, keyBytes);

                return Ok(new Response(STATUS_MESSAGES.OK, token));
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
        [Authorize]
        [Route("Actualizar")]
        public IActionResult UpdateCompany([FromBody] Empresa company)
        {
            string? ID_EMPRESA = Utils.Token.GetClaim(HttpContext, "ID_Empresa") ?? null;
            if (ID_EMPRESA == null) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "No estas autorizado para actualizar una empresa"));
            string q = $"EXECUTE ActualizarEmpresa {ID_EMPRESA}, '{company.Nombre}', '{company.CorreoElectronico}', '{company.Contraseña}', '{company.Logo}', '{company.Dirección}', '{company.Teléfono}'";

            
            Utils.OpenConnection(_conn);
            try
            {
                Utils.ExecuteQuery(q, _conn);
                return Ok(new Response(STATUS_MESSAGES.OK, "Empresa actualizada correctamente"));
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
        [Route("Obtener")]
        public IActionResult GetCompany([FromQuery] int ID_Empresa)
        {
            string q = $"EXECUTE ObtenerEmpresa {ID_Empresa}";
            Utils.OpenConnection(_conn);
            try
            {
                var dt = Utils.GetTableFromQuery(q, _conn);
                if (dt.Rows.Count == 0)
                {
                    return BadRequest(new Response(STATUS_MESSAGES.ERROR, "La empresa que buscas no existe"));
                }
                DataRow row = dt.Rows[0];
                Empresa company = new()
                {
                    ID_Empresa = (int)row["ID_Empresa"],
                    Nombre = row["Nombre"].ToString()!,
                    CorreoElectronico = row["CorreoElectronico"].ToString()!,
                    Contraseña = null,
                    Logo = row["Logo"].ToString(),
                    Dirección = row["Dirección"].ToString(),
                    Teléfono = row["Teléfono"].ToString()!
                };
                return Ok(new Response(STATUS_MESSAGES.OK, company));
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
