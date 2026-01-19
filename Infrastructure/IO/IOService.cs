using System;
using System.Collections.Generic;


namespace warkbench.Infrastructure;

public class IOService
{
    public IOService(PathService pathService)
    {
        PathService = pathService;
        _exporters = new Dictionary<Type, IOBase>
        {
            //{ typeof(HppExporter), new HppExporter(pathService) },
            { typeof(JsonIO), new JsonIO(pathService) }
        };
    }

    public T GetService<T>() where T : class, IOBase
    {
        if (_exporters.TryGetValue(typeof(T), out var exporter))
            return (T)exporter;

        throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
    }
    
    private readonly Dictionary<Type, IOBase> _exporters;
    private PathService PathService { get; }
}