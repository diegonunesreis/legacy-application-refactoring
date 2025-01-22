using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LegacyAppRefactor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly string _connectionString = "YourConnectionStringHere";

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromQuery] string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new { error = "Username is required." });
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT name FROM Users WHERE username = @username";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                string name = reader["name"].ToString();
                                return Ok(new { message = $"Hello, {name}" });
                            }
                            else
                            {
                                return NotFound(new { error = "User not found." });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the exception here (e.g., using a logging library like Serilog)
                return StatusCode(500, new { error = "An error occurred while accessing the database." });
            }
        }
    }
}