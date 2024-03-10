using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Buss_BackEnd.Helpers;
using My_Buss_BackEnd.Models;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Security.Claims;
using static My_Buss_BackEnd.Helpers.Constants;
using API.Models;

namespace My_Buss_BackEnd.Controllers
{
    [Route("Empresa")]
    [ApiController]
    public class CompanyController(IConfiguration _config) : ControllerBase
    {
        //private readonly SqlConnection _conn = Utils.GetConnection(_config.GetConnectionString("DefaultConnection")!);
        //private readonly byte[] keyBytes = Encoding.ASCII.GetBytes(_config.GetSection("SecurityKey").ToString()!);
        //[HttpGet]
        //[Route("Listar")]
        //public IActionResult GetAllCompanies()
        //{
        //    string q = "EXEC usp_ListarEmpresas";
        //    try
        //    {
        //        DataTable dt = Utils.GetTableFromQuery(q, _conn);
        //        List<Empresa> empresas = [];
        //        foreach (DataRow el in dt.Rows)
        //        {
        //            Empresa newCompany = new(
        //                el["IdEmpresa"].ToString()!, el["Nombre"].ToString()!, el["Ubicacion"].ToString()!,
        //                el["Telefono"].ToString()!, el["correo_electronico"].ToString()!, el["Imagen"].ToString() ?? "",
        //                el["Contraseña"].ToString() ?? ""
        //                    );
        //            empresas.Add(newCompany);
        //        }
        //        return Ok(new Response(STATUS_MESSAGES.OK, empresas));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
        //    }
        //}

        //[HttpPost]
        //[Route("Registrar")]
        //public IActionResult RegisterCompany(Empresa company)
        //{
        //    string q = $"EXEC usp_RegistrarEmpresa '{company.IdEmpresa}', '{company.Nombre}', '{company.Ubicacion}', '{company.Telefono}', '{company.Correo_electronico}', '{company.Imagen}', '{company.Contraseña}'";

        //    Utils.OpenConnection(_conn);
        //    try
        //    {
        //        Utils.ExecuteQuery(q, _conn);
        //        return Ok(new Response(STATUS_MESSAGES.OK, "Empresa registrada correctamente"));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
        //    }
        //    finally
        //    {
        //        Utils.CloseConnection(_conn);
        //    }
        //}

        //[HttpPost]
        //[Route("Ingresar")]
        //public IActionResult ValidateCompany([FromBody] InicioSesion request)
        //{
        //    string q = $"EXECUTE usp_iniciarSesionEmpresa '{request.Correo}', '{request.Contraseña}'";


        //    Empresa empresa;
        //    try
        //    {
        //        DataTable dt = Utils.GetTableFromQuery(q, _conn);
        //        if (dt.Rows.Count < 1) return Unauthorized(new Response(STATUS_MESSAGES.DENIED, "Empresa no encontrada"));
        //        empresa = new Empresa(
        //                                dt.Rows[0]["IdEmpresa"].ToString()!, dt.Rows[0]["Nombre"].ToString()!,
        //                               dt.Rows[0]["Ubicacion"].ToString()!, dt.Rows[0]["Telefono"].ToString()!,
        //                               dt.Rows[0]["Correo_electronico"].ToString()!, dt.Rows[0]["Imagen"].ToString()!,
        //                               dt.Rows[0]["Contraseña"].ToString()!
        //                           );

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new Response(STATUS_MESSAGES.ERROR, ex.Message));
        //    }

        //    Claim[] claims = [
        //        new("IdEmpresa", empresa.IdEmpresa),
        //        new("Nombre", empresa.Nombre),
        //        new("Ubicacion", empresa.Ubicacion),
        //        new("Telefono", empresa.Telefono),
        //        new("Correo_electronico", empresa.Correo_electronico),
        //        new("Imagen", empresa.Imagen ?? ""),
        //        ];
        //    string token = Utils.GenerateToken(claims, keyBytes);

        //    return Ok(new Response(STATUS_MESSAGES.OK, token));
        //}

    }
}
