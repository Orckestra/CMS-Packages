namespace LogViewer
{
	public class AuthenticationInfo
	{
		private AuthenticationInfo()
		{
		}

		public AuthenticationInfo(string login, string password)
		{
			Login = login;
			Password = password;
		}


		public static AuthenticationInfo FromLoggerPassword(string loggerPassword)
		{
			return new AuthenticationInfo
					   {
						   AuthToken = loggerPassword
					   };
		}

		public string Login { get; set; }
		public string Password { get; set; }
		public string AuthToken = string.Empty;
	}
}
