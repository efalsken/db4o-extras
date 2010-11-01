using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Db4objects.Db4o.EntityFramework {
	public class ConfigurationEventArgs : EventArgs{
		public Db4objects.Db4o.Config.IConfiguration Configuration;
	}
}
