# legacy-application-refactoring


Author: Diego Nunes Reis

### Potential Security Vulnerabilities:

1. **SQL Injection Vulnerability**

The code uses direct string concatenation to create the SQL query:

```sql
sql = "SELECT * FROM Users WHERE username = '" & username & "'"
```

This makes the system more vulnerable to SQL injection attacks, allowing attackers to insert malicious code in the username field.

2. **Lack of Input Validation**

The value received via Request("username") is not validated, which can lead to input manipulation and facilitate vulnerability exploitation.

3. **Exposure of Sensitive Information**

The "User not found" message provides direct feedback about database data existence, which can be exploited in user enumeration attacks.

4. **Limited Error Handling**

There is no exception handling in the code. Issues like database connection failures or malformed queries are not being handled.

### Refactored Code, in modern ASP.NET Core format:

```csharp
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
                return StatusCode(500, new { error = "An error occurred while accessing the database." });
            }
        }
    }
}
```

### **Applied Improvements**

1. **Use of SQL Query Parameters**

The code uses @username as a parameter in the SQL query, protecting against SQL injection attacks.

2. **Input Validation**

Checks if the username value is null, empty, or contains only whitespace. Returns an appropriate response in case of error.

3. **Error Handling**

The code handles SqlException type exceptions and returns a generic message to the client, protecting internal system details.

4. **Standardized Responses**

The responses follow a consistent format, returning appropriate messages for success, validation errors, and database failures.

If you have any questions or need additional details, please feel free to reach out. Thank you for your time and consideration