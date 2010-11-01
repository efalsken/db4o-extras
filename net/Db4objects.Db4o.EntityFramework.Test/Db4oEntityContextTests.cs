using System;
using System.Dynamic;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Db4objects.Db4o.EntityFramework.Test {
	[TestClass]
	public class Db4oEntityContextTests {
		const string testFileName = "test.db4o";

		[TestMethod]
		public void TestSimpleEmbeddedMode() {

			using(var db = new Model.TestModelEntities(testFileName)){
				db.People.AddObject(new Model.Person() { Name = "Bob Burns" });
				db.People.AddObject(new Model.Person() { Name = "Anne Anders" });
				db.SaveChanges();

				Assert.IsTrue(db.People.Count() == 2);
			}
		}

		[TestMethod]
		public void TestCustomImplMode() {
			using (var db = new Model.TestCustomModelEntities()) {
				db.People.AddObject(new Model.Person() { Name = "Bob Burns" });
				db.People.AddObject(new Model.Person() { Name = "Anne Anders" });
				db.SaveChanges();

				Assert.IsTrue(db.People.Count() == 2);
			}
		}

		[TestCleanup]
		public void Cleanup() {
			if (File.Exists(testFileName))
				File.Delete(testFileName);
		}
	}
}
