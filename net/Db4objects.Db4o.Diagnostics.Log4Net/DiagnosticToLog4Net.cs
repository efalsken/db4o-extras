using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Diagnostic;
using Db4objects.Db4o.Diagnostic.Properties;
using log4net;
using log4net.Core;

namespace Db4objects.Db4o.Diagnostic {
	public class DiagnosticToLog4Net : IDiagnosticListener {
		protected readonly ILog Log;
		private const string DefaultLogger = "Db4objects.Db4o.Diagnostic";

		public DiagnosticToLog4Net() : this(DefaultLogger) { }

		public DiagnosticToLog4Net(string loggerName) {
			this.Log = LogManager.GetLogger(loggerName);
			if(this.Log == null) throw new ArgumentException(String.Format(Resources.LoggerNotFound, loggerName), "loggerName");
			this.DefaultLevel = Level.Debug;
			this.OutputReason = false;
			this.OutputSolution = false;
		}

		public Level DefaultLevel { get; set; }
		public bool OutputReason { get; set; }
		public bool OutputSolution { get; set; }

		public void OnDiagnostic(IDiagnostic d) {
			//Log.DebugFormat("db4o Diagnostic Event {0}", d.GetType().Name);
			var message = d as DiagnosticBase;
			if (message == null) return;

			Level level = DefaultLevel;

			if (d is DeletionFailed)
				level = Level.Error;
			else if (d is DescendIntoTranslator)
				level = Level.Warn;
			else if (d is DefragmentRecommendation ||
					 d is NativeQueryNotOptimized ||
					 d is NativeQueryOptimizerNotLoaded)
				level = Level.Info;

			Log.Log(level, message.Problem());
			if (OutputReason)
				Log.LogFormat(level, "Reason: {0}", message.Reason());
			if (OutputSolution)
				Log.LogFormat(level, "Solution: {0}", message.Solution());
		}
	}
}
