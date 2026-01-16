using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;

namespace warkbench.src.basis.core.Common;

public abstract class BaseIoService
{
    protected static readonly JsonSerializerSettings JsonSettings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        PreserveReferencesHandling = PreserveReferencesHandling.All,
        Formatting = Formatting.Indented,
        ContractResolver = new IoContractResolver()
    };
    
    private class IoContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (prop.Writable) 
                return prop;
            
            var property = member as PropertyInfo;
            var hasPrivateSetter = property?.GetSetMethod(true) != null;
            prop.Writable = hasPrivateSetter;
            return prop;
        }
    }
}