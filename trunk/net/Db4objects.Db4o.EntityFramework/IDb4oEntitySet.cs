using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.Linq;
using System.ComponentModel;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.EntityFramework {
	public interface IDb4oEntitySet {
		string Name { get; }
		Type Type { get; }
		Db4oEntityContext Context { get; }
	}
}