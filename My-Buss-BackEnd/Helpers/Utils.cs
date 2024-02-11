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

        public static void ExecuteQuery(string query, SqlConnection conn)
        {
            SqlCommand cmd = new(query, conn);
            cmd.ExecuteNonQuery();
        }

        public static SqlConnection GetConnection(string connectionString) => new SqlConnection(connectionString);

    }
}
