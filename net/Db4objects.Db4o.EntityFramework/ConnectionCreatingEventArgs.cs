using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.EntityFramework {
	public class ConnectionCreatingEventArgs : EventArgs{
		public readonly ICommonConfigurationProvider Config;
		public IObjectContainer ObjectContainer = null;

		public ConnectionCreatingEventArgs(ICommonConfigurationProvider config) {
			this.Config = config;
		}
	}
}
