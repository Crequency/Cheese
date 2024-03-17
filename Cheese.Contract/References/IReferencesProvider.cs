using System.ComponentModel.Composition;
using Cheese.Shared.References;

namespace Cheese.Contract.References;

[InheritedExport]
public interface IReferencesProvider
{
    public string GetProviderIdentity();
    
    public IEnumerable<ReferenceItem> GetReferences();
}
