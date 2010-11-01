using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Db4objects.Db4o.EntityFramework.Test.Model {
	/// <summary>
	/// Sample implementation that uses the Db4oLocalModule (db4o-extras/web) to use a per-session db4o client instead of a single shared implementation.
	/// </summary>
	public class TestWebModelEntities : Db4oEntityContext{
		public TestWebModelEntities() : base() { }

		public Db4oEntitySet<Person> People {
			get { return GetEntitySet<Person>("People"); }
		}

		protected override IObjectContainer OnConnectionCreating() {
			return Db4objects.Db4o.Web.Db4oLocalModule.Client;
		}
	}
}
