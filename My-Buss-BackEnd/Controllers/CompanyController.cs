using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Text;
using System.Data.SqlClient;
using static My_Buss_BackEnd.Helpers.Constants;
using System.Data;

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


    }
}
