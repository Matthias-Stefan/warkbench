using warkbench.Models;


namespace warkbench.Infrastructure;

public class HppIO
{
    public HppIO(PathService pathService)
    {
        PathService = pathService;
    }
    
    private PathService PathService { get; }
}