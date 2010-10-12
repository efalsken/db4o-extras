using System;

namespace Db4objects.Db4o.AutoIncrement {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class AutoIncrementAttribute : Attribute {
	}
}