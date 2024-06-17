namespace Cheese.Contract.Providers;

public interface ILocalizationProvider : IProvider
{
    void Execute(string target);
}
