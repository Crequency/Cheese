using Cheese.Utils.Cheese;
using Cheese.Utils.I18n;
using Cheese.Utils.Submodules;

namespace Cheese;

internal static class Instances
{
    internal static PathHelper? PathHelper { get; set; }

    internal static I18nManager? I18nManager { get; set; }

    internal static SubmodulesManager? SubmodulesManager { get; set; }

    internal static void Init()
    {
        PathHelper = new();
        I18nManager = new();
        SubmodulesManager = new();
    }
}
