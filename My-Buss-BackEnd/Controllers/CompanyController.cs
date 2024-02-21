using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Data;
using System.Data.SqlClient;
using static My_Buss_BackEnd.Helpers.Constants;

namespace My_Buss_BackEnd.Controllers
{
    [Route("Empresa")]
    [ApiController]
    public class CompanyController(IConfiguration _config) : ControllerBase
    {
        private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);
        [HttpGet]
        [Route("Listar")]
        public IActionResult GetAllCompanies()
        {
            string q = "EXEC usp_ListarEmpresas";
            try
            {
                DataTable dt = Utils.GetTableFromQuery(q, _conn);
                List<Empresa> empresas = [];
                foreach (DataRow el in dt.Rows)
                {
                    Empresa newCompany = new(
                        el["IdEmpresa"].ToString()!, el["Nombre"].ToString()!, el["Ubicacion"].ToString()!,
                        el["Telefono"].ToString()!, el["correo_electronico"].ToString()!, el["Imagen"].ToString() ?? "",
                        el["Contraseña"].ToString() ?? ""
                            );
                    empresas.Add(newCompany);
                }
                return Ok(new Response(STATUS_MESSAGES.OK, empresas));
            }
            catch (Exception ex)
            {
                return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
            }
        }

    }
}
