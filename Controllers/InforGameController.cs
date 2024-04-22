using BackEndGame.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BackEndGame.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class InforGameController : ControllerBase
	{
		private readonly BackEndGameDbContext _dbContext;

		public InforGameController(BackEndGameDbContext dbContext)
		{
			_dbContext = dbContext;

		}
		[HttpGet("search")]
		public IActionResult SearchGames(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
			{
				return BadRequest("Invalid keyword.");
			}

			// Tìm kiếm trò chơi dựa trên keyword
			var games = _dbContext.Games
				.Where(g => g.Title.Contains(keyword) || g.Description.Contains(keyword))
				.ToList();

			if (games.Count == 0)
			{
				return NotFound("No games found.");
			}

			return Ok(games);
		}
		[HttpGet]
		public IActionResult GetGames()
		{
			var games = _dbContext.Games.ToList();
			return Ok(games);
		}

		[HttpGet("{id}")]
		public IActionResult GetGame(int id)
		{
			var games = _dbContext.Games.ToList();
			var game = games.Find(g => g.Id == id);
			if (game == null)
			{
				return NotFound();
			}
			return Ok(game);
		}

		[HttpPost("purchase")]
		public IActionResult PurchaseGame([FromBody] PurchaseModel model)
		{
			if (model == null || model.UserId <= 0 || model.TotalAmount <= 0)
			{
				return BadRequest("Invalid purchase request.");
			}

			// Kiểm tra xem người dùng có tồn tại không
			var user = _dbContext.Users.FirstOrDefault(u => u.Id == model.UserId);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			// Tạo một đơn hàng mới
			var order = new Order
			{
				UserId = model.UserId,
				OrderDate = DateTime.UtcNow,
				TotalAmount = model.TotalAmount,
				PaymentStatus = model.PaymentStatus, // Có thể cần validate giá trị này
				PaymentMethod = model.PaymentMethod, // Có thể cần validate giá trị này
													 // Các thuộc tính khác của đơn hàng, nếu có
			};

			_dbContext.Orders.Add(order);
			_dbContext.SaveChanges();

			return Ok(order);
		}

		[HttpPost("add")]
		public IActionResult AddGame([FromBody] GameModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.Title))
			{
				return BadRequest("Invalid game information.");
			}

			var game = new Game
			{
				Title = model.Title,
				Description = model.Description,
				Price = model.Price,
				ImageUrl = model.ImageUrl,
				Genre = model.Genre,
				Developer = model.Developer,
				// Các thuộc tính khác của trò chơi, nếu có
			};

			_dbContext.Games.Add(game);
			_dbContext.SaveChanges();

			return Ok(game);
		}
		[HttpGet("invoice")]
		public IActionResult PrintInvoice(int orderId)
		{
			// Tìm kiếm đơn hàng dựa trên orderId
			var order = _dbContext.Orders.FirstOrDefault(o => o.Id == orderId);

			if (order == null)
			{
				return NotFound("Order not found.");
			}

			// Tạo nội dung hóa đơn
			string invoiceContent = $"Invoice for Order ID: {order.Id}\n";
			invoiceContent += $"Order Date: {order.OrderDate}\n";
			invoiceContent += $"Total Amount: {order.TotalAmount}\n";
			invoiceContent += $"Payment Status: {order.PaymentStatus}\n";
			invoiceContent += $"Payment Method: {order.PaymentMethod}\n";

			// Gửi nội dung hóa đơn
			return Ok(invoiceContent);
		}
	}
	public class PurchaseModel
	{
		public int UserId { get; set; }
		public decimal TotalAmount { get; set; }
		public string PaymentStatus { get; set; }
		public string PaymentMethod { get; set; }
	}
	public class GameModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; }
		public string Genre { get; set; }
		public string Developer { get; set; }
	}
}

