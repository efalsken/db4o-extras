using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Linq;
using log4net;
using log4net.Core;

namespace db4oLog {
	class Program {
		public const string db4oFile = "db4oLog.db4o";
		public const bool db4oNewFile = true;

		static void Main(string[] args) {

			var log = LogManager.GetLogger(typeof(Program));
			log.Info("Application Started");

			if (db4oNewFile && File.Exists(db4oFile))
				File.Delete(db4oFile);

			var objectContainer = Db4oEmbedded.OpenFile(GetDb4oConfig(), db4oFile);
			var p = new Person() {
				Name = "Paul",
				Age = 22
			};

			objectContainer.Store(p);
			objectContainer.Commit();

			objectContainer.Close();

			objectContainer = Db4oEmbedded.OpenFile(GetDb4oConfig(), db4oFile);
			var results = from Person x in objectContainer.AsQueryable<Person>()
						  select x;
			p = results.FirstOrDefault();
			log.DebugFormat("Found {0} Person results", results.Count());

			p = objectContainer.Query<Person>(x => x.Name == "Paul" ).FirstOrDefault();

			log.Info("Application Shutdown");
		}

		private static IEmbeddedConfiguration GetDb4oConfig() {
			var db4oConfig = Db4oEmbedded.NewConfiguration();
			db4oConfig.Common.Diagnostic.AddListener(new DiagnosticToLog4Net());
			db4oConfig.Common.MessageLevel = 4;
			var textWriter = new TextWriterToLog4Net(Level.Debug) {TrimHeaders = false};
			db4oConfig.Common.OutStream = textWriter;
			textWriter.WriteLine("Testing db4o MessageOut Handler.");
			return db4oConfig;
		}

	}
}
