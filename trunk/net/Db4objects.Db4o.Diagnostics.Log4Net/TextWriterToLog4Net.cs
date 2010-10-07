using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using log4net;
using log4net.Core;

namespace Db4objects.Db4o.Diagnostic {
	public class TextWriterToLog4Net : TextWriter{
		private readonly StringBuilder _buffer = new StringBuilder();
		private const string DefaultLogger = "Db4objects.Db4o.MessageLog";

		public TextWriterToLog4Net(Level defaultLevel) : this(DefaultLogger, defaultLevel) { }

		public TextWriterToLog4Net(string loggerName, Level defaultLevel) : base() {
			this.LoggerName = loggerName;
			this.DefaultLevel = defaultLevel;
			this.TrimHeaders = true;
		}

		public override Encoding Encoding { get { return Encoding.UTF8; } }

		public string LoggerName { get; private set; }

		public Level DefaultLevel { get; private set; }

		public bool TrimHeaders { get; set; }

		private ILog _log;
		public ILog Log {
			get { return _log ?? (_log = LogManager.GetLogger(LoggerName)); }
		}

		public override void Write(char[] buffer) {
			_buffer.Append(buffer);
		}

		public override void WriteLine() {
			_buffer.AppendLine();
			this.Flush();
		}

		public override void WriteLine(string value) {
			if (TrimHeaders && value.StartsWith("[") && value.TrimEnd().EndsWith("]")) return;
			_buffer.AppendLine(value);
			this.Flush();
		}

		public override void Flush() {
			var s = _buffer.ToString();
			s = s.TrimEnd('\r', '\n', '\t', ' ');  //log4net likes to assume that the text logged does not end with a newline.
			s = s.TrimStart(' ', '\t');

			switch (DefaultLevel.Name) {
				case "DEBUG":
					Log.Debug(s);
					break;
				case "INFO":
					Log.Info(s);
					break;
				case "WARN":
					Log.Warn(s);
					break;
				case "ERROR":
					Log.Error(s);
					break;
			}
			_buffer.Clear();
		}
	}
}
