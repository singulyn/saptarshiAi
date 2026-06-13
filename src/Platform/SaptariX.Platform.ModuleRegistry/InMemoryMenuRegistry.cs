using SaptariX.Plugin.Abstractions.Navigation;

namespace SaptariX.Platform.ModuleRegistry;

public sealed class InMemoryMenuRegistry : IMenuRegistry
{
    private readonly List<MenuItemDefinition> _items = [];

    public void Add(MenuItemDefinition item)
    {
        _items.RemoveAll(x => x.Url.Equals(item.Url, StringComparison.OrdinalIgnoreCase));
        _items.Add(item);
    }

    public IReadOnlyList<MenuItemDefinition> GetItems()
    {
        return _items.Where(x => x.Enabled).OrderBy(x => x.Order).ThenBy(x => x.Text).ToList();
    }
}
