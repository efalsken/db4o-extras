using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Db4objects.Db4o.EntityFramework {
	public class ConnectionEventArgs : EventArgs{
		public IObjectContainer ObjectContainer = null;
	}
}
