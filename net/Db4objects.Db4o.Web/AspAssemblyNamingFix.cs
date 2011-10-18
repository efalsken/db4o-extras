using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Web
{
    /// <summary>
    /// This is fix for APS.NET pages using db4o. The generated assembly name for ASP.NET pages
    /// changes on every compile. Therefore db4o doesn't recognise the stored object anymore.
    /// Add this to the configuration if you're encountering this issue.
    /// 
    /// <code>config.Common.AddAlias(new AspAssemblyNamingFix());</code> 
    /// </summary>
    class AspAssemblyNamingFix : IAlias
    {
        private const string FixedName = "AspFixedAssemblyName";
        private readonly string DynamicName = typeof(AspAssemblyNamingFix).Assembly.GetName().Name;

        public string ResolveRuntimeName(string runtimeTypeName)
        {
            return runtimeTypeName.Replace(DynamicName, FixedName);
        }

        public string ResolveStoredName(string storedTypeName)
        {
            return storedTypeName.Replace(FixedName, DynamicName);
        }
    }
}