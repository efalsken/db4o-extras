using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Db4objects.Db4o.EntityFramework.Test.Model {
	public class TestCustomModelEntities : Db4oEntityContext{
		public TestCustomModelEntities() : base() {}

		IObjectContainer db;

		public Db4oEntitySet<Person> People {
			get { return GetEntitySet<Person>("People"); }
		}

		protected override IObjectContainer OnConnectionCreating() {
			db = Db4oEmbedded.OpenFile("test.db4o");
			return db;
		}

		protected override void Dispose(bool disposing) {
			if (disposing)
				db.Close();
		}
	}
}
