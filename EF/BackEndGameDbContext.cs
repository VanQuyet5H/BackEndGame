using BackEndGame.EF;
using Microsoft.EntityFrameworkCore;

namespace BackEndGame
{
	public class BackEndGameDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Game> Games { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

		public BackEndGameDbContext(DbContextOptions<BackEndGameDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Cấu hình các quan hệ giữa các entity (nếu có)

			// Ví dụ: Quan hệ một-nhiều giữa Order và OrderItem
			//modelBuilder.Entity<Order>()
			//	.HasMany(o => o.OrderItem)
			//	.WithOne(oi => oi.Order)
			//	.HasForeignKey(oi => oi.OrderId);

			//// Ví dụ: Cấu hình các index hoặc ràng buộc khóa duy nhất (nếu có)

			//// Ví dụ: Index trên trường Email của User
			//modelBuilder.Entity<User>()
			//	.HasIndex(u => u.Email)
			//	.IsUnique();
		}
	}
}
