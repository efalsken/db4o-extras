using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.CS;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.CS.Config;
using System.Threading;
using System.Reflection;

namespace Db4objects.Db4o.WindowsService {
	public class Db4oServiceHost : System.ServiceProcess.ServiceBase, IMessageRecipient {
		/// <summary> 
		/// Required designer variable.
		/// </summary> 
		private System.ComponentModel.Container components = null;
		
		public Db4oServiceHost() {
			/// Required designer method call.
			InitializeComponent();

			ReadConfiguration();

			CanPauseAndContinue = false;
			CanStop = true;
			ServiceName = ServiceDefinition.ServiceName;
		}

		#region Properties
		protected int listenPort = 0;
		protected string db4oFileName = null;
		protected IObjectServer db4oServer = null;
		protected string clientUsername = null;
		protected string clientPassword = null;

		public int ListenPort {
			get { return this.listenPort; }

			set {
				if (this.db4oServer != null)
					throw new InvalidOperationException("ListenPort cannot be changed while the server is running.");
				this.listenPort = value;
			}
		}

		public string Db4oFileName {
			get { return this.db4oFileName; }
			set {
				if (this.db4oServer != null)
					throw new InvalidOperationException("Db4oFileName cannot be changed while the server is running.");
				this.db4oFileName = value;
			}
		}

		public string ClientUsername {
			get { return this.clientUsername; }
			set {
				if (this.clientUsername != null)
					throw new InvalidOperationException("ClientUsername cannot be changed while the server is running.");
				this.clientUsername = value;
			}
		}

		public string ClientPassword {
			get { return this.clientPassword; }
			set {
				if (this.clientPassword != null)
					throw new InvalidOperationException("ClientPassword cannot be changed while the server is running.");
				this.clientPassword = value;
			}
		}

		public IObjectServer Server {
			get {
				lock (this) {
					if (db4oServer == null)
						throw new InvalidOperationException("Server is not running.");
					return db4oServer;
				}
			}
		}

		public IObjectContainer GetClient() {
			lock (this) {
				if (db4oServer == null)
					throw new InvalidOperationException("Server is not running.");
				return db4oServer.OpenClient();
			}
		}
		#endregion

		private void ReadConfiguration() {
			var settings = new Properties.Settings();

			this.Db4oFileName = System.Environment.ExpandEnvironmentVariables(settings.DatabaseFileName);
			
			//Windows services are executed from the Windows folder ususally. 
			//So if our database should be anywhere else, we need to rebase our db file path.
			if(!System.IO.Path.IsPathRooted(this.Db4oFileName))
				//todo: System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
				this.Db4oFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), this.Db4oFileName);
			
			this.ListenPort = settings.ListenPort;
			
			this.ClientUsername = settings.Username;
			
			this.ClientPassword = settings.Password;
		}

		// Required designer method.
		private void InitializeComponent() {

		}

		#region Service Control Methods
		protected override void OnStart(string[] args) {
			lock (this) {
				if (db4oServer != null)
					throw new InvalidOperationException("Server is already running.");

				db4oServer = InitializeDb4oServer();
				//Any initialization errors will automatically be written to the event log.
			}
		}

		protected virtual void OnStop(string[] args) {
			lock (this) {
				if (db4oServer == null)
					throw new InvalidOperationException("Server is not running.");
				db4oServer.Close();
				db4oServer = null;
			}
		}
		#endregion

		protected virtual void InitializeExtendedConfiguration(IServerConfiguration config) {
			config.Networking.MessageRecipient = this;
		}

		protected virtual void ValidateConfiguration(IServerConfiguration config) {
			if (ListenPort < 1)
				throw new ServiceConfigurationException("ListenPort must be > 0.");
			if (String.IsNullOrEmpty(Db4oFileName))
				throw new ServiceConfigurationException("Missing file name.");
			if (String.IsNullOrEmpty(ClientUsername))
				throw new ServiceConfigurationException("Missing ClientUsername.");
		}
		
		protected virtual IObjectServer InitializeDb4oServer() {
			var config = Db4oClientServer.NewServerConfiguration();
			InitializeExtendedConfiguration(config);
			ValidateConfiguration(config);
			this.EventLog.WriteEntry(String.Format("Opening db4o database {0} on port {1}.", Db4oFileName, ListenPort), System.Diagnostics.EventLogEntryType.Information);
			var server = Db4oClientServer.OpenServer(config, Db4oFileName, ListenPort);
			InitializeAccessControl(server);
			return server;
		}

		protected virtual void InitializeAccessControl(IObjectServer db4oServer) {
			if (!String.IsNullOrEmpty(ClientUsername))
				db4oServer.GrantAccess(ClientUsername, ClientPassword);
		}

		public void ProcessMessage(IMessageContext context, object message) {
			if (message is ServerStopMessage)
				this.Stop();
		}
	}
}
