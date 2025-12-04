namespace warkbench.Models;
public interface IDeepCloneable<out T>
{
    T DeepClone();
}

