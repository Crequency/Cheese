using Cheese.Shared.References;

namespace Cheese.Contract.References;

public interface IReferencesProvider : IProvider
{
    public IEnumerable<ReferenceItem> GetReferences();
}
