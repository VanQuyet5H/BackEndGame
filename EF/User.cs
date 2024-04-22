namespace BackEndGame.EF
{
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; }

		public int? SoDienThoai { get; set; }
		public string? Email { get; set; }
		public string Password { get; set; }
		public string? HoTen { get; set; }
		public string? DiaChi {  get; set; }

		public string? Role { get; set; }
		
	}
}
