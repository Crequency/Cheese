using Cheese.Shared.References;

namespace Cheese.Contract.Providers;

public interface IReferencesProvider : IProvider
{
    public IEnumerable<ReferenceItem> GetReferences();
}
