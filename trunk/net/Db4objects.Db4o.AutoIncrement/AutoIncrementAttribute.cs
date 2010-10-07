using System;

namespace Gamlor.ICOODB.Db4oUtils
{
    [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
    public class AutoIncrementAttribute : Attribute
    {
    }
}