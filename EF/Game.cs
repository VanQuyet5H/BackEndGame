namespace BackEndGame.EF
{
	public class Game
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; } // URL của hình ảnh đại diện cho trò chơi
		public string Genre { get; set; } // Thể loại của trò chơi (hành động, phiêu lưu, chiến thuật, vv.)
		public string Developer { get; set; } // Nhà phát triển của trò chơi
	}
}
