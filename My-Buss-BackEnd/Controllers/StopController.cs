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

    }
}
