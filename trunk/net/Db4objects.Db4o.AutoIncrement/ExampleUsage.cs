using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.AutoIncrement;

namespace Example {
	public class ExampleUsage {
		public void Main(string[] args) {
			var database = Db4oEmbedded.OpenFile("Example.db4o");
			AutoIncrementSupport.Install(database);
			for(int x = 0; x < 10; x++)
				database.Store(new ExampleModel());
			database.Commit();

			var results = database.Query<ExampleModel>();
			foreach (var m in results)
				Console.Write(m.IdAutoProperty);
		}
	}
}
