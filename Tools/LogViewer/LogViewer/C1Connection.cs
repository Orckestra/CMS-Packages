using System;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using LogViewer.C1LogService;

namespace LogViewer
{
	public class C1Connection
	{
		private AuthenticationInfo _authenticationInfo;
		private bool _isHttpConnection;
		private string _url;
		private string _hostName;
		private int _port;

        private static Binding CreateBasicHttpBinding(string url)
        {
            bool isSecureConnection = url.StartsWith("https", StringComparison.CurrentCultureIgnoreCase);
            var bindingName = isSecureConnection
                ? "BasicHttpBinding_Secure_ILogService"
                : "BasicHttpBinding_ILogService";

            return new BasicHttpBinding(bindingName);
        }


		public C1Connection(string url, AuthenticationInfo authenticationInfo)
		{
			_isHttpConnection = true;
			_url = url;
			_authenticationInfo = authenticationInfo;

			EndpointAddress = new EndpointAddress(GetServiceUrl());

			Binding = CreateBasicHttpBinding(url);
		}

		public C1Connection(string hostName, int port, AuthenticationInfo authenticationInfo)
		{
			_hostName = hostName;
			_port = port;
			_authenticationInfo = authenticationInfo;

			throw new NotImplementedException();
		}

		public bool Authenticate()
		{
			if (!string.IsNullOrEmpty(_authenticationInfo.AuthToken))
			{
				try
				{
					using (var client = this.Connect())
					{
						client.Channel.GetServerTime();
					}
				}
				catch (Exception)
				{
					return false;
				}
				return true;
			}

			var clientConnection = ConnectSimple();

			string authInfo = _authenticationInfo.Login + "|" + _authenticationInfo.Password;

			string authResult = clientConnection.Authenticate(authInfo);
			if (string.IsNullOrEmpty(authResult))
			{
				// NOTE: Backward compatibility with C1 1.3
				authResult = clientConnection.Authenticate(_authenticationInfo.Password);
			}

			_authenticationInfo.AuthToken = authResult;

			clientConnection.Close();

			return !string.IsNullOrEmpty(authResult);
		}

		public ConnectionWithContext Connect()
		{
			return new ConnectionWithContext(this);
		}

		private LogServiceClient ConnectSimple()
		{
			if (_isHttpConnection)
			{
				return CreateBasicHttpConnection /*CreateWsHttpConnection*/(GetServiceUrl());
			}

			throw new NotImplementedException();
		}

		private string GetServiceUrl()
		{
			string url = _url;

			if (!url.Contains("://"))
			{
				url = "http://" + url;
			}
			if (!url.EndsWith("/"))
			{
				url += "/";
			}
			url += "Composite/services/LogService/LogService.svc";

			return url;
		}

		private static LogServiceClient CreateBasicHttpConnection(string url)
		{
            var uri = new Uri(url);

			var binding = CreateBasicHttpBinding(url);

			var endPoindAddress = new EndpointAddress(uri);

			var client = new LogServiceClient(binding, endPoindAddress);

			return client;
		}

		private static LogServiceClient CreateWsHttpConnection(string url)
		{
			var uri = new Uri(url);

			var binding = new WSHttpBinding("WSHttpBinding_ILogService");

			var windowsIdentity = WindowsIdentity.GetCurrent();
			if (windowsIdentity == null)
			{
				throw new InvalidOperationException("Failed to get windows identity.");
			}

			if (!windowsIdentity.Name.Contains(@"\"))
			{
				throw new InvalidOperationException("User should be a domain user in order to use that authentication mode");
			}

			// TODO: fix
			EndpointIdentity identity = EndpointIdentity.CreateSpnIdentity(/*windowsIdentity.Name*/ "host/xxxxxxxxxxxxx");

			var endPoindAddress = new EndpointAddress(uri, identity);

			return new LogServiceClient(binding, endPoindAddress);
		}

		public EndpointAddress EndpointAddress
		{
			get;
			private set;
		}

		public Binding Binding
		{
			get;
			private set;
		}

		public string Title
		{
			get
			{
				return _isHttpConnection ? _url : string.Empty;
			}
		}

		public class ConnectionWithContext : IDisposable
		{
			private LogServiceClient _client;
			private OperationContextScope _operationContextScope;


			public ConnectionWithContext(C1Connection connection)
			{
				_client = new LogServiceClient(connection.Binding, connection.EndpointAddress);
				_operationContextScope = new OperationContextScope(_client.InnerChannel);

				var mhg = new MessageHeader<string>(connection._authenticationInfo.AuthToken);
				MessageHeader authToken = mhg.GetUntypedHeader("AuthToken", "Composite.Logger");

				OperationContext.Current.OutgoingMessageHeaders.Add(authToken);
			}

			public OperationContext OperationContext
			{
				get
				{
					return OperationContext.Current;
				}
			}

			public ILogService Channel
			{
				get
				{
					return _client;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				_operationContextScope.Dispose();
				_client.Close();
			}

			#endregion
		}

		public AuthenticationInfo AuthenticationInfo
		{
			get
			{
				return _authenticationInfo;
			}
		}
	}
}
