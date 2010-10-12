using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.AutoIncrement;

namespace Db4objects.Db4o.AutoIncrement.Test.Model {
	public class ModelClassWithReadonlyField {
		[AutoIncrement]
		public readonly int Id;
	}
}
