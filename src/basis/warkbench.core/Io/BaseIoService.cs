using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.core.Io;

public abstract class BaseIoService
{
    protected static void EnsureExtension(AbsolutePath path, string expectedExtension)
    {
        if (!path.Value.EndsWith(expectedExtension, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException(
                $"Invalid file extension. Expected '{expectedExtension}', got '{Path.GetExtension(path.Value)}'."
            );
    }
    
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