namespace SaptariX.Plugin.Abstractions.Navigation;

public interface IMenuRegistry
{
    void Add(MenuItemDefinition item);
    IReadOnlyList<MenuItemDefinition> GetItems();
}
