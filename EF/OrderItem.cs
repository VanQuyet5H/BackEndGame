namespace BackEndGame.EF
{
	public class OrderItem
	{
		public int Id { get; set; }
		public int OrderId { get; set; } // Khóa ngoại tham chiếu đến đơn hàng
		public int GameId { get; set; } // Khóa ngoại tham chiếu đến trò chơi
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		// Các thuộc tính khác của mục trong đơn hàng, như giảm giá, tổng giá, vv.
		public decimal Discount { get; set; } // Giảm giá cho mục hàng
		public decimal TotalPrice { get; set; } // Tổng giá của mục hàng sau khi áp dụng giảm giá (giá * số lượng - giảm giá)
												// Các thuộc tính khác của mục trong đơn hàng, nếu có

		// Mối quan hệ với đơn hàng (khóa ngoại)
		public Order Order { get; set; }

		// Mối quan hệ với trò chơi (khóa ngoại)
		public Game Game { get; set; }
	}
}
