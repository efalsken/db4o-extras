using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o.AutoIncrement;

namespace Db4objects.Db4o.AutoIncrement {
	public class ExampleClass {

		[AutoIncrement]
		public int IdAutoProperty { get; set; }

		[AutoIncrement]
		public int IdAutoProperty2 { get; private set; }

		[AutoIncrement]
		protected int _idField;
		public int IdFieldAccessor {
			get { return _idField; }
			set { _idField = value; }
		}
	}
}
