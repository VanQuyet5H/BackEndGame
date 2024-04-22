namespace BackEndGame.EF
{
	public class Order
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalAmount { get; set; }
		// Các thuộc tính khác của đơn hàng, như trạng thái thanh toán, phương thức thanh toán, vv.
		public string PaymentStatus { get; set; } // Trạng thái thanh toán (đã thanh toán, chưa thanh toán, vv.)
		public string PaymentMethod { get; set; } // Phương thức thanh toán (thẻ tín dụng, chuyển khoản, vv.)
												  // Các thuộc tính khác của đơn hàng, nếu có

		// Mối quan hệ với người dùng (khóa ngoại)
		public User User { get; set; }
	}
}
