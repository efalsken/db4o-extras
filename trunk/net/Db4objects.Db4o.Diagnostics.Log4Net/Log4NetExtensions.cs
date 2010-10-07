using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;

namespace Db4objects.Db4o.Diagnostic {
	internal static class Log4NetExtensions {
		public static void Log(this ILog log, Level level, object message) {
			Log(log, level, message, null);
		}

		public static void Log(this ILog log, Level level, object message, Exception exception) {
			switch (level.Name) {
				case "DEBUG":
					log.Debug(message, exception);
					break;
				case "INFO":
					log.Info(message, exception);
					break;
				case "WARN":
					log.Info(message, exception);
					break;
				case "ERROR":
					log.Info(message, exception);
					break;
				case "FATAL":
					log.Fatal(message, exception);
					break;
			}
		}

		public static void LogFormat(this ILog log, Level level, string message, params object[] args) {
			LogFormat(log, level, null, message, args);
		}

		public static void LogFormat(this ILog log, Level level, IFormatProvider formatProvider, string message, params object[] args) {
			switch (level.Name) {
				case "DEBUG":
					log.DebugFormat(formatProvider, message, args);
					break;
				case "INFO":
					log.InfoFormat(formatProvider, message, args);
					break;
				case "WARN":
					log.InfoFormat(formatProvider, message, args);
					break;
				case "ERROR":
					log.InfoFormat(formatProvider, message, args);
					break;
				case "FATAL":
					log.FatalFormat(formatProvider, message, args);
					break;
			}
		}
	}
}
