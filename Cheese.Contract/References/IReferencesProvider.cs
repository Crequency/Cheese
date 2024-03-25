using System.ComponentModel.Composition;
using Cheese.Shared.References;

namespace Cheese.Contract.References;

[InheritedExport]
public interface IReferencesProvider : IProvider
{
    public IEnumerable<ReferenceItem> GetReferences();
}
