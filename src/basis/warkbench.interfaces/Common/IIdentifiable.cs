namespace warkbench.src.basis.interfaces.Common;

public interface IIdentifiable
{
    /// <summary> Unique identifier for the object. </summary>
    Guid Id { get; }
    
    /// <summary> Display name of the object. </summary>
    string Name { get; }
}