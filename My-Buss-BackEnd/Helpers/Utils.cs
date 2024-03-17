using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace My_Buss_BackEnd.Helpers
{
    internal static class Utils
    {
        public static string GenerateToken(Claim[] claims, byte[] keyBytes)
        {
            var claimsIdentity = new ClaimsIdentity(claims);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static void CloseConnection(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open) conn.Close();
        }
        public static void OpenConnection(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
        }

        public static void ExecuteQuery(string query, SqlConnection conn)
        {
            SqlCommand cmd = new(query, conn);
            cmd.ExecuteNonQuery();
        }

        public static DataTable GetTableFromQuery(string query, SqlConnection conn)
        {
            DataTable dt = new();
            new SqlDataAdapter(query, conn).Fill(dt);
            return dt;
        }

        public static DataTable GetTableFromCommand(SqlCommand cmd)
        {
            DataTable dt = new();
            new SqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public static void ExecuteCommand(SqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
        }


        public static SqlConnection GetConnection(string connectionString) => new(connectionString);


        public static class Token
        {
            public static string? GetClaim(HttpContext context, string claimType) => context.User.FindFirst(claimType)?.Value;

            public static Claim[] GetClaims(HttpContext context)
            {
                string token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                var handler = new JwtSecurityTokenHandler();

                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                return jsonToken?.Claims.ToArray() ?? [];


            }
        }
    }

}
