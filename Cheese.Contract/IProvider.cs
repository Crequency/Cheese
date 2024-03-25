using System.ComponentModel.Composition;

namespace Cheese.Contract;

[InheritedExport]
public interface IProvider
{
    public string GetProviderIdentity();
}