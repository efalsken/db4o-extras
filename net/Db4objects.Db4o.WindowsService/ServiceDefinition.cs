using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ServiceProcess;
using System.Reflection;

namespace Db4objects.Db4o.WindowsService {
	internal static class ServiceDefinition {
		public readonly static ServiceStartMode Startup = ServiceStartMode.Automatic;
		public readonly static ServiceAccount RunAs = ServiceAccount.NetworkService;
		public readonly static string Description = "db4o Network Server (.NET edition)";
		public readonly static string DisplayName = "db4o (.NET)";
		public readonly static string ServiceName = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Name;
	}
}
