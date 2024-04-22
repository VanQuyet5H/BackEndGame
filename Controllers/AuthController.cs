using BackEndGame.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BackEndGame.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly BackEndGameDbContext _dbContext;
		private readonly IConfiguration _configuration;
		public AuthController(BackEndGameDbContext dbContext, IConfiguration configuration)
		{
			_dbContext = dbContext;
			_configuration = configuration;
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] UserRegistrationRequest model)
		{
			// Kiểm tra xem người dùng đã tồn tại trong cơ sở dữ liệu chưa
			if (await _dbContext.Users.AnyAsync(u => u.Username == model.Username))
			{
				return BadRequest("Username is already taken.");
			}

			// Tạo một đối tượng User từ dữ liệu đăng ký
			var user = new User
			{
				Username = model.Username,
				Password = model.Password,
				Email = model.Email
				// Các thông tin khác của người dùng
			};

			// Thêm người dùng mới vào cơ sở dữ liệu
			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();

			return Ok("Registration successful.");
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
		{
			var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

			if (user == null)
			{
				return Unauthorized(new { success = false, message = "Incorrect username or password" });
			}

			var token = GenerateJwtToken(user);
			return Ok(new { Token = token, success = true, message = "Login successful", });

			
		}
		[HttpPost("logout")]
		[Authorize]
		public IActionResult Logout()
		{
			// Trong trường hợp này, việc đăng xuất có thể chỉ là xóa token đã được lưu trữ trên client
			return Ok("Logged out successfully");
		}
		private string GenerateJwtToken(User user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("your_secret_key_here"); // Thay thế bằng một khóa bí mật thực sự
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
				new Claim(ClaimTypes.Name, user.Username),
					// Các thông tin khác của người dùng có thể được thêm vào claim ở đây
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<User>>> GetUsers()
		{
			var users = await _dbContext.Users.ToListAsync();
			return Ok(users);
		}

		// GET: api/admin/accounts/5
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			var user = await _dbContext.Users.FindAsync(id);

			if (user == null)
			{
				return NotFound();
			}

			return Ok(user);
		}

		// POST: api/admin/accounts
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<User>> CreateUser(User user)
		{
			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();

			return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
		}

		// PUT: api/admin/accounts/5
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateUser(int id, User user)
		{
			if (id != user.Id)
			{
				return BadRequest();
			}

			_dbContext.Entry(user).State = EntityState.Modified;

			try
			{
				await _dbContext.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!UserExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return NoContent();
		}

		// DELETE: api/admin/accounts/5
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			var user = await _dbContext.Users.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			_dbContext.Users.Remove(user);
			await _dbContext.SaveChangesAsync();

			return NoContent();
		}

		private bool UserExists(int id)
		{
			return _dbContext.Users.Any(e => e.Id == id);
		}
	}

	public class UserRegistrationRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		// Các thông tin khác bạn muốn thu thập trong quá trình đăng ký
	}
	public class UserLoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
