using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.EntityFramework {
	public class ConfigurationEventArgs : EventArgs{
		public ICommonConfigurationProvider Configuration;
	}
}
