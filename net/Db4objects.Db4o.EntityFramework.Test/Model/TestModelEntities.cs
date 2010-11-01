using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Db4objects.Db4o.EntityFramework.Test.Model {
	public class TestModelEntities : Db4oEntityContext{
		public TestModelEntities() : base("ModelTestEntities.db4o") { }
		public TestModelEntities(string fileName) : base(fileName) { }

		public Db4oEntitySet<Person> People {
			get { return GetEntitySet<Person>("People"); }
		}
	}
}
