using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Db4objects.Db4o.EntityFramework {
	internal class Requires {
		public static T IsTypeOf<T>(string argName, object argValue) {
			if (!(argValue is T)) {
				throw new ArgumentException(String.Format("The argument '{0}' must be of type '{1}'.", argName, typeof(T).FullName ), argName);
			}
			return (T)argValue;
		}

		public static int NotNegative (string argName, int argValue){
			if (argValue < 0) {
				throw new ArgumentOutOfRangeException(argName, argValue, String.Format("The argument '{0}' cannot be negative.", argName));
			}
			return argValue;
		}

		public static double NotNegative(string argName, double argValue) {
			if (argValue < 0.0) {
				throw new ArgumentOutOfRangeException(argName, argValue, String.Format("The argument '{0}' cannot be negative.", argName));
			}
			return argValue;
		}

		public static T NotNull<T>(string argName, T argValue) where T : class{
			if (argValue == null) {
				throw new ArgumentNullException(argName, String.Format("The argument '{0}' must not be null.", argName));
			}
			return argValue;
		}

		public static string NotNullOrEmpty(string argName, string argValue) {
			if (string.IsNullOrEmpty(argValue)) {
				throw new ArgumentException(String.Format("The argument '{0}' cannot be null or empty.", argName), argName);
			}
			return argValue;
		}

		public static TValue PropertyNotEqualTo<TValue>(string argName, string argProperty, TValue propertyValue, TValue testValue) where TValue : IEquatable<TValue> {
			if (propertyValue.Equals(testValue)) {
				throw new ArgumentException(String.Format("The property '{1}' in object '{0}' is invalid.", argName, argProperty), argName);
			}
			return propertyValue;
		}

		public static int PropertyNotNegative(string argName, string argProperty, int propertyValue) {
			if (propertyValue < 0) {
				throw new ArgumentOutOfRangeException(argName, propertyValue, String.Format("The property '{1}' in object '{0}' cannot be negative.", argName, argProperty));
			}
			return propertyValue;
		}

		public static double PropertyNotNegative(string argName, string argProperty, double propertyValue) {
			if (propertyValue < 0.0) {
				throw new ArgumentOutOfRangeException(argName, propertyValue, String.Format("The property '{1}' in object '{0}' cannot be negative.", argName, argProperty));
			}
			return propertyValue;
		}

		public static string PropertyNotNullOrEmpty(string argName, string argProperty, string propertyValue) {
			if (string.IsNullOrEmpty(propertyValue)) {
				throw new ArgumentException(String.Format("The property '{1}' in object '{0}' cannot be null or empty.", argName, argProperty), argName);
			}
			return propertyValue;
		}

		public static void IsTrue(bool condition, string message) {
			if (!condition) {
				throw new InvalidOperationException(message);
			}
		}

		public static void IsTrue(bool condition, string message, params object[] args) {
			IsTrue(condition, String.Format(CultureInfo.CurrentUICulture, message, args));
		}

		public static void IsFalse(bool condition, string message) {
			if (condition) {
				throw new InvalidOperationException(message);
			}
		}

		public static void IsFalse(bool condition, string message, params object[] args) {
			IsFalse(condition, String.Format(CultureInfo.CurrentUICulture, message, args));
		}

	}
}
