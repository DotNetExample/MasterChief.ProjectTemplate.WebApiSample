using System;
using System.Web.Caching;
using MasterChief.DotNet.Core.Config;
using MasterChief.DotNet4.Utilities.WebForm.Core;

namespace MasterChief.ProjectTemplate.WebApiSample
{
    public sealed class CacheConfigContext : ConfigContext
    {
        public override T Get<T>(string index = null)
        {
            if (!(ConfigService is FileConfigService)) throw new NotSupportedException("CacheConfigContext");
            var filePath = GetClusteredIndex<T>(index);
            var key = filePath;
            var cacheContent = CacheManger.Get(key);
            if (cacheContent != null) return (T) cacheContent;
            var value = base.Get<T>(index);
            CacheManger.Set(key, value, new CacheDependency(filePath));
            return value;
        }
    }
}