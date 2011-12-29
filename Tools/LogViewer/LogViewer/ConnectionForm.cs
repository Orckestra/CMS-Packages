using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LogViewer
{
	public partial class ConnectionForm : Form
	{
		private static readonly string AppDataFolderName = "Composite C1 Log Viewer";
		private static readonly string AppDataFileName = "RecentUrls.txt";

		public static C1Connection CurrentConnection { get; protected set; }

		private string _recentEntriesFileName;

		private ConnectionInfo[] _recentConnections;

		private class ConnectionInfo
		{
			public string URL;
			public string LoggerPassword;

			public override string ToString()
			{
				return URL + (LoggerPassword != null ? "|" + LoggerPassword : string.Empty);
			}

			public static ConnectionInfo Parse(string value)
			{
				int separatorIndex = value.IndexOf("|");
				if (separatorIndex < 0)
				{
					return new ConnectionInfo { URL = value };
				}

				string loggerPassword = null;

				if (separatorIndex < value.Length - 1)
				{
					loggerPassword = value.Substring(separatorIndex + 1);
				}
				return new ConnectionInfo
						   {
							   URL = value.Substring(0, separatorIndex),
							   LoggerPassword = loggerPassword
						   };
			}
		}



		public ConnectionForm()
		{
			InitializeComponent();
		}

		private void btnFalse_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			bool useAdminPassword = rbnAdminUser.Checked;

			AuthenticationInfo authenticationInfo;

			if (useAdminPassword)
			{
				string login = txtLogin.Text.Trim();
				if (string.IsNullOrEmpty(login))
				{
					MessageBox.Show("Login cannot be empty");
					return;
				}

				string password = txtPassword.Text;
				if (string.IsNullOrEmpty(password))
				{
					MessageBox.Show("Password cannot be empty");
					return;
				}

				authenticationInfo = new AuthenticationInfo(login, password);
			}
			else
			{
				Guid authToken;

				string loggerPassword = txtLoggerPassword.Text;

				if (string.IsNullOrEmpty(loggerPassword)
					|| !TryParseGuid(loggerPassword, out authToken))
				{
					MessageBox.Show(string.IsNullOrEmpty(loggerPassword) ?
						"Logger password cannot be empty"
						: "Logger password should be a valid guid");
					return;
				}

				authenticationInfo = AuthenticationInfo.FromLoggerPassword(loggerPassword);
			}


			//bool useHttp = rbnHttp.Checked;

			C1Connection connection;

			//if(useHttp)
			{
				string url = txtUrl.Text;

				if (string.IsNullOrEmpty(url))
				{
					MessageBox.Show("URL is empty");
					return;
				}

				connection = new C1Connection(url, authenticationInfo);
			}
			//else
			//{

			//    int portNumber;

			//    if(!int.TryParse(txtPort.Text, out portNumber))
			//    {
			//        MessageBox.Show("Port number is invalid.");
			//        return;
			//    }

			//    string host = txtHost.Text;

			//    if (string.IsNullOrEmpty(host))
			//    {
			//        MessageBox.Show("Host is empty");
			//        return;
			//    }

			//    connection = new C1Connection(host, portNumber, authenticationInfo);
			//}

			bool authenticated;
			try
			{
				authenticated = connection.Authenticate();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to establish connection. Exception message: " + ex.Message);
				return;
			}

			if (!authenticated)
			{
				MessageBox.Show("Login or password is not correct or the user does not have administrative rights.");
				return;
			}

			DialogResult = DialogResult.OK;
			CurrentConnection = connection;
			Close();

			UpdateRecentUrlList(txtUrl.Text, connection.AuthenticationInfo.AuthToken);
		}

		private void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			DisableControls();
		}

		private void UpdateRecentUrlList(string url, string authToken)
		{
			ConnectionInfo newConnectionInfo = new ConnectionInfo { URL = url };
			if (cbxSaveAuthInfo.Checked)
			{
				newConnectionInfo.LoggerPassword = authToken;
			}

			// If settings haven't changed - do nothing
			if (_recentConnections != null && _recentConnections.Length > 0
				&& _recentConnections[0].URL == url
				&& _recentConnections[0].LoggerPassword == newConnectionInfo.LoggerPassword)
			{
				return;
			}

			var newSettings = new List<ConnectionInfo>();
			newSettings.Add(newConnectionInfo);

			if (_recentConnections != null)
			{
				newSettings.AddRange(from setting in _recentConnections
									 where string.Compare(setting.URL, url) != 0
									 select setting);
			}


			txtUrl.Items.Clear();
			foreach (var setting in newSettings)
			{
				txtUrl.Items.Add(setting.URL);
			}

			_recentConnections = newSettings.ToArray();
			try
			{
				using (var file = File.CreateText(_recentEntriesFileName))
				{
					Array.ForEach(_recentConnections, setting => file.WriteLine(setting.ToString()));
				}
			}
			catch
			{
				// silent
			}
		}
		//private void rbnHttp_CheckedChanged(object sender, EventArgs e)
		//{
		//    DisableControls();
		//}

		private void DisableControls()
		{
			//bool useHttp = rbnHttp.Checked;
			//txtUrl.Enabled = useHttp;
			//txtHost.Enabled = !useHttp;
			//txtPort.Enabled = !useHttp;

			bool useAdminPassword = rbnAdminUser.Checked;
			txtLogin.Enabled = useAdminPassword;
			txtPassword.Enabled = useAdminPassword;
			txtLoggerPassword.Enabled = !useAdminPassword;
		}

		//private void rbnTcp_CheckedChanged(object sender, EventArgs e)
		//{
		//    DisableControls();
		//}

		private void rbnAdminUser_CheckedChanged(object sender, EventArgs e)
		{
			DisableControls();
		}

		private void ConnectionForm_Load(object sender, EventArgs e)
		{
			DisableControls();

			string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName);
			_recentEntriesFileName = Path.Combine(appDataFolder, AppDataFileName);

			if (!Directory.Exists(appDataFolder))
			{
				Directory.CreateDirectory(appDataFolder);
			}
			else if (File.Exists(_recentEntriesFileName))
			{
				try
				{
					string[] lastEntries = File.ReadAllLines(_recentEntriesFileName);

					var recentConnections = new List<ConnectionInfo>();
					Array.ForEach(lastEntries, line => recentConnections.Add(ConnectionInfo.Parse(line)));

					txtUrl.Items.Clear();
					foreach (var connectionInfo in recentConnections)
					{
						txtUrl.Items.Add(connectionInfo.URL);
					}

					_recentConnections = recentConnections.ToArray();
				}
				catch
				{
					_recentConnections = new ConnectionInfo[0];
					// silent
				}
			}
		}

		private static bool TryParseGuid(string str, out Guid guid)
		{
			try
			{
				guid = new Guid(str);
			}
			catch (Exception)
			{
				guid = Guid.Empty;
				return false;
			}
			return true;
		}

		private void txtUrl_SelectedIndexChanged(object sender, EventArgs e)
		{
			string url = txtUrl.Text;
			string loggerPassword =
				(from connectionInfo in _recentConnections
				 where string.Compare(connectionInfo.URL, url) == 0
				 select connectionInfo.LoggerPassword).FirstOrDefault();

			if (loggerPassword != null)
			{
				rbnLoggerPassword.Checked = true;
				txtLoggerPassword.Text = loggerPassword;
				cbxSaveAuthInfo.Checked = true;

				DisableControls();
			}
			else if (rbnLoggerPassword.Checked)
			{
				txtLoggerPassword.Text = string.Empty;
			}
		}
	}
}
