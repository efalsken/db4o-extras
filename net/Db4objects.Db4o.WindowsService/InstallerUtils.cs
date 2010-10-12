using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ServiceProcess;

namespace Db4objects.Db4o.WindowsService {
	[RunInstaller(true)]
	public sealed class Db4oServiceInstallerProcess : ServiceProcessInstaller {
		public Db4oServiceInstallerProcess(){
			this.Account = ServiceDefinition.RunAs;
		}
	}

	[RunInstaller(true)]
	public sealed class Db4oServiceInstaller : ServiceInstaller {
		public Db4oServiceInstaller() {
			this.Description = ServiceDefinition.Description;
			this.DisplayName = ServiceDefinition.DisplayName;
			this.ServiceName = ServiceDefinition.ServiceName;
			this.StartType = ServiceDefinition.Startup;
		}
	}
}
