using System;
using System.Configuration;
using System.Web;
using Db4objects.Db4o;

namespace Db4objects.Db4o.Web {
	/// <summary>
	/// Abstracts a local db4o database in an in-process connection-per-request model.
	/// </summary>
	public class Db4oLocalModule : IHttpModule {
		internal static readonly string KEY_DB4O_CONFIG_NAME = "db4oFileName";
		internal static readonly string KEY_DB4O_CLIENT = "db4oClient";
		private static IObjectContainer objectServer = null;

		public void Init(HttpApplication application) {
			application.EndRequest += new EventHandler(Application_EndRequest);
		}


		public static IObjectContainer Client {
			get {
				var context = HttpContext.Current;

				var objectClient = context.Items[KEY_DB4O_CLIENT] as IObjectContainer;

				if (objectClient == null) {
					objectClient = Server.Ext().OpenSession();
					context.Items[KEY_DB4O_CLIENT] = objectClient;
				}

				return objectClient;
			}
		}

		internal static IObjectContainer Server {
			get {
				var context = HttpContext.Current;

				if (objectServer == null) {
					string dbFilePath = context.Server.MapPath(ConfigurationManager.AppSettings[KEY_DB4O_CONFIG_NAME]);

					objectServer = Db4oEmbedded.OpenFile(dbFilePath);
				}

				return objectServer;
			}
		}

		public static string HashCodes {
			get { return "Server: " + Server.GetHashCode().ToString() + " Client: " + Client.GetHashCode(); }
		}
		
		private void Application_EndRequest(object sender, EventArgs e) {
			HttpApplication application = (HttpApplication)sender;
			HttpContext context = application.Context;


			var objectClient = context.Items[KEY_DB4O_CLIENT] as IObjectContainer;

			if (objectClient != null) {
				objectClient.Close();
			}

			objectClient = null;
			context.Items[KEY_DB4O_CLIENT] = null;
		}
		
		public void Dispose() {
			if (objectServer != null) {
				objectServer.Close();
			}

			objectServer = null;
		}
	}
}